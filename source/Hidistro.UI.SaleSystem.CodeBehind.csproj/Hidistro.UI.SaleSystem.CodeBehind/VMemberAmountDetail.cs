using Hidistro.ControlPanel.Members;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VMemberAmountDetail : VMemberTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litPayId;

		private System.Web.UI.WebControls.Literal litTradeType;

		private System.Web.UI.WebControls.Literal litTrade;

		private System.Web.UI.WebControls.Literal litTradeAmount;

		private System.Web.UI.WebControls.Literal litTradeWays;

		private System.Web.UI.WebControls.Literal litTradeTime;

		private System.Web.UI.WebControls.Literal litAmount;

		private System.Web.UI.WebControls.Literal litRemark;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VMemberAmountDetail.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("收支详情");
			int id = Globals.RequestQueryNum("id");
			this.litPayId = (System.Web.UI.WebControls.Literal)this.FindControl("litPayId");
			this.litTradeType = (System.Web.UI.WebControls.Literal)this.FindControl("litTradeType");
			this.litTrade = (System.Web.UI.WebControls.Literal)this.FindControl("litTrade");
			this.litTradeAmount = (System.Web.UI.WebControls.Literal)this.FindControl("litTradeAmount");
			this.litTradeWays = (System.Web.UI.WebControls.Literal)this.FindControl("litTradeWays");
			this.litTradeTime = (System.Web.UI.WebControls.Literal)this.FindControl("litTradeTime");
			this.litAmount = (System.Web.UI.WebControls.Literal)this.FindControl("litAmount");
			this.litRemark = (System.Web.UI.WebControls.Literal)this.FindControl("litRemark");
			MemberAmountDetailedInfo amountDetail = MemberAmountProcessor.GetAmountDetail(id);
			if (amountDetail != null)
			{
				this.litPayId.Text = amountDetail.PayId;
				this.litTradeType.Text = MemberHelper.GetEnumDescription(amountDetail.TradeType);
				this.litTrade.Text = ((amountDetail.TradeAmount > 0m) ? "收入" : "支出");
				this.litTradeAmount.Text = amountDetail.TradeAmount.ToString();
				this.litTradeWays.Text = MemberHelper.GetEnumDescription(amountDetail.TradeWays);
				this.litTradeTime.Text = amountDetail.TradeTime.ToString();
				this.litAmount.Text = amountDetail.AvailableAmount.ToString();
				this.litRemark.Text = amountDetail.Remark;
			}
		}
	}
}
