﻿using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages
{
    public partial class inedex : System.Web.UI.Page
    {
        public string color = "";
        string acc = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];

            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
            }
            else 
                Response.Redirect("../login.aspx");
        }
    }
}