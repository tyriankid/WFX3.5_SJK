using Hidistro.Core;
using System;
using System.Web.UI;

namespace Hidistro.UI.Web.Pay
{
	public class IframeAlipayCharge : System.Web.UI.Page
	{
		protected string IframeUrl = string.Empty;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = Globals.RequestQueryStr("PayId");
			if (string.IsNullOrEmpty(text))
			{
				this.Page.Response.Redirect("/");
				return;
			}
			this.IframeUrl = "/Vshop/FinishRecharge.aspx?PaymentType=1&IsAlipay=1&PayId=" + text;
		}
	}
}
