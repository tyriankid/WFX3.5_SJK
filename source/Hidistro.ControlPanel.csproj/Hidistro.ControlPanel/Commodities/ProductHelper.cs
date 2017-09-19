using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Store;
using Hidistro.Messages;
using Hidistro.SqlDal.Commodities;
using Hishop.Open.Api;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Hidistro.ControlPanel.Commodities
{
	public static class ProductHelper
	{
		public static ProductActionStatus AddProduct(ProductInfo product, Dictionary<string, SKUItem> skus, Dictionary<int, IList<int>> attrs, IList<int> tagsId)
		{
			ProductActionStatus productActionStatu;
			if (null != product)
			{
				Globals.EntityCoding(product, true);
				int decimalLength = SettingsManager.GetMasterSettings(true).DecimalLength;
				if (product.MarketPrice.HasValue)
				{
					decimal? marketPrice = product.MarketPrice;
					product.MarketPrice = new decimal?(Math.Round(marketPrice.Value, decimalLength));
				}
				ProductActionStatus productActionStatu1 = ProductActionStatus.UnknowError;
				DbConnection dbConnection = DatabaseFactory.CreateDatabase().CreateConnection();
				try
				{
					dbConnection.Open();
					DbTransaction dbTransaction = dbConnection.BeginTransaction();
					try
					{
						try
						{
							ProductDao productDao = new ProductDao();
							int num = productDao.AddProduct(product, dbTransaction);
							if (num != 0)
							{
								product.ProductId = num;
								if ((skus == null ? false : skus.Count > 0))
								{
									if (!productDao.AddProductSKUs(num, skus, dbTransaction))
									{
										dbTransaction.Rollback();
										productActionStatu = ProductActionStatus.SKUError;
										return productActionStatu;
									}
								}
								if ((attrs == null ? false : attrs.Count > 0))
								{
									if (!productDao.AddProductAttributes(num, attrs, dbTransaction))
									{
										dbTransaction.Rollback();
										productActionStatu = ProductActionStatus.AttributeError;
										return productActionStatu;
									}
								}
								if ((tagsId == null ? false : tagsId.Count > 0))
								{
									if (!(new TagDao()).AddProductTags(num, tagsId, dbTransaction))
									{
										dbTransaction.Rollback();
										productActionStatu = ProductActionStatus.ProductTagEroor;
										return productActionStatu;
									}
								}
								dbTransaction.Commit();
								productActionStatu1 = ProductActionStatus.Success;
							}
							else
							{
								dbTransaction.Rollback();
								productActionStatu = ProductActionStatus.DuplicateSKU;
								return productActionStatu;
							}
						}
						catch (Exception exception)
						{
							dbTransaction.Rollback();
						}
					}
					finally
					{
						dbConnection.Close();
					}
				}
				finally
				{
					if (dbConnection != null)
					{
						((IDisposable)dbConnection).Dispose();
					}
				}
				if (productActionStatu1 == ProductActionStatus.Success)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] productName = new object[] { product.ProductName };
					EventLogs.WriteOperationLog(Privilege.AddProducts, string.Format(invariantCulture, "上架了一个新商品:”{0}”", productName));
				}
				productActionStatu = productActionStatu1;
			}
			else
			{
				productActionStatu = ProductActionStatus.UnknowError;
			}
			return productActionStatu;
		}

		public static string AddProductNew(ProductInfo product, Dictionary<string, SKUItem> skus, Dictionary<int, IList<int>> attrs, IList<int> tagsId)
		{
			string str;
			string empty = string.Empty;
			if (null != product)
			{
				Globals.EntityCoding(product, true);
				int decimalLength = SettingsManager.GetMasterSettings(true).DecimalLength;
				if (product.MarketPrice.HasValue)
				{
					decimal? marketPrice = product.MarketPrice;
					product.MarketPrice = new decimal?(Math.Round(marketPrice.Value, decimalLength));
				}
				ProductActionStatus productActionStatu = ProductActionStatus.UnknowError;
				DbConnection dbConnection = DatabaseFactory.CreateDatabase().CreateConnection();
				try
				{
					dbConnection.Open();
					DbTransaction dbTransaction = dbConnection.BeginTransaction();
					try
					{
						try
						{
							ProductDao productDao = new ProductDao();
							int num = productDao.AddProduct(product, dbTransaction);
							if (num != 0)
							{
								empty = num.ToString();
								product.ProductId = num;
								if ((skus == null ? false : skus.Count > 0))
								{
									if (!productDao.AddProductSKUs(num, skus, dbTransaction))
									{
										dbTransaction.Rollback();
										str = "添加SUK出错";
										return str;
									}
								}
								if ((attrs == null ? false : attrs.Count > 0))
								{
									if (!productDao.AddProductAttributes(num, attrs, dbTransaction))
									{
										dbTransaction.Rollback();
										str = "添加商品属性出错";
										return str;
									}
								}
								if ((tagsId == null ? false : tagsId.Count > 0))
								{
									if (!(new TagDao()).AddProductTags(num, tagsId, dbTransaction))
									{
										dbTransaction.Rollback();
										str = "添加商品标签出错";
										return str;
									}
								}
								dbTransaction.Commit();
								productActionStatu = ProductActionStatus.Success;
							}
							else
							{
								dbTransaction.Rollback();
								str = "货号重复";
								return str;
							}
						}
						catch (Exception exception)
						{
							dbTransaction.Rollback();
						}
					}
					finally
					{
						dbConnection.Close();
					}
				}
				finally
				{
					if (dbConnection != null)
					{
						((IDisposable)dbConnection).Dispose();
					}
				}
				if (productActionStatu == ProductActionStatus.Success)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] productName = new object[] { product.ProductName };
					EventLogs.WriteOperationLog(Privilege.AddProducts, string.Format(invariantCulture, "上架了一个新商品:”{0}”", productName));
				}
				str = empty;
			}
			else
			{
				str = "未知错误";
			}
			return str;
		}

		public static bool AddSkuStock(string productIds, int addStock)
		{
			return (new ProductBatchDao()).AddSkuStock(productIds, addStock);
		}

		public static bool CheckPrice(string productIds, int baseGradeId, decimal checkPrice, bool isMember)
		{
			return (new ProductBatchDao()).CheckPrice(productIds, baseGradeId, checkPrice, isMember);
		}

		private static ProductInfo ConverToProduct(DataRow productRow, int categoryId, int lineId, int? bandId, ProductSaleStatus saleStatus, bool includeImages)
		{
			string[] strArrays;
			ProductInfo productInfo = new ProductInfo()
			{
				CategoryId = categoryId,
				TypeId = new int?((int)productRow["SelectedTypeId"]),
				ProductName = (string)productRow["ProductName"],
				ProductCode = (string)productRow["ProductCode"],
				BrandId = bandId,
				Unit = (string)productRow["Unit"],
				ShortDescription = (string)productRow["ShortDescription"],
				Description = (string)productRow["Description"],
				AddedDate = DateTime.Now,
				SaleStatus = saleStatus,
				HasSKU = (bool)productRow["HasSKU"],
				MainCategoryPath = string.Concat(CatalogHelper.GetCategory(categoryId).Path, "|"),
				ImageUrl1 = (string)productRow["ImageUrl1"],
				ImageUrl2 = (string)productRow["ImageUrl2"],
				ImageUrl3 = (string)productRow["ImageUrl3"],
				ImageUrl4 = (string)productRow["ImageUrl4"],
				ImageUrl5 = (string)productRow["ImageUrl5"]
			};
			ProductInfo nullable = productInfo;
			if (productRow["MarketPrice"] != DBNull.Value)
			{
				nullable.MarketPrice = new decimal?((decimal)productRow["MarketPrice"]);
			}
			if (includeImages)
			{
				HttpContext current = HttpContext.Current;
				if ((string.IsNullOrEmpty(nullable.ImageUrl1) ? false : nullable.ImageUrl1.Length > 0))
				{
					strArrays = ProductHelper.ProcessImages(current, nullable.ImageUrl1);
					nullable.ThumbnailUrl40 = strArrays[0];
					nullable.ThumbnailUrl60 = strArrays[1];
					nullable.ThumbnailUrl100 = strArrays[2];
					nullable.ThumbnailUrl160 = strArrays[3];
					nullable.ThumbnailUrl180 = strArrays[4];
					nullable.ThumbnailUrl220 = strArrays[5];
					nullable.ThumbnailUrl310 = strArrays[6];
					nullable.ThumbnailUrl410 = strArrays[7];
				}
				if ((string.IsNullOrEmpty(nullable.ImageUrl2) ? false : nullable.ImageUrl2.Length > 0))
				{
					strArrays = ProductHelper.ProcessImages(current, nullable.ImageUrl2);
				}
				if ((string.IsNullOrEmpty(nullable.ImageUrl3) ? false : nullable.ImageUrl3.Length > 0))
				{
					strArrays = ProductHelper.ProcessImages(current, nullable.ImageUrl3);
				}
				if ((string.IsNullOrEmpty(nullable.ImageUrl4) ? false : nullable.ImageUrl4.Length > 0))
				{
					strArrays = ProductHelper.ProcessImages(current, nullable.ImageUrl4);
				}
				if ((string.IsNullOrEmpty(nullable.ImageUrl5) ? false : nullable.ImageUrl5.Length > 0))
				{
					strArrays = ProductHelper.ProcessImages(current, nullable.ImageUrl5);
				}
			}
			return nullable;
		}

		private static Dictionary<string, SKUItem> ConverToSkus(int mappedProductId, DataSet productData, bool includeCostPrice, bool includeStock)
		{
			Dictionary<string, SKUItem> strs;
			DataRow[] dataRowArray = productData.Tables["skus"].Select(string.Concat("ProductId=", mappedProductId.ToString(CultureInfo.InvariantCulture)));
			if ((int)dataRowArray.Length != 0)
			{
				Dictionary<string, SKUItem> strs1 = new Dictionary<string, SKUItem>();
				DataRow[] dataRowArray1 = dataRowArray;
				for (int i = 0; i < (int)dataRowArray1.Length; i++)
				{
					DataRow dataRow = dataRowArray1[i];
					string item = (string)dataRow["NewSkuId"];
					SKUItem sKUItem = new SKUItem()
					{
						SkuId = item,
						SKU = (string)dataRow["SKU"],
						SalePrice = (decimal)dataRow["SalePrice"]
					};
					SKUItem item1 = sKUItem;
					if (dataRow["Weight"] != DBNull.Value)
					{
						item1.Weight = (decimal)dataRow["Weight"];
					}
					if ((!includeCostPrice ? false : dataRow["CostPrice"] != DBNull.Value))
					{
						item1.CostPrice = (decimal)dataRow["CostPrice"];
					}
					if (includeStock)
					{
						item1.Stock = (int)dataRow["Stock"];
					}
					DataRow[] dataRowArray2 = productData.Tables["skuItems"].Select(string.Concat("NewSkuId='", item, "' AND MappedProductId=", mappedProductId.ToString(CultureInfo.InvariantCulture)));
					DataRow[] dataRowArray3 = dataRowArray2;
					for (int j = 0; j < (int)dataRowArray3.Length; j++)
					{
						DataRow dataRow1 = dataRowArray3[j];
						item1.SkuItems.Add((int)dataRow1["SelectedAttributeId"], (int)dataRow1["SelectedValueId"]);
					}
					strs1.Add(item, item1);
				}
				strs = strs1;
			}
			else
			{
				strs = null;
			}
			return strs;
		}

		private static Dictionary<int, IList<int>> ConvertToAttributes(int mappedProductId, DataSet productData)
		{
			Dictionary<int, IList<int>> nums;
			DataRow[] dataRowArray = productData.Tables["attributes"].Select(string.Concat("ProductId=", mappedProductId.ToString(CultureInfo.InvariantCulture)));
			if ((int)dataRowArray.Length != 0)
			{
				Dictionary<int, IList<int>> nums1 = new Dictionary<int, IList<int>>();
				DataRow[] dataRowArray1 = dataRowArray;
				for (int i = 0; i < (int)dataRowArray1.Length; i++)
				{
					DataRow dataRow = dataRowArray1[i];
					int item = (int)dataRow["SelectedAttributeId"];
					if (!nums1.ContainsKey(item))
					{
						nums1.Add(item, new List<int>());
					}
					nums1[item].Add((int)dataRow["SelectedValueId"]);
				}
				nums = nums1;
			}
			else
			{
				nums = null;
			}
			return nums;
		}

		public static int DeleteProduct(string productIds, bool isDeleteImage)
		{
			int num;
			ManagerHelper.CheckPrivilege(Privilege.DeleteProducts);
			if (!string.IsNullOrEmpty(productIds))
			{
				string[] strArrays = productIds.Split(new char[] { ',' });
				IList<int> nums = new List<int>();
				string[] strArrays1 = strArrays;
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
					nums.Add(int.Parse(strArrays1[i]));
				}
				IList<ProductInfo> products = (new ProductDao()).GetProducts(nums, false);
				int num1 = (new ProductDao()).DeleteProduct(productIds);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] count = new object[] { nums.Count };
					EventLogs.WriteOperationLog(Privilege.DeleteProducts, string.Format(invariantCulture, "删除了 “{0}” 件商品", count));
					if (isDeleteImage)
					{
						foreach (ProductInfo product in products)
						{
							try
							{
								ProductHelper.DeleteProductImage(product);
							}
							catch
							{
							}
						}
					}
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		private static void DeleteProductImage(ProductInfo product)
		{
			if (product != null)
			{
				if (!string.IsNullOrEmpty(product.ImageUrl1))
				{
					ResourcesHelper.DeleteImage(product.ImageUrl1);
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs40/40_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs60/60_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs100/100_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs160/160_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs180/180_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs220/220_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs310/310_"));
					ResourcesHelper.DeleteImage(product.ImageUrl1.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs410/410_"));
				}
				if (!string.IsNullOrEmpty(product.ImageUrl2))
				{
					ResourcesHelper.DeleteImage(product.ImageUrl2);
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs40/40_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs60/60_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs100/100_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs160/160_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs180/180_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs220/220_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs310/310_"));
					ResourcesHelper.DeleteImage(product.ImageUrl2.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs410/410_"));
				}
				if (!string.IsNullOrEmpty(product.ImageUrl3))
				{
					ResourcesHelper.DeleteImage(product.ImageUrl3);
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs40/40_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs60/60_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs100/100_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs160/160_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs180/180_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs220/220_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs310/310_"));
					ResourcesHelper.DeleteImage(product.ImageUrl3.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs410/410_"));
				}
				if (!string.IsNullOrEmpty(product.ImageUrl4))
				{
					ResourcesHelper.DeleteImage(product.ImageUrl4);
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs40/40_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs60/60_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs100/100_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs160/160_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs180/180_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs220/220_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs310/310_"));
					ResourcesHelper.DeleteImage(product.ImageUrl4.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs410/410_"));
				}
				if (!string.IsNullOrEmpty(product.ImageUrl5))
				{
					ResourcesHelper.DeleteImage(product.ImageUrl5);
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs40/40_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs60/60_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs100/100_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs160/160_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs180/180_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs220/220_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs310/310_"));
					ResourcesHelper.DeleteImage(product.ImageUrl5.Replace("/Storage/master/product/images/", "/Storage/master/product/thumbs410/410_"));
				}
			}
		}

		public static void EnsureMapping(DataSet mappingSet)
		{
			(new ProductDao()).EnsureMapping(mappingSet);
		}

		public static ProductInfo GetBrowseProductListByView(int productId)
		{
			return (new ProductDao()).GetBrowseProductListByView(productId);
		}

		public static DbQueryResult GetExportProducts(AdvancedProductQuery query, string removeProductIds)
		{
			return (new ProductDao()).GetExportProducts(query, removeProductIds);
		}

		public static DataSet GetExportProducts(AdvancedProductQuery query, bool includeCostPrice, bool includeStock, string removeProductIds)
		{
			DataSet exportProducts = (new ProductDao()).GetExportProducts(query, includeCostPrice, includeStock, removeProductIds);
			exportProducts.Tables[0].TableName = "types";
			exportProducts.Tables[1].TableName = "attributes";
			exportProducts.Tables[2].TableName = "values";
			exportProducts.Tables[3].TableName = "products";
			exportProducts.Tables[4].TableName = "skus";
			exportProducts.Tables[5].TableName = "skuItems";
			exportProducts.Tables[6].TableName = "productAttributes";
			exportProducts.Tables[7].TableName = "taobaosku";
			string empty = string.Empty;
			for (int i = 0; i < exportProducts.Tables[3].Rows.Count; i++)
			{
				empty = (exportProducts.Tables[3].Rows[i]["Description"] == null ? "" : exportProducts.Tables[3].Rows[i]["Description"].ToString());
				if (!string.IsNullOrEmpty(empty))
				{
					exportProducts.Tables[3].Rows[i]["Description"] = Regex.Replace(empty, "alt=\"([^\"]+)\"", "");
				}
			}
			return exportProducts;
		}

		public static DataTable GetGroupBuyProducts(ProductQuery query)
		{
			return (new ProductDao()).GetGroupBuyProducts(query);
		}

		public static int GetMaxSequence()
		{
			return (new ProductDao()).GetMaxSequence();
		}

		public static DataTable GetProductBaseInfo(string productIds)
		{
			return (new ProductBatchDao()).GetProductBaseInfo(productIds);
		}

		public static ProductInfo GetProductBaseInfo(int productId)
		{
			return (new ProductBatchDao()).GetProductBaseInfo(productId);
		}

		public static ProductInfo GetProductDetails(int productId, out Dictionary<int, IList<int>> attrs, out IList<int> tagsId)
		{
			ProductDao productDao = new ProductDao();
			attrs = productDao.GetProductAttributes(productId);
			tagsId = productDao.GetProductTags(productId);
			return productDao.GetProductDetails(productId);
		}

		public static ProductInfo GetProductDetails(int productId)
		{
			return (new ProductDao()).GetProductDetails(productId);
		}

		public static product_item_model GetProductForApi(int productId)
		{
			return (new ProductDao()).GetProductForApi(productId);
		}

		public static bool GetProductHasSku(string skuid, int quantity)
		{
			return (new ProductDao()).GetProductHasSku(skuid, quantity);
		}

		public static IList<int> GetProductIds(ProductQuery query)
		{
			return (new ProductDao()).GetProductIds(query);
		}

		public static DataTable GetProductNum()
		{
			return (new ProductDao()).GetProductNum();
		}

		public static DbQueryResult GetProducts(ProductQuery query)
		{
			return (new ProductDao()).GetProducts(query);
		}

		public static DataTable GetProducts(string products)
		{
			return (new ProductDao()).GetProducts(products);
		}

		public static IList<ProductInfo> GetProducts(IList<int> productIds, bool Resort = false)
		{
			return (new ProductDao()).GetProducts(productIds, Resort);
		}

		public static decimal GetProductSalePrice(int productId)
		{
			return (new ProductDao()).GetProductSalePrice(productId);
		}

		public static int GetProductsCount()
		{
			return (new ProductDao()).GetProductsCount();
		}

		public static int GetProductsCountByDistributor(int rid)
		{
			return (new ProductDao()).GetProductsCountByDistributor(rid);
		}

		public static DbQueryResult GetProductsForApi(ProductQuery query)
		{
			return (new ProductDao()).GetProductsForApi(query);
		}

		public static DbQueryResult GetProductsFromGroup(ProductQuery query, string productIds)
		{
			return (new ProductDao()).GetProductsFromGroup(query, productIds);
		}

		public static DbQueryResult GetProductsImgList(ProductQuery query)
		{
			return (new ProductDao()).GetProductsImgList(query);
		}

		public static long GetProductSumStock(int productId)
		{
			return (new ProductDao()).GetProductSumStock(productId);
		}

		public static string GetPropsForApi(int productId)
		{
			return (new ProductDao()).GetPropsForApi(productId);
		}

		public static bool GetSKUMemberPrice(string productIds, int gradeId)
		{
			return (new ProductBatchDao()).GetSKUMemberPrice(productIds, gradeId);
		}

		public static DataTable GetSkuMemberPrices(string productIds)
		{
			return (new ProductBatchDao()).GetSkuMemberPrices(productIds);
		}

		public static IList<product_sku_model> GetSkusForApi(int productId)
		{
			return (new ProductDao()).GetSkusForApi(productId);
		}

		public static DataTable GetSkuStocks(string productIds)
		{
			return (new ProductBatchDao()).GetSkuStocks(productIds);
		}

		public static DataSet GetTaobaoProductDetails(int productId)
		{
			return (new TaobaoProductDao()).GetTaobaoProductDetails(productId);
		}

		public static DataTable GetTopProductOrder(int top, string showOrder)
		{
			if (top < 1)
			{
				top = 6;
			}
			if (string.IsNullOrEmpty(showOrder))
			{
				showOrder = " ProductId DESC";
			}
			return (new ProductDao()).GetTopProductOrder(top, showOrder);
		}

		public static void ImportProducts(DataTable productData, int categoryId, int lineId, int? brandId, ProductSaleStatus saleStatus, bool isImportFromTaobao)
		{
			string[] strArrays;
			if ((productData == null ? false : productData.Rows.Count > 0))
			{
				foreach (DataRow row in productData.Rows)
				{
					ProductInfo productInfo = new ProductInfo()
					{
						CategoryId = categoryId,
						MainCategoryPath = string.Concat(CatalogHelper.GetCategory(categoryId).Path, "|"),
						ProductName = (string)row["ProductName"],
						ProductCode = (string)row["SKU"],
						BrandId = brandId
					};
					if (row["Description"] != DBNull.Value)
					{
						productInfo.Description = (string)row["Description"];
					}
					productInfo.MarketPrice = new decimal?((decimal)row["SalePrice"]);
					productInfo.AddedDate = DateTime.Now;
					productInfo.SaleStatus = saleStatus;
					productInfo.HasSKU = false;
					HttpContext current = HttpContext.Current;
					if (row["ImageUrl1"] != DBNull.Value)
					{
						productInfo.ImageUrl1 = (string)row["ImageUrl1"];
					}
					if ((string.IsNullOrEmpty(productInfo.ImageUrl1) ? false : productInfo.ImageUrl1.Length > 0))
					{
						strArrays = ProductHelper.ProcessImages(current, productInfo.ImageUrl1);
						productInfo.ThumbnailUrl40 = strArrays[0];
						productInfo.ThumbnailUrl60 = strArrays[1];
						productInfo.ThumbnailUrl100 = strArrays[2];
						productInfo.ThumbnailUrl160 = strArrays[3];
						productInfo.ThumbnailUrl180 = strArrays[4];
						productInfo.ThumbnailUrl220 = strArrays[5];
						productInfo.ThumbnailUrl310 = strArrays[6];
						productInfo.ThumbnailUrl410 = strArrays[7];
					}
					if (row["ImageUrl2"] != DBNull.Value)
					{
						productInfo.ImageUrl2 = (string)row["ImageUrl2"];
					}
					if ((string.IsNullOrEmpty(productInfo.ImageUrl2) ? false : productInfo.ImageUrl2.Length > 0))
					{
						strArrays = ProductHelper.ProcessImages(current, productInfo.ImageUrl2);
					}
					if (row["ImageUrl3"] != DBNull.Value)
					{
						productInfo.ImageUrl3 = (string)row["ImageUrl3"];
					}
					if ((string.IsNullOrEmpty(productInfo.ImageUrl3) ? false : productInfo.ImageUrl3.Length > 0))
					{
						strArrays = ProductHelper.ProcessImages(current, productInfo.ImageUrl3);
					}
					if (row["ImageUrl4"] != DBNull.Value)
					{
						productInfo.ImageUrl4 = (string)row["ImageUrl4"];
					}
					if ((string.IsNullOrEmpty(productInfo.ImageUrl4) ? false : productInfo.ImageUrl4.Length > 0))
					{
						strArrays = ProductHelper.ProcessImages(current, productInfo.ImageUrl4);
					}
					if (row["ImageUrl5"] != DBNull.Value)
					{
						productInfo.ImageUrl5 = (string)row["ImageUrl5"];
					}
					if ((string.IsNullOrEmpty(productInfo.ImageUrl5) ? false : productInfo.ImageUrl5.Length > 0))
					{
						strArrays = ProductHelper.ProcessImages(current, productInfo.ImageUrl5);
					}
					SKUItem sKUItem = new SKUItem()
					{
						SkuId = "0",
						SKU = (string)row["SKU"]
					};
					if (row["Stock"] != DBNull.Value)
					{
						sKUItem.Stock = (int)row["Stock"];
					}
					if (row["Weight"] != DBNull.Value)
					{
						sKUItem.Weight = (decimal)row["Weight"];
					}
					sKUItem.SalePrice = (decimal)row["SalePrice"];
					Dictionary<string, SKUItem> strs = new Dictionary<string, SKUItem>()
					{
						{ sKUItem.SkuId, sKUItem }
					};
					ProductActionStatus productActionStatu = ProductHelper.AddProduct(productInfo, strs, null, null);
					ProductDao productDao = new ProductDao();
					if (productActionStatu == ProductActionStatus.Success)
					{
						productDao.AddProductMinPriceAndMaxPrice(productInfo.ProductId);
					}
					if ((!isImportFromTaobao ? false : productActionStatu == ProductActionStatus.Success))
					{
						TaobaoProductInfo taobaoProductInfo = new TaobaoProductInfo()
						{
							ProductId = productInfo.ProductId,
							ProTitle = productInfo.ProductName,
							Cid = (long)row["Cid"]
						};
						if (row["StuffStatus"] != DBNull.Value)
						{
							taobaoProductInfo.StuffStatus = (string)row["StuffStatus"];
						}
						taobaoProductInfo.Num = (long)row["Num"];
						taobaoProductInfo.LocationState = (string)row["LocationState"];
						taobaoProductInfo.LocationCity = (string)row["LocationCity"];
						taobaoProductInfo.FreightPayer = (string)row["FreightPayer"];
						if (row["PostFee"] != DBNull.Value)
						{
							taobaoProductInfo.PostFee = (decimal)row["PostFee"];
						}
						if (row["ExpressFee"] != DBNull.Value)
						{
							taobaoProductInfo.ExpressFee = (decimal)row["ExpressFee"];
						}
						if (row["EMSFee"] != DBNull.Value)
						{
							taobaoProductInfo.EMSFee = (decimal)row["EMSFee"];
						}
						taobaoProductInfo.HasInvoice = (bool)row["HasInvoice"];
						taobaoProductInfo.HasWarranty = (bool)row["HasWarranty"];
						taobaoProductInfo.HasDiscount = (bool)row["HasDiscount"];
						taobaoProductInfo.ValidThru = (long)row["ValidThru"];
						if (row["ListTime"] == DBNull.Value)
						{
							taobaoProductInfo.ListTime = DateTime.Now;
						}
						else
						{
							taobaoProductInfo.ListTime = (DateTime)row["ListTime"];
						}
						if (row["PropertyAlias"] != DBNull.Value)
						{
							taobaoProductInfo.PropertyAlias = (string)row["PropertyAlias"];
						}
						if (row["InputPids"] != DBNull.Value)
						{
							taobaoProductInfo.InputPids = (string)row["InputPids"];
						}
						if (row["InputStr"] != DBNull.Value)
						{
							taobaoProductInfo.InputStr = (string)row["InputStr"];
						}
						if (row["SkuProperties"] != DBNull.Value)
						{
							taobaoProductInfo.SkuProperties = (string)row["SkuProperties"];
						}
						if (row["SkuQuantities"] != DBNull.Value)
						{
							taobaoProductInfo.SkuQuantities = (string)row["SkuQuantities"];
						}
						if (row["SkuPrices"] != DBNull.Value)
						{
							taobaoProductInfo.SkuPrices = (string)row["SkuPrices"];
						}
						if (row["SkuOuterIds"] != DBNull.Value)
						{
							taobaoProductInfo.SkuOuterIds = (string)row["SkuOuterIds"];
						}
						ProductHelper.UpdateToaobProduct(taobaoProductInfo);
					}
				}
			}
		}

		public static void ImportProducts(DataSet productData, int categoryId, int lineId, int? bandId, ProductSaleStatus saleStatus, bool includeCostPrice, bool includeStock, bool includeImages)
		{
			foreach (DataRow row in productData.Tables["products"].Rows)
			{
				int item = (int)row["ProductId"];
				ProductInfo product = ProductHelper.ConverToProduct(row, categoryId, lineId, bandId, saleStatus, includeImages);
				Dictionary<string, SKUItem> skus = ProductHelper.ConverToSkus(item, productData, includeCostPrice, includeStock);
				Dictionary<int, IList<int>> attributes = ProductHelper.ConvertToAttributes(item, productData);
				ProductHelper.AddProduct(product, skus, attributes, null);
			}
		}

		public static int InStock(string productIds)
		{
			int num;
			ManagerHelper.CheckPrivilege(Privilege.InStockProduct);
			if (!string.IsNullOrEmpty(productIds))
			{
				int num1 = (new ProductDao()).UpdateProductSaleStatus(productIds, ProductSaleStatus.OnStock);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] objArray = new object[] { num1 };
					EventLogs.WriteOperationLog(Privilege.OffShelfProducts, string.Format(invariantCulture, "批量入库了 “{0}” 件商品", objArray));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static bool IsExitTaobaoProduct(long taobaoProductId)
		{
			return (new TaobaoProductDao()).IsExitTaobaoProduct(taobaoProductId);
		}

		public static int OffShelf(string productIds)
		{
			int num;
			ManagerHelper.CheckPrivilege(Privilege.OffShelfProducts);
			if (!string.IsNullOrEmpty(productIds))
			{
				int num1 = (new ProductDao()).UpdateProductSaleStatus(productIds, ProductSaleStatus.UnSale);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] objArray = new object[] { num1 };
					EventLogs.WriteOperationLog(Privilege.OffShelfProducts, string.Format(invariantCulture, "批量下架了 “{0}” 件商品", objArray));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		private static string[] ProcessImages(HttpContext context, string originalSavePath)
		{
			string fileName = Path.GetFileName(originalSavePath);
			string str = string.Concat("/Storage/master/product/thumbs40/40_", fileName);
			string str1 = string.Concat("/Storage/master/product/thumbs60/60_", fileName);
			string str2 = string.Concat("/Storage/master/product/thumbs100/100_", fileName);
			string str3 = string.Concat("/Storage/master/product/thumbs160/160_", fileName);
			string str4 = string.Concat("/Storage/master/product/thumbs180/180_", fileName);
			string str5 = string.Concat("/Storage/master/product/thumbs220/220_", fileName);
			string str6 = string.Concat("/Storage/master/product/thumbs310/310_", fileName);
			string str7 = string.Concat("/Storage/master/product/thumbs410/410_", fileName);
			string str8 = context.Request.MapPath(string.Concat(Globals.ApplicationPath, originalSavePath));
			if (File.Exists(str8))
			{
				try
				{
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str)), 40, 40);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str1)), 60, 60);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str2)), 100, 100);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str3)), 160, 160);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str4)), 180, 180);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str5)), 220, 220);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str6)), 310, 310);
					ResourcesHelper.CreateThumbnail(str8, context.Request.MapPath(string.Concat(Globals.ApplicationPath, str7)), 410, 410);
				}
				catch
				{
				}
			}
			string[] strArrays = new string[] { str, str1, str2, str3, str4, str5, str6, str7 };
			return strArrays;
		}

		public static int RemoveProduct(string productIds)
		{
			int num;
			ManagerHelper.CheckPrivilege(Privilege.DeleteProducts);
			if (!string.IsNullOrEmpty(productIds))
			{
				int num1 = (new ProductDao()).UpdateProductSaleStatus(productIds, ProductSaleStatus.Delete);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] objArray = new object[] { num1 };
					EventLogs.WriteOperationLog(Privilege.OffShelfProducts, string.Format(invariantCulture, "批量删除了 “{0}” 件商品到回收站", objArray));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static bool ReplaceProductNames(string productIds, string oldWord, string newWord)
		{
			return (new ProductBatchDao()).ReplaceProductNames(productIds, oldWord, newWord);
		}

		public static void SendWXMessage_AddNewProduct(ProductInfo product)
		{
			bool flag;
			ProductHelper.WorkDelegate workDelegate = new ProductHelper.WorkDelegate(new ProductHelper.AsyncWorkDelegate().CalData);
			workDelegate.BeginInvoke(product, out flag, null, null);
		}

		public static int SetFreeShip(string productIds, bool isFree)
		{
			int num;
			if (!string.IsNullOrEmpty(productIds))
			{
				int num1 = (new ProductDao()).UpdateProductShipFree(productIds, isFree);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] objArray = new object[] { (isFree ? "设置" : "取消"), num1 };
					EventLogs.WriteOperationLog(Privilege.OffShelfProducts, string.Format(invariantCulture, "{0}了“{1}” 件商品包邮", objArray));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static ProductActionStatus UpdateProduct(ProductInfo product, Dictionary<string, SKUItem> skus, Dictionary<int, IList<int>> attrs, IList<int> tagIds)
		{
			ProductActionStatus productActionStatu;
			if (null != product)
			{
				Globals.EntityCoding(product, true);
				int decimalLength = SettingsManager.GetMasterSettings(true).DecimalLength;
				if (product.MarketPrice.HasValue)
				{
					decimal? marketPrice = product.MarketPrice;
					product.MarketPrice = new decimal?(Math.Round(marketPrice.Value, decimalLength));
				}
				ProductActionStatus productActionStatu1 = ProductActionStatus.UnknowError;
				DbConnection dbConnection = DatabaseFactory.CreateDatabase().CreateConnection();
				try
				{
					dbConnection.Open();
					DbTransaction dbTransaction = dbConnection.BeginTransaction();
					try
					{
						try
						{
							ProductDao productDao = new ProductDao();
							if (!productDao.UpdateProduct(product, dbTransaction))
							{
								dbTransaction.Rollback();
								productActionStatu = ProductActionStatus.DuplicateSKU;
								return productActionStatu;
							}
							else if (productDao.DeleteProductSKUS(product.ProductId, dbTransaction))
							{
								if ((skus == null ? false : skus.Count > 0))
								{
									if (!productDao.AddProductSKUs(product.ProductId, skus, dbTransaction))
									{
										dbTransaction.Rollback();
										productActionStatu = ProductActionStatus.SKUError;
										return productActionStatu;
									}
								}
								if (!productDao.AddProductAttributes(product.ProductId, attrs, dbTransaction))
								{
									dbTransaction.Rollback();
									productActionStatu = ProductActionStatus.AttributeError;
									return productActionStatu;
								}
								else if ((new TagDao()).DeleteProductTags(product.ProductId, dbTransaction))
								{
									if (tagIds.Count > 0)
									{
										if (!(new TagDao()).AddProductTags(product.ProductId, tagIds, dbTransaction))
										{
											dbTransaction.Rollback();
											productActionStatu = ProductActionStatus.ProductTagEroor;
											return productActionStatu;
										}
									}
									dbTransaction.Commit();
									productActionStatu1 = ProductActionStatus.Success;
								}
								else
								{
									dbTransaction.Rollback();
									productActionStatu = ProductActionStatus.ProductTagEroor;
									return productActionStatu;
								}
							}
							else
							{
								dbTransaction.Rollback();
								productActionStatu = ProductActionStatus.SKUError;
								return productActionStatu;
							}
						}
						catch (Exception exception)
						{
							dbTransaction.Rollback();
						}
					}
					finally
					{
						dbConnection.Close();
					}
				}
				finally
				{
					if (dbConnection != null)
					{
						((IDisposable)dbConnection).Dispose();
					}
				}
				if (productActionStatu1 == ProductActionStatus.Success)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] productId = new object[] { product.ProductId };
					EventLogs.WriteOperationLog(Privilege.EditProducts, string.Format(invariantCulture, "修改了编号为 “{0}” 的商品", productId));
				}
				productActionStatu = productActionStatu1;
			}
			else
			{
				productActionStatu = ProductActionStatus.UnknowError;
			}
			return productActionStatu;
		}

		public static int UpdateProductApproveStatusForApi(int num_iid, string approve_status)
		{
			return (new ProductDao()).UpdateProductApproveStatusForApi(num_iid, approve_status);
		}

		public static bool UpdateProductBaseInfo(DataTable dt)
		{
			bool flag;
			flag = ((dt == null ? false : dt.Rows.Count > 0) ? (new ProductBatchDao()).UpdateProductBaseInfo(dt) : false);
			return flag;
		}

		public static bool UpdateProductCategory(int productId, int newCategoryId)
		{
			bool flag;
			flag = (newCategoryId == 0 ? (new ProductDao()).UpdateProductCategory(productId, newCategoryId, null) : (new ProductDao()).UpdateProductCategory(productId, newCategoryId, string.Concat(CatalogHelper.GetCategory(newCategoryId).Path, "|")));
			if (flag)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { productId, newCategoryId };
				EventLogs.WriteOperationLog(Privilege.EditProducts, string.Format(invariantCulture, "修改编号 “{0}” 的店铺分类为 “{1}”", objArray));
			}
			return flag;
		}

		public static string UpdateProductContent(int productid, string content)
		{
			return (new ProductDao()).UpdateProductContent(productid, content);
		}

		public static int UpdateProductFreightTemplate(string productIds, int FreightTemplateId)
		{
			int num;
			if (!string.IsNullOrEmpty(productIds))
			{
				int num1 = (new ProductDao()).UpdateProductFreightTemplate(productIds, FreightTemplateId);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] objArray = new object[] { "设置", num1 };
					EventLogs.WriteOperationLog(Privilege.OffShelfProducts, string.Format(invariantCulture, "{0}了“{1}” 件商品运费", objArray));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static bool UpdateProductNames(string productIds, string prefix, string suffix)
		{
			return (new ProductBatchDao()).UpdateProductNames(productIds, prefix, suffix);
		}

		public static int UpdateProductQuantityForApi(int num_iid, string sku_id, int quantity, int type)
		{
			return (new ProductDao()).UpdateProductQuantityForApi(num_iid, sku_id, quantity, type);
		}

		public static void UpdateShoppingCartsTemplateId(int productId, int templateId)
		{
			(new ProductDao()).UpdateShoppingCartsTemplateId(productId, templateId);
		}

		public static bool UpdateShowSaleCounts(string productIds, int showSaleCounts)
		{
			return (new ProductBatchDao()).UpdateShowSaleCounts(productIds, showSaleCounts);
		}

		public static bool UpdateShowSaleCounts(string productIds, int showSaleCounts, string operation)
		{
			return (new ProductBatchDao()).UpdateShowSaleCounts(productIds, showSaleCounts, operation);
		}

		public static bool UpdateShowSaleCounts(DataTable dt)
		{
			bool flag;
			flag = ((dt == null ? false : dt.Rows.Count > 0) ? (new ProductBatchDao()).UpdateShowSaleCounts(dt) : false);
			return flag;
		}

		public static bool UpdateSkuMemberPrices(string productIds, int gradeId, decimal price)
		{
			return (new ProductBatchDao()).UpdateSkuMemberPrices(productIds, gradeId, price);
		}

		public static bool UpdateSkuMemberPrices(string productIds, int gradeId, int baseGradeId, string operation, decimal price)
		{
			bool flag = (new ProductBatchDao()).UpdateSkuMemberPrices(productIds, gradeId, baseGradeId, operation, price);
			return flag;
		}

		public static bool UpdateSkuMemberPrices(DataSet ds)
		{
			return (new ProductBatchDao()).UpdateSkuMemberPrices(ds);
		}

		public static bool UpdateSkuStock(string productIds, int stock)
		{
			return (new ProductBatchDao()).UpdateSkuStock(productIds, stock);
		}

		public static bool UpdateSkuStock(Dictionary<string, int> skuStocks)
		{
			return (new ProductBatchDao()).UpdateSkuStock(skuStocks);
		}

		public static bool UpdateToaobProduct(TaobaoProductInfo taobaoProduct)
		{
			return (new TaobaoProductDao()).UpdateToaobProduct(taobaoProduct);
		}

		public static int UpShelf(string productIds)
		{
			int num;
			ManagerHelper.CheckPrivilege(Privilege.UpShelfProducts);
			if (!string.IsNullOrEmpty(productIds))
			{
				int num1 = (new ProductDao()).UpdateProductSaleStatus(productIds, ProductSaleStatus.OnSale);
				if (num1 > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] objArray = new object[] { num1 };
					EventLogs.WriteOperationLog(Privilege.UpShelfProducts, string.Format(invariantCulture, "批量上架了 “{0}” 件商品", objArray));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public class AsyncWorkDelegate
		{
			public AsyncWorkDelegate()
			{
			}

			public void CalData(ProductInfo product, out bool result)
			{
				result = false;
				try
				{
					Messenger.SendWeiXinMsg_ProductCreate(product.ProductName, product.MinSalePrice);
				}
				catch (Exception exception)
				{
				}
			}
		}

		public delegate void WorkDelegate(ProductInfo product, out bool result);
	}
}