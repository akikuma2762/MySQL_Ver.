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
    public partial class Asm_NoSolve_New : System.Web.UI.Page
    {
        public string th = "";
        public string tr = "";
        public string color = "";
        string acc = "";
        ShareFunction sfun = new ShareFunction();
        protected void Page_Load(object sender, EventArgs e)
        {

            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (!IsPostBack)
                    GotoCenn();
            }
            else
                Response.Redirect(myclass.logout_url);
        }

        private void GotoCenn()
        {
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                List<string> abnormal = new List<string>(HtmlUtil.Search_Dictionary(keyValues, "mach").Replace('*', '!').Split('!'));

                DataTable dt = sfun.GetNosovle_ITEC(abnormal);

                if (HtmlUtil.Check_DataTable(dt))
                {
                    string titlename = "";
                    th = HtmlUtil.Set_Table_Title(dt, out titlename);
                    tr = HtmlUtil.Set_Table_Content(dt, titlename, Asm_NoSolve_callback);
                }
                else
                    HtmlUtil.NoData(out th, out tr);
            }
            else
                Response.Redirect("waitingfortheproduction_New.aspx", false);
        }

        private string Asm_NoSolve_callback(DataRow row, string field_name)
        {
            string value = "";

            if (field_name == "上線日")
                value = HtmlUtil.StrToDate(DataTableUtils.toString(row[field_name])).ToString("MM/dd");
            else if (field_name == "預定進度")
            {
                string linenum = "";
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                string sqlcmd = $"SELECT  工作站編號 from 工作站型態資料表 where 工作站名稱 = '{row["產線"]}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (HtmlUtil.Check_DataTable(dt))
                    linenum = dt.Rows[0]["工作站編號"].ToString();
                sfun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                string result = sfun.percent_calculation(DataTableUtils.toString(row["編號"]), DataTableUtils.toString(row["進度"]), ref sqlcmd, linenum)[1].ToString();
                if (result == "非數值")
                    value = "開發機";
                else
                {
                    if (DataTableUtils.toDouble(result) * 100 > 100)
                        value = "100%";
                    else
                        value = (DataTableUtils.toDouble(result) * 100).ToString("0") + "%";
                }
            }
            else if (field_name == "進度")
                value = row[field_name].ToString() + "%";
            else if (field_name == "未解決數量")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                string sqlcmd = $"SELECT  工作站編號 from 工作站型態資料表 where 工作站名稱 = '{row["產線"]}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (HtmlUtil.Check_DataTable(dt))
                {
                    string url = $"ErrorID={row["編號"]},ErrorLineNum={dt.Rows[0]["工作站編號"]},ErrorLineName={row["產線"]}";
                    value = $"<u><a href=\"Asm_ErrorDetail.aspx?key={WebUtils.UrlStringEncode(url)}\">{row[field_name]}</a></u>";
                }
            }

            if (value == "")
                return "";
            else
                return $"<td style=\"text-align:center\">{value}</td>";
        }
    }
}