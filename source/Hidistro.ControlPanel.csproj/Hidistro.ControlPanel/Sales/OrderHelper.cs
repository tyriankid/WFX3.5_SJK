using Hidistro.ControlPanel.Commodities;
using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.StatisticsReport;
using Hidistro.Entities.Store;
using Hidistro.Messages;
using Hidistro.SaleSystem.Vshop;
using Hidistro.SqlDal.Commodities;
using Hidistro.SqlDal.Members;
using Hidistro.SqlDal.Orders;
using Hidistro.SqlDal.VShop;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Hidistro.ControlPanel.Sales
{
    public static class OrderHelper
    {
        public static bool MemberAmountAddByRefund(MemberInfo memberInfo, decimal amount, string orderid)
        {
            return new AmountDao().MemberAmountAddByRefund(memberInfo, amount, orderid);
        }

        public static System.Data.DataSet GetTradeOrders(OrderQuery query, out int records)
        {
            return new OrderDao().GetTradeOrders(query, 0, out records);
        }

        public static OrderInfo GetOrderInfo(string orderId)
        {
            return new OrderDao().GetOrderInfo(orderId);
        }

        public static void UpdateOrderItemBalance(string orderid)
        {
            new OrderDao().UpdateOrderItemBalance(orderid);
        }

        public static bool ExistsOrderByBargainDetialId(int userId, int bargainDetialId)
        {
            return new OrderDao().ExistsOrderByBargainDetialId(userId, bargainDetialId);
        }

        public static System.Data.DataTable GetUserOrderPaidWaitFinish(int userId)
        {
            return new OrderDao().GetUserOrderPaidWaitFinish(userId);
        }

        public static bool UpdateCalculadtionCommission(string orderid)
        {
            OrderInfo orderInfo = OrderHelper.GetOrderInfo(orderid);
            orderInfo = OrderHelper.GetCalculadtionCommission(orderInfo);
            new OrderDao().UpdateOrder(orderInfo, null);
            return new OrderDao().UpdateCalculadtionCommission(orderInfo, null);
        }

        public static bool UpdateOrder(OrderInfo order)
        {
            bool flag = new OrderDao().UpdateOrder(order, null);
            if (flag && order.OrderStatus == OrderStatus.Closed)
            {
                new OrderDao().UpdateItemsStatus(order.OrderId, 4, "all");
            }
            return flag;
        }

        public static System.Data.DataSet GetOrdersByOrderIDList(string orderIds)
        {
            return new OrderDao().GetOrdersByOrderIDList(orderIds);
        }

        public static DbQueryResult GetOrders(OrderQuery query)
        {
            return new OrderDao().GetOrders(query);
        }

        public static string GetOrderCountInfo(OrderQuery query)
        {
            return new OrderDao().GetOrderCountInfo(query);
        }
        
        public static DbQueryResult GetMemberDetailOrders(MemberDetailOrderQuery query)
        {
            return new OrderDao().GetMemberDetailOrders(query);
        }

        public static DbQueryResult GetDeleteOrders(OrderQuery query)
        {
            return new OrderDao().GetDeleteOrders(query);
        }

        public static void SetOrderShipNumber(string[] orderIds, string startNumber, string ExpressCom = "")
        {
            string text = startNumber;
            OrderDao orderDao = new OrderDao();
            for (int i = 0; i < orderIds.Length; i++)
            {
                if (i != 0)
                {
                    text = OrderHelper.GetNextExpress(ExpressCom, text);
                }
                else
                {
                    OrderHelper.GetNextExpress(ExpressCom, text);
                }
                orderDao.EditOrderShipNumber(orderIds[i], text);
            }
        }

        public static bool EditOrderShipNumber(string orderid, string shipnumber)
        {
            return new OrderDao().EditOrderShipNumber(orderid, shipnumber);
        }

        public static OrderInfo GetCalculadtionCommission(OrderInfo order)
        {
            return new OrderDao().GetCalculadtionCommission(order, 1);
        }

        private static string GetNextExpress(string ExpressCom, string strno)
        {
            string text = ExpressCom.ToLower();
            string result;
            if (text != null)
            {
                if (text == "ems")
                {
                    result = OrderHelper.getEMSNext(strno);
                    return result;
                }
                if (text == "顺丰快递" || text == "shunfeng")
                {
                    result = OrderHelper.getSFNext(strno);
                    return result;
                }
                if (text == "宅急送" || text == "zhaijisong")
                {
                    result = OrderHelper.getZJSNext(strno);
                    return result;
                }
            }
            result = (long.Parse(strno) + 1L).ToString();
            return result;
        }

        private static string getSFNext(string sfno)
        {
            int[] array = new int[12];
            int[] array2 = new int[12];
            List<char> list = sfno.ToList<char>();
            string value = sfno.Substring(0, 11);
            string text = string.Empty;
            if (sfno.Substring(0, 1) == "0")
            {
                text = "0" + (Convert.ToInt64(value) + 1L).ToString();
            }
            else
            {
                text = (Convert.ToInt64(value) + 1L).ToString();
            }
            for (int i = 0; i < 12; i++)
            {
                array[i] = int.Parse(list[i].ToString());
            }
            List<char> list2 = text.ToList<char>();
            for (int i = 0; i < 11; i++)
            {
                array2[i] = int.Parse(text[i].ToString());
            }
            if (array2[8] - array[8] == 1 && array[8] % 2 == 1)
            {
                if (array[11] - 8 >= 0)
                {
                    array2[11] = array[11] - 8;
                }
                else
                {
                    array2[11] = array[11] - 8 + 10;
                }
            }
            else if (array2[8] - array[8] == 1 && array[8] % 2 == 0)
            {
                if (array[11] - 7 >= 0)
                {
                    array2[11] = array[11] - 7;
                }
                else
                {
                    array2[11] = array[11] - 7 + 10;
                }
            }
            else if ((array[9] == 3 || array[9] == 6) && array[10] == 9)
            {
                if (array[11] - 5 >= 0)
                {
                    array2[11] = array[11] - 5;
                }
                else
                {
                    array2[11] = array[11] - 5 + 10;
                }
            }
            else if (array[10] == 9)
            {
                if (array[11] - 4 >= 0)
                {
                    array2[11] = array[11] - 4;
                }
                else
                {
                    array2[11] = array[11] - 4 + 10;
                }
            }
            else if (array[11] - 1 >= 0)
            {
                array2[11] = array[11] - 1;
            }
            else
            {
                array2[11] = array[11] - 1 + 10;
            }
            return text + array2[11].ToString();
        }

        private static string getEMSNext(string emsno)
        {
            long num = Convert.ToInt64(emsno.Substring(2, 8));
            if (num < 99999999L)
            {
                num += 1L;
            }
            string str = num.ToString().PadLeft(8, '0');
            string emsno2 = emsno.Substring(0, 2) + str + emsno.Substring(10, 1);
            return emsno.Substring(0, 2) + str + OrderHelper.getEMSLastNum(emsno2) + emsno.Substring(11, 2);
        }

        private static string getEMSLastNum(string emsno)
        {
            List<char> list = emsno.ToList<char>();
            int num = int.Parse(list[2].ToString()) * 8;
            num += int.Parse(list[3].ToString()) * 6;
            num += int.Parse(list[4].ToString()) * 4;
            num += int.Parse(list[5].ToString()) * 2;
            num += int.Parse(list[6].ToString()) * 3;
            num += int.Parse(list[7].ToString()) * 5;
            num += int.Parse(list[8].ToString()) * 9;
            num += int.Parse(list[9].ToString()) * 7;
            num = 11 - num % 11;
            if (num == 10)
            {
                num = 0;
            }
            else if (num == 11)
            {
                num = 5;
            }
            return num.ToString();
        }

        private static string getZJSNext(string zjsno)
        {
            long num = Convert.ToInt64(zjsno) + 11L;
            if (num % 10L > 6L)
            {
                num -= 7L;
            }
            return num.ToString().PadLeft(zjsno.Length, '0');
        }

        public static bool SetOrderShipNumber(string orderId, string startNumber)
        {
            OrderInfo orderInfo = new OrderDao().GetOrderInfo(orderId);
            orderInfo.ShipOrderNumber = startNumber;
            return new OrderDao().UpdateOrder(orderInfo, null);
        }

        public static System.Data.DataTable GetSendGoodsOrders(string orderIds)
        {
            return new OrderDao().GetSendGoodsOrders(orderIds);
        }

        public static int DeleteOrders(string orderIds)
        {
            ManagerHelper.CheckPrivilege(Privilege.DeleteOrder);
            int num = new OrderDao().DeleteOrders(orderIds);
            if (num > 0)
            {
                EventLogs.WriteOperationLog(Privilege.DeleteOrder, string.Format(CultureInfo.InvariantCulture, "删除了编号为\"{0}\"的订单", new object[]
				{
					orderIds
				}));
            }
            return num;
        }

        public static int RealDeleteOrders(string orderIds)
        {
            ManagerHelper.CheckPrivilege(Privilege.DeleteOrder);
            int num = new OrderDao().RealDeleteOrders(orderIds);
            if (num > 0)
            {
                EventLogs.WriteOperationLog(Privilege.DeleteOrder, string.Format(CultureInfo.InvariantCulture, "删除了编号为\"{0}\"的订单", new object[]
				{
					orderIds
				}));
            }
            return num;
        }

        public static int RealDeleteOrders(string orderIds, DateTime? orderDate)
        {
            ManagerHelper.CheckPrivilege(Privilege.DeleteOrder);
            int num = new OrderDao().RealDeleteOrders(orderIds);
            if (num > 0)
            {
                string text = "";
                bool flag = new ShopStatisticDao().StatisticsOrdersByRecDate(orderDate.Value, UpdateAction.AllUpdate, 0, out text);
                EventLogs.WriteOperationLog(Privilege.DeleteOrder, string.Format(CultureInfo.InvariantCulture, "删除了编号为\"{0}\"的订单", new object[]
				{
					orderIds
				}));
            }
            return num;
        }

        public static int RestoreOrders(string orderIds)
        {
            ManagerHelper.CheckPrivilege(Privilege.DeleteOrder);
            int num = new OrderDao().RestoreOrders(orderIds);
            if (num > 0)
            {
                EventLogs.WriteOperationLog(Privilege.DeleteOrder, string.Format(CultureInfo.InvariantCulture, "还原了编号为\"{0}\"的订单", new object[]
				{
					orderIds
				}));
            }
            return num;
        }

        public static bool CloseTransaction(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.EditOrders);
            bool result;
            if (order.CheckAction(OrderActions.SELLER_CLOSE))
            {
                order.OrderStatus = OrderStatus.Closed;
                bool flag = new OrderDao().UpdateOrder(order, null);
                if (flag)
                {
                    new OrderDao().UpdateItemsStatus(order.OrderId, 4, "all");
                    Point.SetPointAndBalanceByOrderId(order);
                    EventLogs.WriteOperationLog(Privilege.EditOrders, string.Format(CultureInfo.InvariantCulture, "关闭了订单“{0}”", new object[]
					{
						order.OrderId
					}));
                }
                result = flag;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static bool UpdateOrderPaymentType(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.EditOrders);
            bool result;
            if (order.CheckAction(OrderActions.MASTER_SELLER_MODIFY_PAYMENT_MODE))
            {
                bool flag = new OrderDao().UpdateOrder(order, null);
                if (flag)
                {
                    EventLogs.WriteOperationLog(Privilege.EditOrders, string.Format(CultureInfo.InvariantCulture, "修改了订单“{0}”的支付方式", new object[]
					{
						order.OrderId
					}));
                }
                result = flag;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static bool MondifyAddress(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.EditOrders);
            bool result;
            if (order.CheckAction(OrderActions.MASTER_SELLER_MODIFY_DELIVER_ADDRESS))
            {
                bool flag = new OrderDao().UpdateOrder(order, null);
                if (flag)
                {
                    EventLogs.WriteOperationLog(Privilege.EditOrders, string.Format(CultureInfo.InvariantCulture, "修改了订单“{0}”的收货地址", new object[]
					{
						order.OrderId
					}));
                }
                result = flag;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static bool SaveRemark(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.RemarkOrder);
            bool flag = new OrderDao().UpdateOrder(order, null);
            if (flag)
            {
                EventLogs.WriteOperationLog(Privilege.RemarkOrder, string.Format(CultureInfo.InvariantCulture, "对订单“{0}”进行了备注", new object[]
				{
					order.OrderId
				}));
            }
            return flag;
        }

        public static bool SetOrderShippingMode(string orderIds, int realShippingModeId, string realModeName)
        {
            return new OrderDao().SetOrderShippingMode(orderIds, realShippingModeId, realModeName);
        }

        public static bool SetPrintOrderExpress(string orderId, string expressCompanyName, string expressCompanyAbb, string shipOrderNumber)
        {
            return new OrderDao().SetPrintOrderExpress(orderId, expressCompanyName, expressCompanyAbb, shipOrderNumber);
        }

        public static bool SetOrderExpressComputerpe(string purchaseOrderIds, string expressCompanyName, string expressCompanyAbb)
        {
            return new OrderDao().SetOrderExpressComputerpe(purchaseOrderIds, expressCompanyName, expressCompanyAbb);
        }

        public static DataTable GetQuestList(string where)
        {
            return new OrderDao().GetQuestList(where);
        }

        public static bool CreatMilkSendQuest(OrderInfo info)
        {
            return new OrderDao().CreatMilkSendQuest(info);
        }

        public static bool ConfirmPay(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.CofimOrderPay);
            bool flag = false;
            bool milkQuestFlag = false;
            if (order.CheckAction(OrderActions.SELLER_CONFIRM_PAY))
            {
                OrderDao orderDao = new OrderDao();
                order.OrderStatus = OrderStatus.BuyerAlreadyPaid;
                order.PayDate = new DateTime?(DateTime.Now);
                order.PaymentTypeId = 99;
                order.PaymentType = "线下支付";
                ManagerInfo currentManager = ManagerHelper.GetCurrentManager();
                string str = string.Empty;
                if (currentManager != null)
                {
                    str = string.Concat(new object[]
					{
						currentManager.UserName,
						"(",
						currentManager.UserId,
						")"
					});
                }
                order.ManagerRemark = "管理员" + str + "后台收款";
                order.Gateway = "hishop.plugins.payment.offlinerequest";
                //更新订单状态
                flag = orderDao.UpdateOrder(order, null);
                //生成牛奶配送任务
                milkQuestFlag = CreatMilkSendQuest(order);

                string text = "";
                if (flag && milkQuestFlag)
                {
                    orderDao.UpdatePayOrderStock(order);
                    foreach (LineItemInfo current in order.LineItems.Values)
                    {
                        ProductDao productDao = new ProductDao();
                        text = text + "'" + current.SkuId + "',";
                        ProductInfo productDetails = productDao.GetProductDetails(current.ProductId);
                        productDetails.SaleCounts += current.Quantity;
                        productDetails.ShowSaleCounts += current.Quantity;
                        productDao.UpdateProduct(productDetails, null);
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        orderDao.UpdateItemsStatus(order.OrderId, 2, text.Substring(0, text.Length - 1));
                    }
                    if (!string.IsNullOrEmpty(order.ActivitiesId))
                    {
                        ActivitiesDao activitiesDao = new ActivitiesDao();
                        activitiesDao.UpdateActivitiesTakeEffect(order.ActivitiesId);
                    }
                    MemberHelper.SetOrderDate(order.UserId, 1);
                    SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                    MemberInfo member = new MemberDao().GetMember(order.UserId);
                    bool flag2 = VshopBrowser.IsPassAutoToDistributor(member, true);
                    if (flag2)
                    {
                        DistributorsBrower.MemberAutoToDistributor(member);
                    }
                    try
                    {
                        if (order != null)
                        {
                            Messenger.SendWeiXinMsg_OrderPay(order);
                        }
                    }
                    catch (Exception var_13_24F)
                    {
                    }
                    EventLogs.WriteOperationLog(Privilege.CofimOrderPay, string.Format(CultureInfo.InvariantCulture, "确认收款编号为\"{0}\"的订单", new object[]
					{
						order.OrderId
					}));
                }
            }
            return flag;
        }

        public static bool ConfirmOrderFinish(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.EditOrders);
            bool flag = false;
            if (order.CheckAction(OrderActions.SELLER_FINISH_TRADE))
            {
                DateTime now = DateTime.Now;
                order.OrderStatus = OrderStatus.Finished;
                order.FinishDate = new DateTime?(now);
                if (!order.PayDate.HasValue)
                {
                    order.PayDate = new DateTime?(now);
                }
                flag = new OrderDao().UpdateOrder(order, null);
                if (flag)
                {
                    EventLogs.WriteOperationLog(Privilege.EditOrders, string.Format(CultureInfo.InvariantCulture, "完成编号为\"{0}\"的订单", new object[]
					{
						order.OrderId
					}));
                }
            }
            if (flag)
            {
                DistributorsBrower.UpdateCalculationCommission(order);
                foreach (LineItemInfo current in order.LineItems.Values)
                {
                    if (current.OrderItemsStatus.ToString() == OrderStatus.SellerAlreadySent.ToString())
                    {
                        RefundHelper.UpdateOrderGoodStatu(order.OrderId, current.SkuId, 5, current.ID);
                    }
                }
            }
            return flag;
        }

        public static bool UpdateOrderCompany(string orderId, string companycode, string companyname, string shipNumber)
        {
            return new OrderDao().UpdateOrderCompany(orderId, companycode, companyname, shipNumber);
        }

        public static bool SendGoods(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.OrderSendGoods);
            bool flag = false;
            if (order.CheckAction(OrderActions.SELLER_SEND_GOODS))
            {
                OrderDao orderDao = new OrderDao();
                order.OrderStatus = OrderStatus.SellerAlreadySent;
                order.ShippingDate = new DateTime?(DateTime.Now);
                flag = orderDao.UpdateOrder(order, null);
                string text = "";
                if (flag)
                {
                    bool flag2 = false;
                    foreach (LineItemInfo current in order.LineItems.Values)
                    {
                        OrderStatus orderItemsStatus = current.OrderItemsStatus;
                        switch (orderItemsStatus)
                        {
                            case OrderStatus.WaitBuyerPay:
                            case OrderStatus.BuyerAlreadyPaid:
                                text = text + "'" + current.SkuId + "',";
                                break;
                            default:
                                if (orderItemsStatus == OrderStatus.ApplyForRefund)
                                {
                                    flag2 = true;
                                    text = text + "'" + current.SkuId + "',";
                                }
                                break;
                        }
                    }
                    if (flag2)
                    {
                        orderDao.DeleteReturnRecordForSendGoods(order.OrderId);
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        orderDao.UpdateItemsStatus(order.OrderId, 3, text.Substring(0, text.Length - 1));
                    }
                    bool flag3 = true;
                    foreach (LineItemInfo current in order.LineItems.Values)
                    {
                        if (current.Type == 0)
                        {
                            flag3 = false;
                            break;
                        }
                    }
                    if (order.Gateway == null || order.Gateway.ToLower() == "hishop.plugins.payment.podrequest" || flag3)
                    {
                        orderDao.UpdatePayOrderStock(order);
                        foreach (LineItemInfo current in order.LineItems.Values)
                        {
                            text = text + current.SkuId + ",";
                            ProductDao productDao = new ProductDao();
                            ProductInfo productDetails = productDao.GetProductDetails(current.ProductId);
                            productDetails.SaleCounts += current.Quantity;
                            productDetails.ShowSaleCounts += current.Quantity;
                            productDao.UpdateProduct(productDetails, null);
                        }
                    }
                    MemberInfo member = MemberHelper.GetMember(order.UserId);
                    try
                    {
                        if (order != null)
                        {
                            Messenger.SendWeiXinMsg_OrderDeliver(order);
                        }
                    }
                    catch (Exception var_10_29B)
                    {
                    }
                    EventLogs.WriteOperationLog(Privilege.OrderSendGoods, string.Format(CultureInfo.InvariantCulture, "发货编号为\"{0}\"的订单", new object[]
					{
						order.OrderId
					}));
                }
            }
            return flag;
        }

        public static bool UpdateOrderAmount(OrderInfo order)
        {
            ManagerHelper.CheckPrivilege(Privilege.EditOrders);
            bool flag = false;
            if (order.CheckAction(OrderActions.SELLER_MODIFY_TRADE))
            {
                flag = new OrderDao().UpdateOrder(order, null);
                if (flag)
                {
                    EventLogs.WriteOperationLog(Privilege.EditOrders, string.Format(CultureInfo.InvariantCulture, "修改了编号为\"{0}\"订单的金额", new object[]
					{
						order.OrderId
					}));
                }
            }
            return flag;
        }

        public static bool UpdateOrderItems(OrderInfo order)
        {
            return new OrderDao().UpdateOrderItems(order);
        }

        public static int GetSkuStock(string skuId)
        {
            return new SkuDao().GetSkuItem(skuId).Stock;
        }

        public static System.Data.DataSet GetOrdersAndLines(string orderIds)
        {
            return new OrderDao().GetOrdersAndLines(orderIds);
        }

        public static System.Data.DataSet GetOrderGoods(string orderIds)
        {
            return new OrderDao().GetOrderGoods(orderIds);
        }

        public static System.Data.DataSet GetProductGoods(string orderIds)
        {
            return new OrderDao().GetProductGoods(orderIds);
        }

        public static bool SaveDebitNote(DebitNoteInfo note)
        {
            return new DebitNoteDao().SaveDebitNote(note);
        }

        public static DbQueryResult GetAllDebitNote(DebitNoteQuery query)
        {
            return new DebitNoteDao().GetAllDebitNote(query);
        }

        public static bool DelDebitNote(string[] noteIds, out int count)
        {
            ManagerHelper.CheckPrivilege(Privilege.DeleteOrder);
            bool flag = true;
            count = 0;
            for (int i = 0; i < noteIds.Length; i++)
            {
                string text = noteIds[i];
                if (!string.IsNullOrEmpty(text))
                {
                    flag &= new DebitNoteDao().DelDebitNote(text);
                    if (flag)
                    {
                        count++;
                    }
                }
            }
            return flag;
        }

        public static bool SaveSendNote(SendNoteInfo note)
        {
            return new SendNoteDao().SaveSendNote(note);
        }

        public static DbQueryResult GetAllSendNote(RefundApplyQuery query)
        {
            return new SendNoteDao().GetAllSendNote(query);
        }

        public static bool DelSendNote(string[] noteIds, out int count)
        {
            ManagerHelper.CheckPrivilege(Privilege.DeleteOrder);
            bool flag = true;
            count = 0;
            for (int i = 0; i < noteIds.Length; i++)
            {
                string text = noteIds[i];
                if (!string.IsNullOrEmpty(text))
                {
                    flag &= new SendNoteDao().DelSendNote(text);
                    if (flag)
                    {
                        count++;
                    }
                }
            }
            return flag;
        }

        public static System.Data.DataTable GetAllOrderID(int siteid)
        {
            return new OrderDao().GetAllOrderID(siteid);
        }

        public static int GetCountOrderIDByStatus(OrderStatus? orderstatus, OrderStatus? itemstatus)
        {
            return new OrderDao().GetCountOrderIDByStatus(orderstatus, itemstatus);
        }

        public static bool UpdateAdjustCommssions(string orderId, string itemid, decimal adjustcommssion)
        {
            bool flag = false;
            Database database = DatabaseFactory.CreateDatabase();
            bool result;
            using (System.Data.Common.DbConnection dbConnection = database.CreateConnection())
            {
                dbConnection.Open();
                System.Data.Common.DbTransaction dbTransaction = dbConnection.BeginTransaction();
                try
                {
                    OrderInfo orderInfo = OrderHelper.GetOrderInfo(orderId);
                    if (orderInfo == null)
                    {
                        result = false;
                        return result;
                    }
                    LineItemInfo lineItemInfo = orderInfo.LineItems[itemid];
                    lineItemInfo.ItemAdjustedCommssion = adjustcommssion;
                    if (!new LineItemDao().UpdateLineItem(orderId, lineItemInfo, dbTransaction))
                    {
                        dbTransaction.Rollback();
                    }
                    if (!new OrderDao().UpdateOrder(orderInfo, dbTransaction))
                    {
                        dbTransaction.Rollback();
                        result = false;
                        return result;
                    }
                    dbTransaction.Commit();
                    flag = true;
                }
                catch (Exception var_6_9F)
                {
                    dbTransaction.Rollback();
                }
                finally
                {
                    dbConnection.Close();
                }
                result = flag;
            }
            return result;
        }

        public static int GetItemNumByOrderID(string orderid)
        {
            return new LineItemDao().GetItemNumByOrderID(orderid);
        }

        public static string FormatOrderStr(string str)
        {
            string result;
            if (string.IsNullOrEmpty(str))
            {
                result = "";
            }
            else
            {
                result = str.Replace(",", "，").Replace("\n", "").Replace("\\", "＼");
            }
            return result;
        }

        public static string ExportOrderData(string orderidList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<table  cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
            stringBuilder.Append("<thead><tr style=\"font-weight: bold; white-space: nowrap;background:#ccc;\">");
            stringBuilder.Append("<th style=\"border-right: 1px solid #ccc;\">订单编号</th><th style=\"border-right: 1px solid #ccc;\">商品名称</th><th style=\"border-right: 1px solid #ccc;\">商品编码</th><th style=\"border-right: 1px solid #ccc;\">SKU</th><th style=\"border-right: 1px solid #ccc;\">单价</th><th style=\"border-right: 1px solid #ccc;\">数量</th><th style=\"border-right: 1px solid #ccc;\">涨价或优惠</th><th style=\"border-right: 1px solid #ccc;\">买家会员名</th><th style=\"border-right: 1px solid #ccc;\">买家应付货款</th><th style=\"border-right: 1px solid #ccc;\">买家应付邮费</th><th style=\"border-right: 1px solid #ccc;\">总金额</th><th style=\"border-right: 1px solid #ccc;\">买家实际支付积分</th><th style=\"border-right: 1px solid #ccc;\">订单状态</th><th style=\"border-right: 1px solid #ccc;\">买家留言</th><th style=\"border-right: 1px solid #ccc;\">收货人姓名</th><th style=\"border-right: 1px solid #ccc;\">收货地址</th><th style=\"border-right: 1px solid #ccc;\">运送方式</th><th style=\"border-right: 1px solid #ccc;\">联系电话</th><th style=\"border-right: 1px solid #ccc;\">联系手机</th><th style=\"border-right: 1px solid #ccc;\">订单创建时间</th><th style=\"border-right: 1px solid #ccc;\">订单付款时间</th><th style=\"border-right: 1px solid #ccc;\">物流单号</th><th style=\"border-right: 1px solid #ccc;\">物流公司</th><th style=\"border-right: 1px solid #ccc;\">订单备注</th><th style=\"border-right: 1px solid #ccc;\">宝贝总数量</th><th style=\"border-right: 1px solid #ccc;\">分销商Id</th><th style=\"border-right: 1px solid #ccc;\">分销商店铺名称</th><th style=\"border-right: 1px solid #ccc;\">修改后的收货地址</th><th>积分抵扣</th><th>优惠</th>");
            stringBuilder.Append("</tr></thead><tbody>");
            string[] array = orderidList.Split(new char[]
			{
				','
			});
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string orderId = array2[i];
                OrderInfo orderInfo = OrderHelper.GetOrderInfo(orderId);
                bool flag = true;
                if (orderInfo != null)
                {
                    int count = orderInfo.LineItems.Values.Count;
                    string text = string.Empty;
                    StringBuilder stringBuilder2 = new StringBuilder();
                    int num = 0;
                    int num2 = 0;
                    foreach (LineItemInfo current in orderInfo.LineItems.Values)
                    {
                        stringBuilder.Append("<tr>");
                        if (flag)
                        {
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.OrderId),
								"</td>"
							}));
                        }
                        num += current.PointNumber;
                        num2 += current.Quantity;
                        text = current.SKU;
                        if (string.IsNullOrEmpty(text))
                        {
                            ProductInfo productDetails = ProductHelper.GetProductDetails(current.ProductId);
                            if (productDetails != null)
                            {
                                text = productDetails.ProductCode;
                            }
                        }
                        stringBuilder.Append("<td>" + OrderHelper.FormatOrderStr(current.ItemDescription) + "</td>");
                        stringBuilder.Append("<td>" + OrderHelper.FormatOrderStr(text) + "</td>");
                        stringBuilder.Append("<td>" + OrderHelper.FormatOrderStr(current.SkuId) + "</td>");
                        stringBuilder.Append("<td>" + ((current.Type == 1) ? "0" : OrderHelper.FormatOrderStr(current.ItemAdjustedPrice.ToString("F2"))) + "</td>");
                        stringBuilder.Append("<td>" + current.Quantity.ToString() + "</td>");
                        stringBuilder.Append("<td>" + ((current.Type == 1) ? "0.00" : (current.ItemAdjustedCommssion * -1m).ToString("F2")) + "</td>");
                        if (flag)
                        {
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.Username),
								"</td>"
							}));
                            decimal total = orderInfo.GetTotal();
                            decimal adjustedFreight = orderInfo.AdjustedFreight;
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr((total - adjustedFreight).ToString("F2")),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.AdjustedFreight.ToString("F2")),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(total.ToString("F2")),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.PointExchange.ToString()),
								"</td>"
							}));
                            string str = string.Empty;
                            switch (orderInfo.OrderStatus)
                            {
                                case OrderStatus.WaitBuyerPay:
                                    str = "等待付款";
                                    break;
                                case OrderStatus.BuyerAlreadyPaid:
                                    str = "已付款";
                                    break;
                                case OrderStatus.SellerAlreadySent:
                                    str = "已发货";
                                    break;
                                case OrderStatus.Closed:
                                    str = "已关闭";
                                    break;
                                case OrderStatus.Finished:
                                    str = "交易完成";
                                    break;
                            }
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(str),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.Remark.ToString()),
								"</td>"
							}));
                            string text2 = string.Empty;
                            string oldAddress = orderInfo.OldAddress;
                            if (!string.IsNullOrEmpty(orderInfo.ShippingRegion))
                            {
                                text2 = orderInfo.ShippingRegion.Replace('，', ' ');
                            }
                            if (!string.IsNullOrEmpty(orderInfo.Address))
                            {
                                text2 += orderInfo.Address;
                            }
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.ShipTo.ToString()),
								"</td>"
							}));
                            if (!string.IsNullOrEmpty(orderInfo.ZipCode))
                            {
                                text2 = text2 + " " + orderInfo.ZipCode;
                            }
                            if (!string.IsNullOrEmpty(orderInfo.TelPhone))
                            {
                                text2 = text2 + " " + orderInfo.TelPhone;
                            }
                            if (!string.IsNullOrEmpty(orderInfo.CellPhone))
                            {
                                text2 = text2 + " " + orderInfo.CellPhone;
                            }
                            string text3 = string.Empty;
                            if (string.IsNullOrEmpty(oldAddress))
                            {
                                stringBuilder.Append(string.Concat(new object[]
								{
									"<td rowspan=\"",
									count,
									"\"  style=\"vnd.ms-excel.numberformat: @;\">",
									OrderHelper.FormatOrderStr(text2),
									"</td>"
								}));
                            }
                            else
                            {
                                text3 = text2;
                                stringBuilder.Append(string.Concat(new object[]
								{
									"<td rowspan=\"",
									count,
									"\"  style=\"vnd.ms-excel.numberformat: @;\">",
									OrderHelper.FormatOrderStr(oldAddress),
									"</td>"
								}));
                            }
                            string text4 = string.Empty;
                            if (orderInfo.OrderStatus == OrderStatus.Finished || orderInfo.OrderStatus == OrderStatus.SellerAlreadySent)
                            {
                                text4 = orderInfo.RealModeName;
                                if (string.IsNullOrEmpty(text4))
                                {
                                    text4 = orderInfo.ModeName;
                                }
                            }
                            else
                            {
                                text4 = orderInfo.ModeName;
                            }
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(text4),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.TelPhone),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.CellPhone),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.OrderDate.ToString()),
								"</td>"
							}));
                            string str2 = string.Empty;
                            if (orderInfo.PayDate.HasValue)
                            {
                                str2 = orderInfo.PayDate.ToString();
                            }
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(str2),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.ShipOrderNumber),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr(orderInfo.ExpressCompanyName),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr((orderInfo.ManagerRemark == null) ? "" : orderInfo.ManagerRemark.ToString()),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								orderInfo.GetProductTotalNum(),
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								OrderHelper.FormatOrderStr((orderInfo.ReferralUserId > 0) ? orderInfo.ReferralUserId.ToString() : ""),
								"</td>"
							}));
                            if (orderInfo.ReferralUserId > 0)
                            {
                                DistributorsInfo distributorInfo = DistributorsBrower.GetDistributorInfo(orderInfo.ReferralUserId);
                                if (distributorInfo != null)
                                {
                                    stringBuilder.Append(string.Concat(new object[]
									{
										"<td rowspan=\"",
										count,
										"\"  style=\"vnd.ms-excel.numberformat: @;\">",
										OrderHelper.FormatOrderStr(distributorInfo.StoreName),
										"</td>"
									}));
                                }
                                else
                                {
                                    stringBuilder.Append(string.Concat(new object[]
									{
										"<td rowspan=\"",
										count,
										"\"  style=\"vnd.ms-excel.numberformat: @;\">",
										OrderHelper.FormatOrderStr(""),
										"</td>"
									}));
                                }
                            }
                            else
                            {
                                stringBuilder.Append(string.Concat(new object[]
								{
									"<td rowspan=\"",
									count,
									"\"  style=\"vnd.ms-excel.numberformat: @;\">",
									OrderHelper.FormatOrderStr(""),
									"</td>"
								}));
                            }
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								text3,
								"</td>"
							}));
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								(orderInfo.PointToCash == 0m) ? "" : orderInfo.PointToCash.ToString(),
								"</td>"
							}));
                            OrderInfo orderInfo2 = orderInfo;
                            StringBuilder stringBuilder3 = new StringBuilder();
                            if (!string.IsNullOrEmpty(orderInfo2.ActivitiesName))
                            {
                                stringBuilder3.Append(orderInfo2.ActivitiesName + ":￥" + orderInfo2.DiscountAmount.ToString("F2"));
                            }
                            if (!string.IsNullOrEmpty(orderInfo2.ReducedPromotionName))
                            {
                                if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                {
                                    stringBuilder3.Append("\r\n");
                                }
                                stringBuilder3.Append(orderInfo2.ReducedPromotionName + ":￥" + orderInfo2.ReducedPromotionAmount.ToString("F2"));
                            }
                            if (!string.IsNullOrEmpty(orderInfo2.CouponName))
                            {
                                if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                {
                                    stringBuilder3.Append("\r\n");
                                }
                                stringBuilder3.Append(orderInfo2.CouponName + ":￥" + orderInfo2.CouponAmount.ToString("F2"));
                            }
                            if (!string.IsNullOrEmpty(orderInfo2.RedPagerActivityName))
                            {
                                if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                {
                                    stringBuilder3.Append("\r\n");
                                }
                                stringBuilder3.Append(orderInfo2.RedPagerActivityName + ":￥" + orderInfo2.RedPagerAmount.ToString("F2"));
                            }
                            if (orderInfo2.PointToCash > 0m)
                            {
                                if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                {
                                    stringBuilder3.Append("\r\n");
                                }
                                stringBuilder3.Append("积分抵现:￥" + orderInfo2.PointToCash.ToString("F2"));
                            }
                            decimal adjustCommssion = orderInfo2.GetAdjustCommssion();
                            decimal d = 0m;
                            decimal d2 = 0m;
                            foreach (LineItemInfo current2 in orderInfo2.LineItems.Values)
                            {
                                if (current2.IsAdminModify)
                                {
                                    d += current2.ItemAdjustedCommssion;
                                }
                                else
                                {
                                    d2 += current2.ItemAdjustedCommssion;
                                }
                            }
                            if (d != 0m)
                            {
                                if (d > 0m)
                                {
                                    if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                    {
                                        stringBuilder3.Append("\r\n");
                                    }
                                    stringBuilder3.Append("管理员调价减:￥" + d.ToString("F2"));
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                    {
                                        stringBuilder3.Append("\r\n");
                                    }
                                    stringBuilder3.Append("管理员调价加:￥" + (d * -1m).ToString("F2"));
                                }
                            }
                            if (d2 != 0m)
                            {
                                if (d2 > 0m)
                                {
                                    if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                    {
                                        stringBuilder3.Append("\r\n");
                                    }
                                    stringBuilder3.Append("分销商调价减:￥" + d2.ToString("F2"));
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
                                    {
                                        stringBuilder3.Append("\r\n");
                                    }
                                    stringBuilder3.Append("分销商调价加:￥" + (d2 * -1m).ToString("F2"));
                                }
                            }
                            stringBuilder.Append(string.Concat(new object[]
							{
								"<td rowspan=\"",
								count,
								"\"  style=\"vnd.ms-excel.numberformat: @;\">",
								stringBuilder3.ToString(),
								"</td>"
							}));
                        }
                        flag = false;
                        stringBuilder.Append("</tr>");
                    }
                }
            }
            stringBuilder.Append("<tbody></table>");
            return stringBuilder.ToString().Replace("<td>", "<td style=\"vnd.ms-excel.numberformat: @;\">");
        }
    }
}
