using Hidistro.ControlPanel.Commodities;
using Hidistro.Core;
using Hidistro.Entities.Commodities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Hidistro.UI.ControlPanel.Utility
{
	public class ProductCategoriesDropDownListNew : ProductCategoriesDropDownList
	{
		private string strDepth = "\u3000";

		public override void DataBind()
		{
			this.Items.Clear();
			this.Items.Add(new ListItem(base.NullToDisplay, string.Empty));
			if (base.IsTopCategory)
			{
				IList<CategoryInfo> list = CatalogHelper.GetMainCategories();
				foreach (CategoryInfo current in list)
				{
					this.Items.Add(new ListItem(Globals.HtmlDecode(current.Name), current.CategoryId.ToString()));
				}
			}
			else
			{
				IList<CategoryInfo> list = CatalogHelper.GetSequenceCategories();
				for (int i = 0; i < list.Count; i++)
				{
					this.Items.Add(new ListItem(this.FormatDepth(list[i].Depth, Globals.HtmlDecode(list[i].Name)), list[i].CategoryId.ToString(CultureInfo.InvariantCulture)));
				}
			}
		}

		private string FormatDepth(int depth, string categoryName)
		{
			for (int i = 1; i < depth; i++)
			{
				categoryName = this.strDepth + categoryName;
			}
			return categoryName;
		}
	}
}
