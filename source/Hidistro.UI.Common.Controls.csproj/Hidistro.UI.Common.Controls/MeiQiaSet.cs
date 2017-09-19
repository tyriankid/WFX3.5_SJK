using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Common.Controls
{
	public class MeiQiaSet : Literal
	{
		protected override void Render(HtmlTextWriter writer)
		{
			base.Text = "";
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			string text = string.Empty;
			string text2 = string.Empty;
			if (masterSettings.EnableSaleService)
			{
				if (!string.IsNullOrEmpty(masterSettings.MeiQiaEntId))
				{
					flag = true;
					string text3 = "name: '游客'";
					MemberInfo memberInfo = MemberProcessor.GetCurrentMember();
					if (memberInfo == null)
					{
						string getCurrentWXOpenId = Globals.GetCurrentWXOpenId;
						if (!string.IsNullOrEmpty(getCurrentWXOpenId))
						{
							memberInfo = MemberProcessor.GetOpenIdMember(getCurrentWXOpenId, "wx");
						}
					}
					if (memberInfo != null)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						stringBuilder2.Append("name: '" + (string.IsNullOrEmpty(memberInfo.RealName) ? memberInfo.UserName : memberInfo.RealName) + "'");
						if (!string.IsNullOrEmpty(memberInfo.Email))
						{
							stringBuilder2.Append(",email: '" + memberInfo.Email + "'");
						}
						if (!string.IsNullOrEmpty(memberInfo.Address))
						{
							stringBuilder2.Append(",address: '" + memberInfo.Address.Replace("'", "’") + "'");
						}
						if (!string.IsNullOrEmpty(memberInfo.CellPhone))
						{
							stringBuilder2.Append(",tel: '" + memberInfo.CellPhone + "'");
						}
						if (!string.IsNullOrEmpty(memberInfo.QQ))
						{
							stringBuilder2.Append(",qq: '" + memberInfo.QQ + "'");
						}
						MemberGradeInfo memberGrade = MemberProcessor.GetMemberGrade(memberInfo.GradeId);
						stringBuilder2.Append(string.Concat(new string[]
						{
							",会员帐号: '",
							memberInfo.UserBindName,
							"【",
							(memberGrade != null) ? memberGrade.Name : "普通会员",
							"】'"
						}));
						stringBuilder2.Append(",注册日期: '" + memberInfo.CreateDate.ToString("yyyy-MM-dd") + "'");
						stringBuilder2.Append(",订单量: '" + memberInfo.OrderNumber + "'");
						stringBuilder2.Append(",积分: '" + memberInfo.Points + "'");
						if (memberInfo.LastOrderDate.HasValue)
						{
							stringBuilder2.Append(",最近下单: '" + memberInfo.LastOrderDate.Value.ToString("yyyy-MM-dd") + "'");
						}
						DistributorsInfo userIdDistributors = DistributorsBrower.GetUserIdDistributors(memberInfo.UserId);
						if (userIdDistributors != null)
						{
							DistributorGradeInfo distributorGradeInfo = DistributorGradeBrower.GetDistributorGradeInfo(userIdDistributors.DistriGradeId);
							string text4 = "0.00";
							DistributorsInfo distributorInfo = DistributorsBrower.GetDistributorInfo(userIdDistributors.UserId);
							if (distributorInfo != null)
							{
								text4 = distributorInfo.ReferralBlance.ToString("F2");
							}
							string str = "0";
							string str2 = "0";
							DataTable distributorsSubStoreNum = VShopHelper.GetDistributorsSubStoreNum(userIdDistributors.UserId);
							if (distributorsSubStoreNum != null || distributorsSubStoreNum.Rows.Count > 0)
							{
								str = distributorsSubStoreNum.Rows[0]["firstV"].ToString();
								str2 = distributorsSubStoreNum.Rows[0]["secondV"].ToString();
							}
							stringBuilder2.Append(string.Concat(new string[]
							{
								",店铺名称: '",
								userIdDistributors.StoreName,
								"【",
								distributorGradeInfo.Name,
								"】'"
							}));
							stringBuilder2.Append(",销售额: '￥" + userIdDistributors.OrdersTotal.ToString("F2") + "'");
							stringBuilder2.Append(string.Concat(new string[]
							{
								",佣金信息: '￥",
								text4,
								"（待提现￥",
								userIdDistributors.ReferralBlance.ToString("F2"),
								"，已提现￥",
								userIdDistributors.ReferralRequestBalance.ToString("F2"),
								"）'"
							}));
							string str3 = "一级分店数" + str + "，二级分店数" + str2;
							stringBuilder2.Append(",comment: '" + str3 + "'");
						}
						text3 = stringBuilder2.ToString();
					}
					text = string.Concat(new string[]
					{
						"<script>function MeiQiaInit() {$('#meiqia_serviceico').show();}(function(m, ei, q, i, a, j, s) {m[a] = m[a] || function() {(m[a].a = m[a].a || []).push(arguments)};j = ei.createElement(q),s = ei.getElementsByTagName(q)[0];j.async = true;j.charset = 'UTF-8';j.src = i + '?v=' + new Date().getUTCDate();s.parentNode.insertBefore(j, s);})(window, document, 'script', '//static.meiqia.com/dist/meiqia.js', '_MEIQIA');_MEIQIA('entId', ",
						masterSettings.MeiQiaEntId,
						");_MEIQIA('withoutBtn');_MEIQIA('metadata', {",
						text3,
						"});</script><script>_MEIQIA('allSet', MeiQiaInit);</script>"
					});
					text2 = "<!-- 在线客服 -->\n<div class=\"customer-service\" id=\"meiqia_serviceico\" style=\"position:fixed;bottom:100px;right:10%;width:38px;height:38px;background:url(/Utility/pics/service.png?v1026) no-repeat;background-size:100%;cursor:pointer;display:none\" onclick=\"javascript:_MEIQIA._SHOWPANEL();\"></div>";
				}
				else
				{
					CustomerServiceSettings masterSettings2 = CustomerServiceManager.GetMasterSettings(false);
					if (!string.IsNullOrEmpty(masterSettings2.unitid) && !string.IsNullOrEmpty(masterSettings2.unit) && !string.IsNullOrEmpty(masterSettings2.password))
					{
						text = string.Format("<script src='//meiqia.com/js/mechat.js?unitid={0}&btn=hide' charset='UTF-8' async='async'></script>", masterSettings2.unitid);
						flag = true;
						stringBuilder.Append("<script type=\"text/javascript\">");
						stringBuilder.Append("function mechatFuc()");
						stringBuilder.Append("{");
						stringBuilder.Append("$.get(\"/Api/Hi_Ajax_OnlineServiceConfig.ashx\", function (data) {");
						stringBuilder.Append("if (data != \"\") {");
						stringBuilder.Append("$(data).appendTo('head');");
						stringBuilder.Append("}");
						stringBuilder.Append("mechatClick();");
						stringBuilder.Append("});");
						stringBuilder.Append("}");
						stringBuilder.Append("</script>");
						text2 = "<!-- 在线客服 -->\n<div class=\"customer-service\" style=\"position:fixed;bottom:100px;right:10%;width:38px;height:38px;background:url(/Utility/pics/service.png?v1026) no-repeat;background-size:100%;cursor:pointer;\" onclick=\"javascript:mechatFuc();\"></div>";
					}
				}
				if (flag)
				{
					base.Text = string.Concat(new string[]
					{
						text,
						"\n",
						stringBuilder.ToString(),
						"\n",
						text2
					});
				}
			}
			base.Render(writer);
		}
	}
}
