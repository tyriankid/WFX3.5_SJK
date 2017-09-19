using ASPNET.WebControls;
using Hidistro.ControlPanel.Promotions;
using Hidistro.Core;
using Hidistro.Core.Enums;
using Hidistro.Entities.Promotions;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.promotion
{
	public class MemberCouponList : AdminPage
	{
		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected PageSize hrefPageSize;

		protected System.Web.UI.WebControls.TextBox txt_name;

		protected System.Web.UI.WebControls.TextBox txt_orderNo;

		protected System.Web.UI.WebControls.Button btnSeach;

		protected Grid grdCoupondsList;

		protected Pager pager1;

		protected MemberCouponList() : base("m08", "yxp01")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.btnSeach.Click += new System.EventHandler(this.btnImagetSearch_Click);
			if (!base.IsPostBack)
			{
				this.BindData();
			}
		}

		protected void btnImagetSearch_Click(object sender, System.EventArgs e)
		{
			string text = this.txt_name.Text.Trim();
			string text2 = this.txt_orderNo.Text.Trim();
			string text3 = "MemberCouponList.aspx?";
			if (!string.IsNullOrEmpty(text))
			{
				text3 = text3 + "&cname=" + base.Server.UrlEncode(text);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				text3 = text3 + "&cno=" + base.Server.UrlEncode(text2);
			}
			base.Response.Redirect(text3.Trim(new char[]
			{
				'?'
			}));
		}

		private void BindData()
		{
			string text = Globals.RequestQueryStr("cname").Trim();
			string text2 = Globals.RequestQueryStr("cno").Trim();
			if (!string.IsNullOrEmpty(text))
			{
				this.txt_name.Text = text;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				this.txt_orderNo.Text = text2;
			}
			string arg_53_0 = this.txt_name.Text;
			string arg_5F_0 = this.txt_orderNo.Text;
			int totalRecords = 0;
			System.Data.DataTable memberCoupons = CouponHelper.GetMemberCoupons(new MemberCouponsSearch
			{
				CouponName = text,
				OrderNo = text2,
				IsCount = true,
				PageIndex = this.pager1.PageIndex,
				PageSize = this.pager1.PageSize,
				SortBy = "CouponId",
				SortOrder = SortAction.Desc
			}, ref totalRecords);
			if (memberCoupons != null && memberCoupons.Rows.Count > 0)
			{
				memberCoupons.Columns.Add("useConditon");
				memberCoupons.Columns.Add("sStatus");
				for (int i = 0; i < memberCoupons.Rows.Count; i++)
				{
					decimal d = decimal.Parse(memberCoupons.Rows[i]["ConditionValue"].ToString());
					if (d == 0m)
					{
						memberCoupons.Rows[i]["useConditon"] = "不限制";
					}
					else
					{
						memberCoupons.Rows[i]["useConditon"] = "满" + d.ToString("F2") + "可使用";
					}
					memberCoupons.Rows[i]["sStatus"] = ((int.Parse(memberCoupons.Rows[i]["Status"].ToString()) == 0) ? "已领取" : "已使用");
				}
			}
			this.grdCoupondsList.DataSource = memberCoupons;
			this.grdCoupondsList.DataBind();
			this.pager1.TotalRecords = totalRecords;
		}
	}
}
