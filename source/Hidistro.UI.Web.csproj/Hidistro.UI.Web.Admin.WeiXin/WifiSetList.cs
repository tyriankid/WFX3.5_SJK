using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.WeiXin
{
	public class WifiSetList : AdminPage
	{
		protected Script Script5;

		protected Script Script6;

		protected System.Web.UI.WebControls.Repeater rptWifiSetList;

		protected WifiSetList() : base("m06", "wxp12")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				this.BindData();
			}
		}

		public void BindData()
		{
			System.Data.DataSet dataSet = new System.Data.DataSet();
			string text = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			if (System.IO.File.Exists(text))
			{
				dataSet.ReadXml(text);
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					this.rptWifiSetList.DataSource = dataSet;
					this.rptWifiSetList.DataBind();
				}
			}
		}
	}
}
