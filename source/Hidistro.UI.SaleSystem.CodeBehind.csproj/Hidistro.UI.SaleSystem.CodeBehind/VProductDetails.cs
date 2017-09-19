using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.VShop;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.SaleSystem.Tags;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[ParseChildren(true)]
	public class VProductDetails : VshopTemplatedWebControl
	{
		private int productId;

		private VshopTemplatedRepeater rptProductImages;

		private Literal litProdcutName;
        private Literal litProdcutName2;
        private HtmlInputHidden hidDelayDays;

        private Literal litProdcutTag;

		private Literal litSalePrice;

		private Literal litMarketPrice;

		private Literal litShortDescription;

		private Literal litDescription;

		private Literal litStock;

		private Literal litSoldCount;

		private Literal litConsultationsCount;

		private Literal litReviewsCount;

		private Literal litItemParams;

		private Common_SKUSelector skuSelector;

		private Common_ExpandAttributes expandAttr;

		private HyperLink linkDescription;

		private HtmlInputHidden litHasCollected;

		private HtmlInputHidden litCategoryId;

		private HtmlInputHidden litproductid;

		private HtmlInputHidden litTemplate;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VProductDetails.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			if (!int.TryParse(this.Page.Request.QueryString["productId"], out this.productId))
			{
				base.GotoResourceNotFound("");
			}
			this.rptProductImages = (VshopTemplatedRepeater)this.FindControl("rptProductImages");
			this.litItemParams = (Literal)this.FindControl("litItemParams");
			this.litProdcutName = (Literal)this.FindControl("litProdcutName");
            this.litProdcutName2 = (Literal)this.FindControl("litProdcutName2");
            this.hidDelayDays = (HtmlInputHidden)this.FindControl("hidDelayDays");
            this.litProdcutTag = (Literal)this.FindControl("litProdcutTag");
			this.litSalePrice = (Literal)this.FindControl("litSalePrice");
			this.litMarketPrice = (Literal)this.FindControl("litMarketPrice");
			this.litShortDescription = (Literal)this.FindControl("litShortDescription");
			this.litDescription = (Literal)this.FindControl("litDescription");
			this.litStock = (Literal)this.FindControl("litStock");
			this.skuSelector = (Common_SKUSelector)this.FindControl("skuSelector");
			this.linkDescription = (HyperLink)this.FindControl("linkDescription");
			this.expandAttr = (Common_ExpandAttributes)this.FindControl("ExpandAttributes");
			this.litSoldCount = (Literal)this.FindControl("litSoldCount");
			this.litConsultationsCount = (Literal)this.FindControl("litConsultationsCount");
			this.litReviewsCount = (Literal)this.FindControl("litReviewsCount");
			this.litHasCollected = (HtmlInputHidden)this.FindControl("litHasCollected");
			this.litCategoryId = (HtmlInputHidden)this.FindControl("litCategoryId");
			this.litproductid = (HtmlInputHidden)this.FindControl("litproductid");
			this.litTemplate = (HtmlInputHidden)this.FindControl("litTemplate");
			ProductInfo product = ProductBrowser.GetProduct(MemberProcessor.GetCurrentMember(), this.productId);
			if (product != null)
			{
				this.litproductid.Value = this.productId.ToString();
				this.litTemplate.Value = product.FreightTemplateId.ToString();
				if (product == null)
				{
					base.GotoResourceNotFound("此商品已不存在");
				}
				if (product.SaleStatus != ProductSaleStatus.OnSale)
				{
					base.GotoResourceNotFound(ErrorType.前台商品下架, "此商品已下架");
				}
				if (this.rptProductImages != null)
				{
					string locationUrl = "javascript:;";
					SlideImage[] source = new SlideImage[]
					{
						new SlideImage(product.ImageUrl1, locationUrl),
						new SlideImage(product.ImageUrl2, locationUrl),
						new SlideImage(product.ImageUrl3, locationUrl),
						new SlideImage(product.ImageUrl4, locationUrl),
						new SlideImage(product.ImageUrl5, locationUrl)
					};
					this.rptProductImages.DataSource = from item in source
					where !string.IsNullOrWhiteSpace(item.ImageUrl)
					select item;
					this.rptProductImages.DataBind();
				}
				string mainCategoryPath = product.MainCategoryPath;
				if (!string.IsNullOrEmpty(mainCategoryPath))
				{
					this.litCategoryId.Value = mainCategoryPath.Split(new char[]
					{
						'|'
					})[0];
				}
				else
				{
					this.litCategoryId.Value = "0";
				}
                //通过牛奶粉类获取延迟天数
                CategoryInfo cinfo = CategoryBrowser.GetCategory(Convert.ToInt32(this.litCategoryId.Value));
                this.hidDelayDays.Value = cinfo.DelayDays.ToString();

				string productName = product.ProductName;
				string text = ProductBrowser.GetProductTagName(this.productId);
				if (!string.IsNullOrEmpty(text))
				{
					this.litProdcutTag.Text = "<div class='y-shopicon'>" + text.Trim() + "</div>";
					text = "<span class='producttag'>【" + HttpContext.Current.Server.HtmlEncode(text) + "】</span>";
				}
				this.litProdcutName.Text = text + productName;
                this.litProdcutName2.Text = productName;

                if (product.MinSalePrice != product.MaxSalePrice)
				{
					this.litSalePrice.Text = product.MinSalePrice.ToString("F2") + "~" + product.MaxSalePrice.ToString("F2");
				}
				else
				{
					this.litSalePrice.Text = product.MinSalePrice.ToString("F2");
				}
				if (product.MarketPrice.HasValue)
				{
					if (product.MarketPrice > 0m)
					{
						this.litMarketPrice.Text = "<del class=\"text-muted font-s\">¥" + product.MarketPrice.Value.ToString("F2") + "</del>";
					}
				}
				this.litShortDescription.Text = product.ShortDescription;
				string text2 = product.Description;
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = Regex.Replace(text2, "<img[^>]*\\bsrc=('|\")([^'\">]*)\\1[^>]*>", "<img alt='" + HttpContext.Current.Server.HtmlEncode(productName) + "' src='$2' />", RegexOptions.IgnoreCase);
				}
				if (this.litDescription != null)
				{
					this.litDescription.Text = text2;
				}
				this.litSoldCount.SetWhenIsNotNull(product.ShowSaleCounts.ToString());
				this.litStock.Text = product.Stock.ToString();
				this.skuSelector.ProductId = this.productId;
				if (this.expandAttr != null)
				{
					this.expandAttr.ProductId = this.productId;
				}
				if (this.linkDescription != null)
				{
					this.linkDescription.NavigateUrl = "/Vshop/ProductDescription.aspx?productId=" + this.productId;
				}
				int num = ProductBrowser.GetProductConsultationsCount(this.productId, false);
				this.litConsultationsCount.SetWhenIsNotNull(num.ToString());
				num = ProductBrowser.GetProductReviewsCount(this.productId);
				this.litReviewsCount.SetWhenIsNotNull(num.ToString());
				MemberInfo currentMember = MemberProcessor.GetCurrentMember();
				bool flag = false;
				if (currentMember != null)
				{
					flag = ProductBrowser.CheckHasCollect(currentMember.UserId, this.productId);
				}
				this.litHasCollected.SetWhenIsNotNull(flag ? "1" : "0");
				ProductBrowser.UpdateVisitCounts(this.productId);
				PageTitle.AddSiteNameTitle(productName);
				PageTitle.AddSiteDescription(product.ShortDescription);
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
				string objStr = "";
				if (!string.IsNullOrEmpty(masterSettings.GoodsPic))
				{
					objStr = Globals.HostPath(HttpContext.Current.Request.Url) + masterSettings.GoodsPic;
				}
				this.litItemParams.Text = string.Concat(new string[]
				{
					Globals.GetReplaceStr(objStr, "|", "｜"),
					"|",
					Globals.GetReplaceStr(masterSettings.GoodsName, "|", "｜"),
					"|",
					Globals.GetReplaceStr(masterSettings.GoodsDescription, "|", "｜"),
					"$",
					Globals.HostPath(HttpContext.Current.Request.Url).Replace("|", "｜"),
					Globals.GetReplaceStr(product.ImageUrl1, "|", "｜"),
					"|",
					Globals.GetReplaceStr(product.ProductName, "|", "｜"),
					"|",
					Globals.GetReplaceStr(product.ShortDescription, "|", "｜"),
					"|",
					HttpContext.Current.Request.Url.ToString().Replace("|", "｜")
				});
			}
			else
			{
				HttpContext.Current.Response.Redirect("/default.aspx");
				HttpContext.Current.Response.End();
			}
		}
	}
}
