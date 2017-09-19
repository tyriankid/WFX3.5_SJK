using Hidistro.Entities.StatisticsReport;
using System;
using System.Threading;

namespace Hidistro.ControlPanel.VShop
{
	public class StatisticNotifier
	{
		public string actionDesc = "";

		public UpdateAction updateAction;

		public DateTime RecDateUpdate;

		public StatisticNotifier()
		{
		}

		public virtual void OnDataUpdated(StatisticNotifier.DataUpdatedEventArgs e)
		{
			if (this.DataUpdated != null)
			{
				this.DataUpdated(this, e);
			}
		}

		public void UpdateDB()
		{
			this.OnDataUpdated(new StatisticNotifier.DataUpdatedEventArgs());
		}

		public event StatisticNotifier.DataUpdatedEventHandler DataUpdated;

		public class DataUpdatedEventArgs : EventArgs
		{
			public readonly int temperature;

			public DataUpdatedEventArgs()
			{
			}
		}

		public delegate void DataUpdatedEventHandler(object sender, StatisticNotifier.DataUpdatedEventArgs e);
	}
}