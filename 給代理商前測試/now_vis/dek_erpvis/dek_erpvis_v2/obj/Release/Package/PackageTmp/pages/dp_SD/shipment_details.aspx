<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="shipment_details.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.shipment_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=cust_name %>出貨詳細表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/shipment_details.css" rel="stylesheet" />
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
    <div id="_Title" class="right_col" role="main" style="height: 930px;">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=SLS">業務部</a></u></li>
            <li><u><a href="javascript:void()" onclick="history.go(-1)">出貨統計表</a></u></li>
        </ol>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <div id="shipment_detail"></div>
        <!-----------------/content------------------>
        <!-- Modal -->
        <div id="exampleModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                        <h1>
                            <i id="dlg_title"></i>
                        </h1>
                    </div>
                    <div class="modal-body">
                        <table id="dataTables-modal" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                            <thead>
                                <tr style="background-color: white">
                                    <th>#</th>
                                    <th>出貨日期</th>
                                    <th>出貨單號</th>
                                    <th>製令號</th>
                                    <th>客戶料號</th>
                                    <th style="width: 20%;">備註</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <!-- /Modal -->
    </div>
    <!-- jQuery -->
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //function GetShipment_details(detail) {


        //    var next_list = detail.replaceAll('*', '\r\n').replaceAll('#', ' ').split('^');

        //    var nexttable = $('#dataTables-modal').dataTable();
        //    nexttable.fnClearTable();
        //    var next_data = [];
        //    if (next_list.length > 1) {
        //        for (i = 0; i < next_list.length - 1; i++)
        //            next_data.push('<center>' + next_list[i] + '</center>');
        //        nexttable.fnAddData(next_data);
        //    }
        //}
        function GetShipment_details(cust, code, time_str, time_end) {
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dp_SD.asmx/GetShipment_details",
                data: { cust_name: cust, item_code: code, date_str: time_str, date_end: time_end }, //MR4AK0004A10,20180101,20181231
                success: function (xml) {

                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            var code = $(this).attr("item_code").valueOf();
                            //var name = $(this).attr("item_name").valueOf(); 
                            var thisTable = $('#dataTables-modal').dataTable();
                            thisTable.fnClearTable();
                            $(this).children().each(function (j) {
                                addData = [];
                                addData.push($(this).attr("序列").valueOf());
                                addData.push($(this).attr("出貨日期").valueOf());
                                addData.push($(this).attr("出貨單號").valueOf());
                                addData.push($(this).attr("製令號").valueOf());
                                addData.push($(this).attr("客戶料號").valueOf());
                                addData.push($(this).attr("備註").valueOf());
                                thisTable.fnAddData(addData);
                            })
                            document.getElementById("dlg_title").innerHTML = code;
                        }
                    });
                },
                error: function (data, errorThrown) {
                    alert("Fail");
                }
            });
        }
        //產生表格的HTML碼
        create_tablecode('shipment_detail', '<%=cust_name%>出貨詳細表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');
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
