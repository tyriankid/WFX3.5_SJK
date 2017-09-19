using Hidistro.ControlPanel.Commodities;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities;
using Hidistro.Entities.Commodities;
using Hishop.Open.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Hidistro.UI.Web.OpenAPI.Impl
{
	public class ProductApi : IProduct
	{
		public string GetProduct(int num_iid)
		{
			product_item_model productForApi = ProductHelper.GetProductForApi(num_iid);
			if (productForApi == null)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_Not_Exists, "num_iid");
			}
			productForApi.props_name = ProductHelper.GetPropsForApi(num_iid);
			productForApi.skus = ProductHelper.GetSkusForApi(num_iid);
			string format = "{{\"product_get_response\":{{\"item\":{0}}}}}";
			return string.Format(format, JsonConvert.SerializeObject(productForApi));
		}

		public string GetSoldProducts(System.DateTime? start_modified, System.DateTime? end_modified, string approve_status, string q, string order_by, int page_no, int page_size)
		{
			string format = "{{\"products_get_response\":{{\"total_results\":\"{0}\",\"items\":{1}}}}}";
			ProductQuery productQuery = new ProductQuery
			{
				SortBy = "DisplaySequence",
				SortOrder = SortAction.Asc,
				PageIndex = 1,
				PageSize = 40,
				SaleStatus = ProductSaleStatus.All
			};
			if (start_modified.HasValue)
			{
				productQuery.StartDate = start_modified;
			}
			if (end_modified.HasValue)
			{
				productQuery.EndDate = end_modified;
			}
			if (!string.IsNullOrEmpty(q))
			{
				productQuery.Keywords = DataHelper.CleanSearchString(q);
			}
			if (!string.IsNullOrEmpty(approve_status))
			{
				ProductSaleStatus saleStatus = ProductSaleStatus.All;
				EnumDescription.GetEnumValue<ProductSaleStatus>(approve_status, ref saleStatus);
				productQuery.SaleStatus = saleStatus;
			}
			DbQueryResult productsForApi = ProductHelper.GetProductsForApi(productQuery);
			return string.Format(format, productsForApi.TotalRecords, this.ConvertProductSold((System.Data.DataTable)productsForApi.Data));
		}

		public string ConvertProductSold(System.Data.DataTable dt)
		{
			System.Collections.Generic.List<product_list_model> list = new System.Collections.Generic.List<product_list_model>();
			foreach (System.Data.DataRow dataRow in dt.Rows)
			{
				product_list_model product_list_model = new product_list_model();
				product_list_model.cid = (int)dataRow["CategoryId"];
				if (dataRow["CategoryName"] != System.DBNull.Value)
				{
					product_list_model.cat_name = (string)dataRow["CategoryName"];
				}
				if (dataRow["BrandId"] != System.DBNull.Value)
				{
					product_list_model.brand_id = (int)dataRow["BrandId"];
				}
				if (dataRow["BrandName"] != System.DBNull.Value)
				{
					product_list_model.brand_name = (string)dataRow["BrandName"];
				}
				if (dataRow["TypeId"] != System.DBNull.Value)
				{
					product_list_model.type_id = (int)dataRow["TypeId"];
				}
				if (dataRow["TypeName"] != System.DBNull.Value)
				{
					product_list_model.type_name = (string)dataRow["TypeName"];
				}
				product_list_model.num_iid = (int)dataRow["ProductId"];
				product_list_model.title = (string)dataRow["ProductName"];
				if (dataRow["ProductCode"] != System.DBNull.Value)
				{
					product_list_model.outer_id = (string)dataRow["ProductCode"];
				}
				if (dataRow["ImageUrl1"] != System.DBNull.Value && !string.IsNullOrEmpty((string)dataRow["ImageUrl1"]))
				{
					product_list_model.pic_url.Add((string)dataRow["ImageUrl1"]);
				}
				if (dataRow["ImageUrl2"] != System.DBNull.Value && !string.IsNullOrEmpty((string)dataRow["ImageUrl2"]))
				{
					product_list_model.pic_url.Add((string)dataRow["ImageUrl2"]);
				}
				if (dataRow["ImageUrl3"] != System.DBNull.Value && !string.IsNullOrEmpty((string)dataRow["ImageUrl3"]))
				{
					product_list_model.pic_url.Add((string)dataRow["ImageUrl3"]);
				}
				if (dataRow["ImageUrl4"] != System.DBNull.Value && !string.IsNullOrEmpty((string)dataRow["ImageUrl4"]))
				{
					product_list_model.pic_url.Add((string)dataRow["ImageUrl4"]);
				}
				if (dataRow["ImageUrl5"] != System.DBNull.Value && !string.IsNullOrEmpty((string)dataRow["ImageUrl5"]))
				{
					product_list_model.pic_url.Add((string)dataRow["ImageUrl5"]);
				}
				product_list_model.list_time = new System.DateTime?((System.DateTime)dataRow["AddedDate"]);
				ProductSaleStatus productSaleStatus = (ProductSaleStatus)dataRow["SaleStatus"];
				if (productSaleStatus == ProductSaleStatus.OnSale)
				{
					product_list_model.approve_status = "On_Sale";
				}
				else if (productSaleStatus == ProductSaleStatus.UnSale)
				{
					product_list_model.approve_status = "Un_Sale";
				}
				else
				{
					product_list_model.approve_status = "In_Stock";
				}
				product_list_model.sold_quantity = (int)dataRow["SaleCounts"];
				product_list_model.num = (int)dataRow["Stock"];
				product_list_model.price = (decimal)dataRow["SalePrice"];
				list.Add(product_list_model);
			}
			return JsonConvert.SerializeObject(list);
		}

		public string UpdateProductQuantity(int num_iid, string sku_id, int quantity, int type)
		{
			product_item_model productForApi = ProductHelper.GetProductForApi(num_iid);
			if (productForApi == null)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_Not_Exists, "num_iid");
			}
			if (ProductHelper.UpdateProductQuantityForApi(num_iid, sku_id, quantity, type) <= 0)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_UpdateeQuantity_Faild, "update_quantity");
			}
			productForApi.props_name = ProductHelper.GetPropsForApi(num_iid);
			productForApi.skus = ProductHelper.GetSkusForApi(num_iid);
			string format = "{{\"product_get_response\":{{\"item\":{0}}}}}";
			return string.Format(format, JsonConvert.SerializeObject(productForApi));
		}

		public string UpdateProductApproveStatus(int num_iid, string approve_status)
		{
			product_item_model productForApi = ProductHelper.GetProductForApi(num_iid);
			if (productForApi == null)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_Not_Exists, "num_iid");
			}
			if (ProductHelper.UpdateProductApproveStatusForApi(num_iid, approve_status) <= 0)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Product_ApproveStatus_Faild, "update_approve_status");
			}
			productForApi.props_name = ProductHelper.GetPropsForApi(num_iid);
			productForApi.skus = ProductHelper.GetSkusForApi(num_iid);
			productForApi.approve_status = approve_status;
			string format = "{{\"product_get_response\":{{\"item\":{0}}}}}";
			return string.Format(format, JsonConvert.SerializeObject(productForApi));
		}
	}
}
