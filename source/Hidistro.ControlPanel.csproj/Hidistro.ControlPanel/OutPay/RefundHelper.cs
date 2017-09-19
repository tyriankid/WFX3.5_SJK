using Hidistro.ControlPanel.OutPay.App;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hishop.Weixin.Pay;
using Hishop.Weixin.Pay.Domain;
using System;
using System.Collections.Generic;

namespace Hidistro.ControlPanel.OutPay
{
	public static class RefundHelper
	{
		public static string AlipayRefundRequest(string _notify_url, List<alipayReturnInfo> RefundList)
		{
			string str;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			if (!masterSettings.EnableAlipayRequest)
			{
				str = "支付宝支付功能未开启，无法完成支付！";
			}
			else if ((masterSettings.Alipay_Pid == "" || masterSettings.Alipay_Key == "" || masterSettings.Alipay_mid == "" ? false : !(masterSettings.Alipay_mName == "")))
			{
				string alipayPid = masterSettings.Alipay_Pid;
				string alipayKey = masterSettings.Alipay_Key;
				string alipayMid = masterSettings.Alipay_mid;
				string alipayMName = masterSettings.Alipay_mName;
				string str1 = "utf-8";
				Hidistro.ControlPanel.OutPay.App.Core.setConfig(alipayPid, "MD5", alipayKey, str1);
				string _notifyUrl = _notify_url;
				string alipayMid1 = masterSettings.Alipay_mid;
				string str2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				string str3 = RefundHelper.GenerateRefundOrderId();
				string str4 = RefundList.Count.ToString();
				string str5 = "";
				List<string> strs = new List<string>();
				foreach (alipayReturnInfo refundList in RefundList)
				{
					string[] remark = new string[] { refundList.alipaynum, "^", null, null, null };
					remark[2] = refundList.refundNum.ToString("F2");
					remark[3] = "^";
					remark[4] = refundList.Remark;
					strs.Add(string.Concat(remark));
				}
				str5 = string.Join("#", strs);
				SortedDictionary<string, string> strs1 = new SortedDictionary<string, string>()
				{
					{ "partner", alipayPid },
					{ "_input_charset", str1 },
					{ "service", "refund_fastpay_by_platform_pwd" },
					{ "notify_url", _notifyUrl },
					{ "seller_email", alipayMid1 },
					{ "refund_date", str2 },
					{ "batch_no", str3 },
					{ "batch_num", str4 },
					{ "detail_data", str5 }
				};
				str = Hidistro.ControlPanel.OutPay.App.Core.BuildRequest(strs1, "get", "确认");
			}
			else
			{
				str = "支付宝参数设置错误，请检查支付宝配置参数！";
			}
			return str;
		}

		public static string GenerateRefundOrderId()
		{
			string empty = string.Empty;
			Random random = new Random();
			for (int i = 0; i < 7; i++)
			{
				int num = random.Next();
				char chr = (char)(48 + (ushort)(num % 10));
				empty = string.Concat(empty, chr.ToString());
			}
			DateTime now = DateTime.Now;
			return string.Concat(now.ToString("yyyyMMdd"), empty);
		}

		public static string SendWxRefundRequest(string out_trade_no, decimal orderTotal, decimal RefundMoney, string RefundOrderId, out string WxRefundNum)
		{
			string str;
			if (RefundMoney == new decimal(0))
			{
				RefundMoney = orderTotal;
			}
			RefundInfo refundInfo = new RefundInfo();
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			WxRefundNum = "";
			if (masterSettings.EnableWeiXinRequest)
			{
				refundInfo.out_refund_no = RefundOrderId;
				refundInfo.out_trade_no = out_trade_no;
				refundInfo.RefundFee = new decimal?((int)(RefundMoney * new decimal(100)));
				refundInfo.TotalFee = new decimal?((int)(orderTotal * new decimal(100)));
				PayConfig payConfig = new PayConfig();
				WxRefundNum = "";
				string str1 = "";
				try
				{
					if (!masterSettings.EnableSP)
					{
						payConfig.AppId = masterSettings.WeixinAppId;
						payConfig.MchID = masterSettings.WeixinPartnerID;
						payConfig.Key = masterSettings.WeixinPartnerKey;
						payConfig.sub_appid = "";
						payConfig.sub_mch_id = "";
					}
					else
					{
						payConfig.AppId = masterSettings.Main_AppId;
						payConfig.MchID = masterSettings.Main_Mch_ID;
						payConfig.Key = masterSettings.Main_PayKey;
						payConfig.sub_appid = masterSettings.WeixinAppId;
						payConfig.sub_mch_id = masterSettings.WeixinPartnerID;
					}
					payConfig.AppSecret = masterSettings.WeixinAppSecret;
					payConfig.SSLCERT_PATH = masterSettings.WeixinCertPath;
					payConfig.SSLCERT_PASSWORD = masterSettings.WeixinCertPassword;
					if ((payConfig.AppId == "" || payConfig.MchID == "" || payConfig.AppSecret == "" ? false : !(payConfig.Key == "")))
					{
						str1 = ((payConfig.SSLCERT_PATH != "" ? true : !(payConfig.SSLCERT_PASSWORD == "")) ? Refund.SendRequest(refundInfo, payConfig, out WxRefundNum) : "微信证书以及密码不能为空！解决办法:请到微信-->微信红包-->上传微信证书和填写证书密码。");
					}
					else
					{
						str1 = "微信公众号配置参数错误，不能为空！";
					}
				}
				catch (Exception exception)
				{
					str1 = "ERROR";
				}
				if (str1.ToUpper() == "SUCCESS")
				{
					str1 = "";
				}
				str = str1;
			}
			else
			{
				str = "微信支付功能未开启";
			}
			return str;
		}
	}
}