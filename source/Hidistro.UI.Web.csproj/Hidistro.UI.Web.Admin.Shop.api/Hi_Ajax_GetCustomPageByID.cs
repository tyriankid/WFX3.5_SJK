using Hidistro.ControlPanel.Store;
using Hidistro.Entities.Store;
using System;
using System.Web;

namespace Hidistro.UI.Web.Admin.Shop.api
{
	public class Hi_Ajax_GetCustomPageByID : System.Web.IHttpHandler
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
			string value = context.Request.QueryString["id"];
			context.Response.Write(this.GetTemplateJson(context, System.Convert.ToInt32(value)));
		}

		public string GetTemplateJson(System.Web.HttpContext context, int id)
		{
			string result;
			try
			{
				CustomPage customPageByID = CustomPageHelp.GetCustomPageByID(id);
				if (customPageByID == null)
				{
					result = "";
				}
				else if (customPageByID.Status == 0)
				{
					result = customPageByID.FormalJson.Replace("\r\n", "").Replace("\n", "");
				}
				else
				{
					result = customPageByID.DraftJson.Replace("\r\n", "").Replace("\n", "");
				}
			}
			catch
			{
				result = "";
			}
			return result;
		}
	}
}
