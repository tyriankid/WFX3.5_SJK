using System;
using System.Collections.Generic;

namespace Hidistro.Entities.Orders
{
	public class ExpressQueryResult
	{
		public string shipperCode
		{
			get;
			set;
		}

		public string logisticsCode
		{
			get;
			set;
		}

		public bool success
		{
			get;
			set;
		}

		public string reason
		{
			get;
			set;
		}

		public string state
		{
			get;
			set;
		}

		public System.Collections.Generic.List<Trace> traces
		{
			get;
			set;
		}
	}
}
