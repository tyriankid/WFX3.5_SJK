using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VMemberRecharge : VMemberTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litPaymentType;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VMemberRecharge.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("余额充值");
			this.litPaymentType = (System.Web.UI.WebControls.Literal)this.FindControl("litPaymentType");
			System.Collections.Generic.IList<PaymentModeInfo> paymentModes = ShoppingProcessor.GetPaymentModes();
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string userAgent = this.Page.Request.UserAgent;
			if (masterSettings.EnableWeiXinRequest)
			{
				if (userAgent.ToLower().Contains("micromessenger") && masterSettings.IsValidationService)
				{
					stringBuilder.AppendLine("<div class=\"payway\" name=\"88\">微信支付</div>");
				}
			}
			if (paymentModes != null && paymentModes.Count > 0)
			{
				foreach (PaymentModeInfo current in paymentModes)
				{
					string xml = HiCryptographer.Decrypt(current.Settings);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(xml);
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Partner");
					if (elementsByTagName.Count != 0)
					{
						if (masterSettings.EnableAlipayRequest)
						{
							if (!string.IsNullOrEmpty(xmlDocument.GetElementsByTagName("Partner")[0].InnerText) && !string.IsNullOrEmpty(xmlDocument.GetElementsByTagName("Key")[0].InnerText) && !string.IsNullOrEmpty(xmlDocument.GetElementsByTagName("Seller_account_name")[0].InnerText))
							{
								stringBuilder.AppendFormat("<div class=\"payway\" name=\"{0}\">{1}</div>", current.ModeId, current.Name).AppendLine();
							}
						}
					}
					else if (masterSettings.EnableWapShengPay)
					{
						if (!string.IsNullOrEmpty(xmlDocument.GetElementsByTagName("SenderId")[0].InnerText) && !string.IsNullOrEmpty(xmlDocument.GetElementsByTagName("SellerKey")[0].InnerText))
						{
							stringBuilder.AppendFormat("<div class=\"payway\" name=\"{0}\">{1}</div>", current.ModeId, current.Name).AppendLine();
						}
					}
				}
			}
			this.litPaymentType.Text = stringBuilder.ToString();
		}
	}
}
