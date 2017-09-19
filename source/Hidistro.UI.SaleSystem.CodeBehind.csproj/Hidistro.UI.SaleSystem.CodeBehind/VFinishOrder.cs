using Hidistro.ControlPanel.Commodities;
using Hidistro.ControlPanel.Sales;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Plugins;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VFinishOrder : VMemberTemplatedWebControl
	{
		private string orderId;

		private System.Web.UI.WebControls.Literal litOrderId;

		private System.Web.UI.WebControls.Literal litOrderTotal;

		private System.Web.UI.WebControls.Literal literalOrderTotal;

		private System.Web.UI.WebControls.Literal literalBalancePayInfo;

		private System.Web.UI.WebControls.Literal litOPertorList;

		private System.Web.UI.WebControls.Literal litMessage;

		private System.Web.UI.HtmlControls.HtmlInputHidden litPaymentType;

		private System.Web.UI.WebControls.Literal litHelperText;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VFinishOrder.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.orderId = this.Page.Request.QueryString["orderId"];
			System.Collections.Generic.List<OrderInfo> orderMarkingOrderInfo = ShoppingProcessor.GetOrderMarkingOrderInfo(this.orderId, false);
			decimal d = 0m;
			decimal num = 0m;
			if (orderMarkingOrderInfo.Count == 0)
			{
				this.Page.Response.Redirect("/Vshop/MemberOrders.aspx?status=0");
			}
			bool flag = true;
			foreach (OrderInfo current in orderMarkingOrderInfo)
			{
                //牛奶配送计算总价
                d += current.GetMilkTotal();
				//d += current.GetTotal();
				num += current.GetBalancePayMoneyTotal();
				foreach (LineItemInfo current2 in current.LineItems.Values)
				{
					if (current2.Type == 0)
					{
						flag = false;
					}
					foreach (LineItemInfo current3 in current.LineItems.Values)
					{
						if (!ProductHelper.GetProductHasSku(current3.SkuId, current3.Quantity))
						{
							current.OrderStatus = OrderStatus.Closed;
							current.CloseReason = "库存不足";
							OrderHelper.UpdateOrder(current);
							System.Web.HttpContext.Current.Response.Write("<script>alert('库存不足，订单自动关闭！');location.href='/Vshop/MemberOrders.aspx'</script>");
							System.Web.HttpContext.Current.Response.End();
							return;
						}
					}
				}
			}
			string text = this.Page.Request.Url.ToString().ToLower();
			int num2 = Globals.RequestQueryNum("IsAlipay");
			string userAgent = this.Page.Request.UserAgent;
			if (num2 != 1 && userAgent.ToLower().Contains("micromessenger") && !string.IsNullOrEmpty(orderMarkingOrderInfo[0].Gateway) && orderMarkingOrderInfo[0].Gateway == "hishop.plugins.payment.ws_wappay.wswappayrequest")
			{
				this.Page.Response.Redirect("/Pay/IframeAlipay.aspx?OrderId=" + this.orderId);
			}
			else
			{
				if (!string.IsNullOrEmpty(orderMarkingOrderInfo[0].Gateway) && orderMarkingOrderInfo[0].Gateway == "hishop.plugins.payment.offlinerequest")
				{
					this.litMessage = (System.Web.UI.WebControls.Literal)this.FindControl("litMessage");
					this.litMessage.SetWhenIsNotNull(SettingsManager.GetMasterSettings(false).OffLinePayContent);
				}
				this.litOPertorList = (System.Web.UI.WebControls.Literal)this.FindControl("litOPertorList");
				this.litOPertorList.Text = "<div class=\"btns mt20\"><a id=\"linkToDetail\" class=\"btn btn-default mr10\" role=\"button\">查看订单</a><a href=\"/Default.aspx\" class=\"btn btn-default\" role=\"button\">继续逛逛</a></div>";
				if (!string.IsNullOrEmpty(orderMarkingOrderInfo[0].Gateway) && orderMarkingOrderInfo[0].Gateway == "hishop.plugins.payment.weixinrequest")
				{
					string text2 = "立即支付";
					if (num > 0m && d - num > 0m)
					{
						text2 = "还需支付 " + (d - num).ToString("F2");
					}
					this.litOPertorList.Text = string.Concat(new string[]
					{
						"<div class=\"mt20\"><a href=\"/pay/wx_Submit.aspx?orderId=",
						this.orderId,
						"\" class=\"btn btn-danger\" role=\"button\" id=\"btnToPay\">",
						text2,
						"</a></div>"
					});
				}
				if (!string.IsNullOrEmpty(orderMarkingOrderInfo[0].Gateway) && orderMarkingOrderInfo[0].Gateway != "hishop.plugins.payment.podrequest" && orderMarkingOrderInfo[0].Gateway != "hishop.plugins.payment.offlinerequest" && orderMarkingOrderInfo[0].Gateway != "hishop.plugins.payment.weixinrequest" && orderMarkingOrderInfo[0].Gateway != "hishop.plugins.payment.balancepayrequest" && orderMarkingOrderInfo[0].Gateway != "hishop.plugins.payment.pointtocach" && orderMarkingOrderInfo[0].Gateway != "hishop.plugins.payment.coupontocach")
				{
					PaymentModeInfo paymentMode = ShoppingProcessor.GetPaymentMode(orderMarkingOrderInfo[0].PaymentTypeId);
					string attach = "";
					string showUrl = string.Format("http://{0}/vshop/", System.Web.HttpContext.Current.Request.Url.Host);
					PaymentRequest paymentRequest = PaymentRequest.CreateInstance(paymentMode.Gateway, HiCryptographer.Decrypt(paymentMode.Settings), this.orderId, d - num, "订单支付", "订单号-" + this.orderId, orderMarkingOrderInfo[0].EmailAddress, orderMarkingOrderInfo[0].OrderDate, showUrl, Globals.FullPath("/pay/PaymentReturn_url.aspx"), Globals.FullPath("/pay/PaymentNotify_url.aspx"), attach);
					paymentRequest.SendRequest();
				}
				else
				{
					this.litOrderId = (System.Web.UI.WebControls.Literal)this.FindControl("litOrderId");
					this.litOrderTotal = (System.Web.UI.WebControls.Literal)this.FindControl("litOrderTotal");
					this.literalOrderTotal = (System.Web.UI.WebControls.Literal)this.FindControl("literalOrderTotal");
					this.literalBalancePayInfo = (System.Web.UI.WebControls.Literal)this.FindControl("literalBalancePayInfo");
					this.litPaymentType = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litPaymentType");
					int num3 = 0;
					this.litPaymentType.SetWhenIsNotNull("0");
					if (int.TryParse(this.Page.Request.QueryString["PaymentType"], out num3))
					{
						this.litPaymentType.SetWhenIsNotNull(num3.ToString());
					}
					this.litOrderId.SetWhenIsNotNull(this.orderId);
					if (flag)
					{
						this.litOrderTotal.SetWhenIsNotNull("您需要支付：¥" + d.ToString("F2"));
					}
					this.literalOrderTotal.SetWhenIsNotNull("订单金额：<span style='color:red'>¥" + d.ToString("F2") + "</span>");
					if (num > 0m)
					{
						this.literalBalancePayInfo.Text = "<div class='font-xl'>余额已支付：<span style='color:red'>¥" + num.ToString("F2") + "</span></div>";
					}
					this.litHelperText = (System.Web.UI.WebControls.Literal)this.FindControl("litHelperText");
					SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
					this.litHelperText.SetWhenIsNotNull(masterSettings.OffLinePayContent);
					PageTitle.AddSiteNameTitle("下单成功");
				}
			}
		}
	}
}
