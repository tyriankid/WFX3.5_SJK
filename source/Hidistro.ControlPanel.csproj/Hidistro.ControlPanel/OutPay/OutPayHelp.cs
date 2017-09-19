using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.OutPay;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Hidistro.ControlPanel.OutPay
{
	public class OutPayHelp
	{
		public static char[] Chars;

		private static string WeiXinAppid;

		private static string WeiXinMchid;

		private static string WeiXinKey;

		public static string BatchWeixinPayCheckRealName;

		private static string WeiXinCertPath;

		private static string WeixinCertPassword;

		private static bool IsReadSeting;

		private static string WeiPayUrl;

		private static string GATEWAY_NEW;

		static OutPayHelp()
		{
			OutPayHelp.Chars = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'R', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
			OutPayHelp.WeiXinAppid = "";
			OutPayHelp.WeiXinMchid = "";
			OutPayHelp.WeiXinKey = "";
			OutPayHelp.BatchWeixinPayCheckRealName = "";
			OutPayHelp.WeiXinCertPath = "";
			OutPayHelp.WeixinCertPassword = "";
			OutPayHelp.IsReadSeting = false;
			OutPayHelp.WeiPayUrl = string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers", new object[0]);
			OutPayHelp.GATEWAY_NEW = "https://mapi.alipay.com/gateway.do?";
		}

		public OutPayHelp()
		{
		}

		public static List<WeiPayResult> BatchWeiPay(List<OutPayWeiInfo> BatchUserList)
		{
			List<WeiPayResult> weiPayResults;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			OutPayHelp.WeiXinMchid = masterSettings.WeixinPartnerID;
			OutPayHelp.WeiXinAppid = masterSettings.WeixinAppId;
			OutPayHelp.WeiXinKey = masterSettings.WeixinPartnerKey;
			OutPayHelp.BatchWeixinPayCheckRealName = masterSettings.BatchWeixinPayCheckRealName.ToString();
			OutPayHelp.WeiXinCertPath = masterSettings.WeixinCertPath;
			OutPayHelp.WeixinCertPassword = masterSettings.WeixinCertPassword;
			string batchWeixinPayCheckRealName = OutPayHelp.BatchWeixinPayCheckRealName;
			if (batchWeixinPayCheckRealName != null)
			{
				if (batchWeixinPayCheckRealName == "0")
				{
					OutPayHelp.BatchWeixinPayCheckRealName = "NO_CHECK";
				}
				else if (batchWeixinPayCheckRealName == "1")
				{
					OutPayHelp.BatchWeixinPayCheckRealName = "FORCE_CHECK";
				}
				else if (batchWeixinPayCheckRealName == "2")
				{
					OutPayHelp.BatchWeixinPayCheckRealName = "OPTION_CHECK";
				}
			}
			List<WeiPayResult> weiPayResults1 = new List<WeiPayResult>();
			WeiPayResult weiPayResult = new WeiPayResult()
			{
				return_code = "SUCCESS",
				err_code = "",
				return_msg = "微信企业付款参数配置错误"
			};
			if (OutPayHelp.WeiXinMchid == "")
			{
				weiPayResult.return_code = "FAIL";
				weiPayResult.return_msg = "商户号未配置！";
			}
			else if (OutPayHelp.WeiXinAppid == "")
			{
				weiPayResult.return_code = "FAIL";
				weiPayResult.return_msg = "公众号APPID未配置！";
			}
			else if (OutPayHelp.WeiXinKey == "")
			{
				weiPayResult.return_code = "FAIL";
				weiPayResult.return_msg = "商户密钥未配置！";
			}
			if (!(weiPayResult.return_code == "FAIL"))
			{
				foreach (OutPayWeiInfo batchUserList in BatchUserList)
				{
					WeiPayResult weiPayResult1 = OutPayHelp.WeiXinPayOut(batchUserList, OutPayHelp.WeiXinAppid, OutPayHelp.WeiXinMchid, OutPayHelp.BatchWeixinPayCheckRealName, OutPayHelp.WeiXinKey);
					weiPayResults1.Add(weiPayResult1);
					if (weiPayResult1.return_code == "SUCCESS")
					{
						if ((weiPayResult1.err_code == "NOAUTH" || weiPayResult1.err_code == "NOTENOUGH" || weiPayResult1.err_code == "CA_ERROR" || weiPayResult1.err_code == "SIGN_ERROR" ? true : weiPayResult1.err_code == "XML_ERROR"))
						{
							weiPayResults1.Add(weiPayResult1);
							break;
						}
					}
				}
				weiPayResults = weiPayResults1;
			}
			else
			{
				weiPayResult.return_code = "INITFAIL";
				weiPayResults1.Add(weiPayResult);
				weiPayResults = weiPayResults1;
			}
			return weiPayResults;
		}

		public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue, string _key, string _input_charset)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = OutPayHelp.BuildRequestPara(sParaTemp, _key, _input_charset);
			StringBuilder stringBuilder = new StringBuilder();
			string[] gATEWAYNEW = new string[] { "<form id='alipaysubmit' name='alipaysubmit' action='", OutPayHelp.GATEWAY_NEW, "_input_charset=", _input_charset, "' method='", strMethod.ToLower().Trim(), "'>" };
			stringBuilder.Append(string.Concat(gATEWAYNEW));
			foreach (KeyValuePair<string, string> str in strs)
			{
				gATEWAYNEW = new string[] { "<input type='hidden' name='", str.Key, "' value='", str.Value, "'/>" };
				stringBuilder.Append(string.Concat(gATEWAYNEW));
			}
			stringBuilder.Append(string.Concat("<input type='submit' value='", strButtonValue, "' style='display:none;'></form>"));
			stringBuilder.Append("<script>document.forms['alipaysubmit'].submit();</script>");
			return stringBuilder.ToString();
		}

		private static string BuildRequestMysign(Dictionary<string, string> sPara, string _key, string _input_charset)
		{
			string str = OutPayHelp.CreateLinkString(sPara);
			return OutPayHelp.Sign(str, _key, _input_charset);
		}

		private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp, string _key, string _input_charset)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = OutPayHelp.FilterPara(sParaTemp);
			strs.Add("sign", OutPayHelp.BuildRequestMysign(strs, _key, _input_charset));
			strs.Add("sign_type", "MD5");
			return strs;
		}

		public static string CreateLinkString(Dictionary<string, string> dicArray)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in dicArray)
			{
				stringBuilder.Append(string.Concat(keyValuePair.Key, "=", keyValuePair.Value, "&"));
			}
			int length = stringBuilder.Length;
			stringBuilder.Remove(length - 1, 1);
			return stringBuilder.ToString();
		}

		public static Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> keyValuePair in dicArrayPre)
			{
				if ((!(keyValuePair.Key.ToLower() != "sign") || !(keyValuePair.Key.ToLower() != "sign_type") || !(keyValuePair.Value != "") ? false : keyValuePair.Value != null))
				{
					strs.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return strs;
		}

		public static string GetMD5(string myString, string _input_charset = "utf-8")
		{
			MD5 mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.GetEncoding(_input_charset).GetBytes(myString);
			byte[] numArray = mD5CryptoServiceProvider.ComputeHash(bytes);
			string str = null;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				str = string.Concat(str, numArray[i].ToString("x").PadLeft(2, '0'));
			}
			return str;
		}

		public static string GetRandomString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Random random = new Random();
			string str = DateTime.Now.ToString("yyyyMMdd");
			stringBuilder.Append(str);
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append(OutPayHelp.Chars[random.Next(0, (int)OutPayHelp.Chars.Length)]);
			}
			return stringBuilder.ToString();
		}

		public static string Sign(string prestr, string key, string _input_charset)
		{
			string mD5 = OutPayHelp.GetMD5(string.Concat(prestr, key), "utf-8");
			return mD5;
		}

		public static WeiPayResult SingleWeiPay(int amount, string desc, string useropenid, string realname, string tradeno, int UserId)
		{
			WeiPayResult weiPayResult;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			OutPayHelp.WeiXinMchid = masterSettings.WeixinPartnerID;
			OutPayHelp.WeiXinAppid = masterSettings.WeixinAppId;
			OutPayHelp.WeiXinKey = masterSettings.WeixinPartnerKey;
			OutPayHelp.BatchWeixinPayCheckRealName = masterSettings.BatchWeixinPayCheckRealName.ToString();
			OutPayHelp.WeiXinCertPath = masterSettings.WeixinCertPath;
			OutPayHelp.WeixinCertPassword = masterSettings.WeixinCertPassword;
			string batchWeixinPayCheckRealName = OutPayHelp.BatchWeixinPayCheckRealName;
			if (batchWeixinPayCheckRealName != null)
			{
				if (batchWeixinPayCheckRealName == "0")
				{
					OutPayHelp.BatchWeixinPayCheckRealName = "NO_CHECK";
				}
				else if (batchWeixinPayCheckRealName == "1")
				{
					OutPayHelp.BatchWeixinPayCheckRealName = "FORCE_CHECK";
				}
				else if (batchWeixinPayCheckRealName == "2")
				{
					OutPayHelp.BatchWeixinPayCheckRealName = "OPTION_CHECK";
				}
			}
			WeiPayResult weiPayResult1 = new WeiPayResult()
			{
				return_code = "SUCCESS",
				err_code = "",
				return_msg = "微信企业付款参数配置错误"
			};
			if (OutPayHelp.WeiXinMchid == "")
			{
				weiPayResult1.return_code = "FAIL";
				weiPayResult1.return_msg = "商户号未配置！";
			}
			else if (OutPayHelp.WeiXinAppid == "")
			{
				weiPayResult1.return_code = "FAIL";
				weiPayResult1.return_msg = "公众号APPID未配置！";
			}
			else if (OutPayHelp.WeiXinKey == "")
			{
				weiPayResult1.return_code = "FAIL";
				weiPayResult1.return_msg = "商户密钥未配置！";
			}
			if (!(weiPayResult1.return_code == "FAIL"))
			{
				weiPayResult1.return_code = "FAIL";
				weiPayResult1.return_msg = "用户参数出错了！";
				OutPayWeiInfo outPayWeiInfo = new OutPayWeiInfo()
				{
					Amount = amount,
					Partner_Trade_No = tradeno,
					Openid = useropenid,
					Re_User_Name = realname,
					Desc = desc,
					UserId = UserId,
					device_info = "",
					Nonce_Str = OutPayHelp.GetRandomString(20)
				};
				weiPayResult = OutPayHelp.WeiXinPayOut(outPayWeiInfo, OutPayHelp.WeiXinAppid, OutPayHelp.WeiXinMchid, OutPayHelp.BatchWeixinPayCheckRealName, OutPayHelp.WeiXinKey);
			}
			else
			{
				weiPayResult = weiPayResult1;
			}
			return weiPayResult;
		}

		public static bool Verify(string prestr, string sign, string key, string _input_charset)
		{
			bool flag;
			flag = (!(OutPayHelp.Sign(prestr, key, _input_charset) == sign) ? false : true);
			return flag;
		}

		public static bool VerifyNotify(SortedDictionary<string, string> inputPara, string notify_id, string sign, string _key, string _input_charset, string _partner)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			string str = OutPayHelp.CreateLinkString(OutPayHelp.FilterPara(inputPara));
			bool flag = OutPayHelp.Verify(str, sign, _key, _input_charset);
			string str1 = "true";
			if ((notify_id == null ? false : notify_id != ""))
			{
				string str2 = "https://mapi.alipay.com/gateway.do?service=notify_verify&";
				HttpHelp httpHelp = new HttpHelp();
				string[] strArrays = new string[] { str2, "partner=", _partner, "&notify_id=", notify_id };
				str1 = httpHelp.DoGet(string.Concat(strArrays), null);
			}
			return ((str1 != "true" ? true : !flag) ? false : true);
		}

		public static WeiPayResult WeiXinPayOut(OutPayWeiInfo payinfos, string Mch_appid, string Mchid, string Check_Name, string _key)
		{
			WeiPayResult weiPayResult;
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>()
			{
				{ "mch_appid", Mch_appid },
				{ "mchid", Mchid },
				{ "nonce_str", payinfos.Nonce_Str },
				{ "partner_trade_no", payinfos.Partner_Trade_No },
				{ "openid", payinfos.Openid },
				{ "check_name", Check_Name },
				{ "amount", payinfos.Amount.ToString() },
				{ "desc", payinfos.Desc },
				{ "spbill_create_ip", Globals.ServerIP() },
				{ "re_user_name", payinfos.Re_User_Name },
				{ "device_info", "" }
			};
			string str = _key;
			string mD5 = "";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<xml>");
			foreach (string key in strs.Keys)
			{
				if (strs[key] != "")
				{
					string str1 = mD5;
					string[] item = new string[] { str1, "&", key, "=", strs[key] };
					mD5 = string.Concat(item);
					item = new string[] { "<", key, ">", strs[key], "</", key, ">" };
					stringBuilder.AppendLine(string.Concat(item));
				}
			}
			mD5 = mD5.Remove(0, 1);
			mD5 = string.Concat(mD5, "&key=", str);
			mD5 = OutPayHelp.GetMD5(mD5, "utf-8");
			mD5 = mD5.ToUpper();
			stringBuilder.AppendLine(string.Concat("<sign>", mD5, "</sign>"));
			stringBuilder.AppendLine("</xml>");
			HttpHelp httpHelp = new HttpHelp();
			string str2 = httpHelp.DoPost(OutPayHelp.WeiPayUrl, stringBuilder.ToString(), OutPayHelp.WeixinCertPassword, OutPayHelp.WeiXinCertPath);
			WeiPayResult innerText = new WeiPayResult()
			{
				return_code = "FAIL",
				return_msg = "访问服务器出错了！",
				err_code = "SERVERERR",
				UserId = payinfos.UserId,
				Amount = payinfos.Amount,
				partner_trade_no = payinfos.Partner_Trade_No
			};
			if (!(httpHelp.errstr != ""))
			{
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(str2);
					innerText.return_code = xmlDocument.SelectSingleNode("/xml/return_code").InnerText;
					innerText.return_msg = xmlDocument.SelectSingleNode("/xml/return_msg").InnerText;
					if (!(innerText.return_code.ToUpper() == "SUCCESS"))
					{
						innerText.err_code = "FAIL";
					}
					else
					{
						innerText.result_code = xmlDocument.SelectSingleNode("/xml/result_code").InnerText;
						if (!(innerText.result_code.ToUpper() == "SUCCESS"))
						{
							innerText.err_code = xmlDocument.SelectSingleNode("/xml/err_code").InnerText;
						}
						else
						{
							innerText.mch_appid = xmlDocument.SelectSingleNode("/xml/mch_appid").InnerText;
							innerText.mchid = xmlDocument.SelectSingleNode("/xml/mchid").InnerText;
							innerText.device_info = xmlDocument.SelectSingleNode("/xml/device_info").InnerText;
							innerText.nonce_str = xmlDocument.SelectSingleNode("/xml/nonce_str").InnerText;
							innerText.result_code = xmlDocument.SelectSingleNode("/xml/result_code").InnerText;
							innerText.partner_trade_no = xmlDocument.SelectSingleNode("/xml/partner_trade_no").InnerText;
							innerText.payment_no = xmlDocument.SelectSingleNode("/xml/payment_no").InnerText;
							innerText.payment_time = xmlDocument.SelectSingleNode("/xml/payment_time").InnerText;
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					Globals.Debuglog(str2, "_DebuglogBatchPayment.txt");
					innerText.return_code = "FAIL";
					innerText.return_msg = exception.Message.ToString();
				}
				weiPayResult = innerText;
			}
			else
			{
				innerText.return_msg = httpHelp.errstr;
				weiPayResult = innerText;
			}
			return weiPayResult;
		}
	}
}