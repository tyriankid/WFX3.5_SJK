using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Sales;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Sales;
using Hidistro.Messages;
using Hidistro.SaleSystem.Vshop;
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Hidistro.UI.Web.API
{
	public class VshopRecharge : System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			string text = context.Request["action"];
			string key;
			switch (key = text)
			{
			case "SubmmitAmount":
				this.ProcessSubmmitAmount(context);
				return;
			case "GetAmountList":
				this.ProcessGetAmountList(context);
				return;
			case "GetBalanceWithdrawList":
				this.GetBalanceWithdrawList(context);
				return;
			case "CommissionToAmount":
				this.ProcessCommissionToAmount(context);
				return;
			case "AddAmountApply":
				this.ProcessAddAmountApply(context);
				return;
			case "GetMemberAmountDetails":
				this.ProcessGetMemberAmountDetails(context);
				return;
			case "SetUserAmountByAdmin":
				this.ProcessUserAmountByAdmin(context);
				break;

				return;
			}
		}

		private void ProcessSubmmitAmount(System.Web.HttpContext context)
		{
			context.Response.ContentType = "application/json";
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("{");
			if (currentMember == null)
			{
				stringBuilder.Append("\"Status\":\"Eror\",\"ErrorMsg\":\"请先登录！\"");
				stringBuilder.Append("}");
				context.Response.ContentType = "application/json";
				context.Response.Write(stringBuilder.ToString());
				return;
			}
			int num = int.Parse(context.Request["paymentType"]);
			decimal num2 = decimal.Parse(context.Request["Amount"]);
			string generateId = Globals.GetGenerateId();
			if (num2 > 1000000m)
			{
				stringBuilder.Append("\"Status\":\"Eror\",\"ErrorMsg\":\"最大充值金额不大于100万！\"");
				stringBuilder.Append("}");
				context.Response.ContentType = "application/json";
				context.Response.Write(stringBuilder.ToString());
				return;
			}
			MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo();
			memberAmountDetailedInfo.UserId = currentMember.UserId;
			memberAmountDetailedInfo.UserName = currentMember.UserName;
			memberAmountDetailedInfo.PayId = generateId;
			memberAmountDetailedInfo.TradeAmount = num2;
			memberAmountDetailedInfo.TradeType = TradeType.Recharge;
			memberAmountDetailedInfo.TradeTime = System.DateTime.Now;
			memberAmountDetailedInfo.State = 0;
			memberAmountDetailedInfo.AvailableAmount = currentMember.AvailableAmount + num2;
			memberAmountDetailedInfo.Remark = "余额充值";
			if (num == 88)
			{
				memberAmountDetailedInfo.TradeWays = TradeWays.WeChatWallet;
			}
			else
			{
				PaymentModeInfo paymentMode = ShoppingProcessor.GetPaymentMode(num);
				if (paymentMode != null)
				{
					if (paymentMode.Gateway == "hishop.plugins.payment.ws_wappay.wswappayrequest")
					{
						memberAmountDetailedInfo.TradeWays = TradeWays.Alipay;
					}
					else if (paymentMode.Gateway == "Hishop.Plugins.Payment.ShengPayMobile.ShengPayMobileRequest")
					{
						memberAmountDetailedInfo.TradeWays = TradeWays.ShengFutong;
					}
				}
			}
			if (MemberAmountProcessor.CreatAmount(memberAmountDetailedInfo))
			{
				stringBuilder.Append("\"Status\":\"OK\",\"PayIdStatus\":\"" + memberAmountDetailedInfo.PayId + "\",");
				stringBuilder.AppendFormat("\"PayId\":\"{0}\"", memberAmountDetailedInfo.PayId);
			}
			else
			{
				stringBuilder.Append("\"Status\":\"Error\"");
				stringBuilder.AppendFormat(",\"ErrorMsg\":\"提交充值失败！\"", new object[0]);
			}
			stringBuilder.Append("}");
			context.Response.ContentType = "application/json";
			context.Response.Write(stringBuilder.ToString());
		}

		private void GetBalanceWithdrawList(System.Web.HttpContext context)
		{
			context.Response.ContentType = "application/json";
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			string s = "{\"success\":\"false\"}";
			int type = Globals.RequestFormNum("type");
			int num = Globals.RequestFormNum("page");
			int num2 = Globals.RequestFormNum("pagesize");
			if (num2 < 5)
			{
				num2 = 10;
			}
			if (num < 1)
			{
				num = 1;
			}
			DbQueryResult balanceWithdrawListRequest = MemberAmountProcessor.GetBalanceWithdrawListRequest(type, num, num2, currentMember.UserId);
			object data = balanceWithdrawListRequest.Data;
			if (data != null)
			{
				System.Data.DataTable dataTable = (System.Data.DataTable)data;
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				int count = dataTable.Rows.Count;
				string arg_91_0 = string.Empty;
				string arg_97_0 = string.Empty;
				if (count > 0)
				{
					dataTable.Rows[0]["State"].ToString();
					int i = 0;
					stringBuilder.Append(string.Concat(new string[]
					{
						"{\"State\":",
						dataTable.Rows[i]["State"].ToString(),
						",\"id\":",
						dataTable.Rows[i]["ID"].ToString(),
						",\"Amount\":\"",
						Globals.String2Json(System.Math.Round((decimal)dataTable.Rows[i]["Amount"], 2).ToString()),
						"\",\"RequestTime\":\"",
						System.DateTime.Parse(dataTable.Rows[i]["RequestTime"].ToString()).ToString("yyyy-MM-dd"),
						"\",\"RequestType\":\"",
						VShopHelper.GetCommissionPayType(dataTable.Rows[i]["RequestType"].ToString()),
						"\"}"
					}));
					for (i = 1; i < count; i++)
					{
						stringBuilder.Append(string.Concat(new string[]
						{
							",{\"State\":",
							dataTable.Rows[i]["State"].ToString(),
							",\"id\":",
							dataTable.Rows[i]["ID"].ToString(),
							",\"Amount\":\"",
							Globals.String2Json(System.Math.Round((decimal)dataTable.Rows[i]["Amount"], 2).ToString()),
							"\",\"RequestTime\":\"",
							System.DateTime.Parse(dataTable.Rows[i]["RequestTime"].ToString()).ToString("yyyy-MM-dd"),
							"\",\"RequestType\":\"",
							VShopHelper.GetCommissionPayType(dataTable.Rows[i]["RequestType"].ToString()),
							"\"}"
						}));
					}
				}
				s = string.Concat(new object[]
				{
					"{\"success\":\"true\",\"rowtotal\":\"",
					dataTable.Rows.Count,
					"\",\"total\":\"",
					balanceWithdrawListRequest.TotalRecords,
					"\",\"lihtml\":[",
					stringBuilder.ToString(),
					"]}"
				});
			}
			context.Response.Write(s);
			context.Response.End();
		}

		private void ProcessGetAmountList(System.Web.HttpContext context)
		{
			context.Response.ContentType = "application/json";
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			string s = "{\"success\":\"false\"}";
			int type = Globals.RequestFormNum("type");
			int num = Globals.RequestFormNum("page");
			int num2 = Globals.RequestFormNum("pagesize");
			if (num2 < 5)
			{
				num2 = 10;
			}
			if (num < 1)
			{
				num = 1;
			}
			DbQueryResult amountListRequest = MemberAmountProcessor.GetAmountListRequest(type, num, num2, currentMember.UserId);
			object data = amountListRequest.Data;
			if (data != null)
			{
				System.Data.DataTable dataTable = (System.Data.DataTable)data;
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				int count = dataTable.Rows.Count;
				string text = string.Empty;
				if (count > 0)
				{
					decimal d = decimal.Parse(dataTable.Rows[0]["TradeAmount"].ToString());
					text = System.Math.Round(d, 2).ToString();
					int i = 0;
					stringBuilder.Append(string.Concat(new string[]
					{
						"{\"id\":",
						dataTable.Rows[i]["ID"].ToString(),
						",\"AvailableAmount\":\"",
						Globals.String2Json(System.Math.Round((decimal)dataTable.Rows[i]["AvailableAmount"], 2).ToString()),
						"\",\"TradeTime\":\"",
						System.DateTime.Parse(dataTable.Rows[i]["TradeTime"].ToString()).ToString("yyyy-MM-dd"),
						"\",\"TradeAmount\":\"",
						text,
						"\",\"TradeType\":\"",
						MemberHelper.StringToTradeType(dataTable.Rows[i]["TradeType"].ToString()),
						"\"}"
					}));
					for (i = 1; i < count; i++)
					{
						d = decimal.Parse(dataTable.Rows[i]["TradeAmount"].ToString());
						text = System.Math.Round(d, 2).ToString();
						stringBuilder.Append(string.Concat(new string[]
						{
							",{\"id\":",
							dataTable.Rows[i]["ID"].ToString(),
							",\"AvailableAmount\":\"",
							Globals.String2Json(System.Math.Round((decimal)dataTable.Rows[i]["AvailableAmount"], 2).ToString()),
							"\",\"TradeTime\":\"",
							System.DateTime.Parse(dataTable.Rows[i]["TradeTime"].ToString()).ToString("yyyy-MM-dd"),
							"\",\"TradeAmount\":\"",
							text,
							"\",\"TradeType\":\"",
							MemberHelper.StringToTradeType(dataTable.Rows[i]["TradeType"].ToString()),
							"\"}"
						}));
					}
				}
				s = string.Concat(new object[]
				{
					"{\"success\":\"true\",\"rowtotal\":\"",
					dataTable.Rows.Count,
					"\",\"total\":\"",
					amountListRequest.TotalRecords,
					"\",\"lihtml\":[",
					stringBuilder.ToString(),
					"]}"
				});
			}
			context.Response.Write(s);
			context.Response.End();
		}

		private void ProcessCommissionToAmount(System.Web.HttpContext context)
		{
			context.Response.ContentType = "application/json";
			DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(Globals.GetCurrentMemberUserId(false));
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("{");
			if (userIdDistributors == null)
			{
				stringBuilder.Append("\"Status\":\"Eror\",\"ErrorMsg\":\"您不是分销商！\"");
				stringBuilder.Append("}");
				context.Response.ContentType = "application/json";
				context.Response.Write(stringBuilder.ToString());
				return;
			}
			decimal d = DistributorsBrower.CommionsRequestSumMoney(userIdDistributors.UserId);
			decimal num = decimal.Parse(context.Request["Amount"]);
			if (num < 0.01m || num > userIdDistributors.ReferralBlance - d)
			{
				string str = "您输入正确的金额";
				if (num - 0.01m < userIdDistributors.ReferralBlance - d)
				{
					str = "最多可提现金额为：" + (num - 0.01m).ToString("F2") + "元";
				}
				stringBuilder.Append("\"Status\":\"Eror\",\"ErrorMsg\":\"" + str + "！\"");
				stringBuilder.Append("}");
				context.Response.ContentType = "application/json";
				context.Response.Write(stringBuilder.ToString());
				return;
			}
			if (MemberAmountProcessor.CommissionToAmount(new MemberAmountDetailedInfo
			{
				UserId = currentMember.UserId,
				UserName = currentMember.UserName,
				PayId = Globals.GetGenerateId(),
				TradeAmount = num,
				TradeType = TradeType.CommissionTransfer,
				TradeTime = System.DateTime.Now,
				State = 1,
				AvailableAmount = currentMember.AvailableAmount + num,
				TradeWays = TradeWays.ShopCommission,
				Remark = "佣金转入余额"
			}, userIdDistributors.UserId, num))
			{
				stringBuilder.Append("\"Status\":\"OK\"");
			}
			else
			{
				stringBuilder.Append("\"Status\":\"Error\"");
				stringBuilder.AppendFormat(",\"ErrorMsg\":\"佣金转余额失败！\"", new object[0]);
			}
			stringBuilder.Append("}");
			context.Response.ContentType = "application/json";
			context.Response.Write(stringBuilder.ToString());
		}

		private void ProcessAddAmountApply(System.Web.HttpContext context)
		{
			context.Response.ContentType = "text/json";
			string s = "";
			if (this.CheckAddAmountApply(context, ref s))
			{
				string accountCode = context.Request["account"].Trim();
				decimal amount = decimal.Parse(context.Request["applymoney"].Trim());
				int num = 0;
				int.TryParse(context.Request["requesttype"].Trim(), out num);
				string remark = context.Request["remark"].Trim();
				string text = context.Request["realname"].Trim();
				string bankName = context.Request["bankname"].Trim();
				MemberAmountRequestInfo memberAmountRequestInfo = new MemberAmountRequestInfo();
				MemberInfo currentMember = MemberProcessor.GetCurrentMember();
				memberAmountRequestInfo.UserId = currentMember.UserId;
				memberAmountRequestInfo.UserName = currentMember.UserName;
				memberAmountRequestInfo.RequestTime = System.DateTime.Now;
				memberAmountRequestInfo.Amount = amount;
				memberAmountRequestInfo.RequestType = (RequesType)num;
				memberAmountRequestInfo.AccountCode = accountCode;
				if (num == 3 || num == 0)
				{
					memberAmountRequestInfo.AccountCode = currentMember.OpenId;
				}
				string text2 = string.IsNullOrEmpty(text) ? currentMember.RealName : text;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = currentMember.UserName;
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "未设置";
				}
				memberAmountRequestInfo.AccountName = text2;
				memberAmountRequestInfo.BankName = bankName;
				memberAmountRequestInfo.Remark = remark;
				memberAmountRequestInfo.State = RequesState.未审核;
				memberAmountRequestInfo.CellPhone = currentMember.CellPhone;
				if ((string.IsNullOrEmpty(currentMember.OpenId) || currentMember.OpenId.Length < 28) && (num == 3 || num == 0))
				{
					s = "{\"success\":false,\"msg\":\"您的帐号未绑定，无法通过微信支付余额！\"}";
				}
				else if (MemberAmountProcessor.CreatAmountApplyRequest(memberAmountRequestInfo))
				{
					try
					{
						MemberAmountRequestInfo memberAmountRequestInfo2 = memberAmountRequestInfo;
						if (memberAmountRequestInfo2 != null)
						{
							Messenger.SendWeiXinMsg_MemberAmountDrawCashRequest(memberAmountRequestInfo2);
						}
					}
					catch (System.Exception)
					{
					}
					s = "{\"success\":true,\"msg\":\"申请成功！\"}";
				}
				else
				{
					s = "{\"success\":false,\"msg\":\"申请失败！\"}";
				}
			}
			context.Response.Write(s);
			context.Response.End();
		}

		private bool CheckAddAmountApply(System.Web.HttpContext context, ref string msg)
		{
			int num = 0;
			if (!int.TryParse(context.Request["requesttype"], out num))
			{
				num = 1;
			}
			string text = context.Request["bankname"].Trim();
			string text2 = context.Request["account"];
			if (num == 1 && !Globals.CheckReg(text2, "^1\\d{10}$") && !Globals.CheckReg(text2, "^(\\w-*\\.*)+@(\\w-?)+(\\.\\w{2,})+$"))
			{
				msg = "{\"success\":false,\"msg\":\"支付宝账号格式不正确！\"}";
				return false;
			}
			if (num == 2 && text2.Length < 4)
			{
				msg = "{\"success\":false,\"msg\":\"收款帐号不能为空，请准确填写！\"}";
				return false;
			}
			if (num == 2 && text.Length < 2)
			{
				msg = "{\"success\":false,\"msg\":\"帐号说明不能为空！\"}";
				return false;
			}
			if (string.IsNullOrEmpty(context.Request["applymoney"].Trim()))
			{
				msg = "{\"success\":false,\"msg\":\"提现金额不允许为空！\"}";
				return false;
			}
			if (decimal.Parse(context.Request["applymoney"].Trim()) <= 0m)
			{
				msg = "{\"success\":false,\"msg\":\"提现金额必须大于0！\"}";
				return false;
			}
			decimal num2 = 0m;
			decimal.TryParse(SettingsManager.GetMasterSettings(false).MentionNowMoney, out num2);
			if (num2 > 0m && decimal.Parse(context.Request["applymoney"].Trim()) < num2)
			{
				msg = "{\"success\":false,\"msg\":\"提现金额必须大于等于" + num2.ToString() + "元！\"}";
				return false;
			}
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			if (decimal.Parse(context.Request["applymoney"].Trim()) > currentMember.AvailableAmount)
			{
				msg = "{\"success\":false,\"msg\":\"提现金额必须为小于现有余额！\"}";
				return false;
			}
			return true;
		}

		private void ProcessGetMemberAmountDetails(System.Web.HttpContext context)
		{
			context.Response.ContentType = "application/json";
			int userId = Globals.RequestFormNum("userid");
			MemberInfo member = MemberHelper.GetMember(userId);
			string s = "{\"success\":\"false\"}";
			int num = Globals.RequestFormNum("type");
			int num2 = Globals.RequestFormNum("page");
			int num3 = Globals.RequestFormNum("pagesize");
			string text = context.Request["startTime"].ToString();
			string text2 = context.Request["endTime"].ToString();
			if (num3 < 5)
			{
				num3 = 5;
			}
			if (num2 < 1)
			{
				num2 = 1;
			}
			if (num == 0)
			{
				MemberDetailOrderQuery memberDetailOrderQuery = new MemberDetailOrderQuery();
				memberDetailOrderQuery.UserId = new int?(member.UserId);
				OrderStatus[] status = new OrderStatus[]
				{
					OrderStatus.Finished,
					OrderStatus.BuyerAlreadyPaid,
					OrderStatus.SellerAlreadySent
				};
				memberDetailOrderQuery.Status = status;
				if (!string.IsNullOrEmpty(text))
				{
					memberDetailOrderQuery.StartFinishDate = new System.DateTime?(System.Convert.ToDateTime(text));
				}
				if (!string.IsNullOrEmpty(text2))
				{
					memberDetailOrderQuery.EndFinishDate = new System.DateTime?(System.Convert.ToDateTime(text2));
				}
				memberDetailOrderQuery.PageIndex = num2;
				memberDetailOrderQuery.PageSize = num3;
				memberDetailOrderQuery.SortBy = "OrderDate";
				memberDetailOrderQuery.SortOrder = SortAction.Desc;
				DbQueryResult memberDetailOrders = OrderHelper.GetMemberDetailOrders(memberDetailOrderQuery);
				object data = memberDetailOrders.Data;
				if (data != null)
				{
					System.Data.DataTable dataTable = (System.Data.DataTable)data;
					System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
					int count = dataTable.Rows.Count;
					if (count > 0)
					{
						int i = 0;
						string text3 = (!dataTable.Rows[i].IsNull(dataTable.Columns["GatewayOrderId"])) ? dataTable.Rows[i]["GatewayOrderId"].ToString() : "-";
						string text4 = (dataTable.Rows[i]["FinishDate"] != System.DBNull.Value) ? System.DateTime.Parse(dataTable.Rows[i]["FinishDate"].ToString()).ToString("yyyy-MM-dd") : "-";
						stringBuilder.Append(string.Concat(new string[]
						{
							"{\"GatewayOrderId\":\"",
							text3,
							"\",\"OrderTotal\":\"",
							Globals.String2Json(System.Math.Round((decimal)dataTable.Rows[i]["OrderTotal"], 2).ToString()),
							"\",\"OrderDate\":\"",
							text4,
							"\",\"PaymentType\":\"",
							dataTable.Rows[i]["PaymentType"].ToString(),
							"\",\"OrderId\":\"",
							dataTable.Rows[i]["OrderId"].ToString(),
							"\",\"ShipTo\":\"",
							dataTable.Rows[i]["ShipTo"].ToString(),
							"\",\"Remark\":\"",
							dataTable.Rows[i]["Remark"].ToString(),
							"\"}"
						}));
						for (i = 1; i < count; i++)
						{
							text3 = ((!dataTable.Rows[i].IsNull(dataTable.Columns["GatewayOrderId"])) ? dataTable.Rows[i]["GatewayOrderId"].ToString() : "-");
							text4 = ((dataTable.Rows[i]["FinishDate"] != System.DBNull.Value) ? System.DateTime.Parse(dataTable.Rows[i]["FinishDate"].ToString()).ToString("yyyy-MM-dd") : "-");
							stringBuilder.Append(string.Concat(new string[]
							{
								",{\"GatewayOrderId\":\"",
								text3,
								"\",\"OrderTotal\":\"",
								Globals.String2Json(System.Math.Round((decimal)dataTable.Rows[i]["OrderTotal"], 2).ToString()),
								"\",\"OrderDate\":\"",
								text4,
								"\",\"PaymentType\":\"",
								dataTable.Rows[i]["PaymentType"].ToString(),
								"\",\"OrderId\":\"",
								dataTable.Rows[i]["OrderId"].ToString(),
								"\",\"ShipTo\":\"",
								dataTable.Rows[i]["ShipTo"].ToString(),
								"\",\"Remark\":\"",
								dataTable.Rows[i]["Remark"].ToString(),
								"\"}"
							}));
						}
					}
					s = string.Concat(new object[]
					{
						"{\"success\":\"true\",\"rowtotal\":\"",
						dataTable.Rows.Count,
						"\",\"total\":\"",
						memberDetailOrders.TotalRecords,
						"\",\"lihtml\":[",
						stringBuilder.ToString(),
						"]}"
					});
				}
			}
			if (num == 1)
			{
				DbQueryResult amountListRequestByTime = MemberAmountProcessor.GetAmountListRequestByTime(0, num2, num3, member.UserId, text, text2);
				object data2 = amountListRequestByTime.Data;
				if (data2 != null)
				{
					System.Data.DataTable dataTable2 = (System.Data.DataTable)data2;
					System.Text.StringBuilder stringBuilder2 = new System.Text.StringBuilder();
					int count2 = dataTable2.Rows.Count;
					string text5 = string.Empty;
					if (count2 > 0)
					{
						int j = 0;
						decimal d = decimal.Parse(dataTable2.Rows[j]["TradeAmount"].ToString());
						text5 = System.Math.Round(d, 2).ToString();
						stringBuilder2.Append(string.Concat(new string[]
						{
							"{\"id\":",
							dataTable2.Rows[j]["ID"].ToString(),
							",\"AvailableAmount\":\"",
							Globals.String2Json(System.Math.Round((decimal)dataTable2.Rows[j]["AvailableAmount"], 2).ToString()),
							"\",\"TradeTime\":\"",
							System.DateTime.Parse(dataTable2.Rows[j]["TradeTime"].ToString()).ToString("yyyy-MM-dd"),
							"\",\"TradeAmount\":\"",
							text5,
							"\",\"TradeType\":\"",
							MemberHelper.StringToTradeType(dataTable2.Rows[j]["TradeType"].ToString()),
							"\",\"PayId\":\"",
							dataTable2.Rows[j]["PayId"].ToString(),
							"\",\"TradeWays\":\"",
							MemberHelper.StringToTradeWays(dataTable2.Rows[j]["TradeWays"].ToString()),
							"\",\"Remark\":\"",
							dataTable2.Rows[j]["Remark"].ToString(),
							"\"}"
						}));
						for (j = 1; j < count2; j++)
						{
							d = decimal.Parse(dataTable2.Rows[j]["TradeAmount"].ToString());
							text5 = System.Math.Round(d, 2).ToString();
							stringBuilder2.Append(string.Concat(new string[]
							{
								",{\"id\":",
								dataTable2.Rows[j]["ID"].ToString(),
								",\"AvailableAmount\":\"",
								Globals.String2Json(System.Math.Round((decimal)dataTable2.Rows[j]["AvailableAmount"], 2).ToString()),
								"\",\"TradeTime\":\"",
								System.DateTime.Parse(dataTable2.Rows[j]["TradeTime"].ToString()).ToString("yyyy-MM-dd"),
								"\",\"TradeAmount\":\"",
								text5,
								"\",\"TradeType\":\"",
								MemberHelper.StringToTradeType(dataTable2.Rows[j]["TradeType"].ToString()),
								"\",\"PayId\":\"",
								dataTable2.Rows[j]["PayId"].ToString(),
								"\",\"TradeWays\":\"",
								MemberHelper.StringToTradeWays(dataTable2.Rows[j]["TradeWays"].ToString()),
								"\",\"Remark\":\"",
								dataTable2.Rows[j]["Remark"].ToString(),
								"\"}"
							}));
						}
					}
					s = string.Concat(new object[]
					{
						"{\"success\":\"true\",\"rowtotal\":\"",
						dataTable2.Rows.Count,
						"\",\"total\":\"",
						amountListRequestByTime.TotalRecords,
						"\",\"lihtml\":[",
						stringBuilder2.ToString(),
						"]}"
					});
				}
			}
			context.Response.Write(s);
			context.Response.End();
		}

		private void ProcessUserAmountByAdmin(System.Web.HttpContext context)
		{
			context.Response.ContentType = "application/json";
			int userId = Globals.RequestFormNum("userid");
			MemberInfo member = MemberHelper.GetMember(userId);
			string s = "{\"success\":\"false\"}";
			decimal num = decimal.Parse(context.Request["setAmount"]);
			string remark = context.Request["remark"];
			MemberAmountDetailedInfo amountByShopAdjustment = new MemberAmountDetailedInfo
			{
				UserId = userId,
				UserName = member.UserName,
				PayId = Globals.GetGenerateId(),
				TradeAmount = num,
				TradeType = TradeType.ShopAdjustment,
				TradeTime = System.DateTime.Now,
				State = 1,
				TradeWays = TradeWays.Balance,
				AvailableAmount = member.AvailableAmount + num,
				Remark = remark
			};
			if (MemberAmountProcessor.SetAmountByShopAdjustment(amountByShopAdjustment))
			{
				s = "{\"success\":\"true\"}";
			}
			context.Response.Write(s);
			context.Response.End();
		}
	}
}
