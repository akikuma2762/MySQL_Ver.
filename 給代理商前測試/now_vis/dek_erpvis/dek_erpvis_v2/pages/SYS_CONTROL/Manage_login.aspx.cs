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
    public partial class Manage_login : System.Web.UI.Page
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
            string sqlcmd = "SELECT  now_login.id  刪除, USER_NAME  登入者名稱, WEB_PAGENAME 登入頁面, now_time  登入時間, DPM_NAME2  登入者部門 FROM now_login,users,web_pages,department where  users.USER_ACC = now_login.acc and web_pages.WEB_URL = now_login.page_name and  department.DPM_NAME = now_login.department";
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

            if (field_name == "刪除")
                value = $"<td ><input type=\"button\" class=\"btn btn-danger\"  value=\"刪除\" onclick=set_deleteid(\"{row["刪除"]}\") ></td>";
            else if (field_name == "登入時間")
                value = $"<td>{HtmlUtil.StrToDate(DataTableUtils.toString(row[field_name])):yyyy/MM/dd tt HH:mm:ss}</td>";

            return value;
        }
        //移除該帳號目前的登入狀況
        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            if (TextBox_ID.Text != "")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                string sqlcmd = $"select * from now_login where id = '{TextBox_ID.Text}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (HtmlUtil.Check_DataTable(dt))
                {
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                    if (DataTableUtils.Delete_Record("now_login", $"id = '{TextBox_ID.Text}'"))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('已取消該名使用者的登入');location.href='Manage_login.aspx';</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('刪除失敗');location.href='Manage_login.aspx';</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('刪除失敗');location.href='Manage_login.aspx';</script>");
            }
        }
    }
}