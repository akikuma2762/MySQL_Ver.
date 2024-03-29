﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="dek_erpvis_v2.pages.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>系統登入 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <!-- Bootstrap -->
    <link href="assets/vendors/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
     <link href="../../assets/build/css/Change_Table_Button.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="assets/vendors/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <!-- Animate.css -->
    <link href="assets/vendors/animate.css/animate.min.css" rel="stylesheet" />
    <!-- Custom Theme Style -->
    <link href="assets/build/css/custom.min.css" rel="stylesheet" />
    <!-- iCheck -->
    <link href="assets/vendors/iCheck/skins/flat/green.css" rel="stylesheet" />
    <!-- Select2 -->
    <link href="assets/vendors/select2/dist/css/select2.min.css" rel="stylesheet" />
    <!-- Switchery -->
    <link href="assets/vendors/switchery/dist/switchery.min.css" rel="stylesheet" />
    <!-- starrr -->
    <link href="assets/vendors/starrr/dist/starrr.css" rel="stylesheet" />
</head>

<body class="login">
    <div>
        <a class="hiddenanchor" id="signup"></a>
        <a class="hiddenanchor" id="signin"></a>

        <div class="login_wrapper">
            <div class="animate form login_form">
                <section class="login_content">
                    <form runat="server">
      
                        <h1><strong>德科可視化系統</strong></h1>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 ">
                                <h5 class="modaltextstyle">
                                    <i id="_remind" style="font-size: x-large;"></i>
                                </h5>
                            </div>
                        </div>
                        <div>
                            <asp:TextBox ID="TextBox_account" runat="server" CssClass="form-control" placeholder="請輸入帳號或手機"></asp:TextBox>
                        </div>
                        <div>
                            <asp:TextBox ID="TextBox_password" TextMode="Password" runat="server" CssClass="form-control" placeholder="請輸入密碼"></asp:TextBox>
                        </div>
                        <div>
                            <asp:Button ID="Button_login" class="btn btn-default submit" runat="server" Text="123" OnClick="Button_login_Click" Style="display:none;" />
                            <input type="button" value="登入" id="btncheck" class="btn btn-default" style="float: left;margin-left: 38px;" />
                            <asp:CheckBox type="checkbox" ID="CheckBox_memory" runat="server" Checked="false" CssClass="reset_pass" Text="記住我" Font-Size="Larger" />
                        </div>
                        <div class="clearfix"></div>
                        <div class="separator">
                            <div class="clearfix"></div>
                            <p class="change_link" >
                                第一次使用?
                                <strong><a href="../create_acc.aspx"><u>點我建立帳戶</u> </a></strong>
                            </p>
                            <strong style="display:none"><a href="../forget_pwd.aspx"><u>忘記密碼?</u> </a></strong>
               

                            <div>
                                <h1><strong>VIS　x　ERP </strong></h1>
                                <p>©2018 dek Intelligent Technology Co. Ltd </p>
                            </div>
                        </div>
                    </form>
                </section>
            </div>
        </div>
    </div>
    <!-- jQuery -->
    <script src="assets/vendors/jquery/dist/jquery.min.js"></script>
    <script src="assets/vendors/bootstrap/dist/js/bootstrap.min.js"></script>
    <script src="assets/vendors/fastclick/lib/fastclick.js"></script>
    <script src="assets/vendors/nprogress/nprogress.js"></script>
    <script src="assets/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"></script>
    <script src="assets/vendors/iCheck/icheck.min.js"></script>
    <script src="assets/vendors/moment/min/moment.min.js"></script>
    <script src="assets/vendors/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script src="assets/vendors/bootstrap-wysiwyg/js/bootstrap-wysiwyg.min.js"></script>
    <script src="assets/vendors/jquery.hotkeys/jquery.hotkeys.js"></script>
    <script src="assets/vendors/google-code-prettify/src/prettify.js"></script>
    <script src="assets/vendors/jquery.tagsinput/src/jquery.tagsinput.js"></script>
    <script src="assets/vendors/switchery/dist/switchery.min.js"></script>
    <script src="assets/vendors/select2/dist/js/select2.full.min.js"></script>
    <script src="assets/vendors/parsleyjs/dist/parsley.min.js"></script>
    <script src="assets/vendors/autosize/dist/autosize.min.js"></script>
    <script src="assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"></script>
    <script src="assets/vendors/starrr/dist/starrr.js"></script>
    <script src="assets/build/js/custom.min.js"></script>
    <script>
        $("form input:checkbox").addClass('flat');
        $("#btncheck").click(function () {
            var remind = document.getElementById("_remind");
            remind.style.color = "#FF3333";
            var acc = document.getElementById("TextBox_account").value;
            var pwd = document.getElementById("TextBox_password").value;
            if (acc != "" && pwd != "") {
                document.getElementById('<%=Button_login.ClientID %>').click();
            } else {
                remind.innerHTML = "帳號與密碼不得為空,請重新檢查 !";
            }
        });



    </script>

</body>
</html>








