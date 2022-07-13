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
    public partial class Orders_Details : System.Web.UI.Page
    {

        public string color = "";
        public string cust_name = "";
        public string url_text = "";
        public string Get_cust = "";
        public string Get_Dstart = "";
        public string Get_Dend = "";
        public string Get_SOStatus = "";
        public string title_text = "";
        public string tr = "";
        public string th = "";
        string acc = "";
        string type = "";
        dekERP_dll.OrderStatus status;
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
                            MainProcess();
                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");

                }
                else
                    Response.Redirect("Orders.aspx");
            }
            else
                Response.Redirect("Orders.aspx");
        }
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        private void MainProcess()
        {
            GetCondi();
            Set_Html_Table();
        }
        private void GetCondi()
        {
            Response.Buffer = false;
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                cust_name = HtmlUtil.Search_Dictionary(keyValues, "cust");
                Get_cust = HtmlUtil.Search_Dictionary(keyValues, "cust");
                Get_Dstart = HtmlUtil.Search_Dictionary(keyValues, "date_str");
                Get_Dend = HtmlUtil.Search_Dictionary(keyValues, "date_end");
                Get_SOStatus = HtmlUtil.Search_Dictionary(keyValues, "condi");
                type = HtmlUtil.Search_Dictionary(keyValues, "type");
                //儲存cookie
                Response.Cookies.Add(HtmlUtil.Save_Cookies("Order", Get_cust));
            }
            else
                Response.Redirect("Orders.aspx", false);
        }

        private void Set_Html_Table()
        {
            DataTable dt = new DataTable();
            
            if(type == "0")
            {
                if (Get_Dend != "" && Get_SOStatus != "")
                    dt = SLS.Orders_Details($" '{Get_Dstart}' <= test2.poln.DAT_DELS ", $" and  test2.poln.DAT_DELS <= '{Get_Dend}' " , Get_cust, Get_SOStatus);
                else
                    dt = SLS.Orders_NotFinish_Details($" '{Get_Dstart}' <= test2.poln.DAT_DELS ", Get_cust);
            }
            else
            {
                if (Get_Dend != "" && Get_SOStatus != "")
                    dt = SLS.Orders_Details($" {Get_Dstart} <= test2.pohd.dat_po  ", $" and  test2.pohd.dat_po  <= {Get_Dend} ", Get_cust, Get_SOStatus);
                else
                    dt = SLS.Orders_NotFinish_Details($" {Get_Dstart} <= test2.pohd.dat_po  ", Get_cust);
            }
            dt = myclass.Add_LINE_GROUP(dt).ToTable();
            dt.Columns.Remove("產線代號");
            dt.Columns["產線群組"].SetOrdinal(2);//選擇欄位位置
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

            if (field_name == "成品庫存日" || field_name == "入庫日")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                string sqlcmd = $"select SUBSTRING(實際完成時間, 1, 8) as 實際完成時間 from 工作站狀態資料表 where 排程編號 = '{row["製令號"]}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                    value = $"<td style=\"vertical-align: middle; text-align: center;\">{HtmlUtil.changetimeformat(DataTableUtils.toString(dt.Rows[0]["實際完成時間"]))}</td>";
            }
            else if (field_name == "預計開工日" || field_name=="訂單日期")
                value = $"<td style=\"vertical-align: middle; text-align: center;\">{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]))}</td>";
            return value;
        }

    }
}
