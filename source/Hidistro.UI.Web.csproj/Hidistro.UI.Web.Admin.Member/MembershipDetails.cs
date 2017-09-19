using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Store;
using Hidistro.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.Admin.Member
{
	[PrivilegeCheck(Privilege.Members)]
	public class MembershipDetails : AdminPage
	{
		protected int userid;

		protected HiImage ListImage1;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtUserName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtGrade;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtCellPhone;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtCreateTime;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtMicroName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtRealName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtRefStoreName;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtAddress;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtQQ;

		protected System.Web.UI.HtmlControls.HtmlGenericControl txtOpenId;

		protected System.Web.UI.HtmlControls.HtmlGenericControl ReferralOrders;

		protected System.Web.UI.HtmlControls.HtmlGenericControl OrdersTotal;

		protected System.Web.UI.HtmlControls.HtmlGenericControl TotalReferral;

		protected System.Web.UI.HtmlControls.HtmlGenericControl ReferralBlance;

		protected ucDateTimePicker calendarStartDate;

		protected ucDateTimePicker calendarEndDate;

		protected MembershipDetails() : base("m04", "hyp02")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!int.TryParse(this.Page.Request.QueryString["UserId"], out this.userid))
			{
				base.GotoResourceNotFound();
				return;
			}
			this.ListImage1.ImageUrl = "/Templates/common/images/user.png";
			MemberInfo member = MemberHelper.GetMember(this.userid);
			if (member == null)
			{
				base.GotoResourceNotFound();
				return;
			}
			if (!string.IsNullOrEmpty(member.UserHead))
			{
				this.ListImage1.ImageUrl = member.UserHead;
			}
			this.txtUserName.InnerText = member.UserName;
			MemberGradeInfo memberGrade = MemberHelper.GetMemberGrade(member.GradeId);
			if (memberGrade != null)
			{
				this.txtGrade.InnerText = memberGrade.Name;
			}
			this.txtCellPhone.InnerText = member.CellPhone;
			this.txtCreateTime.InnerText = member.CreateDate.ToString();
			this.txtMicroName.InnerText = member.UserName;
			this.txtRealName.InnerText = member.RealName;
			if (member.ReferralUserId <= 0)
			{
				this.txtRefStoreName.InnerText = "主站";
			}
			else
			{
				DistributorsInfo userIdDistributors = VShopHelper.GetUserIdDistributors(member.ReferralUserId);
				if (userIdDistributors != null)
				{
					this.txtRefStoreName.InnerText = userIdDistributors.StoreName;
				}
			}
			this.txtAddress.InnerText = RegionHelper.GetFullRegion(member.RegionId, "") + member.Address;
			this.txtQQ.InnerText = member.QQ;
			this.txtOpenId.InnerText = member.OpenId;
			this.TotalReferral.InnerText = member.AvailableAmount.ToString("F2");
			System.Collections.Generic.Dictionary<string, decimal> dataByUserId = MemberAmountProcessor.GetDataByUserId(this.userid);
			this.ReferralOrders.InnerText = dataByUserId["OrderCount"].ToString();
			this.OrdersTotal.InnerText = dataByUserId["OrderTotal"].ToString("F2");
			this.ReferralBlance.InnerText = dataByUserId["RequestAmount"].ToString("F2");
		}
	}
}
