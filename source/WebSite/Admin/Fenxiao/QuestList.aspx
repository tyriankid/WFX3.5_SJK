<%@ Page Language="C#" Title="" AutoEventWireup="true"   MasterPageFile="~/Admin/AdminNew.Master" Inherits="Hidistro.UI.Web.Admin.Fenxiao.QuestList" %>

<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.ControlPanel.Utility" Assembly="Hidistro.UI.ControlPanel.Utility" %>
<%@ Register TagPrefix="UI" Namespace="ASPNET.WebControls" Assembly="ASPNET.WebControls" %>
<%@ Import Namespace="Hidistro.Core" %>
<%@ Register Src="../Ascx/ucDateTimePicker.ascx" TagName="DateTimePicker" TagPrefix="Hi" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/utility/skins/blue.css" type="text/css" media="screen" />
    <Hi:Script ID="Script5" runat="server" Src="/utility/jquery.artDialog.js" />
    <Hi:Script ID="Script6" runat="server" Src="/utility/Window.js" />
    
  
    <style type="text/css">
        /*.selectthis {border-color:red; color:red; border:1px solid;}*/
        .tdClass{text-align:center;}
        .labelClass{margin-right:10px;}
        .thCss{text-align:center;}
        .selectthis{border:1px solid;border-color:#999999; color:#c93027;margin-right:2px;}
        .selectthis:hover {border:1px solid;border-color:#999999; color:#c93027;margin-right:2px;}
        .aClass{border:1px solid;border-color:#999999; color:#999999;margin-right:2px;}
        .aClass:hover{border:1px solid;border-color:#999999; color:#999999;margin-right:2px;}
        #datalist td{word-break: break-all;}
        #ctl00_ContentPlaceHolder1_grdMemberList th {margin:0px;border-left:0px;border-right:0px;background-color:#F7F7F7;text-align:center; vertical-align:middle;}
        #ctl00_ContentPlaceHolder1_grdMemberList td {margin:0px;border-left:0px;border-right:0px;text-align:center;vertical-align:middle;}
        .table-bordered > thead > tr > th {
        border:none;}
        .table-bordered {border-right: none;border-left: none;}
        .bl mb5 {cursor: pointer
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="thisForm" runat="server" class="form-horizontal">                     
        <div>
            <div class="page-header">
                <h2>任务列表</h2>
            </div>
        <!--搜索-->

        <!--数据列表区域-->
        <div>
            <ul class="nav nav-tabs">
                <li dataID="normal"><a href="/admin/Fenxiao/QuestList.aspx"><asp:Literal ID="ListActive" Text="配送任务列表" runat="server"></asp:Literal></a></li>
                <li dataID="new"><a href="/admin/Fenxiao/QuestList.aspx?clientType=new"><asp:Literal ID="Listfrozen" Text="按订单查看"  runat="server"></asp:Literal></a></li>
                <li dataID="activy"><a href="/admin/Fenxiao/QuestList.aspx?clientType=activy"><asp:Literal ID="Literal1" Text="按客户查看"  runat="server"></asp:Literal></a></li>
               
            </ul>

            <div class="form-inline mb10">
                 <div class="set-switch">
                    <div class="form-inline  mb10">
                        <div class="form-group mr20" style="margin-left:0px;">
                            <label for="sellshop1" class="ml10">订单号：</label>
                            <asp:TextBox ID="txtOrderId" CssClass="form-control resetSize" runat="server" /> 
                        </div>
                     
                        <div class="form-group" style ="">
                            <label class="ml10">配送状态：</label>
                            <asp:DropDownList  ID="ddlStatus" runat="server" CssClass="form-control  resetSize inputw150">
                                 <asp:ListItem Text="全部" Value="" Selected="True">全部</asp:ListItem>
                                <asp:ListItem Text="未配送" Value="0" >未配送</asp:ListItem>
                                <asp:ListItem Text="已配送" Value="1">已配送</asp:ListItem>
                             
                            </asp:DropDownList>
                        </div> 
                        <div class="form-group mr20" style ="margin-left :30px;">
                              <label for="ctl00_ContentPlaceHolder1_calendarStartDate_txtDateTimePicker">配送时间：</label>
                            <Hi:DateTimePicker CalendarType="StartDate" ID="calendarStartDate" runat="server" CssClass="form-control resetSize inputw100" />
                            至
                                <Hi:DateTimePicker ID="calendarEndDate" runat="server" CalendarType="EndDate" CssClass="form-control resetSize inputw100" />
                        </div>
                         <div class="form-group" style ="margin-left :0px">  <asp:LinkButton ID="LinkBtn"  CssClass="bl mb5" runat="server">今日待配送</asp:LinkButton>
                    
                    </div>  
                    </div>
                    <div class="form-inline ">
                           <div class="form-group">
                            <label for="sellshop1" class="ml10">客户姓名：</label>
                            <asp:TextBox ID="txtUserName" CssClass="form-control resetSize"  runat="server" />
                        </div>

                         
                        <div class="form-group" style ="margin-left:49px">
                            <label class="ml10">排序：</label>
                            <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-control  resetSize inputw150">
                                <asp:ListItem Text="升序" Value="0" Selected="True">升序</asp:ListItem>
                                <asp:ListItem Text="倒序" Value="1">倒序</asp:ListItem>
                             
                            </asp:DropDownList>
                        </div> 
                       
                        <div class="form-group" style ="margin-left :30px">                    
                            <asp:Button ID="btnSearchButton" runat="server" CssClass="btn resetSize btn-primary" Text="搜索" />
                        </div>
                                <div class="form-group mr20" style ="margin-left :15px">  <asp:LinkButton ID="LinkButtonClear"  CssClass="bl mb5" runat="server">清除条件</asp:LinkButton>
                    
                    </div>     
                    </div>   
                </div>          
             </div>

       
            <div class="title-table">
            <div style="margin-bottom:5px;  margin-top:10px;">        
                <div class="form-inline" id="pagesizeDiv" style="float: left; width:100%; margin-bottom:5px;">                
                </div> 
                  <div class="page-box">
                    <div class="page fr">
                        <div class="form-group" style="margin-right:0px;margin-left:0px;background:#fff;">
                            <label for="exampleInputName2">每页显示数量：</label>
                       <UI:PageSize runat="server"  ID="hrefPageSize1" />
                        </div>
                    </div>
                </div>
                <div class="pageNumber" style="float: right;  height:29px; margin-bottom:5px; display:none;" >
                    <label>每页显示数量：</label>
                    <div class="pagination" style="display:none;">
                        <UI:Pager runat="server" ShowTotalPages="false" ID="pager" />
                    </div>
                </div>

                <div class="form-inline" style="text-align: left; margin-top: 5px; background: #fff;">
                    <label>
                        <input type="checkbox" id="selectAll" style="margin: 0px 0px 0px 17px" />
                        全选</label>
                   <asp:Button ID="btnManyChangeStatus" runat="server" Text="批量已配送" class="btn resetSize btn-primary" IsShow="true" OnClientClick="return HiConform('<p>确认要批量已配送任务吗？</p>', this);" />
                  <span style="font-size:inherit;margin-left:15px">统计信息：当前结果集   <span id="spantotaldays" runat="server">共有<span runat="server" style="font-weight:bolder;" id="days">3</span>天</span>  <span runat="server" style="font-weight:bolder;" id="spantotalcount">0</span>条 配送任务,其中<span  runat="server" style="color:green;font-weight:bolder;" id="alreadySend">10</span>条已配送,<span style="color:red;font-weight:bolder;"  runat="server" id="NoSend">3</span>条未配送</span>      
                </div>
                <!--结束-->                           
            </div>
                <table class="table table-hover mar table-bordered" style="border-bottom: none;"><thead><tr>
			<th style="width: 47px; text-align: center;"><span id="ctl00_ContentPlaceHolder1_grdMemberList_ctl01_label">选择</span></th>
                    <th style="text-align: center;width:80px">客户姓名(ID)</th>  <th style="text-align: center;width:90px">联系电话</th><th style="text-align: center;width:90px">配送时间</th>
                  <th style="text-align: center;width:100px">配送地址</th>
                    <th style="text-align: center;width:100px">配送商品</th><th style="text-align: center;width:50px">数量</th>
                   <%-- <th style="text-align: center;width:135px">配送备注</th>--%><th style="text-align: center;width:100px">配送状态</th>
                    <th  style="text-align: center;width:100px">所属订单</th>
		</tr></thead></table></div>
            <div id="datalist">
            <UI:Grid ID="grdMemberList" runat="server" ShowHeader="false" AutoGenerateColumns="false"
                DataKeyNames="QuestId" HeaderStyle-CssClass="table_title" CssClass="table table-hover mar table-bordered"
                GridLines="None" Width="100%">
                <Columns>
                    <UI:CheckBoxColumn  CellWidth="50"  ItemStyle-HorizontalAlign="Center" />
                  
                    <asp:TemplateField HeaderText="客户姓名(ID)" ItemStyle-Width="100" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="RealName" runat="server" Text='<%# Eval("RealName") +"<br>("+Eval("UserId")+")" %>'/></p>
                        </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="联系电话" ItemStyle-Width="112" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="CellPhone" runat="server" Text='<%# Eval("CellPhone") %>'/></p>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="配送时间" ItemStyle-Width="100" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><%# Eval("QuestDate","{0:yyyy-MM-dd}")%>
                             </p>
                        </ItemTemplate>
                    </asp:TemplateField>
                      
                     <asp:TemplateField HeaderText="配送地址" ItemStyle-Width="112" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="Address" runat="server" Text='<%#Eval("ShippingRegion")+" "+Eval("Address") %>'/></p>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="配送商品" ItemStyle-Width="112" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="ProductName" runat="server" Text='<%# Eval("ProductName") %>'/></p>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="数量" ItemStyle-Width="50" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="QuantityPerDay" runat="server" Text='<%# Eval("QuantityPerDay") %>'/></p>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--  <asp:TemplateField HeaderText="配送备注" ItemStyle-Width="112" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="lblUserName" runat="server" /></p>
                        </ItemTemplate>
                    </asp:TemplateField>--%>

                     <asp:TemplateField HeaderText="配送状态" ItemStyle-Width="112" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p> 
                                 <%# Eval("Status").ToString()=="0"? " <label style='color:red'>未配送 </label>":"<label style='color:green'>已配送 </label>" %>
                            </p>

                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="所属订单" ItemStyle-Width="120" SortExpression="UserName"  HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <p><asp:Literal ID="OrderId"  runat="server" Text='<%# Eval("OrderId") %>'/></p>
                        </ItemTemplate>
                    </asp:TemplateField>


                   
                   <%-- <asp:TemplateField HeaderText="操作" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="border_top border_bottom" HeaderStyle-Width="95">
                        <ItemStyle CssClass="spanD spanN actionBtn"/>
                        <ItemTemplate>
                            
                            <p><a href="javascript:ShowGradeUser('<%# Eval("UserId") %>','<%# Eval("QuestId") %>')">查看</a></p>
                            <p>
                                
                                 <%# Eval("Status").ToString()=="0"?"<a  style='cursor:pointer;' onclick='change(\""+Eval("QuestId")+"\")' >已配送</a>":"" %>
                               </p>
                            
                           
                            <input id="hdUserId" type="hidden" value="<%# Eval("UserID") %>" />
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                </Columns>
            </UI:Grid>   

            </div>      
        </div>
        <!--数据列表底部功能区域-->

           <input type="hidden" id="hdUserId" runat="server" value="" />
            <asp:Button ID="huifuUser" Text="huifu" runat="server" Style="display:none"/>
            <asp:Button ID="BatchHuifu" Text="huifu" runat="server" Style="display:none"/>
             <asp:Button ID="BatchCreatDist" Text="huifu" runat="server" Style="display:none"/>


        <div class="bottomPageNumber clearfix">
            <div class="pageNumber">
                <div class="pagination" style="width: auto">
                    <UI:Pager runat="server" ShowTotalPages="true" ID="pager1" />
                </div>
            </div>
        </div>
       
        </div>

       



        </form>
    <script type="text/javascript">

        $(document).ready(function () {

            $('#selectAll').click(function () {
                var check = $(this).prop('checked');
                $("input[type='checkbox']").each(function () {
                    $(this).prop('checked', check);
                });
            });


            $('#datalist').find('th').each(function () {
                $(this).css('text-align', 'center');
            });

            $('#pagesizeDiv').find('a').each(function () {
                if ($(this).attr("class") != "selectthis") {
                    $(this).removeClass();
                    $(this).addClass('aClass');
                }

            });

        });
     
        function change(id)
        {

            alert(id)
        }






    </script>
</asp:Content>
