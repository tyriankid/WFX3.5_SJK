using Hidistro.Core;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VLoginOut : SimpleTemplatedWebControl
	{
		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VLogout.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			string str = Globals.GetCurrentDistributorId().ToString();
			Globals.ClearUserCookie();
			System.Web.UI.WebControls.Literal literal = (System.Web.UI.WebControls.Literal)this.FindControl("litJs");
			literal.Text = "<script type=\"text/javascript\">window.location.href='/default.aspx?ReferralId=" + str + "'</script>";
			PageTitle.AddSiteNameTitle("退出登录");
		}
	}
}
