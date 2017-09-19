using Hidistro.Core.Entities;
using System;

namespace Hidistro.Entities.Members
{
	public class MemberAmountQuery : Pagination
	{
		public int UserId
		{
			get;
			set;
		}

		public string PayId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public string TradeType
		{
			get;
			set;
		}

		public string TradeWays
		{
			get;
			set;
		}

		public string StartTime
		{
			get;
			set;
		}

		public string EndTime
		{
			get;
			set;
		}
	}
}
