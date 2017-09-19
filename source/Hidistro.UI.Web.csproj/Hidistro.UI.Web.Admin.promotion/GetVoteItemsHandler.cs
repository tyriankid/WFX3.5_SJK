using ControlPanel.Promotions;
using Hidistro.Core;
using Hidistro.Entities.Promotions;
using Newtonsoft.Json;
using System;
using System.Web;

namespace Hidistro.UI.Web.Admin.promotion
{
	public class GetVoteItemsHandler : System.Web.IHttpHandler
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
			context.Response.ContentType = "text/plain";
			long num = 0L;
			long.TryParse(context.Request.QueryString["id"], out num);
			try
			{
				if (Globals.GetCurrentManagerUserId() <= 0)
				{
					context.Response.Write("{\"type\":\"error\",\"data\":\"请先登录\"}");
					context.Response.End();
				}
				if (num > 0L)
				{
					VoteInfo vote = VoteHelper.GetVote(num);
					if (vote != null)
					{
						var value = new
						{
							type = "success",
							VoteName = vote.VoteName,
							data = vote.VoteItems
						};
						string s = JsonConvert.SerializeObject(value);
						context.Response.Write(s);
					}
					else
					{
						context.Response.Write("{\"type\":\"error\",\"data\":\"该投票调查不存在！\"}");
					}
				}
				else
				{
					context.Response.Write("{\"type\":\"error\",\"data\":\"参数错误！\"}");
				}
			}
			catch (System.Exception ex)
			{
				context.Response.Write("{\"type\":\"error\",\"data\":\"" + Globals.String2Json(ex.ToString()) + "\"}");
			}
		}
	}
}
