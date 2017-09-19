using System;

namespace Hidistro.Entities.Orders
{
	public class LineItemInfo
	{
		public int ID
		{
			get;
			set;
		}

		public string SkuId
		{
			get;
			set;
		}

		public int ProductId
		{
			get;
			set;
		}

		public string SKU
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public int ShipmentQuantity
		{
			get;
			set;
		}

		public decimal ItemCostPrice
		{
			get;
			set;
		}

		public decimal ItemListPrice
		{
			get;
			set;
		}

		public decimal ItemAdjustedPrice
		{
			get;
			set;
		}

		public string ItemDescription
		{
			get;
			set;
		}

		public string ThumbnailsUrl
		{
			get;
			set;
		}

		public decimal ItemWeight
		{
			get;
			set;
		}

		public string SKUContent
		{
			get;
			set;
		}

		public int PromotionId
		{
			get;
			set;
		}

		public string PromotionName
		{
			get;
			set;
		}

		public decimal ReturnMoney
		{
			get;
			set;
		}

		public string MainCategoryPath
		{
			get;
			set;
		}

		public OrderStatus OrderItemsStatus
		{
			get;
			set;
		}

		public decimal ItemsCommission
		{
			get;
			set;
		}

		public decimal SecondItemsCommission
		{
			get;
			set;
		}

		public decimal ThirdItemsCommission
		{
			get;
			set;
		}

		public decimal ItemsCommissionScale
		{
			get;
			set;
		}

		public decimal SecondItemsCommissionScale
		{
			get;
			set;
		}

		public decimal ThirdItemsCommissionScale
		{
			get;
			set;
		}

		public decimal ItemAdjustedCommssion
		{
			get;
			set;
		}

		public decimal ThirdCommission
		{
			get;
			set;
		}

		public decimal SecondCommission
		{
			get;
			set;
		}

		public decimal FirstCommission
		{
			get;
			set;
		}

		public bool IsSetCommission
		{
			get;
			set;
		}

		public int PointNumber
		{
			get;
			set;
		}

		public int ExchangeId
		{
			get;
			set;
		}

		public int Type
		{
			get;
			set;
		}

		public decimal DiscountAverage
		{
			get;
			set;
		}

		public string OrderID
		{
			get;
			set;
		}

		public decimal BalancePayMoney
		{
			get;
			set;
		}

		public bool IsAdminModify
		{
			get;
			set;
		}

		public int LimitedTimeDiscountId
		{
			get;
			set;
		}

		public OrderStatus OrderStatus
		{
			get;
			set;
		}

		public int CommissionDiscount
		{
			get;
			set;
		}

		public decimal GetSubTotal()
		{
			return this.ItemAdjustedPrice * this.Quantity;
		}

        /// <summary>
        /// 牛奶配送总价
        /// </summary>
        public decimal MilkSubTotal()
        {
                return this.QuantityPerDay * this.SendDays * this.ItemAdjustedPrice;
        }

        /// <summary>
        /// 配送开始日期
        /// </summary>
        public DateTime SendStartDate
        {
            get;
            set;
        }
        /// <summary>
        /// 配送结束日期
        /// </summary>
        public DateTime SendEndDate
        {
            get;
            set;
        }
        /// <summary>
        /// 每天配送件数
        /// </summary>
        public int QuantityPerDay
        {
            get;
            set;
        }
        /// <summary>
        /// 配送天数
        /// </summary>
        public int SendDays
        {
            get;
            set;
        }
    }
}
