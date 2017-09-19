using System;

namespace Hidistro.Messages
{
	public class WxJsonResult
	{
		public int errcode
		{
			get;
			set;
		}

		public string errmsg
		{
			get;
			set;
		}

		public object AppendData
		{
			get;
			set;
		}
	}
}
