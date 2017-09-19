using System;

namespace Hishop.Weixin.Pay.Domain
{
	public enum redPackStatus
	{
		发放中,
		已发放待领取,
		发放失败,
		已领取,
		已退款,
		异常
	}
}
