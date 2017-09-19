using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.VShop;
using Hidistro.UI.Common.Controls;
using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VMemberAmountApply : VMemberTemplatedWebControl
	{
		protected decimal Surpluscommission = 0.00m;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VMemberAmountApply.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("余额提现");
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			if (!masterSettings.EnabelBalanceWithdrawal)
			{
				base.GotoResourceNotFound(ErrorType.前台404, "商家已关闭余额提现功能");
			}
			System.Web.UI.WebControls.Literal literal = (System.Web.UI.WebControls.Literal)this.FindControl("litApplyType");
			MemberInfo currentMemberInfo = this.CurrentMemberInfo;
			if (currentMemberInfo == null)
			{
				this.Page.Response.Redirect("/logout.aspx");
			}
			else
			{
				System.Web.UI.HtmlControls.HtmlInputHidden htmlInputHidden = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("MaxAmount");
				System.Web.UI.HtmlControls.HtmlInputHidden htmlInputHidden2 = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("MinAmount");
				System.Web.UI.HtmlControls.HtmlInputHidden htmlInputHidden3 = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("RealName");
				this.Surpluscommission = System.Math.Round(currentMemberInfo.AvailableAmount, 2);
				htmlInputHidden.Value = System.Math.Round(this.Surpluscommission, 2).ToString();
				htmlInputHidden3.Value = currentMemberInfo.RealName;
				decimal d = 0m;
				if (decimal.TryParse(SettingsManager.GetMasterSettings(false).MentionNowMoney, out d) && d > 0m)
				{
					htmlInputHidden2.Value = d.ToString();
				}
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				SiteSettings masterSettings2 = SettingsManager.GetMasterSettings(true);
				if (masterSettings2.DrawPayType.Contains("0"))
				{
					stringBuilder.AppendFormat("<div class=\"payway\" name=\"{0}\">{1}</div>", 0, "微信钱包").AppendLine();
				}
				if (masterSettings2.DrawPayType.Contains("1"))
				{
					stringBuilder.AppendFormat("<div class=\"payway\" name=\"{0}\">{1}</div>", 1, "支付宝").AppendLine();
				}
				if (masterSettings2.DrawPayType.Contains("2"))
				{
					stringBuilder.AppendFormat("<div class=\"payway\" name=\"{0}\">{1}</div>", 2, "线下转帐").AppendLine();
				}
				if (masterSettings2.DrawPayType.Contains("3"))
				{
					stringBuilder.AppendFormat("<div class=\"payway\" name=\"{0}\">{1}</div>", 3, "微信红包").AppendLine();
				}
				literal.Text = stringBuilder.ToString();
			}
		}
	}
}
