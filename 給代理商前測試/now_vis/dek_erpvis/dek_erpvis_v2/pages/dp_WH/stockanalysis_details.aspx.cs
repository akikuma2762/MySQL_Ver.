using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dekERP_dll;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_SD
{
    public partial class stockanalysis_details : System.Web.UI.Page
    {
        public string color = "";
        public string th = "";
        public string tr = "";
        public string title = "";
        public string title_msg = "";
        public string title_msg_list = "";
        public string title_text = "";
        public string cust_name = "";
        public string date_str = "";
        public string date_end = "";
        string acc = "";
        DataTable public_dt = null;
        myclass myclass = new myclass();
        iTec_House WHE = new iTec_House();
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
                    Response.Redirect("stockanalysis.aspx");
            }
            else
                Response.Redirect("stockanalysis.aspx");
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
                cust_name = HtmlUtil.Search_Dictionary(keyValues, "cust_name");
                date_str = HtmlUtil.Search_Dictionary(keyValues, "date_str");
                Response.Cookies.Add(HtmlUtil.Save_Cookies("stockanalysis", cust_name));
                Set_Html_Table();
            }
            else
                Response.Redirect("stockanalysis.aspx");
        }
        private void Set_Html_Table()
        {
           if(cust_name != "")
                public_dt = WHE.stockanalysis_Details(date_str, $" = '{cust_name}'");
           else
                public_dt = WHE.stockanalysis_Details(date_str, $" IS NULL ");

            DataTable dt_Copy = public_dt.Copy();


            if (HtmlUtil.Check_DataTable(dt_Copy))
            {
                dt_Copy.Columns.Remove("訂單規格");
                dt_Copy = myclass.Add_LINE_GROUP(dt_Copy).Table;
                dt_Copy = HtmlUtil.Get_Warehousingdate(dt_Copy, date_str, true, true);
                dt_Copy.Columns.RemoveAt(1);
                dt_Copy.Columns["產線群組"].SetOrdinal(1);

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt_Copy, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt_Copy, order_list, stockanalysis_details_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string stockanalysis_details_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "入庫日")
                value = $"<td>{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]))}</td>";
            else if(field_name == "訂單號碼")
            {
                DataRow[] rows = public_dt.Select($"訂單號碼='{row["訂單號碼"]}'");
                if (rows != null && rows.Length > 0)
                    value = DataTableUtils.toString(rows[0]["訂單規格"]);
                else
                    value = "無內容";
                value = $"<td data-toggle=\"modal\" data-target=\"#exampleModal_Image\"><u><a href=\"javascript:void(0)\" onclick=show_information(\"{row["訂單號碼"]}\",\"{cust_name}\",\"{date_str}\")>{DataTableUtils.toString(row[field_name])}</a></u></td>";
            }
            return value;         
        }
    }
}