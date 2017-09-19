using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.VShop;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VMemberAmountRecord : VMemberTemplatedWebControl
	{
		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VMemberAmountRecord.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("提现记录");
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			if (!masterSettings.EnabelBalanceWithdrawal)
			{
				base.GotoResourceNotFound(ErrorType.前台404, "商家已关闭提现记录查看功能");
			}
			int num = Globals.RequestQueryNum("type");
			System.Web.UI.HtmlControls.HtmlInputHidden htmlInputHidden = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("hidType");
			htmlInputHidden.Value = num.ToString();
		}
	}
}
