using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class Asm_PersonFinish : System.Web.UI.Page
    {

        public string color = "";
        public StringBuilder th = new StringBuilder();
        public StringBuilder tr = new StringBuilder();
        public string cust_name = "";
        public string date_str = "";
        public string date_end = "";
        public string path = "";
        string acc = "";
        DataTable dt_MonthTotal = new DataTable();
        myclass myclass = new myclass();
        List<string> linelist = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                path = 德大機械.get_title_web_path("PMD");
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0])|| myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            string[] date = 德大機械.德大專用月份(acc).Split(',');
                            textbox_dt1.Text = HtmlUtil.changetimeformat(date[0], "-");
                            textbox_dt2.Text = HtmlUtil.changetimeformat(date[1], "-");
                            MainProcess();
                        }
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
        protected void button_select_Click(object sender, EventArgs e)
        {
            MainProcess();
        }

        private void MainProcess()
        {
            Get_MonthTotal();
            Set_Table();
        }

        private void Get_MonthTotal()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string number = TextBox_Number.Text == "" ? "" : $" and   工作站異常維護資料表.排程編號 = '{TextBox_Number.Text}' ";
            string daterange = textbox_dt1.Text != "" && textbox_dt2.Text != "" ? $"  and  {textbox_dt1.Text.Replace("-", "")} <= substring(a.時間紀錄,1,8) and substring(a.時間紀錄,1,8) <= {textbox_dt2.Text.Replace("-", "")} " : "";
            string sqlcmd = $"SELECT  工作站異常維護資料表.異常維護編號, 工作站異常維護資料表.維護人員姓名, 工作站異常維護資料表.排程編號, 工作站型態資料表.工作站編號, 工作站型態資料表.工作站名稱, a.時間紀錄 結案時間 FROM 工作站異常維護資料表 LEFT JOIN (SELECT  父編號, 結案判定類型,時間紀錄 FROM 工作站異常維護資料表 a WHERE a.結案判定類型 IS NOT NULL) a ON 工作站異常維護資料表.異常維護編號 = a.父編號 LEFT JOIN 工作站型態資料表 ON 工作站型態資料表.工作站編號 = 工作站異常維護資料表.工作站編號 WHERE (工作站異常維護資料表.父編號 IS NULL OR 工作站異常維護資料表.父編號 = 0) AND a.結案判定類型 IS not NULL {number} {daterange}";
            dt_MonthTotal = DataTableUtils.GetDataTable(sqlcmd);
        }

        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_MonthTotal))
            {
                //取得所有產線
                DataTable Line = dt_MonthTotal.DefaultView.ToTable(true, new string[] { "工作站名稱" });
                //取得人員
                DataTable staff = dt_MonthTotal.DefaultView.ToTable(true, new string[] { "維護人員姓名" });

                foreach (DataRow row in Line.Rows)
                {
                    staff.Columns.Add(row["工作站名稱"].ToString());
                    linelist.Add(row["工作站名稱"].ToString());
                }

                if (HtmlUtil.Check_DataTable(staff))
                {
                    List<string> order_list = HtmlUtil.Comparison_ColumnOrder(staff, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                    th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                    tr = HtmlUtil.Set_Table_Content(true, staff, order_list, Asm_PersonFinish_callback);
                }
                else
                    HtmlUtil.NoData(out th, out tr);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

        private string Asm_PersonFinish_callback(DataRow row, string field_name)
        {
            string value = "";
            if (linelist.IndexOf(field_name) != -1)
            {
                DataRow[] rows = dt_MonthTotal.Select($"維護人員姓名='{row["維護人員姓名"]}' and 工作站名稱='{field_name}'");
                string url = $"Asm_PersonFinish_Detail.aspx?key={WebUtils.UrlStringEncode($"staff={row["維護人員姓名"]},workstation={field_name},start={textbox_dt1.Text.Replace("-", "")},end={textbox_dt2.Text.Replace("-", "")}")}";

                value = rows != null && rows.Length > 0 ? $"<u><a href=\"{url}\"> {rows.Length} </a></u>" : "0";
            }
            else
                value = row[field_name].ToString();
            return value == "" ? "" : $"<td  align=\"center\" style=\"vertical-align:middle; color: black; \">{value}</td>";
        }
    }
}