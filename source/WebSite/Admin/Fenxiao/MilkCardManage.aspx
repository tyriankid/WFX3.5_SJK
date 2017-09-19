<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminNew.Master" AutoEventWireup="true" Inherits="Hidistro.UI.Web.Admin.Fenxiao.MilkCardManage" %>

<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.ControlPanel.Utility" Assembly="Hidistro.UI.ControlPanel.Utility" %>
<%@ Register TagPrefix="UI" Namespace="ASPNET.WebControls" Assembly="ASPNET.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">

        $(function () {
          

            $('#ctl00_ContentPlaceHolder1_RegionSelector1').find('select').each(function () {
                $(this).removeClass();
                $(this).addClass('form-control inl autow mr5 resetSize');
            });
        });
        function SendCard(obj) {
            if ($('input[role="chkCard"]:checked').length < 1) {
                //alert("请先选择要修改用户等级的分销商！");
                HiConform('<strong>请先选择要发放的奶卡！</strong>', obj);
                return;
            }
            var cardids = "";
            $('input[role="chkCard"]:checked').each(function () {
                cardids += $(this).attr("value")+",";
            });
            cardids = cardids.substr(0, cardids.length - 1);
            DialogFrame("../settings/SendMilkCards.aspx?cardids=" + cardids, "发放奶卡", 680, 410, function () { location.href = "<%=localUrl%>"; });
        }

        function reset()
        {

            
            $("#ctl00_ContentPlaceHolder1_txtStreetName").val("");
        }
        function ShowEditStreetInfos(StreetId, obj) {

            var temp = $(obj).attr("data");

            $("#streetid").val(StreetId);
            $("#StreetNewName").val(temp);


            $('#EditDistributorInfos').modal('toggle').children().css({
                width: '600px', top: "130px"
            });

        }
        function editStreetName(obj) {
            var StreetNewName = $("#StreetNewName").val();

            if (StreetNewName == "") {
                HiConform('<strong>请设置街道名称！</strong>', obj);
                return;
                
            }

            $.ajax({
                type: "POST",   //访问WebService使用Post方式请求
                contentType: "application/json", //WebService 会返回Json类型
                url: "Scope.aspx/EditStreetName", //调用WebService的地址和方法名称组合 ---- WsURL/方法名
                data: "{id:'" + $("#streetid").val() + "',streetName:'" + StreetNewName + "'}",         //这里是要传递的参数，格式为 data: "{paraName:paraValue}",下面将会看到
                dataType: 'json',
                success: function (result) {     //回调函数，result，返回值
                    if (result.d == "success") {
                        location.reload();
                    }
                }
            });
        }



        function doSearch() {
            $("#ctl00_contentHolder_hidRegionCode").val($("#regionSelectorValue").val());
            return true;
        }

        function doEdit(streetId) {
            //alert(streetId);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <style type="text/css">
        #RemarkOrder .modal-body input.form-control {
            display: inline-block;
        }        
        #liOper2 em {
            font-style: normal;
            color: #1CA47D;
        }

        #liOper6 em {
            font-style: normal;
            color: red;
        }
    </style>
    <div class="page-header">
        <h2>奶卡管理</h2>
    </div>
    <form runat="server">
      <div class="set-switch">
                       
                        <div class="form-inline mb10">
                            <div class="form-group mr20">
                                <label for="sellshop1">　站点：</label>
                              <!-- <asp:TextBox ID="txtStreetName" runat="server" CssClass="form-control resetSize inputw150" />-->
                                <asp:DropDownList ID="ddlSite" runat="server"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                   
                    <asp:Button ID="btnSearchButton" runat="server" class="btn resetSize btn-primary" OnClientClick="return doSearch()" Text="查询" />
                <a href="EditMilkCard.aspx" class="btn btn-info">添加</a>
                            </div>
                              <div class="form-group">
     
                </div>
 <div class="form-group mr20">  
                                <a class="bl mb5"  onclick="reset()" style="cursor: pointer;">清除条件</a>
                            </div>
                        </div>
                    </div>

        <!--数据列表-->

        <asp:Repeater ID="grdMilkCards" runat="server" OnItemCommand="grdMilkCards_ItemCommand">
            <HeaderTemplate>
                <div>
                    <table class="table table-hover mar table-bordered" style="table-layout: fixed">
                        <thead>
                            <tr>
                                <button type="button" class="btn resetSize btn-primary " onclick="SendCard(this)">发送奶卡</button>
                                <th style="width:30px;"></th>
                                <th>卡号</th>
                                <th>密码</th>
                                <th>使用状态</th>
                                <th>用户</th>
                                <th style="width:130px;">绑定商品</th>
                                <th>创建日期</th>
                                <th style="width:110px;">免费配送天数</th>
                                <th style="width:110px;">每天配送件数</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                </tr>
                <td><input name="CheckBoxGroup" role="chkCard" class="fl" type="checkbox" value='<%#Eval("ID") %>' /></td>
               <td>
                   <asp:Label ID="lblCardNum" runat="server" Text='<%# Bind("CardNum") %>'></asp:Label>
               </td>
                <td>
                    <asp:Label ID="lblCardPassword" runat="server" Text='<%# Bind("CardPassword") %>'></asp:Label>
                </td>
                <td>
                    <%# Eval("Status").ToString()=="0"?"未使用": Eval("Status").ToString()=="1"?"已使用":"已冻结"%>
                </td>
                <td>
                    <%#string.IsNullOrEmpty(Eval("UserId").ToString())?"未发放":Eval("UserName") %>
                </td>
                <td>
                    <img style="width:30px;height:30px" src="<%# Eval("ImageUrl1").ToString()%>" '/>  <%# Eval("ProductName").ToString()%>
                </td>
                <td>
                    <%# DateTime.Parse(Eval("CreateDate").ToString())%>
                </td>
                <td>
                    <%#Eval("FreeSendDays") %>
                </td>
                <td>
                    <%#Eval("FreeQuantityPerDay") %>
                </td>
                <td>
                    <asp:Button ID="lbtnDel" CssClass="btnLink" CommandArgument='<%#Eval("ID") %>' runat="server" Text="删除" CommandName="Delete"
                        OnClientClick="return HiConform('<strong>确定要删除这张奶卡吗？</strong><p>删除后不可恢复！</p>',this)" /></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
                 </div>
            </FooterTemplate>
        </asp:Repeater>

        <!--数据列表底部功能区域-->
        <br />
        <div class="select-page clearfix">

            <div class="page fr">
                <div class="pageNumber">
                    <div class="pagination" style="margin: 0px">
                        <UI:Pager runat="server" ShowTotalPages="true" ID="pager1" />
                    </div>
                </div>
            </div>
        </div>




    </form>


</asp:Content>
