<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Orders_Details.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.Orders_Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=Get_cust %>訂單詳細表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/Orders_Details.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main" style="height: 930px;">
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
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=SLS">業務部</a></u></li>
            <li><u><a href="javascript:void()" onclick="history.go(-1)">訂單數量與金額統計</a></u></li>
        </ol>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <div id="order_details"></div>
        <!-----------------/content------------------>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生表格的HTML碼
        create_tablecode('order_details', '<%=cust_name%>詳細表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
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
