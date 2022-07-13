<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_history.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.Asm_history" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>維護歷史搜尋 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
    <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_history.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>
        </ol>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div id="Asm_history"></div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="x_panel">
                                <div class="x_content">
                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                        <div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">
                                            <span>產線名稱</span>
                                        </div>
                                        <div class="col-md-8 col-sm-12 col-xs-7" style="margin: 0px 0px 5px 0px">
                                            <asp:DropDownList ID="DropDownList_Line" runat="server" class="form-control">
                                            </asp:DropDownList>
                                            <asp:TextBox ID="TextBox_Line" runat="server" style="display:none"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                        <div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">
                                            <span>排程編號</span>
                                        </div>
                                        <div class="col-md-8 col-sm-12 col-xs-7" style="margin: 0px 0px 5px 0px">
                                            <input id="Mant_Search" runat="server" type="text" name="Mant_Search" class="form-control" placeholder="EX:C4AQV-180000" onkeydown="Unnamed_ServerClick">
                                        </div>
                                    </div>

                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                        <div class="col-md-4 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                            <span>日期選擇</span>
                                        </div>
                                        <div class="col-md-8 col-sm-9 col-xs-7" style="margin: 0px 0px 5px 0px">
                                            <asp:TextBox ID="textbox_dt1" runat="server" Style="" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-12 col-sm-6 col-xs-12">
                                        <div class="col-md-4 col-sm-3 col-xs-5">
                                        </div>
                                        <div class="col-md-8 col-sm-9 col-xs-7" style="margin: 0px 0px 5px 0px">
                                            <asp:TextBox ID="textbox_dt2" runat="server" CssClass="form-control  text-left" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <div class="col-md-6 col-xs-8">
                                        </div>


                                        <div class="col-md-3 col-xs-12">
                                            <button id="btnprint" type="button" class="btn btn-success">列印問題</button>
                                            <asp:Button ID="Button_Print" runat="server" Text="列印問題" style="display:none" class="btn btn-success" OnClick="Button_Print_Click" Visible="false" />
                                        </div>

                                        <div class="col-md-3 col-xs-12">
                                            <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="Unnamed_ServerClick" Style="display: none" />
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
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        $(document).ready(function () {
            var tharray = [];
            $('#tr_row > th').each(function () {
                tharray.push($(this).text())
            })
            if (tharray[0] == '沒有資料載入')
                document.getElementById('btnprint').style.display = 'none';
            else
                document.getElementById('btnprint').style.display = 'initial';

        });
        $("#btncheck").click(function () {
            document.getElementById('<%=TextBox_Line.ClientID%>').value = document.getElementById('<%=TextBox_Line.ClientID%>').value;
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });

        $("#btnprint").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=Button_Print.ClientID %>').click();
            $.unblockUI();
         });
        //產生表格的HTML碼
        create_tablehtmlcode('Asm_history', '組裝歷史查詢', 'datatable-buttons', '<%=ColumnsData%>', '<%=RowsData%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');
    </script>
</asp:Content>
