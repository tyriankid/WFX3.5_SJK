using Hidistro.Core;
using Hidistro.SaleSystem.Vshop;
using Quartz;
using System;

namespace Hidistro.Jobs
{
	public class RedPachCheckJob : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			try
			{
				RedPackHelp.RedPackCheckJob();
			}
			catch (Exception ex)
			{
				Globals.Debuglog("RedPachCheckJob任务出错了:" + ex.Message, "_Debuglog.txt");
			}
		}
	}
}
