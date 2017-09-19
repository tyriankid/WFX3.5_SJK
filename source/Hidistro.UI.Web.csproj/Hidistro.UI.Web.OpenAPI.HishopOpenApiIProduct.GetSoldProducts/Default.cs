using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiIProduct.GetSoldProducts
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private IProduct productApi = new ProductApi();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.GetSoldProducts(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
		}

		public void GetSoldProducts(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			System.DateTime? start_modified = null;
			System.DateTime? end_modified = null;
			string approve_status = "";
			int page_no = 0;
			int page_size = 0;
			if (!this.CheckSoldProductsParameters(parameters, out start_modified, out end_modified, out approve_status, out page_no, out page_size, out result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.productApi.GetSoldProducts(start_modified, end_modified, approve_status, parameters["q"], parameters["order_by"], page_no, page_size);
		}

		public bool CheckSoldProductsParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out System.DateTime? start_modified, out System.DateTime? end_modified, out string status, out int page_no, out int page_size, out string result)
		{
			start_modified = null;
			end_modified = null;
			status = string.Empty;
			page_no = 1;
			page_size = 10;
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			status = DataHelper.CleanSearchString(parameters["approve_status"]);
			if (!string.IsNullOrWhiteSpace(status) && status != "On_Sale" && status != "Un_Sale" && status != "In_Stock")
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_Status_is_Invalid, "approve_status");
				return false;
			}
			if (!string.IsNullOrEmpty(parameters["start_modified"]) && !OpenApiHelper.IsDate(parameters["start_modified"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "start_modified");
				return false;
			}
			if (!string.IsNullOrEmpty(parameters["end_modified"]) && !OpenApiHelper.IsDate(parameters["end_modified"]))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Timestamp, "end_modified");
				return false;
			}
			if (!string.IsNullOrEmpty(parameters["start_modified"]))
			{
				System.DateTime dateTime;
				System.DateTime.TryParse(parameters["start_modified"], out dateTime);
				start_modified = new System.DateTime?(dateTime);
				if (dateTime > System.DateTime.Now)
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_Start_Now, "start_modified and currenttime");
					return false;
				}
				if (!string.IsNullOrEmpty(parameters["end_modified"]))
				{
					System.DateTime dateTime2;
					System.DateTime.TryParse(parameters["end_modified"], out dateTime2);
					end_modified = new System.DateTime?(dateTime2);
					if (dateTime > dateTime2)
					{
						result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_Start_End, "start_modified and end_created");
						return false;
					}
					if (dateTime2 > System.DateTime.Now)
					{
						result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_End_Now, "end_modified and currenttime");
						return false;
					}
				}
			}
			else if (!string.IsNullOrEmpty(parameters["end_modified"]))
			{
				System.DateTime dateTime2;
				System.DateTime.TryParse(parameters["end_modified"], out dateTime2);
				if (dateTime2 > System.DateTime.Now)
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Time_End_Now, "end_modified and currenttime");
					return false;
				}
			}
			if (!string.IsNullOrEmpty(parameters["order_by"]))
			{
				if (parameters["order_by"].Split(new char[]
				{
					':'
				}).Length != 2)
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Format, "order_by");
					return false;
				}
				string[] array = parameters["order_by"].Split(new char[]
				{
					':'
				});
				string text = DataHelper.CleanSearchString(array[0]);
				string a = DataHelper.CleanSearchString(array[1]);
				if (string.IsNullOrEmpty(text))
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Format, "order_by");
					return false;
				}
				if (text != "display_sequence" || text != "create_time" || text != "sold_quantity")
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Format, "order_by");
					return false;
				}
				if (a != "desc" || a != "asc")
				{
					result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Format, "order_by");
					return false;
				}
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
			return true;
		}
	}
}
