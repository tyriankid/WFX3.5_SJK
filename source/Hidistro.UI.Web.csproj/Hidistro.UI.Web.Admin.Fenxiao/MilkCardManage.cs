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
    public class MilkCardManage : AdminPage
    {
        protected string localUrl = string.Empty;

        protected RegionSelector dropRegion;
      
        protected Pager pager1;
    
        protected System.Web.UI.WebControls.Repeater grdMilkCards;
        protected System.Web.UI.WebControls.Button btnSearchButton;
        protected DropDownList ddlSite;

       
        protected MilkCardManage() : base("m05", "fxp08")
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!this.Page.IsPostBack)
            {
                DataTable dtSite = ManagerHelper.GetSiteManagers();
                ddlSite.DataSource = dtSite;
                ddlSite.DataTextField = "StoreName";
                ddlSite.DataValueField = "SiteId";
                ddlSite.DataBind();
                this.BindData();
            }
            this.btnSearchButton.Click += new System.EventHandler(this.btnSearchButton_Click);
        }

        
        private void BindData()
        {


            MilkCardQuery query = this.GetMilkCardQuery();
            DbQueryResult cardInfos = VShopHelper.GetMilkCards(query);
            grdMilkCards.DataSource = cardInfos.Data;
            grdMilkCards.DataBind();
            ddlSite.SelectedValue  = query.siteId.ToString();
            this.pager1.TotalRecords = cardInfos.TotalRecords;
        }
        private MilkCardQuery GetMilkCardQuery()
        {
            MilkCardQuery query = new MilkCardQuery();
            if (!string.IsNullOrEmpty(Globals.RequestQueryStr("status")))
            {
                query.status = Globals.RequestQueryNum("status");
            }
            if (!string.IsNullOrEmpty(Globals.RequestQueryStr("siteid")))
            {
                query.siteId = Globals.RequestQueryNum("siteId");
            }

            query.SortOrder = Core.Enums.SortAction.Desc;
            query.SortBy = "sm.CreateDate";

            query.PageSize = this.pager1.PageSize;
            query.PageIndex = this.pager1.PageIndex;
            return query;
        }

        private void btnSearchButton_Click(object sender, System.EventArgs e)
        {
            this.ReloadStreetInfos();
        }

        



        

            protected void grdMilkCards_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
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

            queryStrings.Add("SiteId", this.ddlSite.SelectedValue);
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