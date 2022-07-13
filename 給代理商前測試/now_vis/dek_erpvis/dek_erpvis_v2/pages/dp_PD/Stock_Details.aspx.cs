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
    public partial class Stock_Details : System.Web.UI.Page
    {
        //-------------------------------------------------參數 OR 引用------------------------------------------------------------
        public string itemname = "";
        public string color = "";
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
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
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
            itemname = TextBox_item.Text;
        }

        //取得輸入的領料總表
        private void Get_MonthTotal()
        {
            dt_monthtotal = TextBox_item.Text != "" ? custom.Stock_Details(TextBox_item.Text) : null;
            //dt_monthtotal.Columns.Add("料號", typeof(string));
            //dt_monthtotal.Columns.Add("品名", typeof(string));
            //dt_monthtotal.Columns.Add("規格", typeof(string));
            //dt_monthtotal.Columns.Add("良品數", typeof(string));
            //dt_monthtotal.Columns.Add("勘用品數量", typeof(string));
            //dt_monthtotal.Columns.Add("待修品數量", typeof(string));
            //dt_monthtotal.Columns.Add("其他總存量", typeof(string));
            //dt_monthtotal.Columns.Add("總庫存量", typeof(string));

            //dt_monthtotal.Rows.Add("沒有料號", "沒有用品名", "沒有規格", "沒有良品數", "沒有勘用品數量", "沒有待修品數量", "沒有其他總存量", "沒有總庫存量");
            //dt_monthtotal.Rows.Add("有料號", "有用品名", "有規格", "有良品數", "有勘用品數量", "有待修品數量", "有其他總存量", "有總庫存量");
        }
        //產生表格
        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt_monthtotal, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, dt_monthtotal, order_list).Replace(Environment.NewLine, "");
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

    }
}