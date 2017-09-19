using ASPNET.WebControls;
using Hidistro.ControlPanel.Commodities;
using Hidistro.ControlPanel.Promotions;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Promotions;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.promotion
{
	public class LimitedTimeDiscountAddProduct : AdminPage
	{
		protected string actionName;

		protected int id;

		private int? categoryId;

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected PageSize hrefPageSize;

		protected System.Web.UI.WebControls.TextBox txtProductName;

		protected ProductCategoriesDropDownList dropCategories;

		protected System.Web.UI.WebControls.Button btnSeach;

		protected System.Web.UI.WebControls.Repeater grdProducts;

		protected Pager pager;

		protected LimitedTimeDiscountAddProduct() : base("m08", "yxp24")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!base.IsPostBack)
			{
				this.dropCategories.IsUnclassified = true;
				this.dropCategories.DataBind();
				this.DataBindDiscount();
			}
		}

		private void DataBindDiscount()
		{
			string text = Globals.RequestQueryStr("key").Trim();
			if (!string.IsNullOrEmpty(text))
			{
				this.txtProductName.Text = text;
			}
			int num = Globals.RequestQueryNum("cid");
			this.id = Globals.RequestQueryNum("id");
			if (this.id > 0)
			{
				LimitedTimeDiscountInfo discountInfo = LimitedTimeDiscountHelper.GetDiscountInfo(this.id);
				if (discountInfo != null)
				{
					this.actionName = discountInfo.ActivityName;
				}
				int? num2 = null;
				if (num > 0)
				{
					num2 = new int?(num);
					this.dropCategories.SelectedValue = new int?(num);
				}
				ProductQuery productQuery = new ProductQuery
				{
					Keywords = text,
					ProductCode = "",
					CategoryId = num2,
					PageSize = this.pager.PageSize,
					PageIndex = this.pager.PageIndex,
					SortOrder = SortAction.Desc,
					SortBy = "DisplaySequence"
				};
				if (num > 0)
				{
					productQuery.MaiCategoryPath = CatalogHelper.GetCategory(num).Path;
				}
				DbQueryResult discountProduct = LimitedTimeDiscountHelper.GetDiscountProduct(productQuery);
				this.grdProducts.DataSource = discountProduct.Data;
				this.grdProducts.DataBind();
				this.pager.TotalRecords = discountProduct.TotalRecords;
				return;
			}
			base.Response.Redirect("LimitedTimeDiscountList.aspx");
		}

		protected string GetDisable(string ActivityName, object limitedTimeDiscountId, int discountId)
		{
			if (!string.IsNullOrEmpty(ActivityName) && Globals.ToNum(limitedTimeDiscountId) != discountId)
			{
				return "disabled";
			}
			return "";
		}

		protected void btnSeach_Click(object sender, System.EventArgs e)
		{
			string text = this.txtProductName.Text.Trim();
			int num = this.dropCategories.SelectedValue.HasValue ? Globals.ToNum(this.dropCategories.SelectedValue.Value) : 0;
			this.id = Globals.RequestQueryNum("id");
			int num2 = Globals.RequestQueryNum("pagesize");
			string text2 = "LimitedTimeDiscountAddProduct.aspx?id=" + this.id;
			if (num2 > 0)
			{
				text2 = text2 + "&pagesize=" + num2;
			}
			if (num > 0)
			{
				text2 = text2 + "&cid=" + num;
			}
			if (!string.IsNullOrEmpty(text))
			{
				text2 = text2 + "&key=" + base.Server.UrlEncode(text);
			}
			base.Response.Redirect(text2);
		}

		protected string GetDisplayValue(object obj)
		{
			decimal d;
			if (!decimal.TryParse(obj.ToString(), out d))
			{
				return "none";
			}
			if (d > 0m)
			{
				return "";
			}
			return "none";
		}

		protected string GetDisplay(object obj)
		{
			if (!string.IsNullOrEmpty(obj.ToString()))
			{
				return "";
			}
			return "none";
		}
	}
}
