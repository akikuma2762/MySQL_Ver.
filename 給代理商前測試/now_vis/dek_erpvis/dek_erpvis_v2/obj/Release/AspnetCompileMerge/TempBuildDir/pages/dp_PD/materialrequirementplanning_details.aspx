<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="materialrequirementplanning_details.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PD.materialrequirementplanning_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=title_text %>物料領用統計表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_PD/materialrequirementplanning_details.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
    <style>
        #chartpie {
            height: 400px;
            max-width: 920px;
            margin: 0px auto;
        }

        @media screen and (max-width:768px) {
            #chartpie {
                height: 400px;
                width: 100%;
                margin: 0px auto 30px;
            }
        }

        @media screen and (min-width:768px) {
            .pie_height {
                height: 450px;
            }
        }
    </style>
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
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PCD">資材部</a></u></li>
            <li><u><a href="javascript:void()" onclick="history.go(-1)">物料領用統計表</a></u></li>
        </ol>
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
                        <div id="div_pie"></div>
                        <div id="div_column"></div>
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div id="materialrequirementplanning_details"></div>
            </div>
        </div>

        <!-----------------/content------------------>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生IMAGE的HTMLCODE
        create_imghtmlcode('div_pie', 'd1', 'chart_pie', '5', 'pie_height');
        create_imghtmlcode('div_column', 'd2', 'chart_column', '7', 'pie_height');

        //產生圓餅圖的CODE
        set_pie('chart_pie', '<%=title_text%>領用記錄', '<%=start%>~<%=end%>', [<%=pie_data_points%>]);

        //產生多色直方圖
        set_manystackColumn('chart_column', '<%=title_text%>領用記錄', '<%=start%>~<%=end%>', [<%=col_data_points%>]);

        //產生表格的HTML碼
        create_tablecode('materialrequirementplanning_details', '<%=title_text %>領用紀錄', 'datatable-buttons', '<%=th%>', '<%=tr%>');
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
