using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using Support;
using dek_erpvis_v2.webservice;
using dekERP_dll;
using dekERP_dll.dekErp;


namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class supplier_score : System.Web.UI.Page
    {
        public string color = "";
        string URL_NAME = "";
        string acc = "";
        public string th = "";
        public string tr = "";
        public string title_text = "";
        public string date_str = "";// DateTime.Now.ToString("yyyy0101");
        public string date_end = "";// DateTime.Now.ToString("yyyy1231");
        public string time_array = "";
        public string path = "";
        public string timerange = "";
        clsDB_Server clsDB_sw = new clsDB_Server("");
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        iTec_Materials PCD = new iTec_Materials();

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("PCD");
                color = HtmlUtil.change_color(acc);
                if (myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc) || HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                            date_str = daterange[0];
                            date_end = daterange[1];
                            Set_Html_Table();
                        }
                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");

                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");

                if (txt_str.Text == "" && txt_end.Text == "")
                {
                    txt_str.Text = HtmlUtil.changetimeformat(date_str, "-");
                    txt_end.Text = HtmlUtil.changetimeformat(date_end, "-");
                }
            }
            else
                Response.Redirect(myclass.logout_url);

        }

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        private void Set_Html_Table()
        {
            DataTable dt = PCD.Supplierscore(date_str, date_end);
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, suppliescore_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string suppliescore_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "達交率")
            {
                value = (DataTableUtils.toDouble(DataTableUtils.toString(row["期限內已交總數量"])) * 100 / DataTableUtils.toDouble(DataTableUtils.toString(row["採購總數量"]))).ToString("0.00");

                string url = HtmlUtil.AttibuteValue("sup_sname", DataTableUtils.toString(row["供應商"]).Trim(), "") + "," +
                    HtmlUtil.AttibuteValue("date_str", date_str, "") + "," +
                    HtmlUtil.AttibuteValue("date_end", date_end, "");

                string href = $"supplierscore_details.aspx?key={WebUtils.UrlStringEncode(url)}";

                //value = DataTableUtils.toString(row[field_name]);
                value = $"<td><u><a href=\"{href}\">{value}</a></u></td>";
            }
            return value;
        }

        protected void button_select_Click(object sender, EventArgs e)
        {
            date_str = txt_str.Text.Replace("-", "");
            date_end = txt_end.Text.Replace("-", "");
            Set_Html_Table();
        }
    }
}
