using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using dek_erpvis_v2.webservice;
using System.Web.UI.WebControls;
using dekERP_dll;
using dekERP_dll.dekErp;


namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class materialrequirementplanning1 : System.Web.UI.Page
    {
        public string color = "";
        public string th = "";
        public string tr = "";
        public string title_text = "";
        public string type = "";
        public string type_code = "";
        public string title = "";
        public string search_condi = "";
        public string safty_text = "";
        public string min_text = "";
        public string chart_card_text = "";
        public string date_str = "";// DateTime.Now.ToString("yyyy0101");
        public string date_end = "";// DateTime.Now.ToString("yyyy1231");
        public string path = "";
        string URL_NAME = "";
        string acc = "";
        string[] str = null;
        double total_var;
        double month_var;
        public string timerange = "";
        DataTable dt = new DataTable();
        iTec_Materials PCD = new iTec_Materials();
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        List<string> uselist = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("PCD");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {

                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        string[] daterange = null;
                        if (!IsPostBack)
                        {
                            daterange = 德大機械.德大專用月份(acc).Split(',');
                            date_str = daterange[0];
                            date_end = daterange[1];
                            search_condi = "APLD.COD_ITEM ";
                            // get_type();
                            load_page_data();

                        }
                        if (txt_str.Text == "" && txt_end.Text == "")
                        {
                            txt_str.Text = HtmlUtil.changetimeformat(daterange[0], "-");
                            txt_end.Text = HtmlUtil.changetimeformat(daterange[1], "-");
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
        private void load_page_data()
        {
            Response.BufferOutput = false;
            Set_Html_Table();
        }
        private void Set_Html_Table()
        {
            string date_s = HtmlUtil.changetimeformat(date_str);
            string date_e = HtmlUtil.changetimeformat(date_end);
            safty_text = DataTableUtils.toString(DataTableUtils.toDouble(DataTableUtils.toString(demo_vertical3.Value)));
            min_text = DataTableUtils.toString(DataTableUtils.toDouble(DataTableUtils.toString(demo_vertical2.Value)));

            int length = 0;
            if (DropDownList_substring.SelectedValue == "Y")
                length = DataTableUtils.toInt(TextBox_substring.Text);

            if (RadioButtonList_select_type.SelectedItem.Value == "3" || TextBox_keyword.Text != "")
            {
                get_search_parameter();
                dt = PCD.materialrequirementplanning(date_str, date_end, search_condi, type, DropDownList_selectedcondi.SelectedValue, length.ToString());
            }

            string field = "";

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable use = dt.DefaultView.ToTable(true, new string[] { "用途說明" });
                DataTable item = dt.DefaultView.ToTable(true, new string[] { "品號" });

                item.Columns.Add("品名規格");
                foreach (DataRow row in use.Rows)
                {
                    item.Columns.Add(row["用途說明"].ToString());
                    uselist.Add(row["用途說明"].ToString());
                }
                item.Columns.Add("總計");
                item.Columns.Add("月用量");
                item.Columns.Add($"安全存量 (x{safty_text})");
                item.Columns.Add($"最小採購量 (x{min_text})");

                try
                {
                    item.Columns.Remove("Column1");
                }
                catch
                {

                }


                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(item, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, item, order_list, materialrequirementplanning_callback).ToString();

            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string materialrequirementplanning_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "品號")
            {
                string url = HtmlUtil.AttibuteValue("item_code", DataTableUtils.toString(row["品號"]), "") + "," +
                    HtmlUtil.AttibuteValue("date_str", date_str, "") + "," +
                    HtmlUtil.AttibuteValue("date_end", date_end, "");
                string href = string.Format("materialrequirementplanning_details.aspx?key={0}",
                    WebUtils.UrlStringEncode(url));

                value = DataTableUtils.toString(row["品號"]);
                value = $"<u><a href=\"{href}\">{value}</a></u>";
            }
            else if (field_name == "總計")
            {
                total_var = 0;
                string sqlcmd = $"品號 ='{DataTableUtils.toString(row["品號"])}'";
                DataRow[] rows = dt.Select(sqlcmd);
                if (rows != null)
                {
                    for (int i = 0; i < rows.Length; i++)
                        total_var += DataTableUtils.toInt(DataTableUtils.toString(rows[i]["領料數"]).Split('.')[0]);
                }
                value = DataTableUtils.toString(total_var);
            }
            else if (field_name == "月用量")
            {
                month_var = 0;
                month_var = total_var / DaysBetween(date_str, date_end);
                value = month_var.ToString("0.00");
            }
            else if (field_name == $"安全存量 (x{safty_text})")
            {
                double save = month_var * DataTableUtils.toDouble(demo_vertical3.Value);
                value = save.ToString("0.00");
            }
            else if (field_name == $"最小採購量 (x{min_text})")
            {
                double min = month_var * DataTableUtils.toDouble(demo_vertical2.Value);
                value = min.ToString("0.00");
            }

            else if (field_name == "品名規格")
            {
                string sqlcmd = $"品號 ='{DataTableUtils.toString(row["品號"])}'";
                DataRow[] rows = dt.Select(sqlcmd);
                value = DataTableUtils.toString(rows[0][field_name]);
            }
            else if (uselist.IndexOf(field_name) != -1)
            {
                string sqlcmd = $"品號 ='{DataTableUtils.toString(row["品號"])}' and 用途說明 ='{field_name}'";
                DataRow[] rows = dt.Select(sqlcmd);
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (field_name == DataTableUtils.toString(rows[i]["用途說明"]))
                            value = DataTableUtils.toString(rows[i]["領料數"]).Split('.')[0];
                    }
                }
                else
                    value = "0";
            }
            return $"<td>{value}</td>";
        }

        private int DaysBetween(string date_str, string date_end)
        {
            DateTime dt1 = DateTime.ParseExact(date_str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            DateTime dt2 = DateTime.ParseExact(date_end, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            int ans = (dt2.Year - dt1.Year) * 12 + (dt2.Month - dt1.Month);
            if (ans <= 0) ans = 1;
            return ans;
        }
        private void get_search_parameter()
        {
            string radio_list = RadioButtonList_select_type.SelectedValue;
            switch (radio_list)
            {
                case "0"://品名
                    search_condi = "APLD.COD_ITEM  ";
                    type = DataTableUtils.toString(DropDownList_materialstype.SelectedValue);
                    break;
                case "1"://料號
                    search_condi = "APLD.COD_ITEM ";
                    type = DataTableUtils.toString(TextBox_keyword.Text);
                    break;
                case "2"://品名
                    search_condi = "ITEM.NAM_ITEM  ";
                    type = DataTableUtils.toString(TextBox_keyword.Text);
                    break;
                case "3":
                    search_condi = "APLD.COD_ITEM  ";
                    type = "";
                    break;
            }
        }

        //event
        protected void button_select_Click(object sender, EventArgs e)
        {
            //get_search_parameter();
            date_str = txt_str.Text.Replace("-", "");
            date_end = txt_end.Text.Replace("-", "");
            load_page_data();
            TextBox_keyword.Text = "";
        }

    }
}
