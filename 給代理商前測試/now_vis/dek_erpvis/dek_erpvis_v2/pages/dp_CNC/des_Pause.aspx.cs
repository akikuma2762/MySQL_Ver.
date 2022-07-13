using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Support;
using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using System.Data.OleDb;
using dekERP_dll;
using dekERP_dll.dekErp;
using System.Configuration;
using System.Text;


namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class des_Pause : System.Web.UI.Page
    {
        //-------------------------------------------------參數 OR 引用------------------------------------------------------------
        ERP_cnc cnc = new ERP_cnc();
        public string color = "";
        public string path = "";
        public string title = "";
        public string subtitle = "";
        public string xText = "";
        public string col_data_Points = "";
        public string col_data_Points_report = "";
        public string date_str = "";
        public string date_end = "";
        public string unit = "";
        public StringBuilder th = new StringBuilder();
        public StringBuilder tr = new StringBuilder();
        public StringBuilder th_chart = new StringBuilder();
        public StringBuilder tr_chart = new StringBuilder();
        string acc = "";
        string condition = "";
        DataTable dt_monthtotal = new DataTable();
        myclass myclass = new myclass();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("DES");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check("des_Pause", acc))
                {
                    if (!IsPostBack)
                    {
                        string[] date = 德大機械.德大專用月份(acc).Split(',');
                        txt_str.Text = HtmlUtil.changetimeformat(date[0], "-");
                        txt_end.Text = HtmlUtil.changetimeformat(date[1], "-");
                        MainProcess();
                    }
                }
                else
                    Response.Write("<script>alert('您無此權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }

        //執行搜尋事件
        protected void button_select_Click(object sender, EventArgs e)
        {
            DropDownList_MachType.SelectedIndex = DropDownList_MachType.Items.IndexOf(DropDownList_MachType.Items.FindByValue(TextBox_MachTypeValue.Text));
            DropDownList_MachGroup.Items.Clear();
            List<string> list = new List<string>(TextBox_MachTypeValue.Text.Split(','));
            for (int i = 0; i < list.Count - 1; i++)
            {
                ListItem listItem = new ListItem(list[i], list[i + 1]);
                DropDownList_MachGroup.Items.Add(listItem);
                i++;
            }
            DropDownList_MachGroup.SelectedIndex = DropDownList_MachGroup.Items.IndexOf(DropDownList_MachGroup.Items.FindByText(TextBox_MachGroupText.Text));

            foreach (string item in TextBox_Machines.Text.Split(','))
                condition += condition == "" ? $" a.機台名稱='{item}' " : $" OR a.機台名稱='{item}' ";
            condition = condition == "" ? "" : $" and ( {condition} ) ";

            MainProcess();
        }

        //儲存欄位順序
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        //-----------------------------------------------------Function---------------------------------------------------------------------
        //需要執行的程式
        private void MainProcess()
        {
            Set_Dropdownlist();
            Get_MonthTotal();
            ColNameChange();
            Set_Chart();
            Set_Table(dt_monthtotal, out th, out tr, dropdownlist_Z.SelectedItem.Text);
        }
        //設定廠區之下拉選單
        private void Set_Dropdownlist()
        {
            CNCError.Set_FactoryDropdownlist(acc, DropDownList_MachType, Request.FilePath);
        }
        //置換掉欄位名稱
        private void ColNameChange()
        {
            //colname change-0524-juiedit 以後用ini掃 相關文字就替換掉
            //繼承底層 才不用每次寫 兒子沒改寫就用父親
            dt_monthtotal.Columns["暫停工時"].ColumnName = "暫停工時(分)";
            dt_monthtotal.Columns["除外暫停工時"].ColumnName = "除外暫停工時(分)";
        }

        //取得入站時間在本月的資料
        private void Get_MonthTotal()
        {
            string type = dropdownlist_Type.SelectedItem.Text != "全部" ? $" AND a.模式 = '進站{dropdownlist_Type.SelectedItem.Text}' " : "";
            dt_monthtotal = cnc.Des_Pause(type, txt_str.Text.Replace("-", ""), txt_end.Text.Replace("-", ""), condition);

            if (HtmlUtil.Check_DataTable(dt_monthtotal))
                dt_monthtotal = EachProcess(dt_monthtotal);
        }
        //輸出圖形
        private void Set_Chart()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                DataTable dt_Copy = dt_monthtotal.Copy();
                string remove = "";
                //先經過處理類型
                switch (dropdownlist_Z.SelectedItem.Text)
                {
                    case "暫停":
                        dt_Copy = HtmlUtil.Print_DataTable(dt_Copy, "除外暫停類型,除外暫停時間,結案時間,除外暫停工時(分)", true, "暫停類型");
                        remove = "暫停類型";
                        break;
                    case "除外暫停":
                        dt_Copy = HtmlUtil.Print_DataTable(dt_Copy, "暫停類型,暫停時間,取消暫停時間,暫停工時(分)", true, "除外暫停類型");
                        remove = "除外暫停類型";
                        break;
                    case "總暫停":
                        dt_Copy = HtmlUtil.Merge_DataTable(dt_Copy, "暫停類型,暫停時間,取消暫停時間,暫停工時(分)", "除外暫停類型,除外暫停時間,結案時間,除外暫停工時(分)");
                        break;
                }


                switch (dropdownlist_X.SelectedItem.Text)
                {
                    case "設備名稱":
                        dt_Copy = HtmlUtil.Print_DataTable(dt_Copy, "人員名稱,工單號碼", true);
                        break;
                    case "異常類型":
                        dt_Copy = HtmlUtil.Print_DataTable(dt_Copy, "人員名稱,工單號碼", true, remove);
                        break;
                }

                string x_value = dropdownlist_X.SelectedItem.Text != "異常類型" ? dropdownlist_X.SelectedItem.Text : dropdownlist_Z.SelectedItem.Text == "除外暫停" ? "除外暫停類型" : "暫停類型";
                string y_value = dropdownlist_Y.SelectedItem.Text == "數量" ? "次數" : dropdownlist_Z.SelectedItem.Text == "除外暫停" ? "除外暫停工時(分)" : "暫停工時(分)";

                DataTable dt_chart = HtmlUtil.PrintChart_DataTable(dt_Copy, x_value, y_value);
                if (HtmlUtil.Check_DataTable(dt_chart))
                    Set_Table(dt_chart, out th_chart, out tr_chart);
                else
                    HtmlUtil.NoData(out th_chart, out tr_chart);
                Set_Chart_Ex(dt_Copy, x_value, y_value);

            }
            else
            {
                HtmlUtil.NoData(out th_chart, out tr_chart);
                subtitle = "無資料";
            }

        }
        //雙色處理 0601 juiedit-後來決議不做用tooltip代替
        private void Set_Chart_Ex(DataTable dt_Copy, string x_value, string y_value)
        {
            col_data_Points = HtmlUtil.Set_Chart(dt_Copy, x_value, y_value, "", out _);
            subtitle = $"{txt_str.Text.Replace("-", "/")}~{txt_end.Text.Replace("-", "/")}";
        }
        //輸出表格
        private void Set_Table(DataTable dt, out StringBuilder ths, out StringBuilder trs, string text = "")
        {
            if (HtmlUtil.Check_DataTable(dt))
            {
                if (text != "")
                {
                    string cloumn = "";
                    if (text == "暫停")
                        cloumn = "暫停類型";
                    else if (text == "除外暫停")
                        cloumn = "除外暫停類型";

                    if (cloumn != "")
                    {
                        DataRow[] rows = dt.Select($"{cloumn} = '' OR {cloumn} IS NULL");
                        for (int i = 0; i < rows.Length; i++)
                            rows[i].Delete();

                        //暫停
                        if (cloumn == "暫停類型")
                        {
                            dt.Columns.Remove("除外暫停類型");
                            dt.Columns.Remove("除外暫停時間");
                            dt.Columns.Remove("結案時間");
                            dt.Columns.Remove("除外暫停工時(分)");
                        }
                        //除外暫停
                        else if (cloumn == "除外暫停類型")
                        {
                            dt.Columns.Remove("暫停時間");
                            dt.Columns.Remove("取消暫停時間");
                            dt.Columns.Remove("暫停類型");
                            dt.Columns.Remove("暫停工時(分)");
                        }
                    }
                    dt.AcceptChanges();
                }
                if (HtmlUtil.Check_DataTable(dt))
                {

                    List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                    order_list = order_list.Distinct().ToList();
                    ths = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                    trs = HtmlUtil.Set_Table_Content(true, dt, order_list, des_Pause_callback);

                }
                else
                    HtmlUtil.NoData(out ths, out trs);

            }
            else
                HtmlUtil.NoData(out ths, out trs);
        }
        //例外處理
        private string des_Pause_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name.IndexOf("時間") != -1 && row[field_name].ToString() != "")
                value = HtmlUtil.StrToDate(row[field_name].ToString()).ToString();
            return value == "" ? "" : $"<td style=\"vertical-align: middle; text-align: center;\"> {value} </td>";
        }
        //整理表格
        private DataTable EachProcess(DataTable dt_source)
        {
            for (int i = 0; i < dt_source.Columns.Count; i++)
            {
                dt_source.Columns[i].ReadOnly = false;
                if (dt_source.Columns[i].MaxLength < 3)
                {
                    //避免無法變更而造成ERROR
                    try
                    {
                        dt_source.Columns[i].MaxLength = 15;
                    }
                    catch
                    {

                    }
                }
            }
            foreach (DataRow row in dt_source.Rows)
            {
                if (DataTableUtils.toString(row["暫停類型"]) != "" && DataTableUtils.toString(row["除外暫停類型"]) == "")
                    row["暫停工時"] = ShareFunction.WorkTimeCaculator(DataTableUtils.toString(row["暫停時間"]), DataTableUtils.toString(row["取消暫停時間"])).TotalMinutes;
                if (DataTableUtils.toString(row["除外暫停類型"]) != "" && DataTableUtils.toString(row["暫停類型"]) == "")
                    row["除外暫停工時"] = ShareFunction.WorkTimeCaculator(DataTableUtils.toString(row["除外暫停時間"]), DataTableUtils.toString(row["結案時間"])).TotalMinutes;
            }
            return dt_source;
        }
    }
}