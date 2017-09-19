using System;

namespace Hidistro.Messages
{
	public class TempleteModel
	{
		public string touser
		{
			get;
			set;
		}

		public string template_id
		{
			get;
			set;
		}

		public string topcolor
		{
			get;
			set;
		}

		public object data
		{
			get;
			set;
		}

		public string url
		{
			get;
			set;
		}

		public TempleteModel()
		{
			this.topcolor = "#FF0000";
		}
	}
}
