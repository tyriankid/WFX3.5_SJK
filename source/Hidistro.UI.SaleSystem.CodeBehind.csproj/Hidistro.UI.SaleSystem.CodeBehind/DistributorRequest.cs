using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class DistributorRequest : VMemberTemplatedWebControl
	{
		private System.Web.UI.HtmlControls.HtmlInputHidden litIsEnable;

		private System.Web.UI.HtmlControls.HtmlImage idImg;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VDistributorRequest.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			PageTitle.AddSiteNameTitle("申请分销");
			this.Page.Session["stylestatus"] = "2";
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(currentMember.UserId);
			if (userIdDistributors != null && userIdDistributors.ReferralStatus == 0)
			{
				this.Page.Response.Redirect("/Vshop/DistributorCenter.aspx", true);
				this.Page.Response.End();
			}
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			bool flag = VshopBrowser.IsPassAutoToDistributor(currentMember, true);
			if (flag)
			{
				DistributorsBrower.MemberAutoToDistributor(currentMember);
				this.Page.Response.Redirect("/Vshop/DistributorCenter.aspx", true);
				this.Page.Response.End();
			}
			else
			{
				if (!VshopBrowser.IsPassAutoToDistributor(currentMember, false))
				{
					this.Page.Response.Redirect("/Vshop/DistributorRegCheck.aspx", true);
					this.Page.Response.End();
				}
				if (!masterSettings.IsShowDistributorSelfStoreName)
				{
					DistributorsBrower.MemberAutoToDistributor(currentMember);
					this.Page.Response.Redirect("/Vshop/DistributorCenter.aspx", true);
					this.Page.Response.End();
				}
				else
				{
					int num = 0;
					this.idImg = (System.Web.UI.HtmlControls.HtmlImage)this.FindControl("idImg");
					string text = string.Empty;
					if (int.TryParse(this.Page.Request.QueryString["ReferralId"], out num))
					{
						if (num > 0)
						{
							DistributorsInfo userIdDistributors2 = DistributorsBrower.GetUserIdDistributors(num);
							if (userIdDistributors2 != null)
							{
								if (!string.IsNullOrEmpty(userIdDistributors2.Logo))
								{
									text = userIdDistributors2.Logo;
								}
							}
						}
					}
					if (string.IsNullOrEmpty(text))
					{
						text = masterSettings.DistributorLogoPic;
					}
					this.idImg.Src = text;
					if (userIdDistributors != null && userIdDistributors.ReferralStatus != 0)
					{
						this.litIsEnable = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("litIsEnable");
						this.litIsEnable.Value = userIdDistributors.ReferralStatus.ToString();
					}
				}
			}
		}
	}
}
