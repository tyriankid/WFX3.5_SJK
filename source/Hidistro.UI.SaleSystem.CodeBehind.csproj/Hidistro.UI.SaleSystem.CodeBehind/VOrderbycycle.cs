using Hidistro.ControlPanel.Sales;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VOrderbycycle : VshopTemplatedWebControl
	{
		private Literal litProductName;
        private Literal litSalePrice;
        private Literal litSalesCount;
        private Literal litPrice;
        private Image productImg;
        private HtmlInputHidden hidDelayDays;
        private HtmlInputHidden hidPrice;
        private HtmlInputHidden hidQuantityPerDay;
        private HtmlInputHidden hidSendDays;
        private HtmlInputHidden hidProductSku;
        private HtmlInputHidden hidSendStartDate;
        private HtmlInputHidden hidSendEndDate;

        private int productId;
        private string orderId;
        private string categoryId;

        protected override void OnInit(System.EventArgs e)
		{
            if (this.SkinName == null)
			{
				this.SkinName = "skin-vOrderbycycle.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
            this.litProductName = (Literal)this.FindControl("litProductName");
            this.litSalePrice = (Literal)this.FindControl("litSalePrice");
            this.litSalesCount = (Literal)this.FindControl("litSalesCount");
            this.litPrice = (Literal)this.FindControl("litPrice");
            this.productImg = (Image)this.FindControl("productImg");
            this.hidDelayDays = (HtmlInputHidden)this.FindControl("hidDelayDays");
            this.hidPrice = (HtmlInputHidden)this.FindControl("hidPrice");
            this.hidSendDays = (HtmlInputHidden)this.FindControl("hidSendDays");
            this.hidQuantityPerDay = (HtmlInputHidden)this.FindControl("hidQuantityPerDay");
            this.hidProductSku = (HtmlInputHidden)this.FindControl("hidProductSku");
            this.hidSendStartDate = (HtmlInputHidden)this.FindControl("hidSendStartDate");
            this.hidSendEndDate = (HtmlInputHidden)this.FindControl("hidSendEndDate");


            if (!int.TryParse(this.Page.Request.QueryString["productId"], out this.productId))
            {
                orderId = this.Page.Request.QueryString["xdOrderId"];
                if (string.IsNullOrEmpty(orderId))
                {
                    base.GotoResourceNotFound("");
                    return;
                }
                
            }

            OrderInfo orderinfo = OrderHelper.GetOrderInfo(this.orderId);
            if(orderinfo != null)
            {
                foreach (LineItemInfo iteminfo in orderinfo.LineItems.Values)
                {
                    this.productId = iteminfo.ProductId;
                    this.hidSendDays.Value = iteminfo.SendDays.ToString();
                    this.hidQuantityPerDay.Value = iteminfo.QuantityPerDay.ToString();
                    this.hidSendStartDate.Value = iteminfo.SendStartDate.ToString("yyyy-MM-dd");
                    this.hidSendEndDate.Value = iteminfo.SendEndDate.ToString("yyyy-MM-dd");
                }
            }

            ProductInfo product = ProductBrowser.GetProduct(MemberProcessor.GetCurrentMember(), this.productId);



            if (product != null)
            {
                litProductName.Text = product.ProductName;
                litSalePrice.Text = product.MinSalePrice.ToString("F2");
                litPrice.Text = decimal.Parse(product.MarketPrice.ToString()).ToString("F2");
                litSalesCount.Text = product.SaleCounts.ToString();
                hidPrice.Value = product.MinSalePrice.ToString("F2");
                productImg.ImageUrl = product.ThumbnailUrl310;
                //通过牛奶粉类获取延迟天数
                if (!string.IsNullOrEmpty(product.MainCategoryPath))
                {
                    this.categoryId = product.MainCategoryPath.Split(new char[]
                    {
                        '|'
                    })[0];
                }
                else
                {
                    this.categoryId = "0";
                }
                CategoryInfo cinfo = CategoryBrowser.GetCategory(Convert.ToInt32(this.categoryId));
                this.hidDelayDays.Value = cinfo.DelayDays.ToString();
                this.hidProductSku.Value = product.SkuId;
            }


            PageTitle.AddSiteNameTitle("订购页面");
		}
	}
}
