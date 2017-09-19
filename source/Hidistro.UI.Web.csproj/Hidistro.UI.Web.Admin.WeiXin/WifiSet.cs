using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace Hidistro.UI.Web.Admin.WeiXin
{
	public class WifiSet : AdminPage
	{
		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		protected Script Script5;

		protected Script Script6;

		protected System.Web.UI.WebControls.TextBox txt_wifiName;

		protected System.Web.UI.WebControls.TextBox txt_wifiPwd;

		protected System.Web.UI.WebControls.TextBox txt_wifiDescribe;

		protected System.Web.UI.WebControls.HiddenField hd_id;

		protected System.Web.UI.WebControls.Button btnSave;

		protected WifiSet() : base("m06", "wxp11")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				if (base.Request.QueryString["action"] == "edit")
				{
					this.InitData();
					return;
				}
				if (base.Request.QueryString["action"] == "delete")
				{
					this.DeleteData();
				}
			}
		}

		protected void InitData()
		{
			string text = base.Request.QueryString["id"];
			System.Data.DataSet dataSet = new System.Data.DataSet();
			string text2 = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			if (System.IO.File.Exists(text2))
			{
				dataSet.ReadXml(text2);
				if (dataSet != null)
				{
					foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
					{
						if (dataRow["id"].ToString() == text)
						{
							this.hd_id.Value = text;
							this.txt_wifiName.Text = dataRow["wifiName"].ToString();
							this.txt_wifiPwd.Text = dataRow["wifiPwd"].ToString();
							this.txt_wifiDescribe.Text = dataRow["wifiDescribe"].ToString();
						}
					}
				}
			}
		}

		protected void DeleteData()
		{
			string id = base.Request.QueryString["id"];
			WifiSet.DeleteXML(id);
			base.Response.Redirect("WifiSetList.aspx");
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			int num = System.Convert.ToInt32(this.hd_id.Value);
			string text = this.txt_wifiName.Text.Trim();
			string text2 = this.txt_wifiPwd.Text.Trim();
			string wifiDescribe = this.txt_wifiDescribe.Text.Trim();
            string.Concat("wifi_" + text + "|" + text2);
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				if (num > 0)
				{
					WifiSet.UpdateXML(num, text, text2, wifiDescribe);
				}
				else
				{
					WifiSet.InsertXMl(text, text2, wifiDescribe);
				}
				base.Response.Redirect("WifiSetList.aspx");
			}
		}

		internal static void InsertXMl(string wifiName, string wifiPwd, string wifiDescribe)
		{
			string text = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			int num = 100;
			XmlDocument xmlDocument = new XmlDocument();
			if (!System.IO.File.Exists(text))
			{
				XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
				XmlElement documentElement = xmlDocument.DocumentElement;
				xmlDocument.InsertBefore(newChild, documentElement);
				XmlElement newChild2 = xmlDocument.CreateElement("WifiConfig");
				xmlDocument.AppendChild(newChild2);
				xmlDocument.Save(text);
			}
			xmlDocument.Load(text);
			XmlNodeList childNodes = xmlDocument.SelectSingleNode("WifiConfig").ChildNodes;
			if (childNodes.Count > 0)
			{
				num = System.Convert.ToInt32(xmlDocument.DocumentElement.SelectSingleNode("/WifiConfig/WifiConfigs[last()]").Attributes["id"].Value) + 1;
			}
			XmlElement xmlElement = xmlDocument.CreateElement("WifiConfigs");
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("id");
			xmlAttribute.Value = num.ToString();
			xmlElement.Attributes.Append(xmlAttribute);
			XmlElement xmlElement2 = xmlDocument.CreateElement("WifiName");
			xmlElement2.InnerText = wifiName;
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xmlDocument.CreateElement("WifiPwd");
			xmlElement3.InnerText = wifiPwd;
			xmlElement.AppendChild(xmlElement3);
			XmlElement xmlElement4 = xmlDocument.CreateElement("WifiDescribe");
			xmlElement4.InnerText = wifiDescribe;
			xmlElement.AppendChild(xmlElement4);
			xmlDocument.DocumentElement.AppendChild(xmlElement);
			xmlDocument.Save(text);
		}

		internal static void UpdateXML(int id, string wifiName, string wifiPwd, string wifiDescribe)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string filename = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			xmlDocument.Load(filename);
			XmlNodeList childNodes = xmlDocument.SelectSingleNode("WifiConfig").ChildNodes;
			foreach (XmlNode xmlNode in childNodes)
			{
				XmlElement xmlElement = (XmlElement)xmlNode;
				if (xmlElement.GetAttribute("id") == id.ToString())
				{
					XmlNodeList childNodes2 = xmlElement.ChildNodes;
					foreach (XmlNode xmlNode2 in childNodes2)
					{
						XmlElement xmlElement2 = (XmlElement)xmlNode2;
						if (xmlElement2.Name == "WifiName")
						{
							xmlElement2.InnerText = wifiName;
						}
						else if (xmlElement2.Name == "WifiPwd")
						{
							xmlElement2.InnerText = wifiPwd;
						}
						else if (xmlElement2.Name == "WifiDescribe")
						{
							xmlElement2.InnerText = wifiDescribe;
						}
					}
				}
			}
			xmlDocument.Save(filename);
		}

		internal static void DeleteXML(string id)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string filename = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			xmlDocument.Load(filename);
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("/WifiConfig/WifiConfigs[@id=" + id + "]");
			if (xmlNode != null)
			{
				xmlNode.ParentNode.RemoveChild(xmlNode);
			}
			xmlDocument.Save(filename);
		}
	}
}
