<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Error_Detail.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_CNC.Error_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>維護歷程 | 整廠進度管理看板</title>
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
                        <div style="font-size: 20px; margin-bottom: 8px">
                            <label class="control-label">工單號碼<b style="margin-left: 10px;"><asp:Label ID="Label_Order" runat="server" Text="Label"></asp:Label></b></label><br />
                            <label class="control-label">機台名稱<b style="margin-left: 10px;"><asp:Label ID="Label_ShowMachine" runat="server" Text="Label"></asp:Label></b></label><br />
                            <label class="control-label">工件品號<b style="margin-left: 10px;"><asp:Label ID="Label_Number" runat="server" Text="Label"></asp:Label></b></label><br />
                            <label class="control-label">工單人員<b style="margin-left: 10px;"><asp:Label ID="Label_Staff" runat="server" Text="Label"></asp:Label></b></label><br />
                            
                            <asp:Label ID="Label_Group" runat="server" Text="" Style="display: none"></asp:Label>
                            <asp:Label ID="Label_Machine" runat="server" Text="" Style="display: none"></asp:Label>
                            <asp:Label ID="Label_Type" runat="server" Text="" Style="display: none"></asp:Label>
                        </div>

                        <asp:PlaceHolder ID="PlaceHolder_hidden" runat="server">
                            <form id="demo-form2" class="form-horizontal form-label-left">
                                <div class="form-group">
                                    <label class="control-label " for="last-name">異常內容 </label>
                                    <div class="input-group col-xs-12">
                                        <asp:TextBox ID="MantStr" runat="server" TextMode="MultiLine" Width="100%" Height="150%" class="style1"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="form-group _type">
                                    <label class="control-label3 " for="Error-Type">異常類型</label>
                                    <asp:DropDownList ID="DropDownList_Errorfa" runat="server" class="form-control">
                                    </asp:DropDownList>
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
                                <!--待補-->
                                <%=th %>
                            </thead>
                            <tbody>
                                <!--待補-->
                                <%=tr %>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
    </div>

    <!----------------------------------------顯示圖片OR影片---------------------------------------------->
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
    <!----------------------------------------顯示圖片OR影片---------------------------------------------->

    <!----------------------------------------新增 OR 修改回復用---------------------------------------------->
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
                        <asp:TextBox ID="TextBox_Order" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Machine" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="TextBox_Group" runat="server" Style="display: none"></asp:TextBox>
                        <div class="form-group">
                            <div class="form-group">
                                <label class="control-label " for="last-name">異常內容 </label>
                                <div class="input-group col-xs-12">
                                    <asp:TextBox ID="TextContent" runat="server" TextMode="MultiLine" Width="100%" Height="150%" class="style1"></asp:TextBox>
                                    <span class="input-group-btn">
                                        <button id="button_select" type="button" class="btn btn-primary antosubmit2 ">新增</button>
                                        <asp:Button ID="btncheck" runat="server" class="btn btn-primary antosubmit2" OnClick="AddContent_Click" Style="display: none" />
                                    </span>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label3 " for="Error-Type">異常類型 </label>
                                <asp:DropDownList ID="DropDownList_ErrorChild" runat="server" class="form-control">
                                </asp:DropDownList>
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
                                        <asp:ListItem Selected="True" Value="處理">處理</asp:ListItem>
                                        <asp:ListItem Value="結案">結案</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div id="Case_Close" runat="server" style="display: none">
                                    <div class="form-group">
                                        <label class="control-label1 " for="Error-Type">結案說明 <span class="required">:</span></label>
                                        <div class="">
                                            <asp:TextBox ID="TextBox_Report" runat="server" TextMode="MultiLine" Width="100%" Height="150%" class="style1"></asp:TextBox>
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
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!----------------------------------------新增 OR 修改回復用---------------------------------------------->

    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //下拉選單改變
        function Change_Content() {
            if (document.getElementById("ContentPlaceHolder1_DropDownList_Status").value != "結案") {
                document.getElementById("ContentPlaceHolder1_Case_Close").style.display = "none";
            }
            else {
                document.getElementById("ContentPlaceHolder1_Case_Close").style.display = "block";
            }
        }
        var remind = document.getElementById("word");
        //OK
        function Get_ErrorDetails(ID, order, machine, group, update_man, account, power, type) {
            remind.innerHTML = "回覆內容";
            document.getElementById("button_select").innerHTML = '新增';
            //回復表格清空
            $('#<%=TextContent.ClientID%>').val('' + '' + '');
            //結案表格清空
            $('#<%=TextBox_Report.ClientID%>').val('' + '' + '');
            //填入工單
            $('#<%=TextBox_Order.ClientID%>').val('' + order + '');
            //填入機台
            $('#<%=TextBox_Machine.ClientID%>').val('' + machine + '');
            //填入群組
            $('#<%=TextBox_Group.ClientID%>').val('' + group + '');
            //填入使用者
            $('#<%=TextBox_acc.ClientID%>').val('' + account + '');
            //填入ID
            $('#<%=TextBox_textTemp.ClientID%>').val('' + ID + '');

            document.getElementById("<%=DropDownList_Status.ClientID%>").value = '處理';

            if (account == update_man || power == 'Y')
                document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = false;
            else
                document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = true;
            document.getElementById("<%=Case_Close.ClientID%>").style.display = "none";
            document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value = type;
        }

        //更新時使用
        function updates(ID, order, machine, group, Error_Content, Error_Type, Error_File, Status, Close_Content, Close_File, judge) {
            remind.innerHTML = "修改內容";
            document.getElementById("button_select").innerHTML = "修改";
            document.getElementById("<%=TextBox_textTemp.ClientID%>").value = '';
            document.getElementById("<%=TextBox_Order.ClientID%>").value = '';
            document.getElementById("<%=TextBox_Machine.ClientID%>").value = '';
            document.getElementById("<%=TextBox_Group.ClientID%>").value = '';

            $('#<%=TextBox_textTemp.ClientID%>').val('' + ID + '');
            $('#<%=TextBox_Order.ClientID%>').val('' + order + '');
            //填入機台
            $('#<%=TextBox_Machine.ClientID%>').val('' + machine + '');
            //填入群組
            $('#<%=TextBox_Group.ClientID%>').val('' + group + '');

            $('#<%=TextContent.ClientID%>').val('' + Error_Content.replaceAll("^", "'").replaceAll('#', '"').replaceAll("$", " ").replaceAll('@', '\r\n') + '');
            create_item(Error_File, 'Deal_File', 'Deal');

            //該問題尚未結束->可編輯父項目跟子項目
            if (Status == '') {
                document.getElementById("<%=DropDownList_ErrorChild.ClientID%>").value = Error_Type;
                document.getElementById("<%=DropDownList_Status.ClientID%>").value = '處理';
                document.getElementById("<%=Case_Close.ClientID%>").style.display = "none";
                //子項目
                if (judge == '1') {
                    if (Status == '')
                        document.getElementById("<%=DropDownList_Status.ClientID%>").disabled = true;
                    else
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
        }

        //子項目儲存事件
        $("#button_select").click(function () {
            //處理中
            get_checkboxvalue('Deal', '#<%=TextBox_File.ClientID%>')
            //結案
            get_checkboxvalue('Close', '#<%=TextBox_Close.ClientID%>')
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('button_select').disabled = true;
            document.getElementById('<%=btncheck.ClientID %>').click();
        });

        //刪除時使用 OK
        function deletes(ID) {
            var r = confirm('確定要刪除嗎');
            if (r == true) {
                $('#<%=TextBox_num.ClientID%>').val('' + ID + '');
                document.getElementById('<%=bt_del.ClientID %>').click();
            }
        }

        //改變ICON的圖案 OK
        function Click_Num(ID) {
            if (document.getElementById(ID).className == "fa fa-folder-open-o")
                document.getElementById(ID).className = "fa fa-folder-o";
            else
                document.getElementById(ID).className = "fa fa-folder-open-o";
        }

        //父項目儲存事件 OK
        $("#Mant_Btn").click(function () {
            if (document.getElementById("<%=DropDownList_Errorfa.ClientID%>").value == '') {
                alert('請選擇類型');
            }
            else {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                document.getElementById('button_select').disabled = true;
                document.getElementById('<%=Button_Save.ClientID %>').click();
            }

        });
    </script>
    <!-----------------20200424留言板功能------------------------------------->
</asp:Content>
