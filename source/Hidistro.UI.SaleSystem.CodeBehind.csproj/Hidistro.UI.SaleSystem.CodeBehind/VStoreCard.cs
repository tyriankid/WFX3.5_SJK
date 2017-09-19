using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.VShop;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Weixin.MP.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VStoreCard : VshopTemplatedWebControl
	{
		private System.Web.UI.HtmlControls.HtmlImage imglogo;

		private System.Web.UI.HtmlControls.HtmlInputHidden ShareInfo;

		private System.Web.UI.HtmlControls.HtmlControl editPanel;

		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(true);

		private int userId = 0;

		protected override void OnInit(System.EventArgs e)
		{
			string a = System.Web.HttpContext.Current.Request["action"];
			if (a == "ReCreadt")
			{
				System.Web.HttpContext.Current.Response.ContentType = "application/json";
				string text = System.Web.HttpContext.Current.Request["imageUrl"];
				string s = "";
				if (string.IsNullOrEmpty(text))
				{
					s = "{\"success\":\"false\",\"message\":\"图片地址为空\"}";
				}
				try
				{
					MemberInfo currentMember = MemberProcessor.GetCurrentMember();
					DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(currentMember.UserId);
					string userHeadPath = text;
					string storeLogoPath = text;
					ScanInfos scanInfosByUserId = ScanHelp.GetScanInfosByUserId(currentMember.UserId, 0, "WX");
					if (scanInfosByUserId == null)
					{
						ScanHelp.CreatNewScan(currentMember.UserId, "WX", 0);
						scanInfosByUserId = ScanHelp.GetScanInfosByUserId(currentMember.UserId, 0, "WX");
					}
					string codeUrl;
					if (scanInfosByUserId == null)
					{
						codeUrl = Globals.HostPath(System.Web.HttpContext.Current.Request.Url) + "/Follow.aspx?ReferralId=" + currentMember.UserId.ToString();
					}
					else
					{
						codeUrl = BarCodeApi.GetQRImageUrlByTicket(scanInfosByUserId.CodeUrl);
					}
					string setJson = System.IO.File.ReadAllText(System.Web.HttpRuntime.AppDomainAppPath.ToString() + "Storage/Utility/StoreCardSet.js");
					string storeName = userIdDistributors.StoreName;
					if (!this.siteSettings.IsShowDistributorSelfStoreName)
					{
						storeName = this.siteSettings.SiteName;
					}
					StoreCardCreater storeCardCreater = new StoreCardCreater(setJson, userHeadPath, storeLogoPath, codeUrl, currentMember.UserName, storeName, currentMember.UserId, currentMember.UserId);
					string text2 = "";
					if (storeCardCreater.ReadJson() && storeCardCreater.CreadCard(out text2))
					{
						s = "{\"success\":\"true\",\"message\":\"生成成功\"}";
						DistributorsBrower.UpdateStoreCard(currentMember.UserId, text2);
					}
					else
					{
						s = "{\"success\":\"false\",\"message\":\"" + text2 + "\"}";
					}
				}
				catch (System.Exception ex)
				{
					s = "{\"success\":\"false\",\"message\":\"" + ex.Message + "\"}";
				}
				System.Web.HttpContext.Current.Response.Write(s);
				System.Web.HttpContext.Current.Response.End();
			}
			if (this.SkinName == null)
			{
				this.SkinName = "skin-VStoreCard.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			if (!int.TryParse(this.Page.Request.QueryString["ReferralId"], out this.userId))
			{
				this.Context.Response.Redirect("/");
			}
			else
			{
				DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(this.userId);
				if (userIdDistributors == null)
				{
					this.Context.Response.Redirect("/");
				}
				else
				{
					this.imglogo = (System.Web.UI.HtmlControls.HtmlImage)this.FindControl("QrcodeImg");
					int currentMemberUserId = Globals.GetCurrentMemberUserId(false);
					this.editPanel = (System.Web.UI.HtmlControls.HtmlControl)this.FindControl("editPanel");
					this.editPanel.Visible = false;
					if (currentMemberUserId == this.userId)
					{
						this.imglogo.Attributes.Add("Admin", "true");
						MemberInfo currentMember = MemberProcessor.GetCurrentMember();
						System.DateTime cardCreatTime = userIdDistributors.CardCreatTime;
						string text = System.IO.File.ReadAllText(System.Web.HttpRuntime.AppDomainAppPath.ToString() + "Storage/Utility/StoreCardSet.js");
						JObject jObject = JsonConvert.DeserializeObject(text) as JObject;
						System.DateTime t = default(System.DateTime);
						if (jObject != null && jObject["writeDate"] != null)
						{
							t = System.DateTime.Parse(jObject["writeDate"].ToString());
						}
						ScanInfos scanInfosByUserId = ScanHelp.GetScanInfosByUserId(currentMember.UserId, 0, "WX");
						if (scanInfosByUserId == null)
						{
							ScanHelp.CreatNewScan(currentMember.UserId, "WX", 0);
							scanInfosByUserId = ScanHelp.GetScanInfosByUserId(currentMember.UserId, 0, "WX");
						}
						string text2;
						if (scanInfosByUserId == null)
						{
							text2 = Globals.HostPath(System.Web.HttpContext.Current.Request.Url) + "/Follow.aspx?ReferralId=" + currentMember.UserId.ToString();
						}
						else
						{
							text2 = scanInfosByUserId.CodeUrl;
							if (string.IsNullOrEmpty(text2))
							{
								string token_Message = TokenApi.GetToken_Message(this.siteSettings.WeixinAppId, this.siteSettings.WeixinAppSecret);
								if (TokenApi.CheckIsRightToken(token_Message))
								{
									string text3 = BarCodeApi.CreateTicket(token_Message, scanInfosByUserId.Sceneid, "QR_LIMIT_SCENE", "2592000");
									if (!string.IsNullOrEmpty(text3))
									{
										text2 = text3;
										scanInfosByUserId.CodeUrl = text3;
										scanInfosByUserId.CreateTime = System.DateTime.Now;
										scanInfosByUserId.LastActiveTime = System.DateTime.Now;
										ScanHelp.updateScanInfosCodeUrl(scanInfosByUserId);
									}
								}
							}
							if (string.IsNullOrEmpty(text2))
							{
								text2 = Globals.HostPath(System.Web.HttpContext.Current.Request.Url) + "/Follow.aspx?ReferralId=" + currentMember.UserId.ToString();
							}
							else
							{
								text2 = BarCodeApi.GetQRImageUrlByTicket(text2);
							}
						}
						if (string.IsNullOrEmpty(userIdDistributors.StoreCard) || cardCreatTime < t)
						{
							string storeName = userIdDistributors.StoreName;
							if (!this.siteSettings.IsShowDistributorSelfStoreName)
							{
								storeName = this.siteSettings.SiteName;
							}
							StoreCardCreater storeCardCreater = new StoreCardCreater(text, currentMember.UserHead, userIdDistributors.Logo, text2, currentMember.UserName, storeName, this.userId, this.userId);
							string imgUrl = "";
							if (storeCardCreater.ReadJson() && storeCardCreater.CreadCard(out imgUrl))
							{
								DistributorsBrower.UpdateStoreCard(this.userId, imgUrl);
							}
						}
					}
					if (string.IsNullOrEmpty(userIdDistributors.StoreCard))
					{
						userIdDistributors.StoreCard = "/Storage/master/DistributorCards/StoreCard" + this.userId.ToString() + ".jpg";
					}
					this.ShareInfo = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("ShareInfo");
					this.imglogo.Src = userIdDistributors.StoreCard;
					PageTitle.AddSiteNameTitle("掌柜名片");
				}
			}
		}
	}
}
