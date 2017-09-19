using Hidistro.Entities.Settings;
using Hidistro.SqlDal.Settings;
using System;
using System.Collections.Generic;

namespace ControlPanel.Settings
{
	public static class MenuHelper
	{
		public static bool CanAddMenu(int parentId)
		{
			bool flag;
			IList<MenuInfo> menusByParentId = (new MenuDao()).GetMenusByParentId(parentId);
			if ((menusByParentId == null ? false : menusByParentId.Count != 0))
			{
				flag = (parentId != 0 ? menusByParentId.Count < 5 : menusByParentId.Count < 5);
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

		public static MenuInfo GetMenu(int menuId)
		{
			return (new MenuDao()).GetMenu(menuId);
		}

		public static IList<MenuInfo> GetMenus(string shopMenuStyle)
		{
			IList<MenuInfo> menuInfos;
			IList<MenuInfo> menuInfos1 = new List<MenuInfo>();
			MenuDao menuDao = new MenuDao();
			IList<MenuInfo> topMenus = menuDao.GetTopMenus();
			if (topMenus != null)
			{
				foreach (MenuInfo topMenu in topMenus)
				{
					IList<MenuInfo> menusByParentId = menuDao.GetMenusByParentId(topMenu.MenuId);
					if (shopMenuStyle != "1")
					{
						topMenu.ShopMenuPic = "";
					}
					topMenu.SubMenus = menusByParentId;
					menuInfos1.Add(topMenu);
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

		public static IList<MenuInfo> GetTopMenus()
		{
			return (new MenuDao()).GetTopMenus();
		}

		public static int SaveMenu(MenuInfo menu)
		{
			return (new MenuDao()).SaveMenu(menu);
		}

		public static bool UpdateMenu(MenuInfo menu)
		{
			return (new MenuDao()).UpdateMenu(menu);
		}

		public static bool UpdateMenuName(MenuInfo menu)
		{
			return (new MenuDao()).UpdateMenuName(menu);
		}
	}
}