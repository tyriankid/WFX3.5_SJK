using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiIProduct.GetProduct
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private IProduct productApi = new ProductApi();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.GetProduct(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			base.Response.Write(empty);
			base.Response.End();
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
	}
}
