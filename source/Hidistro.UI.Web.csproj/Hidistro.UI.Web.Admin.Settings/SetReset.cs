using Hidistro.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Settings
{
	public class SetReset : System.Web.UI.Page
	{
		private Database database;

		private string allowWeb1 = "http://demo.xiaokeduo.com/";

		private string allowWeb2 = "http://demo.xkd.kuaidiantong.cn/";

		private string allowWeb3 = "http://qdshop.yun.kuaidiantong.cn/";

		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected System.Web.UI.WebControls.Literal litMsg;

		protected System.Web.UI.WebControls.Button btnReset1;

		protected System.Web.UI.WebControls.Button Button1;

		protected System.Web.UI.WebControls.Button btnReset2;

		protected System.Web.UI.WebControls.Button btnReset3;

		protected System.Web.UI.WebControls.Button btnReset4;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.database = DatabaseFactory.CreateDatabase();
			if (Globals.RequestQueryNum("istest") != 123654789)
			{
				base.Response.Write("");
				base.Response.End();
			}
			string text = base.Request.Url.ToString();
			if (!text.StartsWith(this.allowWeb1) && !text.StartsWith(this.allowWeb2) && !text.StartsWith(this.allowWeb3))
			{
				base.Response.Write("");
				base.Response.End();
			}
			this.litMsg.Text = "";
		}

		protected void btnReset_Click(object sender, System.EventArgs e)
		{
			try
			{
				SetReset.CopyFolder(base.Server.MapPath("/Storage/temp/defaultdata/"), base.Server.MapPath("/"));
				this.litMsg.Text = this.litMsg.Text + "<p>重置网站的首页模版文件以及配置文件还原成功！</p>";
			}
			catch (System.Exception ex)
			{
				this.litMsg.Text = ex.ToString();
			}
		}

		protected void btnReset2_Click(object sender, System.EventArgs e)
		{
			string query = "delete from VShop_NavMenu;SET IDENTITY_INSERT [dbo].[VShop_NavMenu] ON\r\nINSERT [dbo].[VShop_NavMenu] ( MenuId,[ParentMenuId], [Name], [Type], [DisplaySequence], [Content], [ShopMenuPic]) VALUES ( 1,0, N'店铺主页', N'click', 0, N'/Default.aspx', N'/Storage/master/ShopMenu/e6b2c1471abe42b5ae4046d8072df383.png')\r\nINSERT [dbo].[VShop_NavMenu] ( MenuId,[ParentMenuId], [Name], [Type], [DisplaySequence], [Content], [ShopMenuPic]) VALUES ( 2,0, N'购物车', N'click', 0, N'/Vshop/ShoppingCart.aspx', N'/Storage/master/ShopMenu/b6b6f69513414e03be3f601e19999616.png')\r\nINSERT [dbo].[VShop_NavMenu] ( MenuId,[ParentMenuId], [Name], [Type], [DisplaySequence], [Content], [ShopMenuPic]) VALUES ( 3,0, N'会员中心', N'click', 0, N'/Vshop/MemberCenter.aspx', N'/Storage/master/ShopMenu/29f95ea3bdf448d6a3b831ed7f07dfcb.png')\r\nINSERT [dbo].[VShop_NavMenu] ( MenuId,[ParentMenuId], [Name], [Type], [DisplaySequence], [Content], [ShopMenuPic]) VALUES ( 4,0, N'申请分销', N'click', 0, N'/Vshop/DistributorRegCheck.aspx', N'/Storage/template/20150826/6357619845391103183922574.png')\r\n SET IDENTITY_INSERT [dbo].[VShop_NavMenu] OFF";
			try
			{
				Database database = DatabaseFactory.CreateDatabase();
				System.Data.Common.DbCommand sqlStringCommand = database.GetSqlStringCommand(query);
				sqlStringCommand = database.GetSqlStringCommand(query);
				database.ExecuteNonQuery(sqlStringCommand);
				this.litMsg.Text = this.litMsg.Text + "<p>重置页面底部导航成功！</p>";
			}
			catch (System.Exception ex)
			{
				this.litMsg.Text = ex.ToString();
			}
		}

		protected void btnReset3_Click(object sender, System.EventArgs e)
		{
			string query = "\r\ndelete from [Hishop_MessageTemplates]\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (1, 21, N'TM00370', N'AccountChangeMsg', N'账户变更提醒', 0, 0, 0, 1, N'AS5UIq3zlOgzzvn8XdbcbV7bYthHAubugGee25J2jak', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (0, 6, N'TM00853', N'CouponWillExpiredMsg', N'优惠券即将到期', 0, 0, 0, 1, N'', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (1, 3, N'OPENTM207126233', N'DistributorCreateMsg', N'分销商申请成功提醒', 0, 0, 0, 1, N'JY4-ZZBd5nljm0cKZ9yvOcfWLGH9-vSPNiqzegE31po', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (1, 5, N'OPENTM207601150', N'DrawCashResultMsg', N'提现结果提醒', 0, 0, 0, 1, N'33Nt6lOW3ysND3EdcgMmKVWRtTxRROiPF0k4iB1CPAA', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (1, 1, N'OPENTM205109409', N'OrderMsg', N'订单消息提醒', 0, 0, 0, 1, N'XtGPwEA45Kyvo6QL821rkYC3LfKYz4wQcUso5vYeaGU', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (1, 22, N'OPENTM207372725', N'PersonalMsg', N'个人消息通知', 0, 0, 0, 1, N'DH4cK9SaRw0KCdQdWjKeXT-OaBFKWjSTYE24vAbuJuA', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (1, 2, N'TM00599', N'RefundSuccessMsg', N'退款通知', 0, 0, 0, 1, N'bC6sAOcH_ank_0q-8TIqvXeOXwdn06ibGhm8WQz8g4o', N'', N'', N'', N'', N'', N'')\r\nINSERT [dbo].[Hishop_MessageTemplates] ([IsValid], [OrderIndex], [WXOpenTM], [MessageType], [Name], [SendEmail], [SendSMS], [SendInnerMessage], [SendWeixin], [WeixinTemplateId], [TagDescription], [EmailSubject], [EmailBody], [InnerMessageSubject], [InnerMessageBody], [SMSBody]) VALUES (0, 4, N'OPENTM207572068', N'ServiceMsg', N'服务消息通知', 0, 0, 0, 1, N'5iDwah2e2KLtv365hdxdanJaQDko1kwaxpjFnC_aXt0', N'', N'', N'', N'', N'', N'')\r\n\r\n";
			string text = base.Request.Url.ToString();
			if (text.StartsWith(this.allowWeb2))
			{
				try
				{
					Database database = DatabaseFactory.CreateDatabase();
					System.Data.Common.DbCommand sqlStringCommand = database.GetSqlStringCommand(query);
					sqlStringCommand = database.GetSqlStringCommand(query);
					database.ExecuteNonQuery(sqlStringCommand);
					this.litMsg.Text = this.litMsg.Text + "<p>重置微信配置信息成功！</p>";
					return;
				}
				catch (System.Exception ex)
				{
					this.litMsg.Text = ex.ToString();
					return;
				}
			}
			this.litMsg.Text = this.litMsg.Text + "<p>不允许微信配置信息修改！</p>";
		}

		protected void btnReset4_Click(object sender, System.EventArgs e)
		{
			string text = base.Request.Url.ToString();
			string value = "http://demo.xiaokeduo.com/";
			string text2 = string.Empty;
			if (text.StartsWith(value))
			{
				text2 = "demo.xiaokeduo.com";
			}
			if (text.Contains("demo.xkd.kuaidiantong.cn/"))
			{
				text2 = "demo.xkd.kuaidiantong.cn";
			}
			if (text.Contains("qdshop.yun.kuaidiantong.cn/"))
			{
				text2 = "qdshop.yun.kuaidiantong.cn";
			}
			if (string.IsNullOrEmpty(text2))
			{
				this.litMsg.Text = "当前域名不允许更新产品数据";
				return;
			}
			string text3 = base.Request.MapPath("~/App_Data/" + text2 + "_product.sql");
			string text4 = string.Empty;
			if (!System.IO.File.Exists(text3))
			{
				text4 = "没有找到数据库架构文件-Schema.sql";
			}
			this.ExecuteScriptFile(text3, out text4);
			if (string.IsNullOrEmpty(text4))
			{
				this.litMsg.Text = "更新产品数据成功";
				return;
			}
			this.litMsg.Text = text4;
		}

		private void ExecuteScriptFile(string pathToScriptFile, out string errorMsg)
		{
			try
			{
				string arg_0D_0 = Globals.ApplicationPath;
				System.IO.StreamReader streamReader2;
				System.IO.StreamReader streamReader = streamReader2 = new System.IO.StreamReader(pathToScriptFile);
				try
				{
					while (!streamReader.EndOfStream)
					{
						string text = SetReset.NextSqlFromStream(streamReader);
						if (!string.IsNullOrEmpty(text))
						{
							System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand(text);
							this.database.ExecuteNonQuery(sqlStringCommand);
						}
					}
					errorMsg = "";
				}
				finally
				{
					if (streamReader2 != null)
					{
						((System.IDisposable)streamReader2).Dispose();
					}
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				errorMsg = ex.Message;
			}
		}

		private static string NextSqlFromStream(System.IO.StreamReader reader)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string text = reader.ReadLine().Trim();
			while (!reader.EndOfStream && string.Compare(text, "GO", true, System.Globalization.CultureInfo.InvariantCulture) != 0)
			{
				stringBuilder.Append(text + System.Environment.NewLine);
				text = reader.ReadLine();
			}
			if (string.Compare(text, "GO", true, System.Globalization.CultureInfo.InvariantCulture) != 0)
			{
				stringBuilder.Append(text + System.Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		private static bool TestFolder(string folderPath, out string errorMsg)
		{
			bool result;
			try
			{
				System.IO.File.WriteAllText(folderPath, "Hi");
				System.IO.File.AppendAllText(folderPath, ",This is a test file.");
				System.IO.File.Delete(folderPath);
				errorMsg = null;
				result = true;
			}
			catch (System.Exception ex)
			{
				errorMsg = ex.Message;
				result = false;
			}
			return result;
		}

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			try
			{
				Database database = DatabaseFactory.CreateDatabase();
				string query = "delete from aspnet_RolePermissions;delete from aspnet_Roles;INSERT INTO aspnet_Roles(RoleName,IsDefault) VALUES('超级管理员',1); SELECT @@IDENTITY";
				System.Data.Common.DbCommand sqlStringCommand = database.GetSqlStringCommand(query);
				int num = Globals.ToNum(database.ExecuteScalar(sqlStringCommand));
				if (num > 0)
				{
					query = string.Concat(new object[]
					{
						"delete from aspnet_Managers;INSERT INTO aspnet_Managers(RoleId, UserName, Password, Email, CreateDate) VALUES (",
						num,
						",'admin','",
						HiCryptographer.Md5Encrypt("admin888"),
						"','admin@hishop.com',getdate())"
					});
					sqlStringCommand = database.GetSqlStringCommand(query);
					database.ExecuteNonQuery(sqlStringCommand);
				}
				this.litMsg.Text = this.litMsg.Text + "<p>重置管理员密码admin888成功！</p>";
			}
			catch (System.Exception ex)
			{
				this.litMsg.Text = ex.ToString();
			}
		}

		public static void CopyFolder(string strFromPath, string strToPath)
		{
			if (!System.IO.Directory.Exists(strFromPath))
			{
				System.IO.Directory.CreateDirectory(strFromPath);
			}
			string text = strFromPath.Substring(strFromPath.LastIndexOf("\\") + 1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
			if (!System.IO.Directory.Exists(strToPath + "\\" + text))
			{
				System.IO.Directory.CreateDirectory(strToPath + "\\" + text);
			}
			string[] files = System.IO.Directory.GetFiles(strFromPath);
			for (int i = 0; i < files.Length; i++)
			{
				string text2 = files[i].Substring(files[i].LastIndexOf("\\") + 1, files[i].Length - files[i].LastIndexOf("\\") - 1);
				System.IO.File.Copy(files[i], string.Concat(new string[]
				{
					strToPath,
					"\\",
					text,
					"\\",
					text2
				}), true);
			}
			System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(strFromPath);
			System.IO.DirectoryInfo[] directories = directoryInfo.GetDirectories();
			for (int j = 0; j < directories.Length; j++)
			{
				string strFromPath2 = strFromPath + "\\" + directories[j].ToString();
				SetReset.CopyFolder(strFromPath2, strToPath + "\\" + text);
			}
		}
	}
}
