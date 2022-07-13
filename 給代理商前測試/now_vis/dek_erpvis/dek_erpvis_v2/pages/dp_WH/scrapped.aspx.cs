using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dekERP_dll;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_WH
{
    public partial class scrapped : System.Web.UI.Page
    {
        public string color = "";
        public string date_str = "";
        public string date_end = "";
        public string th = "";
        public string tr = "";
        public string selected_orver_day = "";
        public string 總報廢金額 = "0";
        public string 最大金額品名規格 = "";
        double money = 0;
        public string path = "";
        string URL_NAME = "";
        string acc = "";
        string view_YN = "N";
        string titlename = "";
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        iTec_Materials PCD = new iTec_Materials();
        iTec_House WHE = new iTec_House();

        protected void Page_Load(object sender, EventArgs e)
        {
            URL_NAME = "scrapped";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("WHE");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc) )
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                            date_str = daterange[0];
                            date_end = daterange[1];
                            iniCbx();
                            if (CheckBoxList_staff.Items.Count > 0)
                                PlaceHolder_hide.Visible = true;
                            else
                                PlaceHolder_hide.Visible = false;
                            load_page_data();
                        }
                        if (txt_str.Text == "" && txt_end.Text == "")
                        {
                            txt_str.Text = HtmlUtil.changetimeformat(date_str, "-");
                            txt_end.Text = HtmlUtil.changetimeformat(date_end, "-");
                        }
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
        private void iniCbx()
        {
            CheckBoxList_staff.Items.Clear();
            DataTable dt = PCD.Item_DataTable("Scrapped_Man", date_str.Replace("-", ""), date_end.Replace("-", ""));
            if (HtmlUtil.Check_DataTable(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string staff = DataTableUtils.toString(dt.Rows[i]["報廢者"]);
                    ListItem listItem = new ListItem(staff, staff);
                    listItem.Selected = true;
                    CheckBoxList_staff.Items.Add(listItem);
                }
            }
        }

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        private void load_page_data()
        {
            view_YN = 德大機械.function_yn(acc, "報廢金額");
            Set_Html_Table();
        }
        private void block_1(DataTable dt)
        {
            if (HtmlUtil.Check_DataTable(dt) == true)
                最大金額品名規格 = DataTableUtils.toString(dt.Rows[0]["品名規格"]);
        }

        private void Set_Html_Table()
        {
            string date_s = HtmlUtil.changetimeformat(date_str);
            string date_e = HtmlUtil.changetimeformat(date_end);
            string col_name = "";
            DataTable dt = WHE.Scrapped(date_str.Replace("-", ""), date_end.Replace("-", ""), myclass.Insert_Condition("test2.USID.USER_NM", get_seleted_staff(), "or"));
            if (HtmlUtil.Check_DataTable(dt))
            {
                //印出欄位名稱
                if (dt.Rows.Count > 0)
                {
                    if (view_YN == "N")
                    {
                        dt.Columns.Remove("金額小計");
                        dt.Columns.Remove("標準成本");
                        PlaceHolder1.Visible = false;
                    }
                    block_1(dt);
                    List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                    th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                    tr = HtmlUtil.Set_Table_Content(true, dt, order_list, scrapped_callback).ToString();
                    總報廢金額 = money.ToString("N0");
                }
            }
            else
            {
                HtmlUtil.NoData(out th, out tr);
                if (view_YN == "N")
                    PlaceHolder1.Visible = false;
            }
        }

        private string scrapped_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "標準成本")
            {
                if (view_YN == "Y")
                    value = $"<td>{DataTableUtils.toString(row["標準成本"]).Trim()}</td>";
            }
            else if (field_name == "金額小計")
            {
                if (view_YN == "Y")
                {
                    value = $"<td>{DataTableUtils.toString(row["金額小計"]).Trim()}</td>";
                    money += DataTableUtils.toDouble(DataTableUtils.toString(row["金額小計"]));
                }
            }
            else if (field_name == "報廢數量")
                value = $"<td>{DataTableUtils.toString(row["報廢數量"])} {DataTableUtils.toString(row["單位"])}</td>";
            else if (field_name == "單據日期")
                value = $"<td>{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]))}</td>";
            return value;
        }
        private List<string> get_seleted_staff()
        {
            List<string> list = new List<string>();
            foreach (ListItem li in CheckBoxList_staff.Items)
            {
                if (li.Selected == true)
                    list.Add(li.Value);
            }
            return list;
        }

        protected void button_select_Click(object sender, EventArgs e)
        {
            date_str = txt_str.Text.Replace("-", "");
            date_end = txt_end.Text.Replace("-", "");
            iniCbx();
            load_page_data();
        }

        protected void txt_date_TextChanged(object sender, EventArgs e)
        {
            date_str = txt_str.Text.Replace("-", "");
            date_end = txt_end.Text.Replace("-", "");
            iniCbx();
            try
            {
                if (CheckBoxList_staff.Items.Count > 0)
                    PlaceHolder_hide.Visible = true;
                else
                    PlaceHolder_hide.Visible = false;
            }
            catch
            {

            }
        }
    }
}