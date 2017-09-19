using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hishop.Components.Validation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Linq;

namespace Hidistro.UI.ControlPanel.Utility
{
	public class AdminPage : Page
	{
        /// <summary>
        /// 获取模块ID
        /// </summary>
		public string ModuleId
		{
			get;
			private set;
		}

        /// <summary>
        /// 获取栏目页ID
        /// </summary>
		public string PageId
		{
			get;
			private set;
		}

        /// <summary>
        /// 获取子栏目页ID
        /// </summary>
		public string SubPageId
		{
			get;
			private set;
		}

		private AdminPage()
		{
		}

		protected AdminPage(string moduleId, string pageId) : this(moduleId, pageId, null)
		{
		}

		protected AdminPage(string moduleId, string pageId, string subPageId)
		{
			this.ModuleId = moduleId;
			this.PageId = pageId;
			this.SubPageId = subPageId;
		}

        /// <summary>
        /// 检查权限
        /// </summary>
		protected override void OnInit(EventArgs e)
		{
			if (ConfigurationManager.AppSettings["Installer"] != null)
			{
				base.Response.Redirect(Globals.ApplicationPath + "/installer/default.aspx", false);
			}
			else
			{
                ManagerInfo currentManager = ManagerHelper.GetCurrentManager();
                if (currentManager != null)
                {
                    DistributorsInfo siteInfo = DistributorsBrower.GetDistributorInfo(currentManager.SiteId);
                    if (siteInfo != null)
                    {
                        if (siteInfo.ReferralStatus == 1) //冻结站点不允许进入后台管理
                        {
                            ShowMsgAndReUrl("您的账号已被冻结！请联系管理员！",false, Globals.ApplicationPath + "/admin/LoginExit.aspx");
                            return;
                        }
                    }
                }
                this.CheckPageAccess();
                base.OnInit(e);


            }
		}

		protected virtual void ShowMsg(ValidationResults validateResults)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ValidationResult current in ((IEnumerable<ValidationResult>)validateResults))
			{
				stringBuilder.Append(Formatter.FormatErrorMessage(current.Message));
			}
			this.ShowMsg(stringBuilder.ToString(), false);
		}

		protected virtual void ShowMsg(string msg, bool success)
		{
			string str = string.Format("HiTipsShow(\"{0}\", {1})", msg, success ? "'success'" : "'error'");
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("ServerMessageScript"))
			{
				this.Page.ClientScript.RegisterStartupScript(base.GetType(), "ServerMessageScript", "<script language='JavaScript' defer='defer'>setTimeout(function(){" + str + "},300);</script>");
			}
		}

		protected virtual void ShowMsgToTarget(string msg, bool success, string targentname)
		{
			string arg = string.Empty;
			string text = targentname.ToLower();
			if (text != null)
			{
				if (text == "parent" || text == "top")
				{
					arg = targentname + ".";
				}
			}
			string str = string.Format("{2}HiTipsShow(\"{0}\", {1})", msg, success ? "'success'" : "'error'", arg);
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("ServerMessageScript"))
			{
				this.Page.ClientScript.RegisterStartupScript(base.GetType(), "ServerMessageScript", "<script language='JavaScript' defer='defer'>setTimeout(function(){" + str + "},300);</script>");
			}
		}

		protected virtual void ShowMsgAndReUrl(string msg, bool success, string url)
		{
			string str = string.Format("ShowMsgAndReUrl(\"{0}\", {1}, \"{2}\")", msg, success ? "true" : "false", url);
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("ServerMessageScript"))
			{
				this.Page.ClientScript.RegisterStartupScript(base.GetType(), "ServerMessageScript", "<script language='JavaScript' defer='defer'>setTimeout(function(){" + str + "},300);</script>");
			}
		}

		protected virtual void ShowMsgAndReUrl(string msg, bool success, string url, string target)
		{
			string str = string.Format("ShowMsgAndReUrl(\"{0}\", {1}, \"{2}\",\"{3}\")", new object[]
			{
				msg,
				success ? "true" : "false",
				url,
				target
			});
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("ServerMessageScript"))
			{
				this.Page.ClientScript.RegisterStartupScript(base.GetType(), "ServerMessageScript", "<script language='JavaScript' defer='defer'>setTimeout(function(){" + str + "},300);</script>");
			}
		}

		protected void ReloadPage(NameValueCollection queryStrings)
		{
			base.Response.Redirect(this.GenericReloadUrl(queryStrings));
		}

		protected void ReloadPage(NameValueCollection queryStrings, bool endResponse)
		{
			base.Response.Redirect(this.GenericReloadUrl(queryStrings), endResponse);
		}

		private string GenericReloadUrl(NameValueCollection queryStrings)
		{
			string result;
			if (queryStrings == null || queryStrings.Count == 0)
			{
				result = base.Request.Url.AbsolutePath;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.Request.Url.AbsolutePath).Append("?");
				foreach (string text in queryStrings.Keys)
				{
					string text2 = queryStrings[text].Trim().Replace("'", "");
					if (!string.IsNullOrEmpty(text2) && text2.Length > 0)
					{
						stringBuilder.Append(text).Append("=").Append(base.Server.UrlEncode(text2)).Append("&");
					}
				}
				queryStrings.Clear();
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				result = stringBuilder.ToString();
			}
			return result;
		}

		protected int GetUrlIntParam(string name)
		{
			string value = base.Request.QueryString.Get(name);
			int result;
			if (string.IsNullOrEmpty(value))
			{
				result = 0;
			}
			else
			{
				try
				{
					result = Convert.ToInt32(value);
				}
				catch (FormatException)
				{
					result = 0;
				}
			}
			return result;
		}

		protected string GetUrlParam(string name)
		{
			string text = base.Request.QueryString.Get(name);
			string result;
			if (string.IsNullOrEmpty(text))
			{
				result = "";
			}
			else
			{
				result = text;
			}
			return result;
		}

		protected int GetFormIntParam(string name)
		{
			string value = base.Request.Form.Get(name);
			int result;
			if (string.IsNullOrEmpty(value))
			{
				result = 0;
			}
			else
			{
				try
				{
					result = Convert.ToInt32(value);
				}
				catch (FormatException)
				{
					result = 0;
				}
			}
			return result;
		}

		protected string CutWords(object obj, int length)
		{
			string result;
			if (obj == null)
			{
				result = string.Empty;
			}
			else
			{
				string text = obj.ToString();
				if (text.Length > length)
				{
					result = text.Substring(0, length) + "......";
				}
				else
				{
					result = text;
				}
			}
			return result;
		}

		protected bool GetUrlBoolParam(string name)
		{
			string value = base.Request.QueryString.Get(name);
			bool result;
			if (string.IsNullOrEmpty(value))
			{
				result = false;
			}
			else
			{
				try
				{
					result = Convert.ToBoolean(value);
				}
				catch (FormatException)
				{
					result = false;
				}
			}
			return result;
		}

		protected void GotoResourceNotFound()

		{
			base.Response.Redirect(Globals.ApplicationPath + "/Admin/NotPermisson.aspx?type=1", true);
		}

		private void CheckPageAccess()
		{
			int roleId = 0;
			bool flag = false;
			if (Globals.GetCurrentManagerUserId(out roleId, out flag) == 0)
			{
				this.Page.Response.Redirect(Globals.ApplicationPath + "/admin/Login.aspx", true);
			}
            if (!flag)
            {
                if (!ManagerHelper.IsHavePermission(this.ModuleId, this.PageId, roleId))
                {
                    //如果进入的页面没有权限,就跳到最近的一个有权限的子页面
                    IList<RolePermissionInfo> list = ManagerHelper.GetRolePremissonsByRoleId(ManagerHelper.GetCurrentManager().RoleId); //当前角色权限合集
                    var listModule = list.Where(p => p.PermissionId.StartsWith(this.ModuleId + "_"));
                    if (listModule.Count() > 0)
                    {
                        //得到当前XML中对应栏目的所有子节点
                        Dictionary<string, NavModule> _moduleList = Navigation.GetNavigation(true).ModuleList;
                        var _modelParentLinq = from item in _moduleList
                                               select new { ID = item.Value.ID, Link = item.Value.Link, ItemList = item.Value.ItemList };
                        var _modelChildList = _modelParentLinq.Where(p => p.ID.Equals(listModule.First().PermissionId.Split('_')[0])).First().ItemList;

                        //获取对应栏目有权限的第一个子节点
                        NavPageLink currNavPageLink = null;
                        foreach (var item in _modelChildList.Values)
                        {
                            bool isFind = false;
                            foreach (var pageLink in item.PageLinks.Values)
                            {
                                if (pageLink.ID == listModule.First().PermissionId.Split('_')[1])
                                {
                                    currNavPageLink = pageLink;
                                    isFind = true;
                                    break;
                                }
                            }
                            if (isFind) break;
                        }

                        //有对应栏目的权限子节点，则跳转
                        if (currNavPageLink != null)
                        {
                            base.Response.Redirect(Globals.ApplicationPath + currNavPageLink.Link, true);
                            return;
                        }
                    }

                    //默认处理
                    this.NotPremissonRedirect(this.ModuleId, roleId, null);
                }
            }
		}

		private void NotPremissonRedirect(string moduleId, int roleId, string msg = null)
		{
			Navigation navigation = Navigation.GetNavigation(true);
			string text = string.Empty;
			if (!string.IsNullOrEmpty(base.Request.QueryString["firstPage"]))
			{
				foreach (KeyValuePair<string, NavModule> current in navigation.ModuleList)
				{
					if (string.Equals(current.Value.ID, moduleId, StringComparison.CurrentCultureIgnoreCase))
					{
						foreach (KeyValuePair<string, NavItem> current2 in current.Value.ItemList)
						{
							foreach (KeyValuePair<string, NavPageLink> current3 in current2.Value.PageLinks)
							{
								bool flag = ManagerHelper.IsHavePermission(current.Value.ID, current3.Value.ID, roleId);
								if (flag)
								{
									text = current3.Value.Link;
									break;
								}
							}
							if (!string.IsNullOrEmpty(text))
							{
								break;
							}
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				base.Response.Redirect(string.Format("{0}{1}", Globals.ApplicationPath, text), false);
			}
			else
			{
				string str = "&m=" + this.ModuleId + "&p=" + this.PageId;
				base.Server.Transfer("/Admin/NoPermissionShow.aspx?type=2" + str);
			}
		}

		public string GetFieldValue(DataRow drOne, string FieldName)
		{
			string result;
			if (drOne != null && !drOne.Table.Columns.Contains(FieldName))
			{
				result = "";
			}
			else if (drOne != null && drOne[FieldName] != null)
			{
				result = drOne[FieldName].ToString();
			}
			else
			{
				result = "";
			}
			return result;
		}

		public decimal GetFieldDecimalValue(DataRow drOne, string FieldName)
		{
			decimal result;
			if (drOne != null && !drOne.Table.Columns.Contains(FieldName))
			{
				result = 0m;
			}
			else if (drOne != null && drOne.Table.Columns.Contains(FieldName) && !string.IsNullOrEmpty(drOne[FieldName].ToString()))
			{
				result = Convert.ToDecimal(drOne[FieldName].ToString());
			}
			else
			{
				result = 0m;
			}
			return result;
		}

		public int GetFieldIntValue(DataRow drOne, string FieldName)
		{
			int result;
			if (drOne != null && !drOne.Table.Columns.Contains(FieldName))
			{
				result = 0;
			}
			else if (drOne != null && !string.IsNullOrEmpty(drOne[FieldName].ToString()))
			{
				result = int.Parse(drOne[FieldName].ToString());
			}
			else
			{
				result = 0;
			}
			return result;
		}
	}
}
