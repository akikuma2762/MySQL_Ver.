using dek_erpvis_v2.cls;
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
    public partial class recordsofchangetheorder_details1 : System.Web.UI.Page
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
        public int HTML_交期變更總次數 = 0;
        public int HTML_數量變更總次數 = 0;
        public int HTML_品號變更總次數 = 0;
        public int HTML_客戶單號變更總次數 = 0;
        string acc = "";
        DataTable public_dt = null;
        myclass myclass = new myclass();
        clsDB_Server clsDB_sw = new clsDB_Server("");
        int total = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);

                color = HtmlUtil.change_color(acc);
                if (acc != "")
                {
                    if (!IsPostBack)
                        GotoCenn();

                }
                else
                    Response.Redirect("recordsofchangetheorder.aspx", false);
            }
            else
                Response.Redirect("recordsofchangetheorder.aspx", false);
        }
        private void GotoCenn()
        {
          //  clsDB_sw.dbOpen(myclass.GetConnByDetaSowon);
            if (clsDB_sw.IsConnected == true)
                load_page_data();
            else
            {
                Response.Write($"<script language='javascript'>alert('伺服器回應 : 無法載入資料，{clsDB_sw.ErrorMessage} 請聯絡德科人員或檢查您的網路連線。');</script>");
                title_text = HtmlUtil.NoData(out th, out tr);
            }
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
                Set_Html_Table();
            }
            else
                Response.Redirect("recordsofchangetheorder.aspx", false);
        }
        private void Set_Html_Table()
        {
            //title
            public_dt = new DataTable();
           // GlobalVar.UseDB_setConnString(myclass.GetConnByDetaEip);
            string strCmd = 德大機械.業務部_訂單變更紀錄.客戶訂單變更(cust_name, date_str, date_end);
            public_dt = DataTableUtils.GetDataTable(strCmd);

            string sqlcmd = 德大機械.業務部_訂單變更紀錄.變更客戶(cust_name, date_str, date_end);
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(public_dt) && HtmlUtil.Check_DataTable(dt))
            {
                string titlename = "";
                th = "<th>客戶名稱</th>\n";
                th += "<th>訂單號碼</th>\n";
                th += "<th>SN</th>\n";
                th += "<th>變更日期</th>\n";
                th += "<th>客戶單號變更次數</th>\n";
                th += "<th>品號變更次數</th>\n";
                th += "<th>數量變更次數</th>\n";
                th += "<th>交期變更次數</th>\n";
                th += "<th>小計</th>\n";
                th += "<th>德大變更</th>\n";
                th += "<th>客戶變更</th>\n";

                titlename = "客戶名稱,訂單號碼,SN,變更日期,客戶單號變更次數,品號變更次數,數量變更次數,交期變更次數,小計,德大變更,客戶變更,";
                tr = HtmlUtil.Set_Table_Content(dt, titlename, recordsofchangetheorderDetails_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);

        }

        private string recordsofchangetheorderDetails_callback(DataRow row, string field_name)
        {
            string value = "";


            if (field_name == "變更日期")
                value = DataTableUtils.toString(row[field_name]).Replace('-', '/').Split(' ')[0];
            else if (field_name == "客戶單號變更次數")
            {
                total = 0;
                string sqlcmd = $"訂單號碼 = '{DataTableUtils.toString(row["訂單號碼"])}' and SN='{DataTableUtils.toString(row["SN"])}' and  變更欄位='{Return_name(field_name)}'";
                DataRow[] rows = public_dt.Select(sqlcmd);

                value = "" + rows.Length;
                total += rows.Length;
                HTML_客戶單號變更總次數 += rows.Length;

            }
            else if (field_name == "品號變更次數")
            {
                string sqlcmd = $"訂單號碼 = '{DataTableUtils.toString(row["訂單號碼"])}' and SN='{DataTableUtils.toString(row["SN"])}' and  變更欄位='{Return_name(field_name)}'";
                DataRow[] rows = public_dt.Select(sqlcmd);

                value = "" + rows.Length;
                total += rows.Length;
                HTML_品號變更總次數 += rows.Length;
            }
            else if (field_name == "數量變更次數")
            {
                string sqlcmd = $"訂單號碼 = '{DataTableUtils.toString(row["訂單號碼"])}' and SN='{DataTableUtils.toString(row["SN"])}' and  變更欄位='{Return_name(field_name)}'";
                DataRow[] rows = public_dt.Select(sqlcmd);

                value = "" + rows.Length;
                total += rows.Length;
                HTML_數量變更總次數 += rows.Length;
            }
            else if (field_name == "交期變更次數")
            {
                string sqlcmd = $"訂單號碼 = '{DataTableUtils.toString(row["訂單號碼"])}' and SN='{DataTableUtils.toString(row["SN"])}' and  變更欄位='{Return_name(field_name)}'";
                DataRow[] rows = public_dt.Select(sqlcmd);

                value = "" + rows.Length;
                total += rows.Length;
                HTML_交期變更總次數 += rows.Length;
            }
            else if (field_name == "小計")
                value = "" + total;
            else if (field_name == "德大變更")
            {
              //  GlobalVar.UseDB_setConnString(myclass.GetConnByDetaEip);
                string sqlcmd = $"select * from CordSub_chg_cnt where TRN_NO = '{DataTableUtils.toString(row["訂單號碼"])}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["德大變更_cnt"])) > DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["客戶變更_cnt"])))
                    value = "" + total;
                else
                    value = "0";
            }

            else if (field_name == "客戶變更")
            {
              //  GlobalVar.UseDB_setConnString(myclass.GetConnByDetaEip);
                string sqlcmd = $"select * from CordSub_chg_cnt where TRN_NO = '{DataTableUtils.toString(row["訂單號碼"])}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["德大變更_cnt"])) <= DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["客戶變更_cnt"])))
                    value = "" + total;
                else
                    value = "0";
            }



            if (value == "")
                return "";
            else
                return "<td>" + value + "</td>";
        }

        private string Return_name(string type)
        {
            switch (type)
            {
                case "品號變更次數":
                    return "品號.變更";
                case "客戶單號變更次數":
                    return "單號.變更";
                case "數量變更次數":
                    return "qty.變更";
                case "交期變更次數":
                    return "D_DATE.變更";
            }
            return "";
        }

    }
}