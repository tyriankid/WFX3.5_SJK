using Hidistro.ControlPanel.Sales;
using Hidistro.Entities.Orders;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.SaleSystem.Tags;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VQuestList : VshopTemplatedWebControl
	{
        string orderId;
        protected Common_Daily daily;
        protected Literal litPrtName;
        protected Literal litPrtPrice;
        protected Literal litUserName;
        protected Literal litPrtQuantity;
        protected Literal litAddress;
        protected Image imgPrt;


        protected override void OnInit(System.EventArgs e)
		{
            if (this.SkinName == null)
			{
				this.SkinName = "skin-vQuestList.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
            this.litPrtName=(Literal)this.FindControl("litPrtName");
            this.litPrtPrice = (Literal)this.FindControl("litPrtPrice");
            this.litUserName = (Literal)this.FindControl("litUserName");
            this.litPrtQuantity = (Literal)this.FindControl("litPrtQuantity");
            this.litAddress = (Literal)this.FindControl("litAddress");
            this.imgPrt = (Image)this.FindControl("imgPrt");


            this.orderId = this.Page.Request.QueryString["OrderId"];
            OrderInfo orderInfo = OrderHelper.GetOrderInfo(orderId);
            if(orderInfo == null)
            {
                this.GotoResourceNotFound(Entities.VShop.ErrorType.前台其它错误, "订单不存在");
                return;
            }
            if(orderInfo.LineItems.Count != 1)
            {
                this.GotoResourceNotFound(Entities.VShop.ErrorType.前台其它错误, "该订单包含了多个配送信息");
                return;
            }
   

            this.daily = (Common_Daily)this.FindControl("daily");
            foreach (LineItemInfo info in orderInfo.LineItems.Values)
            {
                this.daily.startDate = info.SendStartDate; //开始配送日期
                this.daily.endDate = info.SendEndDate; //结束配送日期
                this.litPrtName.Text = info.ItemDescription;
                this.litPrtPrice.Text = "￥"+info.ItemAdjustedPrice.ToString("F2");
                this.litUserName.Text = orderInfo.Username;
                this.litPrtQuantity.Text = "×" + info.QuantityPerDay.ToString();
                this.litAddress.Text = orderInfo.Address;
                this.imgPrt.ImageUrl = info.ThumbnailsUrl;
            }

            IList<DateTime> dateCheck = new List<DateTime>();
            IList<DateTime> dateCheckType2 = new List<DateTime>();

            DataTable dtUnFinishQuest = OrderHelper.GetQuestList(" where status = 0 and orderid = '"+this.orderId+"'");
            DataTable dtFinishQuest = OrderHelper.GetQuestList(" where status = 1  and orderid = '" + this.orderId + "'");
            foreach (DataRow row1 in dtUnFinishQuest.Rows)
            {
                dateCheck.Add((DateTime)row1["QuestDate"]);
            }
            foreach (DataRow row2 in dtFinishQuest.Rows)
            {
                dateCheckType2.Add((DateTime)row2["QuestDate"]);
            }

            this.daily.dateCheck = dateCheck;
            this.daily.dateCheckType2 = dateCheckType2;

            PageTitle.AddSiteNameTitle("牛奶配送列表");
		}
	}
}
