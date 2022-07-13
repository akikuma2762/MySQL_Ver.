using dek_erpvis_v2.cls;
using dekERP_dll.dekErp;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_SD
{
    public partial class recordsofchangetheorder1 : System.Web.UI.Page
    {
        public string color = "";
        public string date_str = "";//new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyyMMdd");
        public string date_end = ""; //new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1).ToString("yyyyMMdd");
        public string time_array = "";
        public string th = "";
        public string tr = "";
        public string col_data_Points = "";
        public string data_name = "";
        public string title_text = "";
        public string time_area_text = "訂單變更紀錄列表";
        public string path = "";
        public string timerange = "";
        string TOP = "";
        string URL_NAME = "";
        string acc = "";
        string[] s = null;
        DataTable public_dt = null;
        clsDB_Server clsDB_sw = new clsDB_Server("");
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        iTec_Sales SLS = new iTec_Sales();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("SLS");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            if (!IsPostBack)
                            {
                                string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                                date_str = daterange[0];
                                date_end = daterange[1];
                                load_page_data();
                            }
                            if (txt_str.Text == "" && txt_end.Text == "")
                            {
                                txt_str.Text = HtmlUtil.changetimeformat(date_str, "-");
                                txt_end.Text = HtmlUtil.changetimeformat(date_end, "-");
                            }
                        }
                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");

                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");

            }
            else
                Response.Redirect(myclass.logout_url);
        }
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        //function

        private void load_page_data()
        {
            col_data_Points = "";

            Response.BufferOutput = false;
            set_col_value();
            Set_Html_Table();

        }
        private void set_col_value()
        {
            //data
            data_name = "'變更次數'";
            if (CheckBox_All.Checked != true)
                TOP = DataTableUtils.toString(DataTableUtils.toDouble(DataTableUtils.toString(txt_showCount.Text)));
            else
                TOP = "0";
            time_array =$"{HtmlUtil.changetimeformat(date_str)}~{HtmlUtil.changetimeformat(date_end)}  累計訂單變更次數前 {TOP} 名客戶";
            string date_s = HtmlUtil.changetimeformat(date_str);
            string date_e = HtmlUtil.changetimeformat(date_end);
            timerange = date_s + " ~ " + date_e;
            /*----------------------------------------------------------------------------------------*/
            public_dt = new DataTable();

           
            public_dt = SLS.Get_recordsofchangetheorder(date_str,date_end,dekERP_dll.dekModel.Image, TOP);
           
            //-----------------------------------------------------------------------

            //-----------------------------------------------------------------------
            if (HtmlUtil.Check_DataTable(public_dt))
            {
                string title_val = "";
                int y_subtotal = 0;    // 變更總計
                col_data_Points = HtmlUtil.Set_Chart(public_dt, "客戶名稱", "變更次數", "", out y_subtotal);
                title_val = DataTableUtils.toString(y_subtotal);
                if (CheckBox_All.Checked == false)
                    title_text = $"'前{txt_showCount.Text}名客戶訂單變更： {title_val} 次'";
                else
                    title_text = $"'所有客戶訂單變更： {title_val} 次'";
            }
            else
                title_text = HtmlUtil.NoData(out th, out tr);

            public_dt.Dispose();
        }

    

        private void Set_Html_Table()
        {
            public_dt = new DataTable();
            public_dt = SLS.Get_recordsofchangetheorder(date_str, date_end, dekERP_dll.dekModel.Image,"0");
            //用於存放所有的欄位名稱(out的方式的話，可以直接回傳字串)
            if (HtmlUtil.Check_DataTable(public_dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(public_dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, public_dt, order_list).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
      
        //event
        protected void button_select_Click(object sender, EventArgs e)
        {
            if (DataTableUtils.toString(((Control)sender).ID.Split('_')[1]) == "select")
            {
                date_str = txt_str.Text.Replace("-", "");
                date_end = txt_end.Text.Replace("-", "");
                load_page_data();
            }
            else
            {
                string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                HtmlUtil.Button_Click(DataTableUtils.toString(((Control)sender).ID.Split('_')[1]), daterange, DataTableUtils.toString(txt_str.Text), DataTableUtils.toString(txt_end.Text), out date_str, out date_end);
                txt_str.Text = HtmlUtil.changetimeformat(date_str, "-");
                txt_end.Text = HtmlUtil.changetimeformat(date_end, "-");
            }
        }
        protected void CheckBox_All_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_All.Checked == true)
                txt_showCount.Enabled = false;
            else
                txt_showCount.Enabled = true;
        }

    }
}