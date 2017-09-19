using Hidistro.ControlPanel.Store;
using Hidistro.Entities.Store;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Shop
{
	public class CustomPageEdit : AdminPage
	{
		public string id;

		public string scriptSrc = "/Templates/vshop/";

		public string cssSrc = "/Templates/vshop/";

		protected string scriptLinkSrc = string.Empty;

		protected string cssLinkSrc = string.Empty;

		public bool isModuleEdit;

		public int modelStatus;

		public CustomPage model;

		protected System.Web.UI.HtmlControls.HtmlInputHidden j_pageID;

		protected System.Web.UI.WebControls.Literal La_script;

		public CustomPageEdit() : base("m01", "dpp13")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!base.IsPostBack)
			{
				this.id = base.Request.QueryString["id"];
				this.model = CustomPageHelp.GetCustomPageByID(System.Convert.ToInt32(this.id));
				this.j_pageID.Value = this.id;
				this.La_script.Text = this.GetTemplatescript(this.model.TempIndexName);
				this.isModuleEdit = true;
				this.modelStatus = this.model.Status;
				if (!string.IsNullOrEmpty(this.model.TempIndexName) && this.model.TempIndexName != "none")
				{
					this.cssSrc = this.cssSrc + this.model.TempIndexName + "/css/head.css";
					this.scriptSrc = this.scriptSrc + this.model.TempIndexName + "/script/head.js";
					this.scriptLinkSrc = "<script src=\"" + this.scriptSrc + "\"></script>";
					this.cssLinkSrc = "<link rel=\"stylesheet\" href=\"" + this.cssSrc + "\">";
				}
			}
		}

		public string GetTemplatescript(string tempName)
		{
			if (string.IsNullOrEmpty(tempName))
			{
				return "";
			}
			string path = base.Server.MapPath("/Templates/vshop/ti/data/default.json");
			if (!string.IsNullOrEmpty(tempName))
			{
				path = base.Server.MapPath("/Templates/vshop/" + tempName + "/script/default.json");
			}
			System.IO.StreamReader streamReader = new System.IO.StreamReader(path, System.Text.Encoding.UTF8);
			string result;
			try
			{
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}
	}
}
