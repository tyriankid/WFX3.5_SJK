using ControlPanel.Promotions;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Data;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.Admin.promotion
{
	public class BatchPrintSendOrderGoods : AdminPage
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl divContent;

		protected BatchPrintSendOrderGoods() : base("m03", "00000")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = base.Request["OrderIds"].Trim(new char[]
			{
				','
			});
			string text2 = base.Request["PIds"].Trim(new char[]
			{
				','
			});
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				return;
			}
			System.Data.DataSet printData = this.GetPrintData(text, text2);
			System.Data.DataTable dataTable = printData.Tables[0];
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				System.Data.DataRow dataRow = dataTable.Rows[i];
				System.Web.UI.HtmlControls.HtmlGenericControl htmlGenericControl = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
				htmlGenericControl.Attributes["class"] = "order print";
				htmlGenericControl.Attributes["style"] = "padding-bottom:60px;padding-top:40px;";
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");
				stringBuilder.AppendFormat("<div class=\"info clear\"><ul class=\"sub-info\"><li><span>中奖编号： </span>{0}</li><li><span>开奖时间： </span>{1}</li></ul></div>", (int.Parse(dataRow["Ptype"].ToString()) == 1) ? dataRow["PrizeNums"].ToString().Remove(dataRow["PrizeNums"].ToString().Length - 1) : (System.Convert.ToDateTime(dataRow["WinTime"]).ToString("yyyy-MM-dd-") + dataRow["LogId"].ToString()), System.Convert.ToDateTime(dataRow["WinTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
				stringBuilder.Append("<table><col class=\"col-1\" /><col class=\"col-3\" /><col class=\"col-3\" /><col class=\"col-3\" /><thead><tr><th>奖品信息</th><th>活动名称</th><th>收货人</th><th>奖品等级</th></tr></thead><tbody>");
				stringBuilder.AppendFormat("<tr><td>{0}<br><span style=\"color: #888\">{1}</span></td>", dataRow["ProductName"].ToString(), dataRow["SkuIdStr"].ToString());
				stringBuilder.AppendFormat("<td>{0}<br /><span class=\"jpname\">[{1}]</span></td>", dataRow["Title"].ToString(), GameHelper.GetGameTypeName(dataRow["GameType"].ToString()));
				stringBuilder.AppendFormat("<td>{0}<br />{1}</td>", dataRow["Receiver"].ToString(), dataRow["Tel"].ToString());
				stringBuilder.AppendFormat("<td style='padding-left:15px;'>{0}</td>", GameHelper.GetPrizeGradeName(dataRow["PrizeGrade"].ToString()));
				stringBuilder.Append("</tr>");
				stringBuilder.Append("</tbody></table>");
				htmlGenericControl.InnerHtml = stringBuilder.ToString();
				this.divContent.Controls.AddAt(0, htmlGenericControl);
			}
		}

		private System.Data.DataSet GetPrintData(string orderIds, string pIds)
		{
			orderIds = "'" + orderIds.Replace(",", "','") + "'";
			pIds = "'" + pIds.Replace(",", "','") + "'";
			return GameHelper.GetOrdersAndLines(orderIds, pIds);
		}
	}
}
