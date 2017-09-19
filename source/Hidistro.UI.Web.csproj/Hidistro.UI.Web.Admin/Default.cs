using Hidistro.ControlPanel.Store;
using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	public class Default : AdminPage
	{
		public string showUrl = "";
		public string WaitSendOrderQty = "0";
		public string OrderQty_Today = "0";
		public string OrderQty_Yesterday = "0";
		public string OrderAmountFee_Today = "0";
		public string OrderAmountFee_Yesterday = "0";
		public string ServiceOrderQty = "0";
		public string GoodsQty = "0";
		public string MemberQty = "0";
		public string DistributorQty = "0";
		public string QtyList = "";
		public string QtyList1 = "";
		public string QtyList2 = "";
		public string QtyList3 = "";
		public string DateList = "";
		protected HtmlForm aspForm;
        protected HtmlImage imgLogo;
		protected Literal lbShopName;
		protected HyperLink lbServiceOrderQty;
		protected Repeater rptDistributor;
		protected Repeater rptMember;

		protected Default() : base("m01", "dpp01")
		{
            
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			DateTime today = DateTime.Today;
			DateTime beginDate = today.AddDays(-6.0);


          
			if (!base.IsPostBack)
			{

               
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
				this.lbShopName.Text = masterSettings.SiteName;
				this.imgLogo.Src = masterSettings.DistributorLogoPic;
				if (!File.Exists(base.Server.MapPath(masterSettings.DistributorLogoPic)))
				{
					this.imgLogo.Src = "/Admin/Shop/Public/images/80x80.png";
				}
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
				DataRow drOne = ShopStatisticHelper.ShopGlobal_GetMemberCount();
				DataRow drOne2 = ShopStatisticHelper.ShopGlobal_GetOrderCountByDate(today);
				DataRow drOne3 = ShopStatisticHelper.ShopGlobal_GetOrderCountByDate(today.AddDays(-1.0));
				this.WaitSendOrderQty = base.GetFieldValue(drOne, "WaitSendOrderQty");
				this.OrderQty_Today = base.GetFieldValue(drOne2, "OrderQty");
				this.OrderQty_Yesterday = base.GetFieldValue(drOne3, "OrderQty");
				this.OrderAmountFee_Today = base.GetFieldDecimalValue(drOne2, "OrderAmountFee").ToString("N2");
				this.OrderAmountFee_Yesterday = base.GetFieldDecimalValue(drOne3, "OrderAmountFee").ToString("N2");
				this.ServiceOrderQty = base.GetFieldValue(drOne, "ServiceOrderQty");
				this.lbServiceOrderQty.Text = this.ServiceOrderQty;
				if (this.ServiceOrderQty == "0")
				{
					this.lbServiceOrderQty.ForeColor = Color.Green;
				}
				else
				{
					this.lbServiceOrderQty.ForeColor = Color.Red;
				}
				this.GoodsQty = base.GetFieldValue(drOne, "GoodsQty");
				this.MemberQty = base.GetFieldValue(drOne, "MemberQty");
				this.DistributorQty = base.GetFieldValue(drOne, "DistributorQty");
				this.LoadTradeDataList(beginDate, 7);
				this.rptDistributor.DataSource = ShopStatisticHelper.ShopGlobal_GetSortList_Distributor(beginDate, 8);
				this.rptDistributor.DataBind();
				this.rptMember.DataSource = ShopStatisticHelper.ShopGlobal_GetSortList_Member(beginDate, 8);
				this.rptMember.DataBind();
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			StringWriter stringWriter = new StringWriter();
			HtmlTextWriter writer2 = new HtmlTextWriter(stringWriter);
			base.Render(writer2);
			string text = stringWriter.ToString();
			text = text.Trim().Replace("</body>", "<div style=\"display:none\"><script src=\"http://s95.cnzz.com/stat.php?id=1259515160&web_id=1259515160\" language=\"JavaScript\"></script></div></body>");
			writer.Write(text);
		}

		private void LoadTradeDataList(DateTime BeginDate, int Days)
		{
			DataTable dataTable = ShopStatisticHelper.ShopGlobal_GetTrendDataList(BeginDate, Days);
			this.DateList = "";
			int num = 0;
			foreach (DataRow dataRow in dataTable.Rows)
			{
				this.DateList = this.DateList + "'" + Convert.ToDateTime(dataRow["RecDate"].ToString()).ToString("yyyy-MM-dd") + "'";
				this.QtyList1 += base.GetFieldValue(dataRow, "OrderCount");
				this.QtyList2 += base.GetFieldValue(dataRow, "NewDistributorCount");
				this.QtyList3 += base.GetFieldValue(dataRow, "NewMemberCount");
				if (num < Days - 1)
				{
					this.DateList += ",";
					this.QtyList1 += ",";
					this.QtyList2 += ",";
					this.QtyList3 += ",";
				}
				num++;
			}
		}
	}
}
