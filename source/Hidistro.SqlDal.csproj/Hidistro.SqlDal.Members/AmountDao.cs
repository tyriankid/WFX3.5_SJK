using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Sales;
using Hidistro.Entities.VShop;
using Hidistro.SqlDal.VShop;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;

namespace Hidistro.SqlDal.Members
{
	public class AmountDao
	{
		private Database database;

		private static object RedLock = new object();

		public AmountDao()
		{
			this.database = DatabaseFactory.CreateDatabase();
		}

		public bool CreatAmount(MemberAmountDetailedInfo mountInfo)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("INSERT INTO Hishop_MemberAmountDetailed(UserId,UserName,PayId,TradeAmount,AvailableAmount,TradeType,TradeWays,TradeTime, Remark,State) VALUES(@UserId,@UserName,@PayId,@TradeAmount,@AvailableAmount, @TradeType, @TradeWays,@TradeTime,@Remark,@State)");
			this.database.AddInParameter(sqlStringCommand, "UserId", System.Data.DbType.Int32, mountInfo.UserId);
			this.database.AddInParameter(sqlStringCommand, "UserName", System.Data.DbType.String, mountInfo.UserName);
			this.database.AddInParameter(sqlStringCommand, "PayId", System.Data.DbType.String, mountInfo.PayId);
			this.database.AddInParameter(sqlStringCommand, "TradeAmount", System.Data.DbType.Decimal, mountInfo.TradeAmount);
			this.database.AddInParameter(sqlStringCommand, "AvailableAmount", System.Data.DbType.Decimal, mountInfo.AvailableAmount);
			this.database.AddInParameter(sqlStringCommand, "TradeType", System.Data.DbType.Int32, mountInfo.TradeType);
			this.database.AddInParameter(sqlStringCommand, "TradeWays", System.Data.DbType.Int32, mountInfo.TradeWays);
			this.database.AddInParameter(sqlStringCommand, "TradeTime", System.Data.DbType.DateTime, mountInfo.TradeTime);
			this.database.AddInParameter(sqlStringCommand, "Remark", System.Data.DbType.String, mountInfo.Remark);
			this.database.AddInParameter(sqlStringCommand, "State", System.Data.DbType.Int32, mountInfo.State);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public decimal GetUserMaxAmountDetailed(int userid)
		{
			string query = "select isnull(Max(TradeAmount),0) from Hishop_MemberAmountDetailed where UserID=" + userid + " and State=1";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			return decimal.Parse(this.database.ExecuteScalar(sqlStringCommand).ToString());
		}

		public bool UseBalance(MemberAmountDetailedInfo mountInfo)
		{
			bool result;
			if (this.CreatAmount(mountInfo))
			{
				string query = "Update aspnet_Members set AvailableAmount=AvailableAmount+@TradeAmount where UserID=" + mountInfo.UserId;
				System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
				this.database.AddInParameter(sqlStringCommand, "UserId", System.Data.DbType.Int32, mountInfo.UserId);
				this.database.AddInParameter(sqlStringCommand, "TradeAmount", System.Data.DbType.Decimal, mountInfo.TradeAmount);
				result = (this.database.ExecuteNonQuery(sqlStringCommand) > 0);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public DbQueryResult GetBalanceWithdrawListRequest(int type, int page, int pagesize, int userId)
		{
			string table = "Hishop_MemberAmountRequest";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" UserId={0} ", userId);
			if (type != 0)
			{
				stringBuilder.AppendFormat(" AND State not in(-1,2) ", new object[0]);
			}
			return DataHelper.PagingByRownumber(page, pagesize, "Id", SortAction.Desc, true, table, "ID", (stringBuilder.Length > 0) ? stringBuilder.ToString() : null, "*");
		}

		public DbQueryResult GetAmountListRequest(int type, int page, int pagesize, int userId)
		{
			string table = "Hishop_MemberAmountDetailed";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" State={0} AND UserId={1} ", 1, userId);
			if (type != 0)
			{
				if (type == 1)
				{
					stringBuilder.AppendFormat(" AND TradeAmount < 0 ", new object[0]);
				}
				else if (type == 2)
				{
					stringBuilder.AppendFormat(" AND TradeAmount > 0 ", new object[0]);
				}
			}
			return DataHelper.PagingByRownumber(page, pagesize, "TradeTime", SortAction.Desc, true, table, "ID", (stringBuilder.Length > 0) ? stringBuilder.ToString() : null, "*");
		}

		public MemberAmountDetailedInfo GetAmountDetail(int Id)
		{
			MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo();
			string query = "select * from Hishop_MemberAmountDetailed where Id=" + Id;
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
			MemberAmountDetailedInfo result;
			if (dataTable.Rows.Count > 0)
			{
				memberAmountDetailedInfo.Id = Id;
				memberAmountDetailedInfo.UserId = int.Parse(dataTable.Rows[0]["UserId"].ToString());
				memberAmountDetailedInfo.PayId = dataTable.Rows[0]["PayId"].ToString();
				memberAmountDetailedInfo.TradeAmount = Math.Round(decimal.Parse(dataTable.Rows[0]["TradeAmount"].ToString()), 2);
				memberAmountDetailedInfo.AvailableAmount = Math.Round(decimal.Parse(dataTable.Rows[0]["AvailableAmount"].ToString()), 2);
				memberAmountDetailedInfo.TradeType = (TradeType)dataTable.Rows[0]["TradeType"];
				memberAmountDetailedInfo.UserName = dataTable.Rows[0]["UserName"].ToString();
				memberAmountDetailedInfo.TradeWays = (TradeWays)dataTable.Rows[0]["TradeWays"];
				memberAmountDetailedInfo.TradeTime = DateTime.Parse(dataTable.Rows[0]["TradeTime"].ToString());
				memberAmountDetailedInfo.Remark = dataTable.Rows[0]["Remark"].ToString();
				memberAmountDetailedInfo.State = int.Parse(dataTable.Rows[0]["State"].ToString());
				memberAmountDetailedInfo.GatewayPayId = dataTable.Rows[0]["GatewayPayId"].ToString();
				result = memberAmountDetailedInfo;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public MemberAmountDetailedInfo GetAmountDetailByPayId(string PayId)
		{
			MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo();
			string query = "select * from Hishop_MemberAmountDetailed where PayId='" + PayId + "'";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
			MemberAmountDetailedInfo result;
			if (dataTable.Rows.Count > 0)
			{
				memberAmountDetailedInfo.Id = int.Parse(dataTable.Rows[0]["Id"].ToString());
				memberAmountDetailedInfo.UserId = int.Parse(dataTable.Rows[0]["UserId"].ToString());
				memberAmountDetailedInfo.PayId = dataTable.Rows[0]["PayId"].ToString();
				memberAmountDetailedInfo.TradeAmount = Math.Round(decimal.Parse(dataTable.Rows[0]["TradeAmount"].ToString()), 2);
				memberAmountDetailedInfo.AvailableAmount = Math.Round(decimal.Parse(dataTable.Rows[0]["AvailableAmount"].ToString()), 2);
				memberAmountDetailedInfo.TradeType = (TradeType)dataTable.Rows[0]["TradeType"];
				memberAmountDetailedInfo.UserName = dataTable.Rows[0]["UserName"].ToString();
				memberAmountDetailedInfo.TradeWays = (TradeWays)dataTable.Rows[0]["TradeWays"];
				memberAmountDetailedInfo.TradeTime = DateTime.Parse(dataTable.Rows[0]["TradeTime"].ToString());
				memberAmountDetailedInfo.Remark = dataTable.Rows[0]["Remark"].ToString();
				memberAmountDetailedInfo.State = int.Parse(dataTable.Rows[0]["State"].ToString());
				memberAmountDetailedInfo.GatewayPayId = dataTable.Rows[0]["GatewayPayId"].ToString();
				result = memberAmountDetailedInfo;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public PaymentModeInfo GetPaymentMode(TradeWays ways)
		{
			string value = "";
			if (ways == TradeWays.Alipay)
			{
				value = "hishop.plugins.payment.ws_wappay.wswappayrequest";
			}
			else if (ways == TradeWays.ShengFutong)
			{
				value = "Hishop.Plugins.Payment.ShengPayMobile.ShengPayMobileRequest";
			}
			PaymentModeInfo result = new PaymentModeInfo();
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("SELECT * FROM Hishop_PaymentTypes WHERE Gateway = @Gateway");
			this.database.AddInParameter(sqlStringCommand, "Gateway", System.Data.DbType.String, value);
			using (System.Data.IDataReader dataReader = this.database.ExecuteReader(sqlStringCommand))
			{
				if (dataReader.Read())
				{
					result = DataMapper.PopulatePayment(dataReader);
				}
			}
			return result;
		}

		public bool UpdateAmount(MemberAmountDetailedInfo model)
		{
			string query = "Update Hishop_MemberAmountDetailed set State=1,GatewayPayId=@GatewayPayId where State<>1 and Id=" + model.Id;
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			this.database.AddInParameter(sqlStringCommand, "State", System.Data.DbType.Int32, model.State);
			this.database.AddInParameter(sqlStringCommand, "GatewayPayId", System.Data.DbType.String, model.GatewayPayId);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public bool UpdateMember(MemberAmountDetailedInfo model)
		{
			string query = "Update aspnet_Members set TotalAmount=TotalAmount+@TotalAmount,AvailableAmount=AvailableAmount+@TradeAmount where UserID=" + model.UserId;
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			this.database.AddInParameter(sqlStringCommand, "UserId", System.Data.DbType.Int32, model.UserId);
			this.database.AddInParameter(sqlStringCommand, "TradeAmount", System.Data.DbType.Decimal, model.TradeAmount);
			this.database.AddInParameter(sqlStringCommand, "TotalAmount", System.Data.DbType.Decimal, (model.TradeAmount > 0m) ? model.TradeAmount : 0m);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public bool AddBalanceDrawRequest(BalanceDrawRequestInfo bdrinfo)
		{
			string query = "INSERT INTO Hishop_BalanceDrawRequest(UserId,RequestType,UserName,RequestTime,Amount,AccountName,BankName,CellPhone,MerchantCode,Remark,CheckTime,IsCheck) VALUES(@UserId,@RequestType,@UserName,@RequestTime,@Amount,@AccountName,@bankName,@CellPhone,@MerchantCode,@Remark,@CheckTime,@IsCheck)";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			this.database.AddInParameter(sqlStringCommand, "UserId", System.Data.DbType.Int32, bdrinfo.UserId);
			this.database.AddInParameter(sqlStringCommand, "RequestType", System.Data.DbType.Int32, bdrinfo.RequestType);
			this.database.AddInParameter(sqlStringCommand, "UserName", System.Data.DbType.String, bdrinfo.UserName);
			this.database.AddInParameter(sqlStringCommand, "RequestTime", System.Data.DbType.DateTime, bdrinfo.RequestTime);
			this.database.AddInParameter(sqlStringCommand, "Amount", System.Data.DbType.Decimal, bdrinfo.Amount);
			this.database.AddInParameter(sqlStringCommand, "AccountName", System.Data.DbType.String, bdrinfo.AccountName);
			this.database.AddInParameter(sqlStringCommand, "bankName", System.Data.DbType.String, bdrinfo.BankName);
			this.database.AddInParameter(sqlStringCommand, "CellPhone", System.Data.DbType.String, bdrinfo.CellPhone);
			this.database.AddInParameter(sqlStringCommand, "MerchantCode", System.Data.DbType.String, bdrinfo.MerchantCode);
			this.database.AddInParameter(sqlStringCommand, "Remark", System.Data.DbType.String, bdrinfo.Remark);
			this.database.AddInParameter(sqlStringCommand, "CheckTime", System.Data.DbType.DateTime, bdrinfo.CheckTime);
			this.database.AddInParameter(sqlStringCommand, "IsCheck", System.Data.DbType.String, bdrinfo.IsCheck);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public bool MemberAmountAddByRefund(MemberInfo memberInfo, decimal amount, string orderid)
		{
			MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo();
			memberAmountDetailedInfo.UserId = memberInfo.UserId;
			memberAmountDetailedInfo.UserName = memberInfo.UserName;
			memberAmountDetailedInfo.PayId = Globals.GetGenerateId();
			memberAmountDetailedInfo.TradeAmount = decimal.Round(amount, 2);
			memberAmountDetailedInfo.TradeType = TradeType.OrderClose;
			memberAmountDetailedInfo.TradeTime = DateTime.Now;
			memberAmountDetailedInfo.TradeWays = TradeWays.Balance;
			memberAmountDetailedInfo.State = 1;
			memberAmountDetailedInfo.AvailableAmount = memberInfo.AvailableAmount + memberAmountDetailedInfo.TradeAmount;
			memberAmountDetailedInfo.Remark = "订单号：" + orderid;
			return new AmountDao().UseBalance(memberAmountDetailedInfo);
		}

		public bool CommissionToAmount(MemberAmountDetailedInfo amountinfo, int userid, decimal amount)
		{
			bool flag = this.CreatAmount(amountinfo);
			if (flag)
			{
				flag = this.UpdateMember(amountinfo);
				if (flag)
				{
					flag = new DistributorsDao().UpdateBalanceDistributors(userid, amount);
					if (flag)
					{
						BalanceDrawRequestInfo balanceDrawRequestInfo = new BalanceDrawRequestInfo();
						MemberInfo member = new MemberDao().GetMember(userid);
						balanceDrawRequestInfo.UserId = member.UserId;
						balanceDrawRequestInfo.RequestType = 4;
						balanceDrawRequestInfo.UserName = member.UserName;
						balanceDrawRequestInfo.RequestTime = DateTime.Now;
						balanceDrawRequestInfo.Amount = amount;
						balanceDrawRequestInfo.AccountName = (string.IsNullOrEmpty(member.RealName) ? member.UserName : member.RealName);
						balanceDrawRequestInfo.BankName = "";
						balanceDrawRequestInfo.MerchantCode = "佣金转余额";
						balanceDrawRequestInfo.Remark = "";
						balanceDrawRequestInfo.CheckTime = DateTime.Now;
						balanceDrawRequestInfo.CellPhone = (string.IsNullOrEmpty(member.CellPhone) ? "" : member.CellPhone);
						balanceDrawRequestInfo.IsCheck = "2";
						flag = this.AddBalanceDrawRequest(balanceDrawRequestInfo);
					}
				}
			}
			return flag;
		}

		public bool CreatAmountApplyRequest(MemberAmountRequestInfo applyInfo)
		{
			string query = "INSERT INTO Hishop_MemberAmountRequest(UserId,UserName,RequestTime,Amount,RequestType,AccountCode,AccountName,BankName,Remark,State,CellPhone) VALUES(@UserId,@UserName,@RequestTime,@Amount,@RequestType,@AccountCode,@AccountName,@BankName,@Remark,@State,@CellPhone)";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			this.database.AddInParameter(sqlStringCommand, "UserId", System.Data.DbType.Int32, applyInfo.UserId);
			this.database.AddInParameter(sqlStringCommand, "UserName", System.Data.DbType.String, applyInfo.UserName);
			this.database.AddInParameter(sqlStringCommand, "RequestTime", System.Data.DbType.DateTime, applyInfo.RequestTime);
			this.database.AddInParameter(sqlStringCommand, "Amount", System.Data.DbType.Decimal, applyInfo.Amount);
			this.database.AddInParameter(sqlStringCommand, "RequestType", System.Data.DbType.Int32, applyInfo.RequestType);
			this.database.AddInParameter(sqlStringCommand, "AccountCode", System.Data.DbType.String, applyInfo.AccountCode);
			this.database.AddInParameter(sqlStringCommand, "AccountName", System.Data.DbType.String, applyInfo.AccountName);
			this.database.AddInParameter(sqlStringCommand, "BankName", System.Data.DbType.String, applyInfo.BankName);
			this.database.AddInParameter(sqlStringCommand, "Remark", System.Data.DbType.String, applyInfo.Remark);
			this.database.AddInParameter(sqlStringCommand, "State", System.Data.DbType.Int32, applyInfo.State);
			this.database.AddInParameter(sqlStringCommand, "CellPhone", System.Data.DbType.String, applyInfo.CellPhone);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public DbQueryResult GetMemberAmountRequest(BalanceDrawRequestQuery query, string[] extendCheckStatus = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(query.StoreName))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" UserName LIKE '%{0}%'", DataHelper.CleanSearchString(query.StoreName));
			}
			if (!string.IsNullOrEmpty(query.UserId))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" UserId = {0}", DataHelper.CleanSearchString(query.UserId));
			}
			if (!string.IsNullOrEmpty(query.RequestTime.ToString()))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" convert(varchar(10),RequestTime,120)='{0}'", query.RequestTime);
			}
			if (!string.IsNullOrEmpty(query.IsCheck.ToString()))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" State={0}", query.IsCheck);
			}
			if (extendCheckStatus != null)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.Append(" State in (" + string.Join(",", extendCheckStatus) + ") ");
			}
			if (!string.IsNullOrEmpty(query.CheckTime.ToString()) && query.CheckTime.ToString() != "CheckTime")
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" convert(varchar(10),CheckTime,120)='{0}'", query.CheckTime);
			}
			if (!string.IsNullOrEmpty(query.CheckTime.ToString()) && query.CheckTime.ToString() == "CheckTime")
			{
				if (!string.IsNullOrEmpty(query.RequestStartTime.ToString()))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" AND ");
					}
					stringBuilder.AppendFormat(" datediff(dd,'{0}',CheckTime)>=0", query.RequestStartTime);
				}
				if (!string.IsNullOrEmpty(query.RequestEndTime.ToString()))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" AND ");
					}
					stringBuilder.AppendFormat("  datediff(dd,'{0}',CheckTime)<=0", query.RequestEndTime);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(query.RequestStartTime.ToString()))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" AND ");
					}
					stringBuilder.AppendFormat(" datediff(dd,'{0}',RequestTime)>=0", query.RequestStartTime);
				}
				if (!string.IsNullOrEmpty(query.RequestEndTime.ToString()))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" AND ");
					}
					stringBuilder.AppendFormat("  datediff(dd,'{0}',RequestTime)<=0", query.RequestEndTime);
				}
			}
			return DataHelper.PagingByRownumber(query.PageIndex, query.PageSize, query.SortBy, query.SortOrder, query.IsCount, "Hishop_MemberAmountRequest ", "Id", (stringBuilder.Length > 0) ? stringBuilder.ToString() : null, "*");
		}

		public bool SetAmountRequestStatus(int[] serialid, int checkValue, string Remark = "", string Amount = "", string Operator = "")
		{
			string text = "UPDATE Hishop_MemberAmountRequest set State=@State ";
			if (!string.IsNullOrEmpty(Remark))
			{
				text += ",Remark=@Remark ";
			}
			if (!string.IsNullOrEmpty(Amount))
			{
				text += ",Amount=@Amount ";
			}
			text += ",CheckTime=@CheckTime,Operator=@Operator";
			text = text + " where State not in(-1,2) and  Id in(" + string.Join<int>(",", serialid) + ")";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(text);
			this.database.AddInParameter(sqlStringCommand, "State", System.Data.DbType.Int16, checkValue);
			if (!string.IsNullOrEmpty(Remark))
			{
				this.database.AddInParameter(sqlStringCommand, "Remark", System.Data.DbType.String, Remark);
			}
			if (!string.IsNullOrEmpty(Amount))
			{
				this.database.AddInParameter(sqlStringCommand, "Amount", System.Data.DbType.Decimal, Amount);
			}
			this.database.AddInParameter(sqlStringCommand, "CheckTime", System.Data.DbType.DateTime, DateTime.Now);
			this.database.AddInParameter(sqlStringCommand, "Operator", System.Data.DbType.String, Operator);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public int GetAmountRequestStatus(int serialid)
		{
			string query = "select State from Hishop_MemberAmountRequest where Id=" + serialid;
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			string s = Globals.ToNum(this.database.ExecuteScalar(sqlStringCommand)).ToString();
			return int.Parse(s);
		}

		public Dictionary<int, int> GetMulAmountRequestStatus(int[] serialids)
		{
			string query = "select State,Id from Hishop_MemberAmountRequest where Id in(" + string.Join<int>(",", serialids) + ")";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (dataTable.Rows.Count > 0)
			{
				foreach (System.Data.DataRow dataRow in dataTable.Rows)
				{
					dictionary.Add((int)dataRow["Id"], (int)dataRow["State"]);
				}
			}
			return dictionary;
		}

		public MemberAmountRequestInfo GetAmountRequestDetail(int serialid)
		{
			MemberAmountRequestInfo memberAmountRequestInfo = new MemberAmountRequestInfo();
			string query = "select * from Hishop_MemberAmountRequest where Id=" + serialid;
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
			MemberAmountRequestInfo result;
			if (dataTable.Rows.Count > 0)
			{
				memberAmountRequestInfo.Id = serialid;
				memberAmountRequestInfo.UserId = int.Parse(dataTable.Rows[0]["UserId"].ToString());
				memberAmountRequestInfo.UserName = dataTable.Rows[0]["UserName"].ToString();
				memberAmountRequestInfo.RequestTime = DateTime.Parse(dataTable.Rows[0]["RequestTime"].ToString());
				memberAmountRequestInfo.Amount = Math.Round(decimal.Parse(dataTable.Rows[0]["Amount"].ToString()), 2);
				memberAmountRequestInfo.RequestType = (RequesType)dataTable.Rows[0]["RequestType"];
				memberAmountRequestInfo.AccountCode = dataTable.Rows[0]["AccountCode"].ToString();
				memberAmountRequestInfo.AccountName = dataTable.Rows[0]["AccountName"].ToString();
				memberAmountRequestInfo.BankName = dataTable.Rows[0]["BankName"].ToString();
				memberAmountRequestInfo.Remark = dataTable.Rows[0]["Remark"].ToString();
				memberAmountRequestInfo.RedpackId = ((dataTable.Rows[0]["RedpackId"] == DBNull.Value) ? "" : dataTable.Rows[0]["RedpackId"].ToString());
				memberAmountRequestInfo.State = (RequesState)dataTable.Rows[0]["State"];
				if (dataTable.Rows[0]["CheckTime"] != DBNull.Value)
				{
					memberAmountRequestInfo.CheckTime = new DateTime?(DateTime.Parse(dataTable.Rows[0]["CheckTime"].ToString()));
				}
				else
				{
					memberAmountRequestInfo.CheckTime = null;
				}
				memberAmountRequestInfo.CellPhone = dataTable.Rows[0]["CellPhone"].ToString();
				memberAmountRequestInfo.Operator = dataTable.Rows[0]["Operator"].ToString();
				if ((memberAmountRequestInfo.RequestType == RequesType.微信红包 || memberAmountRequestInfo.RequestType == RequesType.微信钱包) && string.IsNullOrEmpty(memberAmountRequestInfo.RedpackId))
				{
					SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
					string text = this.CreatRedpackId(masterSettings.WeixinPartnerID);
					query = "update Hishop_MemberAmountRequest set RedpackId=@redId where Id=@SerialID and RedpackId  is null ";
					sqlStringCommand = this.database.GetSqlStringCommand(query);
					this.database.AddInParameter(sqlStringCommand, "SerialID", System.Data.DbType.Int32, serialid);
					this.database.AddInParameter(sqlStringCommand, "redId", System.Data.DbType.String, text);
					int num = this.database.ExecuteNonQuery(sqlStringCommand);
					if (num > 0)
					{
						memberAmountRequestInfo.RedpackId = text;
					}
				}
				result = memberAmountRequestInfo;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public string CreatRedpackId(string mch_id)
		{
			string result = "";
			lock (AmountDao.RedLock)
			{
				result = mch_id + DateTime.Now.ToString("yyyymmdd") + DateTime.Now.ToString("MMddHHmmss");
				Thread.Sleep(1);
			}
			return result;
		}

		public string SendRedPackToAmountRequest(int serialid)
		{
			string result = string.Empty;
			string empty = string.Empty;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			if (!masterSettings.EnableWeiXinRequest)
			{
				result = "管理员后台未开启微信付款！";
			}
			else
			{
				string query = "select a.Id,a.userid,a.Amount,b.OpenID,isnull(b.OpenId,'') as OpenId from Hishop_MemberAmountRequest a inner join aspnet_Members b on a.userid=b.userid where Id=@SerialID and State=1";
				System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
				this.database.AddInParameter(sqlStringCommand, "SerialID", System.Data.DbType.Int32, serialid);
				System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
				string text = string.Empty;
				if (dataTable.Rows.Count > 0)
				{
					text = dataTable.Rows[0]["OpenId"].ToString();
					int userid = int.Parse(dataTable.Rows[0]["UserID"].ToString());
					decimal value = decimal.Parse(dataTable.Rows[0]["Amount"].ToString()) * 100m;
					int amount = Convert.ToInt32(value);
					if (string.IsNullOrEmpty(text))
					{
						result = "用户未绑定微信号";
					}
					else
					{
						query = "select top 1 ID from vshop_SendRedpackRecord where BalanceDrawRequestID=-" + dataTable.Rows[0]["Id"].ToString();
						sqlStringCommand = this.database.GetSqlStringCommand(query);
						if (this.database.ExecuteDataSet(sqlStringCommand).Tables[0].Rows.Count > 0)
						{
							result = "-1";
						}
						else
						{
							result = (this.CreateSendRedpackRecord(-serialid, userid, text, amount, "您的提现申请已成功", "恭喜您提现成功!") ? "1" : "提现操作失败");
						}
					}
				}
				else
				{
					result = "该用户没有提现申请,或者提现申请未审核";
				}
			}
			return result;
		}

		public bool CreateSendRedpackRecord(int serialid, int userid, string openid, int amount, string act_name, string wishing)
		{
			bool flag = true;
			int num = 20000;
			SendRedpackRecordInfo sendRedpackRecordInfo = new SendRedpackRecordInfo();
			sendRedpackRecordInfo.BalanceDrawRequestID = serialid;
			sendRedpackRecordInfo.UserID = userid;
			sendRedpackRecordInfo.OpenID = openid;
			sendRedpackRecordInfo.ActName = act_name;
			sendRedpackRecordInfo.Wishing = wishing;
			sendRedpackRecordInfo.ClientIP = Globals.IPAddress;
			using (System.Data.Common.DbConnection dbConnection = this.database.CreateConnection())
			{
				dbConnection.Open();
				System.Data.Common.DbTransaction dbTransaction = dbConnection.BeginTransaction();
				SendRedpackRecordDao sendRedpackRecordDao = new SendRedpackRecordDao();
				try
				{
					if (amount <= num)
					{
						sendRedpackRecordInfo.Amount = amount;
						flag = sendRedpackRecordDao.AddSendRedpackRecord(sendRedpackRecordInfo, dbTransaction);
					}
					else
					{
						int num2 = amount % num;
						int num3 = amount / num;
						if (num2 > 0)
						{
							sendRedpackRecordInfo.Amount = num2;
							flag = sendRedpackRecordDao.AddSendRedpackRecord(sendRedpackRecordInfo, dbTransaction);
						}
						if (flag)
						{
							for (int i = 0; i < num3; i++)
							{
								sendRedpackRecordInfo.Amount = num;
								flag = sendRedpackRecordDao.AddSendRedpackRecord(sendRedpackRecordInfo, dbTransaction);
								if (!flag)
								{
									dbTransaction.Rollback();
								}
							}
							int num4 = num3 + ((num2 > 0) ? 1 : 0);
							if (!flag)
							{
								dbTransaction.Rollback();
							}
						}
						else
						{
							dbTransaction.Rollback();
						}
					}
				}
				catch
				{
					if (dbTransaction.Connection != null)
					{
						dbTransaction.Rollback();
					}
					flag = false;
				}
				finally
				{
					if (flag)
					{
						dbTransaction.Commit();
					}
					dbConnection.Close();
				}
			}
			return flag;
		}

		public bool SetRedpackRecordIsUsed(int id, bool issend)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("UPDATE vshop_SendRedpackRecord set IsSend=@IsSend,SendTime=getdate() where ID=@ID");
			this.database.AddInParameter(sqlStringCommand, "ID", System.Data.DbType.Int32, id);
			this.database.AddInParameter(sqlStringCommand, "IsSend", System.Data.DbType.Boolean, issend);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public DbQueryResult GetAmountWithUserName(MemberAmountQuery query)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" State=1 ");
			if (query.UserId > 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("UserId = {0}", query.UserId);
			}
			if (!string.IsNullOrEmpty(query.UserName))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("UserName LIKE '%{0}%'", DataHelper.CleanSearchString(query.UserName));
			}
			if (!string.IsNullOrEmpty(query.PayId))
			{
				stringBuilder.AppendFormat(" and PayId = '{0}'", query.PayId);
			}
			if (!string.IsNullOrEmpty(query.TradeType))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" TradeType = {0}", query.TradeType);
			}
			if (!string.IsNullOrEmpty(query.TradeWays))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" TradeWays = {0}", query.TradeWays);
			}
			if (!string.IsNullOrEmpty(query.StartTime.ToString()))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" datediff(dd,'{0}',TradeTime)>=0", query.StartTime);
			}
			if (!string.IsNullOrEmpty(query.EndTime.ToString()))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("  datediff(dd,'{0}',TradeTime)<=0", query.EndTime);
			}
			string table = "Hishop_MemberAmountDetailed";
			return DataHelper.PagingByRownumber(query.PageIndex, query.PageSize, query.SortBy, query.SortOrder, query.IsCount, table, "Id", (stringBuilder.Length > 0) ? stringBuilder.ToString() : null, "*");
		}

		public Dictionary<string, decimal> GetAmountDic(MemberAmountQuery query)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" State=1 ");
			if (query.UserId > 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("UserId = {0}", query.UserId);
			}
			if (!string.IsNullOrEmpty(query.UserName))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("UserName LIKE '%{0}%'", DataHelper.CleanSearchString(query.UserName));
			}
			if (!string.IsNullOrEmpty(query.PayId))
			{
				stringBuilder.AppendFormat(" and PayId = '{0}'", query.PayId);
			}
			if (!string.IsNullOrEmpty(query.TradeType))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" TradeType = {0}", query.TradeType);
			}
			if (!string.IsNullOrEmpty(query.TradeWays))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" TradeWays = {0}", query.TradeWays);
			}
			if (!string.IsNullOrEmpty(query.StartTime.ToString()))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" datediff(dd,'{0}',TradeTime)>=0", query.StartTime);
			}
			if (!string.IsNullOrEmpty(query.EndTime.ToString()))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("  datediff(dd,'{0}',TradeTime)<=0", query.EndTime);
			}
			string query2 = "select isnull(sum(a.TotalAmount),0) AS CurrentTotal,isnull(sum(a.AvailableAmount),0) AS AvailableTotal from aspnet_Members a where exists (select userid  from Hishop_MemberAmountDetailed where userid=a.userid and " + stringBuilder + ") ";
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query2);
			System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
			dictionary.Add("CurrentTotal", decimal.Parse(dataTable.Rows[0]["CurrentTotal"].ToString()));
			dictionary.Add("AvailableTotal", decimal.Parse(dataTable.Rows[0]["AvailableTotal"].ToString()));
			string query3 = "SELECT isnull(sum(Amount),0) FROM Hishop_MemberAmountRequest a WHERE exists (select userid  from Hishop_MemberAmountDetailed where userid=a.userid and " + stringBuilder + ") and State in(0,1,3)";
			System.Data.Common.DbCommand sqlStringCommand2 = this.database.GetSqlStringCommand(query3);
			decimal value = decimal.Parse(this.database.ExecuteScalar(sqlStringCommand2).ToString());
			dictionary.Add("UnliquidatedTotal", value);
			return dictionary;
		}

		public Dictionary<string, decimal> GetDataByUserId(int userid)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			string query = "select COUNT(*) AS OrderCount,isnull(SUM(ValidOrderTotal),0) AS OrderTotal from  dbo.vw_VShop_FinishOrder_Main where UserId=" + userid;
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(query);
			System.Data.DataTable dataTable = this.database.ExecuteDataSet(sqlStringCommand).Tables[0];
			dictionary.Add("OrderCount", decimal.Parse(dataTable.Rows[0]["OrderCount"].ToString()));
			dictionary.Add("OrderTotal", decimal.Parse(dataTable.Rows[0]["OrderTotal"].ToString()));
			string query2 = "SELECT isnull(sum(Amount),0) FROM Hishop_MemberAmountRequest a WHERE State in(0,1,3) and userid=" + userid;
			System.Data.Common.DbCommand sqlStringCommand2 = this.database.GetSqlStringCommand(query2);
			decimal value = decimal.Parse(this.database.ExecuteScalar(sqlStringCommand2).ToString());
			dictionary.Add("RequestAmount", value);
			return dictionary;
		}

		public DbQueryResult GetAmountListRequestByTime(int type, int page, int pagesize, int userId, string startTime = "", string endTime = "")
		{
			string table = "Hishop_MemberAmountDetailed";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" State={0} AND UserId={1} ", 1, userId);
			if (type != 0)
			{
				if (type == 1)
				{
					stringBuilder.AppendFormat(" AND TradeAmount < 0 ", new object[0]);
				}
				else if (type == 2)
				{
					stringBuilder.AppendFormat(" AND TradeAmount > 0 ", new object[0]);
				}
			}
			if (!string.IsNullOrEmpty(startTime))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat(" datediff(dd,'{0}',TradeTime)>=0", startTime);
			}
			if (!string.IsNullOrEmpty(endTime))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				stringBuilder.AppendFormat("  datediff(dd,'{0}',TradeTime)<=0", endTime);
			}
			return DataHelper.PagingByRownumber(page, pagesize, "TradeTime", SortAction.Desc, true, table, "ID", (stringBuilder.Length > 0) ? stringBuilder.ToString() : null, "*");
		}
	}
}
