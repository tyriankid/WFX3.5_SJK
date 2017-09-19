<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminNew.Master" AutoEventWireup="true" CodeBehind="DistributorDetails.aspx.cs" Inherits="Hidistro.UI.Web.Admin.Fenxiao.DistributorDetails" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.ControlPanel.Utility" Assembly="Hidistro.UI.ControlPanel.Utility" %>
<%@ Register TagPrefix="UI" Namespace="ASPNET.WebControls" Assembly="ASPNET.WebControls" %>
<%@ Import Namespace="Hidistro.ControlPanel.Store" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/utility/flashupload/flashupload.css" rel="stylesheet" type="text/css" />
       <Hi:Script ID="Script3" runat="server" Src="/utility/flashupload/flashuploadBanner.js" />
    <style>

        .yejiItem{text-align:center;line-height:30px;float:left;margin:5px;padding:10px 40px;border-left:1px solid #b9caca}
            .yejiItem:first-child {border-left:0px
            }
            .yejiItem .money{color:#125acb;font-weight:bold;font-size:18px}
             .yejiItem .yejitxt{color:#444451;font-weight:bold;font-size:18px}
             .infodiv{float:left;}
             .infosdetail{width:250px;margin-left:10px;line-height:23px}
             .infostitle{width:90px; text-align:center;}
             .infosdetail ul label{width:90px;text-align:right;margin-right:10px;font-weight:normal;}
            .infosdetailLong{width:500px;}
           
    </style>
    <script src="../js/ZeroClipboard.min.js"></script>
    <script language="javascript">
        $(function () {

            var copy = new ZeroClipboard($("#copybutton")[0], {
                moviePath: "../js/ZeroClipboard.swf"
            });

            copy.on('complete', function (client, args) {
                HiTipsShow("已复制:" + args.text, 'success');
            });

        });
        

        function BtnSavePicture() {

            document.getElementById('ctl00_ContentPlaceHolder1_BtnSavePicture').click();
        }
        function upload()
        {

            document.getElementById('ctl00_ContentPlaceHolder1_btnUpload').click();
        }
        $(function () {


            $('#ctl00_ContentPlaceHolder1_dropRegion').find('select').each(function () {
                $(this).removeClass();
                $(this).addClass('form-control inl autow mr5 resetSize');
            });

            $('.actions').each(function () { 
                $(this).text("预览图片");
                $(this).unbind("click");
                $(this).attr("target", "_blank");
               $(this).attr("href",$(this).parent().prev().find('img').attr("src"));
             
            });

            
        });

        function ShowEditDistributorInfos() {
           

                    $('#EditDistributorInfos').modal('toggle').children().css({
                        width: '740px', top: "130px"
                    });
            
        }



</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="page-header">
            <h2>分销商详情</h2>
         </div>

    <form runat="server">

         <!--基本信息-->
        <h3 class="templateTitle">基本信息</h3>
          <div class="set-switch clearfix">
              <div class="infodiv">
                <div class="qrCode">
                            <Hi:HiImage  ID="ListImage1" ImageUrl="/Templates/common/images/user.png"  runat="server" Width="90" Height="90" />
                </div>
                  <div class="infostitle">个人头像</div>
              </div>

                <div class="infodiv infosdetail">
                <ul>
                  <li><label>联系人：</label><span id="txtRealName" runat="server">-</span></li>
                     <li><label>手机号码：</label><span id="txtCellPhone" runat="server">-</span></li>
                    <li><label>用户名：</label><span id="txtUserBindName" runat="server">-</span></li>
                     <li><label>微信昵称：</label><span id="txtMicroName" runat="server">-</span></li>
                </ul>
              </div>
             
                <div class="infodiv">
                <div class="qrCode">
                    <Hi:HiImage  ID="StoreCode"  runat="server" Width="90" Height="90" />     
                </div>
                     <div class="infostitle">店铺二维码</div>
              </div>
             
                
                <div class="infodiv infosdetail infosdetailLong"　>
                    <ul>
                        <li>
                            <a id="copybutton" data-clipboard-target="ctl00_ContentPlaceHolder1_txtUrl" style="cursor:pointer;color:#0f63ac;margin-right: -35px;float:right;">复制链接</a><label>店铺名：</label><span id="txtStoreName" runat="server">-</span> </li>
                        <li style="display:none">
                            <label>分销商等级：</label><span id="txtName" runat="server">-</span></li>
                        <li style="overflow:hidden">
                            <label>店铺链接：</label><span id="txtUrl" runat="server" style="width:300px;overflow-x:hidden;margin:0px">-</span></li>
                        <li>
                            <label>申请时间：</label><span id="txtCreateTime" runat="server">-</span></li>
                         <li>
                            <label>下载二维码：</label>   <a id="upload" onclick="upload()"  style="cursor:pointer;color:#0f63ac;">点击下载</a>  </li>
                    </ul>
              <div style="display:none">
            <asp:Button ID="btnUpload"  runat="server" Text="点击下载" CssClass="btn btn-primary  resetSize " />

        </div>
              </div>
               
             
             
      </div>
         <h3 class="templateTitle">站点信息</h3>
          <div class="set-switch clearfix">
                <div class="infodiv infosdetail" style="width:100%">
                <ul>
                  <li><a id="editsiteinfo" onclick="ShowEditDistributorInfos(this);"   data-clipboard-target="ctl00_ContentPlaceHolder1_txtUrl" style="cursor:pointer;margin-right: 15px;color:#0f63ac;float:right;">编辑站点</a><label>站点电话：</label><span id="SiteTel" runat="server">-</span></li>
                     <li><label>站点地址：</label><span id="SiteAddress" runat="server">-</span></li>
                    <li><label>站点简介：</label><span id="StoreDescription" runat="server">-</span></li>
                    
                </ul>
              </div>

              </div>

          <h3 class="templateTitle">站点轮播图</h3>
          <div class="set-switch clearfix">
                 <div class="col-xs clearbor">
                                    <div class="exitpa">
                                        <div class="clearfix FlashPanel" style="height: 122px;">
                                            <Hi:ProductFlashUpload ID="ucFlashUpload1" runat="server" MaxNum="6" />
                                              <a id="aBtnSavePicture" onclick="BtnSavePicture();"    style="cursor:pointer;color:#0f63ac;float:right;">保存图片</a>
                                           
                                        </div>
                                        <small>建议尺寸640×640像素</small>
                                        <small>支持批量上传，每个商品最多6张图，每张小于300KB，支持jpg、gif、png格式</small>
                                    </div>
                     
                                </div>
             <div style="display:none">  <asp:Button ID="BtnSavePicture"  runat="server"  />  </div>

              </div>


         

         <!--业绩-->
        <h3 class="templateTitle">业绩</h3>
        <div class="set-switch  clearfix" style="height:120px">

            <div class="yejiItem">
             <div class="money" id="ReferralOrders" runat="server">0</div>
             <div class="yejitxt">成交单数</div>
            </div>

             <div class="yejiItem">
             <div class="money"  id="OrdersTotal" runat="server">￥0</div>
             <div class="yejitxt">销售金额</div>
            </div>

             <div class="yejiItem"style="display:none">
             <div class="money"  id="TotalReferral" runat="server">￥0</div>
             <div class="yejitxt">佣金总额</div>
             </div>

             <div class="yejiItem"style="display:none">
             <div class="money"  id="ReferralBlance" runat="server">￥0</div>
             <div class="yejitxt" >可提现佣金余额</div>
            </div>

             <div class="yejiItem"style="display:none">
             <div class="money"  id="ReferralRequestBalance" runat="server">￥0</div>
             <div class="yejitxt">已提现佣金总额</div>
            </div>

        </div>

     <!--数据列表-->
        <h3 class="templateTitle" style="display:none">提现记录</h3>
   
             <div style="display:none">
             <table class="table table-hover mar table-bordered" style="table-layout:fixed">
                        <thead>
                            <tr>
                                <th width="120">申请时间</th>
                                <th  width="120">提现金额</th>
                                 <th  width="100">帐号类型</th>
                                <th  width="100">帐号</th>
                               <th  width="100">收款人</th>
                                 <th width="120">支付日期</th>
                            </tr>
                        </thead>
                        <tbody>

        <asp:Repeater ID="reCommissions"  runat="server" >
     <ItemTemplate>
      <tr  class="td_bg">
          <td width="200">
                                 &nbsp; <%# Eval("RequestTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                 <td>
                                     &nbsp;￥<%# Eval("Amount", "{0:F2}")%></td>
                                  <td>
                                    &nbsp;<%# VShopHelper.GetCommissionPayType(Eval("RequestType").ToString())%>
                                   </td>
                                <td>
                                    &nbsp;<%# Eval("MerchantCode") %></td>
                                <td>
                                    &nbsp;<%# Eval("AccountName") %></td>
                                <td>
                                    &nbsp;<%# Eval("CheckTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                               
        </tr>
     </ItemTemplate>
 </asp:Repeater>
         </tbody>
     </table>
     </div>

    

         <!--数据列表底部功能区域-->
  <br />
        <div class="select-page clearfix">
                    <div class="form-horizontal fl">
                       <a onclick="javascript:history.go(-1)" class="btn btn-primary">返回</a>
                    </div>
                    <div  class="page fr">
                         <div class="pageNumber">
                        <div class="pagination" style="margin:0px">
                        <UI:Pager runat="server" ShowTotalPages="true" DefaultPageSize="5" ID="pager" />　
                       </div>
                      </div>
                    </div>
                </div>

        <div class="clearfix" style="height:30px"></div>

        
            <div class="modal fade" id="EditDistributorInfos">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" style="text-align: left">修改站点信息</h4>
                    </div>
                    <div class="form-horizontal" style="overflow-x: hidden">

                        <asp:HiddenField ID="EditUserID" Value="" runat="server" />

                    
                      
                          <div class="form-group">
                            <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>站点名称：</label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtSiteName" CssClass="form-control  inputw120" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>站点电话：</label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtSiteTel" CssClass="form-control  inputw120" runat="server"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group">
                            <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>省市区：</label>
                            <div class="col-xs-9">
                                 <Hi:RegionSelector runat="server" ID="dropRegion" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>站点地址：</label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtSiteAddress" CssClass="form-control  inputw120" runat="server"></asp:TextBox>
                            </div>
                        </div>


                        <div class="form-group">
                            <label for="inputEmail3" class="col-xs-2 control-label"><em>*</em>站点简介：</label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtStoreDescription"  TextMode="MultiLine" Rows="4" CssClass="form-control  inputw120" runat="server"></asp:TextBox>
                               
                            </div>
                        </div>



                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="EditSave" class="btn btn-primary" Text="确定修改" runat="server" />
                        <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>



    </form>

</asp:Content>
