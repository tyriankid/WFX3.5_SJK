using ASPNET.WebControls;
using Hidistro.ControlPanel.Members;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Member
{
	public class MemberAmountList : AdminPage
	{
		private string PayId = "";

		private string UserName = "";

		private string TradeTypeValue = "";

		private string TradeWaysValue = "";

		private string StartTime = "";

		private string EndTime = "";

		protected decimal CurrentTotal;

		protected decimal AvailableTotal;

		protected decimal UnliquidatedTotal;

		public int lastDay;

		protected System.Web.UI.WebControls.TextBox txtStoreName;

		protected System.Web.UI.WebControls.TextBox txtOrderId;

		protected System.Web.UI.WebControls.DropDownList TradeTypeList;

		protected System.Web.UI.WebControls.DropDownList TradeWaysList;

		protected System.Web.UI.WebControls.HiddenField hidType;

		protected System.Web.UI.WebControls.HiddenField hidWays;

		protected ucDateTimePicker calendarStartDate;

		protected ucDateTimePicker calendarEndDate;

		protected System.Web.UI.WebControls.Button btnQueryLogs;

		protected System.Web.UI.WebControls.Button Button1;

		protected System.Web.UI.WebControls.Button Button4;

		protected System.Web.UI.WebControls.Repeater reCommissions;

		protected Pager pager;

		protected MemberAmountList() : base("m04", "hyp12")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.LoadDropDownList();
			this.LoadParameters();
			if (!base.IsPostBack)
			{
				this.BindData();
			}
		}

		private void BindData()
		{
			MemberAmountQuery memberAmountQuery = new MemberAmountQuery();
			memberAmountQuery.UserName = this.UserName;
			memberAmountQuery.PayId = this.PayId;
			memberAmountQuery.TradeType = this.TradeTypeValue;
			memberAmountQuery.TradeWays = this.TradeWaysValue;
			memberAmountQuery.EndTime = this.EndTime;
			memberAmountQuery.StartTime = this.StartTime;
			memberAmountQuery.PageIndex = this.pager.PageIndex;
			memberAmountQuery.PageSize = this.pager.PageSize;
			memberAmountQuery.SortOrder = SortAction.Desc;
			memberAmountQuery.SortBy = "Id";
			Globals.EntityCoding(memberAmountQuery, true);
			DbQueryResult amountWithUserName = MemberAmountProcessor.GetAmountWithUserName(memberAmountQuery);
			this.reCommissions.DataSource = amountWithUserName.Data;
			this.reCommissions.DataBind();
			this.pager.TotalRecords = amountWithUserName.TotalRecords;
			System.Collections.Generic.Dictionary<string, decimal> amountDic = MemberAmountProcessor.GetAmountDic(memberAmountQuery);
			this.CurrentTotal = amountDic["CurrentTotal"];
			this.AvailableTotal = amountDic["AvailableTotal"];
			this.UnliquidatedTotal = amountDic["UnliquidatedTotal"];
		}

		protected void Button1_Click1(object sender, System.EventArgs e)
		{
			System.DateTime now = System.DateTime.Now;
			this.EndTime = now.ToString("yyyy-MM-dd");
			this.StartTime = now.AddDays(-6.0).ToString("yyyy-MM-dd");
			this.lastDay = 7;
			this.PayId = this.txtOrderId.Text;
			this.UserName = this.txtStoreName.Text;
			this.TradeTypeValue = this.hidType.Value;
			this.TradeWaysValue = this.hidWays.Value;
			this.ReBind(true);
		}

		private void LoadDropDownList()
		{
			System.Collections.Generic.Dictionary<int, string> enumValueAndDescription = MemberHelper.GetEnumValueAndDescription(typeof(TradeType));
			this.TradeTypeList.Items.Clear();
			foreach (System.Collections.Generic.KeyValuePair<int, string> current in enumValueAndDescription)
			{
				System.Web.UI.WebControls.ListItem listItem = new System.Web.UI.WebControls.ListItem();
				listItem.Text = current.Value;
				listItem.Value = current.Key.ToString();
				this.TradeTypeList.Items.Add(listItem);
			}
			this.TradeTypeList.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-全部-", ""));
			System.Collections.Generic.Dictionary<int, string> enumValueAndDescription2 = MemberHelper.GetEnumValueAndDescription(typeof(TradeWays));
			this.TradeWaysList.Items.Clear();
			foreach (System.Collections.Generic.KeyValuePair<int, string> current2 in enumValueAndDescription2)
			{
				System.Web.UI.WebControls.ListItem listItem2 = new System.Web.UI.WebControls.ListItem();
				listItem2.Text = current2.Value;
				listItem2.Value = current2.Key.ToString();
				this.TradeWaysList.Items.Add(listItem2);
			}
			this.TradeWaysList.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-全部-", ""));
		}

		private void LoadParameters()
		{
			if (!this.Page.IsPostBack)
			{
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["UserName"]))
				{
					this.UserName = base.Server.UrlDecode(this.Page.Request.QueryString["UserName"]);
				}
				this.txtStoreName.Text = this.UserName;
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["PayId"]))
				{
					this.PayId = base.Server.UrlDecode(this.Page.Request.QueryString["PayId"]);
				}
				this.txtOrderId.Text = this.PayId;
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["TradeType"]))
				{
					this.TradeTypeValue = base.Server.UrlDecode(this.Page.Request.QueryString["TradeType"]);
				}
				this.hidType.Value = this.TradeTypeValue;
				this.TradeTypeList.SelectedValue = this.TradeTypeValue;
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["TradeWays"]))
				{
					this.TradeWaysValue = base.Server.UrlDecode(this.Page.Request.QueryString["TradeWays"]);
				}
				this.hidWays.Value = this.TradeWaysValue;
				this.TradeWaysList.SelectedValue = this.TradeWaysValue;
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["StartTime"]))
				{
					this.StartTime = base.Server.UrlDecode(this.Page.Request.QueryString["StartTime"]);
					this.calendarStartDate.SelectedDate = new System.DateTime?(System.DateTime.Parse(this.StartTime));
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["EndTime"]))
				{
					this.EndTime = base.Server.UrlDecode(this.Page.Request.QueryString["EndTime"]);
					this.calendarEndDate.SelectedDate = new System.DateTime?(System.DateTime.Parse(this.EndTime));
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["lastDay"]))
				{
					int.TryParse(this.Page.Request.QueryString["lastDay"], out this.lastDay);
					if (this.lastDay == 30)
					{
						this.Button1.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
						this.Button4.BorderColor = System.Drawing.ColorTranslator.FromHtml("#1CA47D");
						return;
					}
					if (this.lastDay == 7)
					{
						this.Button1.BorderColor = System.Drawing.ColorTranslator.FromHtml("#1CA47D");
						this.Button4.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
						return;
					}
					this.Button1.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
					this.Button4.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
					return;
				}
			}
			else
			{
				if (this.calendarStartDate.SelectedDate.HasValue)
				{
					this.StartTime = this.calendarStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");
				}
				if (this.calendarEndDate.SelectedDate.HasValue)
				{
					this.EndTime = this.calendarEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");
				}
			}
		}

		private void ReBind(bool isSearch)
		{
			System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();
			nameValueCollection.Add("PayId", this.PayId);
			nameValueCollection.Add("UserName", this.UserName);
			nameValueCollection.Add("StartTime", this.StartTime);
			nameValueCollection.Add("EndTime", this.EndTime);
			nameValueCollection.Add("TradeType", this.TradeTypeValue);
			nameValueCollection.Add("TradeWays", this.TradeWaysValue);
			nameValueCollection.Add("pageSize", this.pager.PageSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
			if (!isSearch)
			{
				nameValueCollection.Add("pageIndex", this.pager.PageIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
			nameValueCollection.Add("lastDay", this.lastDay.ToString());
			base.ReloadPage(nameValueCollection);
		}

		protected void Button4_Click1(object sender, System.EventArgs e)
		{
			System.DateTime now = System.DateTime.Now;
			this.EndTime = now.ToString("yyyy-MM-dd");
			this.StartTime = now.AddDays(-29.0).ToString("yyyy-MM-dd");
			this.lastDay = 30;
			this.PayId = this.txtOrderId.Text;
			this.UserName = this.txtStoreName.Text;
			this.TradeTypeValue = this.hidType.Value;
			this.TradeWaysValue = this.hidWays.Value;
			this.ReBind(true);
		}

		protected void btnQueryLogs_Click(object sender, System.EventArgs e)
		{
			if (this.calendarEndDate.SelectedDate.HasValue)
			{
				this.EndTime = this.calendarEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");
			}
			if (this.calendarStartDate.SelectedDate.HasValue)
			{
				this.StartTime = this.calendarStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");
			}
			this.PayId = this.txtOrderId.Text;
			this.UserName = this.txtStoreName.Text;
			this.TradeTypeValue = this.hidType.Value;
			this.TradeWaysValue = this.hidWays.Value;
			this.lastDay = 0;
			this.ReBind(true);
		}
	}
}
