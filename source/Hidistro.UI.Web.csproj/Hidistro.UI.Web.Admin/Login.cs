using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	public class Login : Page
	{
		protected string htmlWebTitle = string.Empty;
		private string verifyCodeKey = "VerifyCode";
		protected HtmlForm aspnetForm;
		protected TextBox txtAdminName;
		protected TextBox txtAdminPassWord;
		protected TextBox txtCode;
		protected Button btnAdminLogin;
		protected SmallStatusMessage lblStatus;

		private string ReferralLink
		{
			get
			{
				return this.ViewState["ReferralLink"] as string;
			}
			set
			{
				this.ViewState["ReferralLink"] = value;
			}
		}

		private bool CheckVerifyCode(string verifyCode)
		{
			return base.Request.Cookies[this.verifyCodeKey] != null && string.Compare(HiCryptographer.Decrypt(base.Request.Cookies[this.verifyCodeKey].Value), verifyCode, true, CultureInfo.InvariantCulture) == 0;
		}

		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);
			this.btnAdminLogin.Click += new EventHandler(this.btnAdminLogin_Click);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bool flag = !string.IsNullOrEmpty(base.Request["isCallback"]) && base.Request["isCallback"] == "true";
			if (flag)
			{
				string verifyCode = base.Request["code"];
				string arg;
				if (!this.CheckVerifyCode(verifyCode))
				{
					arg = "0";
				}
				else
				{
					arg = "1";
				}
				base.Response.Clear();
				base.Response.ContentType = "application/json";
				base.Response.Write("{ ");
				base.Response.Write(string.Format("\"flag\":\"{0}\"", arg));
				base.Response.Write("}");
				base.Response.End();
			}
			if (!this.Page.IsPostBack)
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				this.htmlWebTitle = masterSettings.SiteName;
				Uri urlReferrer = this.Context.Request.UrlReferrer;
				if (urlReferrer != null)
				{
					this.ReferralLink = urlReferrer.ToString();
				}
				this.txtAdminName.Focus();
			}
		}

		private void btnAdminLogin_Click(object sender, EventArgs e)
		{
			if (!Globals.CheckVerifyCode(this.txtCode.Text.Trim()))
			{
				this.ShowMessage("验证码不正确");
				return;
			}
			ManagerInfo manager = ManagerHelper.GetManager(this.txtAdminName.Text);
			if (manager == null)
			{
				this.ShowMessage("无效的用户信息");
				return;
			}
			if (manager.Password != HiCryptographer.Md5Encrypt(this.txtAdminPassWord.Text))
			{
				this.ShowMessage("密码不正确");
				return;
			}
			this.WriteCookie(manager);

            if (ManagerHelper.GetCurrentManager()!=null&&ManagerHelper.GetCurrentManager().RoleId != 12) //临时修改
            {
                this.Page.Response.Redirect("../Admin/trade/manageorder.aspx?firstPage=1", true);

            }

            this.Page.Response.Redirect("Default.aspx", true);
		}

		private void WriteCookie(ManagerInfo userToLogin)
		{
			RoleInfo role = ManagerHelper.GetRole(userToLogin.RoleId);
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userToLogin.UserId.ToString(), DateTime.Now, DateTime.Now.AddDays(1.0), true, string.Format("{0}_{1}", role.RoleId, role.IsDefault));
			string value = FormsAuthentication.Encrypt(ticket);
			HttpCookie cookie = new HttpCookie(string.Format("{0}{1}", Globals.DomainName, FormsAuthentication.FormsCookieName), value);
			HttpContext.Current.Response.Cookies.Add(cookie);
		}

		private void ShowMessage(string msg)
		{
			this.lblStatus.Text = msg;
			this.lblStatus.Success = false;
			this.lblStatus.Visible = true;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			SystemAuthorizationInfo systemAuthorization = SystemAuthorizationHelper.GetSystemAuthorization(true);
			if (systemAuthorization == null)
			{
				return;
			}
			switch (systemAuthorization.state)
			{
			case SystemAuthorizationState.已过授权有效期:
				writer.Write(SystemAuthorizationHelper.noticeMsg);
				return;
			case SystemAuthorizationState.未经官方授权:
				writer.Write(SystemAuthorizationHelper.licenseMsg);
				return;
			default:
				base.Render(writer);
				return;
			}
		}
	}
}
