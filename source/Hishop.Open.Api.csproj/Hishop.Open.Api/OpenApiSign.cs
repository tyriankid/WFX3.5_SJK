using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Hishop.Open.Api
{
	public static class OpenApiSign
	{
		public static Dictionary<string, string> Parameterfilter(SortedDictionary<string, string> dicArrayPre)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> current in dicArrayPre)
			{
				if (current.Key.ToLower() != "sign" && current.Key.ToLower() != "sign_type" && current.Value != "" && current.Value != null)
				{
					dictionary.Add(current.Key.ToLower(), current.Value);
				}
			}
			return dictionary;
		}

		public static string BuildSign(Dictionary<string, string> dicArray, string appSecret, string sign_type, string _input_charset)
		{
			string text = OpenApiSign.CreateLinkstring(dicArray);
			text += appSecret;
			return OpenApiSign.Sign(text, sign_type, _input_charset);
		}

		public static string CreateLinkstring(Dictionary<string, string> dicArray)
		{
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(dicArray);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> current in sortedDictionary)
			{
				if (!string.IsNullOrEmpty(current.Key) && !string.IsNullOrEmpty(current.Value))
				{
					stringBuilder.Append(current.Key + current.Value);
				}
			}
			return stringBuilder.ToString();
		}

		public static string Sign(string prestr, string sign_type, string _input_charset)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			if (sign_type.ToUpper() == "MD5")
			{
				MD5 mD = new MD5CryptoServiceProvider();
				byte[] array = mD.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x").PadLeft(2, '0'));
				}
			}
			return stringBuilder.ToString().ToUpper();
		}

		public static string PostData(string url, string postData)
		{
			string result = string.Empty;
			try
			{
				Uri requestUri = new Uri(url);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
				Encoding uTF = Encoding.UTF8;
				byte[] bytes = uTF.GetBytes(postData);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = (long)bytes.Length;
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						Encoding uTF2 = Encoding.UTF8;
						Stream stream = responseStream;
						if (httpWebResponse.ContentEncoding.ToLower() == "gzip")
						{
							stream = new GZipStream(responseStream, CompressionMode.Decompress);
						}
						else if (httpWebResponse.ContentEncoding.ToLower() == "deflate")
						{
							stream = new DeflateStream(responseStream, CompressionMode.Decompress);
						}
						using (StreamReader streamReader = new StreamReader(stream, uTF2))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				result = string.Format("获取信息错误：{0}", ex.Message);
			}
			return result;
		}

		public static string GetData(string url, string method = "GET")
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = method;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}

		public static bool CheckSign(SortedDictionary<string, string> tmpParas, string appSecret, ref string message)
		{
			Dictionary<string, string> dicArray = OpenApiSign.Parameterfilter(tmpParas);
			bool flag = OpenApiSign.BuildSign(dicArray, appSecret, "MD5", "utf-8") == tmpParas["sign"];
			message = (flag ? "" : OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Signature, "sign"));
			return flag;
		}

		public static string GetSign(SortedDictionary<string, string> tmpParas, string keycode)
		{
			Dictionary<string, string> dicArray = OpenApiSign.Parameterfilter(tmpParas);
			return OpenApiSign.BuildSign(dicArray, keycode, "MD5", "utf-8");
		}

		public static bool CheckTimeStamp(string timestamp)
		{
			DateTime dateTime = DateTime.Parse(timestamp);
			TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan timeSpan2 = new TimeSpan(dateTime.Ticks);
			return timeSpan.Minutes - timeSpan2.Minutes <= 10;
		}
	}
}
