using System;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class CustomDraftHomePage : CustomDraftTemplatedWebControl
	{
		[System.ComponentModel.Bindable(true)]
		public string TempFilePath
		{
			get;
			set;
		}

		[System.ComponentModel.Bindable(true)]
		public string CustomPagePath
		{
			get;
			set;
		}

		protected override string SkinPath
		{
			get
			{
				string text = "/Templates/vshop/custom/draft/" + this.CustomPagePath + "/" + this.SkinName;
				if (!System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(text)))
				{
					System.Web.HttpContext.Current.Response.Redirect("/Default.aspx");
				}
				return text;
			}
		}

		protected override void OnInit(System.EventArgs e)
		{
			this.TempFilePath = "Skin-HomePage.html";
			if (this.SkinName == null)
			{
				this.SkinName = this.TempFilePath;
			}
			base.OnInit(e);
		}
	}
}
