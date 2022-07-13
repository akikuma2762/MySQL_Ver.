<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="stockanalysis.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.stockanalysis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>成品庫存分析 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_WH/stockanalysis.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!--局部刷新-->
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <!--順序存至TEXTBOX-->
            <asp:TextBox ID="TextBox_SaveColumn" runat="server" Style="display: none"></asp:TextBox>
            <!--執行動作-->
            <asp:Button ID="Button_SaveColumns" runat="server" Text="Button" OnClick="Button_SaveColumns_Click" Style="display: none" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <%=path %>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <ul id="myTab" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true">圖片模式</a>
            </li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" id="profile-tab" role="tab" data-toggle="tab" aria-expanded="false">表格模式</a>
            </li>
        </ul>
        <div id="myTabContent" class="tab-content">
            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div class="col-md-9 col-sm-12 col-xs-12">
                            <div class="col-md-6 col-sm-12 col-xs-12">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div style="text-align: right; width: 100%; padding: 0;">
                                            <button style="display: none" type="button" id="exportChart" title="另存成圖片">
                                                <img src="../../assets/images/download.jpg" style="width: 36.39px; height: 36.39px;">
                                            </button>
                                            <div class="row">
                                                <div class="col-md-10 col-sm-10 col-xs-12">
                                                    <div id="chartpie" style="height: 450px; width: 100%; margin: 0px auto;"></div>
                                                </div>
                                                <br>
                                                <br>
                                                <div class="col-md-2 col-sm-4 col-xs-4" style="text-align: center">
                                                    <div class="h2 mb-0 text-primary" style="margin-bottom: 10px; color: #221715;"><%=_val總庫存 %></div>
                                                    <div class="text-muted">庫存總數</div>
                                                    <hr>
                                                </div>

                                                <div class="col-md-2 col-sm-4 col-xs-4" style="text-align: center">
                                                    <div class="h2 mb-0 text-primary" style="margin-bottom: 10px; color: #1b5295;"><%=_val一般庫存 %></div>
                                                    <div class="text-muted">一般庫存</div>
                                                    <hr>
                                                </div>
                                                <div class="col-md-2 col-sm-4 col-xs-4" style="text-align: center">
                                                    <div class="h2 mb-0 text-warning" style="margin-bottom: 10px; color: #d93f47;"><%=_val逾期庫存 %></div>
                                                    <div class="text-muted">逾期庫存</div>
                                                    <hr>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-6 col-sm-12 col-xs-12 _BottomImg">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div style="text-align: right; width: 100%; padding: 0;">
                                                    <button style="display: none" type="button" id="exportimage" title="另存成圖片">
                                                        <img src="../../assets/images/download.jpg" style="width: 36.39px; height: 36.39px;">
                                                    </button>
                                                </div>
                                                <div id="chartContainer" style="height: 450px; width: 100%; margin: 0px auto;"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="dashboard_graph x_panel">
                                <div class="x_content">

                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                        <div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">
                                            <span>庫存天數</span>
                                        </div>
                                        <div class="col-md-5 col-sm-12 col-xs-4" style="margin: 0px 0px 5px 0px">

                                            <asp:TextBox ID="txt_showCount" runat="server" Text="30" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                        </div>
                                        <div class="col-md-3 col-sm-12 col-xs-3" style="margin: 5px 0px 5px 0px">
                                            <span>
                                                <asp:CheckBox ID="CheckBox_All" runat="server" Text="全部" onclick="checkstatus()" />
                                            </span>
                                        </div>
                                    </div>




                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <div class="col-md-9 col-xs-8">
                                        </div>
                                        <div class="col-md-3 col-xs-12">
                                            <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行搜索</button>
                                            <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
                                        </div>
                                    </div>


                                </div>
                            </div>
                        </div>



                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="x_content">
                                <div class="x_panel">
                                    <div class="x_title">
                                        <h1 class="text-center _mdTitle" style="width: 100%"><b><%=Table_Title %></b></h1>
                                        <h3 class="text-center _xsTitle" style="width: 100%"><b><%=Table_Title %></b></h3>
                                        <div class="clearfix"></div>
                                    </div>
                                    <p class="text-muted font-13 m-b-30">
                                    </p>
                                    <table id="datatable-buttons" class="table table-ts table-bordered nowrap" cellspacing="0" width="100%">
                                        <thead>
                                            <tr id="tr_row">
                                                <%=th%>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%=tr %>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div id="order_total" style="display: <%=SubTotal%>"></div>
                    </div>
                </div>
            </div>
        </div>
        <!-----------------/content------------------>
    </div>



    <!-- jQuery -->
    <script src="../../assets/vendors/jquery/dist/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="../../assets/vendors/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- FastClick -->
    <script src="../../assets/vendors/fastclick/lib/fastclick.js"></script>
    <!-- iCheck -->
    <script src="../../assets/vendors/iCheck/icheck.min.js"></script>
    <!-- bootstrap-daterangepicker -->
    <script src="../../assets/vendors/moment/min/moment.min.js"></script>
    <script src="../../assets/vendors/bootstrap-daterangepicker/daterangepicker.js"></script>
    <!-- Switchery -->
    <script src="../../assets/vendors/switchery/dist/switchery.min.js"></script>
    <!-- Select2 -->
    <script src="../../assets/vendors/select2/dist/js/select2.full.min.js"></script>
    <!-- Autosize -->
    <script src="../../assets/vendors/autosize/dist/autosize.min.js"></script>
    <!-- jQuery autocomplete -->
    <script src="../../assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"></script>
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
    <script src="../../assets/vendors/datatables.net-colReorder/dataTables.colReorder.min.js"></script>
    <script src="../../assets/vendors/jszip/dist/jszip.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/pdfmake.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <script src="../../assets/vendors/time/loading.js"></script>
    <script src="../../assets/vendors/Create_HtmlCode/HtmlCode20211117.js"></script>
    <!--返回上一個畫面時，要回到上一動-->
    <script src="../../assets/vendors/cookies/cookie_action.js"></script>

    <script>

        function checkstatus() {
            var checkBox = document.getElementById('ContentPlaceHolder1_CheckBox_All');
            var text = document.getElementById('ContentPlaceHolder1_txt_showCount');
            if (checkBox.checked == true) {
                text.disabled = true;
            } else {
                text.disabled = false;
            }
        }
        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });
        var chartpie = new CanvasJS.Chart("chartpie",
            {
                //exportEnabled: true,
                animationEnabled: true,
                title: {
                    text: '總體庫存分析',
                    fontFamily: "NotoSans",
                    fontWeight: "bolder",
                },
                subtitles: [{
                    text: <%=title_text%>,
                    fontFamily: 'NotoSans',
                    fontWeight: 'bolder',
                    textAlign: "'center",
                }],
                legend: {
                    fontSize: 15,
                    cursor: "pointer",
                },
                data: [{
                    showInLegend: true,
                    type: "pie",
                    startAngle: 240,
                    yValueFormatString: "##0'%'",
                    indexLabel: "{label} {y}",
                    indexLabelFontSize: 18,
                    dataPoints: [
                    <%=pie_data_points%>
                    ]
                }]
            });
        document.getElementById("exportChart").addEventListener("click", function () {
            chartpie.exportChart({ format: "png" });
            parent.location.reload();
        });
        var chart = new CanvasJS.Chart("chartContainer", {
            //exportEnabled: true,
            animationEnabled: true,
            theme: "light2",
            title: {
                text: '各產線庫存分析 ',
                fontFamily: "NotoSans",
                fontWeight: "bolder",
            }, subtitles: [{
                text: <%=title_text%>,
                fontFamily: 'NotoSans',
                fontWeight: 'bolder',
                textAlign: "'center",
            }],
            axisX: {
                title: '產線',
                labelAngle: -180,
                intervalType: "year"
            },
            axisY: {
                title: '數量',
                lineThickness: 1,
                lineColor: "#d0d0d0",
                gridColor: "transparent",
            },
            legend: {
                fontSize: 15,
                cursor: "pointer",
            },
            toolTip: {
                shared: true,
                content: toolTipContent
            },
            data: [{
                type: "stackedColumn",
                showInLegend: true,
                color: "#5b59ac",
                //把%數加進去了
                name: "一般庫存" + '<%=nstock %>',
                dataPoints: [
                <%=col_data_points_nor %>
                ]
            },
                {
                    type: "stackedColumn",
                    showInLegend: true,
                    color: "#ff4d4d",
                    //把%數加進去了
                    name: "逾期庫存" + '<%=ostock %>',
                    dataPoints: [
                <%=col_data_points_sply %>
                    ]
                }]
        });
        document.getElementById("exportimage").addEventListener("click", function () {
            chart.exportChart({ format: "png" });
            parent.location.reload();
        });
        chart.render();
        chartpie.render();
        function toolTipContent(e) {
            var str = "";
            var total = 0;
            var str2, str3;
            for (var i = 0; i < e.entries.length; i++) {
                var str1 = "<span style= 'color:" + e.entries[i].dataSeries.color + "'> " + e.entries[i].dataSeries.name + "</span>: <strong>" + e.entries[i].dataPoint.y + "</strong><br/>";
                total = e.entries[i].dataPoint.y + total;
                str = str.concat(str1);
            }
            str2 = "<span style = 'color:DodgerBlue;'><strong>" + (e.entries[0].dataPoint.label) + "</strong></span><br/>";
            str3 = "<span style = 'color:Tomato'>Total:</span><strong>" + total + "</strong><br/>";
            return (str2.concat(str)).concat(str3);
        }
        $(document).ready(function () {
            jQuery('.dataTable').wrap('<div class="dataTables_scroll" />');


            return_preaction('stockanalysis=stockanalysis_cust', '#datatable-buttons')

            $('#example').DataTable({
                responsive: true
            });
            $('#exampleInTab').DataTable({
                responsive: true
            });

            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                $($.fn.dataTable.tables(true)).DataTable()
                    .columns.adjust()
                    .responsive.recalc();
            });
        });
        //返回上一頁的時候在原來的TAB
        $(function () {
            var hash = window.location.hash;
            hash && $('ul.nav a[href="' + hash + '"]').tab('show');

            $('.nav-tabs a').click(function (e) {
                $(this).tab('show');
                var scrollmem = $('body').scrollTop() || $('html').scrollTop();
                window.location.hash = this.hash;
                $('html,body').scrollTop(scrollmem);
            });
        });

        $(document).ready(function () {
            //抓到表格
            var table = $('#datatable-buttons').DataTable();

            //設定開關
            var ok = true;

            //當滑鼠移動至欄位，可以動作
            $('#datatable-buttons').on('mouseenter', 'thead tr', function () {
                ok = true;
            });

            //抓取移動事件
            table.on('column-reorder', function (e, settings, details) {
                //抓取放開事件
                window.addEventListener('mouseup', e => {
                    var tharray = [];
                    //取得欄位名稱及順序
                    $('#tr_row > th').each(function () {
                        tharray.push($(this).text())
                    })

                    //組合成文字
                    var thname = '';
                    for (i = 0; i < tharray.length; i++) {
                        if (tharray[i] != '')
                            thname += tharray[i] + ',';
                    }
                    //只會執行最後一次
                    if (ok) {
                        //寫到TextBox內
                        $('#<%=TextBox_SaveColumn.ClientID%>').val('' + thname + '');
                        //執行事件
                        if (thname != '')
                            document.getElementById('<%=Button_SaveColumns.ClientID %>').click();
                    }
                    ok = false;
                });
            });
        });
        //20220610 新增小計table
        order_TotalTable();
        //-------------------開始-------------------//20220610 總計table監聽
        function order_TotalTable() {
            var td = "<tr><td>小計</td>";
            var th = "<tr><td>機種統計</td>";
            var total = 0;
            var count = 0;
            var count_th = 0;
            var noData = document.getElementsByClassName("dataTables_empty");
            if (noData[0] != undefined && noData[0].innerText == "沒有符合的結果") {
                td = "";
                td += `<td colspan="9" style="text-align:center;" >沒有符合的結果</td>`;
                create_tablecode('order_total', "", 'order_totals', '<%=th.ToString() %>', td);
                set_TotalTable('#order_totals');
                return;
            }
            //計算tbody tr總數
            $("#datatable-buttons tbody tr").each(function () {
                count = count + 1;
            });
            //計算 thead tr總數
            $("#datatable-buttons thead #tr_row th").each(function () {
                count_th = count_th + 1;
            });
            //製作新的thead tr資料
            if (count_th != 0) {
                for (var j = 1; j < count_th; j++) {
                    console.log($("#datatable-buttons").children(0).children(0)[0].cells[j].innerText);
                    var tr = $("#datatable-buttons").children(0).children(0)[0].cells[j].innerText;
                    th += `<td>${tr}</td>`;
                }
                th += `</tr>`;
            }

            //製作新的tbody 資料
            for (var j = 1; j < count_th; j++) {
                total = 0;
                for (var i = 0; i < count; i++) {
                    total += parseInt($("#datatable-buttons").children(0).children(0)[i + 1].cells[j].innerText);
                }
                td += `<td id=total_${j}>${total}</td>`;
            }
            td += "</tr >";
            create_TotalTableCode('order_total', "", 'order_totals', th, td);
            set_TotalTable('#order_totals');
        }
        //監聽這幾行在datatable-buttons 下無效  要再找看看20220616 juiedit
        $('select[name="datatable-buttons_length"]').change(function () {
            order_TotalTable();
        });
        $('#datatable-buttons_filter input').on("input propertychange", function () {
            order_TotalTable();
        });
        $('#tr_row').click(function () {
            order_TotalTable();
        })
        $('#datatable-buttons_paginate').click(function () {
            order_TotalTable();
        })
        //---------------------結束------------------------//
    </script>
</asp:Content>
