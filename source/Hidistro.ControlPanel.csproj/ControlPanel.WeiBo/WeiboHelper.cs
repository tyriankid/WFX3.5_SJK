using Hidistro.Core.Entities;
using Hidistro.Entities.Weibo;
using Hidistro.SqlDal.Weibo;
using System;
using System.Collections.Generic;
using System.Data;

namespace ControlPanel.WeiBo
{
	public static class WeiboHelper
	{
		public static bool CanAddMenu(int parentId)
		{
			bool flag;
			IList<MenuInfo> menusByParentId = (new MenuDao()).GetMenusByParentId(parentId);
			if ((menusByParentId == null ? false : menusByParentId.Count != 0))
			{
				flag = (parentId != 0 ? menusByParentId.Count < 5 : menusByParentId.Count < 3);
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public static bool DeleteMenu(int menuId)
		{
			return (new MenuDao()).DeleteMenu(menuId);
		}

		public static bool DeleteReplyInfo(int ReplyInfoid)
		{
			return (new ReplyDao()).DeleteReplyInfo(ReplyInfoid);
		}

		public static bool DeleteReplyKeyInfo(int ReplyKeyInfoid)
		{
			return (new ReplyDao()).DeleteReplyKeyInfo(ReplyKeyInfoid);
		}

		public static MenuInfo GetMenu(int menuId)
		{
			return (new MenuDao()).GetMenu(menuId);
		}

		public static IList<MenuInfo> GetMenus()
		{
			IList<MenuInfo> menuInfos;
			IList<MenuInfo> menuInfos1 = new List<MenuInfo>();
			MenuDao menuDao = new MenuDao();
			IList<MenuInfo> topMenus = menuDao.GetTopMenus();
			if (topMenus != null)
			{
				foreach (MenuInfo topMenu in topMenus)
				{
					menuInfos1.Add(topMenu);
					IList<MenuInfo> menusByParentId = menuDao.GetMenusByParentId(topMenu.MenuId);
					if (menusByParentId != null)
					{
						foreach (MenuInfo menuInfo in menusByParentId)
						{
							menuInfos1.Add(menuInfo);
						}
					}
				}
				menuInfos = menuInfos1;
			}
			else
			{
				menuInfos = menuInfos1;
			}
			return menuInfos;
		}

		public static IList<MenuInfo> GetMenusByParentId(int parentId)
		{
			return (new MenuDao()).GetMenusByParentId(parentId);
		}

		public static MessageInfo GetMessageInfo(int MessageId)
		{
			return (new MessageDao()).GetMessageInfo(MessageId);
		}

		public static DbQueryResult GetMessages(MessageQuery messageQuery)
		{
			return (new MessageDao()).GetMessages(messageQuery);
		}

		public static DataTable GetReplyAll(int type)
		{
			return (new ReplyDao()).GetReplyAll(type);
		}

		public static IList<ReplyInfo> GetReplyInfo(int ReplyKeyId)
		{
			return (new ReplyDao()).GetReplyInfo(ReplyKeyId);
		}

		public static ReplyInfo GetReplyInfoMes(int id)
		{
			return (new ReplyDao()).GetReplyInfoMes(id);
		}

		public static IList<ReplyInfo> GetReplyTypeInfo(int Type)
		{
			return (new ReplyDao()).GetReplyTypeInfo(Type);
		}

		public static IList<MenuInfo> GetTopMenus()
		{
			return (new MenuDao()).GetTopMenus();
		}

		public static IList<ReplyKeyInfo> GetTopReplyInfos(int Type)
		{
			return (new ReplyDao()).GetTopReplyInfos(Type);
		}

		public static DataTable GetWeibo_Reply(int type)
		{
			return (new ReplyDao()).GetWeibo_Reply(type);
		}

		public static bool SaveMenu(MenuInfo menu)
		{
			return (new MenuDao()).SaveMenu(menu);
		}

		public static int SaveMessage(MessageInfo messageInfo)
		{
			return (new MessageDao()).SaveMessage(messageInfo);
		}

		public static bool SaveReplyInfo(ReplyInfo replyInfo)
		{
			return (new ReplyDao()).SaveReplyInfo(replyInfo);
		}

		public static bool SaveReplyKeyInfo(ReplyKeyInfo replyKeyInfo)
		{
			return (new ReplyDao()).SaveReplyKeyInfo(replyKeyInfo);
		}

		public static bool UpdateMatching(ReplyKeyInfo replyKeyInfo)
		{
			return (new ReplyDao()).UpdateMatching(replyKeyInfo);
		}

		public static bool UpdateMenu(MenuInfo menu)
		{
			return (new MenuDao()).UpdateMenu(menu);
		}

		public static bool UpdateMessage(MessageInfo messageInfo)
		{
			return (new MessageDao()).UpdateMessage(messageInfo);
		}

		public static bool UpdateReplyInfo(ReplyInfo replyInfo)
		{
			return (new ReplyDao()).UpdateReplyInfo(replyInfo);
		}

		public static bool UpdateReplyKeyInfo(ReplyKeyInfo replyKeyInfo)
		{
			return (new ReplyDao()).UpdateReplyKeyInfo(replyKeyInfo);
		}
	}
}