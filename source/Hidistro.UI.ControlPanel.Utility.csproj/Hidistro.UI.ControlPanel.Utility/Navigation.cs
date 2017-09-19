using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Entities.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
namespace Hidistro.UI.ControlPanel.Utility
{
	public class Navigation
	{
		private const string NavigationCacheKey = "FileCache-AdminNavigation";

		private readonly XmlDocument _xmlDoc;

		private static readonly Dictionary<string, NavModule> _moduleList = new Dictionary<string, NavModule>();

		public Dictionary<string, NavModule> ModuleList
		{
			get
			{
				return Navigation._moduleList;
			}
		}

		public static Navigation GetNavigation(bool isUserCache = true)
		{
			Navigation navigation;
			Navigation result;
			if (isUserCache)
			{
				navigation = (HiCache.Get("FileCache-AdminNavigation") as Navigation);
				if (navigation != null)
				{
					result = navigation;
					return result;
				}
			}
			HttpContext current = HttpContext.Current;
			string filename;
			if (current != null)
			{
				try
				{
					filename = current.Request.MapPath("~/Admin/MenuNew.xml");
				}
				catch
				{
					filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Admin\\MenuNew.xml");
				}
			}
			else
			{
				filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Admin\\MenuNew.xml");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			navigation = new Navigation(xmlDocument);
			HiCache.Max("FileCache-AdminNavigation", navigation, new CacheDependency(filename));
			result = navigation;
			return result;
		}

		private Navigation()
		{
		}

		private Navigation(XmlDocument doc)
		{
			this._xmlDoc = doc;
			this.LoadNavigationFromXml();
		}

		private void LoadNavigationFromXml()
		{
			XmlNodeList xmlNodeList = this._xmlDoc.SelectNodes("Menu/Module");
			if (xmlNodeList != null && xmlNodeList.Count != 0)
			{
				Navigation._moduleList.Clear();
				foreach (XmlNode xmlNode in xmlNodeList)
				{
					NavModule navModule = new NavModule
					{
						Title = xmlNode.Attributes["Title"].Value,
						ID = xmlNode.Attributes["ID"].Value
					};
					XmlAttribute xmlAttribute = xmlNode.Attributes["Link"];
					if (xmlAttribute != null)
					{
						navModule.Link = (xmlAttribute.Value.StartsWith("http") ? xmlAttribute.Value : (Globals.ApplicationPath + "/Admin/" + xmlAttribute.Value));
					}
					XmlAttribute xmlAttribute2 = xmlNode.Attributes["IsDivide"];
					if (xmlAttribute2 != null && xmlAttribute2.Value.ToLower() == "true")
					{
						navModule.IsDivide = true;
					}
					XmlAttribute xmlAttribute3 = xmlNode.Attributes["Class"];
					if (xmlAttribute3 != null)
					{
						navModule.Class = xmlAttribute3.Value;
					}
					this.LoadItems(navModule, xmlNode);
					Navigation._moduleList.Add(navModule.ID, navModule);
				}
			}
		}

		private void LoadItems(NavModule module, XmlNode moduleNode)
		{
			XmlNodeList xmlNodeList = moduleNode.SelectNodes("Item");
			if (xmlNodeList != null && xmlNodeList.Count != 0)
			{
				foreach (XmlNode xmlNode in xmlNodeList)
				{
					NavItem navItem = new NavItem();
					XmlAttribute xmlAttribute = xmlNode.Attributes["Title"];
					if (xmlAttribute != null)
					{
						navItem.SpanName = xmlAttribute.Value;
					}
					XmlAttribute xmlAttribute2 = xmlNode.Attributes["Class"];
					if (xmlAttribute2 != null)
					{
						navItem.Class = xmlAttribute2.Value;
					}
					navItem.ID = xmlNode.Attributes["ID"].Value;
					module.ItemList.Add(navItem.ID, navItem);
					this.LoadPageLinks(navItem, xmlNode);
				}
			}
		}

		private void LoadPageLinks(NavItem item, XmlNode itemNode)
		{
			XmlNodeList xmlNodeList = itemNode.SelectNodes("PageLink");
			if (xmlNodeList != null && xmlNodeList.Count != 0)
			{
				foreach (XmlNode xmlNode in xmlNodeList)
				{
					NavPageLink navPageLink = new NavPageLink
					{
						ID = xmlNode.Attributes["ID"].Value,
						Title = xmlNode.Attributes["Title"].Value
					};
					XmlAttribute xmlAttribute = xmlNode.Attributes["Link"];
					if (xmlAttribute != null)
					{
						navPageLink.Link = (xmlAttribute.Value.StartsWith("http") ? xmlAttribute.Value : (Globals.ApplicationPath + "/Admin/" + xmlAttribute.Value));
					}
					XmlAttribute xmlAttribute2 = xmlNode.Attributes["Class"];
					if (xmlAttribute2 != null)
					{
						navPageLink.Class = xmlAttribute2.Value;
					}
					XmlAttribute xmlAttribute3 = xmlNode.Attributes["Style"];
					if (xmlAttribute3 != null)
					{
						navPageLink.Style = xmlAttribute3.Value;
					}
					XmlAttribute xmlAttribute4 = xmlNode.Attributes["Target"];
					if (xmlAttribute4 != null)
					{
						navPageLink.Target = xmlAttribute4.Value;
					}
					item.PageLinks.Add(navPageLink.ID, navPageLink);
				}
			}
		}

		private void CheckRolePermission(NavModule module)
		{
			string link = module.Link;
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (KeyValuePair<string, NavItem> current in module.ItemList)
			{
				foreach (KeyValuePair<string, NavPageLink> current2 in current.Value.PageLinks)
				{
					if (string.Equals(link, current2.Value.Link, StringComparison.CurrentCultureIgnoreCase))
					{
						text = current2.Value.ID;
						break;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
			}
			int roleId = 0;
			bool flag = false;
			Globals.GetCurrentManagerUserId(out roleId, out flag);
			if (!ManagerHelper.IsHavePermission(module.ID, text, roleId))
			{
				foreach (KeyValuePair<string, NavItem> current in module.ItemList)
				{
					foreach (KeyValuePair<string, NavPageLink> current2 in current.Value.PageLinks)
					{
						bool flag2 = ManagerHelper.IsHavePermission(module.ID, current2.Value.ID, roleId);
						if (flag2)
						{
							text2 = current2.Value.Link;
							break;
						}
					}
					if (!string.IsNullOrEmpty(text2))
					{
						break;
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					module.Link = text2;
				}
			}
		}

		public string RenderTopMenu(string currentModuleId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<ul class=\"clearfix\">");

            bool isdefault =true; //是否默认所有权限 1是  0否
            ManagerInfo currentManager = ManagerHelper.GetCurrentManager();
            isdefault= ManagerHelper.GetRole(currentManager.RoleId).IsDefault;  
            IList<RolePermissionInfo>  list= ManagerHelper.GetRolePremissonsByRoleId(currentManager.RoleId); //当前角色权限合集
          

            foreach (NavModule current in Navigation._moduleList.Values)
			{
                if (!isdefault)  //如果是部分权限就进入判断 所有权限不进来
                {
                    bool flag = false;
                    foreach (RolePermissionInfo item in list)
                    {
                        if (item.PermissionId.Split('_')[0] == current.ID) //当前大模块与权限匹配 flag=true
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)  //当前大模块与权限不匹配跳出当前循环不添加到html中
                    {
                        continue;
                    }
                }
                  string text = (!string.IsNullOrEmpty(currentModuleId) && currentModuleId == current.ID) ? (current.Class + " active") : current.Class;
				if (current.IsDivide)
				{
					stringBuilder.Append("<li class=\"divide\">|</li>").AppendLine();
				}
                if (!string.IsNullOrEmpty(text))
                {
                   
                       
                    stringBuilder.AppendFormat("<li class=\"{0}\" mid=\"{1}\"><a href=\"{2}\" title=\"{3}\">{3}</a></li>", new object[]
                    {
                        text,
                        current.ID,
                        current.Link,
                        current.Title
                    }).AppendLine();
                  
                    
				}
				else
				{
                  stringBuilder.AppendFormat("<li mid=\"{0}\"><a href=\"{1}\" title=\"{2}\">{2}</a></li>", current.ID, current.Link, current.Title).AppendLine();
                         
				}
			}
			stringBuilder.AppendLine("</ul>");
			return stringBuilder.ToString();
		}

		public string RenderLeftMenu(string currentModuleId, string currentPageId)
		{
			NavModule navModule = Navigation._moduleList[currentModuleId];
			StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilderTwo = new StringBuilder();
            StringBuilder stringBuilderThree = new StringBuilder();
            
            foreach (NavItem current in navModule.ItemList.Values)
			{
               
                stringBuilderTwo = new StringBuilder("");
                stringBuilderThree = new StringBuilder("");
                if (!string.IsNullOrEmpty(current.SpanName))
				{
                    stringBuilderTwo.AppendFormat("<em class=\"{0}\"></em>", current.Class).AppendLine();
                    stringBuilderTwo.AppendFormat("<span>{0}</span>", current.SpanName).AppendLine();
				}

                stringBuilderTwo.AppendFormat("<ul iid=\"{0}\">", current.ID).AppendLine();

                stringBuilderThree = this.AppendLinks( current, currentPageId);

                stringBuilderTwo.AppendLine(stringBuilderThree.ToString());
                stringBuilderTwo.AppendLine("</ul>");
                if (stringBuilderThree.ToString() != "") //下面有内容才加进去
                {
                    stringBuilder.Append(stringBuilderTwo);
                }
            }
            
            return stringBuilder.ToString();
		}

		private StringBuilder AppendLinks( NavItem item, string currentPageId)
		{

            StringBuilder leftMenu = new StringBuilder("");
            foreach (NavPageLink current in item.PageLinks.Values)
			{

                bool isdefault = true; //是否默认所有权限 1是  0否
                ManagerInfo currentManager = ManagerHelper.GetCurrentManager();
                isdefault = ManagerHelper.GetRole(currentManager.RoleId).IsDefault;
                IList<RolePermissionInfo> list = ManagerHelper.GetRolePremissonsByRoleId(currentManager.RoleId); //当前角色权限合集

                if (!isdefault)  //如果是部分权限就进入判断 所有权限不进来
                {
                    bool flag = false;
                    foreach (RolePermissionInfo iteminfo in list)
                    {
                        if (iteminfo.PermissionId.Split('_')[1] == current.ID) //当前大模块与权限匹配 flag=true
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)  //当前大模块与权限不匹配跳出当前循环不添加到html中
                    {
                        continue;
                    }
                }


                string text = (!string.IsNullOrEmpty(currentPageId) && currentPageId == current.ID) ? (current.Class + " active") : current.Class;
				leftMenu.Append("<li");
				if (!string.IsNullOrEmpty(text))
				{
					leftMenu.AppendFormat(" class=\"{0}\"", text);
				}
				if (!string.IsNullOrEmpty(current.Style))
				{
					leftMenu.AppendFormat(" style=\"{0}\"", current.Style);
				}
				leftMenu.AppendFormat("><a href=\"{0}\" title=\"{1}\"", current.Link, current.Title);
				if (!string.IsNullOrEmpty(current.Target))
				{
					leftMenu.AppendFormat(" target=\"{0}\"", current.Target);
				}
				leftMenu.AppendFormat(">{0}</a></li>", current.Title).AppendLine();
                
			}
            return leftMenu;




        }
	}
}
