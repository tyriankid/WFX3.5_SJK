using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Shop
{
	public class MeiQia : AdminPage
	{
		protected bool enable;

		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.WebControls.TextBox txtKey;

		protected System.Web.UI.WebControls.Button btnSave;

		protected MeiQia() : base("m01", "dpp05")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.enable = this.siteSettings.EnableSaleService;
			if (!base.IsPostBack)
			{
				this.txtKey.Text = this.siteSettings.MeiQiaEntId;
			}
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			this.siteSettings.MeiQiaEntId = this.txtKey.Text.Trim();
			SettingsManager.Save(this.siteSettings);
			this.ShowMsg("修改成功！", true);
		}
	}
}
