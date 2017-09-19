using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.SaleSystem.CodeBehind;
using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Hidistro.UI.Web
{
	public class Custom : System.Web.UI.Page
	{
		protected string cssLinkStr = string.Empty;

		public string cssSrc = "/Templates/vshop/";

		public bool showMenu;

		public string siteName = string.Empty;

		public string imgUrl = string.Empty;

		public string Desc = string.Empty;

		public SiteSettings siteSettings;

		protected string htmlTitleName = string.Empty;

		protected string WeixinfollowUrl = "";

		protected string AlinfollowUrl = "";

		protected bool EnabeHomePageBottomLink;

		protected bool EnableHomePageBottomCopyright;

		protected bool EnableGuidePageSet;

		protected bool IsAutoGuide;

		protected bool IsMustConcern;

		protected string DistributionLinkName = "";

		protected string DistributionLink = "";

		protected string CopyrightLinkName = "";

		protected string CopyrightLink = "";

		protected bool IsShowMenu;

		protected WeixinSet weixin;

		protected CustomHomePage H_Page;

		protected MeiQiaSet MeiQiaSet;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = (this.Page.RouteData.Values["custpath"] != null) ? this.Page.RouteData.Values["custpath"].ToString() : "notfound";
			this.H_Page.CustomPagePath = text;
			CustomPage customPageByPath = CustomPageHelp.GetCustomPageByPath(text);
			if (customPageByPath == null)
			{
				base.Response.Redirect("/default.aspx");
			}
			this.IsShowMenu = customPageByPath.IsShowMenu;
			string value = base.Request.QueryString["ReferralId"];
			if (!string.IsNullOrEmpty(value))
			{
				customPageByPath.PV++;
				CustomPageHelp.Update(customPageByPath);
			}
			if (!string.IsNullOrEmpty(customPageByPath.TempIndexName))
			{
				this.cssSrc = this.cssSrc + customPageByPath.TempIndexName + "/css/head.css";
				this.cssLinkStr = "<link rel=\"stylesheet\" href=\"" + this.cssSrc + "\">";
			}
			this.siteSettings = SettingsManager.GetMasterSettings(true);
			this.htmlTitleName = customPageByPath.Name;
			this.Desc = customPageByPath.Details;
			string userAgent = this.Page.Request.UserAgent;
			if (!base.IsPostBack)
			{
				HiAffiliation.LoadPage();
				string getCurrentWXOpenId = Globals.GetCurrentWXOpenId;
				int num = Globals.RequestQueryNum("go");
				if (userAgent.ToLower().Contains("micromessenger") && string.IsNullOrEmpty(getCurrentWXOpenId) && this.siteSettings.IsValidationService && num != 1)
				{
					this.Page.Response.Redirect("Follow.aspx?ReferralId=" + Globals.GetCurrentDistributorId());
					this.Page.Response.End();
				}
				if (Globals.GetCurrentMemberUserId(false) == 0 && this.siteSettings.IsAutoToLogin && userAgent.ToLower().Contains("micromessenger"))
				{
					System.Uri arg_1DF_0 = System.Web.HttpContext.Current.Request.Url;
					string urlToEncode = Globals.GetWebUrlStart() + "/default.aspx?ReferralId=" + Globals.RequestQueryNum("ReferralId").ToString();
					base.Response.Redirect("/UserLogining.aspx?returnUrl=" + Globals.UrlEncode(urlToEncode));
					base.Response.End();
				}
				this.showMenu = this.siteSettings.EnableShopMenu;
				this.BindWXInfo();
			}
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			System.IO.StringWriter stringWriter = new System.IO.StringWriter();
			System.Web.UI.HtmlTextWriter htmlTextWriter = new System.Web.UI.HtmlTextWriter(stringWriter);
			base.Render(htmlTextWriter);
			htmlTextWriter.Flush();
			htmlTextWriter.Close();
			string text = stringWriter.ToString();
			text = text.Trim();
			string webUrlStart = Globals.GetWebUrlStart();
			text = text.Replace("<img src=\"" + webUrlStart + "/Storage/master/product/", "<img class='imgLazyLoading' src=\"/Utility/pics/lazy-ico.gif\" data-original=\"" + webUrlStart + "/Storage/master/product/");
			text = text.Replace("<img src=\"/Storage/master/product/", "<img class='imgLazyLoading' src=\"/Utility/pics/lazy-ico.gif\" data-original=\"/Storage/master/product/");
			writer.Write(text);
		}

		public void BindWXInfo()
		{
			int currentDistributorId = Globals.GetCurrentDistributorId();
			if (currentDistributorId > 0)
			{
				DistributorsInfo distributorInfo = DistributorsBrower.GetDistributorInfo(currentDistributorId);
				if (distributorInfo != null)
				{
					this.siteName = distributorInfo.StoreName;
					this.imgUrl = "http://" + System.Web.HttpContext.Current.Request.Url.Host + distributorInfo.Logo;
				}
			}
			if (string.IsNullOrEmpty(this.siteName))
			{
				this.siteName = this.siteSettings.SiteName;
				this.imgUrl = "http://" + System.Web.HttpContext.Current.Request.Url.Host + this.siteSettings.DistributorLogoPic;
			}
		}
	}
}
