using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using dekERP_dll;
using dekERP_dll.dekErp;
using Support.DB;

namespace dek_erpvis_v2.pages.dp_WH
{
    public partial class InactiveInventory : System.Web.UI.Page
    {
        public string color = "";
        public string title_text = "";
        public string th = "";
        public string tr = "";
        public string item_type_name = "";
        public string item_type_code = "";
        public string overdate = "";
        public string selected_orver_day = "";
        public string path = "";
        string URL_NAME = "";
        string acc = "";
        string view_YN = "N";
        DataTable dt = new DataTable();
        int total_count;
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        iTec_House WHE = new iTec_House();
        iTec_Materials PCD = new iTec_Materials();
        List<string> Position = new List<string>();
        List<string> item_class = new List<string>();
        public List<double> cost = new List<double>();
        protected void Page_Load(object sender, EventArgs e)
        {
            cost.Add(0);
            cost.Add(0);
            cost.Add(0);
            cost.Add(0);
            URL_NAME = "InactiveInventory";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("WHE");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0])|| myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            iniCbx();
                            get_SqlConndi();
                            load_page_data();
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

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        private void iniCbx()
        {
            dt = PCD.Item_DataTable("InactiveInventory_Class");
            int j = 0;
            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    ListItem listItem = new ListItem(DataTableUtils.toString(row["C_NAME"]), DataTableUtils.toString(row["CLASS"]));
                    if (j++ == 0)
                        listItem.Selected = true;
                    CheckBoxList_itemtype.Items.Add(listItem);
                }
            }
        }
        private void get_SqlConndi()
        {
            item_type_name = DataTableUtils.toString(DropDownList_itemtype.SelectedItem);       // 物料類別名稱
            item_type_code = DataTableUtils.toString(DropDownList_itemtype.SelectedValue);      // 物料類別代碼
            selected_orver_day = DataTableUtils.toString(TextBoxdayval.Text);                  // 庫存期限(天數)
            overdate = date_trn();                                                              // 庫存期限(換算成日期)


            foreach (ListItem li in CheckBoxList_itemtype.Items)
            {
                if (li.Selected == true)
                    item_class.Add(li.Value);
            }

        }
        private void load_page_data()
        {
            Response.BufferOutput = false;
            view_YN = 德大機械.function_yn(acc, "呆料金額");
            Set_Html_Table();
        }
        private void Set_Html_Table()
        {
            string title = "";

            //物料種類 存放位置 N天前
            if (item_class.Count > 0)
                dt = WHE.InactiveInventory(myclass.Insert_Condition("test2.item.TYP_ITEM", item_class, "OR"), "", date_trn());

            if (HtmlUtil.Check_DataTable(dt))
            {

                DataView dv = new DataView(dt);
                DataTable result = dv.ToTable(true, "最後領料日", "品號", "品名規格", "倉位", "類別",  "剩餘總庫存");

                result.Columns.Add("領用數量");
                result.Columns.Add("進貨數量");
                result.Columns.Add("期初庫存");
                result.Columns.Add("領用比例");
                if (view_YN == "Y")
                {
                    result.Columns.Add("標準成本");
                    result.Columns.Add("金額小計");
                }
                else
                    div_present.Visible = false;

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(result, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, result, order_list, InactiveInventory_callback).ToString().Replace("'", "");
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string InactiveInventory_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "最後領料日")
                value = $"<td>{HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]).Split('.')[0])}</td>";


            else if (field_name == "剩餘總庫存")
                value = $"<td>{DataTableUtils.toInt(DataTableUtils.toString(row["剩餘總庫存"]))}</td>";
            else if (field_name == "領用數量")
            {
                value = $"最後領料日='{row["最後領料日"]}' and 品號='{row["品號"]}' and 倉位='{row["倉位"]}' and 類別='{row["類別"]}'";
                DataRow[] rew = dt.Select(value);
                if (rew != null && rew.Length > 0)
                    value = $"<td>{rew[0][field_name]}</td>";
                else
                    value = "<td>0</td>";
            }
            else if (field_name == "進貨數量")
            {
                value = $"最後領料日='{row["最後領料日"]}' and 品號='{row["品號"]}'  and 倉位='{row["倉位"]}' and 類別='{row["類別"]}'";
                DataRow[] rew = dt.Select(value);
                if (rew != null && rew.Length > 0)
                    value = $"<td>{rew[0][field_name]}</td>";
                else
                    value = "<td>0</td>";
            }
            else if (field_name == "期初庫存")
            {
                value = $"最後領料日='{row["最後領料日"]}' and 品號='{row["品號"]}' and 倉位='{row["倉位"]}' and 類別='{row["類別"]}'";
                DataRow[] rew = dt.Select(value);
                if (rew != null && rew.Length > 0)
                    value = $"<td>{rew[0][field_name]}</td>";
                else
                    value = "<td>0</td>";
            }
            else if (field_name == "領用比例")
            {

                value = $"最後領料日='{row["最後領料日"]}' and 品號='{row["品號"]}' and 倉位='{row["倉位"]}' and 類別='{row["類別"]}'";
                DataRow[] rew = dt.Select(value);
                double use = 0, origin_qty = 0, get_qty = 0;

                if (rew != null && rew.Length > 0)
                {
                    use = DataTableUtils.toDouble(DataTableUtils.toString(rew[0]["領用數量"]));
                    origin_qty = DataTableUtils.toDouble(DataTableUtils.toString(rew[0]["期初庫存"]));
                    get_qty = DataTableUtils.toDouble(DataTableUtils.toString(rew[0]["進貨數量"]));
                }

                double now_persent = DataTableUtils.toDouble((use * 100 / (origin_qty + get_qty)).ToString("0"));
                try
                {
                    if (now_persent < 25)
                        cost[0] += total_count * DataTableUtils.toInt(DataTableUtils.toString(row["標準成本"]).Split('.')[0]);
                    else if (now_persent >= 25 && now_persent < 50)
                        cost[1] += total_count * DataTableUtils.toInt(DataTableUtils.toString(row["標準成本"]).Split('.')[0]);
                    else if (now_persent >= 50 && now_persent < 75)
                        cost[2] += total_count * DataTableUtils.toInt(DataTableUtils.toString(row["標準成本"]).Split('.')[0]);
                    else if (now_persent >= 75 && now_persent <= 100)
                        cost[3] += total_count * DataTableUtils.toInt(DataTableUtils.toString(row["標準成本"]).Split('.')[0]);
                }
                catch
                {

                }
                value = $"<td>{now_persent}%</td>";

            }
            else if (field_name == "標準成本")
            {
                value = $"最後領料日='{row["最後領料日"]}' and 品號='{row["品號"]}' and 倉位='{row["倉位"]}' and 類別='{row["類別"]}'";
                DataRow[] rew = dt.Select(value);
                if (rew != null && rew.Length > 0)
                    value = $"<td>{rew[0][field_name]}</td>";
                else
                    value = "<td>0</td>";
            }
            else if (field_name == "金額小計")
            {
                value = $"最後領料日='{row["最後領料日"]}' and 品號='{row["品號"]}'  and 倉位='{row["倉位"]}' and 類別='{row["類別"]}'";
                DataRow[] rew = dt.Select(value);
                if (rew != null && rew.Length > 0)
                    value = $"<td>{DataTableUtils.toInt(rew[0]["標準成本"].ToString()) * total_count}</td>";
                else
                    value = "<td>0</td>";
            }

            if (value == "")
                return "";
            else
                return value;
        }
        private string date_trn()
        {
            return DataTableUtils.toString(DateTime.Now.AddDays(-DataTableUtils.toDouble(TextBoxdayval.Text)).ToString("yyyyMMdd"));
        }
        private string TransThousand(object yValue)//金額，千分位轉換
        {
            int yValue_trans = DataTableUtils.toInt(DataTableUtils.toString(yValue));
            return DataTableUtils.toString(yValue_trans.ToString("N0"));
        }
        protected void button_select_Click(object sender, EventArgs e)
        {
            get_SqlConndi();
            load_page_data();
        }
    }
}
