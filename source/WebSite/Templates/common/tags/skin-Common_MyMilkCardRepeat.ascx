<%@ Control Language="C#" %>
<%@ Import Namespace="Hidistro.Core" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<li>
    <div class="coupon-left">                       
        <p>卡号：<%#Eval("CardNum") %></p>
        <p>每天配送：<%#Eval("FreeQuantityPerDay") %>瓶</p>   
        <p>可配送：<%#Eval("FreeSendDays") %>天 <a href="submmitorder.aspx?productSku=<%#Eval("ProductId") %>_0&from=signBuy&buyAmount=0&cardid=<%#Eval("id") %>&quantityPerDay=<%#Eval("FreeQuantityPerDay") %>&startDate=<%#Eval("startSendDate") %>&sendDays=<%#Eval("FreeSendDays") %>">下单</a></p>
    </div>
    <div class="coupon-right">                      
        <p>发放日期：<%#Eval("CreateDate") %></p>
    </div>
</li>