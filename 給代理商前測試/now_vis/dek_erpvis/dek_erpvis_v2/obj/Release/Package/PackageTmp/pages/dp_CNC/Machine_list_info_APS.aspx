<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Machine_list_info_APS.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_CNC.Aps_Project" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>機台總覽 | 緯凡金屬</title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <%=color %>
    <link rel="stylesheet" href="../../gantt/css/style.css" />
    <link rel="stylesheet" href="../../gantt/css/prettify.min.css" />
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <style>
        .x_panel {
            padding: 14px 14px;
        }

        ._select {
            display: none;
        }

        .button_use {
            z-index: 999;
            margin: 0;
            top: 40%;
            right: 0%;
            -ms-transform: translateY(-30%);
            transform: translateY(-30%);
            width: 33px;
            height: 240px;
            line-height: 235px;
            font-family: verdana;
            text-align: center;
            background: #009efa;
            color: #fff;
            position: fixed;
            bottom: 20px;
            right: 0px;
            text-decoration: none;
            cursor: pointer;
        }

        .tooltip-inner {
            max-width: 700px;
            width: 700px;
            top: 40px;
            font-size: 17px;
            left: 0px;
            display: block;
            text-align: left;
            border: 1px solid black;
            padding: 5px;
        }

        input[type="checkbox"] {
            width: 18px;
            height: 18px;
            cursor: auto;
            -webkit-appearance: default-button;
        }



        @media screen and (min-width: 765px) {


            .modal-dialog2 {
                width: 1700px
            }
        }

        @media screen and (max-width: 765px) {
            .button_use {
                display: none;
            }

            ._select {
                display: block;
            }
        }

        #Table_next_length {
            display: none;
        }

        #Table_next_filter {
            display: none;
        }

        #Table_next_info {
            display: none;
        }

        #Table_next_paginate {
            display: none;
        }

        .rows {
            margin-left: -5px;
            margin-right: -10px;
        }

        #Table_now_length {
            display: none;
        }

        #Table_now_filter {
            display: none;
        }

        #Table_now_info {
            display: none;
        }

        #Table_now_paginate {
            display: none;
        }

        .dataTables_scroll {
            overflow: auto;
        }

        .div_format {
            padding-right: 10px;
            padding-left: 10px;
            float: left;
            min-height: 1px;
            position: relative;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="right_col" role="main">
        <ol class='breadcrumb_'>
            <li><u><a href='../index.aspx'>加工部 </a></u></li>
            <li>設備監控看板</li>
        </ol>
        <br>
        <ul id="myTab" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true" onclick="hide_bar()">圖塊模式</a>
            </li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" role="tab" id="profile-tab" data-toggle="tab" aria-expanded="false" onclick="show_bar()">列表模式</a>
            </li>
            <%=show %>
            <div style="text-align: right">
                <img src="../../assets/images/MOLDING.PNG" alt="..." width="30px" height="30px">&nbsp 段取中 &nbsp
                <img src="../../assets/images/MOLDED.PNG" alt="..." width="30px" height="30px">&nbsp 段取完成 &nbsp
                <img src="../../assets/images/RUN.PNG" alt="..." width="30px" height="30px">&nbsp 加工中 &nbsp
                <img src="../../assets/images/FINISH.PNG" alt="..." width="30px" height="30px">&nbsp 加工完成 &nbsp
                <img src="../../assets/images/ERROR.PNG" alt="..." width="30px" height="30px">&nbsp 異常  &nbsp
                <img src="../../assets/images/READY.PNG" alt="..." width="30px" height="30px">&nbsp   閒置 &nbsp
            </div>

        </ul>

        <div id="myTabContent" class="tab-content">
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div class="rows">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div class="x_panel" id="Div_Shadow">
                            <div class="x_content">

                                <div class="col-md-12 col-sm-12 col-xs-12 div_format" id="table_information">
                                    <div class="x_panel">
                                        <table id="datatable_Info" class="table  table-ts table-bordered nowrap" border="1" cellspacing="0" width="100%">
                                            <thead>
                                                <tr id="tr_row">
                                                    <%=th %>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <%=tr %>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div id="_select" class="col-md-3 col-sm-12 col-xs-12 _select _setborder div_format">
                                    <div class="col-md-12 col-sm-12 col-xs-12 ">
                                        <div class="dashboard_graph x_panel">
                                            <div class="x_content">
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    <div class="col-md-5 col-sm-3 col-xs-5" style="margin: 5px 0px 5px -5px">
                                                        <span>選擇欄位</span>
                                                    </div>
                                                    <div class="col-md-7 col-sm-9 col-xs-7" style="margin: 0px 0px 0px 5px">
                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    <div class="row">
                                                        <div class="col-md-12 col-sm-12 col-xs-12 ">
                                                            <asp:CheckBoxList ID="CheckBoxList_cloumn" RepeatColumns="2" runat="server"></asp:CheckBoxList>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="col-md-9 col-xs-8">
                                                </div>
                                                <div class="col-md-3 col-xs-12">
                                                    <asp:LinkButton ID="LinkButton_ok" runat="server" Style="position: absolute; right: 0; display: none" OnClick="button_select_Click" class="btn btn-primary antosubmit2">選擇</asp:LinkButton>
                                                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行搜索</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--以上列表模式--%>
            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">

                <div class="x_panel" id="Div_Shadow">

                    <div class="x_content">
                        <div class="row">
                            <%=area %>
                        </div>
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane fade" id="tab_content3" aria-labelledby="profile-tab2">

                <div class="x_panel" id="Div_Shadow">
                    <div class="gantt_ot" style="width: 100%; height: 100%; margin: 2px auto;">
                        <div class="gantt"></div>
                    </div>
                </div>

            </div>
        </div>
        <!--以上機台清單列表-->
    </div>
    <div id="div_button" runat="server" class="button_use" onclick="hide_div()" style="display: none">
        <i id="iconimage" class="fa fa-chevron-left"></i>
    </div>

    <!-----以下檢索精靈相關--->
    <!--機台選擇-->


    <!--簡易報工用-->
    <div id="exampleModal_Report" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog2">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda22" style="padding: 5px 20px;">
                        <asp:TextBox ID="TextBox_machstatus" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_project" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_taskname" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_machname" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_task" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_id" runat="server" Style="display: none"></asp:TextBox>

                        <div class="col-xs-12 col-sm-12 col-md-12 ">
                            <div id="div_next" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px;">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                                    <b style="font-size: 30px">下筆工單內容</b>
                                    <hr style="border-bottom: 2px solid #E6E9ED; margin-top: 0px; margin-bottom: 0px" />
                                </div>
                                <br />
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center;">
                                    <table id="Table_next" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                        <thead>
                                            <tr style="background-color: white">
                                                <th style="text-align: center">機台名稱</th>
                                                <th style="text-align: center">品名規格</th>
                                                <th style="text-align: center">現在數量</th>
                                                <th style="text-align: center">目標數量</th>
                                                <th style="text-align: center">預計開工</th>
                                                <th style="text-align: center">預計結束</th>
                                                <th style="text-align: center">製令單號</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>


                            <div id="div_now" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                                    <b style="font-size: 30px">本筆工單內容</b>
                                    <hr style="border-bottom: 2px solid #E6E9ED; margin-top: 0px; margin-bottom: 0px" />
                                </div>
                                <br />
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center;">
                                    <table id="Table_now" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                        <thead>
                                            <tr style="background-color: white">
                                                <th style="text-align: center">機台名稱</th>
                                                <th style="text-align: center">品名規格</th>
                                                <th style="text-align: center">現在數量</th>
                                                <th style="text-align: center">目標數量</th>
                                                <th style="text-align: center">預計開工</th>
                                                <th style="text-align: center">預計結束</th>
                                                <th style="text-align: center">製令單號</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>


                                <div class="col-md-12">
                                    <br />
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12">
                                    <div class="col-md-5 ">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        工單狀態
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">

                                        <asp:RadioButtonList ID="RadioButtonList_status" runat="server" onclick="change_RadioButtonList()">
                                            <asp:ListItem Selected="True" Value="入站">入站</asp:ListItem>
                                            <asp:ListItem Value="出站">出站</asp:ListItem>
                                            <asp:ListItem Value="暫停">暫停</asp:ListItem>
                                            <asp:ListItem Value="取消暫停">取消暫停</asp:ListItem>
                                            <asp:ListItem Value="完成">完成</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:TextBox ID="TextBox_nowstatus" runat="server" Style="display: none"></asp:TextBox>

                                        <asp:TextBox ID="TextBox_ddlstatus" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-md-12">
                                    <br />
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12">
                                    <div class="col-md-5 ">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        人員選擇
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        <asp:DropDownList ID="DropDownList_Workman" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-md-12" style="display: none">
                                    <br />
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="display: none">
                                    <div class="col-md-5 ">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        成品狀態
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        <asp:DropDownList ID="DropDownList_QTYStatus" runat="server">
                                            <asp:ListItem Selected="True" Value="新增">新增</asp:ListItem>
                                            <asp:ListItem Value="不良">不良</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-md-12">
                                    <br />
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12">
                                    <div class="col-md-5 ">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        良品數量
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        <asp:TextBox ID="TextBox_QTY" TextMode="Number" runat="server" Enabled="False"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-md-12">
                                    <br />
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12">
                                    <div class="col-md-5 ">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        不良數量
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        <asp:TextBox ID="TextBox_bad" TextMode="Number" runat="server" Enabled="False"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <br />
                                </div>

                            </div>
                        </div>
                        <br />
                        <br />
                        <div style="text-align: center; padding: 15px;">
                            <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>
                            <button id="btnsave" type="button" class="btn btn-primary antosubmit2 ">送出</button>
                            <asp:Button ID="button_save" runat="server" Text="變更" class="btn btn-primary antosubmit2" OnClick="button_save_Click" Style="display: none" />

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!--簡易報工用-->



    <!--變更狀態用-->
    <div id="exampleModal_status" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmodal34" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down" style="width: 100%; text-align: center"><b>狀態變更</b></i>
                                <asp:TextBox ID="TextBox_machine" runat="server" Style="display: none"></asp:TextBox>
                                <asp:TextBox ID="TextBox_status" runat="server" Style="display: none"></asp:TextBox>


                            </h5>
                            <div class="row">

                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center; margin-bottom: 15px">

                                    <div class="col-xs-12 col-sm-12 col-md-4 ">
                                        異常狀態
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        <asp:DropDownList ID="DropDownList_errorstatus" runat="server"></asp:DropDownList>

                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center; margin-bottom: 15px">

                                    <div class="col-xs-12 col-sm-12 col-md-4 ">
                                        異常類型
                                    </div>

                                    <div class="col-xs-12 col-sm-12 col-md-1 ">
                                        <asp:DropDownList ID="DropDownList_tpye" runat="server"></asp:DropDownList>

                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12">
                                    <br />
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center; margin-bottom: 15px">
                                    <div class="col-xs-12 col-sm-12 col-md-4">
                                        異常原因
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-8 ">
                                        <asp:TextBox ID="TextBox_reason" TextMode="MultiLine" runat="server" Width="100%" Height="150%" Style="resize: none"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div style="text-align: center; padding: 15px;">
                                <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>
                                <button id="btnstatus" type="button" class="btn btn-primary antosubmit2 ">送出</button>
                                <asp:Button ID="Button_status" runat="server" Text="變更狀態" Style="display: none" OnClick="Button_status_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- /page content -->
    <!-- jQuery -->
    <script src="../../assets/vendors/jquery/dist/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="../../assets/vendors/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- FastClick -->
    <script src="../../assets/vendors/fastclick/lib/fastclick.js"></script>
    <!-- NProgress -->
    <script src="../../assets/vendors/nprogress/nprogress.js"></script>
    <!-- bootstrap-progressbar -->
    <script src="../../assets/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"></script>
    <!-- iCheck -->
    <script src="../../assets/vendors/iCheck/icheck.min.js"></script>
    <!-- bootstrap-daterangepicker -->
    <script src="../../assets/vendors/moment/min/moment.min.js"></script>
    <script src="../../assets/vendors/bootstrap-daterangepicker/daterangepicker.js"></script>
    <!-- bootstrap-wysiwyg -->
    <script src="../../assets/vendors/bootstrap-wysiwyg/js/bootstrap-wysiwyg.min.js"></script>
    <script src="../../assets/vendors/jquery.hotkeys/jquery.hotkeys.js"></script>
    <script src="../../assets/vendors/google-code-prettify/src/prettify.js"></script>
    <!-- jQuery Tags Input -->
    <script src="../../assets/vendors/jquery.tagsinput/src/jquery.tagsinput.js"></script>
    <!-- Switchery -->
    <script src="../../assets/vendors/switchery/dist/switchery.min.js"></script>
    <!-- Select2 -->
    <script src="../../assets/vendors/select2/dist/js/select2.full.min.js"></script>
    <!-- Parsley -->
    <script src="../../assets/vendors/parsleyjs/dist/parsley.min.js"></script>
    <!-- Autosize -->
    <script src="../../assets/vendors/autosize/dist/autosize.min.js"></script>
    <!-- jQuery autocomplete -->
    <script src="../../assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"></script>
    <!-- starrr -->
    <script src="../../assets/vendors/starrr/dist/starrr.js"></script>
    <!-- Custom Theme Scripts -->
    <script src="../../assets/build/js/custom.min.js"></script>
    <!-- FloatingActionButton -->
    <script src="../../assets/vendors/FloatingActionButton/js/index.js"></script>
    <!-- canvasjs -->
    <script src="../../assets/vendors/canvas_js/canvasjs.min.js"></script>
    <!-- Datatables -->
    <script src="../../assets/vendors/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="../../assets/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/dataTables.buttons.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.flash.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.html5.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.print.min.js"></script>
    <script src="../../assets/vendors/datatables.net-responsive/js/dataTables.responsive.min.js"></script>
    <script src="../../assets/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js"></script>
    <script src="../../assets/vendors/datatables.net-scroller/js/dataTables.scroller.min.js"></script>
    <script src="../../assets/vendors/jszip/dist/jszip.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/pdfmake.min.js"></script>
    <script src="../../assets/vendors/datatables.net-colReorder/dataTables.colReorder.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <!--甘特圖元件-->

    <script src="../../gantt/js/jquery.fn.gantt.js"></script>
    <script src="../../gantt/js/prettify.min.js"></script>
    <script src="../../assets/vendors/Create_HtmlCode/HtmlCode.js"></script>
    <script>  


        //===========================================表格===========================================
        $('#datatable_Info').dataTable(
            {
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
                "order": [[1, "asc"]],

                scrollCollapse: true,

                "lengthMenu": [[-1, 10, 25, 50, 100], ["All", 10, 25, 50, 100]],
                dom: "<'row'<'pull-left'f>'row'<'col-sm-3'>'row'<'col-sm-3'B>'row'<'pull-right'l>>" +
                    "<rt>" +
                    "<'row'<'pull-left'i>'row'<'col-sm-4'>row'<'col-sm-3'>'row'<'pull-right'p>>",

                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],

            });

        jQuery('.dataTable').wrap('<div class="dataTables_scroll" />');
        //============================================================================================
        window.onload = onload_event();
        function show_bar() {
            var WhatSystem = navigator.userAgent;
            if (WhatSystem.match(/android/i) || WhatSystem.match(/(iphone|ipad|ipod);?/i)) {

            } else
                document.getElementById("ContentPlaceHolder1_div_button").style.display = 'block';

        }
        function hide_bar() {
            var WhatSystem = navigator.userAgent;
            if (WhatSystem.match(/android/i) || WhatSystem.match(/(iphone|ipad|ipod);?/i)) {

            } else
                document.getElementById("ContentPlaceHolder1_div_button").style.display = 'none';
        }
        function onload_event() {
            var urlname = document.URL;

            if (urlname.indexOf('tab_content2') != -1) {
                var WhatSystem = navigator.userAgent;
                if (WhatSystem.match(/android/i) || WhatSystem.match(/(iphone|ipad|ipod);?/i)) {

                } else
                    document.getElementById("ContentPlaceHolder1_div_button").style.display = 'block';
            }

            else
                document.getElementById("ContentPlaceHolder1_div_button").style.display = 'none';
        }

        function status_change(status) {
            $('#ContentPlaceHolder1_TextBox_status').val('' + status + '');
            document.getElementById('<%=Button_status.ClientID %>').click();
        }
        function print_machine(mach) {
            $('#ContentPlaceHolder1_TextBox_reason').val('' + '' + '');


            $('#ContentPlaceHolder1_TextBox_machine').val('' + mach + '');

        }

        //圖塊模式右邊的資訊
        function get_information(text) {
            var divname = document.getElementById('machine_information');
            var string_array = text.split(',');
            var value = '';

            for (i = 0; i < string_array.length - 1; i++) {
                value +=
                    ' <div class="col-md-12 col-sm-12 col-xs-12">' +
                    ' <div class="col-md-12 col-sm-12 col-xs-12" style="font-size:22px;background-color:#f0f0f0;">' +
                    ' <b style="color:red;">' + string_array[i] + '</b>' +
                    ' </div>' +
                    ' <br>' +
                    ' <div class="col-md-12 col-sm-12 col-xs-12" style="font-size:22px;">' +
                    ' <b>' + string_array[i + 1].replaceAll('&', ' ') + '</b>' +
                    ' </div>' +
                    ' </div>';
                i++;
            }
            value += '<br><br><div style="font-size:25px;text-align:right;"><b><button type="button" style="width:100px;height:40px;font-size:18px" class="btn btn-success" onclick="javascript:location.href=' + "'" + '../dp_APS/WorkHourList.aspx?key=' + string_array[string_array.length - 1] + "'" + '">進入報工</button></b></div>';



            divname.innerHTML =
                '<div id="Div_Shadow" class="col-md-3 col-sm-12 col-xs-12" style="border:1px blue solid;padding:20px;">' +
                value +
                '</div>';
        }

        function set_value(status, project, taskname, mach, task, id, now_status, next_detail, now_detail) {

            //先讓全部可以動作
            var rb = document.getElementById("<%=RadioButtonList_status.ClientID %>");
            var rbItems = rb.getElementsByTagName('input');
            for (var itemIndex = 0; itemIndex < rbItems.length; itemIndex++) {
                rbItems[itemIndex].disabled = false;
            }

            //依據上一筆狀態決定可動作按鍵
            switch (now_status) {
                case '入站':
                    rbItems[0].disabled = true;
                    rbItems[3].disabled = true;
                    break;
                case '出站':
                    rbItems[1].disabled = true;
                    rbItems[2].disabled = true;
                    rbItems[3].disabled = true;
                    break;
                case '暫停':
                    rbItems[0].disabled = true;
                    rbItems[2].disabled = true;
                    break;
                case '取消暫停':
                    rbItems[0].disabled = true;
                    rbItems[3].disabled = true;
                    break;
                case '完成':
                    rbItems[2].disabled = true;
                    rbItems[3].disabled = true;
                    break;
                case '':
                    rbItems[2].disabled = true;
                    rbItems[3].disabled = true;
                    break;
            }

            //聚焦在能動作的第一個，並且填到Text內
            var text = '';
            for (var itemIndex = 0; itemIndex < rbItems.length; itemIndex++) {
                if (rbItems[itemIndex].disabled == false) {
                    text = rbItems[itemIndex].value;
                    $("#ContentPlaceHolder1_RadioButtonList_status :radio[value='" + text + "']").prop("checked", true);
                    break;
                }

            }

            if (text == '出站' || text == '完成') {
                document.getElementById('ContentPlaceHolder1_TextBox_QTY').disabled = false;
                document.getElementById('ContentPlaceHolder1_TextBox_bad').disabled = false;
            }
            else {
                document.getElementById('ContentPlaceHolder1_TextBox_QTY').disabled = true;
                document.getElementById('ContentPlaceHolder1_TextBox_bad').disabled = true;
            }


            ////把目前dropdownlist的項目寫入Textbox內
            $('#ContentPlaceHolder1_TextBox_ddlstatus').val('' + text + '');

            //記錄當下最後一筆的資料->用於比對
            $('#ContentPlaceHolder1_TextBox_nowstatus').val('' + now_status + '');
            $('#ContentPlaceHolder1_TextBox_machstatus').val('' + status + '');
            $('#ContentPlaceHolder1_TextBox_project').val('' + project + '');
            $('#ContentPlaceHolder1_TextBox_taskname').val('' + taskname + '');
            $('#ContentPlaceHolder1_TextBox_machname').val('' + mach + '');
            $('#ContentPlaceHolder1_TextBox_task').val('' + task + '');
            $('#ContentPlaceHolder1_TextBox_id').val('' + id + '');

            //顯示下筆排成表格
            var next_list = next_detail.replaceAll('*', ' ').split('^');

            var nexttable = $('#Table_next').dataTable();
            nexttable.fnClearTable();
            var next_data = [];
            if (next_list.length > 1) {
                for (i = 0; i < next_list.length - 1; i++)
                    next_data.push('<center>' + next_list[i] + '</center>');
                nexttable.fnAddData(next_data);
            }



            //顯示當筆排成表格
            var now_list = now_detail.replaceAll('*', ' ').split('^');
            var now_data = [];
            var nowtable = $('#Table_now').dataTable();
            nowtable.fnClearTable();
            if (now_list.length > 1) {
                for (i = 0; i < now_list.length - 1; i++)
                    now_data.push('<center>' + now_list[i] + '</center>');
                nowtable.fnAddData(now_data);
            }

        }

        //取得使用者所選之狀態
        function change_RadioButtonList() {
            var rb = document.getElementById("<%=RadioButtonList_status.ClientID%>");
            var inputs = rb.getElementsByTagName('input');
            var flag = false;
            var selected;
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].checked) {
                    selected = inputs[i];
                    flag = true;
                    break;
                }
            }
            if (selected.value == '出站' || selected.value == '完成') {
                document.getElementById('ContentPlaceHolder1_TextBox_QTY').disabled = false;
                document.getElementById('ContentPlaceHolder1_TextBox_bad').disabled = false;
            }
            else {
                document.getElementById('ContentPlaceHolder1_TextBox_QTY').disabled = true;
                document.getElementById('ContentPlaceHolder1_TextBox_bad').disabled = true;
            }

            $('#ContentPlaceHolder1_TextBox_ddlstatus').val('' + selected.value + '');
        }
        //當按鈕按下的時候，先執行LOADING的JS事件，在進行後台的計算
        $("#btnsave").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('<%=button_save.ClientID %>').click();
        });

        function hide_div() {
            var tablename = document.getElementById("table_information");
            var divname = document.getElementById("_select");
            var icon = document.getElementById("iconimage");


            if (tablename.className == 'col-md-9 col-sm-12 col-xs-12') {
                icon.className = 'fa fa-chevron-left';
                tablename.className = 'col-md-12 col-sm-12 col-xs-12';
                divname.style.display = 'none';

            }
            else {
                icon.className = "fa fa-chevron-right";
                tablename.className = 'col-md-9 col-sm-12 col-xs-12';
                divname.style.display = 'block';
            }



        }
        $(".iconimage").on("click", function () {
            hide_div();
        });

        $("#btncheck").click(function () {

            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btncheck').disabled = true;
            document.getElementById('<%=LinkButton_ok.ClientID %>').click();

        });
        $("#btnstatus").click(function () {

            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btnstatus').disabled = true;
            document.getElementById('<%=Button_status.ClientID %>').click();

        });
        $(function () {
            //            "use strict";
            //初始化gantt
            $(".gantt").gantt({
                source: [
                <%=Gantt_Data%>
                ],
                navigate: 'scroll',//buttons  scroll
                scale: "days",// months  weeks days  hours
                maxScale: "months",
                minScale: "hours",
                itemsPerPage: 20,
                onRender: function () {
                    if (window.console && typeof console.log === "function") {
                        console.log("chart rendered");
                    }
                }
            });
        });



        function gantt_show() {

        
            $(".gantt").gantt({
                source: [
                <%=Gantt_Data%>
                  ],
                  navigate: 'scroll',//buttons  scroll
                  scale: "days",// months  weeks days  hours
                  maxScale: "months",
                  minScale: "hours",
                  itemsPerPage: 20,
                  onRender: function () {
                      if (window.console && typeof console.log === "function") {
                          console.log("chart rendered");
                      }
                  }
              });
             
         }

    </script>

</asp:Content>
