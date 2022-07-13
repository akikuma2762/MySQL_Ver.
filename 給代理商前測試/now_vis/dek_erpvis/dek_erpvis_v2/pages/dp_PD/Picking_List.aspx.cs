using dek_erpvis_v2.cls;
using dekERP_dll.dekErp;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class Picking_List : System.Web.UI.Page
    {
        //-------------------------------------------------參數 OR 引用------------------------------------------------------------
        public string color = "";
        public string order = "";
        string acc = "";
        public StringBuilder th = new StringBuilder();
        public StringBuilder tr = new StringBuilder();
        myclass myclass = new myclass();
        DataTable dt_monthtotal = new DataTable();
        ERP_Customized custom = new ERP_Customized();


        //----------------------------------------------------Event----------------------------------------------------------------------
        //載入事件
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);

                color = HtmlUtil.change_color(acc);
                if (true || HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                            MainProcess();
                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");

                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }

        //查詢事件
        protected void Button_submit_Click(object sender, EventArgs e)
        {
            MainProcess();
        }
        
        //欄位移動儲存事件
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        //----------------------------------------------------Function-------------------------------------------------------------------
        //需要執行副程式
        private void MainProcess()
        {
            Get_MonthTotal();
            Set_Table();
            order = TextBox_Order.Text;
        }

        //取得輸入的領料總表
        private void Get_MonthTotal()
        {
            dt_monthtotal = custom.Picking_List(TextBox_Order.Text, TextBox_Item.Text,TextBox_ItemName.Text);
        }
        //產生表格
        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt_monthtotal, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, dt_monthtotal, order_list, Picking_List_callback).Replace(Environment.NewLine, "");
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        //例外處理
        private string Picking_List_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "需求日")
                value = HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]));
            return value == "" ? "" : $"<td style=\"vertical-align: middle; text-align: center;\">{value}</td>";
        }

    }
}