<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="stockanalysis_details.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.stockanalysis_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=cust_name %>成品庫存詳細表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_WH/stockanalysis_details.css" rel="stylesheet" />
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
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li><u><a href="../index.aspx">倉管部</a></u></li>
            <li><u><a href="javascript:void()" onclick="history.go(-1)">成品庫存分析</a></u></li>
        </ol>
        <br />
        <div class="clearfix"></div>
        <div id="stockanalysis_details"></div>
    </div>


    <div id="exampleModal_Image" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h1>
                        <i id="dlg_title"></i>
                    </h1>
                </div>
                <div class="modal-body">
                    <div id="testmodal25" style="text-align: center;">
                        <label id="lbltipAddedComment"></label>
                    </div>
                </div>
                <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>



    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生表格的HTML碼

        create_tablecode('stockanalysis_details', '<%=cust_name %>成品庫存詳細表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        function show_information(order, cust, date_str) {
            document.getElementById('lbltipAddedComment').innerHTML = '';
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dp_SD.asmx/Getorder",
                data: { order: order, cust: cust, date_str: date_str },
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            var code = $(this).attr("item_code").valueOf();
                            document.getElementById("lbltipAddedComment").innerHTML = code;
                            document.getElementById("dlg_title").innerHTML = order + ' 訂單規格 ';
                        }
                    });
                },
                error: function (data, errorThrown) {
                    alert("Fail");
                }
            });
        }
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
