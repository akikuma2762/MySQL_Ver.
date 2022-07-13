using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dekERP_dll;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_SD
{
    public partial class stockanalysis : System.Web.UI.Page
    {
        public string color = "";
        public string nstock = "";
        public string ostock = "";
        public string _val總庫存 = "";
        public string _val一般庫存 = "";
        public string _val逾期庫存 = "";
        public string Table_Title = "逾期庫存數量";
        public string pie_data_points = "";
        public string col_data_points_nor = "";
        public string col_data_points_sply = "";
        public string th = "";
        public string tr = "";
        public string date_str = "";
        public string date_end = "";
        public string title_msg = "";
        public string title_msg_list = "";
        public string title_text = "";
        public string path = "";
        public string timerange = "";
        public string range = "";
        public string SubTotal = WebUtils.GetAppSettings("show_Subtotal");
        string title = "";
        string sql_condi = "";
        string URL_NAME = "";
        string acc = "";
        string[] str = null;
        int total = 30;
        int CUST_TOTAL;
        DataTable dw = null;
        DataTable public_dt = null;
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        DataTable Line = new DataTable();
        iTec_House WHE = new iTec_House();
        List<string> linelist = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("WHE");
                color = HtmlUtil.change_color(acc);
                if (CheckBox_All.Checked == true)
                    total = 1;
                else
                    total = DataTableUtils.toInt(txt_showCount.Text);

                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                            load_page_data();
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
        private void load_page_data()
        {
            Response.BufferOutput = false;
            set_col_value();
            Set_Html_Table();
        }
        private void set_col_value()
        {
            title_text = "'庫存期限大於" + DataTableUtils.toString(total) + "天'";

            string LINE_GROUP = "";
            float 逾期數量 = 0, 一般庫存 = 0, 總數量 = 0;
            float TOTAL_逾期數量 = 0, TOTAL_一般庫存 = 0, TOTAL_總數量 = 0;
            int i = 0, j = 0;

            public_dt = WHE.stockanalysis(dekModel.Image, DataTableUtils.toInt(DataTableUtils.toString(DateTime.Now.AddDays(-(DataTableUtils.toDouble(txt_showCount.Text))).ToString("yyyyMMdd"))), "N");




            if (HtmlUtil.Check_DataTable(public_dt))
            {
                public_dt = myclass.Add_LINE_GROUP(public_dt).Table;
                public_dt = HtmlUtil.Get_Warehousingdate(public_dt, DataTableUtils.toString(DateTime.Now.AddDays(-(DataTableUtils.toDouble(txt_showCount.Text))).ToString("yyyyMMdd")));

                DataTable Line = public_dt.DefaultView.ToTable(true, new string[] { "產線群組" });
                foreach (DataRow row in Line.Rows)
                {
                    逾期數量 = 0; 一般庫存 = 0;
                    LINE_GROUP = DataTableUtils.toString(row["產線群組"]);
                    string sqlcmd = $"產線群組 ='{DataTableUtils.toString(row["產線群組"])}'";
                    DataRow[] rows = public_dt.Select(sqlcmd);
                    if (rows.Length > 0)
                    {
                        for (int x = 0; x < rows.Length; x++)
                        {
                            逾期數量 += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["逾期數量"]));
                            一般庫存 += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["一般數量"]));
                            TOTAL_逾期數量 += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["逾期數量"]));
                            TOTAL_一般庫存 += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["一般數量"]));
                            TOTAL_總數量 += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["總數量"]));
                        }
                        col_data_points_sply += "{ y: " + 逾期數量 + ", label: '" + LINE_GROUP + "' },";
                        col_data_points_nor += "{ y: " + 一般庫存 + ", label: '" + LINE_GROUP + "' },";
                    }
                }
            }
            set_pie_value(TOTAL_逾期數量, TOTAL_一般庫存, TOTAL_總數量);
        }
        private void set_pie_value(float 逾期數量, float 一般庫存, float 總數量)
        {
            string _per一般庫存 = DataTableUtils.toString(一般庫存 / 總數量 * 100).Split('.')[0];
            string _per逾期庫存 = DataTableUtils.toString(逾期數量 / 總數量 * 100).Split('.')[0];

            pie_data_points = "{y:" + _per一般庫存 + ", name:'一般庫存 " + _per一般庫存 + "%' , label:'一般庫存',color:'#5b59ac'}," +
                              "{y:" + _per逾期庫存 + ",name:'逾期庫存 " + _per逾期庫存 + "%', label:'逾期庫存',color:'#ff4d4d',exploded: true}";
            nstock = _per一般庫存 + "%";
            ostock = _per逾期庫存 + "%";

            _val總庫存 = DataTableUtils.toString(總數量);
            _val一般庫存 = DataTableUtils.toString(一般庫存);
            _val逾期庫存 = DataTableUtils.toString(逾期數量);
        }
        private void Set_Html_Table()
        {

            public_dt = WHE.stockanalysis(dekModel.Table, DataTableUtils.toInt(DataTableUtils.toString(DateTime.Now.AddDays(-(DataTableUtils.toDouble(txt_showCount.Text))).ToString("yyyyMMdd"))), "N");

            if (HtmlUtil.Check_DataTable(public_dt))
            {
                public_dt = myclass.Add_LINE_GROUP(public_dt).Table;
                public_dt = HtmlUtil.Get_Warehousingdate(public_dt, DataTableUtils.toString(DateTime.Now.AddDays(-(DataTableUtils.toDouble(txt_showCount.Text))).ToString("yyyyMMdd")), true);

                if (date_str == "" && date_end == "")
                {
                    date_str = myclass.date_trn(DataTableUtils.toString(total));
                    date_end = "";
                }
                DataTable Name = public_dt.DefaultView.ToTable(true, new string[] { "客戶簡稱" });
                Line = public_dt.DefaultView.ToTable(true, new string[] { "產線群組" });

                foreach (DataRow row in Line.Rows)
                {
                    Name.Columns.Add(row["產線群組"].ToString());
                    linelist.Add(row["產線群組"].ToString());
                }
                Name.Columns.Add("小計");

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(Name, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, Name, order_list, stockanalysis_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string stockanalysis_callback(DataRow row, string field_name)//這裡之後要重新寫過，太吃效能了
        {
            string value = "";
            if (field_name != "客戶簡稱" && field_name != "小計")
            {
                //進入第一個產線後，將上一個總和歸0
                if (linelist.IndexOf(field_name) == 0)
                    CUST_TOTAL = 0;
                string sqlcmd = "客戶簡稱 ='" + DataTableUtils.toString(row["客戶簡稱"]) + "' and 產線群組 = '" + field_name + "'";
                DataRow[] rows = public_dt.Select(sqlcmd);
                int LINE_TOTAL = 0;
                if (rows.Length != 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        for (int j = 0; j < Line.Rows.Count; j++)
                        {
                            if (field_name == DataTableUtils.toString(rows[i]["產線群組"]) && field_name == DataTableUtils.toString(Line.Rows[j]["產線群組"]))
                                LINE_TOTAL += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["數量"]));
                        }
                    }
                }
                value = DataTableUtils.toString(LINE_TOTAL);
                CUST_TOTAL += LINE_TOTAL;
            }
            //小計的地方需要用到超連結
            if (field_name == "小計")
            {
                string url = HtmlUtil.AttibuteValue("cust_name", DataTableUtils.toString(row["客戶簡稱"]).Trim(), "") + "," +
                     HtmlUtil.AttibuteValue("date_str", date_str, "") + "," +
                     HtmlUtil.AttibuteValue("date_end", date_end, "");
                string href = string.Format("stockanalysis_details.aspx?key={0} ",
                    WebUtils.UrlStringEncode(url)
                     );
                CUST_TOTAL = 0;

                DataRow[] rows = public_dt.Select($"客戶簡稱='{row["客戶簡稱"]}'");
                for (int i = 0; i < rows.Length; i++)
                    CUST_TOTAL += DataTableUtils.toInt(rows[i]["數量"].ToString());

                value = DataTableUtils.toString(CUST_TOTAL);
                value = HtmlUtil.ToTag("u", HtmlUtil.ToHref(value, href));
            }
            if (field_name == "客戶簡稱")
                value = DataTableUtils.toString(row["客戶簡稱"]);
            return "<td>" + value + "</td>\n";
        }


        //event
        protected void button_select_Click(object sender, EventArgs e)
        {
            //    sql_condi = "and (SELECT MAX(ITEMIO.TRN_DATE) FROM ITEMIOS LEFT JOIN ITEMIO ON ITEMIOS.IO=ITEMIO.IO AND ITEMIOS.TRN_NO=ITEMIO.TRN_NO WHERE ITEMIOS.IO='2' AND ITEMIOS.MKORD_NO=MKORDSUB.TRN_NO AND ITEMIOS.MKORD_SN=MKORDSUB.SN AND ITEMIO.S_DESC< >'歸還' AND ITEMIO.MK_TYPE='成品入庫') <=" + myclass.date_trn(DataTableUtils.toString(total)) + "";
            title_text = "'庫存期限大於" + DataTableUtils.toString(total) + "天'";
            Table_Title = "逾期庫存數量";
            load_page_data();

        }
        protected void CheckBox_All_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_All.Checked)
                txt_showCount.Enabled = false;
            else
                txt_showCount.Enabled = true;
        }
    }
}