using System;
using System.ComponentModel;

namespace Hidistro.Entities.Members
{
	public enum TradeType
	{
		[Description("提现")]
		Withdrawals,
		[Description("充值")]
		Recharge,
		[Description("佣金转入")]
		CommissionTransfer,
		[Description("在线支付")]
		Payment,
		[Description("售后退款")]
		Refund,
		[Description("订单关闭")]
		OrderClose,
		[Description("提现驳回")]
		WithdrawalsRefuse,
		[Description("商铺调整")]
		ShopAdjustment
	}
}
