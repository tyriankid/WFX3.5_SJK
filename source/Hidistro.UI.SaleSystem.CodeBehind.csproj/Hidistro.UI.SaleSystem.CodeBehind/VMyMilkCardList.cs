using Hidistro.ControlPanel.Promotions;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Enums;
using Hidistro.Entities.Members;
using Hidistro.Entities.Promotions;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VMyMilkCardList : VMemberTemplatedWebControl
	{
		private System.Web.UI.HtmlControls.HtmlInputHidden txtTotal;

		private System.Web.UI.HtmlControls.HtmlInputHidden txtShowTabNum;

		private VshopTemplatedRepeater rptMilkCard;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "skin-VMyMilkCardList.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("我的奶卡");
			this.rptMilkCard = (VshopTemplatedRepeater)this.FindControl("rptMilkCard");
            MemberInfo currentMember = MemberProcessor.GetCurrentMember();
            DataTable dtCardList = VShopHelper.GetMilkCardList(currentMember.UserId); //CouponHelper.GetMemberCoupons(memberCouponsSearch, ref num);

            
			this.rptMilkCard.DataSource = dtCardList;
			this.rptMilkCard.DataBind();
		}
	}
}
