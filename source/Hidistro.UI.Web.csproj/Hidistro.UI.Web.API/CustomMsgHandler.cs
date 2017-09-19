using ControlPanel.WeiBo;
using ControlPanel.WeiXin;
using Hidistro.ControlPanel.Store;
using Hidistro.ControlPanel.VShop;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.VShop;
using Hidistro.Entities.Weibo;
using Hidistro.SaleSystem.Vshop;
using Hidistro.UI.SaleSystem.CodeBehind;
using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Api;
using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Handler;
using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Request.Event;
using Hishop.Weixin.MP.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Hidistro.UI.Web.API
{
	public class CustomMsgHandler : RequestHandler
	{
		public CustomMsgHandler(System.IO.Stream inputStream) : base(inputStream)
		{
		}

		public bool IsOpenManyService()
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
			return masterSettings.OpenManyService;
		}

		public AbstractResponse GotoManyCustomerService(AbstractRequest requestMessage)
		{
			WeiXinHelper.UpdateRencentOpenID(requestMessage.FromUserName);
			if (!this.IsOpenManyService())
			{
				return null;
			}
			return new AbstractResponse
			{
				FromUserName = requestMessage.ToUserName,
				ToUserName = requestMessage.FromUserName,
				MsgType = ResponseMsgType.transfer_customer_service
			};
		}

		public override AbstractResponse DefaultResponse(AbstractRequest requestMessage)
		{
			WeiXinHelper.UpdateRencentOpenID(requestMessage.FromUserName);
			Hidistro.Entities.VShop.ReplyInfo mismatchReply = ReplyHelper.GetMismatchReply();
			if (mismatchReply == null || this.IsOpenManyService())
			{
				return this.GotoManyCustomerService(requestMessage);
			}
			AbstractResponse response = this.GetResponse(mismatchReply, requestMessage.FromUserName);
			if (response == null)
			{
				return this.GotoManyCustomerService(requestMessage);
			}
			response.ToUserName = requestMessage.FromUserName;
			response.FromUserName = requestMessage.ToUserName;
			return response;
		}

		public override AbstractResponse OnTextRequest(TextRequest textRequest)
		{
			WeiXinHelper.UpdateRencentOpenID(textRequest.FromUserName);
			AbstractResponse keyResponse = this.GetKeyResponse(textRequest.Content, textRequest);
			if (keyResponse != null)
			{
				return keyResponse;
			}
			System.Collections.Generic.IList<Hidistro.Entities.VShop.ReplyInfo> replies = ReplyHelper.GetReplies(ReplyType.Keys);
			if (replies == null || (replies.Count == 0 && this.IsOpenManyService()))
			{
				this.GotoManyCustomerService(textRequest);
			}
			foreach (Hidistro.Entities.VShop.ReplyInfo current in replies)
			{
				if (current.MatchType == MatchType.Equal && current.Keys == textRequest.Content)
				{
					AbstractResponse response = this.GetResponse(current, textRequest.FromUserName);
					response.ToUserName = textRequest.FromUserName;
					response.FromUserName = textRequest.ToUserName;
					AbstractResponse result = response;
					return result;
				}
				if (current.MatchType == MatchType.Like && current.Keys.Contains(textRequest.Content))
				{
					AbstractResponse response2 = this.GetResponse(current, textRequest.FromUserName);
					response2.ToUserName = textRequest.FromUserName;
					response2.FromUserName = textRequest.ToUserName;
					AbstractResponse result = response2;
					return result;
				}
			}
			return this.DefaultResponse(textRequest);
		}

		public override AbstractResponse OnEvent_UnSubscribeRequest(UnSubscribeEventRequest unSubscribeEventRequest)
		{
			string fromUserName = unSubscribeEventRequest.FromUserName;
			Globals.Debuglog("取消关注：" + fromUserName, "_DebugUnSubscribeEventRequestlog.txt");
			MemberProcessor.UpdateUserFollowStateByOpenId(fromUserName, 0);
			return this.DefaultResponse(unSubscribeEventRequest);
		}

		public override AbstractResponse OnEvent_SubscribeRequest(SubscribeEventRequest subscribeEventRequest)
		{
			string text = "";
			MemberProcessor.UpdateUserFollowStateByOpenId(subscribeEventRequest.FromUserName, 1);
			if (subscribeEventRequest.EventKey != null)
			{
				text = subscribeEventRequest.EventKey;
			}
			if (text.Contains("qrscene_"))
			{
				text = text.Replace("qrscene_", "").Trim();
				if (text == "1")
				{
					if (WeiXinHelper.BindAdminOpenId.Count > 10)
					{
						WeiXinHelper.BindAdminOpenId.Clear();
					}
					if (WeiXinHelper.BindAdminOpenId.ContainsKey(subscribeEventRequest.Ticket))
					{
						WeiXinHelper.BindAdminOpenId[subscribeEventRequest.Ticket] = subscribeEventRequest.FromUserName;
					}
					else
					{
						WeiXinHelper.BindAdminOpenId.Add(subscribeEventRequest.Ticket, subscribeEventRequest.FromUserName);
					}
					return new TextResponse
					{
						CreateTime = System.DateTime.Now,
						Content = "您正在扫描尝试绑定管理员身份，身份已识别",
						ToUserName = subscribeEventRequest.FromUserName,
						FromUserName = subscribeEventRequest.ToUserName
					};
				}
				ScanInfos scanInfosByTicket = ScanHelp.GetScanInfosByTicket(subscribeEventRequest.Ticket);
				bool flag = MemberProcessor.IsExitOpenId(subscribeEventRequest.FromUserName);
				int num = scanInfosByTicket.BindUserId;
				if (num < 0)
				{
					num = 0;
				}
				if (!flag && scanInfosByTicket != null)
				{
					this.CreatMember(subscribeEventRequest.FromUserName, num, "");
					ScanHelp.updateScanInfosLastActiveTime(System.DateTime.Now, scanInfosByTicket.Sceneid);
				}
			}
			else
			{
				bool flag2 = MemberProcessor.IsExitOpenId(subscribeEventRequest.FromUserName);
				Globals.Debuglog("关注公众号1", "_DebuglogConcern.txt");
				int num2 = 0;
				if (num2 < 0)
				{
					num2 = 0;
				}
				if (!flag2)
				{
					Globals.Debuglog("关注公众号生成用户1", "_DebuglogConcern.txt");
					this.CreatMember(subscribeEventRequest.FromUserName, num2, "");
				}
			}
			WeiXinHelper.UpdateRencentOpenID(subscribeEventRequest.FromUserName);
			string text2 = "";
			System.Data.DataSet dataSet = new System.Data.DataSet();
			string text3 = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			if (System.IO.File.Exists(text3))
			{
				dataSet.ReadXml(text3);
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
					{
						if (dataRow["id"].ToString() == text)
						{
							text2 = string.Concat(new string[]
							{
								dataRow["WifiDescribe"].ToString(),
								"\r\nWIFI帐号：",
								dataRow["WifiName"].ToString(),
								"\r\n WIFI密码：",
								dataRow["WifiPwd"].ToString()
							});
						}
					}
				}
			}
			if (text2 != "")
			{
				return new TextResponse
				{
					CreateTime = System.DateTime.Now,
					Content = text2,
					ToUserName = subscribeEventRequest.FromUserName,
					FromUserName = subscribeEventRequest.ToUserName
				};
			}
			Hidistro.Entities.VShop.ReplyInfo subscribeReply = ReplyHelper.GetSubscribeReply();
			if (subscribeReply == null)
			{
				return null;
			}
			subscribeReply.Keys = "登录";
			AbstractResponse response = this.GetResponse(subscribeReply, subscribeEventRequest.FromUserName);
			if (response == null)
			{
				this.GotoManyCustomerService(subscribeEventRequest);
			}
			response.ToUserName = subscribeEventRequest.FromUserName;
			response.FromUserName = subscribeEventRequest.ToUserName;
			return response;
		}

		private bool CreatMember(string OpenId, int ReferralUserId, string AceessTokenDefault = "")
		{
			if (string.IsNullOrEmpty(AceessTokenDefault))
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
				AceessTokenDefault = TokenApi.GetToken_Message(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret);
			}
			string urlToDecode = "";
			string userHead = "";
			string text = "";
			BarCodeApi.GetHeadImageUrlByOpenID(AceessTokenDefault, OpenId, out text, out urlToDecode, out userHead);
			string generateId = Globals.GetGenerateId();
			MemberInfo memberInfo = new MemberInfo();
			memberInfo.GradeId = MemberProcessor.GetDefaultMemberGrade();
			memberInfo.UserName = Globals.UrlDecode(urlToDecode);
			memberInfo.OpenId = OpenId;
			memberInfo.CreateDate = System.DateTime.Now;
			memberInfo.SessionId = generateId;
			memberInfo.SessionEndTime = System.DateTime.Now.AddYears(10);
			memberInfo.UserHead = userHead;
			memberInfo.ReferralUserId = ReferralUserId;
			memberInfo.Password = HiCryptographer.Md5Encrypt("888888");
			Globals.Debuglog(JsonConvert.SerializeObject(memberInfo), "_DebuglogScanRegisterUserInfo.txt");
			return MemberProcessor.CreateMember(memberInfo);
		}

		public override AbstractResponse OnEvent_ClickRequest(ClickEventRequest clickEventRequest)
		{
			string userOpenId = clickEventRequest.FromUserName;
			WeiXinHelper.UpdateRencentOpenID(userOpenId);
			AbstractResponse result;
			try
			{
				int menuId = System.Convert.ToInt32(clickEventRequest.EventKey);
				Hidistro.Entities.VShop.MenuInfo menu = VShopHelper.GetMenu(menuId);
				if (menu == null)
				{
					result = null;
				}
				else
				{
					if (menu.BindType == BindType.StoreCard)
					{
						try
						{
							SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);
							string access_token = TokenApi.GetToken(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret);
							access_token = JsonConvert.DeserializeObject<Token>(access_token).access_token;
							MemberInfo member = MemberProcessor.GetOpenIdMember(userOpenId, "wx");
							if (member == null)
							{
								this.CreatMember(userOpenId, 0, access_token);
								member = MemberProcessor.GetOpenIdMember(userOpenId, "wx");
							}
							string userHead = member.UserHead;
							string storeLogo = siteSettings.DistributorLogoPic;
							string webStart = Globals.GetWebUrlStart();
							string imageUrl = "/Storage/master/DistributorCards/MemberCard" + member.UserId + ".jpg";
							string mediaid = string.Empty;
							int ReferralId = 0;
							string storeName = siteSettings.SiteName;
							string NotSuccessMsg = string.Empty;
							DistributorsInfo distributorInfo = DistributorsBrower.GetDistributorInfo(member.UserId);
							if (distributorInfo != null)
							{
								ReferralId = member.UserId;
								if (siteSettings.IsShowDistributorSelfStoreName)
								{
									storeName = distributorInfo.StoreName;
									storeLogo = distributorInfo.Logo;
								}
								imageUrl = "/Storage/master/DistributorCards/StoreCard" + ReferralId + ".jpg";
							}
							else if (!siteSettings.IsShowSiteStoreCard)
							{
								string content = "您还不是分销商，不能为您生成推广图片，立即<a href='" + webStart + "/Vshop/DistributorCenter.aspx'>申请分销商</a>";
								if (!string.IsNullOrEmpty(siteSettings.ToRegistDistributorTips))
								{
									content = System.Text.RegularExpressions.Regex.Replace(siteSettings.ToRegistDistributorTips, "{{申请分销商}}", "<a href='" + webStart + "/Vshop/DistributorCenter.aspx'>申请分销商</a>");
								}
								result = new TextResponse
								{
									CreateTime = System.DateTime.Now,
									ToUserName = userOpenId,
									FromUserName = clickEventRequest.ToUserName,
									Content = content
								};
								return result;
							}
							string postData = string.Empty;
							string creatingStoreCardTips = siteSettings.CreatingStoreCardTips;
							if (!string.IsNullOrEmpty(creatingStoreCardTips))
							{
								postData = string.Concat(new string[]
								{
									"{\"touser\":\"",
									userOpenId,
									"\",\"msgtype\":\"text\",\"text\":{\"content\":\"",
									Globals.String2Json(creatingStoreCardTips),
									"\"}}"
								});
								NewsApi.KFSend(access_token, postData);
							}
							string filePath = System.Web.HttpContext.Current.Request.MapPath(imageUrl);
							Task.Factory.StartNew(delegate
							{
								try
								{
									System.IO.File.Exists(filePath);
									string setJson = System.IO.File.ReadAllText(System.Web.HttpRuntime.AppDomainAppPath + "/Storage/Utility/StoreCardSet.js");
									string codeUrl = webStart + "/Follow.aspx?ReferralId=" + ReferralId.ToString();
									ScanInfos scanInfosByUserId = ScanHelp.GetScanInfosByUserId(ReferralId, 0, "WX");
									if (scanInfosByUserId == null)
									{
										ScanHelp.CreatNewScan(ReferralId, "WX", 0);
										scanInfosByUserId = ScanHelp.GetScanInfosByUserId(ReferralId, 0, "WX");
									}
									if (scanInfosByUserId != null && !string.IsNullOrEmpty(scanInfosByUserId.CodeUrl))
									{
										codeUrl = BarCodeApi.GetQRImageUrlByTicket(scanInfosByUserId.CodeUrl);
									}
									else
									{
										string token_Message = TokenApi.GetToken_Message(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret);
										if (TokenApi.CheckIsRightToken(token_Message))
										{
											string text = BarCodeApi.CreateTicket(token_Message, scanInfosByUserId.Sceneid, "QR_LIMIT_SCENE", "2592000");
											if (!string.IsNullOrEmpty(text))
											{
												codeUrl = BarCodeApi.GetQRImageUrlByTicket(text);
												scanInfosByUserId.CodeUrl = text;
												scanInfosByUserId.CreateTime = System.DateTime.Now;
												scanInfosByUserId.LastActiveTime = System.DateTime.Now;
												ScanHelp.updateScanInfosCodeUrl(scanInfosByUserId);
											}
										}
									}
									StoreCardCreater storeCardCreater = new StoreCardCreater(setJson, userHead, storeLogo, codeUrl, member.UserName, storeName, ReferralId, member.UserId);
									if (storeCardCreater.ReadJson() && storeCardCreater.CreadCard(out NotSuccessMsg))
									{
										if (ReferralId > 0)
										{
											DistributorsBrower.UpdateStoreCard(ReferralId, NotSuccessMsg);
										}
										string media_IDByPath = NewsApi.GetMedia_IDByPath(access_token, webStart + imageUrl);
										mediaid = NewsApi.GetJsonValue(media_IDByPath, "media_id");
									}
									else
									{
										Globals.Debuglog(NotSuccessMsg, "_DebugCreateStoreCardlog.txt");
									}
									postData = string.Concat(new string[]
									{
										"{\"touser\":\"",
										userOpenId,
										"\",\"msgtype\":\"image\",\"image\":{\"media_id\":\"",
										mediaid,
										"\"}}"
									});
									NewsApi.KFSend(access_token, postData);
								}
								catch (System.Exception ex3)
								{
									postData = string.Concat(new string[]
									{
										"{\"touser\":\"",
										userOpenId,
										"\",\"msgtype\":\"text\",\"text\":{\"content\":\"生成图片失败，",
										Globals.String2Json(ex3.ToString()),
										"\"}}"
									});
									NewsApi.KFSend(access_token, postData);
								}
							});
							result = null;
							return result;
						}
						catch (System.Exception ex)
						{
							result = new TextResponse
							{
								CreateTime = System.DateTime.Now,
								ToUserName = userOpenId,
								FromUserName = clickEventRequest.ToUserName,
								Content = "问题:" + ex.ToString()
							};
							return result;
						}
					}
					Hidistro.Entities.VShop.ReplyInfo reply = ReplyHelper.GetReply(menu.ReplyId);
					if (reply == null)
					{
						result = null;
					}
					else
					{
						if (reply.MessageType != MessageType.Image)
						{
							AbstractResponse keyResponse = this.GetKeyResponse(reply.Keys, clickEventRequest);
							if (keyResponse != null)
							{
								result = keyResponse;
								return result;
							}
						}
						AbstractResponse response = this.GetResponse(reply, clickEventRequest.FromUserName);
						if (response == null)
						{
							this.GotoManyCustomerService(clickEventRequest);
						}
						response.ToUserName = clickEventRequest.FromUserName;
						response.FromUserName = clickEventRequest.ToUserName;
						result = response;
					}
				}
			}
			catch (System.Exception ex2)
			{
				result = new TextResponse
				{
					CreateTime = System.DateTime.Now,
					ToUserName = clickEventRequest.FromUserName,
					FromUserName = clickEventRequest.ToUserName,
					Content = "问题:" + ex2.ToString()
				};
			}
			return result;
		}

		public override AbstractResponse OnEvent_MassendJobFinishEventRequest(MassendJobFinishEventRequest massendJobFinishEventRequest)
		{
			AbstractResponse result = null;
			string text = string.Concat(new object[]
			{
				"公众号的微信号(加密的):",
				massendJobFinishEventRequest.ToUserName,
				",发送完成时间：",
				massendJobFinishEventRequest.CreateTime,
				"，过滤通过条数：",
				massendJobFinishEventRequest.FilterCount,
				"，发送失败的粉丝数：",
				massendJobFinishEventRequest.ErrorCount
			});
			string status;
			switch (status = massendJobFinishEventRequest.Status)
			{
			case "send success":
				text += "(发送成功)";
				goto IL_20C;
			case "send fail":
				text += "(发送失败)";
				goto IL_20C;
			case "err(10001)":
				text += "(涉嫌广告)";
				goto IL_20C;
			case "err(20001)":
				text += "(涉嫌政治)";
				goto IL_20C;
			case "err(20004)":
				text += "(涉嫌社会)";
				goto IL_20C;
			case "err(20002)":
				text += "(涉嫌色情)";
				goto IL_20C;
			case "err(20006)":
				text += "(涉嫌违法犯罪)";
				goto IL_20C;
			case "err(20008)":
				text += "(涉嫌欺诈)";
				goto IL_20C;
			case "err(20013)":
				text += "(涉嫌版权)";
				goto IL_20C;
			case "err(22000)":
				text += "(涉嫌互相宣传)";
				goto IL_20C;
			case "err(21000)":
				text += "(涉嫌其他)";
				goto IL_20C;
			}
			text = text + "(" + massendJobFinishEventRequest.Status + ")";
			IL_20C:
			WeiXinHelper.UpdateMsgId(0, massendJobFinishEventRequest.MsgId.ToString(), (massendJobFinishEventRequest.Status == "send success") ? 1 : 2, Globals.ToNum(massendJobFinishEventRequest.SentCount), Globals.ToNum(massendJobFinishEventRequest.TotalCount), text);
			return result;
		}

		public override AbstractResponse OnEvent_ScanRequest(ScanEventRequest scanEventRequest)
		{
			string eventKey = scanEventRequest.EventKey;
			if (eventKey == "1")
			{
				if (WeiXinHelper.BindAdminOpenId.Count > 10)
				{
					WeiXinHelper.BindAdminOpenId.Clear();
				}
				if (WeiXinHelper.BindAdminOpenId.ContainsKey(scanEventRequest.Ticket))
				{
					WeiXinHelper.BindAdminOpenId[scanEventRequest.Ticket] = scanEventRequest.FromUserName;
				}
				else
				{
					WeiXinHelper.BindAdminOpenId.Add(scanEventRequest.Ticket, scanEventRequest.FromUserName);
				}
				return new TextResponse
				{
					CreateTime = System.DateTime.Now,
					Content = "您正在扫描尝试绑定管理员身份，身份已识别",
					ToUserName = scanEventRequest.FromUserName,
					FromUserName = scanEventRequest.ToUserName
				};
			}
			ScanInfos scanInfosByTicket = ScanHelp.GetScanInfosByTicket(scanEventRequest.Ticket);
			Globals.Debuglog(eventKey + ":" + scanEventRequest.Ticket, "_Debuglog.txt");
			bool flag = MemberProcessor.IsExitOpenId(scanEventRequest.FromUserName);
			if (!flag && scanInfosByTicket != null && scanInfosByTicket.BindUserId > 0)
			{
				this.CreatMember(scanEventRequest.FromUserName, scanInfosByTicket.BindUserId, "");
			}
			if (scanInfosByTicket != null)
			{
				ScanHelp.updateScanInfosLastActiveTime(System.DateTime.Now, scanInfosByTicket.Sceneid);
			}
			string text = "";
			System.Data.DataSet dataSet = new System.Data.DataSet();
			string text2 = System.Web.HttpContext.Current.Server.MapPath("/config/WifiConfig.xml");
			if (System.IO.File.Exists(text2))
			{
				dataSet.ReadXml(text2);
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
					{
						if (dataRow["id"].ToString() == eventKey)
						{
							text = string.Concat(new string[]
							{
								dataRow["WifiDescribe"].ToString(),
								"\r\nWIFI帐号：",
								dataRow["WifiName"].ToString(),
								"\r\n WIFI密码：",
								dataRow["WifiPwd"].ToString()
							});
						}
					}
				}
			}
			if (text != "")
			{
				return new TextResponse
				{
					CreateTime = System.DateTime.Now,
					Content = text,
					ToUserName = scanEventRequest.FromUserName,
					FromUserName = scanEventRequest.ToUserName
				};
			}
			if (flag)
			{
				return new TextResponse
				{
					CreateTime = System.DateTime.Now,
					Content = "您刚扫描了分销商公众号二维码！",
					ToUserName = scanEventRequest.FromUserName,
					FromUserName = scanEventRequest.ToUserName
				};
			}
			Hidistro.Entities.VShop.ReplyInfo subscribeReply = ReplyHelper.GetSubscribeReply();
			if (subscribeReply == null)
			{
				return null;
			}
			subscribeReply.Keys = "扫描";
			AbstractResponse response = this.GetResponse(subscribeReply, scanEventRequest.FromUserName);
			response.ToUserName = scanEventRequest.FromUserName;
			response.FromUserName = scanEventRequest.ToUserName;
			return response;
		}

		private AbstractResponse GetKeyResponse(string key, AbstractRequest request)
		{
            AbstractResponse abstractResponse;
            IList<Hidistro.Entities.VShop.ReplyInfo> replies = ReplyHelper.GetReplies(ReplyType.Vote);
            if (replies != null && replies.Count > 0)
            {
                using (IEnumerator<Hidistro.Entities.VShop.ReplyInfo> enumerator = replies.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Hidistro.Entities.VShop.ReplyInfo current = enumerator.Current;
                        if (current.Keys != key)
                        {
                            continue;
                        }
                        VoteInfo voteById = StoreHelper.GetVoteById((long)current.ActivityId);
                        if (voteById == null || !voteById.IsBackup)
                        {
                            continue;
                        }
                        NewsResponse newsResponse = new NewsResponse()
                        {
                            CreateTime = DateTime.Now,
                            FromUserName = request.ToUserName,
                            ToUserName = request.FromUserName,
                            Articles = new List<Article>()
                        };
                        IList<Article> articles = newsResponse.Articles;
                        Article article = new Article()
                        {
                            Description = voteById.VoteName,
                            PicUrl = this.FormatImgUrl(voteById.ImageUrl),
                            Title = voteById.VoteName,
                            Url = string.Format("http://{0}/vshop/Vote.aspx?voteId={1}", HttpContext.Current.Request.Url.Host, voteById.VoteId)
                        };
                        articles.Add(article);
                        abstractResponse = newsResponse;
                        return abstractResponse;
                    }
                    return null;
                }
                return abstractResponse;
            }
            return null;
		}

		private string FormatImgUrl(string img)
		{
			if (!img.StartsWith("http"))
			{
				img = string.Format("http://{0}{1}", System.Web.HttpContext.Current.Request.Url.Host, img);
			}
			return img;
		}

		public AbstractResponse GetResponse(Hidistro.Entities.VShop.ReplyInfo reply, string openId)
		{
			string log = string.Concat(new string[]
			{
				reply.MessageType.ToString(),
				"||",
				reply.MessageType.ToString(),
				"||",
				reply.MessageTypeName
			});
			Globals.Debuglog(log, "_DebuglogYY.txt");
			if (reply.MessageType == MessageType.Text)
			{
				TextReplyInfo textReplyInfo = reply as TextReplyInfo;
				TextResponse textResponse = new TextResponse();
				textResponse.CreateTime = System.DateTime.Now;
				textResponse.Content = Globals.FormatWXReplyContent(textReplyInfo.Text);
				if (reply.Keys == "登录")
				{
					string arg = Globals.GetWebUrlStart() + "/Vshop/MemberCenter.aspx";
					textResponse.Content = textResponse.Content.Replace("$login$", string.Format("<a href=\"{0}\">一键登录</a>", arg));
				}
				return textResponse;
			}
			NewsResponse newsResponse = new NewsResponse();
			newsResponse.CreateTime = System.DateTime.Now;
			newsResponse.Articles = new System.Collections.Generic.List<Article>();
			if (reply.ArticleID > 0)
			{
				ArticleInfo articleInfo = ArticleHelper.GetArticleInfo(reply.ArticleID);
				if (articleInfo.ArticleType == ArticleType.News)
				{
					Article item = new Article
					{
						Description = articleInfo.Memo,
						PicUrl = this.FormatImgUrl(articleInfo.ImageUrl),
						Title = articleInfo.Title,
						Url = (string.IsNullOrEmpty(articleInfo.Url) ? string.Format("http://{0}/Vshop/ArticleDetail.aspx?sid={1}", System.Web.HttpContext.Current.Request.Url.Host, articleInfo.ArticleId) : articleInfo.Url)
					};
					newsResponse.Articles.Add(item);
					return newsResponse;
				}
				if (articleInfo.ArticleType != ArticleType.List)
				{
					return newsResponse;
				}
				Article item2 = new Article
				{
					Description = articleInfo.Memo,
					PicUrl = this.FormatImgUrl(articleInfo.ImageUrl),
					Title = articleInfo.Title,
					Url = (string.IsNullOrEmpty(articleInfo.Url) ? string.Format("http://{0}/Vshop/ArticleDetail.aspx?sid={1}", System.Web.HttpContext.Current.Request.Url.Host, articleInfo.ArticleId) : articleInfo.Url)
				};
				newsResponse.Articles.Add(item2);
				using (System.Collections.Generic.IEnumerator<ArticleItemsInfo> enumerator = articleInfo.ItemsInfo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ArticleItemsInfo current = enumerator.Current;
						item2 = new Article
						{
							Description = "",
							PicUrl = this.FormatImgUrl(current.ImageUrl),
							Title = current.Title,
							Url = (string.IsNullOrEmpty(current.Url) ? string.Format("http://{0}/Vshop/ArticleDetail.aspx?iid={1}", System.Web.HttpContext.Current.Request.Url.Host, current.Id) : current.Url)
						};
						newsResponse.Articles.Add(item2);
					}
					return newsResponse;
				}
			}
			foreach (NewsMsgInfo current2 in (reply as NewsReplyInfo).NewsMsg)
			{
				Article item3 = new Article
				{
					Description = current2.Description,
					PicUrl = string.Format("http://{0}{1}", System.Web.HttpContext.Current.Request.Url.Host, current2.PicUrl),
					Title = current2.Title,
					Url = (string.IsNullOrEmpty(current2.Url) ? string.Format("http://{0}/Vshop/ImageTextDetails.aspx?messageId={1}", System.Web.HttpContext.Current.Request.Url.Host, current2.Id) : current2.Url)
				};
				newsResponse.Articles.Add(item3);
			}
			return newsResponse;
		}
	}
}
