using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Store;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.WeiXin
{
	[PrivilegeCheck(Privilege.ProductCategory)]
	public class GuideConcern : AdminPage
	{
		protected bool EnableGuidePageSet;

		protected bool IsAutoGuide;

		protected int concernradio = 1;

		protected bool isMustcheckbox;

		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		private string action = Globals.RequestFormStr("action");

		protected System.Web.UI.HtmlControls.HtmlForm thisForm;

		protected System.Web.UI.WebControls.TextBox txtConcernMsg;

		protected System.Web.UI.WebControls.TextBox txtGuidePageSet;

		protected GuideConcern() : base("m06", "wxp02")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!base.IsPostBack)
			{
				string a;
				if ((a = this.action) != null)
				{
					if (a == "MustGuideConcern")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool isMustConcern = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.IsMustConcern = isMustConcern;
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
					if (a == "EnabeGuidePage")
					{
						try
						{
							base.Response.Clear();
							base.Response.ContentType = "text/plain";
							bool flag = bool.Parse(Globals.RequestFormStr("enable"));
							if (!flag)
							{
								this.siteSettings.IsAutoGuide = false;
							}
							this.siteSettings.EnableGuidePageSet = flag;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex2)
						{
							base.Response.Write("保存失败！（" + ex2.ToString() + ")");
						}
						base.Response.End();
						return;
					}
					if (a == "EnableAutoGuide")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							bool isAutoGuide = bool.Parse(Globals.RequestFormStr("enable"));
							this.siteSettings.IsAutoGuide = isAutoGuide;
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex3)
						{
							base.Response.Write("保存失败！（" + ex3.ToString() + ")");
						}
						base.Response.End();
						return;
					}
					if (a == "ConcernType")
					{
						try
						{
							base.Response.ContentType = "text/plain";
							string concernMsg = Globals.RequestFormStr("txt1");
							string guidePageSet = Globals.RequestFormStr("txt2");
							int num = Globals.RequestFormNum("concernType");
							this.siteSettings.GuideConcernType = num;
							if (num == 0)
							{
								this.siteSettings.ConcernMsg = concernMsg;
							}
							else
							{
								this.siteSettings.GuidePageSet = guidePageSet;
							}
							SettingsManager.Save(this.siteSettings);
							base.Response.Write("保存成功");
						}
						catch (System.Exception ex4)
						{
							base.Response.Write("保存失败！（" + ex4.ToString() + ")");
						}
						base.Response.End();
						return;
					}
				}
				this.EnableGuidePageSet = this.siteSettings.EnableGuidePageSet;
				this.IsAutoGuide = this.siteSettings.IsAutoGuide;
				this.txtConcernMsg.Text = this.siteSettings.ConcernMsg;
				this.txtGuidePageSet.Text = this.siteSettings.GuidePageSet;
				this.concernradio = this.siteSettings.GuideConcernType;
				this.isMustcheckbox = this.siteSettings.IsMustConcern;
			}
		}
	}
}
