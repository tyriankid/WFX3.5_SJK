using Hidistro.Core;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true), System.Web.UI.PersistChildren(false)]
	public abstract class PaymentTemplatedWebControl : SimpleTemplatedWebControl
	{
		private readonly bool isBackRequest;

		protected PaymentNotify Notify;

		protected OrderInfo Order;

		protected string OrderId;

		protected decimal Amount;

		protected string Gateway;

		protected System.Collections.Generic.List<OrderInfo> orderlist;

		public PaymentTemplatedWebControl(bool _isBackRequest)
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
			if (this.isBackRequest)
			{
				this.Notify.ReturnUrl = Globals.FullPath("/pay/PaymentReturn_url.aspx") + "?" + this.Page.Request.Url.Query;
			}
			this.OrderId = this.Notify.GetOrderId();
			string gatewayOrderId = this.Notify.GetGatewayOrderId();
			if (string.IsNullOrEmpty(this.OrderId))
			{
				Globals.Debuglog(" OrderId:没获取到,GetewayOrderId:" + gatewayOrderId, "_DebuglogPaymentTest.txt");
				this.ResponseStatus(true, "noorderId");
			}
			else
			{
				this.orderlist = ShoppingProcessor.GetOrderMarkingOrderInfo(this.OrderId, false);
				if (this.orderlist.Count == 0)
				{
					Globals.Debuglog("更新订单失败，也许是订单已后台付款，OrderId:" + this.OrderId, "_DebugAlipayPayNotify.txt");
					this.ResponseStatus(true, "nodata");
				}
				else
				{
					int modeId = 0;
					foreach (OrderInfo current in this.orderlist)
					{
						this.Amount += current.GetTotal();
						current.GatewayOrderId = gatewayOrderId;
						modeId = current.PaymentTypeId;
					}
					PaymentModeInfo paymentMode = ShoppingProcessor.GetPaymentMode(modeId);
					if (paymentMode == null)
					{
						Globals.Debuglog("gatewaynotfound" + this.OrderId, "_DebugAlipayPayNotify.txt");
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
		}

		private void Notify_Payment(object sender, System.EventArgs e)
		{
			this.UserPayOrder();
		}

		private void Notify_NotifyVerifyFaild(object sender, System.EventArgs e)
		{
			Globals.Debuglog("验证失败", "_DebuglogAlipayFaild.txt");
			try
			{
				System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection
				{
					this.Page.Request.Form,
					this.Page.Request.QueryString
				};
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				foreach (string text in nameValueCollection)
				{
					stringBuilder.Append(text + ":" + nameValueCollection[text] + "；");
				}
				Globals.Debuglog(stringBuilder.ToString(), "_DebuglogAlipayFaild.txt");
			}
			catch (System.Exception var_4_C7)
			{
			}
			this.ResponseStatus(false, "verifyfaild");
		}

		private void Notify_Finished(object sender, FinishedEventArgs e)
		{
			if (e.IsMedTrade)
			{
				this.FinishOrder();
			}
			else
			{
				this.UserPayOrder();
			}
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
			int num = 0;
			int num2 = 0;
			foreach (OrderInfo current in this.orderlist)
			{
				num++;
				if (current.OrderStatus == OrderStatus.BuyerAlreadyPaid)
				{
					num2++;
				}
			}
			if (num2 > 0 && num == num2)
			{
				this.ResponseStatus(true, "success");
			}
			else
			{
				num2 = 0;
				num = 0;
				foreach (OrderInfo current in this.orderlist)
				{
					num++;
					if (current.CheckAction(OrderActions.BUYER_PAY) && MemberProcessor.UserPayOrder(current))
					{
						num2++;
						current.OnPayment();
					}
				}
				if (num2 > 0 && num == num2)
				{
					this.ResponseStatus(true, "success");
				}
				else
				{
					this.ResponseStatus(false, "fail");
				}
			}
		}

		private void FinishOrder()
		{
			int num = 0;
			int num2 = 0;
			foreach (OrderInfo current in this.orderlist)
			{
				num++;
				if (current.OrderStatus == OrderStatus.Finished)
				{
					num2++;
				}
			}
			if (num2 > 0 && num == num2)
			{
				this.ResponseStatus(true, "success");
			}
			else
			{
				num2 = 0;
				num = 0;
				foreach (OrderInfo current in this.orderlist)
				{
					num++;
					if (current.CheckAction(OrderActions.BUYER_CONFIRM_GOODS) && MemberProcessor.ConfirmOrderFinish(current))
					{
						num2++;
					}
				}
				if (num2 > 0 && num == num2)
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
}
