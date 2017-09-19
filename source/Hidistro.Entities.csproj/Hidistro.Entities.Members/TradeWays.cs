using System;
using System.ComponentModel;

namespace Hidistro.Entities.Members
{
	public enum TradeWays
	{
		[Description("微信钱包")]
		WeChatWallet,
		[Description("支付宝")]
		Alipay,
		[Description("店铺佣金")]
		ShopCommission,
		[Description("余额")]
		Balance,
		[Description("盛付通")]
		ShengFutong,
		[Description("线下转账")]
		LineTransfer
	}
}
