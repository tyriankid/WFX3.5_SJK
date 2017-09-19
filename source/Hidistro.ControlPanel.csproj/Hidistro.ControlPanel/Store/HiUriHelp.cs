using Hidistro.Core;
using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Hidistro.ControlPanel.Store
{
	public class HiUriHelp
	{
		private NameValueCollection queryStrings;

		public string NewUrl
		{
			get;
			set;
		}

		public NameValueCollection QueryStrings
		{
			get
			{
				return this.queryStrings;
			}
			set
			{
				this.queryStrings = value;
			}
		}

		public HiUriHelp(string query)
		{
			this.CreateQueryString(query);
		}

		public HiUriHelp(NameValueCollection query)
		{
			this.QueryStrings = new NameValueCollection(query);
		}

		public void AddQueryString(string key, string value)
		{
			if (this.QueryStrings == null)
			{
				this.QueryStrings = new NameValueCollection();
			}
			this.QueryStrings.Add(key, value);
		}

		public void CreateQueryString(string queryString)
		{
			queryString = queryString.Replace("?", "");
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
			if (!string.IsNullOrEmpty(queryString))
			{
				int length = queryString.Length;
				for (int i = 0; i < length; i++)
				{
					int num = i;
					int num1 = -1;
					while (i < length)
					{
						char chr = queryString[i];
						if (chr == '=')
						{
							if (num1 < 0)
							{
								num1 = i;
							}
						}
						else if (chr == '&')
						{
							break;
						}
						i++;
					}
					string str = null;
					string str1 = null;
					if (num1 < 0)
					{
						str = queryString.Substring(num, i - num);
					}
					else
					{
						str = queryString.Substring(num, num1 - num);
						str1 = queryString.Substring(num1 + 1, i - num1 - 1);
					}
					this.AddQueryString(str, str1);
					if ((i != length - 1 ? false : queryString[i] == '&'))
					{
						this.AddQueryString(str, string.Empty);
					}
				}
			}
		}

		public string GetNewQuery()
		{
			string str = "?";
			string[] allKeys = this.QueryStrings.AllKeys;
			for (int i = 0; i < (int)allKeys.Length; i++)
			{
				string str1 = allKeys[i];
				Globals.Debuglog(string.Concat(str1, "：", this.QueryStrings[str1], "\r\n"), "_Debuglog.txt");
				string str2 = str;
				string[] item = new string[] { str2, str1, "=", this.QueryStrings[str1], "&" };
				str = string.Concat(item);
			}
			str = str.TrimEnd(new char[] { '&' });
			return str;
		}

		public string GetQueryString(string key)
		{
			return ((string.IsNullOrEmpty(key) ? true : this.queryStrings == null) ? "" : this.QueryStrings[key]);
		}

		public void RemoveQueryString(string key)
		{
			if ((this.QueryStrings != null ? true : this.QueryStrings.Count > 1))
			{
				this.QueryStrings.Remove(key);
			}
		}

		public void SetQueryString(string key, string value)
		{
			if ((string.IsNullOrEmpty(key) ? false : this.queryStrings != null))
			{
				this.QueryStrings[key] = value;
			}
		}
	}
}