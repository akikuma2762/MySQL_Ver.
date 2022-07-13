<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="SupplierShortage.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PD.SupplierShortage" %>

<%--<%@ OutputCache duration="10" varybyparam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>供應商催料 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <%=color %>
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
    <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/dp_PD/SupplierShortage.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../assets/vendors/jquery/dist/jquery-ui.min.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="right_col" role="main">

        <!-----------------title------------------>
        <%= path %>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <div class="page-title">
            <div class="row">
                <%--                <div class="col-md-6 col-sm-12 col-xs-12">
                    <h4>[搜尋條件]：<u><asp:LinkButton ID="LinkButton1" runat="server" data-toggle="modal" data-target="#exampleModal" Style="font-size: 25px">設定搜尋條件</asp:LinkButton></u></h4>
                </div>--%>
            </div>
        </div>
        <div class="clearfix"></div>
        <!-----------------content------------------>

                <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                                      <div class="row">
                        <div class="col-md-9 col-sm-12 col-xs-12">
                            <div class="x_panel">
                                <div class="x_title">
                                    <h1 class="text-center _mdTitle" style="width: 100%;"><b>未交物料列表</b></h1>
                                    <h3 class="text-center _xsTitle" style="width: 100%;padding:10px"><b>未交物料列表</b></h3>
                                </div>
                                <div class="x_content">
                                    <p class="text-muted font-13 m-b-30">
                                    </p>
                                    <table id="datatable-buttons" class="table table-ts table-bordered nowrap" cellspacing="0" width="100%">
                                        <thead>
                                            <tr id="tr_row">
                                                <%=th%>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%= tr %>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3 col-sm-12 col-xs-12">
                            <div class="col-md-12 col-sm-12 col-xs-12 _summount _setborder">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <h3 style="color: black">總未交量：</h3>
                                        <div class="h2 text-success count" style="color: darkred"><b><%=未交量總計 %> 台</b></div>
                                    </div>
                                </div>
                            </div>


                            <div class="col-md-12 col-sm-12 col-xs-12 _select _setborder">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div class="col-12">
                                            <div class="col-md-5 col-sm-10 col-xs-12" style="margin: 5px 0px 5px 0px">
                                                <span>供應商代碼</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-12">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="textbox_dt1" CssClass="form-control" Text="" Width="100%"  runat="server" placeholder="請輸入供應商代碼"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="col-md-5 col-sm-10 col-xs-12" style="margin: 5px 0px 5px 0px">
                                                <span>供應商簡稱</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-12">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="textbox_dt2" CssClass="form-control" Text="" Width="100%" runat="server" placeholder="請輸入供應商簡稱"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="col-md-5 col-sm-10 col-xs-12" style="margin: 5px 0px 5px 0px">
                                                <span>品號</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-12">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="textbox_item" CssClass="form-control" Text="" Width="100%" runat="server" placeholder="請輸入品號關鍵字"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="col-md-5 col-sm-10 col-xs-12" style="margin: 5px 0px 5px 0px">
                                                <span>催料單號</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-12">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="textbox_BillNo" CssClass="form-control" Text="" Width="100%" runat="server" placeholder="請輸入催料單號"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="col-md-5 col-sm-3 col-xs-12" style="margin: 5px 0px 5px 0px">
                                                <span>催料預交日</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-12">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="txt_str" runat="server" Style="" TextMode="Date" Width="100%" CssClass="form-control   text-left"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12">
                                                                    <asp:TextBox ID="txt_end" runat="server" Style="" Width="100%" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div></div>
                                            <div class="col-md-9 col-xs-8">
                                            </div>
                                            <div class="col-md-3 col-xs-12" style="margin: 10px 0px 0px 0px">
                                                <asp:Button ID="button_select" runat="server" Text="搜尋" class="btn btn-primary" OnClick="button_select_Click" Style="display: none" />
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




        <!-----------------/content------------------>
    </div>
    <!-- set Modal -->
    <!--<div class="backdrop">
    </div>
    <div class="fab child" data-subitem="1" data-toggle="modal" data-target="#exampleModal">
        <span>
            <i class="fa fa-search"></i>
        </span>
    </div>
    <div class="fab" id="masterfab">
        <span>
            <i class="fa fa-list-ul"></i>
        </span>
    </div>
    <!--/set Modal-->
    <!-- Modal -->
    <div id="exampleModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 class="modal-title modaltextstyle" id="myModalLabel2"><i class="fa fa-file-text"></i>資料檢索精靈</h4>
                </div>
                <div class="modal-body">
                    <div id="testmodal2">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down"><b>供應商</b></i>
                            </h5>
                            <!--2019.07.08 autocomplete call webService(ru)-->
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">

                                    <%--<input type="text" id="txt_supplier" class="form-control" placeholder="請輸入供應商簡稱">--%>
                                    <input type="hidden" id="hidden2" runat="server" />
                                </div>
                            </div>
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down"><b>供應商簡稱</b></i>
                            </h5>
                            <!--2019.07.08 autocomplete call webService(ru)-->
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <%--<input type="text" id="txt_supplier" class="form-control" placeholder="請輸入供應商簡稱">--%>
                                    <input type="hidden" id="hidden1" runat="server" />
                                </div>
                            </div>
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down"><b>品號</b></i>
                            </h5>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                </div>
                            </div>
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down"><b>催料單號</b></i>
                            </h5>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down"><b>催料預交日</b> </i><i id="cbx_remind"></i>
                            </h5>
                            <fieldset>
                                <div class="control-group">
                                    <div class="controls">
                                        <div class="col-md-5 xdisplay_inputx form-group has-feedback">
                                            <%--      <input type="text" class="form-control has-feedback-left" id="d_start" name="txt_time_start" value="" runat="server" placeholder="開始日期" aria-describedby="inputSuccess2Status2" autocomplete="off">
                                            <span class="fa fa-calendar-o form-control-feedback left" aria-hidden="true"></span>
                                            <span id="inputSuccess2Status1" class="sr-only">(success)</span>--%>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                            <fieldset>
                                <div class="control-group">
                                    <div class="controls">
                                        <div class="col-md-5 xdisplay_inputx form-group has-feedback">
                                            <%--                                            <input type="text" class="form-control has-feedback-left " id="d_end" name="txt_time_end" value="" runat="server" placeholder="結束日期" aria-describedby="inputSuccess2Status2" autocomplete="off">
                                            <span class="fa fa-calendar-o form-control-feedback left" aria-hidden="true"></span>
                                            <span id="inputSuccess2Status2" class="sr-only">(success)</span>--%>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>

                </div>
            </div>
        </div>
    </div>

    <!-- /Modal -->
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
        <script src="../../assets/vendors/datatables.net-colReorder/dataTables.colReorder.min.js"></script>
    <script src="../../assets/vendors/jszip/dist/jszip.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/pdfmake.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <!-- 2019.07.05 autocomplete -->
    <script src="../../assets/vendors/jquery/dist/jquery-ui.min.js"></script>
    <script src="../../assets/vendors/time/loading.js"></script>
    <script>
        $("#txt_supplier").autocomplete({
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SupplierShortage.aspx/Search",
                    data: "{'FACTNM2':'" + document.getElementById('txt_supplier').value + "'}",
                    dataType: "json",
                    success: function (data) {
                        response(data.d);
                    },
                    error: function (result) {
                        alert("Error");
                    }
                });
            },
            select: function (event, ui) {
                var selectedvalue = ui.item.value;
                //alert(selectedvalue);
                document.getElementById('ContentPlaceHolder1_hidden1').value = selectedvalue;
            }
        });

        var remind = document.getElementById("cbx_remind");
        remind.style.color = "#FF3333";
        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });
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
    </script>
</asp:Content>
