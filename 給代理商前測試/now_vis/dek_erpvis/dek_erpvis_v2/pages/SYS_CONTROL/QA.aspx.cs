using Dapper;
using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.SYS_CONTROL
{
    public partial class QA : System.Web.UI.Page
    {
        public string color = "";
        public string standard = "";
        public string UserDefine = "none";
        string acc = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                MainProcess();
            }
            else Response.Redirect(myclass.logout_url);
        }
        private void MainProcess()
        {
            string Path = System.AppDomain.CurrentDomain.BaseDirectory+ @"pages\File\QA.html";
            if (File.Exists(Path))
            {
                standard = "none";
                UserDefine = "";
            }
            else
            {
                standard = "";
                UserDefine = "none";
            }
        }


    }
}