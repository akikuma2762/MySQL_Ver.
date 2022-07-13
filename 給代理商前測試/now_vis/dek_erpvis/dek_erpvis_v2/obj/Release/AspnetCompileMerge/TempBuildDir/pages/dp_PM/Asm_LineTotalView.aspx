<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_LineTotalView.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PD.Asm_LineTotalView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>整廠進度管理看板 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_LineTotalView.css" rel="stylesheet" />

    <style>
        .breadcrumb_ {
            padding: 0px 0px;
            margin-bottom: 20px;
            list-style: none;
            background-color: #f5f5f5;
            border-radius: 4px;
        }

            .breadcrumb_ > li {
                display: inline-block;
            }

                .breadcrumb_ > li + li:before {
                    padding: 0 5px;
                    color: #ccc;
                    content: "/\00a0";
                }

            .breadcrumb_ > .active {
                color: #777;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main"  >
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>
        </ol>
        <div class="col-md-12 col-sm-12 col-xs-12">
            <div class="row">
                <div class="col-md-10 col-sm-10 col-xs-12 text-center">
                    <h1 class="text-center _mdTitle" style="width: 100%; margin-bottom: 15px"><b>整廠進度管理看板</b></h1>
                    <h3 class="text-center _xsTitle"><b>整廠進度管理看板</b></h3>
                </div>
            </div>
        </div>
        <!-----------------content------------------>
     
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="x_panel Div_Shadow">
                        <div class="x_content">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="x_panel">
                                    <div class="row tile_count" style="margin-top: 0px; margin-bottom: -20px; padding: 0 10px">
                                        <div class="col-md-offset-4 col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                            <span class="count_top"><i class="fa fa-clock-o"></i>今日在線</span>
                                            <div class="count blue"><%=TotalOnLinePiece %><span style="height: 10px"><%=PieceUnit%></span></div>
                                        </div>
                                        <div class="col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                            <span class="count_top"><i class="fa fa-clock-o"></i>今日完成</span>
                                            <div class="count green"><% =TotalFinishPiece%><span style="height: 10px"><%=PieceUnit%></span></div>
                                        </div>
                                        <div class="col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                            <span class="count_top"><i class="fa fa-clock-o"></i>今日異常</span>
                                            <div class="count red"><%=no_solved %><span style="height: 10px"><%=PieceUnit%></span></div>
                                        </div>
                                        <div class="col-md-1 col-sm-3 col-xs-6 tile_stats_count">
                                            <span class="count_top"><i class="fa fa-clock-o"></i>當下落後</span>
                                            <div class="count black"><%=behind %><span style="height: 10px"><%=PieceUnit%></span></div>
                                        </div>
                                    </div>
                                    <hr />
                                    <table id="TB" class="table table-bordered" cellspacing="0" width="100%">
                                        <tr id="tr_row">
                                            <%=ColumnsData%>
                                        </tr>
                                        <tbody>
                                            <%=RowsData%>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    <div class="fab" id="masterfab" data-subitem="1" data-toggle="modal" data-target="#exampleModal" style="display: none">
        <span>
            <i class="fa fa-search"></i>
        </span>
    </div>
    <!--/set Modal-->
    <!-- Modal1 -->
    <div id="exampleModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 class="modal-title modaltextstyle" id="myModalLabel2"><i class="fa fa-file-text"></i>搜尋排程</h4>
                </div>
                <div class="modal-body">
                    <div id="testmodal2" style="padding: 5px 20px;">
                        <div class="form-group">
                            <label id="Record_Number" style="display: none">0</label>
                            <div id="add_element"></div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                            </div>
                               <button type="button" class="btn btn-info" onclick="add_content('add_element')">新增項目</button>
                            <asp:TextBox ID="TextBox_Schdule" runat="server" style="display:none"></asp:TextBox>

                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="col-md-11 xdisplay_inputx form-group has-feedback">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button id="btnSearch" type="button" class="btn btn-primary antosubmit2 ">執行搜索</button>
                    <asp:Button ID="Button_Search" runat="server" Text="查詢" OnClick="Button_Search_Click" style="display:none" />
                </div>
            </div>
        </div>
    </div>

    <!-----------------/content------------------>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        $(document).ready(function () {         
            //網頁一開始，產生5個TEXTBOX給使用者輸入
            for (i = 0; i < 5; i++)
                add_content('add_element');
        });
        function jump_AsmLineOverView(paramer) {
            var WhatSystem = navigator.userAgent;
            if (!WhatSystem.match(/(iphone|ipad|ipod);?/i)) {
            } else {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                $.unblockUI();
            }
            document.location.href = 'Asm_LineOverView.aspx?key=' + paramer;
        }
        $("#btnSearch").click(function () {
            var Number = '';
            //組合使用者選的刀庫 || 製令
            for (i = 0; i < parseInt(document.getElementById('Record_Number').innerHTML, 10); i++)
                Number += document.getElementById('textbox_' + i).value + '#';
            //寫入TEXTBOX內，讓後端進行運算
            document.getElementById('<%=TextBox_Schdule.ClientID %>').value = Number;

            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btnSearch').disabled = true;
            document.getElementById('<%=Button_Search.ClientID %>').click();
            $.unblockUI();
            document.getElementById('btnSearch').disabled = false;
            $('#exampleModal').modal('hide');
        });

        //新增文本輸入框的事件
        function add_content(div) {
            var Number = '';

            //先儲存數值，以免按下新增時要重打
            for (i = 0; i < parseInt(document.getElementById('Record_Number').innerHTML, 10); i++) {
                Number += `textbox_${i}` + '#' + document.getElementById('textbox_' + i).value + '#';
            }
            //取得要新增的DIV名稱
            var divname = document.getElementById(div);
            //新增控制項到該DIV內
            divname.innerHTML = divname.innerHTML +
                '<div class="col-md-12 col-sm-12 col-xs-12">' +
                '<div class="col-md-4 col-sm-12 col-xs-5" style="margin: 5px 0px 5px 0px">' +
                ' <span>製令編號</span>' +
                ' </div>' +
                ' <div class="col-md-8 col-sm-12 col-xs-7" style="margin: 5px 0px 5px 0px">' +
                `<input type="text" id="textbox_${document.getElementById('Record_Number').innerHTML}" name="textbox_${document.getElementById('Record_Number').innerHTML}" class="form-control text-center"  >` +
                '  </div>'
            '</div>';

            //回復原本的數值
            if (document.getElementById('Record_Number').innerHTML != '0') {
                var valuelist = Number.split("#");
                for (j = 0; j < valuelist.length - 1; j++) {
                    document.getElementById(valuelist[j]).value = valuelist[j + 1];
                    j++;
                }
            }
            //把Label+1(需先轉換 文字->數字)
            document.getElementById('Record_Number').innerHTML = parseInt(document.getElementById('Record_Number').innerHTML, 10) + 1;
        }

    </script>
</asp:Content>
