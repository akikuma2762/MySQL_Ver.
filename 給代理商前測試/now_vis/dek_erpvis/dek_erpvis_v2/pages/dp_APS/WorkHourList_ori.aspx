<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="WorkHourList_ori.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.WorkHourList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>APSList |  <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>
    <div class="right_col" role="main">
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁 </a></u></li>
            <li><u><a href="../dp_APS/OrderList.aspx">報工清單</a></u></li>
            <li>報工列表</li>
        </ol>
        <br>
        <asp:TextBox ID="TextBox_textTemp" runat="server" Visible="true" Width="1000px" Style="display: none"></asp:TextBox>

        <!-----------------/title------------------>

        <!-----------------content------------------>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel " id="Div_Shadow">
                    <div id="WorkHourList"></div>
                    <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="dashboard_graph x_panel">
                                <div class="x_content">
                                    <div id="information"></div>

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
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4">
                                                <span>機台名稱</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:DropDownList ID="DropDownList_Resource" runat="server"></asp:DropDownList>
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

        </div>
    </div>


    <div id="exampleModal_information" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmodal33" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i><b></b></i>
                            </h5>
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="col-md-3 col-sm-12 col-xs-12">
                                        請填入數量：
                                    </div>
                                    <div class="col-md-9 col-sm-12 col-xs-12">
                                        <asp:TextBox ID="TextBox_Qty" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="clearfix"></div>
                                <br>
                                <div id="div_change" class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="col-md-3 col-sm-12 col-xs-12">
                                        報工機台：
                                    </div>
                                    <div class="col-md-9 col-sm-12 col-xs-12">
                                        <div id="choose_mach" class="btn-group" data-toggle="buttons">
                                        </div>
                                        <asp:TextBox ID="TextBox_mach" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2 ">送出</button>
                    <asp:Button runat="server" Text="提交" ID="Button_Add" OnClick="Button_Add_Click" CssClass="btn btn-primary" Style="display: none" />
                    <button id="btnchange" type="button" class="btn btn-primary antosubmit2 ">送出</button>
                    <asp:Button runat="server" Text="提交" ID="Button_Change" OnClick="Button_change_Click" CssClass="btn btn-primary" Style="display: none" />

                </div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //當按鈕按下的時候，先執行LOADING的JS事件，在進行後台的計算
        $("#btnsearch").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=Button_Search.ClientID %>').click();
        });

        $("#btnchange").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=Button_Change.ClientID %>').click();
        });
        //執行動作的時候
        function start(WorkHourID, Project, C_TaskName, TaskName) {
            $('#ContentPlaceHolder1_TextBox_textTemp').val('' + WorkHourID + ',' + Project + ',' + C_TaskName + ',' + TaskName + ',' + TaskName + '');
            document.getElementById('<%=Button_Add.ClientID %>').click();
        }
        //新增數量
        function Add(WorkHourID, Project, C_TaskName, TaskName) {
            $('#ContentPlaceHolder1_TextBox_textTemp').val('' + WorkHourID + ',' + Project + ',' + C_TaskName + ',' + TaskName + '');
            document.getElementById('div_change').style.display = 'none'
            document.getElementById('btnchange').style.display = 'none'
            document.getElementById('btncheck').style.display = 'initial';
        }
        //選擇機台並開始
        function Choose_Start(WorkHourID, Project, C_TaskName, TaskName, allmach, nowmach) {
            $('#ContentPlaceHolder1_TextBox_textTemp').val('' + WorkHourID + ',' + Project + ',' + C_TaskName + ',' + TaskName + '');
            set_machinelist('choose_mach', allmach, nowmach, '#ContentPlaceHolder1_TextBox_mach','machchange')
            document.getElementById('btncheck').style.display = 'none'
            document.getElementById('btnchange').style.display = 'initial'
            document.getElementById('div_change').style.display = 'initial';

        }
        $("#btncheck").click(function () {
            document.getElementById('<%=Button_Add.ClientID %>').click();
        });
        function machchange(machname) {
            $('#ContentPlaceHolder1_TextBox_mach').val('' + machname + '');
        }

        //產生表格的HTML碼
        create_tablehtmlcode('WorkHourList', '報工列表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        set_information('information','送料單號,<%=order_num %>,品名規格,<%=product_name %>,母件編號,<%=product_num %>')
    </script>
</asp:Content>
