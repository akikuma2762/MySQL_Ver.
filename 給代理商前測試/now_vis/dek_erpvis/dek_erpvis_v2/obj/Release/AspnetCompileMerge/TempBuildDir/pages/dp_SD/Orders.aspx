<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.Orders" %>

<%--<%@ OutputCache duration="10" varybyparam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>訂單統計 | 德大機械</title>
    <%=color %>

    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/Orders.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>

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
    <style>
        #indexlabel {
            color: black;
            position: absolute;
            font-size: 16px;
        }

        footer {
            padding-top: 12px;
        }
    </style>
    <div class="right_col" role="main">
        <%= path %><br>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <!--以上狀態統計色塊-->
        <ul id="myTab" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true">圖片模式</a>
            </li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" id="profile-tab" role="tab" data-toggle="tab" aria-expanded="false">表格模式</a>
            </li>
        </ul>
        <div id="myTabContent" class="tab-content">
            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">
                <div class="x_panel Div_Shadow">
                    <div class="x_content">
                        <div class="row">
                            <div class="col-md-9 col-sm-12 col-xs-12">
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="dashboard_graph x_panel">
                                        <div class="x_content">
                                            <div class="_safe">
                                                <button style="display: none" type="button" id="exportChart" title="另存成圖片">
                                                    <img src="../../assets/images/download.jpg" style="width: 36.39px; height: 36.39px;">
                                                </button>
                                            </div>
                                            <div class="col-md-12 col-sm-12 col-xs-12 _setborder">
                                                <div id="chartContainer"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12 col-xs-12">
                                <%-- 2019.08.05，訂單總計資訊(右上角) --%>
                                <div class="col-md-12 col-sm-12 col-xs-12 _OrderInformation _setborder">
                                    <div class="x_panel">
                                        <div class="x_content clearfix">
                                            <h3 style="color: black">總計：</h3>
                                            <div class="h2 text-success count" style="color: darkred"><b><%=排行內總計 %></b></div>
                                            <div runat="server" id="divBlock">
                                                <h2 style="color: black"><%=right_title %>： <strong style="color: darkred">
                                                    <br>
                                                    <%=first_N %></strong></h2>
                                                <h2 style="color: black">佔總訂單： <strong class="_ColorDark">
                                                    <br>
                                                    <%=rate %> %</strong></h2>
                                                <div class="progress progress_sm" style="width: 95%;">
                                                    <div class="progress-bar bg-green" role="progressbar" data-transitiongoal="<%=rate %>"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%-- 2019.08.05，篩選條件 --%>
                                <div class="col-md-12 col-sm-12 col-xs-12 _select  _setborder">
                                    <div class="dashboard_graph x_panel">
                                        <div class="x_content">
                                            <i id="cbx_remind"></i>
                                            <div class="col-md-12 col-sm-6 col-xs-12">
                                                <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                    <span>X座標(值)</span>
                                                </div>
                                                <div class="col-md-8 col-sm-9 col-xs-8">
                                                    <asp:DropDownList ID="dropdownlist_X" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">
                                                        <asp:ListItem Value="Line" Selected="True">機種</asp:ListItem>
                                                        <asp:ListItem Value="Custom">客戶</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-12 col-sm-6 col-xs-12">
                                                <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                    <span>Y座標(值)</span>
                                                </div>
                                                <div class="col-md-8 col-sm-9 col-xs-8">
                                                    <asp:DropDownList ID="dropdownlist_y" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="True">
                                                <ContentTemplate>
                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                            <span>日期類型</span>
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-8">
                                                            <asp:DropDownList ID="dropdownlist_Datetype" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">
                                                                <asp:ListItem Value="0">預交日期</asp:ListItem>
                                                                <asp:ListItem Value="1">訂單日期</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                            <span>日期快選</span>
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-8">
                                                            <div class="btn-group btn-group-justified" style="margin: 0px 0px 5px 0px">
                                                                <asp:LinkButton ID="LinkButton_month" runat="server" CssClass="btn btn-default " OnClick="Button_submit_Click" Style="text-align: left">當月</asp:LinkButton>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                            <span>起始日期</span>
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                            <asp:TextBox ID="textbox_dt1" runat="server" Style="" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-4">
                                                            <span>結束日期</span>
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                            <asp:TextBox ID="textbox_dt2" runat="server" CssClass="form-control  text-left" TextMode="Date"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <div class="col-md-12 col-sm-6 col-xs-12">
                                                <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                    <span>訂單狀態</span>
                                                </div>
                                                <div class="col-md-8 col-sm-9 col-xs-8">
                                                    <asp:DropDownList ID="DropDownList_orderStatus" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-12 col-sm-6 col-xs-12">
                                                <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                    <span>顯示筆數</span>
                                                </div>
                                                <div class="col-md-5 col-sm-12 col-xs-5" style="margin: 0px 0px 5px 0px">
                                                    <asp:TextBox ID="txt_showCount" runat="server" Text="10" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                                </div>
                                                <div class="col-md-3 col-sm-12 col-xs-3 text-right" style="margin: 5px 0px 5px 0px;">
                                                    <span>
                                                        <asp:CheckBox ID="CheckBox_All" runat="server" Text="全部" onclick="checkstatus()" />
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                        <br />
                                        <div>
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="col-md-9 col-xs-8">
                                                </div>
                                                <div class="col-md-3 col-xs-12">
                                                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2 ">執行搜索</button>
                                                    <asp:Button runat="server" Text="提交" ID="Button_select" CssClass="btn btn-primary" Style="display: none" OnClick="Button_submit_Click" />
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
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div class="x_panel Div_Shadow">
                    <div class="x_content">
                        <div class="x_panel">
                            <div id="_FormTitle" class="x_title" style="text-align: center">
                                <h1 class="text-center _mdTitle" style="width: 100%"><b>訂單<%=yText %>統計</b></h1>
                                <h3 class="text-center _xsTitle" style="width: 100%"><b>訂單<%=yText %>統計</b></h3>
                                <div class="clearfix"></div>
                            </div>
                            <table id="datatable-buttons" class="table  table-ts table-bordered nowrap" border="1" cellspacing="0" width="100%">
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
                    <div id="order_total" style="display:<%=SubTotal%>"></div>
                </div>
            </div>
            <%--以上圖塊模式--%>
        </div>
        <!--Table-->
        <!-----------------/content------------------>
    </div>
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
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <script src="../../assets/vendors/datatables.net-colReorder/dataTables.colReorder.min.js"></script>
    <script src="../../assets/vendors/time/loading.js"></script>
    <script src="../../assets/vendors/Create_HtmlCode/HtmlCode20211117.js"></script>
    <!--返回上一個畫面時，要回到上一動-->
    <script src="../../assets/vendors/cookies/cookie_action.js"></script>
    <script>
        //20220610 新增小計table
        order_TotalTable();
        //防止切換頁籤時跑版
        $(document).ready(function () {
            return_preaction('Order=Order_cust', '#datatable-buttons')
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
            document.getElementById('<%=Button_select.ClientID %>').click();
        });
        CanvasJS.addColorSet("greenShades", [
            "#4656cc",
            "#3d1b41",
            "#c3cdf5",
            "#e43849",
            "#ea601b",
            "#991f42",
            "#f5b025",
            "#84ac52",
            "#5db0c3",
            "#1ea1d1",
            "#18478e",
            "#003c55"]);
        var chart = new CanvasJS.Chart("chartContainer", {
            animationEnabled: true,
            colorSet: "greenShades",
            theme: "light1",
            title: {
                text: '<%=title%>',
                fontFamily: 'NotoSans',
                fontWeight: 'bolder',
                textAlign: "'center",
            },
            subtitles: [{
                text: '<%=subtitle%>',
                fontFamily: 'NotoSans',
                fontWeight: 'bolder',
                textAlign: "'center",
            }],
            axisX: {
                title: '<%=xString %>',
                labelFontFamily: 'NotoSans',
                fontWeight: 'bolder',
                textAlign: "'center", labelAngle: -180,
                intervalType: "year"
            },
            axisY: [{
                title: '<%=yText %>',
                labelFontFamily: 'NotoSans',
                fontWeight: 'bolder',
                textAlign: "'center",
                lineThickness: 1,
                lineColor: "#d0d0d0",
                gridColor: "transparent",
            }],
            legend: {
                fontSize: 15,
                cursor: "pointer",
            },
            toolTip: {
                shared: true,
            },
            data: [{
                type:  '<%=chartType%>',
                indexLabelPlacement: "outside", indexLabelBackgroundColor: "white",
                name: '<%=chart_unit %>',
                dataPoints: [<%=chartData %>]
            }]
        });
        chart.render();
        document.getElementById("exportChart").addEventListener("click", function () {
            chart.exportChart({ format: "png" });
            parent.location.reload();
        });

        //-----------------------------
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
                    th += `<td style="text-align: center;">${tr}</td>`;
                }
                th += `</tr>`;
            }
            //製作新的tbody 資料
            for (var j = 1; j < count_th; j++) {
                total = 0;
			
                for (var i = 0; i < count; i++) {
				  var num = $("#datatable-buttons").children(0).children(0)[i + 1].cells[j].innerText;
                    total += parseInt(num.replace(/,/gi,''));
					 
                }
                td += `<td id=total_${j} align="right">${toCurrency(total)}</td>`;
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
		function toCurrency(num){
		var parts = num.toString().split('.');
		parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
		return parts.join('.');
        }
        //---------------------結束------------------------//
    </script>
</asp:Content>
