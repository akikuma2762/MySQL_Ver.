﻿<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Set_LineTree.aspx.cs" Inherits="dek_erpvis_v2.pages.SYS_CONTROL.Set_LineTree" MaintainScrollPositionOnPostback="true" %>

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
    <div class="right_col" role="main">
        <div class="row">
            <div class="col-md-4 col-sm-6 col-xs-12" style="padding-right: 1px">
                <div class="x_panel" id="Div_Shadow">
                    <div class="x_title">
                        <h2><b>儲存結果</b></h2>
                        <br>
                        <hr />
                        <div class="col-md-10 col-sm-10 col-xs-10">
                            <asp:Button ID="Button_Open" runat="server" Text="展開" OnClick="Button_Action_Click" class="btn btn-default" />
                        </div>
                        <div class="col-md-2 col-sm-2 col-xs-2">
                            <asp:Button ID="Button_Shrink" runat="server" Text="收合" OnClick="Button_Action_Click" class="btn btn-default" />
                        </div>

                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                        </asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
                            <ContentTemplate>

                                <asp:TreeView ID="TreeView_Result" runat="server" ShowCheckBoxes="None" ImageSet="XPFileExplorer" NodeIndent="15">
                                    <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
                                    <NodeStyle Font-Names="Tahoma" Font-Size="12pt" ForeColor="Black" HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" />
                                    <ParentNodeStyle Font-Bold="False" />
                                    <SelectedNodeStyle BackColor="#B5B5B5" Font-Underline="False" HorizontalPadding="0px" VerticalPadding="0px" />
                                </asp:TreeView>

                            </ContentTemplate>
                        </asp:UpdatePanel>


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
                    <div class="x_title">
                        <h2><b>
                            <div class="col-md-12 col-sm-12 col-xs-12" style="text-align: center">
                                節點變更
                            </div>
                        </b></h2>
                        <br>
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
                        <div class="col-md-6 col-sm-12 col-xs-12" align="center" style="display: none">
                            <asp:ImageButton ID="ImageButton_Left" runat="server" ImageUrl="../../assets/images/left.png" Height="40px" Width="40px" OnClick="Button_Horizontal_Click" /><br>
                        </div>
                        <div class="col-md-6 col-sm-12 col-xs-12" align="center" style="display: none">
                            <asp:ImageButton ID="ImageButton_Right" runat="server" ImageUrl="../../assets/images/right.png" Height="40px" Width="40px" OnClick="Button_Horizontal_Click" />
                        </div>
                        <div class="col-md-12 col-sm-12 col-xs-12" align="center">
                            <asp:ImageButton ID="ImageButton_Down" runat="server" ImageUrl="../../assets/images/down.png" Height="40px" Width="40px" OnClick="Button_Vertical_Click" /><br>
                        </div>
                        <div class="col-md-12 col-sm-12 col-xs-12" align="center" style="display: none">
                            <br>
                            <asp:TextBox ID="TextBox_Edit" runat="server" placeholder="輸入名稱" Width="80%"></asp:TextBox>
                            <asp:Button ID="Button_Edit" runat="server" Text="修改節點名稱" OnClick="Button_Edit_Click" class="btn btn-info" />
                        </div>
                        <br>
                        <div class="clearfix"></div>
                    </div>
                </div>
            </div>
            <div class="col-md-6 col-sm-12 col-xs-12" style="padding-left: 5px">
                <div class="x_panel" id="Div_Shadow">
                    <div class="x_title">
                        <div class="col-md-4 col-sm-4 col-xs-0"></div>
                        <div class="col-md-8 col-sm-8 col-xs-12">
                            <asp:TextBox ID="TextBox_dpm" runat="server" placeholder="請輸入機種名稱" Font-Size="18px" Width="150px"></asp:TextBox>&nbsp&nbsp&nbsp
                        <asp:Button ID="Button_dpm" runat="server" Text="新增機種" OnClick="Button_dpm_Click" class="btn btn-primary antosubmit2" />
                            <asp:Button ID="Button_print" runat="server" Text="列印未設定機型" OnClick="Button_print_Click" class="btn btn-info antosubmit2" />


                        </div>
                        <br>
                        <hr />

                        <div class="col-md-12 col-sm-12 col-xs-12">

                            <div class="col-md-12 col-sm-12 col-xs-12" style="display: none">
                                <asp:TextBox ID="TextBox_Search" runat="server" placeholder="請輸入關鍵字"></asp:TextBox>
                                <asp:Button ID="Button_Search" runat="server" Text="快速搜尋" OnClick="Button_Search_Click" class="btn btn-default" />
                            </div>
                            <asp:CheckBoxList ID="CheckBoxList_Edit" runat="server" RepeatColumns="2"></asp:CheckBoxList>
                        </div>
                        <div class="col-md-5 col-sm-5 col-xs-4"></div>
                        <div class="col-md-7 col-sm-7 col-xs-8">
                            <asp:Button ID="Button_Add" runat="server" Text="新增機型" OnClick="Button_Add_Click" class="btn btn-primary antosubmit2" />
                        </div>
                        <div class="clearfix"></div>
                    </div>
                </div>
            </div>
        </div>

        <!---------------->
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
    </script>
</asp:Content>

