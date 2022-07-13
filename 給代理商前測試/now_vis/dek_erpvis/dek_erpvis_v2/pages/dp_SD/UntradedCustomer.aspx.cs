using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using dekERP_dll.dekErp;
using Support;
using Support.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace dek_erpvis_v2.pages.dp_SD
{
    public partial class UntradedCustomer : System.Web.UI.Page
    {
        public string color = "";
        public string date_str = "";
        public string date_end = "";
        public string th = "";
        public string tr = "";
        public string path = "";
        string acc = "";
        myclass myclass = new myclass();
        iTec_Sales SLS = new iTec_Sales();
        protected void Page_Load(object sender, EventArgs e)
        {

            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                path = 德大機械.get_title_web_path("SLS");
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                            Set_HtmlTable();
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

        private void Set_HtmlTable()
        {
            date_str = DataTableUtils.toString(txt_str.Text);
            date_end = DataTableUtils.toString(txt_end.Text);
            DataTable dt = SLS.UntradedCustomer(date_str.Replace("-",""), date_end.Replace("-", ""), DropDownList_selectedcondi.SelectedValue, DataTableUtils.toInt(TextBox_dayval.Text));
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, UntradedCustomer_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string UntradedCustomer_callback(DataRow row, string field_name)
        {
            if (field_name == "最後交易日")
                return $"<td>{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]))}</td>";
            else
                return "";
        }


        protected void button_select_Click(object sender, EventArgs e)
        {
            Set_HtmlTable();
        }
    }
}