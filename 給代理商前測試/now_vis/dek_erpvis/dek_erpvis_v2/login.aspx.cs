using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace dek_erpvis_v2.pages
{
    public partial class login : System.Web.UI.Page
    {
        clsDB_Server clsDB_sw = new clsDB_Server("");
        myclass myclass = new myclass();
        protected void Page_Load(object sender, EventArgs e)
        {
            string memory_ = DataTableUtils.toString(CheckBox_memory.Checked);
            if (!IsPostBack)
            {
                HttpCookie userInfo = Request.Cookies["userInfo"];
                if (userInfo != null)
                {
                    if (DataTableUtils.toString(userInfo["user_MRY"]) == "True")
                    {
                        TextBox_account.Text = DataTableUtils.toString(userInfo["user_ACC"]);
                        TextBox_password.Attributes.Add("value", DataTableUtils.toString(userInfo["user_PWD"]));
                        CheckBox_memory.Checked = true;
                    }
                    else
                    {
                        TextBox_account.Text = "";
                        TextBox_password.Text = "";
                        CheckBox_memory.Checked = false;
                    }
                }
            }
        }
        protected void Button_login_Click(object sender, EventArgs e)
        {
            string acc_ = DataTableUtils.toString(TextBox_account.Text.Trim());//.Replace("-", "").Replace("'", "");
            string pwd_ = DataTableUtils.toString(TextBox_password.Text.Trim());
            string memory_ = DataTableUtils.toString(CheckBox_memory.Checked);
            string dpm_ = "";
            string name_ = "";
            bool CheckOneDevice = false;
            bool error = true;

            if (pwd_.Length < 50)
                pwd_ = dekSecure.dekHashCode.SHA256(pwd_);
            if (acc_ != "" && pwd_ != "")
            {
                Regex regex = new Regex("[-']");
                error = regex.IsMatch(acc_) || regex.IsMatch(pwd_);
            }

            if (!error)
            {
                DataTable dt = myclass.user_login(acc_, pwd_);
                CheckOneDevice = WebUtils.GetAppSettings("Login_OneDevice") == "1" ? true : false;

                if (HtmlUtil.Check_DataTable(dt))
                {
                    //不卡單一登入直接進 //管理者不受限制// 卡單一登入還要驗證  
                    if (!CheckOneDevice || dt.Rows[0]["ADM"].ToString() == "Y" || (CheckOneDevice && HtmlUtil.check_loginLive(acc_, get_client_ip(2), Request.Browser.Platform.ToString())))
                    {
                        name_ = DataTableUtils.toString(dt.Rows[0]["USER_NAME"]);
                        dpm_ = DataTableUtils.toString(dt.Rows[0]["USER_DPM"]);
                        acc_ = DataTableUtils.toString(dt.Rows[0]["USER_ACC"]);
                        setCookies(name_, acc_, pwd_, memory_, dpm_);

                        users_login_record(acc_, 1);
                        login_success(德大機械.控制台_權限管理.是否為新用戶(acc_));
                    }
                    else
                    {
                        //帳號已在其他裝置登入過
                        Response.Write("<script language='javascript'>alert('伺服器回應 :該帳號目前登入中，請至原登入裝置登出!。');window.location.href='login.aspx'</script>");
                    }
                }
                else if (dt != null && dt.Rows.Count == 0)
                {
                    users_login_record(acc_, 0);
                    login_failed(dt);
                }
                else if (dt == null)
                {
                    users_login_record(acc_, 0);
                    login_failed(dt, true);
                }

            }
            else
                login_failed(null);

        }
        private void setCookies(string session_name, string session_acc, string session_pwd, string memory_, string dpm_)
        {
            HttpCookie userInfo = new HttpCookie("userInfo");
            userInfo["user_ACC"] = session_acc;                            //帳號
            userInfo["user_PWD"] = session_pwd;                            //密碼
            userInfo["user_NAME"] = HttpUtility.UrlEncode(session_name);   //用戶名稱
            userInfo["user_MRY"] = memory_;                                //是否記住
            userInfo["user_DPM"] = dpm_;                                   //所屬部門
            userInfo.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(userInfo);
        }
        private void login_success(int FirstTime)
        {
            string initpage = WebUtils.GetAppSettings("Inital_Page");
            if (string.IsNullOrEmpty(initpage))
            {
                if (FirstTime <= 1)
                    Response.Write("<script language='javascript'>alert('伺服器回應 :首次登入系統，您已登入成功! 將為您轉至 操作手冊。');window.location.href='/pages/SYS_CONTROL/QA.aspx'</script>");
                else
                    Response.Write("<script language='javascript'>window.location.href='/pages/Index.aspx'</script>");
            }
            else
            {
                    Response.Write("<script language='javascript'>window.location.href='/pages/"+ initpage + "'</script>");
            }
        }
        private void login_failed(DataTable dt, bool ok = false)
        {
            if (dt != null)
                Response.Write($"<script language='javascript'>alert('伺服器回應 : 您的帳號已被停用或密碼輸入錯誤');</script>");
            else
            {
                if (ok)
                    Response.Write($"<script language='javascript'>alert('伺服器回應 : 資料庫開啟失敗');</script>");
                else
                    Response.Write($"<script language='javascript'>alert('伺服器回應 : 您輸入的帳號或密碼存在特殊符號');</script>");
            }
        }
        private void users_login_record(string user_acc, int status)
        {
            bool ok = false;
            clsDB_sw.dbOpen(myclass.GetConnByDekVisErp);
            Random Rnd = new Random();
            DataTable dt = clsDB_sw.GetDataTable("SELECT * FROM SYSTEM_USERSLOGIN_log ");
            if (dt != null)
            {
                //clsDB_sw.Txact_begin();
                DataRow row = dt.NewRow();
                row["SU_ID"] = DataTableUtils.toString("SU" + Rnd.Next(99999));
                row["USER_ACC"] = user_acc;
                row["LOGIN_TIME"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                row["CLIENT_IP"] = get_client_ip(2);
                row["STATUS"] = status;
                row["FROM_"] = Request.Browser.Platform.ToString();//瀏覽器平台..
                ok = clsDB_sw.Insert_DataRow("SYSTEM_USERSLOGIN_log", row);

                DataTable dt1 = clsDB_sw.DataTable_GetTable($"SELECT * FROM SYSTEM_USERSLOGIN_log where su='{row["SU_ID"]}'");
               // clsDB_sw.Txact_commit();
            }
        }
        private string get_client_ip(int return_type)
        {

            switch (return_type)
            {
                case 0: // 判斷是否有使用 Proxy
                    if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == "")
                        return Request.ServerVariables["REMOTE_ADDR"];
                    else // proxy
                        return Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                case 1:  // 上一頁的網址（從哪來）
                    return Request.ServerVariables["HTTP_REFERER"];
                case 2: // 直接回傳抓到的 client IP
                    return Request.ServerVariables["REMOTE_ADDR"];
            }
            return "";
        }
    }
}