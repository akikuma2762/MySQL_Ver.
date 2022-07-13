<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="permission_log.aspx.cs" Inherits="dek_erpvis_v2.pages.SYS_CONTROL.permission_log" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>權限修改記錄查詢 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/UntradedCustomer.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main">
        <br />
        <div class="clearfix"></div>
        <div class="col-md-12 col-sm-12 col-xs-12">
        </div>
        <div class="x_panel Div_Shadow">
            <div class="x_content">
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <!--表格畫面-->
                        <div class="col-md-12 col-sm-12 col-xs-12" id="table_information">
                            <div id="Enter_ReportView"></div>
                        </div>
                    </div>

                </div>
            </div>
            <%=Use_Javascript.Quote_Javascript() %>
            <script>
                //產生表格的HTML碼
                create_tablecode_noshdrow('Enter_ReportView', '權限修改記錄查詢', 'datatable-buttons', '<%=th%>', '<%=tr%>');
            </script>
</asp:Content>
