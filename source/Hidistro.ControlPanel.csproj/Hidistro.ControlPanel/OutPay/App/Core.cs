using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Hidistro.ControlPanel.OutPay.App
{
	public class Core
	{
		public static string _sign_type;

		public static string _private_key;

		public static string _input_charset;

		public static string GATEWAY_NEW;

		public static string _partner;

		static Core()
		{
			Hidistro.ControlPanel.OutPay.App.Core._sign_type = "";
			Hidistro.ControlPanel.OutPay.App.Core._private_key = "";
			Hidistro.ControlPanel.OutPay.App.Core._input_charset = "";
			Hidistro.ControlPanel.OutPay.App.Core.GATEWAY_NEW = "https://mapi.alipay.com/gateway.do?";
			Hidistro.ControlPanel.OutPay.App.Core._partner = "";
		}

		public Core()
		{
		}

		public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Hidistro.ControlPanel.OutPay.App.Core.BuildRequestPara(sParaTemp);
			StringBuilder stringBuilder = new StringBuilder();
			string[] gATEWAYNEW = new string[] { "<form id='alipaysubmit' name='alipaysubmit' action='", Hidistro.ControlPanel.OutPay.App.Core.GATEWAY_NEW, "_input_charset=", Hidistro.ControlPanel.OutPay.App.Core._input_charset, "' method='", strMethod.ToLower().Trim(), "'>" };
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

		private static string BuildRequestMysign(Dictionary<string, string> sPara)
		{
			string str;
			string str1 = Hidistro.ControlPanel.OutPay.App.Core.CreateLinkString(sPara);
			string mD5 = "";
			string _signType = Hidistro.ControlPanel.OutPay.App.Core._sign_type;
			if (_signType != null)
			{
				if (_signType == "RSA")
				{
					mD5 = RSAFromPkcs8.sign(str1, Hidistro.ControlPanel.OutPay.App.Core._private_key, Hidistro.ControlPanel.OutPay.App.Core._input_charset);
					str = mD5;
					return str;
				}
				else
				{
					if (_signType != "MD5")
					{
						mD5 = "";
						str = mD5;
						return str;
					}
					mD5 = Hidistro.ControlPanel.OutPay.App.Core.GetMD5(string.Concat(str1, Hidistro.ControlPanel.OutPay.App.Core._private_key), Hidistro.ControlPanel.OutPay.App.Core._input_charset);
					str = mD5;
					return str;
				}
			}
			mD5 = "";
			str = mD5;
			return str;
		}

		private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Hidistro.ControlPanel.OutPay.App.Core.FilterPara(sParaTemp);
			strs.Add("sign", Hidistro.ControlPanel.OutPay.App.Core.BuildRequestMysign(strs));
			strs.Add("sign_type", Hidistro.ControlPanel.OutPay.App.Core._sign_type);
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

		public static string CreateLinkStringUrlencode(Dictionary<string, string> dicArray, Encoding code)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in dicArray)
			{
				stringBuilder.Append(string.Concat(keyValuePair.Key, "=", HttpUtility.UrlEncode(keyValuePair.Value, code), "&"));
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

		public static string GetAbstractToMD5(Stream sFile)
		{
			byte[] numArray = (new MD5CryptoServiceProvider()).ComputeHash(sFile);
			StringBuilder stringBuilder = new StringBuilder(32);
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				stringBuilder.Append(numArray[i].ToString("x").PadLeft(2, '0'));
			}
			return stringBuilder.ToString();
		}

		public static string GetAbstractToMD5(byte[] dataFile)
		{
			byte[] numArray = (new MD5CryptoServiceProvider()).ComputeHash(dataFile);
			StringBuilder stringBuilder = new StringBuilder(32);
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				stringBuilder.Append(numArray[i].ToString("x").PadLeft(2, '0'));
			}
			return stringBuilder.ToString();
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

		public static void LogResult(string sWord)
		{
			string str = HttpContext.Current.Server.MapPath("log");
			DateTime now = DateTime.Now;
			str = string.Concat(str, "\\", now.ToString().Replace(":", ""), ".txt");
			StreamWriter streamWriter = new StreamWriter(str, false, Encoding.Default);
			streamWriter.Write(sWord);
			streamWriter.Close();
		}

		public static void setConfig(string partner, string sing_type, string private_key, string input_charset)
		{
			Hidistro.ControlPanel.OutPay.App.Core._partner = partner;
			Hidistro.ControlPanel.OutPay.App.Core._sign_type = sing_type;
			Hidistro.ControlPanel.OutPay.App.Core._input_charset = input_charset;
			Hidistro.ControlPanel.OutPay.App.Core._private_key = private_key;
		}
	}
}