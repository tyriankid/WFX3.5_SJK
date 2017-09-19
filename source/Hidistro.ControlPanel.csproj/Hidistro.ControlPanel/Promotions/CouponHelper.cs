using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Promotions;
using Hidistro.SaleSystem.Vshop;
using Hidistro.SqlDal.Members;
using Hidistro.SqlDal.Promotions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hidistro.ControlPanel.Promotions
{
	public static class CouponHelper
	{
		public static bool AddCouponProducts(int couponId, bool IsAllProduct, IList<string> productIDs)
		{
			return (new CouponDao()).AddCouponProducts(couponId, IsAllProduct, productIDs);
		}

		public static bool AddCouponProducts(int couponId, int productID)
		{
			return (new CouponDao()).AddCouponProducts(couponId, productID);
		}

		public static bool CheckCouponsIsUsed(int MemberCouponsId)
		{
			return (new CouponDao()).CheckCouponsIsUsed(MemberCouponsId);
		}

		public static CouponActionStatus CreateCoupon(CouponInfo coupon, int count, out string lotNumber)
		{
			Globals.EntityCoding(coupon, true);
			lotNumber = "";
			return (new CouponDao()).CreateCoupon(coupon);
		}

		public static CouponActionStatus CreateCoupon(CouponInfo coupon)
		{
			Globals.EntityCoding(coupon, true);
			return (new CouponDao()).CreateCoupon(coupon);
		}

		public static bool DeleteCoupon(int couponId)
		{
			return (new CouponDao()).DeleteCoupon(couponId);
		}

		public static bool DeleteProducts(int couponId, string productIds)
		{
			return (new CouponDao()).DeleteProducts(couponId, productIds);
		}

		public static CouponInfo GetCoupon(int couponId)
		{
			return (new CouponDao()).GetCouponDetails(couponId);
		}

		public static CouponInfo GetCoupon(string couponName)
		{
			return (new CouponDao()).GetCouponDetails(couponName);
		}

		public static DbQueryResult GetCouponInfos(CouponsSearch search)
		{
			return (new CouponDao()).GetCouponInfos(search);
		}

		public static IList<CouponItemInfo> GetCouponItemInfos(string lotNumber)
		{
			return null;
		}

		public static string GetCouponProductIds(int couponId)
		{
			DataTable couponProducts = CouponHelper.GetCouponProducts(couponId);
			StringBuilder stringBuilder = new StringBuilder();
			if (couponProducts != null)
			{
				int count = couponProducts.Rows.Count;
				for (int i = 0; i < count; i++)
				{
					stringBuilder.Append(couponProducts.Rows[i]["ProductId"].ToString());
					if (i != count - 1)
					{
						stringBuilder.Append("_");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static DataTable GetCouponProducts(int couponId)
		{
			return (new CouponDao()).GetCouponProducts(couponId);
		}

		public static DbQueryResult GetCouponsList(CouponItemInfoQuery query)
		{
			return (new CouponDao()).GetCouponsList(query);
		}

		public static DataTable GetCouponsListByIds(int[] CouponId)
		{
			return (new CouponDao()).GetCouponsListByIds(CouponId);
		}

		public static string GetCouponsProductIds(int CouponId)
		{
			return (new CouponDao()).GetCouponsProductIds(CouponId);
		}

		public static string GetCouponsProductIdsByMemberCouponIDByRedPagerId(int redPagerId)
		{
			return (new CouponDao()).GetCouponsProductIdsByMemberCouponIDByRedPagerId(redPagerId);
		}

		public static DataTable GetMemberCoupons(MemberCouponsSearch search, ref int total)
		{
			return (new CouponDao()).GetMemberCoupons(search, ref total);
		}

		public static int GetMemberCouponsNumbyUserId(int UserId)
		{
			return (new CouponDao()).GetMemberCouponsNumbyUserId(UserId);
		}

		public static int GetMemeberNumBySearch(string gradeIds, string referralUserId, string beginCreateDate, string endCreateDate, int userType, string customGroup)
		{
			int currentManagerUserId = Globals.GetCurrentManagerUserId();
			int memeberNumBySearch = (new MemberDao()).GetMemeberNumBySearch(gradeIds, referralUserId, beginCreateDate, endCreateDate, userType, customGroup, currentManagerUserId);
			return memeberNumBySearch;
		}

		public static DbQueryResult GetNewCoupons(Pagination page)
		{
			return (new CouponDao()).GetNewCoupons(page);
		}

		public static DataTable GetUnFinishedCoupon(DateTime end, CouponType? type = null)
		{
			return (new CouponDao()).GetUnFinishedCoupon(end, type);
		}

		public static SendCouponResult IsCanSendCouponToMember(int couponId, int userId)
		{
			SendCouponResult sendCouponResult;
			CouponInfo couponDetails = (new CouponDao()).GetCouponDetails(couponId);
			sendCouponResult = (MemberProcessor.CheckCurrentMemberIsInRange(couponDetails.MemberGrades, couponDetails.DefualtGroup, couponDetails.CustomGroup) ? (new CouponDao()).IsCanSendCouponToMember(couponId, userId) : SendCouponResult.会员不在此活动范内);
			return sendCouponResult;
		}

		public static bool SaveWeiXinPromptInfo(CouponInfo_MemberWeiXin info)
		{
			return (new CouponDao()).SaveWeiXinPromptInfo(info);
		}

		public static bool SelectCouponWillExpiredList(int DayLimit, ref List<CouponInfo_MemberWeiXin> SendToUserList)
		{
			return (new CouponDao()).SelectCouponWillExpiredList(DayLimit, ref SendToUserList);
		}

		public static void SendClaimCodes(int couponId, IList<CouponItemInfo> listCouponItem)
		{
			foreach (CouponItemInfo couponItemInfo in listCouponItem)
			{
				(new CouponDao()).SendClaimCodes(couponId, couponItemInfo);
			}
		}

		public static SendCouponResult SendCouponToMember(int couponId, int userId)
		{
			SendCouponResult sendCouponResult;
			CouponInfo couponDetails = (new CouponDao()).GetCouponDetails(couponId);
			sendCouponResult = (MemberProcessor.CheckCurrentMemberIsInRange(couponDetails.MemberGrades, couponDetails.DefualtGroup, couponDetails.CustomGroup) ? (new CouponDao()).SendCouponToMember(couponId, userId) : SendCouponResult.会员不在此活动范内);
			return sendCouponResult;
		}

		public static bool SendCouponToMemebers(int couponId)
		{
			int currentManagerUserId = Globals.GetCurrentManagerUserId();
			return (new CouponDao()).SendCouponToMemebers(couponId, currentManagerUserId);
		}

		public static bool SendCouponToMemebers(int couponId, string userIds)
		{
			bool memebers;
			if (string.IsNullOrWhiteSpace(userIds))
			{
				throw new ArgumentNullException("userIds不能为空");
			}
			string[] strArrays = userIds.Split(new char[] { '\u005F' });
			List<int> nums = new List<int>();
			string[] strArrays1 = strArrays;
			int num = 0;
			while (true)
			{
				if (num < (int)strArrays1.Length)
				{
					string str = strArrays1[num];
					try
					{
						nums.Add(int.Parse(str));
					}
					catch (Exception exception)
					{
						memebers = false;
						break;
					}
					num++;
				}
				else
				{
					memebers = (new CouponDao()).SendCouponToMemebers(couponId, nums);
					break;
				}
			}
			return memebers;
		}

		public static bool setCouponFinished(int couponId, bool flag)
		{
			return (new CouponDao()).setCouponFinished(couponId, flag);
		}

		public static bool SetProductsStatus(int couponId, int status, string productIds)
		{
			return (new CouponDao()).SetProductsStatus(couponId, status, productIds);
		}

		public static CouponActionStatus UpdateCoupon(CouponInfo coupon)
		{
			return CouponActionStatus.UnknowError;
		}

		public static string UpdateCoupon(int couponId, CouponEdit coupon, ref string msg)
		{
			Globals.EntityCoding(coupon, true);
			return (new CouponDao()).UpdateCoupon(couponId, coupon, ref msg);
		}
	}
}