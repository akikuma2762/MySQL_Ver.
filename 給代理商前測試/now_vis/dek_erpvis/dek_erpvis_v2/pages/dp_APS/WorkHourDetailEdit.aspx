<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="WorkHourDetailEdit.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_APS.WorkHourDetailEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>APSList |  <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-----------------title------------------>
    <div class="right_col" role="main">
        <%=Super_Link %>
        <br>
        <div class="">
        </div>

        <!-----------------/title------------------>

        <!-----------------content------------------>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel" id="Div_Shadow">
                    <div class="col-md-12 col-sm-12 col-xs-12 _select _setborder">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="x_title">
                                <h1 class="text-center _mdTitle" style="width: 100%"><b><%=pagename %></b></h1>
                            </div>
                            <div class="x_content">
                                <div class="col-md-6 col-sm-12 col-xs-12">
                                    <div class="dashboard_graph x_panel">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                品名規格：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span><%=P_Order %></span>
                                            </div>
                                            <br>
                                            <hr />
                                        </div>

                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                送料單號：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span><%=O_Order %></span>
                                            </div>
                                            <br>
                                            <hr />
                                        </div>

                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                工藝名稱：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span><%=T_Order %></span>
                                            </div>
                                            <br>
                                            <hr />
                                        </div>

                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                目前數量：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span><%=CurrentPiece %></span>
                                            </div>
                                            <br>
                                            <hr />
                                        </div>

                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                需求數量：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span><%=TargetPiece %></span>
                                            </div>
                                            <br>
                                            <hr />
                                        </div>

                                    </div>
                                </div>

                                <div class="col-md-6 col-sm-12 col-xs-12">
                                    <div class="dashboard_graph x_panel">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                報工人員：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span>
                                                    <asp:TextBox ID="TextBox_Man" runat="server" Width="180px"></asp:TextBox></span>
                                            </div>
                                        </div>
                                        <br>
                                        <hr />
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                報工狀態：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <asp:TextBox ID="TextBox_status" runat="server" Style="display: none"></asp:TextBox>
                                                <div class="btn-group" data-toggle="buttons">
                                                    <%=status_button %>
                                                </div>
                                            </div>
                                        </div>
                                        <br>
                                        <hr />
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                報工日期：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span>
                                                    <asp:TextBox ID="TextBox_Date" runat="server" TextMode="Date" Width="180px"></asp:TextBox></span>
                                            </div>
                                        </div>
                                        <br>
                                        <hr />
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                報工時間：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span>
                                                    <asp:TextBox ID="TextBox_Time" runat="server" TextMode="Time" Width="180px"></asp:TextBox></span>
                                            </div>
                                        </div>
                                        <br>
                                        <hr />
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-12 col-xs-12">
                                                報工數量：
                                            </div>
                                            <div class="col-md-7 col-sm-12 col-xs-12">
                                                <span>
                                                    <asp:TextBox ID="TextBox_Count" runat="server" TextMode="Number" Text="0" Width="180px"></asp:TextBox></span>
                                            </div>
                                        </div>
                                        <br>
                                        <hr />
                                        <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: right;">
                                            <span>
                                                <asp:Button ID="Button_Save" runat="server" class="btn btn-success" Text="儲存" OnClick="Button_Save_Click" />
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
    <!-----------------/content------------------>
<%=Use_Javascript.Quote_Javascript() %>
    <script>
        function status(now_status) {
            $('#ContentPlaceHolder1_TextBox_status').val('' + now_status + '');
        }
    </script>

</asp:Content>

