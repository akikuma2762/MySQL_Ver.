<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="supplierscore.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PD.supplier_score" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>供應商達交率 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <%=color %>
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_PD/supplierscore.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main">
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
        <%= path %><br>
        <!-----------------title------------------>
        <div class="page-title">
        </div>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="x_content">
                        <div id="supplierscore_table"></div>

                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="dashboard_graph x_panel">
                                <div class="x_content">
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                交貨日期
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-7">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="txt_str" runat="server" Style="" TextMode="Date" Width="98%" CssClass="form-control   text-left"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="txt_end" runat="server" Style="" TextMode="Date" Width="98%" CssClass="form-control   text-left"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <div class="col-md-9 col-xs-8">
                                        </div>
                                        <div class="col-md-3 col-xs-12">
                                            <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
                                            <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行搜索</button>
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
        //產生表格的HTML碼
        create_tablehtmlcode('supplierscore_table', '供應商達交率', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('supplierscore=supplierscore_cust', '#datatable-buttons');

        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });
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
