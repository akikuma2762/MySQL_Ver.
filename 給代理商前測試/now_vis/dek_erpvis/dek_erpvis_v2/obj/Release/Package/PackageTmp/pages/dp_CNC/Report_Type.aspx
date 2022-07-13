<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Report_Type.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_CNC.Report_Type" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>報工類型選擇 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/dp_CNC/Report_Type.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .modal-lg2 {
            width: 1700px
        }

        #Select_Aps {
            padding-top: 7%
        }

        #Table_ApsData_length {
            display: none;
        }

        #Table_ApsData_filter {
            display: none;
        }

        #Table_ApsData_info {
            display: none;
        }

        #Table_ApsData_paginate {
            display: none;
        }
    </style>
    <div class="right_col" role="main">

        <%=path %>
        <!--選擇類型-->
        <div class="row" id="Select_Type">
            <!--當作空白使用-->
            <div class="col-md-12 col-xs-12 col-sm-12 div_space" style="border-top: 15px #2A3F54 groove; border-left: 15px #2A3F54 groove; border-right: 15px #2A3F54 groove;">
            </div>
            <!--當作空白使用-->

            <div class="col-md-12 col-xs-12 col-sm-12" style="border-left: 15px #2A3F54 groove; border-right: 15px #2A3F54 groove;">
                <div class="col-md-3 col-xs-12 col-sm-12">
                </div>
                <div class="col-md-3 col-xs-12 col-sm-12">
                    <button type="button" class="btn_css btn_brown" onclick="change_btntext('進站維護')">維護作業</button>
                </div>
                <div class="col-md-1 col-xs-12 col-sm-12">
                </div>
                <div class="col-md-3 col-xs-12 col-sm-12">
                    <button type="button" class="btn_css btn_green" onclick="change_btntext('進站報工')">進入報工</button>
                </div>
            </div>

            <!--當作空白使用-->
            <div class="col-md-12 col-xs-12 col-sm-12 div_space2" style="border-left: 15px #2A3F54 groove; border-right: 15px #2A3F54 groove;">
            </div>
            <!--當作空白使用-->

            <div class="col-md-12 col-xs-12 col-sm-12 " style="border-left: 15px #2A3F54 groove; border-right: 15px #2A3F54 groove;">

                <div class="col-md-3 col-xs-12 col-sm-12">
                </div>
                <div class="col-md-3 col-xs-12 col-sm-12">
                    <button type="button" class="btn_css btn_purple" onclick="document.location.href='Enter_MaintainView.aspx'">維護總覽</button>
                </div>
                <div class="col-md-1 col-xs-12 col-sm-12">
                </div>
                <div class="col-md-3 col-xs-12 col-sm-12">
                    <button type="button" class="btn_css btn_blue" onclick="document.location.href='Enter_ReportView.aspx'">報工總覽</button>
                </div>
            </div>
            <div class="col-md-12 col-xs-12 col-sm-12 " style="border-bottom: 15px #2A3F54 groove; border-left: 15px #2A3F54 groove; border-right: 15px #2A3F54 groove; height: 210px">
            </div>

        </div>

        <!--讀取設備、工單-->
        <div class="row" style="margin-bottom: 3px; display: none" id="Read_Order">
            <div class="col-md-12 col-xs-12 col-sm-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-top: 10px; text-align: center">
                <b id="first_title" style="font-size: 35px"></b>
            </div>
            <div class="col-md-12 col-xs-12 col-sm-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-top: 10px">
                <div class="col-md-12 col-sm-12 col-xs-12" style="margin-top: 10px">
                    <!--讀取設備內容-->
                    <div class="col-md-2 col-xs-12 col-sm-12 div_style">
                        <b>讀取設備</b>
                    </div>
                    <div class="col-md-2 col-xs-12 col-sm-12" style="margin-top: 3px">
                        <asp:TextBox ID="TextBox_ReadMachine" runat="server" Style="font-size: 29px; width: 100%" onchange="Get_MachineInformation('#Table_Machine','#Table_Order','#ContentPlaceHolder1_TextBox_ReadMachine','#Table_SubmitMachine')" onkeydown="KeyDown('ContentPlaceHolder1_TextBox_Order')"></asp:TextBox>
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="Del_Machine">刪除</button>
                    </div>
                    <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                        <table id="Table_Machine" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                            <thead>
                                <tr style="background-color: white">
                                    <th style="text-align: center">設備代號</th>
                                    <th style="text-align: center">設備群組</th>
                                    <th style="text-align: center">設備名稱</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 3px; margin-bottom: 8px;">
                    <hr style="border: 2px solid #dfdfdf;" />
                </div>
                <!--讀取設備內容-->
                <!--讀取工單-->
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <asp:TextBox ID="TextBox_ApsMachine" runat="server" Style="display: none"></asp:TextBox>
                    <asp:Label ID="Label_RecordSelectOrder" runat="server" Text="" Style="display: none"></asp:Label>

                    <div class="col-md-2 col-xs-12 col-sm-12 div_style">
                        <b>讀取工單</b>
                    </div>
                    <div class="col-md-2 col-xs-12 col-sm-12" style="margin-top: 3px">
                        <asp:TextBox ID="TextBox_Order" runat="server" Style="font-size: 29px; width: 100%" onchange="Get_OrderInformation('#Table_Order','#ContentPlaceHolder1_TextBox_Order','#Table_Machine')" onkeydown="KeyDown('ContentPlaceHolder1_TextBox_Order')"></asp:TextBox>
                    </div>

                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="Del_Order">刪除</button>
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="Aps_Source" style="display: none" data-target="#Select_Aps" data-toggle="modal">APS訂單</button>
                    </div>

                    <div class="col-md-1 col-xs-12 col-sm-12" style="margin-top: 3px">
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12 div_style">
                        <b>乘</b>
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12" style="margin-top: 3px">
                        <asp:TextBox ID="TextBox_Multiplication" runat="server" Style="font-size: 29px; width: 100%; text-align: center" TextMode="Number">1</asp:TextBox>
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12" style="margin-top: 3px">
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12 div_style">
                        <b>除</b>
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12" style="margin-top: 3px">
                        <asp:TextBox ID="TextBox_Division" runat="server" Style="font-size: 29px; width: 100%; text-align: center" TextMode="Number">1</asp:TextBox>
                    </div>

                    <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                        <table id="Table_Order" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                            <thead>
                                <tr style="background-color: white">
                                    <th style="text-align: center">工單號碼</th>
                                    <th style="text-align: center">品號</th>
                                    <th style="text-align: center">品名</th>
                                    <th style="text-align: center">規格</th>
                                    <th style="text-align: center">加工順序</th>
                                    <th style="text-align: center">預計產量</th>
                                    <th style="text-align: center">已生產量</th>
                                    <th style="text-align: center">未生產量</th>
                                    <th style="text-align: center">預交日期</th>
                                    <th style="text-align: center">製程代號</th>
                                    <th style="text-align: center">製程名稱</th>
                                    <%--<th style="text-align: center">標準工時</th>--%>
                                    <th style="text-align: center">客戶代號</th>
                                    <th style="text-align: center">客戶名稱</th>
                                    <th style="text-align: center">製程型態</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--讀取工單-->

                <!--底下按鈕-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="pre_first" onclick="Previous_btn()">上一頁</button>
                    </div>
                    <div class="col-md-10 col-xs-12 col-sm-12">
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="first_submit" onclick="Next_btn()">確認</button>
                    </div>
                </div>
                <!--底下按鈕-->
            </div>
        </div>

        <!--讀取人員條碼-->
        <div class="row" style="margin-bottom: 3px; display: none" id="Read_Staff">
            <div class="col-md-12 col-xs-12 col-sm-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-top: 10px; text-align: center">
                <b id="second_title" style="font-size: 35px"></b>
            </div>
            <div class="col-md-12 col-xs-12 col-sm-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-top: 10px">
                <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: center; font-size: 30px">
                    <b>本筆工單內容</b>
                </div>
                <!--本筆工單資訊-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <table id="Table_NowImformation" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                        <thead>
                            <tr style="background-color: white">
                                <th style="text-align: center">工單號碼</th>
                                <th style="text-align: center">品號</th>
                                <th style="text-align: center">品名</th>
                                <th style="text-align: center">規格</th>
                                <th style="text-align: center">加工順序</th>
                                <th style="text-align: center">預計產量</th>
                                <th style="text-align: center">已生產量</th>
                                <th style="text-align: center">未生產量</th>
                                <th style="text-align: center">預交日期</th>
                                <th style="text-align: center">製程代號</th>
                                <th style="text-align: center">製程名稱</th>
                                <%--<th style="text-align: center">標準工時</th>--%>
                                <th style="text-align: center">客戶代號</th>
                                <th style="text-align: center">客戶名稱</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <!--本筆工單資訊-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 3px; margin-bottom: 8px;">
                    <hr style="border: 2px solid #dfdfdf;" />
                </div>
                <!--讀取人員-->
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="col-md-2 col-xs-12 col-sm-12 div_style">
                        <b>讀取人員條碼</b>
                    </div>
                    <div class="col-md-2 col-xs-12 col-sm-12" style="margin-top: 3px">
                        <asp:TextBox ID="TextBox_Person" runat="server" Style="font-size: 29px; width: 100%" onchange="Get_StaffInformation('#Table_Person','#Table_SubmitPerson','#ContentPlaceHolder1_TextBox_Person')" onkeydown="KeyDown('ContentPlaceHolder1_TextBox_Person')"></asp:TextBox>
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="Del_Staff">刪除</button>
                    </div>
                </div>

                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                        <table id="Table_Person" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                            <thead>
                                <tr style="background-color: white">
                                    <th style="text-align: center">人員代號</th>
                                    <th style="text-align: center">人員姓名</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--讀取人員-->

                <!--底下按鈕-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="pre_second" onclick="Previous_btn()">上一頁</button>
                    </div>

                    <div class="col-md-10 col-xs-12 col-sm-12">
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="second_submit" onclick="Next_btn()">確認</button>
                    </div>
                </div>
                <!--底下按鈕-->
            </div>
        </div>

        <!--最後確認畫面-->
        <div class="row" style="margin-bottom: 3px; display: none" id="Submit_Imformation">
            <asp:TextBox ID="TextBox_SaveMachine" runat="server" Style="display: none"></asp:TextBox>
            <asp:TextBox ID="TextBox_SaveAllOrder" runat="server" Style="display: none"></asp:TextBox>
            <asp:TextBox ID="TextBox_SaveStaff" runat="server" Style="display: none"></asp:TextBox>
            <asp:TextBox ID="TextBox_SaveMode" runat="server" Style="display: none"></asp:TextBox>
            <asp:Label ID="Label_Collect" runat="server" Text="" Style="display: none"></asp:Label>
            <asp:Label ID="Label_MachID" runat="server" Text="" Style="display: none"></asp:Label>

            <div class="col-md-12 col-xs-12 col-sm-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-top: 10px; text-align: center">
                <b id="third_title" style="font-size: 35px"></b>
            </div>
            <div class="col-md-12 col-xs-12 col-sm-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-top: 10px">
                <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: center; font-size: 30px">
                    <b>當前機台資訊</b>
                </div>
                <!--確定機台資訊-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <table id="Table_SubmitMachine" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                        <thead>
                            <tr style="background-color: white">
                                <th style="text-align: center">設備代號</th>
                                <th style="text-align: center">設備群組</th>
                                <th style="text-align: center">設備名稱</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 3px; margin-bottom: 8px;">
                    <hr style="border: 2px solid #dfdfdf;" />
                </div>
                <!--確定機台資訊-->

                <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: center; font-size: 30px">
                    <b>本筆工單內容</b>
                </div>
                <!--確定工單資訊-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <table id="Table_SubmitOrder" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                        <thead>
                            <tr style="background-color: white">
                                <th style="text-align: center">工單號碼</th>
                                <th style="text-align: center">品號</th>
                                <th style="text-align: center">品名</th>
                                <th style="text-align: center">規格</th>
                                <th style="text-align: center">加工順序</th>
                                <th style="text-align: center">預計產量</th>
                                <th style="text-align: center">已生產量</th>
                                <th style="text-align: center">未生產量</th>
                                <th style="text-align: center">預交日期</th>
                                <th style="text-align: center">製程代號</th>
                                <th style="text-align: center">製程名稱</th>
                                <%--<th style="text-align: center">標準工時</th>--%>
                                <th style="text-align: center">客戶代號</th>
                                <th style="text-align: center">客戶名稱</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 3px; margin-bottom: 8px;">
                    <hr style="border: 2px solid #dfdfdf;" />
                </div>
                <!--確定工單資訊-->

                <!--確定人員資訊-->
                <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: center; font-size: 30px">
                    <b>進站人員資訊</b>
                </div>
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <table id="Table_SubmitPerson" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                        <thead>
                            <tr style="background-color: white">
                                <th style="text-align: center">人員代號</th>
                                <th style="text-align: center">人員名稱</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>

                <!--確定人員資訊-->
                <!--底下按鈕-->
                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-top: 20px">
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="pre_third" onclick="Previous_btn()">上一頁</button>
                    </div>
                    <div class="col-md-10 col-xs-12 col-sm-12">
                    </div>
                    <div class="col-md-1 col-xs-12 col-sm-12">
                        <button type="button" class="btn_orange" id="check_btn" onclick="Enter_Report()">進站報工</button>
                        <asp:Button ID="Button_Save" runat="server" OnClick="Button_Save_Click" Text="Button" Style="display: none" />
                    </div>
                </div>
                <!--底下按鈕-->
            </div>

        </div>

    </div>

    <!--跳出APS的資料給使用者選取-->

    <div id="Select_Aps" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog  modal-lg2 ">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda2f2" style="padding: 5px 20px;">
                        <asp:Label ID="Label_IsDelete" runat="server" Text="" Style="display: none"></asp:Label>
                        <div id="div_person" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px">
                            <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                                <b style="font-size: 30px">本筆工單報工</b>
                                <hr style="border-bottom: 2px solid #E6E9ED; margin-top: 0px; margin-bottom: 0px" />
                            </div>
                            <br />
                            <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                <table id="Table_ApsData" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                    <thead>
                                        <tr style="background-color: white">
                                            <th style="text-align: center">機台名稱</th>
                                            <th style="text-align: center">工單號碼</th>
                                            <th style="text-align: center">製程名稱</th>
                                            <th style="text-align: center">工藝名稱</th>
                                            <th style="text-align: center">需求數量</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="text-align: center; padding: 15px;">
                    <button type="button" class="btn btn-primary antoclose2" id="select_btn" style="margin-top: 10px">選擇</button>
                    <button type="button" class="btn btn-default antoclose2" data-dismiss="modal" style="margin-top: 10px">退出作業</button>
                </div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <link href="../../assets/vendors/Create_HtmlCode/jquery.dataTables.min.css" rel="stylesheet" />
    <script>
        //api網址(不帶參數的)


        //改變按鈕文字跟按鈕顏色
        function change_btntext(text) {
            var hide_id = document.getElementById("Select_Type");
            hide_id.style.display = 'none';
            var show_id = document.getElementById("Read_Order");
            show_id.style.display = 'initial';
            var btn_id = document.getElementById("check_btn");
            btn_id.innerHTML = text;
            $('#<%=TextBox_SaveMode.ClientID%>').val('' + text + '');
            document.getElementById("first_title").innerHTML = text;
            document.getElementById("second_title").innerHTML = text;
            document.getElementById("third_title").innerHTML = text;

            //改變後面按鈕顏色
            if (text == '進站維護')
                color = '#843C0C';
            else
                color = 'green';
            document.getElementById("Del_Machine").style.background = color;
            document.getElementById("Del_Order").style.background = color;
            document.getElementById("first_submit").style.background = color;
            document.getElementById("Del_Staff").style.background = color;
            document.getElementById("second_submit").style.background = color;
            document.getElementById("check_btn").style.background = color;
            document.getElementById("pre_third").style.background = color;
            document.getElementById("pre_second").style.background = color;
            document.getElementById("pre_first").style.background = color;

            //若重新進入選擇頁面，則需清空全部的表
            $('#Table_Machine').dataTable().fnClearTable();
            $('#Table_Order').dataTable().fnClearTable();
            $('#Table_NowImformation').dataTable().fnClearTable();
            $('#Table_Person').dataTable().fnClearTable();
            $('#Table_SubmitMachine').dataTable().fnClearTable();
            $('#Table_SubmitOrder').dataTable().fnClearTable();
            $('#Table_SubmitPerson').dataTable().fnClearTable();
        }

        //確認
        function Next_btn() {
            var div_list = ["Select_Type", "Read_Order", "Read_Staff", "Submit_Imformation"];
            var now_show = '';
            var next_show = '';
            for (var i = 0; i < div_list.length; i++) {
                if (document.getElementById(div_list[i]).style.display == 'initial') {
                    now_show = div_list[i];
                    next_show = div_list[i + 1];
                    break;
                }
            }

            var machine_count = $('#Table_Machine').DataTable().data().count();
            var order_count = $('#Table_Order').DataTable().data().count();
            var staff_count = $('#Table_Person').DataTable().data().count();



            var ok = true;
            if (now_show == 'Read_Order') {
                if (machine_count == 0 || order_count == 0)
                    ok = false;
            }
            else if (now_show == 'Read_Staff') {
                if (staff_count == 0)
                    ok = false;
            }

            if (ok) {
                var hide_id = document.getElementById(now_show);
                hide_id.style.display = 'none';
                var show_id = document.getElementById(next_show);
                show_id.style.display = 'initial';
            }
            else
                alert('請確認資訊是否填寫正常');
        }

        //上一頁
        function Previous_btn() {
            var div_list = ["Select_Type", "Read_Order", "Read_Staff", "Submit_Imformation"];
            var now_show = '';
            var previous_show = '';
            for (var i = 0; i < div_list.length; i++) {
                if (document.getElementById(div_list[i]).style.display == 'initial') {
                    now_show = div_list[i];
                    previous_show = div_list[i - 1];
                    break;
                }
            }
            var hide_id = document.getElementById(now_show);
            hide_id.style.display = 'none';
            var show_id = document.getElementById(previous_show);
            show_id.style.display = 'initial';
        }

        //觸發事件
        $(document).ready(function () {

            //刪除機台資訊
            var Table_Machine = $('#Table_Machine').DataTable();
            $('#Table_Machine tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    Table_Machine.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
            $('#Del_Machine').click(function () {
                Table_Machine.row('.selected').remove().draw(false);
            });

            //刪除工單資訊
            var Table_Order = $('#Table_Order').DataTable();
            $('#Table_Order tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    Table_Order.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
            $('#Del_Order').click(function () {
                Table_Order.row('.selected').remove().draw(false);
            });

            //刪除人員資訊
            var Table_Staff = $('#Table_Person').DataTable();
            $('#Table_Person tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    Table_Staff.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
            $('#Del_Staff').click(function () {
                Table_Staff.row('.selected').remove().draw(false);
            });



            //複製選取之排程
            $('#first_submit').click(function () {

                //複製人員的table
                var tables = ($('#Table_Order').DataTable().rows().data());
                var now_order = $('#Table_NowImformation').dataTable();
                var sumbit_order = $('#Table_SubmitOrder').dataTable();
                now_order.fnClearTable();
                sumbit_order.fnClearTable();

                var allorder = '';
                for (var i = 0; i < tables.length; i++) {
                    var datas = [];
                    var order_list = tables[i];
                    for (j = 0; j < order_list.length; j++) {
                        allorder += order_list[j] + '#';
                        datas.push(order_list[j]);
                    }

                    now_order.fnAddData(datas);
                    sumbit_order.fnAddData(datas);
                }
                $('#<%=TextBox_SaveAllOrder.ClientID%>').val('' + allorder.replaceAll('</center>', '').replaceAll('<center>', '') + '');
            });

            //複製人員
            $('#second_submit').click(function () {
                //複製人員的table
                var tables = ($('#Table_Person').DataTable().rows().data());
                var submit_staff = $('#Table_SubmitPerson').dataTable();
                submit_staff.fnClearTable();
                var allstaff = '';
                for (var i = 0; i < tables.length; i++) {
                    var datas = [];
                    var staff_list = tables[i];
                    for (j = 0; j < staff_list.length; j++) {
                        allstaff += staff_list[j] + '#';
                        datas.push(staff_list[j]);
                    }
                    submit_staff.fnAddData(datas);
                }
                $('#<%=TextBox_SaveStaff.ClientID%>').val('' + allstaff.replaceAll('</center>', '').replaceAll('<center>', '') + '');
            });
            //跳出目前在APS內的訂單資訊
            $('#Aps_Source').click(function () {
                var savedtable = $('#Table_ApsData').dataTable();
                savedtable.fnClearTable();
                $.ajax({
                    type: 'POST',
                    dataType: 'xml',
                    url: "../../webservice/dpCNC_MachData.asmx/Get_ApsData",
                    data: { machine: document.getElementById('<%=TextBox_ApsMachine.ClientID%>').value },

                    success: function (xml) {
                        $(xml).find("ROOT_PIE").each(function (i) {
                            if ($(xml).find("ROOT_PIE").length > 0) {
                                $(this).children().each(function (j) {
                                    var now_data = [];
                                    now_data.push('<center>' + $(this).attr("機台名稱").valueOf() + '</center>');
                                    now_data.push('<center>' + $(this).attr("訂單號碼").valueOf() + '</center>');
                                    now_data.push('<center>' + $(this).attr("製程代號").valueOf() + '</center>');
                                    now_data.push('<center>' + $(this).attr("工藝名稱").valueOf() + '</center>');
                                    now_data.push('<center>' + $(this).attr("預計數量").valueOf() + '</center>');
                                    savedtable.fnAddData(now_data);
                                })
                            }

                        });
                    },
                    error: function (data, errorThrown) {
                        savedtable.fnClearTable();
                    }
                });
            });

            //選擇APS的資料表
            var Table_APS = $('#Table_ApsData').DataTable();
            $('#Table_ApsData tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    Table_APS.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });

            //把選擇之項目，傳至TEXTBOX內
            $('#select_btn').click(function () {
                var tablevalues = ($('#Table_ApsData').DataTable().rows('.selected').data());
                if (tablevalues.length > 0) {
                    var value = tablevalues[0];
                    var orders = value[1].replaceAll('</center>', '').replaceAll('<center>', '');
                    var craft = value[2].replaceAll('</center>', '').replaceAll('<center>', '');

                    if (craft.length == 1)
                        craft = '000' + craft;
                    else if (craft.length == 2)
                        craft = '00' + craft;
                    else if (craft.length == 3)
                        craft = '0' + craft;

                    document.getElementById('<%=TextBox_Order.ClientID %>').value = orders + '-' + craft;

                    Get_OrderInformation('#Table_Order', '#ContentPlaceHolder1_TextBox_Order', '#Table_Machine');

                    setTimeout(function () {
                        if (document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML == '1')
                            Table_APS.row('.selected').remove().draw(false);

                    }, 300);
                }
                else
                    alert('未選取上傳之項目');
            });
        });

        //從API處取得機台資訊(tablename 機台資訊表,cleartext 參數之textbox,ordertable 訂單資料表,submittable 最後確認資料表)
        function Get_MachineInformation(tablename, ordertable, cleartext, submittable) {
            //取得輸入之機台
            var id = document.getElementById(cleartext.replaceAll('#', '')).value;

            //取得API網址
            //取得新增之Table
            var nowtable = $(tablename).dataTable();
            var subtable = $(submittable).dataTable();
            var str_list;
            var save_machine = '';

            nowtable.fnClearTable();
            subtable.fnClearTable();

            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/GetMachine",
                data: { mach: id },

                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            $(this).children().each(function (j) {
                                var now_data = [];

                                now_data.push('<center>' + $(this).attr("設備代號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("設備群組").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("設備名稱").valueOf() + '</center>');
                                $('#<%=TextBox_SaveMachine.ClientID%>').val('' + $(this).attr("設備代號").valueOf() + '#' + $(this).attr("設備群組").valueOf() + '#' + $(this).attr("設備名稱").valueOf() + '#' + '');
                                nowtable.fnAddData(now_data);
                                subtable.fnAddData(now_data);
                                //202220425 補上判斷是否為機報工//Label_MachID
                                document.getElementById('<%=Label_Collect.ClientID%>').innerHTML = $(this).attr("是否採集").valueOf();
                                document.getElementById('<%=Label_MachID.ClientID%>').innerHTML = $(this).attr("設備代號").valueOf();

                            })
                            document.getElementById('Aps_Source').style.display = 'none';
                            document.getElementById('<%=TextBox_ApsMachine.ClientID %>').value = id;
                        }
                        else {
                            alert('此機台不存在，請重新輸入');
                            nowtable.fnClearTable();
                            subtable.fnClearTable();
                            document.getElementById('Aps_Source').style.display = 'none';
                            document.getElementById('<%=TextBox_ApsMachine.ClientID %>').value = '';
                        }
                    });
                },
                error: function (data, errorThrown) {
                    alert('此機台不存在，請重新輸入');
                    nowtable.fnClearTable();
                    subtable.fnClearTable();
                    document.getElementById('Aps_Source').style.display = 'none';
                    document.getElementById('<%=TextBox_ApsMachine.ClientID %>').value = '';
                }
            });

            var savedtable = $(ordertable).dataTable();
            savedtable.fnClearTable();
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/GetMachineOrder",
                data: { mach: id, type: document.getElementById('<%=TextBox_SaveMode.ClientID%>').value },

                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            $(this).children().each(function (j) {
                                var now_data = [];
                                now_data.push('<center>' + $(this).attr("工單號碼").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("品號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("品名").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("規格").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("加工順序").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("預計產量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("已生產量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("未生產量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("預交日期").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("製程代號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("製程名稱").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("客戶代號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("客戶名稱").valueOf() + '</center>');
                                now_data.push('<center>update</center>');
                                savedtable.fnAddData(now_data);
                            })
                        }

                    });
                },
                error: function (data, errorThrown) {
                    savedtable.fnClearTable();

                }
            });

            //清空TEXTBOX
            $(cleartext).val('' + '');
        }

        //機台報工要先將計數歸零
        function Set_MachCountInit(machineID) {
            //取得輸入之機台
            //var id = document.getElementById(cleartext.replaceAll('#', '')).value;
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/CNCCountInit",
                data: { mach: machineID },
                //ROOT_PIE API那邊定義標籤的
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            //執行webservice就可以沒有其他動作-->webservice內執行API呼叫
                        }
                        else {
                            alert('Ini-01 此機台不存在，請重新輸入');
                            document.getElementById('<%=Label_Collect.ClientID %>').innerHTML = '';
                            document.getElementById('<%=Label_MachID.ClientID%>').innerHTML = '';
                        }
                    });
                },
                error: function (data, errorThrown) {
                    alert('Ini-01 此機台不存在，請重新輸入');
                    document.getElementById('<%=Label_Collect.ClientID %>').value = '';
                    document.getElementById('<%=Label_MachID.ClientID%>').innerHTML = '';
                }
            });
        }

        //從API處取得工單資訊(tablename 工單資料表,cleartext 參數之textbox)
        function Get_OrderInformation(tablename, cleartext, machTbName) {
            var sameor = false;
            //取得輸入之機台
            var id = document.getElementById(cleartext.replaceAll('#', '')).value;
            //找到機台資料表的第一筆資料Table_Machine
            var Machtables = document.getElementById('Table_Machine');
            //取得機號
            var Mach = Machtables.rows[1].cells[0].innerText;
            //取得報工模式
            var mode_t = document.getElementById('first_title').innerHTML;
            //取得API網址
            //取得新增之Table
            var nowtable = $(tablename).dataTable();
            //檢查相同工單 是否已入站 0601 juiedit
            var Odtables = ($('#Table_Order').DataTable().rows().data());
            var allstaff = '';
            for (var i = 0; i < Odtables.length; i++) {
                var datas = [];
                var order_list = Odtables[i];
                for (j = 0; j < order_list.length; j++) {
                    if (order_list[j] == ('<center>' + id + '</center>')) {
                        sameor = true;
                    }
                }
            }
            if (!sameor) {
                $.ajax({
                    type: 'POST',
                    dataType: 'xml',
                    url: "../../webservice/dpCNC_MachData.asmx/GetOrderInfromation",
                    data: { ordernumber: id, machine: Mach },

                    success: function (xml) {
                        $(xml).find("ROOT_PIE").each(function (i) {
                            if ($(xml).find("ROOT_PIE").length > 0) {
                                $(this).children().each(function (j) {
                                    if ($(this).attr("ERROR").valueOf()!= "0") {
                                        alert('ERROR:' + $(this).attr("ERROR").valueOf() + '!');
                                        document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                                    }
                                    else {
                                        //找到訂單資料表的第一筆資料
                                        var tables = ($('#Table_Order').DataTable().rows(0).data());
                                        //存入陣列
                                        var order_list = tables[0];

                                        //取出品號
                                        var order_first = '';
                                        if (order_list != undefined) {
                                            order_first = tables[0][1];
                                        }
                                        //取出製成代號
                                        var order_craft = '';
                                        if (order_list != undefined) {
                                            order_craft = tables[0][9];
                                        }
                                        //取出製成代號
                                        var order_craftType = '';
                                        if (order_list != undefined) {
                                            order_craftType = tables[0][13];
                                        }
                                        //alert(order_craftType);
                                        //
                                        //alert($(this).attr("製程型態").valueOf());
                                        if ((order_first != '' || order_list != undefined) && order_first != '<center>' + $(this).attr("品號").valueOf() + '</center>' && (order_craftType == '<center>1</center>'||$(this).attr("製程型態").valueOf() != "2")) {
                                            alert('請輸入相同料號');
                                            document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                                        }
                                        else if ((order_craft != '' || order_list != undefined) && order_craft != '<center>' + $(this).attr("製程代號").valueOf() + '</center>' && (order_craftType == '<center>1</center>' || $(this).attr("製程型態").valueOf() != "2")) {
                                            alert('請輸入相同製程');
                                            document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                                        }
                                        else if (mode_t == "進站維護" && $(this).attr("工單號碼").valueOf() == "xxxxdek") {
                                            alert('目前工單報工入站中，請先工單出站，再進行維護!');
                                            document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                                        }
                                        else {
                                            var now_data = [];
                                            now_data.push('<center>' + $(this).attr("工單號碼").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("品號").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("品名").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("規格").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("加工順序").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("預計產量").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("已生產量").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("未生產量").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("預交日期").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("製程代號").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("製程名稱").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("客戶代號").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("客戶名稱").valueOf() + '</center>');
                                            now_data.push('<center>' + $(this).attr("製程型態").valueOf() + '</center>');
                                            now_data.push('<center>insert</center>');
                                            nowtable.fnAddData(now_data);
                                            document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '1';
                                        }

                                    }
                                })
                            }
                            else {
                                alert('此工單' + id + '不存在，請重新輸入');
                                document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                            }
                        });
                    },
                    error: function (data, errorThrown) {
                        alert('此工單' + id + '不存在，請重新輸入');
                        document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                    }
                });
                $(cleartext).val('' + '');
            }
            else {
                alert('工單已入站，請重新輸入!');
                document.getElementById('<%=Label_IsDelete.ClientID %>').innerHTML = '0';
                $(cleartext).val('' + '');
            }
        }


        //從API處取得人員資訊
        function Get_StaffInformation(tablename, submittable, cleartext) {
            //取得輸入之機台
            var id = document.getElementById(cleartext.replaceAll('#', '')).value;
            //取得API網址
            //取得新增之Table
            var nowtable = $(tablename).dataTable();

            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/GetStaff",
                data: { staffNumber: id },
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            $(this).children().each(function (j) {
                                var now_data = [];
                                now_data.push('<center>' + $(this).attr("人員代號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("人員姓名").valueOf() + '</center>');
                                nowtable.fnAddData(now_data);
                            })
                        }
                        else
                            alert('此人員不存在，請重新輸入');
                    });
                },
                error: function (data, errorThrown) {
                    alert('此人員不存在，請重新輸入');
                }
            });
            $(cleartext).val('' + '');
        }

        //進入報工
        function Enter_Report() {
            var machine_count = $('#Table_SubmitMachine').DataTable().data().count();
            var order_count = $('#Table_SubmitOrder').DataTable().data().count();
            var staff_count = $('#Table_SubmitPerson').DataTable().data().count();
            if (machine_count > 0 && order_count > 0 && staff_count > 0) {
                answer = confirm("即將入站報工，請確認資訊是否正確");
                if (answer) {
                    <%--20220425--檢查是否機報工-是的話要清除機台歸零---%>
                    var isCollect = document.getElementById('<%=Label_Collect.ClientID%>');
                    if (isCollect.innerHTML == "是") {
                        var id = document.getElementById('<%=Label_MachID.ClientID%>').innerHTML;
                        Set_MachCountInit(id);
                    }
                    <%----報工---%>
                    document.getElementById('<%=Button_Save.ClientID %>').click();
                }
            }
            else
                alert('請確認資訊是否填寫正常');
        }

        //決定按下ENTER之後，focus位置
        function KeyDown(textid) {
            if (event.keyCode == 13) {
                event.preventDefault();
                document.getElementById('<%=TextBox_ReadMachine.ClientID%>').focus();
                document.getElementById('Del_Staff').focus();
                document.getElementById(textid).focus();
            }
        }
    </script>
</asp:Content>

