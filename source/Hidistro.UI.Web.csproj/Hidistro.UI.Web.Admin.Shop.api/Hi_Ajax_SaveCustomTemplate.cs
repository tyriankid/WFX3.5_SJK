using Hidistro.ControlPanel.Store;
using Hidistro.Entities.Store;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Hidistro.UI.Web.Admin.Shop.api
{
	public class Hi_Ajax_SaveCustomTemplate : System.Web.IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			string text = context.Request.Form["content"];
			string value = context.Request.Form["id"];
			CustomPageStatus customPageStatus = (CustomPageStatus)((context.Request.Form["type"] != null) ? System.Convert.ToInt32(context.Request.Form["type"]) : 1);
			JObject jObject = (JObject)JsonConvert.DeserializeObject(text);
			string text2 = "保存成功";
			string text3 = "1";
			try
			{
				CustomPage customPageByID = CustomPageHelp.GetCustomPageByID(System.Convert.ToInt32(value));
				if (customPageByID == null)
				{
					context.Response.Write("{\"status\":" + 0 + ",\"msg\":\"找不到相关页面\"}");
					return;
				}
				if (customPageStatus == CustomPageStatus.正常)
				{
					customPageByID.PageUrl = jObject["page"]["pageurl"].ToString().Trim();
					customPageByID.Name = jObject["page"]["title"].ToString();
					customPageByID.IsShowMenu = System.Convert.ToBoolean(jObject["page"]["isshowmenu"]);
					customPageByID.Details = jObject["page"]["subtitle"].ToString();
					customPageByID.FormalJson = text;
				}
				else
				{
					customPageByID.DraftPageUrl = jObject["page"]["pageurl"].ToString().Trim();
					customPageByID.DraftName = jObject["page"]["title"].ToString();
					customPageByID.DraftIsShowMenu = System.Convert.ToBoolean(jObject["page"]["isshowmenu"]);
					customPageByID.DraftDetails = jObject["page"]["subtitle"].ToString();
					customPageByID.DraftJson = text;
				}
				customPageByID.Status = (int)customPageStatus;
				if (!CustomPageHelp.Update(customPageByID))
				{
					context.Response.Write("{\"status\":" + 0 + ",\"msg\":\"保存失败\"}");
					return;
				}
				if (customPageStatus == CustomPageStatus.正常)
				{
					string text4 = "<%@ Control Language=\"C#\" %>\n<%@ Register TagPrefix=\"Hi\" Namespace=\"HiTemplate\" Assembly=\"HiTemplate\" %>";
					text4 += this.GetPModulesHtml(context, jObject);
					string lModulesHtml = this.GetLModulesHtml(context, jObject);
					text4 += lModulesHtml;
					string text5 = "/Templates/vshop/custom/" + customPageByID.PageUrl;
					if (!System.IO.Directory.Exists(context.Server.MapPath(text5)))
					{
						System.IO.Directory.CreateDirectory(context.Server.MapPath(text5));
					}
					System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(context.Server.MapPath(text5 + "/Skin-HomePage.html"), false, System.Text.Encoding.UTF8);
					string text6 = text4;
					for (int i = 0; i < text6.Length; i++)
					{
						char value2 = text6[i];
						streamWriter.Write(value2);
					}
					streamWriter.Close();
				}
				else
				{
					string text7 = "<%@ Control Language=\"C#\" %>\n<%@ Register TagPrefix=\"Hi\" Namespace=\"HiTemplate\" Assembly=\"HiTemplate\" %>";
					text7 += this.GetPModulesHtml(context, jObject);
					string lModulesHtml2 = this.GetLModulesHtml(context, jObject);
					text7 += lModulesHtml2;
					string text8 = "/Templates/vshop/custom/draft/" + customPageByID.DraftPageUrl;
					if (!System.IO.Directory.Exists(context.Server.MapPath(text8)))
					{
						System.IO.Directory.CreateDirectory(context.Server.MapPath(text8));
					}
					System.IO.StreamWriter streamWriter2 = new System.IO.StreamWriter(context.Server.MapPath(text8 + "/Skin-HomePage.html"), false, System.Text.Encoding.UTF8);
					string text9 = text7;
					for (int j = 0; j < text9.Length; j++)
					{
						char value3 = text9[j];
						streamWriter2.Write(value3);
					}
					streamWriter2.Close();
				}
			}
			catch (System.Exception ex)
			{
				text2 = ex.Message;
				text3 = "0";
			}
			if (context.Request.Form["is_preview"] == "1")
			{
				context.Response.Write(string.Concat(new string[]
				{
					"{\"status\":",
					text3,
					",\"msg\":\"",
					text2,
					"\",\"link\":\"default.aspx\"}"
				}));
				return;
			}
			context.Response.Write(string.Concat(new string[]
			{
				"{\"status\":",
				text3,
				",\"msg\":\"",
				text2,
				"\"}"
			}));
		}

		public string GetPModulesHtml(System.Web.HttpContext context, JObject jo)
		{
			string text = "";
			foreach (JToken current in ((System.Collections.Generic.IEnumerable<JToken>)jo["PModules"]))
			{
				text += this.Base64Decode(current["dom_conitem"].ToString());
			}
			return text;
		}

		public string GetLModulesHtml(System.Web.HttpContext context, JObject jo)
		{
			string text = "";
			foreach (JToken current in ((System.Collections.Generic.IEnumerable<JToken>)jo["LModules"]))
			{
				if (current["type"].ToString() == "5")
				{
					text += this.GetGoodGroupTag(context, this.Base64Decode(current["dom_conitem"].ToString()), current);
				}
				else if (current["type"].ToString() == "4")
				{
					text += this.GetGoodTag(context, current);
				}
				else
				{
					text += this.Base64Decode(current["dom_conitem"].ToString());
				}
			}
			return text;
		}

		public string GetGoodTag(System.Web.HttpContext context, JToken data)
		{
			string result;
			try
			{
				string text = "";
				foreach (JToken current in ((System.Collections.Generic.IEnumerable<JToken>)data["content"]["goodslist"]))
				{
					text = text + current["item_id"] + ",";
				}
				text = text.TrimEnd(new char[]
				{
					','
				});
				string text2 = "";
				if (!string.IsNullOrEmpty(text))
				{
					string text3 = "/admin/shop/Modules/GoodGroup" + data["content"]["layout"] + ".cshtml";
					text2 = string.Concat(new object[]
					{
						"<Hi:GoodsMobule runat=\"server\" Layout=\"",
						data["content"]["layout"],
						"\" ShowName=\"",
						data["content"]["showName"],
						"\" IDs=\"",
						text,
						"\" ShowIco=\"",
						data["content"]["showIco"],
						"\" ShowPrice=\"",
						data["content"]["showPrice"],
						"\" DataUrl=\"",
						context.Request.Form["getGoodUrl"],
						"\" ID=\"goods_",
						System.Guid.NewGuid().ToString("N"),
						"\" TemplateFile=\"",
						text3,
						"\"    />"
					});
				}
				else
				{
					text2 += this.Base64Decode(data["dom_conitem"].ToString());
				}
				result = text2;
			}
			catch
			{
				result = "";
			}
			return result;
		}

		public string GetGoodGroupTag(System.Web.HttpContext context, string path, JToken data)
		{
			string result;
			try
			{
				string text = string.Concat(new object[]
				{
					"<Hi:GoodsListModule runat=\"server\"  Type=\"goodGroup\" Layout=\"",
					data["content"]["layout"],
					"\" ShowName=\"",
					data["content"]["showName"],
					"\" ShowIco=\"",
					data["content"]["showIco"],
					"\" ShowPrice=\"",
					data["content"]["showPrice"],
					"\" DataUrl=\"",
					context.Request.Form["getGoodGroupUrl"],
					"\" ID=\"group_",
					System.Guid.NewGuid().ToString("N"),
					"\" TemplateFile=\"",
					path,
					"\"  GoodListSize=\"",
					data["content"]["goodsize"],
					"\" FirstPriority=\"",
					data["content"]["firstPriority"],
					"\"  SecondPriority=\"",
					data["content"]["secondPriority"],
					"\"  ShowMaketPrice=\"",
					data["content"]["showMaketPrice"],
					"\"  />"
				});
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}

		public string Base64Code(string message)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
			return System.Convert.ToBase64String(bytes);
		}

		public string Base64Decode(string message)
		{
			byte[] bytes = System.Convert.FromBase64String(message);
			return System.Text.Encoding.UTF8.GetString(bytes);
		}
	}
}
