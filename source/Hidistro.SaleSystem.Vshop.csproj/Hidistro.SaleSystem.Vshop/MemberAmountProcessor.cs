using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Sales;
using Hidistro.Entities.VShop;
using Hidistro.Messages;
using Hidistro.SqlDal.Members;
using Hidistro.SqlDal.VShop;
using System;
using System.Collections.Generic;

namespace Hidistro.SaleSystem.Vshop
{
	public static class MemberAmountProcessor
	{
		public static bool CreatAmount(MemberAmountDetailedInfo AmountInfo)
		{
			AmountDao amountDao = new AmountDao();
			return amountDao.CreatAmount(AmountInfo);
		}

		public static decimal GetUserMaxAmountDetailed(int userid)
		{
			return new AmountDao().GetUserMaxAmountDetailed(userid);
		}

		public static DbQueryResult GetBalanceWithdrawListRequest(int type, int page, int pagesize, int userId)
		{
			return new AmountDao().GetBalanceWithdrawListRequest(type, page, pagesize, userId);
		}

		public static DbQueryResult GetAmountListRequest(int type, int page, int pagesize, int userId)
		{
			AmountDao amountDao = new AmountDao();
			return amountDao.GetAmountListRequest(type, page, pagesize, userId);
		}

		public static MemberAmountDetailedInfo GetAmountDetail(int Id)
		{
			AmountDao amountDao = new AmountDao();
			return amountDao.GetAmountDetail(Id);
		}

		public static MemberAmountDetailedInfo GetAmountDetailByPayId(string PayId)
		{
			AmountDao amountDao = new AmountDao();
			return amountDao.GetAmountDetailByPayId(PayId);
		}

		public static PaymentModeInfo GetPaymentMode(TradeWays ways)
		{
			return new AmountDao().GetPaymentMode(ways);
		}

		public static bool UserPayOrder(MemberAmountDetailedInfo model)
		{
			AmountDao amountDao = new AmountDao();
			model.State = 1;
			bool flag = amountDao.UpdateAmount(model);
			if (flag)
			{
				flag = amountDao.UpdateMember(model);
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				Globals.Debuglog("触发自动成为分销商的条件", "_DebuglogMemberAutoToDistributor.txt");
				MemberInfo member = MemberProcessor.GetMember(model.UserId, true);
				bool flag2 = VshopBrowser.IsPassAutoToDistributor(member, true);
				if (flag2)
				{
					DistributorsBrower.MemberAutoToDistributor(member);
				}
			}
			else
			{
				Globals.Debuglog("充值操作重复提交了！！！" + model.UserId, "_DebuglogMemberAutoToDistributor.txt");
			}
			return flag;
		}

		public static bool CommissionToAmount(MemberAmountDetailedInfo amountinfo, int userid, decimal amount)
		{
			AmountDao amountDao = new AmountDao();
			return amountDao.CommissionToAmount(amountinfo, userid, amount);
		}

		public static bool CreatAmountApplyRequest(MemberAmountRequestInfo applyInfo)
		{
			AmountDao amountDao = new AmountDao();
			bool flag = amountDao.CreatAmountApplyRequest(applyInfo);
			if (flag)
			{
				MemberInfo member = new MemberDao().GetMember(applyInfo.UserId);
				MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo
				{
					UserId = applyInfo.UserId,
					TradeAmount = -applyInfo.Amount,
					PayId = Globals.GetGenerateId(),
					UserName = applyInfo.UserName,
					TradeType = TradeType.Withdrawals,
					TradeTime = DateTime.Now,
					State = 1,
					TradeWays = MemberAmountProcessor.GetWaysByRequestType(applyInfo.RequestType),
					AvailableAmount = member.AvailableAmount - applyInfo.Amount,
					Remark = "余额提现。收款账号：" + applyInfo.AccountCode
				};
				flag = (amountDao.UpdateMember(memberAmountDetailedInfo) && MemberAmountProcessor.CreatAmount(memberAmountDetailedInfo));
			}
			return flag;
		}

		public static TradeWays GetWaysByRequestType(RequesType type)
		{
			TradeWays result = TradeWays.Balance;
			switch (type)
			{
			case RequesType.微信钱包:
				result = TradeWays.WeChatWallet;
				break;
			case RequesType.支付宝:
				result = TradeWays.Alipay;
				break;
			case RequesType.线下支付:
				result = TradeWays.LineTransfer;
				break;
			case RequesType.微信红包:
				result = TradeWays.WeChatWallet;
				break;
			}
			return result;
		}

		public static DbQueryResult GetMemberAmountRequest(BalanceDrawRequestQuery query, string[] extendChecks = null)
		{
			return new AmountDao().GetMemberAmountRequest(query, extendChecks);
		}

		public static bool SetAmountRequestStatus(int[] serialids, int checkValue, string Remark = "", string Amount = "", string Operator = "")
		{
			bool flag = new AmountDao().SetAmountRequestStatus(serialids, checkValue, Remark, Amount, Operator);
			if (checkValue == -1 && flag)
			{
				for (int i = 0; i < serialids.Length; i++)
				{
					int serialid = serialids[i];
					MemberAmountRequestInfo amountRequestDetail = MemberAmountProcessor.GetAmountRequestDetail(serialid);
					MemberInfo member = new MemberDao().GetMember(amountRequestDetail.UserId);
					MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo
					{
						UserId = amountRequestDetail.UserId,
						TradeAmount = amountRequestDetail.Amount,
						PayId = Globals.GetGenerateId(),
						UserName = amountRequestDetail.UserName,
						TradeType = TradeType.WithdrawalsRefuse,
						TradeTime = DateTime.Now,
						State = 1,
						TradeWays = MemberAmountProcessor.GetWaysByRequestType(amountRequestDetail.RequestType),
						AvailableAmount = member.AvailableAmount + amountRequestDetail.Amount,
						Remark = "余额提现驳回"
					};
					flag = (new AmountDao().UpdateMember(memberAmountDetailedInfo) && MemberAmountProcessor.CreatAmount(memberAmountDetailedInfo));
					MemberAmountRequestInfo amountRequestDetail2 = MemberAmountProcessor.GetAmountRequestDetail(serialid);
					if (amountRequestDetail2 != null)
					{
						string url = Globals.FullPath("/Vshop/MemberAmountRequestDetail.aspx?Id=" + amountRequestDetail2.Id);
						try
						{
							Messenger.SendWeiXinMsg_MemberAmountDrawCashRefuse(amountRequestDetail2, url);
						}
						catch
						{
						}
					}
				}
			}
			return flag;
		}

		public static int GetAmountRequestStatus(int serialid)
		{
			return new AmountDao().GetAmountRequestStatus(serialid);
		}

		public static Dictionary<int, int> GetMulAmountRequestStatus(int[] serialids)
		{
			return new AmountDao().GetMulAmountRequestStatus(serialids);
		}

		public static MemberAmountRequestInfo GetAmountRequestDetail(int serialid)
		{
			return new AmountDao().GetAmountRequestDetail(serialid);
		}

		public static string SendRedPackToBalanceDrawRequest(int serialid)
		{
			return new AmountDao().SendRedPackToAmountRequest(serialid);
		}

		public static SendRedpackRecordInfo GetSendRedpackRecordByID(string id = null, string sid = null)
		{
			return new SendRedpackRecordDao().GetSendRedpackRecordByID(id, sid);
		}

		public static bool SetRedpackRecordIsUsed(int id, bool issend)
		{
			return new AmountDao().SetRedpackRecordIsUsed(id, issend);
		}

		public static DbQueryResult GetAmountWithUserName(MemberAmountQuery query)
		{
			return new AmountDao().GetAmountWithUserName(query);
		}

		public static Dictionary<string, decimal> GetAmountDic(MemberAmountQuery query)
		{
			return new AmountDao().GetAmountDic(query);
		}

		public static Dictionary<string, decimal> GetDataByUserId(int userid)
		{
			return new AmountDao().GetDataByUserId(userid);
		}

		public static DbQueryResult GetAmountListRequestByTime(int type, int page, int pagesize, int userId, string startTime = "", string endTime = "")
		{
			AmountDao amountDao = new AmountDao();
			return amountDao.GetAmountListRequestByTime(type, page, pagesize, userId, startTime, endTime);
		}

		public static bool SetAmountByShopAdjustment(MemberAmountDetailedInfo model)
		{
			AmountDao amountDao = new AmountDao();
			bool flag = amountDao.CreatAmount(model);
			if (flag)
			{
				flag = amountDao.UpdateMember(model);
			}
			return flag;
		}
	}
}
