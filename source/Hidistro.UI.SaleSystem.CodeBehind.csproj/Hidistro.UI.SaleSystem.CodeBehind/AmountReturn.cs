using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class AmountReturn : RePaymentTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litMessage;

		public AmountReturn() : base(false)
		{
		}

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-RePaymentReturn.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.litMessage = (System.Web.UI.WebControls.Literal)this.FindControl("litMessage");
		}

		protected override void DisplayMessage(string status)
		{
			if (status != null)
			{
				if (status == "ordernotfound")
				{
					this.litMessage.Text = string.Format("没有找到对应的充值信息，充值号：{0}", this.PayId);
					return;
				}
				if (status == "gatewaynotfound")
				{
					this.litMessage.Text = "没有找到与此充值方式对应的支付方式，系统无法自动完成操作，请联系管理员";
					return;
				}
				if (status == "verifyfaild")
				{
					this.litMessage.Text = "支付返回验证失败，操作已停止";
					return;
				}
				if (status == "success")
				{
					this.litMessage.Text = string.Format("恭喜您，充值已成功完成支付：{0}</br>支付金额：{1}", this.PayId, this.Amount.ToString("F"));
					return;
				}
				if (status == "fail")
				{
					this.litMessage.Text = string.Format("充值支付已成功，但是系统在处理过程中遇到问题，请联系管理员</br>支付金额：{0}", this.Amount.ToString("F"));
					return;
				}
			}
			this.litMessage.Text = "未知错误，操作已停止";
		}
	}
}
