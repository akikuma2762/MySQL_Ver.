<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Manage_login.aspx.cs" Inherits="dek_erpvis_v2.pages.SYS_CONTROL.Manage_login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>登入人員管控 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/UntradedCustomer.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main">
        <br />
        <asp:TextBox ID="TextBox_ID" runat="server" Style="display: none"></asp:TextBox>
        <asp:Button ID="Button_Delete" runat="server" Text="Button" OnClick="Button_Delete_Click" Style="display: none" />
        <div id="Manage_Login"></div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生表格的HTML碼
        create_tablecode('Manage_Login', '登入人員管控', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        function set_deleteid(id) {
            answer = confirm("您確定要剔除他嗎??");
            if (answer) {
                $('#ContentPlaceHolder1_TextBox_ID').val('' + id + '');
                document.getElementById('<%=Button_Delete.ClientID %>').click();
            }
        }
    </script>
</asp:Content>
