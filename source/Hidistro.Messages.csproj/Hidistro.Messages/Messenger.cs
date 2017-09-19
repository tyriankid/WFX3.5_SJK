using Aop.Api;
using Aop.Api.Response;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.VShop;
using Hidistro.SqlDal.Commodities;
using Hidistro.SqlDal.Members;
using Hidistro.SqlDal.Orders;
using Hishop.AlipayFuwu.Api.Model;
using Hishop.AlipayFuwu.Api.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hidistro.Messages
{
    public static class Messenger
    {
        private static void FuwuSend(string FuwuTemplateId, SiteSettings settings, AliTemplateMessage templateMessage)
        {
            if ((string.IsNullOrWhiteSpace(FuwuTemplateId) || settings.AlipayAppid.Length <= 15 ? false : templateMessage != null))
            {
                AliOHHelper.TemplateSend(templateMessage);
            }
        }

        private static AliTemplateMessage GenerateFuwuMessage_AccountChangeMsg(string templateId, SiteSettings settings, MemberInfo member, string FirstData = "", string ChangeTypeDesc = "", string RemarkData = "")
        {
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? "帐号更新提醒" : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart[] messagePartArray2 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2",
                Value = (string.IsNullOrEmpty(member.UserBindName) ? member.UserName : member.UserBindName)
            };
            messagePartArray2[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark",
                Value = string.Concat("消息类型[", ChangeTypeDesc, "]，", RemarkData)
            };
            messagePartArray[3] = messagePart3;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_DistributorCreateMsg(string templateId, SiteSettings settings, DistributorsInfo distributor, MemberInfo member, string FirstData = "")
        {
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? "您好，有一位新分销商申请了店铺" : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2",
                Value = string.Concat(member.UserName, "，", member.CellPhone)
            };
            messagePartArray[2] = messagePart2;
            AliTemplateMessage.MessagePart[] messagePartArray2 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark",
                Value = (string.IsNullOrEmpty(member.UserBindName) ? member.UserName : member.UserBindName)
            };
            messagePartArray2[3] = messagePart3;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_DrawCashResultMsg(string templateId, SiteSettings settings, BalanceDrawRequestInfo balance, string FirstData = "", string IsCheckDesc = "")
        {
            string weixinToken = settings.WeixinToken;
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? "分销商提现" : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2"
            };
            string storeName = balance.StoreName;
            decimal amount = balance.Amount;
            messagePart2.Value = string.Concat(storeName, "申请金额：￥", amount.ToString("F2"));
            messagePartArray[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark"
            };
            string[] merchantCode = new string[] { "提现帐号[", balance.MerchantCode, "],当前状态：[", IsCheckDesc, "]，备注：", balance.Remark };
            messagePart3.Value = string.Concat(merchantCode);
            messagePartArray[3] = messagePart3;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_MemberAmountDrawCashResultMsg(string templateId, SiteSettings settings, MemberAmountRequestInfo balance, string FirstData = "", string IsCheckDesc = "", string url = "")
        {
            string weixinToken = settings.WeixinToken;
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = url,
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? "账户余额提现" : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2"
            };
            string userName = balance.UserName;
            decimal amount = balance.Amount;
            messagePart2.Value = string.Concat(userName, "申请金额：￥", amount.ToString("F2"));
            messagePartArray[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark"
            };
            string[] accountCode = new string[] { "提现帐号[", balance.AccountCode, "],当前状态：[", IsCheckDesc, "]，备注：", balance.Remark };
            messagePart3.Value = string.Concat(accountCode);
            messagePartArray[3] = messagePart3;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_OrderMsg(string templateId, SiteSettings settings, OrderInfo order, string FirstData = "")
        {
            string firstProductName = (new OrderDao()).GetFirstProductName(order.OrderId);
            string weixinToken = settings.WeixinToken;
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? firstProductName : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = order.OrderId
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2",
                Value = OrderInfo.GetOrderStatusName(order.OrderStatus)
            };
            messagePartArray[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark"
            };
            decimal total = order.GetTotal();
            messagePart3.Value = string.Concat("订单总金额￥", total.ToString("F2"));
            messagePartArray[3] = messagePart3;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_PersonalMsg(string templateId, SiteSettings settings, MemberInfo member, string FirstData = "", string ContentData = "")
        {
            string weixinToken = settings.WeixinToken;
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? "个人消息通知" : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart[] messagePartArray2 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2",
                Value = (string.IsNullOrEmpty(ContentData) ? "获得了奖品" : ContentData)
            };
            messagePartArray2[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark",
                Value = ""
            };
            messagePartArray[3] = messagePart3;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_RefundSuccessMsg(string templateId, SiteSettings settings, OrderInfo order, RefundInfo refundInfo, string FirstData = "")
        {
            string productName = (new ProductDao()).GetProductDetails(refundInfo.ProductId).ProductName;
            string weixinToken = settings.WeixinToken;
            if (string.IsNullOrEmpty(refundInfo.RefundRemark))
            {
                refundInfo.RefundRemark = "";
            }
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage aliTemplateMessage1 = aliTemplateMessage;
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[5];
            AliTemplateMessage.MessagePart[] messagePartArray1 = messagePartArray;
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = (string.IsNullOrEmpty(FirstData) ? string.Concat("订单号：", order.OrderId) : FirstData)
            };
            messagePartArray1[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = refundInfo.RefundMoney.ToString("F2")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2",
                Value = productName
            };
            messagePartArray[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword3",
                Value = order.OrderId
            };
            messagePartArray[3] = messagePart3;
            AliTemplateMessage.MessagePart messagePart4 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark",
                Value = refundInfo.RefundRemark.Replace("\r", "").Replace("\n", "")
            };
            messagePartArray[4] = messagePart4;
            aliTemplateMessage1.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static AliTemplateMessage GenerateFuwuMessage_ServiceMsg(string templateId, SiteSettings settings, string FirstData = "", string TitleStr = "", string RemarkData = "")
        {
            AliTemplateMessage aliTemplateMessage = new AliTemplateMessage()
            {
                Url = "",
                TemplateId = templateId,
                Touser = ""
            };
            AliTemplateMessage.MessagePart[] messagePartArray = new AliTemplateMessage.MessagePart[4];
            AliTemplateMessage.MessagePart messagePart = new AliTemplateMessage.MessagePart()
            {
                Name = "first",
                Value = FirstData
            };
            messagePartArray[0] = messagePart;
            AliTemplateMessage.MessagePart messagePart1 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword1",
                Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            messagePartArray[1] = messagePart1;
            AliTemplateMessage.MessagePart messagePart2 = new AliTemplateMessage.MessagePart()
            {
                Name = "keyword2",
                Value = TitleStr
            };
            messagePartArray[2] = messagePart2;
            AliTemplateMessage.MessagePart messagePart3 = new AliTemplateMessage.MessagePart()
            {
                Name = "remark",
                Value = RemarkData
            };
            messagePartArray[3] = messagePart3;
            aliTemplateMessage.Data = messagePartArray;
            return aliTemplateMessage;
        }

        private static TempleteModel GenerateWeixinMessage_AccountChangeMsg(string templateId, MemberInfo member, string FirstData = "", string ChangeTypeDesc = "", string RemarkData = "")
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TempleteModel variable = templeteModel;
            TemplateDataItem templateDataItem = new TemplateDataItem((string.IsNullOrEmpty(FirstData) ? "帐号更新提醒" : FirstData), "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem((string.IsNullOrEmpty(member.UserBindName) ? member.UserName : member.UserBindName), "#173177");
            DateTime now = DateTime.Now;
            variable.data = new { first = templateDataItem, account = templateDataItem1, time = new TemplateDataItem(now.ToString("yyyy-MM-dd HH:mm:ss"), "#173177"), type = new TemplateDataItem(ChangeTypeDesc, "#173177"), remark = new TemplateDataItem(RemarkData, "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_ConsultMsg(string templateId, string title, string nickName, string consult, string remark)
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = "",
                data = new { first = new TemplateDataItem(title, "#173177"), keyword1 = new TemplateDataItem(nickName, "#173177"), keyword2 = new TemplateDataItem(consult, "#173177"), remark = new TemplateDataItem(remark, "#173177") }
            };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_DistributorCreateMsg(string templateId, DistributorsInfo distributor, MemberInfo member, string FirstData = "")
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TempleteModel variable = templeteModel;
            TemplateDataItem templateDataItem = new TemplateDataItem((string.IsNullOrEmpty(FirstData) ? "您好，有一位新分销商申请了店铺" : FirstData), "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem((string.IsNullOrEmpty(member.UserBindName) ? member.UserName : member.UserBindName), "#173177");
            TemplateDataItem templateDataItem2 = new TemplateDataItem(member.CellPhone, "#173177");
            DateTime now = DateTime.Now;
            variable.data = new { first = templateDataItem, keyword1 = templateDataItem1, keyword2 = templateDataItem2, keyword3 = new TemplateDataItem(now.ToString("yyyy-MM-dd HH:mm:ss"), "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_DrawCashResultMsg(string templateId, BalanceDrawRequestInfo balance, string FirstData = "", string IsCheckDesc = "")
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TempleteModel variable = templeteModel;
            TemplateDataItem templateDataItem = new TemplateDataItem((string.IsNullOrEmpty(FirstData) ? "分销商提现" : FirstData), "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem(balance.StoreName, "#173177");
            decimal amount = balance.Amount;
            TemplateDataItem templateDataItem2 = new TemplateDataItem(amount.ToString("F2"), "#173177");
            TemplateDataItem templateDataItem3 = new TemplateDataItem(string.Concat(balance.StoreName, "[", balance.MerchantCode, "]"), "#173177");
            DateTime requestTime = balance.RequestTime;
            variable.data = new { first = templateDataItem, keyword1 = templateDataItem1, keyword2 = templateDataItem2, keyword3 = templateDataItem3, keyword4 = new TemplateDataItem(requestTime.ToString("yyyy-MM-dd HH:mm:ss"), "#173177"), keyword5 = new TemplateDataItem(IsCheckDesc, "#173177"), remark = new TemplateDataItem(balance.Remark, "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_DrawscashMsg(string templateId, string title, decimal totalMoney, string timeStr, string statusStr, string remark)
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = "",
                data = new { first = new TemplateDataItem(title, "#173177"), keyword1 = new TemplateDataItem(totalMoney.ToString("F2"), "#173177"), keyword2 = new TemplateDataItem(timeStr, "#173177"), keyword3 = new TemplateDataItem(statusStr, "#173177"), remark = new TemplateDataItem(remark, "#173177") }
            };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_JuniorRegisterMsg(string templateId, string title, string nickName, string remark)
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TemplateDataItem templateDataItem = new TemplateDataItem(title, "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem(nickName, "#173177");
            DateTime now = DateTime.Now;
            templeteModel.data = new { first = templateDataItem, keyword1 = templateDataItem1, keyword2 = new TemplateDataItem(now.ToString("yyyy-MM-dd HH:mm:ss"), "#173177"), remark = new TemplateDataItem(remark, "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_MemberAmountDrawCashResultMsg(string templateId, MemberAmountRequestInfo balance, string FirstData = "", string IsCheckDesc = "", string url = "")
        {
            SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = url
            };
            TempleteModel variable = templeteModel;
            TemplateDataItem templateDataItem = new TemplateDataItem((string.IsNullOrEmpty(FirstData) ? "账户余额提现" : FirstData), "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem(masterSettings.SiteName, "#173177");
            decimal amount = balance.Amount;
            TemplateDataItem templateDataItem2 = new TemplateDataItem(amount.ToString("F2"), "#173177");
            TemplateDataItem templateDataItem3 = new TemplateDataItem(string.Concat(balance.UserName, "[", balance.AccountCode, "]"), "#173177");
            DateTime requestTime = balance.RequestTime;
            variable.data = new { first = templateDataItem, keyword1 = templateDataItem1, keyword2 = templateDataItem2, keyword3 = templateDataItem3, keyword4 = new TemplateDataItem(requestTime.ToString("yyyy-MM-dd"), "#173177"), keyword5 = new TemplateDataItem(IsCheckDesc, "#173177"), remark = new TemplateDataItem(balance.Remark, "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_OrderMsg(string templateId, OrderInfo order, string FirstData = "")
        {
            string firstProductName = (new OrderDao()).GetFirstProductName(order.OrderId);
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TempleteModel variable = templeteModel;
            TemplateDataItem templateDataItem = new TemplateDataItem((string.IsNullOrEmpty(FirstData) ? string.Concat("订单号：", order.OrderId) : FirstData), "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem(firstProductName, "#173177");
            decimal total = order.GetTotal();
            variable.data = new { first = templateDataItem, keyword1 = templateDataItem1, keyword2 = new TemplateDataItem(total.ToString("F2"), "#173177"), keyword3 = new TemplateDataItem(OrderInfo.GetOrderStatusName(order.OrderStatus), "#173177"), remark = new TemplateDataItem("", "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_PrizeMsg(string templateId, string title, string actName, string prizeName, string remark, string url = "")
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = url,
                data = new { first = new TemplateDataItem(title, "#173177"), keyword1 = new TemplateDataItem(actName, "#173177"), keyword2 = new TemplateDataItem(prizeName, "#173177"), remark = new TemplateDataItem(remark, "#173177") }
            };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_ProductMsg(string templateId, string title, string storeName, string productName, decimal price, string remark)
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = "",
                data = new { first = new TemplateDataItem(title, "#173177"), keyword1 = new TemplateDataItem(storeName, "#173177"), keyword2 = new TemplateDataItem(productName, "#173177"), keyword3 = new TemplateDataItem(price.ToString("F2"), "#173177"), remark = new TemplateDataItem(remark, "#173177") }
            };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_RefundSuccessMsg(string templateId, OrderInfo order, RefundInfo refundInfo, string FirstData = "")
        {
            string productName = (new ProductDao()).GetProductDetails(refundInfo.ProductId).ProductName;
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TempleteModel variable = templeteModel;
            TemplateDataItem templateDataItem = new TemplateDataItem((string.IsNullOrEmpty(FirstData) ? string.Concat("订单号：", order.OrderId) : FirstData), "#173177");
            decimal refundMoney = refundInfo.RefundMoney;
            variable.data = new { first = templateDataItem, keynote1 = new TemplateDataItem(refundMoney.ToString("f2"), "#173177"), keynote2 = new TemplateDataItem("请联系商家", "#173177"), keynote3 = new TemplateDataItem("请联系商家", "#173177"), keynote4 = new TemplateDataItem(productName, "#173177"), keynote5 = new TemplateDataItem(order.OrderId, "#173177"), keynote6 = new TemplateDataItem(refundInfo.Comments.Replace("\r", "").Replace("\n", ""), "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_RegisterMsg(string templateId, string title, string nickName, string remark)
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = ""
            };
            TemplateDataItem templateDataItem = new TemplateDataItem(title, "#173177");
            TemplateDataItem templateDataItem1 = new TemplateDataItem(nickName, "#173177");
            TemplateDataItem templateDataItem2 = new TemplateDataItem("无", "#173177");
            DateTime now = DateTime.Now;
            templeteModel.data = new { first = templateDataItem, keyword1 = templateDataItem1, keyword2 = templateDataItem2, keyword3 = new TemplateDataItem(now.ToString("yyyy-MM-dd HH:mm:ss"), "#173177"), remark = new TemplateDataItem(remark, "#173177") };
            return templeteModel;
        }

        private static TempleteModel GenerateWeixinMessage_ServiceMsg(string templateId, string title, string serviceType, string productName, string orderId, string timeStr, string remark)
        {
            TempleteModel templeteModel = new TempleteModel()
            {
                template_id = templateId,
                topcolor = "#00FF00",
                url = "",
                data = new { first = new TemplateDataItem(title, "#173177"), keyword1 = new TemplateDataItem(serviceType, "#173177"), keyword2 = new TemplateDataItem(productName, "#173177"), keyword3 = new TemplateDataItem(orderId, "#173177"), keyword4 = new TemplateDataItem(timeStr, "#173177"), remark = new TemplateDataItem(remark, "#173177") }
            };
            return templeteModel;
        }

        private static MailMessage GenericOrderEmail(MessageTemplate template, SiteSettings settings, string UserName, string userEmail, string orderId, decimal total, string memo, string shippingType, string shippingName, string shippingAddress, string shippingZip, string shippingPhone, string shippingCell, string shippingEmail, string shippingBillno, decimal refundMoney, string closeReason)
        {
            MailMessage mailMessage;
            MailMessage emailTemplate = MessageTemplateHelper.GetEmailTemplate(template, userEmail);
            if (emailTemplate != null)
            {
                emailTemplate.Subject = Messenger.GenericOrderMessageFormatter(settings, UserName, emailTemplate.Subject, orderId, total, memo, shippingType, shippingName, shippingAddress, shippingZip, shippingPhone, shippingCell, shippingEmail, shippingBillno, refundMoney, closeReason);
                emailTemplate.Body = Messenger.GenericOrderMessageFormatter(settings, UserName, emailTemplate.Body, orderId, total, memo, shippingType, shippingName, shippingAddress, shippingZip, shippingPhone, shippingCell, shippingEmail, shippingBillno, refundMoney, closeReason);
                mailMessage = emailTemplate;
            }
            else
            {
                mailMessage = null;
            }
            return mailMessage;
        }

        private static string GenericOrderMessageFormatter(SiteSettings settings, string UserName, string stringToFormat, string orderId, decimal total, string memo, string shippingType, string shippingName, string shippingAddress, string shippingZip, string shippingPhone, string shippingCell, string shippingEmail, string shippingBillno, decimal refundMoney, string closeReason)
        {
            stringToFormat = stringToFormat.Replace("$SiteName$", settings.SiteName.Trim());
            stringToFormat = stringToFormat.Replace("$UserName$", UserName);
            stringToFormat = stringToFormat.Replace("$OrderId$", orderId);
            stringToFormat = stringToFormat.Replace("$Total$", total.ToString("F"));
            stringToFormat = stringToFormat.Replace("$Memo$", memo);
            stringToFormat = stringToFormat.Replace("$Shipping_Type$", shippingType);
            stringToFormat = stringToFormat.Replace("$Shipping_Name$", shippingName);
            stringToFormat = stringToFormat.Replace("$Shipping_Addr$", shippingAddress);
            stringToFormat = stringToFormat.Replace("$Shipping_Zip$", shippingZip);
            stringToFormat = stringToFormat.Replace("$Shipping_Phone$", shippingPhone);
            stringToFormat = stringToFormat.Replace("$Shipping_Cell$", shippingCell);
            stringToFormat = stringToFormat.Replace("$Shipping_Email$", shippingEmail);
            stringToFormat = stringToFormat.Replace("$Shipping_Billno$", shippingBillno);
            stringToFormat = stringToFormat.Replace("$RefundMoney$", refundMoney.ToString("F"));
            stringToFormat = stringToFormat.Replace("$CloseReason$", closeReason);
            return stringToFormat;
        }

        private static void GenericOrderMessages(SiteSettings settings, string UserName, string userEmail, string orderId, decimal total, string memo, string shippingType, string shippingName, string shippingAddress, string shippingZip, string shippingPhone, string shippingCell, string shippingEmail, string shippingBillno, decimal refundMoney, string closeReason, MessageTemplate template, out MailMessage email, out string smsMessage, out string innerSubject, out string innerMessage)
        {
            email = null;
            smsMessage = null;
            object obj = null;
            string str = (string)obj;
            innerMessage = (string)obj;
            innerSubject = str;
            if ((template == null ? false : settings != null))
            {
                if ((!template.SendEmail ? false : settings.EmailEnabled))
                {
                    email = Messenger.GenericOrderEmail(template, settings, UserName, userEmail, orderId, total, memo, shippingType, shippingName, shippingAddress, shippingZip, shippingPhone, shippingCell, shippingEmail, shippingBillno, refundMoney, closeReason);
                }
                if ((!template.SendSMS ? false : settings.SMSEnabled))
                {
                    smsMessage = Messenger.GenericOrderMessageFormatter(settings, UserName, template.SMSBody, orderId, total, memo, shippingType, shippingName, shippingAddress, shippingZip, shippingPhone, shippingCell, shippingEmail, shippingBillno, refundMoney, closeReason);
                }
                if (template.SendInnerMessage)
                {
                    innerSubject = Messenger.GenericOrderMessageFormatter(settings, UserName, template.InnerMessageSubject, orderId, total, memo, shippingType, shippingName, shippingAddress, shippingZip, shippingPhone, shippingCell, shippingEmail, shippingBillno, refundMoney, closeReason);
                    innerMessage = Messenger.GenericOrderMessageFormatter(settings, UserName, template.InnerMessageBody, orderId, total, memo, shippingType, shippingName, shippingAddress, shippingZip, shippingPhone, shippingCell, shippingEmail, shippingBillno, refundMoney, closeReason);
                }
            }
        }

        private static MailMessage GenericUserEmail(MessageTemplate template, SiteSettings settings, string UserName, string userEmail, string password, string dealPassword)
        {
            MailMessage mailMessage;
            MailMessage emailTemplate = MessageTemplateHelper.GetEmailTemplate(template, userEmail);
            if (emailTemplate != null)
            {
                emailTemplate.Subject = Messenger.GenericUserMessageFormatter(settings, emailTemplate.Subject, UserName, userEmail, password, dealPassword);
                emailTemplate.Body = Messenger.GenericUserMessageFormatter(settings, emailTemplate.Body, UserName, userEmail, password, dealPassword);
                mailMessage = emailTemplate;
            }
            else
            {
                mailMessage = null;
            }
            return mailMessage;
        }

        private static string GenericUserMessageFormatter(SiteSettings settings, string stringToFormat, string UserName, string userEmail, string password, string dealPassword)
        {
            stringToFormat = stringToFormat.Replace("$SiteName$", settings.SiteName.Trim());
            stringToFormat = stringToFormat.Replace("$UserName$", UserName.Trim());
            stringToFormat = stringToFormat.Replace("$Email$", userEmail.Trim());
            stringToFormat = stringToFormat.Replace("$Password$", password);
            stringToFormat = stringToFormat.Replace("$DealPassword$", dealPassword);
            return stringToFormat;
        }

        private static void GenericUserMessages(SiteSettings settings, string UserName, string userEmail, string password, string dealPassword, MessageTemplate template, out MailMessage email, out string smsMessage, out string innerSubject, out string innerMessage)
        {
            email = null;
            smsMessage = null;
            object obj = null;
            string str = (string)obj;
            innerMessage = (string)obj;
            innerSubject = str;
            if ((template == null ? false : settings != null))
            {
                if ((!template.SendEmail ? false : settings.EmailEnabled))
                {
                    email = Messenger.GenericUserEmail(template, settings, UserName, userEmail, password, dealPassword);
                }
                if ((!template.SendSMS ? false : settings.SMSEnabled))
                {
                    smsMessage = Messenger.GenericUserMessageFormatter(settings, template.SMSBody, UserName, userEmail, password, dealPassword);
                }
                if (template.SendInnerMessage)
                {
                    innerSubject = Messenger.GenericUserMessageFormatter(settings, template.InnerMessageSubject, UserName, userEmail, password, dealPassword);
                    innerMessage = Messenger.GenericUserMessageFormatter(settings, template.InnerMessageBody, UserName, userEmail, password, dealPassword);
                }
            }
        }

        public static SiteSettings GetMasterSettings()
        {
            return SettingsManager.GetMasterSettings(true);
        }

        private static string GetUserCellPhone(MemberInfo user)
        {
            string cellPhone;
            if (user != null)
            {
                cellPhone = user.CellPhone;
            }
            else
            {
                cellPhone = null;
            }
            return cellPhone;
        }

        private static void Send_Fuwu_ToListUser(List<string> SendToUserList, AliTemplateMessage templateMessage)
        {
            if (templateMessage != null)
            {
                foreach (string sendToUserList in SendToUserList)
                {
                    templateMessage.Touser = sendToUserList;
                    AliOHHelper.log(AliOHHelper.templateSendMessage(templateMessage, "查看详情"));
                    try
                    {
                        AliOHHelper.log(AliOHHelper.TemplateSend(templateMessage).Body);
                    }
                    catch (Exception exception)
                    {
                        AliOHHelper.log(exception.Message.ToString());
                    }
                }
            }
        }

        private static void Send_Fuwu_ToMoreUser(string TemplateDetailType, string BuyerWXOpenId, string SalerWXOpenId, MessageTemplate template, SiteSettings settings, bool sendFirst, AliTemplateMessage templateMessage)
        {
            if (!string.IsNullOrEmpty(templateMessage.TemplateId))
            {
                List<string> strs = new List<string>();
                string[] templateDetailType = new string[] { "当前模板类型：", TemplateDetailType, ",会员", BuyerWXOpenId, "|分销商", SalerWXOpenId, "|", templateMessage.TemplateId };
                AliOHHelper.log(string.Concat(templateDetailType));
                if ((settings.AlipayAppid.Length <= 15 || string.IsNullOrWhiteSpace(template.WeixinTemplateId) ? false : templateMessage != null))
                {
                    string str = "";
                    if (TemplateDetailType == "OrderCreate")
                    {
                        str = "Msg1";
                    }
                    else if (TemplateDetailType == "OrderPay")
                    {
                        str = "Msg2";
                    }
                    else if (TemplateDetailType == "ServiceRequest")
                    {
                        str = "Msg3";
                    }
                    else if (!(TemplateDetailType == "DrawCashRequest" ? false : !(TemplateDetailType == "MemberAmountDrawCashRequest")))
                    {
                        str = "Msg4";
                    }
                    else if (TemplateDetailType == "ProductAsk")
                    {
                        str = "Msg5";
                    }
                    else if (TemplateDetailType == "DistributorCreate")
                    {
                        str = "Msg6";
                    }
                    if (str != "")
                    {
                        strs = MessageTemplateHelper.GetFuwuAdminUserMsgList(str);
                    }
                    if ((string.IsNullOrEmpty(BuyerWXOpenId) ? false : template.IsSendWeixin_ToMember))
                    {
                        strs.Add(BuyerWXOpenId);
                    }
                    if ((string.IsNullOrEmpty(SalerWXOpenId) ? false : template.IsSendWeixin_ToDistributor))
                    {
                        if (!(SalerWXOpenId == "*"))
                        {
                            strs.Add(SalerWXOpenId);
                        }
                        else
                        {
                            (new DistributorsDao()).SelectDistributorsAliOpenId(ref strs);
                        }
                    }
                    strs = strs.Distinct<string>().ToList<string>();
                    Messenger.Send_Fuwu_ToListUser(strs, templateMessage);
                }
            }
            else
            {
                AliOHHelper.log(string.Concat("模板ID为空值,当前模板类型", TemplateDetailType));
            }
        }

        private static void Send_WeiXin_ToListUser(List<string> SendToUserList, TempleteModel templateMessage)
        {
            if (templateMessage != null)
            {
                string accessToken = WxTemplateSendHelp.GetAccessToken();
                foreach (string sendToUserList in SendToUserList)
                {
                    templateMessage.touser = sendToUserList;
                    WxTemplateMessageResult wxTemplateMessageResult = WxTemplateSendHelp.SendTemplateMessage(accessToken, templateMessage);
                    if ((wxTemplateMessageResult.errcode == 0 ? false : wxTemplateMessageResult.errmsg.Contains("invalid credential")))
                    {
                        accessToken = WxTemplateSendHelp.GetAccessToken();
                        wxTemplateMessageResult = WxTemplateSendHelp.SendTemplateMessage(accessToken, templateMessage);
                    }
                    if (wxTemplateMessageResult.errcode != 0)
                    {
                        WxTemplateSendHelp.Logwx(string.Concat("发送出错了：", wxTemplateMessageResult.errmsg));
                        WxTemplateSendHelp.Logwx(string.Concat("当前发送消息：", JsonConvert.SerializeObject(templateMessage)));
                    }
                }
            }
        }

        private static void Send_WeiXin_ToMoreUser(string TemplateDetailType, string BuyerWXOpenId, string SalerWXOpenId, MessageTemplate template, bool sendFirst, TempleteModel templateMessage)
        {
            List<string> strs = new List<string>();
            if ((!template.SendWeixin || string.IsNullOrWhiteSpace(template.WeixinTemplateId) ? false : templateMessage != null))
            {
                string str = "";
                if (TemplateDetailType == "OrderCreate")
                {
                    str = "Msg1";
                }
                else if (TemplateDetailType == "OrderPay")
                {
                    str = "Msg2";
                }
                else if (TemplateDetailType == "ServiceRequest")
                {
                    str = "Msg3";
                }
                else if (!(TemplateDetailType == "DrawCashRequest" ? false : !(TemplateDetailType == "MemberAmountDrawCashRequest")))
                {
                    str = "Msg4";
                }
                else if (TemplateDetailType == "ProductAsk")
                {
                    str = "Msg5";
                }
                else if (TemplateDetailType == "DistributorCreate")
                {
                    str = "Msg6";
                }
                if (str != "")
                {
                    strs = MessageTemplateHelper.GetAdminUserMsgList(str);
                }
                if ((string.IsNullOrEmpty(BuyerWXOpenId) ? false : template.IsSendWeixin_ToMember))
                {
                    strs.Add(BuyerWXOpenId);
                }
                if ((string.IsNullOrEmpty(SalerWXOpenId) ? false : template.IsSendWeixin_ToDistributor))
                {
                    if (!(SalerWXOpenId == "*"))
                    {
                        strs.Add(SalerWXOpenId);
                    }
                    else
                    {
                        (new DistributorsDao()).SelectDistributorsOpenId(ref strs);
                    }
                }
                strs = strs.Distinct<string>().ToList<string>();
                Messenger.Send_WeiXin_ToListUser(strs, templateMessage);
            }
        }

        private static bool Send_WeiXin_ToOneUser(string OpenId, TempleteModel templateMessage)
        {
            bool flag = false;
            if (templateMessage != null)
            {
                string accessToken = WxTemplateSendHelp.GetAccessToken();
                templateMessage.touser = OpenId;
                WxTemplateMessageResult wxTemplateMessageResult = WxTemplateSendHelp.SendTemplateMessage(accessToken, templateMessage);
                if ((wxTemplateMessageResult.errcode == 0 ? false : wxTemplateMessageResult.errmsg.Contains("invalid credential")))
                {
                    wxTemplateMessageResult = WxTemplateSendHelp.SendTemplateMessage(WxTemplateSendHelp.GetAccessToken(), templateMessage);
                }
                if (wxTemplateMessageResult.errcode == 0)
                {
                    flag = true;
                }
                else
                {
                    WxTemplateSendHelp.Logwx(string.Concat("发送出错了：", wxTemplateMessageResult.errmsg));
                    WxTemplateSendHelp.Logwx(string.Concat("当前发送消息：", JsonConvert.SerializeObject(templateMessage)));
                    flag = false;
                }
            }
            return flag;
        }

        public static void SendFuwuMsg_AccountLockOrUnLock(MemberInfo member, bool IsLock)
        {
            string str = "AccountLock";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = Messenger.GetMasterSettings();
                AliTemplateMessage aliTemplateMessage = null;
                aliTemplateMessage = (!IsLock ? Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "您的分销商资格已解冻", "账户解冻", "您的分销商资格账户已解冻，如有疑问，请联系客服！") : Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "您的分销商资格已被冻结", "账户冻结", "您的分销商资格已被冻结，如有疑问，请联系客服！"));
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, member.AlipayOpenid, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_DistributorCancel(MemberInfo member)
        {
            string str = "DistributorCancel";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "您已经被取消分销资质", "账户被取消分销商资质", "您的分销商资格已被取消，如有疑问，请联系客服！");
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, member.AlipayOpenid, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_DistributorCreate(DistributorsInfo distributor, MemberInfo member)
        {
            string alipayOpenid = "";
            if (member.AlipayOpenid == null)
            {
                member.AlipayOpenid = "";
            }
            else
            {
                alipayOpenid = member.AlipayOpenid;
            }
            string str = "DistributorCreate";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_DistributorCreateMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, distributor, member, "您好，有一位新分销商申请了店铺");
                Messenger.Send_Fuwu_ToMoreUser(str, alipayOpenid, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
                int num = 0;
                int distributorNumOfTotal = (new MemberDao()).GetDistributorNumOfTotal(member.ReferralUserId, out num);
                List<string> strs = new List<string>();
                if (!string.IsNullOrEmpty(member.AlipayOpenid))
                {
                    AliTemplateMessage aliTemplateMessage1 = Messenger.GenerateFuwuMessage_DistributorCreateMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, distributor, member, string.Format("恭喜您，成为{0}的第{1}位分销商", masterSettings.SiteName, distributorNumOfTotal));
                    strs.Add(member.AlipayOpenid);
                    Messenger.Send_Fuwu_ToListUser(strs, aliTemplateMessage1);
                }
                if ((member.ReferralUserId <= 0 ? false : member.ReferralUserId != member.UserId))
                {
                    string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(member.ReferralUserId);
                    if (!string.IsNullOrEmpty(aliOpenIDByUserId))
                    {
                        strs.Clear();
                        strs.Add(aliOpenIDByUserId);
                        AliTemplateMessage aliTemplateMessage2 = Messenger.GenerateFuwuMessage_DistributorCreateMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, distributor, member, string.Format("恭喜您，{0}成功开店，成为您的第{1}位下级分销商", member.UserName, num));
                        Messenger.Send_Fuwu_ToListUser(strs, aliTemplateMessage2);
                    }
                }
            }
        }

        public static void SendFuwuMsg_DistributorGradeChange(MemberInfo member, string gradeName)
        {
            if (!string.IsNullOrEmpty(member.AlipayOpenid))
            {
                string str = "DistributorGradeChange";
                MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
                if (fuwuMessageTemplateByDetailType != null)
                {
                    SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                    AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "恭喜您成功升级！", "分销商账户升级", string.Concat("恭喜您成功升级为[", gradeName, "]，您将享受到更多的分销商特权！"));
                    Messenger.Send_Fuwu_ToMoreUser(str, "", member.AlipayOpenid, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
                }
            }
        }

        public static void SendFuwuMsg_DrawCashReject(BalanceDrawRequestInfo balance)
        {
            string str = "DrawCashReject";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_DrawCashResultMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, balance, "分销商提现结果被驳回", "驳回");
                string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(balance.UserId);
                Messenger.Send_Fuwu_ToMoreUser(str, aliOpenIDByUserId, aliOpenIDByUserId, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_DrawCashRelease(BalanceDrawRequestInfo balance)
        {
            string str = "DrawCashRelease";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_DrawCashResultMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, balance, "分销商提现已通过", "通过");
                string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(balance.UserId);
                Messenger.Send_Fuwu_ToMoreUser(str, aliOpenIDByUserId, aliOpenIDByUserId, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_DrawCashRequest(BalanceDrawRequestInfo balance)
        {
            string str = "其它";
            if (balance.RequestType == 0)
            {
                str = "微信钱包";
            }
            else if (balance.RequestType == 1)
            {
                str = string.Concat("(支付宝)", balance.MerchantCode);
            }
            else if (balance.RequestType == 2)
            {
                str = string.Concat("(", balance.BankName, ")", balance.MerchantCode);
            }
            else if (balance.RequestType == 3)
            {
                str = "微信红包";
            }
            string str1 = "DrawCashRequest";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str1);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                string weixinTemplateId = fuwuMessageTemplateByDetailType.WeixinTemplateId;
                string[] userName = new string[] { "分销商", balance.UserName, "（", balance.StoreName, "）申请提现" };
                string str2 = string.Concat(userName);
                decimal amount = balance.Amount;
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_ServiceMsg(weixinTemplateId, masterSettings, str2, "分销商提现申请", string.Concat("申请金额：￥", amount.ToString("F2"), "   提现账户：", str));
                string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(balance.UserId);
                Messenger.Send_Fuwu_ToMoreUser(str1, aliOpenIDByUserId, aliOpenIDByUserId, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_MemberAmountDrawCashRefuse(MemberAmountRequestInfo balance, string url)
        {
            string str = "MemberAmountDrawCashRefuse";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_MemberAmountDrawCashResultMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, balance, "余额提现申请失败,已驳回。立即查看>>", "驳回", url);
                string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(balance.UserId);
                Messenger.Send_Fuwu_ToMoreUser(str, aliOpenIDByUserId, aliOpenIDByUserId, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_MemberAmountDrawCashRelease(MemberAmountRequestInfo balance, string url)
        {
            string str = "MemberAmountDrawCashRelease";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_MemberAmountDrawCashResultMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, balance, "余额提现申请成功,已发放。立即查看>>", "通过", url);
                string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(balance.UserId);
                Messenger.Send_Fuwu_ToMoreUser(str, aliOpenIDByUserId, aliOpenIDByUserId, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_MemberAmountDrawCashRequest(MemberAmountRequestInfo balance)
        {
            string str = "其它";
            if (balance.RequestType == RequesType.微信钱包)
            {
                str = "微信钱包";
            }
            else if (balance.RequestType == RequesType.支付宝)
            {
                str = string.Concat("(支付宝)", balance.AccountCode);
            }
            else if (balance.RequestType == RequesType.线下支付)
            {
                str = string.Concat("(", balance.BankName, ")", balance.AccountCode);
            }
            else if (balance.RequestType == RequesType.微信红包)
            {
                str = "微信红包";
            }
            string str1 = "MemberAmountDrawCashRequest";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str1);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                string weixinTemplateId = fuwuMessageTemplateByDetailType.WeixinTemplateId;
                string str2 = string.Concat("会员", balance.UserName, "余额申请提现");
                decimal amount = balance.Amount;
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_ServiceMsg(weixinTemplateId, masterSettings, str2, "会员余额提现申请", string.Concat("申请金额：￥", amount.ToString("F2"), "   提现账户：", str));
                string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(balance.UserId);
                Messenger.Send_Fuwu_ToMoreUser(str1, aliOpenIDByUserId, aliOpenIDByUserId, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_MemberGradeChange(MemberInfo member)
        {
            string str = "MemberGradeChange";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "恭喜您的会员等级升级", "账户升级", "恭喜您成功升级，您将享受到更多的会员特权！");
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, member.AlipayOpenid, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_MemberRegister(MemberInfo member)
        {
            AliTemplateMessage aliTemplateMessage;
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType("PrizeRelease");
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                int num = 0;
                int memberNumOfTotal = (new MemberDao()).GetMemberNumOfTotal(member.ReferralUserId, out num);
                List<string> strs = new List<string>();
                if (!string.IsNullOrEmpty(member.AlipayOpenid))
                {
                    aliTemplateMessage = Messenger.GenerateFuwuMessage_PersonalMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "会员注册通知", string.Format("恭喜您，成为{0}的第{1}位会员", masterSettings.SiteName, memberNumOfTotal));
                    strs.Add(member.AlipayOpenid);
                    Messenger.Send_Fuwu_ToListUser(strs, aliTemplateMessage);
                }
                if ((member.ReferralUserId <= 0 ? false : member.ReferralUserId != member.UserId))
                {
                    string aliOpenIDByUserId = (new MemberDao()).GetAliOpenIDByUserId(member.ReferralUserId);
                    if (!string.IsNullOrEmpty(aliOpenIDByUserId))
                    {
                        aliTemplateMessage = Messenger.GenerateFuwuMessage_PersonalMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "下级会员注册通知", string.Format("恭喜您！您成功邀请到{0}成为您店铺的第{1}位下级会员！", member.UserName, num));
                        strs.Clear();
                        strs.Add(aliOpenIDByUserId);
                        Messenger.Send_Fuwu_ToListUser(strs, aliTemplateMessage);
                    }
                }
            }
        }

        public static string SendFuwuMsg_OrderCreate(OrderInfo order)
        {
            string str;
            string str1;
            string str2;
            (new OrderDao()).GetOrderUserAliOpenId(order.OrderId, out str, out str1);
            string str3 = "OrderCreate";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str3);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_OrderMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, order, string.Concat("您好，订单【", order.OrderId, "】已经提交成功。"));
                Messenger.Send_Fuwu_ToMoreUser(str3, str, str1, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
                str2 = "OK";
            }
            else
            {
                str2 = string.Concat("消息模板不存在。模板：", str3);
            }
            return str2;
        }

        public static void SendFuwuMsg_OrderDeliver(OrderInfo order)
        {
            string str;
            string str1;
            (new OrderDao()).GetOrderUserAliOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderDeliver";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str2);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_OrderMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, order, string.Concat("您好，您的订单", order.OrderId, "已经发货！"));
                Messenger.Send_Fuwu_ToMoreUser(str2, str, str1, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_OrderGetCommission(OrderInfo order, string AliOpneid, decimal CommissionAmount)
        {
            string str = "OrderGetCommission";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_OrderMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, order, string.Concat("您好，分销订单", order.OrderId, "已经完成，您获得佣金￥", CommissionAmount.ToString("F2")));
                Messenger.Send_Fuwu_ToMoreUser(str, "", AliOpneid, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_OrderGetCoupon(OrderInfo order)
        {
            string str;
            string str1;
            (new OrderDao()).GetOrderUserAliOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderGetCoupon";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str2);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_OrderMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, order, string.Concat("您好，订单", order.OrderId, "已经完成，您获得优惠券一张"));
                Messenger.Send_Fuwu_ToMoreUser(str2, str, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_OrderGetPoint(OrderInfo order, int integral)
        {
            string str;
            string str1;
            (new OrderDao()).GetOrderUserAliOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderGetPoint";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str2);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_OrderMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, order, string.Concat("您好，订单", order.OrderId, "已经完成，您获得积分", integral.ToString()));
                Messenger.Send_Fuwu_ToMoreUser(str2, str, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_OrderPay(OrderInfo order)
        {
            string str;
            string str1;
            (new OrderDao()).GetOrderUserAliOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderPay";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str2);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_OrderMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, order, string.Concat("您好，订单【", order.OrderId, "】已付款成功。请等待卖家发货！"));
                Messenger.Send_Fuwu_ToMoreUser(str2, str, str1, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_PasswordReset(MemberInfo member)
        {
            string str = "PasswordReset";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "您重置了商城账户的登录密码", "密码重置", "您成功修改了账户的登录密码，请牢记。如有问题，请联系客服。");
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_PasswordReset(MemberInfo member, string password)
        {
            string str = "PasswordReset";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "您重置了商城账户的登录密码 ", "密码重置", string.Concat("您账户修改登录密码:", password, "，请牢记。如有问题，请联系客服。"));
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_PrizeRelease(MemberInfo member, string GameTitle)
        {
            if (GameTitle == null)
            {
                GameTitle = "";
            }
            string str = "PrizeRelease";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_PersonalMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "您好，您的奖品已经发货！", string.Concat("您参与的抽奖活动【", GameTitle, "】所获得奖品已经发货，请注意收货"));
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, member.AlipayOpenid, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_ProductAsk(string ProductName, string SalerOpenId, string AskContent)
        {
            string str = "ProductAsk";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AskContent = AskContent.Replace("\r", "").Replace("\n", "");
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_ServiceMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, "您有最新的商品咨询待处理", "商品咨询", string.Concat("商品名称：", ProductName, "  咨询内容：", AskContent));
                Messenger.Send_Fuwu_ToMoreUser(str, SalerOpenId, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_ProductCreate(string ProductName)
        {
            string str = "ProductCreate";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_ServiceMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, "有新品上线了。", "新品上架提醒", string.Concat("商品名称：", ProductName));
                Messenger.Send_Fuwu_ToMoreUser(str, "", "*", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_RefundSuccess(RefundInfo refundInfo)
        {
            string str;
            string str1;
            OrderInfo orderInfo = (new OrderDao()).GetOrderInfo(refundInfo.OrderId);
            (new OrderDao()).GetOrderUserAliOpenId(refundInfo.OrderId, out str, out str1);
            string str2 = "RefundSuccess";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str2);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_RefundSuccessMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, orderInfo, refundInfo, "您的退款申请已经发放，敬请关注！");
                Messenger.Send_Fuwu_ToMoreUser(str2, str, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_ServiceRequest(OrderInfo order, int refundType)
        {
            string str;
            string str1;
            (new OrderDao()).GetOrderUserAliOpenId(order.OrderId, out str, out str1);
            string str2 = "退款";
            if (refundType == 1)
            {
                str2 = "退货";
            }
            string str3 = "ServiceRequest";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str3);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                string weixinTemplateId = fuwuMessageTemplateByDetailType.WeixinTemplateId;
                string str4 = string.Concat("买家申请", str2, "，请注意处理");
                string str5 = string.Concat("买家", str2, "申请");
                string[] refundRemark = new string[] { "买家由于：", order.RefundRemark, ",对订单", order.OrderId, " 申请", str2, "，请及时处理。" };
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_ServiceMsg(weixinTemplateId, masterSettings, str4, str5, string.Concat(refundRemark));
                Messenger.Send_Fuwu_ToMoreUser(str3, str, str1, fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendFuwuMsg_SysBindUserName(MemberInfo member, string password)
        {
            string str = "PasswordReset";
            MessageTemplate fuwuMessageTemplateByDetailType = MessageTemplateHelper.GetFuwuMessageTemplateByDetailType(str);
            if (fuwuMessageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                AliTemplateMessage aliTemplateMessage = Messenger.GenerateFuwuMessage_AccountChangeMsg(fuwuMessageTemplateByDetailType.WeixinTemplateId, masterSettings, member, "管理员为您绑定了商城账户及密码 ", "系统帐号绑定", string.Concat("您的系统账户登录密码:", password, "，请牢记。如有问题，请联系客服。"));
                Messenger.Send_Fuwu_ToMoreUser(str, member.AlipayOpenid, "", fuwuMessageTemplateByDetailType, masterSettings, true, aliTemplateMessage);
            }
        }

        public static void SendWeiXinMsg_AccountLock(MemberInfo member)
        {
            Messenger.SendWeiXinMsg_AccountLockOrUnLock(member, true);
        }

        public static void SendWeiXinMsg_AccountLockOrUnLock(MemberInfo member, bool IsLock)
        {
            (new Thread(() => Messenger.SendFuwuMsg_AccountLockOrUnLock(member, IsLock))).Start();
            string str = "AccountLock";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = null;
                templeteModel = (!IsLock ? Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "您的分销商资格已解冻", "账户解冻", "您的分销商资格账户已解冻，如有疑问，请联系客服！") : Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "您的分销商资格已被冻结", "账户冻结", "您的分销商资格已被冻结，如有疑问，请联系客服！"));
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_AccountUnLock(MemberInfo member)
        {
            Messenger.SendWeiXinMsg_AccountLockOrUnLock(member, false);
        }

        public static void SendWeiXinMsg_DistributorCancel(MemberInfo member)
        {
            (new Thread(() => Messenger.SendFuwuMsg_DistributorCancel(member))).Start();
            string str = "DistributorCancel";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "您已经被取消分销资质", "账户被取消分销商资质", "您的分销商资格已被取消，如有疑问，请联系客服！");
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_DistributorCreate(DistributorsInfo distributor, MemberInfo member)
        {
            (new Thread(() => Messenger.SendFuwuMsg_DistributorCreate(distributor, member))).Start();
            string str = "DistributorCreate";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_DistributorCreateMsg(messageTemplateByDetailType.WeixinTemplateId, distributor, member, "您好，有一位新分销商申请了店铺");
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, "", messageTemplateByDetailType, true, templeteModel);
                int num = 0;
                int distributorNumOfTotal = (new MemberDao()).GetDistributorNumOfTotal(member.ReferralUserId, out num);
                List<string> strs = new List<string>();
                if (!string.IsNullOrEmpty(member.OpenId))
                {
                    SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                    TempleteModel templeteModel1 = Messenger.GenerateWeixinMessage_DistributorCreateMsg(messageTemplateByDetailType.WeixinTemplateId, distributor, member, string.Format("恭喜您，成为{0}的第{1}位分销商", masterSettings.SiteName, distributorNumOfTotal));
                    strs.Add(member.OpenId);
                    Messenger.Send_WeiXin_ToListUser(strs, templeteModel1);
                }
                if ((member.ReferralUserId <= 0 ? false : member.ReferralUserId != member.UserId))
                {
                    string openIDByUserId = (new MemberDao()).GetOpenIDByUserId(member.ReferralUserId);
                    if (!string.IsNullOrEmpty(openIDByUserId))
                    {
                        strs.Clear();
                        strs.Add(openIDByUserId);
                        TempleteModel templeteModel2 = Messenger.GenerateWeixinMessage_DistributorCreateMsg(messageTemplateByDetailType.WeixinTemplateId, distributor, member, string.Format("恭喜您，{0}成功开店，成为您的第{1}位下级分销商", member.UserName, num));
                        Messenger.Send_WeiXin_ToListUser(strs, templeteModel2);
                    }
                }
            }
        }

        public static void SendWeiXinMsg_DistributorGradeChange(MemberInfo member, string gradeName)
        {
            (new Thread(() => Messenger.SendFuwuMsg_DistributorGradeChange(member, gradeName))).Start();
            if (!string.IsNullOrEmpty(member.OpenId))
            {
                string str = "DistributorGradeChange";
                MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
                if (messageTemplateByDetailType != null)
                {
                    TempleteModel templeteModel = Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "恭喜您成功升级！", "分销商账户升级", string.Concat("恭喜您成功升级为[", gradeName, "]，您将享受到更多的分销商特权！"));
                    Messenger.Send_WeiXin_ToMoreUser(str, "", member.OpenId, messageTemplateByDetailType, true, templeteModel);
                }
            }
        }

        public static void SendWeiXinMsg_DrawCashReject(BalanceDrawRequestInfo balance)
        {
            (new Thread(() => Messenger.SendFuwuMsg_DrawCashReject(balance))).Start();
            string str = "DrawCashReject";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                balance.Remark = string.Concat("驳回原因：", balance.Remark);
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_DrawCashResultMsg(messageTemplateByDetailType.WeixinTemplateId, balance, "分销商提现结果被驳回", "被驳回");
                Messenger.Send_WeiXin_ToMoreUser(str, balance.UserOpenId, balance.UserOpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_DrawCashRelease(BalanceDrawRequestInfo balance)
        {
            (new Thread(() => Messenger.SendFuwuMsg_DrawCashRelease(balance))).Start();
            string str = "DrawCashRelease";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_DrawCashResultMsg(messageTemplateByDetailType.WeixinTemplateId, balance, "分销商提现已通过", "通过");
                Messenger.Send_WeiXin_ToMoreUser(str, balance.UserOpenId, balance.UserOpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_DrawCashRequest(BalanceDrawRequestInfo balance)
        {
            (new Thread(() => Messenger.SendFuwuMsg_DrawCashRequest(balance))).Start();
            string str = "其它";
            if (balance.RequestType == 0)
            {
                str = "微信钱包";
            }
            else if (balance.RequestType == 1)
            {
                str = string.Concat("(支付宝)", balance.MerchantCode);
            }
            else if (balance.RequestType == 2)
            {
                str = string.Concat("(", balance.BankName, ")", balance.MerchantCode);
            }
            else if (balance.RequestType == 3)
            {
                str = "微信红包";
            }
            string str1 = "DrawCashRequest";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str1);
            if (messageTemplateByDetailType != null)
            {
                string weixinTemplateId = messageTemplateByDetailType.WeixinTemplateId;
                string str2 = string.Concat("分销商", balance.UserName, "申请提现");
                decimal amount = balance.Amount;
                DateTime now = DateTime.Now;
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_DrawscashMsg(weixinTemplateId, str2, amount, now.ToString("yyyy-MM-dd HH:mm:ss"), "申请提现", string.Concat("提现账户：", str));
                Messenger.Send_WeiXin_ToMoreUser(str1, balance.UserOpenId, balance.UserOpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_MemberAmountDrawCashRefuse(MemberAmountRequestInfo balance, string url)
        {
            (new Thread(() => Messenger.SendFuwuMsg_MemberAmountDrawCashRefuse(balance, url))).Start();
            string str = "MemberAmountDrawCashRefuse";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                MemberInfo member = (new MemberDao()).GetMember(balance.UserId);
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_MemberAmountDrawCashResultMsg(messageTemplateByDetailType.WeixinTemplateId, balance, "余额提现申请失败,已驳回。立即查看>>", "驳回", url);
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_MemberAmountDrawCashRelease(MemberAmountRequestInfo balance, string url)
        {
            (new Thread(() => Messenger.SendFuwuMsg_MemberAmountDrawCashRelease(balance, url))).Start();
            string str = "MemberAmountDrawCashRelease";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                MemberInfo member = (new MemberDao()).GetMember(balance.UserId);
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_MemberAmountDrawCashResultMsg(messageTemplateByDetailType.WeixinTemplateId, balance, "余额提现申请成功,已发放。立即查看>>", "通过", url);
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_MemberAmountDrawCashRequest(MemberAmountRequestInfo balance)
        {
            (new Thread(() => Messenger.SendFuwuMsg_MemberAmountDrawCashRequest(balance))).Start();
            string str = "其它";
            if (balance.RequestType == RequesType.微信钱包)
            {
                str = "微信钱包";
            }
            else if (balance.RequestType == RequesType.支付宝)
            {
                str = string.Concat("(支付宝)", balance.AccountCode);
            }
            else if (balance.RequestType == RequesType.线下支付)
            {
                str = string.Concat("(", balance.BankName, ")", balance.AccountCode);
            }
            else if (balance.RequestType == RequesType.微信红包)
            {
                str = "微信红包";
            }
            string str1 = "MemberAmountDrawCashRequest";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str1);
            if (messageTemplateByDetailType != null)
            {
                string weixinTemplateId = messageTemplateByDetailType.WeixinTemplateId;
                string str2 = string.Concat("会员", balance.UserName, "余额申请提现");
                decimal amount = balance.Amount;
                DateTime now = DateTime.Now;
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_DrawscashMsg(weixinTemplateId, str2, amount, now.ToString("yyyy-MM-dd HH:mm:ss"), "余额申请提现", string.Concat("提现账户：", str));
                MemberInfo member = (new MemberDao()).GetMember(balance.UserId);
                Messenger.Send_WeiXin_ToMoreUser(str1, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_MemberGradeChange(MemberInfo member)
        {
            (new Thread(() => Messenger.SendFuwuMsg_MemberGradeChange(member))).Start();
            string str = "MemberGradeChange";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "恭喜您的会员等级升级", "账户升级", "恭喜您成功升级，您将享受到更多的会员特权！");
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_MemberRegister(MemberInfo member)
        {
            TempleteModel templeteModel;
            (new Thread(() => Messenger.SendFuwuMsg_MemberRegister(member))).Start();
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType("MemberRegister");
            if (messageTemplateByDetailType != null)
            {
                int num = 0;
                int memberNumOfTotal = (new MemberDao()).GetMemberNumOfTotal(member.ReferralUserId, out num);
                List<string> strs = new List<string>();
                SiteSettings masterSettings = Messenger.GetMasterSettings();
                if (!string.IsNullOrEmpty(member.OpenId))
                {
                    templeteModel = Messenger.GenerateWeixinMessage_RegisterMsg(messageTemplateByDetailType.WeixinTemplateId, "您好，您的会员帐号注册成功！", (string.IsNullOrEmpty(member.UserBindName) ? member.UserName : member.UserBindName), string.Format("恭喜您，成为{0}的第{1}位会员", masterSettings.SiteName, memberNumOfTotal));
                    strs.Add(member.OpenId);
                    Messenger.Send_WeiXin_ToListUser(strs, templeteModel);
                }
                if ((member.ReferralUserId <= 0 ? false : member.ReferralUserId != member.UserId))
                {
                    messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType("SubMemberRegister");
                    if (messageTemplateByDetailType == null)
                    {
                        return;
                    }
                    string openIDByUserId = (new MemberDao()).GetOpenIDByUserId(member.ReferralUserId);
                    if (!string.IsNullOrEmpty(openIDByUserId))
                    {
                        templeteModel = Messenger.GenerateWeixinMessage_JuniorRegisterMsg(messageTemplateByDetailType.WeixinTemplateId, "您好，有新的下级会员注册！", member.UserName, string.Format("恭喜您！您成功邀请到{0}成为您店铺的第{1}位下级会员！", member.UserName, num));
                        strs.Clear();
                        strs.Add(openIDByUserId);
                        Messenger.Send_WeiXin_ToListUser(strs, templeteModel);
                    }
                }
            }
        }

        public static string SendWeiXinMsg_OrderCreate(OrderInfo order)
        {
            string str;
            string str1;
            string str2;
            (new Thread(() => Messenger.SendFuwuMsg_OrderCreate(order))).Start();
            (new OrderDao()).GetOrderUserOpenId(order.OrderId, out str, out str1);
            string str3 = "OrderCreate";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str3);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_OrderMsg(messageTemplateByDetailType.WeixinTemplateId, order, string.Concat("您好，订单【", order.OrderId, "】已经提交成功。"));
                Messenger.Send_WeiXin_ToMoreUser(str3, str, str1, messageTemplateByDetailType, true, templeteModel);
                str2 = "OK";
            }
            else
            {
                str2 = string.Concat("消息模板不存在。模板：", str3);
            }
            return str2;
        }

        public static void SendWeiXinMsg_OrderDeliver(OrderInfo order)
        {
            string str;
            string str1;
            (new Thread(() => Messenger.SendFuwuMsg_OrderDeliver(order))).Start();
            (new OrderDao()).GetOrderUserOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderDeliver";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str2);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_OrderMsg(messageTemplateByDetailType.WeixinTemplateId, order, string.Concat("您好，您的订单", order.OrderId, "已经发货！"));
                Messenger.Send_WeiXin_ToMoreUser(str2, str, str1, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_OrderGetCommission(OrderInfo order, string WxOpenId, string AliOpneid, decimal CommissionAmount)
        {
            AliOHHelper.log(string.Concat("AliOpneid:", AliOpneid));
            (new Thread(() => Messenger.SendFuwuMsg_OrderGetCommission(order, AliOpneid, CommissionAmount))).Start();
            string str = "OrderGetCommission";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_OrderMsg(messageTemplateByDetailType.WeixinTemplateId, order, string.Concat("您好，分销订单", order.OrderId, "已经完成，您获得佣金￥", CommissionAmount.ToString("F2")));
                Messenger.Send_WeiXin_ToMoreUser(str, "", WxOpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_OrderGetCoupon(OrderInfo order)
        {
            string str;
            string str1;
            (new Thread(() => Messenger.SendFuwuMsg_OrderGetCoupon(order))).Start();
            (new OrderDao()).GetOrderUserOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderGetCoupon";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str2);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_OrderMsg(messageTemplateByDetailType.WeixinTemplateId, order, string.Concat("您好，订单", order.OrderId, "已经完成，您获得优惠券一张"));
                Messenger.Send_WeiXin_ToMoreUser(str2, str, "", messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_OrderGetPoint(OrderInfo order, int integral)
        {
            string str;
            string str1;
            (new Thread(() => Messenger.SendFuwuMsg_OrderGetPoint(order, integral))).Start();
            (new OrderDao()).GetOrderUserOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderGetPoint";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str2);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_OrderMsg(messageTemplateByDetailType.WeixinTemplateId, order, string.Concat("您好，订单", order.OrderId, "已经完成，您获得积分", integral.ToString()));
                Messenger.Send_WeiXin_ToMoreUser(str2, str, "", messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_OrderPay(OrderInfo order)
        {
            string str;
            string str1;
            (new Thread(() => Messenger.SendFuwuMsg_OrderPay(order))).Start();
            (new OrderDao()).GetOrderUserOpenId(order.OrderId, out str, out str1);
            string str2 = "OrderPay";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str2);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_OrderMsg(messageTemplateByDetailType.WeixinTemplateId, order, string.Concat("您好，订单【", order.OrderId, "】已付款成功。请等待卖家发货！"));
                Messenger.Send_WeiXin_ToMoreUser(str2, str, str1, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_PasswordReset(MemberInfo member)
        {
            (new Thread(() => Messenger.SendFuwuMsg_PasswordReset(member))).Start();
            string str = "PasswordReset";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "您重置了商城账户的登录密码", "密码重置", "您成功修改了账户的登录密码，请牢记。如有问题，请联系客服。");
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, "", messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_PasswordReset(MemberInfo member, string password)
        {
            (new Thread(() => Messenger.SendFuwuMsg_PasswordReset(member, password))).Start();
            string str = "PasswordReset";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "您重置了商城账户的登录密码 ", "密码重置", string.Concat("您账户修改登录密码:", password, "，请牢记。如有问题，请联系客服。"));
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, "", messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_PrizeRelease(MemberInfo member, string GameTitle, string prizeName)
        {
            string gameTitle = GameTitle;
            (new Thread(() => Messenger.SendFuwuMsg_PrizeRelease(member, gameTitle))).Start();
            if (gameTitle == null)
            {
                gameTitle = "";
            }
            string str = "PrizeRelease";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_PrizeMsg(messageTemplateByDetailType.WeixinTemplateId, "您好，您的奖品已经发货", string.Concat("活动【", gameTitle, "】"), prizeName, string.Concat("您参与的抽奖活动【", gameTitle, "】所获得奖品已经发货，请注意收货"), "");
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, member.OpenId, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_ProductAsk(string ProductName, string SalerOpenId, string AskContent, string NickName)
        {
            string askContent = AskContent;
            (new Thread(() => Messenger.SendFuwuMsg_ProductAsk(ProductName, SalerOpenId, askContent))).Start();
            string str = "ProductAsk";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                askContent = askContent.Replace("\r", "").Replace("\n", "");
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_ConsultMsg(messageTemplateByDetailType.WeixinTemplateId, "您有最新的商品咨询待处理", NickName, askContent, string.Concat("商品名称：", ProductName));
                Messenger.Send_WeiXin_ToMoreUser(str, SalerOpenId, "", messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_ProductCreate(string ProductName, decimal price)
        {
            (new Thread(() => Messenger.SendFuwuMsg_ProductCreate(ProductName))).Start();
            string str = "ProductCreate";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                SiteSettings masterSettings = Messenger.GetMasterSettings();
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_ProductMsg(messageTemplateByDetailType.WeixinTemplateId, "有新品上线了", masterSettings.SiteName, ProductName, price, "新品上架提醒");
                Messenger.Send_WeiXin_ToMoreUser(str, "", "*", messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_RefundSuccess(RefundInfo refundInfo)
        {
            string str;
            string str1;
            (new Thread(() => Messenger.SendFuwuMsg_RefundSuccess(refundInfo))).Start();
            OrderInfo orderInfo = (new OrderDao()).GetOrderInfo(refundInfo.OrderId);
            (new OrderDao()).GetOrderUserOpenId(refundInfo.OrderId, out str, out str1);
            string str2 = "RefundSuccess";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str2);
            if (messageTemplateByDetailType != null)
            {
                SettingsManager.GetMasterSettings(true);
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_RefundSuccessMsg(messageTemplateByDetailType.WeixinTemplateId, orderInfo, refundInfo, "您的退款申请已经发放，敬请关注！");
                if (templeteModel != null)
                {
                    Messenger.Send_WeiXin_ToMoreUser(str2, str, "", messageTemplateByDetailType, true, templeteModel);
                }
            }
        }

        public static void SendWeiXinMsg_ServiceRequest(OrderInfo order, int refundType)
        {
            string str;
            string str1;
            (new Thread(() => Messenger.SendFuwuMsg_ServiceRequest(order, refundType))).Start();
            (new OrderDao()).GetOrderUserOpenId(order.OrderId, out str, out str1);
            string str2 = "退款";
            if (refundType == 1)
            {
                str2 = "退货";
            }
            string str3 = "ServiceRequest";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str3);
            if (messageTemplateByDetailType != null)
            {
                string itemDescription = "";
                KeyValuePair<string, LineItemInfo> keyValuePair = order.LineItems.FirstOrDefault<KeyValuePair<string, LineItemInfo>>((KeyValuePair<string, LineItemInfo> t) => (t.Value.OrderItemsStatus == OrderStatus.ApplyForRefund ? true : t.Value.OrderItemsStatus == OrderStatus.ApplyForReturns));
                if (keyValuePair.Value != null)
                {
                    itemDescription = keyValuePair.Value.ItemDescription;
                }
                string weixinTemplateId = messageTemplateByDetailType.WeixinTemplateId;
                string str4 = string.Concat("买家申请", str2, "，请注意处理");
                string orderId = order.OrderId;
                string str5 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string[] refundRemark = new string[] { "买家由于：", order.RefundRemark, ",对订单", order.OrderId, " 申请", str2, "，请及时处理。" };
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_ServiceMsg(weixinTemplateId, str4, str2, itemDescription, orderId, str5, string.Concat(refundRemark));
                Messenger.Send_WeiXin_ToMoreUser(str3, str, str1, messageTemplateByDetailType, true, templeteModel);
            }
        }

        public static void SendWeiXinMsg_SysBindUserName(MemberInfo member, string password)
        {
            (new Thread(() => Messenger.SendFuwuMsg_SysBindUserName(member, password))).Start();
            string str = "PasswordReset";
            MessageTemplate messageTemplateByDetailType = MessageTemplateHelper.GetMessageTemplateByDetailType(str);
            if (messageTemplateByDetailType != null)
            {
                TempleteModel templeteModel = Messenger.GenerateWeixinMessage_AccountChangeMsg(messageTemplateByDetailType.WeixinTemplateId, member, "管理员为您绑定了商城账户及密码", "系统帐号绑定", string.Concat("您的系统账户登录密码:", password, "，请牢记。如有问题，请联系客服。"));
                Messenger.Send_WeiXin_ToMoreUser(str, member.OpenId, "", messageTemplateByDetailType, true, templeteModel);
            }
        }
    }
}