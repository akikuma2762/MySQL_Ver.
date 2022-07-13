<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="InactiveInventory.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_WH.InactiveInventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>呆滯物料統計表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_WH/InactiveInventory.css" rel="stylesheet" />
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
        <%=path %>
        <br>
        <div class="clearfix"></div>
        <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="x_content">
                        <div id="InactiveInventory"></div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">


                            <div class="x_panel" runat="server" id="div_present" visible="false">
                                <table id="TB" class="table table-ts table-bordered nowrap" cellspacing="0" width="100%">
                                    <thead>
                                        <tr id="tr_row">
                                            <th class="th_centet">使用率</th>
                                            <th class="th_centet">呆滯金額</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>

                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>0%~25% ：</b>
                                            </td>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>
                                                    <%=cost[0] %>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>26%~50% ：</b>
                                            </td>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>
                                                    <%=cost[1] %>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>51%~75% ：</b>
                                            </td>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>
                                                    <%=cost[2] %>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>75%~100% ：</b>
                                            </td>
                                            <td align="center" style='font-size: 20px; color: black'>
                                                <b>
                                                    <%=cost[3] %>
                                                </b>
                                            </td>
                                        </tr>



                                    </tbody>
                                </table>
                            </div>


                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>計算天數</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:TextBox ID="TextBoxdayval" class="form-control" runat="server" TextMode="Number" Text="180"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>物料類別</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:CheckBoxList ID="CheckBoxList_itemtype" Font-Names="NotoSans" runat="server" CssClass="table-striped">
                                                </asp:CheckBoxList>
                                                <asp:DropDownList ID="DropDownList_itemtype" Width="100%" Style="display: none" Font-Names="NotoSans" runat="server" CssClass="btn btn-default dropdown-toggle">
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-md-12 col-sm-6 col-xs-12" style="display: none">
                                            <div class="col-md-4 col-sm-3 col-xs-12" style="margin: 5px 0px 5px 0px">
                                                <span>存放儲位</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                <%-- <asp:CheckBoxList style="display:none" ID="CheckBoxList_spaces" Font-Names="NotoSans" runat="server" CssClass="table-striped">
                                                </asp:CheckBoxList>--%>
                                                <asp:DropDownList ID="DropDownList_spaces" Width="100%" Font-Names="NotoSans" runat="server" CssClass="btn btn-default dropdown-toggle"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <div class="col-md-9 col-xs-8">
                                        </div>
                                        <div class="col-md-3 col-xs-12" style="margin-left: -3px">
                                            <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
                                            <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行運算</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-----------------/content------------------>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>

    <script>
        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });
        //產生表格的HTML碼
        create_tablehtmlcode('InactiveInventory', '呆滯物料統計表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
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
