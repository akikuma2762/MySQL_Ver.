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
    public partial class des_Defective : System.Web.UI.Page
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
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
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
                condition += condition == "" ? $" mach_show_name='{item}' " : $" OR mach_show_name='{item}' ";
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
            string type = dropdownlist_Type.SelectedItem.Text != "全部" ? $" and type_mode = '進站{dropdownlist_Type.SelectedItem.Text}'  " : "";
            dt_monthtotal = cnc.Des_Defective(type, txt_str.Text.Replace("-", ""), txt_end.Text.Replace("-", ""), condition);

        }
        //輸出圖形
        private void Set_Chart()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                DataTable dt_Copy = dt_monthtotal.Copy();

                DataTable dt_chart = HtmlUtil.PrintChart_DataTable(dt_Copy, dropdownlist_X.SelectedItem.Text, "不良數量");
                if (HtmlUtil.Check_DataTable(dt_chart))
                    Set_Table(dt_chart, out th_chart, out tr_chart);
                else
                    HtmlUtil.NoData(out th_chart, out tr_chart);
                col_data_Points = HtmlUtil.Set_Chart(dt_Copy, dropdownlist_X.SelectedItem.Text, "不良數量", "", out _);
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
                trs = HtmlUtil.Set_Table_Content(true, dt, order_list, des_Pause_callback);
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
            return value == "" ? $"<td style=\"vertical-align: middle; text-align: center;\"> {row[field_name]} </td>" : $"<td style=\"vertical-align: middle; text-align: center;\"> {value} </td>";
        }

    }
}