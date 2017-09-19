using Ajax;
using ASPNET.WebControls;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Store;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Globalization;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace Hidistro.UI.Web.Admin.Shop
{
	public class CustomPageManage : AdminPage
	{
		public const string tempFileDic = "/admin/shop/ShopEdit.aspx";

		public const string tempImgDic = "/Templates/vshop/";

		public int status = 1;

		public string tempLatePath = "";

		public string templateCuName = "";

		public string showUrl = "";

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.WebControls.Repeater Repeater1;

		protected Pager pager;

		public CustomPageManage() : base("m01", "dpp13")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Utility.RegisterTypeForAjax(typeof(CustomPageManage));
			if (!base.IsPostBack)
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				this.tempLatePath = masterSettings.VTheme;
				this.GetIndexName(this.tempLatePath);
				this.status = ((this.Page.Request.QueryString["Status"] != null) ? System.Convert.ToInt32(this.Page.Request.QueryString["Status"]) : 0);
				this.DataBind();
				this.GetIndexUrl();
			}
		}

		public void GetIndexUrl()
		{
			int port = base.Request.Url.Port;
			string text = (port == 80) ? "" : (":" + port.ToString());
			this.showUrl = string.Concat(new string[]
			{
				"http://",
				base.Request.Url.Host,
				text,
				Globals.ApplicationPath,
				"/default.aspx"
			});
		}

		public string GetPageUrl(string url)
		{
			int port = base.Request.Url.Port;
			string text = (port == 80) ? "" : (":" + port.ToString());
			return string.Concat(new string[]
			{
				"http://",
				base.Request.Url.Host,
				text,
				"/custom/",
				url
			});
		}

		public string GetDraftPageUrl(string url)
		{
			int port = base.Request.Url.Port;
			string text = (port == 80) ? "" : (":" + port.ToString());
			return string.Concat(new string[]
			{
				"http://",
				base.Request.Url.Host,
				text,
				"/draftcustom/",
				url
			});
		}

		public override void DataBind()
		{
			DbQueryResult pages = CustomPageHelp.GetPages(new CustomPageQuery
			{
				Name = this.Page.Request.QueryString["Name"],
				Status = new int?(this.status),
				PageIndex = this.pager.PageIndex,
				PageSize = this.pager.PageSize
			});
			this.Repeater1.DataSource = pages.Data;
			this.Repeater1.DataBind();
			this.pager.TotalRecords = pages.TotalRecords;
		}

		[AjaxMethod]
		public bool DeleteCustomPage(int id)
		{
			return id >= 1 && CustomPageHelp.DeletePage(id);
		}

		public string GetTempLateLogicName(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				return fileName.Substring(0, base.Eval("Name").ToString().LastIndexOf("."));
			}
			return "ti";
		}

		public void GetIndexName(string tempName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(base.Server.MapPath("/Templates/vshop/" + tempName));
			string text = directoryInfo.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			if (text.Length > 0 && !text.StartsWith("_"))
			{
				System.IO.FileInfo[] files = directoryInfo.GetFiles("template.xml");
				System.IO.FileInfo[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					System.IO.FileInfo fileInfo = array[i];
					System.IO.FileStream fileStream = fileInfo.OpenRead();
					xmlDocument.Load(fileStream);
					fileStream.Close();
					this.templateCuName = xmlDocument.SelectSingleNode("root/Name").InnerText;
				}
			}
		}

		public string GetImgName(string fileName)
		{
			return "/Templates/vshop/" + fileName + "/default.png";
		}

		public string GetTempUrl(string tempLateLogicName)
		{
			if (!string.IsNullOrEmpty(tempLateLogicName))
			{
				return "/admin/shop/ShopEdit.aspx?tempName=" + tempLateLogicName;
			}
			return "/admin/shop/ShopEdit.aspx?tempName=ti";
		}
	}
}
