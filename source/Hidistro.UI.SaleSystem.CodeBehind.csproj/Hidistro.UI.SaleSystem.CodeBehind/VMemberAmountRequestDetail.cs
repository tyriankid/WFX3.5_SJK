using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VMemberAmountRequestDetail : VMemberTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litState;

		private System.Web.UI.WebControls.Literal litTradeType;

		private System.Web.UI.WebControls.Literal litAccountCode;

		private System.Web.UI.WebControls.Literal litRequestType;

		private System.Web.UI.WebControls.Literal litRequestTime;

		private System.Web.UI.WebControls.Literal litAmount;

		private System.Web.UI.WebControls.Literal litRemark;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VMemberAmountRequestDetail.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("提现详情");
			int serialid = Globals.RequestQueryNum("id");
			this.litState = (System.Web.UI.WebControls.Literal)this.FindControl("litState");
			this.litTradeType = (System.Web.UI.WebControls.Literal)this.FindControl("litTradeType");
			this.litAccountCode = (System.Web.UI.WebControls.Literal)this.FindControl("litAccountCode");
			this.litRequestType = (System.Web.UI.WebControls.Literal)this.FindControl("litRequestType");
			this.litRequestTime = (System.Web.UI.WebControls.Literal)this.FindControl("litRequestTime");
			this.litAmount = (System.Web.UI.WebControls.Literal)this.FindControl("litAmount");
			this.litRemark = (System.Web.UI.WebControls.Literal)this.FindControl("litRemark");
			MemberAmountRequestInfo amountRequestDetail = MemberAmountProcessor.GetAmountRequestDetail(serialid);
			if (amountRequestDetail != null)
			{
				string text = string.Empty;
				switch (amountRequestDetail.State)
				{
				case RequesState.驳回:
					text = "<span class='colorr'>已驳回</span>";
					break;
				case RequesState.未审核:
					text = "<span class='colory'>待审核</span>";
					break;
				case RequesState.已审核:
					text = "<span class='colorh'>已审核</span>";
					break;
				case RequesState.已发放:
					text = "<span class='colorg'>已发放</span>";
					break;
				case RequesState.发放异常:
					text = "<span class='colorr'>发放异常</span>";
					break;
				}
				this.litState.Text = text;
				this.litRequestType.Text = VShopHelper.GetCommissionPayType(((int)amountRequestDetail.RequestType).ToString());
				this.litAccountCode.Text = amountRequestDetail.AccountCode;
				this.litRequestTime.Text = amountRequestDetail.RequestTime.ToString("yyyy-MM-dd HH:mm:ss");
				this.litAmount.Text = amountRequestDetail.Amount.ToString();
				this.litRemark.Text = amountRequestDetail.Remark;
			}
		}
	}
}
