using System;

namespace Hidistro.Entities.Members
{
	public class MemberAmountDetailedInfo
	{
		public int Id
		{
			get;
			set;
		}

		public int UserId
		{
			get;
			set;
		}

		public string PayId
		{
			get;
			set;
		}

		public decimal TradeAmount
		{
			get;
			set;
		}

		public decimal AvailableAmount
		{
			get;
			set;
		}

		public TradeType TradeType
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public TradeWays TradeWays
		{
			get;
			set;
		}

		public System.DateTime TradeTime
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public int State
		{
			get;
			set;
		}

		public string GatewayPayId
		{
			get;
			set;
		}
	}
}
