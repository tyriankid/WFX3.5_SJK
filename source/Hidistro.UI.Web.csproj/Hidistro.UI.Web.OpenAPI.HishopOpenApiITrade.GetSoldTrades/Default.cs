using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiITrade.GetSoldTrades
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
			this.GetSoldTrades(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			Globals.Debuglog(base.Request.Url.ToString() + "||" + Globals.SubStr(empty, 200, "------------结束"), "_DebugERP.txt");
			base.Response.Write(empty);
			base.Response.End();
		}

		private void GetSoldTrades(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string results)
		{
			System.DateTime? start_created = null;
			System.DateTime? end_created = null;
			string empty = string.Empty;
			int page_no = 0;
			int page_size = 0;
			if (!this.CheckSoldTradesParameters(parameters, out start_created, out end_created, out empty, out page_no, out page_size, ref results))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref results))
			{
				return;
			}
			results = this.tradeApi.GetSoldTrades(start_created, end_created, empty, parameters["buyer_uname"], page_no, page_size);
		}

		private bool CheckSoldTradesParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out System.DateTime? start_time, out System.DateTime? end_time, out string status, out int page_no, out int page_size, ref string result)
		{
			start_time = null;
			end_time = null;
			page_size = 10;
			page_no = 1;
			status = DataHelper.CleanSearchString(parameters["status"]);
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(parameters["start_created"]) && !OpenApiHelper.IsDate(parameters["start_created"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "start_created");
				return false;
			}
			if (!string.IsNullOrEmpty(parameters["end_created"]) && !OpenApiHelper.IsDate(parameters["end_created"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "end_created");
				return false;
			}
			if (!string.IsNullOrEmpty(parameters["start_created"]))
			{
				System.DateTime dateTime;
				System.DateTime.TryParse(parameters["start_created"], out dateTime);
				start_time = new System.DateTime?(dateTime);
				if (dateTime > System.DateTime.Now)
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_Start_Now, "start_created and currenttime");
					return false;
				}
				if (!string.IsNullOrEmpty(parameters["end_created"]))
				{
					System.DateTime dateTime2;
					System.DateTime.TryParse(parameters["end_created"], out dateTime2);
					end_time = new System.DateTime?(dateTime2);
					if (dateTime > dateTime2)
					{
						result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_Start_End, "start_created and end_created");
						return false;
					}
					if (dateTime2 > System.DateTime.Now)
					{
						result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_End_Now, "end_created and currenttime");
						return false;
					}
				}
			}
			else if (!string.IsNullOrEmpty(parameters["end_created"]))
			{
				System.DateTime dateTime2;
				System.DateTime.TryParse(parameters["end_created"], out dateTime2);
				if (dateTime2 > System.DateTime.Now)
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_End_Now, "end_created and currenttime");
					return false;
				}
			}
			if (!string.IsNullOrWhiteSpace(status) && status != "WAIT_BUYER_PAY" && status != "WAIT_SELLER_SEND_GOODS" && status != "WAIT_BUYER_CONFIRM_GOODS" && status != "TRADE_CLOSED" && status != "TRADE_FINISHED")
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Status_is_Invalid, "status");
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
			return true;
		}
	}
}
