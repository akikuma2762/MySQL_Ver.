using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using dekERP_dll.dekErp;
using Support;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class Record_Worktime : System.Web.UI.Page
    {
        public string path = "";
        public string color = "";
        string acc = "";
        public StringBuilder th = new StringBuilder();
        public StringBuilder tr = new StringBuilder();
        DataTable dt_monthtotal = new DataTable();
        string condition = "";
        myclass myclass = new myclass();
        //載入事件
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("DES");
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    if (!IsPostBack)
                    {
                        string[] date = 德大機械.德大專用月份(acc).Split(',');
                        textbox_dt1.Text = HtmlUtil.changetimeformat(date[0], "-");
                        textbox_dt2.Text = HtmlUtil.changetimeformat(date[1], "-");
                        MainProcess();
                    }
                }
                else
                    Response.Write("<script>alert('您無此權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //查詢事件
        protected void Unnamed_ServerClick(object sender, EventArgs e)
        {
            DropDownList_MachType.SelectedIndex = DropDownList_MachType.Items.IndexOf(DropDownList_MachType.Items.FindByValue(TextBox_MachTypeValue.Text));
            DropDownList_MachGroup.Items.Clear();
            List<string> list = new List<string>(TextBox_MachTypeValue.Text.Split(','));
            for (int i = 0; i < list.Count - 1; i++)
            {
                ListItem listItem = new ListItem(list[i], list[i + 1]);
                DropDownList_MachGroup.Items.Add(listItem);
                i++;
            }
            DropDownList_MachGroup.SelectedIndex = DropDownList_MachGroup.Items.IndexOf(DropDownList_MachGroup.Items.FindByText(TextBox_MachGroupText.Text));

            if (DropDownList_MachGroup.Items.Count != 0 && DropDownList_MachGroup.SelectedItem.Value.Split('^')[0] != "1")
            {
                MainProcess();
                Response.Redirect(DropDownList_MachGroup.SelectedItem.Value.Split('^')[0], "_blank", "");
                DropDownList_MachGroup.Items.Clear();
            }
            else if (DropDownList_MachType.SelectedItem.Text != "--Select--")
            {
                List<string> machlist = new List<string>(TextBox_MachGroupValue.Text.Split('^'));

                for (int i = 1; i < machlist.Count - 1; i++)
                    condition += condition == "" ? $" (  a.機台名稱='{machlist[i]}' ) " : $" OR (  a.機台名稱='{machlist[i]}' ) ";
                condition = condition == "" ? "" : $" and ( {condition} ) ";

                MainProcess();
            }
            else
                Response.Redirect("Record_Worktime.aspx");

        }
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        //須執行之事件
        private void MainProcess()
        {
            Get_MonthTotal();
            Set_Dropdownlist();
            Set_Table();
        }
        //取得入站時間在本月之工單
        private void Get_MonthTotal()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string type = DropDownList_Type.SelectedItem.Text != "全部" ? $" and a.模式 = '進站{DropDownList_Type.SelectedItem.Text}' " : "";

            string sqlcmd = $"SELECT distinct a.*, (a.良品數量 + a.不良品數量) 完工數量, b.暫停時間, b.取消暫停時間, b.除外暫停時間, b.結案時間, b.問題歸屬 FROM (SELECT record_worktime.mach_name 機台代號, machine_info.mach_show_name 機台名稱, record_worktime.manu_id 工單號碼, record_worktime.type_mode 模式, orderno 加工順序, workorder_information.craft_Number 製程代碼, workorder_information.craft_name 製程名稱, workorder_information.product_number 品號, workorder_information.product_name 品名, specification 規格, record_worktime.work_staff 人員名稱, record_worktime.now_time 開始日期, '' 開始時間, (SELECT MIN(a.now_time) FROM record_worktime a WHERE workman_status = '出站' AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.type_mode = record_worktime.type_mode AND a.manu_id = record_worktime.manu_id) 結束日期, '' 結束時間, '' [生產時間(分)], '' [人時(分)], '' [機時(分)], '' [暫停時間(分)], '' [暫停人時(分)], '' [暫停機時(分)],  '' [除外時間(分)], '' [除外人時(分)], '' [除外機時(分)], (SELECT max(report_qty) FROM record_worktime a WHERE ((workman_status = '良品出站') OR (workman_status = '出站' AND type_mode='進站維護' AND qty_status <> '不良品') OR (workman_status = '中途報工' AND qty_status = '良品')) AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.type_mode = record_worktime.type_mode AND a.manu_id = record_worktime.manu_id) 良品數量, (SELECT IsNULL(SUM(cast(bad_qty AS int)), 0) FROM bad_total WHERE bad_total.mach_name = record_worktime.mach_name AND bad_total.manu_id = record_worktime.manu_id AND bad_total.type_mode = record_worktime.type_mode) 不良品數量, '' [秒/PCS], workorder_information.custom_number 客戶代號, workorder_information.custom_name 客戶名稱 FROM record_worktime, machine_info, workorder_information WHERE machine_info.mach_name = record_worktime.mach_name AND record_worktime.mach_name = workorder_information.mach_name AND workorder_information.manu_id = record_worktime.manu_id AND workorder_information.type_mode = record_worktime.type_mode AND workman_status = '入站') a LEFT JOIN (SELECT DISTINCT a.機台編號, a.機台名稱 設備名稱, a.操作人員 人員名稱, a.工單號碼, a.模式, (CASE WHEN 暫停類型 IS NULL THEN NULL ELSE a.暫停時間 END) 暫停時間, (CASE WHEN 暫停類型 IS NULL THEN NULL ELSE a.取消暫停時間 END) 取消暫停時間, b.起始時間 除外暫停時間, IsNULL(b.結案時間, (CASE WHEN b.類型 IS NULL THEN NULL ELSE CONVERT(VARCHAR,GETDATE(), 112) + LEFT(REPLACE(CONVERT(VARCHAR,GETDATE(), 114), ':', ''), 6) END)) 結案時間, (SELECT belong FROM error_type WHERE error_type.err_type = IsNULL(a.暫停類型, b.類型)) 問題歸屬 FROM (SELECT record_worktime.mach_name 機台編號, machine_info.mach_show_name 機台名稱, work_staff 操作人員, manu_id 工單號碼, now_time 暫停時間, (SELECT IsNULL(MIN(a.now_time), CONVERT(VARCHAR,GETDATE(), 112) + LEFT(REPLACE(CONVERT(VARCHAR,GETDATE(), 114), ':', ''), 6)) FROM record_worktime a WHERE workman_status = '取消暫停' AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id and a.type_mode = record_worktime.type_mode) 取消暫停時間, record_worktime.type_mode 模式, (CASE WHEN stop_type = 'ERROR' THEN NULL ELSE stop_type END) 暫停類型, '' 暫停工時 FROM record_worktime, machine_info WHERE workman_status = '暫停' AND record_worktime.mach_name = machine_info.mach_name) a LEFT JOIN (SELECT 工單號碼, 機台名稱, 模式, IsNULL(最近回復類型, 異常原因類型) 類型, 時間紀錄 起始時間, (CASE WHEN 結案類型 IS NOT NULL THEN 最近回覆時間 ELSE NULL END) 結案時間 FROM (SELECT 工單號碼, 機台名稱, 異常原因類型, 時間紀錄, 模式, (SELECT MAX(時間紀錄) FROM error_report a WHERE a.父編號 = error_report.異常維護編號) 最近回覆時間, (SELECT IsNULL(結案判定類型, 異常原因類型) FROM error_report WHERE error_report.時間紀錄 = (SELECT MAX(時間紀錄) FROM error_report a WHERE a.父編號 = error_report.異常維護編號)) 最近回復類型, (SELECT a.結案判定類型 FROM error_report a WHERE a.時間紀錄 = (SELECT MAX(時間紀錄) FROM error_report a WHERE a.父編號 = error_report.異常維護編號)) 結案類型 FROM error_report WHERE (父編號 IS NULL OR 父編號 = 0)) b) b ON a.機台編號 = b.機台名稱 AND a.工單號碼 = b.工單號碼 AND 暫停類型 IS NULL  WHERE (a.暫停類型 IS NOT NULL OR b.類型 IS NOT NULL)) b ON a.機台代號 = b.機台編號 AND a.工單號碼 = b.工單號碼 AND a.人員名稱 = b.人員名稱 AND a.模式 = b.模式 where  {textbox_dt1.Text.Replace("-", "")} <= substring(a.開始日期,1,8)  and substring(a.開始日期,1,8)<={textbox_dt2.Text.Replace("-", "")} {type}  {condition}      ";
            dt_monthtotal = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                dt_monthtotal.Columns["完工數量"].SetOrdinal(26);
                dt_monthtotal = EachProcess(dt_monthtotal);
                //0628 juiedit --沒有結束時間就不要出現在報表
                var dt_monthtotal_Finish = dt_monthtotal.AsEnumerable().Where(w => !string.IsNullOrEmpty(w.Field<string>("結束日期")));
                if (dt_monthtotal_Finish.FirstOrDefault() != null)
                    dt_monthtotal = dt_monthtotal_Finish.CopyToDataTable();
            }

        }
        //設定廠區之下拉選單
        private void Set_Dropdownlist()
        {
            CNCError.Set_FactoryDropdownlist(acc, DropDownList_MachType, Request.FilePath);
        }
        //設定表格
        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt_monthtotal, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                order_list = order_list.Distinct().ToList();
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, dt_monthtotal, order_list);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        //整理表格
        private DataTable EachProcess(DataTable dt_source)
        {
            DataTable dt_Copy = dt_source.Copy();
            //整理表格
            dt_source = HtmlUtil.Print_DataTable(dt_source, "暫停時間,取消暫停時間,除外暫停時間,結案時間,問題歸屬", true);
            //變更長度欄位
            for (int i = 0; i < dt_source.Columns.Count; i++)
            {
                dt_source.Columns[i].ReadOnly = false;
                if (dt_source.Columns[i].MaxLength < 3)
                {
                    //避免無法變更而造成ERROR
                    try
                    {
                        dt_source.Columns[i].MaxLength = 15;
                    }
                    catch
                    {

                    }
                }
            }


            foreach (DataRow row in dt_source.Rows)
            {
                if (row["開始日期"].ToString() != "" && row["結束日期"].ToString() != "")
                {
                    //填入生產時間
                    row["生產時間(分)"] = ShareFunction.WorkTimeCaculator(row["開始日期"].ToString(), row["結束日期"].ToString()).TotalMinutes;
                    row["人時(分)"] = ShareFunction.WorkTimeCaculator(row["開始日期"].ToString(), row["結束日期"].ToString()).TotalMinutes;
                    row["機時(分)"] = ShareFunction.WorkTimeCaculator(row["開始日期"].ToString(), row["結束日期"].ToString()).TotalMinutes;
                    row["結束時間"] = HtmlUtil.StrToDate(row["結束日期"].ToString()).ToString("HH:mm:ss");
                }

                //填入開始 || 結束時間
                row["開始時間"] = HtmlUtil.StrToDate(row["開始日期"].ToString()).ToString("HH:mm:ss");

                double stop_time = 0;
                double exstop_time = 0;

                //填入屬於人員的暫停時間
                string sqlcmd = $"機台代號='{row["機台代號"]}' and 工單號碼='{row["工單號碼"]}' and 模式='{row["模式"]}' and 人員名稱='{row["人員名稱"]}' and 開始日期='{row["開始日期"]}'  ";
                DataRow[] rows = dt_Copy.Select(sqlcmd);
                if (rows != null && rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        stop_time += DataTableUtils.toString(rows[i]["暫停時間"]) != "" && DataTableUtils.toString(rows[i]["取消暫停時間"]) != "" ? ShareFunction.WorkTimeCaculator(DataTableUtils.toString(rows[i]["暫停時間"]), DataTableUtils.toString(rows[i]["取消暫停時間"])).TotalMinutes : 0;
                        exstop_time += DataTableUtils.toString(rows[i]["除外暫停時間"]) != "" && DataTableUtils.toString(rows[i]["結案時間"]) != "" ? ShareFunction.WorkTimeCaculator(DataTableUtils.toString(rows[i]["除外暫停時間"]), DataTableUtils.toString(rows[i]["結案時間"])).TotalMinutes : 0;
                    }
                }
                row["暫停人時(分)"] = stop_time;
                row["除外人時(分)"] = exstop_time;

                stop_time = 0;
                exstop_time = 0;

                //填入屬於機台的暫停時間
                sqlcmd = $"機台代號='{row["機台代號"]}' and 工單號碼='{row["工單號碼"]}' and 模式='{row["模式"]}' and 人員名稱='{row["人員名稱"]}' and 開始日期='{row["開始日期"]}'  and 問題歸屬='M'";
                rows = dt_Copy.Select(sqlcmd);
                if (rows != null && rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        stop_time += DataTableUtils.toString(rows[i]["暫停時間"]) != "" && DataTableUtils.toString(rows[i]["取消暫停時間"]) != "" ? ShareFunction.WorkTimeCaculator(DataTableUtils.toString(rows[i]["暫停時間"]), DataTableUtils.toString(rows[i]["取消暫停時間"])).TotalMinutes : 0;
                        exstop_time += DataTableUtils.toString(rows[i]["除外暫停時間"]) != "" && DataTableUtils.toString(rows[i]["結案時間"]) != "" ? ShareFunction.WorkTimeCaculator(DataTableUtils.toString(rows[i]["除外暫停時間"]), DataTableUtils.toString(rows[i]["結案時間"])).TotalMinutes : 0;
                    }
                }
                row["暫停機時(分)"] = stop_time;
                row["除外機時(分)"] = exstop_time;

                row["暫停時間(分)"] = DataTableUtils.toDouble(DataTableUtils.toString(row["暫停人時(分)"])) + DataTableUtils.toDouble(DataTableUtils.toString(row["暫停機時(分)"]));
                row["除外時間(分)"] = DataTableUtils.toDouble(DataTableUtils.toString(row["除外人時(分)"])) + DataTableUtils.toDouble(DataTableUtils.toString(row["除外機時(分)"]));

                row["秒/PCS"] = DataTableUtils.toString(row["完工數量"]) != "" && DataTableUtils.toString(row["完工數量"]) != "0" ? (DataTableUtils.toDouble(DataTableUtils.toString(row["生產時間(分)"])) * 60 / DataTableUtils.toDouble(DataTableUtils.toString(row["完工數量"]))).ToString("0.00") : "0";

                //最後再填入
                row["開始日期"] = HtmlUtil.StrToDate(row["開始日期"].ToString()).ToString("yyyy/MM/dd");
                if (row["結束日期"].ToString() != "")
                    row["結束日期"] = HtmlUtil.StrToDate(row["結束日期"].ToString()).ToString("yyyy/MM/dd");
            }

            return dt_source;
        }
    }
}