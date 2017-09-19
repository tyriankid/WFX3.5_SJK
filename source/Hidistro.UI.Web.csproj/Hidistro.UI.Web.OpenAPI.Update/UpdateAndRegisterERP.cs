using Hidistro.Core;
using Hidistro.Core.Entities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.OpenAPI.Update
{
	public class UpdateAndRegisterERP : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected System.Web.UI.WebControls.Button btnUpdateAndRegisterERP;

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		protected void btnUpdateAndRegisterERP_Click(object sender, System.EventArgs e)
		{
			string str = string.Empty;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			string text = masterSettings.AppKey;
			string text2 = masterSettings.CheckCode;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				str = "版本已更新，无需重复更新";
			}
			else
			{
				text = this.CreateAppKey();
				text2 = UpdateAndRegisterERP.CreateKey(20);
				try
				{
					this.RegisterERP(text, text2);
					masterSettings.AppKey = text;
					masterSettings.CheckCode = text2;
					SettingsManager.Save(masterSettings);
					str = "更新版本及注册成功";
				}
				catch (System.Exception ex)
				{
					str = "注册失败，错误信息：" + ex.Message;
				}
			}
			base.Response.Write("<script language='javascript'>alert('" + str + "');</script>");
		}

		private string CreateAppKey()
		{
			string text = string.Empty;
			System.Random random = new System.Random();
			for (int i = 0; i < 7; i++)
			{
				int num = random.Next();
				text += ((char)(48 + (ushort)(num % 10))).ToString();
			}
			return System.DateTime.Now.ToString("yyyyMMdd") + text;
		}

		private static string CreateKey(int len)
		{
			byte[] array = new byte[len];
			new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(array);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(string.Format("{0:X2}", array[i]));
			}
			return stringBuilder.ToString();
		}

		private void RegisterERP(string appkey, string appsecret)
		{
			string url = "http://hierp.kuaidiantong.cn/api/commercialtenantregister";
			string postResult = Globals.GetPostResult(url, string.Concat(new string[]
			{
				"appKey=",
				appkey,
				"&appSecret=",
				appsecret,
				"&routeAddress=",
				Globals.GetWebUrlStart(),
				"/OpenAPI/"
			}));
			Globals.Debuglog(postResult, "_DebuglogERP.txt");
		}
	}
}
