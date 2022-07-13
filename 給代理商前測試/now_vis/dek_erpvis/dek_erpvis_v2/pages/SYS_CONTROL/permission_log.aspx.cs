using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.SYS_CONTROL
{
    public partial class permission_log : System.Web.UI.Page
    {
        public string color = "";
        public string th = "";
        public string tr = "";
        string acc = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.Search_acc_Column(acc, "ADM") == "Y")
                {
                    if (!IsPostBack)
                        Set_Table();
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        private void Set_Table()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = "SELECT id 編號,acc 被變更者,changer 變更者,changetime 變更時間,permission 變更權限結果 from permission_log";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                string title = "";
                th = HtmlUtil.Set_Table_Title(dt, out title);
                tr = HtmlUtil.Set_Table_Content(dt, title, Manage_login_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string Manage_login_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "變更權限結果")
            {
                if (DataTableUtils.toString(row[field_name]) == "1")
                    value = $"<td >管理者</td>";
                else if(DataTableUtils.toString(row[field_name]) == "2")
                    value = $"<td >生管人員</td>";
                else if(DataTableUtils.toString(row[field_name]) == "3")
                    value = $"<td >現場人員</td>";
                else if(DataTableUtils.toString(row[field_name]) == "4")
                    value = $"<td >現場主管</td>";
                else if(DataTableUtils.toString(row[field_name]) == "5")
                    value = $"<td >一般使用者</td>";
                else
                value = $"<td ></td>";
            }
            else if (field_name == "變更時間")
                value = $"<td>{HtmlUtil.StrToDate(DataTableUtils.toString(row[field_name])):yyyy/MM/dd tt HH:mm:ss}</td>";
            return value;
        }
    }
}