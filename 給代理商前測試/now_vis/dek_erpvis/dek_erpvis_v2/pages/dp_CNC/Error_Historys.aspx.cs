using ClosedXML.Excel;
using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using dekERP_dll.dekErp;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class Error_Historys : System.Web.UI.Page
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
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check("Error_Historys", acc))
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
                Response.Redirect("Error_Historys.aspx");

        }
        //列印事件
        protected void Button_Print_Click(object sender, EventArgs e)
        {
            List<string> machlist = new List<string>(TextBox_MachGroupValue.Text.Split('^'));

            for (int i = 1; i < machlist.Count - 1; i++)
                condition += condition == "" ? $" (  mach_show_name='{machlist[i]}' ) " : $" OR (  mach_show_name='{machlist[i]}' ) ";
            condition = condition == "" ? "" : $" and ( {condition} ) ";

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"SELECT  mach_show_name 機台名稱, 工單號碼 工單號碼, 時間紀錄 發起時間, 維護人員姓名 發起人, 維護內容 發起內容, 異常維護編號 FROM error_report, machine_info WHERE (父編號 IS NULL OR 父編號 = 0) AND 模式 = '進站{DropDownList_Type.SelectedItem.Text}' {condition} AND machine_info.mach_name = error_report.機台名稱 ";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable ds = new DataTable();
                ds.Columns.Add("機台名稱");
                ds.Columns.Add("工單號碼");
                ds.Columns.Add("發起時間");
                ds.Columns.Add("發起人");
                ds.Columns.Add("發起內容");
                ds.Columns.Add("回覆時間");
                ds.Columns.Add("回覆人");
                ds.Columns.Add("回覆內容");
                ds.Columns.Add("異常連結");

                foreach (DataRow row in dt.Rows)
                {
                    DataRow rew = ds.NewRow();
                    rew["機台名稱"] = row["機台名稱"];
                    rew["工單號碼"] = row["工單號碼"];
                    rew["發起時間"] = HtmlUtil.StrToDate(DataTableUtils.toString(row["發起時間"]));
                    rew["發起人"] = row["發起人"];
                    rew["發起內容"] = row["發起內容"];
                    rew["異常連結"] = "連結";
                    ds.Rows.Add(rew);
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    sqlcmd = $"select * from error_report where 父編號 = {DataTableUtils.toString(row["異常維護編號"])}";
                    DataTable dd = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dd))
                    {
                        foreach (DataRow rrw in dd.Rows)
                        {
                            rew = ds.NewRow();
                            //表尚未結案
                            if (DataTableUtils.toString(rrw["結案判定類型"]) == null || DataTableUtils.toString(rrw["結案判定類型"]) == "")
                            {
                                rew["機台名稱"] = row["機台名稱"];
                                rew["工單號碼"] = row["工單號碼"];
                                rew["回覆時間"] = HtmlUtil.StrToDate(DataTableUtils.toString(rrw["時間紀錄"]));
                                rew["回覆人"] = DataTableUtils.toString(rrw["維護人員姓名"]);
                                rew["回覆內容"] = DataTableUtils.toString(rrw["維護內容"]);
                            }
                            //表已結案
                            else
                            {
                                rew["機台名稱"] = row["機台名稱"];
                                rew["工單號碼"] = row["工單號碼"];
                                rew["回覆時間"] = HtmlUtil.StrToDate(DataTableUtils.toString(rrw["時間紀錄"]));
                                rew["回覆人"] = "[結案類型]：" + DataTableUtils.toString(rrw["結案判定類型"]);
                                if (DataTableUtils.toString(rrw["結案內容"]) == null || DataTableUtils.toString(rrw["結案內容"]) == "")
                                    rew["回覆內容"] = DataTableUtils.toString(rrw["維護內容"]);
                                else
                                    rew["回覆內容"] = DataTableUtils.toString(rrw["結案內容"]);
                            }
                            rew["異常連結"] = "連結";
                            ds.Rows.Add(rew);
                        }
                    }
                    //用一個空格去做區分
                    rew = ds.NewRow();
                    rew["機台名稱"] = "";
                    rew["工單號碼"] = "";
                    rew["發起時間"] = "";
                    rew["發起人"] = "";
                    rew["發起內容"] = "";
                    rew["回覆時間"] = "";
                    rew["回覆人"] = "";
                    rew["回覆內容"] = "";
                    rew["異常連結"] = "";
                    ds.Rows.Add(rew);
                }
                ToExcel(ds);
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('列印失敗');location.href='Error_Historys.aspx';</script>");
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
            //GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            //string timerange = textbox_dt1.Text != "" && textbox_dt2.Text != "" ? $"  {textbox_dt1.Text.Replace("-", "")} <= substring(a.入站時間,1,8) and substring(a.入站時間,1,8) <= {textbox_dt2.Text.Replace("-", "")} " : "";
            //string type = DropDownList_Type.SelectedItem.Text != "全部" ? $" a.類型 = '進站{DropDownList_Type.SelectedItem.Text}' " : "";
            //string sqlcmd = $" SELECT  a.*, (SELECT  COUNT(*) FROM error_report WHERE a.工單號碼 = error_report.工單單號 AND a.機台代碼 = error_report.機台名稱 AND (父編號 IS NULL OR 父編號 = 0) AND a.入站時間 <= 時間紀錄 AND 時間紀錄 <= a.出站時間 AND error_report.模式 = a.類型) 異常數量, (SELECT  COUNT(*) FROM error_report WHERE a.工單號碼 = error_report.工單編號 AND a.機台代碼 = error_report.機台名稱 AND (結案判定類型 IS NOT NULL) AND a.入站時間 <= 時間紀錄 AND 時間紀錄 <= a.出站時間 AND error_report.模式 = a.類型) 結案數量, MIN(b.最後回覆時間) 最小回覆時間, '' `未解決/全部` FROM (SELECT DISTINCT record_worktime.mach_name 機台代碼, machine_info.mach_show_name 機台名稱, machine_info.area_name 機台群組, workorder_information.product_number 料號, workorder_information.work_staff 人員, record_worktime.manu_id 工單號碼, record_worktime.now_time 入站時間, record_worktime.type_mode 類型, (SELECT  MIN(now_time) FROM record_worktime a WHERE record_worktime.mach_name = a.mach_name AND record_worktime.manu_id = a.manu_id AND record_worktime.type_mode = a.type_mode AND record_worktime.product_number = a.product_number AND record_worktime.product_name = a.product_name AND a.now_time >= record_worktime.now_time AND workman_status = '出站') 出站時間 FROM record_worktime, machine_info, workorder_information WHERE workman_status = '入站' AND record_worktime.mach_name = machine_info.mach_name AND record_worktime.mach_name = workorder_information.mach_name AND record_worktime.manu_id = workorder_information.manu_id) a LEFT JOIN (SELECT  * FROM (SELECT  工單編號, 機台名稱, 模式, IFNULL((SELECT  MAX(時間紀錄) FROM error_report a WHERE a.父編號 = error_report.異常維護編號), 時間紀錄) 最後回覆時間, (SELECT  結案判定類型 FROM error_report a WHERE a.父編號 = error_report.異常維護編號 AND (結案判定類型 IS NOT NULL OR LENGTH(結案判定類型) > 0)) 結案判定類型 FROM error_report WHERE 父編號 IS NULL) a WHERE a.結案判定類型 IS NULL) b ON a.機台代碼 = b.機台名稱 AND a.工單號碼 = b.工單編號 AND a.入站時間 <= b.最後回覆時間 AND b.最後回覆時間 <= a.出站時間 AND b.模式 = a.類型 where   {timerange} {type}   {condition} GROUP BY 機台代碼 , 工單號碼 , 入站時間 , 出站時間 ";
            //dt_monthtotal = DataTableUtils.GetDataTable(sqlcmd);
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string timerange = textbox_dt1.Text != "" && textbox_dt2.Text != "" ? $"  {textbox_dt1.Text.Replace("-", "")} <= substring(workorder_information.now_time,1,8) and substring(workorder_information.last_updatetime,1,8) <= {textbox_dt2.Text.Replace("-", "")} " : "";
            string type = DropDownList_Type.SelectedItem.Text != "全部" ? $" and  a.類型 = '進站{DropDownList_Type.SelectedItem.Text}' " : "";

            string sqlcmd = $"SELECT DISTINCT a.機台代碼,a.機台名稱,a.機台群組,a.料號,a.人員,a.工單號碼, workorder_information.now_time 入站時間, workorder_information.last_updatetime 出站時間, a.類型, (SELECT COUNT(*) FROM error_report WHERE a.工單號碼 = error_report.工單號碼 AND a.機台代碼 = error_report.機台名稱 AND (父編號 IS NULL OR 父編號 = 0) AND a.入站時間 <= 時間紀錄 AND error_report.模式 = a.類型) 異常數量, (SELECT COUNT(*) FROM error_report WHERE a.工單號碼 = error_report.工單號碼 AND a.機台代碼 = error_report.機台名稱 AND (結案判定類型 IS NOT NULL) AND a.入站時間 <= 時間紀錄 AND error_report.模式 = a.類型) 結案數量, MIN(b.最後回覆時間) 最小回覆時間, '' [未解決/全部] FROM (SELECT DISTINCT record_worktime.mach_name 機台代碼, machine_info.mach_show_name 機台名稱, machine_info.area_name 機台群組, workorder_information.product_number 料號, workorder_information.work_staff 人員, record_worktime.manu_id 工單號碼, record_worktime.now_time 入站時間, record_worktime.type_mode 類型, (SELECT MIN(now_time) FROM record_worktime a WHERE record_worktime.mach_name = a.mach_name AND record_worktime.manu_id = a.manu_id AND record_worktime.type_mode = a.type_mode AND record_worktime.product_number = a.product_number AND record_worktime.product_name = a.product_name AND a.now_time >= record_worktime.now_time and a.type_mode = record_worktime.type_mode AND workman_status = '出站') 出站時間 FROM record_worktime, machine_info, workorder_information WHERE workman_status = '入站' AND record_worktime.mach_name = machine_info.mach_name AND record_worktime.mach_name = workorder_information.mach_name AND record_worktime.manu_id = workorder_information.manu_id and workorder_information.type_mode = record_worktime.type_mode) a LEFT JOIN (SELECT * FROM (SELECT 工單號碼, 機台名稱, 模式, IsNULL( (SELECT MAX(時間紀錄) FROM error_report a WHERE a.父編號 = error_report.異常維護編號), 時間紀錄) 最後回覆時間, (SELECT 結案判定類型 FROM error_report a WHERE a.父編號 = error_report.異常維護編號 AND (結案判定類型 IS NOT NULL OR LEN(結案判定類型) > 0)) 結案判定類型 FROM error_report WHERE 父編號 IS NULL) a WHERE a.結案判定類型 IS NULL) b ON a.機台代碼 = b.機台名稱 AND a.工單號碼 = b.工單號碼 AND a.入站時間 <= b.最後回覆時間 AND b.最後回覆時間 <= a.出站時間 AND b.模式 = a.類型 LEFT JOIN workorder_information ON workorder_information.mach_name = a.機台代碼 AND workorder_information.manu_id = a.工單號碼 and workorder_information.type_mode = a.類型 WHERE   {timerange} {type}   {condition}   GROUP BY a.機台代碼,a.機台名稱,a.機台群組,a.料號,a.人員,a.工單號碼, workorder_information.now_time, workorder_information.last_updatetime, a.類型,a.入站時間";
            dt_monthtotal = DataTableUtils.GetDataTable(sqlcmd);

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
                DataTable dt_Copy = HtmlUtil.Print_DataTable(dt_monthtotal, "機台名稱,工單號碼,類型,入站時間,出站時間,未解決/全部");

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt_Copy, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                order_list = order_list.Distinct().ToList();
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, dt_Copy, order_list, Error_Historys_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        //例外處理
        private string Error_Historys_callback(DataRow row, string field_name)
        {
            string value = "";
            if (field_name == "入站時間" || field_name == "出站時間")
            {
                if (DataTableUtils.toString(row[field_name]) != "")
                    value = HtmlUtil.StrToDate(DataTableUtils.toString(row[field_name])).ToString();
            }

            else if (field_name == "未解決/全部")
            {
                DataRow[] rows = dt_monthtotal.Select($"機台名稱='{row["機台名稱"]}' and 工單號碼='{row["工單號碼"]}' and 入站時間='{row["入站時間"]}' and 出站時間='{row["出站時間"]}'");
                if (rows != null && rows.Length > 0 && DataTableUtils.toString(rows[0]["異常數量"]) != "0")
                {
                    string url = WebUtils.UrlStringEncode($"machine={rows[0]["機台代碼"]},number={rows[0]["料號"]},order={rows[0]["工單號碼"]},show_name={rows[0]["機台名稱"]},group={rows[0]["機台群組"]},type={rows[0]["類型"]},staff={rows[0]["人員"]}");
                    string img_url = "";
                    double alert_time = DataTableUtils.toString(rows[0]["最小回覆時間"]) == "" ? 0 : ShareFunction.WorkTimeCaculator(DataTableUtils.toString(rows[0]["最小回覆時間"]), DateTime.Now.ToString("yyyyMMddHHmmss")).TotalMinutes;
                    double alert_max = DataTableUtils.toDouble(WebUtils.GetAppSettings("alert_time"));
                    if (alert_time >= alert_max)
                        img_url = "<img src=\"../../assets/images/shutdown.gif\" width=\"26px\" height=\"26px\">";
                    else
                        img_url = "<img src=\"../../assets/images/normal.png\" width=\"26px\" height=\"26px\">";

                    value = $"<a onclick=jump_ErrorDetail(\"{url}\") href=\"javascript: void()\"><div style=\"height:100%;width:100%;font-size:20px;vertical-align: middle\"> {img_url} <u> [  {DataTableUtils.toInt(rows[0]["異常數量"].ToString()) - DataTableUtils.toInt(rows[0]["結案數量"].ToString())}  /  {DataTableUtils.toInt(rows[0]["異常數量"].ToString())}  ] </u></div></a>";
                }
                else
                    value = $"<div style=\"height:100%;width:100%;font-size:20px;vertical-align: middle\"> </div>";
            }

            return value == "" ? "" : $"<td style=\"vertical-align: middle; text-align: center;\">{value}</td>";
        }
        public void ToExcel(DataTable dt)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "分頁");
                List<string> urllist = new List<string>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string url = "";
                    if (urllist.IndexOf(DataTableUtils.toString(dt.Rows[i]["機台名稱"]) + DataTableUtils.toString(dt.Rows[i]["工單號碼"])) == -1)
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        string sqlcmd = $"select workorder_information.mach_name,manu_id,type_mode,product_number,machine_info.mach_show_name,machine_info.area_name,work_staff from workorder_information,machine_info where workorder_information.mach_name = machine_info.mach_name and machine_info.mach_show_name = '{dt.Rows[i]["機台名稱"]}' and manu_id = '{dt.Rows[i]["工單號碼"]}'";
                        DataTable dts = DataTableUtils.GetDataTable(sqlcmd);
                        if (HtmlUtil.Check_DataTable(dts))
                            url = $"machine={dts.Rows[0]["mach_name"]},order={dts.Rows[0]["manu_id"]},type={dts.Rows[0]["type_mode"]},number={dts.Rows[0]["product_number"]},show_name={dts.Rows[0]["mach_show_name"]},group={dts.Rows[0]["area_name"]},staff={dts.Rows[0]["work_staff"]}";
                        url = $"{WebUtils.GetAppSettings("Line_ip")}:{WebUtils.GetAppSettings("Line_port")}/pages/dp_CNC/Error_Detail.aspx?key={WebUtils.UrlStringEncode(url)}";
                        urllist.Add(DataTableUtils.toString(dt.Rows[i]["機台名稱"]) + DataTableUtils.toString(dt.Rows[i]["工單號碼"]));
                        urllist.Add(url);
                    }
                    else
                        url = urllist[urllist.IndexOf(DataTableUtils.toString(dt.Rows[i]["機台名稱"]) + DataTableUtils.toString(dt.Rows[i]["工單號碼"])) + 1];

                    ws.Cell(i + 2, 9).Hyperlink = new XLHyperlink(@"" + url);
                }
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename={DateTime.Now:yyyyMMdd}.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}