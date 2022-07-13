using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class waitingfortheproduction_details_New : System.Web.UI.Page
    {
        public string color = "";
        public string th = "";
        public string tr = "";
        public string cust_sname = "";
        public string date_str = "";
        public string date_end = "";
        string acc = "";
        public string path = "";
        myclass myclass = new cls.myclass();
        ShareFunction sFun = new ShareFunction();
        DataTable dt_本月應生產 = new DataTable();
        string Line = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("PMD");
                color = HtmlUtil.change_color(acc);
                if (acc != "")
                {
                    if (!IsPostBack)
                        load_page_data();
                }
                else
                    Response.Redirect("waitingfortheproduction_New.aspx", false);
            }
            else
                Response.Redirect("waitingfortheproduction_New.aspx", false);
        }

        private void load_page_data()
        {
            Response.BufferOutput = false;
            if (Request.QueryString["key"] != null)
            {
                //確認參數是否存在
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                cust_sname = HtmlUtil.Search_Dictionary(keyValues, "cust_name");
                date_str = HtmlUtil.Search_Dictionary(keyValues, "date_str");
                date_end = HtmlUtil.Search_Dictionary(keyValues, "date_end");
                List<string> Linelist = new List<string>(HtmlUtil.Search_Dictionary(keyValues, "line").Split('#'));
                for (int i = 0; i < Linelist.Count - 1; i++)
                    Line += i == 0 ? $" 工作站編號={Linelist[i]} " : $" OR 工作站編號={Linelist[i]} ";
                Line = Line != "" ? $" and ( {Line} ) " : "";
                //儲存cookie
                Response.Cookies.Add(HtmlUtil.Save_Cookies("waitingfortheproduction_New", cust_sname));

                Set_Table();
            }
            else
                Response.Redirect("waitingfortheproduction_New.aspx", false);
        }
        //印出表格
        private void Set_Table()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $"SELECT  工作站編號 產線代號,組裝資料表.*,組裝日 預計開工日,進度,狀態,組裝日,組裝日 上線日,實際完成時間,(CASE 工作站編號 WHEN 1 THEN 組裝工時*60*60 WHEN 4 THEN ((CAST(刀臂點數 AS float)+CAST(刀套點數 AS float))*60*60) WHEN 5 THEN (CAST(刀鍊點數 AS float)*60*60) WHEN 6 THEN (CAST(全油壓點數 AS float)*60*60) END) 標準工時 FROM 工作站狀態資料表 LEFT JOIN 組裝資料表 ON 工作站狀態資料表.排程編號 = 組裝資料表.排程編號 LEFT JOIN 臥式工藝 ON 臥式工藝.機種編號 = SUBSTRING(工作站狀態資料表.排程編號, 1, length(工作站狀態資料表.排程編號)-8) WHERE ((組裝日>={HtmlUtil.StrToDate(date_str).AddMonths(-1):yyyyMMdd} AND 組裝日 <={date_end}) OR (實際完成時間 IS NULL OR length(實際完成時間) =0))  {Line} AND CUSTNM = '{cust_sname}'";
            DataTable dt_now = DataTableUtils.GetDataTable(sqlcmd);
            DataTable dt_NoFinish = new DataTable();
            if (HtmlUtil.Check_DataTable(dt_now))
            {
                if (HtmlUtil.Check_DataTable(dt_now))
                    dt_now = sFun.Return_NowMonthTotal(dt_now, date_str, date_end, out _, out dt_NoFinish);
                //合併
                if (dt_now != null)
                {
                    dt_本月應生產 = dt_now.Clone();
                    dt_now.PrimaryKey = new DataColumn[] { dt_now.Columns["排程編號"], dt_now.Columns["工作站編號"] };
                    dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"], dt_本月應生產.Columns["工作站編號"] };
                    dt_本月應生產.Merge(dt_now);
                }
                if (HtmlUtil.Check_DataTable(dt_NoFinish))
                {
                    dt_NoFinish.PrimaryKey = new DataColumn[] { dt_NoFinish.Columns["排程編號"], dt_NoFinish.Columns["工作站編號"] };
                    dt_本月應生產.Merge(dt_NoFinish, true, MissingSchemaAction.Ignore);
                }

                //移除本月已完成
                sqlcmd = "狀態='完成'";
                DataRow[] rows = dt_本月應生產.Select(sqlcmd);
                for (int i = 0; i < rows.Length; i++)
                    rows[i].Delete();
                dt_本月應生產.AcceptChanges();

                if (HtmlUtil.Check_DataTable(dt_本月應生產))
                {
                    //加入產線
                    dt_本月應生產 = myclass.Add_LINE_GROUP(dt_本月應生產, "hor").ToTable();

                    //移除無須顯示項
                    dt_本月應生產.Columns.Remove("產線代號");
                    dt_本月應生產.Columns.Remove("上線日");
                    dt_本月應生產.Columns.Remove("實際完成時間");
                    dt_本月應生產.Columns.Remove("標準工時");
                    dt_本月應生產.Columns.Remove("預計完工日");
                    dt_本月應生產.Columns["產線群組"].SetOrdinal(1);
                    string title = "";
                    th = HtmlUtil.Set_Table_Title(dt_本月應生產, out title);
                    tr = HtmlUtil.Set_Table_Content(dt_本月應生產, title, waitingfortheproduction_details_ITEC_callback);

                }
                else
                    HtmlUtil.NoData(out th, out tr);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string waitingfortheproduction_details_ITEC_callback(DataRow row, string field_name)
        {
            string value = "";
            value = field_name == "預計開工日" || field_name == "組裝日" ? HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name])) : "";
            return value == "" ? "" : $"<td>{value}</td>";

        }
    }
}