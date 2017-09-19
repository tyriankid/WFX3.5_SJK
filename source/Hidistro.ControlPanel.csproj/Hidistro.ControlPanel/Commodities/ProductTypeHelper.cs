using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Store;
using Hidistro.SqlDal.Commodities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;

namespace Hidistro.ControlPanel.Commodities
{
	public sealed class ProductTypeHelper
	{
		private ProductTypeHelper()
		{
		}

		public static bool AddAttribute(AttributeInfo attribute)
		{
			return (new AttributeDao()).AddAttribute(attribute);
		}

		public static bool AddAttributeName(AttributeInfo attribute)
		{
			return (new AttributeDao()).AddAttributeName(attribute) > 0;
		}

		public static int AddAttributeValue(AttributeValueInfo attributeValue)
		{
			return (new AttributeValueDao()).AddAttributeValue(attributeValue);
		}

		public static int AddProductType(ProductTypeInfo productType)
		{
			int num;
			if (productType != null)
			{
				Globals.EntityCoding(productType, true);
				int num1 = (new ProductTypeDao()).AddProductType(productType);
				if (num1 > 0)
				{
					if (productType.Brands.Count > 0)
					{
						(new ProductTypeDao()).AddProductTypeBrands(num1, productType.Brands);
					}
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] typeName = new object[] { productType.TypeName };
					EventLogs.WriteOperationLog(Privilege.AddProductType, string.Format(invariantCulture, "创建了一个新的商品类型:”{0}”", typeName));
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static bool ClearAttributeValue(int attributeId)
		{
			return (new AttributeValueDao()).ClearAttributeValue(attributeId);
		}

		public static bool DeleteAttribute(int attriubteId)
		{
			return (new AttributeDao()).DeleteAttribute(attriubteId);
		}

		public static bool DeleteAttributeValue(int attributeValueId)
		{
			return (new AttributeValueDao()).DeleteAttributeValue(attributeValueId);
		}

		public static bool DeleteProductType(int typeId)
		{
			ManagerHelper.CheckPrivilege(Privilege.DeleteProductType);
			bool flag = (new ProductTypeDao()).DeleteProducType(typeId);
			if (flag)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { typeId };
				EventLogs.WriteOperationLog(Privilege.DeleteProductType, string.Format(invariantCulture, "删除了编号为”{0}”的商品类型", objArray));
			}
			return flag;
		}

		public static AttributeInfo GetAttribute(int attributeId)
		{
			return (new AttributeDao()).GetAttribute(attributeId);
		}

		public static IList<AttributeInfo> GetAttributes(int typeId)
		{
			return (new AttributeDao()).GetAttributes(typeId);
		}

		public static IList<AttributeInfo> GetAttributes(int typeId, AttributeUseageMode attributeUseageMode)
		{
			return (new AttributeDao()).GetAttributes(typeId, attributeUseageMode);
		}

		public static AttributeValueInfo GetAttributeValueInfo(int valueId)
		{
			return (new AttributeValueDao()).GetAttributeValueInfo(valueId);
		}

		public static DataTable GetBrandCategoriesByTypeId(int typeId)
		{
			return (new ProductTypeDao()).GetBrandCategoriesByTypeId(typeId);
		}

		public static string GetBrandName(int type)
		{
			return (new ProductTypeDao()).GetBrandName(type);
		}

		public static ProductTypeInfo GetProductType(int typeId)
		{
			return (new ProductTypeDao()).GetProductType(typeId);
		}

		public static DbQueryResult GetProductTypes(ProductTypeQuery query)
		{
			return (new ProductTypeDao()).GetProductTypes(query);
		}

		public static IList<ProductTypeInfo> GetProductTypes()
		{
			return (new ProductTypeDao()).GetProductTypes();
		}

		public static int GetSpecificationId(int typeId, string specificationName)
		{
			int num;
			int specificationId = (new AttributeDao()).GetSpecificationId(typeId, specificationName);
			if (specificationId <= 0)
			{
				AttributeInfo attributeInfo = new AttributeInfo()
				{
					TypeId = typeId,
					UsageMode = AttributeUseageMode.Choose,
					UseAttributeImage = false,
					AttributeName = specificationName
				};
				num = (new AttributeDao()).AddAttributeName(attributeInfo);
			}
			else
			{
				num = specificationId;
			}
			return num;
		}

		public static int GetSpecificationValueId(int attributeId, string valueStr)
		{
			int num;
			int specificationValueId = (new AttributeValueDao()).GetSpecificationValueId(attributeId, valueStr);
			if (specificationValueId <= 0)
			{
				AttributeValueInfo attributeValueInfo = new AttributeValueInfo()
				{
					AttributeId = attributeId,
					ValueStr = valueStr
				};
				num = (new AttributeValueDao()).AddAttributeValue(attributeValueInfo);
			}
			else
			{
				num = specificationValueId;
			}
			return num;
		}

		public static int GetTypeId(string typeName)
		{
			int num;
			int typeId = (new ProductTypeDao()).GetTypeId(typeName);
			if (typeId <= 0)
			{
				ProductTypeInfo productTypeInfo = new ProductTypeInfo()
				{
					TypeName = typeName
				};
				num = (new ProductTypeDao()).AddProductType(productTypeInfo);
			}
			else
			{
				num = typeId;
			}
			return num;
		}

		public static void SwapAttributeSequence(int attributeId, int replaceAttributeId, int displaySequence, int replaceDisplaySequence)
		{
			(new AttributeDao()).SwapAttributeSequence(attributeId, replaceAttributeId, displaySequence, replaceDisplaySequence);
		}

		public static void SwapAttributeValueSequence(int attributeValueId, int replaceAttributeValueId, int displaySequence, int replaceDisplaySequence)
		{
			(new AttributeValueDao()).SwapAttributeValueSequence(attributeValueId, replaceAttributeValueId, displaySequence, replaceDisplaySequence);
		}

		public static bool UpdateAttribute(AttributeInfo attribute)
		{
			return (new AttributeDao()).UpdateAttribute(attribute);
		}

		public static bool UpdateAttributeName(AttributeInfo attribute)
		{
			return (new AttributeDao()).UpdateAttributeName(attribute);
		}

		public static bool UpdateAttributeValue(AttributeValueInfo attributeValue)
		{
			return (new AttributeValueDao()).UpdateAttributeValue(attributeValue);
		}

		public static bool UpdateProductType(ProductTypeInfo productType)
		{
			bool flag;
			if (productType != null)
			{
				Globals.EntityCoding(productType, true);
				bool flag1 = (new ProductTypeDao()).UpdateProductType(productType);
				if (flag1)
				{
					if ((new ProductTypeDao()).DeleteProductTypeBrands(productType.TypeId))
					{
						(new ProductTypeDao()).AddProductTypeBrands(productType.TypeId, productType.Brands);
					}
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] typeId = new object[] { productType.TypeId };
					EventLogs.WriteOperationLog(Privilege.EditProductType, string.Format(invariantCulture, "修改了编号为”{0}”的商品类型", typeId));
				}
				flag = flag1;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static string UploadSKUImage(HttpPostedFile postedFile)
		{
			string empty;
			if (ResourcesHelper.CheckPostedFile(postedFile, "image"))
			{
				string str = string.Concat(Globals.GetStoragePath(), "/sku/", ResourcesHelper.GenerateFilename(Path.GetExtension(postedFile.FileName)));
				Globals.UploadFileAndCheck(postedFile, HttpContext.Current.Request.MapPath(string.Concat(Globals.ApplicationPath, str)));
				empty = str;
			}
			else
			{
				empty = string.Empty;
			}
			return empty;
		}
	}
}