using Hidistro.Core;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.OpenAPI.Test
{
	public class TestProductApi : AdminPage
	{
		private IProduct productApi = new ProductApi();

		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected System.Web.UI.WebControls.Button btnTestGetSoldProducts;

		protected System.Web.UI.WebControls.TextBox txtTestGetSoldProducts;

		protected System.Web.UI.WebControls.Label lbProductId;

		protected System.Web.UI.WebControls.TextBox txtProductId;

		protected System.Web.UI.WebControls.Button btnTestGetProduct;

		protected System.Web.UI.WebControls.TextBox txtTestGetProduct;

		protected System.Web.UI.WebControls.Label lbPId;

		protected System.Web.UI.WebControls.TextBox txtPId;

		protected System.Web.UI.WebControls.Label lbProductSku;

		protected System.Web.UI.WebControls.TextBox txtProductSku;

		protected System.Web.UI.WebControls.Label lbProductSkuAmount;

		protected System.Web.UI.WebControls.TextBox txtProductSkuAmount;

		protected System.Web.UI.WebControls.Label lbUpdateType;

		protected System.Web.UI.WebControls.TextBox txtUpdateType;

		protected System.Web.UI.WebControls.Button btnTestUpdateProductQuantity;

		protected System.Web.UI.WebControls.TextBox txtTestUpdateProductQuantity;

		protected System.Web.UI.WebControls.Label lbId;

		protected System.Web.UI.WebControls.TextBox txtId;

		protected System.Web.UI.WebControls.Label lbStatus;

		protected System.Web.UI.WebControls.DropDownList ddlStatus;

		protected System.Web.UI.WebControls.Button btnTestUpdateProductApproveStatus;

		protected System.Web.UI.WebControls.TextBox txtUpdateProductApproveStatus;

		protected TestProductApi() : base("m03", "00000")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		protected void btnTestGetSoldProducts_Click(object sender, System.EventArgs e)
		{
			string soldProducts = this.productApi.GetSoldProducts(null, null, "", "", "", 1, 10);
			this.txtTestGetSoldProducts.Text = soldProducts;
		}

		protected void btnTestGetProduct_Click(object sender, System.EventArgs e)
		{
			int num_iid = Globals.ToNum(this.txtProductId.Text);
			string product = this.productApi.GetProduct(num_iid);
			this.txtTestGetProduct.Text = product;
		}

		protected void btnTestUpdateProductQuantity_Click(object sender, System.EventArgs e)
		{
			int num_iid = Globals.ToNum(this.txtPId.Text);
			string text = this.txtProductSku.Text;
			int quantity = Globals.ToNum(this.txtProductSkuAmount.Text);
			int type = Globals.ToNum(this.txtUpdateType.Text);
			string text2 = this.productApi.UpdateProductQuantity(num_iid, text, quantity, type);
			this.txtTestUpdateProductQuantity.Text = text2;
		}

		protected void btnTestUpdateProductApproveStatus_Click(object sender, System.EventArgs e)
		{
			int num_iid = Globals.ToNum(this.txtId.Text);
			string selectedValue = this.ddlStatus.SelectedValue;
			string text = this.productApi.UpdateProductApproveStatus(num_iid, selectedValue);
			this.txtUpdateProductApproveStatus.Text = text;
		}
	}
}
