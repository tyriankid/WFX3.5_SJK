using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hidistro.Vshop;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Hidistro.UI.Web.OpenAPI
{
	public class ITradeApi : System.Web.IHttpHandler
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private ITrade tradeApi = new TradeApi();

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			string s = string.Empty;
			string text = string.Empty;
			string arg_11_0 = string.Empty;
			text = context.Request["HIGW"];
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(context);
			string key;
			switch (key = text)
			{
			case "GetSoldTrades":
				this.GetSoldTrades(sortedParams, ref s);
				goto IL_115;
			case "GetTrade":
				this.GetTrade(sortedParams, ref s);
				goto IL_115;
			case "GetIncrementSoldTrades":
				this.GetIncrementSoldTrades(sortedParams, ref s);
				goto IL_115;
			case "SendLogistic":
				this.SendLogistic(sortedParams, ref s);
				goto IL_115;
			case "UpdateTradeMemo":
				this.UpdateTradeMemo(sortedParams, ref s);
				goto IL_115;
			case "ChangLogistics":
				this.ChangLogistics(sortedParams, ref s);
				goto IL_115;
			}
			s = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Method, "HIGW");
			IL_115:
			context.Response.ContentType = "text/json";
			context.Response.Write(s);
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

		public void ChangLogistics(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			if (!this.CheckChangLogisticsParameters(parameters, ref result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.tradeApi.ChangLogistics(parameters["tid"], parameters["company_name"], parameters["out_sid"]);
		}

		private bool CheckChangLogisticsParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["tid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "tid");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["company_name"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "company_name");
				return false;
			}
			if (!ExpressHelper.IsExitExpress(DataHelper.CleanSearchString(parameters["company_name"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Company_not_Exists, "company_name");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["out_sid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "out_sid");
				return false;
			}
			if (DataHelper.CleanSearchString(parameters["out_sid"]).Length > 20)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Out_Sid_Too_Long, "out_sid");
				return false;
			}
			return true;
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

		public void SendLogistic(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			if (!this.CheckSendLogisticParameters(parameters, ref result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.tradeApi.SendLogistic(parameters["tid"], parameters["company_name"], parameters["out_sid"]);
		}

		private bool CheckSendLogisticParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["tid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "tid");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["company_name"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "company_name");
				return false;
			}
			if (!ExpressHelper.IsExitExpress(DataHelper.CleanSearchString(parameters["company_name"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Company_not_Exists, "company_name");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["out_sid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "out_sid");
				return false;
			}
			if (DataHelper.CleanSearchString(parameters["out_sid"]).Length > 20)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Out_Sid_Too_Long, "out_sid");
				return false;
			}
			return true;
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

		public bool CheckHasNext(int totalrecord, int pagesize, int pageindex)
		{
			int num = pagesize * pageindex;
			return totalrecord > num;
		}
	}
}
