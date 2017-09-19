using Hidistro.Core;
using Hidistro.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.Sales;
using Hidistro.SqlDal;
using Hidistro.SqlDal.Commodities;
using Hidistro.SqlDal.Members;
using Hidistro.SqlDal.Orders;
using Hidistro.SqlDal.Promotions;
using Hidistro.SqlDal.Sales;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Hidistro.SaleSystem.Vshop
{
	public static class ShoppingProcessor
	{
		private static object createOrderLocker = new object();

		private static object BalanceUpdateLock = new object();

		public static SKUItem GetProductAndSku(MemberInfo member, int productId, string options)
		{
			return new SkuDao().GetProductAndSku(member, productId, options);
		}

		public static IList<PaymentModeInfo> GetPaymentModes()
		{
			return new PaymentModeDao().GetPaymentModes();
		}

		public static PaymentModeInfo GetPaymentMode(int modeId)
		{
			return new PaymentModeDao().GetPaymentMode(modeId);
		}

		public static OrderInfo ConvertShoppingCartToOrder(ShoppingCartInfo shoppingCart, bool isCountDown, bool isSignBuy)
		{
			OrderInfo result;
			if (shoppingCart.LineItems.Count == 0)
			{
				result = null;
			}
			else
			{
				OrderInfo orderInfo = new OrderInfo();
				orderInfo.Points = shoppingCart.GetPoint();
				orderInfo.ReducedPromotionId = shoppingCart.ReducedPromotionId;
				orderInfo.ReducedPromotionName = shoppingCart.ReducedPromotionName;
				orderInfo.ReducedPromotionAmount = shoppingCart.ReducedPromotionAmount;
				orderInfo.IsReduced = shoppingCart.IsReduced;
				orderInfo.SentTimesPointPromotionId = shoppingCart.SentTimesPointPromotionId;
				orderInfo.SentTimesPointPromotionName = shoppingCart.SentTimesPointPromotionName;
				orderInfo.IsSendTimesPoint = shoppingCart.IsSendTimesPoint;
				orderInfo.TimesPoint = shoppingCart.TimesPoint;
				orderInfo.FreightFreePromotionId = shoppingCart.FreightFreePromotionId;
				orderInfo.FreightFreePromotionName = shoppingCart.FreightFreePromotionName;
				orderInfo.IsFreightFree = shoppingCart.IsFreightFree;

				string str = string.Empty;
				if (shoppingCart.LineItems.Count > 0)
				{
					foreach (ShoppingCartItemInfo current in shoppingCart.LineItems)
					{
						str += string.Format("'{0}',", current.SkuId);
					}
				}
				if (shoppingCart.LineItems.Count > 0)
				{
					foreach (ShoppingCartItemInfo current in shoppingCart.LineItems)
					{
						LineItemInfo lineItemInfo = new LineItemInfo();
						lineItemInfo.SkuId = current.SkuId;
						lineItemInfo.ProductId = current.ProductId;
						lineItemInfo.SKU = current.SKU;
						lineItemInfo.Quantity = current.Quantity;
						lineItemInfo.ShipmentQuantity = current.ShippQuantity;
						lineItemInfo.CommissionDiscount = 100;
                        //牛奶配送相关
                        lineItemInfo.SendStartDate = current.SendStartDate;
                        lineItemInfo.SendEndDate = current.SendEndDate;
                        lineItemInfo.SendDays = current.SendDays;
                        lineItemInfo.QuantityPerDay = current.QuantityPerDay;

						if (current.LimitedTimeDiscountId > 0)
						{
							bool flag = true;
							LimitedTimeDiscountInfo discountInfo = new LimitedTimeDiscountDao().GetDiscountInfo(current.LimitedTimeDiscountId);
							if (discountInfo == null)
							{
								flag = false;
							}
							else
							{
								lineItemInfo.CommissionDiscount = discountInfo.CommissionDiscount;
							}
							if (!flag)
							{
								current.LimitedTimeDiscountId = 0;
							}
						}
						lineItemInfo.ItemCostPrice = new SkuDao().GetSkuItem(current.SkuId).CostPrice;
						lineItemInfo.ItemListPrice = current.MemberPrice;
						lineItemInfo.ItemAdjustedPrice = current.AdjustedPrice;
						lineItemInfo.ItemDescription = current.Name;
						lineItemInfo.ThumbnailsUrl = current.ThumbnailUrl60;
						lineItemInfo.ItemWeight = current.Weight;
						lineItemInfo.SKUContent = current.SkuContent;
						lineItemInfo.PromotionId = current.PromotionId;
						lineItemInfo.PromotionName = current.PromotionName;
						lineItemInfo.MainCategoryPath = current.MainCategoryPath;
						lineItemInfo.Type = current.Type;
						lineItemInfo.ExchangeId = current.ExchangeId;
						lineItemInfo.PointNumber = current.PointNumber * lineItemInfo.Quantity;
						lineItemInfo.ThirdCommission = current.ThirdCommission;
						lineItemInfo.SecondCommission = current.SecondCommission;
						lineItemInfo.FirstCommission = current.FirstCommission;
						lineItemInfo.IsSetCommission = current.IsSetCommission;
						lineItemInfo.LimitedTimeDiscountId = current.LimitedTimeDiscountId;
						orderInfo.LineItems.Add(lineItemInfo.SkuId + lineItemInfo.Type + lineItemInfo.LimitedTimeDiscountId, lineItemInfo);
					}
				}
				orderInfo.Tax = 0.00m;
				orderInfo.InvoiceTitle = "";
				result = orderInfo;
			}
			return result;
		}

		public static int CreatOrder(OrderInfo orderInfo, bool isUseBalance, decimal remainingMondy)
		{
			int num = 0;
            //if (orderInfo.GetTotal() <= 0m)
            //牛奶配送相关
            if (orderInfo.GetMilkTotal() <= 0m)
            {
				orderInfo.OrderStatus = OrderStatus.BuyerAlreadyPaid;
				orderInfo.PayDate = new DateTime?(DateTime.Now);
			}
			if (orderInfo.PaymentType == null && orderInfo.PointExchange > 0)
			{
				orderInfo.PaymentType = "积分抵现";
				orderInfo.Gateway = "hishop.plugins.payment.pointtocach";
				orderInfo.PaymentTypeId = 77;
			}
			else if (orderInfo.PaymentType == null && orderInfo.RedPagerID.HasValue && orderInfo.RedPagerID.Value > 0)
			{
				orderInfo.PaymentType = "优惠券抵扣";
				orderInfo.Gateway = "hishop.plugins.payment.coupontocach";
				orderInfo.PaymentTypeId = 55;
			}
			MemberInfo currentMember = MemberProcessor.GetCurrentMember();
			Database database = DatabaseFactory.CreateDatabase();
			int quantity = orderInfo.LineItems.Sum((KeyValuePair<string, LineItemInfo> item) => item.Value.Quantity);
			int result;
			lock (ShoppingProcessor.createOrderLocker)
			{
				if (orderInfo.GroupBuyId > 0)
				{
					ShoppingProcessor.checkCanGroupBuy(quantity, orderInfo.GroupBuyId);
				}
				using (System.Data.Common.DbConnection dbConnection = database.CreateConnection())
				{
					dbConnection.Open();
					System.Data.Common.DbTransaction dbTransaction = dbConnection.BeginTransaction();
					try
					{
						orderInfo.ClientShortType = (ClientShortType)Globals.GetClientShortType();
						if (!new OrderDao().CreatOrder(orderInfo, dbTransaction))
						{
							dbTransaction.Rollback();
							result = 0;
							return result;
						}
						if (orderInfo.LineItems.Count > 0)
						{
							if (orderInfo.OrderStatus == OrderStatus.BuyerAlreadyPaid)
							{
								foreach (LineItemInfo lineItemInfo in orderInfo.LineItems.Values)
								{
									lineItemInfo.OrderItemsStatus = OrderStatus.BuyerAlreadyPaid;
								}
							}
							if (!new LineItemDao().AddOrderLineItems(orderInfo.OrderId, orderInfo.LineItems.Values, dbTransaction))
							{
								dbTransaction.Rollback();
								result = 0;
								return result;
							}
						}
						if (!string.IsNullOrEmpty(orderInfo.CouponCode))
						{
							if (!new CouponDao().AddCouponUseRecord(orderInfo, dbTransaction))
							{
								dbTransaction.Rollback();
								result = 0;
								return result;
							}
						}


                        ICollection values = orderInfo.LineItems.Values;
						foreach (LineItemInfo lineItemInfo in values)
						{
							if (lineItemInfo.Type == 1)
							{
								if (lineItemInfo.ExchangeId > 0)
								{
									PointExchangeChangedInfo pointExchangeChangedInfo = new PointExchangeChangedInfo();
									pointExchangeChangedInfo.exChangeId = lineItemInfo.ExchangeId;
									pointExchangeChangedInfo.exChangeName = new OrderDao().GetexChangeName(pointExchangeChangedInfo.exChangeId);
									pointExchangeChangedInfo.ProductId = lineItemInfo.ProductId;
									pointExchangeChangedInfo.PointNumber = lineItemInfo.PointNumber;
									pointExchangeChangedInfo.MemberID = orderInfo.UserId;
									pointExchangeChangedInfo.Date = DateTime.Now;
									pointExchangeChangedInfo.MemberGrades = currentMember.GradeId;
									if (!new OrderDao().InsertPointExchange_Changed(pointExchangeChangedInfo, dbTransaction, lineItemInfo.Quantity))
									{
										dbTransaction.Rollback();
										result = 0;
										return result;
									}
									IntegralDetailInfo integralDetailInfo = new IntegralDetailInfo();
									integralDetailInfo.IntegralChange = -lineItemInfo.PointNumber;
									integralDetailInfo.IntegralSource = "积分兑换商品-订单号：" + orderInfo.OrderMarking;
									integralDetailInfo.IntegralSourceType = 2;
									integralDetailInfo.Remark = "积分兑换商品";
									integralDetailInfo.Userid = orderInfo.UserId;
									integralDetailInfo.GoToUrl = Globals.ApplicationPath + "/Vshop/MemberOrderDetails.aspx?OrderId=" + orderInfo.OrderId;
									integralDetailInfo.IntegralStatus = Convert.ToInt32(IntegralDetailStatus.IntegralExchange);
									if (!new IntegralDetailDao().AddIntegralDetail(integralDetailInfo, dbTransaction))
									{
										dbTransaction.Rollback();
										result = 0;
										return result;
									}
								}
							}
						}
						if (orderInfo.PointExchange > 0)
						{
							IntegralDetailInfo integralDetailInfo = new IntegralDetailInfo();
							integralDetailInfo.IntegralChange = -orderInfo.PointExchange;
							integralDetailInfo.IntegralSource = "积分抵现，订单号：" + orderInfo.OrderId;
							integralDetailInfo.IntegralSourceType = 2;
							integralDetailInfo.Remark = "";
							integralDetailInfo.Userid = orderInfo.UserId;
							integralDetailInfo.GoToUrl = Globals.ApplicationPath + "/Vshop/MemberOrderDetails.aspx?OrderId=" + orderInfo.OrderId;
							integralDetailInfo.IntegralStatus = Convert.ToInt32(IntegralDetailStatus.NowArrived);
							if (!new IntegralDetailDao().AddIntegralDetail(integralDetailInfo, dbTransaction))
							{
								dbTransaction.Rollback();
								result = 0;
								return result;
							}
						}
						if (orderInfo.RedPagerID > 0)
						{
							if (!new OrderDao().UpdateCoupon_MemberCoupons(orderInfo, dbTransaction))
							{
								dbTransaction.Rollback();
								result = 0;
								return result;
							}
						}
						dbTransaction.Commit();
						num = 1;
						if (orderInfo.OrderStatus == OrderStatus.BuyerAlreadyPaid)
						{
							num = 2;
						}
					}
					catch
					{
						dbTransaction.Rollback();
						throw;
					}
					finally
					{
						dbConnection.Close();
					}
				}
			}
			if (isUseBalance && num == 1)
			{
				orderInfo = new OrderDao().GetOrderInfo(orderInfo.OrderId);
				lock (ShoppingProcessor.BalanceUpdateLock)
				{
					num = ShoppingProcessor.OrderBalanceUpdate(orderInfo, currentMember.UserId, remainingMondy);
				}
			}
			result = num;
			return result;
		}

		public static int OrderBalanceUpdate(OrderInfo orderInfo, int userid, decimal remainingMondy)
		{
			MemberInfo member = new MemberDao().GetMember(userid);
			int result;
			if (member == null)
			{
				result = 0;
			}
			else
			{
				decimal num = member.AvailableAmount;
				decimal num2 = 0m;
				ICollection values = orderInfo.LineItems.Values;
				OrderStatus orderStatus = orderInfo.OrderStatus;
				foreach (LineItemInfo lineItemInfo in values)
				{
					if (lineItemInfo.Type == 0)
					{
						decimal num3 = lineItemInfo.ItemAdjustedPrice * lineItemInfo.Quantity - lineItemInfo.DiscountAverage;
						decimal num4;
						if (num >= num3)
						{
							num -= num3;
							num4 = num3;
						}
						else
						{
							num4 = num;
							num = 0m;
						}
						if (num4 <= 0m)
						{
							break;
						}
						num2 += num4;
						lineItemInfo.BalancePayMoney = num4;
					}
				}
				if (num > 0m)
				{
					decimal num5 = orderInfo.AdjustedFreight - orderInfo.CouponFreightMoneyTotal;
					if (remainingMondy > 0m)
					{
						num5 -= remainingMondy;
						if (num5 < 0m)
						{
							num5 = 0m;
						}
					}
					decimal num6;
					if (num > num5)
					{
						num6 = num5;
					}
					else
					{
						num6 = num;
					}
					orderInfo.BalancePayFreightMoneyTotal = num6;
					num2 += num6;
				}
				if (orderInfo.GetCashPayMoney() <= 0m)
				{
					orderStatus = OrderStatus.BuyerAlreadyPaid;
				}
				foreach (LineItemInfo lineItemInfo in values)
				{
					new LineItemDao().UpdateBalancePayMoney(lineItemInfo.ID, lineItemInfo.BalancePayMoney, orderStatus, null);
				}
				int num7 = 1;
				if (orderStatus == OrderStatus.BuyerAlreadyPaid)
				{
					orderInfo.OrderStatus = OrderStatus.BuyerAlreadyPaid;
					orderInfo.PayDate = new DateTime?(DateTime.Now);
					num7 = 2;
					if (orderInfo.GetBalancePayMoneyTotal() > 0m)
					{
						orderInfo.PaymentTypeId = 66;
						orderInfo.PaymentType = "余额支付";
						orderInfo.Gateway = "hishop.plugins.payment.balancepayrequest";
					}
				}
				new OrderDao().UpdateOrder(orderInfo, null);
				if (num2 > 0m)
				{
					MemberAmountDetailedInfo memberAmountDetailedInfo = new MemberAmountDetailedInfo();
					memberAmountDetailedInfo.UserId = member.UserId;
					memberAmountDetailedInfo.UserName = member.UserName;
					memberAmountDetailedInfo.PayId = Globals.GetGenerateId();
					memberAmountDetailedInfo.TradeAmount = -num2;
					memberAmountDetailedInfo.TradeType = TradeType.Payment;
					memberAmountDetailedInfo.TradeTime = DateTime.Now;
					memberAmountDetailedInfo.TradeWays = TradeWays.Balance;
					memberAmountDetailedInfo.State = 1;
					memberAmountDetailedInfo.AvailableAmount = member.AvailableAmount + memberAmountDetailedInfo.TradeAmount;
					memberAmountDetailedInfo.Remark = "订单号：" + orderInfo.OrderId;
					new AmountDao().UseBalance(memberAmountDetailedInfo);
				}
				result = num7;
			}
			return result;
		}

		private static void checkCanGroupBuy(int quantity, int groupBuyId)
		{
			GroupBuyInfo groupBuyInfo = null;
			if (groupBuyInfo.Status != GroupBuyStatus.UnderWay)
			{
				throw new OrderException("当前团购状态不允许购买");
			}
			if (groupBuyInfo.StartDate > DateTime.Now || groupBuyInfo.EndDate < DateTime.Now)
			{
				throw new OrderException("当前不在团购时间范围内");
			}
			int num = groupBuyInfo.MaxCount - groupBuyInfo.SoldCount;
			if (quantity > num)
			{
				throw new OrderException("剩余可购买团购数量不够");
			}
		}

		public static int GetUserOrders(int userId)
		{
			return new OrderDao().GetUserOrders(userId);
		}

		public static OrderInfo GetOrderInfo(string orderId)
		{
			return new OrderDao().GetOrderInfoForLineItems(orderId);
		}

		public static List<OrderInfo> GetOrderMarkingOrderInfo(string OrderMarking, bool IsWaitPay = true)
		{
			List<OrderInfo> list = new List<OrderInfo>();
			System.Data.DataTable orderMarkingAllOrderID = new OrderDao().GetOrderMarkingAllOrderID(OrderMarking, IsWaitPay);
			for (int i = 0; i < orderMarkingAllOrderID.Rows.Count; i++)
			{
				list.Add(new OrderDao().GetOrderInfo(orderMarkingAllOrderID.Rows[i]["OrderId"].ToString()));
			}
			return list;
		}

		public static bool InsertCalculationCommission(ArrayList UserIdList, ArrayList ReferralBlanceList, string orderid, ArrayList OrdersTotalList, string userid)
		{
			return new OrderDao().InsertCalculationCommission(UserIdList, ReferralBlanceList, orderid, OrdersTotalList, userid);
		}

		public static bool UpdateOrder(OrderInfo order, System.Data.Common.DbTransaction dbTran = null)
		{
			return new OrderDao().UpdateOrder(order, dbTran);
		}

		public static string UpdateAdjustCommssions(string orderId, string itemid, decimal commssionmoney, decimal adjustcommssion)
		{
			string text = string.Empty;
			Database database = DatabaseFactory.CreateDatabase();
			string result;
			using (System.Data.Common.DbConnection dbConnection = database.CreateConnection())
			{
				dbConnection.Open();
				System.Data.Common.DbTransaction dbTransaction = dbConnection.BeginTransaction();
				try
				{
					OrderInfo orderInfo = ShoppingProcessor.GetOrderInfo(orderId);
					if (orderId == null)
					{
						result = "订单编号不合法";
						return result;
					}
					int userId = DistributorsBrower.GetCurrentDistributors(true).UserId;
					if (orderInfo.ReferralUserId != userId || orderInfo.OrderStatus != OrderStatus.WaitBuyerPay)
					{
						result = "不是您的订单";
						return result;
					}
					LineItemInfo lineItemInfo = orderInfo.LineItems[itemid];
					if (lineItemInfo == null || lineItemInfo.ItemsCommission < adjustcommssion)
					{
						if (!(lineItemInfo.ItemsCommission.ToString("F2") == adjustcommssion.ToString("F2")))
						{
							result = "修改金额过大";
							return result;
						}
						adjustcommssion = lineItemInfo.ItemsCommission;
					}
					lineItemInfo.ItemAdjustedCommssion = adjustcommssion;
					lineItemInfo.IsAdminModify = false;
					if (!new LineItemDao().UpdateLineItem(orderId, lineItemInfo, dbTransaction))
					{
						dbTransaction.Rollback();
					}
					if (!new OrderDao().UpdateOrder(orderInfo, dbTransaction))
					{
						dbTransaction.Rollback();
						result = "更新订单信息失败";
						return result;
					}
					dbTransaction.Commit();
					text = "1";
				}
				catch (Exception ex)
				{
					text = ex.ToString();
					dbTransaction.Rollback();
				}
				finally
				{
					dbConnection.Close();
				}
				result = text;
			}
			return result;
		}

		public static bool UpdateCalculadtionCommission(OrderInfo order)
		{
			return new OrderDao().UpdateCalculadtionCommission(order, null);
		}

		public static OrderInfo GetCalculadtionCommission(OrderInfo order)
		{
			return new OrderDao().GetCalculadtionCommission(order, 0);
		}

		public static CouponInfo GetCoupon(string couponCode)
		{
			return new CouponDao().GetCouponDetails(int.Parse(couponCode));
		}

		public static System.Data.DataTable GetCoupon(decimal orderAmount)
		{
			return null;
		}

		public static CouponInfo UseCoupon(decimal orderAmount, string claimCode)
		{
			CouponInfo result;
			if (string.IsNullOrEmpty(claimCode))
			{
				result = null;
			}
			else
			{
				CouponInfo coupon = ShoppingProcessor.GetCoupon(claimCode);
				if (coupon.ConditionValue <= orderAmount)
				{
					result = coupon;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static decimal CalcFreight(int regionId, decimal totalWeight, ShippingModeInfo shippingModeInfo)
		{
			decimal result = 0m;
			int topRegionId = RegionHelper.GetTopRegionId(regionId);
			int value = 1;
			if (totalWeight > shippingModeInfo.Weight && shippingModeInfo.AddWeight.HasValue && shippingModeInfo.AddWeight.Value > 0m)
			{
				if ((totalWeight - shippingModeInfo.Weight) % shippingModeInfo.AddWeight == 0m)
				{
					value = Convert.ToInt32(Math.Truncate((totalWeight - shippingModeInfo.Weight) / shippingModeInfo.AddWeight.Value));
				}
				else
				{
					value = Convert.ToInt32(Math.Truncate((totalWeight - shippingModeInfo.Weight) / shippingModeInfo.AddWeight.Value)) + 1;
				}
			}
			if (shippingModeInfo.ModeGroup == null || shippingModeInfo.ModeGroup.Count == 0)
			{
				if (totalWeight > shippingModeInfo.Weight && shippingModeInfo.AddPrice.HasValue)
				{
					result = value * shippingModeInfo.AddPrice.Value + shippingModeInfo.Price;
				}
				else
				{
					result = shippingModeInfo.Price;
				}
			}
			else
			{
				int? num = null;
				foreach (ShippingModeGroupInfo current in shippingModeInfo.ModeGroup)
				{
					foreach (ShippingRegionInfo current2 in current.ModeRegions)
					{
						if (topRegionId == current2.RegionId)
						{
							num = new int?(current2.GroupId);
							break;
						}
					}
					if (num.HasValue)
					{
						if (totalWeight > shippingModeInfo.Weight)
						{
							result = value * current.AddPrice + current.Price;
						}
						else
						{
							result = current.Price;
						}
						break;
					}
				}
				if (!num.HasValue)
				{
					if (totalWeight > shippingModeInfo.Weight && shippingModeInfo.AddPrice.HasValue)
					{
						result = value * shippingModeInfo.AddPrice.Value + shippingModeInfo.Price;
					}
					else
					{
						result = shippingModeInfo.Price;
					}
				}
			}
			return result;
		}

		public static decimal CalcPayCharge(decimal cartMoney, PaymentModeInfo paymentModeInfo)
		{
			decimal result;
			if (!paymentModeInfo.IsPercent)
			{
				result = paymentModeInfo.Charge;
			}
			else
			{
				result = cartMoney * (paymentModeInfo.Charge / 100m);
			}
			return result;
		}

		public static bool InsertOrderRefund(RefundInfo refundInfo)
		{
			return new RefundDao().InsertOrderRefund(refundInfo);
		}

		public static bool UpdateOrderGoodStatu(string orderid, string skuid, int OrderItemsStatus, int itemid)
		{
			return new RefundDao().UpdateOrderGoodStatu(orderid, skuid, OrderItemsStatus, itemid);
		}

		public static bool GetReturnMes(int userid, string OrderId, int ProductId, int HandleStatus)
		{
			return new RefundDao().GetReturnMes(userid, OrderId, ProductId, HandleStatus);
		}

		public static bool GetReturnInfo(int userid, string OrderId, int ProductId, string SkuID)
		{
			return new RefundDao().GetReturnInfo(userid, OrderId, ProductId, SkuID);
		}

		public static System.Data.DataTable GetOrderReturnTable(int userid, string ReturnsId, int type)
		{
			return new RefundDao().GetOrderReturnTable(userid, ReturnsId, type);
		}

		public static bool CombineOrderToPay(string orderIds, string orderMarking)
		{
			return new OrderDao().CombineOrderToPay(orderIds, orderMarking);
		}
	}
}
