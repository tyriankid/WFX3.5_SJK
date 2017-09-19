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
	public class DraftCustom : System.Web.UI.Page
	{
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

		protected bool DraftIsShowMenu;

		protected WeixinSet weixin;

		protected CustomDraftHomePage H_Page;

		protected MeiQiaSet MeiQiaSet;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = (this.Page.RouteData.Values["custpath"] != null) ? this.Page.RouteData.Values["custpath"].ToString() : "notfound";
			this.H_Page.CustomPagePath = text;
			CustomPage customDraftPageByPath = CustomPageHelp.GetCustomDraftPageByPath(text);
			if (customDraftPageByPath == null)
			{
				base.Response.Redirect("/default.aspx");
			}
			this.DraftIsShowMenu = customDraftPageByPath.DraftIsShowMenu;
			string arg_8A_0 = base.Request.QueryString["ReferralId"];
			this.cssSrc = this.cssSrc + customDraftPageByPath.TempIndexName + "/css/head.css";
			this.siteSettings = SettingsManager.GetMasterSettings(true);
			this.htmlTitleName = customDraftPageByPath.DraftName;
			this.Desc = customDraftPageByPath.DraftDetails;
			string userAgent = this.Page.Request.UserAgent;
			this.EnabeHomePageBottomLink = this.siteSettings.EnabeHomePageBottomLink;
			this.EnableHomePageBottomCopyright = this.siteSettings.EnableHomePageBottomCopyright;
			this.EnableGuidePageSet = this.siteSettings.EnableGuidePageSet;
			this.IsAutoGuide = this.siteSettings.IsAutoGuide;
			this.IsMustConcern = this.siteSettings.IsMustConcern;
			this.DistributionLinkName = this.siteSettings.DistributionLinkName;
			this.DistributionLink = (string.IsNullOrEmpty(this.siteSettings.DistributionLink) ? "javascript:void(0);" : this.siteSettings.DistributionLink);
			this.CopyrightLinkName = this.siteSettings.CopyrightLinkName;
			this.CopyrightLink = (string.IsNullOrEmpty(this.siteSettings.CopyrightLink) ? "javascript:void(0);" : this.siteSettings.CopyrightLink);
			if (this.siteSettings.EnableAliPayFuwuGuidePageSet)
			{
				this.AlinfollowUrl = this.siteSettings.AliPayFuwuGuidePageSet;
			}
			if (this.siteSettings.EnableGuidePageSet)
			{
				this.WeixinfollowUrl = this.siteSettings.GuidePageSet;
			}
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
					System.Uri arg_29F_0 = System.Web.HttpContext.Current.Request.Url;
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
