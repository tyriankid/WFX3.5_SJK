using Hidistro.ControlPanel.Members;
using Hidistro.Core;
using Hidistro.Entities.Store;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web.Caching;

namespace Hidistro.ControlPanel.Store
{
	public static class SystemAuthorizationHelper
	{
		private readonly static string authorizationUrl;

		public readonly static string noticeMsg;

		public readonly static string licenseMsg;

		static SystemAuthorizationHelper()
		{
			SystemAuthorizationHelper.authorizationUrl = "http://ysc.kuaidiantong.cn/wfxvalid.ashx";
			SystemAuthorizationHelper.noticeMsg = string.Concat("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <Hi:HeadContainer ID=\"HeadContainer1\" runat=\"server\" />\r\n    <Hi:PageTitle ID=\"PageTitle1\" runat=\"server\" />\r\n    <link rel=\"stylesheet\" href=\"css/login.css\" type=\"text/css\" media=\"screen\" />\r\n</head>\r\n<body>\r\n<div class=\"admin\">\r\n<div id=\"\" class=\"wrap\">\r\n<div class=\"main\" style=\"position:relative\">\r\n    <div class=\"LoginBack\">\r\n     <div>\r\n     <table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">\r\n      <tr>\r\n        <td class=\"td1\"><img src=\"images/comeBack.gif\" width=\"56\" height=\"49\" /></td>\r\n        <td class=\"td2\">您正在使用的系统已过授权有效期，无法登录后台管理。请续费。感谢您的关注！</td>\r\n      </tr>\r\n      <tr>\r\n        <th colspan=\"2\"><a href=\"", Globals.ApplicationPath, "/Default.aspx\">返回前台</a></th>\r\n        </tr>\r\n    </table>\r\n     </div>\r\n    </div>\r\n</div>\r\n</div><div class=\"footer\">Copyright 2015 hishop.com.cn all Rights Reserved. 本产品资源均为 Hishop 版权所有</div>\r\n</div>\r\n</body>\r\n</html>");
			SystemAuthorizationHelper.licenseMsg = string.Concat("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <Hi:HeadContainer ID=\"HeadContainer1\" runat=\"server\" />\r\n    <Hi:PageTitle ID=\"PageTitle1\" runat=\"server\" />\r\n    <link rel=\"stylesheet\" href=\"css/login.css\" type=\"text/css\" media=\"screen\" />\r\n</head>\r\n<body>\r\n<div class=\"admin\">\r\n<div id=\"\" class=\"wrap\">\r\n<div class=\"main\" style=\"position:relative\">\r\n    <div class=\"LoginBack\">\r\n     <div>\r\n     <table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">\r\n      <tr>\r\n        <td class=\"td1\"><img src=\"images/comeBack.gif\" width=\"56\" height=\"49\" /></td>\r\n        <td class=\"td2\">您正在使用的系统未经官方授权，无法登录后台管理。请联系官方购买软件使用权。感谢您的关注！</td>\r\n      </tr>\r\n      <tr>\r\n        <th colspan=\"2\"><a href=\"", Globals.ApplicationPath, "/Default.aspx\">返回前台</a></th>\r\n        </tr>\r\n    </table>\r\n     </div>\r\n    </div>\r\n</div>\r\n</div><div class=\"footer\">Copyright 2015 hishop.com.cn all Rights Reserved. 本产品资源均为 Hishop 版权所有</div>\r\n</div>\r\n</body>\r\n</html>");
		}

		public static bool CheckDistributorIsCanAuthorization()
		{
			int num = 0;
			return SystemAuthorizationHelper.CheckDistributorIsCanAuthorization(1, out num);
		}

		public static bool CheckDistributorIsCanAuthorization(int number, out int leftNumber)
		{
			bool distributorCount;
			leftNumber = 0;
			SystemAuthorizationInfo systemAuthorization = SystemAuthorizationHelper.GetSystemAuthorization(false);
			if (systemAuthorization.DistributorCount <= 0)
			{
				distributorCount = true;
			}
			else
			{
				int systemDistributorsCount = MemberHelper.GetSystemDistributorsCount();
				leftNumber = systemAuthorization.DistributorCount - systemDistributorsCount;
				distributorCount = systemAuthorization.DistributorCount >= systemDistributorsCount + number;
			}
			return distributorCount;
		}

		public static SystemAuthorizationInfo GetSystemAuthorization(bool iscreate)
		{
            //string str = "DataCache-SystemAuthorizationInfo";
            //SystemAuthorizationInfo systemAuthorizationInfo = HiCache.Get(str) as SystemAuthorizationInfo;
            //if ((systemAuthorizationInfo == null ? true : iscreate))
            //{
            //    string str1 = SystemAuthorizationHelper.PostData(SystemAuthorizationHelper.authorizationUrl, string.Concat("host=", Globals.DomainName));
            //    if (!string.IsNullOrEmpty(str1))
            //    {
            //        TempAuthorizationInfo tempAuthorizationInfo = JsonConvert.DeserializeObject<TempAuthorizationInfo>(str1);
            //        SystemAuthorizationInfo systemAuthorizationInfo1 = new SystemAuthorizationInfo()
            //        {
            //            state = (SystemAuthorizationState)tempAuthorizationInfo.state,
            //            DistributorCount = tempAuthorizationInfo.count,
            //            type = tempAuthorizationInfo.type,
            //            IsShowJixuZhiChi = tempAuthorizationInfo.isshowjszc == "1"
            //        };
            //        systemAuthorizationInfo = systemAuthorizationInfo1;
            //        HiCache.Insert(str, systemAuthorizationInfo, 360, CacheItemPriority.Normal);
            //    }
            //}
            //return systemAuthorizationInfo;
            string key = "DataCache-SystemAuthorizationInfo";
            SystemAuthorizationInfo systemAuthorizationInfo = HiCache.Get(key) as SystemAuthorizationInfo;
            bool flag = systemAuthorizationInfo == null;
            if (flag)
            {
                SystemAuthorizationInfo systemAuthorizationInfo2 = new SystemAuthorizationInfo
                {
                    state = SystemAuthorizationState.正常权限,
                    DistributorCount = 1000000,
                    type = "1",
                    IsShowJixuZhiChi = true
                };
                systemAuthorizationInfo = systemAuthorizationInfo2;
                HiCache.Insert(key, systemAuthorizationInfo, 360, CacheItemPriority.Normal);
            }
            return systemAuthorizationInfo;
		}

		public static string PostData(string url, string postData)
		{
			string empty = string.Empty;
			try
			{
				HttpWebRequest length = (HttpWebRequest)WebRequest.Create(new Uri(url));
				byte[] bytes = Encoding.UTF8.GetBytes(postData);
				length.Method = "POST";
				length.ContentType = "application/x-www-form-urlencoded";
				length.ContentLength = (long)((int)bytes.Length);
				Stream requestStream = length.GetRequestStream();
				try
				{
					requestStream.Write(bytes, 0, (int)bytes.Length);
				}
				finally
				{
					if (requestStream != null)
					{
						((IDisposable)requestStream).Dispose();
					}
				}
				HttpWebResponse response = (HttpWebResponse)length.GetResponse();
				try
				{
					Stream responseStream = response.GetResponseStream();
					try
					{
						Encoding uTF8 = Encoding.UTF8;
						Stream gZipStream = responseStream;
						if (response.ContentEncoding.ToLower() == "gzip")
						{
							gZipStream = new GZipStream(responseStream, CompressionMode.Decompress);
						}
						else if (response.ContentEncoding.ToLower() == "deflate")
						{
							gZipStream = new DeflateStream(responseStream, CompressionMode.Decompress);
						}
						StreamReader streamReader = new StreamReader(gZipStream, uTF8);
						try
						{
							empty = streamReader.ReadToEnd();
						}
						finally
						{
							if (streamReader != null)
							{
								((IDisposable)streamReader).Dispose();
							}
						}
					}
					finally
					{
						if (responseStream != null)
						{
							((IDisposable)responseStream).Dispose();
						}
					}
				}
				finally
				{
					if (response != null)
					{
						((IDisposable)response).Dispose();
					}
				}
			}
			catch (Exception exception)
			{
				empty = string.Empty;
			}
			return empty;
		}
	}
}