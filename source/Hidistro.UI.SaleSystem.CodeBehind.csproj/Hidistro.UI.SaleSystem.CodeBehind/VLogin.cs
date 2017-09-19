using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VLogin : VshopTemplatedWebControl
	{
		private string sessionId;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "skin-VLogin.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.Page.Response.Redirect("Default.aspx");
		}
	}
}
