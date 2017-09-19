using Hidistro.Entities.Settings;
using Hidistro.SqlDal.Settings;
using System;
using System.Data;

namespace ControlPanel.Settings
{
	public class CustomerServiceHelper
	{
		private static CustomerServiceDao dao;

		static CustomerServiceHelper()
		{
			CustomerServiceHelper.dao = new CustomerServiceDao();
		}

		public CustomerServiceHelper()
		{
		}

		public static int CreateCustomer(CustomerServiceInfo info, ref string msg)
		{
			return CustomerServiceHelper.dao.CreateCustomer(info, ref msg);
		}

		public static bool DeletCustomer(int id)
		{
			return CustomerServiceHelper.dao.DeletCustomer(id);
		}

		public static CustomerServiceInfo GetCustomer(int id)
		{
			return CustomerServiceHelper.dao.GetCustomer(id);
		}

		public static DataTable GetCustomers(string unit)
		{
			return CustomerServiceHelper.dao.GetCustomers(unit);
		}

		public static bool UpdateCustomer(CustomerServiceInfo info, ref string msg)
		{
			return CustomerServiceHelper.dao.UpdateCustomer(info, ref msg);
		}
	}
}