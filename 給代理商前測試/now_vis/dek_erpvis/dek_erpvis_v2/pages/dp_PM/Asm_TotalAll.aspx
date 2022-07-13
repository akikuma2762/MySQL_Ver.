<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_TotalAll.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.Asm_TotalAll" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>全排程看板 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_LineOverView20220324.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        <%=Set_CSS(5) %>
    </style>
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>
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
                                <div class="row tile_count" style="margin-top: 0px; margin-bottom: -20px">
                                    <div class="col-md-offset-4 col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                        <span class="count_top"><i class="fa fa-clock-o"></i>今日在線</span>
                                        <div class="count blue"><%=OnLinePiece %><span style="height: 10px"><%=PieceUnit%></span></div>
                                    </div>
                                    <div class="col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                        <span class="count_top"><i class="fa fa-clock-o"></i>今日完成</span>
                                        <div class="count green"><% =FinishPiece%><span style="height: 10px"><%=PieceUnit%></span></div>
                                    </div>
                                    <div class="col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                        <span class="count_top"><i class="fa fa-clock-o"></i>今日異常</span>
                                        <div class="count red"><%=alarm_total %><span style="height: 10px"><%=PieceUnit%></span></div>
                                    </div>
                                    <div class="col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                        <span class="count_top"><i class="fa fa-clock-o"></i>當下落後</span>
                                        <div class="count black"><%=behind %><span style="height: 10px"><%=PieceUnit%></span></div>
                                    </div>
                                </div>
                            </div>

                            <div class="x_panel">
                                <table id="TB" class="table table-bordered" border="1" cellspacing="0" style="width: 100%">
                                    <thead>
                                        <%=ColumnsData%>
                                    </thead>
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
    <%=Use_Javascript.Quote_Javascript(false) %>
    <script> 


        //防止切換頁籤時跑版
        $(document).ready(function () {

            $('#TB').DataTable({
                destroy: true,
                language: {
                    "processing": "處理中...",
                    "loadingRecords": "載入中...",
                    "lengthMenu": "顯示 _MENU_ 項結果",
                    "zeroRecords": "沒有符合的結果",
                    "info": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
                    "infoEmpty": "顯示第 0 至 0 項結果，共 0 項",
                    "infoFiltered": "(從 _MAX_ 項結果中過濾)",
                    "infoPostFix": "",
                    "search": "搜尋:",
                    "paginate": {
                        "first": "第一頁",
                        "previous": "上一頁",
                        "next": "下一頁",
                        "last": "最後一頁"

                    }
                },
                colReorder: true,
                "order": [[0, "asc"]],

                scrollCollapse: true,

                "lengthMenu": [[-1, 10, 25, 50, 100], ["All", 10, 25, 50, 100]],
                dom: "<'row'<'pull-left'f>'row'<'col-sm-3'>'row'<'col-sm-3'B>'row'<'pull-right'>>" +
                    "<rt>" +
                    "<'row'<'pull-left'>'row'<'col-sm-4'>row'<'col-sm-3'>'row'<'pull-right'>>",

                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],

            });

         
        });


        //顯示進度，問題
        function SetValue(number, percent, Report,nowstatus) {
            $('#ContentPlaceHolder1_TextBox_Number').val('' + number + '');
            $('#ContentPlaceHolder1_TextBox_show').val('' + number + '');
            $('#ContentPlaceHolder1_TextBox_Report').val('' + Report.replaceAll("^", "'").replaceAll('#', '"').replaceAll("$", " ").replaceAll('@', '\r\n') + '');


            if (nowstatus == '啟動' && percent == '99%')
                $("#<%=RadioButtonList_select_type.ClientID %> :radio[value='3']").prop("checked", true);
            else if (nowstatus == '啟動' || nowstatus == '未動工')
                $("#<%=RadioButtonList_select_type.ClientID %> :radio[value='0']").prop("checked", true);
            else if (nowstatus == '暫停')
                $("#<%=RadioButtonList_select_type.ClientID %> :radio[value='1']").prop("checked", true);

            else if (nowstatus == '完成')
                $("#<%=RadioButtonList_select_type.ClientID %> :radio[value='2']").prop("checked", true);

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
    </script>
</asp:Content>
