using Hidistro.SaleSystem.Vshop;
using Quartz;
using System;

namespace Hidistro.Jobs
{
	public class OrderJob : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			MemberProcessor.GetAutoBatchOrdersIdList();
		}
	}
}
