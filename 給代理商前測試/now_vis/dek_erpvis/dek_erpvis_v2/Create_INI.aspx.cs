using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2
{
    public partial class Create_INI : System.Web.UI.Page
    {
        dekERP_dll.dekErp.IniManager iniManager = new dekERP_dll.dekErp.IniManager(WebUtils.GetAppSettings("ini_road"));
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //資料庫加密
        protected void Button1_Click(object sender, EventArgs e)
        {
            //產生名稱
            string title = HtmlUtil.Encrypted_Text(TextBox2.Text);

            //資料庫類型
            HtmlUtil.Set_Ini(title, "inikey1", HtmlUtil.Encrypted_Text(TextBox1.Text));

            //資料庫名稱
            HtmlUtil.Set_Ini(title, "inikey", HtmlUtil.Encrypted_Text(TextBox2.Text));

            //資料庫IP
            HtmlUtil.Set_Ini(title, "inikey2", HtmlUtil.Encrypted_Text(TextBox3.Text));

            //資料庫帳號
            HtmlUtil.Set_Ini(title, "inikey3", HtmlUtil.Encrypted_Text(TextBox4.Text));

            //資料庫密碼
            HtmlUtil.Set_Ini(title, "inikey4", HtmlUtil.Encrypted_Text(TextBox5.Text));

        }
        //不加密
        protected void Button2_Click(object sender, EventArgs e)
        {
            List<string> title_list = new List<string>(TextBox7.Text.Split('^'));
            List<string> content_list = new List<string>(TextBox8.Text.Split('^'));

            for (int i = 0; i < title_list.Count; i++)
                HtmlUtil.Set_Ini(TextBox6.Text, title_list[i], content_list[i]); 
        }
        //須加密
        protected void Button3_Click(object sender, EventArgs e)
        {
            //產生名稱
            string title = HtmlUtil.Encrypted_Text(TextBox9.Text);

            //產生資料
            HtmlUtil.Set_Ini(title, "inikey", HtmlUtil.Encrypted_Text(TextBox10.Text));
        }
    }
}