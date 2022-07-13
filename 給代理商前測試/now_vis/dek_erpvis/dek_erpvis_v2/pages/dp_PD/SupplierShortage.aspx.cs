﻿using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class SupplierShortage : System.Web.UI.Page
    {
        public string color = "";
        public string timerange = "";
        public string path = "";
        public string supplier = "";
        public string supplierNM = "";
        public string title_text = "";
        public string th = "";
        public string tr = "";
        public string 未交量總計 = "0";
        public string 催料日期_RC = "";
        public string 基準日加45 = "";
        public string 基準日加30 = "";
        public string 基準日加16 = "";
        public string 基準日加14 = "";
        public string 基準日加23 = "";
        public string dt_st = "";
        public string dt_ed = "";
        string 催料日期_MP = "";
        string 最新催料日期_MP = "";
        string searchDateId = "";
        string searchDate = "";
        string itemNo = "";
        string supplierName = "";
        string URL_NAME = "";
        string acc = "";
        string titlename = "";
        int total = 0;
        public string dt_start = "";
        public string dt_end = "";
        DataTable dt_MP = null;
        DataTable dt_RC = null;
        clsDB_Server clsDB_eip = new clsDB_Server("");
        clsDB_Server clsDB_sw = new clsDB_Server("");
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                //效能測試用

                path = 德大機械.get_title_web_path("PCD");
                URL_NAME = "SupplierShortage";
                color = HtmlUtil.change_color(acc);
                if (myclass.user_view_check(URL_NAME, acc) )
                {
                    if (!IsPostBack)
                    {
                        GetNewDate();             // 取得最新催料表單號
                        SetTableTitle();
                        LeadTime();
                    }
                }
                else 
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");

            }
            else  Response.Redirect(myclass.logout_url); 

        }

        //Process-------------------------------
        private void MainProcess()
        {
            GetCondi();
            ChangeDateFormat();                     // 取得採購單的條件:催料單日期
            SetTableTitle();
            LeadTime();
        }
        private void GetCondi()
        {
            supplier = textbox_dt1.Text;            // 供應商代碼    
            supplierName = textbox_dt2.Text;
            itemNo = textbox_item.Text;             // 品號
            dt_st = txt_str.Text.Replace("-", "");                  // 催料預交日(起)
            dt_ed = txt_end.Text.Replace("-", ""); ;                    // 催料預交日(訖)

            催料日期_RC = textbox_BillNo.Text;       // 催料單號(RC用) 080710001
        }
        private void SetTableTitle()
        {
          //  clsDB_eip.dbOpen(myclass.GetConnByDetaEip);
          //  clsDB_sw.dbOpen(myclass.GetConnByDetaSowon);
            string RC = "", MP = "";
            RC = 德大機械.資材部_供應商催料總表.催料表清單_依收貨單(supplier, supplierName, searchDateId, 催料日期_RC);
            MP = 德大機械.資材部_供應商催料總表.催料表清單_依採購單(supplier, supplierName, searchDate, dt_st, dt_ed, itemNo, 催料日期_MP);
            dt_RC = clsDB_eip.DataTable_GetTable(德大機械.資材部_供應商催料總表.催料表清單_依收貨單(supplier, supplierName, searchDateId, 催料日期_RC));
            dt_MP = clsDB_sw.DataTable_GetTable(德大機械.資材部_供應商催料總表.催料表清單_依採購單(supplier, supplierName, searchDate, dt_st, dt_ed, itemNo, 催料日期_MP)); // connect EIP 最新催料表
            dt_start = HtmlUtil.changetimeformat(dt_st);
            dt_end = HtmlUtil.changetimeformat(dt_ed);
            int 剩餘數量 = 0;

            if (dt_RC.Rows.Count > 0)
            {
                foreach (DataRow row_ in dt_RC.Rows) //收貨單 10
                {
                    string RC_item_no = DataTableUtils.toString(row_["品號"]);
                    int 收貨量 = DataTableUtils.toInt(DataTableUtils.toString(row_["收貨量"]));
                    foreach (DataRow row in dt_MP.Rows)//採購 9
                    {
                        string MP_item_no = DataTableUtils.toString(row["品號"]);
                        if (RC_item_no == MP_item_no)
                        {
                            int 催料數量 = DataTableUtils.toInt(DataTableUtils.toString(row["催料數量"]));  // 1
                            int 未交量 = 收貨量 - 催料數量;
                            if (未交量 >= 0)
                            {
                                剩餘數量 = 未交量;
                                if (未交量 > 0) 未交量 = 0;
                                row["未交量"] = 未交量;

                                收貨量 = 剩餘數量;
                            }
                            else // 如果收貨量已為0，則未交量 = 採購催料數量
                                row["未交量"] = DataTableUtils.toInt(DataTableUtils.toString(row["催料數量"]));
                        }
                        //else // 如果有收貨單，但收貨品號都非採購品號，則未交量 = 採購催料數量
                        //    row["未交量"] = DataTableUtils.toInt(DataTableUtils.toString(row["催料數量"]));
                    }
                }
            }
            else // 如果沒有收貨單，則未交量 = 採購催料數量
            {
                foreach (DataRow row in dt_MP.Rows)
                    row["未交量"] = DataTableUtils.toInt(DataTableUtils.toString(row["催料數量"]));
            }

            // output table title
            if (dt_MP.Rows.Count > 0)
            {
                th = "";
                //th += "<th>催料單號</th>\n";
                th += "<th>採購/加工單</th>\n";
                th += "<th>開單日期</th>\n";
                th += "<th>廠商簡稱</th>\n";
                th += "<th>品號</th>\n";
                th += "<th>品名規格</th>\n";
                th += "<th>催料預交日</th>\n";
                th += "<th>催料數量</th>\n";
                th += "<th>未交量</th>\n";

                titlename = "採購單號,開單日期,廠商簡稱,品號,品名規格,催料預交日,催料數量,未交量,";

                //先把未交量=0的資料移除
                DataRow[] rows = dt_MP.Select("未交量='0'");
                for (int i = 0; i < rows.Length; i++)
                    rows[i].Delete();
                //把剩餘資料列印
                if (dt_MP.Rows.Count > 0)
                    tr = HtmlUtil.Set_Table_Content(dt_MP, titlename, SupplierShortage_callback);

            }
            else
                title_text = HtmlUtil.NoData(out th, out tr);
        }

        private string SupplierShortage_callback(DataRow row, string field_name)
        {
            string value = "";

            DateTime 催料預交日 = DateTime.ParseExact(row["催料預交日"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            DateTime 今天 = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            string field = field_name;
            if (field_name == field)
                value = ">" + DataTableUtils.toString(row[field_name]) + "</td>\n";
            if (field_name == "催料數量")
                value = "align='right'>" + DataTableUtils.toString(row["催料數量"]) + "</td>\n";
            if (field_name == "未交量")
            {
                value = "align='right' >" + DataTableUtils.toString(row["催料數量"]) + "</td>\n";
                total += DataTableUtils.toInt(DataTableUtils.toString(row["催料數量"]));
            }
            if (field_name == "開單日期")
                value = ">" + HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name])) + "</td>\n";
            if (field_name == "催料預交日")
                value = ">" + HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name])) + "</td>\n";

            未交量總計 = DataTableUtils.toString(total.ToString("###,###"));

            if (DateTime.Compare(催料預交日, 今天) < 0)
                value = "<td style='color: #ff0000;'" + value;
            else
                value = "<td " + value;
            return value;
        }

        //Function------------------------------
        private void GetNewDate()//取得最新催料表日期
        {
            string 明國年 = "";
            string 民國年取後兩位 = "";
            明國年 = (Convert.ToInt32(DateTime.Today.AddDays(-6).ToString("yyyy")) - 1911).ToString(); //民國年: 108
            民國年取後兩位 = 明國年.Substring(1, 2);                                                    //民國年取兩位數 08            

          //  clsDB_eip.dbOpen(myclass.GetConnByDetaEip);
            string get = 德大機械.資材部_供應商催料總表.最新催料表日期();
            DataTable dt = clsDB_eip.DataTable_GetTable(德大機械.資材部_供應商催料總表.最新催料表日期());
            if (dt != null)
            {
                DataRow row = dt.Rows[0];
                催料日期_MP = DataTableUtils.toString(row["催料表日期"]);//催料單號 : 20190710 (MP用)
            }
            //催料日期_MP = 最新催料日期_MP;
            string 日期 = 催料日期_MP.Substring(4, 4);                  //取日期 0710
            催料日期_RC = 民國年取後兩位 + 日期 + "001";                 //催料單號 : 080710001 (RC用)

            textbox_BillNo.Text = 催料日期_RC;                         //預設填入前端催料單號欄位值      
            dt_st = 催料日期_MP;
            dt_ed = DataTableUtils.toString(DateTime.ParseExact(催料日期_MP, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddDays(6).ToString("yyyyMMdd"));

            txt_str.Text = HtmlUtil.changetimeformat(dt_st, "-");
            txt_end.Text = HtmlUtil.changetimeformat(dt_ed, "-");
            最新催料日期_MP = 催料日期_MP;
            string date_s = HtmlUtil.changetimeformat(dt_st);
            string date_e = HtmlUtil.changetimeformat(dt_ed);
            timerange = date_s + " ~ " + date_e;

        }
        private void ChangeDateFormat()//轉換採購單的催料日期格式
        {
            string 今年 = DateTime.Now.ToString("yyyy");
            string 日期 = 催料日期_RC.Substring(2, 4);
            催料日期_MP = 今年 + 日期;

            //送貨日期與天數說明，基準日用
         //   clsDB_eip.dbOpen(myclass.GetConnByDetaEip);
            string get = 德大機械.資材部_供應商催料總表.最新催料表日期();
            DataTable dt = clsDB_eip.DataTable_GetTable(德大機械.資材部_供應商催料總表.最新催料表日期());
            if (dt != null)
            {
                DataRow row = dt.Rows[0];
                最新催料日期_MP = DataTableUtils.toString(row["催料表日期"]);//催料單號 : 20190710 (MP用)
            }

        }
        private void LeadTime()//取得各物料類別前置時間
        {
            DateTime 基準日 = DateTime.ParseExact(最新催料日期_MP, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            基準日加45 = 基準日.AddDays(45).ToString("yyyy/MM/dd");
            基準日加30 = 基準日.AddDays(30).ToString("yyyy/MM/dd");
            基準日加16 = 基準日.AddDays(16).ToString("yyyy/MM/dd");
            基準日加14 = 基準日.AddDays(14).ToString("yyyy/MM/dd");
            基準日加23 = 基準日.AddDays(23).ToString("yyyy/MM/dd");
        }

        [WebMethod]// 2019.07.08 call webService (ru)
        public static List<string> Search(string FACTNM2)
        {
            List<string> result = new List<string>();
            clsDB_Server clsDB_sw = new clsDB_Server("");
            myclass myclass = new myclass();

        //    clsDB_sw.dbOpen(myclass.GetConnByDetaSowon);
            string sqlcmd = "select FACTNM2 from fact where FACTNM2 like '%" + FACTNM2 + "%'";
            DataTable dt = clsDB_sw.DataTable_GetTable(sqlcmd);

            for (int i = 0; i < dt.Rows.Count; i++)
                result.Add(DataTableUtils.toString(dt.Rows[i]["FACTNM2"]));
            return result;

        }

        //Event
        protected void button_select_Click(object sender, EventArgs e)
        {
            MainProcess();
        }
    }
}



/*if (dt_MP.Rows.Count > 0)
            {
                for (int i = 0; i < dt_MP.Rows.Count; i++)
                {
                    string supplier = dt_MP.Rows[i]["廠商"].ToString();
                    MP_item = dt_MP.Rows[i]["品號"].ToString();
                    MP_Qty = int.Parse(dt_MP.Rows[i]["催料數量"].ToString());

                    clsDB_sw.dbOpen(myclass.GetConnByDetaEip);
                    dt_RC = clsDB_sw.DataTable_GetTable(SQLCMD_RC(supplier, searchDateId));
                    if (dt_RC.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt_RC.Rows.Count; j++)
                        {
                            RC_item = dt_RC.Rows[j]["品號"].ToString();
                            RC_Qty = int.Parse(DataTableUtils.toString(dt_RC.Rows[j]["收貨量"]));
                            if (MP_item == RC_item)
                            {
                                if (MP_Qty - RC_Qty < 0)
                                {
                                    dt_MP.Rows[i]["未交量"] = MP_Qty - RC_Qty;
                                    //tmp_Qty = Math.Abs(MP_Qty - RC_Qty);
                                    //tmp_item = dt_RC.Rows[i]["品號"].ToString();
                                }
                                else
                                {
                                    dt_MP.Rows[i]["未交量"] = MP_Qty - RC_Qty;
                                }
                            }
                            else { dt_MP.Rows[i]["未交量"] = MP_Qty; }
                        }
                    }
                    else
                    {
                        dt_MP.Rows[i]["未交量"] = MP_Qty;
                    }

                }
            }*/
//string todayOfWeek = DateTime.Today.DayOfWeek.ToString("d"); //4 (今天星期4)
//string datediff = "";
//string today = DateTime.Today.ToString();            // 今天(周二) 2019.07.02
//DateTime Wed = DateTime.Today;


//            switch (todayOfWeek)//4
//            {
//                case "0":
//                    Wed = DateTime.Today.AddDays(-4);
//                    break;
//                case "1":
//                    Wed = DateTime.Today.AddDays(-5);
//                    break;
//                case "2":
//                    Wed = DateTime.Today.AddDays(-6);
//                    break;
//                case "3":
//                    Wed = DateTime.Today;
//                    break;
//                case "4":
//                    Wed = DateTime.Today.AddDays(-1); // 7/3
//                    break;
//                case "5":
//                    Wed = DateTime.Today.AddDays(-2);
//                    break;
//                case "6":
//                    Wed = DateTime.Today.AddDays(-3);
//                    break;
//            }
//            datediff = Wed.ToString("MMdd").ToString(); //0703    
//searchDate = Wed.ToString("yyyy-MM-dd").ToString();   //組合最新催料表日期: 2019-07-03