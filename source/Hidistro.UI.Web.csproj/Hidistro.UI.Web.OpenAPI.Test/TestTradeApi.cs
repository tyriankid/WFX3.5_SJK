using Hidistro.Core;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hishop.Open.Api;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.OpenAPI.Test
{
	public class TestTradeApi : AdminPage
	{
		private ITrade tradeApi = new TradeApi();

		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected System.Web.UI.WebControls.DropDownList ddlStatus1;

		protected System.Web.UI.WebControls.TextBox txtBuyerName1;

		protected System.Web.UI.WebControls.Button btnTestGetSoldTrades;

		protected System.Web.UI.WebControls.TextBox txtTestGetSoldTrades;

		protected System.Web.UI.WebControls.DropDownList ddlStatus2;

		protected System.Web.UI.WebControls.TextBox txtBuyerName2;

		protected System.Web.UI.WebControls.Button btnTestGetIncrementSoldTrades;

		protected System.Web.UI.WebControls.TextBox txtTestGetIncrementSoldTrades;

		protected System.Web.UI.WebControls.TextBox txtTradeId1;

		protected System.Web.UI.WebControls.TextBox txtCompanyName1;

		protected System.Web.UI.WebControls.TextBox txtTransId1;

		protected System.Web.UI.WebControls.Button btnTestChangLogistics;

		protected System.Web.UI.WebControls.TextBox txtTestChangLogistics;

		protected System.Web.UI.WebControls.TextBox txtTradeId2;

		protected System.Web.UI.WebControls.Button btnTestGetTrade;

		protected System.Web.UI.WebControls.TextBox txtTestGetTrade;

		protected System.Web.UI.WebControls.TextBox txtTradeId3;

		protected System.Web.UI.WebControls.TextBox txtRemark1;

		protected System.Web.UI.WebControls.TextBox txtTag;

		protected System.Web.UI.WebControls.Button btnTestUpdateTradeMemo;

		protected System.Web.UI.WebControls.TextBox txtTestUpdateTradeMemo;

		protected System.Web.UI.WebControls.TextBox txtTradeId4;

		protected System.Web.UI.WebControls.TextBox txtCompanyName4;

		protected System.Web.UI.WebControls.TextBox txtTransId4;

		protected System.Web.UI.WebControls.Button btnTestSendLogistic;

		protected System.Web.UI.WebControls.TextBox txtSendLogistic;

		protected TestTradeApi() : base("m03", "00000")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		protected void btnTestGetSoldTrades_Click(object sender, System.EventArgs e)
		{
			string status = (this.ddlStatus1.SelectedValue == "all") ? "" : this.ddlStatus1.SelectedValue;
			string text = this.txtBuyerName1.Text;
			string soldTrades = this.tradeApi.GetSoldTrades(new System.DateTime?(System.DateTime.Now.AddMonths(-1)), new System.DateTime?(System.DateTime.Now), status, text, 1, 10);
			this.txtTestGetSoldTrades.Text = soldTrades;
		}

		protected void btnTestGetIncrementSoldTrades_Click(object sender, System.EventArgs e)
		{
			string status = (this.ddlStatus2.SelectedValue == "all") ? "" : this.ddlStatus1.SelectedValue;
			string text = this.txtBuyerName2.Text;
			string incrementSoldTrades = this.tradeApi.GetIncrementSoldTrades(System.DateTime.Now.AddDays(-1.0), System.DateTime.Now, status, text, 1, 10);
			this.txtTestGetIncrementSoldTrades.Text = incrementSoldTrades;
		}

		protected void btnTestChangLogistics_Click(object sender, System.EventArgs e)
		{
			string text = this.txtTradeId1.Text;
			string text2 = this.txtCompanyName1.Text;
			string text3 = this.txtTransId1.Text;
			string text4 = this.tradeApi.ChangLogistics(text, text2, text3);
			this.txtTestChangLogistics.Text = text4;
		}

		protected void btnTestGetTrade_Click(object sender, System.EventArgs e)
		{
			string text = this.txtTradeId2.Text;
			string trade = this.tradeApi.GetTrade(text);
			this.txtTestGetTrade.Text = trade;
		}

		protected void btnTestUpdateTradeMemo_Click(object sender, System.EventArgs e)
		{
			string text = this.txtTradeId3.Text;
			string text2 = this.txtRemark1.Text;
			int flag = Globals.ToNum(this.txtTag.Text);
			string text3 = this.tradeApi.UpdateTradeMemo(text, text2, flag);
			this.txtTestUpdateTradeMemo.Text = text3;
		}

		protected void btnTestSendLogistic_Click(object sender, System.EventArgs e)
		{
			string text = this.txtTradeId4.Text;
			string text2 = this.txtCompanyName4.Text;
			string text3 = this.txtTransId4.Text;
			string text4 = this.tradeApi.SendLogistic(text, text2, text3);
			this.txtSendLogistic.Text = text4;
		}
	}
}
