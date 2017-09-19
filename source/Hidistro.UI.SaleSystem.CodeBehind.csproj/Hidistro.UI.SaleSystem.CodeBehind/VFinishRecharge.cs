using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Plugins;
using System;
using System.Web;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VFinishRecharge : VMemberTemplatedWebControl
	{
		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VFinishRecharge.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			string text = this.Page.Request.QueryString["PayId"];
			MemberAmountDetailedInfo amountDetailByPayId = MemberAmountProcessor.GetAmountDetailByPayId(text);
			if (amountDetailByPayId == null)
			{
				this.Page.Response.Redirect("/Vshop/MemberRecharge.aspx");
			}
			string text2 = this.Page.Request.Url.ToString().ToLower();
			int num = Globals.RequestQueryNum("IsAlipay");
			string userAgent = this.Page.Request.UserAgent;
			if (num != 1 && userAgent.ToLower().Contains("micromessenger") && amountDetailByPayId.TradeWays == TradeWays.Alipay)
			{
				this.Page.Response.Redirect("/Pay/IframeAlipayCharge.aspx?PayId=" + text);
			}
			else if (amountDetailByPayId.TradeWays == TradeWays.WeChatWallet)
			{
				this.Page.Response.Redirect("~/pay/wx_SubmitCharge.aspx?PayId=" + text);
			}
			else if (amountDetailByPayId.TradeWays != TradeWays.WeChatWallet && amountDetailByPayId.TradeWays != TradeWays.LineTransfer)
			{
				PaymentModeInfo paymentMode = MemberAmountProcessor.GetPaymentMode(amountDetailByPayId.TradeWays);
				string attach = "";
				string showUrl = string.Format("http://{0}/vshop/", System.Web.HttpContext.Current.Request.Url.Host);
				PaymentRequest paymentRequest = PaymentRequest.CreateInstance(paymentMode.Gateway, HiCryptographer.Decrypt(paymentMode.Settings), text, amountDetailByPayId.TradeAmount, "会员充值", "充值号-" + text, "", amountDetailByPayId.TradeTime, showUrl, Globals.FullPath("/pay/RePaymentReturn_url.aspx"), Globals.FullPath("/pay/RePaymentNotify_url.aspx"), attach);
				paymentRequest.SendRequest();
			}
		}
	}
}
