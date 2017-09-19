using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.SqlDal.Members;
using Hishop.Weixin.Pay;
using Hishop.Weixin.Pay.Domain;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Hidistro.SqlDal.Store
{
	public class RedPackDao
	{
		private Database database;

		public RedPackDao()
		{
			this.database = DatabaseFactory.CreateDatabase();
		}

		public void RedPackCheckJob()
		{
			StringBuilder stringBuilder = new StringBuilder();
			DateTime dateTime = DateTime.Now.Date.AddDays(-3.0);
			System.Data.DataTable dataTable = new System.Data.DataTable();
			string query = "select UserId,SerialID,RedpackId,Amount from Hishop_BalanceDrawRequest WHERE IsCheck=2 AND RequestType=3 AND CheckTime>=@CheckTime";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			this.database.AddInParameter(sqlStringCommand, "CheckTime", System.Data.DbType.DateTime, dateTime);
			using (System.Data.IDataReader dataReader = this.database.ExecuteReader(sqlStringCommand))
			{
				dataTable = DataHelper.ConverDataReaderToDataTable(dataReader);
			}
			RedPackClient redPackClient = new RedPackClient();
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			DistributorsDao distributorsDao = new DistributorsDao();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				foreach (System.Data.DataRow dataRow in dataTable.Rows)
				{
					string mch_billno = dataRow["RedpackId"].ToString();
					RedPackInfo redpackInfo = redPackClient.GetRedpackInfo(masterSettings.WeixinAppId, masterSettings.WeixinPartnerID, mch_billno, masterSettings.WeixinPartnerKey, masterSettings.WeixinCertPath, masterSettings.WeixinCertPassword);
					if (redpackInfo != null)
					{
						redPackStatus redPackStatus = redpackInfo.Getstatus();
						if (redPackStatus == redPackStatus.已退款 || redPackStatus == redPackStatus.发放失败)
						{
							int num = int.Parse(dataRow["SerialID"].ToString());
							Globals.Debuglog(string.Concat(new object[]
							{
								"BalanceDrawRequest-",
								num,
								":",
								redpackInfo.ToString()
							}), "RedPackCheck.txt");
							decimal d = decimal.Parse(dataRow["Amount"].ToString());
							int userId = int.Parse(dataRow["UserId"].ToString());
							distributorsDao.UpdateBalanceDistributors(userId, -1m * d);
							distributorsDao.UpdateRedPackStatus(num, "红包" + redPackStatus.ToString(), null);
						}
					}
				}
			}
			query = "select UserId,Id,RedpackId,Amount from Hishop_MemberAmountRequest WHERE State=2 AND RequestType=3 AND CheckTime>=@CheckTime";
			AmountDao amountDao = new AmountDao();
			sqlStringCommand = this.database.GetSqlStringCommand(query);
			this.database.AddInParameter(sqlStringCommand, "CheckTime", System.Data.DbType.DateTime, dateTime);
			using (System.Data.IDataReader dataReader = this.database.ExecuteReader(sqlStringCommand))
			{
				dataTable = DataHelper.ConverDataReaderToDataTable(dataReader);
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				foreach (System.Data.DataRow dataRow in dataTable.Rows)
				{
					string mch_billno = dataRow["RedpackId"].ToString();
					RedPackInfo redpackInfo = redPackClient.GetRedpackInfo(masterSettings.WeixinAppId, masterSettings.WeixinPartnerID, mch_billno, masterSettings.WeixinPartnerKey, masterSettings.WeixinCertPath, masterSettings.WeixinCertPassword);
					if (redpackInfo != null)
					{
						redPackStatus redPackStatus = redpackInfo.Getstatus();
						if (redPackStatus == redPackStatus.已退款 || redPackStatus == redPackStatus.发放失败)
						{
							int num = int.Parse(dataRow["Id"].ToString());
							Globals.Debuglog(string.Concat(new object[]
							{
								"MemberAmountRequest-",
								num,
								":",
								redpackInfo.ToString()
							}), "RedPackCheck.txt");
							decimal d = decimal.Parse(dataRow["Amount"].ToString());
							int userId = int.Parse(dataRow["UserId"].ToString());
							amountDao.SetAmountRequestStatus(new int[]
							{
								num
							}, 3, "红包" + redPackStatus.ToString(), "", "SYSJOB");
						}
					}
				}
			}
		}
	}
}
