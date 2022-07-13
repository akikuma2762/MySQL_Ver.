<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="rights_mag.aspx.cs" Inherits="dek_erpvis_v2.pages.sys_control.rights_mag" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%=color %>
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
    <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    <title>權限管理 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/index.css" rel="stylesheet" />
    <link href="../../Content/phone.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <style>
        /*隱藏權限複製*/
        #datatable-buttons thead tr th:nth-child(9) {
            display: none;
        }

        #datatable-buttons tbody tr td:nth-child(9) {
            display: none;
        }

        .Div_Shadow {
            box-shadow: 3px 3px 9px gray;
        }

        @media screen and (max-width:768px) {
            .Div_Shadow {
                box-shadow: none;
            }

            .page-title .title_left {
                width: 100%;
            }
        }

        .modal-dialog2 {
            width: 1200px;
            margin: 15% 30px 30px 20%;
        }

        .modal-dialog3 {
            margin: 15% 30px 30px 35%;
        }

        .modal-dialog3 {
            margin: 10% 30px 30px 35%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main">
        <div>
            <div class="page-title">
                <div class="title_left">
                    <h3>&nbsp 可視化權限管理</h3>
                </div>
                <asp:Button ID="Button_change" runat="server" Text="Button" OnClick="Button_change_Click" Style="display: none" />
                <asp:TextBox ID="TextBox_textTemp" runat="server" Style="display: none"></asp:TextBox>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="row">
            <asp:TextBox ID="TextBox_ID" runat="server" Style="display: none"></asp:TextBox>
            <asp:TextBox ID="TextBox_Act" runat="server" Style="display: none"></asp:TextBox>
            <asp:Button ID="Button_Act" runat="server" Text="Button" OnClick="Button_Act_Click" Style="display: none" />
             <asp:Button ID="Button_FLogout" runat="server" Text="Button" OnClick="Button_FLogout_Click" Style="display: none" />
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="x_title">
                        <h2>會員列表</h2>
                        <div style="position: absolute; right: 0">
                            <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#AddUsersModal">批次新增</button>
                            <button type="button" class="btn btn-info" data-toggle="modal" data-target="#AddUserModal">新增使用者</button>
                        </div>
                        <asp:Button ID="Button_Add" runat="server" Text="新增使用者" OnClick="Button_Add_Click" Style="display: none" />
                        <div class="clearfix"></div>
                    </div>
                    <div class="x_content">
                        <table id="datatable-buttons" class="table table-ts table-bordered nowrap" cellspacing="0" width="100%">
                            <thead>
                                <tr id="tr_row">
                                    <%=th%>
                                </tr>
                            </thead>
                            <tbody>
                                <%= set_table_content()%>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!--複製使用者權限-->
    <div id="exampleModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog2">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmodal34" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down" style="width: 100%; text-align: center"><b>權限複製</b></i>
                            </h5>

                            <hr />
                            <div class="row">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center; margin-bottom: 15px">
                                    <div class="col-xs-12 col-sm-12 col-md-4 " style="display: inline-block; text-align: right;">
                                        來源使用者
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-3 " style="display: inline-block; text-align: left;">
                                        <asp:TextBox ID="TextBox_Source" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-4 " style="display: inline-block; text-align: left;">
                                        <asp:DropDownList ID="DropDownList_Source" runat="server" onchange="set_textbox()"></asp:DropDownList>
                                        <asp:DropDownList ID="DropDownList_NoReset" runat="server" Style="display: none"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center; margin-bottom: 15px">
                                    <div class="col-xs-12 col-sm-12 col-md-4 " style="display: inline-block; text-align: right;">
                                        拷貝至
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-3 " style="display: inline-block; text-align: left;">
                                        <asp:TextBox ID="TextBox_TargetID" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-4 " style="display: inline-block; text-align: left;">
                                        <asp:TextBox ID="TextBox_TargetName" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center;">
                                    <div class="col-xs-12 col-sm-12 col-md-4 " style="display: inline-block; text-align: left;">
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-3 " style="display: inline-block; text-align: left;">
                                        <asp:CheckBox ID="CheckBox_SaveOriginal" runat="server" Text="保留原使用者權限" Checked="true" />
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-4 " style="display: inline-block; text-align: left;">
                                        <asp:CheckBox ID="CheckBox_HighPower" runat="server" Text="金額權限複製" Checked="true" />
                                    </div>
                                </div>
                            </div>

                            <hr />
                            <div style="text-align: center; padding: 15px;">
                                <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>
                                <button id="btncopy" type="button" class="btn btn-primary antosubmit2 ">複製</button>
                                <asp:Button ID="Button_copy" runat="server" Text="Button" OnClick="Button_copy_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--設定使用者群組-->
    <div id="GroupModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog3">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmodal234" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down" style="width: 100%; text-align: center"><b>選擇群組</b></i>
                            </h5>

                            <hr />
                            <div class="row" style="text-align: center">
                                <div class="col-xs-12 col-sm-12 col-md-12" style="text-align: center; margin-bottom: 15px">
                                    <asp:CheckBoxList ID="CheckBoxList_Group" runat="server" RepeatColumns="3"></asp:CheckBoxList>
                                </div>

                            </div>
                            <asp:TextBox ID="TextBox_temp" runat="server" Style="display: none"></asp:TextBox>
                            <asp:TextBox ID="TextBox_group" runat="server" Style="display: none"></asp:TextBox>
                            <hr />
                            <div style="text-align: center; padding: 15px;">
                                <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>
                                <button id="btnsave" type="button" class="btn btn-primary antosubmit2 ">儲存</button>
                                <asp:Button ID="Button_Save" runat="server" Text="Button" OnClick="Button_Save_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--批次新增-->
    <div id="AddUsersModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog3">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda0l234" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down" style="width: 100%; text-align: center"><b>批次新增</b></i>
                            </h5>

                            <hr />
                            <div class="row" style="text-align: center">
                                <a href="../File/格式Ver2.xlsx"><b>範例檔下載</b></a>
                            </div>
                            <hr />
                            <div class="row" style="text-align: center">
                                <div class="col-md-6">
                                    <b>檔案上傳</b>
                                </div>
                                <div class="col-md-6">
                                    <asp:FileUpload ID="FileUpload_File" runat="server" />
                                </div>
                            </div>

                            <hr />
                            <div style="text-align: center; padding: 15px;">
                                <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>
                                <button id="btnsaveuser" type="button" class="btn btn-primary antosubmit2 ">儲存</button>
                                <asp:Button ID="Button_AddUsers" runat="server" Text="批次新增" OnClick="Button_AddUsers_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--新增使用者-->
    <div id="AddUserModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog4">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmodal1234" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down" style="width: 100%; text-align: center"><b>建立VIS帳號</b></i>
                            </h5>

                            <hr />
                            <i id="_remind" style="font-size: x-large;"></i>
                            <div class="row" style="text-align: center; margin-bottom: 15px">
                                <div class="col-md-8 col-sm-8 col-xs-8 ">
                                    <asp:TextBox ID="TextBox_Name" runat="server" placeholder="*姓名" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-md-4 col-sm-4 col-xs-4 ">
                                    <asp:DropDownList ID="DropDownList_depart" runat="server" class="form-control">
                                    </asp:DropDownList>
                                </div>
                                <div id="factory" style="display: none">
                                    <div class="col-md-5 col-sm-5 col-xs-5 ">
                                        <asp:TextBox ID="TextBox1" runat="server" Style="text-align: center;" placeholder="廠區選擇" Height="34px" Font-Size="14px" Enabled="false" class="form-control" autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 ">
                                        <asp:DropDownList ID="DropDownList_Factory" runat="server" class="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-12 col-sm-12 col-xs-12 ">
                                    <asp:TextBox ID="TextBox_Acc" runat="server" placeholder="*申請帳號(不可包含['、-、*、,、.、?] 及中文字)" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-12 col-sm-12 col-xs-12 ">
                                    <asp:TextBox ID="TextBox_phone" runat="server" placeholder="手機號碼 ex.0911234567" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-12 col-sm-12 col-xs-12 ">
                                    <asp:TextBox ID="TextBox_email" runat="server" placeholder="信箱 ex.aaa@gmail.com" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-12 col-sm-12 col-xs-12 ">
                                    <asp:TextBox ID="TextBox_birth" runat="server" placeholder="請輸入生日" class="form-control" autocomplete="off" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-12 col-sm-12 col-xs-12 ">
                                    <asp:TextBox ID="TextBox_Pwd1" type="password" runat="server" placeholder="*登入密碼(不可包含['、-、*、,、.、?])" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-12 col-sm-12 col-xs-12 ">
                                    <asp:TextBox ID="TextBox_Pwd2" type="password" runat="server" placeholder="*再次輸入密碼" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row" style="margin-bottom: 15px">
                                <div class="col-md-4 col-sm-4 col-xs-4 ">
                                    <asp:Label ID="Label_code" runat="server" Text="" class="form-control"></asp:Label>
                                </div>
                                <div class="col-md-8 col-sm-8 col-xs-8 ">
                                    <asp:TextBox ID="TextBox_code" runat="server" placeholder="*請輸入左側驗證碼" class="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div style="text-align: center; padding: 15px;">
                                <button type="button" class="btn btn-default antoclose2" data-dismiss="modal">退出作業</button>
                                <button id="btnadd" type="button" class="btn btn-primary antosubmit2 ">建立帳號</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>

    <script>
        //防止切換頁籤時跑版
        $(document).ready(function () {
            $('#example').DataTable({
                responsive: true
            });
            $('#exampleInTab').DataTable({
                responsive: true
            });

            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                $($.fn.dataTable.tables(true)).DataTable()
                    .columns.adjust();
            });
        });

        function button_delete(ACC) {
            //管理頁面防呆
            answer = confirm("您確定要刪除嗎??");
            if (answer) {
                $.ajax({
                    type: 'POST',
                    dataType: 'xml',
                    url: "../../webservice/permission.asmx/delete_ACC",
                    data: 'key=' + ACC,
                    success: function (xml) {
                        $(xml).find("ROOT").each(function (i) {
                            alert($(this).attr("system_msg").valueOf());
                            location.reload();
                        });
                    },
                    error: function (data, errorThrown) {
                        alert("Fail");
                    }
                });
            }
        }

        function isfreezeacc(id, act) {
            $('#ContentPlaceHolder1_TextBox_ID').val('' + id + '');
            $('#ContentPlaceHolder1_TextBox_Act').val('' + act + '');
            document.getElementById('<%=Button_Act.ClientID %>').click();
        }//ForceLogdout
        function ForceLogdout(id, act) {
            $('#ContentPlaceHolder1_TextBox_ID').val('' + id + '');
            $('#ContentPlaceHolder1_TextBox_Act').val('' + act + '');
            document.getElementById('<%=Button_FLogout.ClientID %>').click();
               }

        function set_userinformation(userid, username, useracc) {
            $('#ContentPlaceHolder1_TextBox_Source').val('' + '' + '');
            document.getElementById('<%=TextBox_TargetID.ClientID%>').disabled = false;
            document.getElementById('<%=TextBox_TargetName.ClientID%>').disabled = false;
            $('#ContentPlaceHolder1_TextBox_TargetID').val('' + userid + '');
            $('#ContentPlaceHolder1_TextBox_TargetName').val('' + username + '');
            document.getElementById('<%=TextBox_TargetID.ClientID%>').disabled = true;
            document.getElementById('<%=TextBox_TargetName.ClientID%>').disabled = true;
            //設定使用者到DropDownList內
            set_dropdownlist('ContentPlaceHolder1_DropDownList_NoReset', 'ContentPlaceHolder1_DropDownList_Source', useracc);
        }

        function set_textbox() {
            var select = document.getElementById('ContentPlaceHolder1_DropDownList_Source');
            $('#ContentPlaceHolder1_TextBox_Source').val('' + select.options[select.selectedIndex].text + '');

        }

        $("#btncopy").click(function () {
            var copy = document.getElementById("ContentPlaceHolder1_TextBox_Source").value;
            if (copy != '') {
                $.blockUI({ message: '<img src="../../images/loading.gif" />' });
                document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
                document.getElementById('btncopy').disabled = true;
                document.getElementById('<%=TextBox_TargetID.ClientID%>').disabled = false;
                document.getElementById('<%=TextBox_TargetName.ClientID%>').disabled = false;
                document.getElementById('<%=Button_copy.ClientID %>').click();
            }
            else
                alert('請選擇複製對象');
        });
        //下拉選單事件
        function func(str) {
            var e = document.getElementById("ContentPlaceHolder1_DropDownList_" + str);
            var strUser = e.options[e.selectedIndex].text;
            $('#ContentPlaceHolder1_TextBox_textTemp').val('' + str + '^' + strUser + '');
            document.getElementById('<%=Button_change.ClientID %>').click();
        }

        //設定群組用
        function Set_Group(groupname, useracc) {
            $('#ContentPlaceHolder1_TextBox_temp').val('' + useracc + '');
            //先讓全部false
            var rb = document.getElementById("<%=CheckBoxList_Group.ClientID %>");
            var rbItems = rb.getElementsByTagName('input');
            var group = groupname.split('^');
            for (var itemIndex = 0; itemIndex < rbItems.length; itemIndex++) {
                //先讓其為false
                rbItems[itemIndex].checked = false;

                //若陣列內存在該元素，則讓其為true
                if (group.indexOf(rbItems[itemIndex].value) != -1)
                    rbItems[itemIndex].checked = true;
            }
        }


        $("#btnsave").click(function () {

            var groupname = '';

            var rb = document.getElementById("<%=CheckBoxList_Group.ClientID %>");
            var rbItems = rb.getElementsByTagName('input');
            for (var itemIndex = 0; itemIndex < rbItems.length; itemIndex++) {
                if (rbItems[itemIndex].checked)
                    groupname += rbItems[itemIndex].value + '^';
            }
            $('#ContentPlaceHolder1_TextBox_group').val('' + groupname + '');


            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btnsave').disabled = true;

            document.getElementById('<%=Button_Save.ClientID %>').click();
        });

        //新增單一帳號
        $("#btnadd").click(function () {
            var dpm = document.getElementById("ContentPlaceHolder1_DropDownList_depart").selectedIndex;
            var name = document.getElementById("ContentPlaceHolder1_TextBox_Name").value;
            var num = document.getElementById("ContentPlaceHolder1_TextBox_phone").value;
            var acc = document.getElementById("ContentPlaceHolder1_TextBox_Acc").value;
            var pwd1 = document.getElementById("ContentPlaceHolder1_TextBox_Pwd1").value;
            var pwd2 = document.getElementById("ContentPlaceHolder1_TextBox_Pwd2").value;
            var code = document.getElementById("ContentPlaceHolder1_TextBox_code").value;
            var lable = document.getElementById("ContentPlaceHolder1_Label_code").innerText;
            var remind = document.getElementById("_remind");
            var re = /^[0-9]+$/; remind.style.color = "#FF3333";
            if (dpm != 0) {
                if (pwd1 == pwd2) {
                    if (code == lable) {
                        document.getElementById('<%=Button_Add.ClientID %>').click();
                    }
                    else {
                        remind.innerHTML = "驗證錯誤,請重新檢查 !";
                    }
                }
                else {
                    remind.innerHTML = "密碼不一致,請重新檢查 !";
                }
            }
            else {
                remind.innerHTML = "請選擇部門";
            }
        });


        $("#btnsaveuser").click(function () {

            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btnsaveuser').disabled = true;

            document.getElementById('<%=Button_AddUsers.ClientID %>').click();
        });



    </script>
</asp:Content>
