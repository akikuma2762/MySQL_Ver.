<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Worktime_Total.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.Worktime_Total" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>人員工時統計表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/Orders_Details.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main" style="height: 930px;">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=APS">自動排程</a></u></li>
            <li>人員工時統計表</li>
        </ol>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <div id="work_total"></div>
        <!-----------------/content------------------>
    </div>
      <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生表格的HTML碼
        create_tablecode('work_total', '人員工時統計表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');
    </script>
</asp:Content>