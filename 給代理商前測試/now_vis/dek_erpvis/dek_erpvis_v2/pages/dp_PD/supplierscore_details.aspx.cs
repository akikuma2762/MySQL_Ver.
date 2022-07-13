using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class supplierscore_details : System.Web.UI.Page
    {
        public string color = "";
        public string sup_sname = "";
        public string date_str = "";
        public string date_end = "";
        public string th = "";
        public string tr = "";
        public string acc = "";
        iTec_Materials PCD = new iTec_Materials();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (acc != "")
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
                    Response.Redirect("supplierscore.aspx.aspx");
            }
            else
                Response.Redirect("supplierscore.aspx");
        }

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        private void load_page_data()
        {
            Response.BufferOutput = false;
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                sup_sname = HtmlUtil.Search_Dictionary(keyValues, "sup_sname");
                date_str = HtmlUtil.Search_Dictionary(keyValues, "date_str");
                date_end = HtmlUtil.Search_Dictionary(keyValues, "date_end");
                //儲存cookie
                Response.Cookies.Add(HtmlUtil.Save_Cookies("supplierscore", sup_sname));
                Set_Html_Table();
            }
            else
                Response.Redirect("supplierscore.aspx");
        }
        private void Set_Html_Table()
        {
            string sup_name = sup_sname == "" ? " IS null " : $" = '{sup_sname}' ";
            sup_sname = sup_sname == "" ? " 未填寫客戶 " : sup_sname;

            DataTable dt = PCD.Supplierscore_Detail(date_str, date_end, sup_name);
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, supplierscore_details_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }


        private string supplierscore_details_callback(DataRow row, string field_name)
        {
            if (field_name.Contains("日期"))
                return $"<td>{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]))}</td>";
            else
                return "";
        }

    }
}
