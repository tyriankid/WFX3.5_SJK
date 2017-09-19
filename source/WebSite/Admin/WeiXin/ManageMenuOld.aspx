<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminNew.Master" AutoEventWireup="true" CodeBehind="ManageMenu.aspx.cs" Inherits="Hidistro.UI.Web.Admin.WeiXin.ManageMenu" %>

<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.ControlPanel.Utility" Assembly="Hidistro.UI.ControlPanel.Utility" %>
<%@ Register TagPrefix="UI" Namespace="ASPNET.WebControls" Assembly="ASPNET.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/admin/css/weibo.css">
    <style type="text/css">
        .subtitlespan {
            display: inline-block;
            width: 220px;
            overflow: hidden;
        }
    </style>

    <script>

        $(function () {
            loadmenu();
        })
        var setmenuid = "";
        function onemenu(menuid) {
            setmenuid = menuid;
            bishi = "1";
        }
        function edittitle(id) {
            $("#txtedittitle" + id).focus();
            $("#spantitlename" + id).css("display", "none");
            $("#spanedittile" + id).css("display", "");
            $("#btnedit" + id).css("display", "none");
            $("#submitedit" + id).css("display", "");
        }
        function conseleditwin(id) {
            $("#spantitlename" + id).css("display", "");
            $("#spanedittile" + id).css("display", "none");
            $("#btnedit" + id).css("display", "");
            $("#submitedit" + id).css("display", "none");
        }
        //菜单显示
        function showmenu(data) {
            $("#showmenuul").html("");
            $("#showtextul").html("");
            if (data.enableshopmenu == "True") {
                var html = "";
                if (data.shopmenustyle == "1") {
                    $("#textmenu").remove();
                    for (var i in data.data) {
                        var menudata = data.data[i];
                        html += "<li class=\"child\">";

                        html += "<div class=\"img\">";
                        html += " <img src=\"" + menudata.shopmenupic + "\"/>";
                        html += "</div>";

                        html += "<p>" + menudata.name + "</p>";

                        if (menudata.childdata.length > 0) {
                            html += " <div class=\"inner-nav\"><ul>";
                            for (var j in menudata.childdata) {
                                var childmenudata = menudata.childdata[j];

                                html += " <li>" + childmenudata.name + "</li>";

                            }
                            html += " </ul></div>";
                        }
                        html += "</li>";

                    }
                    $("#showmenuul").append(html);
                }
                else {
                    $("#picmenu").remove();
                    for (var i in data.data) {
                        var menudata = data.data[i];
                        if (i == 0)
                            html += "<li class=\"noborder\">";
                        else
                            html += "<li>";
                        html += "<p>" + menudata.name + "</p>";
                        if (menudata.childdata.length > 0) {
                            html += " <div class=\"inner-nav\"><ul>";
                            for (var j in menudata.childdata) {
                                var childmenudata = menudata.childdata[j];

                                html += " <li>" + childmenudata.name + "</li>";

                            }
                            html += " </ul></div>";
                        }
                        html += "</li>";

                    }
                    $("#showtextul").append(html);
                }
                $('.mobile-nav ul li').not('.mobile-nav ul li li').css('width', $('.mobile-nav').width() / $('.mobile-nav ul li').not('.mobile-nav ul li li').length).hover(function () {
                    $(this).find('.inner-nav').show().css({
                        'top': -$(this).find('.inner-nav').height() - 20,
                        'left': '50%',
                        'marginLeft': -$(this).find('.inner-nav').width() / 2
                    });
                }, function () {
                    $(this).find('.inner-nav').hide();
                })
            }
        }
        function EnterPress(e, id) {
            var e = e || window.event;
            if (e.keyCode == 13) {
                updatename(id);
            }
        }
        // 加载菜单列表
        function loadmenu() {
            $.getJSON("../../API/WXMenuProcess.ashx?action=gettopmenus").done(function (d) {
                if (d.status == "0") {
                    showmenu(d);
                    var menuhtml = "";
                    $('#ulmenu li').remove();
                    $("#content").html("");
                    $('#tabpane').remove();
                    var b = 0;
                    var menuid = 0;
                    if (d.data.length == 3) {
                        $("#addmenu").css("display", "none");
                    }
                    else {
                        $("#addmenu").css("display", "");
                    }
                    for (var i in d.data) {
                        var menudata = d.data[i];
                        var active = "";
                        if (setmenuid == "menu_" + menudata.menuid)
                            active = "class=\"active\"";
                        if (i == 0) {
                            if (i == d.data.length - 1) {
                                active = "class=\"active\"";
                                b = 1;
                            }

                            if (setmenuid == "" && bishi != "0") {
                                active = "class=\"active\"";
                                b = 1;
                            }
                            menuhtml = "  <li " + active + " id=\"" + menudata.menuid + "\"><a  id=\"menu_" + menudata.menuid + "\" onclick=\"onemenu('menu_" + menudata.menuid + "')\">" + menudata.name + "<i class=\"glyphicon glyphicon-remove\"   onclick=\"delmenu('" + menudata.menuid + "','0')\"></i></a> </li>";

                        }
                        else {
                            b = 0;

                            if (bishi == "0" && (d.data.length - 1) == i) {
                                menuhtml = "  <li class=\"active\" id=\"" + menudata.menuid + "\"><a id=\"menu_" + menudata.menuid + "\"  onclick=\"onemenu('menu_" + menudata.menuid + "')\">" + menudata.name + "<i class=\"glyphicon glyphicon-remove\"   onclick=\"delmenu('" + menudata.menuid + "','0')\"></i></a></li>";
                                menuid = menudata.menuid;
                                $("#" + setmenuid.split('_')[1]).removeClass('active');
                            }
                            else
                                menuhtml = "  <li " + active + " id=\"" + menudata.menuid + "\"><a id=\"menu_" + menudata.menuid + "\"  onclick=\"onemenu('menu_" + menudata.menuid + "')\">" + menudata.name + "<i class=\"glyphicon glyphicon-remove\"   onclick=\"delmenu('" + menudata.menuid + "','0')\"></i></a></li>";
                        }
                        var childmenuhtml = "";
                        var js = 0;
                        for (var j in menudata.childdata) {
                            //&nbsp;|<span style=\"cursor: pointer;\" onclick=\"delmenu('" + childmenudata.menuid + "','1')\">删除</span>
                            var childmenudata = menudata.childdata[j];
                            childmenuhtml += "<div class='list'><span><span class=\"subtitlespan\" id=\"spantitlename" + childmenudata.menuid + "\">" + childmenudata.name + "</span><span style=\"display:none;\" id=\"spanedittile" + childmenudata.menuid + "\"><input type=\"text\" onkeypress=\"EnterPress(event,'" + childmenudata.menuid + "');\" class=\"form-control\" id=\"txtedittitle" + childmenudata.menuid + "\"  maxlenth=\"14\" onkeyup=\"$(this).val(getStrbylen($(this).val(), 14))\" onpaste=\"$(this).val(getStrbylen($(this).val(), 14))\" style=\"width:120px;\" value=\"" + childmenudata.name + "\"></span></span><span  class='edit' ><span id=\"btnedit" + childmenudata.menuid + "\" style=\"cursor: pointer;\" onclick=\"edittitle('" + childmenudata.menuid + "')\" title=\"修改菜单名称\">编辑</span><span style=\"display:none;\" id=\"submitedit" + childmenudata.menuid + "\"><input id=\"saveedit\" type=\"button\" value=\"保存\" onclick=\"updatename('" + childmenudata.menuid + "')\" class=\"btn btn-primary\"/>&nbsp;<input id=\"conseledit\" class=\"btn btn-danger\" onclick=\"conseleditwin('" + childmenudata.menuid + "')\" type=\"button\" value=\"取消\" /></span>&nbsp;|&nbsp;<span style=\"cursor: pointer;color:blue\" onclick=\"addandeditmenu('1','" + childmenudata.menuid + "','" + childmenudata.parentmenuid + "','two')\">链接</span>&nbsp;|&nbsp;<span style=\"cursor: pointer;\" onclick=\"delmenu('" + childmenudata.menuid + "','1')\">删除</span></span></div>";

                        }
                        $("#addmenu").before(menuhtml);//添加父菜单的Tab选项卡
                        var tabcontent = $($("#tabcontent").html());
                        tabcontent.find('#fltitle').text(menudata.name);
                        tabcontent.find('#EditMenu').attr("onclick", "addandeditmenu('1','" + menudata.menuid + "','','one')");

                        tabcontent.find('#addtwomenu').attr("onclick", "addandeditmenu('0','','" + menudata.menuid + "','two')");
                        tabcontent.find("#childmenu").append(childmenuhtml);//添加子菜单

                        $("#content").append(tabcontent);//添加父菜单
                        tabcontent.find("#tabpane").attr("id", "tabmenu_" + menudata.menuid + "");
                        tabcontent.find("#tabmenu_" + menudata.menuid).parent('#panediv').attr("id", "toptabmenu_" + menudata.menuid);
                        if (setmenuid == "" && b == 1) {
                            $("#toptabmenu_" + menudata.menuid).addClass('active');
                        }
                        else {
                            if (bishi == "0") {
                                $("#toptabmenu_" + setmenuid.split('_')[1]).removeClass('active');
                                $("#toptabmenu_" + menuid).addClass('active');
                            }
                            else {
                                $("#toptabmenu_" + setmenuid.split('_')[1]).addClass('active');
                            }
                        }
                        if (menudata.childdata.length == 0) {
                            tabcontent.find("#spanhid").text("");
                        }
                    }
                    setload();
                }
                else {

                }
            });

        }
        function updatename(id) {
            var name = $("#txtedittitle" + id).val();
            var url = "&MenuId=" + id + "&Name=" + name;
            if ($.trim(name) == "") {
                HiTipsShow("请填写标题！", 'warning');
                return;
            }
            if (byteLength($.trim(name)) > 14) {
                HiTipsShow("二级菜单标题不能大于14个字符！", 'warning');
                return;
            }
            $.getJSON("../../API/WXMenuProcess.ashx?action=updatename" + url).done(function (d) {
                if (d.status == "0") {
                    loadmenu();
                    conseleditwin(id);
                    $("#spantitlename" + id).text(name);
                    HiTipsShow("修改成功！", 'success');
                }
                else {
                    HiTipsShow("修改失败！", 'fail');
                }
            });
        }
        //加载Tab选项卡
        function setload() {
            $('#mytabl > ul li').click(function () {
                $('#mytabl > ul li').removeClass('active');
                $(this).addClass('active');
                $(this).parent().next().children().removeClass('active');
                $(this).parent().next().children().eq($(this).index()).addClass('active');
            })
        }
        var andedit;
        var bishi;
        var editid;
        var parentid;
        //打开窗口
        function addandeditmenu(type, id, parentmenuid, oneortwo) {
            var targetUrl = "editmenu.aspx";
            if (type == 0) {
                if (parentmenuid > 0) {
                    targetUrl += "?pid=" + parentmenuid;
                }
            }
            else {
                $("#myModalLabel").text('修改菜单');
                targetUrl += "?MenuId=" + id;
            }

            $("#ifmMobile").attr("src", targetUrl);
            $('#myModal').modal('toggle').children().css({
                width: '500px',
                height: '300px'
            })
            $("#myModal").modal({ show: true });
        }
        //添加和修改菜单
        function submitaddandedit() {
            if ($.trim($("#txttitle").val()) == "") {
                HiTipsShow("请填写标题！", 'warning');
                return;
            }
            if (parentid == "") {
                if ($.trim($("#txttitle").val()).length > 4) {
                    HiTipsShow("一级菜单标题最多4个字！", 'warning');
                    return;
                }
            }
            else {
                if ($.trim($("#txttitle").val()).length > 7) {
                    HiTipsShow("二级菜单标题不能大于7个字！", 'warning');
                    return;
                }
                if ($.trim($("#txtContent").val()) == "") {
                    HiTipsShow("链接内容不能为空！", 'warning');
                    return;
                }
            }
            var Type = 'click';
            if (mestype != 0)
                Type = 'view';
            if (andedit == 0) {//添加一级和二级菜单
                var pic = "";
                if ($.trim($("#uploader1_preview").html()) != "") {
                    pic = $("#uploader1_image").attr('src');
                }
                var url = "&ParentMenuId=" + parentid + "&Name=" + $("#txttitle").val() + "&Content=" + $("#txtContent").val() + "&Type=" + Type + "&ShopMenuPic=" + pic;

                $.getJSON("../../API/WXMenuProcess.ashx?action=addmenu" + url).done(function (d) {
                    if (d.status == "0") {
                        HiTipsShow("添加成功！", 'success');
                        loadmenu();
                        $('#myModal').modal('hide')
                    }
                    else {
                        if (d.status == "1") {
                            HiTipsShow("添加菜单失败！", 'fail');
                        }
                        else {
                            HiTipsShow("一级菜单最多只能添加3个,二级菜单最多只能添加5个！", 'fail');
                        }
                    }
                });
            }
            if (andedit == 1)//修改一级和二级菜单
            {
                var pic = "";
                if ($.trim($("#uploader1_preview").html()) != "") {
                    pic = $("#uploader1_image").attr('src');
                }
                var url = "&ParentMenuId=" + parentid + "&MenuId=" + editid + "&Name=" + $("#txttitle").val() + "&Content=" + $("#txtContent").val() + "&Type=" + Type + "&ShopMenuPic=" + pic;
                $.getJSON("../../API/WXMenuProcess.ashx?action=editmenu" + url).done(function (d) {
                    if (d.status == "0") {
                        HiTipsShow("修改成功！", 'success');
                        loadmenu();
                        $('#myModal').modal('hide')
                    }
                    else {
                        HiTipsShow("修改菜单失败！", 'fail');
                    }
                });
            }
        }
        function delmenu(id, type) {
            if (confirm("确定要删除该菜单吗？")) {
                var url = "&MenuId=" + id;
                $.getJSON("../../API/WXMenuProcess.ashx?action=delmenu" + url).done(function (d) {
                    if (d.status == "0") {
                        if (type == 0) {
                            setmenuid = "";
                        }
                        $('#ulmenu li').remove();
                        $("#content").html("");
                        $('#tabpane').remove();
                        loadmenu();
                        HiTipsShow("删除成功！", 'success');
                    }
                });
            }
        }
        $(function () {
            $('body').on('mouseover', '#mytabl ul li', function () {
                $(this).find('i').show();
            });
            $('body').on('mouseout', '#mytabl ul li', function () {
                $(this).find('i').hide();
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-header">
        <h2>自定义菜单</h2>
    </div>

    <form runat="server">
        <div class="shop-navigation clearfix">
            <div class="fl">
                <div class="mobile-border">
                    <div class="mobile-d">
                        <div class="mobile-header">
                            <i></i>
                            <div class="mobile-title">店铺首页</div>
                        </div>
                        <div class="set-overflow">
                            <div class="white-box"></div>
                            <div class="mobile-nav" id="picmenu">
                                <ul class="clearfix" id="showmenuul">
                                </ul>
                            </div>
                            <div class="mobile-nav mobile-nav-text" id="textmenu">
                                <ul class="clearfix" id="showtextul">
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="clear-line">
                        <div class="mobile-footer"></div>
                    </div>
                </div>
            </div>
            <div class="fl frwidth">
                <script type="text/template" id="tabcontent">
                    <div id="panediv" class="tab-pane navgation">
                        <div class="tab-pane navgation" id="tabpane">
                            <p class="nav-one">一级菜单</p>
                            <div class="shop-index clearfix"><span class="fl" id="fltitle">店铺主页</span><span class="fr"><span style="cursor: pointer;" id="EditMenu">编辑</span>&nbsp;<span id="spanhid">|&nbsp;设置二级菜单以后，主链接已失效。</span></span></div>
                            <p class="nav-two">二级菜单</p>
                            <div class="nav-two-list" id="childmenu">
                            </div>
                            <div class="add-navgation" id="addtwomenu">
                                ＋添加二级菜单
                            </div>
                        </div>
                    </div>
                </script>

                <div id="mytabl">
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs margin" id="ulmenu">
                        <div id="addmenu" class="addmenu-one" onclick="addandeditmenu(0,'','','one');">＋添加一级菜单</div>
                    </ul>
                    <!-- Tab panes -->
                    <div class="tab-content" id="content">
                    </div>
                </div>
            </div>
        </div>
        <br />

        <div style="text-align: center; margin-bottom: 20px;">
            <asp:Button ID="BtnSave" class="btn btn-primary" runat="server" Text="保存到微信" />
        </div>
        <div class="modal fade" id="myModal">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="modal-header">
                        <button type="button" class="close"
                            data-dismiss="modal" aria-hidden="true">
                            &times;
                        </button>
                        <h4 class="modal-title" id="myModalLabel">菜单管理
                        </h4>
                    </div>
                    <div class="modal-body">
                        <iframe src="" id="ifmMobile" width="420" height="240" scrolling="No"></iframe>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <div class="modal fade" id="myIframeModal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">商品及分类</h4>
                    </div>
                    <div class="modal-body">
                        <iframe src="" id="MyGoodsAndTypeIframe" width="750" height="370"></iframe>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>

                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
    </form>
</asp:Content>
