using Hidistro.Core;
using Hidistro.Core.Entities;
using Hishop.Weixin.MP.Api;
using Hishop.Weixin.MP.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hidistro.Messages
{
	public static class WxTemplateSendHelp
	{
		public static WxTemplateMessageResult SendTemplateMessage(string accessTocken, TempleteModel TempleteModel)
		{
			string urlFormat = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
			return WxTemplateSendHelp.SendCommonJson<WxTemplateMessageResult>(accessTocken, urlFormat, TempleteModel);
		}

		public static WxJsonResult SetIndustry()
		{
			string accessToken = WxTemplateSendHelp.GetAccessToken();
			WxJsonResult result;
			if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("errcode"))
			{
				result = new WxJsonResult
				{
					errcode = 40001,
					errmsg = "令牌获取失败"
				};
			}
			else
			{
				string urlFormat = "https://api.weixin.qq.com/cgi-bin/template/api_set_industry?access_token={0}";
				var data = new
				{
					industry_id1 = "1",
					industry_id2 = "2"
				};
				WxJsonResult wxJsonResult = WxTemplateSendHelp.SendCommonJson<WxJsonResult>(accessToken, urlFormat, data);
				result = wxJsonResult;
			}
			return result;
		}

		public static GetIndustryJsonResult GetIndustryJsonResult(string accessToken)
		{
			string urlFormat = "https://api.weixin.qq.com/cgi-bin/template/get_industry?access_token={0}";
			return WxTemplateSendHelp.SendCommonJson<GetIndustryJsonResult>(accessToken, urlFormat, null);
		}

		public static void Logwx(string msg)
		{
			Globals.Debuglog(msg, "WxTemplate.txt");
		}

		public static AddtemplateJsonResult AddtemplateJsonResult(string accessToken, string template_id_short)
		{
			string urlFormat = "https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token={0}";
			var data = new
			{
				template_id_short
			};
			return WxTemplateSendHelp.SendCommonJson<AddtemplateJsonResult>(accessToken, urlFormat, data);
		}

		public static GetPrivateTemplateJsonResult GetPrivateTemplateJsonResult(string accessToken)
		{
			string urlFormat = "https://api.weixin.qq.com/cgi-bin/template/get_all_private_template?access_token={0}";
			return WxTemplateSendHelp.SendCommonJson<GetPrivateTemplateJsonResult>(accessToken, urlFormat, null);
		}

		public static WxJsonResult DelPrivateTemplate(string template_id)
		{
			string accessToken = WxTemplateSendHelp.GetAccessToken();
			WxJsonResult result;
			if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("errcode"))
			{
				result = new WxJsonResult
				{
					errcode = 40001,
					errmsg = "Token获取失败"
				};
			}
			else
			{
				string urlFormat = "https://api.weixin.qq.com/cgi-bin/template/del_private_template?access_token={0}";
				var data = new
				{
					template_id
				};
				WxJsonResult wxJsonResult = WxTemplateSendHelp.SendCommonJson<WxJsonResult>(accessToken, urlFormat, data);
				result = wxJsonResult;
			}
			return result;
		}

		public static WxJsonResult QuickSetWeixinTemplates()
		{
			string accessToken = WxTemplateSendHelp.GetAccessToken();
			WxJsonResult result;
			if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("errcode"))
			{
				result = new WxJsonResult
				{
					errcode = 40001,
					errmsg = "Token获取失败"
				};
			}
			else
			{
				GetIndustryJsonResult industryJsonResult = WxTemplateSendHelp.GetIndustryJsonResult(accessToken);
				if (industryJsonResult.errcode != 0 && industryJsonResult.errcode != -1)
				{
					industryJsonResult.errmsg = WxTemplateSendHelp.GetErrorMsg(industryJsonResult.errcode, industryJsonResult.errmsg);
					result = industryJsonResult;
				}
				else
				{
					if (industryJsonResult.errcode == -1 || industryJsonResult.primary_industry.ConvertToIndustryCode() != IndustryCode.IT科技_互联网_电子商务 || industryJsonResult.secondary_industry.ConvertToIndustryCode() != IndustryCode.IT科技_IT软件与服务)
					{
						WxJsonResult wxJsonResult = WxTemplateSendHelp.SetIndustry();
						if (wxJsonResult.errcode != 0)
						{
							wxJsonResult.errmsg = WxTemplateSendHelp.GetErrorMsg(wxJsonResult.errcode, wxJsonResult.errmsg);
							result = wxJsonResult;
							return result;
						}
					}
					GetPrivateTemplateJsonResult privateTemplateJsonResult = WxTemplateSendHelp.GetPrivateTemplateJsonResult(accessToken);
					if (privateTemplateJsonResult.errcode != 0)
					{
						privateTemplateJsonResult.errmsg = WxTemplateSendHelp.GetErrorMsg(privateTemplateJsonResult.errcode, privateTemplateJsonResult.errmsg);
						result = privateTemplateJsonResult;
					}
					else
					{
						List<GetPrivateTemplate_TemplateItem> template_list = privateTemplateJsonResult.template_list;
						List<WxtemplateId> wxtemplateIds = WxTemplateSendHelp.GetWxtemplateIds();
						int num = template_list.Count;
						using (List<WxtemplateId>.Enumerator enumerator = wxtemplateIds.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								WxtemplateId item = enumerator.Current;
								GetPrivateTemplate_TemplateItem getPrivateTemplate_TemplateItem = template_list.FirstOrDefault((GetPrivateTemplate_TemplateItem t) => t.title == item.name && t.primary_industry == "IT科技");
								if (getPrivateTemplate_TemplateItem != null)
								{
									item.templateid = getPrivateTemplate_TemplateItem.template_id;
								}
								else if (num >= 25)
								{
									item.templateid = "公众号已有模板数量越额了！";
								}
								else
								{
									AddtemplateJsonResult addtemplateJsonResult = WxTemplateSendHelp.AddtemplateJsonResult(accessToken, item.shortId);
									if (addtemplateJsonResult.errcode != 0)
									{
										item.templateid = addtemplateJsonResult.errmsg;
									}
									else
									{
										num++;
										item.templateid = addtemplateJsonResult.template_id;
									}
								}
							}
						}
						result = new WxJsonResult
						{
							errcode = 0,
							errmsg = "设置成功",
							AppendData = wxtemplateIds
						};
					}
				}
			}
			return result;
		}

		private static List<WxtemplateId> GetWxtemplateIds()
		{
			return new List<WxtemplateId>
			{
				new WxtemplateId
				{
					name = "分销商申请成功提醒",
					shortId = "OPENTM207126233",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "账户变更提醒",
					shortId = "TM00370",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "提现结果提醒",
					shortId = "OPENTM207601150",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "订单消息提醒",
					shortId = "OPENTM205109409",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "退款通知",
					shortId = "TM00599",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "中奖结果通知",
					shortId = "OPENTM204632492",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "售后申请提醒",
					shortId = "OPENTM401701827",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "提现申请通知",
					shortId = "OPENTM401873794",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "商品详情通知",
					shortId = "OPENTM207331564",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "用户咨询提醒",
					shortId = "OPENTM202119578",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "会员注册成功提醒",
					shortId = "OPENTM207207788",
					templateid = ""
				},
				new WxtemplateId
				{
					name = "下级会员注册提示",
					shortId = "OPENTM207777500",
					templateid = ""
				}
			};
		}

		public static string GetErrorMsg(int code, string oldMsg)
		{
			string result = oldMsg;
			try
			{
				result = ((ReturnCode)code).ToString();
			}
			catch
			{
			}
			return result;
		}

		public static string GetAccessToken()
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
			return TokenApi.GetToken_Message(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret);
		}

		public static T SendCommonJson<T>(string accessToken, string urlFormat, object data = null) where T : WxJsonResult, new()
		{
			WebUtils webUtils = new WebUtils();
			string url = string.IsNullOrEmpty(accessToken) ? urlFormat : string.Format(urlFormat, accessToken.AsUrlData());
			string value = "";
			if (data != null)
			{
				value = JsonConvert.SerializeObject(data);
			}
			string returnText = webUtils.HttpSend(url, value);
			return WxTemplateSendHelp.GetResult<T>(returnText);
		}

		public static string AsUrlData(this string data)
		{
			return Uri.EscapeDataString(data);
		}

		public static T GetResult<T>(string returnText) where T : WxJsonResult, new()
		{
			T result;
			try
			{
				T t = JsonConvert.DeserializeObject<T>(returnText);
				result = t;
				return result;
			}
			catch
			{
			}
			T t2 = Activator.CreateInstance<T>();
			t2.errcode = -2;
			t2.errmsg = returnText;
			result = t2;
			return result;
		}
	}
}
