using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.VShop;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Weixin.MP.Api;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VStorePromotion : VshopTemplatedWebControl
	{
		private System.Web.UI.WebControls.Image Logoimage;

		private System.Web.UI.WebControls.Literal litStoreurl;

		private System.Web.UI.WebControls.Literal litStroeName;

		private System.Web.UI.WebControls.Literal litStroeDesc;

		private System.Web.UI.WebControls.Literal litLinkurl;

		private System.Web.UI.HtmlControls.HtmlImage storeCode;

		private System.Web.UI.HtmlControls.HtmlImage storeFollowCode;

		private int userId = 0;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VStorePromotion.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("店铺推广");
			int num = Globals.RequestQueryNum("ReferralId");
			if (!int.TryParse(this.Page.Request.QueryString["UserId"], out this.userId))
			{
				this.Page.Response.Redirect("/default.aspx?ReferralId=" + num);
				this.Page.Response.End();
			}
			DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(this.userId);
			if (userIdDistributors == null)
			{
				this.Page.Response.Redirect("/default.aspx?ReferralId=" + this.userId.ToString());
				this.Page.Response.End();
			}
			else
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				bool isShowDistributorSelfStoreName = masterSettings.IsShowDistributorSelfStoreName;
				num = userIdDistributors.UserId;
				this.litStroeDesc = (System.Web.UI.WebControls.Literal)this.FindControl("litStroeDesc");
				this.litLinkurl = (System.Web.UI.WebControls.Literal)this.FindControl("litLinkurl");
				this.litStoreurl = (System.Web.UI.WebControls.Literal)this.FindControl("litStoreurl");
				string text = Globals.FullPath("/Default.aspx?ReferralId=" + num);
				this.litLinkurl.Text = text;
				this.litStoreurl.Text = text;
				this.Logoimage = (System.Web.UI.WebControls.Image)this.FindControl("Logoimage");
				this.storeCode = (System.Web.UI.HtmlControls.HtmlImage)this.FindControl("storeCode");
				this.storeFollowCode = (System.Web.UI.HtmlControls.HtmlImage)this.FindControl("storeFollowCode");
				if (!string.IsNullOrEmpty(userIdDistributors.Logo))
				{
					this.Logoimage.ImageUrl = Globals.HostPath(this.Page.Request.Url) + userIdDistributors.Logo;
				}
				else
				{
					userIdDistributors.Logo = "/Utility/pics/headLogo.jpg";
				}
				this.storeCode.Src = "/Api/CreatQRCode.ashx?code=" + Globals.UrlEncode(text) + "&Logo=" + userIdDistributors.Logo;
				if (masterSettings.IsValidationService)
				{
					this.storeFollowCode.Src = "";
					ScanInfos scanInfosByUserId = ScanHelp.GetScanInfosByUserId(num, 0, "WX");
					if (scanInfosByUserId == null)
					{
						ScanHelp.CreatNewScan(num, "WX", 0);
						scanInfosByUserId = ScanHelp.GetScanInfosByUserId(num, 0, "WX");
					}
					string text2 = "";
					if (scanInfosByUserId != null && !string.IsNullOrEmpty(scanInfosByUserId.CodeUrl))
					{
						text2 = BarCodeApi.GetQRImageUrlByTicket(scanInfosByUserId.CodeUrl);
					}
					else
					{
						string token_Message = TokenApi.GetToken_Message(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret);
						if (TokenApi.CheckIsRightToken(token_Message))
						{
							string text3 = BarCodeApi.CreateTicket(token_Message, scanInfosByUserId.Sceneid, "QR_LIMIT_SCENE", "2592000");
							if (!string.IsNullOrEmpty(text3))
							{
								text2 = BarCodeApi.GetQRImageUrlByTicket(text3);
								scanInfosByUserId.CodeUrl = text3;
								scanInfosByUserId.CreateTime = System.DateTime.Now;
								scanInfosByUserId.LastActiveTime = System.DateTime.Now;
								ScanHelp.updateScanInfosCodeUrl(scanInfosByUserId);
							}
						}
					}
					if (!string.IsNullOrEmpty(text2))
					{
						this.storeFollowCode.Src = "/Api/CreatQRCode.ashx?Combin=" + Globals.UrlEncode(text2) + "&Logo=" + userIdDistributors.Logo;
					}
					else
					{
						this.storeFollowCode.Src = "";
					}
				}
				this.litStroeName = (System.Web.UI.WebControls.Literal)this.FindControl("litStroeName");
				this.litStroeName.Text = userIdDistributors.StoreName;
				this.litStroeDesc.Text = userIdDistributors.StoreDescription;
				if (!isShowDistributorSelfStoreName)
				{
					this.Logoimage.ImageUrl = masterSettings.DistributorLogoPic;
					this.litStroeName.Text = masterSettings.SiteName;
					this.litStroeDesc.Text = masterSettings.ShopIntroduction;
					this.storeCode.Src = "/Api/CreatQRCode.ashx?code=" + Globals.UrlEncode(text) + "&Logo=" + masterSettings.DistributorLogoPic;
				}
			}
		}
	}
}
