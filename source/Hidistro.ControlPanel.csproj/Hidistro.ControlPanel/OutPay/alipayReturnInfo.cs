using System;
using System.Runtime.CompilerServices;

namespace Hidistro.ControlPanel.OutPay
{
	public class alipayReturnInfo
	{
		public string alipaynum
		{
			get;
			set;
		}

		public decimal refundNum
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public alipayReturnInfo()
		{
		}
	}
}