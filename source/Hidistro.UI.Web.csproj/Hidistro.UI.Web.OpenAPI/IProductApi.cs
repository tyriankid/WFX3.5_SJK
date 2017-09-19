using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web;

namespace Hidistro.UI.Web.OpenAPI
{
	public class IProductApi : System.Web.IHttpHandler
	{
		private SiteSettings site;

		private IProduct productApi = new ProductApi();

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
			this.site = SettingsManager.GetMasterSettings(true);
			string a;
			if ((a = text) != null)
			{
				if (a == "GetSoldProducts")
				{
					this.GetSoldProducts(sortedParams, ref s);
					goto IL_AE;
				}
				if (a == "GetProduct")
				{
					this.GetProduct(sortedParams, ref s);
					goto IL_AE;
				}
				if (a == "UpdateProductQuantity")
				{
					this.UpdateProductQuantity(sortedParams, ref s);
					goto IL_AE;
				}
				if (a == "UpdateProductApproveStatus")
				{
					this.UpdateProductApproveStatus(sortedParams, ref s);
					goto IL_AE;
				}
			}
			s = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Invalid_Method, "HIGW");
			IL_AE:
			context.Response.ContentType = "text/json";
			context.Response.Write(s);
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

		public void GetProduct(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			int num_iid = 0;
			if (!this.CheckProductParameters(parameters, out num_iid, out result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.productApi.GetProduct(num_iid);
		}

		public bool CheckProductParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out int num_iid, out string result)
		{
			num_iid = 0;
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (!int.TryParse(parameters["num_iid"], out num_iid))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "num_iid");
				return false;
			}
			return true;
		}

		public void UpdateProductQuantity(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			int num_iid = 0;
			int quantity = 0;
			int type = 1;
			if (!this.CheckUpdateQuantityParameters(parameters, out num_iid, out quantity, out type, out result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.productApi.UpdateProductQuantity(num_iid, parameters["sku_id"], quantity, type);
		}

		public bool CheckUpdateQuantityParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out int num_iid, out int quantity, out int type, out string result)
		{
			num_iid = 0;
			quantity = 0;
			type = 1;
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (!int.TryParse(parameters["num_iid"], out num_iid))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "num_iid");
				return false;
			}
			if (!int.TryParse(parameters["quantity"], out quantity))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "quantity");
				return false;
			}
			if (!int.TryParse(parameters["type"], out type))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "type");
				return false;
			}
			return true;
		}

		public void UpdateProductApproveStatus(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			int num_iid = 0;
			string empty = string.Empty;
			if (!this.CheckUpdateApproveStatusParameters(parameters, out num_iid, out empty, out result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.productApi.UpdateProductApproveStatus(num_iid, empty);
		}

		public bool CheckUpdateApproveStatusParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, out int num_iid, out string status, out string result)
		{
			num_iid = 0;
			status = string.Empty;
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (!int.TryParse(parameters["num_iid"], out num_iid))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Parameters_Format_Error, "num_iid");
				return false;
			}
			status = DataHelper.CleanSearchString(parameters["approve_status"]);
			if (status != "On_Sale" && status != "Un_Sale" && status != "In_Stock")
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_Status_is_Invalid, "approve_status");
				return false;
			}
			return true;
		}
	}
}
