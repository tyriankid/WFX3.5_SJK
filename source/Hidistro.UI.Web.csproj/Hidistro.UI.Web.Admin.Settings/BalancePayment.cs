using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.Admin.Settings
{
	public class BalancePayment : AdminPage
	{
		protected bool _EnableBalancePayment;

		protected bool _EnabelBalanceWithdrawal;

		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected BalancePayment() : base("m09", "szp15")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!base.IsPostBack)
			{
				string text = Globals.RequestFormStr("type");
				string a;
				if ((a = text) != null)
				{
					if (a == "EnableBalancePayment")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool enableBalancePayment = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.EnableBalancePayment = enableBalancePayment;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex)
						{
							base.Response.Write("保存失败！（" + ex.ToString() + ")");
						}
						base.Response.End();
						return;
					}
					if (a == "EnabelBalanceWithdrawal")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool enabelBalanceWithdrawal = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.EnabelBalanceWithdrawal = enabelBalanceWithdrawal;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex2)
						{
							base.Response.Write("保存失败！（" + ex2.ToString() + ")");
						}
						base.Response.End();
						return;
					}
				}
				this._EnableBalancePayment = this.siteSettings.EnableBalancePayment;
				this._EnabelBalanceWithdrawal = this.siteSettings.EnabelBalanceWithdrawal;
			}
		}
	}
}
