<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="WorkHourDetail.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.WorkHourDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>APSList |  <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>
    <div class="right_col" role="main">
        <%=Super_Link %>
        <br>
        <asp:TextBox ID="TextBox_textTemp" runat="server" Visible="true" Width="0" Style="display: none"></asp:TextBox>
        <asp:Button ID="button_delete" runat="server" Text="搜尋" class="btn btn-secondary" OnClick="button_delete_Click" Style="display: none" />
        <!-----------------/title------------------>

        <!-----------------content------------------>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel" id="Div_Shadow">
                    <div id="WorkHourDetail"></div>
                    <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="dashboard_graph x_panel">
                                <div class="x_content">
                                    <div id="information"></div>
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        function get_id(id) {
            var r = confirm("確定要刪除該筆資料嗎");
            if (r == true) {
                $('#ContentPlaceHolder1_TextBox_textTemp').val('' + id + '');
                document.getElementById('<%=button_delete.ClientID %>').click();
            }
        }
        set_information('information','群組編號,<%=G_Order %>,送料單號,<%=O_Order %>,品名規格,<%=N_Order %>,工藝名稱,<%=T_Order %>')
        create_tablehtmlcode('WorkHourDetail', '報工列表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');
    </script>
</asp:Content>
