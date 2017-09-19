using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiITrade.GetIncrementSoldTrades
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private ITrade tradeApi = new TradeApi();

		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.GetIncrementSoldTrades(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
		}

		private void GetIncrementSoldTrades(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string results)
		{
			string status = "";
			int page_no = 0;
			int page_size = 0;
			System.DateTime start_modified;
			System.DateTime end_modified;
			if (!this.CheckIncrementSoldTradesParameters(parameters, out start_modified, out end_modified, out status, out page_no, out page_size, ref results))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref results))
			{
				return;
			}
			results = this.tradeApi.GetIncrementSoldTrades(start_modified, end_modified, status, parameters["buyer_uname"], page_no, page_size);
		}

		private bool CheckIncrementSoldTradesParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out System.DateTime start_modified, out System.DateTime end_modified, out string status, out int page_no, out int page_size, ref string result)
		{
			start_modified = System.DateTime.Now;
			end_modified = System.DateTime.Now;
			page_size = 10;
			page_no = 1;
			status = DataHelper.CleanSearchString(parameters["status"]);
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (!string.IsNullOrWhiteSpace(status) && status != "WAIT_BUYER_PAY" && status != "WAIT_SELLER_SEND_GOODS " && status != "WAIT_BUYER_CONFIRM_GOODS" && status != "TRADE_CLOSED" && status != "TRADE_FINISHED")
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Status_is_Invalid, "status");
				return false;
			}
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["page_size"])) && !int.TryParse(parameters["page_size"].ToString(), out page_size))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "page_size");
				return false;
			}
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["page_size"])) && (page_size <= 0 || page_size > 100))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Page_Size_Too_Long, "page_size");
				return false;
			}
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["page_no"])) && !int.TryParse(parameters["page_no"].ToString(), out page_no))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "page_no");
				return false;
			}
			if (!string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["page_no"])) && page_no <= 0)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Page_Size_Too_Long, "page_no");
				return false;
			}
			if (string.IsNullOrEmpty(parameters["start_modified"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "start_modified");
				return false;
			}
			if (!OpenApiHelper.IsDate(parameters["start_modified"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "start_modified");
				return false;
			}
			System.DateTime.TryParse(parameters["start_modified"], out start_modified);
			if (start_modified > System.DateTime.Now)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_Start_Now, "start_modified and currenttime");
				return false;
			}
			if (string.IsNullOrEmpty(parameters["end_modified"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "end_modified");
				return false;
			}
			if (!OpenApiHelper.IsDate(parameters["end_modified"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "end_modified");
				return false;
			}
			System.DateTime.TryParse(parameters["end_modified"], out end_modified);
			if (start_modified > end_modified)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_Start_End, "start_modified and end_modified");
				return false;
			}
			if ((end_modified - start_modified).TotalDays > 1.0)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_StartModified_AND_EndModified, "start_modified and end_modified");
				return false;
			}
			if (end_modified > System.DateTime.Now)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_End_Now, "end_modified and currenttime");
				return false;
			}
			return true;
		}
	}
}
