using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Shop
{
	public class Config : AdminPage
	{
		protected Script Script4;

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.WebControls.HiddenField hidpic;

		protected System.Web.UI.WebControls.HiddenField hidpicdel;

		protected System.Web.UI.WebControls.TextBox txtSiteName;

		protected System.Web.UI.WebControls.TextBox txtShopTel;

		protected System.Web.UI.WebControls.TextBox txtShopIntroduction;

		protected System.Web.UI.WebControls.Button btnSave;

		protected Config() : base("m01", "dpp02")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			if (!base.IsPostBack)
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
				this.txtSiteName.Text = masterSettings.SiteName;
				this.txtShopIntroduction.Text = masterSettings.ShopIntroduction;
				this.hidpic.Value = masterSettings.DistributorLogoPic;
				this.txtShopTel.Text = masterSettings.ShopTel;
				if (!System.IO.File.Exists(base.Server.MapPath(masterSettings.DistributorLogoPic)))
				{
					this.hidpic.Value = "http://fpoimg.com/70x70";
				}
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			string text = this.txtSiteName.Text.Trim();
			if (text.Length < 1 || text.Length > 10)
			{
				this.ShowMsg("请填写您的店铺名称，长度在10个字符以内", false);
				return;
			}
			string shopTel = this.txtShopTel.Text.Trim();
			string text2 = this.txtShopIntroduction.Text.Trim();
			if (text2.Length > 60)
			{
				this.ShowMsg("店铺介绍的长度不能超过60个字符", false);
				return;
			}
			masterSettings.SiteName = text;
			masterSettings.ShopIntroduction = text2;
			masterSettings.ShopTel = shopTel;
			SettingsManager.Save(masterSettings);
			this.hidpicdel.Value = "";
			this.ShowMsg("保存成功!", true);
		}
	}
}
