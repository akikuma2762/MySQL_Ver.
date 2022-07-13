<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="waitingfortheproduction.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.waitingfortheproduction_Ver2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>生產推移圖 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
<link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    <%--<link href="../../Content/Default_input.css" rel="stylesheet" />--%>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/waitingfortheproduction_Ver2.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        /*手機*/
        @media screen and (max-width: 765px) {
            .Canvas_height {
                height: 400px;
                max-width: 95%;
                margin: 0 auto;
                padding-top: 10px;
            }

            .Div_Shadow {
                height: 680px;
                box-shadow: 3px 3px 9px gray;
            }
        }
        /*電腦*/
        @media screen and (min-width: 765px) {
            .Canvas_height {
                height: 550px;
                max-width: 100%;
            }

            .Div_Shadow {
                height: 1000px;
                box-shadow: 3px 3px 9px gray;
            }
        }

        .zpanel {
            height: 680px;
        }

        .div_heigth {
            height: 650px;
        }

        .dashboard_graph {
            height: 650px;
        }

    </style>
    <!--局部刷新-->
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="True">
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
        <!-----------------/title------------------>
        <div class="page-title">
            <div class="row">
            </div>
        </div>
        <div class="clearfix"></div>
        <!-----------------content------------------>
        <!--全部-->
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
                        <div class="col-md-9 col-sm-12 col-xs-12">
                            <div class="col-md-12 col-sm-12 col-xs-12" id="hidepercent">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div style="text-align: right; width: 100%; padding: 0;">
                                            <button style="display: none" type="button" id="exportChart" title="另存成圖片">
                                                <img src="../../assets/images/download.jpg" style="width: 36.39px; height: 36.39px;">
                                            </button>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    <div id="chart_bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-12 col-sm-12 col-xs-12" id="hidediv" style="display: none">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
                                        <div style="text-align: right; width: 100%; padding: 0;">
                                            <button style="display: none" type="button" id="exportimage" title="另存成圖片">
                                                <img src="../../assets/images/download.jpg" style="width: 36.39px; height: 36.39px;">
                                            </button>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">

                                                <div class="col-md-12 col-sm-12 col-xs-10">
                                                    <div id="chartContainer" style="height: 500px; max-width: 100%;"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>


                        </div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="x_panel">
                                <table id="TB" class="table table-ts table-bordered nowrap" cellspacing="0" width="100%">
                                    <thead style="display: none">
                                        <tr id="tr_row">
                                            <th>狀態</th>
                                            <th>時間</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>本期計畫生產 ：</b>
                                            </td>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>
                                                    <span style="display: inline-block; width: 45%; text-align: right;"><%=detail[2] %></span>
                                                    <span style="display: inline-block; width: 50%; text-align: right;">(100%)</span>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>應有進度： </b>
                                            </td>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>
                                                    <span style="display: inline-block; width: 45%; text-align: right;"><%=detail[1] %></span>
                                                    <span style="display: inline-block; width: 50%; text-align: right;">(<%=detail[0] %>)</span>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>實際進度： </b>
                                            </td>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>
                                                    <span style="display: inline-block; width: 45%; text-align: right;"><%=detail[4] %></span>
                                                    <span style="display: inline-block; width: 50%; text-align: right;">(<%=detail[3] %>)</span>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>差異： </b>
                                            </td>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>
                                                    <span style="display: inline-block; width: 45%; text-align: right;"><%=Int16.Parse(detail[4])-Int16.Parse(detail[1]) %></span>
                                                    <span style="display: inline-block; width: 50%; text-align: right;">(<%=Double.Parse(detail[3].Replace("%",""))-Double.Parse(detail[0].Replace("%","")) %>%)</span>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>本期尚未生產： </b>
                                            </td>
                                            <td align="center" style="font-size: 20px; color: black">
                                                <b>
                                                    <span style="display: inline-block; width: 45%; text-align: right;"><%=Int16.Parse(detail[2])-Int16.Parse(detail[4]) %></span>
                                                    <span style="display: inline-block; width: 50%; text-align: right;">(<%=100-Double.Parse(detail[3].Replace("%","")) %>%)</span>
                                                </b>
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="x_panel">

                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
                                    <ContentTemplate>
                                        <div class="col-md-12 col-sm-6 col-xs-12" style="display: none">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>圖片選擇</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8">
                                                <asp:DropDownList ID="dropdownlist_type" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">
                                                    <asp:ListItem>生產推移圖</asp:ListItem>
                                                    <asp:ListItem>生產領料圖</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>選擇產線</span>
                                            </div>
                                            <div class="col-md-8 col-sm-12 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:CheckBoxList ID="CheckBoxList_Line" RepeatColumns="2" runat="server"></asp:CheckBoxList>
                                            </div>
                                        </div>

                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>日期快選</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <div class="btn-group btn-group-justified">
                                                    <asp:LinkButton ID="LinkButton_month" runat="server" CssClass="btn btn-default " OnClick="button_select_Click" Style="text-align: left">當月</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>日期選擇</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:TextBox ID="txt_str" runat="server" Style="" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4">
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:TextBox ID="txt_end" runat="server" CssClass="form-control  text-left" TextMode="Date"></asp:TextBox>
                                            </div>
                                        </div>

                                    </ContentTemplate>
                                </asp:UpdatePanel>


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
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="x_panel">
                                <div class="x_title">
                                    <h1 class="text-center _mdTitle" style="width: 100%"><b>未結案列表</b></h1>
                                    <h3 class="text-center _xsTitle" style="width: 100%"><b>未結案列表</b></h3>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="x_content">
                                    <p class="text-muted font-13 m-b-30">
                                    </p>
                                    <table id="datatable-buttons" class="table table-ts table-bordered nowrap" cellspacing="0" width="100%">
                                        <thead>
                                            <tr id="tr_row">
                                                <%=table_context[0].ToString()%>
                                            </tr>
                                        </thead>
                                        <tbody>

                                            <%=table_context[1].ToString() %>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
            <!-----------------/content------------------>
        </div>

        <%=Use_Javascript.Quote_Javascript() %>
        <script>
          <%=Js_Image%>       
            $(document).ready(function () {
          <%=Js_Table%>
                jQuery('.dataTable').wrap('<div class="dataTables_scroll" />');
            });


            $("#btncheck").click(function () {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.getElementById('<%=button_select.ClientID %>').click();
            });

            $(document).ready(function () {
                $('#example').DataTable({
                    responsive: true
                });
                $('#exampleInTab').DataTable({
                    responsive: true
                });

                $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                    $($.fn.dataTable.tables(true)).DataTable()
                        .columns.adjust()
                        .responsive.recalc();
                });
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
