<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="transportrackstatistics.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_SD.transportrackstatistics" %>

<%--<%@ OutputCache duration="10" varybyparam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>運輸架未歸還統計 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
    <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_SD/transportrackstatistics.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="right_col" role="main">
        <!-----------------title------------------>
        <%=path %>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>

        <ul id="myTab" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true">圖片模式</a>
            </li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" id="profile-tab" role="tab" data-toggle="tab" aria-expanded="false">表格模式</a>
            </li>
        </ul>
        <div id="myTabContent" class="tab-content">
            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div id="Transportrackstatistics_Image"></div>
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div id="Transportrackstatistics_Table"></div>
            </div>
        </div>
        <!-----------------/content------------------>
    </div>
   <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生圖片的HTML碼
        create_imgcode('Transportrackstatistics_Image', 'download', 'Transportrackstatistics_canvas')
        //產生圖片的JS碼
        set_stackedColumn('Transportrackstatistics_canvas', '運輸架在外數量', '運輸架名稱', '數量', '正常數量', '<%=col_data_points_nor%>', '異常數量', '<%=col_data_points_sply%>');

        //產生表格的HTML碼
        create_tablecode('Transportrackstatistics_Table', '運輸架未歸還統計', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

    </script>
</asp:Content>
