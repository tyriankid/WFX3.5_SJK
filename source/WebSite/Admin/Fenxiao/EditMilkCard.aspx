<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminNew.Master" AutoEventWireup="true" CodeBehind="EditMilkCard.aspx.cs" Inherits="Hidistro.UI.Web.Admin.Fenxiao.EditMilkCard" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.ControlPanel.Utility" Assembly="Hidistro.UI.ControlPanel.Utility" %>
<%@ Register TagPrefix="UI" Namespace="ASPNET.WebControls" Assembly="ASPNET.WebControls" %>
<%@ Import Namespace="Hidistro.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
            function ShowProduct() {
                $DialogFrame_ReturnValue = "";// 返回值，IsMultil=1可多选
            DialogFrame("/Admin/Oneyuan/ProductSelect.aspx?IsMultil=0", "选择商品", 680, 500, function (rs) {
                if (rs != "") {
                    var rsArray = rs.split("^");
                    if (rsArray.length == 7) {
                        //获取到正确的数值44^男鞋^8.00^8.00^/Storage/master/product/thumbs60/60_b1d772be0a2b4f469cca08044c1c4c48.jpg^3000^0
                        var imageUrl = rsArray[4];
                        if (rsArray[4] == "")
                            imageUrl = "/utility/pics/none.gif";

                        var phtml = '  <div class="shop-img fl">' +
                        '  <img src="' + imageUrl + '" width="60" style="height:60px!important" /></div>' +
                        '  <div class="shop-username fl ml10">' +
                        '   <p style="color:#222">' + rsArray[1] + '</p></div>' +
                        '  <p class="fl ml20">现价：￥' + rsArray[2] + '</p>' +
                        '  <p class="fl ml20">库存：' + rsArray[5] + '</p>';
                        $("#productInfo").html(phtml);
                        ///赋值
                        $("#ctl00_ContentPlaceHolder1_hiddProductId").val(rsArray[0]);
                        $("#ctl00_ContentPlaceHolder1_productImage").attr("src", imageUrl)
                        $("#ctl00_ContentPlaceHolder1_lbProductName").html(rsArray[1]);
                    }
                }
            });
        }





    $(function(){
    
    
        $("#Distributorform").formvalidation({
            //'submit': '#ctl00_ContentPlaceHolder1_PassCheck',
            'ctl00$ContentPlaceHolder1$txtFreeSendDays': {
                validators: {
                    notEmpty: {
                        message: '不能为空'
                    },
                    regexp: {
                        regexp: /^\d+(\.\d+)?$/,
                        message: '数据类型错误，只能输入实数型数值'
                    }
                }
            },
            'ctl00$ContentPlaceHolder1$txtCardCount': {
                validators: {
                    notEmpty: {
                        message: '不能为空'
                    },
                    regexp: {
                        regexp: /^\d+(\.\d+)?$/,
                        message: '数据类型错误，只能输入实数型数值'
                    }
                }
            },
            'ctl00$ContentPlaceHolder1$txtFreeQuantityPerDay': {
                validators: {
                    notEmpty: {
                        message: '佣金百分比不能为空'
                    },
                    regexp: {
                        regexp: /^[0-9]\d{0,1}(\.\d+)?$/,
                        message: '数据类型错误，100以内数值'
                    }
                }
            },
           
        });


    });
   </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="page-header">
            <h2 id="EditTitle" runat="server"><%=htmlOperatorName %>奶卡</h2>
   </div>

    <form runat="server">
          <div class="form-horizontal" id="Distributorform">

               <div class="form-group">
                        <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>站点：</label>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlSite" runat="server"></asp:DropDownList>
                        </div>
                </div>
                <div class="form-group">
                        <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>生成卡数量：</label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtCardCount" runat="server" CssClass="form-control  inputw120"></asp:TextBox>
                        </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-2 pad resetSize control-label" for="pausername"><em>*</em>&nbsp;&nbsp;选择商品：</label>
                    <div class="form-inline col-xs-9 pt3">
                        <a href="javascript:ShowProduct()">点击选择</a>&nbsp;&nbsp;&nbsp;&nbsp;
                            &nbsp; &nbsp; 选择的多规格商品每个规格的价格必须相同 
                    </div>
                </div>
                <div class="form-group ">
                    <label class="col-xs-3 pad resetSize control-label" for="pausername">&nbsp;&nbsp;</label>
                    <div class="form-inline col-xs-9">
                        <asp:HiddenField runat="server" ID="hiddProductId" />
                        <div class="y3-prize-info clearfix" id="productInfo">
                            <%= productInfoHtml %>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                        <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>免费配送天数：</label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtFreeSendDays" runat="server" CssClass="form-control  inputw120"></asp:TextBox>
                        </div>
                </div>
                <div class="form-group">
                        <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>每天配送件数：</label>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtFreeQuantityPerDay" runat="server" CssClass="form-control  inputw120"></asp:TextBox>
                        </div>
                </div>


               <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-10">
                             <asp:Button ID="btnEditUser" runat="server" Text="确 定"  CssClass="btn btn-success" />
                        </div>
                 </div>

       


      </div>














    </form>

</asp:Content>
