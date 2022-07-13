using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
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
    public partial class Account_Outstanding_Details : System.Web.UI.Page
    {

        public string color = "";
        public string title_text = "";
        public string tr = "";
        public string th = "";
        public string path = "";
        string acc = "";
        dekERP_dll.OrderStatus status;
        myclass myclass = new myclass();
        ERP_Customized cus = new ERP_Customized();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                path = 德大機械.get_title_web_path("SLS");
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                            MainProcess();
                    }

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
        private void MainProcess()
        {
            Set_Html_Table();
        }
        private void Set_Html_Table()
        {
            DataTable dt = cus.Account_Outstanding_Details();

            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, Orders_Details_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

        private string Orders_Details_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "出貨日期")
                value = $"<td style=\"vertical-align: middle; text-align: center;\">{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]))}</td>";
            return value;
        }
    }
}