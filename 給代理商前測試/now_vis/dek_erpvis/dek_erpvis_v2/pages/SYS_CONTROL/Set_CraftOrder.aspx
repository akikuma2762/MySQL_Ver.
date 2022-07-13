<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Set_CraftOrder.aspx.cs" Inherits="dek_erpvis_v2.pages.SYS_CONTROL.Set_CraftOrder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%=color %>

    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/Orders.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- page content -->
    <style>
        input[type="checkbox"] {
            width: 20px;
            height: 20px;
            cursor: auto;
            -webkit-appearance: default-button;
        }

            input[type="checkbox"] + label {
                font-size: 18px;
            }
    </style>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <div class="right_col" role="main" style="min-height: 910px;">
                <div class="row">
                    <div class="col-md-4 col-sm-6 col-xs-12" style="padding-right: 1px">
                        <div class="x_panel" id="Div_Shadow">
                            <div class="x_title" style="text-align: center">
                                <div><b>儲存結果</b></div>
                                <hr />
                                <div class="col-md-10 col-sm-10 col-xs-10" style="text-align: left">
                                    <asp:Button ID="Button_Open" runat="server" Text="展開" OnClick="Button_Action_Click" class="btn btn-default" />
                                </div>
                                <div class="col-md-2 col-sm-2 col-xs-2">
                                    <asp:Button ID="Button_Shrink" runat="server" Text="收合" OnClick="Button_Action_Click" class="btn btn-default" />
                                </div>

                                <asp:TreeView ID="TreeView_Result" runat="server" ShowCheckBoxes="None" ImageSet="XPFileExplorer" NodeIndent="15">
                                    <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
                                    <NodeStyle Font-Names="Tahoma" Font-Size="12pt" ForeColor="Black" HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" />
                                    <ParentNodeStyle Font-Bold="False" />
                                    <SelectedNodeStyle BackColor="#B5B5B5" Font-Underline="False" HorizontalPadding="0px" VerticalPadding="0px" />
                                </asp:TreeView>
                                <div class="clearfix"></div>
                            </div>
                            <div class="col-md-10 col-sm-10 col-xs-10">
                                <asp:Button ID="Button_Save" runat="server" Text="確定並送出" OnClick="Button_Save_Click" class="btn btn-success" />
                            </div>
                            <div class="col-md-2 col-sm-2 col-xs-2">
                                <button id="btnclear" type="button" onclick="Clear_Treeview()" class="btn btn-danger">清除</button>
                                <asp:Button ID="Button_Clear" runat="server" Text="清除" OnClick="Button_Action_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-2 col-sm-6 col-xs-12">
                        <div class="x_panel" id="Div_Shadow">
                            <div class="x_title" style="text-align: center">
                                <div><b>節點變更</b></div>
                                <hr />
                                <div class="col-md-12 col-sm-12 col-xs-12" align="center">
                                    <asp:Button ID="Button_Delete" runat="server" Text="刪除節點" OnClick="Button_Delete_Click" class="btn btn-danger" />
                                </div>
                                <br>
                                <br>
                                <br>
                                <div class="col-md-12 col-sm-12 col-xs-12" align="center">
                                    <asp:ImageButton ID="ImageButton_Up" runat="server" ImageUrl="../../assets/images/up.png" Height="40px" Width="40px" OnClick="Button_Vertical_Click" /><br>
                                </div>

                                <div class="col-md-12 col-sm-12 col-xs-12" align="center">
                                    <asp:ImageButton ID="ImageButton_Down" runat="server" ImageUrl="../../assets/images/down.png" Height="40px" Width="40px" OnClick="Button_Vertical_Click" /><br>
                                </div>

                                <br>
                                <div class="clearfix"></div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6 col-sm-12 col-xs-12" style="padding-left: 5px">
                        <div class="x_panel" id="Div_Shadow">
                            <div class="x_title" style="text-align: center">
                                <div>
                                    <b style="font-size: 25px">建立工藝</b>
                                    <button type="button" style="position: absolute; right: 0px" class="btn btn-primary" data-toggle="modal" data-target="#ImportCraft">匯入工藝</button>
                                </div>
                                <hr />
                                <b style="font-size: 25px">機型</b><b style="font-size: 25px; margin-left: 10px" onclick="close_div('div_machine')"><a class="collapse-link" onclick="close_div('div_machine')"><i class="fa fa-chevron-up" onclick="close_div('div_machine')"></i></a></b>
                                <div id="div_machine">
                                    <asp:CheckBoxList ID="CheckBoxList_Machine" RepeatColumns="2" runat="server" class="text-left"></asp:CheckBoxList>
                                </div>
                                <div>
                                    <asp:Button ID="Button_AddMachine" runat="server" Text="新增機型" class="btn btn-primary antosubmit2" OnClick="Button_AddMachine_Click" />
                                </div>
                                <hr />

                                <div>
                                    <asp:TextBox ID="TextBox_Craft" runat="server" placeholder="請輸入工藝" Font-Size="18px" Width="150px"></asp:TextBox>
                                    <b style="font-size: 25px; margin-left: 10px" onclick="close_div('div_craft')"><a class="collapse-link" onclick="close_div('div_craft')"><i class="fa fa-chevron-up" onclick="close_div('div_craft')"></i></a></b>
                                </div>
                                <div id="div_craft">
                                    <b style="font-size: 25px">已輸入之工藝</b>
                                    <asp:CheckBoxList ID="CheckBoxList_Craft" RepeatColumns="2" runat="server" class="text-left"></asp:CheckBoxList>
                                </div>
                                <div>
                                    <asp:Button ID="Button_AddCraft" runat="server" Text="新增工藝" class="btn btn-primary antosubmit2" OnClick="Button_AddCraft_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!---------------->
    </div>

    <!--匯入工藝-->
    <div id="ImportCraft" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog3">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="testmoda0l234" style="padding: 5px 20px;">
                        <div class="form-group">
                            <h5 class="modaltextstyle">
                                <i class="fa fa-caret-down" style="width: 100%; text-align: center"><b>匯入工藝</b></i>
                            </h5>

                            <hr />
                            <div class="row" style="text-align: center">
                                <a href="../File/工藝範例VER2.xlsx"><b>範例檔下載</b></a>
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
                                <button id="btnsavecraft" type="button" class="btn btn-primary antosubmit2 ">匯入工藝</button>
                                <asp:Button ID="Button_Craft" runat="server" Text="匯入工藝" OnClick="Button_Craft_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        function Clear_Treeview() {
            answer = confirm("您確定要清除嗎??");
            if (answer) {
                document.getElementById('<%=Button_Clear.ClientID %>').click();
            }
        }

        //抓取點選元素之ID
        $(document).click(function (event) {
            var div_id = event.target.id;
            if (div_id.indexOf('TreeView_Result') != -1 && div_id.indexOf('Nodes') == -1 && div_id.indexOf('CheckBox') != -1) {

                //底下子項目隨父項目 選取/不選取
                var check_id = div_id.replaceAll('CheckBox', 'Nodes');
                var checks = document.querySelectorAll('#' + check_id + ' input[type="checkbox"]');
                for (var i = 0; i < checks.length; i++) {
                    var check = checks[i];
                    if (!check.disabled) {
                        check.checked = document.getElementById(div_id).checked;
                    }
                }

                //子項目被選取時，父項目也須被選取
                var cbx = document.getElementById(div_id);//获取按钮节点
                var div = cbx.parentNode.parentNode.parentNode.parentNode.parentNode.id;
                document.getElementById(div.replaceAll('Nodes', 'CheckBox')).checked = true;
            }
        });

        $("#btnsavecraft").click(function () {

            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.querySelector(".blockUI.blockMsg.blockPage").style.zIndex = 1000000;
            document.getElementById('btnsavecraft').disabled = true;

            document.getElementById('<%=Button_Craft.ClientID %>').click();
        });

        function close_div(div) {
            if (document.getElementById(div).style.display == 'none')
                document.getElementById(div).style.display = 'inline';
            else
                document.getElementById(div).style.display = 'none';
        }
    </script>
</asp:Content>

