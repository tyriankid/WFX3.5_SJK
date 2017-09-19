using Hidistro.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Hidistro.Core
{
	public static class ExpressTrackingSetService
	{
		public static string GetHiShopExpTrackInfo(string shipperCode, string logisticsCode)
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			string value = masterSettings.Exp_appKey.Trim();
			string appSecret = masterSettings.Exp_appSecret.Trim();
			string text = "http://wuliu.kuaidiantong.cn/api/logistics";
			string result;
			if (string.IsNullOrWhiteSpace(text))
			{
				result = "没有配置快递接口地址!";
			}
			else if (string.IsNullOrWhiteSpace(shipperCode))
			{
				result = "没有输入快递公司编码,无法查询!";
			}
			else if (string.IsNullOrWhiteSpace(logisticsCode))
			{
				result = "没有输入快递编号,无法查询!";
			}
			else
			{
				IDictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("app_key", value);
				dictionary.Add("timestamp", DateTime.Now.ToString());
				dictionary.Add("shipperCode", shipperCode);
				dictionary.Add("logisticsCode", logisticsCode);
				string sign = ExpressTrackingSetService.GetSign(dictionary, appSecret);
				string text2 = ExpressTrackingSetService.GetRequestAPI(text, dictionary, sign);
				text2 = ExpressTrackingSetService.GetKuaidi100Format(text2);
				result = text2;
			}
			return result;
		}

		private static string GetKuaidi100Format(string str)
		{
			string result;
			if (string.IsNullOrEmpty(str))
			{
				result = "";
			}
			else
			{
				string text = str.Replace("\"traces\"", "\"data\"").Replace("\"acceptTime\"", "\"time\"").Replace("\"acceptStation\"", "\"context\"");
				result = text;
			}
			return result;
		}

		private static string GetRequestAPI(string apiUrl, IDictionary<string, string> dic, string sign)
		{
			string requestUriString = string.Format("{0}?app_key={1}&timestamp={2}&shipperCode={3}&logisticsCode={4}&sign={5}", new object[]
			{
				apiUrl,
				dic["app_key"],
				dic["timestamp"],
				dic["shipperCode"],
				dic["logisticsCode"],
				sign
			});
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			StreamReader streamReader = null;
			string text = "";
			string result;
			try
			{
				httpWebRequest = (WebRequest.Create(requestUriString) as HttpWebRequest);
				httpWebRequest.Method = "GET";
				httpWebResponse = (httpWebRequest.GetResponse() as HttpWebResponse);
				stream = httpWebResponse.GetResponseStream();
				streamReader = new StreamReader(stream);
				text = streamReader.ReadToEnd();
				result = text;
			}
			catch (Exception var_6_AF)
			{
				result = text;
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Dispose();
					streamReader = null;
				}
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
					httpWebResponse = null;
				}
				if (httpWebRequest != null)
				{
					httpWebRequest.Abort();
					httpWebRequest = null;
				}
			}
			return result;
		}

		private static string GetSign(IDictionary<string, string> parameters, string appSecret)
		{
			IDictionary<string, string> dictionary = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
			IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
			StringBuilder stringBuilder = new StringBuilder();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> current = enumerator.Current;
				string key = current.Key;
				current = enumerator.Current;
				string value = current.Value;
				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
				{
					stringBuilder.Append(key).Append(value);
				}
			}
			stringBuilder.Append(appSecret);
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder2.Append(array[i].ToString("X2"));
			}
			return stringBuilder2.ToString();
		}
	}
}
