using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hishop.Weixin.MP.Api;
using System;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.WeiXin
{
	public class ShowQRCode : AdminPage
	{
		protected Script Script5;

		protected Script Script6;

		protected System.Web.UI.WebControls.Image imgQRCode;

		protected ShowQRCode() : base("m06", "wxp12")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.Page.IsPostBack && base.Request.QueryString["action"] == "show")
			{
				this.ShowQRCodeImage();
			}
		}

		protected void ShowQRCodeImage()
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			string text = base.Request.QueryString["id"];
			if (string.IsNullOrEmpty(text))
			{
				base.Response.Redirect("WifiSetList.aspx");
			}
			string wifiInfo = "wifi_" + text;
			wifiInfo = text;
			string token_Message = TokenApi.GetToken_Message(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret);
			string tICKET = BarCodeApi.CreateTicketWifi(token_Message, wifiInfo);
			string qRImageUrlByTicket = BarCodeApi.GetQRImageUrlByTicket(tICKET);
			this.imgQRCode.ImageUrl = qRImageUrlByTicket;
		}
	}
}
