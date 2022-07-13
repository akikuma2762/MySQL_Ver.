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
    public partial class Asm_ResponeDepartment : System.Web.UI.Page
    {
        public string color = "";
        string acc = "";
        public StringBuilder th = new StringBuilder();
        public StringBuilder tr = new StringBuilder();
        DataTable dt_monthtotal = new DataTable();
        myclass myclass = new myclass();
        protected void Page_Load(object sender, EventArgs e)
        {
            string CompLoacation = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];

            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                CompLoacation = ShareFunction.Last_Place(acc);
                ShareFunction.Last_Place(acc, CompLoacation);

                if (true ||  HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
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
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管主管申請權限!');location.href='../index.aspx';</script>");
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
            Get_Monthtotal();
            Set_Table();
        }

        private void Get_Monthtotal()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = "SELECT  工作站異常維護資料表.工作站編號, 工作站異常維護資料表.排程編號, 工作站型態資料表.工作站名稱, IFNULL(b.責任單位, 工作站異常維護資料表.責任單位) 責任單位, IFNULL(b.異常原因類型, 工作站異常維護資料表.異常原因類型) 異常類型 FROM 工作站異常維護資料表 LEFT JOIN (SELECT  * FROM 工作站異常維護資料表, (SELECT MAX(異常維護編號) 異編, 排程編號 排編, 父編號 父編 FROM 工作站異常維護資料表 WHERE 父編號 IS NOT NULL GROUP BY 排程編號 , 父編號) a WHERE 工作站異常維護資料表.異常維護編號 = a.異編 AND 工作站異常維護資料表.排程編號 = a.排編 AND 工作站異常維護資料表.父編號 = a.父編) b ON 工作站異常維護資料表.排程編號 = b.排程編號 AND 工作站異常維護資料表.異常維護編號 = b.父編號 LEFT JOIN 工作站型態資料表 ON 工作站異常維護資料表.工作站編號 = 工作站型態資料表.工作站編號 left join 工作站狀態資料表 on 工作站狀態資料表.排程編號 = 工作站異常維護資料表.排程編號 WHERE 工作站異常維護資料表.父編號 IS NULL AND (b.結案判定類型 IS NULL OR LENGTH(b.結案判定類型) = 0) and (工作站狀態資料表.狀態 <> '完成' OR 進度 <> '100')";
            dt_monthtotal = DataTableUtils.GetDataTable(sqlcmd);
        }
        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                DataTable dt_Line = dt_monthtotal.DefaultView.ToTable(true, new string[] { "工作站名稱" });
                DataTable dt_dpm = dt_monthtotal.DefaultView.ToTable(true, new string[] { "責任單位" });

                //產生datatable
                DataTable dt = new DataTable();
                //產生資料
                dt.Columns.Add("責任單位");
                foreach (DataRow row in dt_dpm.Rows)
                    dt.Rows.Add(DataTableUtils.toString(row["責任單位"]));

                //產生欄位
                foreach (DataRow row in dt_Line.Rows)
                    dt.Columns.Add(DataTableUtils.toString(row["工作站名稱"]));

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, Asm_ResponeDepartment_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

        private string Asm_ResponeDepartment_callback(DataRow row, string fieldname)
        {
            string value = "";
            string url = "";
            if (fieldname != "責任單位")
            {
                string sqlcmd = $" 工作站名稱='{fieldname}' and 責任單位='{row["責任單位"]}'";
                DataRow[] rows = dt_monthtotal.Select(sqlcmd);
                string Number = "";
                if (rows != null && rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                        Number += $"{rows[i]["排程編號"]}#";
                    url = $"ReqType=Line,schdule={Number},LineNum={rows[0]["工作站編號"]},responedep={row["責任單位"]}";
                }
                value = rows.Length.ToString();
            }
            return value == "" ? "" : value == "0" ? $"<td style=\"text-align:right\">0</td>" : $"<td style=\"text-align:right\"><u><a href=\"Asm_LineSearch.aspx?key={WebUtils.UrlStringEncode(url)}\">{value}</a></u></td>";
        }
    }
}