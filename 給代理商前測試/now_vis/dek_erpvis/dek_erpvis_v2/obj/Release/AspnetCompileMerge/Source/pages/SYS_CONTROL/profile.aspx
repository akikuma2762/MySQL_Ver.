﻿<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="dek_erpvis_v2.pages.SYS_CONTROL.profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%=page_name %> | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <!--<link href="../../assets/build/css/custom.css" rel="stylesheet" />
    <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />-->
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/SYS-CONTROL/profile.css" rel="stylesheet" />
        <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/index.css" rel="stylesheet" />
    <link href="../../Content/phone.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="right_col" role="main" onkeydown="if(event.keyCode==13) return false;">
        <div class="">
            <div class="page-title">
                <div class="title_left">
                    <h3 class="_mdTitle"><%=page_name %></h3>
                    <h5 class="_xsTitle"><%=page_name %></h5>
                </div>
                <div class="title_right"></div>
            </div>
            <div class="clearfix"></div>
            <div class="row">
                <div class="x_panel Div_Shadow">
                    <div class="x_title">
                        <h3 class="_mdTitle">檔案管理表單 </h3>
                        <h5 class="_xsTitle">檔案管理表單 </h5>
                        <div class="clearfix"></div>
                    </div>
                    <div class="x_content">
                        <div class="col-md-3 col-sm-3 col-xs-12">
                            <ul id="_Set" class="nav nav-tabs tabs-left">
                                <li style="display: none"><a href="#info_view" data-toggle="tab">基本資料</a></li>
                                <li class="active"><a href="#info_update" data-toggle="tab">資料維護</a></li>
                                <li><a href="#password_update" data-toggle="tab">變更密碼</a></li>
                                <%--  <li><a href="#settings" data-toggle="tab">設定</a></li>--%>
                            </ul>
                        </div>
                        <div class="col-md-9 col-sm-9 col-xs-12">
                            <!-- Tab panes -->
                            <div class="tab-content Div_s">
                                <div class="tab-pane " id="info_view">
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <p class="lead">
                                            <label class="control-label">基本資料</label>
                                        </p>
                                    </div>
                                    <div class="col-md-12 cl-sm-6 col-xs-12">
                                        <div class="col-md-2 col-sm-4 col-xs-2">
                                            <label class="control-label">姓名 </label>
                                        </div>
                                        <div class="col-md-10 col-sm-8 col-xs-10">
                                            <asp:Label ID="Label_name" runat="server" Text=""></asp:Label>
                                            <strong>
                                                <asp:Label ID="Label_ADM" runat="server" Text=""></asp:Label>
                                            </strong>
                                        </div>
                                    </div>
                                    <div class="col-md-12 cl-sm-6 col-xs-6">
                                        <div class="col-md-2 col-sm-4 col-xs-4">
                                            <label>電郵</label>
                                        </div>
                                        <div class="col-md-10 col-sm-8 col-xs-8">
                                            <asp:Label ID="Label_email" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <!--20191115總經理說不需要 
                                        <div class="row">
                                        <div class="col-md-2 col-sm-4 col-xs-3">
                                            <label>生日</label>
                                        </div>
                                        <div class="col-md-10 col-sm-8 col-xs-9">
                                            <asp:Label ID="Label_birthday" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>-->
                                    <div class="col-md-12 cl-sm-6 col-xs-6">
                                        <div class="col-md-2 col-sm-4 col-xs-4">
                                            <label>手機</label>
                                        </div>
                                        <div class="col-md-10 col-sm-8 col-xs-8">
                                            <asp:Label ID="Label_number" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-md-12 cl-sm-6 col-xs-6">
                                        <div class="col-md-2 col-sm-4 col-xs-4">
                                            <label>部門</label>
                                        </div>
                                        <div class="col-md-10 col-sm-8 col-xs-8">
                                            <asp:Label ID="Label_dpm" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>

                                    <br />
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <table class="table">
                                            <thead>
                                                <tr id="tr_row">
                                                    <th>站內權限</th>
                                                    <th>頁面類別</th>
                                                    <th>頁面名稱</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <%=tr %>
                                            </tbody>
                                        </table>
                                    </div>

                                </div>
                                <div class="tab-pane active" id="info_update">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <p class="lead">
                                                <label>資料維護</label>
                                            </p>
                                            <div class="col-12">
                                                <label for="fullname">姓名Full Name * :</label>
                                                <asp:TextBox ID="TextBox_fullname" runat="server" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-12">
                                                <label for="fullname">手機Phone Number :</label>
                                                <asp:TextBox ID="TextBox_num" runat="server" class="form-control" placeholder=" ex. 0911234567"></asp:TextBox>
                                            </div>
                                            <div class="col-12 _Birthday">
                                                <label class="_Birthday_label" for="heard">生日Birthday :</label>
                                                <fieldset>
                                                    <div class="control-group">
                                                        <div class="controls">
                                                            <div class="col-md-12 col-sm-12 col-xs-12 xdisplay_inputx form-group has-feedback">
                                                                <input type="text" class="form-control has-feedback-left" id="TextBox_birthday" name="TextBox_birthday" runat="server" value="" placeholder="ex. yyyyMMdd" aria-describedby="inputSuccess2Status1" autocomplete="off">
                                                                <span class="fa fa-calendar-o form-control-feedback left" aria-hidden="true"></span>
                                                                <span id="inputSuccess2Status1" class="sr-only">(success)</span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                            <div class="col-12">
                                                <label for="email">電郵Email :</label>
                                                <asp:TextBox ID="TextBox_email" runat="server" class="form-control" placeholder=" ex. ABC123@deta.com" TextMode="Email"></asp:TextBox>
                                            </div>
                                            <div class="col-12">
                                                <label for="heard">部門Department * :</label>
                                                <asp:DropDownList ID="DropDownList_dmt" runat="server" class="form-control">
                                                </asp:DropDownList>
                                            </div>
                                            <div class=" row">
                                                <p><i id="info_remind"></i></p>
                                            </div>
                                            <br />
                                            <asp:Button ID="Button_info" runat="server" Style="display: none;" OnClick="Button_info_Click" />
                                            <button id="btninfo" class="btn btn-success" type="button">提交表單</button>
                                            <button id="btn_cancel" class="btn btn-default" type="button">取消操作</button>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="password_update">
                                    <div class="row">
                                        <div class="col-md-6 col-sm-8 col-xs-12">
                                            <p class="lead">
                                                <label>變更密碼</label>
                                            </p>
                                            <div class="col-12">
                                                <label class="col-md-12 col-xs-4" for="email">原密碼 * :</label>
                                                <asp:TextBox ID="TextBox_oldpwd" class="form-control col-md-12 col-xs-8" runat="server" placeholder=" Old Password" TextMode="Password"></asp:TextBox>
                                            </div>
                                            <div class="col-12">
                                                <label class="col-md-12 col-xs-4" for="password1">新密碼  * :</label>
                                                <asp:TextBox ID="TextBox_pwd1" class="form-control col-md-12 col-xs-8" runat="server" placeholder=" New Password" TextMode="Password"></asp:TextBox>
                                            </div>
                                            <div class="col-12">
                                                <label class="col-md-12 col-xs-4" for="password2">再次輸入新密碼 * :</label>
                                                <asp:TextBox ID="TextBox_pwd2" class="form-control col-md-12 col-xs-8" runat="server" placeholder=" New Password again" TextMode="Password"></asp:TextBox>
                                            </div>
                                            <div class="col-12">
                                                <label class="col-md-12 col-sm-4 col-xs-4" for="code">驗證碼 * :</label>
                                                <div class="col-md-4 col-sm-4 ">
                                                    <asp:Label ID="Label_code" runat="server" Text="" class="form-control text-center"></asp:Label>
                                                </div>
                                                <div class="col-md-8 col-sm-8">
                                                    <asp:TextBox ID="TextBox_code" runat="server" placeholder="*請輸入左側驗證碼" class="form-control" autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                            <br />
                                        </div>
                                    </div>
                                    <div class=" row">
                                        <p><i id="pwd_remind"></i></p>
                                    </div>
                                    <asp:Button ID="Button_pwd" runat="server" Style="display: none;" OnClick="Button_pwd_Click" />
                                    <button id="btnpassword" class="btn btn-success" type="button">提交表單</button>
                                    <button id="btncancel" class="btn btn-default" type="button">取消操作</button>
                                </div>
                                <div class="tab-pane" id="settings">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
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

    <!-- /page content -->
    <!-- jQuery -->
    <script src="../../assets/vendors/jquery/dist/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="../../assets/vendors/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- FastClick -->
    <script src="../../assets/vendors/fastclick/lib/fastclick.js"></script>
    <!-- NProgress -->
    <script src="../../assets/vendors/nprogress/nprogress.js"></script>
    <!-- bootstrap-progressbar -->
    <script src="../../assets/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"></script>
    <!-- iCheck -->
    <script src="../../assets/vendors/iCheck/icheck.min.js"></script>
    <!-- bootstrap-daterangepicker -->
    <script src="../../assets/vendors/moment/min/moment.min.js"></script>
    <script src="../../assets/vendors/bootstrap-daterangepicker/daterangepicker.js"></script>
    <!-- bootstrap-wysiwyg -->
    <script src="../../assets/vendors/bootstrap-wysiwyg/js/bootstrap-wysiwyg.min.js"></script>
    <script src="../../assets/vendors/jquery.hotkeys/jquery.hotkeys.js"></script>
    <script src="../../assets/vendors/google-code-prettify/src/prettify.js"></script>
    <!-- jQuery Tags Input -->
    <script src="../../assets/vendors/jquery.tagsinput/src/jquery.tagsinput.js"></script>
    <!-- Switchery -->
    <script src="../../assets/vendors/switchery/dist/switchery.min.js"></script>
    <!-- Select2 -->
    <script src="../../assets/vendors/select2/dist/js/select2.full.min.js"></script>
    <!-- Autosize -->
    <script src="../../assets/vendors/autosize/dist/autosize.min.js"></script>
    <!-- jQuery autocomplete -->
    <script src="../../assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"></script>
    <!-- starrr -->
    <script src="../../assets/vendors/starrr/dist/starrr.js"></script>
    <!-- Custom Theme Scripts -->
    <script src="../../assets/build/js/custom.min.js"></script>
    <!-- FloatingActionButton -->
    <script src="../../assets/vendors/FloatingActionButton/js/index.js"></script>
    <!-- canvasjs -->
    <script src="../../assets/vendors/canvas_js/canvasjs.min.js"></script>
    <!-- Datatables -->
    <script src="../../assets/vendors/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="../../assets/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/dataTables.buttons.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.flash.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.html5.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.print.min.js"></script>
    <script src="../../assets/vendors/datatables.net-responsive/js/dataTables.responsive.min.js"></script>
    <script src="../../assets/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js"></script>
    <script src="../../assets/vendors/datatables.net-scroller/js/dataTables.scroller.min.js"></script>
    <script src="../../assets/vendors/jszip/dist/jszip.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/pdfmake.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <script>
        $("#btninfo").click(function () {
            var fullname = document.getElementById("ContentPlaceHolder1_TextBox_fullname").value;
            var birthday = document.getElementById("ContentPlaceHolder1_TextBox_birthday").value;
            var number = document.getElementById("ContentPlaceHolder1_TextBox_num").value;
            var email = document.getElementById("ContentPlaceHolder1_TextBox_email").value;
            var e = document.getElementById("ContentPlaceHolder1_DropDownList_dmt");
            var dpm = e.options[e.selectedIndex].value;
            var remind = document.getElementById("info_remind");
            var re = /^[0-9]+$/; remind.style.color = "#FF3333";
            if (fullname != "") {
                if (dpm != "NULL") {
                    document.getElementById('<%=Button_info.ClientID %>').click();
                } else {
                    remind.innerHTML = "* 部門Department,不得為空!";
                }
            } else {
                remind.innerHTML = "* 姓名Full Name,不得為空!";
            }
        });
        $("#btnpassword").click(function () {
            var oldpwd = document.getElementById("ContentPlaceHolder1_TextBox_oldpwd").value;
            var pwd1 = document.getElementById("ContentPlaceHolder1_TextBox_pwd1").value;
            var pwd2 = document.getElementById("ContentPlaceHolder1_TextBox_pwd2").value;
            var code = document.getElementById("ContentPlaceHolder1_TextBox_code").value;
            var label = document.getElementById("ContentPlaceHolder1_Label_code").value;
            var remind = document.getElementById("pwd_remind");
            remind.style.color = "#FF3333";
            if (oldpwd != "") {
                if (pwd1 != "" && pwd2 != "") {
                    if (pwd1 == pwd2) {
                        if (code != "") {
                            if (code != label) {
                                document.getElementById('<%=Button_pwd.ClientID %>').click();
                            } else {
                                remind.innerHTML = "* 驗證碼錯誤,請重新檢查!";
                            }
                        } else {
                            remind.innerHTML = "* 驗證碼,不得為空!";
                        }
                    } else {
                        remind.innerHTML = "* 您的新密碼不一致!";
                    }
                } else {
                    remind.innerHTML = "* 新密碼,不得為空!";
                }
            } else {
                remind.innerHTML = "* 原密碼,不得為空!";
            }
        });
        $(function () {
            $('#ContentPlaceHolder1_TextBox_birthday').daterangepicker({
                singleDatePicker: true,
                autoUpdateInput: false,
                singleClasses: "picker_3",
                locale: {
                    format: 'YYYYMMDD',
                    cancelLabel: 'Clear',
                    daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                    monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
                }
            });
            $('#ContentPlaceHolder1_TextBox_birthday').on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.startDate.format('YYYYMMDD'));
            });
        });

        function submit_click(ID, URL) {
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/permission.asmx/authorize_permission",
                data: 'key=' + URL,
                success: function (xml) {
                    $(xml).find("ROOT").each(function (i) {
                        alert($(this).attr("system_msg").valueOf());
                        document.getElementById(ID).innerText = "待審核中";
                        document.getElementById(ID).classList.remove('btn-danger');
                        document.getElementById(ID).classList.add('btn-warning');
                        document.getElementById(ID).disabled = true;
                    });
                },
                error: function (data, errorThrown) {
                    alert("Fail");
                }
            });
        }
        $("form input:checkbox").addClass('flat');
    </script>
</asp:Content>
