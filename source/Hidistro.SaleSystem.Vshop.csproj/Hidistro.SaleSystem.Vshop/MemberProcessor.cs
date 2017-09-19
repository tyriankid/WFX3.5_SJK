using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Sales;
using Hidistro.Messages;
using Hidistro.SqlDal.Commodities;
using Hidistro.SqlDal.Members;
using Hidistro.SqlDal.Orders;
using Hidistro.SqlDal.Promotions;
using Hidistro.SqlDal.Sales;
using Hidistro.SqlDal.VShop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Caching;

namespace Hidistro.SaleSystem.Vshop
{
	public static class MemberProcessor
	{
		public static string GetBuyerName(int userId)
		{
			string result = string.Empty;
			MemberInfo member = MemberProcessor.GetMember(userId, true);
			if (member != null)
			{
				if (!string.IsNullOrEmpty(member.OpenId))
				{
					result = member.UserName;
				}
				else if (!string.IsNullOrEmpty(member.UserBindName))
				{
					result = member.UserBindName;
				}
				else
				{
					result = member.RealName;
				}
			}
			return result;
		}

		public static decimal GetIntegral(int userId)
		{
			return new PointDetailDao().GetIntegral(userId);
		}

		public static bool AddIntegralDetail(IntegralDetailInfo point)
		{
			return new IntegralDetailDao().AddIntegralDetail(point, null);
		}

		public static DbQueryResult GetIntegralDetail(IntegralDetailQuery query)
		{
			return new IntegralDetailDao().GetIntegralDetail(query);
		}

		public static bool CancelOrder(OrderInfo order)
		{
			bool result = false;
			if (order.CheckAction(OrderActions.SELLER_CLOSE))
			{
				order.OrderStatus = OrderStatus.Closed;
				result = new OrderDao().UpdateOrder(order, null);
				new OrderDao().UpdateItemsStatus(order.OrderId, 4, "all");
				Point.SetPointAndBalanceByOrderId(order);
			}
			return result;
		}

		public static void GetAutoBatchOrdersIdList()
		{
			System.Data.DataSet autoBatchOrdersIdList = new OrderDao().GetAutoBatchOrdersIdList();
			if (autoBatchOrdersIdList.Tables.Count == 2)
			{
				System.Data.DataTable dataTable = autoBatchOrdersIdList.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					OrderInfo orderInfo = new OrderDao().GetOrderInfo(dataTable.Rows[i]["OrderID"].ToString());
					LineItemInfo lineItemInfo = new LineItemInfo();
					Dictionary<string, LineItemInfo> lineItems = orderInfo.LineItems;
					orderInfo.CloseReason = "到期自动关闭";
					MemberProcessor.CancelOrder(orderInfo);
				}
				dataTable = autoBatchOrdersIdList.Tables[1];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					OrderInfo orderInfo = new OrderDao().GetOrderInfo(dataTable.Rows[i]["OrderID"].ToString());
					Dictionary<string, LineItemInfo> lineItems = orderInfo.LineItems;
					LineItemInfo lineItemInfo = new LineItemInfo();
					orderInfo.CloseReason = "订单自动完成";
					if (MemberProcessor.ConfirmOrderFinish(orderInfo))
					{
						DistributorsBrower.UpdateCalculationCommission(orderInfo);
						foreach (LineItemInfo current in orderInfo.LineItems.Values)
						{
							if (current.OrderItemsStatus.ToString() == OrderStatus.SellerAlreadySent.ToString())
							{
								ShoppingProcessor.UpdateOrderGoodStatu(orderInfo.OrderId, current.SkuId, 5, current.ID);
							}
						}
					}
				}
			}
		}

		public static bool ConfirmOrderFinish(OrderInfo order)
		{
			bool result = false;
			if (order.CheckAction(OrderActions.BUYER_CONFIRM_GOODS))
			{
				DateTime now = DateTime.Now;
				order.OrderStatus = OrderStatus.Finished;
				order.FinishDate = new DateTime?(now);
				if (!order.PayDate.HasValue)
				{
					order.PayDate = new DateTime?(now);
					new MemberDao().SetOrderDate(order.UserId, 1);
				}
				result = new OrderDao().UpdateOrder(order, null);
				HiCache.Remove(string.Format("DataCache-Member-{0}", order.UserId));
			}
			return result;
		}

		public static bool UserPayOrder(OrderInfo order)
		{
			OrderDao orderDao = new OrderDao();
			order.OrderStatus = OrderStatus.BuyerAlreadyPaid;
			order.PayDate = new DateTime?(DateTime.Now);
			bool flag = orderDao.UpdateOrder(order, null);
			string text = "";
			Globals.Debuglog(string.Concat(new string[]
			{
				"调用付款:",
				order.OrderId,
				",成功否：",
				flag.ToString(),
				",order.UserId:",
				order.UserId.ToString()
			}), "_DebuglogPaymentTest.txt");
			if (flag)
			{
				orderDao.UpdatePayOrderStock(order);
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				MemberInfo member = MemberProcessor.GetMember(order.UserId, true);
				bool flag2 = VshopBrowser.IsPassAutoToDistributor(member, true);
				if (flag2)
				{
					DistributorsBrower.MemberAutoToDistributor(member);
				}
				new MemberDao().SetOrderDate(order.UserId, 1);
				foreach (LineItemInfo current in order.LineItems.Values)
				{
					ProductDao productDao = new ProductDao();
					text = text + "'" + current.SkuId + "',";
					ProductInfo productDetails = productDao.GetProductDetails(current.ProductId);
					productDetails.SaleCounts += current.Quantity;
					productDetails.ShowSaleCounts += current.Quantity;
					productDao.UpdateProduct(productDetails, null);
				}
				if (!string.IsNullOrEmpty(text))
				{
					orderDao.UpdateItemsStatus(order.OrderId, 2, text.Substring(0, text.Length - 1));
				}
				if (!string.IsNullOrEmpty(order.ActivitiesId))
				{
					ActivitiesDao activitiesDao = new ActivitiesDao();
					activitiesDao.UpdateActivitiesTakeEffect(order.ActivitiesId);
				}
				if (member != null)
				{
					Globals.Debuglog(string.Concat(new string[]
					{
						"发微信消息:",
						order.OrderId,
						",成功：",
						flag.ToString(),
						",order.UserId:",
						order.UserId.ToString()
					}), "_DebuglogPaymentTest.txt");
					try
					{
						Messenger.SendWeiXinMsg_OrderPay(order);
					}
					catch
					{
					}
				}
			}
			return flag;
		}

		public static IList<MemberGradeInfo> GetMemberGrades(string grids)
		{
			return new MemberGradeDao().GetMemberGrades(grids);
		}

		public static void UpdateUserAccount(OrderInfo order)
		{
			MemberDao memberDao = new MemberDao();
			decimal money = order.GetTotal() - order.AdjustedFreight;
			int point = Globals.GetPoint(money);
			if (point > 0)
			{
				IntegralDetailInfo integralDetailInfo = new IntegralDetailInfo();
				integralDetailInfo.IntegralChange = point;
				integralDetailInfo.IntegralSource = "购物送积分,订单号：" + order.OrderId;
				integralDetailInfo.IntegralSourceType = 1;
				integralDetailInfo.IntegralStatus = 1;
				integralDetailInfo.Userid = order.UserId;
				integralDetailInfo.Remark = "";
				new IntegralDetailDao().AddIntegralDetail(integralDetailInfo, null);
				try
				{
					if (order != null)
					{
						Messenger.SendWeiXinMsg_OrderGetPoint(order, point);
					}
				}
				catch (Exception var_4_B6)
				{
				}
			}
			MemberInfo member = new MemberDao().GetMember(order.UserId);
			member.Expenditure += order.GetTotal();
			member.OrderNumber++;
			memberDao.Update(member);
			MemberGradeInfo memberGrade = MemberProcessor.GetMemberGrade(member.GradeId);
			if (memberGrade != null)
			{
				bool flag = false;
				if (memberGrade.TranVol.HasValue)
				{
					flag = (memberGrade.TranVol.Value < double.Parse(member.Expenditure.ToString()));
				}
				bool flag2 = false;
				if (memberGrade.TranTimes.HasValue)
				{
					flag2 = (memberGrade.TranTimes < member.OrderNumber);
				}
				if (flag || flag2)
				{
					List<MemberGradeInfo> source = new MemberGradeDao().GetMemberGrades("") as List<MemberGradeInfo>;
					MemberGradeInfo memberGradeInfo = null;
					if (flag)
					{
						IOrderedEnumerable<MemberGradeInfo> source2 = from m in source
						where m.TranVol.HasValue
						orderby m.TranVol descending
						select m;
						memberGradeInfo = source2.FirstOrDefault((MemberGradeInfo m) => (decimal)m.TranVol.Value <= member.Expenditure);
					}
					MemberGradeInfo memberGradeInfo2;
					if (flag2)
					{
						IOrderedEnumerable<MemberGradeInfo> source2 = from m in source
						where m.TranTimes.HasValue
						orderby m.TranTimes descending
						select m;
						memberGradeInfo2 = source2.FirstOrDefault((MemberGradeInfo m) => m.TranTimes.Value <= member.OrderNumber);
					}
					else
					{
						memberGradeInfo2 = memberGradeInfo;
					}
					if (memberGradeInfo == null)
					{
						memberGradeInfo = memberGradeInfo2;
					}
					if (memberGradeInfo != null)
					{
						double? tranVol = memberGradeInfo.TranVol;
						double? tranVol2 = memberGradeInfo2.TranVol;
						MemberGradeInfo memberGradeInfo3;
						if (tranVol.GetValueOrDefault() > tranVol2.GetValueOrDefault() && (tranVol.HasValue & tranVol2.HasValue))
						{
							memberGradeInfo3 = memberGradeInfo;
						}
						else
						{
							memberGradeInfo3 = memberGradeInfo2;
						}
						bool arg_3C7_0;
						if (memberGrade.GradeId != memberGradeInfo3.GradeId)
						{
							tranVol = memberGrade.TranVol;
							tranVol2 = memberGradeInfo3.TranVol;
							arg_3C7_0 = (tranVol.GetValueOrDefault() <= tranVol2.GetValueOrDefault() || !(tranVol.HasValue & tranVol2.HasValue));
						}
						else
						{
							arg_3C7_0 = false;
						}
						if (arg_3C7_0)
						{
							member.GradeId = memberGradeInfo3.GradeId;
							memberDao.Update(member);
							try
							{
								MemberInfo member2 = member;
								if (member2 != null)
								{
									Messenger.SendWeiXinMsg_MemberGradeChange(member);
								}
							}
							catch (Exception var_4_B6)
							{
							}
						}
					}
				}
			}
		}

		public static string GetUserOpenIdByUserId(int UserId)
		{
			MemberDao memberDao = new MemberDao();
			return memberDao.GetOpenIDByUserId(UserId);
		}

		public static string GetAliUserOpenIdByUserId(int UserId)
		{
			MemberDao memberDao = new MemberDao();
			return memberDao.GetAliOpenIDByUserId(UserId);
		}

		public static System.Data.DataTable GetMembersByUserId(int referralUserId, int pageIndex, int pageSize, out int total, string sort, string order)
		{
			return new MemberDao().GetMembersByUserId(referralUserId, pageIndex, pageSize, out total, sort, order);
		}

		public static System.Data.DataTable GetUserCoupons(int userId, int useType = 0)
		{
			return new CouponDao().GetUserCoupons(userId, useType);
		}

		public static System.Data.DataTable GetUserCoupons()
		{
			return new CouponDao().GetUserCoupons();
		}

		public static System.Data.DataTable GetCouponByProducts(int couponId, int ProductId)
		{
			return new CouponDao().GetCouponByProducts(couponId, ProductId);
		}

		public static OrderInfo GetUserLastOrder(int userId)
		{
			return new OrderDao().GetUserLastOrder(userId);
		}

		public static System.Data.DataSet GetUserOrder(int userId, OrderQuery query)
		{
			return new OrderDao().GetUserOrder(userId, query);
		}

		public static DbQueryResult GetUserOrderByPage(int userId, OrderQuery query)
		{
			return new OrderDao().GetUserOrderByPage(userId, query);
		}

		public static System.Data.DataSet GetUserOrderReturn(int userId, OrderQuery query)
		{
			return new OrderDao().GetUserOrderReturn(userId, query);
		}

		public static int GetUserOrderReturnCount(int userId)
		{
			return new OrderDao().GetUserOrderReturnCount(userId);
		}

		public static int GetUserOrderCount(int userId, OrderQuery query)
		{
			return new OrderDao().GetUserOrderCount(userId, query);
		}

		public static int GetDefaultMemberGrade()
		{
			return new MemberGradeDao().GetDefaultMemberGrade();
		}

		public static bool SetAlipayInfos(MemberInfo user)
		{
			return new MemberDao().SetAlipayInfos(user) > 0;
		}

		public static int GetMemberNumOfTotal(int topUserId, out int topNum)
		{
			return new MemberDao().GetMemberNumOfTotal(topUserId, out topNum);
		}

		public static int GetDistributorNumOfTotal(int topUserId, out int topNum)
		{
			return new MemberDao().GetDistributorNumOfTotal(topUserId, out topNum);
		}

		public static MemberGradeInfo GetMemberGrade(int gradeId)
		{
			return new MemberGradeDao().GetMemberGrade(gradeId);
		}

		public static bool IsExitOpenId(string Opneid)
		{
			return new MemberDao().IsExitOpenId(Opneid);
		}

		public static bool CreateMember(MemberInfo member)
		{
			MemberDao memberDao = new MemberDao();
			bool flag = memberDao.CreateMember(member);
			if (flag)
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				if (member.UserId == 0)
				{
					Globals.Debuglog("没有UserID的注册身份" + member.SessionId, "_DebuglogReg.txt");
					member = MemberProcessor.GetMember(member.SessionId);
				}
				bool flag2 = VshopBrowser.IsPassAutoToDistributor(member, true);
				if (flag2)
				{
					DistributorsBrower.MemberAutoToDistributor(member);
				}
				try
				{
					Messenger.SendWeiXinMsg_MemberRegister(member);
				}
				catch (Exception var_4_83)
				{
				}
				CouponProcessor.RegisterSendCoupon(member.SessionId);
			}
			return flag;
		}

		public static bool BindUserName(int UserId, string UserBindName, string Password)
		{
			MemberDao memberDao = new MemberDao();
			return memberDao.BindUserName(UserId, UserBindName, Password);
		}

		public static MemberInfo GetBindusernameMember(string UserBindName)
		{
			MemberDao memberDao = new MemberDao();
			return memberDao.GetBindusernameMember(UserBindName);
		}

		public static MemberInfo GetOpenIdMember(string OpenId, string From = "wx")
		{
			MemberDao memberDao = new MemberDao();
			return memberDao.GetOpenIdMember(OpenId, From);
		}

		public static bool SetMemberSessionId(MemberInfo member)
		{
			MemberDao memberDao = new MemberDao();
			return memberDao.SetMemberSessionId(member.SessionId, member.SessionEndTime, member.OpenId);
		}

		public static bool SetMemberSessionId(string sessionId, DateTime sessionEndTime, string openId)
		{
			return new MemberDao().SetMemberSessionId(sessionId, sessionEndTime, openId);
		}

		public static bool SetPwd(string userid, string pwd)
		{
			return new MemberDao().SetPwd(userid, pwd);
		}

		public static bool ReSetUserHead(string userid, string wxName, string wxHead, string Openid = "")
		{
			return new MemberDao().ReSetUserHead(userid, wxName, wxHead, Openid);
		}

		public static int SetMultiplePwd(string userids, string pwd)
		{
			return new MemberDao().SetMultiplePwd(userids, pwd);
		}

		public static List<MemberInfo> GetMemberInfoList(string userIds)
		{
			return new MemberDao().GetMemberInfoList(userIds);
		}

		public static bool UpdateMember(MemberInfo member)
		{
			HiCache.Remove(string.Format("DataCache-Member-{0}", member.UserId));
			return new MemberDao().Update(member);
		}

		public static bool Delete(int userId)
		{
			bool flag = new MemberDao().Delete(userId);
			if (flag)
			{
				HiCache.Remove(string.Format("DataCache-Member-{0}", userId));
			}
			return flag;
		}

		public static MemberInfo GetMember(int userId, bool isUserCache = true)
		{
			MemberInfo memberInfo;
			if (isUserCache)
			{
				memberInfo = (HiCache.Get(string.Format("DataCache-Member-{0}", userId)) as MemberInfo);
				if (memberInfo == null)
				{
					memberInfo = new MemberDao().GetMember(userId);
					HiCache.Insert(string.Format("DataCache-Member-{0}", userId), memberInfo, 360, CacheItemPriority.Normal);
				}
			}
			else
			{
				memberInfo = new MemberDao().GetMember(userId);
			}
			return memberInfo;
		}

		public static MemberInfo GetCurrentMember()
		{
			return new MemberDao().GetMember(Globals.GetCurrentMemberUserId(false));
		}

		public static MemberInfo GetMember()
		{
			return MemberProcessor.GetMember(Globals.GetCurrentMemberUserId(false), true);
		}

		public static MemberInfo GetMember(string sessionId)
		{
			return new MemberDao().GetMember(sessionId);
		}

		public static MemberInfo GetusernameMember(string username)
		{
			return new MemberDao().GetusernameMember(username);
		}

		public static bool DelUserMessage(int userid, string openid, string userhead, int olduserid)
		{
			return new MemberDao().DelUserMessage(userid, openid, userhead, olduserid);
		}

		public static bool DelUserMessage(MemberInfo newuser, int olduserid)
		{
			return new MemberDao().DelUserMessage(newuser, olduserid);
		}

		public static IList<ShippingAddressInfo> GetShippingAddresses()
		{
			return new ShippingAddressDao().GetShippingAddresses(Globals.GetCurrentMemberUserId(false));
		}

		public static ShippingAddressInfo GetShippingAddress(int shippingId)
		{
			return new ShippingAddressDao().GetShippingAddress(shippingId, Globals.GetCurrentMemberUserId(false));
		}

		public static ShippingAddressInfo GetDefaultShippingAddress()
		{
			IList<ShippingAddressInfo> shippingAddresses = new ShippingAddressDao().GetShippingAddresses(Globals.GetCurrentMemberUserId(false));
			ShippingAddressInfo result;
			foreach (ShippingAddressInfo current in shippingAddresses)
			{
				if (current.IsDefault)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		public static int GetShippingAddressCount()
		{
			return new ShippingAddressDao().GetShippingAddresses(Globals.GetCurrentMemberUserId(false)).Count;
		}

		public static int AddShippingAddress(ShippingAddressInfo shippingAddress)
		{
			ShippingAddressDao shippingAddressDao = new ShippingAddressDao();
			MemberDao memberDao = new MemberDao();
			int shippingId = shippingAddressDao.AddShippingAddress(shippingAddress);
			int result;
			if (shippingAddressDao.SetDefaultShippingAddress(shippingId, Globals.GetCurrentMemberUserId(false)))
			{
				memberDao.SaveMemberInfoByAddress(shippingAddress);
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static bool UpdateShippingAddress(ShippingAddressInfo shippingAddress)
		{
			return new ShippingAddressDao().UpdateShippingAddress(shippingAddress);
		}

		public static bool SetDefaultShippingAddress(int shippingId, int UserId)
		{
			return new ShippingAddressDao().SetDefaultShippingAddress(shippingId, UserId);
		}

		public static bool DelShippingAddress(int shippingid, int userid)
		{
			return new ShippingAddressDao().DelShippingAddress(shippingid, userid);
		}

		public static bool UpdateUserFollowStateByUserId(int UserId, int state, string type = "wx")
		{
			return new MemberDao().UpdateUserFollowStateByUserId(UserId, state, type);
		}

		public static bool UpdateUserFollowStateByOpenId(string openid, int state)
		{
			return new MemberDao().UpdateUserFollowStateByOpenId(openid, state);
		}

		public static int GetUserFollowStateByUserId(int UserId, string type = "wx")
		{
			return new MemberDao().GetUserFollowStateByUserId(UserId, type);
		}

		public static bool DelFuwuFollowUser(string openid)
		{
			return new MemberDao().DelFuwuFollowUser(openid);
		}

		public static bool AddFuwuFollowUser(string openid)
		{
			return new MemberDao().addFuwuFollowUser(openid);
		}

		public static bool IsFuwuFollowUser(string openid)
		{
			return new MemberDao().IsFuwuFollowUser(openid);
		}

		public static bool CheckCurrentMemberIsInRange(string Grades, string DefualtGroup, string CustomGroup)
		{
			return new MemberDao().CheckCurrentMemberIsInRange(Grades, DefualtGroup, CustomGroup, 0);
		}

		public static bool CheckMemberIsBuyProds(int userId, string prodIds, DateTime? startTime, DateTime? endTime)
		{
			return new MemberDao().CheckMemberIsBuyProds(userId, prodIds, startTime, endTime);
		}
	}
}
