using dek_erpvis_v2.cls;
using Support;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2
{
    public partial class forget_pwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Label_code.Text = new Random().Next(9999).ToString();
        }
        protected void Button_add_Click(object sender, EventArgs e)
        {
            string acc = TextBox_Acc.Text;
            string phone_number = TextBox_Phone.Text;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = $"SELECT * FROM USERS where USER_ACC = '{acc}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                string table_name = "USERS";
                string sql_condi = $"USER_ACC = '{acc}' and USER_NUM = '{phone_number}'";
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                dt = DataTableUtils.DataTable_GetTable(table_name, sql_condi);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    DataRow row = DataTableUtils.DataTable_GetDataRow(table_name, sql_condi);
                    row["USER_PWD"] = dekSecure.dekHashCode.SHA256(TextBox_Pwd1.Text);
                    if (DataTableUtils.Update_DataRow(table_name, sql_condi, row))
                        Response.Write("<script>alert('伺服器回應 : 您已成功更新密碼! 日後請使用新密碼登入並請妥善保存!(確認後將跳轉到登入頁)');location.href='../login.aspx'</script>");
                }
                else
                    Response.Write("<script>alert('伺服器回應 : 手機號碼有誤! 請重新確認!');</script>");
            }
            else
                Response.Write("<script>alert('伺服器回應 : 找不到此帳號! 請重新確認!');</script>");
        }

    }
}