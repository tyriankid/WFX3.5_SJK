using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true), System.Web.UI.PersistChildren(false)]
	public abstract class RePaymentTemplatedWebControl : SimpleTemplatedWebControl
	{
		private readonly bool isBackRequest;

		protected PaymentNotify Notify;

		protected MemberAmountDetailedInfo Model;

		protected string PayId;

		protected decimal Amount;

		protected string Gateway;

		public RePaymentTemplatedWebControl(bool _isBackRequest)
		{
			this.isBackRequest = _isBackRequest;
		}

		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			if (!this.isBackRequest)
			{
				if (!base.LoadHtmlThemedControl())
				{
					throw new SkinNotFoundException(this.SkinPath);
				}
				this.AttachChildControls();
			}
			this.DoValidate();
		}

		private void DoValidate()
		{
			System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection
			{
				this.Page.Request.Form,
				this.Page.Request.QueryString
			};
			if (!this.isBackRequest)
			{
				nameValueCollection.Add("IsReturn", "true");
			}
			this.Gateway = "hishop.plugins.payment.ws_wappay.wswappayrequest";
			this.Notify = PaymentNotify.CreateInstance(this.Gateway, nameValueCollection);
			Globals.Debuglog("充值支付：0-" + JsonConvert.SerializeObject(this.Notify), "_DebugAlipayPayNotify.txt");
			try
			{
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				foreach (string text in nameValueCollection)
				{
					stringBuilder.Append(text + ":" + nameValueCollection[text] + "；");
				}
				Globals.Debuglog(stringBuilder.ToString(), "_DebugAlipayPayNotify.txt");
			}
			catch (System.Exception var_3_117)
			{
			}
			if (this.isBackRequest)
			{
				this.Notify.ReturnUrl = Globals.FullPath("/pay/RePaymentReturn_url.aspx") + "?" + this.Page.Request.Url.Query;
			}
			Globals.Debuglog("充值支付：1-" + JsonConvert.SerializeObject(this.Notify), "_DebugAlipayPayNotify.txt");
			this.PayId = this.Notify.GetOrderId();
			this.Model = MemberAmountProcessor.GetAmountDetailByPayId(this.PayId);
			if (this.Model != null)
			{
				this.Amount = this.Model.TradeAmount;
				this.Model.GatewayPayId = this.Notify.GetGatewayOrderId();
				PaymentModeInfo paymentMode = MemberAmountProcessor.GetPaymentMode(this.Model.TradeWays);
				if (paymentMode == null)
				{
					this.ResponseStatus(true, "gatewaynotfound");
				}
				else
				{
					this.Notify.Finished += new System.EventHandler<FinishedEventArgs>(this.Notify_Finished);
					this.Notify.NotifyVerifyFaild += new System.EventHandler(this.Notify_NotifyVerifyFaild);
					this.Notify.Payment += new System.EventHandler(this.Notify_Payment);
					string configXml = HiCryptographer.Decrypt(paymentMode.Settings);
					this.Notify.VerifyNotify(30000, configXml);
				}
			}
		}

		private void Notify_Payment(object sender, System.EventArgs e)
		{
			this.UserPayOrder();
		}

		private void Notify_NotifyVerifyFaild(object sender, System.EventArgs e)
		{
			this.ResponseStatus(false, "verifyfaild");
		}

		private void Notify_Finished(object sender, FinishedEventArgs e)
		{
			this.UserPayOrder();
		}

		protected abstract void DisplayMessage(string status);

		private void ResponseStatus(bool success, string status)
		{
			if (this.isBackRequest)
			{
				this.Notify.WriteBack(System.Web.HttpContext.Current, success);
			}
			else
			{
				this.DisplayMessage(status);
			}
		}

		private void UserPayOrder()
		{
			if (this.Model.State == 1)
			{
				this.ResponseStatus(true, "success");
			}
			else if (this.Model.TradeType == TradeType.Recharge && MemberAmountProcessor.UserPayOrder(this.Model))
			{
				this.ResponseStatus(true, "success");
			}
			else
			{
				this.ResponseStatus(false, "fail");
			}
		}
	}
}
