using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Sales;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Core.Enums;
using Hidistro.Entities;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.Sales;
using Hidistro.Vshop;
using Hishop.Open.Api;
using Hishop.Plugins;
using Hishop.Weixin.Pay;
using Hishop.Weixin.Pay.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Hidistro.UI.Web.OpenAPI.Impl
{
	public class TradeApi : ITrade
	{
		public string ChangLogistics(string tid, string company_name, string out_sid)
		{
			OrderInfo orderInfo = OrderHelper.GetOrderInfo(tid);
			if (orderInfo == null || string.IsNullOrEmpty(orderInfo.OrderId))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_not_Exists, "tid");
			}
			if (orderInfo.OrderStatus == OrderStatus.Refunded || orderInfo.OrderStatus == OrderStatus.Returned || orderInfo.OrderStatus == OrderStatus.Closed || (orderInfo.OrderStatus != OrderStatus.BuyerAlreadyPaid && (!(orderInfo.Gateway == "hishop.plugins.payment.podrequest") || orderInfo.OrderStatus != OrderStatus.WaitBuyerPay) && orderInfo.OrderStatus != OrderStatus.SellerAlreadySent))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Status_Print, "orderstatue");
			}
			ExpressCompanyInfo expressCompanyInfo = ExpressHelper.FindNode(company_name);
			orderInfo.ExpressCompanyAbb = expressCompanyInfo.Kuaidi100Code;
			orderInfo.ExpressCompanyName = expressCompanyInfo.Name;
			orderInfo.IsPrinted = true;
			orderInfo.ShipOrderNumber = out_sid;
			if (!OrderHelper.UpdateOrder(orderInfo))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Print_Faild, "order");
			}
			string format = "{{\"logistics_change_response\":{{\"shipping\":{{\"is_success\":{0}}}}}}}";
			return string.Format(format, "true");
		}

		public string GetIncrementSoldTrades(System.DateTime start_modified, System.DateTime end_modified, string status, string buyer_uname, int page_no, int page_size)
		{
			string format = "{{\"trades_sold_get_response\":{{\"total_results\":\"{0}\",\"has_next\":\"{1}\",\"trades\":{2}}}}}";
			OrderQuery orderQuery = new OrderQuery
			{
				PageSize = 40,
				PageIndex = 1,
				Status = OrderStatus.All,
				UserName = buyer_uname,
				SortBy = "UpdateDate",
				SortOrder = SortAction.Desc,
				StartDate = new System.DateTime?(start_modified),
				EndDate = new System.DateTime?(end_modified)
			};
			OrderStatus status2 = OrderStatus.All;
			if (!string.IsNullOrEmpty(status))
			{
				EnumDescription.GetEnumValue<OrderStatus>(status, ref status2);
			}
			orderQuery.Status = status2;
			if (page_no > 0)
			{
				orderQuery.PageIndex = page_no;
			}
			if (page_size > 0)
			{
				orderQuery.PageSize = page_size;
			}
			Globals.EntityCoding(orderQuery, true);
			int num = 0;
			System.Data.DataSet tradeOrders = OrderHelper.GetTradeOrders(orderQuery, out num);
			string arg = this.ConvertTrades(tradeOrders);
			bool flag = this.CheckHasNext(num, orderQuery.PageSize, orderQuery.PageIndex);
			return string.Format(format, num, flag, arg);
		}

		public string GetSoldTrades(System.DateTime? start_created, System.DateTime? end_created, string status, string buyer_uname, int page_no, int page_size)
		{
			string format = "{{\"trades_sold_get_response\":{{\"total_results\":\"{0}\",\"has_next\":\"{1}\",\"trades\":{2}}}}}";
			OrderQuery orderQuery = new OrderQuery
			{
				PageSize = 40,
				PageIndex = 1,
				Status = OrderStatus.All,
				UserName = buyer_uname,
				SortBy = "OrderDate",
				SortOrder = SortAction.Desc
			};
			if (start_created.HasValue)
			{
				orderQuery.StartDate = start_created;
			}
			if (end_created.HasValue)
			{
				orderQuery.EndDate = end_created;
			}
			OrderStatus status2 = OrderStatus.All;
			if (!string.IsNullOrEmpty(status))
			{
				EnumDescription.GetEnumValue<OrderStatus>(status, ref status2);
			}
			orderQuery.Status = status2;
			if (page_no > 0)
			{
				orderQuery.PageIndex = page_no;
			}
			if (page_size > 0)
			{
				orderQuery.PageSize = page_size;
			}
			Globals.EntityCoding(orderQuery, true);
			int num = 0;
			System.Data.DataSet tradeOrders = OrderHelper.GetTradeOrders(orderQuery, out num);
			string arg = this.ConvertTrades(tradeOrders);
			bool flag = this.CheckHasNext(num, orderQuery.PageSize, orderQuery.PageIndex);
			return string.Format(format, num, flag, arg);
		}

		public string ConvertTrades(System.Data.DataSet dstrades)
		{
			System.Collections.Generic.List<trade_list_model> list = new System.Collections.Generic.List<trade_list_model>();
			foreach (System.Data.DataRow dataRow in dstrades.Tables[0].Rows)
			{
				trade_list_model trade_list_model = new trade_list_model();
				System.Data.DataRow[] childRows = dataRow.GetChildRows("OrderRelation");
				for (int i = 0; i < childRows.Length; i++)
				{
					System.Data.DataRow dataRow2 = childRows[i];
					string sku_properties_name = Globals.HtmlEncode(dataRow2["SKUContent"].ToString());
					trade_itme_model item = new trade_itme_model
					{
						sku_id = (string)dataRow2["SkuId"],
						sku_properties_name = sku_properties_name,
						num_id = dataRow2["ProductId"].ToString(),
						num = (int)dataRow2["Quantity"],
						title = (string)dataRow2["ItemDescription"],
						outer_sku_id = (string)dataRow2["SKU"],
						pic_path = ((dataRow2["ThumbnailsUrl"] != System.DBNull.Value) ? ((string)dataRow2["ThumbnailsUrl"]) : ""),
						price = (decimal)dataRow2["ItemAdjustedPrice"],
						refund_status = EnumDescription.GetEnumDescription((OrderStatus)System.Enum.Parse(typeof(OrderStatus), dataRow2["OrderItemsStatus"].ToString()), 1)
					};
					trade_list_model.orders.Add(item);
				}
				trade_list_model.tid = (string)dataRow["OrderId"];
				if (dataRow["Remark"] != System.DBNull.Value)
				{
					trade_list_model.buyer_memo = dataRow["Remark"].ToString();
				}
				if (dataRow["ManagerRemark"] != System.DBNull.Value)
				{
					trade_list_model.seller_memo = dataRow["ManagerRemark"].ToString();
				}
				if (dataRow["ManagerMark"] != System.DBNull.Value)
				{
					trade_list_model.seller_flag = dataRow["ManagerMark"].ToString();
				}
				trade_list_model.discount_fee = (decimal)dataRow["AdjustedDiscount"];
				trade_list_model.status = EnumDescription.GetEnumDescription((OrderStatus)System.Enum.Parse(typeof(OrderStatus), dataRow["OrderStatus"].ToString()), 1);
				if (dataRow["Gateway"].ToString() == "hishop.plugins.payment.podrequest" && trade_list_model.status == EnumDescription.GetEnumDescription((OrderStatus)System.Enum.Parse(typeof(OrderStatus), System.Convert.ToInt16(OrderStatus.WaitBuyerPay).ToString()), 1))
				{
					trade_list_model.status = EnumDescription.GetEnumDescription((OrderStatus)System.Enum.Parse(typeof(OrderStatus), System.Convert.ToInt16(OrderStatus.BuyerAlreadyPaid).ToString()), 1);
				}
				if (dataRow["CloseReason"] != System.DBNull.Value)
				{
					trade_list_model.close_memo = (string)dataRow["CloseReason"];
				}
				trade_list_model.created = new System.DateTime?(System.DateTime.Parse(dataRow["OrderDate"].ToString()));
				if (dataRow["UpdateDate"] != System.DBNull.Value)
				{
					trade_list_model.modified = new System.DateTime?(System.DateTime.Parse(dataRow["UpdateDate"].ToString()));
				}
				if (dataRow["PayDate"] != System.DBNull.Value)
				{
					trade_list_model.pay_time = new System.DateTime?(System.DateTime.Parse(dataRow["PayDate"].ToString()));
				}
				if (dataRow["ShippingDate"] != System.DBNull.Value)
				{
					trade_list_model.consign_time = new System.DateTime?(System.DateTime.Parse(dataRow["ShippingDate"].ToString()));
				}
				if (dataRow["FinishDate"] != System.DBNull.Value)
				{
					trade_list_model.end_time = new System.DateTime?(System.DateTime.Parse(dataRow["FinishDate"].ToString()));
				}
				trade_list_model.buyer_uname = (string)dataRow["Username"];
				if (dataRow["EmailAddress"] != System.DBNull.Value)
				{
					trade_list_model.buyer_email = (string)dataRow["EmailAddress"];
				}
				if (dataRow["RealName"] != System.DBNull.Value)
				{
					trade_list_model.buyer_nick = (string)dataRow["RealName"];
				}
				if (dataRow["ShipTo"] != System.DBNull.Value)
				{
					trade_list_model.receiver_name = (string)dataRow["ShipTo"];
				}
				string fullRegion = RegionHelper.GetFullRegion(System.Convert.ToInt32(dataRow["RegionId"]), "-");
				if (!string.IsNullOrEmpty(fullRegion))
				{
					string[] array = fullRegion.Split(new char[]
					{
						'-'
					});
					trade_list_model.receiver_state = array[0];
					if (array.Length >= 2)
					{
						trade_list_model.receiver_city = array[1];
					}
					if (array.Length >= 3)
					{
						trade_list_model.receiver_district = array[2];
					}
					else if (array.Length >= 2)
					{
						trade_list_model.receiver_district = array[1];
					}
					if (array.Length >= 4)
					{
						trade_list_model.receiver_town = array[3];
					}
				}
				trade_list_model.receiver_address = (string)dataRow["Address"];
				trade_list_model.receiver_mobile = (string)dataRow["CellPhone"];
				trade_list_model.receiver_zip = (string)dataRow["ZipCode"];
				trade_list_model.invoice_fee = (decimal)dataRow["Tax"];
				trade_list_model.invoice_title = (string)dataRow["InvoiceTitle"];
				trade_list_model.payment = (decimal)dataRow["OrderTotal"];
				trade_list_model.storeId = "0";
				list.Add(trade_list_model);
			}
			return JsonConvert.SerializeObject(list);
		}

		public bool CheckHasNext(int totalrecord, int pagesize, int pageindex)
		{
			int num = pagesize * pageindex;
			return totalrecord > num;
		}

		public string GetTrade(string tid)
		{
			OrderInfo orderInfo = OrderHelper.GetOrderInfo(tid);
			if (orderInfo == null || string.IsNullOrEmpty(orderInfo.OrderId))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_not_Exists, "tid");
			}
			string format = "{{\"trade_get_response\":{{\"trade\":{0}}}}}";
			string arg = this.ConvertTrades(orderInfo);
			return string.Format(format, arg);
		}

		private string ConvertTrades(OrderInfo orderInfo)
		{
			trade_list_model trade_list_model = new trade_list_model();
			trade_list_model.tid = orderInfo.OrderId;
			trade_list_model.buyer_memo = orderInfo.Remark;
			trade_list_model.seller_memo = orderInfo.ManagerRemark;
			trade_list_model.seller_flag = (orderInfo.ManagerMark.HasValue ? System.Convert.ToInt16(orderInfo.ManagerMark).ToString() : "0");
			trade_list_model.discount_fee = orderInfo.AdjustedDiscount;
			trade_list_model.status = EnumDescription.GetEnumDescription(orderInfo.OrderStatus, 1);
			trade_list_model.close_memo = orderInfo.CloseReason;
			trade_list_model.created = new System.DateTime?(orderInfo.OrderDate);
			trade_list_model.modified = new System.DateTime?(orderInfo.UpdateDate);
			trade_list_model.pay_time = orderInfo.PayDate;
			trade_list_model.consign_time = orderInfo.ShippingDate;
			trade_list_model.end_time = orderInfo.FinishDate;
			trade_list_model.buyer_uname = orderInfo.Username;
			trade_list_model.buyer_email = orderInfo.EmailAddress;
			trade_list_model.buyer_nick = orderInfo.RealName;
			trade_list_model.receiver_name = orderInfo.ShipTo;
			string fullRegion = RegionHelper.GetFullRegion(orderInfo.RegionId, "-");
			if (!string.IsNullOrEmpty(fullRegion))
			{
				string[] array = fullRegion.Split(new char[]
				{
					'-'
				});
				trade_list_model.receiver_state = array[0];
				if (array.Length >= 2)
				{
					trade_list_model.receiver_city = array[1];
				}
				if (array.Length >= 3)
				{
					trade_list_model.receiver_district = array[2];
				}
				if (array.Length >= 4)
				{
					trade_list_model.receiver_town = array[3];
				}
			}
			trade_list_model.receiver_address = orderInfo.Address;
			trade_list_model.receiver_mobile = orderInfo.CellPhone;
			trade_list_model.receiver_zip = orderInfo.ZipCode;
			trade_list_model.invoice_fee = orderInfo.Tax;
			trade_list_model.invoice_title = orderInfo.InvoiceTitle;
			trade_list_model.payment = orderInfo.GetTotal();
			trade_list_model.storeId = "0";
			foreach (LineItemInfo current in orderInfo.LineItems.Values)
			{
				string sku_properties_name = Globals.HtmlEncode(current.SKUContent);
				trade_itme_model item = new trade_itme_model
				{
					sku_id = current.SkuId,
					sku_properties_name = sku_properties_name,
					num_id = current.ProductId.ToString(),
					num = current.Quantity,
					title = current.ItemDescription,
					outer_sku_id = current.SKU,
					pic_path = current.ThumbnailsUrl,
					price = decimal.Round(current.ItemAdjustedPrice, 2),
					refund_status = EnumDescription.GetEnumDescription(current.OrderItemsStatus, 1)
				};
				trade_list_model.orders.Add(item);
			}
			return JsonConvert.SerializeObject(trade_list_model);
		}

		public string SendLogistic(string tid, string company_name, string out_sid)
		{
			OrderInfo orderInfo = OrderHelper.GetOrderInfo(tid);
			if (orderInfo == null || string.IsNullOrEmpty(orderInfo.OrderId))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_not_Exists, "tid");
			}
			if (orderInfo.GroupBuyId > 0 && orderInfo.GroupBuyStatus != GroupBuyStatus.Success)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Status_Send, "group order");
			}
			if (!orderInfo.CheckAction(OrderActions.SELLER_SEND_GOODS))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Status_Send, "orderstatue");
			}
			if (string.IsNullOrEmpty(out_sid))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "out_sid");
			}
			ExpressCompanyInfo expressCompanyInfo = ExpressHelper.FindNode(company_name);
			if (expressCompanyInfo == null)
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Company_not_Exists, "company_name");
			}
			orderInfo.ExpressCompanyAbb = expressCompanyInfo.Kuaidi100Code;
			orderInfo.ExpressCompanyName = expressCompanyInfo.Name;
			orderInfo.ShipOrderNumber = out_sid;
			if (!OrderHelper.SendGoods(orderInfo))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_Status_Send, "send good");
			}
			Express.SubscribeExpress100(orderInfo.ExpressCompanyAbb, out_sid);
			OrderHelper.SaveSendNote(new SendNoteInfo
			{
				NoteId = Globals.GetGenerateId(),
				OrderId = orderInfo.OrderId,
				Operator = orderInfo.UserId.ToString(),
				Remark = "接口发货成功"
			});
			if (!string.IsNullOrEmpty(orderInfo.GatewayOrderId) && orderInfo.GatewayOrderId.Trim().Length > 0)
			{
				if (orderInfo.Gateway == "hishop.plugins.payment.ws_wappay.wswappayrequest")
				{
					PaymentModeInfo paymentMode = SalesHelper.GetPaymentMode(orderInfo.PaymentTypeId);
					if (paymentMode != null)
					{
						PaymentRequest paymentRequest = PaymentRequest.CreateInstance(paymentMode.Gateway, HiCryptographer.Decrypt(paymentMode.Settings), orderInfo.OrderId, orderInfo.GetTotal(), "订单发货", "订单号-" + orderInfo.OrderId, orderInfo.EmailAddress, orderInfo.OrderDate, Globals.FullPath(Globals.GetSiteUrls().Home), Globals.FullPath(Globals.GetSiteUrls().UrlData.FormatUrl("PaymentReturn_url", new object[]
						{
							paymentMode.Gateway
						})), Globals.FullPath(Globals.GetSiteUrls().UrlData.FormatUrl("PaymentNotify_url", new object[]
						{
							paymentMode.Gateway
						})), "");
						paymentRequest.SendGoods(orderInfo.GatewayOrderId, orderInfo.RealModeName, orderInfo.ShipOrderNumber, "EXPRESS");
					}
				}
				if (orderInfo.Gateway == "hishop.plugins.payment.weixinrequest")
				{
					SiteSettings masterSettings = SettingsManager.GetMasterSettings(false);
					PayClient payClient;
					if (masterSettings.EnableSP)
					{
						payClient = new PayClient(masterSettings.Main_AppId, masterSettings.WeixinAppSecret, masterSettings.Main_Mch_ID, masterSettings.Main_PayKey, true, masterSettings.WeixinAppId, masterSettings.WeixinPartnerID);
					}
					else
					{
						payClient = new PayClient(masterSettings.WeixinAppId, masterSettings.WeixinAppSecret, masterSettings.WeixinPartnerID, masterSettings.WeixinPartnerKey, false, "", "");
					}
					payClient.DeliverNotify(new DeliverInfo
					{
						TransId = orderInfo.GatewayOrderId,
						OutTradeNo = orderInfo.OrderId,
						OpenId = MemberHelper.GetMember(orderInfo.UserId).OpenId
					});
				}
			}
			orderInfo.OnDeliver();
			string format = "{{\"logistics_send_response\":{{\"shipping\":{{\"is_success\":{0}}}}}}}";
			return string.Format(format, "true");
		}

		public string UpdateTradeMemo(string tid, string memo, int flag)
		{
			OrderInfo orderInfo = OrderHelper.GetOrderInfo(tid);
			if (orderInfo == null || string.IsNullOrEmpty(orderInfo.OrderId))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Trade_not_Exists, "tid");
			}
			if (flag > 0)
			{
				orderInfo.ManagerMark = new OrderMark?((OrderMark)flag);
			}
			orderInfo.ManagerRemark = Globals.HtmlEncode(memo);
			if (!OrderHelper.SaveRemark(orderInfo))
			{
				return OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.System_Error, "save remark");
			}
			string format = "{{\"trade_memo_update_response\":{{\"trade\":{{\"tid\":\"{0}\",\"modified\":\"{1}\"}}}}}}";
			return string.Format(format, orderInfo.OrderId, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
		}
	}
}
