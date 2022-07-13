<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="APSList_old.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.APSList_old" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>APSList |  <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/dp_APS/APSList_old.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>
    <div class="right_col" role="main">
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li>排程列表</li>
        </ol>
        <br>
        <div class="x_panel" id="Div_Shadow">
            <div class="col-md-9 col-sm-12 col-xs-12">
                <div class="x_content">
                    <div class="x_panel">
                        <div id="_FormTitle" class="x_title" style="text-align: center">
                            <h1 class="text-center _mdTitle" style="width: 100%"><b>報工清單</b></h1>
                            <h3 class="text-center _xsTitle" style="width: 100%"><b>報工清單</b></h3>
                            <div class="clearfix"></div>
                        </div>
                        <ul id="myTab" class="nav nav-tabs" role="tablist">
                            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true">詳細資訊</a>
                            </li>
                            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" role="tab" id="profile-tab" data-toggle="tab" aria-expanded="false">簡短資訊</a>
                            </li>
                        </ul>
                        <div id="myTabContent" class="tab-content">
                            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">
                                <div id="Detail"></div>
                            </div>
                            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                                <div id="short"></div>
                            </div>
                        </div>
                    </div>
                    <!--部分資訊-->
                </div>
            </div>
            <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="dashboard_graph x_panel">
                        <div class="x_content">
                            <div id="light"></div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                        <span>開始時間</span>
                                    </div>
                                    <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                        <asp:TextBox ID="TextBox_Start" runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-12 col-sm-6 col-xs-12">
                                    <div class="col-md-4 col-sm-3 col-xs-4">
                                        <span>結束時間</span>
                                    </div>
                                    <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                        <asp:TextBox ID="TextBox_End" runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />

                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="col-md-9 col-xs-8">
                            </div>
                            <div class="col-md-3 col-xs-12">
                                <button id="btnsearch" type="button" class="btn btn-primary antosubmit2 ">搜尋</button>
                                <asp:Button ID="Button_Search" runat="server" class="btn btn-primary antosubmit2" Text="搜尋" OnClick="Button_Search_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

    </div>
    <!-----------------/content------------------>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //當按鈕按下的時候，先執行LOADING的JS事件，在進行後台的計算
        $("#btnsearch").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=Button_Search.ClientID %>').click();
        });
        //產生各顏色燈號的意思
        show_light('light');

        //產生表格的HTML碼
        create_tablecode('Detail', '未結案列表', 'dt_Detail', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#dt_Detail');
        //防止頁籤跑版
        loadpage('', '');

        //產生表格的HTML碼
        create_tablecode('short', '已結案列表', 'dt_short', '<%=title%>', '<%=content%>');
        //產生相對應的JScode
        set_Table('#dt_short');
        //防止頁籤跑版
        loadpage('', '');
        //============================================================================================
        function jump(Project) {
            document.location.href = "APSList_Details.aspx?key=" + Project;
        }

    </script>
</asp:Content>
