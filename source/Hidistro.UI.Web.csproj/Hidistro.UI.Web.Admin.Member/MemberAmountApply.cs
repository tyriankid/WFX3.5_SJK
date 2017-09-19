using ASPNET.WebControls;
using Hidistro.ControlPanel.OutPay;
using Hidistro.ControlPanel.Store;
using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.OutPay;
using Hidistro.Entities.StatisticsReport;
using Hidistro.Messages;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using Hishop.Weixin.Pay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Member
{
	public class MemberAmountApply : AdminPage
	{
		private string RequestStartTime = "";

		private string RequestEndTime = "";

		private string StoreName = "";

		public bool BatchAlipay;

		public bool BatchWeipay;

		public int DrawMinNum = 1;

		protected string DrawPayType = "";

		private bool IsGetSetting;

		private StatisticNotifier myNotifier = new StatisticNotifier();

		private UpdateStatistics myEvent = new UpdateStatistics();

		private int lastDay;

		protected System.Web.UI.WebControls.HiddenField HiddenSid;

		protected System.Web.UI.HtmlControls.HtmlInputHidden hduserid;

		protected System.Web.UI.HtmlControls.HtmlInputHidden hdreferralblance;

		protected System.Web.UI.HtmlControls.HtmlInputHidden hdredpackrecordnum;

		protected System.Web.UI.HtmlControls.HtmlInputHidden PaybatchType;

		protected System.Web.UI.WebControls.Button BatchPaySend;

		protected System.Web.UI.WebControls.Button alipaySend;

		protected System.Web.UI.WebControls.Button weipaySend;

		protected System.Web.UI.WebControls.Button WeiRedPack;

		protected System.Web.UI.WebControls.TextBox bankPayDate;

		protected System.Web.UI.WebControls.TextBox bankPayNum;

		protected System.Web.UI.WebControls.Button BankPaySave;

		protected System.Web.UI.WebControls.TextBox txtStoreName;

		protected ucDateTimePicker calendarStartDate;

		protected ucDateTimePicker calendarEndDate;

		protected System.Web.UI.WebControls.Button btnSearchButton;

		protected System.Web.UI.WebControls.Button Button1;

		protected System.Web.UI.WebControls.Button Button4;

		protected System.Web.UI.WebControls.Repeater reCommissions;

		protected Pager pager;

		protected System.Web.UI.WebControls.Button BatchPass;

		protected System.Web.UI.WebControls.TextBox RefuseMks;

		protected System.Web.UI.WebControls.Button Button2;

		protected System.Web.UI.WebControls.HiddenField hSerialID;

		protected System.Web.UI.WebControls.TextBox SignalrefuseMks;

		protected System.Web.UI.WebControls.Button Button3;

		protected System.Web.UI.WebControls.HiddenField HSid;

		protected System.Web.UI.WebControls.TextBox ReqSum;

		protected System.Web.UI.WebControls.Button PassCheck;

		protected MemberAmountApply() : base("m04", "hyp11")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.btnSearchButton.Click += new System.EventHandler(this.btnSearchButton_Click);
			this.Button3.Click += new System.EventHandler(this.Button3_Click);
			this.Button2.Click += new System.EventHandler(this.Button2_Click);
			this.BatchPass.Click += new System.EventHandler(this.BatchPass_Click);
			this.PassCheck.Click += new System.EventHandler(this.PassCheck_Click);
			this.BankPaySave.Click += new System.EventHandler(this.BankPaySave_Click);
			this.WeiRedPack.Click += new System.EventHandler(this.WeiRedPack_Click);
			this.alipaySend.Click += new System.EventHandler(this.alipaySend_Click);
			this.weipaySend.Click += new System.EventHandler(this.weipaySend_Click);
			this.BatchPaySend.Click += new System.EventHandler(this.BatchPaySend_Click);
			this.ReqSum.Attributes.Add("readonly", "true");
			this.GetPaySetting();
			this.LoadParameters();
			if (!base.IsPostBack)
			{
				this.BindData();
			}
		}

		private void GetPaySetting()
		{
			if (!this.IsGetSetting)
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				this.BatchWeipay = masterSettings.BatchWeixinPay;
				this.BatchAlipay = masterSettings.BatchAliPay;
				this.DrawPayType = masterSettings.DrawPayType;
				this.DrawMinNum = Globals.ToNum(masterSettings.MentionNowMoney);
				this.IsGetSetting = true;
			}
		}

		private void BatchPaySend_Click(object sender, System.EventArgs e)
		{
			this.GetPaySetting();
			if (string.IsNullOrEmpty(base.Request["CheckBoxGroup"]))
			{
				this.ShowMsg("参数错误！", false);
				return;
			}
			string text = base.Request["CheckBoxGroup"];
			string value = this.PaybatchType.Value;
			if (value == "0")
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				System.Collections.Generic.List<OutPayWeiInfo> list = new System.Collections.Generic.List<OutPayWeiInfo>();
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string s = array2[i];
					MemberAmountRequestInfo amountRequestDetail = MemberAmountProcessor.GetAmountRequestDetail(int.Parse(s));
					if (amountRequestDetail != null && amountRequestDetail.State == RequesState.已审核 && this.DrawMinNum <= amountRequestDetail.Amount)
					{
						list.Add(new OutPayWeiInfo
						{
							Amount = (int)amountRequestDetail.Amount * 100,
							Partner_Trade_No = amountRequestDetail.RedpackId,
							Openid = amountRequestDetail.AccountCode,
							Re_User_Name = amountRequestDetail.AccountName,
							Desc = " 用户余额发放",
							UserId = amountRequestDetail.UserId,
							Nonce_Str = OutPayHelp.GetRandomString(20),
							Sid = amountRequestDetail.Id
						});
					}
				}
				if (list.Count < 1)
				{
					this.ShowMsg("没有满足条件的提现请求！", false);
					this.LoadParameters();
					this.BindData();
					return;
				}
				System.Collections.Generic.List<WeiPayResult> list2 = OutPayHelp.BatchWeiPay(list);
				if (list2.Count < 1)
				{
					this.ShowMsg("系统异常，请联系管理员！", false);
					return;
				}
				if (list2[0].return_code == "INITFAIL")
				{
					this.ShowMsg(list2[0].return_msg, false);
					return;
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = list.Count;
				string text2 = "<div class='errRow'>支付失败信息如下：";
				using (System.Collections.Generic.List<WeiPayResult>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WeiPayResult rItem = enumerator.Current;
						int sid = list.FirstOrDefault((OutPayWeiInfo t) => t.Partner_Trade_No == rItem.partner_trade_no).Sid;
						if (rItem.result_code == "SUCCESS")
						{
							int[] serialids = new int[]
							{
								sid
							};
							MemberAmountProcessor.SetAmountRequestStatus(serialids, 2, "微信企业付款：流水号" + rItem.payment_no, "", ManagerHelper.GetCurrentManager().UserName);
							int arg_298_0 = rItem.UserId;
							MemberAmountRequestInfo amountRequestDetail2 = MemberAmountProcessor.GetAmountRequestDetail(sid);
							if (amountRequestDetail2 != null)
							{
								string url = Globals.FullPath("/Vshop/MemberAmountRequestDetail.aspx?Id=" + amountRequestDetail2.Id);
								try
								{
									Messenger.SendWeiXinMsg_MemberAmountDrawCashRelease(amountRequestDetail2, url);
								}
								catch
								{
								}
							}
							num3 += rItem.Amount / 100;
							num++;
						}
						else
						{
							if (rItem.err_code == "OPENID_ERROR" || rItem.err_code == "NAME_MISMATCH" || rItem.return_msg.Contains("openid字段") || rItem.err_code == "FATAL_ERROR")
							{
								MemberAmountProcessor.SetAmountRequestStatus(new int[]
								{
									sid
								}, 3, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + rItem.return_msg, "", ManagerHelper.GetCurrentManager().UserName);
							}
							else
							{
								MemberAmountProcessor.SetAmountRequestStatus(new int[]
								{
									sid
								}, 1, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + rItem.return_msg, "", ManagerHelper.GetCurrentManager().UserName);
							}
							string text3 = text2;
							text2 = string.Concat(new string[]
							{
								text3,
								"<br>ID：",
								rItem.partner_trade_no,
								", 出错提示：",
								rItem.return_msg
							});
							num2++;
						}
					}
				}
				num4 = num4 - num2 - num;
				text2 += "</div>";
				if (num3 == num && num3 != 0)
				{
					this.ShowMsg("全部支付成功", true);
				}
				else
				{
					this.ShowMsg(string.Concat(new object[]
					{
						"本次成功支付金额",
						num3,
						"元，其中成功",
						num,
						"笔，失败",
						num2,
						"笔，异常放弃",
						num4,
						"笔",
						text2
					}), false);
				}
				this.LoadParameters();
				this.BindData();
				return;
			}
			else
			{
				if (value == "1")
				{
					this.ShowMsg("接口暂未开通", false);
					return;
				}
				this.ShowMsg("未定义支付方式", false);
				return;
			}
		}

		private void alipaySend_Click(object sender, System.EventArgs e)
		{
			this.ShowMsg("接口暂未开通", false);
		}

		private void weipaySend_Click(object sender, System.EventArgs e)
		{
			int num = Globals.ToNum(this.HiddenSid.Value);
			decimal d = decimal.Parse(this.hdreferralblance.Value);
			MemberAmountRequestInfo amountRequestDetail = MemberAmountProcessor.GetAmountRequestDetail(num);
			if (amountRequestDetail == null)
			{
				this.ShowMsg("参数错误！", false);
				return;
			}
			if (amountRequestDetail.State == RequesState.已发放)
			{
				this.ShowMsg("该申请已经支付，请检查", false);
				return;
			}
			if (amountRequestDetail.State == RequesState.驳回)
			{
				this.ShowMsg("该申请已经驳回，请检查", false);
				return;
			}
			if (string.IsNullOrEmpty(amountRequestDetail.RedpackId))
			{
				this.ShowMsg("商户订单ID为空，请重试！", false);
				return;
			}
			WeiPayResult weiPayResult = OutPayHelp.SingleWeiPay((int)(d * 100m), "用户余额发放！", amountRequestDetail.AccountCode, amountRequestDetail.AccountName, amountRequestDetail.RedpackId, amountRequestDetail.UserId);
			if (weiPayResult.result_code == "SUCCESS")
			{
				int[] serialids = new int[]
				{
					num
				};
				MemberAmountProcessor.SetAmountRequestStatus(serialids, 2, "微信企业付款", "", ManagerHelper.GetCurrentManager().UserName);
				int arg_F0_0 = amountRequestDetail.UserId;
				string url = Globals.FullPath("/Vshop/MemberAmountRequestDetail.aspx?Id=" + amountRequestDetail.Id);
				try
				{
					Messenger.SendWeiXinMsg_MemberAmountDrawCashRelease(amountRequestDetail, url);
				}
				catch
				{
				}
				this.LoadParameters();
				this.BindData();
				this.ShowMsg("支付成功！", true);
				return;
			}
			if (weiPayResult.err_code == "OPENID_ERROR" || weiPayResult.err_code == "NAME_MISMATCH" || weiPayResult.return_msg.Contains("openid字段") || weiPayResult.err_code == "FATAL_ERROR")
			{
				MemberAmountProcessor.SetAmountRequestStatus(new int[]
				{
					num
				}, 3, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + weiPayResult.return_msg, d.ToString(), ManagerHelper.GetCurrentManager().UserName);
				this.LoadParameters();
				this.BindData();
			}
			else
			{
				MemberAmountProcessor.SetAmountRequestStatus(new int[]
				{
					num
				}, 1, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + weiPayResult.return_msg, d.ToString(), ManagerHelper.GetCurrentManager().UserName);
			}
			this.ShowMsg("微信企业付款失败，" + weiPayResult.return_msg, false);
		}

		private void WeiRedPack_Click(object sender, System.EventArgs e)
		{
			int num = Globals.ToNum(this.HiddenSid.Value);
			decimal num2 = decimal.Parse(this.hdreferralblance.Value);
			if (num2 > 200m)
			{
				this.ShowMsg("红包金额大于200，无法发放！", false);
				return;
			}
			MemberAmountRequestInfo amountRequestDetail = MemberAmountProcessor.GetAmountRequestDetail(num);
			if (amountRequestDetail == null || string.IsNullOrEmpty(amountRequestDetail.RedpackId))
			{
				this.ShowMsg("发送失败0！", false);
				return;
			}
			if (amountRequestDetail.State == RequesState.已发放)
			{
				this.ShowMsg("该申请已经支付，请检查", false);
				return;
			}
			if (amountRequestDetail.State == RequesState.驳回)
			{
				this.ShowMsg("该申请已经驳回，请检查", false);
				return;
			}
			string text = this.SendRedPack(amountRequestDetail.AccountCode, "", "恭喜您提现成功!", "您的提现申请已成功", "会员余额发红包提现", (int)num2 * 100, amountRequestDetail.RedpackId);
			if (text == "success")
			{
				MemberAmountProcessor.SetAmountRequestStatus(new int[]
				{
					num
				}, 2, "微信红包付款", "", ManagerHelper.GetCurrentManager().UserName);
				string url = Globals.FullPath("/Vshop/MemberAmountRequestDetail.aspx?Id=" + amountRequestDetail.Id);
				try
				{
					Messenger.SendWeiXinMsg_MemberAmountDrawCashRelease(amountRequestDetail, url);
				}
				catch
				{
				}
				this.ShowMsg("红包发送成功！", true);
				this.LoadParameters();
				this.BindData();
				return;
			}
			if (text.Contains("openid"))
			{
				MemberAmountProcessor.SetAmountRequestStatus(new int[]
				{
					num
				}, 3, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + text, num2.ToString(), ManagerHelper.GetCurrentManager().UserName);
				this.LoadParameters();
				this.BindData();
				return;
			}
			MemberAmountProcessor.SetAmountRequestStatus(new int[]
			{
				num
			}, 1, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + text, num2.ToString(), ManagerHelper.GetCurrentManager().UserName);
			this.ShowMsg("发送失败，原因是：" + text, false);
		}

		public string SendRedPack(string re_openid, string sub_mch_id, string wishing, string act_name, string remark, int amount, string sendredpackrecordid)
		{
			string result = string.Empty;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			if (masterSettings.EnableWeiXinRequest)
			{
				System.DateTime now = System.DateTime.Now;
				System.DateTime t = System.DateTime.Parse(now.ToString("yyyy-MM-dd") + " 00:00:01");
				System.DateTime t2 = System.DateTime.Parse(now.ToString("yyyy-MM-dd") + " 08:00:00");
				if (now > t && now < t2)
				{
					result = "北京时间0：00-8：00不触发红包赠送";
					return result;
				}
				if (string.IsNullOrEmpty(masterSettings.WeixinAppId) || string.IsNullOrEmpty(masterSettings.WeixinPartnerID) || string.IsNullOrEmpty(masterSettings.WeixinPartnerKey) || string.IsNullOrEmpty(masterSettings.WeixinCertPath) || string.IsNullOrEmpty(masterSettings.WeixinCertPassword))
				{
					result = "系统微信发红包配置接口未配置好";
					return result;
				}
				if (string.IsNullOrEmpty(re_openid))
				{
					result = "用户未绑定微信";
					return result;
				}
				string siteName = masterSettings.SiteName;
				string siteName2 = masterSettings.SiteName;
				RedPackClient redPackClient = new RedPackClient();
				try
				{
					string text = Globals.IPAddress;
					if (text.Length < 8)
					{
						text = "192.168.1.1";
					}
					result = redPackClient.SendRedpack(masterSettings.WeixinAppId, masterSettings.WeixinPartnerID, sub_mch_id, siteName, siteName2, re_openid, wishing, text, act_name, remark, amount, masterSettings.WeixinPartnerKey, masterSettings.WeixinCertPath, masterSettings.WeixinCertPassword, sendredpackrecordid, masterSettings.EnableSP, masterSettings.Main_AppId, masterSettings.Main_Mch_ID, masterSettings.Main_PayKey);
					return result;
				}
				catch (System.Exception ex)
				{
					result = ex.Message.ToString().Trim();
					return result;
				}
			}
			result = "管理员后台未开启微信付款！";
			return result;
		}

		private void BankPaySave_Click(object sender, System.EventArgs e)
		{
			int[] serialids = new int[]
			{
				Globals.ToNum(this.HiddenSid.Value)
			};
			string remark = "转账流水号：" + this.bankPayNum.Text;
			if (MemberAmountProcessor.SetAmountRequestStatus(serialids, 2, remark, "", ManagerHelper.GetCurrentManager().UserName))
			{
				MemberAmountRequestInfo amountRequestDetail = MemberAmountProcessor.GetAmountRequestDetail(Globals.ToNum(this.HiddenSid.Value));
				if (amountRequestDetail != null)
				{
					string url = Globals.FullPath("/Vshop/MemberAmountRequestDetail.aspx?Id=" + amountRequestDetail.Id);
					try
					{
						Messenger.SendWeiXinMsg_MemberAmountDrawCashRelease(amountRequestDetail, url);
					}
					catch
					{
					}
				}
				this.ShowMsg("结算成功", true);
				this.BindData();
				return;
			}
			this.ShowMsg("结算失败", false);
		}

		private void btnSearchButton_Click(object sender, System.EventArgs e)
		{
			if (this.calendarStartDate.SelectedDate.HasValue)
			{
				this.RequestStartTime = this.calendarStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");
			}
			if (this.calendarEndDate.SelectedDate.HasValue)
			{
				this.RequestEndTime = this.calendarEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");
			}
			this.lastDay = 7;
			this.ReBind(true);
		}

		private void LoadParameters()
		{
			if (!this.Page.IsPostBack)
			{
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["UserName"]))
				{
					this.StoreName = base.Server.UrlDecode(this.Page.Request.QueryString["UserName"]);
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["RequestEndTime"]))
				{
					this.RequestEndTime = base.Server.UrlDecode(this.Page.Request.QueryString["RequestEndTime"]);
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["RequestStartTime"]))
				{
					this.RequestStartTime = base.Server.UrlDecode(this.Page.Request.QueryString["RequestStartTime"]);
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["RequestStartTime"]))
				{
					this.RequestStartTime = base.Server.UrlDecode(this.Page.Request.QueryString["RequestStartTime"]);
					this.calendarStartDate.SelectedDate = new System.DateTime?(System.DateTime.Parse(this.RequestStartTime));
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["RequestEndTime"]))
				{
					this.RequestEndTime = base.Server.UrlDecode(this.Page.Request.QueryString["RequestEndTime"]);
					this.calendarEndDate.SelectedDate = new System.DateTime?(System.DateTime.Parse(this.RequestEndTime));
				}
				if (!string.IsNullOrEmpty(this.Page.Request.QueryString["lastDay"]))
				{
					int.TryParse(this.Page.Request.QueryString["lastDay"], out this.lastDay);
					if (this.lastDay == 30)
					{
						this.Button1.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
						this.Button4.BorderColor = System.Drawing.ColorTranslator.FromHtml("#1CA47D");
					}
					else if (this.lastDay == 7)
					{
						this.Button1.BorderColor = System.Drawing.ColorTranslator.FromHtml("#1CA47D");
						this.Button4.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
					}
					else
					{
						this.Button1.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
						this.Button4.BorderColor = System.Drawing.ColorTranslator.FromHtml("");
					}
				}
				this.txtStoreName.Text = this.StoreName;
				return;
			}
			this.StoreName = this.txtStoreName.Text;
			if (this.calendarStartDate.SelectedDate.HasValue)
			{
				this.RequestStartTime = this.calendarStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");
			}
			if (this.calendarEndDate.SelectedDate.HasValue)
			{
				this.RequestEndTime = this.calendarEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");
			}
		}

		protected void Button1_Click1(object sender, System.EventArgs e)
		{
			System.DateTime now = System.DateTime.Now;
			this.RequestEndTime = now.ToString("yyyy-MM-dd");
			this.RequestStartTime = now.AddDays(-6.0).ToString("yyyy-MM-dd");
			this.lastDay = 7;
			this.ReBind(true);
		}

		protected void Button4_Click1(object sender, System.EventArgs e)
		{
			System.DateTime now = System.DateTime.Now;
			this.RequestEndTime = now.ToString("yyyy-MM-dd");
			this.RequestStartTime = now.AddDays(-29.0).ToString("yyyy-MM-dd");
			this.lastDay = 30;
			this.ReBind(true);
		}

		protected void Button3_Click(object sender, System.EventArgs e)
		{
			int[] array = new int[]
			{
				Globals.ToNum(this.hSerialID.Value)
			};
			if (array[0] != 0)
			{
				int amountRequestStatus = MemberAmountProcessor.GetAmountRequestStatus(array[0]);
				if (amountRequestStatus == -1 || amountRequestStatus == 2)
				{
					this.ShowMsg("当前项数据不可以驳回，操作终止！", false);
					return;
				}
				if (MemberAmountProcessor.SetAmountRequestStatus(array, -1, this.SignalrefuseMks.Text, "", ManagerHelper.GetCurrentManager().UserName))
				{
					this.UpdateNotify("申请提现驳回");
					this.ShowMsg("申请已驳回！", true);
					this.LoadParameters();
					this.BindData();
					return;
				}
				this.ShowMsg("申请驳回失败，请再次尝试", false);
			}
		}

		protected void Button2_Click(object sender, System.EventArgs e)
		{
			if (string.IsNullOrEmpty(base.Request["CheckBoxGroup"]))
			{
				this.ShowMsg("参数错误！", false);
				return;
			}
			string text = base.Request["CheckBoxGroup"];
			string[] array = text.Split(new char[]
			{
				','
			});
			int[] array2 = System.Array.ConvertAll<string, int>(array, (string s) => Globals.ToNum(s));
			System.Collections.Generic.Dictionary<int, int> mulAmountRequestStatus = MemberAmountProcessor.GetMulAmountRequestStatus(array2);
			System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
			int[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				int num = array3[i];
				if (mulAmountRequestStatus.ContainsKey(num) && (mulAmountRequestStatus[num] == 0 || mulAmountRequestStatus[num] == 1))
				{
					arrayList.Add(num);
				}
			}
			if (arrayList.Count == 0)
			{
				this.ShowMsg("当前选择项没有数据可以驳回，操作终止！", false);
				return;
			}
			array2 = (int[])arrayList.ToArray(typeof(int));
			if (array2.Length > 0)
			{
				if (MemberAmountProcessor.SetAmountRequestStatus(array2, -1, this.RefuseMks.Text, "", ManagerHelper.GetCurrentManager().UserName))
				{
					this.UpdateNotify("申请提现批量驳回");
					this.ShowMsg("批量驳回成功！", true);
				}
				else
				{
					this.ShowMsg("批量驳回失败，请再次尝试", false);
				}
				this.LoadParameters();
				this.BindData();
			}
		}

		protected void PassCheck_Click(object sender, System.EventArgs e)
		{
			decimal num = 0m;
			int num2 = 0;
			if (int.TryParse(this.HSid.Value, out num2) && decimal.TryParse(this.ReqSum.Text, out num))
			{
				int[] serialids = new int[]
				{
					num2
				};
				if (MemberAmountProcessor.SetAmountRequestStatus(serialids, 1, "", num.ToString(), ManagerHelper.GetCurrentManager().UserName))
				{
					this.UpdateNotify("申请提现审核成功");
					this.ShowMsg("审核已通过", true);
				}
				else
				{
					this.ShowMsg("审核失败，请再次尝试", false);
				}
				this.LoadParameters();
				this.BindData();
				return;
			}
			this.ShowMsg("提现金额请填写数值", false);
		}

		protected void BatchPass_Click(object sender, System.EventArgs e)
		{
			if (string.IsNullOrEmpty(base.Request["CheckBoxGroup"]))
			{
				this.ShowMsg("参数错误！", false);
				return;
			}
			string text = base.Request["CheckBoxGroup"];
			string[] array = text.Split(new char[]
			{
				','
			});
			int[] array2 = System.Array.ConvertAll<string, int>(array, (string s) => Globals.ToNum(s));
			System.Collections.Generic.Dictionary<int, int> mulAmountRequestStatus = MemberAmountProcessor.GetMulAmountRequestStatus(array2);
			System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
			int[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				int num = array3[i];
				if (mulAmountRequestStatus.ContainsKey(num) && mulAmountRequestStatus[num] == 0)
				{
					arrayList.Add(num);
				}
			}
			if (arrayList.Count == 0)
			{
				this.ShowMsg("没有未审核状态的数据，操作终止！", false);
				return;
			}
			array2 = (int[])arrayList.ToArray(typeof(int));
			if (array2.Length > 0)
			{
				if (MemberAmountProcessor.SetAmountRequestStatus(array2, 1, "", "", ManagerHelper.GetCurrentManager().UserName))
				{
					this.UpdateNotify("申请提现批量审核成功");
					this.ShowMsg("批量审核成功！", true);
				}
				else
				{
					this.ShowMsg("批量审核失败，请再次尝试", false);
				}
				this.LoadParameters();
				this.BindData();
			}
		}

		private void ReBind(bool isSearch)
		{
			System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();
			nameValueCollection.Add("UserName", this.txtStoreName.Text);
			nameValueCollection.Add("RequestStartTime", this.RequestStartTime);
			nameValueCollection.Add("RequestEndTime", this.RequestEndTime);
			nameValueCollection.Add("lastDay", this.lastDay.ToString());
			nameValueCollection.Add("pageSize", this.pager.PageSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
			if (!isSearch)
			{
				nameValueCollection.Add("pageIndex", this.pager.PageIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
			base.ReloadPage(nameValueCollection);
		}

		private void BindData()
		{
			BalanceDrawRequestQuery balanceDrawRequestQuery = new BalanceDrawRequestQuery();
			balanceDrawRequestQuery.RequestTime = "";
			balanceDrawRequestQuery.CheckTime = "";
			balanceDrawRequestQuery.StoreName = this.StoreName;
			balanceDrawRequestQuery.PageIndex = this.pager.PageIndex;
			balanceDrawRequestQuery.PageSize = this.pager.PageSize;
			balanceDrawRequestQuery.SortOrder = SortAction.Desc;
			balanceDrawRequestQuery.SortBy = "Id";
			balanceDrawRequestQuery.RequestEndTime = this.RequestEndTime;
			balanceDrawRequestQuery.RequestStartTime = this.RequestStartTime;
			balanceDrawRequestQuery.IsCheck = "";
			balanceDrawRequestQuery.UserId = "";
			string[] extendChecks = new string[]
			{
				0.ToString(),
				1.ToString(),
				3.ToString()
			};
			Globals.EntityCoding(balanceDrawRequestQuery, true);
			DbQueryResult memberAmountRequest = MemberAmountProcessor.GetMemberAmountRequest(balanceDrawRequestQuery, extendChecks);
			this.reCommissions.DataSource = memberAmountRequest.Data;
			this.reCommissions.DataBind();
			this.pager.TotalRecords = memberAmountRequest.TotalRecords;
		}

		protected void rptList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
			{
				ImageLinkButton imageLinkButton = (ImageLinkButton)e.Item.FindControl("CheckOrGive");
				int num = Globals.ToNum(((System.Data.DataRowView)e.Item.DataItem).Row["State"].ToString());
				switch (num)
				{
				case 0:
					imageLinkButton.Text = "审核";
					return;
				case 1:
					imageLinkButton.Text = "发放";
					imageLinkButton.CssClass = "btn btn-info btn-xs";
					return;
				case 3:
					imageLinkButton.Text = "继续发放";
					imageLinkButton.CssClass = "btn btn-info btn-xs";
					return;
				}
				imageLinkButton.Text = "未知" + num;
				imageLinkButton.Enabled = false;
			}
		}

		private void UpdateNotify(string actionDesc)
		{
			try
			{
				this.myNotifier.updateAction = UpdateAction.MemberUpdate;
				this.myNotifier.actionDesc = actionDesc;
				this.myNotifier.RecDateUpdate = System.DateTime.Today;
				this.myNotifier.DataUpdated += new StatisticNotifier.DataUpdatedEventHandler(this.myEvent.Update);
				this.myNotifier.UpdateDB();
			}
			catch (System.Exception)
			{
			}
		}

		protected void Second_Click(object sender, System.EventArgs e)
		{
			base.Response.Redirect("BalanceDrawApplyErrorList.aspx");
		}
	}
}
