﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forget_pwd.aspx.cs" Inherits="dek_erpvis_v2.forget_pwd" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>忘記密碼 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
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
            <section class="login_content">
                <form runat="server">
                    <h1><strong>帳戶救援</strong></h1>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 ">
                            <h5 class="modaltextstyle">
                                <i id="_remind" style="font-size: x-large;"></i>
                            </h5>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 ">
                            <asp:TextBox ID="TextBox_Acc" runat="server" placeholder="*可視化系統帳號" class="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 ">
                            <asp:TextBox ID="TextBox_Phone" runat="server" placeholder="*手機號碼" class="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 ">
                            <asp:TextBox ID="TextBox_Pwd1" type="password" runat="server" placeholder="*建立新密碼" class="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 ">
                            <asp:TextBox ID="TextBox_Pwd2" type="password" runat="server" placeholder="*再次輸入新密碼" class="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4 col-xs-4 ">
                            <asp:Label ID="Label_code" runat="server" Text="" class="form-control"></asp:Label>
                        </div>
                        <div class="col-md-8 col-sm-8 col-xs-8 ">
                            <asp:TextBox ID="TextBox_code" runat="server" placeholder="*請輸入左側驗證碼" class="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <button id="btncheck" type="button" class="btn btn-default" style="margin-left: 0; float: none;">送出</button>
                    <asp:Button ID="Button_add" runat="server" class="btn btn-default" Text="送出" Style="margin-left: 0; float: none; display: none;" onclick="Button_add_Click" />
                    <div class="clearfix"></div>
                    <div class="separator">
                        <p class="change_link">
                            找回密碼了嗎 ?
                            <strong><a href="../login.aspx" class="to_register"><u>點我登入 </u></a></strong>
                        </p>

                        <div class="clearfix"></div>
                        <br />

                        <div>
                            <h1><strong>VIS　x　ERP </strong></h1>
                            <p>©2018 dek Intelligent Technology Co. Ltd </p>
                            <!--<p>©2016 All Rights Reserved. Gentelella Alela! is a Bootstrap 3 template. Privacy and Terms</p>-->
                        </div>
                    </div>
                </form>
            </section>
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
    <script src="assets/vendors/jquery.tagsinput/src/jquery.tagsinput.js"></script>
    <script src="assets/vendors/switchery/dist/switchery.min.js"></script>
    <script src="assets/vendors/select2/dist/js/select2.full.min.js"></script>
    <script src="assets/vendors/autosize/dist/autosize.min.js"></script>
    <script src="assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"></script>
    <script src="assets/vendors/starrr/dist/starrr.js"></script>
    <script src="assets/build/js/custom.min.js"></script>
    <script>
        $("#btncheck").click(function () {
            var num = document.getElementById("TextBox_Phone").value;
            var acc = document.getElementById("TextBox_Acc").value;
            var pwd1 = document.getElementById("TextBox_Pwd1").value;
            var pwd2 = document.getElementById("TextBox_Pwd2").value;
            var code = document.getElementById("TextBox_code").value;
            var lable = document.getElementById("Label_code").innerText;
            var remind = document.getElementById("_remind");
            var re = /^[0-9]+$/; remind.style.color = "#FF3333";
            if (acc != "" && pwd1 != "" && pwd2 != "" && code != "" && num != "") {
                if (re.test(num) && num.length==10) {
                    if (pwd1 == pwd2) {
                        if (code == lable) {
                            document.getElementById('<%=Button_add.ClientID %>').click();
                        } else {
                            remind.innerHTML = "驗證錯誤,請重新檢查 !";
                        }
                    } else {
                        remind.innerHTML = "密碼不一致,請重新檢查 !";
                    }
                } else {
                    remind.innerHTML = "手機號碼有誤,請重新檢查 !";
                }
            } else {
                remind.innerHTML = "帳戶資料不得為空,請重新檢查 !";
            }
        });
    </script>
</body>

</html>
