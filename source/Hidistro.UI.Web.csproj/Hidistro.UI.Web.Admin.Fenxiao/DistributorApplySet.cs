using Hidistro.ControlPanel.Members;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using Hidistro.UI.Web.hieditor.ueditor.controls;
using System;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
	public class DistributorApplySet : AdminPage
	{
		public string tabnum = "0";

		public string productHtml = "";

		protected bool _EnableCommission;

		protected bool _CommissionAutoToBalance;

		protected bool _IsRequestDistributor = true;

		protected bool _DistributorApplicationCondition;

		protected bool _EnableMemberAutoToDistributor;

		protected bool _IsShowDistributorSelfStoreName;

		protected bool _IsDistributorBuyCanGetCommission;

		protected Hidistro.UI.Common.Controls.Style Style1;

		protected Script Script4;

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.HtmlControls.HtmlInputCheckBox radioDistributorApplicationCondition;

		protected System.Web.UI.HtmlControls.HtmlInputCheckBox cbRechargeMoneyToDistributor;

		protected System.Web.UI.HtmlControls.HtmlInputText txtRechargeMoneyToDistributor;

		protected System.Web.UI.HtmlControls.HtmlInputCheckBox HasConditions;

		protected System.Web.UI.HtmlControls.HtmlInputText txtrequestmoney;

		protected System.Web.UI.HtmlControls.HtmlInputCheckBox HasProduct;

		protected ucDateTimePicker calendarStartDate;

		protected ucDateTimePicker calendarEndDate;

		protected System.Web.UI.WebControls.Button btnSave;

		protected System.Web.UI.HtmlControls.HtmlInputHidden hiddProductId;

		protected ucUeditor fckDescription;

		protected System.Web.UI.WebControls.Button Button1;

		protected DistributorApplySet() : base("m05", "fxp02")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				this.bindData();
			}
		}

		protected void bindData()
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			this.tabnum = base.Request.QueryString["tabnum"];
			if (string.IsNullOrEmpty(this.tabnum))
			{
				this.tabnum = "0";
			}
			this.txtrequestmoney.Value = masterSettings.FinishedOrderMoney.ToString();
			this.fckDescription.Text = masterSettings.DistributorDescription;
			this.radioDistributorApplicationCondition.Checked = masterSettings.DistributorApplicationCondition;
			this._DistributorApplicationCondition = masterSettings.DistributorApplicationCondition;
			this._EnableMemberAutoToDistributor = masterSettings.EnableMemberAutoToDistributor;
			this._IsShowDistributorSelfStoreName = masterSettings.IsShowDistributorSelfStoreName;
			this._IsDistributorBuyCanGetCommission = masterSettings.IsDistributorBuyCanGetCommission;
			this._IsRequestDistributor = true;
			if (!masterSettings.IsRequestDistributor)
			{
				this._IsRequestDistributor = false;
			}
			this._CommissionAutoToBalance = false;
			if (masterSettings.CommissionAutoToBalance)
			{
				this._CommissionAutoToBalance = true;
			}
			this._EnableCommission = false;
			if (masterSettings.EnableCommission)
			{
				this._EnableCommission = true;
			}
			if (masterSettings.RechargeMoneyToDistributor > 0m)
			{
				this.cbRechargeMoneyToDistributor.Checked = true;
				this.txtRechargeMoneyToDistributor.Value = masterSettings.RechargeMoneyToDistributor.ToString("F2");
			}
			if (masterSettings.FinishedOrderMoney > 0)
			{
				this.HasConditions.Checked = true;
			}
			this.HasProduct.Checked = masterSettings.EnableDistributorApplicationCondition;
			string distributorProducts = masterSettings.DistributorProducts;
			if (!string.IsNullOrEmpty(distributorProducts))
			{
				this.hiddProductId.Value = distributorProducts;
			}
			if (!string.IsNullOrEmpty(masterSettings.DistributorProductsDate) && masterSettings.DistributorProductsDate.Contains("|"))
			{
				if (!string.IsNullOrEmpty(masterSettings.DistributorProductsDate.Split(new char[]
				{
					'|'
				})[0]))
				{
					this.calendarStartDate.SelectedDate = new System.DateTime?(System.Convert.ToDateTime(masterSettings.DistributorProductsDate.Split(new char[]
					{
						'|'
					})[0]));
				}
				if (!string.IsNullOrEmpty(masterSettings.DistributorProductsDate.Split(new char[]
				{
					'|'
				})[1]))
				{
					this.calendarEndDate.SelectedDate = new System.DateTime?(System.Convert.ToDateTime(masterSettings.DistributorProductsDate.Split(new char[]
					{
						'|'
					})[1]));
				}
			}
			string text = Globals.RequestFormStr("type");
			string key;
			switch (key = text)
			{
			case "EnableCommission":
				try
				{
					base.Response.ContentType = "text/plain";
					bool enableCommission = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.EnableCommission = enableCommission;
					SettingsManager.Save(masterSettings);
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex)
				{
					base.Response.Write("保存失败！（" + ex.ToString() + ")");
				}
				base.Response.End();
				return;
			case "CommissionAutoToBalance":
				try
				{
					base.Response.ContentType = "text/plain";
					bool commissionAutoToBalance = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.CommissionAutoToBalance = commissionAutoToBalance;
					SettingsManager.Save(masterSettings);
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex2)
				{
					base.Response.Write("保存失败！（" + ex2.ToString() + ")");
				}
				base.Response.End();
				return;
			case "IsRequestDistributor":
				try
				{
					base.Response.ContentType = "text/plain";
					bool isRequestDistributor = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.IsRequestDistributor = isRequestDistributor;
					SettingsManager.Save(masterSettings);
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex3)
				{
					base.Response.Write("保存失败！（" + ex3.ToString() + ")");
				}
				base.Response.End();
				return;
			case "IsShowDistributorSelfStoreName":
				try
				{
					base.Response.ContentType = "text/plain";
					bool isShowDistributorSelfStoreName = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.IsShowDistributorSelfStoreName = isShowDistributorSelfStoreName;
					SettingsManager.Save(masterSettings);
					MemberHelper.UpdateSetCardCreatTime();
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex4)
				{
					base.Response.Write("保存失败！（" + ex4.ToString() + ")");
				}
				base.Response.End();
				return;
			case "IsDistributorBuyCanGetCommission":
				try
				{
					base.Response.ContentType = "text/plain";
					bool isDistributorBuyCanGetCommission = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.IsDistributorBuyCanGetCommission = isDistributorBuyCanGetCommission;
					SettingsManager.Save(masterSettings);
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex5)
				{
					base.Response.Write("保存失败！（" + ex5.ToString() + ")");
				}
				base.Response.End();
				return;
			case "DistributorApplicationCondition":
				try
				{
					base.Response.ContentType = "text/plain";
					bool distributorApplicationCondition = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.DistributorApplicationCondition = distributorApplicationCondition;
					SettingsManager.Save(masterSettings);
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex6)
				{
					base.Response.Write("保存失败！（" + ex6.ToString() + ")");
				}
				base.Response.End();
				return;
			case "EnableMemberAutoToDistributor":
				try
				{
					base.Response.ContentType = "text/plain";
					bool enableMemberAutoToDistributor = bool.Parse(Globals.RequestFormStr("enable"));
					masterSettings.EnableMemberAutoToDistributor = enableMemberAutoToDistributor;
					SettingsManager.Save(masterSettings);
					base.Response.Write("保存成功");
				}
				catch (System.Exception ex7)
				{
					base.Response.Write("保存失败！（" + ex7.ToString() + ")");
				}
				base.Response.End();
				break;

				return;
			}
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			this.tabnum = "0";
			int num = 0;
			this._DistributorApplicationCondition = true;
			if (this.radioDistributorApplicationCondition.Checked && !this.HasProduct.Checked && !this.HasConditions.Checked && !this.cbRechargeMoneyToDistributor.Checked)
			{
				this.ShowMsg("请选择分销商申请条件", false);
				return;
			}
			if (this.HasConditions.Checked && (!int.TryParse(this.txtrequestmoney.Value.Trim(), out num) || num < 1))
			{
				this.ShowMsg("累计消费金额必须为大于0的整数金额", false);
				return;
			}
			decimal rechargeMoneyToDistributor = 0m;
			if (this.cbRechargeMoneyToDistributor.Checked)
			{
				if (string.IsNullOrEmpty(this.txtRechargeMoneyToDistributor.Value))
				{
					this.ShowMsg("请填写账户单次充值金额", false);
					return;
				}
				decimal num2 = 0m;
				decimal.TryParse(this.txtRechargeMoneyToDistributor.Value, out num2);
				if (!(num2 > 0m))
				{
					this.ShowMsg("账户单次充值金额必须大于0", false);
					return;
				}
				rechargeMoneyToDistributor = num2;
			}
			if (this.HasProduct.Checked && string.IsNullOrEmpty(this.hiddProductId.Value))
			{
				this.ShowMsg("请选择指定商品", false);
				return;
			}
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			masterSettings.DistributorApplicationCondition = this.radioDistributorApplicationCondition.Checked;
			masterSettings.RechargeMoneyToDistributor = rechargeMoneyToDistributor;
			masterSettings.FinishedOrderMoney = num;
			masterSettings.EnableDistributorApplicationCondition = this.HasProduct.Checked;
			if (this.HasProduct.Checked)
			{
				masterSettings.DistributorProducts = this.hiddProductId.Value;
			}
			else
			{
				masterSettings.DistributorProducts = "";
			}
			string text = "";
			if (this.calendarStartDate.SelectedDate.HasValue)
			{
				text = this.calendarStartDate.SelectedDate.Value.ToString();
			}
			text += "|";
			if (this.calendarEndDate.SelectedDate.HasValue)
			{
				text += this.calendarEndDate.SelectedDate.Value.ToString();
			}
			masterSettings.DistributorProductsDate = text;
			SettingsManager.Save(masterSettings);
			System.Web.HttpCookie httpCookie = System.Web.HttpContext.Current.Request.Cookies["Admin-Product"];
			if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
			{
				httpCookie.Value = null;
				httpCookie.Expires = System.DateTime.Now.AddYears(-1);
				System.Web.HttpContext.Current.Response.Cookies.Set(httpCookie);
			}
			this.bindData();
			this.ShowMsgAndReUrl("修改成功", true, "DistributorApplySet.aspx");
		}

		protected void btnSave_Description(object sender, System.EventArgs e)
		{
			this.tabnum = "1";
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			masterSettings.DistributorDescription = this.fckDescription.Text.Trim();
			SettingsManager.Save(masterSettings);
			this.ShowMsg("分销说明修改成功", true);
		}
	}
}
