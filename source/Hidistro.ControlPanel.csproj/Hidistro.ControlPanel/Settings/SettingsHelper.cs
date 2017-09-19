using Hidistro.Entities.Settings;
using Hidistro.SqlDal.Sales;
using Hidistro.SqlDal.Settings;
using System;
using System.Collections.Generic;
using System.Data;

namespace Hidistro.ControlPanel.Settings
{
	public sealed class SettingsHelper
	{
		public static string Error;

		static SettingsHelper()
		{
			SettingsHelper.Error = "";
		}

		public SettingsHelper()
		{
		}

		public static bool CreateShippingTemplate(FreightTemplate freightTemplate)
		{
			ShippingModeDao shippingModeDao = new ShippingModeDao();
			bool flag = shippingModeDao.CreateShippingTemplate(freightTemplate);
			SettingsHelper.Error = shippingModeDao.Error;
			return flag;
		}

		public static int DeleteExpressTemplates(string expressIds)
		{
			return (new ExpressTemplateDao()).DeleteExpressTemplates(expressIds);
		}

		public static bool DeleteShippingTemplate(int templateId)
		{
			return (new ShippingModeDao()).DeleteShippingTemplate(templateId);
		}

		public static int DeleteShippingTemplates(string templateIds)
		{
			return (new ShippingModeDao()).DeleteShippingTemplates(templateIds);
		}

		public static DataTable GetAllFreightItems()
		{
			return (new ShippingModeDao()).GetAllFreightItems();
		}

		public static string getDefaultShipText(bool IsDefault)
		{
			string str = "";
			if (IsDefault)
			{
				str = "全国";
			}
			return str;
		}

		public static string getFreeShipText(bool FreeShip)
		{
			string str = "卖家承担";
			if (FreeShip)
			{
				str = "包邮";
			}
			return str;
		}

		public static DataTable GetFreeTemplateShipping(string RegionId, int TemplateId, int ModeId)
		{
			return (new ShippingModeDao()).GetFreeTemplateShipping(RegionId, TemplateId, ModeId);
		}

		public static FreightTemplate GetFreightTemplate(int templateId, bool includeDetail)
		{
			return (new ShippingModeDao()).GetFreightTemplate(templateId, includeDetail);
		}

		public static IList<FreightTemplate> GetFreightTemplates()
		{
			return (new ExpressTemplateDao()).GetFreightTemplates();
		}

		public static string getMUnitText(int MUnit)
		{
			string str = "件";
			switch (MUnit)
			{
				case 1:
				{
					str = "件";
					break;
				}
				case 2:
				{
					str = "KG";
					break;
				}
				case 3:
				{
					str = "立方";
					break;
				}
			}
			return str;
		}

		public static string GetShippingTemplateLinkProduct(int[] templateIds)
		{
			return (new ShippingModeDao()).GetShippingTemplateLinkProduct(templateIds);
		}

		public static string getShippingTypeByModeId(int ModeId)
		{
			string str = "未知";
			switch (ModeId)
			{
				case 1:
				{
					str = "快递";
					break;
				}
				case 2:
				{
					str = "EMS";
					break;
				}
				case 3:
				{
					str = "顺丰";
					break;
				}
				case 4:
				{
					str = "平邮";
					break;
				}
			}
			return str;
		}

		public static IList<SpecifyRegionGroup> GetSpecifyRegionGroups(int templateId)
		{
			return (new ShippingModeDao()).GetSpecifyRegionGroups(templateId);
		}

		public static DataTable GetSpecifyRegionGroupsModeId(string TemplateIds, string RegionId)
		{
			return (new ShippingModeDao()).GetSpecifyRegionGroupsModeId(TemplateIds, RegionId);
		}

		public static FreightTemplate GetTemplateMessage(int TemplateId)
		{
			return (new ShippingModeDao()).GetTemplateMessage(TemplateId);
		}

		public static bool SetExpressIsDefault(int expressId)
		{
			return (new ExpressTemplateDao()).SetExpressDefault(expressId);
		}

		public static bool UpdateShippingTemplate(FreightTemplate freightTemplate, string templateName)
		{
			ShippingModeDao shippingModeDao = new ShippingModeDao();
			bool flag = shippingModeDao.UpdateShippingTemplate(freightTemplate, templateName);
			SettingsHelper.Error = shippingModeDao.Error;
			return flag;
		}
	}
}