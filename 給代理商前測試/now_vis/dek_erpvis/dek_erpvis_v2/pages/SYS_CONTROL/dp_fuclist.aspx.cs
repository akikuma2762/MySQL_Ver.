using dek_erpvis_v2.cls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Support;

namespace dek_erpvis_v2.pages.SYS_CONTROL
{
    public partial class dp_fuclist : System.Web.UI.Page
    {
        public string color = "";
        myclass myclass = new myclass();
        clsDB_Server clsDB_sw = new clsDB_Server("");
        public string content = "";
        string acc = "";
        public string dp = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null && Request.QueryString["dp"] != null)
            {
                dp = Request.QueryString["dp"].Split('=')[0];
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
            load_page_data();
        }
        private void load_page_data()
        {
            clsDB_sw.dbOpen(myclass.GetConnByDekVisErp);
            string sqlcmd = 德大機械.控制台_權限管理.取得用戶個人資訊(acc);
            DataTable dt = clsDB_sw.GetDataTable(sqlcmd);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    sqlcmd = 德大機械.控制台_權限管理.取得選取用戶的權限詳細(acc, dp);
                    dt = clsDB_sw.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt))
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                            content += $"<u><a href='../{ DataTableUtils.toString(dt.Rows[i]["web_path"])}/{DataTableUtils.toString(dt.Rows[i]["web_url"])}.aspx'>{DataTableUtils.toString(dt.Rows[i]["web_pagename"])} </a></u><br /><br />";
                    }
                }
            }
            sqlcmd = 德大機械.控制台_權限管理.取得部門("SYS") + $" and DPM_NAME = '{dp}'";
            dp = DataTableUtils.toString(clsDB_sw.GetDataTable(sqlcmd).Rows[0]["DPM_NAME2"]);
        }
       
    }
}