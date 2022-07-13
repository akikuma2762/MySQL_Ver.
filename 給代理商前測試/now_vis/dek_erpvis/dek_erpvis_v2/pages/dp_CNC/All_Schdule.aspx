<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="All_Schdule.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_CNC.All_Schdule" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>生產進度看板 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>
    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_SD/Orders_Details.css" rel="stylesheet" />
    <link href="../../Content/dp_CNC/Enter_ReportView.css" rel="stylesheet" />
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
        <%=path %>
        <br>
        <asp:TextBox ID="TextBox_MachTypeText" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_MachTypeValue" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_MachGroupText" runat="server" Style="display: none"></asp:TextBox>
        <asp:TextBox ID="TextBox_MachGroupValue" runat="server" Style="display: none"></asp:TextBox>
         <asp:TextBox ID="TextBox1" runat="server" onclick="GetMachineData()"></asp:TextBox>
        <div class="col-md-12 col-xs-12 col-sm-12"> 
            <div class="col-md-4">
            </div>
            <div class="col-md-1 col-sm-12 col-xs-12">
                <div class="col-md-5 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_Running.PNG" alt="..." width="30px" height="30px" />
                </div>
                <div class="col-md-7 col-sm-9 col-xs-9"><span>入站</span></div>
                <hr />
            </div>


            <div class="col-md-1 col-sm-12 col-xs-12">
                <div class="col-md-5 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_Maintain.png" alt="..." width="30px" height="30px" />
                </div>
                <div class="col-md-7 col-sm-9 col-xs-9"><span>維護</span></div>
                <hr />
            </div>
            <div class="col-md-1 col-sm-12 col-xs-12">
                <div class="col-md-5 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_Stopping.PNG" alt="..." width="30px" height="30px" />
                </div>
                <div class="col-md-7 col-sm-9 col-xs-9"><span>暫停</span></div>
                <hr />
            </div>

            <div class="col-md-2 col-sm-12 col-xs-12">
                <div class="col-md-3 col-sm-3 col-xs-3">
                    <img src="../../assets/images/Light_ExStopping.PNG" alt="..." width="30px" height="30px" />
                </div>
                <div class="col-md-9 col-sm-9 col-xs-9"><span>除外暫停</span></div>
                <hr />
            </div>
        </div>
        <div class="clearfix"></div>
        <div class="x_panel Div_Shadow">
            <div class="x_content">
                <div class="row">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBox_SaveColumn" runat="server" Style="display: none"></asp:TextBox>
                            <asp:Button ID="Button_SaveColumns" runat="server" Text="Button" OnClick="Button_SaveColumns_Click" Style="display: none" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <div class="col-md-12 col-sm-12 col-xs-12" id="table_information">
                        <div id="All_Schdule"></div>
                    </div>
                    <div id="_select" class="col-md-3 col-sm-12 col-xs-12 _select _setborder">
                        <div class="x_panel">

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
                                <div class="col-md-4 col-sm-12 col-xs-4" style="margin: 5px 0px 5px 0px">
                                    <span>選擇欄位</span>
                                </div>
                                <div class="col-md-8 col-sm-12 col-xs-8">
                                    <asp:CheckBoxList ID="CheckBoxList_Columns" runat="server" RepeatColumns="2"></asp:CheckBoxList>
                                </div>
                            </div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="col-md-9 col-xs-8">
                                </div>
                                <div class="col-md-3 col-xs-12">
                                    <button id="btncheck" type="button" class="btn btn-primary antosubmit2">執行搜索</button>
                                    <asp:Button ID="Button_Check" runat="server" Text="Button" OnClick="Button_Check_Click" Style="display: none" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="div_button" class="button_use" onclick="hide_div()">
        <i id="iconimage" class="fa fa-chevron-left"></i>
    </div>

    <%=Use_Javascript.Quote_Javascript() %>
    <link href="../../assets/vendors/Create_HtmlCode/jquery.dataTables.min.css" rel="stylesheet" />

    <script>

        //觸發事件
        $(document).ready(function () {
            //-------------------恢復原本搜尋機制-0620 juiedit
            //清空搜尋內容-- 
            //var div_content = document.getElementById('datatable-buttons_filter');
            //div_content.innerHTML = '';
            ////填入搜尋控制項
            //div_content.innerHTML = '<label>請輸入機台:<input type="text" class="form-control input-sm" placeholder="" id="machine" name="machine"></label>';
            //恢復原本搜尋機制-0620 juiedit
            var table = $('#datatable-buttons').DataTable();
            //---------------------恢復原本搜尋機制-0620 juiedit 
            //監聽事件(大概)
            //$('#machine').keyup(function () {
            //    table.draw();
            //});
            //
            //$(".dataTables_filter").parent(".pull-left").css("margin-left", "-120px");
            //--------------------
            $(".paginate_button ").css("min-width", "0em");
            $(".paginate_button ").css("padding", "0em 0em");
            $(".paginate_button ").css("margin-left", "0px");
            $(".paginate_button ").css("display", "initial");

            //前頁
            $('#datatable-buttons_previous').click(function () {
                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });

            //次頁
            $('#datatable-buttons_next').click(function () {
                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });
            //數字
            $('#datatable-buttons_paginate').click(function () {
                $(".paginate_button ").css("min-width", "0em");
                $(".paginate_button ").css("padding", "0em 0em");
                $(".paginate_button ").css("margin-left", "0px");
                $(".paginate_button ").css("display", "initial");
            });

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
            //搜尋動作
            $('#btncheck').click(function () {
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
                    document.getElementById('<%=Button_Check.ClientID %>').click();
                }
                else
                    alert('請選擇群組');
            });

        });

        //只搜尋機台名稱，並顯示包含其字串機台(可惜會影響到其他表單)
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var machine = $('#machine').val();

                var tharray = [];
                //取得欄位名稱及順序
                $('#tr_row > th').each(function () {
                    tharray.push($(this).text())
                })
                var machinename = data[tharray.indexOf('設備名稱')]; // 要抓取浮動欄位(因為欄位可以左右移動)
                var nowinformation = data[0];//避免第二個datatable被影響到

                if (machine == undefined)//第一次剛產生讀不到，所以為undefined
                    return true;
                if (machine == '')//第二次產生後，讀到的值為空白
                    return true;
                else if (machinename == machine)//若字串包含，則顯示相關內容
                    return true;
                else if (nowinformation == machine)//若字串包含，則顯示相關內容
                    return true;
                else
                    return false;
            }
        );

        //產生表格的HTML碼
        create_tablecode_noshdrow('All_Schdule', '生產進度看板', 'datatable-buttons', '<%=th%>', '<%=tr%>');
        //產生相對應的JScode
        set_Table('#datatable-buttons');
        //防止頁籤跑版
        loadpage('', '');

        //表格閃爍機制
        var mTimer;
        var Timer_Count = 0;//閃爍機制
        var loadTime = <%=Refresh_Time%>;
        mTimer = setTimeout(function () { GetMachineData(); }, loadTime);

        //更新最外層的DataTable
        function GetMachineData() {
            clearTimeout(mTimer);
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: "../../webservice/dpCNC_MachData.asmx/GetMachineList",
                data: { machine: '<%=mach %>', type: '進站報工' },
                success: function (xml) {
                    $(xml).find("ROOT_PIE").each(function (i) {
                        if ($(xml).find("ROOT_PIE").length > 0) {
                            $(this).children().each(function (j) {
                                var btn = $(this).attr("工單報工").valueOf();
                                var group = $(this).attr("設備群組").valueOf();
                                var name = $(this).attr("設備名稱").valueOf();
                                var name_number = $(this).attr("設備代號").valueOf();
                                var manu_id = $(this).attr("工單號碼").valueOf();
                                var product_number = $(this).attr("品號").valueOf();
                                var product_name = $(this).attr("品名").valueOf();
                                var custom_name = $(this).attr("客戶名稱").valueOf();
                                var exp_count = $(this).attr("預計產量").valueOf();
                                var now_count = $(this).attr("已生產量").valueOf();
                                var yet_count = $(this).attr("未生產量").valueOf();
                                var process_number = $(this).attr("製程代號").valueOf();
                                var process_name = $(this).attr("製程名稱").valueOf();
                                var status = $(this).attr("工單狀態").valueOf();
                                var workstaff = $(this).attr("人員名稱").valueOf();
                                var now_time = $(this).attr("開工時間").valueOf();
                                var persent = $(this).attr("進度").valueOf();
                                var todayproduct = $(this).attr("今日產量").valueOf();

                                var tablearray = ['工單報工', btn,
                                    '設備群組', group,
                                    '設備名稱', name,
                                    '工單號碼', manu_id,
                                    '品號', product_number,
                                    '品名', product_name,
                                    '客戶名稱', custom_name,
                                    '預計產量', exp_count,
                                    '當下產量', now_count,
                                    '未生產量', yet_count,
                                    '製程代號', process_number,
                                    '製程名稱', process_name,
                                    '工單狀態', status,
                                    '人員名稱', workstaff,
                                    '已生產量', todayproduct,
                                    '進度', persent,
                                    '開工時間', now_time]

                                //取得目前欄位排序順序
                                var tharray = [];
                                $('#tr_row > th').each(function () {
                                    tharray.push($(this).text())
                                })

                                for (var j = 0; j < tharray.length; j++) {
                                    var td_column = name_number + '_' + tharray[j] + '_' + manu_id;
                                    var td_value = tablearray[tablearray.indexOf(tharray[j]) + 1];

                                    if (tharray[j] == '工單報工') {
                                        try {
                                            document.getElementById(td_column).innerHTML = check_value(td_value);
                                        }
                                        catch {

                                        }
                                    }
                                    else {
                                        try {
                                            if (tharray[j] == '預計產量' || tharray[j] == '已生產量' || tharray[j] == '未生產量')
                                                document.getElementById(td_column).innerHTML = check_tdvalue(td_value);
                                            else
                                                document.getElementById(td_column).innerHTML = check_value(td_value);
                                        }
                                        catch {

                                        }
                                    }
                                }

                                //非數字欄位空白補換行
                                function check_value(value) {
                                    if (value == '')
                                        value = '\n';
                                    return value;
                                }

                                //數字欄位空白補0
                                function check_tdvalue(value) {
                                    if (value.length == '0' || value.length == 0)
                                        value = '0';
                                    return value;
                                }
                                //alert("AAAA");
                            })
                        }
                    });
                },
                error: function (data, errorThrown) {

                }
            });
            mTimer = setTimeout(function () { GetMachineData(); }, loadTime);
            Timer_Count++;
        }


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

        }
        $(".iconimage").on("click", function () {
            hide_div();
        });
    </script>
</asp:Content>
