using System;

namespace Hidistro.Entities.Members
{
	public enum RequesState
	{
		None = -2,
		未审核 = 0,
		已审核,
		已发放,
		驳回 = -1,
		发放异常 = 3
	}
}
