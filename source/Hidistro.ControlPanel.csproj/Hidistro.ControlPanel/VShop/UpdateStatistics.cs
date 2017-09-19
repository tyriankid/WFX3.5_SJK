using System;

namespace Hidistro.ControlPanel.VShop
{
	public class UpdateStatistics
	{
		public UpdateStatistics()
		{
		}

		public void Update(object sender, StatisticNotifier.DataUpdatedEventArgs e)
		{
			StatisticNotifier statisticNotifier = (StatisticNotifier)sender;
			string str = "";
			try
			{
				ShopStatisticHelper.StatisticsOrdersByNotify(statisticNotifier.RecDateUpdate, statisticNotifier.updateAction, statisticNotifier.actionDesc, out str);
			}
			catch (Exception exception)
			{
			}
		}
	}
}