using ASPNET.WebControls;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.FenXiao;
using Hidistro.Entities.Store;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
    public class Scope : AdminPage
    {

        protected string localUrl = string.Empty;

        protected RegionSelector dropRegion;
      
        protected Pager pager1;
    
        protected System.Web.UI.WebControls.Repeater grdStreetsInfo;
        protected System.Web.UI.WebControls.Button btnSearchButton;
        protected System.Web.UI.WebControls.TextBox txtStreetName;

       
        protected Scope() : base("m05", "fxp06")
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!this.Page.IsPostBack)
            {
                this.BindData();
            }

            this.btnSearchButton.Click += new System.EventHandler(this.btnSearchButton_Click);
        }

        
        private void BindData()
        {
            StreetInfoQuery query = this.GetStreetQuery();
            DbQueryResult streetInfos = VShopHelper.GetScopeListByUserId(query);
            //根据取出来的regionId获取完整的省市区名
            DataTable dtStreet = (DataTable)streetInfos.Data;
            dtStreet.Columns.Add("regionName");
            foreach (DataRow row in dtStreet.Rows)
            {
                row["regionName"] = Hidistro.Entities.RegionHelper.GetFullRegion(Convert.ToInt32(row["regionCode"]), " ");
            }


            grdStreetsInfo.DataSource = streetInfos.Data;
            grdStreetsInfo.DataBind();
            txtStreetName.Text = query.streetName;
            this.pager1.TotalRecords = streetInfos.TotalRecords;
        }
        private StreetInfoQuery GetStreetQuery()
        {
            StreetInfoQuery query = new StreetInfoQuery();
            string streetNameStr = Globals.RequestQueryStr("StreetName");
            if (!string.IsNullOrEmpty(streetNameStr))
            {
                query.streetName = Globals.RequestQueryStr("StreetName");
            }

            ManagerInfo currentManager = ManagerHelper.GetCurrentManager();
            query.distributorId = currentManager.UserId;

            query.SortOrder = Core.Enums.SortAction.Desc;
            query.SortBy = "CreateTime";

            query.PageSize = this.pager1.PageSize;
            query.PageIndex = this.pager1.PageIndex;
            return query;
        }

        private void btnSearchButton_Click(object sender, System.EventArgs e)
        {
            this.ReloadStreetInfos();
        }

        



        

            protected void grdStreetsInfot_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
           
                if (e.CommandName == "Delete")
                {
              
                      string streetId = e.CommandArgument.ToString();
                if (!VShopHelper.DeleteStreetInfo(streetId.ToString()))
                {
                    this.ShowMsg("未知错误", false);
                }
                else
                {
                    this.BindData();
                    this.ShowMsg("成功删除了一个街道信息", true);
                }

            }
               
        }

   
        private void ReloadStreetInfos()
        {
            System.Collections.Specialized.NameValueCollection queryStrings = new System.Collections.Specialized.NameValueCollection();

            queryStrings.Add("StreetName", this.txtStreetName.Text);
            base.ReloadPage(queryStrings);
        }



        [System.Web.Services.WebMethod]
        public static string AddStreetInfo(string regionCode, string streetName)
        {
            ManagerInfo currentManager = ManagerHelper.GetCurrentManager();
          
             if (VShopHelper.AddStreetInfo(regionCode, streetName, currentManager.UserId))//更新成功
            {
                return string.Format("success");
            }
            else
            {
                return string.Format("fail");
            }
        }

        [System.Web.Services.WebMethod]
        public static string EditStreetName(string id, string streetName)
        {
           

            if (VShopHelper.EditStreetName(id, streetName))//更新成功
            {
                return string.Format("success");
            }
            else
            {
                return string.Format("fail");
            }
        }
       
        

    }
}