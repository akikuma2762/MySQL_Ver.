﻿<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_Cahrt_Error_old.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.Asm_Cahrt_Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>異常統計分析 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_Cahrt_Error.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
 @media screen and (min-width:768px) {
            .canvas_height {
                height: 450px;

            }
        }
    </style>
    <!-----------------content------------------>
    <div class="right_col" role="main">
        <!------------------TitleRef----------------------->
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>
        </ol>
        <ul id="myTab" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true">圖片模式</a></li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" id="profile-tab" role="tab" data-toggle="tab" aria-expanded="false">表格模式</a></li>
        </ul>
        <div id="myTabContent" class="tab-content">
            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">

                            <div id="Error_image"></div>

                            <div class="col-md-3 col-sm-12 col-xs-12 _select">
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="dashboard_graph x_panel">
                                        <div class="x_content">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="col-md-4 col-sm-3 col-xs-12">
                                                    <span>選擇產線</span>
                                                </div>
                                                <div class="col-md-8 col-sm-9 col-xs-12">
                                                    <asp:CheckBoxList ID="CheckBoxList_Line" runat="server" CssClass="table-striped" Font-Size="20px"></asp:CheckBoxList>
                                                </div>
                                            </div>
                                            <div class="col-md-12 col-sm-6 col-xs-12">
                                                <div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                    <span>顯示筆數</span>
                                                </div>
                                                <div class="col-md-5 col-sm-12 col-xs-4" style="margin: 0px 0px 5px 0px">

                                                    <asp:TextBox ID="txt_showCount" runat="server" Text="10" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                                </div>
                                                <div class="col-md-3 col-sm-12 col-xs-3" style="margin: 5px 0px 5px 0px;">
                                                    <span>
                                                        <asp:CheckBox ID="CheckBox_All" runat="server" Text="全部" onclick="checkstatus()" />
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="col-md-12 col-sm-6 col-xs-12">
                                                <div class="col-md-4 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                    <span>異常日期</span>
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
                                                <div class="col-md-9 col-xs-8">
                                                </div>
                                                <div class="col-md-3 col-xs-12">
                                                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2 ">執行搜索</button>
                                                    <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" Style="display: none" OnClick="button_select_Click" />
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
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div id="Error_Table"></div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript()%>
    <!-----------------------Chart-------------------------------------->
    <script>
        //產生Htmlcode

        create_imghtmlcode('Error_image', 'd1', 'Error_canvas','9', 'canvas_height');
        set_column('Error_canvas', '異常次數統計', '<%=time_area_text%>', '異常類型', '次數', '', [<%=ChartData_Count%>]);

        //產生表格的HTML碼
        create_tablecode('Error_Table', '異常統計列表', 'datatable-buttons', '<%=ColumnsData%>', '<%=RowsData%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        $('#ContentPlaceHolder1_CheckBoxList_Line input').change(function () {
            var check = $(this).context.checked;
            var val = $(this).val();
            if (val == "" && check == true) {
                seletedAllItem(true);
            } else if (val == "" && check == false) {
                seletedAllItem(false);
            } else if (val != "" && check == false) {
                if ($(this)[0].id.split("_")[3] != 0) {
                    var x = document.getElementById("ContentPlaceHolder1_CheckBoxList_Line_0");
                    x.checked = false;
                }
            }
        });
        function seletedAllItem(seleted) {
            $('#ContentPlaceHolder1_CheckBoxList_Line input').each(function () {
                $(this).context.checked = seleted;
            });
        }
        function seletedItem(num, seleted) {
            var x = document.getElementById("ContentPlaceHolder1_CheckBoxList_Line_" + num);
            x.checked = true;
        }
        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });
    </script>
</asp:Content>
