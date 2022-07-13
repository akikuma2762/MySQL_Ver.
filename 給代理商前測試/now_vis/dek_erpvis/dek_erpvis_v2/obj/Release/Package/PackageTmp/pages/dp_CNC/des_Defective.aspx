<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="des_Defective.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_CNC.des_Defective" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title id="titlename">不良品原因分析 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <link href="../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.css" rel="stylesheet" type="text/css" media="all">
    <%=color %>
    <link href="../../Content/Default_input.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/shipment.css" rel="stylesheet" />
    <link href="../../Content/Default.css" rel="stylesheet" />
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

    <div class="right_col" role="main">
        <!-----------------title------------------>
        <%=path %>
        <br>
        <div class="clearfix"></div>
        <!-----------------/title------------------>
        <div class="page-title">
            <div class="row">
            </div>
        </div>
        <div class="clearfix"></div>
        <!-----------------content------------------>
        <ul id="myTab" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content1" id="home-tab" role="tab" data-toggle="tab" aria-expanded="true" onclick="div_close('block','1')">統計圖</a>
            </li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content3" id="profile-tab2" role="tab" data-toggle="tab" aria-expanded="false" onclick="div_close('none','2')">統計表</a>
            </li>
            <li role="presentation" class="" style="box-shadow: 3px 3px 9px gray;"><a href="#tab_content2" id="profile-tab" role="tab" data-toggle="tab" aria-expanded="false" onclick="div_close('none','3')">明細表</a>
            </li>
        </ul>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <asp:TextBox ID="TextBox_SaveColumn" runat="server" Style="display: none"></asp:TextBox>
                <asp:Button ID="Button_SaveColumns" runat="server" Text="Button" OnClick="Button_SaveColumns_Click" Style="display: none" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="myTabContent" class="tab-content">
            <div role="tabpanel" class="tab-pane fade active in" id="tab_content1" aria-labelledby="home-tab">
                <div class="x_panel Div_Shadow">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12" id="table_information">
                            <div id="des_Defective_img"></div>
                        </div>
                        <div id="_select" class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="dashboard_graph x_panel">
                                    <div class="x_content">
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
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>生產型別</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8">
                                                <asp:DropDownList ID="dropdownlist_Type" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">
                                                    <asp:ListItem Value="全部" Selected="True">全部</asp:ListItem>
                                                    <asp:ListItem Value="維護">維護</asp:ListItem>
                                                    <asp:ListItem Value="報工">報工</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>X座標(值)</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8">
                                                <asp:DropDownList ID="dropdownlist_X" runat="server" CssClass="btn btn-default dropdown-toggle" Width="100%">

                                                    <%--<asp:ListItem Value="人員名稱" >人員名稱</asp:ListItem>--%>
                                                    <asp:ListItem Value="設備名稱" Selected="True">設備名稱</asp:ListItem>

                                                    <asp:ListItem Value="類型">不良類型</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>


                                        <asp:TextBox ID="TextBox_MachTypeText" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:TextBox ID="TextBox_MachTypeValue" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:TextBox ID="TextBox_MachGroupText" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:TextBox ID="TextBox_MachGroupValue" runat="server" Style="display: none"></asp:TextBox>


                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>起始日期</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:TextBox ID="txt_str" runat="server" Style="" TextMode="Date" CssClass="form-control   text-center"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-6 col-xs-12">
                                            <div class="col-md-4 col-sm-3 col-xs-4">
                                                <span>結束日期</span>
                                            </div>
                                            <div class="col-md-8 col-sm-9 col-xs-8" style="margin: 0px 0px 5px 0px">
                                                <asp:TextBox ID="txt_end" runat="server" CssClass="form-control  text-center" TextMode="Date"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-6 col-xs-12" style="display: none">
                                            <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                                <span>顯示筆數</span>
                                            </div>
                                            <div class="col-md-5 col-sm-12 col-xs-5" style="margin: 0px 0px 5px 0px">
                                                <asp:TextBox ID="txt_showCount" runat="server" Text="10" CssClass="form-control text-center" TextMode="Number"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3 col-sm-12 col-xs-3">
                                                <span>
                                                    <asp:CheckBox ID="CheckBox_All" runat="server" Text="全部" onclick="checkstatus('ContentPlaceHolder1_CheckBox_All','ContentPlaceHolder1_txt_showCount')" />
                                                </span>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                                <br />
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <div class="col-md-9 col-xs-8">
                                    </div>
                                    <div class="col-md-3 col-xs-12">
                                        <button id="btncheck" type="button" class="btn btn-primary antosubmit2 ">執行搜索</button>
                                        <asp:Button runat="server" Text="提交" ID="Button_select" CssClass="btn btn-primary" Style="display: none" OnClick="button_select_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane fade" id="tab_content3" aria-labelledby="profile-tab2">
                <div id="des_Defective_chart"></div>
            </div>
            <div role="tabpanel" class="tab-pane fade" id="tab_content2" aria-labelledby="profile-tab">
                <div id="des_Defective"></div>
            </div>
        </div>
    </div>
    <div id="div_button" class="button_use" onclick="hide_div()">
        <i id="iconimage" class="fa fa-chevron-left"></i>
    </div>
    <%=Use_Javascript.Quote_Javascript()%>
    <script>
        create_imgcode('des_Defective_img', 'export_image', 'chartContainer')
        //產生圖片
        set_column('chartContainer', '不良品統計圖', '<%=subtitle%>', '<%=dropdownlist_X.SelectedItem.Value.Replace("設備名稱","數量") %>', '數量', '數量', [<%=col_data_Points%>]);

        //產生表格的HTML碼
        create_tablecode('des_Defective', '不良品<%=dropdownlist_X.SelectedItem.Value.Replace("設備名稱","數量") %>明細表', 'datatable-buttons', '<%=th.ToString() %>', '<%=tr.ToString() %>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        //產生表格的HTML碼
        create_tablecode('des_Defective_chart', '不良品<%=dropdownlist_X.SelectedItem.Value.Replace("設備名稱","數量") %>統計表', 'datatable-buttonss', '<%=th_chart.ToString() %>', '<%=tr_chart.ToString() %>');
        //產生相對應的JScode
        set_Table('#datatable-buttonss');
        //防止頁籤跑版
        loadpage('', '');

        $("#btncheck").click(function () {
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
                document.getElementById('<%=Button_select.ClientID %>').click();
            }
            else
                alert('請選擇群組');
        });

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
                    $('#datatable-buttons > thead > #tr_row > th').each(function () {
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
            set_column('chartContainer', '不良品統計圖', '<%=subtitle%>', '<%=dropdownlist_X.SelectedItem.Text %>', '數量', '數量', [<%=col_data_Points%>]);

        }
        $(".iconimage").on("click", function () {
            hide_div();
        });
        function div_close(ok, pages) {
            document.getElementById("div_button").style.display = ok;

            var title = document.getElementById("titlename");
            var date = new Date();
            var dateformat = date.getFullYear() + ("0" + (date.getMonth() + 1)).slice(-2) + ("0" + date.getDate()).slice(-2) + ("0" + date.getHours()).slice(-2) + ("0" + date.getMinutes()).slice(-2) + ("0" + date.getSeconds()).slice(-2);

            if (pages == '1')
                title.innerHTML = '不良品原因分析 | <%= WebUtils.GetAppSettings("Company_Name") %>';
            else if (pages == '2')
                title.innerHTML = '不良品<%=dropdownlist_X.SelectedItem.Value.Replace("設備名稱","數量") %>統計表' + dateformat;
            else if (pages == '3')
                title.innerHTML = '不良品<%=dropdownlist_X.SelectedItem.Value.Replace("設備名稱","數量") %>明細表' + dateformat;
        }
    </script>
</asp:Content>

