<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="APSList_Details.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.APSList_Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>APSList |  <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>
    <div class="right_col" role="main">
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li><u><a href="../dp_APS/APSList.aspx">排程列表</a></u></li>
            <li>排程列表明細</li>
        </ol>
        <br>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel" id="Div_Shadow">
                    <div id="APSList_Detail"></div>
                    <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                        <div id="light"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        show_light('light');
        //產生表格的HTML碼
        create_tablehtmlcode('APSList_Detail', '排程列表明細', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');
    </script>
</asp:Content>
