using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using Support;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net.Mail;
using Telegram.Bot;

namespace dek_erpvis_v2.cls
{
    public class ShareFunction
    {
        public enum departmentSelect { 個人, 列表 };
        enum ErrorProcessStatus { 處理, 結案 };
        //
        clsDB_Server clsDB_Switch = new clsDB_Server("");
        clsDB_Server clsdb = new clsDB_Server(myclass.GetConnByDekdekVisAssmHor);
        //connect String
        public string GetConnByDekdekVisAssm = myclass.GetConnByDekdekVisAssm;
        public string GetConnByDekdekVisAssmHor = myclass.GetConnByDekdekVisAssmHor;
        public string GetConnByDekVisErp = myclass.GetConnByDekVisErp;
        public string GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
        //取得DekErp的資料-臥式-圓盤的資料(立式廠)
        // public static string GetConnByDekERPDataTable = myclass.GetConnByDekERPDataTable;
        static List<DateTime> holiday_list = new List<DateTime>();
        public static MyWorkTime work = new MyWorkTime(IsHoliday);
        static DataTable dt_holiday = dt_work();

        private static DataTable dt_work()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            return DataTableUtils.DataTable_GetTable("WorkTime_Holiday", "");
        }
        string change = "";
        string _cTotalTagetPiece;
        public string cTotalTagetPiece { get { return _cTotalTagetPiece; } set { _cTotalTagetPiece = value; } }
        string _cTotalTagetPerson;
        public string cTotalTagetPerson { get { return _cTotalTagetPerson; } set { _cTotalTagetPerson = value; } }
        string _cTotalFinishPiece;
        public string cTotalFinishPiece { get { return _cTotalFinishPiece; } set { _cTotalFinishPiece = value; } }
        string _cTotalErrorPiece;
        public string cTotalErrorPiece { get { return _cTotalErrorPiece; } set { _cTotalErrorPiece = value; } }
        string _cTotalOnLinePiece;
        public string cTotalOnLinePiece { get { return _cTotalOnLinePiece; } set { _cTotalOnLinePiece = value; } }
        //==========
        string _td_cTotalFinishPiece;
        public string td_cTotalFinishPiece { get { return _td_cTotalFinishPiece; } set { _td_cTotalFinishPiece = value; } }
        string _td_cTotalErrorPiece;
        public string td_cTotalErrorPiece { get { return _td_cTotalErrorPiece; } set { _td_cTotalErrorPiece = value; } }
        string _td_cTotalOnLinePiece;
        public string td_cTotalOnLinePiece { get { return _td_cTotalOnLinePiece; } set { _td_cTotalOnLinePiece = value; } }
        //======================================================================================
        public List<string> AnsQueryString(string QueryString)
        {
            string[] Qstr;
            string[] QSubStr = new string[2] { "0", "0" };
            List<string> StrParList = new List<string>();
            if (!string.IsNullOrEmpty(QueryString))
            {
                Qstr = QueryString.Split(',');
                StrParList.Add(Qstr[0]); // First  ?AA = 1
                if (Qstr.Length > 1)
                {
                    foreach (string Str in Qstr)
                    {
                        QSubStr = Str.Split('=');
                        if (QSubStr.Length > 1)
                            StrParList.Add(QSubStr[1]);
                    }
                }
                return StrParList;
            }
            else
                return StrParList;
        }
        //設定啟動時間 暫停時間 完成時間
        public bool change_status(string Link, DataTable dt, string tablename, string condition, string status, string Report, string percent = "")
        {
            if (dt != null)
            {
                DataRow row = dt.NewRow();
                string now_status = DataTableUtils.toString(dt.Rows[0]["狀態"]);
                if (percent != "100")
                {
                    switch (status)
                    {
                        case "啟動":
                            row["狀態"] = status;
                            row["進度"] = percent;
                            if (now_status == "未動工")
                                row["實際啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            else if (now_status == "暫停")
                                row["再次啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            row["實際完成時間"] = "";
                            break;
                        case "暫停":
                            row["狀態"] = status;
                            row["進度"] = percent;
                            if (DataTableUtils.toString(dt.Rows[0]["實際啟動時間"]) == "")
                                row["實際啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            row["暫停時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            row["實際完成時間"] = "";
                            break;
                        case "跑合":
                            row["狀態"] = "啟動";
                            row["進度"] = 99;
                            if (DataTableUtils.toString(dt.Rows[0]["實際啟動時間"]) == "")
                                row["實際啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            row["再次啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            row["實際完成時間"] = "";
                            break;
                        case "完成":
                            row["狀態"] = status;
                            row["進度"] = 100;
                            if (DataTableUtils.toString(dt.Rows[0]["實際啟動時間"]) == "")
                                row["實際啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            row["實際完成時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                            break;
                    }
                }
                else
                {
                    row["狀態"] = "完成";
                    row["進度"] = 100;
                    if (DataTableUtils.toString(dt.Rows[0]["實際啟動時間"]) == "")
                        row["實際啟動時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                    row["實際完成時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                row["問題回報"] = Report;

                return clsdb.Update_DataRow(tablename, condition, row);
            }
            else
                return false;
        }
        //增加或是修改子項目
        public bool Add_Content(string responedep, string showerrorlight, string post, string ID, string Error, string ErrorID, string Account, string Department, string Content, string ImageLink = "", int number = 0, string ErrorType = "", string close_content = "", string close_file = "")
        {

            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            string sqlcmd = "";
            DataTable dt = new DataTable();
            sqlcmd = $"select * from 工作站異常維護資料表 where 異常維護編號 = '{number}'";
            dt = DataTableUtils.GetDataTable(sqlcmd);
            int num = 0;
            string workstation = "";
            DataRow dt_Name = GetAccInf(Account);
            if (HtmlUtil.Check_DataTable(dt))
            {
                num = DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["父編號"]));
                workstation = DataTableUtils.toString(dt.Rows[0]["工作站編號"]);
                DataRow row = dt.NewRow();
                row["異常維護編號"] = dt.Rows[0]["異常維護編號"];
                row["維護內容"] = Content;
                if (ImageLink != "")
                    row["圖片檔名"] = ImageLink;
                row["異常原因類型"] = Error;
                if (DataTableUtils.toString(dt.Rows[0]["父編號"]) != "")
                {

                    row["結案判定類型"] = ErrorType;
                    row["結案內容"] = close_content;
                    if (close_file != "")
                        row["結案附檔"] = close_file;
                }
                if (post == "1")
                    row["是否有發送LINE"] = "Y";
                else
                    row["是否有發送LINE"] = "N";
                if (showerrorlight == "1")
                    row["是否持續顯示異常"] = "Y";
                else
                    row["是否持續顯示異常"] = "N";

                row["責任單位"] = responedep;

                GlobalVar.UseDB_setConnString(GetConnByDekVisTmp);
                if (DataTableUtils.Update_DataRow("工作站異常維護資料表", $"異常維護編號 = '{number}'", row))
                {
                    LineNote(post, DataTableUtils.toInt(workstation), ErrorID, "", Content, GetConnByDekVisTmp, num.ToString(), dt_Name["USER_NAME"].ToString(), responedep);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                GlobalVar.UseDB_setConnString(GetConnByDekVisTmp);
                sqlcmd = $"select * from 工作站異常維護資料表 where 異常維護編號 = '{ID}' and 排程編號 = '{ErrorID}'";
                dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {

                    DataRow row = dt.NewRow();
                    GlobalVar.UseDB_setConnString(GetConnByDekVisTmp);
                    sqlcmd = "select  * from 工作站異常維護資料表 order by 異常維護編號 desc limit 1 ";
                    DataTable dr = DataTableUtils.GetDataTable(sqlcmd);
                    if (number == 0)
                    {
                        num = DataTableUtils.toInt(DataTableUtils.toString(dr.Rows[0]["異常維護編號"])) + 1;
                        row["異常維護編號"] = num;
                        number = num;
                    }
                    else
                        row["異常維護編號"] = number;

                    row["排程編號"] = ErrorID;
                    row["異常原因類型"] = Error;
                    row["維護人員姓名"] = dt_Name["USER_NAME"].ToString();
                    row["維護人員單位"] = Department;
                    row["維護內容"] = Content;
                    row["時間紀錄"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                    row["圖片檔名"] = ImageLink;
                    row["工作站編號"] = dt.Rows[0]["工作站編號"];
                    row["父編號"] = ID;
                    row["結案判定類型"] = ErrorType;
                    row["結案內容"] = close_content;
                    row["結案附檔"] = close_file;
                    if (post == "1")
                        row["是否有發送LINE"] = "Y";
                    else
                        row["是否有發送LINE"] = "N";

                    if (showerrorlight == "1")
                        row["是否持續顯示異常"] = "Y";
                    else
                        row["是否持續顯示異常"] = "N";

                    row["責任單位"] = responedep;

                    GlobalVar.UseDB_setConnString(GetConnByDekVisTmp);
                    if (DataTableUtils.Insert_DataRow("工作站異常維護資料表", row))
                    {

                        LineNote(post, DataTableUtils.toInt(dt.Rows[0]["工作站編號"].ToString()), ErrorID, "", Content, GetConnByDekVisTmp, number.ToString(), dt_Name["USER_NAME"].ToString(), responedep, close_content);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

        }
        public List<string> CaseErrorType(string Link)
        {
            List<string> list = new List<string>();
            GlobalVar.UseDB_setConnString(Link);
            string sqlcmd = "select 備註內容 from 工作站結案異常類型資料表";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                list.Add("");
                foreach (DataRow row in dt.Rows)
                    list.Add(DataTableUtils.toString(row["備註內容"]));
            }
            return list;
        }
        public int SetErrorDataToDataTable(string LineNum, string Pk, string ErrorType)
        {
            bool OK = false;
            int ErrorNum = 0;
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            DataTable dt_er = DataTableUtils.DataTable_GetRowHeader(ShareMemory.SQLAsm_WorkStation_Error);
            DataRow dr = dt_er.NewRow();
            ErrorNum = GetSeriesNum(ShareMemory.SQLAsm_WorkStation_Error, 0);
            dr["異常排除時間"] = "Time";
            dr["異常編號"] = ErrorNum;
            dr["異常起始時間"] = DateTime.Now.ToString("yyyyMMddHHmmss");
            dr["異常原因"] = ErrorType;
            dr["工作站編號"] = LineNum;
            dr["排程編號"] = Pk;
            OK = DataTableUtils.Insert_DataRow(ShareMemory.SQLAsm_WorkStation_Error, dr);
            return ErrorNum;
        }
        /// <summary>
        /// 異常回復新增父項目
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="key">排程編號</param>
        /// <param name="ErrorType">異常類型</param>
        /// <param name="InputStr">異常訊息內容</param>
        /// <param name="dep">帳號所屬部門</param>
        /// <param name="Status">目前狀態 處理中/結案</param>
        /// <param name="StationInf"></param>
        /// <param name="value">是否要發送LINE</param>
        /// <param name="responedep">責任歸屬部門</param>
        /// <param name="errorlight">是否持續閃燈</param>
        /// <param name="Image_Save">照片 影片連結</param>
        /// <returns></returns>
        public bool SetMantDataToDataTable(string Account, string key, string ErrorType, string InputStr, string dep, string Status, Dictionary<string, string> StationInf, string value, string responedep, string errorlight, string Image_Save = "", string LineNumber = "")
        {
            //if (Account == "visrd")
            //    value = "0";
            InputStr = InputStr.Replace("'", " ");
            string backman = "";
            ////Mant
            bool OK = false;
            bool ProcessOK = false;

            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            string condition = "";
            string SqlStr = $"select  * From {ShareMemory.SQLAsm_WorkStation_ErrorMant} where 排程編號 = '{key}' ORDER BY 異常維護編號 desc limit 1";
            DataTable dt_er = DataTableUtils.DataTable_GetTable(SqlStr);
            DataRow dt_Name = GetAccInf(Account);
            if (dt_er == null)
                dt_er = DataTableUtils.DataTable_GetRowHeader(ShareMemory.SQLAsm_WorkStation_ErrorMant);
            if (Status == ErrorProcessStatus.結案.ToString())
                ProcessOK = true;
            //
            DataRow dr = dt_er.NewRow();
            int ErrorNum = GetSeriesNum(ShareMemory.SQLAsm_WorkStation_ErrorMant, 0);
            dr["異常維護編號"] = ErrorNum;
            dr["排程編號"] = key;
            dr["時間紀錄"] = $"{DateTime.Now:yyyyMMddHHmmss}";
            dr["維護人員單位"] = dep;
            dr["維護內容"] = InputStr;
            dr["異常原因類型"] = ErrorType;
            dr["處理狀態"] = ProcessOK;
            dr["圖片檔名"] = Image_Save;
            dr["責任單位"] = responedep;

            if (errorlight == "1")
                dr["是否持續顯示異常"] = "Y";
            else
                dr["是否持續顯示異常"] = "N";

            if (value == "1")
                dr["是否有發送LINE"] = "Y";
            else
                dr["是否有發送LINE"] = "N";

            if (dt_Name != null)
            {
                dr["維護人員姓名"] = dt_Name["USER_NAME"].ToString();
                backman = dt_Name["USER_NAME"].ToString();
            }
            if (GetConnByDekVisTmp.Contains("dekVisAssm"))
            {
                dr[ShareMemory.WorkStationNum] = LineNumber;
                condition = $"{ShareMemory.PrimaryKey}='{key}'";
            }
            else
            {
                dr[ShareMemory.WorkStationNum] = LineNumber;
                condition = $"{ShareMemory.PrimaryKey}='{key}' AND {ShareMemory.WorkStationNum}='{LineNumber}'";
            }
            // Mantan Table
            bool ok = DataTableUtils.Insert_DataRow(ShareMemory.SQLAsm_WorkStation_ErrorMant, dr);
            // Status Table
            DataRow dr_status = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLAsm_WorkStation_State, condition);
            if (dr_status != null)
            {
                dr_status["維護"] = $"{InputStr} {DateTime.Now:yyyy/MM/dd HH:mm:ss}";
                dr_status["異常"] = ErrorType;
                OK = DataTableUtils.Update_DataRow(ShareMemory.SQLAsm_WorkStation_State, condition, dr_status);
                //Updata Flag
                Note_MachineID_Line_Updata(dr_status["工作站編號"].ToString());

                LineNote(value, DataTableUtils.toInt(dr_status["工作站編號"].ToString()), key, ErrorType, InputStr, GetConnByDekVisTmp, ErrorNum.ToString(), backman, responedep);
                return OK;
            }
            else
                return OK;
        }
        public void Set_MachineID_Line_Updata(string LineNum)
        {
            bool OK = false;
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            DataTable dt = DataTableUtils.GetDataTable(ShareMemory.SQLAsm_MachineID_Line, $"機台產線代號='{LineNum}'");
            foreach (DataRow dr in dt.Rows)
            {
                if (!(dr["是否有更新資料現場"].ToString().ToUpper() == "TRUE" || dr["是否有更新資料現場"].ToString().ToUpper() == "1"))
                {
                    dr["是否有更新資料現場"] = true;
                    //insert or updata
                    if (DataTableUtils.RowCount(ShareMemory.SQLAsm_MachineID_Line, $"機台產線代號='{LineNum}'") != 0)
                        OK = DataTableUtils.Update_DataRow(ShareMemory.SQLAsm_MachineID_Line, $"機台編號 ='{dr["機台編號"]}'", dr);
                }
            }
        }
        public DataRow Set_NewStatus(DataRow dr, string radio_Select, string Progress)
        {
            string NowTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (dr != null)
            {
                //string radio_Select = RadioButtonList_select_type.SelectedValue;
                switch (radio_Select)
                {
                    case "0"://啟動
                        dr["狀態"] = "啟動";
                        if (dr["實際啟動時間"].ToString().ToUpper() == "NULL")
                            dr["實際啟動時間"] = NowTime;
                        dr["再次啟動時間"] = NowTime;
                        dr["進度"] = Progress.Trim('%');
                        break;
                    case "1"://暫停
                        dr["狀態"] = "暫停";
                        dr["暫停時間"] = NowTime;
                        dr["異常狀態號"] = SetErrorDataToDataTable(dr["工作站編號"].ToString(), dr[ShareMemory.PrimaryKey].ToString(), null);
                        dr["進度"] = Progress.Trim('%');
                        break;
                    case "2"://完成
                        dr["狀態"] = "完成";
                        dr["實際完成時間"] = NowTime;
                        dr["進度"] = "100";
                        break;
                }
            }
            return dr;
        }
        public string[] GetLineTotal(int infCount, ref int no_detail, ref int behind, string db = "Hor")
        {
            List<string> list = new List<string>();
            int count = 0;
            int nosolved = 0;
            string td = "";
            int totalTaget = 0;
            int totalTagetPerson = 0;
            int totalFinish = 0;
            int totalError = 0;
            int totalOnLine = 0;
            int td_totalFinish = 0;
            int td_totalError = 0;
            int td_totalOnLine = 0;

            string[] str = new string[2];
            DataTable dt_line = null;
            int alarm_count = 0;

            dt_line = clsdb.DataTable_GetTable($"select * from {ShareMemory.SQLAsm_WorkStation_Type} where 工作站是否使用中 = 1 or 工作站是否使用中 = 'True' ");
            if (HtmlUtil.Check_DataTable(dt_line))
            {
                DataTable dt_select = tableColumnSelectForTotalLine(dt_line); // 12s
                //Rows
                if (HtmlUtil.Check_DataTable(dt_select))
                {
                    for (int i = 0; i < dt_select.Rows.Count; i++)
                    {
                        int x = 0, y = 0;
                        //計算落後的
                        Calculation_Alarm_Or_Behind(DataTableUtils.toString(dt_select.Rows[i]["工作站編號"]), ref nosolved, ref count);//7S
                        td += "<tr class='gradeX'> \n";
                        for (int j = 0; j < dt_select.Columns.Count; j++)
                        {
                            if (dt_select.Columns[j].ColumnName != "工作站是否使用中" && dt_select.Columns[j].ColumnName != "工作站編號" && dt_select.Columns[j].ColumnName != "人數配置")
                            {
                                if (DataTableUtils.toInt(dt_select.Rows[i]["暫停"].ToString()) != 0)
                                {
                                    if (dt_select.Columns[j].ColumnName == "工作站名稱")
                                    {
                                        string url = $"LineNum={DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])},ReqType=Line";
                                        td += "<td style='text-align:center;'>" +
                                                  "<u>" +
                                                      $"<a onclick=jump_AsmLineOverView('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\">    " +
                                                          "<div style=\"height:100%;width:100%\">" +
                                                            DataTableUtils.toString(dt_select.Rows[i][j]) +
                                                          "</div>" +
                                                      "</a>" +
                                                  "</u>" +
                                              "</td> \n";
                                    }
                                    else if (dt_select.Columns[j].ColumnName == "暫停")
                                    {
                                        string url = $"LineNum={DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])},ReqType=Error";
                                        td += "<td style='text-align:center;background-color:lightcoral;color:white;font-size:20px;'>" +
                                                  "<u>" +
                                                      $"<a onclick=jump_AsmLineOverView('{WebUtils.UrlStringEncode(url)}') style='color:white;' href=\"javascript: void()\">  " +
                                                          "<div style=\"height:100%;width:100%\">" +
                                                              DataTableUtils.toString(dt_select.Rows[i][j]) +
                                                          "</div>" +
                                                      "</a>" +
                                                  "</u>" +
                                              "</td> \n";
                                    }
                                    else if (dt_select.Columns[j].ColumnName == "目標件數")
                                    {
                                    }
                                    else if (dt_select.Columns[j].ColumnName == "今日完成" || dt_select.Columns[j].ColumnName == "今日暫停" || dt_select.Columns[j].ColumnName == "今日生產中")
                                    {

                                    }
                                    else
                                        td += $"<td style='text-align:center;'>{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";
                                }
                                else if (dt_select.Columns[j].ColumnName == "今日完成" || dt_select.Columns[j].ColumnName == "今日暫停" || dt_select.Columns[j].ColumnName == "今日生產中")
                                {

                                }
                                else
                                {
                                    if (dt_select.Columns[j].ColumnName == "工作站名稱")//.ASPX?+parameter(Ex:LineNum=1)
                                    {
                                        string url = $"LineNum={DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])},ReqType=Line";
                                        td += "<td style='text-align:center;'>" +
                                                  "<u>" +
                                                      $"<a onclick=jump_AsmLineOverView('{WebUtils.UrlStringEncode(url)}') href=\"javascript: void()\" >" +
                                                          "  <div style=\"height:100%;width:100%\">" +
                                                             DataTableUtils.toString(dt_select.Rows[i][j]) +
                                                          "</div>" +
                                                      "</a>" +
                                                  "</u>" +
                                              "</td>\n";

                                    }
                                    else if (dt_select.Columns[j].ColumnName == "目標件數")
                                    {
                                    }
                                    else if (dt_select.Columns[j].ColumnName == "今日完成" || dt_select.Columns[j].ColumnName == "今日暫停" || dt_select.Columns[j].ColumnName == "今日生產中")
                                    {
                                    }
                                    else
                                        td += $"<td style='text-align:center;'>{DataTableUtils.toString(dt_select.Rows[i][j])}</td>\n";
                                }
                            }
                            else
                                continue;
                        }
                        //放今日落後用的
                        list = Calculation_Alarm_Or_Behind(DataTableUtils.toString(dt_select.Rows[i]["工作站編號"]), ref x, ref y, true);

                        td += $"<td style='text-align:center;'>{y}</td>\n";
                        //計算未解決的機台數量
                        alarm_count += list.Count;

                        // Count Piece
                        totalTaget += DataTableUtils.toInt(dt_select.Rows[i]["目標件數"].ToString());
                        totalTagetPerson += DataTableUtils.toInt(dt_select.Rows[i]["人數配置"].ToString());
                        totalFinish += DataTableUtils.toInt(dt_select.Rows[i]["完成"].ToString());
                        totalError += DataTableUtils.toInt(dt_select.Rows[i]["暫停"].ToString());
                        totalOnLine += DataTableUtils.toInt(dt_select.Rows[i]["生產中"].ToString());
                        td_totalFinish += DataTableUtils.toInt(dt_select.Rows[i]["今日完成"].ToString());
                        td_totalError += DataTableUtils.toInt(dt_select.Rows[i]["今日暫停"].ToString());
                        td_totalOnLine += DataTableUtils.toInt(dt_select.Rows[i]["今日生產中"].ToString());
                        td += "</tr> \n";
                    }
                    str[1] = td;
                    _cTotalTagetPiece = totalTaget.ToString();
                    _cTotalTagetPerson = totalTagetPerson.ToString();
                    _cTotalFinishPiece = totalFinish.ToString();
                    _cTotalErrorPiece = totalError.ToString();
                    _cTotalOnLinePiece = totalOnLine.ToString();
                    _td_cTotalFinishPiece = td_totalFinish.ToString();
                    _td_cTotalErrorPiece = td_totalError.ToString();
                    _td_cTotalOnLinePiece = td_totalOnLine.ToString();

                    no_detail = alarm_count;
                    behind = count;
                    return str;
                }
                else
                {
                    no_detail = alarm_count;
                    behind = count;
                    return str;
                }
            }
            else
            {
                no_detail = alarm_count;
                behind = count;
                str[0] = " no data";
                return str;
            }
        }
        //計算未解決跟落後用，回傳未解決的機台編號
        private List<string> Calculation_Alarm_Or_Behind(string LineNum, ref int alarm, ref int count, bool recover = false)
        {
            //LineNum→產線編號 alarm→未解決數量 count→落後數量 recover→要不要重新計算
            List<double> list = new List<double>();
            string Condition = "";
            List<string> alarm_num = new List<string>();
            string LineNumber = LineNum == "" ? "" : $" 工作站編號 = {LineNum} AND ";

            Condition = $"({LineNumber} 實際組裝時間 = {DateTime.Now:yyyyMMdd}) OR ({LineNumber}  實際組裝時間 <= {DateTime.Now:yyyyMMdd} AND 狀態 != '完成') OR ({LineNumber} substring(實際完成時間,1,8)  = {DateTime.Now:yyyyMMdd})";
            GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            DataTable dt = clsdb.DataTable_GetTable($"select * from {ShareMemory.SQLAsm_WorkStation_State} where {Condition}");
            //暫停
            if (HtmlUtil.Check_DataTable(dt))
            {
                if (recover)
                    alarm = 0;

                if (dt.Columns.IndexOf("預計完成日") == -1)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            check_case($" and 排程編號='{DataTableUtils.toString(row["排程編號"])}' ", GetConnByDekVisTmp, LineNum.ToString(), ref alarm, out _);
                        }
                        catch
                        {

                        }

                        try
                        {
                            string no_mean = "";
                            list = percent_calculation(DataTableUtils.toString(row["排程編號"]), DataTableUtils.toString(row["進度"]), ref no_mean, LineNum);
                            bool delay = false;
                            Comparison_Schedule(list[0], list[1], ref count, ref delay);
                        }
                        catch
                        {

                        }

                        if (alarm > 0 && alarm_num.IndexOf(DataTableUtils.toString(row["排程編號"])) == -1)
                            alarm_num.Add(DataTableUtils.toString(row["排程編號"]));
                        alarm = 0;
                    }
                }
                else
                {
                    string Line = "";

                    foreach (DataRow row in dt.Rows)
                        Line += Line == "" ? $" 排程編號='{DataTableUtils.toString(row["排程編號"])}' " : $" OR  排程編號='{DataTableUtils.toString(row["排程編號"])}' ";
                    Line = Line == "" ? "" : $" and ({Line})";
                    List<string> no_solved = new List<string>();
                    check_case(Line, GetConnByDekVisTmp, LineNum.ToString(), ref alarm, out no_solved);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMdd")) > DataTableUtils.toDouble(row["預計完成日"]))
                            count++;
                        if (alarm > 0 && no_solved.IndexOf(DataTableUtils.toString(row["排程編號"])) != -1 && alarm_num.IndexOf(DataTableUtils.toString(row["排程編號"])) == -1 && DataTableUtils.toString(row["維護"]) != "")
                            alarm_num.Add(DataTableUtils.toString(row["排程編號"]));
                    }
                }
            }
            return alarm_num;

        }
        /*整理完畢*/
        public string[] GetHistoryRowsData(string ErrorID, ListItem Line, string start = "", string end = "", List<string> Number = null, string LineNum = "", string db = "", string Datestart = "", string Dateend = "", string errortype = "")
        {
            string td = "";
            string[] str = new string[4];//0:un 1:html cmd
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            DataTable dt = new DataTable();
            DateTime time = new DateTime();
            if (Number == null && LineNum == "")
            {
                string Condition = GetHistorySearchCondition(ErrorID, Line.Value);
                string daterange = "";
                if (start != "" && end != "")
                    daterange = $" and 組裝日 >= {start.Replace("-", "")} and 組裝日 <= {end.Replace("-", "")} ";
                string Sql = $"select  * from {ShareMemory.SQLAsm_WorkStation_State} INNER JOIN 工作站型態資料表 ON 工作站狀態資料表.工作站編號 = 工作站型態資料表.工作站編號  where {Condition} {daterange} order by 組裝日 desc limit 500";
                dt = DataTableUtils.GetDataTable(Sql);
            }
            else if (Number != null && LineNum != "")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                string schdule_number = "";
                for (int i = 0; i < Number.Count - 1; i++)
                {
                    if (i == 0)
                        schdule_number += $" and (  排程編號 ='{Number[i]}' ";
                    else
                        schdule_number += $" OR  排程編號 ='{Number[i]}' ";
                }
                schdule_number = schdule_number + ")";
                string Sql = $"select  * from {ShareMemory.SQLAsm_WorkStation_State} INNER JOIN 工作站型態資料表 ON 工作站狀態資料表.工作站編號 = 工作站型態資料表.工作站編號  where 工作站狀態資料表.工作站編號='{LineNum}' {schdule_number}  order by 組裝日 desc";
                dt = DataTableUtils.GetDataTable(Sql);
            }
            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable dt_select = tableColumnSelectForHistorySearchList(dt);
                dt_select.Columns.Add("異常歷程");
                if (HtmlUtil.Check_DataTable(dt_select))
                {
                    for (int i = 0; i < dt_select.Rows.Count; i++)
                    {
                        td += "<tr class=\"gradeX\">";
                        for (int j = 0; j < dt_select.Columns.Count; j++)
                        {
                            if (dt_select.Columns[j].ColumnName == "排程編號")//維護人員姓名                              
                                td += $"<td style=\"text-align:center;\">{DataTableUtils.toString(dt_select.Rows[i]["排程編號"])}</td>";
                            else if (dt_select.Columns[j].ColumnName == "工作站名稱")//維護人員姓名   
                                td += $"<td style=\"text-align:center;\">{DataTableUtils.toString(dt_select.Rows[i]["工作站名稱"])}</td>";
                            else if (dt_select.Columns[j].ColumnName == "實際啟動時間")//維護人員姓名 
                            {
                                string date = dt_select.Rows[i]["實際啟動時間"].ToString() != "" ? $"{StrToDate(dt_select.Rows[i]["實際啟動時間"].ToString()):yyyy/MM/dd HH:mm:ss}" : "";
                                td += $"<td style=\"text-align:center;\">{date}</td>";
                            }
                            else if (dt_select.Columns[j].ColumnName == "實際完成時間")
                            {
                                string date = dt_select.Rows[i]["實際完成時間"].ToString() != "" ? $"{StrToDate(dt_select.Rows[i]["實際完成時間"].ToString()):yyyy/MM/dd HH:mm:ss}" : "";
                                td += $"<td style=\"text-align:center;\">{date}</td>";
                            }//維護人員姓名  
                            else if (dt_select.Columns[j].ColumnName == "異常歷程")
                            {
                                string date = "";
                                if (db == "")
                                    DataTableUtils.Conn_String = GetConnByDekVisTmp;
                                else
                                {
                                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                                    date = $" and '{Datestart}' <= substring(時間紀錄,1,8)  and substring(時間紀錄,1,8) <= '{Dateend}' and 結案判定類型 = '{errortype}' ";
                                }
                                string sqlcmd = $"select * from 工作站異常維護資料表 where (父編號 is null OR 父編號 = '0') and 排程編號 = '{ DataTableUtils.toString(dt_select.Rows[i]["排程編號"])}' and 工作站編號 = '{DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])}' ";
                                DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                                int count = 0;
                                if (HtmlUtil.Check_DataTable(ds))
                                {
                                    if (db != "")
                                    {
                                        foreach (DataRow data in ds.Rows)
                                        {
                                            sqlcmd = $"select * from 工作站異常維護資料表 where 父編號 = '{DataTableUtils.toString(data["異常維護編號"])}' {date} ";
                                            DataTable de = DataTableUtils.GetDataTable(sqlcmd);
                                            if (de != null)
                                                count += de.Rows.Count;
                                        }
                                    }
                                    else
                                        count = ds.Rows.Count;
                                }
                                string urlpre = $"ErrorID={DataTableUtils.toString(dt_select.Rows[i]["排程編號"])},ErrorLineNum={DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])},ErrorLineName={DataTableUtils.toString(dt_select.Rows[i]["工作站名稱"])},history=1";
                                if (db != "")
                                    urlpre = $"{urlpre},Date_str={Datestart},Date_end={Dateend},ErrorType={errortype},db={db}";

                                string url = WebUtils.UrlStringEncode(urlpre);
                                td += $"<td style=\"text-align:center;\"><a href=\"Asm_ErrorDetail.aspx?key={url}\"><div style=\"height:100%;width:100%\">{count}</div></a></td>";
                            }
                            else if (dt_select.Columns[j].ColumnName == "工作站編號")
                            {
                            }
                            else
                            {
                                DateTime starttime = StrToDate(dt_select.Rows[i]["實際啟動時間"].ToString());
                                DateTime endtime = StrToDate(dt_select.Rows[i]["實際完成時間"].ToString());
                                if (dt_select.Rows[i]["實際完成時間"].ToString() != "")
                                {
                                    TimeSpan span = endtime - starttime;
                                    td += $"<td style=\"text-align:center;\">{span.TotalMinutes:0}</td>";
                                }
                                else
                                    td += $"<td style=\"text-align:center;\">{0}</td>";

                            }
                        }
                        td += "</tr>";
                    }
                    str[1] = td;
                    return str;
                }
                else
                    return str;
            }
            else
            {
                str[0] = " no data";
                return str;
            }
        }
        public string[] GetMantRowsData(string acc, string ErrorID, string LineNum, ref string[] _errorTitleShow, ref string[] _dep, ref string[] _status, string f_type = "", string MantID = "", string startdate = "", string enddate = "", string errortype = "", string db = "")
        {
            string type = HtmlUtil.Search_acc_Column(acc, "Last_Model");
            string YN = HtmlUtil.Search_acc_Column(acc, "Can_Close");
            acc = HtmlUtil.Search_acc_Column(acc, "USER_NAME");

            string update = "";
            string delete = "";
            string td = "";
            string[] str = new string[6];//0:un 1:html cmd
            string strTime;
            string Condition = "";
            //------------------------------------20200424新增-----------------------------------

            if (f_type == "")
            {
                //if (type == "Hor")
                //{
                //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                //}
                //else
                //{
                //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                //}

                if (MantID != "")
                {
                    //先找該ID的父項目
                    string sql = $"select * from 工作站異常維護資料表 where 異常維護編號 = {MantID}";
                    DataTable dt_row = clsdb.GetDataTable(sql);

                    if (HtmlUtil.Check_DataTable(dt_row) && DataTableUtils.toString(dt_row.Rows[0]["父編號"]) != "" && DataTableUtils.toString(dt_row.Rows[0]["父編號"]) != "0")
                        MantID = $" and 異常維護編號={DataTableUtils.toString(dt_row.Rows[0]["父編號"])} ";
                    else
                        MantID = $" and 異常維護編號={MantID} ";
                }


                Condition = $"排程編號='{ErrorID}'  and (父編號 is null OR 父編號 = 0) {MantID}";
            }
            else
            {
                //if (type == "Hor")
                //{
                //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                //}
                //else
                //{
                //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                //}

                Condition = $" 排程編號='{ErrorID}'  and (父編號 is null OR 父編號 = 0) {MantID}";
            }

            //------------------------------------20200424新增-----------------------------------

            int ErrorTitleCount = 0;
            List<string> Errors = new List<string>();
            //if (type == "Hor")
            //{
            //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            //}
            //else
            //{
            //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
            //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
            //}
            DataTable dt = clsdb.DataTable_GetTable($"select * from {ShareMemory.SQLAsm_WorkStation_ErrorMant} where {Condition}");
            if (db != "")
            {
                List<string> schdule_list = new List<string>();
                foreach (DataRow datarow in dt.Rows)
                {
                    //if (type == "Hor")
                    //{
                    //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                    //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                    //}
                    //else
                    //{
                    //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                    //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                    //}
                    string sqlcmd = $"select * from 工作站異常維護資料表 where 父編號 = '{DataTableUtils.toString(datarow["異常維護編號"])}' and '{startdate}' <= 時間紀錄 and 時間紀錄 <= '{enddate}' and 結案判定類型 = '{errortype}' ";
                    DataTable de = clsdb.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(de))
                        schdule_list.Add(DataTableUtils.toString(datarow["異常維護編號"]));
                }

                for (int x = 0; x < schdule_list.Count; x++)
                {
                    DataRow[] rows = dt.Select($"異常維護編號='{schdule_list[x]}'");
                    for (int i = 0; i < rows.Length; i++)
                        rows[i].Delete();
                }
            }


            if (HtmlUtil.Check_DataTable(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Errors.Add("異常");
                    _errorTitleShow[0] = "異常";
                }
                DataTable dt_select = EtableColumnSelectForLineDetail(dt);
                string new_ID = "";
                //Rows
                if (HtmlUtil.Check_DataTable(dt_select))
                {
                    for (int i = 0; i < dt_select.Rows.Count; i++)
                    {
                        //if (type == "Hor")
                        //{
                        //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                        //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                        //}
                        //else
                        //{
                        //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                        //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                        //}

                        //得到子項目的datatable
                        string sqlcmd = $"select * from 工作站異常維護資料表 where 父編號 ={DataTableUtils.toString(dt_select.Rows[i]["異常維護編號"])} and 排程編號 = '{ErrorID}' order by 時間紀錄 asc";
                        DataTable ds = clsdb.GetDataTable(sqlcmd);
                        if (!string.IsNullOrEmpty(dt_select.Rows[i]["異常原因類型"].ToString()))
                        {
                            td += "<tr class='gradeX'> \n";

                            for (int j = 0; j < dt_select.Columns.Count; j++)
                            {
                                if (dt_select.Columns[j].ColumnName == "維護人員姓名")//維護人員姓名
                                {
                                    string part = "";
                                    if (DataTableUtils.toString(dt_select.Rows[i][j]) != "")
                                    {
                                        if (_dep[0] == "nodata")
                                            _dep[0] = DataTableUtils.toString(dt_select.Rows[i]["維護人員單位"]);
                                        part = DataTableUtils.toString(dt_select.Rows[i]["維護人員單位"]);
                                    }
                                    else
                                    {
                                        if (_dep[0] == "nodata")
                                            _dep[0] = "生產部";
                                        part = "生產部";
                                    }

                                    strTime = StrToDate(dt_select.Rows[i]["時間紀錄"].ToString()).ToString("yyyy/MM/dd tt hh:mm:ss");

                                    td += $"<td style='text-align:center;width:15%'>" +
                                                $"<span style='font-size:8px ;text-align:right;color:red;'> " +
                                                    $"<b>{strTime}</b>" +
                                                "</span>" +
                                                "<br>" +
                                                    $"[{part}]{DataTableUtils.toString(dt_select.Rows[i][j])}" +
                                                "<br>" +
                                           "</td> \n";
                                }
                                else if (dt_select.Columns[j].ColumnName == "時間紀錄")
                                {
                                }
                                else if (dt_select.Columns[j].ColumnName == "異常原因類型")
                                {
                                }
                                else if (dt_select.Columns[j].ColumnName == "維護人員單位")
                                {
                                }
                                else if (dt_select.Columns[j].ColumnName == "處理狀態")
                                {
                                    _status[0] = DataTableUtils.toString(dt_select.Rows[i][j]);
                                }
                                else if (dt_select.Columns[j].ColumnName == "異常維護編號")
                                {
                                }
                                else if (dt_select.Columns[j].ColumnName == "圖片檔名")
                                {
                                }
                                else if (dt_select.Columns[j].ColumnName == "結案判定類型")
                                {
                                    //if (type == "Hor")
                                    //{
                                    //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                                    //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                                    //}
                                    //else
                                    //{
                                    //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                                    //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                                    //}
                                    string sql = $"select * from 工作站異常維護資料表 where 父編號 = '{dt_select.Rows[i]["異常維護編號"]}' order by 時間紀錄 desc";
                                    DataTable dt_row = clsdb.GetDataTable(sql);

                                    if (HtmlUtil.Check_DataTable(dt_row))
                                    {
                                        string responedep = DataTableUtils.toString(dt_row.Rows[0]["責任單位"]) == "" ? "未填寫" : DataTableUtils.toString(dt_row.Rows[0]["責任單位"]);


                                        if (DataTableUtils.toString(dt_row.Rows[0]["結案判定類型"]) != "")
                                            td += "<td style='text-align:center;min-width:45px;max-width:25%;'>" +
                                                    "<span style='color:green'>" +
                                                        "結案" +
                                                    "</span> " +
                                                    $"<br>{DataTableUtils.toString(dt_row.Rows[0]["結案判定類型"])}<br>" +
                                                     "<span style='color:blue'>" +
                                                        $"[責任單位] <br /> {responedep}" +
                                                    "</span>" +
                                                  "</td> \n";
                                        else
                                            td += "<td style='text-align:center;min-width:45px;max-width:25%;'>" +
                                                    "<span style='color:red'>" +
                                                        "處理中" +
                                                    "</span> <br />" +
                                                    "<span style='color:blue'>" +
                                                        $"[責任單位] <br /> {responedep}" +
                                                    "</span>" +
                                                  "</td> \n";
                                    }
                                    else
                                    {
                                        string responedep = DataTableUtils.toString(dt_select.Rows[i]["責任單位"]) == "" ? "未填寫" : DataTableUtils.toString(dt_select.Rows[i]["責任單位"]);

                                        td += "<td style='text-align:center;min-width:45px;max-width:25%;'>" +
                                                "<span style='color:red'>" +
                                                    "處理中" +
                                                "</span> <br />" +
                                                 "<span style='color:blue'>" +
                                                        $"[責任單位] <br /> {responedep}" +
                                                    "</span>" +
                                              "</td> \n";
                                    }

                                }
                                else if (dt_select.Columns[j].ColumnName == "是否持續顯示異常")
                                {

                                }
                                else if (dt_select.Columns[j].ColumnName == "責任單位")
                                {

                                }
                                //------------------------------------20200424新增-----------------------------------
                                else if (dt_select.Columns[j].ColumnName == "維護內容")
                                {
                                    string recovery = "";
                                    if (HtmlUtil.Check_DataTable(ds))
                                        recovery = DataTableUtils.toString(ds.Rows[ds.Rows.Count - 1]["結案判定類型"]);
                                    string file = "";
                                    string Message = "";
                                    string Message_Open = "";
                                    //先確認是否含有子項目->有，前面加上ICON
                                    if (HtmlUtil.Check_DataTable(ds))
                                    {
                                        if (MantID != "")
                                            new_ID = " in ";
                                        else
                                            new_ID = "";

                                        //子項目的內容
                                        for (int a = 0; a < ds.Rows.Count; a++)
                                        {
                                            strTime = StrToDate(ds.Rows[a]["時間紀錄"].ToString()).ToString("yyyy/MM/dd tt hh:mm:ss");
                                            //子項目附檔
                                            string videos = "";
                                            file = "";
                                            file = Return_fileurl(DataTableUtils.toString(ds.Rows[a]["圖片檔名"]), out videos);
                                            //回覆人員 訊息 時間
                                            string End_Report = "";
                                            string close_file = "";
                                            string close_video = "";
                                            //if (type == "Hor")
                                            //{
                                            //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                                            //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                                            //}
                                            //else
                                            //{
                                            //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                                            //    GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                                            //}
                                            string sql = $"select * from 工作站異常維護資料表 where 父編號 = '{dt_select.Rows[i]["異常維護編號"]}' order by 時間紀錄 desc";
                                            DataTable dt_row = clsdb.GetDataTable(sql);

                                            if (HtmlUtil.Check_DataTable(dt_row))
                                                close_file = Return_fileurl(DataTableUtils.toString(dt_row.Rows[0]["結案附檔"]), out close_video);

                                            if (a == ds.Rows.Count - 1 && recovery != "")
                                                End_Report = $"<div class=\"_EndDescripfa fa-chevron-circle-downtion\">" +
                                                                $"<span style='color:green'>" +
                                                                    $"[結案說明]{DataTableUtils.toString(ds.Rows[a]["維護人員姓名"])}" +
                                                                $"</span>" +
                                                                $":{br_text(DataTableUtils.toString(ds.Rows[a]["結案內容"]))}<br>{close_file}{close_video}" +
                                                             $" </div>";
                                            delete = "";
                                            update = "";
                                            if (YN == "Y" || acc == DataTableUtils.toString(ds.Rows[a]["維護人員姓名"]))
                                            {
                                                update = $" <div>" +
                                                             $"<a href='javascript:void(0)' onclick=updates('up_{DataTableUtils.toString(ds.Rows[a]["異常維護編號"])}','{ErrorID}','{DataTableUtils.toString(ds.Rows[a]["維護內容"]).Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}','{ds.Rows[a]["異常原因類型"]}','{ds.Rows[a]["圖片檔名"].ToString().Replace("\n", "^")}','{ds.Rows[a]["結案判定類型"]}','{ds.Rows[a]["結案內容"].ToString().Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}','{ds.Rows[a]["結案附檔"].ToString().Replace("\n", "^")}','1','{DataTableUtils.toString(ds.Rows[a]["責任單位"])}','{DataTableUtils.toString(ds.Rows[a]["是否持續顯示異常"])}') data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                                 $"<u>" +
                                                                    $"編輯" +
                                                                 $"</u>" +
                                                             $"</a>" +
                                                         $"</div>";
                                                delete = $"<a href='javascript:void(0)' onclick=deletes('{DataTableUtils.toString(ds.Rows[a]["異常維護編號"])}') >" +
                                                            $"<u>" +
                                                                $"刪除" +
                                                            $"</u>" +
                                                         $"</a>";
                                            }
                                            else
                                            {
                                                update = "<div>編輯</div>";
                                                delete = "刪除";
                                            }
                                            Message += $"<div class=\"_EndDescription\">" +
                                                           $"<span style='color:blue'>" +
                                                                $"[{DataTableUtils.toString(ds.Rows[a]["維護人員單位"])}] {DataTableUtils.toString(ds.Rows[a]["維護人員姓名"])}" +
                                                           $"</span>" +
                                                           $":{br_text(DataTableUtils.toString(ds.Rows[a]["維護內容"]))} {file}" +
                                                       $"</div>{videos}" +
                                                       $"<div id=\"_Response\" class=\"col-12 col-xs-12 text-right\" style=\"margin-right:-6px;\">" +
                                                           $"<h6 style=\"text-align:right;\">" +
                                                                $"{strTime}" +
                                                           $"</h6>" +
                                                           $"<div class='_update'>" +
                                                                $"{update}" +
                                                           $"</div>" +
                                                           $"<div class='_delete'>" +
                                                                $"{delete}" +
                                                           $"</div><hr class=\"_hr\"/>" +
                                                       $"</div>{End_Report}";
                                        }

                                        string icon = "fa fa-folder-o";
                                        if (MantID != "")
                                            icon = "fa fa-folder-open-o";

                                        Message_Open = "<div class=\"col-md-1 col-xs-12\" style=\"width: 3%; margin:0px 10px 0px 0px \">" +
                                                            "<u>" +
                                                                $" <a data-toggle=\"collapse\" data-parent=\"#accordion\"  href=\"#collapse{dt_select.Rows[i]["異常維護編號"]}\" >" +
                                                                    $" <i id=\"Open{dt_select.Rows[i]["異常維護編號"]}\" onclick=Click_Num('Open{dt_select.Rows[i]["異常維護編號"]}') class='{icon}'  style='color:black;width:3%;font-size: 1.6em;'>" +
                                                                     "</i>" +
                                                                 "</a>" +
                                                            "</u>" +
                                                        "</div>";
                                    }

                                    //父項目的檔案
                                    string video = "";
                                    file = "";
                                    file = Return_fileurl(DataTableUtils.toString(dt_select.Rows[i]["圖片檔名"]), out video);
                                    //顯示異常類型
                                    string error = HtmlUtil.Check_DataTable(ds) ? DataTableUtils.toString(ds.Rows[ds.Rows.Count - 1]["異常原因類型"]) : DataTableUtils.toString(dt_select.Rows[i]["異常原因類型"]);

                                    //顯示責任單位
                                    string responedep = HtmlUtil.Check_DataTable(ds) ? DataTableUtils.toString(ds.Rows[ds.Rows.Count - 1]["責任單位"]) : DataTableUtils.toString(dt_select.Rows[i]["責任單位"]);

                                    //顯示是否持續燈
                                    string show_errorlight = HtmlUtil.Check_DataTable(ds) ? DataTableUtils.toString(ds.Rows[ds.Rows.Count - 1]["是否持續顯示異常"]) : DataTableUtils.toString(dt_select.Rows[i]["是否持續顯示異常"]);

                                    if (recovery == "")
                                        recovery = $"<div>" +
                                                     $"<a href='javascript:void(0)' onclick=Get_ErrorDetails('{DataTableUtils.toString(dt_select.Rows[i]["異常維護編號"])}','{ErrorID}','{DataTableUtils.toString(dt_select.Rows[i]["維護人員姓名"])}','{acc}','{YN}','{error}','{responedep}','{show_errorlight}') data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                         $"<u>" +
                                                            $"回覆" +
                                                         $"</u>" +
                                                     $"</a>" +
                                                 $"</div>";
                                    else
                                        recovery = $"<div>" +
                                                     $"<a href='javascript:void(0)' onclick=alert('該回文已結案')>" +
                                                         $"<u>" +
                                                            $"回覆" +
                                                         $"</u>" +
                                                     $"</a>" +
                                                 $"</div>";

                                    update = "";
                                    delete = "";
                                    if (YN == "Y" || acc == DataTableUtils.toString(dt_select.Rows[i]["維護人員姓名"]))
                                    {
                                        update = $"<div>" +
                                                     $"<a href='javascript:void(0)' onclick=updates('up_{DataTableUtils.toString(dt_select.Rows[i]["異常維護編號"])}','{ErrorID}','{DataTableUtils.toString(dt_select.Rows[i]["維護內容"]).Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}','{DataTableUtils.toString(dt_select.Rows[i]["異常原因類型"])}','{DataTableUtils.toString(dt_select.Rows[i]["圖片檔名"]).Replace("\n", "^")}','','','','0','{responedep}','{show_errorlight}') data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                         $"<u>" +
                                                            $"編輯" +
                                                         $"</u>" +
                                                     $"</a>" +
                                                 $"</div>";
                                        delete = $"<a href='javascript:void(0)' onclick=deletes('{DataTableUtils.toString(dt_select.Rows[i]["異常維護編號"])}') >" +
                                                     $"<u>" +
                                                        $"刪除" +
                                                     $"</u>" +
                                                 $"</a>";
                                    }
                                    else
                                    {
                                        update = "<div>編輯</div>";
                                        delete = "刪除";
                                    }



                                    //父項目內容(子項目收縮在裡面)
                                    td += $"<td style='text-align:left;max-width:55%'>{Message_Open} {br_text(DataTableUtils.toString(dt_select.Rows[i][j]).Replace(';', '\n'))} {file}  {video} " +
                                            $"<div id=\"_Middle\" class=\"col-md-12 col-xs-12 text-right\" style=\"height:30px;\">" +
                                                //回復
                                                $"<div class=\"_status\">{recovery}</div>" +
                                                //編輯
                                                $"<div class=\"_update\">{update}</div>" +
                                                //刪除
                                                $"<div class=\"_delete\" style=\"margin:0px 0px 0px 3px\">{delete}</div>" +
                                            $"</div>" +
                                            $"<div id=\"collapse{dt_select.Rows[i]["異常維護編號"]}\" class=\"panel-collapse collapse {new_ID} \">" +
                                                $"<div class=\"panel-body\"> {Message}" +
                                                $"</div>" +
                                            $"</div>" +
                                     $"</td> \n";
                                }
                                else
                                    td += $"<td style='text-align:left;max-width:60%'>{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";

                            }
                            td += "</tr> \n";
                        }
                    }
                    str[0] = td;
                    td = "";
                    return str;
                }
                else
                    return str;
            }
            else
            {
                str[0] = "";
                return str;
            }
            //------------------------------------20200424新增-----------------------------------
            //-------------------------------------20200505-------------------------------------
        }
        //把文字分行
        public string br_text(string word)
        {
            string back = "";
            if (word.Trim() == "")
                return "無內容";
            else
            {
                List<string> list = new List<string>(word.Split('\n'));
                for (int i = 0; i < list.Count; i++)
                    back += list[i] + " <br/> ";

                return back;
            }
        }
        //pdf，excel，ppt，word用文字方式呈現
        public string Return_fileurl(string file_name, out string image_mp4, string height = "248")
        {
            string file = "";
            string Return_imge = "";
            if (file_name == "")
            {
                image_mp4 = Return_imge;
                return "";
            }
            else
            {
                string[] name_list = file_name.Split('\n');
                if (name_list.Length == 0)
                {
                    image_mp4 = Return_imge;
                    return "";
                }
                else
                {
                    for (int x = 0; x < name_list.Length - 1; x++)
                    {
                        int num = x + 1;
                        string[] sp = name_list[x].Split('.');
                        if (sp[sp.Length - 1].ToLower() == "xls" || sp[sp.Length - 1].ToLower() == "xlsx")
                            file += $"<u><a href = '{name_list[x]}'  href=\"javascript: void()\" >EXCEL表{num}</a></u>";
                        else if (sp[sp.Length - 1].ToLower() == "pdf")
                            file += $"<u><a href = '{name_list[x]}'  href=\"javascript: void()\" target='_blank' >PDF檔{num}</a></u>";
                        else if (sp[sp.Length - 1].ToLower() == "mp4")//判斷是影片或是圖片
                            Return_imge += Return_file(name_list[x], height, "mp4");
                        else
                            Return_imge += Return_file(name_list[x], height);
                    }
                    if (Return_imge != "")
                        image_mp4 = $"<div class=\"col-md-12\">{Return_imge}</div>";
                    else
                        image_mp4 = "";

                    if (file != "")
                        return $"({file})";
                    else
                        return file;
                }
            }
        }
        //回傳圖片跟影片的html碼
        public string Return_file(string file, string height, string type = "")
        {
            if (type.ToLower() == "mp4")
                file = $"<a onclick=show_image('{file}','video')  data-toggle=\"modal\" data-target=\"#exampleModal_Image\"  href=\"javascript: void()\"><video  width=\"97%\" height=\"{height}px\" src={file} controls=\"\"></video></a>";
            else
                file = $"<a onclick=show_image('{file}','image') data-toggle=\"modal\" data-target=\"#exampleModal_Image\"  href=\"javascript: void()\"><img src={file} alt=\"...\" width=\"97%\" height=\"{height}px\"></a>";
            return $"<div class=\"col-md-4 col-xs-4 gradeX_Img\" style=\"margin-bottom:8px;padding:0\">{file}</div>";

        }
        //取得圓盤產線的狀態
        public string Test_calculation_finish(string date, string worktime)
        {
            if (date != "")
                date = date + "080000";

            int standard_worktime = DataTableUtils.toInt(worktime);
            work.工作時段_新增(8, 0, 12, 0);
            work.工作時段_新增(13, 0, 17, 0);
            if (date != "")
            {
                DateTime stand_endtime = work.目標日期(StrToDate(date), new TimeSpan(0, 0, standard_worktime));
                return stand_endtime.ToString("yyyyMMdd");
            }
            else
                return "";
        }
        //計算完預計完成時間+狀態、進度、預計完工日(Ver.2用)
        public DataTable Get_dekInformation(DataTable dt, string start, string end, int type)
        {
            //先取得工時
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
            string sqlcmd = "select 機種編號,組裝時間 from 立式工藝";
            DataTable dt_工時 = DataTableUtils.GetDataTable(sqlcmd);
            //取得進度 狀態 完成時間
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
            sqlcmd = "select 排程編號,進度,狀態,實際完成時間,組裝日 from 工作站狀態資料表 where 工作站編號 = 11";
            DataTable dt_組裝資訊 = DataTableUtils.GetDataTable(sqlcmd);

            DataRow[] rows;
            if (!HtmlUtil.Check_DataTable(dt))
                return null;
            else
            {
                dt.Columns.Add("預計完工日", typeof(string));
                dt.Columns.Add("進度", typeof(int));
                dt.Columns.Add("狀態", typeof(string));
                dt.Columns.Add("實際完成時間", typeof(string));
                dt.Columns.Add("組裝日", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    //加入預計完工日
                    sqlcmd = $"機種編號='{DataTableUtils.toString(row["客戶機型"])}'";
                    if (HtmlUtil.Check_DataTable(dt_工時))
                    {
                        rows = dt_工時.Select(sqlcmd);
                        if (rows.Length > 0)
                            row["預計完工日"] = Test_calculation_finish(DataTableUtils.toString(row["預計開工日"]), DataTableUtils.toString(rows[0]["組裝時間"]));
                        else
                            row["預計完工日"] = "開發機";
                    }
                    else
                        row["預計完工日"] = "開發機";

                    //加入進度、狀態、實際完成時間
                    sqlcmd = $"排程編號='{DataTableUtils.toString(row["排程編號"])}'";
                    if (HtmlUtil.Check_DataTable(dt_組裝資訊))
                    {
                        rows = dt_組裝資訊.Select(sqlcmd);
                        if (rows.Length > 0)
                        {
                            row["進度"] = DataTableUtils.toInt(DataTableUtils.toString(rows[0]["進度"]));
                            row["狀態"] = DataTableUtils.toString(rows[0]["狀態"]);
                            row["實際完成時間"] = DataTableUtils.toString(rows[0]["實際完成時間"]);
                            row["組裝日"] = DataTableUtils.toString(rows[0]["組裝日"]);
                        }
                    }
                }
                //只留下本月應做的
                if (type == 1)
                    sqlcmd = $"預計完工日<{start} OR 預計完工日>{end}";
                //留下上個月應做完，但是在這個月做完
                else if (type == 2)
                    sqlcmd = $"(預計完工日>='{start}' and 預計完工日 <> '開發機') OR 實際完成時間<'{start}000000'";
                //到目前完止皆未完成
                else if (type == 3)
                    sqlcmd = $"(預計完工日>='{start}' and 預計完工日 <> '開發機')  OR 狀態='完成'";

                rows = dt.Select(sqlcmd);
                for (int i = 0; i < rows.Length; i++)
                    dt.Rows.Remove(rows[i]);

                //除去非必要欄位
                DataView dv = new DataView(dt);
                dv.Sort = "預計完工日 asc";
                dt = dv.ToTable(true, "客戶簡稱", "產線代號", "排程編號", "預計開工日", "預計完工日", "進度", "狀態", "實際完成時間", "組裝日", "訂單號碼", "客戶訂單", "品號", "訂單日期");
                return dt;
            }
        }
        //有給預計完工日用
        public DataTable Get_Imformation(DataTable dt, string start, string end, int type)
        {
            dt.Columns.Add("進度");
            dt.Columns.Add("狀態");
            dt.Columns.Add("實際完成時間");
            dt.Columns.Add("組裝日");

            string condition = "";
            bool ok = false;
            foreach (DataRow row in dt.Rows)
            {
                if (!ok)
                    condition = $" 排程編號 = '{row["排程編號"]}' ";
                else
                    condition += $" OR 排程編號 = '{row["排程編號"]}' ";
                ok = true;
            }
            condition = $" where ( {condition} )";


            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $"select 排程編號,進度,狀態,實際完成時間,組裝日 from 工作站狀態資料表 {condition}";
            DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

            foreach (DataRow row in dt.Rows)
            {
                sqlcmd = $"排程編號 = '{row["排程編號"]}'";
                DataRow[] rew = ds.Select(sqlcmd);
                if (rew != null && rew.Length > 0)
                {
                    row["進度"] = rew[0]["進度"];
                    row["狀態"] = rew[0]["狀態"];
                    row["實際完成時間"] = rew[0]["實際完成時間"];
                    row["組裝日"] = rew[0]["組裝日"];
                }
            }

            DataTable dt_Return = new DataTable();
            dt_Return = dt.Clone();

            //留下本月應做的
            if (type == 1)
                sqlcmd = $" 預計完工日>='{start}' and 預計完工日<='{end}' ";
            //留下上個月沒做完，但在這個月做完
            else if (type == 2)
                sqlcmd = $" 預計完工日<'{start}' and 實際完成時間>='{start}000000' ";
            //到目前完止皆未完成
            else if (type == 3)
                sqlcmd = $"預計完工日<'{start}' and 實際完成時間 is null  and (狀態 <> '完成' OR 狀態 is null)";


            DataRow[] rsw = dt.Select(sqlcmd);
            List<string> schdule_list = new List<string>();
            if (rsw != null && rsw.Length > 0)
            {
                for (int i = 0; i < rsw.Length; i++)
                {
                    if (schdule_list.IndexOf(DataTableUtils.toString(rsw[i]["排程編號"])) == -1)
                    {
                        dt_Return.ImportRow(rsw[i]);
                        schdule_list.Add(DataTableUtils.toString(rsw[i]["排程編號"]));
                    }
                }
            }
            return dt_Return;
        }
        public string[] GetTagetPiece(int LineNum)
        {
            string[] str = new string[3];//0:piece 1:LineName
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            string StrCmd = $"工作站編號={LineNum}";
            DataRow dr_taget = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLAsm_WorkStation_Type, StrCmd);


            if (dr_taget != null)
            {
                str[0] = dr_taget["目標件數"].ToString();
                str[1] = dr_taget["工作站名稱"].ToString();
                str[2] = dr_taget["人數配置"].ToString();
                return str;
            }
            else
            {
                str[0] = "Null";
                return str;
            }
        }

        public string[] GetOverViewData(int LineNum, string LineName, string Acc, string _reqType, ref int total, ref int alarm_total, string judge = "", string schdule = "")
        {
            string td = "";
            string[] str = new string[7] { "0", "0", "0", "0", "0", "0", "0" };//0:all   1:finsh   2:Stop  3:all   4:td_finish   5:td_Stop  7:Data
            string[] ErrorStr;
            string color = "";
            List<string> td_list = new List<string>();
            string SubError = "";
            string Condition = "";
            string Condition1 = "";
            string Condition2 = "";
            string Condition3 = "";
            string Condition4 = "";
            string PredictionTimeStatus = "";
            List<double> percent = new List<double>();
            int nosolved = 0;
            List<string> list = new List<string>();
            List<string> alarm_list = new List<string>();
            int count = 0;
            int PredictionProgress = 0;
            if (judge == "Hor")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            }
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
            }
            if (_reqType == "Line")
            {
                Condition1 = $"工作站編號 = {LineNum} AND 實際組裝時間 ={DateTime.Now:yyyyMMdd} ";
                Condition2 = $"工作站編號 = {LineNum} AND 實際組裝時間 <={DateTime.Now:yyyyMMdd} AND 狀態!='完成' ";
                Condition3 = $"工作站編號 = {LineNum} AND substring(實際完成時間,1,8)  = {DateTime.Now:yyyyMMdd} ";
                Condition4 = schdule == "" ? "" : $" and 排程編號 = '{schdule}' ";
                Condition = $" (({Condition1}) OR ({Condition2}) OR ({Condition3})) {Condition4} order by 組裝日,組裝編號 ";
            }
            else
            {
                Condition1 = $"工作站編號 = {LineNum} AND 實際組裝時間 ={DateTime.Now:yyyyMMdd} AND 狀態='暫停' ";
                Condition2 = $"工作站編號 = {LineNum} AND 實際組裝時間 <={DateTime.Now:yyyyMMdd} AND 狀態='暫停' ";
                Condition3 = $"工作站編號 = {LineNum} AND substring(實際完成時間,1,8)  = {DateTime.Now:yyyyMMdd}";
                Condition = $" ({Condition1}) OR ({Condition2}) order by 組裝日,組裝編號 ";
            }
            DataTable dt = null;

            int x = 0, y = 0;
            alarm_list = Calculation_Alarm_Or_Behind(LineNum.ToString(), ref x, ref y, true);

            if (_reqType == "Line")
                dt = DataTableUtils.DataTable_GetTable(ShareMemory.SQLAsm_WorkStation_State, Condition);
            else
            {
                DataTableUtils.Conn_String = GetConnByDekVisTmp;
                for (int a = 0; a < alarm_list.Count; a++)
                {
                    if (a == 0)
                        Condition = $"工作站編號 = {LineNum} and 狀態 <> '完成' and (排程編號 = '{alarm_list[a]}'";
                    else
                        Condition += $" OR 排程編號 = '{alarm_list[a]}'";
                }
                Condition += ")";
                dt = DataTableUtils.DataTable_GetTable(ShareMemory.SQLAsm_WorkStation_State, Condition);
            }

            if (HtmlUtil.Check_DataTable(dt))
            {

                DataTable dt_select = tableColumnSelectForLineDetail(dt, LineNum.ToString());
                str = GetEachPiece(dt);

                if (HtmlUtil.Check_DataTable(dt_select))
                {

                    for (int i = 0; i < dt_select.Rows.Count; i++)
                    {
                        td += "<tr class='gradeX'> \n";
                        for (int j = 0; j < dt_select.Columns.Count; j++)
                        {
                            if (dt_select.Columns[j].ColumnName == "狀態")
                            {
                                td_list.Clear();
                                if (dt.Rows[i]["狀態"].ToString() == "暫停")
                                {
                                    td_list.Add("red");
                                    td_list.Add(dt.Rows[i]["進度"].ToString());
                                }
                                else if (dt.Rows[i]["狀態"].ToString() == "完成")
                                {
                                    td_list.Add("green");
                                    td_list.Add(dt.Rows[i]["進度"].ToString());
                                }
                                else if (dt.Rows[i]["狀態"].ToString() == "啟動")
                                {
                                    td_list.Add("blue");
                                    td_list.Add(dt.Rows[i]["進度"].ToString());
                                }
                                else
                                {
                                    td_list.Add("black");
                                    td_list.Add("0");
                                }

                                td += $"<td style='text-align:center;width:10%;vertical-align: middle'>" +
                                          $"<a href='javascript:void(0)' onclick=SetValue('{dt.Rows[i]["排程編號"]}','{dt.Rows[i]["進度"]}%','{dt.Rows[i]["問題回報"]}','{dt.Rows[i]["狀態"].ToString()}') data-toggle='modal' data-target='#exampleModal'> " +
                                             $"<span style=color:{td_list[0]}>" +
                                                 $"<u>" +
                                                     $"{td_list[1]}%" +
                                                 $"</u>" +
                                             $"</span>" +
                                         $"</a>" +
                                       $"</td> \n";


                                color = "";

                                if (dt_select.Columns.IndexOf("預計完成日") == -1)
                                {
                                    //預定進度
                                    try
                                    {
                                        if (DataTableUtils.toDouble(DataTableUtils.toString(dt_select.Rows[i]["組裝日"])) < DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMdd")) && DataTableUtils.toString(dt_select.Rows[i]["實際啟動時間"]) == "" && DataTableUtils.toString(dt_select.Rows[i]["異常"]) == "")
                                            color = "red";

                                        string no_mean = "";
                                        percent = percent_calculation(DataTableUtils.toString(dt_select.Rows[i]["排程編號"]), DataTableUtils.toString(dt_select.Rows[i]["進度"]), ref no_mean, LineNum.ToString());

                                        bool delay = false;
                                        string target = Comparison_Schedule(percent[0], percent[1], ref count, ref delay);
                                        if (delay)
                                            color = "red";
                                        td += $"<td style='text-align:center;width:10%;vertical-align: middle'><b style='color:{color}'>{target}</b> </td> \n";
                                    }
                                    catch
                                    {
                                        if (DataTableUtils.toDouble(DataTableUtils.toString(dt_select.Rows[i]["組裝日"])) < DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMdd")) && DataTableUtils.toString(dt_select.Rows[i]["實際啟動時間"]) == "" && DataTableUtils.toString(dt_select.Rows[i]["異常"]) == "")
                                            color = "red";

                                        td += $"<td style='text-align:center;width:10%;vertical-align: middle' ><b style='color:{color}'>0%</b> </td> \n";
                                    }
                                    td += "<td></td>";
                                }
                                else
                                    td += "<td></td>";


                            }
                            else if (dt_select.Columns[j].ColumnName == "預計完成日")
                            {
                                if (DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMdd")) <= DataTableUtils.toDouble(dt_select.Rows[i][j].ToString()))
                                    td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{HtmlUtil.changetimeformat(dt_select.Rows[i][j].ToString())}</td> \n";
                                else
                                {
                                    td += $"<td style='text-align:center;width:10%;vertical-align: middle;color:red'>{HtmlUtil.changetimeformat(dt_select.Rows[i][j].ToString())}</td> \n";
                                    count++;
                                }

                            }
                            else if (dt_select.Columns[j].ColumnName == "進度")
                            {

                            }
                            else if (dt_select.Columns[j].ColumnName == "問題回報")
                            {

                            }
                            else if (dt_select.Columns[j].ColumnName == "實際啟動時間")
                            {
                            }
                            else if (dt_select.Columns[j].ColumnName == "組裝日")
                                td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{DataTableUtils.toString(dt_select.Rows[i][j].ToString().Substring(4, 2))}/{DataTableUtils.toString(dt_select.Rows[i][j].ToString().Substring(6, 2))}</td> \n";
                            else if (dt_select.Columns[j].ColumnName == "異常")//<a href="#">
                            {
                                if (dt_select.Rows[i][j].ToString() != "") //  
                                {
                                    SubError = "";
                                    ErrorStr = DataTableUtils.toString(dt_select.Rows[i][j]).Split(',');
                                    for (int k = 1; k < ErrorStr.Length - 2; k++)
                                        SubError += " " + ErrorStr[k];
                                    list = check_case($" and 排程編號 = '{DataTableUtils.toString(dt_select.Rows[i]["排程編號"])}' ", GetConnByDekVisTmp, LineNum.ToString(), ref nosolved, out _);
                                    // content
                                    string url = $"ErrorID={DataTableUtils.toString(dt_select.Rows[i]["排程編號"])},ErrorLineNum={ dt.Rows[0]["工作站編號"]},ErrorLineName={LineName}";
                                    if (ConfigurationManager.AppSettings["show_function"] == "1")
                                        td += $"<td style='text-align:center;width:48%;vertical-align: middle'>" +
                                            $"<a onclick=jump_Asm_ErrorDetail('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\">" +
                                            $"<div  style='height:100%;width:100%;font-size:20px;vertical-align: middle'>{list[0]} \t " +
                                            $"<u> [{list[1]}] </u>" +
                                            "</div>" +
                                            "</a>" +
                                            "</td> \n";
                                    else
                                        td += "<td style='text-align:center;width:48%'><a onclick=jump_Asm_ErrorDetail('" + WebUtils.UrlStringEncode(url) + "')  href=\"javascript: void()\"><div style='height:100%;width:100%'>異常歷程</div></a></td> \n";

                                }
                                else
                                {
                                    list = check_case($" and 排程編號 = '{DataTableUtils.toString(dt_select.Rows[i]["排程編號"])}' ", GetConnByDekVisTmp, LineNum.ToString(), ref nosolved, out _);
                                    string url = $"ErrorID={DataTableUtils.toString(dt_select.Rows[i]["排程編號"])},ErrorLineNum={dt.Rows[0]["工作站編號"]},ErrorLineName={LineName}";
                                    if (ConfigurationManager.AppSettings["show_function"] == "1")
                                        td += $"<td style='text-align:center;width:48%;vertical-align: middle'><a onclick=jump_Asm_ErrorDetail('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\"><div style='height:100%;width:100%;font-size:20px;vertical-align: middle'>{list[0]} \t <u> [{list[1]}]</u></div></a></td> \n";
                                    else
                                        td += $"<td style='text-align:center;width:48%'><u><a onclick=jump_Asm_ErrorDetail('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\"><div style='height:100%;width:100%'>" + "編輯" + "</div></a></u></td> \n";
                                }
                            }
                            else if (dt_select.Columns[j].ColumnName == "排程編號")
                            {
                                /*20200221修改這裡*/
                                //圖片
                                string image = "";
                                if (image != "")
                                    image = $"<img src='{image}' alt='...' width='200px' height='200px'>";

                                //啟動時間
                                string start_time = "";
                                if (DataTableUtils.toString(dt_select.Rows[i]["實際啟動時間"]) != "")
                                    start_time = $"實際啟動時間： <br/> {StrToDate(DataTableUtils.toString(dt_select.Rows[i]["實際啟動時間"]))} <br/> ";

                                //預計完成時間
                                string finish_time = "";
                                //percent_calculation(dt_select.Rows[i]["排程編號"].ToString(), "0", ref finish_time, LineNum.ToString(), true);
                                //if (finish_time != "")
                                //    finish_time = $"預計完成時間： <br/> {finish_time}";
                                //tooltip
                                string tooltip = "";
                                if (image != "" || start_time != "" || finish_time != "")
                                    tooltip = $"data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"left\"  data-html=\"true\" title=\"\" data-original-title=\" {start_time} {finish_time} {image}  \"";

                                if (judge == "")
                                    td += $"<td style='text-align:center;width:12%;vertical-align: middle' {tooltip} >{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";

                                else
                                    td += $"<td style='text-align:center;width:12%;vertical-align: middle' {tooltip} >{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";
                            }
                            else
                                td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";
                        }
                        td += "</tr> \n";
                    }
                    str[6] = td;
                    total = count;
                    alarm_total = alarm_list.Count;
                    return str;
                }
                else
                {
                    alarm_total = alarm_list.Count;
                    total = count;
                    return str;
                }
            }
            else
            {
                total = count;
                alarm_total = alarm_list.Count;
                str[6] = " no data";
                return str;
            }
        }

        //new 
        public string[] GetOverViewData(string LineName, string Acc, string _reqType, ref int total, ref int alarm_total, string judge = "", string schdule = "", string responedep = "")
        {
            string td = "";
            string[] str = new string[7] { "0", "0", "0", "0", "0", "0", "0" };//0:all   1:finsh   2:Stop  3:all   4:td_finish   5:td_Stop  7:Data
            string[] ErrorStr;
            string color = "";
            List<string> td_list = new List<string>();
            string SubError = "";
            string Condition = "";
            string Condition1 = "";
            string Condition2 = "";
            string Condition3 = "";
            string Condition4 = "";
            string PredictionTimeStatus = "";
            List<double> percent = new List<double>();
            int nosolved = 0;
            List<string> list = new List<string>();
            List<string> alarm_list = new List<string>();
            int count = 0;
            int PredictionProgress = 0;
            string Line_Names = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            Condition1 = $" 實際組裝時間 ='{DateTime.Now:yyyyMMdd}' ";
            Condition2 = $" 實際組裝時間 <='{DateTime.Now:yyyyMMdd}' AND 狀態!='完成' ";
            Condition3 = $" 實際完成時間 <='{DateTime.Now:yyyyMMdd}235959' AND 實際完成時間 >='{DateTime.Now:yyyyMMdd}010101' ";

            List<string> schdule_list = new List<string>(schdule.Split('#'));
            string schdules = "";
            for (int i = 0; i < schdule_list.Count; i++)
            {
                if (schdule_list[i] != "")
                    schdules += schdules == "" ? $" 排程編號='{schdule_list[i]}' " : $" OR 排程編號='{schdule_list[i]}' ";
            }
            schdules = schdules != "" ? $" and {schdules} " : "";

            Condition = $" (({Condition1}) OR ({Condition2}) OR ({Condition3})) {schdules} order by 組裝日,組裝編號 ";
            DataTable dt = DataTableUtils.DataTable_GetTable(ShareMemory.SQLAsm_WorkStation_State, Condition);
            DataTable dt_line = DataTableUtils.GetDataTable("select 工作站編號,工作站名稱 from 工作站型態資料表");
            int x = 0, y = 0;
            alarm_list = Calculation_Alarm_Or_Behind("", ref x, ref y, true);

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable dt_select = tableColumnSelectForLineDetail(dt, "");

                str = GetEachPiece(dt);

                if (HtmlUtil.Check_DataTable(dt_select))
                {
                    for (int i = 0; i < dt_select.Rows.Count; i++)
                    {
                        td += "<tr class='gradeX'> \n";
                        for (int j = 0; j < dt_select.Columns.Count; j++)
                        {
                            switch (dt_select.Columns[j].ColumnName)
                            {
                                case "狀態":
                                    td_list.Clear();
                                    if (dt.Rows[i]["狀態"].ToString() == "暫停")
                                    {
                                        td_list.Add("red");
                                        td_list.Add(dt.Rows[i]["進度"].ToString());
                                    }
                                    else if (dt.Rows[i]["狀態"].ToString() == "完成")
                                    {
                                        td_list.Add("green");
                                        td_list.Add(dt.Rows[i]["進度"].ToString());
                                    }
                                    else if (dt.Rows[i]["狀態"].ToString() == "啟動")
                                    {
                                        td_list.Add("blue");
                                        td_list.Add(dt.Rows[i]["進度"].ToString());
                                    }
                                    else
                                    {
                                        td_list.Add("black");
                                        td_list.Add("0");
                                    }

                                    td += $"<td style='text-align:center;width:10%;vertical-align: middle'>" +
                                              $"<a href='javascript:void(0)' onclick=SetValue('{dt.Rows[i]["排程編號"]}','{dt.Rows[i]["進度"]}%','{dt.Rows[i]["問題回報"]}','{dt.Rows[i]["狀態"].ToString()}') data-toggle='modal' data-target='#exampleModal'> " +
                                                 $"<span style=color:{td_list[0]}>" +
                                                     $"<u>" +
                                                         $"{td_list[1]}%" +
                                                     $"</u>" +
                                                 $"</span>" +
                                             $"</a>" +
                                           $"</td> \n";
                                    color = "";
                                    td += "<td></td>";
                                    break;
                                case "預計完成日":
                                    DataRow[] rows = dt_line.Select($"工作站編號='{DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])}'");
                                    Line_Names = rows != null && rows.Length > 0 ? DataTableUtils.toString(rows[0]["工作站名稱"]) : "";


                                    td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{Line_Names}</td> \n";
                                    if (DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMdd")) <= DataTableUtils.toDouble(dt_select.Rows[i][j].ToString()))
                                        td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{HtmlUtil.changetimeformat(dt_select.Rows[i][j].ToString())}</td> \n";
                                    else
                                    {
                                        td += $"<td style='text-align:center;width:10%;vertical-align: middle;color:red'>{HtmlUtil.changetimeformat(dt_select.Rows[i][j].ToString())}</td> \n";
                                        count++;
                                    }
                                    break;
                                case "進度":
                                    break;
                                case "問題回報":
                                    break;
                                case "實際啟動時間":
                                    break;
                                case "工作站編號":
                                    break;
                                case "組裝日":
                                    td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{DataTableUtils.toString(dt_select.Rows[i][j].ToString().Substring(4, 2))}/{DataTableUtils.toString(dt_select.Rows[i][j].ToString().Substring(6, 2))}</td> \n";
                                    break;
                                case "異常":
                                    if (dt_select.Rows[i][j].ToString() != "") //  
                                    {

                                        SubError = "";
                                        ErrorStr = DataTableUtils.toString(dt_select.Rows[i][j]).Split(',');
                                        for (int k = 1; k < ErrorStr.Length - 2; k++)
                                            SubError += " " + ErrorStr[k];
                                        list = check_case($" and 排程編號 = '{DataTableUtils.toString(dt_select.Rows[i]["排程編號"])}' ", GetConnByDekVisTmp, DataTableUtils.toString(dt_select.Rows[i]["工作站編號"]), ref nosolved, out _, responedep);
                                        // content
                                        string url = $"ErrorID={DataTableUtils.toString(dt_select.Rows[i]["排程編號"])},ErrorLineNum={DataTableUtils.toString(dt_select.Rows[i]["工作站編號"])},ErrorLineName={Line_Names}";
                                        if (ConfigurationManager.AppSettings["show_function"] == "1")
                                            td += $"<td style='text-align:center;width:48%;vertical-align: middle'>" +
                                                $"<a onclick=jump_Asm_ErrorDetail('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\">" +
                                                $"<div  style='height:100%;width:100%;font-size:20px;vertical-align: middle'>{list[0]} \t " +
                                                $"<u> [{list[1]}] </u>" +
                                                "</div>" +
                                                "</a>" +
                                                "</td> \n";
                                        else
                                            td += "<td style='text-align:center;width:48%'><a onclick=jump_Asm_ErrorDetail('" + WebUtils.UrlStringEncode(url) + "')  href=\"javascript: void()\"><div style='height:100%;width:100%'>異常歷程</div></a></td> \n";
                                    }
                                    else
                                    {
                                        list = check_case($" and 排程編號 = '{DataTableUtils.toString(dt_select.Rows[i]["排程編號"])}' ", GetConnByDekVisTmp, DataTableUtils.toString(dt_select.Rows[i]["工作站編號"]), ref nosolved, out _);
                                        string url = $"ErrorID={DataTableUtils.toString(dt_select.Rows[i]["排程編號"])},ErrorLineNum={dt.Rows[0]["工作站編號"]},ErrorLineName={LineName}";
                                        if (ConfigurationManager.AppSettings["show_function"] == "1")
                                            td += $"<td style='text-align:center;width:48%;vertical-align: middle'><a onclick=jump_Asm_ErrorDetail('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\"><div style='height:100%;width:100%;font-size:20px;vertical-align: middle'>{list[0]} \t <u> [{list[1]}]</u></div></a></td> \n";
                                        else
                                            td += $"<td style='text-align:center;width:48%'><u><a onclick=jump_Asm_ErrorDetail('{WebUtils.UrlStringEncode(url)}')  href=\"javascript: void()\"><div style='height:100%;width:100%'>" + "編輯" + "</div></a></u></td> \n";
                                    }
                                    break;
                                case "排程編號":
                                    //啟動時間
                                    string start_time = "";
                                    if (DataTableUtils.toString(dt_select.Rows[i]["實際啟動時間"]) != "")
                                        start_time = $"實際啟動時間： <br/> {StrToDate(DataTableUtils.toString(dt_select.Rows[i]["實際啟動時間"]))} <br/> ";

                                    //預計完成時間
                                    string finish_time = "";

                                    string tooltip = "";
                                    if (start_time != "" || finish_time != "")
                                        tooltip = $"data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"left\"  data-html=\"true\" title=\"\" data-original-title=\" {start_time} {finish_time}  \"";

                                    if (judge == "")
                                        td += $"<td style='text-align:center;width:12%;vertical-align: middle' {tooltip} >{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";

                                    else
                                        td += $"<td style='text-align:center;width:12%;vertical-align: middle' {tooltip} >{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";
                                    break;
                                default:
                                    td += $"<td style='text-align:center;width:10%;vertical-align: middle'>{DataTableUtils.toString(dt_select.Rows[i][j])}</td> \n";
                                    break;
                            }
                        }
                        td += "</tr> \n";
                    }
                    str[6] = td;
                    total = count;
                    alarm_total = alarm_list.Count;
                    return str;
                }
                else
                {
                    alarm_total = alarm_list.Count;
                    total = count;
                    return str;
                }
            }
            else
            {
                total = count;
                alarm_total = alarm_list.Count;
                str[6] = " no data";
                return str;
            }
        }

        //計算臥式的預計完成日
        public DataTable Return_NowMonthTotal(DataTable dt, string start_date, string end_date, out DataTable Finished, out DataTable NoFinish)
        {
            try
            {

                dt.Columns.Add("預計完工日");
                work.工作時段_新增(8, 0, 12, 0);
                work.工作時段_新增(13, 0, 17, 0);

                int standard_work = 0;
                foreach (DataRow row in dt.Rows)
                {
                    standard_work = DataTableUtils.toInt(DataTableUtils.toString(row["標準工時"]));
                    standard_work = standard_work == 0 ? 1 : standard_work;

                    DateTime stand_endtime = work.目標日期(StrToDate(row["上線日"].ToString()), new TimeSpan(0, 0, standard_work));
                    row["預計完工日"] = stand_endtime.ToString("yyyyMMdd");
                }

            }
            catch
            {

            }

            //撈出預計下架日在本月的
            DataTable dt_now = dt.Clone();
            string sqlcmd = $"預計完工日>='{start_date}' and 預計完工日<='{end_date}'";
            DataRow[] rows = dt.Select(sqlcmd);
            if (rows != null && rows.Length > 0)
                for (int i = 0; i < rows.Length; i++)
                    dt_now.ImportRow(rows[i]);

            //撈出預計下架日在上個月，但在本月完成
            DataTable dt_Finish = dt.Clone();
            sqlcmd = $"預計完工日<'{start_date}' and (實際完成時間>='{start_date}000000' OR 實際完成時間>='{start_date}')";
            rows = dt.Select(sqlcmd);
            if (rows != null && rows.Length > 0)
                for (int i = 0; i < rows.Length; i++)
                    dt_Finish.ImportRow(rows[i]);

            //撈出到目前為止皆未完成的
            DataTable dt_NoFinish = dt.Clone();
            sqlcmd = $"預計完工日<='{start_date}' and (實際完成時間 IS null OR 實際完成時間='' ) ";
            rows = dt.Select(sqlcmd);
            if (rows != null && rows.Length > 0)
                for (int i = 0; i < rows.Length; i++)
                    dt_NoFinish.ImportRow(rows[i]);
            Finished = dt_Finish;
            NoFinish = dt_NoFinish;

            return dt_now;
        }

        //查詢機台的照片
        private string Search_Image(string Machine, string link)
        {
            GlobalVar.UseDB_setConnString(link);
            string sqlcmd = $"select * from Machine_Image where machine_Name = '{Machine.Split('-')[0]}' ";
            DataRow row = DataTableUtils.DataTable_GetDataRow(sqlcmd);
            if (row != null)
            {
                try
                {
                    return DataTableUtils.toString(row["Machine_ImageUrl"]);
                }
                catch
                {
                    return "";
                }
            }
            else
                return "";
        }
        //取得異常頁面
        public DataTable GetNosovle_ITEC(List<string> list)
        {
            if (list.Count <= 1)
                return null;
            else
            {
                DataTable dt = new DataTable();
                List<string> copylist = new List<string>();
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (i % 2 == 0)
                        copylist.Add(list[i]);
                }
                copylist = copylist.Distinct().ToList();

                dt.Columns.Add("上線日");
                dt.Columns.Add("產線");
                dt.Columns.Add("客戶");
                dt.Columns.Add("編號");
                dt.Columns.Add("進度");
                dt.Columns.Add("預定進度");
                dt.Columns.Add("未解決數量");

                for (int i = 0; i < copylist.Count; i++)
                {
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                    string sqlcmd = $"select  組裝日 as 上線日,工作站名稱 as 產線名稱,     CUSTNM 客戶, 工作站狀態資料表.排程編號 as 排程編號,進度 ,實際啟動時間 as 預定進度 " +
                                    $" from 工作站狀態資料表 " +
                                    $" left join 組裝資料表 on 組裝資料表.排程編號 = 工作站狀態資料表.排程編號 " +
                                    $" left join 工作站型態資料表 on 工作站狀態資料表.工作站編號 = 工作站型態資料表.工作站編號 " +
                                    $" where 工作站狀態資料表.排程編號 = '{copylist[i]}' and 工作站狀態資料表.工作站編號=1";
                    DataTable dt_detail = DataTableUtils.GetDataTable(sqlcmd);

                    if (HtmlUtil.Check_DataTable(dt_detail))
                        dt.Rows.Add(DataTableUtils.toString(dt_detail.Rows[0]["上線日"]),
                                    DataTableUtils.toString(dt_detail.Rows[0]["產線名稱"]),
                                    DataTableUtils.toString(dt_detail.Rows[0]["客戶"]),
                                    DataTableUtils.toString(dt_detail.Rows[0]["排程編號"]),
                                    DataTableUtils.toString(dt_detail.Rows[0]["進度"]),
                                    DataTableUtils.toString(dt_detail.Rows[0]["預定進度"]),
                                    list.AsEnumerable().Where(w => w == copylist[i]).Count());


                }
                return dt;
            }

        }
        //計算預定%數跟實際%數
        public List<double> percent_calculation(string schedule_number, string percent, ref string prediction_finish, string LineNum = "", bool fix = false)
        {

            string times = "";
            work.工作時段_新增(8, 0, 12, 0);
            work.工作時段_新增(13, 0, 17, 0);
            List<double> list = new List<double>();
            string sqlcmd = "";
            DataTable dr = new DataTable();
            double real_percent = 0, predict_percent = 0;

            string finishTime = "";
            //預定進度

            //立式場用
            if (GetConnByDekVisTmp.ToLower().Contains("assm"))
                sqlcmd = $" SELECT 組裝日, 實際啟動時間, (case  when 選用通用工時 = '0' Then 標準工時  when 選用通用工時 <> '0' Then IFNULL( 組裝時間 ,標準工時) end ) as 標準工時 FROM 工作站狀態資料表 LEFT JOIN 工藝名稱資料表 ON 工藝名稱資料表.工作站編號 = 工作站狀態資料表.工作站編號 LEFT JOIN 立式工藝 ON   SUBSTRING_INDEX(排程編號, '-', 1)= 立式工藝.機種編號 LEFT JOIN 工作站型態資料表 ON 工作站型態資料表.工作站編號 = 工作站狀態資料表.工作站編號 WHERE 工作站狀態資料表.排程編號 = '{schedule_number}' and (組裝時間 is not null OR 標準工時 is not null) ";
            else if (GetConnByDekVisTmp.ToLower().Contains("hor"))
            {
                sqlcmd = $"select 實際啟動時間,(組裝工時*60*60) as 標準工時 from 工作站狀態資料表 left join 臥式工藝 on    SUBSTRING(工作站狀態資料表.排程編號,  1, LENGTH(工作站狀態資料表.排程編號) - 8)= 臥式工藝.機種編號  where 排程編號 = '{schedule_number}' and 工作站編號 = '{LineNum}' ";
            }
            dr = clsdb.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dr))
            {
                if (DataTableUtils.toString(dr.Rows[0]["標準工時"]) == "")
                    predict_percent = double.NaN;
                int standard_worktime = DataTableUtils.toInt(DataTableUtils.toString(dr.Rows[0]["標準工時"]));

                DateTime start_time;

                string timestart = "";
                if (LineNum == "11" && !fix)
                    timestart = DataTableUtils.toString(dr.Rows[0]["組裝日"]).Trim() + "080000";

                else
                    timestart = DataTableUtils.toString(dr.Rows[0]["實際啟動時間"]);

                if (timestart != "")
                {
                    start_time = HtmlUtil.StrToDate(timestart);
                    DateTime stand_endtime = work.目標日期(start_time, new TimeSpan(0, 0, standard_worktime));//預計完成時間
                    finishTime = stand_endtime.ToString();
                    DateTime end_time = DateTime.Now;
                    TimeSpan prediction_time = work.工作時數(start_time, end_time);
                    real_percent = DataTableUtils.toDouble(percent) / 100;
                    predict_percent = prediction_time.TotalSeconds / standard_worktime;

                }

            }
            prediction_finish = finishTime;

            predict_percent = dr == null || DataTableUtils.toString(dr.Rows[0]["標準工時"]) == "" ? double.NaN : predict_percent;



            //都沒有抓到的話，就會回傳0
            list.Add(real_percent);
            list.Add(predict_percent);
            return list;
        }
        //比對進度是否落後
        public string Comparison_Schedule(double real_percent, double perdict_percent, ref int count, ref bool delay)
        {
            int x = 1;

            delay = false;
            if (Math.Round(perdict_percent * 100) > Math.Round(real_percent * 100) && real_percent < 1 && Double.IsInfinity(perdict_percent) == false && Math.Round(real_percent * 100) < 99)
            {
                delay = true;
                count += x;
            }
            if (perdict_percent.ToString() == "非數值" || Double.IsInfinity(perdict_percent))
                return "開發機";
            else if (perdict_percent > 1)
                return 100 + "%";
            else
                return Math.Round(perdict_percent * 100) + "%";
        }
        //找尋該排程編號的是否含有超過4小時未處理的案子
        private List<string> check_case(string scheduleID, string link, string LineNum, ref int alarm_nosolved, out List<string> nosolved_list, string responedep = "")
        {
            int alarm_time = 240;
            string show_errorlight = "Y";
            string copy_errorlight = "Y";
            List<string> relist = new List<string>();
            int total = 0, not_sloved = 0;
            string alarm = "";
            string show_YN = "Y";
            List<string> list = new List<string>();
            LineNum = LineNum == "" ? "" : $" and 工作站異常維護資料表.工作站編號 = {LineNum} ";



            //GlobalVar.UseDB_setConnString(link);

            string sqlcmd = $"SELECT  異警時間, 異常維護編號, 異常原因類型, 排程編號, IFNULL((SELECT  MAX(時間紀錄) FROM 工作站異常維護資料表 a WHERE a.父編號 = 工作站異常維護資料表.異常維護編號), 時間紀錄) 最後回覆時間, 是否持續顯示異常, 異警持續顯示, (SELECT  異常原因類型 FROM 工作站異常維護資料表 a WHERE a.父編號 = 工作站異常維護資料表.異常維護編號 and 最後回覆時間 = a.時間紀錄 AND 結案判定類型 IS  NULL) 當前類型,  IFNULL((SELECT 責任單位 FROM 工作站異常維護資料表 a WHERE a.父編號 = 工作站異常維護資料表.異常維護編號 AND 最後回覆時間 = a.時間紀錄 ),工作站異常維護資料表.責任單位) 當前責任單位, (SELECT  結案判定類型 FROM 工作站異常維護資料表 a WHERE a.父編號 = 工作站異常維護資料表.異常維護編號 AND 結案判定類型 IS NOT NULL) 結案判定類型 FROM 工作站異常維護資料表, 工作站型態資料表, 工作站結案異常類型資料表 WHERE 工作站異常維護資料表.工作站編號 = 工作站型態資料表.工作站編號 AND 工作站結案異常類型資料表.備註內容 = 工作站異常維護資料表.異常原因類型 AND (父編號 IS NULL OR 父編號 = 0) {LineNum}  {scheduleID}";
            DataTable dt = clsdb.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                if (responedep != "")
                {
                    DataRow[] rows = dt.Select($"當前責任單位 <> '{responedep}'");
                    for (int i = 0; i < rows.Length; i++)
                        rows[i].Delete();
                    dt.AcceptChanges();
                }

                total = dt.Rows.Count;
                foreach (DataRow row in dt.Rows)
                {
                    alarm_time = DataTableUtils.toInt(DataTableUtils.toString(row["異警時間"])) == 0 ? 240 : DataTableUtils.toInt(DataTableUtils.toString(row["異警時間"]));
                    show_errorlight = DataTableUtils.toString(row["是否持續顯示異常"]);
                    copy_errorlight = show_errorlight;

                    string problem_type = row["結案判定類型"].ToString();
                    //問題尚未解決
                    if (problem_type == "")
                    {
                        string errortype = row["當前類型"].ToString() != "" ? row["當前類型"].ToString() : row["異常原因類型"].ToString();
                        show_YN = row["異警持續顯示"].ToString();
                        show_errorlight = show_YN;
                        not_sloved++;
                        DateTime start_time = StrToDate(DataTableUtils.toString(row["最後回覆時間"]));
                        TimeSpan span = work.工作時數(start_time, DateTime.Now);
                        if (Math.Abs(span.TotalMinutes) >= Math.Abs(alarm_time))
                            relist.Add(row["排程編號"].ToString());
                        else
                            relist.Add(row["排程編號"].ToString());

                        if (Math.Abs(span.TotalMinutes) >= Math.Abs(alarm_time) && show_errorlight == "Y")
                            alarm = "<img src=\"../../assets/images/shutdown.gif\" width=\"26px\" height=\"26px\">";
                    }
                }
            }
            if (alarm == "")
                list.Add("<img src=\"../../assets/images/normal.png\" width=\"26px\" height=\"26px\">");
            else
                list.Add(alarm);

            list.Add($"  {not_sloved}  /  {total}  ");
            alarm_nosolved += not_sloved;
            nosolved_list = relist;
            return list;
        }
        public bool GetRW(string _acc)
        {
            bool Write = false;
            clsDB_Switch.dbOpen(GetConnByDekVisErp);
            DataTableUtils.Conn_String = GetConnByDekVisErp;
            if (clsDB_Switch.IsConnected == true)
            {
                DataRow dr_rw = DataTableUtils.DataTable_GetDataRow($"select FUNC_YN from  SYSTEM_PMR where USER_ACC='{_acc}' and FUNC_DES = '產線異動'");

                if (dr_rw != null && dr_rw[0].ToString().ToUpper() == "Y")
                {
                    Write = true;
                    //back
                    clsDB_Switch.dbOpen(GetConnByDekVisTmp);
                    DataTableUtils.Conn_String = GetConnByDekVisTmp;
                }
            }
            else
                DataTableUtils.Conn_String = GetConnByDekVisTmp;
            return Write;
        }
        public DataRow GetAccInf(string _acc)
        {
            DataRow dr_rw = null;
            clsDB_Switch.dbOpen(GetConnByDekVisErp);
            DataTableUtils.Conn_String = GetConnByDekVisErp;
            if (clsDB_Switch.IsConnected == true)
            {
                dr_rw = DataTableUtils.DataTable_GetDataRow("select * from  USERS where USER_ACC=" + "'" + _acc + "'");

                clsDB_Switch.dbOpen(GetConnByDekVisTmp);
                DataTableUtils.Conn_String = GetConnByDekVisTmp;
            }
            else
                DataTableUtils.Conn_String = GetConnByDekVisTmp;
            return dr_rw;
        }
        public List<string> GetErrorProcessStatus()
        {
            List<string> ErrorStatus = new List<string>();
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            DataTable dt_ErrorStatus = DataTableUtils.DataTable_GetTable("select 狀態名稱  from " + ShareMemory.SQLAsm_WorkStation_ErrorProcessStatus, 0, 0);

            if (HtmlUtil.Check_DataTable(dt_ErrorStatus))
            {
                foreach (DataRow dr in dt_ErrorStatus.Rows)
                    ErrorStatus.Add(dr["狀態名稱"].ToString());
            }
            return ErrorStatus;
        }
        public int GetSeriesNum(string tableName, int ColumnIndex)
        {
            int Count = 1;
            DataTableUtils.Conn_String = GetConnByDekVisTmp;

            DataTable dr_hear = DataTableUtils.DataTable_GetRowHeader(tableName);
            DataTable dt = DataTableUtils.DataTable_GetTable($"select {dr_hear.Columns[ColumnIndex]} from {tableName} order by {dr_hear.Columns[ColumnIndex]} desc");
            string field_name = DataTableUtils.toString(dr_hear.Columns[ColumnIndex]);

            if (HtmlUtil.Check_DataTable(dt))
            {
                Count = DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0][field_name])) + 1;
                return Count;
            }
            else if (dt != null && dt.Rows.Count == 0)
                return Count;
            else
                return GetSeriesNum(tableName, ColumnIndex);
        }
        public string[] GetTimeType(string Str)
        {
            string[] time = new string[7];
            if (Str == "day")
            {
                time[0] = DateTime.Now.ToString("yyyyMMdd").ToString() + "010101";
                time[1] = DateTime.Now.ToString("yyyyMMdd").ToString() + "235959";
                time[3] = "HH:mm";
                // time[4] = "產線狀態 " + "(" + "今日" + ")";
                //time[4] = "(" + "本日" + ")";
                time[4] = "(" + DateTime.Now.ToString("yyyy-MM-dd").ToString() + ")";
                //time[4] = "產線狀態 " + "(" + DateTime.Now.ToString("yyyy/MM/dd").ToString() + ")";
                time[5] = "1";
                time[6] = "hour";
            }
            else if (Str == "week")
            {
                time[0] = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).ToString("yyyyMMdd");
                time[1] = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 5).ToString("yyyyMMdd");
                time[3] = "DD MM YYYY";
                //time[4] = "產線狀態 " + "( 一周)";
                //time[4] = "( 一周)";
                time[4] = "(" + DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).ToString("MM/dd") + "-" + DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 5).ToString("MM/dd") + ")";
                time[5] = "1";
                time[6] = "day";
                //time[0] = DateTime.Now.AddDays(-9).ToString("yyyyMMddmmhhss").ToString();
                //time[1] = DateTime.Now.ToString("yyyyMMddmmhhss").ToString();
            }
            else if (Str == "month")
            {
                time[0] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyyMMdd");
                time[1] = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1).ToString("yyyyMMdd");
                time[3] = "DD";
                //time[4] = "產線狀態 " + "(" + DateTime.Now.ToString("yyyy/MM").ToString() + ")";
                //time[4] = "產線狀態 " + "(" + "本月" + ")";
                //time[4] = "(" + "本月" + ")";
                time[4] = "(" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy年MM月") + ")";
                time[5] = "1";
                time[6] = "day";
                //time[0] = DateTime.Now.AddMonths(-1).ToString("yyyyMMddmmhhss").ToString();
                //time[1] = DateTime.Now.ToString("yyyyMMddmmhhss").ToString();
            }
            else if (Str == "season")
            {
                time[0] = DateTime.Now.AddMonths(-3).ToString("yyyyMMdd").ToString();
                time[1] = DateTime.Now.ToString("yyyyMMdd").ToString();
                time[3] = "MMM";
                time[4] = "產線狀態 " + "(本季)";
                time[5] = "1";
                time[6] = "month";
            }
            else if (Str == "fyear")
            {
                time[0] = DateTime.Now.ToString("yyyy0101");
                time[1] = DateTime.Now.ToString("yyyy0630");
                time[3] = "MMM";
                //time[4] = "產線狀態 " + "(前半年度)";
                time[4] = "(1-6月)";
                time[5] = "1";
                time[6] = "month";
                //time[0] = DateTime.Now.AddMonths(-6).ToString("yyyyMMddmmhhss").ToString();
                //time[1] = DateTime.Now.ToString("yyyyMMddmmhhss").ToString();
            }
            else if (Str == "byear")
            {
                time[0] = DateTime.Now.ToString("yyyy0701");
                time[1] = DateTime.Now.ToString("yyyy1231");
                time[3] = "MMM";
                //time[4] = "產線狀態 " + "(後半年度)";
                time[4] = "(7-12月)";
                time[5] = "1";
                time[6] = "month";
                //time[0] = DateTime.Now.AddMonths(-6).ToString("yyyyMMddmmhhss").ToString();
                //time[1] = DateTime.Now.ToString("yyyyMMddmmhhss").ToString();
            }
            else if (Str == "year")
            {
                time[0] = DateTime.Now.ToString("yyyy0101");
                time[1] = DateTime.Now.ToString("yyyy1231");
                time[3] = "MMM YYYY";
                // time[4] = "產線狀態 " + "(全年度)";
                // time[4] = "(全年度)";
                time[4] = DateTime.Now.ToString("yyyy") + "年";
                time[5] = "1";
                time[6] = "month";
                // time[0] = DateTime.Now.AddYears(-1).ToString("yyyyMMddmmhhss").ToString();
                // time[1] = DateTime.Now.ToString("yyyyMMddmmhhss").ToString();
            }
            else
            {
                time[0] = DateTime.Now.ToString("yyyyMMdd").ToString() + "010101";
                time[1] = DateTime.Now.ToString("yyyyMMdd").ToString() + "235959";
                time[3] = "hh:mm";
                //time[4] = "產線狀態 " + "(" + DateTime.Now.ToString("yyyy/MM/dd").ToString() + ")";
                time[4] = "(" + DateTime.Now.ToString("yyyy-MM-dd").ToString() + ")";
            }
            time[2] = Str;
            return time;
        }
        public object GetLineList()
        {
            DataTableUtils.Conn_String = GetConnByDekdekVisAssmHor;
            string condition = " where 工作站是否使用中='1' OR  工作站是否使用中= 'True'";
            //string condition = "";
            //string condition = "";
            DataTable dt_line = DataTableUtils.DataTable_GetTable("select 工作站編號,工作站名稱 from " + ShareMemory.SQLAsm_WorkStation_Type + condition, 0, 0);
            List<LineData> Lines = new List<LineData>();//special Math
            if (dt_line != null)
            {
                foreach (DataRow dr in dt_line.Rows)
                    Lines.Add(new LineData() { LineId = DataTableUtils.toInt(dr["工作站編號"].ToString()), LineName = dr["工作站名稱"].ToString() });
            }
            return Lines;
        }
        public string GetColumnName(string PageName)
        {
            string ColumnName = "";
            string[] muilt;
            //string[] Colweight;
            switch (PageName)
            {

                case "Asm_LineTotalView":
                    muilt = new string[5] { "機種名稱", "在線", "完成", "異常", "落後" };
                    break;
                case "Asm_LineOverView":
                    if (ConfigurationManager.AppSettings["show_function"] == "1")
                        muilt = new string[7] { "上線日", "客戶", "編號", "進度", "預定進度", "預計完成日", "未解決/全部" };
                    else
                        muilt = new string[6] { "上線日", "客戶", "編號", "進度", "預定進度", "異常歷程" };
                    break;
                case "Asm_LineSearch":
                    if (ConfigurationManager.AppSettings["show_function"] == "1")
                        muilt = new string[8] { "上線日", "客戶", "編號", "進度", "預定進度", "工作站名稱", "預計完成日", "未解決/全部" };
                    else
                        muilt = new string[7] { "上線日", "客戶", "編號", "進度", "預定進度", "工作站名稱", "異常歷程" };
                    break;
                /*20200221修改這裡*/
                case "Asm_NumView":
                    muilt = new string[6] { "上線日", "工作站名稱", "客戶", "編號", "進度", "備註" };
                    break;
                /*20200221修改這裡*/
                case "Asm_ErrorSearch":
                    muilt = new string[4] { "編號", "排程編號", "站名", "原因" };
                    break;
                case "Asm_ErrorDetail":
                    muilt = new string[3] { "人員", "內容", "狀態" };
                    break;
                case "ErrorChat":
                    muilt = new string[4] { "排程編號", "起始時間", "排除時間", "處理時間" };
                    break;
                case "Product_Detail":
                    muilt = new string[4] { "工藝時間", "起始時間", "完成時間", "預計完成" };
                    break;
                case "Asm_history":
                    muilt = new string[6] { "排程編號", "產線名稱", "起始時間", "完成時間", "組裝時間[分]", "異常件數" };
                    break;
                default:
                    muilt = new string[5] { "產線編號", "產線名稱", "排程產能", "實際產量", "達成率(%)" };
                    break;
            }

            ColumnName = " <tr id=\"tr_row\">";
            for (int i = 0; i < muilt.Length; i++)
                ColumnName += $"<th style=\"text-align:center;\">{muilt[i]}</th>";
            ColumnName += "</tr>";
            return ColumnName;
        }
        public string GetCharstColumnName_Error(string SelectLine, CheckBoxList ColumnNameList, string PageName)
        {
            string ColumnName = "";
            string FirstTitle = "";
            if (PageName == "Asm_Cahrt_Error")
                FirstTitle = "錯誤類型";
            else if (PageName == "Asm_Compliance_rate")
                FirstTitle = "時間列表";
            else
                FirstTitle = "未定義";
            ColumnName = "";

            if (SelectLine == "0")
            {
                ColumnName += $"<th>{FirstTitle}</th>";
                foreach (ListItem Name in ColumnNameList.Items)
                    ColumnName += $"<th >{Name.Text}</th>";
            }

            return ColumnName;
        }
        public static string Last_Place(string acc, string factory = "")
        {
            string str = "";
            clsDB_Server clsDB = new clsDB_Server("");
            clsDB.dbOpen(myclass.GetConnByDekVisErp);
            DataTableUtils.Conn_String = myclass.GetConnByDekVisErp;

            string sql_cmd = $"select * from users where USER_ACC = '{acc}'";
            DataTable dt = clsDB.GetDataTable(sql_cmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                if (factory == "")
                {
                    if (DataTableUtils.toString(dt.Rows[0]["Last_Model"]) != "")
                        return DataTableUtils.toString(dt.Rows[0]["Last_Model"]);
                    else
                        return "Ver";
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.NewRow();
                        //row["Last_Model"] =factory;
                        row["Last_Model"] = "Hor";

                        if (clsDB.Update_DataRow("users", $"USER_ACC = '{acc}'", row))
                        {
                            str = "";
                        }
                    }
                }
            }
            //return str;
            return "Hor";
        }
        //=========================Tool===============================================
        public string TrsDate(string Source)
        {
            string[] date;
            if (Source.Contains('_'))
            {
                date = Source.Split('_');
                return date[1];
            }
            else
                return "month";
        }
        public string TrsTime(string Src, string Unit)
        {
            string TrsTime = "0";
            string tmp = "";
            if (DataTableUtils.toInt(Src) >= 60)
            {
                TimeSpan ts = new TimeSpan(0, 0, DataTableUtils.toInt(Src));
                if (Unit.Contains('m'))
                    TrsTime = (ts.Days * 24 * 60 + ts.Hours * 60 + ts.Minutes).ToString();//Minutes
                else
                {
                    tmp = (DataTableUtils.toDouble(ts.Minutes.ToString()) / 60).ToString();
                    if (tmp.Length >= 3)
                    {
                        tmp = tmp.Remove(0, 1).Substring(0, 2);
                        if (!tmp.Contains('0'))
                            TrsTime = (ts.Days * 24 + ts.Hours).ToString() + tmp; //Minutes
                        else
                            TrsTime = (ts.Days * 24 + ts.Hours).ToString();
                    }
                    else
                        TrsTime = (ts.Days * 24 + ts.Hours).ToString(); //Minutes
                }
            }
            else
            {
                if (Src != "0")
                    TrsTime = "> 1";
            }
            return TrsTime;
        }
        //==========================Chart===============================================
        public string[] GetErrorInf(CheckBoxList LineList, string timetype, string TimeType, string StartDate = "Today", string EndDate = "Today", List<string> SelectLine = null)
        {
            //ALL Line

            double ErrorTimeSum = 0;
            string td = "";
            string ChartDataStr_Count = "";
            string ChartDataStr_Time = "";
            string[] infStr = new string[6];////0:ChartDataStr 1:ErrorCount 2:   3:td content:
            string[] ErrorType;
            string condition = "";
            string ViewCondition = "";
            string sortname = "";
            string CountCondition = "";
            string FinisCondition = "";
            string Location = "";
            int BreakCount = 1;
            int TimeCount = 0;
            int ErrorCount = 0;
            string ErrorStr = "";
            string url = "";
            Dictionary<string, int> ErrorType_ListD = new Dictionary<string, int>();
            Dictionary<string, double> ErrorTime_ListD = new Dictionary<string, double>();
            List<string> ErrorType_List = new List<string>();
            DataTable dt_er_sort;
            DataTable dT_errorFather;
            DataTable dT_errorSon;
            DataRow dr_son;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            DataTable dt_Line = DataTableUtils.DataTable_GetTable(ShareMemory.SQLAsm_WorkStation_Type, "工作站是否使用中='1' OR 工作站是否使用中= 'True'");

            if (StartDate == "Today" && EndDate == "Today")
                condition = $"where {condition} 時間紀錄>='{DateTime.Now:yyyyMMdd}010101' AND 時間紀錄 <= '{DateTime.Now:yyyyMMdd}235959'";
            else
                condition = $"where {condition} 時間紀錄>='{StartDate}010101' AND 時間紀錄 <= '{EndDate}235959' and  LENGTH( 結案判定類型 )>0 and 結案判定類型 is not null";

            DataTableUtils.Conn_String = GetConnByDekVisTmp;

            dT_errorFather = DataTableUtils.DataTable_GetTable($"select * from {ShareMemory.SQLAsm_WorkStation_ErrorMant} " + condition, 0, 0);
            if (SelectLine != null)
            {
                if (dT_errorFather != null)
                {
                    dT_errorSon = dT_errorFather.Clone();
                    foreach (DataRow dr_fa in dT_errorFather.Rows)
                    {
                        DataTableUtils.Conn_String = GetConnByDekVisTmp;
                        dr_son = DataTableUtils.DataTable_GetDataRow($"select tb.異常維護編號,  ta.工作站編號,ta.排程編號,ta.時間紀錄 as 維護內容,tb.時間紀錄,tb.結案判定類型,tb.父編號 from 工作站異常維護資料表 as ta left join 工作站異常維護資料表 as tb on tb.父編號 = ta.異常維護編號  where ta.異常維護編號='{dr_fa["父編號"]}' and tb.結案判定類型 is not null and LENGTH(tb.結案判定類型) >0");
                        if (dr_son != null)
                        {
                            if (SelectLine.IndexOf("0") != -1)
                                dT_errorSon.ImportRow(dr_son);
                            else if (SelectLine.IndexOf(DataTableUtils.toString(dr_son["工作站編號"])) != -1)
                                dT_errorSon.ImportRow(dr_son);
                        }

                    }
                }
                else
                    dT_errorSon = null;
            }
            else
                dT_errorSon = null;

            if (dT_errorSon != null && dT_errorSon.Rows.Count != 0)
            {
                DataView dV_error = dT_errorSon.DefaultView;
                DataView dV_error_Count = dT_errorSon.DefaultView;
                var ErrorTyped = dT_errorSon.AsEnumerable().GroupBy(g => g.Field<string>("結案判定類型")).Where(w => !string.IsNullOrEmpty(w.Key)).Select(s => s.Key);
                foreach (string key in ErrorTyped)
                    ErrorType_ListD.Add(key, dT_errorSon.AsEnumerable().Where(w => w.Field<string>("結案判定類型") == key).Count());

                var dicSort = from objDic in ErrorType_ListD orderby objDic.Value descending select objDic;//dec descending
                BreakCount = 1;

                TimeCount = ErrorType_ListD.Count;
                foreach (KeyValuePair<string, int> kvp in dicSort)
                {
                    if (kvp.Key == "" || kvp.Key.ToUpper() == "NOUSE") continue;
                    ErrorTimeSum = 0;
                    td += "<tr class=\"gradeX\" style=\"text-align:center;\">";
                    td += $"<td style=\"text-align:center;\">{kvp.Key}</td>";
                    foreach (ListItem item in LineList.Items)
                    {
                        if (item.Value == "0") continue;
                        ViewCondition = $"結案判定類型 Like '%{kvp.Key}%'";
                        dV_error.RowFilter = ViewCondition;//有結案判定類型的
                        if (dV_error.Count != 0)
                        {
                            ErrorCount = 0;
                            ErrorStr = "";
                            foreach (DataRow dr in dV_error.ToTable().Rows)
                            {
                                if (dt_Line != null && dt_Line.Rows.Count != 0)
                                {
                                    var LineSN = dt_Line.AsEnumerable().Where(w => w.Field<string>("工作站名稱") == item.Text).Select(s => s.Field<string>("工作站編號")).FirstOrDefault();
                                    var liveError = dT_errorSon.AsEnumerable().Where(w => dr["異常維護編號"].ToString() == w.Field<string>("異常維護編號") && w.Field<string>("工作站編號") == LineSN.ToString()).FirstOrDefault();
                                    if (liveError != null)
                                    {
                                        ErrorCount++;
                                        if (dV_error.ToTable().Rows.IndexOf(dr) == dV_error.ToTable().Rows.Count - 1)
                                            ErrorStr += liveError["排程編號"].ToString();
                                        else
                                            ErrorStr += liveError["排程編號"].ToString() + "$";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(ErrorStr))
                            {
                                var LineSnn = dt_Line.AsEnumerable().Where(w => w.Field<string>("工作站名稱") == item.Text).Select(s => s.Field<string>("工作站編號")).FirstOrDefault();
                                Location = GetConnByDekVisTmp.Contains("Hor") ? "Hor" : "Ver";
                                url = $"Local={Location},ErrorLineNum={LineSnn},Errorkey={ErrorStr},Date_str={StartDate},Date_end={EndDate},ErrorType={kvp.Key}";
                                if (ConfigurationManager.AppSettings["URL_ENCODE"] == "1")
                                    url = WebUtils.UrlStringEncode(url);
                                td += $"<td style=\"text-align:center;\"><a href=\"Asm_history.aspx?key={url}\"><div style=\"height:100%;width:100%\">{ErrorCount}</div></a></td>";
                            }
                            else
                                td += $"<td style=\"text-align:center;\">{ErrorCount}</td>";
                        }
                        else
                            td += $"<td style=\"text-align:center;\">{dV_error.Count}</td>";
                    }
                    td += "</tr>";
                    dV_error_Count = dT_errorSon.DefaultView;
                    CountCondition = $"結案判定類型 Like '%{kvp.Key}%'";
                    dV_error_Count.RowFilter = CountCondition;
                    if (BreakCount <= TimeCount)
                    {
                        ChartDataStr_Count += "{" +
                                               $"y:{dV_error_Count.Count},indexLabel:'{kvp.Key}'" +
                                              "},";
                        change += kvp.Key + ",";
                        infStr[4] += kvp.Value + ",";
                    }
                    //時間計算
                    dt_er_sort = dV_error.ToTable();

                    foreach (DataRow dr in dt_er_sort.Rows)
                    {
                        work.工作時段_新增(8, 0, 12, 0);
                        work.工作時段_新增(13, 0, 17, 0);
                        DateTime StratTime = StrToDateTime(dr["維護內容"].ToString(), "yyyyMMddHHmmss");
                        DateTime Endtime = StrToDateTime(dr["時間紀錄"].ToString(), "yyyyMMddHHmmss");
                        TimeSpan tsp = work.工作時數(StratTime, Endtime);
                        ErrorTimeSum += tsp.TotalSeconds;
                    }
                    if (BreakCount <= TimeCount)
                    {
                        if (BreakCount != TimeCount)
                            ErrorTime_ListD.Add(kvp.Key, ErrorTimeSum);
                        else
                        {
                            ErrorTime_ListD.Add(kvp.Key, ErrorTimeSum);
                            var dicTimeSort = from objDic in ErrorTime_ListD orderby objDic.Value descending select objDic;//dec descending
                            {
                                foreach (KeyValuePair<string, double> kvp_time in dicTimeSort)//時間
                                {
                                    sortname += $"{kvp_time.Value},{kvp_time.Key}:";
                                    infStr[5] += $"{kvp_time.Value},";
                                }
                                ChartDataStr_Time = sortresult(change, sortname, timetype);
                            }
                        }
                        BreakCount++;
                    }
                    dt_er_sort.Clear();
                }
                if (ChartDataStr_Count != "" && td != "")
                {
                    infStr[0] = ChartDataStr_Count.Remove(ChartDataStr_Count.LastIndexOf(","), 1);
                    infStr[2] = ChartDataStr_Time.Remove(ChartDataStr_Time.LastIndexOf(","), 1);
                    infStr[3] = td;
                }
            }
            return infStr;
        }
        //讓時間的標籤跟隨次數
        public string sortresult(string change, string sortname, string timetype)
        {
            if (timetype == "")
                timetype = "(分鐘)";

            string[] chsplit = change.Split(',');
            string[] sosplit = sortname.Split(':');
            int i;
            string[] all = new string[chsplit.Length - 1];

            for (int a = 0; a < sosplit.Length - 1; a++)
            {
                i = 0;
                for (int j = 0; j < chsplit.Length - 1; j++)
                {
                    if (chsplit[j] == sosplit[a].Split(',')[1])
                    {
                        if (timetype == "(分鐘)")
                        {
                            double times = double.Parse(sosplit[a].Split(',')[0]);
                            double minutes = times / 60;
                            all[i] = "{y:" + minutes + "," + "indexLabel:" + "'" + sosplit[a].Split(',')[1] + "'" + "}" + "," + "\n";
                        }
                        if (timetype == "(小時)")
                        {
                            double times = double.Parse(sosplit[a].Split(',')[0]);
                            double minutes = times / 3600;
                            all[i] = "{y:" + minutes + "," + "indexLabel:" + "'" + sosplit[a].Split(',')[1] + "'" + "}" + "," + "\n";
                        }

                        break;
                    }
                    else
                        i++;
                }
            }
            string result = "";
            for (int b = 0; b < all.Length; b++)
            {
                result += all[b];
            }
            return result;
        }
        public bool GetPredictionTimeStatus(string Key, ref int _PredictionProgress)
        {
            bool AdvanceStr = false;
            if (GetConnByDekVisTmp.IndexOf("DetaVisHor") < 0)
            {
                List<string> PredictionTimeInf = new List<string>();
                AdvanceStr = false;
                string Condition = "排程編號=" + "'" + Key + "'";
                DataTableUtils.Conn_String = GetConnByDekVisTmp;
                // DataTable dt = DataTableUtils.DataTable_GetTable("select 工作站型態資料表.工作站名稱,工作站型態資料表.工藝工程編號,執行工藝,實際啟動時間,實際完成時間,組裝累積時間,派工狀態,工作站狀態資料表.工作站編號 from " + ShareMemory.SQLAsm_WorkStation_State + " INNER JOIN 工作站型態資料表 ON 工作站狀態資料表.工作站編號 = 工作站型態資料表.工作站編號 " + Condition);
                DataRow dr_status = DataTableUtils.DataTable_GetDataRow("select 工作站型態資料表.工作站名稱,工作站型態資料表.工藝流程點編號,執行工藝,派工狀態,工作站狀態資料表.工作站編號 ,進度,狀態,實際啟動時間,實際完成時間,組裝累積時間 from " + ShareMemory.SQLAsm_WorkStation_State + " INNER JOIN 工作站型態資料表 ON 工作站狀態資料表.工作站編號 = 工作站型態資料表.工作站編號 " + " where " + Condition);
                DataRow Dr_Craft = null;
                DateTime PredictionTime;
                DateTime FinishTime;
                DateTime StartTime;
                DateTime RestTime;
                DateTime Result_StartTime;
                double ProcessSec;
                TimeSpan ts;
                //"工藝名稱", 
                if (dr_status != null)
                {
                    //"標準工時", 
                    Condition = "工作流程序號=" + "'" + dr_status["工藝流程點編號"].ToString() + "'";

                    Dr_Craft = DataTableUtils.DataTable_GetDataRow("select 工藝流程點資料表.工作流程名稱,工藝名稱資料表.最大工時,工藝名稱資料表.最小工時,工藝名稱資料表.目前工時,工藝名稱資料表.標準工時 from " + ShareMemory.SQLAsm_WorkPoint + " INNER JOIN 工藝名稱資料表 ON 工藝流程點資料表.工作流程名稱 = 工藝名稱資料表.工藝名稱 " + " where " + Condition);

                    //Dr_Craft = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLAsm_WorkCraft_Name, Condition);
                    if (Dr_Craft != null)
                    {
                        //DateTime.ParseExact(DataTableUtils.toString(dr_status["實際完成時間"]), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString("dd日HH時mm分ss秒");
                        //  GetPredictionTime(dr_status["啟動時間"].ToString(), Dr_Craft["標準工時"].ToString(), "早班", WorkType.人).ToString("dd日HH時mm分ss秒");
                        //"實際完成", 
                        if (dr_status["實際完成時間"].ToString() != null && dr_status["實際完成時間"].ToString() != "" && dr_status["實際完成時間"].ToString().ToUpper() != "NULL")
                        {
                            FinishTime = DateTime.ParseExact(DataTableUtils.toString(dr_status["實際完成時間"]), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                            PredictionTime = GetPredictionTime(dr_status["實際啟動時間"].ToString(), Dr_Craft["目前工時"].ToString(), "早班", WorkType.人);
                            _PredictionProgress = 100;
                            //PredictionTime = GetPredictionTime(dr_status["啟動時間"].ToString(), Dr_Craft["標準工時"].ToString(), "早班", WorkType.人);
                            if (FinishTime < PredictionTime)
                                return AdvanceStr = true;

                        }
                        else
                        {
                            if (DateTime.TryParseExact(dr_status["實際啟動時間"].ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal, out Result_StartTime))
                            {
                                StartTime = Result_StartTime;
                                //StartTime = DateTime.ParseExact(dr_status["實際啟動時間"].ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                RestTime = DateTime.ParseExact(dr_status["實際啟動時間"].ToString().Substring(0, 8) + "120000", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                //if ((StartTime < RestTime && DateTime.Now < RestTime) || (StartTime > RestTime && DateTime.Now > RestTime))//morning to  today morning or  afternoon to  today afternoon
                                //    ts = DateTime.Now - StartTime;
                                //else 
                                if ((StartTime < RestTime && DateTime.Now < RestTime) || (StartTime > RestTime && DateTime.Now > RestTime))// //morning to  morning  or afternoon to  afternoon
                                    ts = DateTime.Now - StartTime;
                                else if (StartTime < RestTime && DateTime.Now > RestTime)// //morning to  afternoon
                                    ts = DateTime.Now.AddHours(-1) - StartTime;
                                else if ((StartTime < RestTime && DateTime.Now < RestTime.AddDays(1)) || (StartTime > RestTime && DateTime.Now > RestTime.AddDays(1)))//morning to  tomorrow morning or afternoon to  tomorrow afternoon
                                    ts = DateTime.Now.AddHours(-16) - StartTime;
                                else if (StartTime > RestTime && DateTime.Now < RestTime.AddDays(1)) //afternoon to  tomorrow morning
                                    ts = DateTime.Now.AddHours(-15) - StartTime;
                                else if (StartTime < RestTime && DateTime.Now > RestTime.AddDays(1))///morning to  tomorrow afternoon
                                    ts = DateTime.Now.AddHours(-17) - StartTime;
                                else
                                    ts = DateTime.Now - StartTime;
                                if (DataTableUtils.toInt(Dr_Craft["目前工時"].ToString()) != 0)
                                {
                                    ProcessSec = ts.TotalSeconds / DataTableUtils.toDouble(Dr_Craft["目前工時"].ToString()) * 100;
                                    if ((int)ProcessSec <= 100)
                                        _PredictionProgress = (int)ProcessSec;
                                    else
                                        _PredictionProgress = 100;
                                    PredictionTimeInf.Add(ProcessSec.ToString());
                                    if (ProcessSec < DataTableUtils.toInt(dr_status["進度"].ToString()))
                                        return AdvanceStr = true;
                                }
                            }
                        }
                    }
                }
            }

            return AdvanceStr;
        }
        public static void LineNote(string post, int LineNum, string PK, string ErrorMsgType, string ErrorMsg, string ConnByDekVisTmp, string MantID = "", string backman = "", string responedep = "", string close_content = "")
        {
            GlobalVar.UseDB_setConnString(ConnByDekVisTmp);

            string IP_Port = WebUtils.GetAppSettings("Line_port");
            //Get Group and Token
            string CCSStr = "";
            string sql = $"select 群組序號,工作站編號,工作站異常通知群組資料表.群組編號,工作站名稱,工作站異常通知資料表.群組編碼 " +
                            $"from {ShareMemory.SQLAsm_WorkStation_ErrorRingGroup} " +
                         $"INNER JOIN 工作站異常通知資料表 on 工作站異常通知群組資料表.群組編號 = 工作站異常通知資料表.群組編號";
            string condition = $" where 工作站編號='{LineNum}'";
            DataRow dr_ccs;
            DataRow dr_line = DataTableUtils.DataTable_GetDataRow(sql + condition);
            DataRow dr_line_Name = DataTableUtils.DataTable_GetDataRow($"select 工作站名稱 from {ShareMemory.SQLAsm_WorkStation_Type} {condition}");


            CCSStr = WebUtils.GetAppSettings("RowData_Cloumns");
            string sqlcmds = $"select {CCSStr} from {ShareMemory.SQLAsm_RowsData} where 排程編號='{PK}'";
            dr_ccs = DataTableUtils.DataTable_GetDataRow(sqlcmds);



            //發送EMAIL
            try
            {
                if(WebUtils.GetAppSettings("email_account") != "")
                {
                    string url = $"ErrorID={PK},ErrorLineNum={LineNum},ErrorLineName={dr_line_Name["工作站名稱"]},MantID={MantID}";
                    url = $"{WebUtils.GetAppSettings("Line_ip")}:{IP_Port}/pages/dp_PM/Asm_ErrorDetail.aspx?key={WebUtils.UrlStringEncode(url)}";
                    ErrorMsg = ErrorMsg.Replace('&', '，').Replace('+', '，').Replace('<', '，').Replace('>', '，').Replace('"', '，').Replace("'", "，").Replace('%', '，');
                    Post_Email(ErrorMsg, $"[機種名稱]:{dr_line_Name["工作站名稱"]} <br/>[回覆人員]:{backman} <br/> [排程編號]:{PK} <br/>  [排程品號]:{dr_ccs[CCSStr]} <br/> [責任歸屬]:{responedep} <br/> [狀態]:處理中 <br/> [異常內容]:{ErrorMsg} <br/> [連結]:<a href=\"{url}\">請點此</a>", responedep);
                }

            }
            catch
            {

            }

            //發送TELEGRAM
            try
            {
                if (WebUtils.GetAppSettings("telegram_account") != "")
                {
                    string url = $"ErrorID={PK},ErrorLineNum={LineNum},ErrorLineName={dr_line_Name["工作站名稱"]},MantID={MantID}";
                    url = $"{WebUtils.GetAppSettings("Line_ip")}:{IP_Port}/pages/dp_PM/Asm_ErrorDetail.aspx?key={WebUtils.UrlStringEncode(url)}";
                    Post_Telegram($"\r\n[機種名稱]:{dr_line_Name["工作站名稱"]}\r\n[回覆人員]:{backman}\r\n[排程編號]:{PK}\r\n[排程品號]:{dr_ccs[CCSStr]}\r\n[責任歸屬]:{responedep}\r\n[狀態]:處理中\r\n[異常內容]:{ErrorMsg}\r\n[連結]:{url}");
                }

            }
            catch
            {

            }

            //發送LINE
            try
            {

                if (post == "1")
                {

                    string url = $"ErrorID={PK},ErrorLineNum={LineNum},ErrorLineName={dr_line_Name["工作站名稱"]},MantID={MantID}";
                    url = $"{WebUtils.GetAppSettings("Line_ip")}:{IP_Port}/pages/dp_PM/Asm_ErrorDetail.aspx?key={WebUtils.UrlStringEncode(url)}";
                    ErrorMsg = ErrorMsg.Replace('&', '，').Replace('+', '，').Replace('<', '，').Replace('>', '，').Replace('"', '，').Replace("'", "，").Replace('%', '，');
                    if (close_content == "" && ErrorMsgType != "")
                        lineNotify(dr_line["群組編碼"].ToString(), $"\r\n[機種名稱]:{dr_line_Name["工作站名稱"]}\r\n[回覆人員]:{backman}\r\n[排程編號]:{PK}\r\n[排程品號]:{dr_ccs[CCSStr]}\r\n[責任歸屬]:{responedep}\r\n[狀態]:處理中\r\n[異常內容]:{ErrorMsg}\r\n[連結]:{url}");
                    else if (close_content == "" && ErrorMsgType == "")
                        lineNotify(dr_line["群組編碼"].ToString(), $"\r\n[機種名稱]:{dr_line_Name["工作站名稱"]}\r\n[回覆人員]:{backman}\r\n[排程編號]:{PK}\r\n[排程品號]:{dr_ccs[CCSStr]}\r\n[責任歸屬]:{responedep}\r\n[狀態]:處理中\r\n[回復內容]:{ErrorMsg}\r\n[連結]:{url}");
                    else
                        lineNotify(dr_line["群組編碼"].ToString(), $"\r\n[機種名稱]:{dr_line_Name["工作站名稱"]}\r\n[回覆人員]:{backman}\r\n[排程編號]:{PK}\r\n[排程品號]:{dr_ccs[CCSStr]}\r\n[責任歸屬]:{responedep}\r\n[狀態]:結案\r\n[結案內容]:{close_content}\r\n[連結]:{url}");

                }
            }
            catch
            {

            }
        }

        public static void lineNotify(string token, string msg)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", msg);
                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + token);

                using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void Post_Email(string title, string content, string department)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = $"select USER_EMAIL from department,users where department.DPM_NAME = users.USER_DPM and DPM_NAME2 = '{department}' and length(USER_EMAIL)>0";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                MailMessage mail = new MailMessage();
                //前面是發信email後面是顯示的名稱
                mail.From = new MailAddress(WebUtils.GetAppSettings("email_account"), $"{WebUtils.GetAppSettings("Company_Name")}組裝異常");

                foreach (DataRow row in dt.Rows)
                    mail.To.Add(row["USER_EMAIL"].ToString());

                //設定優先權
                mail.Priority = MailPriority.Normal;

                //標題
                mail.Subject = title;

                //內容
                mail.Body = $"<h1>{content}</h1>";

                //內容使用html
                mail.IsBodyHtml = true;

                //設定gmail的smtp (這是google的)
                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

                //您在gmail的帳號密碼
                MySmtp.Credentials = new System.Net.NetworkCredential(WebUtils.GetAppSettings("email_account"), WebUtils.GetAppSettings("email_password"));

                //開啟ssl
                MySmtp.EnableSsl = true;

                //發送郵件
                MySmtp.Send(mail);

                //放掉宣告出來的MySmtp
                MySmtp = null;

                //放掉宣告出來的mail
                mail.Dispose();
            }

        }
        public static void Post_Telegram(string message)
        {
            TelegramBotClient tg = new TelegramBotClient(WebUtils.GetAppSettings("telegram_account"));
            long chanelId = DataTableUtils.toLong(WebUtils.GetAppSettings("telegram_password"));//chneel dekMsgtestNew
            var r = tg.SendTextMessageAsync(chanelId, message).Result;
        }

        public string ErrorDetailDeleteProcess(string ErrorNum, string acc, string judge = "")
        {
            // check acc can delete this Mant rec
            string deleteFailNum = "";
            string message = "";
            int LineNum = 0;
            DataTable dr_mant = ErrorDetail_CheckIdMappping(ErrorNum, acc);
            if (dr_mant != null)
            {
                // delete rec 
                if (!ErrorDetailDeleteActive(dr_mant, ref deleteFailNum))
                    message = "刪除維護編號" + deleteFailNum + "失敗!";
                if (dr_mant.Rows[0]["父編號"].ToString() == null && dr_mant.Rows[0]["父編號"].ToString() == "0")
                {
                    // updata winForm && webform Vis 
                    LineNum = reCorrectMantToStationStatus(dr_mant.Rows[0][ShareMemory.PrimaryKey].ToString());
                    // updata machine_ID
                    Set_MachineID_Line_Updata(LineNum.ToString());
                }
                message = "維護編號" + ErrorNum + "刪除完成!";
            }
            else
            {
                message = "不能刪除非該帳號建立的維護訊息!";
            }
            return message;
        }
        public static DateTime StrToDate(string _date)
        {
            try
            {
                if (_date != null && _date.Length < 14 && _date.Substring(0, 1) != "0")
                    _date = _date + "080000";
            }
            catch
            {
                _date = "00000000000000";
            }
            DateTime Trs;
            Trs = StrToDateTime(_date, "yyyyMMddHHmmss");

            return Trs;
        }
        public static DateTime StrToDateTime(string time, string Sourceformat)
        {
            try
            {
                return DateTime.ParseExact(time, Sourceformat, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                return new DateTime();
            }
        }
        public static string GetTimeStamp(DateTime time)
        {
            //  DateTime time = DateTime.Now;
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString();
        }
        private static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        //==========================private==============================================
        private DataTable ErrorDetail_CheckIdMappping(string ErrorNum, string acc)
        {
            string Acc_Name = "";
            string[] ErrorNumAry = ErrorNum.Split(',');
            DataRow dr_Acc_Name = GetAccInf(acc);
            DataTable dt_mant;
            DataView dv_mant;
            if (dr_Acc_Name != null)
            {
                Acc_Name = dr_Acc_Name["USER_NAME"].ToString();
                // linq
                // dataview
                dt_mant = GetMantDataFromNum(ErrorNumAry);
                dv_mant = new DataView(dt_mant);
                if (dr_Acc_Name["Power"].ToString().ToUpper() != "Y")
                {
                    dv_mant.RowFilter = "維護人員姓名=" + "'" + Acc_Name + "'";
                    if (dv_mant.Count == 0)
                        return null;
                    else
                        return dt_mant;
                }
                else
                    return dt_mant;
            }
            return null;
        }
        private bool ErrorDetailDeleteActive(DataTable dt_mant, ref string num)
        {
            bool OK = false;
            foreach (DataRow dr in dt_mant.Rows)
            {
                if (DataTableUtils.Delete_Record(ShareMemory.SQLAsm_WorkStation_ErrorMant, "異常維護編號=" + "'" + dr["異常維護編號"].ToString() + "'"))
                    OK = true;
                else
                {
                    num = dr["異常維護編號"].ToString();
                    return false;
                }
            }
            return OK;
        }
        private int reCorrectMantToStationStatus(string key)
        {
            bool ok = false;
            DataTable dt_mant = DataTableUtils.DataTable_GetTable(ShareMemory.SQLAsm_WorkStation_ErrorMant, ShareMemory.PrimaryKey + "=" + "'" + key + "'", 0, 0);
            DataView dv_mant = new DataView(dt_mant);
            dv_mant.Sort = "異常維護編號 desc";
            DataRow dr_mant = dv_mant.ToTable().Rows[0];
            //
            DataRow dr_Key = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLAsm_WorkStation_State, ShareMemory.PrimaryKey + "=" + "'" + dr_mant[ShareMemory.PrimaryKey].ToString() + "'");
            //if (dr_Key["異常"].ToString() == dr_mant["異常原因類型"].ToString()) //same type
            //{
            dr_Key["維護"] = dr_mant["維護內容"].ToString() + " " + Convert.ToDateTime(dr_mant["時間紀錄"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
            ok = DataTableUtils.Update_DataRow(ShareMemory.SQLAsm_WorkStation_State, ShareMemory.PrimaryKey + "=" + "'" + dr_mant[ShareMemory.PrimaryKey].ToString() + "'", dr_Key);
            //}
            //
            return DataTableUtils.toInt(dr_Key["工作站編號"].ToString());
        }
        private string[] GetEachPiece(DataTable dt)
        {
            string[] str = new string[7] { "0", "0", "0", "0", "0", "0", "0" };//0:all   1:finsh   2:Stop  3:all   4:td_finish   5:td_Stop  7:Data
                                                                               //全部
            DataView dt_fin = new DataView(dt);
            str[0] = dt_fin.Count.ToString();
            //完成
            dt_fin = dt.DefaultView;
            dt_fin.RowFilter = "狀態 = '完成'";
            if (dt.Rows[0]["工作站編號"].ToString() != "5" && dt.Rows[0]["工作站編號"].ToString() != "6" && dt.Rows[0]["工作站編號"].ToString() != "7")
                str[1] = dt_fin.Count.ToString();
            else // special for deta
                str[1] = " <a href = 'Asm_Cahrt_Detail.aspx?Key=Asm_LineOverView_FinishSpecialFunction,LineNum=" + dt.Rows[0]["工作站編號"].ToString() + "' >" + dt_fin.Count.ToString() + "</a>";

            //暫停
            dt_fin = dt.DefaultView;
            dt_fin.RowFilter = "狀態 = '暫停'";
            str[2] = dt_fin.Count.ToString();

            //今日全部
            dt_fin = dt.DefaultView;
            dt_fin.RowFilter = "組裝日 =" + "'" + DateTime.Now.ToString("yyyyMMdd").ToString() + "'";
            str[3] = dt_fin.Count.ToString();

            //今日完成
            dt_fin = dt.DefaultView;
            dt_fin.RowFilter = "狀態 = '完成'" + " AND " + "組裝日 =" + "'" + DateTime.Now.ToString("yyyyMMdd").ToString() + "'";
            str[4] = dt_fin.Count.ToString();

            //今日暫停
            dt_fin = dt.DefaultView;
            dt_fin.RowFilter = "狀態 = '暫停'" + " AND " + "組裝日 =" + "'" + DateTime.Now.ToString("yyyyMMdd").ToString() + "'";
            str[5] = dt_fin.Count.ToString();


            return str;
        }

        private string GetHistorySearchCondition(string key, string LineNum)
        {
            //=============
            string condition = "";
            string condition_Key = "排程編號  " + " Like " + "'" + "%" + key + "%" + "'";
            string condition_LineNum = "工作站狀態資料表.工作站編號" + " = " + "'" + LineNum + "'";
            //=============
            if (key != "" && LineNum != "0")//11
                condition = condition_Key + " AND " + condition_LineNum;
            else if (key != "" && LineNum == "0")//10
                condition = condition_Key;
            else if (key == "" && LineNum != "0")//01
                condition = condition_LineNum;
            else //if (key == "" && LineNum == "0" && ErrorType != "--Select--")//00
                condition = condition_Key;
            return condition;
        }
        private void Note_MachineID_Line_Updata(string LineNum)
        {
            bool ok = false;
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            DataTable dt = DataTableUtils.GetDataTable(ShareMemory.SQLAsm_MachineID_Line, $"機台產線代號='{LineNum}'");
            foreach (DataRow dr in dt.Rows)
            {
                if (!(dr["是否有更新資料現場"].ToString().ToUpper() == "TRUE" || dr["是否有更新資料現場"].ToString().ToUpper() == "1"))
                {
                    dr["是否有更新資料現場"] = true;
                    ok = DataTableUtils.Update_DataRow(ShareMemory.SQLAsm_MachineID_Line, $"機台編號 ='{dr["機台編號"]}'", dr);
                }
            }
        }
        private DataTable tableColumnSelectForLineDetail(DataTable src, string LineNum, string field = "", string interjoin = "")
        {
            string[] StrError;
            string CustomStr = "CUSTNM";
            DataTableUtils.Conn_String = GetConnByDekVisTmp;

            System.Data.DataView view = new System.Data.DataView(src);
            System.Data.DataTable selected = new DataTable();
            try
            {
                selected = LineNum == "" ? view.ToTable("Selected", false, "組裝日", "排程編號", "工作站編號", "進度", "異常", "預計完成日", "維護", "實際啟動時間", "問題回報") : view.ToTable("Selected", false, "組裝日", "排程編號", "進度", "異常", "預計完成日", "維護", "實際啟動時間", "問題回報");
            }
            catch
            {
                selected = view.ToTable("Selected", false, "組裝日", "排程編號", "進度", "異常", "維護", "實際啟動時間", "問題回報");
            }
            DataTable tmp;
            string Condition1 = "";
            string Condition2 = "";
            string Condition3 = "";
            string LineNumber = LineNum == "" ? "" : $" 工作站編號 = {LineNum} AND ";
            if (field == "")
            {
                Condition1 = $" {LineNumber} 實際組裝時間 ={DateTime.Now:yyyyMMdd}";
                Condition2 = $" {LineNumber} 實際組裝時間 <={DateTime.Now:yyyyMMdd} AND 狀態!='完成'";
                Condition3 = $"OR {LineNumber} substring(實際完成時間,1,8)  = {DateTime.Now:yyyyMMdd}";
            }
            else
            {
                Condition1 = $"工作站狀態資料表.排程編號 = '{LineNum}' AND 實際組裝時間 ={DateTime.Now:yyyyMMdd}";
                Condition2 = $"工作站狀態資料表.排程編號 = '{LineNum}' AND 實際組裝時間 <={DateTime.Now:yyyyMMdd}";
                Condition3 = "";
            }
            string Condition = $"{Condition1} OR {Condition2} {Condition3}";
            GlobalVar.UseDB_setConnString(GetConnByDekVisTmp);
            DataTable Cust_Name = DataTableUtils.GetDataTable($"SELECT 工作站狀態資料表.排程編號,組裝資料表.CUSTNM FROM 工作站狀態資料表  INNER JOIN 組裝資料表 ON 工作站狀態資料表.排程編號 = 組裝資料表.排程編號 where {Condition}");
            if (Cust_Name == null)
            {
                GlobalVar.UseDB_setConnString(GetConnByDekVisTmp);
                Cust_Name = DataTableUtils.GetDataTable($"SELECT 工作站狀態資料表.排程編號,組裝資料表.客戶 {field}  FROM 工作站狀態資料表  INNER JOIN 組裝資料表 ON 工作站狀態資料表.排程編號 = 組裝資料表.排程編號  {interjoin}  where {Condition} order by 工作站狀態資料表.工作站編號 desc");
                CustomStr = "客戶";
            }
            DataView view_Name = new DataView(Cust_Name);

            selected.Columns.Add("狀態");
            selected.Columns.Add("客戶");
            selected.Columns["狀態"].SetOrdinal(3);//series
            selected.Columns["客戶"].SetOrdinal(1);
            if (field != "")
            {
                selected.Columns.Add("工作站名稱");
                selected.Columns["工作站名稱"].SetOrdinal(1);
            }

            try
            {
                for (int i = 0; i < selected.Rows.Count; i++)
                {
                    view_Name = Cust_Name.DefaultView;
                    view_Name.RowFilter = $"排程編號 = '{selected.Rows[i]["排程編號"]}'";
                    tmp = view_Name.ToTable();
                    selected.Rows[i]["客戶"] = tmp.Rows.Count > 0 ? tmp.Rows[0][CustomStr].ToString() : "";
                    if (field != "")
                        selected.Rows[i]["工作站名稱"] = tmp.Rows[i]["工作站名稱"].ToString();
                    if (selected.Rows[i]["異常"].ToString() != "")
                    {
                        StrError = selected.Rows[i]["維護"].ToString().Split(' ');
                        foreach (string Er in StrError)
                            selected.Rows[i]["異常"] += "," + Er;
                    }
                }
            }
            catch
            {
                return selected;
            }
            try
            {
                selected.Columns["預計完成日"].SetOrdinal(5);

            }
            catch
            {

            }

            selected.Columns.Remove("維護");
            return selected;
        }

        //這裡要修改
        private DataTable EtableColumnSelectForLineDetail(DataTable src)
        {
            System.Data.DataView view = new System.Data.DataView(src);
            view.Sort = "時間紀錄 asc";
            System.Data.DataTable selected = new DataTable();
            try
            {
                selected = view.ToTable("Selected", false, "異常維護編號", "時間紀錄", "維護人員姓名", "維護人員單位", "異常原因類型", "維護內容", "處理狀態", "圖片檔名", "結案判定類型", "責任單位", "是否持續顯示異常");
            }
            catch (Exception ex)
            {
                string esc = ex.Message;
            }
            return selected;
        }

        private DataTable tableColumnSelectForHistorySearchList(DataTable src)
        {
            System.Data.DataView view = new System.Data.DataView(src);
            //System.Data.DataTable selected = view.ToTable("Selected", false, "排程編號", "時間紀錄", "維護人員姓名", "維護人員單位", "異常原因類型", "維護內容", "處理狀態");
            System.Data.DataTable selected = view.ToTable("Selected", false, "排程編號", "工作站名稱", "實際啟動時間", "實際完成時間", "組裝累積時間", "工作站編號");
            return selected;
        }
        private DataTable tableColumnSelectForTotalLine(DataTable src)
        {
            List<string> list = new List<string>();
            int x, y;
            System.Data.DataView view = new System.Data.DataView(src);
            System.Data.DataTable selected = view.ToTable("Selected", false, "工作站名稱", "目標件數", "工作站是否使用中", "工作站編號", "人數配置");
            //add Finish Count
            selected.Columns.Add("生產中");
            selected.Columns.Add("完成");
            selected.Columns.Add("暫停");
            selected.Columns.Add("今日生產中");
            selected.Columns.Add("今日完成");
            selected.Columns.Add("今日暫停");
            for (int i = 0; i < selected.Rows.Count; i++)
            {
                list.Clear();
                x = 0;
                y = 0;
                DataTable dt = Get_Status(DataTableUtils.toInt(selected.Rows[i]["工作站編號"].ToString()));
                if (HtmlUtil.Check_DataTable(dt))
                {
                    selected.Rows[i]["生產中"] = DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["生產中"]));
                    selected.Rows[i]["完成"] = DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["完成"]));
                }
                else
                {
                    selected.Rows[i]["生產中"] = 0;
                    selected.Rows[i]["完成"] = 0;
                }
                selected.Rows[i]["今日生產中"] = 0;
                selected.Rows[i]["今日完成"] = 0;
                selected.Rows[i]["今日暫停"] = 0;

                //計算裡面未解決的案件
                list = Calculation_Alarm_Or_Behind(selected.Rows[i]["工作站編號"].ToString(), ref x, ref y, true);
                selected.Rows[i]["暫停"] = list.Count.ToString();
            }
            return selected;
        }

        private DataTable Get_Status(int LineNum)
        {
            //GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $"SELECT distinct (select count(*) from 工作站狀態資料表  where (工作站編號 = '{LineNum}' AND 實際組裝時間 = '{DateTime.Now:yyyyMMdd}') OR (工作站編號 = '{LineNum}' AND 實際組裝時間 <= '{DateTime.Now:yyyyMMdd}' AND 狀態 != '完成') OR (工作站編號 = '{LineNum}' AND 實際完成時間 >= '{DateTime.Now:yyyyMMdd}000000' AND 實際完成時間 <= '{DateTime.Now:yyyyMMdd}235959' AND 狀態 = '完成'))  生產中, (select count(*) from 工作站狀態資料表 where 工作站編號 = '{LineNum}' AND 實際完成時間 >= '{DateTime.Now:yyyyMMdd}000000' AND 實際完成時間 <= '{DateTime.Now:yyyyMMdd}235959' AND 狀態 = '完成') 完成 FROM 工作站狀態資料表";
            return clsdb.GetDataTable(sqlcmd);
        }

        private int GetLinePieceCount(int LineNum, string Status, string td = null)
        {
            string Condition1 = "";
            string Condition2 = "";
            string Condition3 = "";
            string Condition = "";

            string CountCondition = "";
            Condition1 = $"工作站編號 = '{LineNum}' AND 實際組裝時間 ='{DateTime.Now:yyyyMMdd}' ";
            Condition2 = $"工作站編號 = '{LineNum}' AND 實際組裝時間 <='{DateTime.Now:yyyyMMdd}'  AND 狀態!='完成'";
            Condition3 = $"工作站編號 = '{LineNum}' AND 實際完成時間>='{DateTime.Now:yyyyMMdd}000000' AND 實際完成時間 <='{DateTime.Now:yyyyMMdd}235959'  AND 狀態 ='完成'";
            if (td != "td")
                Condition = $"({Condition1}) OR ({Condition2}) OR ({Condition3})";
            else
                Condition = $"工作站編號 = '{LineNum}' AND 組裝日 ='{DateTime.Now:yyyyMMdd}' ";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            DataTable dt = DataTableUtils.DataTable_GetTable("工作站狀態資料表", Condition);
            DataView dV_Status_Count = dt.DefaultView;
            if (Status == "產線" || (dt != null && dt.Rows.Count == 0))
                return dt.Rows.Count;
            else
            {
                dV_Status_Count = dt.DefaultView;
                CountCondition = $"狀態 Like '%{Status}%'";
                dV_Status_Count.RowFilter = CountCondition;
                return dV_Status_Count.Count;
            }
        }
        private DateTime GetPredictionTime(string StartTimeStr, string StandardTime, string WorkClass, WorkType _WorkType)
        {
            DateTime PredictionTime;
            DataTableUtils.Conn_String = GetConnByDekVisTmp;
            try
            {
                DataRow dr_class = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLAsm_WorkClass, "班次名稱=" + "'" + WorkClass + "'");
                string EndTime = dr_class["結束時間"].ToString();
                DateTime StartTime = DateTime.ParseExact(StartTimeStr, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                DateTime EarlyClassTime = DateTime.ParseExact(StartTimeStr.Substring(0, 8) + dr_class["結束時間"].ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                DateTime RestTime = DateTime.ParseExact(StartTimeStr.Substring(0, 8) + "120000", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                PredictionTime = StartTime.AddSeconds(DataTableUtils.toDouble(StandardTime));//StartTime+CraftStandardTime
                                                                                             //can not use mix work  class (human or work)
                if (StartTime < RestTime && PredictionTime < RestTime && PredictionTime < EarlyClassTime)//morning && not cross RestTime && corss early class
                    return PredictionTime;
                else if (StartTime < RestTime && PredictionTime > RestTime && PredictionTime < EarlyClassTime)//morning && not cross RestTime && corss early class
                {
                    if (PredictionTime.AddHours(1) > EarlyClassTime)//cross early class
                        return PredictionTime.AddHours(16);
                    else
                        return PredictionTime.AddHours(1);
                }
                else if (StartTime < RestTime && PredictionTime > RestTime && PredictionTime > EarlyClassTime)//morning && not cross RestTime && corss early class
                {
                    if (PredictionTime.AddHours(15) > RestTime.AddDays(1)) //cross tomorrow restTime
                        return PredictionTime.AddHours(17);
                    else
                        return PredictionTime.AddHours(16);
                }
                else if (StartTime > RestTime && PredictionTime < EarlyClassTime)//afternoon && not corss early class
                    return PredictionTime;
                else if (StartTime > RestTime && PredictionTime > EarlyClassTime)//afternoon && corss early class
                {
                    if (PredictionTime.AddHours(15) > RestTime.AddDays(1)) //cross tomorrow restTime
                        return PredictionTime.AddHours(16);
                    else
                        return PredictionTime.AddHours(15);
                }
                else
                    return PredictionTime;
            }
            catch
            {
                return DateTime.Now;
            }
        }
        private DataTable GetMantDataFromNum(string[] Mant_Num)
        {
            string condition = "";
            foreach (string num in Mant_Num)
            {
                if (num != Mant_Num.Last())
                    condition += "異常維護編號=" + "'" + num + "'" + " OR ";
                else
                    condition += "異常維護編號=" + "'" + num + "'";
            }
            DataTable dt_mant = DataTableUtils.DataTable_GetTable(ShareMemory.SQLAsm_WorkStation_ErrorMant, condition, 0, 0);
            return dt_mant;
        }
        static bool IsHoliday(DateTime dt)
        {
            ////判斷傳入的日期dt 是否為假日
            //// 若是，傳回true
            //DayOfWeek week = dt.DayOfWeek;
            //if (week == DayOfWeek.Saturday || week == DayOfWeek.Sunday)
            //    //if (dt_holiday.AsEnumerable().Where(d => d.Field<string>("PK_Holiday") == dt.ToString("yyyyMMdd")).Count() != 0)
            //    return true;
            //foreach (DateTime date in holiday_list)
            //    if (date.Date == dt.Date) return true;
            //return false;
            //判斷傳入的日期dt 是否為假日
            // 若是，傳回true
            // DataTable dt_holiday = DataTableUtils.DataTable_GetTable("WorkTime_Holiday", "");

            if (dt_holiday != null && dt_holiday.Rows.Count > 0)
            {
                if (dt_holiday.AsEnumerable().Where(d => d.Field<string>("PK_Holiday") == dt.ToString("yyyyMMdd")).Count() != 0)
                    return true;
                foreach (DateTime date in holiday_list)
                    if (date.Date == dt.Date) return true;
            }
            else
            {
                DayOfWeek week = dt.DayOfWeek;
                if (week == DayOfWeek.Saturday || week == DayOfWeek.Sunday)
                    return true;
            }

            return false;

        }
        public static TimeSpan WorkTimeCaculator(string startTime, string EndTime)
        {
            work.工作時段_新增(8, 0, 12, 0);
            work.工作時段_新增(13, 0, 17, 0);
            return work.工作時數(StrToDate(startTime), StrToDate(EndTime));
        }
    }
    //=====================Class==============================================================
    class LineData
    {
        public int LineId { get; set; }
        public string LineName { get; set; }
    }
    public static class DateTransfor
    {
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        public static IEnumerable<DateTime> EachMonth(DateTime from, DateTime thru)
        {
            for (var month = from.Date; month.Date <= thru.Date || month.Month == thru.Month; month = month.AddMonths(1))
                yield return month;
        }
        public static IEnumerable<DateTime> EachDayTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo);
        }
        public static IEnumerable<DateTime> EachMonthTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo);
        }
    }

    public class MyTimePeriod
    {
        public int minutes_start, // 起始時間 new-2020/01/20
                   minutes_end;   // 結束時間
        /*
                        00:00          10:15             24:00  
                          [--------------------------------]
         以分為計算單位: 0*60+0--------10*60+15----------24*60+0
        */
        public MyTimePeriod()
        {
            minutes_start = minutes_end = 0;
        }
        public MyTimePeriod(int start_hour, int start_minute, int end_hour, int end_minute)
        {
            SetTimePeriod(start_hour, start_minute, end_hour, end_minute);
        }

        public void SetTimePeriod(int start_hour, int start_minute, int end_hour, int end_minute)
        {
            minutes_start = start_hour * 60 + start_minute;
            minutes_end = end_hour * 60 + end_minute;
            if (minutes_start > minutes_end)
            {
                // 起始時間大於結束時間: 交換時間順序
                int t = minutes_start;
                minutes_start = minutes_end;
                minutes_end = t;
            }
        }

        void ToTimeValue(int minutes_total, out int hour, out int min)
        {
            // 將分為計算單位的值轉換為小時與分鐘
            hour = minutes_total / 60;
            min = minutes_total % 60;
        }

        public void GetStartTime(out int hour, out int min) // 取得開始時間
        {
            ToTimeValue(minutes_start, out hour, out min);
        }

        public void GetEndTime(out int hour, out int min)  // 取得結束時間
        {
            ToTimeValue(minutes_end, out hour, out min);
        }

        public override string ToString()
        {
            return ToText();
        }

        public string ToText() // 將工作時間範圍轉成文字
        {
            int min = TotalMinutes();
            return String.Format("{0:00}:{1:00}~{2:00}:{3:00}(時數={4}:{5})",
                minutes_start / 60, minutes_start % 60,
                minutes_end / 60, minutes_end % 60,
                min / 60, min % 60
                );
        }

        public int TotalMinutes() // 工作時間範圍的工作時數: 分
        {
            return minutes_end - minutes_start;
        }

        public bool IsEqual(MyTimePeriod mt)
        {
            if (mt == null) return false;
            return minutes_start == mt.minutes_start && minutes_end == mt.minutes_end;
        }

        public bool IsOverlap(MyTimePeriod mt) // 指定的時間範圍物件是否與自己有重疊時間
        {
            if (minutes_start < mt.minutes_start)
            {
                /* minutes_start  minutes_end 
                     [-------------]
                 mt:   [---]
                          [-----------]
                                     [-------]  X
                 */
                if (minutes_end <= mt.minutes_start) return false;
            }
            else if (minutes_start > mt.minutes_start)
            {
                /* minutes_start   minutes_end 
                             [-------------]
                 mt:   [---] x
                          [-----------]
                                     [-------]
                 */
                if (mt.minutes_end <= minutes_start) return false;
            }
            return true;
        }

    }

    //----------------------------------------------------------
    public class MyWorkTime
    {
        public List<MyTimePeriod> WorkHoursList = new List<MyTimePeriod>();
        public int DayWork_TotalMinutes = 24 * 60; // default 24 hours
        Func<DateTime, bool> IsHoliday;
        public MyWorkTime(Func<DateTime, bool> IsHolidayFunction)
        {
            IsHoliday = IsHolidayFunction;
        }

        int 工作時段_compare(MyTimePeriod mtp1, MyTimePeriod mtp2)
        {
            return mtp1.minutes_start - mtp2.minutes_start;
        }
        public bool 工作時段_新增(MyTimePeriod mt)
        {
            int total_min = 0;
            foreach (MyTimePeriod cur_mt in WorkHoursList)
            {
                if (cur_mt.IsOverlap(mt)) return false;
                total_min += cur_mt.TotalMinutes();
            }
            DayWork_TotalMinutes = total_min + mt.TotalMinutes();
            WorkHoursList.Add(mt);
            if (WorkHoursList.Count > 1)
                WorkHoursList.Sort(工作時段_compare);
            return true;
        }

        public bool 工作時段_新增(int start_hour, int start_min, int end_hour, int end_min)
        {
            return 工作時段_新增(new MyTimePeriod(start_hour, start_min, end_hour, end_min));
        }

        public bool 工作時段_刪除(MyTimePeriod mt)
        {
            foreach (MyTimePeriod cur_mt in WorkHoursList)
            {
                if (cur_mt.IsEqual(mt))
                {
                    DayWork_TotalMinutes -= cur_mt.TotalMinutes();
                    WorkHoursList.Remove(cur_mt);
                    return true;
                }
            }
            return false;
        }

        public bool 工作時段_刪除(int start_hour, int start_min, int end_hour, int end_min)
        {
            return 工作時段_刪除(new MyTimePeriod(start_hour, start_min, end_hour, end_min));
        }

        public string 工作時段_ToText()
        {
            string text = "";
            foreach (MyTimePeriod period in WorkHoursList)
            {
                if (text != "") text += ",";
                text += period.ToText();
            }
            return text + string.Format(" 全日工時={0:00}:{1:00}", DayWork_TotalMinutes / 60, DayWork_TotalMinutes % 60);
        }

        public TimeSpan 工作時段_工時(MyTimePeriod mtp)
        {
            int minutes = 0;
            if (WorkHoursList.Count <= 0)
                minutes = mtp.TotalMinutes();
            else
            {
                foreach (MyTimePeriod cur_mtp in WorkHoursList)
                {
                    if (cur_mtp.IsOverlap(mtp) != true) continue;
                    if (mtp.minutes_end <= cur_mtp.minutes_end)
                    {
                        /*
                        cur_mtp:   [-------------]
                        mtp:   [--------------]
                                   [----------]
                            ===>   [----------]
                        */
                        if (mtp.minutes_start < cur_mtp.minutes_start)
                            mtp.minutes_start = cur_mtp.minutes_start;
                        minutes += mtp.TotalMinutes();
                        break;
                    }
                    else
                    {
                        /*
                        cur_mtp:   [-------------]
                        mtp:   [-----------------------]
                                      [----------------]
                            ====>  [-------------]
                                               + [-----]  
                        */
                        if (mtp.minutes_start < cur_mtp.minutes_start)
                            mtp.minutes_start = cur_mtp.minutes_start;
                        int temp = mtp.minutes_end;
                        mtp.minutes_end = cur_mtp.minutes_end;
                        minutes += mtp.TotalMinutes();
                        mtp.minutes_start = cur_mtp.minutes_end;
                        mtp.minutes_end = temp;
                    }
                }
            }
            return new TimeSpan(0, minutes, 0);
        }
        public TimeSpan 工作時數(DateTime start_dt, DateTime end_dt)
        {
            bool is_minus = false;
            if (start_dt > end_dt)
            {
                DateTime dt = start_dt;
                start_dt = end_dt;
                end_dt = dt;
                is_minus = true;
            }
            TimeSpan total_time = new TimeSpan(0, 0, 0);
            DateTime cur_dt = start_dt;
            MyTimePeriod mtp = new MyTimePeriod();
            //-------------------------------
            if (IsHoliday(cur_dt) != true) // 起始日期
            {
                int end_hour = 24, end_min = 0;
                if (cur_dt.Date == end_dt.Date)
                {
                    end_hour = end_dt.Hour;
                    end_min = end_dt.Minute;
                }
                mtp.SetTimePeriod(cur_dt.Hour, cur_dt.Minute, end_hour, end_min);
                total_time += 工作時段_工時(mtp);
            }
            while (true)
            {
                cur_dt = cur_dt.AddDays(1);
                if (cur_dt.Date >= end_dt.Date) break;
                if (IsHoliday(cur_dt) != true)
                    total_time += new TimeSpan(0, DayWork_TotalMinutes, 0);
            }
            if (cur_dt.Day == end_dt.Day && IsHoliday(cur_dt) != true) // 結束日期
            {
                mtp.SetTimePeriod(0, 0, end_dt.Hour, end_dt.Minute);
                total_time += 工作時段_工時(mtp);
            }
            //-------------------------------
            if (is_minus) total_time = -total_time;
            return total_time;
        }

        public DateTime 目標日期(DateTime cur_date, TimeSpan span)
        {
            int min_total = (int)span.TotalMinutes;
            if (WorkHoursList.Count <= 0) // 沒有設定工作時間 ==> 24hours
            {
                while (min_total > 0)
                {
                    if (IsHoliday(cur_date) != true)
                    {
                        MyTimePeriod mtp = new MyTimePeriod(cur_date.Hour, cur_date.Minute, 24, 0);
                        int today_min = mtp.TotalMinutes();
                        if (min_total <= today_min)
                        {
                            cur_date = cur_date.AddMinutes(min_total);
                            break;
                        }
                        else
                        {
                            min_total -= today_min;
                        }
                    }
                    cur_date = new DateTime(cur_date.Year, cur_date.Month, cur_date.Day,
                                        0, 0, 0).AddDays(1);
                }
            }
            else
            {
                int hour, min, min_period;
                while (min_total > 0)
                {
                    if (IsHoliday(cur_date) != true)
                    {
                        MyTimePeriod mtp = new MyTimePeriod(cur_date.Hour, cur_date.Minute, 24, 0);
                        foreach (MyTimePeriod period in WorkHoursList)
                        {
                            if (period.IsOverlap(mtp) != true) continue;
                            min_period = period.TotalMinutes();
                            if (min_total >= min_period)
                            {
                                period.GetEndTime(out hour, out min);
                                mtp.SetTimePeriod(cur_date.Hour, cur_date.Minute, hour, min);
                                /*
                                  period_min        ---------
                                  min_total:        ---------------
                                                 HH:MM              24:00 
                                  mtp:             [------------------]
                                                startTime    EndTime
                                  period:          [-----------]
                                  mtp:       1. [--------------] 
                                             ==>    -----------     period_min
                                             2.       [--------] 
                                             ==>       --------     period_min = mtp.TotalMinutes();

                                */
                                // case 2 ??
                                if (mtp.minutes_start > period.minutes_start)
                                    min_period = mtp.TotalMinutes();
                                if (hour == 24)  // a new date ??
                                {
                                    min_total -= min_period;
                                    break;
                                }
                                cur_date = new DateTime(cur_date.Year, cur_date.Month, cur_date.Day,
                                    hour, min, 0);
                            }
                            else
                            {
                                /*
                                  period_min   ----------------
                                  min_total    ----------
                                                startTime    EndTime
                                  period:          [-----------] 24:00
                                  mtp:     1.   [------------------]
                                                   [-----------]
                                               ==>  -----
                                           2.         [------------] 
                                                      [--------]
                                               ==>     -----
                                           3.              [-------]
                                                           [---]
                                               ==>          ---   
                                */
                                if (mtp.minutes_start < period.minutes_start)
                                    mtp.minutes_start = period.minutes_start;
                                mtp.minutes_end = period.minutes_end;
                                min_period = mtp.TotalMinutes();
                                if (min_period > min_total)
                                    min_period = min_total;
                                mtp.GetStartTime(out hour, out min);
                                cur_date = new DateTime(cur_date.Year, cur_date.Month, cur_date.Day,
                                    hour, min, 0).AddMinutes(min_period);
                            }
                            min_total -= min_period;
                            if (min_total <= 0) return cur_date;
                            mtp.SetTimePeriod(cur_date.Hour, cur_date.Minute, 24, 0);
                        }
                    }
                    cur_date = new DateTime(cur_date.Year, cur_date.Month, cur_date.Day,
                                        0, 0, 0).AddDays(1);
                }
            }
            return cur_date;
        }
    }


}