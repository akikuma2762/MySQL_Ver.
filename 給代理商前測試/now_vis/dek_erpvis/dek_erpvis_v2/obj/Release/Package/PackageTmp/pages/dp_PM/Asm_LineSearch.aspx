<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_LineSearch.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.Asm_LineSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>整廠進度管理看板 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_LineOverView20211207.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>
            <li><u><a href="Asm_LineTotalView.aspx">整廠進度管理看板</a></u></li>
        </ol>
        <!-----------------title------------------>


        <asp:TextBox ID="TextBox_textTemp" runat="server" Style="display: none"></asp:TextBox>
        <!-- top tiles -->

        <!-----------------/title------------------>

        <!-----------------content------------------>
        <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">

                    <div class="x_content">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="x_panel">
                                <table id="TB" class="table table-bordered" border="1" cellspacing="0" style="width: 100%">
                                    <tr id="tr_row">
                                        <%=ColumnsData%>
                                    </tr>
                                    <tbody>
                                        <%=RowsData%>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-----------------/content------------------>
        <!-- Modal -->
        <div id="exampleModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                        <h4 class="modal-title" id="myModalLabel2"><i class="fa fa-file-text"></i>狀態變更精靈</h4>
                    </div>
                    <div class="modal-body">
                        <div id="testmodal2">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="form-group">
                                        <h3>
                                            <i class="fa fa-caret-down">製令編號</i>
                                        </h3>
                                        <div class="btn-group btn-group-justified" data-toggle="buttons">
                                            <asp:TextBox ID="TextBox_Number" runat="server" ReadOnly="true" Width="100%"></asp:TextBox>
                                            <asp:TextBox ID="TextBox_show" runat="server" Style="display: none" Width="100%"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="col-md-4 col-sm-12 col-xs-12">
                                    <div class="form-group">
                                        <h3>
                                            <i class="fa fa-caret-down">進度選擇:</i>
                                        </h3>
                                        <fieldset>
                                            <asp:DropDownList ID="DropDownList_progress" runat="server"></asp:DropDownList>
                                        </fieldset>
                                    </div>
                                    <div class="form-group">
                                        <h3>
                                            <i class="fa fa-caret-down">狀態選擇:</i>
                                        </h3>

                                        <asp:RadioButtonList ID="RadioButtonList_select_type" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Value="0" Selected="True" style="color: deepskyblue">啟動</asp:ListItem>
                                            <asp:ListItem Value="1" style="color: red">暫停</asp:ListItem>
                                            <asp:ListItem Value="3" style="color: black">跑合</asp:ListItem>
                                            <asp:ListItem Value="2" style="color: green">完成</asp:ListItem>
                                        </asp:RadioButtonList>

                                    </div>
                                </div>
                                <div class="col-md-8 col-sm-12 col-xs-12">
                                    <h3>
                                        <i class="fa fa-caret-down">問題回報:</i>
                                    </h3>
                                    <asp:TextBox ID="TextBox_Report" runat="server" TextMode="MultiLine" Style="resize: none; width: 100%; height: 165px"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="margin_style">
                        <button type="button" class="btn btn-primary" data-dismiss="modal">取消</button>
                        <button type="button" class="btn btn-success" id="btncheck">送出</button>
                        <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="fab" id="masterfab" data-subitem="1" data-toggle="modal" data-target="#SearchModal" >
        <span >
            <i class="fa fa-search"></i>
        </span>
    </div>
    <div id="SearchModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 class="modal-title modaltextstyle" id="myModalLabel23"><i class="fa fa-file-text"></i>搜尋排程</h4>
                </div>
                <div class="modal-body">
                    <div id="testmodal23" style="padding: 5px 20px;">
                        <div class="form-group">
                            <label id="Record_Number" style="display: none">0</label>
                            <div id="add_element"></div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                            </div>
                            <button type="button" class="btn btn-info" onclick="add_content('add_element')">新增項目</button>
                            <asp:TextBox ID="TextBox_Schdule" runat="server" Style="display: none"></asp:TextBox>

                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="col-md-11 xdisplay_inputx form-group has-feedback">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button id="btnSearch" type="button" class="btn btn-primary antosubmit2 ">執行搜索</button>
                    <asp:Button ID="Button_Search" runat="server" Text="查詢" OnClick="Button_Search_Click" Style="display: none" />
                </div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script> 
        //顯示進度，問題
        function SetValue(number, percent, Report) {

            $('#ContentPlaceHolder1_TextBox_Number').val('' + number + '');
            $('#ContentPlaceHolder1_TextBox_show').val('' + number + '');
            $('#ContentPlaceHolder1_TextBox_Report').val('' + Report.replaceAll("^", "'").replaceAll('#', '"').replaceAll("$", " ").replaceAll('@', '\r\n') + '');

            selectElement('ContentPlaceHolder1_DropDownList_progress', percent);
            checkpower();
        }
        //儲存進度跟問題
        $("#btncheck").click(function () {
            var WhatSystem = navigator.userAgent;
            if (WhatSystem.match(/(iphone|ipad|ipod);?/i)) {
            } else {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                document.getElementById('btncheck').disabled = true;
            }
            document.getElementById('<%=button_select.ClientID %>').click();
        });
        //跳頁
        function jump_Asm_ErrorDetail(paramer) {
            var WhatSystem = navigator.userAgent;
            if (WhatSystem.match(/(iphone|ipad|ipod);?/i)) {
            } else {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                $.unblockUI();
            }
            document.location.href = "Asm_ErrorDetail.aspx?key=" + paramer;

        }
        //檢察部門別，是否可以做修改的動作
        function checkpower() {
            var power = '<%=power %>';
            if (power == 'PMD')
                document.getElementById('btncheck').style.visibility = 'visible';
            else
                document.getElementById('btncheck').style.visibility = 'hidden';
        }

        $(document).ready(function () {
            //網頁一開始，產生5個TEXTBOX給使用者輸入
            for (i = 0; i < 5; i++)
                add_content('add_element');
        });

        $("#btnSearch").click(function () {
            var Number = '';
            //組合使用者選的刀庫 || 製令
            for (i = 0; i < parseInt(document.getElementById('Record_Number').innerHTML, 10); i++)
                Number += document.getElementById('textbox_' + i).value + '#';
            //寫入TEXTBOX內，讓後端進行運算
            document.getElementById('<%=TextBox_Schdule.ClientID %>').value = Number;

            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btnSearch').disabled = true;
            document.getElementById('<%=Button_Search.ClientID %>').click();
            $.unblockUI();
            document.getElementById('btnSearch').disabled = false;
            $('#exampleModal').modal('hide');
        });

        //新增文本輸入框的事件
        function add_content(div) {
            var Number = '';

            //先儲存數值，以免按下新增時要重打
            for (i = 0; i < parseInt(document.getElementById('Record_Number').innerHTML, 10); i++) {
                Number += `textbox_${i}` + '#' + document.getElementById('textbox_' + i).value + '#';
            }
            //取得要新增的DIV名稱
            var divname = document.getElementById(div);
            //新增控制項到該DIV內
            divname.innerHTML = divname.innerHTML +
                '<div class="col-md-12 col-sm-12 col-xs-12">' +
                '<div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">' +
                ' <span>製令編號</span>' +
                ' </div>' +
                ' <div class="col-md-8 col-sm-12 col-xs-7" style="margin: 5px 0px 5px 0px">' +
                `<input type="text" id="textbox_${document.getElementById('Record_Number').innerHTML}" name="textbox_${document.getElementById('Record_Number').innerHTML}" class="form-control text-center"  >` +
                '  </div>'
            '</div>';

            //回復原本的數值
            if (document.getElementById('Record_Number').innerHTML != '0') {
                var valuelist = Number.split("#");
                for (j = 0; j < valuelist.length - 1; j++) {
                    document.getElementById(valuelist[j]).value = valuelist[j + 1];
                    j++;
                }
            }
            //把Label+1(需先轉換 文字->數字)
            document.getElementById('Record_Number').innerHTML = parseInt(document.getElementById('Record_Number').innerHTML, 10) + 1;
        }

    </script>
</asp:Content>
