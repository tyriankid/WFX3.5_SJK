using Ajax;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Entities.Store;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace Hidistro.UI.Web.Admin.Shop
{
	public class SelectTemplate : AdminPage
	{
		public const string tempFileDic = "/admin/shop/ShopEdit.aspx";

		public const string tempImgDic = "/Templates/vshop/";

		public string tempLatePath = "";

		public string templateCuName = "";

		public string showUrl = "";

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.WebControls.Repeater Repeater1;

		public SelectTemplate() : base("m01", "dpp13")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Utility.RegisterTypeForAjax(typeof(SelectTemplate));
			if (!base.IsPostBack)
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
				SettingsManager.GetMasterSettings(true);
				this.DataBind();
			}
		}

		public override void DataBind()
		{
			this.Repeater1.DataSource = this.LoadThemes();
			this.Repeater1.DataBind();
		}

		public string GetImgName(string fileName)
		{
			if (string.IsNullOrEmpty(fileName) || fileName == "none")
			{
				return "/Templates/vshop/custom/default/empty.jpg";
			}
			return "/Templates/vshop/" + fileName + "/default.png";
		}

		[AjaxMethod]
		public int CreateCustomTemplate(string tempName)
		{
			if (string.IsNullOrEmpty(tempName))
			{
				return 0;
			}
			CustomPage customPage = new CustomPage();
			customPage.Name = "页面名称";
			customPage.CreateTime = System.DateTime.Now;
			customPage.Status = 1;
			customPage.TempIndexName = ((tempName == "none") ? "" : tempName);
			customPage.PageUrl = customPage.CreateTime.ToString("yyyyMMddHHmmss");
			customPage.Details = "自定义页面";
			customPage.IsShowMenu = true;
			customPage.DraftDetails = "自定义页面";
			customPage.DraftName = "页面名称";
			customPage.DraftPageUrl = customPage.PageUrl;
			customPage.PV = 0;
			customPage.DraftJson = ((tempName == "none") ? this.GetCustomTempDefaultJson() : this.GetIndexTempJsonByName(tempName));
			customPage.FormalJson = ((tempName == "none") ? this.GetCustomTempDefaultJson() : this.GetIndexTempJsonByName(tempName));
			return CustomPageHelp.Create(customPage);
		}

		public string GetCustomTempDefaultJson()
		{
			System.IO.StreamReader streamReader = new System.IO.StreamReader(base.Server.MapPath("/Templates/vshop/custom/default/data/default.json"), System.Text.Encoding.UTF8);
			string result;
			try
			{
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				text = text.Replace("\r\n", "").Replace("\n", "");
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}

		public string GetIndexTempJsonByName(string tempName)
		{
			System.IO.StreamReader streamReader = new System.IO.StreamReader(base.Server.MapPath("/Templates/vshop/" + tempName + "/data/default.json"), System.Text.Encoding.UTF8);
			string result;
			try
			{
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				text = text.Replace("\r\n", "").Replace("\n", "");
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}

		protected System.Collections.Generic.IList<ShopIndex.ManageThemeInfo> LoadThemes()
		{
			XmlDocument xmlDocument = new XmlDocument();
			System.Collections.Generic.IList<ShopIndex.ManageThemeInfo> list = new System.Collections.Generic.List<ShopIndex.ManageThemeInfo>();
			string[] array = System.IO.Directory.Exists(base.Server.MapPath("/Templates/vshop/")) ? System.IO.Directory.GetDirectories(base.Server.MapPath("/Templates/vshop/")) : null;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string path = array2[i];
				System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
				string text = directoryInfo.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (text.Length > 0 && !text.StartsWith("_"))
				{
					System.IO.FileInfo[] files = directoryInfo.GetFiles("template.xml");
					System.IO.FileInfo[] array3 = files;
					for (int j = 0; j < array3.Length; j++)
					{
						System.IO.FileInfo fileInfo = array3[j];
						ShopIndex.ManageThemeInfo manageThemeInfo = new ShopIndex.ManageThemeInfo();
						System.IO.FileStream fileStream = fileInfo.OpenRead();
						xmlDocument.Load(fileStream);
						fileStream.Close();
						manageThemeInfo.Name = xmlDocument.SelectSingleNode("root/Name").InnerText;
						manageThemeInfo.ThemeName = text;
						if (text == this.tempLatePath)
						{
							this.templateCuName = xmlDocument.SelectSingleNode("root/Name").InnerText;
						}
						list.Add(manageThemeInfo);
					}
				}
			}
			list.Insert(0, new ShopIndex.ManageThemeInfo
			{
				Name = "空白模板",
				ThemeImgUrl = "/admin/shop/Public/images/empty.jpg",
				ThemeName = "none"
			});
			return list;
		}
	}
}
