using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.VShop;
using Hishop.Weixin.MP.Api;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.API.qrcode
{
	public class Default : System.Web.UI.Page
	{
		protected string ReferralId = "0";

		private string webStart = Globals.GetWebUrlStart();

		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (base.Request.UrlReferrer == null)
			{
				return;
			}
			string text = base.Request.UrlReferrer.ToString();
			if (!text.StartsWith(this.webStart))
			{
				return;
			}
			string imgUrl = this.GetImgUrl();
			if (string.IsNullOrEmpty(imgUrl))
			{
				return;
			}
			System.Drawing.Image bitMapImage = this.GetBitMapImage(imgUrl);
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
			bitMapImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
			base.Response.ClearContent();
			base.Response.ContentType = "image/png";
			base.Response.BinaryWrite(memoryStream.ToArray());
			memoryStream.Dispose();
			bitMapImage.Dispose();
			base.Response.End();
		}

		private System.Drawing.Bitmap GetBitMapImage(string path)
		{
			if (path.StartsWith("http"))
			{
				System.Net.WebRequest webRequest = System.Net.WebRequest.Create(path);
				webRequest.Timeout = 10000;
				System.Net.HttpWebResponse httpWebResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
				System.IO.Stream responseStream = httpWebResponse.GetResponseStream();
				System.Drawing.Image original = System.Drawing.Image.FromStream(responseStream);
				return new System.Drawing.Bitmap(original);
			}
			return new System.Drawing.Bitmap(base.Server.MapPath(path));
		}

		protected string GetImgUrl()
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			string pattern = "ReferralId=(?<url>d+)";
			string input = base.Request.UrlReferrer.ToString();
			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(input, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			if (match.Success)
			{
				this.ReferralId = match.Value;
			}
			int num = Globals.ToNum(this.ReferralId);
			string result = this.webStart + "/Follow.aspx?ReferralId=" + this.ReferralId;
			ScanInfos scanInfosByUserId = ScanHelp.GetScanInfosByUserId(num, 0, "WX");
			if (scanInfosByUserId == null)
			{
				ScanHelp.CreatNewScan(num, "WX", 0);
				scanInfosByUserId = ScanHelp.GetScanInfosByUserId(num, 0, "WX");
			}
			if (scanInfosByUserId != null && !string.IsNullOrEmpty(scanInfosByUserId.CodeUrl))
			{
				result = BarCodeApi.GetQRImageUrlByTicket(scanInfosByUserId.CodeUrl);
			}
			else
			{
				if (string.IsNullOrEmpty(masterSettings.WeixinAppId) || string.IsNullOrEmpty(masterSettings.WeixinAppSecret))
				{
					return "";
				}
				string token_Message = TokenApi.GetToken_Message(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret);
				if (TokenApi.CheckIsRightToken(token_Message))
				{
					string text = BarCodeApi.CreateTicket(token_Message, scanInfosByUserId.Sceneid, "QR_LIMIT_SCENE", "2592000");
					if (!string.IsNullOrEmpty(text))
					{
						result = BarCodeApi.GetQRImageUrlByTicket(text);
						scanInfosByUserId.CodeUrl = text;
						scanInfosByUserId.CreateTime = System.DateTime.Now;
						scanInfosByUserId.LastActiveTime = System.DateTime.Now;
						ScanHelp.updateScanInfosCodeUrl(scanInfosByUserId);
					}
				}
			}
			return result;
		}
	}
}
