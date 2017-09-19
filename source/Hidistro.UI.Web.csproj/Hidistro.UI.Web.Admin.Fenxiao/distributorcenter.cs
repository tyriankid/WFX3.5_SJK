using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.ControlPanel.Utility;
using System;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
	public class distributorcenter : AdminPage
	{
		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		protected string DistributorCenterName;

		protected string CommissionName;

		protected string DistributionTeamName;

		protected string MyShopName;

		protected string FirstShopName;

		protected string SecondShopName;

		protected string MyCommissionName;

		protected string DistributionDescriptionName;

		protected distributorcenter() : base("m05", "fxp14")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!base.IsPostBack)
			{
				string text = Globals.RequestFormStr("action");
				string a;
				if ((a = text) != null)
				{
					if (!(a == "Save"))
					{
						if (!(a == "Huifu"))
						{
							goto IL_13F;
						}
					}
					try
					{
						base.Response.ContentType = "text/plain";
						this.siteSettings.DistributorCenterName = Globals.RequestFormStr("fx0");
						this.siteSettings.CommissionName = Globals.RequestFormStr("fx1");
						this.siteSettings.DistributionTeamName = Globals.RequestFormStr("fx2");
						this.siteSettings.MyShopName = Globals.RequestFormStr("fx3");
						this.siteSettings.FirstShopName = Globals.RequestFormStr("fx4");
						this.siteSettings.SecondShopName = Globals.RequestFormStr("fx5");
						this.siteSettings.MyCommissionName = Globals.RequestFormStr("fx6");
						this.siteSettings.DistributionDescriptionName = Globals.RequestFormStr("fx7");
						SettingsManager.Save(this.siteSettings);
						base.Response.Write("保存成功");
					}
					catch (System.Exception ex)
					{
						base.Response.Write("保存失败！（" + ex.ToString() + ")");
					}
					base.Response.End();
					return;
				}
				IL_13F:
				this.DistributorCenterName = this.siteSettings.DistributorCenterName;
				this.CommissionName = this.siteSettings.CommissionName;
				this.DistributionTeamName = this.siteSettings.DistributionTeamName;
				this.MyShopName = this.siteSettings.MyShopName;
				this.FirstShopName = this.siteSettings.FirstShopName;
				this.SecondShopName = this.siteSettings.SecondShopName;
				this.MyCommissionName = this.siteSettings.MyCommissionName;
				this.DistributionDescriptionName = this.siteSettings.DistributionDescriptionName;
			}
		}
	}
}
