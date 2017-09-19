using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hishop.Weixin.Pay;
using Hishop.Weixin.Pay.Notify;
using System;
using System.Web.UI;

namespace Hidistro.UI.Web.Pay
{
	public class wx_PayCharge : System.Web.UI.Page
	{
		protected string PayId;

		protected MemberAmountDetailedInfo model;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			NotifyClient notifyClient;
			if (masterSettings.EnableSP)
			{
				notifyClient = new NotifyClient(masterSettings.Main_AppId, masterSettings.WeixinAppSecret, masterSettings.Main_Mch_ID, masterSettings.Main_PayKey, true, masterSettings.WeixinAppId, masterSettings.WeixinPartnerID);
			}
			else
			{
				notifyClient = new NotifyClient(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret, masterSettings.WeixinPartnerID, masterSettings.WeixinPartnerKey, false, "", "");
			}
			PayNotify payNotify = notifyClient.GetPayNotify(base.Request.InputStream);
			if (payNotify == null)
			{
				return;
			}
			this.PayId = payNotify.PayInfo.OutTradeNo;
			this.model = MemberAmountProcessor.GetAmountDetailByPayId(this.PayId);
			if (this.model == null)
			{
				base.Response.Write("success");
				return;
			}
			this.model.GatewayPayId = payNotify.PayInfo.TransactionId;
			this.UserPayOrder();
		}

		private void UserPayOrder()
		{
			if (this.model.State == 1)
			{
				base.Response.Write("success");
				return;
			}
			if (this.model.TradeType == TradeType.Recharge && MemberAmountProcessor.UserPayOrder(this.model))
			{
				base.Response.Write("success");
			}
		}
	}
}
