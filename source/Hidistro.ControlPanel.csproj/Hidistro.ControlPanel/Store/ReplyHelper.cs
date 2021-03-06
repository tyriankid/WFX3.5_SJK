using Hidistro.Entities.Store;
using Hidistro.Entities.VShop;
using Hidistro.SqlDal.VShop;
using System;
using System.Collections.Generic;

namespace Hidistro.ControlPanel.Store
{
	public class ReplyHelper
	{
		public ReplyHelper()
		{
		}

		public static void DeleteNewsMsg(int id)
		{
			(new ReplyDao()).DeleteNewsMsg(id);
		}

		public static bool DeleteReply(int id)
		{
			return (new ReplyDao()).DeleteReply(id);
		}

		public static IList<ReplyInfo> GetAllFuwuReply()
		{
			return (new ReplyDao()).GetAllReply();
		}

		public static IList<ReplyInfo> GetAllReply()
		{
			return (new ReplyDao()).GetAllReply();
		}

		public static int GetArticleIDByOldArticle(int replyid, MessageType msgtype)
		{
			return (new ReplyDao()).GetArticleIDByOldArticle(replyid, msgtype);
		}

		public static ReplyInfo GetMismatchReply()
		{
			ReplyInfo item;
			IList<ReplyInfo> replies = (new ReplyDao()).GetReplies(ReplyType.NoMatch);
			if ((replies == null ? true : replies.Count <= 0))
			{
				item = null;
			}
			else
			{
				item = replies[0];
			}
			return item;
		}

		public static int GetNoMatchReplyID(int compareid)
		{
			return (new ReplyDao()).GetNoMatchReplyID(compareid);
		}

		public static IList<ReplyInfo> GetReplies(ReplyType type)
		{
			return (new ReplyDao()).GetReplies(type);
		}

		public static ReplyInfo GetReply(int id)
		{
			return (new ReplyDao()).GetReply(id);
		}

		public static int GetSubscribeID(int compareid)
		{
			return (new ReplyDao()).GetSubscribeID(compareid);
		}

		public static ReplyInfo GetSubscribeReply()
		{
			ReplyInfo item;
			IList<ReplyInfo> replies = (new ReplyDao()).GetReplies(ReplyType.Subscribe);
			if ((replies == null ? true : replies.Count <= 0))
			{
				item = null;
			}
			else
			{
				item = replies[0];
			}
			return item;
		}

		public static bool HasReplyKey(string key)
		{
			return (new ReplyDao()).HasReplyKey(key);
		}

		public static bool HasReplyKey(string key, int replyid)
		{
			return (new ReplyDao()).HasReplyKey(key, replyid);
		}

		public static bool SaveReply(ReplyInfo reply)
		{
			reply.LastEditDate = DateTime.Now;
			reply.LastEditor = ManagerHelper.GetCurrentManager().UserName;
			return (new ReplyDao()).SaveReply(reply);
		}

		public static bool UpdateReply(ReplyInfo reply)
		{
			reply.LastEditDate = DateTime.Now;
			reply.LastEditor = ManagerHelper.GetCurrentManager().UserName;
			return (new ReplyDao()).UpdateReply(reply);
		}

		public static bool UpdateReplyRelease(int id)
		{
			return (new ReplyDao()).UpdateReplyRelease(id);
		}
	}
}