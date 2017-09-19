using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiIProduct.UpdateProductApproveStatus
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private IProduct productApi = new ProductApi();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.UpdateProductApproveStatus(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
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
