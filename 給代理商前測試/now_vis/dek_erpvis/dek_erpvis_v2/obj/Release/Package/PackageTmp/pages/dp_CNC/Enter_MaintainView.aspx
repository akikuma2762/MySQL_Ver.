<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Enter_MaintainView.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_CNC.Enter_MaintainView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>進站維護總覽 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/Orders_Details.css" rel="stylesheet" />
    <link href="../../Content/dp_CNC/Enter_ReportView.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        ._select {
            display: none;
        }

        .button_use {
            z-index: 999;
            margin: 0;
            top: 35%;
            right: 0%;
            -ms-transform: translateY(-30%);
            transform: translateY(-30%);
            width: 33px;
            height: 240px;
            line-height: 235px;
            font-family: verdana;
            text-align: center;
            background: #009efa;
            color: #fff;
            position: fixed;
            bottom: 20px;
            right: 0px;
            text-decoration: none;
            cursor: pointer;
        }

        @media screen and (max-width:768px) {
            .button_use {
                display: none;
            }

            ._select {
                display: block;
            }

            .exportChart {
                display: none;
            }
        }
    </style>

    <!--主畫面-->
    <div class="right_col" role="main">
        <%=path %>
        <br>
        <asp:TextBox ID="TextBox_MachTypeText" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_MachTypeValue" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_MachGroupText" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_MachGroupValue" runat="server" Style="display: none"></asp:TextBox>

        <div class="clearfix"></div>
        <div class="col-md-12 col-sm-12 col-xs-12">
            <div class="col-md-4 ">
            </div>
            <div class="col-md-1 col-sm-12 col-xs-12">
                <div class="col-md-5 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_Running.PNG" alt="..." width="30px" height="30px">
                </div>
                <div class="col-md-7 col-sm-9 col-xs-9"><span>入站</span></div>
                <hr />
            </div>

            <div class="col-md-1 col-sm-12 col-xs-12">
                <div class="col-md-5 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_Stopping.PNG" alt="..." width="30px" height="30px">
                </div>
                <div class="col-md-7 col-sm-9 col-xs-9"><span>暫停</span></div>
                <hr />
            </div>

            <div class="col-md-2 col-sm-12 col-xs-12">
                <div class="col-md-3 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_ExStopping.PNG" alt="..." width="30px" height="30px">
                </div>
                <div class="col-md-9 col-sm-9 col-xs-9"><span>除外暫停</span></div>
                <hr />
            </div>
        </div>
        <div class="x_panel Div_Shadow">
            <div class="x_content">
                <div class="row">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBox_SaveColumn" runat="server" Style="display: none"></asp:TextBox>
                            <asp:Button ID="Button_SaveColumns" runat="server" Text="Button" OnClick="Button_SaveColumns_Click" Style="display: none" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <!--表格畫面-->
                    <div class="col-md-12 col-sm-12 col-xs-12" id="table_information">
                        <div id="Enter_ReportView"></div>
                    </div>
                    <!--控制項-->
                    <div id="_select" class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                        <div class="x_panel">


                            <div class="col-md-12 col-sm-6 col-xs-12">
                                <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 4px 0px 5px 0px">
                                    <span style="font-size: 18px">廠區名稱</span>
                                </div>
                                <div class="col-md-8 col-sm-9 col-xs-8">
                                    <asp:DropDownList ID="DropDownList_MachType" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%" onchange="dropdownlist_change('ContentPlaceHolder1_DropDownList_MachType','ContentPlaceHolder1_DropDownList_MachGroup')"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-12 col-sm-6 col-xs-12">
                                <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                    <span>群組名稱</span>
                                </div>
                                <div class="col-md-8 col-sm-12 col-xs-8">
                                    <asp:DropDownList ID="DropDownList_MachGroup" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%" onchange="show_machines('ContentPlaceHolder1_DropDownList_MachGroup')"></asp:DropDownList>
                                </div>
                            </div>
                            <div id="div_machines" class="col-md-12 col-sm-6 col-xs-12" style="display: none">
                                <asp:TextBox ID="TextBox_Machines" runat="server" Style="display: none"></asp:TextBox>
                                <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                    <span>群組機台</span>
                                </div>
                                <div class="col-md-8 col-sm-12 col-xs-8">
                                    <label id="machines"></label>
                                </div>
                            </div>
                            <div class="col-md-12 col-sm-6 col-xs-12">
                                <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                    <span>選擇欄位</span>
                                </div>
                                <div class="col-md-8 col-sm-12 col-xs-8">
                                    <asp:CheckBoxList ID="CheckBoxList_Columns" runat="server" RepeatColumns="2"></asp:CheckBoxList>
                                </div>
                            </div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="col-md-9 col-xs-8">
                                </div>
                                <div class="col-md-3 col-xs-12">
                                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行搜索</button>
                                    <asp:Button ID="Button_Check" runat="server" Text="Button" OnClick="Button_Check_Click" Style="display: none" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--控制項收縮-->
    <div id="div_button" class="button_use" onclick="hide_div()">
        <i id="iconimage" class="fa fa-chevron-left"></i>
    </div>

    <!--人員維護 model-->
    <div id="Report_Model" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-lg2">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda22" style="padding: 5px 20px;">
                        <asp:TextBox ID="TextBox_Order" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Machine" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Staffs" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Number" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Show" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Group" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Status" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_BadInformation" runat="server" Style="display: none"></asp:TextBox>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div id="div_now" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                                    <b style="font-size: 30px">本筆工單維護</b>
                                    <hr style="border-bottom: 2px solid #E6E9ED; margin-top: 0px; margin-bottom: 0px" />
                                </div>
                                <br />
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <table id="Table_now" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                        <thead>
                                            <tr style="background-color: white">
                                                <th style="text-align: center">設備名稱</th>
                                                <th style="text-align: center">工單號碼</th>
                                                <th style="text-align: center">品號</th>
                                                <th style="text-align: center">品名</th>
                                                <th style="text-align: center">預計產量</th>
                                                <th style="text-align: center">已生產量</th>
                                                <th style="text-align: center">今日產量</th>
                                                <th style="text-align: center">未生產量</th>
                                                <th style="text-align: center">進度</th>
                                                <th style="text-align: center">開工時間</th>
                                                <th style="text-align: center">製程名稱</th>
                                                <th style="text-align: center">人員名稱</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <div class="col-xs-12 col-sm-12 col-md-5" style="margin-bottom: 20px">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        進站人員
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        <asp:DropDownList ID="DropDownList_Staff" runat="server" Style="text-align: center" Width="100%"></asp:DropDownList>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        <button type="button" id="change_staff" onclick="Set_StaffTable()" data-target="#staff_change" data-toggle="modal">人員異動</button>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <div class="col-xs-12 col-sm-12 col-md-5" style="margin-bottom: 20px">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        累計良品數量
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        <asp:TextBox ID="TextBox_Good" min="0" runat="server" TextMode="Number" Text="0" Style="text-align: center" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <div class="col-xs-12 col-sm-12 col-md-5" style="margin-bottom: 20px">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        本次數量
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        <asp:TextBox ID="TextBox_NowCount" min="0" runat="server" TextMode="Number" Text="0" Style="text-align: center" Width="100%"></asp:TextBox>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1" style="margin-bottom: 20px">
                                        <button type="button" id="report_count" data-toggle="modal" data-target="#Report_Count" onclick="previous_result('#Table_Report')">回報數量</button>
                                    </div>

                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <div class="col-xs-12 col-sm-12 col-md-2">
                                        <button type="button" class=" btn_red" id="Stop" data-toggle="modal" data-target="#stop_reason">暫停</button>
                                        <asp:Button ID="Button_Cancel" runat="server" Text="Button" OnClick="Button_Cancel_Click" Style="display: none" />
                                        <button type="button" class=" btn_red" id="Write" style="display: none">異常填寫</button>
                                        <button type="button" class=" btn_blue" id="Action" style="display: none">取消暫停</button>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-3">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1">
                                        不良數量
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1">
                                        <asp:TextBox ID="TextBox_NoGood" runat="server" TextMode="Number" Style="text-align: center" Text="0" Width="100%" Enabled="false"></asp:TextBox>
                                        <asp:TextBox ID="TextBox_badqty" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-1">
                                        <button type="button" id="bad_reason" data-toggle="modal" onclick="clear_bad()" data-target="#nogood_reason">不良原因</button>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-2">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-2">
                                        <asp:Button ID="Button_Exit" runat="server" Text="Button" OnClick="Button_Exit_Click" Style="display: none" />
                                        <button type="button" class="btn_green" data-toggle="modal" data-target="#Preview_Results" id="Pre_Exit" onclick="previous_result('#Table_Result')">出站前預覽</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="text-align: center; padding: 15px;">
                        <b style="color: white">s</b>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--人員異動 model-->
    <div id="staff_change" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog divstaff_change ">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda2f2" style="padding: 5px 20px;">
                        <asp:TextBox ID="TextBox_Staff" runat="server" Style="display: none"></asp:TextBox>
                        <div id="div_person" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px">
                            <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                            </div>
                            <div class="col-md-12 col-xs-12 col-sm-12">
                                <div class="col-md-1 col-xs-12 col-sm-12"></div>
                                <div class="col-md-4 col-xs-12 col-sm-12" style="background-color: #b0d15e; border-radius: 10px; height: 30px; text-align: center">
                                    <b style="font-size: 18px;">讀取人員條碼</b>
                                </div>
                                <div class="col-md-2 col-xs-12 col-sm-12">
                                    <asp:TextBox ID="TextBox_Person" runat="server" onchange="add_staff('#Table_person','#ContentPlaceHolder1_TextBox_Person')"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                <table id="Table_person" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
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
                    </div>
                </div>
                <div style="text-align: center; padding: 15px;">
                    <button type="button" class="btn btn-primary antoclose2" id="exit_btn" style="margin-top: 10px">出站</button>
                    <button type="button" class="btn btn-primary antoclose2" id="save_btn" style="margin-top: 10px">儲存</button>
                    <button type="button" class="btn btn-default antoclose2" data-dismiss="modal" style="margin-top: 10px">退出作業</button>
                </div>
            </div>
        </div>
    </div>

    <!--填寫暫停 model-->
    <div id="stop_reason" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header" style="text-align: center">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h1>
                        <b>暫停原因填寫
                        </b>
                    </h1>
                </div>
                <div class="modal-body">
                    <div id="testmodal2" style="padding: 5px 20px;">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="btn-group btn-group-justified">
                                        <b style="font-size: 25px">問題類型：</b>
                                        <asp:DropDownList ID="DropDownList_StopType" runat="server" Style="font-size: 25px" onchange="change_text()"></asp:DropDownList>
                                        <asp:Button ID="Button_jump" runat="server" Text="Button" OnClick="Button_jump_Click" Style="display: none" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="btn-group btn-group-justified">
                                        <b style="font-size: 25px">內容填寫 </b>
                                        <asp:TextBox ID="TextBox_content" runat="server" TextMode="MultiLine" Style="resize: none; width: 100%; height: 200px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <div style="text-align: center; padding: 15px;">
                    <asp:Button ID="Button_Save" runat="server" Text="Button" OnClick="Button_Save_Click" Style="display: none" />
                    <button type="button" class="btn btn-primary antoclose2" id="save_stop" style="margin-top: 10px">儲存</button>
                    <button type="button" class="btn btn-default antoclose2" data-dismiss="modal" style="margin-top: 10px">退出作業</button>
                </div>
            </div>

        </div>
    </div>

    <!--填寫不良 model-->
    <div id="nogood_reason" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header" style="text-align: center">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h1>
                        <b>不良原因填寫
                        </b>
                    </h1>
                </div>
                <div class="modal-body">
                    <div id="testmodal552" style="padding: 5px 20px;">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="btn-group btn-group-justified">
                                        <b style="font-size: 25px">不良類型：</b>
                                        <asp:DropDownList ID="DropDownList_bad" runat="server" Style="font-size: 25px; text-align: center"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="btn-group btn-group-justified">
                                        <b style="font-size: 25px">數量：</b>
                                        <asp:TextBox ID="TextBox_Qty" min="0" runat="server" TextMode="Number" Style="font-size: 25px; margin-left: 50px; width: 21%; text-align: center" Text="0"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="btn-group btn-group-justified">
                                        <b style="font-size: 25px">內容填寫 </b>
                                        <asp:TextBox ID="TextBox_bad" runat="server" TextMode="MultiLine" onkeyup="autogrow(this);" Style="overflow: hidden; resize: none; width: 100%; height: 100px;"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                            <b style="font-size: 25px">填寫歷程</b>
                            <button type="button" id="delbad" style="margin-left: 30px">刪除歷程</button>
                            <br />
                        </div>
                        <label id="Count_Use" style="display: none">1</label>
                        <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                            <table id="Table_bad" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                <thead>
                                    <tr style="background-color: white">
                                        <th style="text-align: center">填寫順序</th>
                                        <th style="text-align: center">類型</th>
                                        <th style="text-align: center">數量</th>
                                        <th style="text-align: center">內容</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <hr />
                <div style="text-align: center; padding: 15px;">
                    <button type="button" class="btn btn-primary antoclose2" onclick="save_baddt()" style="margin-top: 10px">寫入歷程</button>
                    <button type="button" class="btn btn-success antoclose2" data-dismiss="modal" onclick="save_txt()" style="margin-top: 10px">儲存</button>
                    <button type="button" class="btn btn-default antoclose2" data-dismiss="modal" style="margin-top: 10px">退出作業</button>
                </div>
            </div>

        </div>
    </div>

    <!--出站前預覽 model-->
    <div id="Preview_Results" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-lg2" style="margin-top: 180px; margin-left: 102px">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda8822" style="padding: 5px 20px;">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div id="div_result" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                                    <b style="font-size: 30px">出站前預覽</b>
                                    <hr style="border-bottom: 2px solid #E6E9ED; margin-top: 0px; margin-bottom: 0px" />
                                </div>
                                <br />
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <table id="Table_Result" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                        <thead>
                                            <tr style="background-color: white">
                                                <th style="text-align: center">工單號碼</th>
                                                <th style="text-align: center">設備名稱</th>
                                                <th style="text-align: center">品號</th>
                                                <th style="text-align: center">品名</th>
                                                <th style="text-align: center">預計產量</th>
                                                <th style="text-align: center">已生產量</th>
                                                <th style="text-align: center">未生產量</th>
                                                <th style="text-align: center">製程代號</th>
                                                <th style="text-align: center">製程名稱</th>
                                                <th style="text-align: center">維護數量</th>
                                                <th style="text-align: center">良品數量</th>
                                                <th style="text-align: center">不良數量</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                    <div class="col-xs-12 col-sm-12 col-md-2" style="margin-top: 20px">
                                        <button type="button" class="btn_red" data-dismiss="modal">取消</button>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-8" style="margin-top: 20px">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-2" style="margin-top: 20px">
                                        <button type="button" class="btn_green" id="Exit">工單出站</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="text-align: center; padding: 15px;">
                        <b style="color: white">s</b>


                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--數量回報的 model-->
    <div id="Report_Count" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-lg2" style="margin-top: 180px; margin-left: 102px">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda88722" style="padding: 5px 20px;">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div id="div_results" class="col-xs-12 col-sm-12 col-md-12" style="border-width: 3px; border-style: solid; border-color: #000; margin-bottom: 10px">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center">
                                    <b style="font-size: 30px">回報前預覽</b>
                                    <hr style="border-bottom: 2px solid #E6E9ED; margin-top: 0px; margin-bottom: 0px" />
                                </div>
                                <br />
                                <div class="col-xs-12 col-sm-12 col-md-12" style="margin-bottom: 20px">
                                    <table id="Table_Report" class="table table-ts table-bordered dt-responsive nowrap " cellspacing="0" width="100%">
                                        <thead>
                                            <tr style="background-color: white">
                                                <th style="text-align: center">工單號碼</th>
                                                <th style="text-align: center">設備名稱</th>
                                                <th style="text-align: center">品號</th>
                                                <th style="text-align: center">品名</th>
                                                <th style="text-align: center">預計產量</th>
                                                <th style="text-align: center">已生產量</th>
                                                <th style="text-align: center">未生產量</th>
                                                <th style="text-align: center">製程代號</th>
                                                <th style="text-align: center">製程名稱</th>
                                                <th style="text-align: center">維護數量</th>
                                                <th style="text-align: center">良品數量</th>
                                                <th style="text-align: center">不良數量</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                    <div class="col-xs-12 col-sm-12 col-md-2" style="margin-top: 20px">
                                        <button type="button" class="btn_red" data-dismiss="modal">取消</button>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-8" style="margin-top: 20px">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-2" style="margin-top: 20px">
                                        <button type="button" class="btn_green" id="Report">回報數量</button>
                                        <asp:Button ID="Button_Report" runat="server" Text="" OnClick="Button_Exit_Click" Style="display: none" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="text-align: center; padding: 15px;">
                        <b style="color: white">s</b>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%=Use_Javascript.Quote_Javascript() %>
    <link href="../../assets/vendors/Create_HtmlCode/jquery.dataTables.min.css" rel="stylesheet" />

    <script>
        //展開縮合div
        function hide_div() {
            var tablename = document.getElementById("table_information");
            var divname = document.getElementById("_select");
            var icon = document.getElementById("iconimage");


            if (tablename.className == 'col-md-9 col-sm-12 col-xs-12') {
                icon.className = 'fa fa-chevron-left';
                tablename.className = 'col-md-12 col-sm-12 col-xs-12';
                divname.style.display = 'none';

            }
            else {
                icon.className = "fa fa-chevron-right";
                tablename.className = 'col-md-9 col-sm-12 col-xs-12';
                divname.style.display = 'block';
            }

        }
        //點擊到ICON後，確保會進行事件
        $(".iconimage").on("click", function () {
            hide_div();
        });

        //隨著文字長度變更TEXTBOX長度
        function autogrow(textarea) {
            var adjustedHeight = textarea.clientHeight;
            adjustedHeight = Math.max(textarea.scrollHeight, adjustedHeight);
            if (adjustedHeight > textarea.clientHeight)
                textarea.style.height = adjustedHeight + 'px';
            if (textarea.value == '')
                textarea.style.height = 100 + 'px';
        }

        //產生表格的HTML碼
        create_tablecode_noshdrow('Enter_ReportView', '進站維護總覽', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        //工單號碼 機台名稱 本筆工單資訊 現時工作人員
        //group→群組 machine_show_name→機台名稱 status→當前工單狀態 Number→品號 order_number→工單號碼 machine_name→機台代號 now_infromation→當前工單資訊 now_staffs→目前加工人員 type→報工/維護 bad_information→不良資訊 ok_qty→維護數量 ng_qty→不良數量
        function Set_Information(group, machine_show_name, status, Number, order_number, machine_name, now_infromation, now_staffs, type, bad_information, ok_qty, ng_qty) {
            //寫入工單
            $('#<%=TextBox_Order.ClientID%>').val('' + order_number + '');
            //寫入機台
            $('#<%=TextBox_Machine.ClientID%>').val('' + machine_name + '');
            //寫入人員
            $('#<%=TextBox_Staffs.ClientID%>').val('' + now_staffs + '');
            //寫入料號
            $('#<%=TextBox_Number.ClientID%>').val('' + Number + '');
            //寫入機台名稱
            $('#<%=TextBox_Show.ClientID%>').val('' + machine_show_name + '');
            //設備群組
            $('#<%=TextBox_Group.ClientID%>').val('' + group + '');
            //存入狀態
            $('#<%=TextBox_Status.ClientID%>').val('' + status + '');

            //設定不良品數
            if (parseInt(ng_qty, 10) > 0) {
                $('#<%=TextBox_NoGood.ClientID%>').val('' + ng_qty + '');
                $('#<%=TextBox_badqty.ClientID%>').val('' + ng_qty + '');
            }
            else {
                $('#<%=TextBox_NoGood.ClientID%>').val('' + 0 + '');
                $('#<%=TextBox_badqty.ClientID%>').val('' + 0 + '');
            }

            //設定良品數
            if (parseInt(ok_qty, 10) > 0)
                $('#<%=TextBox_Good.ClientID%>').val('' + ok_qty + '');
            else
                $('#<%=TextBox_Good.ClientID%>').val('' + 0 + '');

            //清空不良項目表
            $('#Table_bad').dataTable().fnClearTable();

            //顯示當筆排成表格
            var now_list = now_infromation.replaceAll('*', ' ').split('^');
            var now_data = [];
            var nowtable = $('#Table_now').dataTable();
            nowtable.fnClearTable();
            if (now_list.length > 1) {
                for (i = 0; i < now_list.length - 1; i++)
                    now_data.push('<center>' + now_list[i] + '</center>');
                nowtable.fnAddData(now_data);
            }

            //將人員寫入下拉選單
            $('#<%=DropDownList_Staff.ClientID%>').empty();
            var nowlist = now_staffs.split('#');
            var ddl = document.getElementById('<%=DropDownList_Staff.ClientID%>');
            for (i = 0; i < nowlist.length - 1; i++) {
                var option = document.createElement("OPTION");
                option.value = nowlist[i];
                option.innerHTML = nowlist[i + 1];
                ddl.options.add(option);
                i++;
            }

            //依據狀態判斷按鈕顯示與否
            if (status == '暫停') {
                document.getElementById("Stop").style.display = 'none';
                document.getElementById("Action").style.display = 'initial';
                if (type == 'ERROR')
                    document.getElementById("Write").style.display = 'initial';
                else
                    document.getElementById("Write").style.display = 'none';
            }
            else {
                document.getElementById("Action").style.display = 'none';
                document.getElementById("Write").style.display = 'none';
                document.getElementById("Stop").style.display = 'initial';
            }

            //把不良陣列寫入DataTable
            //先分割
            var bad_list = bad_information.split('Ω');
            //判斷有幾筆的不良資料
            var bad_count = (bad_list.length - 1) / 3;

            //匯入表格內部
            var badtable = $('#Table_bad').dataTable();
            badtable.fnClearTable();
            if (bad_list.length > 1) {
                for (i = 0; i < bad_count; i++) {
                    var bad_data = [];
                    bad_data.push('<center>' + (i + 1) + '</center>');
                    bad_data.push('<center>' + bad_list[i * 3] + '</center>');
                    bad_data.push('<center>' + bad_list[i * 3 + 1] + '</center>');
                    bad_data.push('<center>' + bad_list[i * 3 + 2] + '</center>');
                    badtable.fnAddData(bad_data);
                }
                document.getElementById("Count_Use").innerHTML = bad_count + 1;
            }

            if (status != '入站') {
                document.getElementById("change_staff").disabled = true;
                document.getElementById("report_count").disabled = true;
                document.getElementById("bad_reason").disabled = true;
                document.getElementById("Pre_Exit").style.display = 'none';
            }
            else {
                document.getElementById("change_staff").disabled = false;
                document.getElementById("report_count").disabled = false;
                document.getElementById("bad_reason").disabled = false;
                document.getElementById("Pre_Exit").style.display = 'inline';
            }

        }

        //將人員填入表格內(避免出入站有BUG)
        function Set_StaffTable() {
            var staffs = document.getElementById("<%=TextBox_Staffs.ClientID%>").value;
            var nowlist = staffs.split('#');
            //將目前所有人員填入資料表內
            var nowtable = $('#Table_person').dataTable();
            nowtable.fnClearTable();
            var rows = (nowlist.length - 1) / 2;
            var now_data = [];
            for (j = 0; j < rows; j++) {
                var x = j * 2;
                for (i = x; i < nowlist.length - 1; i++) {
                    now_data.push('<center>' + nowlist[i] + '</center>');
                    if (i >= (j + 1) * 2 - 1)
                        break;
                }
                //把row加入DATATABLE
                nowtable.fnAddData(now_data);
                now_data = [];
            }
        }

        //觸發事件
        $(document).ready(function () {
            //變更搜尋控制項內容
            //清空搜尋內容
            //var div_content = document.getElementById('datatable-buttons_filter');
            //div_content.innerHTML = '';
            ////填入搜尋控制項
            //div_content.innerHTML = '<label>請輸入機台:<input type="text" class="form-control input-sm" placeholder="" id="machine" name="machine"></label>';

            var table = $('#datatable-buttons').DataTable();

            //修正上下頁的CSS格式
            $('#machine').keyup(function () {
                table.draw();
               // $(".dataTables_filter").parent(".pull-left").css("margin-left", "-120px");

                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });
            //修正上下頁的CSS格式
            //$(".dataTables_filter").parent(".pull-left").css("margin-left", "-120px");
            $(".paginate_button ").css("min-width", "0em");
            $(".paginate_button ").css("padding", "0em 0em");
            $(".paginate_button ").css("margin-left", "0px");
            $(".paginate_button ").css("display", "initial");

            //選取想刪除之人員
            var Table_Staff = $('#Table_person').DataTable();
            $('#Table_person tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    Table_Staff.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });

            //人員出站
            $('#exit_btn').click(function () {
                //將選取人員填入TEXTBOX內
                var save_staff = '';
                var tablevalues = [];
                //選取的row
                tablevalues = ($('#Table_person').DataTable().rows('.selected').data());

                //此處只有單一行，用0即可
                if (tablevalues.length == 0) {
                    alert('請選擇人員出站');
                } else {
                    var tablelist = tablevalues[0];
                    for (var i = 0; i < tablelist.length; i++) {
                        save_staff += tablelist[i] + '#';
                    }
                    $('#<%=TextBox_Staff.ClientID%>').val('' + save_staff.replaceAll('</center>', '').replaceAll('<center>', '') + '');

                    //移除該人員
                    Table_Staff.row('.selected').remove().draw(false);

                    //以webservice呼叫進行儲存的動作(避免資料消失)
                    var orders = document.getElementById('<%=TextBox_Order.ClientID%>').value;
                    var machines = document.getElementById('<%=TextBox_Machine.ClientID%>').value;
                    var number = document.getElementById('<%=TextBox_Number.ClientID%>').value;
                    staff_Exit(orders, machines, save_staff.replaceAll('</center>', '').replaceAll('<center>', ''), number);
                }
            });

            //入站人員儲存
            $('#save_btn').click(function () {
                //把目前所有人員存入TEXTBOX內
                var save_staff = '';
                var tablevalues = [];
                tablevalues = ($('#Table_person').DataTable().rows().data());

                for (var j = 0; j < tablevalues.length; j++) {
                    //此處有多行，需跑迴圈
                    var tablelist = tablevalues[j];
                    for (var i = 0; i < tablelist.length; i++) {
                        save_staff += tablelist[i] + '#';
                    }
                }

                $('#<%=TextBox_Staff.ClientID%>').val('' + save_staff.replaceAll('</center>', '').replaceAll('<center>', '') + '');

                //以webservice呼叫進行儲存的動作(避免資料消失)
                var orders = document.getElementById('<%=TextBox_Order.ClientID%>').value;
                var machines = document.getElementById('<%=TextBox_Machine.ClientID%>').value;
                var number = document.getElementById('<%=TextBox_Number.ClientID%>').value;
                staff_Save(orders, machines, save_staff.replaceAll('</center>', '').replaceAll('<center>', ''), number);


                //儲存資料後，關閉人員的modal
                $('#staff_change').modal('hide');
            });

            //工單出站
            $('#Exit').click(function () {
                var count = 0;
                count = parseInt(document.getElementById("<%=TextBox_Good.ClientID%>").value, 10) + parseInt(document.getElementById("<%=TextBox_NowCount.ClientID%>").value, 10);
                document.getElementById("<%=TextBox_Good.ClientID%>").value = count;

                //先檢查狀態
                var now_status = document.getElementById("<%=TextBox_Status.ClientID%>").value;
                var badqty = document.getElementById("<%=TextBox_NoGood.ClientID%>").value;

                //狀態為暫停時，限制其出站
                if (now_status == '暫停')
                    alert('請先取消暫停，才可以出站!!');
                else {
                    //填入不良品數量後，由後端進行分配動作 
                    $('#<%=TextBox_badqty.ClientID %>').val('' + badqty + '');
                    document.getElementById('<%=Button_Exit.ClientID %>').click();
                }

            });

            //暫停
            $('#Stop').click(function () {
                //下拉選單回復第一個
                $("select#<%=DropDownList_StopType.ClientID%>").prop('selectedIndex', 0);
                //清空TextBOX
                document.getElementById("<%=TextBox_content.ClientID%>").value = '';
            });

            //儲存暫停原因跟類型
            $('#save_stop').click(function () {
                document.getElementById('<%=Button_Save.ClientID %>').click();
            });

            //異常填寫
            $('#Write').click(function () {
                document.getElementById('<%=Button_jump.ClientID %>').click();
            });

            //數量填寫
            $('#Report').click(function () {
                var count = 0;
                count = parseInt(document.getElementById("<%=TextBox_Good.ClientID%>").value, 10) + parseInt(document.getElementById("<%=TextBox_NowCount.ClientID%>").value, 10);
                document.getElementById("<%=TextBox_Good.ClientID%>").value = count;
                save_txt();

                var badqty = document.getElementById("<%=TextBox_NoGood.ClientID%>").value;

                $('#<%=TextBox_badqty.ClientID %>').val('' + badqty + '');
                document.getElementById('<%=Button_Report.ClientID %>').click();
            });

            //搜尋動作，必定填入選廠區與群組
            $('#btncheck').click(function () {
                var type = document.getElementById('<%=DropDownList_MachType.ClientID %>');
                var group = document.getElementById('<%=DropDownList_MachGroup.ClientID %>');
                var grouptext = '';

                $('#<%=TextBox_MachTypeText.ClientID %>').val('' + type.options[type.selectedIndex].text + '');
                $('#<%=TextBox_MachTypeValue.ClientID %>').val('' + type.value + '');
                try {
                    $('#<%=TextBox_MachGroupText.ClientID %>').val('' + group.options[group.selectedIndex].text + '');
                    $('#<%=TextBox_MachGroupValue.ClientID %>').val('' + group.value + '');
                    grouptext = group.options[group.selectedIndex].text;
                }
                catch {

                }
                if (grouptext != '' && grouptext != '--Select--') {
                    $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                    document.getElementById('<%=Button_Check.ClientID %>').click();
                }
                else
                    alert('請選擇群組');
            });

            //取消暫停
            $('#Action').click(function () {
                document.getElementById('<%=Button_Cancel.ClientID %>').click();
            });

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

            //前頁CSS變更
            $('#datatable-buttons_previous').click(function () {
                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });

            //次頁CSS變更
            $('#datatable-buttons_next').click(function () {
                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });
            //數字CSS變更
            $('#datatable-buttons_paginate').click(function () {
                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });

            //選取想刪除之不良項目
            var Table_bad = $('#Table_bad').DataTable();
            $('#Table_bad tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    Table_bad.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });

            //刪除不良項目
            $('#delbad').click(function () {
                //選取的row
                tablevalues = ($('#Table_bad').DataTable().rows('.selected').data());
                if (tablevalues.length > 0) {

                    //移除該不良項目
                    Table_bad.row('.selected').remove().draw(false);
                    //確認不會造成ID重疊
                    if (parseInt(document.getElementById("Count_Use").innerHTML, 10) == parseInt(tablevalues[0].replaceAll('</center>', '').replaceAll('<center>', ''), 10))
                        document.getElementById("Count_Use").innerHTML = parseInt(document.getElementById("Count_Use").innerHTML, 10) - 1;
                }
                else
                    alert('未選取刪除之項目');
            });
        });

        //只搜尋機台名稱，並顯示包含其字串機台(可惜會影響到其他表單)
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var machine = $('#machine').val();
                var tharray = [];
                //取得欄位名稱及順序
                $('#tr_row > th').each(function () {
                    tharray.push($(this).text())
                })
                var machinename = data[tharray.indexOf('設備名稱')]; // 要抓取浮動欄位(因為欄位可以左右移動)
                var nowinformation = data[0];//避免人員的資料表被影響
                var machineinformation = data[1];//避免預覽的資料表被影響

                if (machine == undefined)//第一次剛產生讀不到，所以為undefined
                    return true;
                else if (machine == '')//第二次產生後，讀到的值為空白
                    return true;
                else if (machinename == undefined)//避免人員資料表異常
                    return true;
                else if (machinename == '')//避免人員資料表異常
                    return true;
                else if (machinename == machine)//若字串包含，則顯示相關內容
                    return true;
                else if (nowinformation == machine)//若字串包含，則顯示相關內容
                    return true;
                else if (machineinformation == machine)//若字串包含，則顯示相關內容
                    return true;
                else
                    return false;
            }
        );

        //跳頁事件，當選擇之項目為ERROR時，跳頁至異常回復
        function change_text() {
            //取得當前下拉選單的值
            var ddl_value = document.getElementById("<%=DropDownList_StopType.ClientID%>").value;
            //依據值做後續事件判斷
            if (ddl_value == 'ERROR') {
                var message = confirm('即將進入異常填寫，請確認是否進入??')
                if (message) {
                    document.getElementById('<%=Button_jump.ClientID %>').click();
                }
            }
        }

        //透過API新增人員
        //tablename→表格ID cleartext→textboxid
        function add_staff(tablename, cleartext) {
            //取得輸入之機台
            id = document.getElementById(cleartext.replaceAll('#', '')).value;
            //取得API網址
            //取得新增之Table
            var nowtable = $(tablename).dataTable();
            var str_list;
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

        //表格閃爍機制
        var mTimer;
        var Timer_Count = 0;//閃爍機制
        var loadTime = <%=Refresh_Time%>;
        mTimer = setTimeout(function () { GetMachineData(); }, loadTime);

        //更新最外層的DataTable
        function GetMachineData() {
            clearTimeout(mTimer);
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/GetMachineList",
                data: { machine: '<%=mach %>', type: '進站維護' },
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            $(this).children().each(function (j) {
                                var btn = $(this).attr("工單報工").valueOf();
                                var group = $(this).attr("設備群組").valueOf();
                                var name = $(this).attr("設備名稱").valueOf();
                                var name_number = $(this).attr("設備代號").valueOf();
                                var manu_id = $(this).attr("工單號碼").valueOf();
                                var product_number = $(this).attr("品號").valueOf();
                                var product_name = $(this).attr("品名").valueOf();
                                var custom_name = $(this).attr("客戶名稱").valueOf();
                                var exp_count = $(this).attr("預計產量").valueOf();
                                var now_count = $(this).attr("當下產量").valueOf();
                                var yet_count = $(this).attr("未生產量").valueOf();
                                var process_number = $(this).attr("製程代號").valueOf();
                                var process_name = $(this).attr("製程名稱").valueOf();
                                var status = $(this).attr("工單狀態").valueOf();
                                var workstaff = $(this).attr("人員名稱").valueOf();
                                var now_time = $(this).attr("開工時間").valueOf();
                                var persent = $(this).attr("進度").valueOf();
                                var todayproduct = $(this).attr("已生產量").valueOf();

                                var tablearray = ['工單維護', btn,
                                    '設備群組', group,
                                    '設備名稱', name,
                                    '工單號碼', manu_id,
                                    '品號', product_number,
                                    '品名', product_name,
                                    '客戶名稱', custom_name,
                                    '預計產量', exp_count,
                                    '當下產量', now_count,
                                    '未生產量', yet_count,
                                    '製程代號', process_number,
                                    '製程名稱', process_name,
                                    '工單狀態', status,
                                    '人員名稱', workstaff,
                                    '已生產量', todayproduct,
                                    '進度', persent,
                                    '開工時間', now_time]

                                //取得目前欄位排序順序
                                var tharray = [];
                                $('#tr_row > th').each(function () {
                                    tharray.push($(this).text())
                                })

                                for (var j = 0; j < tharray.length; j++) {
                                    var td_column = name_number + '_' + tharray[j] + '_' + manu_id;
                                    var td_value = tablearray[tablearray.indexOf(tharray[j]) + 1];

                                    if (tharray[j] == '工單維護') {
                                        try {
                                            document.getElementById(td_column).innerHTML = check_value(td_value);
                                        }
                                        catch {

                                        }
                                    }
                                    else {
                                        try {
                                            if (tharray[j] == '預計產量' || tharray[j] == '已生產量' || tharray[j] == '未生產量')
                                                document.getElementById(td_column).innerHTML = check_tdvalue(td_value);
                                            else
                                                document.getElementById(td_column).innerHTML = check_value(td_value);
                                        }
                                        catch {

                                        }
                                    }
                                }

                                //非數字欄位空白補換行
                                function check_value(value) {
                                    if (value == '')
                                        value = '\n';
                                    return value;
                                }

                                //數字欄位空白補0
                                function check_tdvalue(value) {
                                    if (value.length == '0' || value.length == 0)
                                        value = '0';
                                    return value;
                                }
                            })
                        }
                    });
                },
                error: function (data, errorThrown) {

                }
            });
            mTimer = setTimeout(function () { GetMachineData(); }, loadTime);
            Timer_Count++;
        }

        //用WEBSERVICE儲存  避免資料不見
        //人員出站
        function staff_Exit(order, machine, staff, number) {
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/Staff_Exit",
                data: { order: order, machine: machine, staff: staff, product_number: number, type: '進站維護' },
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            var staffs = $(this).attr("人員清單").valueOf();
                            var workstaff = $(this).attr("人員列表").valueOf();
                            var Table = $('#Table_now').dataTable();
                            Table.fnClearTable();
                            $(this).children().each(function (j) {
                                addData = [];
                                addData.push($(this).attr("設備名稱").valueOf());
                                addData.push($(this).attr("工單號碼").valueOf());
                                addData.push($(this).attr("品號").valueOf());
                                addData.push($(this).attr("品名").valueOf());
                                addData.push($(this).attr("預計產量").valueOf());
                                addData.push($(this).attr("已生產量").valueOf());
                                addData.push($(this).attr("今日產量").valueOf());
                                addData.push($(this).attr("未生產量").valueOf());
                                addData.push($(this).attr("進度").valueOf());
                                addData.push($(this).attr("開工時間").valueOf());
                                addData.push($(this).attr("製程名稱").valueOf());
                                addData.push($(this).attr("人員名稱").valueOf());
                                Table.fnAddData(addData);
                            })

                            $('#<%=TextBox_Staffs.ClientID%>').val('' + staffs + '');


                            //將人員寫入下拉選單
                            $('#<%=DropDownList_Staff.ClientID%>').empty();
                            var nowlist = workstaff.split('#');
                            var ddl = document.getElementById('<%=DropDownList_Staff.ClientID%>');
                            for (i = 0; i < nowlist.length - 1; i++) {
                                var option = document.createElement("OPTION");
                                option.value = nowlist[i];
                                option.innerHTML = nowlist[i];
                                ddl.options.add(option);
                            }

                            GetMachineData();
                        }
                    });
                },
                error: function (data, errorThrown) {

                }
            });
        }

        //新增人員入站
        function staff_Save(order, machine, staff, number) {
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/Staff_Save",
                data: { order: order, machine: machine, staff: staff, product_number: number, type: '進站維護' },
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            var staffs = $(this).attr("人員清單").valueOf();
                            var workstaff = $(this).attr("人員列表").valueOf();
                            var Table = $('#Table_now').dataTable();
                            Table.fnClearTable();
                            $(this).children().each(function (j) {
                                addData = [];
                                addData.push($(this).attr("設備名稱").valueOf());
                                addData.push($(this).attr("工單號碼").valueOf());
                                addData.push($(this).attr("品號").valueOf());
                                addData.push($(this).attr("品名").valueOf());
                                addData.push($(this).attr("預計產量").valueOf());
                                addData.push($(this).attr("已生產量").valueOf());
                                addData.push($(this).attr("今日產量").valueOf());
                                addData.push($(this).attr("未生產量").valueOf());
                                addData.push($(this).attr("進度").valueOf());
                                addData.push($(this).attr("開工時間").valueOf());
                                addData.push($(this).attr("製程名稱").valueOf());
                                addData.push($(this).attr("人員名稱").valueOf());
                                Table.fnAddData(addData);
                            })

                            $('#<%=TextBox_Staffs.ClientID%>').val('' + staffs + '');


                            //將人員寫入下拉選單
                            $('#<%=DropDownList_Staff.ClientID%>').empty();
                            var nowlist = workstaff.split('#');
                            var ddl = document.getElementById('<%=DropDownList_Staff.ClientID%>');
                            for (i = 0; i < nowlist.length - 1; i++) {
                                var option = document.createElement("OPTION");
                                option.value = nowlist[i];
                                option.innerHTML = nowlist[i];
                                ddl.options.add(option);
                            }
                            GetMachineData();
                        }
                    });
                },
                error: function (data, errorThrown) {

                }
            });

        }

        //依據X軸內容改變Y軸內容
        function dropdownlist_change(x, y) {
            $('#' + y).empty();
            var ddlist = document.getElementById(x);
            //取得當前X的值
            var now_value = ddlist.value;
            var nowlist = now_value.split(',');
            var ddl = document.getElementById(y);

            for (i = 0; i < nowlist.length - 1; i++) {
                var option = document.createElement("OPTION");
                option.innerHTML = nowlist[i];
                option.value = nowlist[i + 1];
                ddl.options.add(option);
                i++;
            }
            document.getElementById('div_machines').style.display = 'none';

        }

        //依據Y軸顯示機台
        function show_machines(y) {
            var machine_name = '';
            var ddlist = document.getElementById(y);
            //取得當前X的值
            var now_value = ddlist.value;
            var nowlist = now_value.split('^');
            var label = document.getElementById('machines');
            label.innerHTML = '';
            for (i = 1; i < nowlist.length - 1; i++) {
                if (nowlist[i] != '') {
                    label.innerHTML += nowlist[i] + '<br />';
                    machine_name += nowlist[i] + ',';
                }
            }
            $('#<%=TextBox_Machines.ClientID %>').val('' + machine_name + '');
            if (nowlist.length - 1 > 1)
                document.getElementById('div_machines').style.display = 'block';
        }

        //清空不良的相關彈跳視窗
        function clear_bad() {
            //下拉選單回復第一個
            $("select#<%=DropDownList_bad.ClientID%>").prop('selectedIndex', 0);
            //回復高度
            document.getElementById("<%=TextBox_bad.ClientID%>").style.height = 100 + 'px';
            //清空TextBOX
            document.getElementById("<%=TextBox_bad.ClientID%>").value = '';
        }

        //存入下方資料表
        function save_baddt() {
            var badtype = document.getElementById("<%=DropDownList_bad.ClientID%>").value;
            var badqty = document.getElementById("<%=TextBox_Qty.ClientID%>").value;
            var badcontent = document.getElementById("<%=TextBox_bad.ClientID%>").value;
            var count_use = document.getElementById("Count_Use").innerHTML;
            if (badtype == '' && badqty == '0')
                alert('請填寫原因及數量');
            else if (badtype == '')
                alert('請填寫原因');
            else if (badqty == '0')
                alert('請填寫數量');
            else {
                var Table = $('#Table_bad').dataTable();
                addData = [];
                addData.push('<center>' + count_use + '</center>');
                addData.push('<center>' + badtype + '</center>');
                addData.push('<center>' + badqty + '</center>');
                addData.push('<center>' + badcontent + '</center>');
                Table.fnAddData(addData);

                //下拉選單回復第一個
                $("select#<%=DropDownList_bad.ClientID%>").prop('selectedIndex', 0);
                //回復高度
                document.getElementById("<%=TextBox_bad.ClientID%>").style.height = 100 + 'px';
                //清空TextBOX
                document.getElementById("<%=TextBox_bad.ClientID%>").value = '';
                document.getElementById("<%=TextBox_Qty.ClientID%>").value = '0';
                document.getElementById("Count_Use").innerHTML = parseInt(count_use, 10) + 1;
            }
        }

        //寫入數量與資訊
        function save_txt() {
            var count = 0;
            var tables = ($('#Table_bad').DataTable().rows().data());

            var allstaff = '';
            for (var i = 0; i < tables.length; i++) {
                var staff_list = tables[i];
                for (j = 0; j < staff_list.length; j++) {
                    if (j != 0) {
                        if (j == 2)
                            count += parseInt(staff_list[j].replaceAll('</center>', '').replaceAll('<center>', ''), 10);
                        allstaff += staff_list[j] + '#';
                    }
                }
            }
            $('#<%=TextBox_NoGood.ClientID%>').val('' + count + '');

            $('#<%=TextBox_BadInformation.ClientID%>').val('' + allstaff.replaceAll('</center>', '').replaceAll('<center>', '') + '');
        }

        //預覽結果
        function previous_result(tablename) {
            var count = 0;
            count = parseInt(document.getElementById("<%=TextBox_Good.ClientID%>").value, 10) + parseInt(document.getElementById("<%=TextBox_NowCount.ClientID%>").value, 10);

            var resultdt = $(tablename).dataTable();
            resultdt.fnClearTable();
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/Preview_Results",
                data: { machine: document.getElementById('<%=TextBox_Machine.ClientID%>').value, manu_id: document.getElementById('<%=TextBox_Order.ClientID%>').value, type_mode: '進站維護', good_qty: count, bad_qty: document.getElementById("<%=TextBox_NoGood.ClientID%>").value },

                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            $(this).children().each(function (j) {
                                var now_data = [];
                                now_data.push('<center>' + $(this).attr("工單號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("機台名稱").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("品號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("品名").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("預計產量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("已生產量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("未生產量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("製程代號").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("製程名稱").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("維護數量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("良品數量").valueOf() + '</center>');
                                now_data.push('<center>' + $(this).attr("不良數量").valueOf() + '</center>');

                                resultdt.fnAddData(now_data);
                            })
                        }

                    });
                },
                error: function (data, errorThrown) {
                    resultdt.fnClearTable();
                }
            });
        }


    </script>
</asp:Content>
