using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Promotions;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.Entities.Promotions;
using Hidistro.SaleSystem.Vshop;
using System;
using System.Web;

namespace Hidistro.UI.Web.API
{
	public class LimitedTimeDiscountHandler : System.Web.IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			string text = Globals.RequestFormStr("action");
			string a;
			if ((a = text.ToLower()) != null)
			{
				if (!(a == "isdiscountproduct"))
				{
					return;
				}
				this.IsDiscountProduct(context);
			}
		}

		private void IsDiscountProduct(System.Web.HttpContext context)
		{
			int num = Globals.RequestFormNum("productId");
			if (num > 0)
			{
				LimitedTimeDiscountProductInfo discountProductInfoByProductId = LimitedTimeDiscountHelper.GetDiscountProductInfoByProductId(num);
				if (discountProductInfoByProductId != null)
				{
					LimitedTimeDiscountInfo discountInfo = LimitedTimeDiscountHelper.GetDiscountInfo(discountProductInfoByProductId.LimitedTimeDiscountId);
					MemberInfo currentMember = MemberProcessor.GetCurrentMember();
					int num2 = 0;
					int num3 = discountInfo.LimitNumber;
					if (discountInfo != null)
					{
						if (currentMember != null)
						{
							if (MemberHelper.CheckCurrentMemberIsInRange(discountInfo.ApplyMembers, discountInfo.DefualtGroup, discountInfo.CustomGroup, currentMember.UserId))
							{
								int limitedTimeDiscountUsedNum = ShoppingCartProcessor.GetLimitedTimeDiscountUsedNum(discountProductInfoByProductId.LimitedTimeDiscountId, null, num, currentMember.UserId, false);
								if (discountInfo.LimitNumber == 0)
								{
									num2 = discountInfo.LimitedTimeDiscountId;
								}
								else if (discountInfo.LimitNumber - limitedTimeDiscountUsedNum > 0)
								{
									num2 = discountInfo.LimitedTimeDiscountId;
									num3 = discountInfo.LimitNumber - limitedTimeDiscountUsedNum;
								}
								else
								{
									num3 = 0;
								}
							}
						}
						else
						{
							num2 = discountInfo.LimitedTimeDiscountId;
						}
					}
					if (discountInfo != null)
					{
						context.Response.Write(string.Concat(new object[]
						{
							"{\"msg\":\"success\",\"ActivityName\":\"",
							Globals.String2Json(discountInfo.ActivityName),
							"\",\"FinalPrice\":\"",
							discountProductInfoByProductId.FinalPrice.ToString("f2"),
							"\",\"LimitedTimeDiscountId\":\"",
							num2,
							"\",\"LimitNumber\":\"",
							discountInfo.LimitNumber,
							"\",\"RemainNumber\":\"",
							num3,
							"\"}"
						}));
					}
				}
			}
		}
	}
}
