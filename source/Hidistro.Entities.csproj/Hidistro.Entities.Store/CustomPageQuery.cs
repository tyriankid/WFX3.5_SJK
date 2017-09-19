using Hidistro.Core.Entities;
using System;

namespace Hidistro.Entities.Store
{
	public class CustomPageQuery : Pagination
	{
		public string Name
		{
			get;
			set;
		}

		public int? Status
		{
			get;
			set;
		}
	}
}
