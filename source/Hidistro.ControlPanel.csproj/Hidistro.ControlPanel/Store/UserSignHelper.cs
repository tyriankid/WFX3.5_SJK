using Hidistro.ControlPanel.Members;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.VShop;
using Hidistro.SqlDal.VShop;
using System;
using System.Data;

namespace Hidistro.ControlPanel.Store
{
	public static class UserSignHelper
	{
		public static int AddPoint(UserSign us)
		{
			int num;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			if (masterSettings.sign_score_Enable)
			{
				IntegralDetailInfo integralDetailInfo = new IntegralDetailInfo()
				{
					IntegralSourceType = 1,
					IntegralSource = "签到",
					Userid = us.UserID,
					IntegralChange = masterSettings.SignPoint,
					IntegralStatus = Convert.ToInt32(IntegralDetailStatus.SignToIntegral)
				};
				if (masterSettings.sign_score_Enable)
				{
					if (us.Continued >= masterSettings.SignWhere)
					{
						IntegralDetailInfo integralChange = integralDetailInfo;
						integralChange.IntegralChange = integralChange.IntegralChange + masterSettings.SignWherePoint;
						us.Continued = 0;
					}
				}
				IntegralDetailHelp.AddIntegralDetail(integralDetailInfo, null);
				num = Convert.ToInt32(integralDetailInfo.IntegralChange);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static int InsertUserSign(UserSign us)
		{
			return (new UserSignDao()).InsertUserSign(us);
		}

		public static bool IsSign(int userID)
		{
			bool flag;
			DataTable dataTable = UserSignHelper.SignInfoByUser(userID);
			if (dataTable.Rows.Count >= 1)
			{
				DateTime dateTime = Convert.ToDateTime(dataTable.Rows[0]["SignDay"]);
				flag = (!(DateTime.Now.ToString("yyyyMMdd") == dateTime.ToString("yyyyMMdd")) ? true : false);
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public static int MaxContinued(DateTime t1, DateTime t2)
		{
			return (t2 - t1).Days;
		}

		public static DataTable SignInfoByUser(int userID)
		{
			return (new UserSignDao()).SignInfoByUser(userID);
		}

		public static int UpdateUserSign(UserSign us)
		{
			return (new UserSignDao()).UpdateUserSign(us);
		}

		public static int USign(int userID)
		{
			int num;
			DataTable dataTable = UserSignHelper.SignInfoByUser(userID);
			UserSign userSign = new UserSign();
			if (dataTable.Rows.Count >= 1)
			{
				userSign.ID = Convert.ToInt32(dataTable.Rows[0]["ID"]);
				userSign.SignDay = DateTime.Now;
				userSign.UserID = Convert.ToInt32(dataTable.Rows[0]["UserID"]);
				userSign.Continued = Convert.ToInt32(dataTable.Rows[0]["Continued"]);
				DateTime dateTime = Convert.ToDateTime(dataTable.Rows[0]["SignDay"]);
				DateTime date = dateTime.Date;
				dateTime = userSign.SignDay;
				int num1 = UserSignHelper.MaxContinued(date, dateTime.Date);
				if (num1 <= 0)
				{
					num = -1;
					return num;
				}
				if (num1 == 1)
				{
					UserSign continued = userSign;
					continued.Continued = continued.Continued + 1;
				}
				else if (num1 > 1)
				{
					userSign.Continued = 1;
				}
			}
			else
			{
				userSign.UserID = userID;
				userSign.Continued = 1;
				UserSignHelper.InsertUserSign(userSign);
			}
			int num2 = UserSignHelper.AddPoint(userSign);
			UserSignHelper.UpdateUserSign(userSign);
			num = num2;
			return num;
		}
	}
}