<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="materialrequirementplanning.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PD.materialrequirementplanning1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>物料領用統計表 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" media="screen and (max-width:768px)" />
    <link href="../../Content/dp_PD/materialrequirementplanning.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!--局部刷新-->
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <!--順序存至TEXTBOX-->
            <asp:TextBox ID="TextBox_SaveColumn" runat="server" Style="display: none"></asp:TextBox>
            <!--執行動作-->
            <asp:Button ID="Button_SaveColumns" runat="server" Text="Button" OnClick="Button_SaveColumns_Click" Style="display: none" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="right_col" role="main">
        <!-----------------title------------------>
        <%=path %>
        <br>
        <div class="clearfix"></div>
        <!-----------------content------------------>
        <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">
                    <div class="x_content">
                        <div id="materialrequirementplanning"></div>
                        <div class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">

                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-5 col-sm-3 col-xs-12 _SelectType">
                                                <span>搜尋方式</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-12">

                                                <asp:RadioButtonList ID="RadioButtonList_select_type" Font-Names="NotoSans" runat="server" RepeatDirection="Vertical" CssClass="table-striped">
                                                    <%--<asp:ListItem Value="0">依 分類 檢索</asp:ListItem>--%>
                                                    <asp:ListItem Value="1" Selected="True">依 品號 檢索</asp:ListItem>
                                                    <asp:ListItem Value="2">依 品名規格 檢索</asp:ListItem>
                                                    <asp:ListItem Value="3">均不指定</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-xs-12 col-sm-12">
                                            <div class="col-md-5 col-xs-12 col-sm-6 col-xs-6">
                                                <asp:DropDownList ID="DropDownList_selectedcondi" Font-Names="NotoSans" Style="width: 100%" runat="server" CssClass="btn btn-default dropdown-toggle">
                                                    <asp:ListItem Value="內含">內含</asp:ListItem>
                                                    <asp:ListItem Value="等於">等於</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-7 col-xs-12 col-sm-6 col-xs-6" style="margin: 0px 0px 0px -5px">
                                                <div id="input_keyword">
                                                    <asp:TextBox ID="TextBox_keyword" data-validate-length-range="6" data-validate-words="2" runat="server" Width="100%" placeholder="請輸入字串"></asp:TextBox>
                                                </div>
                                                <div id="warp_droplist" style="display: initial;">
                                                    <asp:DropDownList ID="DropDownList_materialstype" Font-Names="NotoSans" runat="server" Width="100%" CssClass="btn btn-default dropdown-toggle">
                                                        <asp:ListItem Value="壓缸固定鈑" Selected="True">壓缸固定鈑</asp:ListItem>
                                                        <asp:ListItem Value="刀套">刀套</asp:ListItem>
                                                        <asp:ListItem Value="刀臂">刀臂</asp:ListItem>
                                                        <asp:ListItem Value="頂刀爪">頂刀爪</asp:ListItem>
                                                        <asp:ListItem Value="扣刀爪">扣刀爪#50</asp:ListItem>
                                                        <asp:ListItem Value="圓筒凸輪">圓筒凸輪</asp:ListItem>
                                                        <asp:ListItem Value="馬達座">馬達座</asp:ListItem>
                                                        <asp:ListItem Value="鍊輪座">鍊輪座</asp:ListItem>
                                                        <asp:ListItem Value="減速機固定鈑">減速機固定鈑</asp:ListItem>
                                                        <asp:ListItem Value="mazak inte">mazak inte</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-xs-12 col-sm-6 col-xs-12">
                                            <div class="col-md-5 col-xs-12 col-sm-6 col-xs-6" style="margin: 3px 0px 0px 0px">
                                                <asp:DropDownList ID="DropDownList_substring" Width="97%" Font-Names="NotoSans" runat="server" CssClass="btn btn-default dropdown-toggle" onchange="getval(this);">
                                                    <asp:ListItem Value="N" Selected="True">不擷取位數</asp:ListItem>
                                                    <asp:ListItem Value="Y">擷取位數</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-7 col-xs-12 col-sm-6 col-xs-6">
                                                <div id="input_subs" style="margin: 0px 0px 0px -5px">
                                                    <p></p>
                                                    <asp:TextBox ID="TextBox_substring" Font-Names="NotoSans" CssClass="form-control" runat="server" Width="100%" placeholder="只能輸入數字" TextMode="Number" Enabled="false" min="12" max="99" Text="12"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-5 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                <span>安全庫存量</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-7">
                                                <input id="demo_vertical3" runat="server" type="text" value="1" name="demo_vertical3" data-bts-button-down-class="btn btn-secondary" data-bts-button-up-class="btn btn-secondary">
                                            </div>
                                        </div>

                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-5 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                <span>最小採購量</span>
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-7">
                                                <input id="demo_vertical2" runat="server" type="text" value="2" name="demo_vertical2" data-bts-button-down-class="btn btn-secondary" data-bts-button-up-class="btn btn-secondary">
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-5 col-sm-3 col-xs-5" style="margin: 5px 0px 5px 0px">
                                                領料日期
                                            </div>
                                            <div class="col-md-7 col-sm-9 col-xs-7">
                                                <div class="row">
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-sm-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="txt_str" runat="server" Style="" Width="97%" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                    <fieldset>
                                                        <div class="control-group">
                                                            <div class="controls">
                                                                <div class="col-md-12 col-sm-12 col-xs-12" style="margin: 0px 0px 5px 0px">
                                                                    <asp:TextBox ID="txt_end" runat="server" Style="" Width="97%" TextMode="Date" CssClass="form-control   text-left"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="col-md-9 col-xs-8">
                                            </div>
                                            <div class="col-md-3 col-xs-12">
                                                <asp:Button ID="button_select" runat="server" Text="執行檢索" class="btn btn-secondary" OnClick="button_select_Click" Style="display: none" />
                                                <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行搜索</button>
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
        <!-----------------/content------------------>
    </div>
    <%=Use_Javascript.Quote_Javascript() %>
    <script>
        //public variable

        input_keyword.style.display = 'none';
        //function
        function initialization() {


            document.getElementById('ContentPlaceHolder1_TextBox_substring').disabled = true;
            $("select#ContentPlaceHolder1_DropDownList_substring").prop('selectedIndex', 0);
        }
        function getval(sel) {
            //擷取品號位數
            if (sel.value == "Y") {
                document.getElementById('ContentPlaceHolder1_TextBox_substring').disabled = false;
            }
            if (sel.value == "N") {
                document.getElementById('ContentPlaceHolder1_TextBox_substring').disabled = true;
            }
        }
        function relocation(selValue) {
            //2019/06/17，依檢索方式，動態產生TextBox或DropDownList
            var input_keyword = document.getElementById('input_keyword');
            var warp_droplist = document.getElementById('warp_droplist');
            var selectedcondi = document.getElementById('ContentPlaceHolder1_DropDownList_selectedcondi');
            var materialstype = document.getElementById('ContentPlaceHolder1_DropDownList_materialstype');
            var txt = document.getElementById('ContentPlaceHolder1_TextBox_keyword');

            switch (selValue) {
                case "0":
                    input_keyword.style.display = 'none';
                    warp_droplist.style.display = 'initial';
                    selectedcondi.disabled = false;
                    materialstype.disabled = false;
                    txt.disabled = false;
                    break;
                case "1":
                    input_keyword.style.display = 'initial';
                    warp_droplist.style.display = 'none';
                    selectedcondi.disabled = false;
                    materialstype.disabled = false;
                    txt.disabled = false;
                    break;
                case "2":
                    input_keyword.style.display = 'initial';
                    warp_droplist.style.display = 'none';
                    selectedcondi.disabled = false;
                    materialstype.disabled = false;
                    txt.disabled = false;
                    break;
                case "3":
                    input_keyword.style.display = 'initial';
                    warp_droplist.style.display = 'none';
                    txt.disabled = true;
                    selectedcondi.disabled = true;
                    materialstype.disabled = true;
                    break;
            }
        }
        function checkthetextcount(id) {
            var text_count = document.getElementById("ContentPlaceHolder1_TextBox_substring").value;     // 品號位數
            var selected = $("#ContentPlaceHolder1_DropDownList_substring option:selected").val();       // 擷取品號位數
            var selValue = $('#ContentPlaceHolder1_RadioButtonList_select_type input:checked').val();    // 檢索方式
            var re = /^[0-9]+$/;
            //----------------------------------------
            if (selected == "Y") {
                if (text_count != "") {
                    if (re.test(text_count)) {
                        check_val(selValue, id);
                    }
                }
            } else if (selected == "N") {
                check_val(selValue, id);
            }
            //-----------------------------------------
        }
        function check_val(selValue, id) {
            var itme = document.getElementById("ContentPlaceHolder1_TextBox_keyword").value;// 品號、品名規格
            if (selValue == 0) {
                search_go(id);
            }
            else if (selValue == 1) {
                if (itme != "") {
                    search_go(id);
                }
            }
            else if (selValue == 2) {
                if (itme != "") {
                    search_go(id);
                }
            }
            else if (selValue == 3) { //20191001新增均不指定
                search_go(id);
            }

        }
        function search_go(id) {

            switch (id) {

                case "btncheck":
                    document.getElementById('<%=button_select.ClientID %>').click();
                    break;
            }
        }

        //evet
        $("#btncheck").click(function () {
            $.blockUI({ message: '<img src="../../images/loading.gif" />' });
            document.getElementById('<%=button_select.ClientID %>').click();
        });
        $('#ContentPlaceHolder1_RadioButtonList_select_type input').change(function () {
            relocation($(this).val());
        });
        $("input[name='ctl00$ContentPlaceHolder1$demo_vertical2']").TouchSpin({
            //verticalbuttons: true,
            //prefix: '#最小採購量',
            min: 0,
            max: 100,
            step: 0.1,
            decimals: 1,
            boostat: 5,
            maxboostedstep: 10,
        });
        $("input[name='ctl00$ContentPlaceHolder1$demo_vertical3']").TouchSpin({
            //verticalbuttons: true,
            //prefix: '#安全庫存量',
            min: 0,
            max: 100,
            step: 0.1,
            decimals: 1,
            boostat: 5,
            maxboostedstep: 10,
        });
        initialization();
        relocation($('#ContentPlaceHolder1_RadioButtonList_select_type input:checked').val());

        //產生表格的HTML碼
        create_tablehtmlcode('materialrequirementplanning', '物料領用統計表', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('materialrequirementplanning=materialrequirementplanning_cust', '#datatable-buttons');

        $(document).ready(function () {
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
        });
    </script>
</asp:Content>
