using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiITrade.UpdateTradeMemo
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private ITrade tradeApi = new TradeApi();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.UpdateTradeMemo(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
		}

		public void UpdateTradeMemo(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			int flag = 0;
			if (!this.CheckUpdateTradeMemoParameters(parameters, out flag, ref result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.tradeApi.UpdateTradeMemo(parameters["tid"], parameters["memo"], flag);
		}

		private bool CheckUpdateTradeMemoParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out int flag, ref string result)
		{
			flag = 0;
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["tid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "tid");
				return false;
			}
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["flag"])))
			{
				if (!int.TryParse(DataHelper.CleanSearchString(parameters["flag"]), out flag))
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "flag");
					return false;
				}
				if (flag < 1 || flag > 6)
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Flag_Too_Long, "flag");
					return false;
				}
			}
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["memo"])) && DataHelper.CleanSearchString(parameters["memo"]).Length > 300)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Memo_Too_Long, "memo");
				return false;
			}
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^(?!_)(?!.*?_$)(?!-)(?!.*?-$)[a-zA-Z0-9._一-龥-]+$");
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["memo"])) && !regex.IsMatch(DataHelper.CleanSearchString(parameters["memo"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "memo");
				return false;
			}
			return true;
		}
	}
}
