using Hidistro.Core;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Hidistro.UI.Web.OpenAPI
{
	public class OpenApiHelper
	{
		public static bool CheckSystemParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, string app_key, out string result)
		{
			result = string.Empty;
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["app_key"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_App_Key, "app_key");
				return false;
			}
			if (app_key != parameters["app_key"])
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_App_Key, "app_key");
				return false;
			}
			if (!parameters.Keys.Contains("timestamp") || string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["timestamp"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Timestamp, "timestamp");
				return false;
			}
			if (!OpenApiHelper.IsDate(parameters["timestamp"]) || !OpenApiSign.CheckTimeStamp(parameters["timestamp"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "timestamp");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["sign"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Signature, "sign");
				return false;
			}
			return true;
		}

		public static System.Collections.Generic.SortedDictionary<string, string> GetSortedParams(System.Web.HttpContext context)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedDictionary = new System.Collections.Generic.SortedDictionary<string, string>();
			System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection
			{
				context.Request.Form,
				context.Request.QueryString
			};
			string[] allKeys = nameValueCollection.AllKeys;
			for (int i = 0; i < allKeys.Length; i++)
			{
				sortedDictionary.Add(allKeys[i], nameValueCollection[allKeys[i]]);
			}
			sortedDictionary.Remove("HIGW");
			return sortedDictionary;
		}

		public static bool IsDate(string s)
		{
			System.DateTime dateTime;
			return System.DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime);
		}
	}
}
