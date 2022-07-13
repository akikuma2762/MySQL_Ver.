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
    public partial class Asm_PersonFinish_Detail : System.Web.UI.Page
    {
        public string color = "";
        public string th = "";
        public string tr = "";
        public string path = "";
        string acc = "";
        public string staff = "";
        string start = "";
        string end = "";
        string workstation = "";
        DataTable dt_MonthTotal = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                path = 德大機械.get_title_web_path("PMD");
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
                Response.Redirect(myclass.logout_url);
        }
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        private void MainProcess()
        {
            GetCondi();
            Get_MonthTotal();
            Set_Table();
        }
        //設定數值
        private void GetCondi()
        {
            Response.Buffer = false;
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                staff = HtmlUtil.Search_Dictionary(keyValues, "staff");
                workstation = HtmlUtil.Search_Dictionary(keyValues, "workstation");
                start = HtmlUtil.Search_Dictionary(keyValues, "start");
                end = HtmlUtil.Search_Dictionary(keyValues, "end");
            }
            else
                Response.Redirect("Asm_PersonFinish.aspx", false);
        }


        private void Get_MonthTotal()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $"SELECT  工作站異常維護資料表.維護人員姓名, 工作站異常維護資料表.排程編號, 工作站型態資料表.工作站編號, 工作站型態資料表.工作站名稱, a.時間紀錄  結案時間 FROM 工作站異常維護資料表 LEFT JOIN (SELECT  父編號, 結案判定類型,時間紀錄 FROM 工作站異常維護資料表 a WHERE a.結案判定類型 IS NOT NULL) a ON 工作站異常維護資料表.異常維護編號 = a.父編號 LEFT JOIN 工作站型態資料表 ON 工作站型態資料表.工作站編號 = 工作站異常維護資料表.工作站編號 WHERE (工作站異常維護資料表.父編號 IS NULL OR 工作站異常維護資料表.父編號 = 0) AND a.結案判定類型 IS NOT NULL AND 工作站名稱 = '{workstation}' AND 維護人員姓名 = '{staff}' and {start} <= substring(a.時間紀錄,1,8) and substring(a.時間紀錄,1,8) <= {end}";
            dt_MonthTotal = DataTableUtils.GetDataTable(sqlcmd);
        }

        private void Set_Table()
        {
            //取得所有產線
            DataTable Number = dt_MonthTotal.DefaultView.ToTable(true, new string[] { "排程編號" });
            Number.Columns.Add("維護人員姓名");
            Number.Columns.Add("結案數量");

            List<string> order_list = HtmlUtil.Comparison_ColumnOrder(Number, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

            th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
            tr = HtmlUtil.Set_Table_Content(true, Number, order_list, Asm_PersonFinish_Detail_callback).ToString();
        }

        private string Asm_PersonFinish_Detail_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "維護人員姓名")
                value = staff;
            else if (field_name == "結案數量")
            {
                DataRow[] rows = dt_MonthTotal.Select($"排程編號='{row["排程編號"]}' and 維護人員姓名='{staff}'");
                string url = $"Asm_ErrorDetail.aspx?key={WebUtils.UrlStringEncode($"ErrorID={row["排程編號"]},ErrorLineNum={rows[0]["工作站編號"]},ErrorLineName={rows[0]["工作站名稱"]}")}";

                value = rows != null && rows.Length > 0 ? $"<u><a href=\"{url}\"> {rows.Length} </a></u>" : "0";
            }
            else
                value = row[field_name].ToString();
            return value == "" ? "" : $"<td  align=\"center\" style=\"vertical-align:middle; color: black; \">{value}</td>";
        }
    }
}