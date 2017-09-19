using System;

namespace Hidistro.Messages
{
	public class TemplateDataItem
	{
		public string color
		{
			get;
			set;
		}

		public string value
		{
			get;
			set;
		}

		public TemplateDataItem(string v, string c = "#173177")
		{
			this.value = v;
			this.color = c;
		}
	}
}
