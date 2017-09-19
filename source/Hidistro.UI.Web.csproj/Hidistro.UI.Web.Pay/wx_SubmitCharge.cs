using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hishop.Weixin.Pay;
using Hishop.Weixin.Pay.Domain;
using System;
using System.Web.UI;

namespace Hidistro.UI.Web.Pay
{
	public class wx_SubmitCharge : System.Web.UI.Page
	{
		public string pay_json = string.Empty;

		public string CheckValue = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = base.Request.QueryString.Get("PayId");
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			MemberAmountDetailedInfo amountDetailByPayId = MemberAmountProcessor.GetAmountDetailByPayId(text);
			if (amountDetailByPayId == null)
			{
				return;
			}
			decimal tradeAmount = amountDetailByPayId.TradeAmount;
			PackageInfo packageInfo = new PackageInfo();
			packageInfo.Body = text;
			packageInfo.NotifyUrl = string.Format("http://{0}/pay/wx_PayCharge.aspx", base.Request.Url.Host);
			packageInfo.OutTradeNo = text;
			packageInfo.TotalFee = (int)(tradeAmount * 100m);
			if (packageInfo.TotalFee < 1m)
			{
				packageInfo.TotalFee = 1m;
			}
			string openId = "";
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			if (currentMember != null)
			{
				openId = currentMember.OpenId;
			}
			packageInfo.OpenId = openId;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			PayClient payClient;
			if (masterSettings.EnableSP)
			{
				payClient = new PayClient(masterSettings.Main_AppId, masterSettings.WeixinAppSecret, masterSettings.Main_Mch_ID, masterSettings.Main_PayKey, true, masterSettings.WeixinAppId, masterSettings.WeixinPartnerID);
			}
			else
			{
				payClient = new PayClient(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret, masterSettings.WeixinPartnerID, masterSettings.WeixinPartnerKey, false, "", "");
			}
			if (!payClient.checkSetParams(out this.CheckValue))
			{
				return;
			}
			if (!payClient.checkPackage(packageInfo, out this.CheckValue))
			{
				return;
			}
			PayRequestInfo payRequestInfo = payClient.BuildPayRequest(packageInfo);
			this.pay_json = this.ConvertPayJson(payRequestInfo);
			if (!payRequestInfo.package.ToLower().StartsWith("prepay_id=wx"))
			{
				this.CheckValue = payRequestInfo.package;
			}
		}

		public string ConvertPayJson(PayRequestInfo req)
		{
			string str = "{";
			str = str + "\"appId\":\"" + req.appId + "\",";
			str = str + "\"timeStamp\":\"" + req.timeStamp + "\",";
			str = str + "\"nonceStr\":\"" + req.nonceStr + "\",";
			str = str + "\"package\":\"" + req.package + "\",";
			str = str + "\"signType\":\"" + req.signType + "\",";
			str = str + "\"paySign\":\"" + req.paySign + "\"";
			return str + "}";
		}
	}
}
