using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VMemberAccountBalance : VMemberTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litAmount;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VMemberAccountBalance.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("账户余额");
			MemberInfo currentMemberInfo = this.CurrentMemberInfo;
			if (currentMemberInfo == null)
			{
				this.Page.Response.Redirect("/logout.aspx");
			}
			else
			{
				this.litAmount = (System.Web.UI.WebControls.Literal)this.FindControl("litAmount");
				this.litAmount.Text = System.Math.Round(currentMemberInfo.AvailableAmount, 2).ToString();
				System.Web.UI.HtmlControls.HtmlInputHidden htmlInputHidden = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("EnabelBalanceWithdrawal");
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
				htmlInputHidden.Value = masterSettings.EnabelBalanceWithdrawal.ToString().ToLower();
			}
		}
	}
}
