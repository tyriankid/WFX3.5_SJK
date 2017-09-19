using Hidistro.ControlPanel.Commodities;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.FenXiao;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hishop.Components.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
	public class EditMilkCard : AdminPage
	{
		protected string ReUrl = "MilkCardManage.aspx";

        protected string productInfoHtml = "奶卡商品尚未绑定,请绑定商品";

        private Guid CardId;

		protected string htmlOperatorName = "编辑";

        protected DropDownList ddlSite;

        protected System.Web.UI.HtmlControls.HtmlGenericControl EditTitle;

		protected System.Web.UI.WebControls.TextBox txtCardCount;

		protected System.Web.UI.WebControls.TextBox txtFreeSendDays;

		protected System.Web.UI.WebControls.TextBox txtFreeQuantityPerDay;

		protected System.Web.UI.WebControls.RadioButtonList rbtnlIsDefault;

		protected System.Web.UI.WebControls.Button btnEditUser;

        protected System.Web.UI.WebControls.HiddenField hiddProductId;



        protected EditMilkCard() : base("m05", "fxp08")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (this.Page.Request.QueryString["ID"] != null)
			{
				if (!Guid.TryParse(this.Page.Request.QueryString["ID"], out this.CardId))
				{
					base.GotoResourceNotFound();
					return;
				}
			}
			else
			{
				this.htmlOperatorName = "新增";
			}
			this.btnEditUser.Click += new System.EventHandler(this.btnEditUser_Click);
			if (!this.Page.IsPostBack)
			{
                DataTable dtSite = ManagerHelper.GetSiteManagers();
                ddlSite.DataSource = dtSite;
                ddlSite.DataTextField = "StoreName";
                ddlSite.DataValueField = "SiteId";
                ddlSite.DataBind();
                this.LoadCardInfo();
			}
		}

        public string GetProductInfoHtml(ProductInfo product)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("<div class='shop-img fl'>");
            stringBuilder.Append("<img src='" + (string.IsNullOrEmpty(product.ImageUrl1) ? "/utility/pics/none.gif" : product.ImageUrl1) + "' width='60' height='60' >");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class='shop-username fl ml10'>");
            stringBuilder.Append("<p>" + product.ProductName + "</p>");
            stringBuilder.Append("</div>");
            stringBuilder.Append(" <p class='fl ml20'>现价：￥" + product.MarketPrice.Value.ToString("f2") + "</p>");
            stringBuilder.Append(" <p class='fl ml20'>库存：" + ProductHelper.GetProductSumStock(product.ProductId) + "</p>");
            return stringBuilder.ToString();
        }

        protected void btnEditUser_Click(object sender, System.EventArgs e)
		{
            if(this.Page.Request.QueryString["ID"] == null)
            {
                MilkCardInfo mcinfo = new MilkCardInfo()
                {
                    SiteId = Globals.ToNum(this.ddlSite.SelectedValue),
                    Status = 0,
                    CreateDate = DateTime.Now,
                    ProductId = Globals.ToNum(this.hiddProductId.Value),
                    FreeSendDays = Globals.ToNum(txtFreeSendDays.Text),
                    FreeQuantityPerDay = Globals.ToNum(txtFreeQuantityPerDay.Text)
                };
                if (VShopHelper.CreateMilkCards(mcinfo, Globals.ToNum(txtCardCount.Text)))
                {
                    this.ShowMsgAndReUrl("创建成功", true, "MilkCardManage.aspx");
                }
            }
            else
            {
                MilkCardInfo mcinfo =VShopHelper.GetMilkCard(CardId);
                mcinfo.ProductId = Globals.ToNum(this.hiddProductId.Value);
                mcinfo.FreeSendDays = Globals.ToNum(txtFreeSendDays.Text);
                mcinfo.FreeQuantityPerDay = Globals.ToNum(txtFreeQuantityPerDay.Text);
                if (VShopHelper.UpdateMilkCard(mcinfo))
                {
                    this.ShowMsgAndReUrl("编辑成功", true, "MilkCardManage.aspx");
                }
            }


		}

		private bool ValidationMember(MemberInfo member)
		{
			ValidationResults validationResults = Validation.Validate<MemberInfo>(member, new string[]
			{
				"ValMember"
			});
			string text = string.Empty;
			if (!validationResults.IsValid)
			{
				foreach (ValidationResult current in ((System.Collections.Generic.IEnumerable<ValidationResult>)validationResults))
				{
					text += Formatter.FormatErrorMessage(current.Message);
				}
				this.ShowMsg(text, false);
			}
			return validationResults.IsValid;
		}

		private void LoadCardInfo()
		{
			if (this.Page.Request.QueryString["ID"] != null)
			{
				MilkCardInfo mcinfo =  VShopHelper.GetMilkCard(this.CardId);
				if (mcinfo == null)
				{
					base.GotoResourceNotFound();
					return;
				}
                this.ddlSite.SelectedValue = mcinfo.SiteId.ToString();
                this.txtFreeSendDays.Text = mcinfo.FreeSendDays.ToString();
                this.txtFreeQuantityPerDay.Text = mcinfo.FreeQuantityPerDay.ToString();
                this.hiddProductId.Value = mcinfo.ProductId.ToString();
                if (mcinfo.ProductId > 0)
                {
                    ProductInfo productDetails = ProductHelper.GetProductDetails(mcinfo.ProductId);
                    this.productInfoHtml = this.GetProductInfoHtml(productDetails);
                }


            }
		}
	}
}
