using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.Store;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace Hidistro.UI.Web.Admin.Shop.api
{
	public class Hi_Ajax_GetCustomPages : System.Web.IHttpHandler
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
			context.Response.Write(this.GetModelJson(context));
		}

		public string GetModelJson(System.Web.HttpContext context)
		{
			DbQueryResult customPageTable = this.GetCustomPageTable(context);
			int pageCount = TemplatePageControl.GetPageCount(customPageTable.TotalRecords, 10);
			if (customPageTable != null)
			{
				string str = "{\"status\":1,";
				str = str + this.GetGraphicesListJson(customPageTable, context) + ",";
				str = str + "\"page\":\"" + this.GetPageHtml(pageCount, context) + "\"";
				return str + "}";
			}
			return "{\"status\":1,\"list\":[],\"page\":\"\"}";
		}

		public string GetPageHtml(int pageCount, System.Web.HttpContext context)
		{
			int pageIndex = (context.Request.Form["p"] == null) ? 1 : System.Convert.ToInt32(context.Request.Form["p"]);
			return TemplatePageControl.GetPageHtml(pageCount, pageIndex);
		}

		public string GetGraphicesListJson(DbQueryResult GraphicesTable, System.Web.HttpContext context)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("\"list\":[");
			System.Data.DataTable dataTable = (System.Data.DataTable)GraphicesTable.Data;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				stringBuilder.Append("{");
				stringBuilder.Append("\"item_id\":\"" + Globals.String2Json(dataTable.Rows[i]["Id"].ToString()) + "\",");
				stringBuilder.Append("\"title\":\"" + Globals.String2Json(dataTable.Rows[i]["Name"].ToString()) + "\",");
				stringBuilder.Append("\"create_time\":\"" + System.DateTime.Now + "\",");
				stringBuilder.Append("\"link\":\"/custom/" + dataTable.Rows[i]["PageUrl"].ToString() + "\",");
				stringBuilder.Append("\"pic\":\"\"");
				stringBuilder.Append("},");
			}
			string str = stringBuilder.ToString().TrimEnd(new char[]
			{
				','
			});
			return str + "]";
		}

		private string GetChildGraphicesListJson(System.Data.DataTable dt, string categoryId)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string result = "";
			dt.DefaultView.RowFilter = "ParentCategoryId=" + categoryId;
			System.Data.DataTable dataTable = dt.DefaultView.ToTable();
			if (dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append("\"item_id\":\"" + dataTable.Rows[i]["Id"] + "\",");
					stringBuilder.Append("\"title\":\"" + Globals.String2Json(dataTable.Rows[i]["Name"].ToString()) + "\",");
					stringBuilder.Append("\"create_time\":\"" + System.DateTime.Now + "\",");
					stringBuilder.Append("\"link\":\"\",");
					stringBuilder.Append("\"pic\":\"\"");
					stringBuilder.Append("},");
				}
				result = stringBuilder.ToString().TrimEnd(new char[]
				{
					','
				});
			}
			return result;
		}

		public DbQueryResult GetCustomPageTable(System.Web.HttpContext context)
		{
			return CustomPageHelp.GetPages(this.GetCustomPageSearch(context));
		}

		public CustomPageQuery GetCustomPageSearch(System.Web.HttpContext context)
		{
			return new CustomPageQuery
			{
				Name = ((context.Request.Form["title"] == null) ? "" : context.Request.Form["title"]),
				PageIndex = ((context.Request.Form["p"] == null) ? 1 : System.Convert.ToInt32(context.Request.Form["p"])),
				SortOrder = SortAction.Desc,
				SortBy = "Id",
				Status = new int?(0)
			};
		}
	}
}
