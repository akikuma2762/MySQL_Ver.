<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Color_Change.aspx.cs" Inherits="dek_erpvis_v2.pages.SYS_CONTROL.Color_Change" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>顏色設定 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <link href="../../assets/build/css/custom_person.css" rel="stylesheet">
    <link href="../../assets/build/css/Change_Table_Button_person.css" rel="stylesheet">
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
    <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    <style>
        #colortest {
            background: var(--Color_Label);
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>
    <div class="right_col" role="main">
        <div class="">
            <div class="page-title">
                <div class="title_left">
                </div>
            </div>
        </div>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <!--跑版加入這個即可<div class="row"></div>-->
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="dashboard_graph x_panel" id="Div_Shadow">
                    <div class="x_content">
                        <div class="row">

                            <!--變更顏色的數值-->
                            <div class="controls">
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>側欄顏色</h2>
                                    <input type="color" id="Color_Sidebar" name="Color_Sidebar" list="colors" value="#000000">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>側欄文字顏色</h2>
                                    <input type="color" id="Color_SidebarText" name="Color_SidebarText" list="colors" value="#ffffff">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>標籤顏色</h2>
                                    <input type="color" id="Color_Label" name="Color_Label" list="colors" value="#000000">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>背景顏色</h2>
                                    <input type="color" id="Color_Background" name="Color_Background" list="colors" value="#000000">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>欄位顏色</h2>
                                    <input type="color" id="Color_Column" name="Color_Column" list="colors" value="#000000">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>欄位文字顏色</h2>
                                    <input type="color" id="Color_ColumnText" name="Color_ColumnText" list="colors" value="#ffffff">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>表格顏色</h2>
                                    <input type="color" id="Color_DataTable" name="Color_DataTable" list="colors" value="#000000">
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <h2>表格文字顏色</h2>
                                    <input type="color" id="Color_DataTableText" name="Color_DataTableText" list="colors" value="#ffffff">
                                </div>
                            </div>
                            <div class="col-md-6 col-sm-6 col-xs-6">
                                <h2>表格測試</h2>
                                <table class="table-bordered nowrap table-ts" id="tr_row " width="80%">
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
                            <br>

                            <div class="clearfix"></div>
                            <div class="col-md-6 col-sm-6 col-xs-6">
                                <input type="button" value="儲存" onclick="Saveclick()" class="btn btn-primary antosubmit2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <input type="button" value="返回參數設定畫面" onclick="backpage()" class="btn btn-danger">
                                <asp:Button ID="button_select" runat="server" Text="保存" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
                            </div>


                        </div>
                    </div>
                </div>
                <!-----------------/content------------------>
            </div>
        </div>
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
    <script src="../../assets/vendors/jszip/dist/jszip.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/pdfmake.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <script>
        //存入cookies的變數名稱
        var colorname = ["Sidebar_Color=", "SidebarText_Color=",
            "Label_Color=", "Background_Color=",
            "Column_Color=", "ColumnText_Color=",
            "DataTable_Color=", "DataTableText_Color="
        ];

                //連接伺服器
                  <%=give_js%>

        //---------------------顏色調整相關JS---------------------
        // 選擇網頁上的元素
        const inputs = document.querySelectorAll('.controls input')

        // 當使用者拉動更新元素時設定 CSS 變數值
        function updateProperty() {
            const suffix = this.dataset.sizing || '';
            document.documentElement.style.setProperty(`--${this.name}`, this.value + suffix);
        }

        // 監控每一個 input 元素
        inputs.forEach(input => {
            input.addEventListener('input', updateProperty)
        })

        //---------------------顏色調整相關JS---------------------

        //---------------------把cookies或是DataTable的資料回填至Text內部---------------------

        var colors = ["Color_Sidebar", "Color_SidebarText",
            "Color_Label", "Color_Background",
            "Color_Column", "Color_ColumnText",
            "Color_DataTable", "Color_DataTableText"];
        if (colorvalue.length == 8) {
            for (var i = 0; i < colors.length; i++) {
                document.getElementById(colors[i]).value = colorvalue[i];
            }
        }
        else {
            for (var i = 0; i < colors.length; i++) {
                if (i == 1 || i == 5 || i == 7)
                    document.getElementById(colors[i]).value = "#ffffff";
                else
                    document.getElementById(colors[i]).value = "#000000";
            }
        }
        //---------------------把cookies或是DataTable的資料回填至Text內部---------------------

        //將數值存入cookies
        function Saveclick() {
            //背景顏色
            var ColorSidebar = document.getElementById("Color_Sidebar").value;
            //側欄顏色
            var ColorSidebarText = document.getElementById("Color_SidebarText").value;
            //側欄文字顏色
            var ColorLabel = document.getElementById("Color_Label").value;
            //標籤
            var ColorBackground = document.getElementById("Color_Background").value;
            //欄位
            var ColorColumn = document.getElementById("Color_Column").value;
            //欄位文字
            var ColorColumnText = document.getElementById("Color_ColumnText").value;
            //表格
            var ColorDataTable = document.getElementById("Color_DataTable").value;
            //表格文字
            var ColorDataTableText = document.getElementById("Color_DataTableText").value;
            //用陣列去跑迴圈比較簡潔
            var colornum = [ColorSidebar, ColorSidebarText,
                ColorLabel, ColorBackground,
                ColorColumn, ColorColumnText,
                ColorDataTable, ColorDataTableText
            ];

            for (var i = 0; i < colorname.length; i++) {
                //先清除後寫入
                document.cookie = colorname[i] + "; expires=Thu, 01 Jan 1970 00:00:00 GMT";
                document.cookie = colorname[i] + colornum[i] + ";path=/";
            }
            alert('儲存成功!'); location.href = 'set_parameter.aspx';
        }
        function backpage() {
            document.getElementById('<%=button_select.ClientID %>').click();
        }
    </script>
</asp:Content>
