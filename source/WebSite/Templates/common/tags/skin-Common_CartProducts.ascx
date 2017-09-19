<%@ Control Language="C#" %>
<%@ Import Namespace="Hidistro.Core" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<style>
    .GoodsBox{
        padding:10px;
        width:100%;
        /*display:none;*/
    }
    .kangbox{
        border: 1px solid #EC4B00;
        height:auto;
    }
    .kangbox .title{
        height: 30px;
        line-height: 30px;
        background: #EC4B00;
        color:#FFF;
        padding:0 10px;
    }
    .content{
        margin-top:10px;
        height:208px;
    }
    .content .content-left{
        width:30%;
        float:left;
    }
    .content .content-left img{
        width:80px;
        height:80px;
    }
    .content .content-right{
        width:60%;
        float:right;
    }
    .content .content-right p{
       
        line-height:20px;
    }
    .kangbox .zong{
        width:100%;
        height: 35px;
        line-height: 35px;
        background: #FBE2E2;
        color:#000;
        padding:0 10px;
        margin-top:20px;
        text-align:right;
    }
</style>
<div id="cartProducts" class="well shopcart">
<asp:Literal ID="litpromotion" runat="server"></asp:Literal> 

<asp:Repeater ID="rptCartProduct" runat="server" DataSource='<%# Eval("LineItems") %>' >
    <ItemTemplate>
    <!--<hr style="margin:0 0px 0 0px;">-->
        <div class="GoodsBox">
            <div class="kangbox">
                <p class="title">订购商品</p>
                <div class="content">
                    <div class="content-left">
                        <img  src="/Utility/pics/logo2.jpg"  />
                    </div>
                    <div class="content-right" style="width:60%;float:right;">
                        <h6 style="font-weight:bold;font-size:14px;">三剑客牧场鲜牛奶</h6>
                        <p>订购类型：<span>正常订购</span></p>
                        <p>起订日期：<span>2017-09-03</span></p>
                        <p>截止日期：<span>2017-10-02</span></p>
                        <p>送奶时间段：<span>下午送奶</span></p>
                        <p>单价（RMB）：<span>4.00</span></p>
                        <p>每天订奶数量：<span>1</span></p>
                        <p>总天数：<span>30</span></p>
                        <p>总数量：<span>30</span></p>
                        <p>金额：<span>￥120.00</span></p>
                        <p id="delete" style="text-align:right;margin-right:20px;background:#CCC;display:inline-block;float:right;padding:2px 10px;">移除</p>
                    </div>  
                   
                </div> 
                <div class="zong"><p >总金额：<span style="color:red;font-size:14px;">120￥</span></p></div>
            </div>
        </div>
    </ItemTemplate>
    
</asp:Repeater>
<div><asp:Literal ID="litline" runat="server"></asp:Literal> </div>

</div>
