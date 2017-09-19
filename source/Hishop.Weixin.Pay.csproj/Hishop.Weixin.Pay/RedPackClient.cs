using Hishop.Weixin.Pay.Domain;
using Hishop.Weixin.Pay.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Hishop.Weixin.Pay
{
	public class RedPackClient
	{
		public static readonly string SendRedPack_Url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";

		public static readonly string QueryRedPackUrl = "https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo";

		private static object LockLog = new object();

		public string SendRedpack(string appId, string mch_id, string sub_mch_id, string nick_name, string send_name, string re_openid, string wishing, string client_ip, string act_name, string remark, int amount, string partnerkey, string weixincertpath, string weixincertpassword, string mch_billno, bool enablesp, string main_appId, string main_mch_id, string main_paykey)
		{
			return this.SendRedpack(new SendRedPackInfo
			{
				WXAppid = appId,
				Mch_Id = mch_id,
				Sub_Mch_Id = mch_id,
				Main_AppId = main_appId,
				Main_Mch_ID = main_mch_id,
				Main_PayKey = main_paykey,
				EnableSP = enablesp,
				Nick_Name = nick_name,
				Send_Name = send_name,
				Re_Openid = re_openid,
				Wishing = wishing,
				Client_IP = client_ip,
				Act_Name = act_name,
				Remark = remark,
				Total_Amount = amount,
				PartnerKey = partnerkey,
				WeixinCertPath = weixincertpath,
				WeixinCertPassword = weixincertpassword,
				SendRedpackRecordID = mch_billno
			});
		}

		public string CreatRedpackId(string mch_id)
		{
			return mch_id + DateTime.Now.ToString("yyyymmdd") + DateTime.Now.ToString("MMddHHmmss");
		}

		public RedPackInfo GetRedpackInfo(string appId, string mch_id, string mch_billno, string partnerkey, string weixincertpath, string weixincertpassword)
		{
			PayDictionary payDictionary = new PayDictionary();
			payDictionary.Add("nonce_str", Utils.CreateNoncestr());
			payDictionary.Add("mch_billno", mch_billno);
			payDictionary.Add("mch_id", mch_id);
			payDictionary.Add("appid", appId);
			payDictionary.Add("bill_type", "MCHT");
			string value = SignHelper.SignPackage(payDictionary, partnerkey);
			payDictionary.Add("sign", value);
			string data = SignHelper.BuildXml(payDictionary, false);
			string text = "";
			try
			{
				text = RedPackClient.Send(weixincertpath, weixincertpassword, data, RedPackClient.QueryRedPackUrl);
			}
			catch (Exception ex)
			{
				text = ex.Message;
			}
			RedPackInfo result;
			if (!string.IsNullOrEmpty(text) && text.Contains("return_code"))
			{
				result = RedPackClient.ConvertDic<RedPackInfo>(RedPackClient.FromXml(text));
			}
			else
			{
				result = new RedPackInfo
				{
					return_code = "FAIL",
					return_msg = text,
					status = ""
				};
			}
			return result;
		}

		public static T ConvertDic<T>(Dictionary<string, object> dic)
		{
			T t = Activator.CreateInstance<T>();
			PropertyInfo[] properties = t.GetType().GetProperties();
			if (properties.Length > 0 && dic.Count > 0)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					if (dic.ContainsKey(properties[i].Name))
					{
						if (properties[i].PropertyType.IsEnum)
						{
							object value = Enum.ToObject(properties[i].PropertyType, dic[properties[i].Name]);
							properties[i].SetValue(t, value, null);
						}
						else
						{
							properties[i].SetValue(t, RedPackClient.CheckType(dic[properties[i].Name], properties[i].PropertyType), null);
						}
					}
				}
			}
			return t;
		}

		private static object CheckType(object value, Type conversionType)
		{
			object result;
			if (value == null)
			{
				result = null;
			}
			else
			{
				result = Convert.ChangeType(value, conversionType);
			}
			return result;
		}

		public static Dictionary<string, object> FromXml(string xml)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> result;
			if (string.IsNullOrEmpty(xml))
			{
				result = null;
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xml);
				XmlNode firstChild = xmlDocument.FirstChild;
				XmlNodeList childNodes = firstChild.ChildNodes;
				foreach (XmlNode xmlNode in childNodes)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					dictionary[xmlElement.Name] = xmlElement.InnerText;
				}
				result = dictionary;
			}
			return result;
		}

		public string SendRedpack(SendRedPackInfo sendredpack)
		{
			string result = string.Empty;
			PayDictionary payDictionary = new PayDictionary();
			payDictionary.Add("nonce_str", Utils.CreateNoncestr());
			if (sendredpack.EnableSP)
			{
				if (!string.IsNullOrEmpty(sendredpack.SendRedpackRecordID))
				{
					payDictionary.Add("mch_billno", sendredpack.SendRedpackRecordID);
				}
				else
				{
					payDictionary.Add("mch_billno", this.CreatRedpackId(sendredpack.Main_Mch_ID));
				}
				payDictionary.Add("mch_id", sendredpack.Main_Mch_ID);
				payDictionary.Add("sub_mch_id", sendredpack.Sub_Mch_Id);
				payDictionary.Add("wxappid", sendredpack.Main_AppId);
				payDictionary.Add("msgappid", sendredpack.Main_AppId);
			}
			else
			{
				if (!string.IsNullOrEmpty(sendredpack.SendRedpackRecordID))
				{
					payDictionary.Add("mch_billno", sendredpack.SendRedpackRecordID);
				}
				else
				{
					payDictionary.Add("mch_billno", this.CreatRedpackId(sendredpack.Mch_Id));
				}
				payDictionary.Add("mch_id", sendredpack.Mch_Id);
				payDictionary.Add("wxappid", sendredpack.WXAppid);
				payDictionary.Add("nick_name", sendredpack.Nick_Name);
				payDictionary.Add("min_value", sendredpack.Total_Amount);
				payDictionary.Add("max_value", sendredpack.Total_Amount);
			}
			payDictionary.Add("send_name", sendredpack.Send_Name);
			payDictionary.Add("re_openid", sendredpack.Re_Openid);
			payDictionary.Add("total_amount", sendredpack.Total_Amount);
			payDictionary.Add("total_num", sendredpack.Total_Num);
			payDictionary.Add("wishing", sendredpack.Wishing);
			payDictionary.Add("client_ip", sendredpack.Client_IP);
			payDictionary.Add("act_name", sendredpack.Act_Name);
			payDictionary.Add("remark", sendredpack.Remark);
			string value = SignHelper.SignPackage(payDictionary, sendredpack.PartnerKey);
			payDictionary.Add("sign", value);
			string text = SignHelper.BuildXml(payDictionary, false);
			RedPackClient.Debuglog(text, "_DebugRedPacklog.txt");
			string text2 = RedPackClient.Send(sendredpack.WeixinCertPath, sendredpack.WeixinCertPassword, text, RedPackClient.SendRedPack_Url);
			RedPackClient.Debuglog(text2, "_DebugRedPacklog.txt");
			if (!string.IsNullOrEmpty(text2) && text2.Contains("SUCCESS") && !text2.Contains("<![CDATA[FAIL]]></result_code>"))
			{
				result = "success";
			}
			else
			{
				Regex regex = new Regex("<return_msg><!\\[CDATA\\[(?<code>(.*))\\]\\]></return_msg>");
				Match match = regex.Match(text2);
				string empty = string.Empty;
				if (match.Success)
				{
					result = match.Groups["code"].Value;
				}
				else
				{
					result = text2;
				}
			}
			return result;
		}

		public static void Debuglog(string log, string logname = "_DebugRedPacklog.txt")
		{
			lock (RedPackClient.LockLog)
			{
				try
				{
					string str = DateTime.Now.ToString("yyyyMMdd") + logname;
					string path = HttpRuntime.AppDomainAppPath.ToString() + "App_Data/" + str;
					StreamWriter streamWriter = File.AppendText(path);
					streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + log);
					streamWriter.WriteLine("---------------");
					streamWriter.Close();
				}
				catch (Exception var_3_88)
				{
				}
			}
		}

		public static string Send(string cert, string password, string data, string url)
		{
			return RedPackClient.Send(cert, password, Encoding.GetEncoding("UTF-8").GetBytes(data), url);
		}

		public static string Send(string cert, string password, byte[] data, string url)
		{
			ServicePointManager.ServerCertificateValidationCallback = ((object s, X509Certificate ch, X509Chain er, SslPolicyErrors c) => true);
			X509Certificate2 value;
			try
			{
				byte[] array;
				using (FileStream fileStream = new FileStream(cert, FileMode.Open, FileAccess.Read))
				{
					array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
					fileStream.Close();
				}
				value = new X509Certificate2(array, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
			if (httpWebRequest == null)
			{
				throw new ApplicationException(string.Format("Invalid url string: {0}", url));
			}
			httpWebRequest.UserAgent = "Hishop";
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.ClientCertificates.Add(value);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentLength = (long)data.Length;
			Stream requestStream = httpWebRequest.GetRequestStream();
			requestStream.Write(data, 0, data.Length);
			requestStream.Close();
			Stream responseStream;
			try
			{
				responseStream = httpWebRequest.GetResponse().GetResponseStream();
			}
			catch (Exception ex2)
			{
				throw ex2;
			}
			string result = string.Empty;
			using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
			{
				result = streamReader.ReadToEnd();
			}
			responseStream.Close();
			return result;
		}

		public static void writeLog(IDictionary<string, string> param, string sign, string url, string msg)
		{
		}

		public static string PostData(string url, string postData)
		{
			string text = string.Empty;
			string result;
			try
			{
				Uri requestUri = new Uri(url);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
				Encoding uTF = Encoding.UTF8;
				byte[] bytes = uTF.GetBytes(postData);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "text/xml";
				httpWebRequest.ContentLength = (long)postData.Length;
				using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					streamWriter.Write(postData);
				}
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						Encoding uTF2 = Encoding.UTF8;
						StreamReader streamReader = new StreamReader(responseStream, uTF2);
						text = streamReader.ReadToEnd();
						XmlDocument xmlDocument = new XmlDocument();
						try
						{
							xmlDocument.LoadXml(text);
						}
						catch (Exception ex)
						{
							text = string.Format("获取信息错误doc.load：{0}", ex.Message) + text;
						}
						try
						{
							if (xmlDocument == null)
							{
								result = text;
								return result;
							}
							XmlNode xmlNode = xmlDocument.SelectSingleNode("xml/return_code");
							if (xmlNode == null)
							{
								result = text;
								return result;
							}
							if (!(xmlNode.InnerText == "SUCCESS"))
							{
								result = xmlDocument.InnerXml;
								return result;
							}
							XmlNode xmlNode2 = xmlDocument.SelectSingleNode("xml/prepay_id");
							if (xmlNode2 != null)
							{
								result = xmlNode2.InnerText;
								return result;
							}
						}
						catch (Exception ex)
						{
							text = string.Format("获取信息错误node.load：{0}", ex.Message) + text;
						}
					}
				}
			}
			catch (Exception ex)
			{
				text = string.Format("获取信息错误post error：{0}", ex.Message) + text;
			}
			result = text;
			return result;
		}
	}
}
