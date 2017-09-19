using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiIProduct.UpdateProductQuantity
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private IProduct productApi = new ProductApi();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.UpdateProductQuantity(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
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
	}
}
