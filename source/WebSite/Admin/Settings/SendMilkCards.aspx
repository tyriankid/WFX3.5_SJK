<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/SimplePage.Master" AutoEventWireup="true" CodeBehind="SendMilkCards.aspx.cs" Inherits="Hidistro.UI.Web.Admin.settings.SendMilkCards" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.ControlPanel.Utility" Assembly="Hidistro.UI.ControlPanel.Utility" %>
<%@ Register TagPrefix="UI" Namespace="ASPNET.WebControls" Assembly="ASPNET.WebControls" %>

<%@ Import Namespace="Hidistro.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/utility/skins/blue.css" type="text/css" media="screen" />

    <style>
        textarea {
            overflow: auto;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="formSendMilkCard" runat="server">
        <div class="manualreleasebox boxsize" style="margin-left:97px;margin-top:36px">
            <span class="title">请指会员用户名</span>
            <div class="form-horizontal" >
                <div class="form-group mt10">
                    <label for="inputEmail3" class="col-xs-3 control-label resetSize">用户名：</label>
                    <div class="col-xs-9">
                    
                        <asp:TextBox VerticalScrollBarVisibility="Auto"  TextWrapping="Wrap"  id="usernamename" runat="server" class="resetSize inputw250" Height="80" TextMode="MultiLine" Wrap="False"></asp:TextBox>
                        <small>填写单个或多个用户名，每个用户名占一行</small>

                    </div>
                </div>
            </div>
        </div>
        <div class="form-group" >
            <label for="inputEmail3" class="col-xs-2 control-label"></label>
            <div class="col-xs-10" style="margin-left: 20rem; margin-top: 3rem;">
                <asp:Button class="btn btn-primary" id="btnSend"  runat="server" Text="确定发送" OnClick="btnSend_Click" />
                
            </div>
        </div>
    </form>

    <script>

        $(function () {


        });

    </script>

</asp:Content>
