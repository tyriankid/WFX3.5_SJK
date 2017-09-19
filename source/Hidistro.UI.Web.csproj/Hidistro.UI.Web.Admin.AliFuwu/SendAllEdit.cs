using Aop.Api.Response;
using ControlPanel.WeiBo;
using ControlPanel.WeiXin;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Store;
using Hidistro.Entities.VShop;
using Hidistro.Entities.Weibo;
using Hidistro.Entities.WeiXin;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.hieditor.ueditor.controls;
using Hishop.AlipayFuwu.Api.Model;
using Hishop.AlipayFuwu.Api.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.AliFuwu
{
	public class SendAllEdit : AdminPage
	{
		private string errcode = string.Empty;

		protected string htmlInfo = string.Empty;

		protected int sendID = Globals.RequestQueryNum("ID");

		protected int LocalArticleID = Globals.RequestQueryNum("aid");

		protected string type = Globals.RequestQueryStr("type");

		protected System.Web.UI.HtmlControls.HtmlForm aspnetForm;

		protected System.Web.UI.WebControls.Literal litInfo;

		protected System.Web.UI.WebControls.TextBox txtTitle;

		protected System.Web.UI.WebControls.HiddenField hdfSendID;

		protected System.Web.UI.WebControls.HiddenField hdfMessageType;

		protected System.Web.UI.WebControls.HiddenField hdfArticleID;

		protected System.Web.UI.WebControls.HiddenField hdfIsOldArticle;

		protected ucUeditor fkContent;

		protected SendAllEdit() : base("m11", "fwp03")
		{
		}

		[PrivilegeCheck(Privilege.Summary)]
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!base.IsPostBack)
            {
                if (this.type == "getarticleinfo")
                {
                    base.Response.ContentType = "application/json";
                    string str1 = "{\"type\":\"0\",\"tips\":\"操作失败\"}";
                    int num = Globals.RequestFormNum("articleid");
                    if (num > 0)
                    {
                        ArticleInfo articleInfo = ArticleHelper.GetArticleInfo(num);
                        if (articleInfo != null)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            switch (articleInfo.ArticleType)
                            {
                                case ArticleType.News:
                                    {
                                        object[] articleType = new object[] { "{\"type\":\"1\",\"articletype\":", (int)articleInfo.ArticleType, ",\"title\":\"", this.String2Json(articleInfo.Title), "\",\"date\":\"", null, null, null, null, null, null };
                                        DateTime pubTime = articleInfo.PubTime;
                                        articleType[5] = this.String2Json(pubTime.ToString("M月d日"));
                                        articleType[6] = "\",\"imgurl\":\"";
                                        articleType[7] = this.String2Json(articleInfo.ImageUrl);
                                        articleType[8] = "\",\"memo\":\"";
                                        articleType[9] = this.String2Json(articleInfo.Memo);
                                        articleType[10] = "\"}";
                                        str1 = string.Concat(articleType);
                                        break;
                                    }
                                case ArticleType.Text | ArticleType.News:
                                    {
                                        object[] objArray = new object[] { "{\"type\":\"1\",\"articletype\":", (int)articleInfo.ArticleType, ",\"title\":\"", this.String2Json(articleInfo.Title), "\",\"date\":\"", null, null, null, null, null, null };
                                        DateTime dateTime = articleInfo.PubTime;
                                        objArray[5] = this.String2Json(dateTime.ToString("M月d日"));
                                        objArray[6] = "\",\"imgurl\":\"";
                                        objArray[7] = this.String2Json(articleInfo.ImageUrl);
                                        objArray[8] = "\",\"memo\":\"";
                                        objArray[9] = this.String2Json(articleInfo.Content);
                                        objArray[10] = "\"}";
                                        str1 = string.Concat(objArray);
                                        break;
                                    }
                                case ArticleType.List:
                                    {
                                        foreach (ArticleItemsInfo itemsInfo in articleInfo.ItemsInfo)
                                        {
                                            string[] strArrays = new string[] { "{\"title\":\"", this.String2Json(itemsInfo.Title), "\",\"imgurl\":\"", this.String2Json(itemsInfo.ImageUrl), "\"}," };
                                            stringBuilder.Append(string.Concat(strArrays));
                                        }
                                        object[] articleType1 = new object[] { "{\"type\":\"1\",\"articletype\":", (int)articleInfo.ArticleType, ",\"title\":\"", this.String2Json(articleInfo.Title), "\",\"date\":\"", null, null, null, null, null, null };
                                        DateTime pubTime1 = articleInfo.PubTime;
                                        articleType1[5] = this.String2Json(pubTime1.ToString("M月d日"));
                                        articleType1[6] = "\",\"imgurl\":\"";
                                        articleType1[7] = this.String2Json(articleInfo.ImageUrl);
                                        articleType1[8] = "\",\"items\":[";
                                        string str2 = stringBuilder.ToString();
                                        char[] chrArray = new char[] { ',' };
                                        articleType1[9] = str2.Trim(chrArray);
                                        articleType1[10] = "]}";
                                        str1 = string.Concat(articleType1);
                                        break;
                                    }
                                default:
                                    {
                                        goto case ArticleType.Text | ArticleType.News;
                                    }
                            }
                        }
                    }
                    base.Response.Write(str1);
                    base.Response.End();
                    return;
                }
                if (this.type == "postdata")
                {
                    base.Response.ContentType = "application/json";
                    string str3 = "{\"type\":\"1\",\"tips\":\"操作成功\"}";
                    this.sendID = Globals.RequestFormNum("sendid");
                    int num1 = Globals.RequestFormNum("sendtype");
                    int num2 = Globals.RequestFormNum("msgtype");
                    int num3 = Globals.RequestFormNum("articleid");
                    string str4 = Globals.RequestFormStr("title");
                    string str5 = Globals.RequestFormStr("content");
                    int num4 = Globals.RequestFormNum("isoldarticle");
                    string str6 = this.SavePostData(num2, num3, str4, str5, num4, this.sendID, true);
                    if (!string.IsNullOrEmpty(str6))
                    {
                        str3 = string.Concat("{\"type\":\"0\",\"tips\":\"", str6, "\"}");
                    }
                    else
                    {
                        MessageType messageType = (MessageType)num2;
                        string empty = string.Empty;
                        Articles article = new Articles()
                        {
                            msgType = "text"
                        };
                        string str7 = Globals.HostPath(HttpContext.Current.Request.Url);
                        if (messageType == MessageType.List || messageType == MessageType.News)
                        {
                            this.sendID = Globals.ToNum(this.SavePostData(num2, num3, str4, str5, num4, this.sendID, false));
                            ArticleInfo articleInfo1 = ArticleHelper.GetArticleInfo(num3);
                            if (articleInfo1 == null)
                            {
                                str3 = "{\"type\":\"0\",\"tips\":\"素材不存在了\"}";
                                base.Response.Write(str3);
                                base.Response.End();
                            }
                            article = this.GetAlipayArticlesFromArticleInfo(articleInfo1, str7);
                        }
                        else
                        {
                            this.sendID = Globals.ToNum(this.SavePostData(num2, num3, str4, str5, num4, this.sendID, false));
                            Articles article1 = article;
                            MessageText messageText = new MessageText()
                            {
                                content = Globals.StripHtmlXmlTags(str5)
                            };
                            article1.text = messageText;
                        }
                        SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                        if (AlipayFuwuConfig.appId.Length < 15)
                        {
                            AlipayFuwuConfig.CommSetConfig(masterSettings.AlipayAppid, base.Server.MapPath("~/"), "GBK");
                        }
                        if (num1 != 1)
                        {
                            List<string> strs = new List<string>();
                            DataTable rencentAliOpenID = WeiXinHelper.GetRencentAliOpenID();
                            if (rencentAliOpenID != null)
                            {
                                for (int i = 0; i < rencentAliOpenID.Rows.Count; i++)
                                {
                                    strs.Add(rencentAliOpenID.Rows[i][0].ToString());
                                }
                            }
                            if (strs.Count <= 0)
                            {
                                str3 = "{\"type\":\"0\",\"tips\":\"暂时没有关注的用户可以发送信息\"}";
                            }
                            else
                            {
                                WeiXinHelper.UpdateMsgId(this.sendID, "", 0, 0, strs.Count, "");
                                (new Thread(() =>
                                {
                                    try
                                    {
                                        bool flag = false;
                                        foreach (string str in strs)
                                        {
                                            if (str.Length <= 16)
                                            {
                                                continue;
                                            }
                                            article.toUserId = str;
                                            AlipayMobilePublicMessageCustomSendResponse alipayMobilePublicMessageCustomSendResponse = AliOHHelper.CustomSend(article);
                                            if (alipayMobilePublicMessageCustomSendResponse == null || !alipayMobilePublicMessageCustomSendResponse.IsError)
                                            {
                                                flag = true;
                                                WeiXinHelper.UpdateAddSendCount(this.sendID, 1, -1);
                                            }
                                            else
                                            {
                                                AliOHHelper.log(alipayMobilePublicMessageCustomSendResponse.Body);
                                            }
                                            Thread.Sleep(10);
                                        }
                                        if (!flag)
                                        {
                                            WeiXinHelper.UpdateAddSendCount(this.sendID, 0, 2);
                                        }
                                        else
                                        {
                                            WeiXinHelper.UpdateAddSendCount(this.sendID, 0, 1);
                                        }
                                        Thread.Sleep(10);
                                    }
                                    catch (Exception exception)
                                    {
                                        AliOHHelper.log(exception.Message.ToString());
                                    }
                                })).Start();
                                str3 = "{\"type\":\"1\",\"tips\":\"信息正在后台推送中，请稍后刷新群发列表查看结果\"}";
                            }
                        }
                        else
                        {
                            AlipayMobilePublicMessageTotalSendResponse alipayMobilePublicMessageTotalSendResponse = AliOHHelper.TotalSend(article);
                            if (alipayMobilePublicMessageTotalSendResponse.IsError || !(alipayMobilePublicMessageTotalSendResponse.Code == "200"))
                            {
                                str3 = string.Concat("{\"type\":\"0\",\"tips\":\"", alipayMobilePublicMessageTotalSendResponse.Msg, "\"}");
                                WeiXinHelper.UpdateMsgId(this.sendID, "", 2, 0, 0, alipayMobilePublicMessageTotalSendResponse.Body);
                            }
                            else
                            {
                                str3 = "{\"type\":\"1\",\"tips\":\"服务窗群发成功，请于一天后到服务窗后台查询送达结果！\"}";
                                string str8 = "";
                                if (!string.IsNullOrEmpty(alipayMobilePublicMessageTotalSendResponse.Data) && alipayMobilePublicMessageTotalSendResponse.Data.Length > 50)
                                {
                                    str8 = alipayMobilePublicMessageTotalSendResponse.Data.Substring(0, 49);
                                }
                                int alypayUserNum = WeiXinHelper.getAlypayUserNum();
                                WeiXinHelper.UpdateMsgId(this.sendID, str8, 1, alypayUserNum, alypayUserNum, "");
                            }
                        }
                    }
                    base.Response.Write(str3);
                    base.Response.End();
                    return;
                }
                if (this.sendID > 0)
                {
                    this.hdfSendID.Value = this.sendID.ToString();
                    SendAllInfo sendAllInfo = WeiXinHelper.GetSendAllInfo(this.sendID);
                    if (sendAllInfo == null)
                    {
                        base.Response.Redirect("sendalllist.aspx");
                        base.Response.End();
                    }
                    else
                    {
                        MessageType messageType1 = sendAllInfo.MessageType;
                        this.hdfMessageType.Value = sendAllInfo.MessageType.ToString();
                        int articleID = sendAllInfo.ArticleID;
                        this.hdfArticleID.Value = articleID.ToString();
                        this.txtTitle.Text = sendAllInfo.Title;
                        switch (messageType1)
                        {
                            case MessageType.Text:
                                {
                                    this.fkContent.Text = sendAllInfo.Content;
                                    break;
                                }
                            case MessageType.News:
                                {
                                    if (articleID > 0)
                                    {
                                        break;
                                    }
                                    this.hdfIsOldArticle.Value = "1";
                                    NewsReplyInfo reply = ReplyHelper.GetReply(this.sendID) as NewsReplyInfo;
                                    if (reply == null || reply.NewsMsg == null || reply.NewsMsg.Count == 0)
                                    {
                                        break;
                                    }
                                    string[] title = new string[] { "<div class=\"mate-inner\"><h3 id=\"singelTitle\">", reply.NewsMsg[0].Title, "</h3><span>", null, null, null, null, null, null };
                                    title[3] = reply.LastEditDate.ToString("M月d日");
                                    title[4] = "</span><div class=\"mate-img\"><img id=\"img1\" src=\"";
                                    title[5] = reply.NewsMsg[0].PicUrl;
                                    title[6] = "\" class=\"img-responsive\"></div><div class=\"mate-info\" id=\"Lbmsgdesc\">";
                                    title[7] = reply.NewsMsg[0].Description;
                                    title[8] = "</div><div class=\"red-all clearfix\"><strong class=\"fl\">查看全文</strong><em class=\"fr\">&gt;</em></div></div>";
                                    this.htmlInfo = string.Concat(title);
                                    break;
                                }
                            case MessageType.List:
                                {
                                    if (articleID > 0)
                                    {
                                        break;
                                    }
                                    this.hdfIsOldArticle.Value = "1";
                                    NewsReplyInfo newsReplyInfo = ReplyHelper.GetReply(this.sendID) as NewsReplyInfo;
                                    if (newsReplyInfo == null)
                                    {
                                        break;
                                    }
                                    StringBuilder stringBuilder1 = new StringBuilder();
                                    if (newsReplyInfo.NewsMsg == null || newsReplyInfo.NewsMsg.Count <= 0)
                                    {
                                        break;
                                    }
                                    int num5 = 0;
                                    foreach (NewsMsgInfo newsMsg in newsReplyInfo.NewsMsg)
                                    {
                                        num5++;
                                        if (num5 != 1)
                                        {
                                            string[] title1 = new string[] { "             <div class=\"mate-inner\">                 <div class=\"child-mate\">                     <div class=\"child-mate-title clearfix\">                         <div class=\"title\">", newsMsg.Title, "</div>                         <div class=\"img\">                             <img src=\"", newsMsg.PicUrl, "\" class=\"img-responsive\">                         </div>                     </div>                 </div>             </div>" };
                                            stringBuilder1.Append(string.Concat(title1));
                                        }
                                        else
                                        {
                                            string[] picUrl = new string[] { "<div class=\"mate-inner top\">                 <div class=\"mate-img\" >                     <img id=\"img1\" src=\"", newsMsg.PicUrl, "\" class=\"img-responsive\">                     <div class=\"title\" id=\"title1\">", newsMsg.Title, "</div>                 </div>             </div>" };
                                            stringBuilder1.Append(string.Concat(picUrl));
                                        }
                                    }
                                    this.htmlInfo = stringBuilder1.ToString();
                                    break;
                                }
                        }
                    }
                }
                else if (this.LocalArticleID > 0)
                {
                    ArticleInfo articleInfo2 = ArticleHelper.GetArticleInfo(this.LocalArticleID);
                    if (articleInfo2 != null)
                    {
                        this.hdfArticleID.Value = this.LocalArticleID.ToString();
                        this.hdfMessageType.Value = articleInfo2.ArticleType.ToString();
                    }
                }
                if (string.IsNullOrEmpty(this.htmlInfo))
                {
                    this.htmlInfo = "<div class=\"exit-shop-info\">内容区</div>";
                }
                this.litInfo.Text = this.htmlInfo;
            }
        }

		private string GetKFSendImageJson(string openid, ArticleInfo articleinfo)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			if (articleinfo != null && articleinfo != null)
			{
				switch (articleinfo.ArticleType)
				{
				case ArticleType.News:
					break;
				case (ArticleType)3:
					goto IL_200;
				case ArticleType.List:
				{
					stringBuilder.Append(string.Concat(new string[]
					{
						"{\"title\":\"",
						this.String2Json(articleinfo.Title),
						"\",\"description\":\"",
						this.String2Json(articleinfo.Memo),
						"\",\"url\":\"",
						this.String2Json(this.FormatUrl(articleinfo.Url)),
						"\",\"picurl\":\"",
						this.String2Json(this.FormatUrl(articleinfo.ImageUrl)),
						"\"}"
					}));
					System.Collections.Generic.IList<ArticleItemsInfo> itemsInfo = articleinfo.ItemsInfo;
					using (System.Collections.Generic.IEnumerator<ArticleItemsInfo> enumerator = itemsInfo.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ArticleItemsInfo current = enumerator.Current;
							stringBuilder.Append(string.Concat(new string[]
							{
								",{\"title\":\"",
								this.String2Json(current.Title),
								"\",\"description\":\"\",\"url\":\"",
								this.String2Json(this.FormatUrl(current.Url)),
								"\",\"picurl\":\"",
								this.String2Json(this.FormatUrl(current.ImageUrl)),
								"\"}"
							}));
						}
						goto IL_200;
					}
					break;
				}
				default:
					goto IL_200;
				}
				stringBuilder.Append(string.Concat(new string[]
				{
					"{\"title\":\"",
					this.String2Json(articleinfo.Title),
					"\",\"description\":\"",
					this.String2Json(articleinfo.Memo),
					"\",\"url\":\"",
					this.String2Json(this.FormatUrl(articleinfo.Url)),
					"\",\"picurl\":\"",
					this.String2Json(this.FormatUrl(articleinfo.ImageUrl)),
					"\"}"
				}));
			}
			IL_200:
			return string.Concat(new string[]
			{
				"{\"touser\":\"",
				openid,
				"\",\"msgtype\":\"news\",\"news\":{\"articles\": [",
				stringBuilder.ToString(),
				"]}}"
			});
		}

		private string SavePostData(int msgType, int articleid, string title, string content, int isoldarticle, int sendid, bool isonlycheck)
		{
			string result = string.Empty;
			if (string.IsNullOrEmpty(title))
			{
				return "请输入标题！";
			}
			if (articleid < 1 && msgType != 1)
			{
				if (isoldarticle == 0)
				{
					return "请先选择图文！";
				}
				if (sendid > 0 && !isonlycheck)
				{
					articleid = ReplyHelper.GetArticleIDByOldArticle(sendid, (MessageType)msgType);
				}
			}
			if (!isonlycheck)
			{
				SendAllInfo sendAllInfo = new SendAllInfo();
				if (sendid > 0)
				{
					sendAllInfo = WeiXinHelper.GetSendAllInfo(sendid);
				}
				sendAllInfo.Title = title;
				sendAllInfo.MessageType = (MessageType)msgType;
				sendAllInfo.ArticleID = articleid;
				sendAllInfo.Content = content;
				sendAllInfo.SendState = 0;
				sendAllInfo.SendTime = System.DateTime.Now;
				sendAllInfo.SendCount = 0;
				string s = WeiXinHelper.SaveSendAllInfo(sendAllInfo, 1);
				int num = Globals.ToNum(s);
				if (num == 0)
				{
					return "服务窗群发保存失败！";
				}
				if (num > 0)
				{
					result = num.ToString();
				}
			}
			return result;
		}

		private Articles GetAlipayArticlesFromArticleInfo(ArticleInfo articleInfo, string storeUrl)
		{
			Articles articles = null;
			if (articleInfo != null)
			{
				articles = new Articles();
				articles.msgType = "image-text";
				articles.articles = new System.Collections.Generic.List<article>();
				string text = articleInfo.ImageUrl;
				if (!text.ToLower().StartsWith("http"))
				{
					text = storeUrl + text;
				}
				string text2 = Globals.StripAllTags(articleInfo.Content);
				if (text2.Length > 30)
				{
					text2 = text2.Substring(0, 30);
				}
				article item = new article
				{
					actionName = "立即查看",
					title = articleInfo.Title,
					desc = text2,
					imageUrl = text,
					url = articleInfo.Url
				};
				articles.articles.Add(item);
				if (articleInfo.ItemsInfo != null && articleInfo.ItemsInfo.Count > 0)
				{
					foreach (ArticleItemsInfo current in articleInfo.ItemsInfo)
					{
						string text3 = current.ImageUrl;
						if (!text3.ToLower().StartsWith("http"))
						{
							text3 = storeUrl + text3;
						}
						string text4 = Globals.StripAllTags(current.Content);
						if (text4.Length > 30)
						{
							text4 = text4.Substring(0, 30);
						}
						article item2 = new article
						{
							actionName = "立即查看",
							title = current.Title,
							desc = text4,
							imageUrl = text3,
							url = current.Url
						};
						articles.articles.Add(item2);
					}
				}
			}
			return articles;
		}

		private string FormatUrl(string url)
		{
			string result = url;
			if (url.StartsWith("/"))
			{
				result = "http://" + Globals.DomainName + url;
			}
			return result;
		}

		private string String2Json(string s)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			int i = 0;
			while (i < s.Length)
			{
				char c = s.ToCharArray()[i];
				char c2 = c;
				if (c2 <= '"')
				{
					switch (c2)
					{
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\v':
						goto IL_C0;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					default:
						if (c2 != '"')
						{
							goto IL_C0;
						}
						stringBuilder.Append("\\\"");
						break;
					}
				}
				else if (c2 != '/')
				{
					if (c2 != '\\')
					{
						goto IL_C0;
					}
					stringBuilder.Append("\\\\");
				}
				else
				{
					stringBuilder.Append("\\/");
				}
				IL_C8:
				i++;
				continue;
				IL_C0:
				stringBuilder.Append(c);
				goto IL_C8;
			}
			return stringBuilder.ToString();
		}

		private string FormatSendContent(string content)
		{
			string text = System.Text.RegularExpressions.Regex.Replace(content, "</?([^>^a^p]*)>", "");
			text = System.Text.RegularExpressions.Regex.Replace(text, "<img([^>]*)>", "");
			text = text.Replace("<p>", "");
			text = text.Replace("</p>", "\r");
			text = text.Trim(new char[]
			{
				'\r'
			});
			return text.Replace("\r", "\r\n");
		}
	}
}
