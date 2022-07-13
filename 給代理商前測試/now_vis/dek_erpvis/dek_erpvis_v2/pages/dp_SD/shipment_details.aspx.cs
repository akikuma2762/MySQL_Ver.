using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using Support;
using dekERP_dll;
using dekERP_dll.dekErp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_SD
{
    public partial class shipment_details : System.Web.UI.Page
    {
        public string color = "";
        public string th = "";
        public string tr = "";
        public string cust_name = "";
        public string date_str = "";
        string type = "";
        public string date_end = "";
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
                    Response.Redirect("Shipment.aspx", false);
            }
            else
                Response.Redirect("Shipment.aspx", false);
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
                date_end = HtmlUtil.Search_Dictionary(keyValues, "date_end");
                type = HtmlUtil.Search_Dictionary(keyValues, "type");
                //儲存cookie
                Response.Cookies.Add(HtmlUtil.Save_Cookies("shipment", cust_name));
                Set_Html_Table();
            }
            else
                Response.Redirect("shipment.aspx");
        }

        private void Set_Html_Table()
        {
            DataTable dt = new DataTable();
          

            switch (type)
            {
                case "客戶簡稱":
                    dt = SLS.Shipment_Detail(date_str, date_end, $" and test2.delh.NUM_CUST = '{cust_name}'");
                    break;
                case "國家別":
                    dt = SLS.Shipment_Detail(date_str, date_end,$" and NVL( (select content  from test2.codd where test2.codd.code_id='NUMWAY' and delh.num_way  =test2.codd.code ),'未填寫' ) = '{cust_name}' " );
                    break;
                case "機型":
                    dt = SLS.Shipment_Detail(date_str, date_end, $" and test2.item.cod_modl = '{cust_name}' ");
                    break;
            }
            if (HtmlUtil.Check_DataTable(dt))
            {
                dt = myclass.Add_LINE_GROUP(dt).ToTable();
                dt.Columns.RemoveAt(1);
                dt.Columns["產線群組"].SetOrdinal(1);

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, ShipmentDetail_Callback).ToString();

            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string ShipmentDetail_Callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "小計")
                value = $"<td data-toggle=\"modal\" data-target=\"#exampleModal\"><u><a href=\"javascript:void(0)\" onclick=GetShipment_details(\"{row["客戶簡稱"]}\",\"{DataTableUtils.toString(row["品號"])}\",\"{date_str}\",\"{date_end}\")>{DataTableUtils.toString(row["小計"])}</a></u></td>";

            if (value == "")
                return "";
            else
                return value;
        }
    }
}