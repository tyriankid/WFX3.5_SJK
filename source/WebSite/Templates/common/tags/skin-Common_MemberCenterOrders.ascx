<%@ Control Language="C#" %>
<%@ Import Namespace="Hidistro.Core" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<div class="orderlist">
    <div class="orderinfo">
        <p>订单编号：<%#Eval("OrderId") %></p>
        <%--<p>订单日期：<%# Eval("OrderDate","{0:d}")%><a href="#" style="float:right;">配送详情</a></p>--%>
        <p class="zt"><Hi:OrderStatusLabel ID="OrderStatusLabel1" IsShowToUser="true" Gateway='<%#Eval("Gateway") %>' OrderStatusCode='<%# Eval("OrderStatus") %>' runat="server" /></p>
        <%--<span class="price">￥<em><%# Eval("OrderTotal","{0:F2}")%></em></span>--%>
        <span class="price"><a style='<%# Eval("orderstatus").ToString()=="1"?"display:block":"display:none" %>' href="<%# Globals.ApplicationPath + "/Vshop/FinishOrder.aspx?PaymentType=1&OrderId=" +Eval("OrderMarking")%>">立即付款</a><a href="<%# Globals.ApplicationPath + "/Vshop/Orderbycycle.aspx?xdOrderId=" +Eval("OrderId")%>" style='<%# Eval("orderstatus").ToString()=="2"?"display:block":"display:none" %>'>我要续订</a><a href="#" style="display:none;">申请退款</a></span>
    </div>
    <asp:Repeater ID="rporderitems" runat="server" DataSource='<%# Eval("OrderItems") %>'>
        <ItemTemplate>
            <div class="orderimg">
                <Hi:ListImage ID="ListImage1" runat="server" DataField="ThumbnailsUrl" />
                <div class="orderimginfo">
                    <a href="<%# Globals.ApplicationPath + "/Vshop/MemberOrderDetails.aspx?OrderId=" + Eval("OrderId") %>">
                        <div class="name bcolor">
                            <%#Hidistro.SaleSystem.Vshop.VshopBrowser.GetLimitedTimeDiscountNameStr(Globals.ToNum(Eval("LimitedTimeDiscountId"))) %> <%# Eval("ItemDescription")%>
                            <%#Eval("OrderItemsStatus").ToString()=="9"?("<span class='text-danger'>(已退款，金额￥"+decimal.Parse( Eval("ReturnMoney").ToString()).ToString("F2"))+")</span>":"" %>
                            <%#Eval("OrderItemsStatus").ToString()=="10"?("<span class='text-danger'>(已退货，金额￥"+decimal.Parse( Eval("ReturnMoney").ToString()).ToString("F2"))+")</span>":"" %>
                            <span>￥4.5</span>
                        </div>
                    </a>
                    <div class="specification">
                        <input type="hidden" value="<%# Eval("SkuContent")%>" />
                    </div>
                        <div class="ordertime">
                            <p>收货人：张三<span class="sl"><%#Eval("QuantityPerDay") %></span></p>
                            <p style="display:none">数量：<span class="sl" role="sl_<%# Eval("OrderId")%>" totalCount="<%#Convert.ToInt32(Eval("QuantityPerDay"))*Convert.ToInt32(Eval("SendDays")) %>"><%#Eval("QuantityPerDay") %></span></p>
                            <%--<p>配送模式：每日送</p>--%>
                            <p>配送时间：<%#((DateTime)Eval("SendStartDate")).ToString("yyyy-MM-dd") %>至<%#((DateTime)Eval("SendEndDate")).ToString("yyyy-MM-dd") %></p>             
                            <%--<p class="xq"><a href="#">配送详情(已配送1，未配送29)</a></p>--%>
                        </div>
                        <div class="orderreturn">
                             <%//增加了单选付款金额是否为0的判断（ItemAdjustedPrice==DiscountAverage），为0则不显示退货和退款按钮 %>   <%#(Eval("ItemAdjustedPrice","{0:F2}")!=Eval("DiscountAverage","{0:F2}"))? ( int.Parse(Eval("OrderItemsStatus").ToString().Trim()) == (int)Hidistro.Entities.Orders.OrderStatus.BuyerAlreadyPaid ? ( "<button class=\"btn btn-default btn-xs \" orderid=\""+Eval("OrderID")+"\" skuid=\""+Eval("SkuID")+"\" onclick=\"urllink('" + Eval("ID") + "','" + Eval("OrderId") + "',"+Eval("Type")+")\" typeid='"+Eval("Type")+"'>申请退款</button>") : int.Parse(Eval("OrderItemsStatus").ToString().Trim()) == (int)Hidistro.Entities.Orders.OrderStatus.SellerAlreadySent ? "<button class=\"btn btn-default btn-xs waittochangestatus\"  orderid=\""+Eval("OrderID")+"\" skuid=\""+Eval("SkuID")+"\" orderitemid=\""+Eval("ID")+"\" onclick=\"urllink('" + Eval("ID") + "','" + Eval("OrderId") + "',"+Eval("Type")+")\" typeid='"+Eval("Type")+"'>申请退货</button>" : int.Parse(Eval("OrderItemsStatus").ToString().Trim()) == (int)Hidistro.Entities.Orders.OrderStatus.ApplyForRefund ? "<button class=\"btn btn-info btn-xs\" >已申请退款</button>" : int.Parse(Eval("OrderItemsStatus").ToString().Trim()) == (int)Hidistro.Entities.Orders.OrderStatus.ApplyForReturns?"<button class=\"btn btn-info btn-xs\" >已申请退货</button>":""):""%>
                        </div>
                    
                </div>            
                <div class="calendar"><a href="<%# Globals.ApplicationPath + "/Vshop/QuestList.aspx?OrderId=" + Eval("OrderId") %>">订单日历</a></div>
            </div>
            <div></div>
        </ItemTemplate>
    </asp:Repeater>
    <div class="linkbtn" style="height: 36px;"><p class="Total" ><span role="total_<%# Eval("OrderId")%>">共配送0件商品</span> <span>订单总额：￥<%# decimal.Parse(Eval("OrderTotal").ToString()).ToString("F2") %></span></p>
        <%#(Eval("PayDate")==DBNull.Value&&(int)Eval("OrderStatus") == 1&&decimal.Parse(Eval("BalancePayMoneyTotal").ToString())>0)?" <span style='color:red;margin-right:3px;'>待支付：￥"+(decimal.Parse(Eval("OrderTotal").ToString())-decimal.Parse(Eval("BalancePayMoneyTotal").ToString())).ToString("F2")+" </span>":"" %>
        <%/*# ((int)Eval("OrderStatus") == 3 || (int)Eval("OrderStatus") == 5) ? "<a href='"+Globals.ApplicationPath + "/Vshop/MyLogistics.aspx?OrderId=" + Eval("OrderId")+"' class='btn btn-info btn-xs'>查看物流</a>" : ""*/%>
       <%-- <%# (int)Eval("OrderStatus") == 1&&(int)Eval("PaymentTypeId")!=99&&(int)Eval("PaymentTypeId")!=0&&(string)Eval("GateWay")!="hishop.plugins.payment.bankrequest"&&(string)Eval("GateWay")!="hishop.plugins.payment.podrequest"? "<a href='"+ Globals.ApplicationPath + "/Vshop/FinishOrder.aspx?PaymentType=1&OrderId=" + Eval("OrderMarking")+"' class='btn btn-danger btn-xs'>去付款</a> " : ""%>--%>
        <%/*# (int)Eval("OrderStatus") == 3 ? "<a href='javascript:void(0)' onclick=\"FinishOrder('"+Eval("OrderId")+"')\" class='btn btn-danger btn-xs'>确认收货</a>" : ""*/%>
        <%# (int)Eval("PaymentTypeId")==99&&(int)Eval("OrderStatus")==1 ? "<a class='btn btn-warning btn-xs' onclick='CancelOrder(\""+Eval("OrderId")+"\")'>取消订单</a> " : ""%>
        <%#(Eval("HasRedPage")).ToString()=="1"?"<a href='/Vshop/GetRedShare.aspx?orderid="+Eval("OrderId")+"' class='btn btn-warning btn-xs btn-danger'>发钱咯</a>":"" %>
        
    </div>
    <script>
        var orderid =<%# Eval("OrderId")%>;
        $("[role='total_" + orderid + "']").html("共配送" + $("[role='sl_" + orderid + "']").attr("totalCount")+"件商品")
    </script>
</div>