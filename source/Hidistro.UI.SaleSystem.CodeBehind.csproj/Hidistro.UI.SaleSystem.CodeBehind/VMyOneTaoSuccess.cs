using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Sales;
using Hidistro.Entities.VShop;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
    public class VMyOneTaoSuccess : VMemberTemplatedWebControl
    {
        private VshopTemplatedRepeater rptAddress;

        private Literal litShipTo;

        private Literal litAddAddress;

        private Literal litCellPhone;

        private Literal litAddress;

        private Literal litShowMes;

        private HtmlInputHidden selectShipTo;

        private HtmlInputHidden regionId;

        public VMyOneTaoSuccess()
        {
        }

        protected override void AttachChildControls()
        {
            ShippingAddressInfo shippingAddressInfo;
            this.litShipTo = (Literal)this.FindControl("litShipTo");
            this.litCellPhone = (Literal)this.FindControl("litCellPhone");
            this.litAddress = (Literal)this.FindControl("litAddress");
            this.rptAddress = (VshopTemplatedRepeater)this.FindControl("rptAddress");
            IList<ShippingAddressInfo> shippingAddresses = MemberProcessor.GetShippingAddresses();
            this.rptAddress.DataSource =
                from item in shippingAddresses
                orderby item.IsDefault
                select item;
            this.rptAddress.DataBind();
            this.litAddAddress = (Literal)this.FindControl("litAddAddress");
            this.selectShipTo = (HtmlInputHidden)this.FindControl("selectShipTo");
            this.regionId = (HtmlInputHidden)this.FindControl("regionId");
            ShippingAddressInfo shippingAddressInfo1 = shippingAddresses.FirstOrDefault<ShippingAddressInfo>((ShippingAddressInfo item) => item.IsDefault);
            if (shippingAddressInfo1 == null)
            {
                if (shippingAddresses.Count > 0)
                {
                    shippingAddressInfo = shippingAddresses[0];
                }
                else
                {
                    shippingAddressInfo = null;
                }
                shippingAddressInfo1 = shippingAddressInfo;
            }
            if (shippingAddressInfo1 != null)
            {
                this.litShipTo.Text = shippingAddressInfo1.ShipTo;
                this.litCellPhone.Text = shippingAddressInfo1.CellPhone;
                this.litAddress.Text = shippingAddressInfo1.Address;
                HtmlInputHidden htmlInputHidden = this.selectShipTo;
                int shippingId = shippingAddressInfo1.ShippingId;
                htmlInputHidden.SetWhenIsNotNull(shippingId.ToString());
                HtmlInputHidden htmlInputHidden1 = this.regionId;
                shippingId = shippingAddressInfo1.RegionId;
                htmlInputHidden1.SetWhenIsNotNull(shippingId.ToString());
            }
            this.litAddAddress.Text = string.Concat(" href='/Vshop/AddShippingAddress.aspx?returnUrl=", Globals.UrlEncode(HttpContext.Current.Request.Url.ToString()), "'");
            PageTitle.AddSiteNameTitle("中奖记录");
        }

        private void DoAction(string Action)
        {
            string str = "{\"state\":false,\"msg\":\"未定义操作\"}";
            int num = Globals.RequestFormNum("pageIndex");
            if (num <= 0)
            {
                str = "{\"state\":false,\"msg\":\"参数不正确\"}";
            }
            else
            {
                int currentMemberUserId = Globals.GetCurrentMemberUserId(false);
                OneyuanTaoPartInQuery oneyuanTaoPartInQuery = new OneyuanTaoPartInQuery()
                {
                    PageIndex = num,
                    PageSize = 10,
                    ActivityId = "",
                    UserId = Globals.GetCurrentMemberUserId(false),
                    state = 3,
                    SortBy = "BuyTime",
                    IsPay = -1
                };
                DbQueryResult oneyuanPartInDataTable = OneyuanTaoHelp.GetOneyuanPartInDataTable(oneyuanTaoPartInQuery);
                DataTable dataTable = new DataTable();
                if (oneyuanPartInDataTable.Data != null)
                {
                    dataTable = (DataTable)oneyuanPartInDataTable.Data;
                    dataTable.Columns.Add("LuckNumList");
                    dataTable.Columns.Add("PostSate");
                    dataTable.Columns.Add("PostBtn");
                    dataTable.Columns.Add("tabid");
                    foreach (DataRow row in dataTable.Rows)
                    {
                        IList<LuckInfo> luckInfoList = OneyuanTaoHelp.getLuckInfoList(true, row["ActivityId"].ToString());
                        luckInfoList = (
                            from t in luckInfoList
                            where (t.UserId != currentMemberUserId ? false : t.Pid == row["Pid"].ToString())
                            select t).ToList<LuckInfo>();
                        row["PostBtn"] = "0";
                        row["tabid"] = "0";
                        if (luckInfoList != null)
                        {
                            List<string> strs = new List<string>();
                            foreach (LuckInfo luckInfo in luckInfoList)
                            {
                                strs.Add(luckInfo.PrizeNum);
                            }
                            row["LuckNumList"] = string.Join(",", strs);
                            DataTable dataTable1 = OneyuanTaoHelp.PrizesDeliveryRecord(row["Pid"].ToString());
                            if ((dataTable1 == null ? false : dataTable1.Rows.Count != 0))
                            {
                                row["PostSate"] = OneyuanTaoHelp.GetPrizesDeliveStatus(dataTable1.Rows[0]["status"].ToString());
                                row["PostBtn"] = dataTable1.Rows[0]["status"].ToString();
                                row["tabid"] = dataTable1.Rows[0]["Id"].ToString();
                            }
                            else
                            {
                                row["PostSate"] = "收货地址未确认";
                            }
                        }
                    }
                }
                IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter()
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                };
                JsonConverter[] jsonConverterArray = new JsonConverter[] { isoDateTimeConverter };
                string str1 = JsonConvert.SerializeObject(dataTable, jsonConverterArray);
                str = string.Concat("{\"state\":true,\"msg\":\"读取成功\",\"Data\":", str1, "}");
            }
            this.Page.Response.ClearContent();
            this.Page.Response.ContentType = "application/json";
            this.Page.Response.Write(str);
            this.Page.Response.End();
        }

        protected override void OnInit(EventArgs e)
        {
            string str = Globals.RequestFormStr("action");
            if (!string.IsNullOrEmpty(str))
            {
                this.DoAction(str);
                this.Page.Response.End();
            }
            if (this.SkinName == null)
            {
                this.SkinName = "Skin-VMyOneTaoSuccess.html";
            }
            base.OnInit(e);
        }
    }
}