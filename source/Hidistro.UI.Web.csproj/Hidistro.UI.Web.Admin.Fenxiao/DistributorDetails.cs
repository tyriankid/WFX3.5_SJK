using ASPNET.WebControls;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
	public class DistributorDetails : AdminPage
	{
		private int userid;

		protected HiImage ListImage1;





        protected RegionSelector dropRegion;
        protected System.Web.UI.WebControls.TextBox txtSiteName;
        protected System.Web.UI.WebControls.TextBox txtSiteTel;
        protected System.Web.UI.WebControls.TextBox txtSiteAddress;
        protected System.Web.UI.WebControls.TextBox txtStoreDescription;


        protected System.Web.UI.HtmlControls.HtmlGenericControl txtRealName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtCellPhone;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtUserBindName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtMicroName;

        protected System.Web.UI.HtmlControls.HtmlGenericControl SiteTel;

        protected System.Web.UI.HtmlControls.HtmlGenericControl SiteAddress;

        protected System.Web.UI.HtmlControls.HtmlGenericControl StoreDescription;

        protected HiImage StoreCode;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtStoreName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtUrl;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtCreateTime;

		protected System.Web.UI.HtmlControls.HtmlGenericControl ReferralOrders;

		protected System.Web.UI.HtmlControls.HtmlGenericControl OrdersTotal;

		protected System.Web.UI.HtmlControls.HtmlGenericControl TotalReferral;

		protected System.Web.UI.HtmlControls.HtmlGenericControl ReferralBlance;

		protected System.Web.UI.HtmlControls.HtmlGenericControl ReferralRequestBalance;

		protected System.Web.UI.WebControls.Repeater reCommissions;

        protected System.Web.UI.WebControls.Button btnUpload;
        protected System.Web.UI.WebControls.Button EditSave;
        protected System.Web.UI.WebControls.Button BtnSavePicture;
        protected ProductFlashUpload ucFlashUpload1;


        protected Pager pager;

		protected DistributorDetails() : base("m05", "fxp03")
		{
		}

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!int.TryParse(this.Page.Request.QueryString["UserId"], out this.userid))
            {
               
                this.Page.Response.Redirect("DistributorList.aspx");
            }


           
            this.ListImage1.ImageUrl = "/Templates/common/images/user.png";
            DistributorsQuery distributorsQuery = new DistributorsQuery();
            distributorsQuery.UserId = this.userid;
            distributorsQuery.ReferralStatus = -1;
            distributorsQuery.PageIndex = 1;
            distributorsQuery.PageSize = 1;
            distributorsQuery.SortOrder = SortAction.Desc;
            distributorsQuery.SortBy = "userid";
            Globals.EntityCoding(distributorsQuery, true);
            DbQueryResult distributors = VShopHelper.GetDistributors(distributorsQuery, null, null);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            if (distributors.Data != null)
            {
                dataTable = (System.Data.DataTable)distributors.Data;
            }
            else
            {
                this.Page.Response.Redirect("DistributorList.aspx");
            }
            if (dataTable.Rows[0]["UserHead"] != System.DBNull.Value && dataTable.Rows[0]["UserHead"].ToString().Trim() != "")
            {
                this.ListImage1.ImageUrl = dataTable.Rows[0]["UserHead"].ToString();
            }




            this.txtCellPhone.InnerText = ((dataTable.Rows[0]["CellPhone"] == System.DBNull.Value) ? "" : ((string)dataTable.Rows[0]["CellPhone"]));

           
            this.txtMicroName.InnerText = (string)dataTable.Rows[0]["UserName"];
            this.txtUserBindName.InnerText = ((dataTable.Rows[0]["UserBindName"] == System.DBNull.Value) ? "" : dataTable.Rows[0]["UserBindName"].ToString());
            this.txtRealName.InnerText = ((dataTable.Rows[0]["RealName"] == System.DBNull.Value) ? "" : ((string)dataTable.Rows[0]["RealName"]));
            this.txtCreateTime.InnerText = ((System.DateTime)dataTable.Rows[0]["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss");
            this.txtName.InnerText = ((dataTable.Rows[0]["Name"] == System.DBNull.Value) ? "" : ((string)dataTable.Rows[0]["Name"]));
            string text = Globals.HostPath(System.Web.HttpContext.Current.Request.Url) + "/Default.aspx?ReferralId=" + distributorsQuery.UserId;
            this.txtUrl.InnerText = text;
            this.StoreCode.ImageUrl = "http://s.jiathis.com/qrcode.php?url=" + text;
            this.OrdersTotal.InnerText = "￥" + System.Convert.ToDouble(dataTable.Rows[0]["OrdersTotal"]).ToString("0.00");
            this.ReferralOrders.InnerText = dataTable.Rows[0]["ReferralOrders"].ToString();
            this.ReferralBlance.InnerText = "￥" + System.Convert.ToDouble(dataTable.Rows[0]["ReferralBlance"]).ToString("0.00");
            this.ReferralRequestBalance.InnerText = "￥" + System.Convert.ToDouble(dataTable.Rows[0]["ReferralRequestBalance"]).ToString("0.00");
            this.ucFlashUpload1.Value = ((dataTable.Rows[0]["BannerURL"] == System.DBNull.Value) ? "" : dataTable.Rows[0]["BannerURL"].ToString());  


            decimal num = decimal.Parse(dataTable.Rows[0]["ReferralBlance"].ToString()) + decimal.Parse(dataTable.Rows[0]["ReferralRequestBalance"].ToString());
            this.TotalReferral.InnerText = "￥" + System.Convert.ToDouble(num.ToString()).ToString("0.00");
            this.BindData(distributorsQuery.UserId);
            if (!IsPostBack)
            {
                txtSiteName.Text = this.txtStoreName.InnerText = (string)dataTable.Rows[0]["StoreName"];
                txtSiteTel.Text = this.SiteTel.InnerText = ((dataTable.Rows[0]["SiteTel"] == System.DBNull.Value) ? "" : ((string)dataTable.Rows[0]["SiteTel"]));
                this.SiteAddress.InnerText = ((dataTable.Rows[0]["SiteAddress"] == System.DBNull.Value) ? "" : Hidistro.Entities.RegionHelper.GetFullRegion(Convert.ToInt32(dataTable.Rows[0]["SiteRegionId"]), " ") + " " + ((string)dataTable.Rows[0]["SiteAddress"]));
                txtSiteAddress.Text = ((dataTable.Rows[0]["SiteAddress"] == System.DBNull.Value) ? "" : ((string)dataTable.Rows[0]["SiteAddress"]));
                if (dataTable.Rows[0]["SiteRegionId"] != System.DBNull.Value)
                {
                    this.dropRegion.SetSelectedRegionId(Convert.ToInt32(dataTable.Rows[0]["SiteRegionId"]));
                }

                txtStoreDescription.Text = this.StoreDescription.InnerText = ((dataTable.Rows[0]["StoreDescription"] == System.DBNull.Value) ? "" : ((string)dataTable.Rows[0]["StoreDescription"]));
            }
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            this.EditSave.Click += new System.EventHandler(this.EditSave_Click);
              this.BtnSavePicture.Click += new System.EventHandler(this.BtnSavePicture_Click);
        }


        private void BtnSavePicture_Click(object sender, System.EventArgs e)
        {
            try
            {
              
               string []url= ucFlashUpload1.Value.Split(',');
                //for (int i = 0; i < url.Length; i++)
                //{
                //    url
                //}

                if (this.Page.Request.QueryString["UserId"] != null&& ucFlashUpload1.Value!="")
                {
                    DistributorsInfo entity = new DistributorsInfo();
                    entity.UserId = Convert.ToInt32(this.Page.Request.QueryString["UserId"]);
                   
                    entity.BannerURL = ucFlashUpload1.Value;
                    if (DistributorsBrower.EditDisbutosBannerURL(entity))
                    {
                       // this.Page.ClientScript.RegisterStartupScript(typeof(string), DateTime.Now.ToString(), "alert(\"保存成功!\");", true);
                        Response.Redirect("DistributorDetails.aspx?UserId=" + Convert.ToInt32(this.Page.Request.QueryString["UserId"]));
                    }

                }

            }
            catch (Exception ex)
            {

            }

        }

        
        private void EditSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.Page.Request.QueryString["UserId"] != null)
                {
                    DistributorsInfo entity = new DistributorsInfo();
                    entity.UserId = Convert.ToInt32(this.Page.Request.QueryString["UserId"]);
                    entity.StoreName = txtSiteName.Text;
                    entity.SiteTel = txtSiteTel.Text;
                    entity.SiteRegionId = (int)dropRegion.GetSelectedRegionId();
                    entity.SiteAddress = txtSiteAddress.Text;
                    entity.StoreDescription=txtStoreDescription.Text;
                    if (DistributorsBrower.EditDisbutosSiteInfos(entity))
                    {
                        Response.Redirect("DistributorDetails.aspx?UserId=" + Convert.ToInt32(this.Page.Request.QueryString["UserId"]));
                    }
                    
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void btnUpload_Click(object sender, System.EventArgs e)
        {
           

            string path = @"D:\";// AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"/File/";  //目录  
            try
            {
                string picUrl = this.StoreCode.ImageUrl;
                if (!String.IsNullOrEmpty(picUrl))
                {
                    Random rd = new Random();
                    DateTime nowTime = DateTime.Now;
                  //  string fileName = nowTime.Month.ToString() + nowTime.Day.ToString() + nowTime.Hour.ToString() + nowTime.Minute.ToString() + nowTime.Second.ToString() + rd.Next(1000, 1000000) + ".jpeg";
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(picUrl, path + this.txtStoreName.InnerText+"二维码.jpeg");
                  
                    this.Page.ClientScript.RegisterStartupScript(typeof(string), DateTime.Now.ToString(), "alert(\"已成功下载至D盘!\");", true);

                }
            }
            catch(Exception ex)
            {

            }
           
        }


        private void BindData(int UserId)
		{
			BalanceDrawRequestQuery balanceDrawRequestQuery = new BalanceDrawRequestQuery();
			balanceDrawRequestQuery.CheckTime = "";
			balanceDrawRequestQuery.UserId = UserId.ToString();
			balanceDrawRequestQuery.RequestTime = "";
			balanceDrawRequestQuery.StoreName = "";
			balanceDrawRequestQuery.PageIndex = this.pager.PageIndex;
			balanceDrawRequestQuery.PageSize = this.pager.PageSize;
			balanceDrawRequestQuery.SortOrder = SortAction.Desc;
			balanceDrawRequestQuery.SortBy = "RequestTime";
			balanceDrawRequestQuery.RequestEndTime = "";
			balanceDrawRequestQuery.RequestStartTime = "";
			balanceDrawRequestQuery.IsCheck = "";
			Globals.EntityCoding(balanceDrawRequestQuery, true);
			DbQueryResult balanceDrawRequest = VShopHelper.GetBalanceDrawRequest(balanceDrawRequestQuery);
			this.reCommissions.DataSource = balanceDrawRequest.Data;
			this.reCommissions.DataBind();
			this.pager.TotalRecords = balanceDrawRequest.TotalRecords;
		}
	}
}
