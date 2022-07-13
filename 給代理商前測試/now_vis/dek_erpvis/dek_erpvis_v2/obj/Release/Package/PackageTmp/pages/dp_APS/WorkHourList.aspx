<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="WorkHourList.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.WorkHourList1" %>

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
        <asp:TextBox ID="TextBox_status" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_project" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_TaskName" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_WorkHourID" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_RealRsrc" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_Task" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_LostStatus" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_NowStatus" runat="server" Style="display: none"></asp:TextBox>
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
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2 ">送出</button>
                    <asp:Button runat="server" Text="提交" ID="Button_Add" OnClick="Button_Add_Click" CssClass="btn btn-primary" Style="display: none" />
                    <button id="btnchange" type="button" class="btn btn-primary antosubmit2 " style="display: none">送出</button>
                    <asp:Button runat="server" Text="提交" ID="Button_Change" OnClick="Button_change_Click" CssClass="btn btn-primary" Style="display: none" />

                </div>
            </div>
        </div>
    </div>
    <asp:Button ID="Button_savedb" runat="server" Text="Button" OnClick="Button_savedb_Click" Style="display: none" />
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

        //依據狀態填入資料庫
        function save_db(id, status, task_name, mach_name, project, task, lost_status, now_status) {
            $('#ContentPlaceHolder1_TextBox_status').val('' + status + '');
            $('#ContentPlaceHolder1_TextBox_project').val('' + project + '');
            $('#ContentPlaceHolder1_TextBox_TaskName').val('' + task_name + '');
            $('#ContentPlaceHolder1_TextBox_WorkHourID').val('' + id + '');
            $('#ContentPlaceHolder1_TextBox_RealRsrc').val('' + mach_name + '');
            $('#ContentPlaceHolder1_TextBox_Task').val('' + task + '');
            $('#ContentPlaceHolder1_TextBox_LostStatus').val('' + lost_status + '');
            $('#ContentPlaceHolder1_TextBox_NowStatus').val('' + now_status + '');
            if (now_status == '入站' || now_status == '暫停' || now_status == '取消暫停') {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.getElementById('<%=Button_savedb.ClientID %>').click();
            }

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
