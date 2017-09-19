using System;
using System.ComponentModel;

namespace Hidistro.Entities.Orders
{
	public enum OrderStatus
	{
		[Description("所有订单|Trade_ALL")]
		All,
		[Description("等待买家付款|WAIT_BUYER_PAY")]
		WaitBuyerPay,
		[Description("等待发货|WAIT_SELLER_SEND_GOODS")]
		BuyerAlreadyPaid,
		[Description("已发货|WAIT_BUYER_CONFIRM_GOODS")]
		SellerAlreadySent,
		[Description("交易关闭|TRADE_CLOSED")]
		Closed,
		[Description("交易完成|TRADE_FINISHED")]
		Finished,
		[Description("申请退款|TRADE_APPLY_FOR_REFUND")]
		ApplyForRefund,
		[Description("申请退货|TRADE_APPLY_FOR_RETURN")]
		ApplyForReturns,
		[Description("申请换货|TRADE_APPLY_FOR_REPLACE")]
		ApplyForReplacement,
		[Description("已退款|TRADE_REFUND_FINISHED")]
		Refunded,
		[Description("已退货|TRADE_RETURNED_FINISHED")]
		Returned,
		[Description("今日订单|TRADE_HISTORY")]
		Today,
		[Description("已删除的订单|TRADE_HISTORY")]
		Deleted,
		[Description("历史订单|TRADE_HISTORY")]
		History = 99
	}
}
