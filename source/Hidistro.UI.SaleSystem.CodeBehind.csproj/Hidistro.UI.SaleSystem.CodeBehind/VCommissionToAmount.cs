using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VCommissionToAmount : VMemberTemplatedWebControl
	{
		protected decimal surpluscommission = 0.00m;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-CommissionToAmount.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("佣金转余额");
			DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(Globals.GetCurrentMemberUserId(false));
			if (userIdDistributors != null && userIdDistributors.UserId > 0)
			{
				this.surpluscommission = userIdDistributors.ReferralBlance;
				decimal d = DistributorsBrower.CommionsRequestSumMoney(userIdDistributors.UserId);
				this.surpluscommission -= d;
			}
			System.Web.UI.HtmlControls.HtmlInputHidden htmlInputHidden = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("MaxCommission");
			htmlInputHidden.Value = System.Math.Round(this.surpluscommission - 0.005m, 2).ToString();
		}
	}
}
