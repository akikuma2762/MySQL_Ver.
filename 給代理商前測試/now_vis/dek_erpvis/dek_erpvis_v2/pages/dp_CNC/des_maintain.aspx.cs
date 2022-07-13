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

namespace dek_erpvis_v2.pages.statistics
{
    public partial class maintain : System.Web.UI.Page
    {
        //-------------------------------------------------參數 OR 引用------------------------------------------------------------
        ERP_cnc cnc = new ERP_cnc();
        public string color = "";
        public string path = "";
        public string title = "";
        public string subtitle = "";
        public string xText = "";
        public string col_data_Points = "";
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
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check("des_maintain", acc))
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
                condition += condition == "" ? $" a.設備名稱='{item}' " : $" OR a.設備名稱='{item}' ";
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
            Set_Table(dt_monthtotal, out th, out tr);
        }
        //設定廠區之下拉選單
        private void Set_Dropdownlist()
        {
            CNCError.Set_FactoryDropdownlist(acc, DropDownList_MachType, Request.FilePath);
        }

        //取得入站時間在本月的資料
        private void Get_MonthTotal()
        {
            string type = dropdownlist_Type.SelectedItem.Text != "全部" ? $" and a.報工類型 = '進站{dropdownlist_Type.SelectedItem.Text}'   " : "";
            dt_monthtotal = cnc.Des_Maintain(txt_str.Text.Replace("-", ""), txt_end.Text.Replace("-", ""), type, condition);
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
                dt_monthtotal = EachProcess(dt_monthtotal);
            //0628 juiedit --沒有結束時間就不要出現在報表
            var dt_monthtotal_Finish = dt_monthtotal.AsEnumerable().Where(w => !string.IsNullOrEmpty(w.Field<string>("出站時間")));
            if (dt_monthtotal_Finish.FirstOrDefault() != null)
                dt_monthtotal = dt_monthtotal_Finish.CopyToDataTable();
        }
        private void ColNameChange()
        {
            //colname change-0524-juiedit 以後用ini掃 相關文字就替換掉
            dt_monthtotal.Columns["除外工時"].ColumnName = "除外工時(分)";
            dt_monthtotal.Columns["生產工時"].ColumnName = "生產工時(分)";
        }
        //輸出圖形
        private void Set_Chart()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                DataTable dt_Copy = dt_monthtotal.Copy();
                if (dropdownlist_X.SelectedItem.Text == "設備名稱")
                    dt_Copy = HtmlUtil.Print_DataTable(dt_Copy, "人員名稱,工單號碼,人員代碼", true);
                string y = "";
                switch (dropdownlist_Y.SelectedItem.Text)
                {
                    case "數量":
                        col_data_Points = HtmlUtil.Set_Chart(dt_Copy, dropdownlist_X.SelectedItem.Text, "完工數量", "", out _);
                        y = "完工數量";
                        unit = "完工數量";
                        break;
                    case "時間(分)":
                        col_data_Points = HtmlUtil.Set_Chart(dt_Copy, dropdownlist_X.SelectedItem.Text, "生產工時(分)", "", out _);
                        y = "生產工時(分)";
                        unit = "生產工時";
                        break;
                    case "工單":
                        col_data_Points = HtmlUtil.Set_Chart(dt_Copy, dropdownlist_X.SelectedItem.Text, "工單數量", "", out _);
                        y = "工單數量";
                        unit = "工單數量";
                        break;
                }

                DataTable dt_chart = HtmlUtil.PrintChart_DataTable(dt_Copy, dropdownlist_X.SelectedItem.Text, y);
                if (HtmlUtil.Check_DataTable(dt_chart))
                {
                    Set_Table(dt_chart, out th_chart, out tr_chart);
                    //th_chart.Replace("生產工時", "生產工時");
                }
                else
                    HtmlUtil.NoData(out th_chart, out tr_chart);

                subtitle = $"{txt_str.Text.Replace("-", "/")}~{txt_end.Text.Replace("-", "/")}";
            }
            else
            {
                subtitle = "無資料";
                HtmlUtil.NoData(out th_chart, out tr_chart);

            }

        }
        //輸出表格
        private void Set_Table(DataTable dt, out StringBuilder ths, out StringBuilder trs)
        {
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                order_list = order_list.Distinct().ToList();
                ths = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                trs = HtmlUtil.Set_Table_Content(true, dt, order_list, maintain_callback);
            }
            else
                HtmlUtil.NoData(out ths, out trs);
        }
        //例外處理
        private string maintain_callback(DataRow row, string field_name)
        {
            string value = "";
            //小計的處理
            if (field_name == "完工數量")
            {
                try
                {
                    value = (DataTableUtils.toInt(row["出站數量"].ToString()) - DataTableUtils.toInt(row["入站數量"].ToString())).ToString();
                }
                catch
                {
                    value = row["完工數量"].ToString();
                }

            }
            else if (field_name == "入站時間" || field_name == "出站時間" || field_name == "暫停時間" || field_name == "取消暫停時間")
            {
                if (!string.IsNullOrEmpty(row[field_name].ToString()))
                    value = string.Format("{0:g}", ShareFunction.StrToDate(row[field_name].ToString()));
            }
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

            DataTable dt_Copy = dt_source.Copy();
            //移除影響計算之欄位

            dt_source.Columns.Remove("暫停時間");
            dt_source.Columns.Remove("取消暫停時間");

            //合併唯一值
            dt_source = dt_source.DefaultView.ToTable(true);

            //填入除外工時 生產工時 完工數量
            string sqlcmd = "";
            foreach (DataRow row in dt_source.Rows)
            {
                double out_time = 0;
                double product_time = 0;
                double finish_qty = 0;

                sqlcmd = $"設備名稱='{row["設備名稱"]}' and 人員名稱 ='{row["人員名稱"]}' and 工單號碼='{row["工單號碼"]}' and 報工類型='{row["報工類型"]}' and 品號='{row["品號"]}' and 入站時間='{row["入站時間"]}' and 出站時間='{row["出站時間"]}'";
                DataRow[] rows = dt_Copy.Select(sqlcmd);
                //取得暫停時間
                if (rows != null && rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (DataTableUtils.toString(rows[i]["暫停時間"]) != "" && DataTableUtils.toString(rows[i]["取消暫停時間"]) != "")
                            out_time += ShareFunction.WorkTimeCaculator(DataTableUtils.toString(rows[i]["暫停時間"]), DataTableUtils.toString(rows[i]["取消暫停時間"])).TotalMinutes;
                        else
                            out_time += 0;
                    }
                }
                if (DataTableUtils.toString(row["入站時間"]) != "" && DataTableUtils.toString(row["出站時間"]) != "")
                    product_time = ShareFunction.WorkTimeCaculator(DataTableUtils.toString(row["入站時間"]), DataTableUtils.toString(row["出站時間"])).TotalMinutes;

                if (DataTableUtils.toString(row["出站數量"]) != "")
                    finish_qty = DataTableUtils.toDouble(DataTableUtils.toString(row["出站數量"])) - DataTableUtils.toDouble(DataTableUtils.toString(row["入站數量"]));

                row["除外工時"] = out_time;
                row["生產工時"] = product_time;
                row["完工數量"] = finish_qty;
            }
            return dt_source;
        }
    }
}