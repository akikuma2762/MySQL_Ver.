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

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class waitingfortheproduction_Ver2 : System.Web.UI.Page
    {
        iTec_Product PMD = new iTec_Product();
        public string noselectdiv = "";
        myclass myclass = new myclass();
        string acc = "";
        public string color = "";
        public string timerange = "";
        public string 預定生產_data = "";
        public string 實際生產_data = "";
        public List<string> image_value = new List<string>();
        public List<StringBuilder> table_context = new List<StringBuilder>();
        public List<List<string>> image_data = new List<List<string>>();
        public List<StringBuilder> table_schdule = new List<StringBuilder>();
        public List<string> detail = new List<string>();
        //本月應生產之數量
        DataTable dt_本月應生產 = new DataTable();
        int count = 0;
        //扣除完成的數量
        DataTable dt_copy = new DataTable();
        //各產線用
        DataTable ds = new DataTable();
        ShareFunction sFun = new ShareFunction();
        德大機械 德大機械 = new 德大機械();
        string Line = "";
        string date_str = "";
        string date_end = "";
        public string path = "";
        public string Html_Code = "";
        public string Js_Image = "";
        public string Js_Table = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                path = 德大機械.get_title_web_path("PMD");
                var daterange = 德大機械.德大專用月份(acc).Split(',');
                string URL_NAME = "waitingfortheproduction";
                if ( HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0])|| myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            if (txt_str.Text == "" && txt_end.Text == "")
                            {
                                txt_str.Text = HtmlUtil.changetimeformat(daterange[0], "-");
                                txt_end.Text = HtmlUtil.changetimeformat(daterange[1], "-");
                            }
                            Gocenn();
                        }
                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");

                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }//4.423

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        private void Gocenn()
        {
            Html_Code = "";
            Js_Image = "";
            Js_Table = "";
            date_str = txt_str.Text.Replace("-", "");
            date_end = txt_end.Text.Replace("-", "");
            Set_Checkboxlist();
            Get_MonthTarget();//0.382
            timerange = $"'{HtmlUtil.changetimeformat(date_str)} ~ {HtmlUtil.changetimeformat(date_end)}'";//5.586
            noselectdiv = "";
            //存入產線名稱
            List<string> List_Text = new List<string>();
            //存入產線代號
            List<string> List_Value = new List<string>();
            int i = 0;
            //只顯示全部 OR 複合產線
            for (i = 0; i < CheckBoxList_Line.Items.Count; i++)
            {
                if (CheckBoxList_Line.Items[i].Selected)
                {
                    List_Text.Add(CheckBoxList_Line.Items[i].Text);
                    List_Value.Add(CheckBoxList_Line.Items[i].Value);
                }
            }
            //全部產線的生產推移圖 && 表格
            Get_All_Summary_Graph(dt_本月應生產, "", List_Text);
            Show_AllDataTable();

            i = 0;
            //產生顯示到前端的HTML碼
            Html_Code += Set_Html.Set_TabModel(List_Text[i], table_context[0 + (i * 2)].ToString(), table_context[1 + (i * 2)].ToString(), image_data[i]);



            Js_Image += Set_Html.Set_Image(List_Text[i], timerange, image_value[0 + (i * 2)], image_value[1 + (i * 2)]);
            Js_Table += Set_Html.Set_Table(List_Text[i]);


            //產生顯示到前端的HTML碼
            detail = image_data[i];


        }
        private void Get_All_Summary_Graph(DataTable dt, string LineNum = "", List<string> linelist = null)
        {
            double 應有台數 = 0;
            double 實際台數 = 0;
            double 總台數 = 0;
            double 應有進度 = 0;
            double 實際進度 = 0;
            double 未下架台數 = 0;
            double 落後台數 = 0;
            List<string> list = new List<string>();
            預定生產_data = "";
            實際生產_data = "";
            string LineNumber = "";
            List<string> Line = new List<string>();

            if (linelist != null)
            {
                for (int i = 0; i < linelist.Count; i++)
                {
                    if (i == 0)
                        LineNumber += $" 產線群組 = '{linelist[i]}' ";
                    else
                        LineNumber += $" OR 產線群組 = '{linelist[i]}' ";
                }
                if (LineNumber != "")
                    LineNumber = $" and ( {LineNumber} ) ";
            }

            if (LineNum != "")
            {
                Line = new List<string>(LineNum.Split(','));
                for (int i = 0; i < Line.Count - 1; i++)
                {
                    if (i == 0)
                        LineNumber += $" 產線代號 = '{Line[i]}' ";
                    else
                        LineNumber += $" OR 產線代號 = '{Line[i]}' ";
                }
                if (LineNumber != "")
                    LineNumber = $" and ( {LineNumber} ) ";
            }

            DataTable dt_day = Get_Monthday(date_str, date_end);
            string sqlcmd = "";
            DataRow[] rows = null;
            for (int i = 0; i < dt_day.Rows.Count; i++)
            {
                if (i == 0)
                    sqlcmd = $"(預計完工日 <= '{dt_day.Rows[i]["日期"]}' OR 預計完工日='開發機') and (substring(實際完成時間, 1, 8)>='{date_str}'  OR 實際完成時間 IS NULL  OR 實際完成時間 ='' ) {LineNumber} ";
                else
                    sqlcmd = $"預計完工日 = '{dt_day.Rows[i]["日期"]}' and (substring(實際完成時間, 1, 8)>='{date_str}' OR 實際完成時間 IS NULL  OR 實際完成時間 ='') {LineNumber}";
                rows = dt.Select(sqlcmd);

                預定生產_data += "{" + $"label:'{dt_day.Rows[i]["日期"].ToString().Substring(6, 2)}日',y:{rows.Length}" + "},";
                總台數 += rows.Length;
                if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(DataTableUtils.toString(dt_day.Rows[i]["日期"])))
                    應有台數 += rows.Length;

                sqlcmd = $" 實際完成時間  LIKE '%{dt_day.Rows[i]["日期"]}%' and 狀態 = '完成' {LineNumber}";
                rows = dt.Select(sqlcmd);

                if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(DataTableUtils.toString(dt_day.Rows[i]["日期"])))
                {
                    實際生產_data += "{" + $"label:'{dt_day.Rows[i]["日期"].ToString().Substring(6, 2)}日',y:{rows.Length} " + "},";
                    實際台數 += rows.Length;
                }
                if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) < DataTableUtils.toInt(date_str))
                    實際生產_data += "{" + $"label:'{dt_day.Rows[i]["日期"].ToString().Substring(6, 2)}日',y:{0} " + "},";
            }
            image_value.Add(預定生產_data);
            image_value.Add(實際生產_data);

            sqlcmd = $"(預計完工日 <= '{DateTime.Now.ToString("yyyyMMdd")}' OR 預計完工日='開發機') {LineNumber} and (狀態<>'完成' OR 狀態 IS NULL)";
            rows = dt.Select(sqlcmd);
            未下架台數 = rows.Length;

            sqlcmd = $"(預計完工日 < '{DateTime.Now.ToString("yyyyMMdd")}')  {LineNumber} and (狀態<>'完成' OR 狀態 IS NULL)";
            rows = dt.Select(sqlcmd);
            落後台數 = rows.Length;

            應有進度 = 應有台數 / 總台數;
            if (應有進度.ToString() == "非數值" || Double.IsInfinity(應有進度))
                應有進度 = 0;

            實際進度 = 實際台數 / 總台數;
            if (實際進度.ToString() == "非數值" || Double.IsInfinity(實際進度))
                實際進度 = 0;

            //應有進度
            list.Add(Math.Floor(應有進度 * 100) + "%");
            //應有台數
            list.Add("" + 應有台數);
            //總台數
            list.Add("" + 總台數);
            //實際進度
            list.Add(Math.Floor(實際進度 * 100) + "%");
            //實際生產台數
            list.Add("" + 實際台數);
            //總台數
            list.Add("" + 總台數);
            //未下架台數
            list.Add("" + 未下架台數);
            //落後台數
            list.Add("" + 落後台數);
            image_data.Add(list);
            //  Show_Schdule(LineNumber);
        }
        //顯示全部產線資訊
        private void Show_AllDataTable()
        {
            dt_copy = dt_本月應生產.Copy();
            //先扣除完成的數量
            DataRow[] rows = dt_copy.Select("狀態='完成'");
            for (int i = 0; i < rows.Length; i++)
                rows[i].Delete();

            DataTable Line = dt_copy.DefaultView.ToTable(true, new string[] { "產線群組" });
            DataTable custom = dt_copy.DefaultView.ToTable(true, new string[] { "客戶簡稱" });
            //清空重複項
            List<string> list = new List<string>();
            foreach (DataRow row in custom.Rows)
                list.Add(DataTableUtils.toString(row["客戶簡稱"]).Trim());
            list = list.Distinct().ToList();
            custom.Clear();
            for (int i = 0; i < list.Count; i++)
                custom.Rows.Add(list[i]);

            //存放產線
            StringBuilder sb_Column = new StringBuilder();
            //存放內容
            StringBuilder sb_Context = new StringBuilder();
            if (HtmlUtil.Check_DataTable(dt_copy))
            {

                foreach (DataRow row in Line.Rows)
                    custom.Columns.Add(row["產線群組"].ToString());
                custom.Columns.Add("小計");

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(custom, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                sb_Column = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                sb_Context = HtmlUtil.Set_Table_Content(true, custom, order_list, AllLine_callback);
            }
            else
            {
                sb_Column.Append("<th class='center'>沒有資料載入</th>");
                sb_Context.Append("<tr> <td class='center'> no data </td></tr>");
            }
            table_context.Add(sb_Column);
            table_context.Add(sb_Context);
        }
        //全產線用Callback
        private string AllLine_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name != "客戶簡稱" && field_name != "小計")
            {
                value = $"客戶簡稱='{DataTableUtils.toString(row["客戶簡稱"])}' and 產線群組='{field_name}' and (狀態<>'完成' OR 狀態 IS NULL)";
                DataRow[] rew = dt_copy.Select(value);
                if (rew.Length != 0)
                    value = "" + rew.Length;
                else
                    value = "0";
                count += DataTableUtils.toInt(value);
            }
            else if (field_name == "小計")//連結做在這裡
                value = Set_Url(DataTableUtils.toString(count), DataTableUtils.toString(row["客戶簡稱"]), date_str, date_end, "", "");
            else if (field_name == "客戶簡稱")
            {
                count = 0;
                value = DataTableUtils.toString(row["客戶簡稱"]).Trim();
            }

            if (value == "")
                return "";
            else
                return $"<td>{value}</td>\n";
        }
        //顯示各產線資訊


        //列出當月每一天
        private DataTable Get_Monthday(string start, string end)
        {
            DateTime start_time = ShareFunction.StrToDate(start);
            DateTime end_time = ShareFunction.StrToDate(end);
            TimeSpan span = end_time - start_time;
            DataTable dt = new DataTable();
            dt.Columns.Add("日期", typeof(string));
            for (int i = 0; i < Int16.Parse(span.TotalDays.ToString()) + 1; i++)
                dt.Rows.Add(start_time.AddDays(i).ToString("yyyyMMdd"));
            return dt;
        }
        //統計出本月應作之項目
        private void Get_MonthTarget()
        {
            dt_本月應生產 = new DataTable();
            //取得預計開工日 OR 預計完工日若在本月的
            DataTable dt_預生產數量 = PMD.waitingfortheproduction(date_str, date_end);
            if (HtmlUtil.Check_DataTable(dt_預生產數量))
                dt_預生產數量 = sFun.Get_Imformation(dt_預生產數量, date_str, date_end, 1);
            DataTable dt_未完工列表 = new DataTable();
            DataTable dt_未如期下架 = new DataTable();
            if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(date_str))
            {
                //未如期下架數量

                dt_未如期下架 = PMD.waitingfortheproduction(date_str, "");
                if (HtmlUtil.Check_DataTable(dt_未如期下架))
                    dt_未如期下架 = sFun.Get_Imformation(dt_未如期下架, date_str, date_end, 2);
                //未完成之數量

                dt_未完工列表 = PMD.waitingfortheproduction(date_str, "");
                if (HtmlUtil.Check_DataTable(dt_未完工列表))
                    dt_未完工列表 = sFun.Get_Imformation(dt_未完工列表, date_str, date_end, 3);
            }


            //本月應做
            if (HtmlUtil.Check_DataTable(dt_預生產數量))
            {
                dt_本月應生產 = dt_預生產數量.Clone();
                dt_預生產數量.PrimaryKey = new DataColumn[] { dt_預生產數量.Columns["排程編號"] };
                dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"] };
                dt_本月應生產.Merge(dt_預生產數量);
            }
            //上月應做 但本月完成
            if (HtmlUtil.Check_DataTable(dt_未如期下架))
            {
                if (HtmlUtil.Check_DataTable(dt_本月應生產))
                {
                    dt_未如期下架.PrimaryKey = new DataColumn[] { dt_未如期下架.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未如期下架, true, MissingSchemaAction.Ignore);
                }
                else
                {
                    dt_本月應生產 = dt_未如期下架.Clone();
                    dt_未如期下架.PrimaryKey = new DataColumn[] { dt_未如期下架.Columns["排程編號"] };
                    dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未如期下架);
                }
            }
            //到目前為止未完成
            if (HtmlUtil.Check_DataTable(dt_未完工列表))
            {
                if (HtmlUtil.Check_DataTable(dt_本月應生產))
                {
                    dt_未完工列表.PrimaryKey = new DataColumn[] { dt_未完工列表.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未完工列表, true, MissingSchemaAction.Ignore);
                }
                else
                {
                    dt_本月應生產 = dt_未完工列表.Clone();
                    dt_未完工列表.PrimaryKey = new DataColumn[] { dt_未完工列表.Columns["排程編號"] };
                    dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未完工列表);
                }

            }

            //依據產線作排序
            DataView dv_mant = new DataView(dt_本月應生產);
            dv_mant.Sort = "產線代號 asc";
            dt_本月應生產 = dv_mant.ToTable();
            //加入產線名稱
            dt_本月應生產 = myclass.Add_LINE_GROUP(dt_本月應生產).ToTable();
        }
        protected void button_select_Click(object sender, EventArgs e)
        {
            Gocenn();
        }
        private void Set_Checkboxlist()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = 德大機械.生產部.取得德大產線();
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            DataRow[] rows;
            string Line_Number;
            if (HtmlUtil.Check_DataTable(dt) && CheckBoxList_Line.Items.Count == 0)
            {
                CheckBoxList_Line.Items.Clear();
                DataTable Line = dt.DefaultView.ToTable(true, new string[] { "GROUP_NAME" });
                ListItem listItem = new ListItem("全產線", "全產線");
                listItem.Selected = true;
                CheckBoxList_Line.Items.Add(listItem);
                foreach (DataRow row in Line.Rows)
                {
                    Line_Number = "";
                    sqlcmd = $"GROUP_NAME='{DataTableUtils.toString(row["GROUP_NAME"])}'";
                    rows = dt.Select(sqlcmd);
                    if (rows.Length > 0)
                    {
                        for (int i = 0; i < rows.Length; i++)
                            Line_Number += rows[i]["LINE_ID"] + ",";
                    }
                    listItem = new ListItem(DataTableUtils.toString(row["GROUP_NAME"]), Line_Number);
                    listItem.Selected = true;
                    if (DataTableUtils.toString(row["GROUP_NAME"]) != "臥式")
                        CheckBoxList_Line.Items.Add(listItem);
                }
            }
        }
        //設定超連結
        private string Set_Url(string value, string cust, string start, string end, string type, string LineName)
        {
            string url = "";
            Line = LineName;

            int conut = 0;
            DataRow[] rows = dt_copy.Select($"客戶簡稱='{cust}' and (狀態<>'完成' OR 狀態 IS NULL)");
            for (int i = 0; i < rows.Length; i++)
                count = rows.Length;
            //加密後網址
            url = WebUtils.UrlStringEncode($"cust_name={cust.Trim()},date_str={start},date_end={end},Type={type},LineNumber={LineName}");
            value = $"<u><a href='waitingfortheproduction_details.aspx?key={url}'>{count}</a></u>";

            return value;
        }

    }
}