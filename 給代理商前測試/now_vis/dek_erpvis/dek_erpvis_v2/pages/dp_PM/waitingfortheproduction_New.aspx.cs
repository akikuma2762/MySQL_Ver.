using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class waitingfortheproduction_New : System.Web.UI.Page
    {
        public string path = "";
        public string color = "";
        string acc = "";
        string date_str = "";
        string date_end = "";
        string URL_NAME = "";
        myclass myclass = new myclass();
        德大機械 德大機械 = new 德大機械();
        DataTable dt_Finish = new DataTable();
        DataTable dt_NoFinish = new DataTable();
        DataTable dt_now = new DataTable();
        ShareFunction sfun = new ShareFunction();
        public int 預定生產_data_y_max = 0;
        public int 預計生產量_至今 = 0;
        public string 應有進度 = "3";
        public int 實際生產_data_y_max = 0;
        public string 實際進度 = "3";
        public string 差值 = "";
        public string sovling = "0";
        public string timerange = "";
        public string hide_image = "1";
        public string 預定生產_data = "";
        public string 實際生產_data = "";
        public string 實領料數_data = "";
        public string th = "";
        public string tr = "";
        DataTable dt_本月應生產 = new DataTable();
        DataTable dt_未生產完成 = new DataTable();
        int total = 0;
        string Line = "";
        string condition = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("PMD");
                URL_NAME = "waitingfortheproduction";
                color = HtmlUtil.change_color(acc);

                if (myclass.user_view_check(URL_NAME, acc) || true)
                {
                    if (!IsPostBack)
                    {
                        var daterange = 德大機械.德大專用月份(acc).Split(',');//0.1s
                        date_str = daterange[0];
                        date_end = daterange[1];
                        if (txt_str.Text == "" && txt_end.Text == "")
                        {
                            txt_str.Text = HtmlUtil.changetimeformat(date_str, "-");
                            txt_end.Text = HtmlUtil.changetimeformat(date_end, "-");
                        }
                        Print_Data();
                    }
                }
                else
                    Response.Write("<script>alert('您無此權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);

        }
        //------------------------------------------------------------按鈕事件-------------------------------------------------
        //把起始與結束時間設定成本月 || 搜尋事件
        protected void button_select_Click(object sender, EventArgs e)
        {
            if (DataTableUtils.toString(((Control)sender).ID.Split('_')[1]) == "select")
            {
                date_str = txt_str.Text.Replace("-", "");
                date_end = txt_end.Text.Replace("-", "");
                Print_Data();
            }
            else
            {
                string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                txt_str.Text = HtmlUtil.changetimeformat(daterange[0], "-");
                txt_end.Text = HtmlUtil.changetimeformat(daterange[1], "-");
            }

        }

        //------------------------------------------------------------副程式-------------------------------------------------
        //需要執行副程式
        private void Print_Data()
        {
            Set_CheckBox();
            Set_SearchLine();
            Get_MonthTotal();
            Show_Image();
            Show_Error();
            Show_DataTable();
            Set_ShowImage();
            //   Set_Factory();
        }
        //顯示目前有的產線
        private void Set_CheckBox()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = "select * from 工作站型態資料表 where (工作站是否使用中 = '1' OR 工作站是否使用中='True') and 工作站名稱 not like '%副線%'  order by 工作站編號 asc";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt) && checkBoxList_LINE.Items.Count == 0)
            {
                ListItem list = new ListItem();

                bool ok = true;
                foreach (DataRow row in dt.Rows)
                {
                    list = new ListItem(DataTableUtils.toString(row["工作站名稱"]), DataTableUtils.toString(row["工作站編號"]));
                    list.Selected = ok;
                    checkBoxList_LINE.Items.Add(list);

                }
            }
        }
        //使用者設定的產線
        private void Set_SearchLine()
        {
            condition = "";
            Line = "";
            bool ok = true;
            for (int i = 0; i < checkBoxList_LINE.Items.Count; i++)
            {
                if (checkBoxList_LINE.Items[i].Selected)
                {
                    condition += ok ? $" 工作站編號={checkBoxList_LINE.Items[i].Value} " : $" OR 工作站編號={checkBoxList_LINE.Items[i].Value} ";
                    Line += $"{checkBoxList_LINE.Items[i].Value}#";
                    ok = false;
                }
            }
            condition = condition != "" ? $" and ( {condition} ) " : "";

            //防呆用，怕沒選顯示副線
            if (condition == "")
            {
                ok = true;
                for (int i = 0; i < checkBoxList_LINE.Items.Count; i++)
                {
                    condition += ok ? $" 工作站編號={checkBoxList_LINE.Items[i].Value} " : $" OR 工作站編號={checkBoxList_LINE.Items[i].Value} ";
                    Line += $"{checkBoxList_LINE.Items[i].Value}#";
                    ok = false;
                }
                condition = condition != "" ? $" and ( {condition} ) " : "";
            }
        }
        //取得本月應做資料
        private void Get_MonthTotal()
        {
            timerange = $"'{HtmlUtil.changetimeformat(date_str, "/")}~{HtmlUtil.changetimeformat(date_end, "/")}'";
            dt_Finish = new DataTable();
            dt_NoFinish = new DataTable();
            //找出本月需做之數量
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $"SELECT 工作站編號 產線代號,工作站狀態資料表.排程編號,concat(組裝日,'080000') 上線日,CUSTNM 客戶簡稱,工作站編號,實際完成時間,狀態, 組裝日,(CASE 工作站編號 WHEN 1 THEN 組裝工時*60*60 WHEN 4 THEN ((CAST(刀臂點數 AS float)+CAST(刀套點數 AS float))*60*60) WHEN 5 THEN (CAST(刀鍊點數 AS float)*60*60) WHEN 6 THEN (CAST(全油壓點數 AS float)*60*60) END) 標準工時 FROM 工作站狀態資料表 LEFT JOIN 臥式工藝 ON 臥式工藝.機種編號 = SUBSTRING(排程編號, 1, length(排程編號)-8) LEFT JOIN 組裝資料表 ON 工作站狀態資料表.排程編號 = 組裝資料表.排程編號 WHERE ((組裝日>={HtmlUtil.StrToDate(date_str).AddMonths(-1):yyyyMMdd}         AND 組裝日 <={date_end})       OR (實際完成時間 IS NULL           OR Length(實際完成時間) =0)) {condition} ORDER BY 上線日";
            dt_now = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt_now))
                dt_now = sfun.Return_NowMonthTotal(dt_now, date_str, date_end, out dt_Finish, out dt_NoFinish);
            //合併
            if (dt_now != null)
            {
                dt_本月應生產 = dt_now.Clone();
                dt_now.PrimaryKey = new DataColumn[] { dt_now.Columns["排程編號"], dt_now.Columns["工作站編號"] };
                dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"], dt_本月應生產.Columns["工作站編號"] };
                dt_本月應生產.Merge(dt_now);
            }
            if (HtmlUtil.Check_DataTable(dt_Finish))
            {
                dt_Finish.PrimaryKey = new DataColumn[] { dt_Finish.Columns["排程編號"], dt_Finish.Columns["工作站編號"] };
                dt_本月應生產.Merge(dt_Finish, true, MissingSchemaAction.Ignore);
            }
            if (HtmlUtil.Check_DataTable(dt_NoFinish))
            {
                dt_NoFinish.PrimaryKey = new DataColumn[] { dt_NoFinish.Columns["排程編號"], dt_NoFinish.Columns["工作站編號"] };
                dt_本月應生產.Merge(dt_NoFinish, true, MissingSchemaAction.Ignore);
            }
        }
        //顯示圖片
        private void Show_Image()
        {
            if (HtmlUtil.Check_DataTable(dt_本月應生產))
            {
                DataTable dt_day = Get_Monthday(date_str, date_end);
                string sqlcmd = "";
                DataRow[] rows = null;
                for (int i = 0; i < dt_day.Rows.Count; i++)
                {
                    //預計生產
                    if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(date_str))
                        sqlcmd = (i == 0) ? $"(預計完工日 <= '{dt_day.Rows[i]["日期"]}' OR 預計完工日='開發機') and (substring(實際完成時間, 1, 8)>='{date_str}'  OR 實際完成時間 IS NULL  OR 實際完成時間 ='' ) " : $"預計完工日 = '{dt_day.Rows[i]["日期"]}' and (substring(實際完成時間, 1, 8)>='{date_str}' OR 實際完成時間 IS NULL  OR 實際完成時間 ='') ";
                    else
                        sqlcmd = $"預計完工日 = '{dt_day.Rows[i]["日期"]}' and (substring(實際完成時間, 1, 8)>='{date_str}' OR 實際完成時間 IS NULL  OR 實際完成時間 ='') ";


                    rows = dt_本月應生產.Select(sqlcmd);
                    預定生產_data += "{" + $"label:'{dt_day.Rows[i]["日期"].ToString().Substring(6, 2)}日',y:{rows.Length}" + "},";
                    預定生產_data_y_max += rows.Length;

                    //到今天為止應生產多少
                    預計生產量_至今 += DataTableUtils.toInt(dt_day.Rows[i]["日期"].ToString()) <= DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) ? rows.Length : 0;

                    //實際生產
                    sqlcmd = $" 實際完成時間  LIKE '%{dt_day.Rows[i]["日期"]}%' and 狀態 = '完成' ";
                    rows = dt_本月應生產.Select(sqlcmd);
                    if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(date_str))
                        實際生產_data += DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(DataTableUtils.toString(dt_day.Rows[i]["日期"])) ? "{" + $"label:'{dt_day.Rows[i]["日期"].ToString().Substring(6, 2)}日',y:{rows.Length} " + "}," : "";
                    else
                        實際生產_data += "{" + $"label:'{dt_day.Rows[i]["日期"].ToString().Substring(6, 2)}日',y:{rows.Length} " + "},";

                    實際生產_data_y_max += DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(DataTableUtils.toString(dt_day.Rows[i]["日期"])) ? rows.Length : 0;

                }
                應有進度 = DataTableUtils.toDouble(預計生產量_至今 * 100 / 預定生產_data_y_max).ToString("0") + "%";
                實際進度 = DataTableUtils.toDouble(實際生產_data_y_max * 100 / 預定生產_data_y_max).ToString("0") + "%";
                差值 = (實際生產_data_y_max - 預計生產量_至今).ToString();
            }
          
            else
            {
                應有進度 = "0%";
                實際進度 = "0%";
                差值 = "0";
                預定生產_data_y_max = 1;
            }
        }
        //顯示錯誤台數
        private void Show_Error()
        {
            //列出目前所有排程
            string condition = "";
            bool ok = true;
            if (HtmlUtil.Check_DataTable(dt_本月應生產))
            {
                foreach (DataRow row in dt_本月應生產.Rows)
                {
                    condition += ok ? $"  排程編號='{row["排程編號"]}' " : $" OR 排程編號='{row["排程編號"]}' ";
                    ok = false;
                }
                condition = condition != "" ? $" where ( {condition} ) " : "";
            }
            //取得目前未結案之數量
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $" SELECT DISTINCT 異常維護編號,排程編號,a.結案判定類型  FROM 工作站異常維護資料表 LEFT JOIN (SELECT max(異常維護編號) 編號,排程編號 排程號碼,父編號,結案判定類型 FROM 工作站異常維護資料表 where    結案判定類型 IS NOT NULL group by 父編號,排程編號,結案判定類型) a ON a.排程號碼 = 工作站異常維護資料表.排程編號 AND 工作站異常維護資料表.異常維護編號 = a.父編號 {condition} AND (工作站異常維護資料表.父編號 IS NULL OR 工作站異常維護資料表.父編號=0) AND 工作站編號=1 AND a.結案判定類型 IS NULL ORDER BY 工作站異常維護資料表.排程編號";
            DataTable dt_nosolve = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt_nosolve) && HtmlUtil.Check_DataTable(dt_本月應生產))
            {
                //存入目前未結案的排程編號
                List<string> list_schdule = new List<string>();
                for (int i = 0; i < dt_nosolve.Rows.Count; i++)
                    list_schdule.Add(dt_nosolve.Rows[i]["排程編號"].ToString());
                //排除重複項
                list_schdule = list_schdule.Distinct().ToList();
                //寫入排程編號跟維護編號
                string nosovle = "";
                foreach (DataRow row in dt_nosolve.Rows)
                    nosovle += $"{row["排程編號"]}!{row["異常維護編號"]}*";
                //導向網址
                sovling = $"<u><a href=\"Asm_NoSolve_New.aspx?key={WebUtils.UrlStringEncode("mach=" + nosovle)}\">{list_schdule.Count}</a></u>";

            }
            else
                sovling = "0";
        }
        //顯示表格
        private void Show_DataTable()
        {
            dt_本月應生產 = myclass.Add_LINE_GROUP(dt_本月應生產, "hor").ToTable();
            dt_未生產完成 = dt_本月應生產.Clone();
            string sqlcmd = " (實際完成時間 IS NULL  OR 實際完成時間 ='') and 狀態 <> '完成'";
            DataRow[] rows = dt_本月應生產.Select(sqlcmd);

            if (rows != null && rows.Length > 0)
                for (int i = 0; i < rows.Length; i++)
                    dt_未生產完成.ImportRow(rows[i]);


            //列出目前所有客戶
            DataTable custom = dt_未生產完成.DefaultView.ToTable(true, new string[] { "客戶簡稱" });
            //列出目前所有產線
            DataTable Line = dt_未生產完成.DefaultView.ToTable(true, new string[] { "產線群組" });
            //確定有客戶再列出
            if (HtmlUtil.Check_DataTable(custom))
            {
                foreach (DataRow row in Line.Rows)
                    custom.Columns.Add(row["產線群組"].ToString());
                custom.Columns.Add("小計");

                string title = "";
                th = HtmlUtil.Set_Table_Title(custom, out title);
                tr = HtmlUtil.Set_Table_Content(custom, title, waitingfortheproduction_ITEC_callback);
            }
            //沒有客戶不列出
            else
                HtmlUtil.NoData(out th, out tr);
        }
        //欄位處理用
        private string waitingfortheproduction_ITEC_callback(DataRow row, string field_name)
        {
            string value = "";

            if (field_name != "客戶簡稱" && field_name != "小計")
            {
                string sqlcmd = $"客戶簡稱='{row["客戶簡稱"]}' and 產線群組='{field_name}'";
                DataRow[] rows = dt_未生產完成.Select(sqlcmd);
                value = rows.Length.ToString();
                total += rows.Length;
            }
            else if (field_name == "小計")
            {
                //產生連結
                string url = $"cust_name={row["客戶簡稱"]},date_str={date_str},date_end={date_end},line={Line}";
                url = WebUtils.UrlStringEncode(url);
                value = $"<u><a href='waitingfortheproduction_details_New.aspx?key={url}' >{total} </a></u>";
                //把到目前為止的統計規零
                total = 0;
            }
            return value == "" ? "" : $"<td>{value}</td>";
        }
        //設定應顯示推移圖 || 領料圖
        private void Set_ShowImage()
        {
            if (dropdownlist_type.SelectedItem.Text == "生產推移圖")
                hide_image = "hidediv";
            else
                hide_image = "hidepercent";
        }
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
    }
}