using ControlPanel.Promotions;
using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Promotions;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.FenXiao;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VSubmmitOrder : VMemberTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litShipTo;

		private System.Web.UI.WebControls.Literal litCellPhone;

		private System.Web.UI.WebControls.Literal litAddress;

		private System.Web.UI.WebControls.Literal litShowMes;

		private System.Web.UI.WebControls.Literal litUseMembersPointShow;

		private System.Web.UI.WebControls.Literal litIsUseBalance;

		private System.Web.UI.HtmlControls.HtmlInputControl groupbuyHiddenBox;

		private VshopTemplatedRepeater rptCartProducts;

		private VshopTemplatedRepeater rptAddress;

		private System.Web.UI.WebControls.Literal litOrderTotal;

		private System.Web.UI.WebControls.Literal litPointNumber;

		private System.Web.UI.WebControls.Literal litDisplayPointNumber;

		private System.Web.UI.WebControls.Literal litDisplayPoint;

		private System.Web.UI.HtmlControls.HtmlInputHidden selectShipTo;

		private System.Web.UI.HtmlControls.HtmlInputHidden regionId;

		private System.Web.UI.HtmlControls.HtmlInputHidden MembersPointMoney;

		private System.Web.UI.HtmlControls.HtmlInputHidden BalanceCanPayMoney;

		private System.Web.UI.WebControls.Literal litAddAddress;

		private int buyAmount;

		private string productSku;

		public DataTable GetUserCoupons = null;

		private DataTable dtActivities = ActivityHelper.GetActivities();

		private bool isbargain = Globals.RequestQueryNum("bargainDetialId") > 0;

        private DateTime SendStartDate;
        private int QuantityPerDay;
        private int SendDays;

        
        protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VSubmmitOrder.html";
			}
			base.OnInit(e);
		}

		private void rptCartProducts_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
			{
				System.Collections.Generic.List<ShoppingCartItemInfo> list = (System.Collections.Generic.List<ShoppingCartItemInfo>)System.Web.UI.DataBinder.Eval(e.Item.DataItem, "LineItems");
				System.Web.UI.WebControls.Literal literal = (System.Web.UI.WebControls.Literal)e.Item.Controls[0].FindControl("LitCoupon");
				System.Web.UI.WebControls.Literal literal2 = (System.Web.UI.WebControls.Literal)e.Item.Controls[0].FindControl("litExemption");
				System.Web.UI.WebControls.Literal literal3 = (System.Web.UI.WebControls.Literal)e.Item.Controls[0].FindControl("litoldExemption");
				System.Web.UI.WebControls.Literal literal4 = (System.Web.UI.WebControls.Literal)e.Item.Controls[0].FindControl("litoldTotal");
				System.Web.UI.WebControls.Literal literal5 = (System.Web.UI.WebControls.Literal)e.Item.Controls[0].FindControl("litTotal");
				System.Web.UI.WebControls.Literal literal6 = (System.Web.UI.WebControls.Literal)e.Item.Controls[0].FindControl("litbFreeShipping");
				string text = "";
				string text2 = " <div class=\"btn-group coupon\">";
				object obj = text2;
				text2 = string.Concat(new object[]
				{
					obj,
					"<button type=\"button\" class=\"btn btn-default dropdown-toggle coupondropdown\" data-toggle=\"dropdown\"   id='coupondropdown",
					System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
					"'>选择优惠券<span class=\"caret\"></span></button>"
				});
				obj = text2;
				text2 = string.Concat(new object[]
				{
					obj,
					"<ul id=\"coupon",
					System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
					"\" class=\"dropdown-menu\" role=\"menu\">"
				});
				if (this.GetUserCoupons.Rows.Count > 0 && !this.isbargain)
				{
					obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"<li><a onclick=\"Couponasetselect('",
						System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
						"','不使用','0',0,'0')\"   value=\"0\">不使用</a></li>"
					});
				}
				if (!this.isbargain)
				{
					for (int i = 0; i < this.GetUserCoupons.Rows.Count; i++)
					{
						if (MemberProcessor.CheckCurrentMemberIsInRange(this.GetUserCoupons.Rows[i]["MemberGrades"].ToString(), this.GetUserCoupons.Rows[i]["DefualtGroup"].ToString(), this.GetUserCoupons.Rows[i]["CustomGroup"].ToString()) || this.GetUserCoupons.Rows[i]["MemberGrades"].ToString() == "0" || this.GetUserCoupons.Rows[i]["MemberGrades"].ToString() == this.CurrentMemberInfo.GradeId.ToString())
						{
							if (bool.Parse(this.GetUserCoupons.Rows[i]["IsAllProduct"].ToString()))
							{
								decimal num = 0m;
								foreach (ShoppingCartItemInfo current in list)
								{
									if (current.Type == 0)
									{
										num += current.SubTotal;
									}
								}
								if (decimal.Parse(this.GetUserCoupons.Rows[i]["ConditionValue"].ToString()) <= num)
								{
									obj = text;
									text = string.Concat(new object[]
									{
										obj,
										"<li><a onclick=\"Couponasetselect('",
										System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
										"','",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"元现金券','",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"',",
										this.GetUserCoupons.Rows[i]["Id"],
										",'",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"元现金券|",
										this.GetUserCoupons.Rows[i]["Id"],
										"|",
										this.GetUserCoupons.Rows[i]["ConditionValue"],
										"|",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"')\" id=\"acoupon",
										System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
										this.GetUserCoupons.Rows[i]["Id"],
										"\" value=\"",
										this.GetUserCoupons.Rows[i]["Id"],
										"\">",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"元现金券</a></li>"
									});
								}
							}
							else
							{
								decimal num = 0m;
								bool flag = false;
								foreach (ShoppingCartItemInfo current in list)
								{
									if (current.Type == 0)
									{
										DataTable dataTable = MemberProcessor.GetCouponByProducts(int.Parse(this.GetUserCoupons.Rows[i]["CouponId"].ToString()), current.ProductId);
										if (dataTable.Rows.Count > 0)
										{
											num += current.SubTotal;
											flag = true;
										}
									}
								}
								if (flag && decimal.Parse(this.GetUserCoupons.Rows[i]["ConditionValue"].ToString()) <= num)
								{
									obj = text;
									text = string.Concat(new object[]
									{
										obj,
										"<li><a onclick=\"Couponasetselect('",
										System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
										"','",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"元现金券','",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"',",
										this.GetUserCoupons.Rows[i]["Id"],
										",'",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"元现金券|",
										this.GetUserCoupons.Rows[i]["Id"],
										"|",
										this.GetUserCoupons.Rows[i]["ConditionValue"],
										"|",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"')\" id=\"acoupon",
										System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
										this.GetUserCoupons.Rows[i]["Id"],
										"\" value=\"",
										this.GetUserCoupons.Rows[i]["Id"],
										"\">",
										this.GetUserCoupons.Rows[i]["CouponValue"],
										"元现金券</a></li>"
									});
								}
							}
						}
					}
				}
				text2 += text;
				obj = text2;
				text2 = string.Concat(new object[]
				{
					obj,
					"</ul></div><input type=\"hidden\"  class=\"ClassCoupon\"   id=\"selectCoupon",
					System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
					"\"/>  "
				});
				if (!string.IsNullOrEmpty(text))
				{
					literal.Text = string.Concat(new object[]
					{
						text2,
						"<input type=\"hidden\"   id='selectCouponValue",
						System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId"),
						"' class=\"selectCouponValue\" />"
					});
				}
				else
				{
					literal.Text = "<input type=\"hidden\"   id='selectCouponValue" + System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TemplateId") + "' class=\"selectCouponValue\" />";
				}
				decimal d = 0m;
				decimal num2 = 0m;
				decimal num3 = 0m;
				decimal d2 = 0m;
				decimal num4 = 0m;
				int num5 = 0;
				foreach (ShoppingCartItemInfo current2 in list)
				{
					if (current2.Type == 0)
					{
						num4 += current2.MilkSubTotal; //牛奶配送总价
						num5 += current2.Quantity;
					}
				}
				d2 = num4;
                //如果选择了奶卡,并且奶卡属于当前用户,则订单总价为0
                string cardidstr = this.Page.Request.QueryString["cardid"];
                Guid cardid = new Guid();
                if(Guid.TryParse(cardidstr, out cardid))
                {
                    MilkCardInfo milkCard = VShopHelper.GetMilkCard(cardid);
                    if (milkCard != null)
                    {
                        d2 = 0m;
                    }
                }

				if (!this.isbargain)
				{
					for (int j = 0; j < this.dtActivities.Rows.Count; j++)
					{
						if (int.Parse(this.dtActivities.Rows[j]["attendTime"].ToString()) == 0 || int.Parse(this.dtActivities.Rows[j]["attendTime"].ToString()) > ActivityHelper.GetActivitiesMember(this.CurrentMemberInfo.UserId, int.Parse(this.dtActivities.Rows[j]["ActivitiesId"].ToString())))
						{
							decimal num = 0m;
							int num6 = 0;
							DataTable activities_Detail = ActivityHelper.GetActivities_Detail(int.Parse(this.dtActivities.Rows[j]["ActivitiesId"].ToString()));
							foreach (ShoppingCartItemInfo current2 in list)
							{
								if (current2.Type == 0)
								{
									DataTable dataTable = ActivityHelper.GetActivitiesProducts(int.Parse(this.dtActivities.Rows[j]["ActivitiesId"].ToString()), current2.ProductId);
									if (dataTable.Rows.Count > 0)
									{
										num += current2.SubTotal;
										num6 += current2.Quantity;
									}
								}
							}
							bool flag2 = false;
							if (activities_Detail.Rows.Count > 0)
							{
								for (int i = 0; i < activities_Detail.Rows.Count; i++)
								{
									if (MemberHelper.CheckCurrentMemberIsInRange(activities_Detail.Rows[i]["MemberGrades"].ToString(), activities_Detail.Rows[i]["DefualtGroup"].ToString(), activities_Detail.Rows[i]["CustomGroup"].ToString(), this.CurrentMemberInfo.UserId))
									{
										if (bool.Parse(this.dtActivities.Rows[j]["isAllProduct"].ToString()))
										{
											if (decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString()) > 0m)
											{
												if (num4 != 0m && num4 >= decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
													d = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
													literal6.Text = activities_Detail.Rows[activities_Detail.Rows.Count - 1]["bFreeShipping"].ToString();
													break;
												}
												if (num4 != 0m && num4 < decimal.Parse(activities_Detail.Rows[0]["MeetMoney"].ToString()))
												{
													break;
												}
												if (num4 != 0m && num4 >= decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString());
													d = decimal.Parse(activities_Detail.Rows[i]["ReductionMoney"].ToString());
													literal6.Text = activities_Detail.Rows[i]["bFreeShipping"].ToString();
												}
											}
											else
											{
												if (num5 != 0 && num5 >= int.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetNumber"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
													num3 = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
													flag2 = true;
													literal6.Text = activities_Detail.Rows[activities_Detail.Rows.Count - 1]["bFreeShipping"].ToString();
													break;
												}
												if (num5 != 0 && num5 < int.Parse(activities_Detail.Rows[0]["MeetNumber"].ToString()))
												{
													break;
												}
												if (num5 != 0 && num5 >= int.Parse(activities_Detail.Rows[i]["MeetNumber"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString());
													num3 = decimal.Parse(activities_Detail.Rows[i]["ReductionMoney"].ToString());
													flag2 = true;
													literal6.Text = activities_Detail.Rows[i]["bFreeShipping"].ToString();
												}
											}
										}
										else
										{
											num4 = num;
											num5 = num6;
											if (decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString()) > 0m)
											{
												if (num4 != 0m && num4 >= decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
													d = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
													literal6.Text = activities_Detail.Rows[activities_Detail.Rows.Count - 1]["bFreeShipping"].ToString();
													break;
												}
												if (num4 != 0m && num4 < decimal.Parse(activities_Detail.Rows[0]["MeetMoney"].ToString()))
												{
													break;
												}
												if (num4 != 0m && num4 >= decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString());
													d = decimal.Parse(activities_Detail.Rows[i]["ReductionMoney"].ToString());
													literal6.Text = activities_Detail.Rows[i]["bFreeShipping"].ToString();
												}
											}
											else
											{
												if (num5 != 0 && num5 >= int.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetNumber"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
													d = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
													flag2 = true;
													literal6.Text = activities_Detail.Rows[activities_Detail.Rows.Count - 1]["bFreeShipping"].ToString();
													break;
												}
												if (num5 != 0 && num5 < int.Parse(activities_Detail.Rows[0]["MeetNumber"].ToString()))
												{
													break;
												}
												if (num5 != 0 && num5 >= int.Parse(activities_Detail.Rows[i]["MeetNumber"].ToString()))
												{
													num2 = decimal.Parse(activities_Detail.Rows[i]["MeetMoney"].ToString());
													d = decimal.Parse(activities_Detail.Rows[i]["ReductionMoney"].ToString());
													flag2 = true;
													literal6.Text = activities_Detail.Rows[i]["bFreeShipping"].ToString();
												}
											}
										}
									}
								}
								if (flag2)
								{
									if (num5 > 0)
									{
										num3 += d;
									}
								}
								else if (num4 != 0m && num2 != 0m && num4 >= num2)
								{
									num3 += d;
								}
							}
						}
					}
				}
				literal2.Text = num3.ToString("F2");
				literal3.Text = num3.ToString("F2");
				literal5.Text = (d2 - num3).ToString("F2");
				literal4.Text = (d2 - num3).ToString("F2");
			}
		}


        protected override void AttachChildControls()
		{
			this.litShipTo = (System.Web.UI.WebControls.Literal)this.FindControl("litShipTo");
			this.litIsUseBalance = (System.Web.UI.WebControls.Literal)this.FindControl("litIsUseBalance");
			this.litCellPhone = (System.Web.UI.WebControls.Literal)this.FindControl("litCellPhone");
			this.litAddress = (System.Web.UI.WebControls.Literal)this.FindControl("litAddress");
			this.litShowMes = (System.Web.UI.WebControls.Literal)this.FindControl("litShowMes");
			this.GetUserCoupons = MemberProcessor.GetUserCoupons();
			this.rptCartProducts = (VshopTemplatedRepeater)this.FindControl("rptCartProducts");
			this.rptCartProducts.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.rptCartProducts_ItemDataBound);
			this.litOrderTotal = (System.Web.UI.WebControls.Literal)this.FindControl("litOrderTotal");
			this.litPointNumber = (System.Web.UI.WebControls.Literal)this.FindControl("litPointNumber");
			this.litUseMembersPointShow = (System.Web.UI.WebControls.Literal)this.FindControl("litUseMembersPointShow");
			this.litDisplayPointNumber = (System.Web.UI.WebControls.Literal)this.FindControl("litDisplayPointNumber");
			this.litDisplayPoint = (System.Web.UI.WebControls.Literal)this.FindControl("litDisplayPoint");
			this.BalanceCanPayMoney = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("BalanceCanPayMoney");
			this.groupbuyHiddenBox = (System.Web.UI.HtmlControls.HtmlInputControl)this.FindControl("groupbuyHiddenBox");
			this.rptAddress = (VshopTemplatedRepeater)this.FindControl("rptAddress");
			this.selectShipTo = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("selectShipTo");
			this.MembersPointMoney = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("MembersPointMoney");
			this.regionId = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("regionId");
			this.litAddAddress = (System.Web.UI.WebControls.Literal)this.FindControl("litAddAddress");

            if (Globals.GetCurrentDistributorId() == CurrentMemberInfo.UserId)
            {
                base.GotoResourceNotFound("站点管理员无法在自己店内购买！");
                return;
            }
            if ( Globals.GetCurrentDistributorId()==0)//CurrentMemberInfo.ReferralUserId ==0  &&
            {
                base.GotoResourceNotFound("请先扫码关注站点后再下单！");
                return;
            }


            System.Collections.Generic.IList<ShippingAddressInfo> shippingAddresses = MemberProcessor.GetShippingAddresses();
			this.rptAddress.DataSource = from item in shippingAddresses
			orderby item.IsDefault
			select item;
			this.rptAddress.DataBind();
			ShippingAddressInfo shippingAddressInfo = shippingAddresses.FirstOrDefault((ShippingAddressInfo item) => item.IsDefault);

            if (!string.IsNullOrEmpty(this.Page.Request.QueryString["startDate"]))
            {
                this.SendStartDate = DateTime.Parse(this.Page.Request.QueryString["startDate"]);
            }
            if (!string.IsNullOrEmpty(this.Page.Request.QueryString["quantityPerDay"]))
            {
                this.QuantityPerDay = Convert.ToInt32(this.Page.Request.QueryString["quantityPerDay"]);
            }
            if (!string.IsNullOrEmpty(this.Page.Request.QueryString["sendDays"]))
            {
                this.SendDays = Convert.ToInt32(this.Page.Request.QueryString["sendDays"]);
            }


            if (shippingAddressInfo == null)
			{
				shippingAddressInfo = ((shippingAddresses.Count > 0) ? shippingAddresses[0] : null);
			}
			if (shippingAddressInfo != null)
			{
				this.litShipTo.Text = shippingAddressInfo.ShipTo;
				this.litCellPhone.Text = shippingAddressInfo.CellPhone;
				this.litAddress.Text = shippingAddressInfo.Address;
				this.selectShipTo.SetWhenIsNotNull(shippingAddressInfo.ShippingId.ToString());
				this.regionId.SetWhenIsNotNull(shippingAddressInfo.RegionId.ToString());
			}
			this.litAddAddress.Text = "<li><a href='/Vshop/AddShippingAddress.aspx?returnUrl=" + Globals.UrlEncode(System.Web.HttpContext.Current.Request.Url.ToString()) + "'>新增收货地址</a></li>";
			if (shippingAddresses == null || shippingAddresses.Count == 0)
			{
				this.Page.Response.Redirect(Globals.ApplicationPath + "/Vshop/AddShippingAddress.aspx?returnUrl=" + Globals.UrlEncode(System.Web.HttpContext.Current.Request.Url.ToString()));
			}
			else
			{
				System.Collections.Generic.List<ShoppingCartInfo> list = new System.Collections.Generic.List<ShoppingCartInfo>();
				if (int.TryParse(this.Page.Request.QueryString["buyAmount"], out this.buyAmount) && !string.IsNullOrEmpty(this.Page.Request.QueryString["productSku"]) && !string.IsNullOrEmpty(this.Page.Request.QueryString["from"]) && (this.Page.Request.QueryString["from"] == "signBuy" || this.Page.Request.QueryString["from"] == "groupBuy"))
				{
					this.productSku = this.Page.Request.QueryString["productSku"];
					if (this.isbargain)
					{
						int bargainDetialId = Globals.RequestQueryNum("bargainDetialId");
						list = ShoppingCartProcessor.GetListShoppingCart(this.productSku, this.buyAmount, bargainDetialId, 0);
					}
					else
					{
						int num = this.buyAmount;
						int num2 = Globals.RequestQueryNum("limitedTimeDiscountId");
						if (num2 > 0)
						{
							bool flag = true;
							LimitedTimeDiscountInfo discountInfo = LimitedTimeDiscountHelper.GetDiscountInfo(num2);
							if (discountInfo == null)
							{
								flag = false;
							}
							if (flag)
							{
								if (MemberHelper.CheckCurrentMemberIsInRange(discountInfo.ApplyMembers, discountInfo.DefualtGroup, discountInfo.CustomGroup, this.CurrentMemberInfo.UserId))
								{
									if (discountInfo.LimitNumber != 0)
									{
										int limitedTimeDiscountUsedNum = ShoppingCartProcessor.GetLimitedTimeDiscountUsedNum(num2, this.productSku, 0, this.CurrentMemberInfo.UserId, false);
										if (this.buyAmount > discountInfo.LimitNumber - limitedTimeDiscountUsedNum)
										{
											num = discountInfo.LimitNumber - limitedTimeDiscountUsedNum;
										}
									}
								}
								else
								{
									num2 = 0;
								}
							}
							else
							{
								num2 = 0;
							}
						}
						if (num2 > 0)
						{
							ShoppingCartProcessor.RemoveLineItem(this.productSku, 0, num2);
						}
						if (num == 0 && num2 > 0)
						{
							num = this.buyAmount;
							num2 = 0;
						}
						list = ShoppingCartProcessor.GetListShoppingCart(SendStartDate,QuantityPerDay,SendDays, this.productSku, num, 0, num2);
					}
				}
				else
				{
					list = ShoppingCartProcessor.GetOrderSummitCart();
				}
				if (list == null)
				{
					System.Web.HttpContext.Current.Response.Write("<script>alert('商品已下架或没有需要结算的订单！');location.href='/Vshop/ShoppingCart.aspx'</script>");
				}
				else
				{
					if (list.Count > 1)
					{
						this.litShowMes.Text = "<div style=\"color: #F60; \"><img  src=\"/Utility/pics/u77.png\">您所购买的商品不支持同一个物流规则发货，系统自动拆分成多个子订单处理</div>";
					}
					this.rptCartProducts.DataSource = list;
					this.rptCartProducts.DataBind();
					decimal d = 0m;
					decimal num3 = 0m;
					decimal d2 = 0m;
					int num4 = 0;
					foreach (ShoppingCartInfo current in list)
					{
						num4 += current.GetPointNumber;
						d += current.Total;
						num3 += current.Exemption;
						d2 += current.ShipCost;
					}
					decimal d3 = num3;
					decimal num5 = d - d3;
					if (num5 <= 0m)
					{
						num5 = 0m;
					}
					num5 = decimal.Round(num5, 2);
					this.litOrderTotal.Text = num5.ToString("F2");
					if (num4 == 0)
					{
						this.litDisplayPointNumber.Text = "style=\"display:none;\"";
					}
					this.litPointNumber.Text = num4.ToString();
					int num6 = this.CurrentMemberInfo.Points - num4;
					SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
					decimal num7;
					if (num5 * masterSettings.PointToCashRate > this.CurrentMemberInfo.Points)
					{
						if (num6 > masterSettings.PonitToCash_MaxAmount * masterSettings.PointToCashRate)
						{
							num7 = masterSettings.PonitToCash_MaxAmount;
							num6 = (int)masterSettings.PonitToCash_MaxAmount * masterSettings.PointToCashRate;
						}
						else
						{
							num7 = num6 / masterSettings.PointToCashRate;
						}
					}
					else
					{
						num7 = masterSettings.PonitToCash_MaxAmount;
						if (num7 > num5)
						{
							num7 = num5;
						}
						num7 = decimal.Round(num7, 2);
						num6 = (int)(num7 * masterSettings.PointToCashRate);
					}
					if (num6 <= 0)
					{
						num6 = 0;
						num7 = 0m;
					}
					this.MembersPointMoney.Value = num7.ToString("F2");
					if (num6 > 0)
					{
						this.litUseMembersPointShow.Text = string.Concat(new object[]
						{
							"<input type='hidden' id='hdCanUsePoint' value='",
							num6,
							"'/><input type='hidden' id='hdCanUsePointMoney' value='",
							num7.ToString("F2"),
							"'/><div class=\"prompt-text pull-left\" id=\"divUseMembersPointShow\">可用<span  id=\"usepointnum\">",
							num6,
							"</span>积分抵 <span class=\"colorr\">¥<span  id=\"usepointmoney\">",
							num7.ToString("F2"),
							"</span></span>元</div><div class=\"switch pull-right\" id=\"mySwitchUseMembersPoint\"><input  type=\"checkbox\" /></div>"
						});
					}
					else
					{
						this.litUseMembersPointShow.Text = "<input type='hidden' id='hdCanUsePoint' value='0'/><input type='hidden' id='hdCanUsePointMoney' value='0'/><div class=\"prompt-text pull-left\" id=\"divUseMembersPointShow\">可用<span  id=\"usepointnum\">0</span>积分 <span  id=\"usepointmoney\" style=\"display:none\">" + num7.ToString("F2") + "</span></div><div class=\"switch pull-right\" id=\"mySwitchUseMembersPoint\" style=\"display:none\"><input type=\"checkbox\" disabled /></div>";
					}
					decimal d4;
					if (num5 > this.CurrentMemberInfo.AvailableAmount)
					{
						d4 = this.CurrentMemberInfo.AvailableAmount;
						this.BalanceCanPayMoney.Value = this.CurrentMemberInfo.AvailableAmount.ToString("F2");
					}
					else
					{
						d4 = num5;
						this.BalanceCanPayMoney.Value = num5.ToString("F2");
					}
					if (this.CurrentMemberInfo.AvailableAmount > 0m && masterSettings.EnableBalancePayment)
					{
						this.litIsUseBalance.Text = string.Concat(new string[]
						{
							"<div class=\"prompt-text pull-left\">余额支付 <span class=\"colorr\">¥<span id=\"spCanpayMoney\">",
							d4.ToString("F2"),
							"</span></span>(可用 ¥<span id=\"spAvailableAmount\">",
							this.CurrentMemberInfo.AvailableAmount.ToString("F2"),
							"</span>)</div><div class=\"switch pull-right\" id=\"mySwitchUseBalance\"><input type=\"checkbox\" ",
							(d4 > 0m) ? "" : " disabled",
							" /></div></div>"
						});
					}
					else
					{
						this.litIsUseBalance.Text = "<div class=\"prompt-text pull-left\"" + (masterSettings.EnableBalancePayment ? "" : " style=\"display:none\"") + ">余额可用 <span class=\"colorr\">¥<span id=\"spCanpayMoney\">0.00</span></span><span id=\"spAvailableAmount\" style=\"display:none\">0.00</span></div><div class=\"switch pull-right\" id=\"mySwitchUseBalance\" style=\"display:none\"><input type=\"checkbox\" disabled /></div></div>";
					}
					if (!masterSettings.PonitToCash_Enable)
					{
						this.litDisplayPoint.Text = " style=\"display:none;\"";
					}
					PageTitle.AddSiteNameTitle("订单确认");
				}
			}
		}

		public decimal DiscountMoney(System.Collections.Generic.List<ShoppingCartInfo> infoList)
		{
			decimal d = 0m;
			decimal num = 0m;
			decimal num2 = 0m;
			decimal d2 = 0m;
			int num3 = 0;
			foreach (ShoppingCartInfo current in infoList)
			{
				foreach (ShoppingCartItemInfo current2 in current.LineItems)
				{
					if (current2.Type == 0)
					{
						d2 += current2.SubTotal;
						num3 += current2.Quantity;
					}
				}
			}
			for (int i = 0; i < this.dtActivities.Rows.Count; i++)
			{
				decimal num4 = 0m;
				int num5 = 0;
				DataTable activities_Detail = ActivityHelper.GetActivities_Detail(int.Parse(this.dtActivities.Rows[i]["ActivitiesId"].ToString()));
				foreach (ShoppingCartInfo current in infoList)
				{
					foreach (ShoppingCartItemInfo current2 in current.LineItems)
					{
						if (current2.Type == 0)
						{
							DataTable activitiesProducts = ActivityHelper.GetActivitiesProducts(int.Parse(this.dtActivities.Rows[i]["ActivitiesId"].ToString()), current2.ProductId);
							if (activitiesProducts.Rows.Count > 0)
							{
								num4 += current2.SubTotal;
								num5 += current2.Quantity;
							}
						}
					}
				}
				if (activities_Detail.Rows.Count > 0)
				{
					for (int j = 0; j < activities_Detail.Rows.Count; j++)
					{
						if (bool.Parse(this.dtActivities.Rows[i]["isAllProduct"].ToString()))
						{
							if (decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString()) > 0m)
							{
								if (d2 >= decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
									break;
								}
								if (d2 < decimal.Parse(activities_Detail.Rows[0]["MeetMoney"].ToString()))
								{
									break;
								}
								if (d2 >= decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[j]["ReductionMoney"].ToString());
								}
							}
							else
							{
								if (num3 >= int.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetNumber"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
									num2 = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
									break;
								}
								if (num3 < int.Parse(activities_Detail.Rows[0]["MeetNumber"].ToString()))
								{
									break;
								}
								if (num3 >= int.Parse(activities_Detail.Rows[j]["MeetNumber"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString());
									num2 = decimal.Parse(activities_Detail.Rows[j]["ReductionMoney"].ToString());
								}
							}
						}
						else
						{
							d2 = num4;
							num3 = num5;
							if (decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString()) > 0m)
							{
								if (d2 >= decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
									break;
								}
								if (d2 <= decimal.Parse(activities_Detail.Rows[0]["MeetMoney"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[0]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[0]["ReductionMoney"].ToString());
									break;
								}
								if (d2 >= decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[j]["ReductionMoney"].ToString());
								}
							}
							else
							{
								if (num3 >= int.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetNumber"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[activities_Detail.Rows.Count - 1]["ReductionMoney"].ToString());
									break;
								}
								if (num3 < int.Parse(activities_Detail.Rows[0]["MeetNumber"].ToString()))
								{
									break;
								}
								if (num3 >= int.Parse(activities_Detail.Rows[j]["MeetNumber"].ToString()))
								{
									num = decimal.Parse(activities_Detail.Rows[j]["MeetMoney"].ToString());
									d = decimal.Parse(activities_Detail.Rows[j]["ReductionMoney"].ToString());
								}
							}
						}
					}
					if (d2 >= num || num == 0m)
					{
						num2 += d;
					}
				}
			}
			return num2;
		}
	}
}
