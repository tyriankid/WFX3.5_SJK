using Ajax;
using ASPNET.WebControls;
using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Store;
using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.FenXiao;
using Hidistro.Entities.Members;
using Hidistro.Entities.StatisticsReport;
using Hidistro.Entities.Store;
using Hidistro.Messages;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using Hidistro.UI.Web.Admin.member;
using Hishop.Plugins;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
   public class QuestList : AdminPage
    {
       
        protected System.Web.UI.WebControls.TextBox txtOrderId;

        protected System.Web.UI.WebControls.DropDownList ddlStatus;

        protected ucDateTimePicker calendarStartDate;

        protected ucDateTimePicker calendarEndDate;

        protected System.Web.UI.WebControls.TextBox txtUserName;

        protected System.Web.UI.WebControls.DropDownList ddlSort;

        protected PageSize hrefPageSize1;

        protected Pager pager1;

        protected Grid grdMemberList;
        
        protected System.Web.UI.WebControls.Button btnSearchButton;

        protected System.Web.UI.WebControls.Button btnManyChangeStatus;
        protected System.Web.UI.WebControls.LinkButton LinkBtn;
        protected System.Web.UI.WebControls.LinkButton LinkButtonClear;

        protected System.Web.UI.HtmlControls.HtmlGenericControl days;
        protected System.Web.UI.HtmlControls.HtmlGenericControl alreadySend;
        protected System.Web.UI.HtmlControls.HtmlGenericControl NoSend;
        protected System.Web.UI.HtmlControls.HtmlGenericControl spantotaldays;
        protected System.Web.UI.HtmlControls.HtmlGenericControl spantotalcount;
        

        public QuestList() : base("m05", "fxp15")
		{
        }

       


        protected void Page_Load(object sender, System.EventArgs e)
        {
           
            if (!this.Page.IsPostBack)
            {
                this.ViewState["ClientType"] = ((base.Request.QueryString["clientType"] != null) ? base.Request.QueryString["clientType"] : null);

              
                this.BindData();
               
            }
            // CheckBoxColumn.RegisterClientCheckEvents(this.Page, this.Page.Form.ClientID);

            
            this.btnManyChangeStatus.Click += new System.EventHandler(this.btnManyChangeStatus_Click);
            this.btnSearchButton.Click += new System.EventHandler(this.btnSearchButton_Click);
            this.LinkBtn.Click += new System.EventHandler(this.LinkBtn_Click);
             this.LinkButtonClear.Click += new System.EventHandler(this.LinkButtonClear_Click);


        }

      
      
        private void ReBind(bool isSearch)
        {
            System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();
           
            //nameValueCollection.Add("Username", this.txtSearchText.Text);
          
          
            //nameValueCollection.Add("MemberStatus", this.MemberStatus.SelectedItem.Value);
            //nameValueCollection.Add("clientType", (this.ViewState["ClientType"] != null) ? this.ViewState["ClientType"].ToString() : "");
            //nameValueCollection.Add("pageSize", this.pager.PageSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
            //nameValueCollection.Add("phone", this.txtPhone.Text);
            //if (!isSearch)
            //{
            //    nameValueCollection.Add("pageIndex", this.pager.PageIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
            //}
            base.ReloadPage(nameValueCollection);
        }

        protected void BindData()
        {
            QuestInfoQuery questinfoQuery = GetQuery();
            

            DbQueryResult members = VShopHelper.GetQuestList(questinfoQuery);
            this.grdMemberList.DataSource = members.Data;
            this.grdMemberList.DataBind();
            this.pager1.TotalRecords = (this.pager1.TotalRecords = members.TotalRecords);


            this.txtOrderId.Text = questinfoQuery.OrderId;
            this.ddlStatus.SelectedValue = questinfoQuery.Status;
            this.calendarStartDate.Text = questinfoQuery.SendStartDate;
            this.calendarEndDate.Text = questinfoQuery.SendEndDate;
            this.txtUserName.Text = questinfoQuery.UserName;
            this.ddlSort.SelectedValue = questinfoQuery.Sort;


            string countinfo= VShopHelper.GetQuestCount(questinfoQuery);

          
            NoSend.InnerText = countinfo.Split(',')[0];
            alreadySend.InnerText = countinfo.Split(',')[1];
            spantotalcount.InnerText =(int.Parse( countinfo.Split(',')[0])+ int.Parse(countinfo.Split(',')[1])).ToString();
            if (!string.IsNullOrEmpty(questinfoQuery.SendStartDate) && !string.IsNullOrEmpty(questinfoQuery.SendEndDate))
            {
                DateTime t1 = DateTime.Parse(questinfoQuery.SendStartDate);
                DateTime t2 = DateTime.Parse(questinfoQuery.SendEndDate);

                System.TimeSpan t3 = t2 - t1;
                days.InnerText = (t3.TotalDays+1).ToString();
            }
            else
            {

                spantotaldays.Style.Add("display", "none");
            }
           



        }

        


             private void LinkButtonClear_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("QuestList.aspx");
             
        }
        private void LinkBtn_Click(object sender, System.EventArgs e)
        {
            System.Collections.Specialized.NameValueCollection queryStrings = new System.Collections.Specialized.NameValueCollection();

           
            queryStrings.Add("Status", "0");
            queryStrings.Add("SendStartDate",Convert.ToDateTime( DateTime.Now.ToString()).ToString("yyyy-MM-dd"));
            queryStrings.Add("SendEndDate", Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-dd"));


            base.ReloadPage(queryStrings);
        }


        private void btnSearchButton_Click(object sender, System.EventArgs e)
        {
            System.Collections.Specialized.NameValueCollection queryStrings = new System.Collections.Specialized.NameValueCollection();

            queryStrings.Add("OrderId", this.txtOrderId.Text);
            queryStrings.Add("Status", this.ddlStatus.SelectedValue);
            queryStrings.Add("SendStartDate", this.calendarStartDate.Text);
            queryStrings.Add("SendEndDate", this.calendarEndDate.Text);
            queryStrings.Add("UserName", this.txtUserName.Text);
            queryStrings.Add("Sort", this.ddlSort.SelectedValue);

            base.ReloadPage(queryStrings);
        }



        public QuestInfoQuery GetQuery()
        {
            QuestInfoQuery entity = new QuestInfoQuery();

            if (!IsPostBack)
            {
                entity.SendStartDate = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-01");
                string yyyymm = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM");
                int days = DateTime.DaysInMonth(int.Parse(yyyymm.Split('-')[0]), int.Parse(yyyymm.Split('-')[1]));
                entity.SendEndDate = yyyymm + "-" + days.ToString();

            }


            if (Request.QueryString["OrderId"]!=null) 
            entity.OrderId = Request.QueryString["OrderId"].ToString();

            if (Request.QueryString["Status"] != null)
                entity.Status = Request.QueryString["Status"].ToString();

            if (Request.QueryString["SendStartDate"] != null)
                entity.SendStartDate = Request.QueryString["SendStartDate"].ToString();

            if (Request.QueryString["SendEndDate"] != null)
                entity.SendEndDate = Request.QueryString["SendEndDate"].ToString();
            
            if (Request.QueryString["UserName"] != null)
               entity.UserName = Request.QueryString["UserName"].ToString();

            if (Request.QueryString["Sort"] != null)
                entity.Sort = Request.QueryString["Sort"].ToString();

          
          


            entity.UserId = ManagerHelper.GetCurrentManager().SiteId.ToString();
            entity.PageIndex = this.pager1.PageIndex;
            entity.PageSize = this.pager1.PageSize;
            entity.SortBy = "QuestDate";
            entity.SortOrder = SortAction.Asc;

            if (!string.IsNullOrEmpty(entity.Sort)&& entity.Sort=="1")
            {
                entity.SortOrder = SortAction.Desc;
            }

            return entity;

        }
        


        private void btnManyChangeStatus_Click(object sender, System.EventArgs e)
        {
            string text = "";
            ManagerHelper.CheckPrivilege(Privilege.DeleteMember);
            foreach (System.Web.UI.WebControls.GridViewRow gridViewRow in this.grdMemberList.Rows)
            {
                System.Web.UI.WebControls.CheckBox checkBox = (System.Web.UI.WebControls.CheckBox)gridViewRow.FindControl("checkboxCol");
                if (checkBox.Checked)
                {
                    text = text + "'"+ this.grdMemberList.DataKeys[gridViewRow.RowIndex].Value.ToString() + "'"+",";
                }
            }
            text = text.TrimEnd(new char[]
            {
                ','
            });
            if (string.IsNullOrEmpty(text))
            {
                this.ShowMsg("请先选择要已配送的任务！", false);
                return;
            }
          
            if (VShopHelper.ChangStatus(text, ManagerHelper.GetCurrentManager().UserId))
            {
                this.ShowMsg("成功已配送了任务！", true);
                this.BindData();
            }
        }




        private void ddlApproved_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.ReBind(false);
        }
        
       
    }
}