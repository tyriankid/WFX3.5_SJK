using Hidistro.Core;
using System;
using System.Web.UI;

namespace Hidistro.UI.Web.Pay
{
	public class IframeAlipay : System.Web.UI.Page
	{
		protected string IframeUrl = string.Empty;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = Globals.RequestQueryStr("OrderId");
			if (string.IsNullOrEmpty(text))
			{
				this.Page.Response.Redirect("/");
				return;
			}
			this.IframeUrl = "/Vshop/FinishOrder.aspx?PaymentType=1&IsAlipay=1&OrderId=" + text;
		}
	}
}
