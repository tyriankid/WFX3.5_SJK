using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiITrade.GetTrade
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private ITrade tradeApi = new TradeApi();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.GetTrade(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
		}

		public void GetTrade(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string results)
		{
			if (!this.CheckTradesParameters(parameters, ref results))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref results))
			{
				return;
			}
			results = this.tradeApi.GetTrade(parameters["tid"]);
		}

		private bool CheckTradesParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string results)
		{
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out results))
			{
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["tid"])))
			{
				results = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "tid");
				return false;
			}
			return true;
		}
	}
}
