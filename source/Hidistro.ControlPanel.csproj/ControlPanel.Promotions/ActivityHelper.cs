using Hidistro.Core.Entities;
using Hidistro.Entities.Promotions;
using Hidistro.SqlDal.Promotions;
using System;
using System.Collections.Generic;
using System.Data;

namespace ControlPanel.Promotions
{
	public class ActivityHelper
	{
		private static ActivityDao _act;

		static ActivityHelper()
		{
			ActivityHelper._act = new ActivityDao();
		}

		public ActivityHelper()
		{
		}

		public static bool AddActivitiesMember(int ActivitiesId, int Userid)
		{
			return ActivityHelper._act.AddActivitiesMember(ActivitiesId, Userid, null);
		}

		public static bool AddProducts(int couponId, bool IsAllProduct, IList<string> productIDs)
		{
			return ActivityHelper._act.AddProducts(couponId, IsAllProduct, productIDs);
		}

		public static bool AddProducts(int couponId, int productID)
		{
			return ActivityHelper._act.AddProducts(couponId, productID);
		}

		public static int Create(ActivityInfo act, ref string msg)
		{
			return ActivityHelper._act.Create(act, ref msg);
		}

		public static bool Delete(int Id)
		{
			return ActivityHelper._act.Delete(Id);
		}

		public static bool DeleteProducts(int couponId, string productIds)
		{
			return ActivityHelper._act.DeleteProducts(couponId, productIds);
		}

		public static bool EndAct(int Aid)
		{
			return ActivityHelper._act.EndAct(Aid);
		}

		public static ActivityInfo GetAct(int Id)
		{
			return ActivityHelper._act.GetAct(Id);
		}

		public static ActivityInfo GetAct(string name)
		{
			return ActivityHelper._act.GetAct(name);
		}

		public static DataTable GetActivities()
		{
			return ActivityHelper._act.GetActivities();
		}

		public static DataTable GetActivities_Detail(int actId)
		{
			return ActivityHelper._act.GetActivities_Detail(actId);
		}

		public static int GetActivitiesMember(int Userid, int ActivitiesId)
		{
			return ActivityHelper._act.GetActivitiesMember(Userid, ActivitiesId);
		}

		public static DataTable GetActivitiesProducts(int actid, int ProductID)
		{
			return ActivityHelper._act.GetActivitiesProducts(actid, ProductID);
		}

		public static DataTable GetActivityTopics(string types = "0")
		{
			return ActivityHelper._act.GetActivityTopics(types);
		}

		public static int GetActivityTopicsNum(string types = "0")
		{
			return ActivityHelper._act.GetActivityTopicsNum(types);
		}

		public static int GetHishop_Activities(int Activities_DetailID)
		{
			return ActivityHelper._act.GetHishop_Activities(Activities_DetailID);
		}

		public static bool HasPartProductAct()
		{
			return ActivityHelper._act.HasPartProductAct();
		}

		public static DbQueryResult Query(ActivitySearch query)
		{
			return ActivityHelper._act.Query(query);
		}

		public static DataTable QueryProducts(int actid)
		{
			return ActivityHelper._act.QueryProducts(actid);
		}

		public static DataTable QueryProducts()
		{
			return ActivityHelper._act.QueryProducts();
		}

		public static DataTable QuerySelProducts()
		{
			return ActivityHelper._act.QuerySelProducts();
		}

		public static bool SetProductsStatus(int couponId, int status, string productIds)
		{
			return ActivityHelper._act.SetProductsStatus(couponId, status, productIds);
		}

		public static bool Update(ActivityInfo act, ref string msg)
		{
			return ActivityHelper._act.Update(act, ref msg);
		}
	}
}