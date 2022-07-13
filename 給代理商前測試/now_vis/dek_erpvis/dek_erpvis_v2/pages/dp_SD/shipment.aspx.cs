using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using dekERP_dll;
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
    public partial class shipment : System.Web.UI.Page
    {
        public string x_value = "";
        public string color = "";
        public string dt_st = "";
        public string dt_ed = "";
        public string select_year = "";
        public string title_text = "'此區間內尚無資料'";
        public string title_text_cust = "'此區間內尚無資料'";
        public string col_data_Points = "";
        public string col_data_Points_cust = "";
        public string time_area_text = "";
        public string th = "";
        public string tr = "";
        public string timerange = "";
        public string path = "";
        public string SubTotal = WebUtils.GetAppSettings("show_Subtotal");
        string URL_NAME = "";
        string acc = "";
        double CUST_TOTAL;
        string[] str = null;
        DataTable dt = new DataTable();
        DataTable custom = new DataTable();
        DataTable Line = new DataTable();
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        iTec_Sales SLS = new iTec_Sales();
        List<string> linelist = new List<string>();
        string x_text = "";
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
                            string[] s = 德大機械.德大專用月份(acc).Split(',');
                            dt_st = s[0];
                            dt_ed = s[1];

                            GotoCenn();
                            txt_str.Text = HtmlUtil.changetimeformat(s[0], "-");
                            txt_end.Text = HtmlUtil.changetimeformat(s[1], "-");

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
        // function
        private void GotoCenn()
        {
            Set_Dropdownlist();
            string date_s = HtmlUtil.changetimeformat(dt_st);
            string date_e = HtmlUtil.changetimeformat(dt_ed);
            timerange = date_s + " ~ " + date_e;
            set_col_value();
            set_table_title();
        }
        private void set_col_value()
        {

            dt = SLS.Shipment(dt_st, dt_ed, dekModel.Image);

            if (HtmlUtil.Check_DataTable(dt) == true)
            {
                dt = myclass.Add_LINE_GROUP(dt).ToTable();
                int Total_Quantity;
                col_data_Points = HtmlUtil.Set_Chart(dt, "產線群組", dropdownlist_Y.SelectedItem.Text, "", out Total_Quantity, DataTableUtils.toInt(txt_showCount.Text)).Replace("產線", "機種");
                title_text = " '總出貨" + dropdownlist_Y.SelectedItem.Text + " : " + Total_Quantity + "'";
                dt.Dispose();
            }
            else
            {
                col_data_Points = "";
                title_text = "'此區間內尚無資料'";
            }

        }
        private void set_table_title()
        {
            dt = SLS.Shipment(dt_st, dt_ed, dekModel.Table);

            string title = "";
            if (HtmlUtil.Check_DataTable(dt))
            {
                dt = myclass.Add_LINE_GROUP(dt).ToTable();
                dt.Columns.RemoveAt(0);
                dt.Columns["產線群組"].SetOrdinal(1);

                Line = dt.DefaultView.ToTable(true, new string[] { "產線群組" });

                switch (dropdownlist_X.SelectedItem.Text)
                {
                    case "機種":
                    case "客戶":
                        x_text = "客戶簡稱";
                        break;
                    case "國家別":
                        x_text = "國家別";
                        break;
                    case "機型":
                        x_text = "機型";
                        break;
                }


                custom = dt.DefaultView.ToTable(true, new string[] { x_text });

                foreach (DataRow row in Line.Rows)
                {
                    custom.Columns.Add(row["產線群組"].ToString());
                    linelist.Add(row["產線群組"].ToString());
                }
                custom.Columns.Add("小計");

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(custom, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, custom, order_list, shipment_callback).ToString();

                if (dropdownlist_X.SelectedItem.Value == "PLINE_NO")
                    set_col_value();
                else
                {
                    if (CheckBox_All.Checked == true)
                        set_col_value_cust(dt);
                    else
                        set_col_value_cust(dt, txt_showCount.Text);
                }

            }
            else
                HtmlUtil.NoData(out th, out tr);
            x_value = dropdownlist_X.SelectedItem.Text;
        }
        private string shipment_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "小計")
            {
                string url = HtmlUtil.AttibuteValue("cust_name", DataTableUtils.toString(row[x_text]).Trim(), "") + "," +
                                                                        HtmlUtil.AttibuteValue("type", x_text, "") + "," +
                                                                        HtmlUtil.AttibuteValue("date_str", dt_st, "") + "," +
                                                                        HtmlUtil.AttibuteValue("date_end", dt_ed, "");
                string href = string.Format("Shipment_Details.aspx?key={0}", WebUtils.UrlStringEncode(url));

                CUST_TOTAL = 0;
                DataRow[] rows = dt.Select($"{x_text}='{row[x_text]}'");
                for (int i = 0; i < rows.Length; i++)
                    CUST_TOTAL += DataTableUtils.toDouble(rows[i][dropdownlist_Y.SelectedItem.Text].ToString());


                value = CUST_TOTAL.ToString("0");
                value = HtmlUtil.ToTag("u", HtmlUtil.ToHref(value, href));
            }
            else if (field_name != x_text && field_name != "小計")
            {
                if (linelist.IndexOf(field_name) == 0)
                    CUST_TOTAL = 0;
                string sqlcmd = $"{x_text} ='{DataTableUtils.toString(row[x_text])}' and 產線群組 = '{field_name}'";
                DataRow[] rows = dt.Select(sqlcmd);
                int LINE_TOTAL = 0;
                if (rows.Length != 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        for (int j = 0; j < Line.Rows.Count; j++)
                        {
                            if (field_name == DataTableUtils.toString(rows[i]["產線群組"]) && field_name == DataTableUtils.toString(Line.Rows[j]["產線群組"]))
                                LINE_TOTAL += DataTableUtils.toInt(DataTableUtils.toString(rows[i][dropdownlist_Y.SelectedItem.Text]).Split('.')[0]);
                        }
                    }
                }
                value = DataTableUtils.toString(LINE_TOTAL);
                CUST_TOTAL += LINE_TOTAL;
            }
            if (value == "")
                return "";
            else
                return "<td align='right'>" + value + "</td>";
        }

        private void set_col_value_cust(DataTable dt_CustList, string cust_count = "")
        {
            if (!CheckBox_All.Checked)
                title_text = $"'{dropdownlist_X.SelectedItem.Text}出貨{dropdownlist_Y.SelectedItem.Text}排行前 {txt_showCount.Text} 名'";
            else
                title_text = $"'所有{dropdownlist_X.SelectedItem.Text}出貨{dropdownlist_Y.SelectedItem.Text}排行'";
            col_data_Points = "";

            dt = SLS.Shipment(dt_st, dt_ed, dekModel.Table);
            if (HtmlUtil.Check_DataTable(dt))
            {
                dt.Columns.Remove("產線代號");
                int Total_Quantity = 0;
                col_data_Points = HtmlUtil.Set_Chart(dt, x_text, dropdownlist_Y.SelectedItem.Text, "", out Total_Quantity, DataTableUtils.toInt(txt_showCount.Text), true).Replace("產線", "機種");

                dt.Dispose();
                dt.Clear();
            }
            else
            {
                col_data_Points = "";
                title_text = "'此區間內尚無資料'";
            }
        }
        protected void button_select_Click(object sender, EventArgs e)
        {
            if (DataTableUtils.toString(((Control)sender).ID.Split('_')[1]) == "select")
            {
                dt_st = txt_str.Text.Replace("-", "");
                dt_ed = txt_end.Text.Replace("-", "");
                GotoCenn();
            }
            else
            {
                string[] s = 德大機械.德大專用月份(acc).Split(',');
                HtmlUtil.Button_Click(DataTableUtils.toString(((Control)sender).ID.Split('_')[1]), s, DataTableUtils.toString(txt_str.Text), DataTableUtils.toString(txt_end.Text), out dt_st, out dt_ed);
                txt_str.Text = HtmlUtil.changetimeformat(dt_st, "-");
                txt_end.Text = HtmlUtil.changetimeformat(dt_ed, "-");
            }
        }
        private void Set_Dropdownlist()
        {
            if (dropdownlist_Y.Items.Count == 0)
            {
                string view_YN = 德大機械.function_yn(acc, "出貨金額");
                if (view_YN == "Y")
                {
                    dropdownlist_Y.Items.Add(new ListItem("數量", "QUANTITY"));
                    dropdownlist_Y.Items.Add(new ListItem("金額", "AMOUNT"));
                }
                else
                    dropdownlist_Y.Items.Add(new ListItem("數量", "QUANTITY"));

            }
        }
    }
}