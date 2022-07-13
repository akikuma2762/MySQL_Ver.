<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Picking_List.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PD.Picking_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=order%> 領料明細 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>

    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_history.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .row_update {
            margin-left: -3.8px;
        }
    </style>
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
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PCD">資材部</a></u></li>
        </ol>
        <!-----------------/title------------------>
        <!-----------------content------------------>
        <div class="row row_update">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div id="Picking_List"></div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="x_panel">
                                <div class="x_content">
                                    <div class="col-md-12 col-sm-6 col-xs-12" style="margin-top: 5px">
                                        <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                            <span>製令號*</span>
                                        </div>
                                        <div class="col-md-8 col-sm-9 col-xs-8">
                                            <asp:TextBox ID="TextBox_Order" runat="server" CssClass="form-control   text-center"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 col-sm-6 col-xs-12" style="margin-top: 5px">
                                        <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                            <span>料號</span>
                                        </div>
                                        <div class="col-md-8 col-sm-9 col-xs-8">
                                            <asp:TextBox ID="TextBox_Item" runat="server" CssClass="form-control   text-center"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 col-sm-6 col-xs-12" style="margin-top: 5px">
                                        <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                            <span>品名</span>
                                        </div>
                                        <div class="col-md-8 col-sm-9 col-xs-8">
                                            <asp:TextBox ID="TextBox_ItemName" runat="server" CssClass="form-control   text-center"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-12 col-sm-12 col-xs-12" style="margin-top: 5px">
                                        <div class="col-md-9 col-xs-8">
                                        </div>
                                        <div class="col-md-3 col-xs-12">
                                            <button id="btncheck" type="button" class="btn btn-primary antosubmit2 ">執行搜索</button>
                                            <asp:Button runat="server" Text="提交" ID="Button_select" CssClass="btn btn-primary" Style="display: none" OnClick="Button_submit_Click" />
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

    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //產生表格的HTML碼
        create_tablehtmlcode('Picking_List', '<%=order%> 領料明細', 'datatable-buttons', '<%=th.ToString() %>', '<%=tr.ToString()%>');
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
        $("#btncheck").click(function () {
            if (document.getElementById('<%=TextBox_Order.ClientID %>').value != '') {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.getElementById('<%=Button_select.ClientID %>').click();
            }
            else
                alert('請填寫製令號');
        });
    </script>
</asp:Content>
