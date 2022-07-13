<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_ErrorDetail.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.Asm_ErrorDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=LineName %> 維護歷程 | 整廠進度管理看板</title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_ErrorDetail.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-----------------content------------------>
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>
            <li><u><a href="Asm_LineTotalView.aspx">整廠進度管理看板</a></u></li>
            <li><u><a href="Asm_LineOverView.aspx?key=<%=UrlLink %>"><%=LineName %></a></u></li>
            <%=go_back %>
        </ol>
        <br />
        <div class="clearfix"></div>
        <!-----------------/title------------------>

        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="x_title ">
                        <!-- top tiles -->
                        <div class="row">
                            <h1 class="text-center _mdTitle" style="width: 100%"><b>異常歷程</b></h1>
                            <h3 class="text-center _xsTitle" style="width: 100%"><b>異常歷程</b></h3>
                        </div>
                        <!-- /top tiles -->
                        <div class="clearfix"></div>
                    </div>
                    <div class="x_content _select _setborder">
                        <label class="control-label">製令編號<b style="margin-left: 10px;"><%=Key %></b></label>
                        <asp:PlaceHolder ID="PlaceHolder_hidden" runat="server">
                            <form id="demo-form2" class="form-horizontal form-label-left">
                                <div class="form-group">
                                    <label class="control-label " for="last-name">異常內容 </label>
                                    <div class="input-group col-xs-12">
                                        <asp:TextBox ID="MantStr" runat="server" TextMode="MultiLine" onkeyup="autogrow(this);" Width="100%" Height="150%" class="style1"  style="overflow: hidden;"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="form-group _type">
                                    <label class="control-label3 " for="Error-Type">異常類型</label>
                                    <asp:DropDownList ID="DropDownList_Errorfa" runat="server" class="form-control">
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group _type">
                                    <label class="control-label3 " for="Error-Type">責任單位</label>
                                    <asp:DropDownList ID="DropDownList_respone" runat="server" class="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="form-group _line" style="display: none">
                                    <label class="control-label3 " for="Error-Type">是否持續顯示異警</label>
                                    <asp:RadioButtonList ID="RadioButtonList_show" runat="server" CssClass="table-striped" RepeatColumns="2">
                                        <asp:ListItem Selected="True" Value="1">是&nbsp&nbsp&nbsp&nbsp</asp:ListItem>
                                        <asp:ListItem Value="0">否</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>

                                <div class="form-group _line">
                                    <label class="control-label3 " for="Error-Type">發佈到LINE</label>
                                    <asp:RadioButtonList ID="RadioButtonList_Post" runat="server" CssClass="table-striped" RepeatColumns="2">
                                        <asp:ListItem Selected="True" Value="1">是&nbsp&nbsp&nbsp&nbsp</asp:ListItem>
                                        <asp:ListItem Value="0">否</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="form-group _UpdateImg">
                                    <label class="control-label3 " for="Error-Type">上傳圖片(可多張上傳)<span class="required"></span></label>
                                    <asp:FileUpload ID="FileUpload_image" runat="server" AllowMultiple="True" />
                                </div>
                                <div class="form-group _button col-12 col-md-1 col-md-offset-11">
                                    <asp:Button ID="Button_Save" runat="server" class="btn btn-primary antosubmit2" OnClick="Unnamed_ServerClick" Style="display: none" />
                                    <button id="Mant_Btn" type="button" class="btn btn-primary antosubmit2" style="width: 100%">新增</button>
                                </div>
                            </form>
                        </asp:PlaceHolder>
                        <div class="clearfix"></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="clearfix"></div>
        <div id="_InformationDetail" class="row">
            <asp:TextBox ID="TextBox_num" runat="server" Style="display: none"></asp:TextBox>
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="x_title">
                        <ul class="nav navbar-right panel_toolbox">
                            <li>
                                <input id="bt_del" type="button" class="btn btn-danger" name="table1_bt_del" runat="server" onserverclick="bt_del_ServerClick" value="刪除" style="display: none" />
                            </li>
                        </ul>
                        <div class="clearfix"></div>
                    </div>
                    <div class="x_content">

                        <table id="datatable-checkbox0" class="table table-bordered bulk_action StandardTable" cellspacing="0" width="100%">
                            <thead>
                                <%=ColumnsData%>
                            </thead>
                            <tbody>
                                <%=RowsDataArray[0]%>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
    </div>
    <!-----------------顯示圖片OR影片---------------------------------------------->

    <div id="exampleModal_Image" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                </div>
                <div class="modal-body">
                    <div id="testmodal25" style="text-align: center;">
                        <label id="lbltipAddedComment">test</label>
                    </div>
                </div>
                <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>


    <!--新增 OR 修改回復用-->
    <div id="exampleModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: center">
                            <h1 class="text-center" style="width: 100%" id="word"><b></b></h1>
                        </div>
                    </div>
                </div>
                <div class="modal-body">
                    <div id="testmodal2">
                        <asp:TextBox ID="TextBox_textTemp" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_ErrorID" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_acc" runat="server" Style="display: none"></asp:TextBox>

                        <div class="form-group">


                            <div class="form-group" runat="server" id="error_content">
                                <label class="control-label " for="last-name">異常內容 </label>
                                <div class="input-group col-xs-12">
                                    <asp:TextBox ID="TextContent" runat="server" TextMode="MultiLine" onkeyup="autogrow(this);" Width="100%" Height="150%" class="style1"  style="overflow: hidden;"></asp:TextBox>
                                    <span class="input-group-btn">
                                        <button id="button_select" type="button" class="btn btn-primary antosubmit2 ">新增</button>
                                        <asp:Button ID="btncheck" runat="server" class="btn btn-primary antosubmit2" OnClick="AddContent_Click" Style="display: none"  />
                                    </span>
                                </div>
                            </div>


                            <div class="form-group">
                                <label class="control-label3 " for="Error-Type">異常類型 </label>
                                <asp:DropDownList ID="DropDownList_ErrorChild" runat="server" class="form-control">
                                </asp:DropDownList>
                            </div>

                            <div class="form-group _type">
                                <label class="control-label3 " for="Error-Type">責任單位</label>
                                <asp:DropDownList ID="DropDownList_responechild" runat="server" class="form-control">
                                </asp:DropDownList>
                            </div>

                            <div class="form-group _line" style="display: none">
                                <label class="control-label3 " for="Error-Type">是否持續顯示異警</label>
                                <asp:RadioButtonList ID="RadioButtonList_showchild" runat="server" CssClass="table-striped" RepeatColumns="2">
                                    <asp:ListItem Selected="True" Value="1">是&nbsp&nbsp&nbsp&nbsp</asp:ListItem>
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>

                            <div class="form-group _line">
                                <label class="control-label3 " for="Error-Type">發佈到LINE</label>
                                <asp:RadioButtonList ID="RadioButtonList_Upost" runat="server" CssClass="table-striped" RepeatColumns="2">
                                    <asp:ListItem Selected="True" Value="1">是&nbsp&nbsp&nbsp&nbsp</asp:ListItem>
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                            <asp:PlaceHolder ID="PlaceHolder_image" runat="server">
                                <div class="form-group">
                                    <label class="control-label3 " for="Error-Type">上傳圖片(可多張上傳)<span class="required">:</span></label>
                                    <asp:FileUpload ID="FileUpload_Content" runat="server" AllowMultiple="True" />
                                </div>
                                <asp:TextBox ID="TextBox_File" runat="server" Style="display: none"></asp:TextBox>
                                <ul id="Deal_File">
                                </ul>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <label class="control-label3 " for="Error-Type">處理狀態<span class="required">:</span></label>
                                <div class="">
                                    <asp:DropDownList ID="DropDownList_Status" runat="server" class="form-control" onchange="Change_Content()">
                                    </asp:DropDownList>
                                </div>

                                <div id="Case_Close" runat="server" style="display: none">
                                    <div class="form-group">
                                        <label class="control-label1 " for="Error-Type">結案說明 <span class="required">:</span></label>
                                        <div class="">
                                            <asp:TextBox ID="TextBox_Report" runat="server" onkeyup="autogrow(this);" TextMode="MultiLine" Width="100%" Height="150%" class="style1" style="overflow: hidden;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="control-label1 " for="Error-Type">結案附檔 <span class="required">:</span></label>
                                        <div class="">
                                            <asp:FileUpload ID="FileUpload_Close" runat="server" AllowMultiple="True" />
                                        </div>
                                        <asp:TextBox ID="TextBox_Close" runat="server" Style="display: none"></asp:TextBox>
                                        <ul id="Close_File">
                                        </ul>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="modal-footer">
                        <button id="button_close" type="button" class="btn btn-primary antosubmit2 " style="display:none;float:right">新增</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-----------------20200424留言板功能---------------------------------------------->

    <%=Use_Javascript.Quote_Javascript() %>
    <!-----------------20200424留言板功能------------------------------------->
    <script>

        //隨著文字長度變更TEXTBOX長度
        function autogrow(textarea) {
            var adjustedHeight = textarea.clientHeight;
            adjustedHeight = Math.max(textarea.scrollHeight, adjustedHeight);
            if (adjustedHeight > textarea.clientHeight)
                textarea.style.height = adjustedHeight + 'px';
            if (textarea.value == '')
                textarea.style.height = 100 + 'px';
        }

        var remind = document.getElementById("word");
        //回覆時使用->類型帶入上一次填寫的類型，內容清空
        //ID->異常維護編號 Error_ID->排程編號 acc->建立者名稱 username->維護人員名稱 userpower->是否能夠結案 errortype->上次填寫的異常類型 responedep-> 上次填寫之責任部門 showerrorlight->上次填寫知是否閃燈
        function Get_ErrorDetails(ID, Error_ID, acc, username, userpower, errortype, responedep, showerrorlight) {
            remind.innerHTML = "回覆內容";
            document.getElementById("button_select").innerHTML = '新增';
            $('#<%=TextContent.ClientID%>').val('' + '' + '');
            $('#<%=TextBox_Report.ClientID%>').val('' + '' + '');
            $('#<%=TextBox_acc.ClientID%>').val('' + acc + '');
            $('#<%=TextBox_textTemp.ClientID%>').val('' + ID + '');
            $('#<%=TextBox_ErrorID.ClientID%>').val('' + Error_ID + '');
            document.getElementById("<%=DropDownList_Status.ClientID%>").value = '處理';
            if (acc == username || userpower == 'Y')
                document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = false;
            else
                document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = true;
            document.getElementById("<%=Case_Close.ClientID%>").style.display = "none";

            document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value = errortype;
            document.getElementById("<%=DropDownList_responechild.ClientID%>").value = responedep;

            var rb = document.getElementById("<%=RadioButtonList_showchild.ClientID%>");//Client ID of the radiolist
            var radio = rb.getElementsByTagName("input");
            if (showerrorlight == 'Y') {
                radio[0].checked = true;
                radio[1].checked = false;
            }
            else {
                radio[1].checked = true;
                radio[0].checked = false;
            }

        }
        //刪除時使用
        function deletes(ID) {
            var r = confirm('確定要刪除嗎');
            if (r == true) {
                $('#<%=TextBox_num.ClientID%>').val('' + ID + '');
                document.getElementById('<%=bt_del.ClientID %>').click();
            }
        }
        //更新時使用
        function updates(ID, Error_ID, Error_Content, Error_Type, Error_File, Status, Close_Content, Close_File, judge, responedep, showerrorlight) {
            remind.innerHTML = "修改內容";
            document.getElementById("<%=TextBox_textTemp.ClientID%>").value = '';
            document.getElementById("<%=TextBox_ErrorID.ClientID%>").value = '';
            $('#<%=TextBox_textTemp.ClientID%>').val('' + ID + '');
            $('#<%=TextBox_ErrorID.ClientID%>').val('' + Error_ID + '');

            $('#<%=TextContent.ClientID%>').val('' + Error_Content.replaceAll("^", "'").replaceAll('#', '"').replaceAll("$", " ").replaceAll('@', '\r\n') + '');
            create_item(Error_File, 'Deal_File', 'Deal');

            document.getElementById("<%=DropDownList_responechild.ClientID%>").value = responedep;

            var rb = document.getElementById("<%=RadioButtonList_showchild.ClientID%>");//Client ID of the radiolist
            var radio = rb.getElementsByTagName("input");
            if (showerrorlight == 'Y') {
                radio[0].checked = true;
                radio[1].checked = false;
            }
            else {
                radio[1].checked = true;
                radio[0].checked = false;
            }

            //該問題尚未結束->可編輯父項目跟子項目
            if (Status == '') {
                document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value = Error_Type;
                document.getElementById("<%=DropDownList_Status.ClientID%>").value = '處理';
                document.getElementById("<%=Case_Close.ClientID%>").style.display = "none";
                //子項目
                if (judge == '1') {
                    document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = false;
                }
                //父項目
                else {
                    document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = true;
                }
            }
            //已完成->可編輯結案內容
            else {
                document.getElementById("<%=DropDownList_Status.ClientID%>").value = '結案';
                document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value = Status;
                document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = false;
                $('#<%=TextBox_Report.ClientID%>').val('' + Close_Content.replaceAll("^", "'").replaceAll('#', '"').replaceAll("$", " ").replaceAll('@', '\r\n') + '');
                document.getElementById("<%=Case_Close.ClientID%>").style.display = "block";
                create_item(Close_File, 'Close_File', 'Close');
            }
            document.getElementById("button_select").innerHTML = "修改";
        }
        //改變ICON的圖案
        function Click_Num(ID) {
            if (document.getElementById(ID).className == "fa fa-folder-open-o")
                document.getElementById(ID).className = "fa fa-folder-o";
            else
                document.getElementById(ID).className = "fa fa-folder-open-o";
        }

        //子項目儲存事件 優先確認類型 部門 內容 結案內容是否填寫
        $("#button_select").click(function () {
            //處理中
            get_checkboxvalue('Deal', '#<%=TextBox_File.ClientID%>');
            //結案
            get_checkboxvalue('Close', '#<%=TextBox_Close.ClientID%>');

            //異常回復
            var txt = document.getElementById("<%=TextContent.ClientID%>").value;
            //異常類型
            var type = document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value;
            //責任單位
            var respone = document.getElementById("<%=DropDownList_responechild.ClientID%>").value;
            //處理/結案
            var nowstatus = document.getElementById("<%=DropDownList_Status.ClientID%>").value;
            //結案內容
            var closetxt = document.getElementById("<%=TextBox_Report.ClientID%>").value;


            if (nowstatus == '處理') {
                if (txt != '' && type != '' && respone != '') {
                    $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                    document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                    document.getElementById('button_select').disabled = true;
                    document.getElementById('<%=btncheck.ClientID %>').click();
                }
                else
                    alert('請確認 異常內容 異常類型 責任單位 是否確實填寫');
            }
            else if (nowstatus == '結案') {
                if (type != '' && respone != '' && closetxt != '') {
                    $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                    document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                    document.getElementById('button_select').disabled = true;
                    document.getElementById('<%=btncheck.ClientID %>').click();
                }
                else alert('請確認  異常類型 責任單位 結案內容 是否確實填寫');
            }
        });


        //子項目儲存事件 優先確認類型 部門 內容 結案內容是否填寫
        $("#button_close").click(function () {
            //處理中
            get_checkboxvalue('Deal', '#<%=TextBox_File.ClientID%>');
            //結案
            get_checkboxvalue('Close', '#<%=TextBox_Close.ClientID%>');

            //異常回復
            var txt = document.getElementById("<%=TextContent.ClientID%>").value;
            //異常類型
            var type = document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value;
            //責任單位
            var respone = document.getElementById("<%=DropDownList_responechild.ClientID%>").value;
            //處理/結案
            var nowstatus = document.getElementById("<%=DropDownList_Status.ClientID%>").value;
            //結案內容
            var closetxt = document.getElementById("<%=TextBox_Report.ClientID%>").value;


            if (nowstatus == '處理') {
                if (txt != '' && type != '' && respone != '') {
                    $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                    document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                    document.getElementById('button_close').disabled = true;
                    document.getElementById('<%=btncheck.ClientID %>').click();
                }
                else
                    alert('請確認 異常內容 異常類型 責任單位 是否確實填寫');
            }
            else if (nowstatus == '結案') {
                if (type != '' && respone != '' && closetxt != '') {
                    $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                    document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                    document.getElementById('button_close').disabled = true;
                    document.getElementById('<%=btncheck.ClientID %>').click();
                }
                else alert('請確認  異常類型 責任單位 結案內容 是否確實填寫');
            }
        });


        //父項目儲存事件 優先確認類型 部門 內容是否填寫
        $("#Mant_Btn").click(function () {
            var txt = document.getElementById("<%=MantStr.ClientID%>").value;
            var type = document.getElementById("<%=DropDownList_Errorfa.ClientID%>").value;
            var respone = document.getElementById("<%=DropDownList_respone.ClientID%>").value;

            if (txt != '' && type != '' && respone != '') {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                document.getElementById('button_select').disabled = true;
                document.getElementById('<%=Button_Save.ClientID %>').click();
            }
            else
                alert('請確認 異常內容 異常類型 責任單位 是否確實填寫');
        });


        //下拉選單改變
        function Change_Content() {
            if (document.getElementById("ContentPlaceHolder1_DropDownList_Status").value != "結案") {
                document.getElementById("ContentPlaceHolder1_Case_Close").style.display = "none";
                document.getElementById("button_close").style.display = "none";
                document.getElementById("ContentPlaceHolder1_error_content").style.display = "block";
            }
            else {
                document.getElementById("ContentPlaceHolder1_error_content").style.display = "none";
                document.getElementById("button_close").style.display = "block";
                document.getElementById("ContentPlaceHolder1_Case_Close").style.display = "block";
            }
        }


    </script>
    <!-----------------20200424留言板功能------------------------------------->
</asp:Content>
