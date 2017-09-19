using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.VShop;
using Hidistro.SqlDal.Commodities;
using Hidistro.SqlDal.Orders;
using Hidistro.SqlDal.Promotions;
using Hidistro.SqlDal.VShop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Hidistro.SaleSystem.Vshop
{
	public static class VshopBrowser
	{
		public static MessageInfo GetMessage(int messageId)
		{
			return new ReplyDao().GetMessage(messageId);
		}

		public static System.Data.DataTable GetHomeProducts()
		{
			return new HomeProductDao().GetHomeProducts();
		}

		public static System.Data.DataTable GetVote(int voteId, out string voteName, out int checkNum, out int voteNum)
		{
			return new VoteDao().LoadVote(voteId, out voteName, out checkNum, out voteNum);
		}

		public static bool Vote(int voteId, string itemIds)
		{
			return new VoteDao().Vote(voteId, itemIds);
		}

		public static bool IsVote(int voteId)
		{
			return new VoteDao().IsVote(voteId);
		}

		public static Hidistro.Entities.VShop.ActivityInfo GetActivity(int activityId)
		{
			return new Hidistro.SqlDal.VShop.ActivityDao().GetActivity(activityId);
		}

		public static IList<BannerInfo> GetAllBanners()
		{
			return new BannerDao().GetAllBanners();
		}

		public static IList<NavigateInfo> GetAllNavigate()
		{
			return new BannerDao().GetAllNavigate();
		}

		public static string GetLimitedTimeDiscountName(int limitedTimeDiscountId)
		{
			string result = string.Empty;
			LimitedTimeDiscountInfo discountInfo = new LimitedTimeDiscountDao().GetDiscountInfo(limitedTimeDiscountId);
			if (discountInfo != null)
			{
				result = discountInfo.ActivityName;
			}
			return result;
		}

		public static string GetLimitedTimeDiscountNameStr(int limitedTimeDiscountId)
		{
			string text = VshopBrowser.GetLimitedTimeDiscountName(limitedTimeDiscountId);
			if (!string.IsNullOrEmpty(text))
			{
				text = "<span style='background-color: rgb(246, 187, 66); border-color: rgb(246, 187, 66); color: rgb(255, 255, 255);'>" + HttpContext.Current.Server.HtmlEncode(text) + "</span>";
			}
			return text;
		}

		public static bool IsPassAutoToDistributor(MemberInfo cuser, bool isNeedToCheckAutoToDistributor = true)
		{
			bool flag = false;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			if (masterSettings.DistributorApplicationCondition)
			{
				decimal expenditure = cuser.Expenditure;
				int finishedOrderMoney = masterSettings.FinishedOrderMoney;
				if (finishedOrderMoney > 0)
				{
					decimal num = 0m;
					System.Data.DataTable userOrderPaidWaitFinish = new OrderDao().GetUserOrderPaidWaitFinish(cuser.UserId);
					for (int i = 0; i < userOrderPaidWaitFinish.Rows.Count; i++)
					{
						OrderInfo orderInfo = new OrderDao().GetOrderInfo(userOrderPaidWaitFinish.Rows[i]["orderid"].ToString());
						if (orderInfo != null)
						{
							decimal total = orderInfo.GetTotal();
							if (total > 0m)
							{
								num += total;
							}
						}
					}
					if (cuser.Expenditure + num >= finishedOrderMoney)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (masterSettings.EnableDistributorApplicationCondition)
					{
						if (!string.IsNullOrEmpty(masterSettings.DistributorProductsDate) && !string.IsNullOrEmpty(masterSettings.DistributorProducts))
						{
							if (masterSettings.DistributorProductsDate.Contains("|"))
							{
								DateTime value = default(DateTime);
								DateTime value2 = default(DateTime);
								bool flag2 = DateTime.TryParse(masterSettings.DistributorProductsDate.Split(new char[]
								{
									'|'
								})[0].ToString(), out value);
								bool flag3 = DateTime.TryParse(masterSettings.DistributorProductsDate.Split(new char[]
								{
									'|'
								})[1].ToString(), out value2);
								if (flag2 && flag3 && DateTime.Now.CompareTo(value) >= 0 && DateTime.Now.CompareTo(value2) < 0)
								{
									if (MemberProcessor.CheckMemberIsBuyProds(cuser.UserId, masterSettings.DistributorProducts, new DateTime?(value), new DateTime?(value2)))
									{
										flag = true;
									}
								}
							}
						}
					}
				}
				if (!flag && masterSettings.RechargeMoneyToDistributor > 0m && MemberAmountProcessor.GetUserMaxAmountDetailed(cuser.UserId) >= masterSettings.RechargeMoneyToDistributor)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (isNeedToCheckAutoToDistributor && flag)
			{
				flag = masterSettings.EnableMemberAutoToDistributor;
			}
			return flag;
		}
	}
}
