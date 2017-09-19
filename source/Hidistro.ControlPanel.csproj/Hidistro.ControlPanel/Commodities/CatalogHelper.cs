using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Sales;
using Hidistro.Entities.Store;
using Hidistro.SqlDal.Commodities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Web.Caching;

namespace Hidistro.ControlPanel.Commodities
{
	public sealed class CatalogHelper
	{
		private const string CategoriesCachekey = "DataCache-Categories";

		private CatalogHelper()
		{
		}

		public static bool AddBrandCategory(BrandCategoryInfo brandCategory)
		{
			bool flag;
			int num = (new BrandCategoryDao()).AddBrandCategory(brandCategory);
			if (num > 0)
			{
				if (brandCategory.ProductTypes.Count > 0)
				{
					(new BrandCategoryDao()).AddBrandProductTypes(num, brandCategory.ProductTypes);
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static CategoryActionStatus AddCategory(CategoryInfo category)
		{
			CategoryActionStatus categoryActionStatu;
			if (null != category)
			{
				Globals.EntityCoding(category, true);
				if ((new CategoryDao()).CreateCategory(category) > 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] name = new object[] { category.Name };
					EventLogs.WriteOperationLog(Privilege.AddProductCategory, string.Format(invariantCulture, "创建了一个新的店铺分类:”{0}”", name));
					HiCache.Remove("DataCache-Categories");
				}
				categoryActionStatu = CategoryActionStatus.Success;
			}
			else
			{
				categoryActionStatu = CategoryActionStatus.UnknowError;
			}
			return categoryActionStatu;
		}

		public static bool AddProductTags(int productId, IList<int> tagsId, DbTransaction dbtran)
		{
			return (new TagDao()).AddProductTags(productId, tagsId, dbtran);
		}

		public static int AddTags(string tagName)
		{
			int num = 0;
			if ((new TagDao()).GetTags(tagName) <= 0)
			{
				num = (new TagDao()).AddTags(tagName);
			}
			return num;
		}

		public static bool BrandHvaeProducts(int brandId)
		{
			return (new BrandCategoryDao()).BrandHvaeProducts(brandId);
		}

		public static bool DeleteBrandCategory(int brandId)
		{
			return (new BrandCategoryDao()).DeleteBrandCategory(brandId);
		}

		public static bool DeleteCategory(int categoryId)
		{
			ManagerHelper.CheckPrivilege(Privilege.DeleteProductCategory);
			bool flag = (new CategoryDao()).DeleteCategory(categoryId);
			if (flag)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { categoryId };
				EventLogs.WriteOperationLog(Privilege.DeleteProductCategory, string.Format(invariantCulture, "删除了编号为 “{0}” 的店铺分类", objArray));
				HiCache.Remove("DataCache-Categories");
			}
			return flag;
		}

		public static bool DeleteProductTags(int productId, DbTransaction tran)
		{
			return (new TagDao()).DeleteProductTags(productId, tran);
		}

		public static bool DeleteTags(int tagId)
		{
			return (new TagDao()).DeleteTags(tagId);
		}

		public static int DisplaceCategory(int oldCategoryId, int newCategory)
		{
			return (new CategoryDao()).DisplaceCategory(oldCategoryId, newCategory);
		}

		public static DataTable GetBrandCategories()
		{
			return (new BrandCategoryDao()).GetBrandCategories();
		}

		public static DataTable GetBrandCategories(string brandName)
		{
			return (new BrandCategoryDao()).GetBrandCategories(brandName);
		}

		public static BrandCategoryInfo GetBrandCategory(int brandId)
		{
			return (new BrandCategoryDao()).GetBrandCategory(brandId);
		}

		public static DbQueryResult GetBrandQuery(BrandQuery query)
		{
			return (new BrandCategoryDao()).Query(query);
		}

		public static DataTable GetCategories()
		{
			DataTable categories = HiCache.Get("DataCache-Categories") as DataTable;
			if (null == categories)
			{
				categories = (new CategoryDao()).GetCategories();
				HiCache.Insert("DataCache-Categories", categories, 360, CacheItemPriority.Normal);
			}
			return categories;
		}

		public static CategoryInfo GetCategory(int categoryId)
		{
			return (new CategoryDao()).GetCategory(categoryId);
		}

		public static string GetFullCategory(int categoryId)
		{
			string str;
			CategoryInfo category = CatalogHelper.GetCategory(categoryId);
			if (category != null)
			{
				string name = category.Name;
				while (true)
				{
					if ((category == null ? true : !category.ParentCategoryId.HasValue))
					{
						break;
					}
					category = CatalogHelper.GetCategory(category.ParentCategoryId.Value);
					if (category != null)
					{
						name = string.Concat(category.Name, " &raquo; ", name);
					}
				}
				str = name;
			}
			else
			{
				str = null;
			}
			return str;
		}

		public static IList<CategoryInfo> GetMainCategories()
		{
			IList<CategoryInfo> categoryInfos = new List<CategoryInfo>();
			DataRow[] dataRowArray = CatalogHelper.GetCategories().Select("Depth = 1");
			for (int i = 0; i < (int)dataRowArray.Length; i++)
			{
				categoryInfos.Add(DataMapper.ConvertDataRowToProductCategory(dataRowArray[i]));
			}
			return categoryInfos;
		}

		public static IList<CategoryInfo> GetSequenceCategories()
		{
			IList<CategoryInfo> categoryInfos = new List<CategoryInfo>();
			foreach (CategoryInfo mainCategory in CatalogHelper.GetMainCategories())
			{
				categoryInfos.Add(mainCategory);
				CatalogHelper.LoadSubCategorys(mainCategory.CategoryId, categoryInfos);
			}
			return categoryInfos;
		}

		public static IList<CategoryInfo> GetSubCategories(int parentCategoryId)
		{
			IList<CategoryInfo> categoryInfos = new List<CategoryInfo>();
			string str = string.Concat("ParentCategoryId = ", parentCategoryId.ToString(CultureInfo.InvariantCulture));
			DataRow[] dataRowArray = CatalogHelper.GetCategories().Select(str);
			for (int i = 0; i < (int)dataRowArray.Length; i++)
			{
				categoryInfos.Add(DataMapper.ConvertDataRowToProductCategory(dataRowArray[i]));
			}
			return categoryInfos;
		}

		public static string GetTagName(int tagId)
		{
			return (new TagDao()).GetTagName(tagId);
		}

		public static DataTable GetTags()
		{
			return (new TagDao()).GetTags();
		}

		public static bool IsExitProduct(string CategoryId)
		{
			return (new CategoryDao()).IsExitProduct(CategoryId);
		}

		private static void LoadSubCategorys(int parentCategoryId, IList<CategoryInfo> categories)
		{
			IList<CategoryInfo> subCategories = CatalogHelper.GetSubCategories(parentCategoryId);
			if ((subCategories == null ? false : subCategories.Count > 0))
			{
				foreach (CategoryInfo subCategory in subCategories)
				{
					categories.Add(subCategory);
					CatalogHelper.LoadSubCategorys(subCategory.CategoryId, categories);
				}
			}
		}

		public static DbQueryResult Query(CategoriesQuery query)
		{
			return (new CategoryDao()).Query(query);
		}

		public static bool SetBrandCategoryThemes(int brandid, string themeName)
		{
			bool flag = (new BrandCategoryDao()).SetBrandCategoryThemes(brandid, themeName);
			if (flag)
			{
				HiCache.Remove("DataCache-Categories");
			}
			return flag;
		}

		public static bool SetCategoryThemes(int categoryId, string themeName)
		{
			if ((new CategoryDao()).SetCategoryThemes(categoryId, themeName))
			{
				HiCache.Remove("DataCache-Categories");
			}
			return false;
		}

		public static bool SetProductExtendCategory(int productId, string extendCategoryPath)
		{
			return (new CategoryDao()).SetProductExtendCategory(productId, extendCategoryPath);
		}

		public static bool SwapCategorySequence(int categoryId, int displaysequence)
		{
			return (new CategoryDao()).SwapCategorySequence(categoryId, displaysequence);
		}

		public static void UpdateBrandCategorieDisplaySequence(int brandId, SortAction action)
		{
			(new BrandCategoryDao()).UpdateBrandCategoryDisplaySequence(brandId, action);
		}

		public static bool UpdateBrandCategory(BrandCategoryInfo brandCategory)
		{
			bool flag = (new BrandCategoryDao()).UpdateBrandCategory(brandCategory);
			if ((!flag ? false : (new BrandCategoryDao()).DeleteBrandProductTypes(brandCategory.BrandId)))
			{
				(new BrandCategoryDao()).AddBrandProductTypes(brandCategory.BrandId, brandCategory.ProductTypes);
			}
			return flag;
		}

		public static bool UpdateBrandCategoryDisplaySequence(int barndId, int displaysequence)
		{
			return (new BrandCategoryDao()).UpdateBrandCategoryDisplaySequence(barndId, displaysequence);
		}

		public static CategoryActionStatus UpdateCategory(CategoryInfo category)
		{
			CategoryActionStatus categoryActionStatu;
			if (null != category)
			{
				Globals.EntityCoding(category, true);
				CategoryActionStatus categoryActionStatu1 = (new CategoryDao()).UpdateCategory(category);
				if (categoryActionStatu1 == CategoryActionStatus.Success)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] categoryId = new object[] { category.CategoryId };
					EventLogs.WriteOperationLog(Privilege.EditProductCategory, string.Format(invariantCulture, "修改了编号为 “{0}” 的店铺分类", categoryId));
					HiCache.Remove("DataCache-Categories");
				}
				categoryActionStatu = categoryActionStatu1;
			}
			else
			{
				categoryActionStatu = CategoryActionStatus.UnknowError;
			}
			return categoryActionStatu;
		}

		public static bool UpdateTags(int tagId, string tagName)
		{
			bool flag = false;
			int tags = (new TagDao()).GetTags(tagName);
			if ((tags == tagId ? true : tags <= 0))
			{
				flag = (new TagDao()).UpdateTags(tagId, tagName);
			}
			return flag;
		}
	}
}