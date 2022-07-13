<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="recordsofchangetheorder.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.recordsofchangetheorder1" %>

<%--<%@ OutputCache duration="10" varybyparam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>訂單變更紀錄 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_SD/recordsofchangetheorder.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!--局部刷新-->
    <asp:ScriptManager ID="ScriptManager2" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <!--順序存至TEXTBOX-->
            <asp:TextBox ID="TextBox_SaveColumn" runat="server" Style="display: none"></asp:TextBox>
            <!--執行動作-->
            <asp:Button ID="Button_SaveColumns" runat="server" Text="Button" OnClick="Button_SaveColumns_Click" Style="display: none" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <%= path %>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <div class="page-title">
            <div class="row">
                <div class="col-md-6 col-sm-12 col-xs-12">
                </div>
            </div>
        </div>
        <div class="clearfix"></div>
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
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="col-md-9 col-sm-12 col-xs-12 _Img">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div style="text-align: right; width: 100%; padding: 0;">
                                            <button style="display: none" type="button" id="exportChart" title="另存成圖片">
                                                <img src="../../assets/images/download.jpg" style="width: 36.39px; height: 36.39px;">
                                            </button>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div id="chartContainer"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="dashboard_graph x_panel">
                                        <div class="x_content">
                                            
                                                <ContentTemplate>

                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                            <span>顯示筆數</span>
                                                        </div>
                                                        <div class="col-md-5 col-sm-12 col-xs-4" style="margin: 0px 0px 5px 0px">

                                                            <asp:TextBox ID="txt_showCount" runat="server" Text="10" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-3 col-sm-12 col-xs-3" style="margin: 5px 0px 5px 0px">
                                                            <span>
                                                                <asp:CheckBox ID="CheckBox_All" runat="server" Text="全部" AutoPostBack="true" OnCheckedChanged="CheckBox_All_CheckedChanged" /></span>

                                                        </div>
                                                    </div>


                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                            <span>日期快選</span>
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-7" style="margin: 0px 0px 5px 0px">
                                                            <div class="btn-group btn-group-justified">
                                                                <asp:LinkButton ID="LinkButton_month" runat="server" CssClass="btn btn-default " OnClick="button_select_Click" Style="text-align: left">當月</asp:LinkButton>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                            <span>變更日期</span>
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-7" style="margin: 0px 0px 5px 0px">
                                                            <asp:TextBox ID="txt_str" runat="server" Style="" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                                        <div class="col-md-4 col-sm-3 col-xs-5">
                                                        </div>
                                                        <div class="col-md-8 col-sm-9 col-xs-7" style="margin: 0px 0px 5px 0px">
                                                            <asp:TextBox ID="txt_end" runat="server" Style="" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>


                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="col-md-9 col-xs-8">
                                                </div>
                                                <div class="col-md-3 col-xs-12">
                                                    <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
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
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">


                            <div class="x_content">
                                <div class="x_panel">
                                    <div class="x_title">
                                        <h1 class="text-center _mdTitle" style="width: 100%"><b><%=time_area_text %></b></h1>
                                        <h3 class="text-center _xsTitle" style="width: 100%"><b><%=time_area_text %></b></h3>
                                        <div class="clearfix"></div>
                                    </div>
                                    <p class="text-muted font-13 m-b-30">
                                    </p>
                                    <table id="datatable-buttons" class="table  table-ts table-bordered nowrap" cellspacing="0" width="100%">
                                        <!--<table id="datatable-responsive" class="table table-striped table-bordered dt-responsive nowrap" cellspacing="0" width="100%">-->
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
                    </div>
                </div>
            </div>
        </div>

        <!-----------------/content------------------>
    </div>


    <!-- /Modal -->
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
    <!-- bootstrap-touchspin-master -->
    <script src="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.js"></script>
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
    <script>
        // 2019/06/11，資料送出前，進行日期格式驗證
        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
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
            //exportEnabled: true,
            animationEnabled: true,
            colorSet: "greenShades",
            theme: "light2",
            title: {
                fontFamily: "NotoSans",
                text: <%=title_text%>,
                fontWeight: "bolder",
            }, subtitles: [{
                text: '<%=timerange %>',
                fontFamily: 'NotoSans',
                fontWeight: 'bolder',
                textAlign: "'center",
            }],
            //dataPointWidth: 40,
            axisX: {
                title: "客戶",
                interval: 1, labelAngle: -180,
                intervalType: "year"
            },
            axisY: [{
                title: "次數",
                lineThickness: 1,
                lineColor: "#d0d0d0",
                gridColor: "transparent",
            }, {
                title: "",
                lineColor: "#369EAD",
                tickColor: "#369EAD",
                labelFontColor: "#369EAD",
                titleFontColor: "#369EAD",
                suffix: ""

            }],
            legend: {
                fontSize: 15,
                cursor: "pointer",
            },
            toolTip: {
                shared: true,
                content: toolTipContent
            },
            data: [{
                type: "column",
                indexLabelPlacement: "outside", indexLabelBackgroundColor: "white",
                //showInLegend: true,
                name: <%= data_name%>,
                dataPoints: [
                    <%=col_data_Points%>
                ]
            }]
        });
        chart.render();

        document.getElementById("exportChart").addEventListener("click", function () {
            chart.exportChart({ format: "png" });
            parent.location.reload();
        });

        function toolTipContent(e) {
            var str = "";
            var total = 0;
            var str2;
            for (var i = 0; i < e.entries.length; i++) {
                var str1 = "<span style= 'color:" + e.entries[i].dataSeries.color + "'> " + e.entries[i].dataSeries.name + "</span>: <strong>" + e.entries[i].dataPoint.y + "</strong><br/>";
                total = e.entries[i].dataPoint.y + total;
                str = str.concat(str1);
            }
            str2 = "<span style = 'color:DodgerBlue;'><strong>" + (e.entries[0].dataPoint.label) + "</strong></span><br/>";
            str3 = "<span style = 'color:Tomato'>Total:</span><strong>" + total + "</strong><br/>";
            return (str2.concat(str)).concat(/*str3*/);
        }


        //防止切換頁籤時跑版
        $(document).ready(function () {
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
    </script>
</asp:Content>
