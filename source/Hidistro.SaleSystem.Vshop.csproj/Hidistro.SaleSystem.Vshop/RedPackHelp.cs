using Hidistro.SqlDal.Store;
using System;

namespace Hidistro.SaleSystem.Vshop
{
	public static class RedPackHelp
	{
		public static void RedPackCheckJob()
		{
			RedPackDao redPackDao = new RedPackDao();
			redPackDao.RedPackCheckJob();
		}
	}
}
