using Hidistro.ControlPanel.Sales;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class VDistributorRegCheck : VshopTemplatedWebControl
	{
		private System.Web.UI.WebControls.Literal litAddTips;

		private System.Web.UI.HtmlControls.HtmlInputHidden litExpenditure;

		private System.Web.UI.HtmlControls.HtmlInputHidden litMemberAmountPass;

		private System.Web.UI.HtmlControls.HtmlInputHidden litIsMember;

		private System.Web.UI.HtmlControls.HtmlInputHidden litIsEnable;

		private System.Web.UI.HtmlControls.HtmlInputHidden litminMoney;

		private System.Web.UI.HtmlControls.HtmlInputHidden litProds;

		private System.Web.UI.HtmlControls.HtmlInputHidden litProdOK;

		private System.Web.UI.HtmlControls.HtmlInputHidden UserBindName;

		protected string IsEnable = "0";

		protected bool IsMemberAmountPass = false;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VDistributorRegCheck.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.litAddTips = (System.Web.UI.WebControls.Literal)this.FindControl("litAddTips");
			this.litIsEnable = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litIsEnable");
			this.litIsMember = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litIsMember");
			this.litExpenditure = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litExpenditure");
			this.litMemberAmountPass = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litMemberAmountPass");
			this.litminMoney = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litminMoney");
			this.litProds = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litProds");
			this.litProdOK = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litProdOK");
			int currentMemberUserId = Globals.GetCurrentMemberUserId(false);
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			if (masterSettings.DistributorApplicationCondition && masterSettings.RechargeMoneyToDistributor > 0m)
			{
				decimal userMaxAmountDetailed = MemberAmountProcessor.GetUserMaxAmountDetailed(currentMemberUserId);
				if (userMaxAmountDetailed >= masterSettings.RechargeMoneyToDistributor)
				{
					this.IsMemberAmountPass = true;
					this.litAddTips.Text = "<li class=\"pl50\"><a href=\"/Vshop/MemberRecharge.aspx\" style=\"color:red\"><i class=\"iconfont icon-tubiaoweb09 mr5 pull-left\"></i>一次性预存" + masterSettings.RechargeMoneyToDistributor.ToString("F2").Replace(".00", "") + "元，即可成为分销商！</a><p class=\"success\">已达成</p></li>";
				}
				else
				{
					this.litAddTips.Text = "<li class=\"pl50\"><a href=\"/Vshop/MemberRecharge.aspx\" style=\"color:red\"><i class=\"iconfont icon-tubiaoweb09 mr5 pull-left\"></i>一次性预存" + masterSettings.RechargeMoneyToDistributor.ToString("F2").Replace(".00", "") + "元，即可成为分销商！</a></li>";
				}
			}
			if (currentMemberUserId > 0)
			{
				this.litIsMember.Value = "1";
				DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(currentMemberUserId);
				MemberInfo currentMember = MemberProcessor.GetCurrentMember();
				if (currentMember == null)
				{
					this.Page.Response.Redirect("/Vshop/DistributorCenter.aspx");
					return;
				}
				this.UserBindName = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("UserBindName");
				this.UserBindName.Value = currentMember.UserBindName;
				decimal d = currentMember.Expenditure;
				if (userIdDistributors != null)
				{
					if (userIdDistributors.ReferralStatus == 0)
					{
						this.IsEnable = "1";
						this.Context.Response.Redirect("/Vshop/DistributorCenter.aspx");
						this.Context.Response.End();
					}
					else if (userIdDistributors.ReferralStatus == 1)
					{
						this.IsEnable = "3";
					}
					else if (userIdDistributors.ReferralStatus == 9)
					{
						this.IsEnable = "9";
					}
				}
				else
				{
					bool flag = VshopBrowser.IsPassAutoToDistributor(currentMember, true);
					if (flag)
					{
						if (!SystemAuthorizationHelper.CheckDistributorIsCanAuthorization())
						{
							DistributorsBrower.MemberAutoToDistributor(currentMember);
							this.Page.Response.Redirect("/Vshop/DistributorCenter.aspx", true);
							this.Page.Response.End();
							return;
						}
					}
					decimal num = 0m;
					DataTable userOrderPaidWaitFinish = OrderHelper.GetUserOrderPaidWaitFinish(currentMemberUserId);
					for (int i = 0; i < userOrderPaidWaitFinish.Rows.Count; i++)
					{
						OrderInfo orderInfo = OrderHelper.GetOrderInfo(userOrderPaidWaitFinish.Rows[i]["orderid"].ToString());
						if (orderInfo != null)
						{
							decimal total = orderInfo.GetTotal();
							if (total > 0m)
							{
								num += total;
							}
						}
					}
					d += num;
					if (!masterSettings.DistributorApplicationCondition)
					{
						bool flag2 = SystemAuthorizationHelper.CheckDistributorIsCanAuthorization();
						if (flag2)
						{
							this.IsEnable = "2";
						}
						else
						{
							this.IsEnable = "4";
						}
					}
					else
					{
						int finishedOrderMoney = masterSettings.FinishedOrderMoney;
						this.litminMoney.Value = finishedOrderMoney.ToString();
						if (finishedOrderMoney > 0 && d >= finishedOrderMoney)
						{
							bool flag2 = SystemAuthorizationHelper.CheckDistributorIsCanAuthorization();
							if (flag2)
							{
								this.IsEnable = "2";
							}
							else
							{
								this.IsEnable = "4";
							}
						}
						if (masterSettings.EnableDistributorApplicationCondition)
						{
							if (!string.IsNullOrEmpty(masterSettings.DistributorProductsDate))
							{
								if (!string.IsNullOrEmpty(masterSettings.DistributorProducts))
								{
									this.litProds.Value = masterSettings.DistributorProducts;
									if (masterSettings.DistributorProductsDate.Contains("|"))
									{
										System.DateTime value = default(System.DateTime);
										System.DateTime value2 = default(System.DateTime);
										System.DateTime.TryParse(masterSettings.DistributorProductsDate.Split(new char[]
										{
											'|'
										})[0].ToString(), out value);
										System.DateTime.TryParse(masterSettings.DistributorProductsDate.Split(new char[]
										{
											'|'
										})[1].ToString(), out value2);
										if (value.CompareTo(System.DateTime.Now) > 0 || value2.CompareTo(System.DateTime.Now) < 0)
										{
											this.litProds.Value = "";
											this.litIsEnable.Value = "0";
										}
										else if (MemberProcessor.CheckMemberIsBuyProds(currentMemberUserId, this.litProds.Value, new System.DateTime?(value), new System.DateTime?(value2)))
										{
											bool flag2 = SystemAuthorizationHelper.CheckDistributorIsCanAuthorization();
											if (flag2)
											{
												this.IsEnable = "2";
												this.litProdOK.Value = "(已购买指定商品,在" + value2.ToString("yyyy-MM-dd") + "之前申请有效)";
											}
											else
											{
												this.IsEnable = "4";
											}
										}
									}
								}
								else
								{
									this.IsEnable = "6";
								}
							}
						}
					}
				}
				this.litExpenditure.Value = d.ToString("F2");
				if (this.IsMemberAmountPass)
				{
					this.litMemberAmountPass.Value = "1";
				}
			}
			else
			{
				this.litIsMember.Value = "0";
			}
			this.litIsEnable.Value = this.IsEnable;
			PageTitle.AddSiteNameTitle("申请分销商");
		}
	}
}
