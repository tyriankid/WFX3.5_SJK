using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Shop
{
	public class Setting : AdminPage
	{
		protected bool _EnabeHomePageBottomLink;

		protected bool _EnableHomePageBottomCopyright;

		protected bool _IsHomeShowFloatMenu;

		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		protected Script Script4;

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.HtmlControls.HtmlInputText fenxiaoName;

		protected System.Web.UI.HtmlControls.HtmlInputText fenxiaoAddress;

		protected System.Web.UI.WebControls.TextBox txtName;

		protected System.Web.UI.WebControls.TextBox txtDistributionLink;

		protected System.Web.UI.WebControls.Button btnResetHomePageBottomLink;

		protected System.Web.UI.HtmlControls.HtmlInputText fenxiaoCopyright;

		protected System.Web.UI.HtmlControls.HtmlInputText fenxiaoCopyLink;

		protected System.Web.UI.WebControls.TextBox txtCopyName;

		protected System.Web.UI.WebControls.TextBox txtCopyLink;

		protected System.Web.UI.WebControls.Button Button1;

		protected Setting() : base("m01", "dpp12")
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
					if (a == "EnabeHomePageBottomLink")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool enabeHomePageBottomLink = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.EnabeHomePageBottomLink = enabeHomePageBottomLink;
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
					if (a == "EnableHomePageBottomCopyright")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool enableHomePageBottomCopyright = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.EnableHomePageBottomCopyright = enableHomePageBottomCopyright;
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
					if (a == "DistributionLink")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							string distributionLinkName = Globals.RequestFormStr("txt1");
							string distributionLink = Globals.RequestFormStr("txt2");
							this.siteSettings.DistributionLinkName = distributionLinkName;
							this.siteSettings.DistributionLink = distributionLink;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex3)
						{
							base.Response.Write("保存失败！（" + ex3.ToString() + ")");
						}
						base.Response.End();
						return;
					}
					if (a == "CopyrightLink")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							string copyrightLinkName = Globals.RequestFormStr("txt3");
							string copyrightLink = Globals.RequestFormStr("txt4");
							this.siteSettings.CopyrightLinkName = copyrightLinkName;
							this.siteSettings.CopyrightLink = copyrightLink;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex4)
						{
							base.Response.Write("保存失败！（" + ex4.ToString() + ")");
						}
						base.Response.End();
						return;
					}
					if (a == "IsHomeShowFloatMenu")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool isHomeShowFloatMenu = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.IsHomeShowFloatMenu = isHomeShowFloatMenu;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex5)
						{
							base.Response.Write("保存失败！（" + ex5.ToString() + ")");
						}
						base.Response.End();
						return;
					}
				}
				this._EnabeHomePageBottomLink = this.siteSettings.EnabeHomePageBottomLink;
				this._EnableHomePageBottomCopyright = this.siteSettings.EnableHomePageBottomCopyright;
				this.txtName.Text = this.siteSettings.DistributionLinkName;
				this.txtDistributionLink.Text = this.siteSettings.DistributionLink;
				this.txtCopyName.Text = this.siteSettings.CopyrightLinkName;
				this.txtCopyLink.Text = this.siteSettings.CopyrightLink;
				this.fenxiaoName.Value = this.siteSettings.DistributionLinkName;
				this.fenxiaoAddress.Value = this.siteSettings.DistributionLink;
				this.fenxiaoCopyright.Value = this.siteSettings.CopyrightLinkName;
				this.fenxiaoCopyLink.Value = this.siteSettings.CopyrightLink;
				if (this.siteSettings.IsHomeShowFloatMenu)
				{
					this._IsHomeShowFloatMenu = true;
				}
			}
		}

		protected void btnResetHomePageBottomLink_Click(object sender, System.EventArgs e)
		{
			this.siteSettings.DistributionLinkName = "申请分销";
			this.siteSettings.DistributionLink = "/Vshop/DistributorRegCheck.aspx";
			SettingsManager.Save(this.siteSettings);
			this._EnabeHomePageBottomLink = this.siteSettings.EnabeHomePageBottomLink;
			this._EnableHomePageBottomCopyright = this.siteSettings.EnableHomePageBottomCopyright;
			this.txtName.Text = this.siteSettings.DistributionLinkName;
			this.txtDistributionLink.Text = this.siteSettings.DistributionLink;
			this.txtCopyName.Text = this.siteSettings.CopyrightLinkName;
			this.txtCopyLink.Text = this.siteSettings.CopyrightLink;
			this.ShowMsgAndReUrl("底部文字链接恢复默认成功！", true, "Setting.aspx");
		}

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			this.siteSettings.CopyrightLinkName = "Hishop技术支持";
			this.siteSettings.CopyrightLink = "http://www.hishop.com.cn/support/";
			SettingsManager.Save(this.siteSettings);
			this._EnabeHomePageBottomLink = this.siteSettings.EnabeHomePageBottomLink;
			this._EnableHomePageBottomCopyright = this.siteSettings.EnableHomePageBottomCopyright;
			this.txtName.Text = this.siteSettings.DistributionLinkName;
			this.txtDistributionLink.Text = this.siteSettings.DistributionLink;
			this.txtCopyName.Text = this.siteSettings.CopyrightLinkName;
			this.txtCopyLink.Text = this.siteSettings.CopyrightLink;
			this.fenxiaoName.Value = this.siteSettings.DistributionLinkName;
			this.fenxiaoAddress.Value = this.siteSettings.DistributionLink;
			this.fenxiaoCopyright.Value = this.siteSettings.CopyrightLinkName;
			this.fenxiaoCopyLink.Value = this.siteSettings.CopyrightLink;
			this.ShowMsgAndReUrl("底部版权信息恢复默认成功！", true, "Setting.aspx");
		}
	}
}
