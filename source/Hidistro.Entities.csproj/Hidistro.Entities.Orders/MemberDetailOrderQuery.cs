using Hidistro.Core.Entities;
using System;

namespace Hidistro.Entities.Orders
{
	public class MemberDetailOrderQuery : Pagination
	{
		public OrderStatus[] Status
		{
			get;
			set;
		}

		public System.DateTime? StartDate
		{
			get;
			set;
		}

		public System.DateTime? EndDate
		{
			get;
			set;
		}

		public System.DateTime? StartFinishDate
		{
			get;
			set;
		}

		public System.DateTime? EndFinishDate
		{
			get;
			set;
		}

		public int? UserId
		{
			get;
			set;
		}
	}
}
