<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminNew.Master" AutoEventWireup="true" Inherits="Hidistro.UI.Web.Admin.Fenxiao.Scope" %>

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


        function doAdd(obj) {
            // var RegionId = $("").val();
            var RegionId = $("#regionSelectorValue").val();
            var RegionName = $("#regionSelectorName").val();
            var streetName = $("#ctl00_ContentPlaceHolder1_txtStreetName").val();

            if (RegionId == "" || RegionId == undefined) {
                HiConform('<strong>请选择一个区域！</strong>', obj);
                return ;
            }
            if (streetName == "" || streetName == undefined) {
                HiConform('<strong>请选填写街道名称！</strong>', obj);
              
                return ;
            }

            else {
                $.ajax({
                    type: "POST",   //访问WebService使用Post方式请求
                    contentType: "application/json", //WebService 会返回Json类型
                    url: "Scope.aspx/AddStreetInfo", //调用WebService的地址和方法名称组合 ---- WsURL/方法名
                    data: "{regionCode:'" + RegionId + "',streetName:'" + streetName + "'}",         //这里是要传递的参数，格式为 data: "{paraName:paraValue}",下面将会看到      
                    dataType: 'json',
                    success: function (result) {     //回调函数，result，返回值
                        if (result.d == "success") {
                            location.reload();
                        }
                    }
                });

            }

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
        <h2>配送范围</h2>
    </div>
    <form runat="server">
      <div class="set-switch">
                       
                        <div class="form-inline mb10">
                            
                             <div class="form-group">
                                <label for="sellshop1">省市区：</label>
                                <Hi:RegionSelector runat="server" ID="RegionSelector1" />
                            </div>


                           

                            <div class="form-group mr20">
                                <label for="sellshop1">　街道名称：</label>
                               <asp:TextBox ID="txtStreetName" runat="server" CssClass="form-control resetSize inputw150" />
                            </div>
                            <div class="form-group">
                   
                    <asp:Button ID="btnSearchButton" runat="server" class="btn resetSize btn-primary" OnClientClick="return doSearch()" Text="查询" />
                </div>
                              <div class="form-group">
                   
                   <input type="button" id="btnAdd" onclick="doAdd(this)" class="btn resetSize btn-info" value="添加" />
                </div>
 <div class="form-group mr20">  
                                <a class="bl mb5"  onclick="reset()" style="cursor: pointer;">清除条件</a>
                            </div>

                        </div>
                      
                    </div>




        <!--数据列表-->

        <asp:Repeater ID="grdStreetsInfo" runat="server" OnItemCommand="grdStreetsInfot_ItemCommand">
            <HeaderTemplate>
                <div>
                    <table class="table table-hover mar table-bordered" style="table-layout: fixed">
                        <thead>
                            <tr>
                                <th>街道名称</th>
                                <th>省市区名称</th>
                                <th>操作</th>

                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                </tr>
       <td>
           <asp:Label ID="lblStreetName" runat="server" Text='<%# Bind("StreetName") %>'></asp:Label>
       </td>
                <td>
                    <asp:Label ID="lblRegionName" runat="server" Text='<%# Bind("RegionName") %>'></asp:Label>
                </td>
                <td><a class="table-icon edit" href="#" onclick='ShowEditStreetInfos("<%#Eval("StreetId")%>",this)' data="<%#Eval("StreetName")%>">编辑</a>
                    <asp:Button ID="lbtnDel" CssClass="btnLink" CommandArgument='<%#Eval("StreetId") %>' runat="server" Text="删除" CommandName="Delete"
                        OnClientClick="return HiConform('<strong>确定要删除这条街道信息吗？</strong><p>删除街道后不可恢复！</p>',this)" /></td>
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



        <!--编辑用户信息-->
        <div class="modal fade" id="EditDistributorInfos">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" style="text-align: left">修改街道信息</h4>
                    </div>
                    <div class="form-horizontal" style="overflow-x: hidden">

                        <input type="hidden" id="streetid" />


                        <div class="form-group">
                            <label for="inputEmail3" class="col-xs-4 control-label">街道名称：</label>
                            <div class="col-xs-6">

                                <input type="text" id="StreetNewName" class="form-control  inputw120" />
                            </div>
                        </div>


                    </div>
                    <div class="modal-footer">

                        <button type="button" class="btn btn-primary" onclick="editStreetName(this)">确定修改</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
    </form>


</asp:Content>
