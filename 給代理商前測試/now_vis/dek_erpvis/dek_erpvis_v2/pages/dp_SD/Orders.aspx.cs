using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Support;
using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using System.Data.OleDb;
using dekERP_dll;
using dekERP_dll.dekErp;
using dekDB;
using Support.DB;

namespace dek_erpvis_v2.pages.dp_SD
{
    public partial class Orders : System.Web.UI.Page
    {
        public string first_N = "";
        public string yText;
        public string unit = "";
        public string color = "";//顏色修改相關
        public string path = "";
        public string title = "";
        public string subtitle = "";
        public string dt_st = "";
        public string dt_ed = "";
        public string chart_unit = "數量";
        public string chartType = "";
        public string right_title = "";
        public string xString = "";
        public string XString = "";
        public string yString = "";
        public string chartData = "";
        public string status = "";
        public string Xtext = "";
        public string Ytext = "";
        string statusText = "";
        int CUST_TOTAL = 0;
        string[] str = null;
        public string th = "";
        public string tr = "";
        public string 排行內總計 = "";
        public string rate = "";
        public string Total = "";
        public int yTotal = 0;
        public string SubTotal = WebUtils.GetAppSettings("show_Subtotal");
        string acc = "";
        string URL_NAME = "";
        string view_YN = "N";
        int count = 0;
        DataTable Line = new DataTable();
        DataTable custom = new DataTable();
        DataTable dt = new DataTable();
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        iTec_Sales SLS = new iTec_Sales();
        iTec_Materials PCD = new iTec_Materials();
        OrderStatus orderstatus;
        OrderType orderType;
        OrderLineorCust orderLineor;
        List<string> linelist = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //效能測試
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("SLS");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            string[] s = 德大機械.德大專用月份(acc).Split(',');
                            dt_st = s[0];
                            dt_ed = s[1];
                            textbox_dt1.Text = HtmlUtil.changetimeformat(s[0], "-");
                            textbox_dt2.Text = HtmlUtil.changetimeformat(s[1], "-");
                            get_yStringContent();
                            MainProcess();
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
        //Process-------------------------------
        private void MainProcess()
        {
            Set_OrderStatus();
            GetCondition();
            GetChartData();
            Set_HTML_Table();
        }

        private void Set_OrderStatus()
        {
            DataTable dt = PCD.Item_DataTable("Order_Item");
            ListItem item = new ListItem();

            if (HtmlUtil.Check_DataTable(dt))
            {
                item = new ListItem("全部", "all");
                DropDownList_orderStatus.Items.Add(item);
                foreach (DataRow row in dt.Rows)
                {
                    item = new ListItem(DataTableUtils.toString(row["text"]), DataTableUtils.toString(row["value"]));
                    DropDownList_orderStatus.Items.Add(item);
                }
            }
        }
        private void GetCondition()
        {
            xString = dropdownlist_X.SelectedItem.Text;
            XString = dropdownlist_X.SelectedItem.Text;
            yString = dropdownlist_y.SelectedItem.Text;
            Xtext = dropdownlist_X.SelectedValue;
            Ytext = dropdownlist_y.SelectedValue;
            status = DropDownList_orderStatus.SelectedValue;
            statusText = DropDownList_orderStatus.SelectedItem.Text;
            title = statusText + "統計";

            status = DropDownList_orderStatus.SelectedValue;
            if (CheckBox_All.Checked != true)
                count = DataTableUtils.toInt(txt_showCount.Text);
            chartType = "column";

            switch (Xtext)
            {
                case "Custom":
                    orderLineor = OrderLineorCust.Custom;
                    break;
                case "Line":
                    orderLineor = OrderLineorCust.Line;
                    break;
            }

            switch (Ytext)
            {
                case "QUANTITY":
                    orderType = OrderType.數量;
                    break;
                case "AMOUNT":
                    orderType = OrderType.金額;
                    break;
            }
        }

        private void GetChartData()
        {
            string all_total = "";
            DataTable dt = new DataTable();
            //產線用
            if (Xtext == "Line")
            {
                if (dropdownlist_Datetype.SelectedItem.Value == "0")
                    dt = SLS.Orders($" '{dt_st}' <= test2.poln.DAT_DELS ", $" and  test2.poln.DAT_DELS <= '{dt_ed}' ", DropDownList_orderStatus.SelectedValue, dekModel.Image, orderLineor, orderType, 0);
                else
                    dt = SLS.Orders($" {dt_st} <= test2.pohd.dat_po  ", $" and  test2.pohd.dat_po  <= {dt_ed} ", DropDownList_orderStatus.SelectedValue, dekModel.Image, orderLineor, orderType, 0);
                right_title = "訂單總數量";
            }
            //客戶用
            else if (Xtext == "Custom")
            {
                if (dropdownlist_Datetype.SelectedItem.Value == "0")
                    dt = SLS.Orders($" '{dt_st}' <= test2.poln.DAT_DELS ", $" and  test2.poln.DAT_DELS <= '{dt_ed}' ", DropDownList_orderStatus.SelectedValue, dekModel.Image, orderLineor, orderType, 0);
                else
                    dt = SLS.Orders($" {dt_st} <= test2.pohd.dat_po  ", $" and  test2.pohd.dat_po  <= {dt_ed} ", DropDownList_orderStatus.SelectedValue, dekModel.Image, orderLineor, orderType, 0);

                if (HtmlUtil.Check_DataTable(dt))
                {
                    int num = 0;
                    foreach (DataRow row in dt.Rows)
                        num += DataTableUtils.toInt(DataTableUtils.toString(row[yString]).Split('.')[0]);
                    all_total = num.ToString();
                    Total = TransThousand(num);
                }
                else
                    Total = "0";

                if (dropdownlist_Datetype.SelectedItem.Value == "0")
                    dt = SLS.Orders($" '{dt_st}' <= test2.poln.DAT_DELS ", $" and  test2.poln.DAT_DELS <= '{dt_ed}' ", DropDownList_orderStatus.SelectedValue, dekModel.Image, orderLineor, orderType, count);
                else
                    dt = SLS.Orders($" {dt_st} <= test2.pohd.dat_po  ", $" and  test2.pohd.dat_po  <= {dt_ed} ", DropDownList_orderStatus.SelectedValue, dekModel.Image, orderLineor, orderType, count);

                right_title = "前" + count + "名客戶下單總數量";
            }






            if (HtmlUtil.Check_DataTable(dt))
            {
                if (Xtext == "Line")
                {
                    dt = myclass.Add_LINE_GROUP(dt).ToTable();
                    xString = "產線群組";
                }
                else if (Xtext == "Custom")
                    xString = "客戶簡稱";

                yTotal = 0;
                chartData = HtmlUtil.Set_Chart(dt, xString, yString, "", out yTotal, count).Replace("產線", "機種");

                if (Ytext == "QUANTITY")
                {
                    排行內總計 = yTotal.ToString();
                }
                else
                {

                    unit = "NTD ";
                    chart_unit = "金額";
                    if (Xtext == "Line")
                        排行內總計 = "NTD " + TransThousand(yTotal);
                    else
                        排行內總計 = "NTD " + TransThousand(all_total);
                }

                //判斷X軸"客戶"或"產線"，區分顯示標題
                if (xString == "客戶簡稱") //X軸= 客戶
                {
                    first_N = unit + TransThousand(yTotal);
                    subtitle = "前" + count + "名客戶總" + yString + "  " + unit + TransThousand(yTotal);
                    right_title = "前" + count + "名客戶下單總" + yString;
                    divBlock.Attributes["style"] = "display:block";
                }
                else                                      //X軸= 產線
                {
                    first_N = unit + TransThousand(yTotal);
                    subtitle = "總" + yString + "  " + unit + TransThousand(yTotal);
                    right_title = "訂單總" + yString;
                    divBlock.Attributes["style"] = "display:none";
                }
                //平均占比 = 前幾名數量 / 總數量 * 100 (For右上方小圖)            
                rate = DataTableUtils.toString(DataTableUtils.toDouble(yTotal) / DataTableUtils.toDouble(Total) * 100).Split('.')[0];
                xString = xString.Replace("產線", "機種");
            }
            else
            {
                subtitle = "沒有資料";
                排行內總計 = "沒有資料";
                rate = "0";
            }
        }
        private void Set_HTML_Table()
        {
            if (dropdownlist_Datetype.SelectedItem.Value == "0")
                dt = SLS.Orders($" '{dt_st}' <= test2.poln.DAT_DELS ", $" and  test2.poln.DAT_DELS <= '{dt_ed}' ", DropDownList_orderStatus.SelectedValue, dekModel.Table, orderLineor, orderType, 0);
            else
                dt = SLS.Orders($" {dt_st} <= test2.pohd.dat_po  ", $" and  test2.pohd.dat_po  <= {dt_ed} ", DropDownList_orderStatus.SelectedValue, dekModel.Table, orderLineor, orderType, 0);
            string titlename = "";
            if (HtmlUtil.Check_DataTable(dt))
            {
                dt = myclass.Add_LINE_GROUP(dt).ToTable();

                dt.Columns.RemoveAt(0);
                dt.Columns["產線群組"].SetOrdinal(1);

                //取得所有產線
                Line = dt.DefaultView.ToTable(true, new string[] { "產線群組" });
                //取得所有客戶
                custom = dt.DefaultView.ToTable(true, new string[] { "客戶簡稱" });

                foreach (DataRow row in Line.Rows)
                {
                    custom.Columns.Add(row["產線群組"].ToString());
                    linelist.Add(row["產線群組"].ToString());
                }
                custom.Columns.Add("小計");


                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(custom, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, custom, order_list, order_callback).ToString();

            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

        private string order_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "小計")
            {
                string url = HtmlUtil.AttibuteValue("cust", DataTableUtils.toString(row["客戶簡稱"]), "").Trim() + "," +
                            HtmlUtil.AttibuteValue("date_str", dt_st, "") + "," +
                            HtmlUtil.AttibuteValue("date_end", dt_ed, "") + "," +
                            HtmlUtil.AttibuteValue("type", dropdownlist_Datetype.SelectedItem.Value, "") + "," +
                            HtmlUtil.AttibuteValue("condi", DropDownList_orderStatus.SelectedValue, "");
                string href = string.Format("Orders_Details.aspx?key={0} ' ",
                    WebUtils.UrlStringEncode(url));
                CUST_TOTAL = 0;

                DataRow[] rows = dt.Select($"客戶簡稱='{row["客戶簡稱"]}'");
                for (int i = 0; i < rows.Length; i++)
                    CUST_TOTAL += DataTableUtils.toInt(rows[i][dropdownlist_y.SelectedItem.Text].ToString());

                value = TransThousand(DataTableUtils.toString(CUST_TOTAL));
                value = " align='right'>" + HtmlUtil.ToTag("u", HtmlUtil.ToHref(value, href));
            }
            else if (linelist.IndexOf(field_name) != -1)
            {
                //進入第一個產線後，將上一個總和歸0
                if (linelist.IndexOf(field_name) == 0)
                    CUST_TOTAL = 0;
                string sqlcmd = "客戶簡稱 ='" + DataTableUtils.toString(row["客戶簡稱"]) + "' and 產線群組 = '" + field_name + "'";
                DataRow[] rows = dt.Select(sqlcmd);
                int LINE_TOTAL = 0;
                if (rows.Length != 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        for (int j = 0; j < Line.Rows.Count; j++)
                        {
                            if (field_name == DataTableUtils.toString(rows[i]["產線群組"]) && field_name == DataTableUtils.toString(Line.Rows[j]["產線群組"]))
                                LINE_TOTAL += DataTableUtils.toInt(DataTableUtils.toString(rows[i][dropdownlist_y.SelectedItem.Text]));
                        }
                    }
                }
                value = "  align='right'>" + TransThousand(DataTableUtils.toString(LINE_TOTAL));
                CUST_TOTAL += LINE_TOTAL;
            }

            if (value == "")
                return value;
            else
                return "<td" + value + "</td>\n";
        }
        //Function------------------------------
        private string TransThousand(object yValue)//金額，千分位轉換
        {
            int yValue_trans = DataTableUtils.toInt(DataTableUtils.toString(yValue));
            return DataTableUtils.toString(yValue_trans.ToString("N0"));
        }

        private void get_yStringContent()
        {
            if (dropdownlist_y.Items.Count == 0)
            {
                // 2019.07.02，動態DropDownList，控管金額權限顯示(ru)
                view_YN = 德大機械.function_yn(acc, "訂單金額");
                if (view_YN == "Y")
                {
                    dropdownlist_y.Items.Add(new ListItem("數量", "QUANTITY"));
                    dropdownlist_y.Items.Add(new ListItem("金額", "AMOUNT"));
                }
                else
                    dropdownlist_y.Items.Add(new ListItem("數量", "QUANTITY"));

            }
        }

        //Event-------------------------------
        protected void Button_submit_Click(object sender, EventArgs e)
        {
            if (DataTableUtils.toString(((Control)sender).ID.Split('_')[1]) == "select")
            {
                dt_st = textbox_dt1.Text.Replace("-", "");
                dt_ed = textbox_dt2.Text.Replace("-", "");
                MainProcess();
            }
            else
            {
                string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                HtmlUtil.Button_Click(DataTableUtils.toString(((Control)sender).ID.Split('_')[1]), daterange, DataTableUtils.toString(textbox_dt1.Text), DataTableUtils.toString(textbox_dt2.Text), out dt_st, out dt_ed);
                textbox_dt1.Text = HtmlUtil.changetimeformat(dt_st, "-");
                textbox_dt2.Text = HtmlUtil.changetimeformat(dt_ed, "-");
            }
        }

    }
}
