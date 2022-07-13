using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using dekERP_dll.dekErp;
using System.Configuration;

namespace dek_erpvis_v2
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// 此處為避免由excel OR WORD 等等處點擊連結需重新登入帳號密碼
        /// </summary>
        private static string MSUserAgentsRegex = @"[^\w](Word|Excel|PowerPoint|ms-office)([^\w]|\z)";
        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Request.UserAgent, MSUserAgentsRegex))
            {
                Response.Write("<html><head><meta http-equiv='refresh' content='0'/></head><body></body></html>");
                Response.End();
            }
        }
        
        protected void Application_Start(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 此處為多久要執行一次檢查任務 OR 一開起執行
        /// </summary>
        protected void Session_Start(object sender, EventArgs e)
        {
            if (DataTableUtils.toInt(HtmlUtil.Get_Ini("time_out", "inikey", "15")) > 0)
                Session.Timeout = DataTableUtils.toInt(HtmlUtil.Get_Ini("time_out", "inikey", "15"));
            delete_onlineuser();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 此處為離開時執行事件
        /// </summary>
        protected void Session_End(object sender, EventArgs e)
        {
            delete_onlineuser();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 移除紀錄時間為(目前時間-N分鐘)前之登入人員 
        /// </summary>
        private void delete_onlineuser()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = $"select * from now_login where now_time<='{DateTime.Now.AddMinutes(-DataTableUtils.toInt(HtmlUtil.Get_Ini("time_out", "inikey", "15"))):yyyyMMddHHmmss}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
                DataTableUtils.Delete_Record("now_login", $"now_time<='{DateTime.Now.AddMinutes(-DataTableUtils.toInt(HtmlUtil.Get_Ini("time_out", "inikey", "15"))):yyyyMMddHHmmss}'");
        }
    }
}