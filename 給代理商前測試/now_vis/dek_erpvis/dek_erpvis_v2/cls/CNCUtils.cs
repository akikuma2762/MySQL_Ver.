using System;
using System.Collections.Generic;
using MTLinkiDB;
using System.Linq;
using System.Web;
using Support;
using System.Data;
using System.Net;
using System.Text;
using System.IO;
using System.Timers;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace dek_erpvis_v2.cls
{
    public class CNC_Web_Data
    {
        public DataTable dt_data = null;
        public DataTable dt_data_1 = null;
        public DataTable dt_data_2 = null;
        public List<string> ls_data = new List<string>();
        public string Mysql_conn_str = myclass.GetConnByDekVisCnc_inside;
        private static Timer AlarmTimer;
        //取得目前排程資料，若沒有則表示空
        public DataTable Get_MachInfo(string Dev_Name)
        {
            List<string> list = new List<string>();
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            //Pre_dt
            dt_data_1 = DataTableUtils.GetDataTable($"select * from aps_info where mach_name = '{Dev_Name}'  and order_number > '0'  and start_time <= '{DateTime.Now:yyyyMMddHHmmss}' and end_time >= '{DateTime.Now:yyyyMMddHHmmss}' ");
            if (HtmlUtil.Check_DataTable(dt_data_1))
                return dt_data_1;
            else if (dt_data_1 == null)
                return Get_MachInfo(Dev_Name);
            else if (dt_data_1 != null && dt_data_1.Rows.Count < 1)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                dt_data_1 = DataTableUtils.GetDataTable($"select * from aps_info where mach_name = '{Dev_Name}'");
                if (HtmlUtil.Check_DataTable(dt_data_1))
                {
                    foreach (DataRow dr in dt_data_1.Rows) // search whole table
                    {
                        dr["check_staff"] = "";
                        dr["work_staff"] = "";
                        dr["manu_id"] = "";
                        dr["custom_name"] = "";
                        dr["product_name"] = "";
                        dr["product_number"] = "";
                        dr["craft_name"] = "";
                        dr["exp_product_count_day"] = "";
                        // dr["exp_product_count"] = "";
                        dr["product_count"] = "";
                        dr["product_count_day"] = "";
                        dr["product_rate_day"] = "";
                        dr["finish_time"] = "";
                        dr["complete_time"] = "";
                    }
                    return dt_data_1;
                }
                return null;
            }
            else
                return null;
        }
        public string Next_MachInfo(string Dev_Name, string order_number, string now_time = "")
        {
            //先找到當下的資料
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from aps_info where mach_name = '{Dev_Name}'";
            DataTable dt_now = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt_now))
                order_number = dt_now.Rows[0]["order_number"].ToString();
            else
                order_number = "0";

            //先找出當下的
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            sqlcmd = $"select * from aps_info where mach_name = '{Dev_Name}' and CAST(order_number AS double)  = {order_number}";
            DataTable machine_info = DataTableUtils.GetDataTable(sqlcmd);
            //找出下一筆比他大的
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            sqlcmd = $"select * from aps_list_info where mach_name = '{Dev_Name}' and CAST(order_number AS double)  > {order_number}  order by CAST(order_number AS double) asc limit 1 ";
            DataTable Next_DataTable = DataTableUtils.GetDataTable(sqlcmd);

            if (order_number == "0")
                return "工單已完結，請由可視化主控台指派";
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                sqlcmd = "select * from aps_info_report ";
                DataTable dt_report = DataTableUtils.GetDataTable(sqlcmd);

                //該排程與報工皆存在時
                if (HtmlUtil.Check_DataTable(machine_info) && dt_report != null)
                {
                    //建立例外存取之清單
                    List<string> list = new List<string>();
                    list.Add("end_time");
                    list.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //將該排程存入aps_info_report
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    bool ok = insert_Row(dt_report, machine_info, "aps_info_report", list);
                    if (HtmlUtil.Check_DataTable(Next_DataTable))
                    {
                        list.Clear();
                        //寫入派工資料表
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        sqlcmd = "select * from aps_info_dispatch";
                        DataTable dt_dispatch = DataTableUtils.GetDataTable(sqlcmd);
                        list.Add("end_time");
                        list.Add("");
                        list.Add("list_id");
                        list.Add(DataTableUtils.toString(Next_DataTable.Rows[0]["_id"]));
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        ok = insert_Row(dt_dispatch, Next_DataTable, "aps_info_dispatch", list);

                        //紀錄回報時間
                        Update_Starttime("aps_list_info", Next_DataTable);

                        Update_Starttime("aps_list_info", Next_DataTable, machine_info);

                        list.Clear();
                        list.Add("start_time");
                        list.Add(now_time);
                        list.Add("list_id");
                        list.Add(DataTableUtils.toString(Next_DataTable.Rows[0]["_id"]));
                        list.Add("end_time");
                        list.Add(HtmlUtil.StrToDate(DataTableUtils.toString(Next_DataTable.Rows[0]["end_time"])).AddYears(10).ToString("yyyyMMddHHmmss"));
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        return Update_Row(Dev_Name, machine_info, Next_DataTable, "儲存", false, list, true);
                    }
                    else
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        return Update_Row(Dev_Name, machine_info, Next_DataTable, "儲存", true);
                    }

                }
                else
                    return "儲存失敗";
            }
        }
        //修改目前排程
        public string Update_Row(string machine_name, DataTable Pre_dt, DataTable Next_dt, string action, bool null_value = false, List<string> fixed_list = null, bool check = false)
        {
            List<string> list = new List<string>();
            if (Pre_dt == null || Next_dt == null)
                return $"{action}失敗";
            else
            {
                list = Return_List(Pre_dt, Next_dt, "_id");
                DataRow row = Pre_dt.NewRow();
                row["_id"] = Pre_dt.Rows[0]["_id"];
                if (!null_value)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (fixed_list.IndexOf(list[i]) == -1)
                            row[list[i]] = Next_dt.Rows[0][list[i]];
                    }
                    if (fixed_list != null)
                    {
                        for (int i = 0; i < fixed_list.Count; i++)
                        {
                            row[fixed_list[i]] = fixed_list[i + 1];
                            i++;
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] == "order_number")
                            row[list[i]] = "0";
                        else if (fixed_list != null && fixed_list.IndexOf(list[i]) != -1)
                            row[list[i]] = fixed_list[fixed_list.IndexOf(list[i]) + 1];
                        else if (list[i] != "mach_name")
                            row[list[i]] = "";
                    }
                }
                if (check)
                    row["check_time"] = "False";

                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Update_DataRow("aps_info", $"mach_name = '{machine_name}'", row))
                    return $"已完成{action},請記得清空機台數量";
                else
                {
                    string errormsg = DataTableUtils.ErrorMessage;
                    return $"{action}失敗";
                }

            }
        }
        //新增報工跟派工資料表
        public bool insert_Row(DataTable Pre_dt, DataTable Next_dt, string Table_Name, List<string> export_list = null)
        {
            List<string> list = new List<string>();
            if (Pre_dt == null || Next_dt == null)
                return false;
            else
            {
                list = Return_List(Pre_dt, Next_dt);
                DataRow row = Pre_dt.NewRow();

                for (int i = 0; i < list.Count; i++)
                {
                    if (export_list.IndexOf(list[i]) == -1)
                        row[list[i]] = Next_dt.Rows[0][list[i]];
                }
                if (export_list != null)
                {
                    for (int i = 0; i < export_list.Count; i++)
                    {
                        row[export_list[i]] = export_list[i + 1];
                        i++;
                    }
                }
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                bool ok = DataTableUtils.Insert_DataRow(Table_Name, row);
                string ss = DataTableUtils.ErrorMessage;
                return ok;
            }
        }

        //填入回報時間
        public void Update_Starttime(string tablename, DataTable dt, DataTable dt_pre = null)
        {
            if (dt_pre == null)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                DataRow row = dt.NewRow();
                row["_id"] = dt.Rows[0]["_id"];
                row["report_start_time"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                DataTableUtils.Update_DataRow(tablename, $"_id='{dt.Rows[0]["_id"]}'", row);
            }
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                DataRow row = dt.NewRow();
                row["_id"] = dt_pre.Rows[0]["list_id"];
                row["order_status"] = "已完成報工";
                DataTableUtils.Update_DataRow(tablename, $"_id='{dt_pre.Rows[0]["list_id"]}'", row);
            }

        }
        //回傳資料表的欄位陣列
        public List<string> Return_List(DataTable dt, DataTable ds, string Ex_Column = "")
        {
            List<string> list = new List<string>();
            List<string> ex = new List<string>(Ex_Column.Split(','));
            if (dt != null && ds != null)
            {
                //抓出相同的欄位
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    for (int j = 0; j < ds.Columns.Count; j++)
                    {
                        if (DataTableUtils.toString(dt.Columns[i]) == DataTableUtils.toString(ds.Columns[j]))
                        {
                            if (ex.IndexOf(DataTableUtils.toString(dt.Columns[i])) == -1)
                                list.Add(DataTableUtils.toString(dt.Columns[i]));
                        }
                    }
                }

            }
            return list;
        }
        public string Get_CheckStaff(DataTable dt_Mach_Info)
        {
            return Aviod_NoValue(dt_Mach_Info, "check_staff");
        }
        public string Get_WorkStaff(DataTable dt_Mach_Info)
        {
            //if (HtmlUtil.Check_DataTable(dt_Mach_Info) && Aviod_NoValue(dt_Mach_Info, "work_staff") != "")
            //{
            //    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            //    string sqlcmd = $"select WorkMan from record_worktime where WORK_MACHINE = '{dt_Mach_Info.Rows[0]["mach_name"]}' and start_time = '{dt_Mach_Info.Rows[0]["start_time"]}' and end_time = '{dt_Mach_Info.Rows[0]["end_time"]}' order by NOW_TIME desc";
            //    DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            //    if (HtmlUtil.Check_DataTable(dt))
            //        return DataTableUtils.toString(dt.Rows[0]["WorkMan"]);
            //    else
            //        return Aviod_NoValue(dt_Mach_Info, "work_staff");//加工人員           
            //}
            //else
            return Aviod_NoValue(dt_Mach_Info, "work_staff");//加工人員           
        }
        public string Get_MachName(DataTable dt_Mach_Info, string machine_name)
        {
            return Aviod_NoValue(dt_Mach_Info, "mach_name", machine_name);//設備名稱           
        }
        public string Get_MachShowName(DataTable dt_Mach_Info, string machine_name)
        {
            return CNCUtils.MachName_translation(Aviod_NoValue(dt_Mach_Info, "mach_name", machine_name));//顯示名稱           
        }
        public string Get_CustomName(DataTable dt_Mach_Info)
        {
            return Aviod_NoValue(dt_Mach_Info, "custom_name");//客戶名稱         
        }
        public string Get_ManuID(DataTable dt_Mach_Info)
        {
            return Aviod_NoValue(dt_Mach_Info, "manu_id");//製令單號         
        }
        public string Get_ProductName(DataTable dt_Mach_Info)
        {
            return Aviod_NoValue(dt_Mach_Info, "product_name");//產品(料件)名稱        
        }
        public string Get_ProductNumber(DataTable dt_Mach_Info)
        {
            return Aviod_NoValue(dt_Mach_Info, "product_number");//產品(料件)編號        
        }
        public string Get_CraftName(DataTable dt_Mach_Info)
        {
            return Aviod_NoValue(dt_Mach_Info, "craft_name");//工藝名稱       
        }
        public string Get_CountTotal(DataTable dt_Mach_Info)//設備生產總數
        {
            return Aviod_NoValue(dt_Mach_Info, "product_count");
        }
        public string Get_CountToday(DataTable dt_Mach_Info)//今日生產總數
        {
            return Aviod_NoValue(dt_Mach_Info, "product_count_day");
        }
        public string Get_ExpCountToday(DataTable dt_Mach_Info)//預計今日生產總數
        {
            if (Aviod_NoValue(dt_Mach_Info, "exp_product_count_day") == "")
                return "0";
            else
                return Aviod_NoValue(dt_Mach_Info, "exp_product_count_day");
        }
        public string Get_CountTodayRate(DataTable dt_Mach_Info)//今日生產進度
        {
            if (HtmlUtil.Check_DataTable(dt_Mach_Info))
                return Aviod_NoValue(dt_Mach_Info, "product_rate_day");
            else
                return "0";
        }
        public string Get_FinishTime(DataTable dt_Mach_Info)//預計完成時間
        {
            return Aviod_NoValue(dt_Mach_Info, "finish_time");
        }
        public string Get_Operate_Rate(DataTable dt_Mach_Info, string mach = "")//今日目前稼動率
        {
            if (HtmlUtil.Check_DataTable(dt_Mach_Info))
            {
                //如果出現NaN，則顯示為0
                if (dt_Mach_Info.Rows[0]["operate_rate"].ToString() == "NaN" || dt_Mach_Info.Rows[0]["operate_rate"].ToString() == "非數值" || Double.IsInfinity(DataTableUtils.toDouble(dt_Mach_Info.Rows[0]["operate_rate"].ToString())))
                    return "0";
                else
                    return Aviod_NoValue(dt_Mach_Info, "operate_rate");
            }
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                string sqlcmd = $"select * from aps_info where mach_name =  '{mach}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                    return DataTableUtils.toString(dt.Rows[0]["operate_rate"]);
                else
                    return "0";
            }
        }
        public string Get_ProgramRun(DataTable dt_Mach_Info, string machine_name)//目前執行程式
        {
            return Aviod_NoValue(dt_Mach_Info, "prog_run", machine_name);
        }
        public string Get_ProgramMain(DataTable dt_Mach_Info)//目前主程式
        {
            return Aviod_NoValue(dt_Mach_Info, "prog_main");
        }
        public string Get_AlarmMesg(DataTable dt_Mach_Info)//目前警報
        {
            return Aviod_NoValue(dt_Mach_Info, "alarm_mesg");
        }
        public string Get_MachStatus(DataTable dt_Mach_Info, string machine_name)//目前狀態
        {
            return Aviod_NoValue(dt_Mach_Info, "mach_status", machine_name);
        }

        //進給率 主軸轉速 主軸溫度 主軸速度 主軸負載 切削時間 通電時間 運轉時間 主程式 主程式註解 運行程式註解 用
        public string Get_Information(DataTable dt_Mach_Info, string column)
        {
            return Aviod_NoValue(dt_Mach_Info, column);
        }
        //避免出錯的方式
        private string Aviod_NoValue(DataTable dt_Mach_Info, string value, string mach_name = "")
        {
            try
            {
                if (value == "run_time" || value == "cut_time" || value == "poweron_time")
                {
                    string dt_value = dt_Mach_Info.Rows[0][value].ToString();
                    dt_value = dt_value.Replace("H", ":").Replace("M", ":").Replace("S", ":");
                    List<string> time = new List<string>(dt_value.Split(':'));
                    int day = DataTableUtils.toInt(time[0]) / 24;
                    int hour = DataTableUtils.toInt(time[0]) % 24;
                    if (WebUtils.GetAppSettings("time_type") == "0")
                        return $"{day}天 {hour}小時 {time[1]}分鐘 {time[2]}秒";
                    else if (WebUtils.GetAppSettings("time_type") == "1")
                        return $"{day}天 {hour}小時 {time[1]}分鐘";
                    else if (WebUtils.GetAppSettings("time_type") == "2")
                        return $"{day}天 {hour}小時";
                    else if (WebUtils.GetAppSettings("time_type") == "3")
                        return $"{time[0]}小時";
                    else
                        return $"{day}天 {hour}小時 {time[1]}分鐘 {time[2]}秒";
                }
                else if (value == "prog_main" || value == "prog_run")
                {
                    List<string> progarm = new List<string>(dt_Mach_Info.Rows[0][value].ToString().Split('/'));
                    return progarm[progarm.Count - 1];
                }
                else
                    return dt_Mach_Info.Rows[0][value].ToString();
            }
            catch
            {
                if (mach_name != "" && value == "mach_status")
                {
                    GlobalVar.UseDB_setConnString(Mysql_conn_str);
                    string sqlcmd = $"select * from cnc_db.status_currently_info where mach_name = '{mach_name}'";
                    DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                    try
                    {
                        return dt.Rows[0]["status"].ToString();
                    }
                    catch
                    {
                        return "";
                    }
                }
                else if (mach_name != "" && value == "prog_run")
                {
                    GlobalVar.UseDB_setConnString(Mysql_conn_str);
                    string sqlcmd = $"select * from aps_info where mach_name = '{mach_name}'";
                    DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                    try
                    {
                        List<string> progarm = new List<string>(DataTableUtils.toString(dt.Rows[0]["prog_run"]).Split('/'));
                        return progarm[progarm.Count - 1];
                    }
                    catch
                    {
                        return "";
                    }

                }
                else if (mach_name != "" && value != "mach_status")
                    return mach_name;
                else
                    return "";
            }
        }
        public List<string> Get_Operate_Rate(string s_time, string e_time, string MachName)//稼動率//統計分析用
        {
            List<string> Operate_Rate = new List<string>();
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            DataTable dt_operate_rate = DataTableUtils.GetDataTable($"select * from status_realtime_info where mach_name = '{MachName}'and work_date >= '{s_time}' and work_date <= '{e_time}'");
            if (HtmlUtil.Check_DataTable(dt_operate_rate))
            {
                s_time = HtmlUtil.changetimeformat(s_time);
                DateTime st = DateTime.Parse(s_time.ToString());//取得該區間開始時間
                e_time = HtmlUtil.changetimeformat(e_time);
                DateTime end = DateTime.Parse(e_time.ToString());//取得該區間結束時間(+1式)
                DateTime noon = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 下午 12:00:00");
                List<string> list = new List<string>();//將時間放置於此
                List<string> list_num = new List<string>();
                TimeSpan ts = end - st;//日期相減
                TimeSpan distance = end - noon;
                int day = 1;
                day += Int16.Parse(ts.TotalDays.ToString("00"));//取得整數 

                //當月到目前為止非假日的時間點
                for (int i = 0; i < day; i++)
                {
                    if (st.AddDays(i).ToString() != "")
                        list.Add(st.AddDays(i).ToString("yyyyMMdd"));
                }
                //機台所有非假日時間點
                for (int iIndex = 0; iIndex < dt_operate_rate.Rows.Count; iIndex++)
                {
                    if (HtmlUtil.changetimeformat(dt_operate_rate.Rows[iIndex]["work_date"].ToString()) != "")
                        list_num.Add(dt_operate_rate.Rows[iIndex]["work_date"].ToString() + ":" +
                                     dt_operate_rate.Rows[iIndex]["work_time"].ToString() + "," +
                                     dt_operate_rate.Rows[iIndex]["disc_time"].ToString() + "," +
                                     dt_operate_rate.Rows[iIndex]["idle_time"].ToString() + "," +
                                     dt_operate_rate.Rows[iIndex]["alarm_time"].ToString() + "," +
                                     dt_operate_rate.Rows[iIndex]["operate_time"].ToString() + "," +
                                    Avoid_Null(dt_operate_rate.Rows[iIndex]["emergency_time"].ToString()) + "," +
                                     Avoid_Null(dt_operate_rate.Rows[iIndex]["suspend_time"].ToString()) + "," +
                                      Avoid_Null(dt_operate_rate.Rows[iIndex]["manual_time"].ToString()) + "," +
                                     Avoid_Null(dt_operate_rate.Rows[iIndex]["warmup_time"].ToString()));
                }

                for (int i = 0; i < list.Count; i++)
                {
                    bool isok = false;
                    for (int j = 0; j < list_num.Count; j++)
                    {
                        if (isok == false)
                        {
                            //有存在
                            if (list_num[j].IndexOf(list[i]) != -1)
                            {
                                Operate_Rate.Add(list_num[j]);
                                isok = true;
                            }
                        }
                    }
                    if (isok == false)
                        Operate_Rate.Add(list[i] + ":" + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0);
                }
            }
            return Operate_Rate;
        }
        public string Avoid_Null(string str)
        {
            if (str == "" || str == null)
                return "0";
            else
                return str;
        }
        public DateTime EndTime(DateTime end_time)
        {
            if (end_time >= DateTime.Now) end_time = DateTime.Now;
            return end_time;
        }
    }

    public class CNCUtils
    {
        private string str_FirstDay = "";
        private string str_LastDay = "";
        MTLinkiDB.MTLinkiDB MTLinki_DB = new MTLinkiDB.MTLinkiDB();

        public List<string> Status_Bar_Info(DataTable DT_Data, DateTime FirstDay)
        {
            string Update_time_date, Start_time_min, Cycle_time_min, Status, Start_time_line, End_time_line;
            DateTime Start_time, End_time;
            List<string> status_web = new List<string>();
            if (DT_Data != null && DT_Data.Rows.Count != 0)
            {
                for (int iIndex_1 = 0; iIndex_1 < DT_Data.Rows.Count; iIndex_1++)
                {
                    Update_time_date = "Update_time=" + DT_Data.Rows[iIndex_1]["update_time"].ToString().Split('.')[0];
                    Start_time = DateTime.ParseExact(DT_Data.Rows[iIndex_1]["update_time"].ToString(), "yyyyMMddHHmmss.f", null, DateTimeStyles.AllowWhiteSpaces);
                    Start_time_min = "Start_time=" + Math.Round(Start_time.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                    End_time = DateTime.ParseExact(DT_Data.Rows[iIndex_1]["enddate_time"].ToString(), "yyyyMMddHHmmss.f", null, DateTimeStyles.AllowWhiteSpaces);
                    Cycle_time_min = "Cycle_time=" + Math.Round(End_time.Subtract(Start_time).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                    Status = "Nc_Status=" + DT_Data.Rows[iIndex_1]["status"].ToString();
                    Start_time_line = "Start_time_line=" + DT_Data.Rows[iIndex_1]["update_time"].ToString().Substring(4, 10);
                    End_time_line = "End_time_line=" + DT_Data.Rows[iIndex_1]["enddate_time"].ToString().Substring(4, 10);

                    status_web.Add(Update_time_date + "," + Start_time_min + "," + Cycle_time_min + "," + Status + "," + Start_time_line + "," + End_time_line);
                }
            }
            return status_web;
        }
        //機台英文名稱→中文
        public static string MachName_translation(string id)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from machine_info where mach_name = '{id}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
                id = DataTableUtils.toString(dt.Rows[0]["mach_show_name"]);

            return id;
        }

        static Dictionary<CNCStatusCode, string> CNCStatusColor_list = new Dictionary<CNCStatusCode, string>()
        {
            { CNCStatusCode.OPERATE,"#04ba26"}, { CNCStatusCode.SUSPEND, "#808000"},//運轉//暫停
            { CNCStatusCode.MANUAL,"#00EFEF"}, { CNCStatusCode.WARMUP,"#890F27"},//手動//暖機
            { CNCStatusCode.ALARM,"#f73939"}, { CNCStatusCode.EMERGENCY, "#f73939"},//警告//警報
            { CNCStatusCode.STOP,"#ff9900"},//閒置
            { CNCStatusCode.DISCONNECT, "#737373"},//離線
            { CNCStatusCode.NONE, "#737373"}//關機//先用none代替
        };

        public string mach_status_Color(CNCStatusCode status)
        {
            string color;
            try
            {
                color = CNCStatusColor_list[status];
            }
            catch
            {
                color = "0x000000";
            }
            return color;
        }
        public string mach_status_Color(string status)
        {
            string color = "";
            switch (status)
            {
                case "OPERATE":
                    color = "#00b400";
                    break;
                case "DISCONNECT":
                    color = "#a9a9a9";
                    break;
                case "ALARM":
                    color = "#ff0000";
                    break;
                case "EMERGENCY":
                    color = "#ff00ff";
                    break;
                case "SUSPEND":
                    color = "#ffff00";
                    break;
                case "STOP":
                    color = "#ff9b32";
                    break;
                case "MANUAL":
                    color = "#02cdfc";
                    break;
                case "WARMUP":
                    color = "#b22222";
                    break;
            }
            return color;
        }
        public string mach_font_Color(string status)
        {
            string color = "";
            if (status == "閒置" || status == "待機" || status == "離線" || status == "暫停") color = "black";
            else color = "black";
            return color;
        }
        public string mach_background_Color(string status)
        {
            string color = "";
            switch (status)
            {
                case "運轉":
                case "OPERATE":
                    color = "#00b400";
                    break;
                case "離線":
                case "DISCONNECT":
                    color = "#a9a9a9";
                    break;
                case "警報":
                case "ALARM":
                    color = "#ff0000";
                    break;
                case "警告":
                case "EMERGENCY":
                    color = "#ff00ff";
                    break;
                case "暫停":
                case "SUSPEND":
                    color = "#ffff00";
                    break;
                case "待機":
                case "STOP":
                    color = "#ff9b32";
                    break;
                case "手動":
                case "MANUAL":
                    color = "#02cdfc";
                    break;
                case "暖機":
                case "WARMUP":
                    color = "#b22222";
                    break;
            }
            return color;
        }
        public string mach_status_EN2CH(string status_en)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from status_change where status_english = '{status_en}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                try
                {
                    return DataTableUtils.toString(dt.Rows[0]["status_chinese"]);
                }
                catch
                {
                    return status_en;
                }
            }
            else
                return status_en;
        }
        public List<string> get_search_time(string btnID, string str_time = "", string end_time = "")
        {
            List<string> ST_First_Last_Time = new List<string>();
            switch (btnID)
            {
                case "day":
                    str_FirstDay = DateTime.Today.ToString("yyyyMMddHHmmss");
                    str_LastDay = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyyMMddHHmmss");
                    break;
                case "week":
                    str_FirstDay = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToString("yyyyMMddHHmmss");  //單位：周
                    str_LastDay = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 7).AddSeconds(-1).ToString("yyyyMMddHHmmss");
                    break;
                case "month":
                    str_FirstDay = DateTime.Now.AddMonths(0).Date.AddDays(1 - DateTime.Now.Day).ToString("yyyyMMddHHmmss");
                    str_LastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(1).AddSeconds(-1).ToString("yyyyMMddHHmmss");
                    break;
                case "season":
                    DateTime dTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM"));                  //自訂月份//{2019/8/1 上午 12:00:00}
                    DateTime FirstDay = dTime.AddMonths(0 - (dTime.Month - 1) % 3).AddDays(1 - dTime.Day);  //本季度初
                    DateTime LastDay = FirstDay.AddMonths(3).AddSeconds(-1);
                    str_FirstDay = FirstDay.ToString("yyyyMMddHHmmss");
                    str_LastDay = LastDay.ToString("yyyyMMddHHmmss");
                    break;
                case "firsthalf":
                    str_FirstDay = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyyMMddHHmmss");  //2019-01-01
                    str_LastDay = new DateTime(DateTime.Now.Year, 7, 1).AddSeconds(-1).ToString("yyyyMMddHHmmss"); //2019-12-31
                    break;
                case "lasthalf":
                    str_FirstDay = new DateTime(DateTime.Now.Year, 7, 1).ToString("yyyyMMddHHmmss");  //2019-01-01
                    str_LastDay = new DateTime(DateTime.Now.Year, 12, 1).AddMonths(1).AddSeconds(-1).ToString("yyyyMMddHHmmss"); //2019-12-31
                    break;
                case "year":
                    str_FirstDay = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyyMMddHHmmss");  //2019-01-01
                    str_LastDay = new DateTime(DateTime.Now.Year, 12, 1).AddMonths(1).AddDays(-1).AddSeconds(-1).ToString("yyyyMMddHHmmss"); //2019-12-31
                    break;
                case "select":
                    if (DaysBetween(ret_date(str_time), ret_date(end_time)) > 730)
                    {
                        HttpContext.Current.Response.Write("<script language='javascript'>alert('伺服器回應 : 日期搜尋範圍不得大於 730 天 !');</script>");
                        str_FirstDay = DateTime.Now.AddMonths(0).Date.AddDays(1 - DateTime.Now.Day).ToString("yyyyMMddHHmmss");
                        str_LastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(1).AddSeconds(-1).ToString("yyyyMMddHHmmss");
                    }
                    else
                    {
                        str_FirstDay = ret_date(str_time).ToString("yyyyMMddHHmmss");
                        str_LastDay = ret_date(end_time).ToString("yyyyMMddHHmmss");
                    }
                    break;
                case "selectdate"://status_bar
                    str_FirstDay = ret_date(str_time).ToString("yyyyMMddHHmmss");
                    end_time = DateTime.ParseExact(str_FirstDay, "yyyyMMddHHmmss", null, DateTimeStyles.AllowWhiteSpaces).AddDays(1).ToString("yyyyMMddHHmmss");
                    str_LastDay = ret_date(end_time.Substring(0, 8)).ToString("yyyyMMddHHmmss");
                    break;
            }
            ST_First_Last_Time.Add(str_FirstDay + "," + str_LastDay);
            ST_First_Last_Time.Add(btnID);
            return ST_First_Last_Time;
        }
        public int DaysBetween(DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);
            return (int)span.TotalDays;
        }
        public DateTime ret_date(string date)
        {
            DateTime NewDate = new DateTime();
            var isContain = date.IndexOf(",", StringComparison.OrdinalIgnoreCase);
            if (isContain != -1 && date.Split(',')[1] == "PM")
                NewDate = (DateTime.ParseExact(date.Split(',')[0], "yyyyMMddhhmm", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces)).AddHours(12);
            else if (isContain != -1 && date.Split(',')[1] == "AM")
                NewDate = DateTime.ParseExact(date.Split(',')[0], "yyyyMMddhhmm", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            else
                NewDate = DateTime.ParseExact(date + "0000".Split(',')[0], "yyyyMMddhhmm", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            return NewDate;
        }
        public double Math_Round(double value_1, double value_2, int point)
        {
            return Math.Round(value_1 / value_2, point, MidpointRounding.AwayFromZero);
        }
        public static string date_Substring(string date)
        {
            try
            {
                return date.Substring(0, 11);
            }
            catch
            {
                return date;
            }

        }
        //加工程式→工藝名稱
        public static string change_productname(string product)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from product_info where craft_name = '{product}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
                product = DataTableUtils.toString(dt.Rows[0]["cnc_craft_name"]);

            return product;
        }
        //找到有攝影機的機台
        public static string Have_CameraLink(string machine)
        {
            string value = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from machine_info where mach_name = '{machine}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
                value = DataTableUtils.toString(dt.Rows[0]["camera_address"]);

            if (value == "")
                return "";
            else
                return value;
        }
        //找到對應的群組
        public static string Find_Group(string group)
        {
            if (group == "All" || group == "全部" || group == "" || group == "全廠")
                return "";
            else
                return group;
        }

    }

    public static class CNCError
    {
        /// <summary>
        /// 異常回復新增父項目
        /// </summary>
        /// <param name="type">進站報工/進佔維護</param>
        /// <param name="order">工單號碼</param>
        /// <param name="machine">機台名稱</param>
        /// <param name="machine_group">機台群組</param>
        /// <param name="Acc">使用者帳號</param>
        /// <param name="Errortype">異常類型</param>
        /// <param name="Father_Message">異常原因</param>
        /// <param name="Father_File">異常附件</param>
        /// <param name="Linemessage">是否有發送LINE</param>
        /// <returns></returns>
        public static bool Save_FatherMessage(string orderman, string type, string order, string machine, string machine_group, string Acc, string Errortype, string Father_Message, string Father_File, string Linemessage)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = "select * from error_report";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            sqlcmd = "select max(異常維護編號) 異常維護編號 from error_report";
            DataTable dt_max = DataTableUtils.GetDataTable(sqlcmd);
            int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["異常維護編號"].ToString()) + 1 : 1;

            if (dt != null)
            {
                DataRow row = dt.NewRow();
                row["異常維護編號"] = max;
                row["工單號碼"] = order;
                row["機台名稱"] = machine;

                row["維護人員姓名"] = HtmlUtil.Search_acc_Column(Acc, "USER_NAME");
                row["維護人員單位"] = HtmlUtil.Search_acc_Column(Acc, "DPM_NAME2");
                row["異常原因類型"] = Errortype;
                row["維護內容"] = Father_Message.Replace("'", " ");
                row["圖片檔名"] = Father_File;
                row["時間紀錄"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                row["模式"] = type;
                row["是否有發送LINE"] = Linemessage == "0" ? "N" : "Y";
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Insert_DataRow("error_report", row))
                {
                    LineNote(type, machine, order, Acc, Father_Message, max.ToString(), orderman);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 顯示異常歷程
        /// </summary>
        /// <param name="type">進站報工/進站維護</param>
        /// <param name="acc">使用者帳號</param>
        /// <param name="order">工單</param>
        /// <param name="machine">機台</param>
        /// <param name="group">機台群組</param>
        /// <param name="MantID">從LINE進入之ID</param>
        /// <returns></returns>
        public static string Question_History(string type, string acc, string order, string machine, string group, string MantID)
        {

            string YN = HtmlUtil.Search_acc_Column(acc, "Can_Close");
            acc = HtmlUtil.Search_acc_Column(acc, "USER_NAME");
            string update = "";
            string delete = "";
            string td = "";

            MantID = MantID == "" ? "" : $" and 異常維護編號 = '{MantID}' ";

            //取得所有的異常回覆訊息    
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"SELECT DISTINCT * FROM (SELECT 異常維護編號, 時間紀錄, 維護人員姓名, 維護人員單位, 異常原因類型, 維護內容, 圖片檔名 FROM error_report WHERE (父編號 IS NULL OR 父編號 = 0) and  工單號碼 = '{order}' AND 機台名稱 = '{machine}'  and 模式='{type}' ) a LEFT JOIN (SELECT 異常維護編號 回覆編號, 父編號, 時間紀錄 回覆時間, 維護人員姓名 回覆人員, 維護人員單位 回覆人員單位, 異常原因類型 回覆類型, 維護內容 回覆內容, 圖片檔名 回覆附件 FROM error_report WHERE (父編號 IS NOT NULL OR 父編號 <> 0)) b ON a.異常維護編號 = b.父編號 LEFT JOIN (SELECT MIN(異常維護編號) 結案編號, 父編號 結案父編號, 時間紀錄 結案時間, 維護人員姓名 結案人員, 維護人員單位 結案人員單位, 結案判定類型 結案類型, 結案內容 結案內容, 結案附檔 結案附件 FROM error_report WHERE (父編號 IS NOT NULL OR 父編號 <> 0) AND 結案判定類型 IS NOT NULL group by 父編號,異常維護編號,時間紀錄,維護人員姓名,維護人員單位,結案判定類型,  結案內容 , 結案附檔) c ON c.結案父編號 = a.異常維護編號 WHERE (c.結案編號 >= b.回覆編號 OR (b.回覆編號 IS null OR c.結案編號 IS null)) {MantID} ORDER BY 時間紀錄 , 回覆時間";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataView dv = dt.DefaultView;
                DataTable dt_father = dv.ToTable(true, "異常維護編號", "時間紀錄", "維護人員姓名", "維護人員單位", "異常原因類型", "維護內容", "圖片檔名", "結案類型");

                for (int i = 0; i < dt_father.Rows.Count; i++)
                {
                    td += "<tr class=\"gradeX\">";
                    for (int j = 0; j < dt_father.Columns.Count; j++)
                    {
                        if (dt_father.Columns[j].ColumnName == "異常維護編號")
                        {

                        }
                        else if (dt_father.Columns[j].ColumnName == "時間紀錄")
                        {

                        }
                        else if (dt_father.Columns[j].ColumnName == "維護人員單位")
                        {

                        }
                        else if (dt_father.Columns[j].ColumnName == "異常原因類型")
                        {

                        }
                        else if (dt_father.Columns[j].ColumnName == "圖片檔名")
                        {

                        }
                        else if (dt_father.Columns[j].ColumnName == "維護人員姓名")
                        {
                            td += $"<td style=\"text-align:center;width:15%\">" +
                                        $"<span style=\"font-size:8px ;text-align:right;color:red;\"> " +
                                            $"<b>{HtmlUtil.StrToDates(dt_father.Rows[i]["時間紀錄"].ToString()):yyyyMMddHHmmss}</b>" +
                                        "</span>" +
                                        "<br>" +
                                            $"[{dt_father.Rows[i]["維護人員單位"]}]{DataTableUtils.toString(dt_father.Rows[i][j])}" +
                                        "<br>" +
                                   "</td>\n";
                        }
                        else if (dt_father.Columns[j].ColumnName == "結案類型")
                        {
                            if (DataTableUtils.toString(dt_father.Rows[i][j]) != "")
                            {
                                td += "<td style='text-align:center;min-width:45px;max-width:25%;'>" +
                                            "<span style='color:green'>" +
                                                "結案" +
                                            "</span> " +
                                            $"<br>{DataTableUtils.toString(dt_father.Rows[i][j])}<br>" +
                                      "</td> ";
                            }
                            else
                                td += "<td style='text-align:center;min-width:45px;max-width:25%;'>" +
                                        "<span style='color:red'>" +
                                            "處理中" +
                                        "</span>" +
                                      "</td>\n";
                        }
                        else if (dt_father.Columns[j].ColumnName == "維護內容")
                        {
                            string close_type = DataTableUtils.toString(dt_father.Rows[i]["結案類型"]);
                            string file = "";
                            string Message = "";
                            string Message_Open = "";
                            string new_ID = "";
                            DataTable dt_child = new DataTable();

                            //----------------------------------子回覆部分------------------------------------------------------
                            var dt_format = dt.AsEnumerable().Where(w => w.Field<string>("父編號") == DataTableUtils.toString(dt_father.Rows[i]["異常維護編號"]) || w.Field<string>("結案父編號") == DataTableUtils.toString(dt_father.Rows[i]["異常維護編號"]));
                            if (dt_format.FirstOrDefault() != null)
                                dt_child = dt_format.CopyToDataTable();

                            if (HtmlUtil.Check_DataTable(dt_child))
                            {
                                //透過LINE進來的部分
                                if (MantID != "")
                                    new_ID = " in ";
                                else
                                    new_ID = "";

                                for (int a = 0; a < dt_child.Rows.Count; a++)
                                {
                                    string videos = "";
                                    file = "";
                                    file = Return_fileurl(DataTableUtils.toString(dt_child.Rows[a]["回覆附件"]), out videos);
                                    //回覆人員 訊息 時間
                                    string End_Report = "";
                                    string close_file = "";
                                    string close_video = "";

                                    if (a == dt_child.Rows.Count - 1 && close_type != "")
                                    {
                                        close_file = Return_fileurl(DataTableUtils.toString(dt_child.Rows[a]["結案附件"]), out close_video);
                                        End_Report = $"<div class=\"_EndDescripfa fa-chevron-circle-downtion\">" +
                                                        $"<span style=\"color:green\">" +
                                                            $"[結案說明]{DataTableUtils.toString(dt_child.Rows[a]["結案人員"])}" +
                                                        $"</span>" +
                                                        $":{br_text(DataTableUtils.toString(dt_child.Rows[a]["結案內容"]))}<br>{close_file}{close_video}" +
                                                     $" </div>";
                                    }
                                    delete = "";
                                    update = "";
                                    if (YN == "Y" || acc == DataTableUtils.toString(dt_child.Rows[a]["維護人員姓名"]))
                                    {
                                        if (a == dt_child.Rows.Count - 1)
                                            update = $" <div>" +
                                                         $"<a href='javascript:void(0)' onclick=updates('up_{DataTableUtils.toString(dt_child.Rows[a]["回覆編號"])}'," +
                                                                                                      $"'{order}'," +
                                                                                                      $"'{machine}'," +
                                                                                                      $"'{group}'," +
                                                                                                      $"'{DataTableUtils.toString(dt_child.Rows[a]["回覆內容"]).Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}'," +
                                                                                                      $"'{dt_child.Rows[a]["結案類型"]}'," +
                                                                                                      $"'{dt_child.Rows[a]["回覆附件"].ToString().Replace("\n", "^")}'," +
                                                                                                      $"'{dt_child.Rows[a]["結案類型"]}'," +
                                                                                                      $"'{dt_child.Rows[a]["結案內容"].ToString().Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}'," +
                                                                                                      $"'{dt_child.Rows[a]["結案附件"].ToString().Replace("\n", "^")}'," +
                                                                                                      $"'1') " +
                                                                                                      $"data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                            $"<u>" +
                                                                $"編輯" +
                                                             $"</u>" +
                                                         $"</a>" +
                                                     $"</div>";
                                        else
                                            update = $" <div>" +
                                                         $"<a href='javascript:void(0)' onclick=updates('up_{DataTableUtils.toString(dt_child.Rows[a]["回覆編號"])}'," +
                                                                                                      $"'{order}'," +
                                                                                                      $"'{machine}'," +
                                                                                                      $"'{group}'," +
                                                                                                      $"'{DataTableUtils.toString(dt_child.Rows[a]["回覆內容"]).Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}'," +
                                                                                                      $"'{dt_child.Rows[a]["回覆類型"]}'," +
                                                                                                      $"'{dt_child.Rows[a]["回覆附件"].ToString().Replace("\n", "^")}'," +
                                                                                                      $"''," +
                                                                                                      $"''," +
                                                                                                      $"''," +
                                                                                                      $"'1') " +
                                                                                                      $"data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                            $"<u>" +
                                                                $"編輯" +
                                                             $"</u>" +
                                                         $"</a>" +
                                                     $"</div>";

                                        delete = $"<a href='javascript:void(0)' onclick=deletes('{DataTableUtils.toString(dt_child.Rows[a]["回覆編號"])}') >" +
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
                                                        $"[{DataTableUtils.toString(dt_child.Rows[a]["回覆人員單位"])}] {DataTableUtils.toString(dt_child.Rows[a]["回覆人員"])}" +
                                                   $"</span>" +
                                                   $":{br_text(DataTableUtils.toString(dt_child.Rows[a]["回覆內容"]))} {file}" +
                                               $"</div>{videos}" +
                                               $"<div id=\"_Response\" class=\"col-12 col-xs-12 text-right\" style=\"margin-right:-6px;\">" +
                                                   $"<h6 style=\"text-align:right;\">" +
                                                        $"{HtmlUtil.StrToDates(DataTableUtils.toString(dt_child.Rows[a]["回覆時間"]))}" +
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
                                                        $" <a data-toggle=\"collapse\" data-parent=\"#accordion\"  href=\"#collapse{dt_father.Rows[i]["異常維護編號"]}\" >" +
                                                            $" <i id=\"Open{dt_father.Rows[i]["異常維護編號"]}\" onclick=Click_Num('Open{dt_father.Rows[i]["異常維護編號"]}') class='{icon}'  style='color:black;width:3%;font-size: 1.6em;'>" +
                                                             " </i>" +
                                                         "</a>" +
                                                    "</u>" +
                                                "</div>";
                            }


                            //----------------------------------子回覆部分------------------------------------------------------
                            //----------------------------------父回覆部分------------------------------------------------------
                            //父項目的檔案
                            string video = "";
                            file = "";
                            file = Return_fileurl(DataTableUtils.toString(dt_father.Rows[i]["圖片檔名"]), out video);
                            string error = HtmlUtil.Check_DataTable(dt_child) ? DataTableUtils.toString(dt_child.Rows[dt_child.Rows.Count - 1]["回覆類型"]) : DataTableUtils.toString(dt_father.Rows[i]["異常原因類型"]);
                            if (close_type == "")
                                close_type = $"<div>" +
                                                $"<a href='javascript:void(0)' onclick=Get_ErrorDetails('{DataTableUtils.toString(dt_father.Rows[i]["異常維護編號"])}'," +
                                                                                                       $"'{order}'," +
                                                                                                       $"'{machine}'," +
                                                                                                       $"'{group}'," +
                                                                                                       $"'{DataTableUtils.toString(dt_father.Rows[i]["維護人員姓名"])}'," +
                                                                                                       $"'{acc}'," +
                                                                                                       $"'{YN}'," +
                                                                                                       $"'{error}')" +
                                                                                                       $" data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                     $"<u>" +
                                                        $"回覆" +
                                                     $"</u>" +
                                                 $"</a>" +
                                             $"</div>";
                            else
                                close_type = $"<div>" +
                                             $"<a href='javascript:void(0)' onclick=alert('該回文已結案')>" +
                                                 $"<u>" +
                                                    $"回覆" +
                                                 $"</u>" +
                                             $"</a>" +
                                         $"</div>";

                            update = "";
                            delete = "";
                            if (YN == "Y" || acc == DataTableUtils.toString(dt_father.Rows[i]["維護人員姓名"]))
                            {
                                update = $"<div>" +
                                             $"<a href='javascript:void(0)' onclick=updates('up_{DataTableUtils.toString(dt_father.Rows[i]["異常維護編號"])}'," +
                                                                                          $"'{order}'," +
                                                                                          $"'{machine}'," +
                                                                                          $"'{group}'," +
                                                                                          $"'{DataTableUtils.toString(dt_father.Rows[i]["維護內容"]).Replace(" ", "$").Replace('"', '#').Replace("'", "^").Replace("\r\n", "@")}'," +
                                                                                          $"'{DataTableUtils.toString(dt_father.Rows[i]["異常原因類型"])}'," +
                                                                                          $"'{DataTableUtils.toString(dt_father.Rows[i]["圖片檔名"]).Replace("\n", "^")}'," +
                                                                                          $"''," +
                                                                                          $"''," +
                                                                                          $"''," +
                                                                                          $"'0') data-toggle = \"modal\" data-target = \"#exampleModal\">" +
                                                 $"<u>" +
                                                    $"編輯" +
                                                 $"</u>" +
                                             $"</a>" +
                                         $"</div>";
                                delete = $"<a href='javascript:void(0)' onclick=deletes('{DataTableUtils.toString(dt_father.Rows[i]["異常維護編號"])}') >" +
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
                            td += $"<td style='text-align:left;max-width:55%'>{Message_Open} {br_text(DataTableUtils.toString(dt_father.Rows[i][j]).Replace(';', '\n'))} {file}  {video} " +
                                    $"<div id=\"_Middle\" class=\"col-md-12 col-xs-12 text-right\" style=\"height:30px;\">" +
                                        //回復
                                        $"<div class=\"_status\">{close_type}</div>" +
                                        //編輯
                                        $"<div class=\"_update\">{update}</div>" +
                                        //刪除
                                        $"<div class=\"_delete\" style=\"margin:0px 0px 0px 3px\">{delete}</div>" +
                                    $"</div>" +
                                    $"<div id=\"collapse{dt_father.Rows[i]["異常維護編號"]}\" class=\"panel-collapse collapse {new_ID} \">" +
                                        $"<div class=\"panel-body\"> " +
                                            $"{Message}" +
                                        $"</div>" +
                                    $"</div>" +
                                $"</td>";
                            //----------------------------------父回覆部分------------------------------------------------------
                        }
                        else
                            td += $"<td style='text-align:left;max-width:60%'>{DataTableUtils.toString(dt_father.Rows[i][j])}</td>\n";
                    }
                    td += "</tr>\n";
                }

            }
            return td;
        }

        /// <summary>
        /// 文字分行
        /// </summary>
        /// <param name="word">文字</param>
        /// <returns></returns>
        public static string br_text(string word)
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
        /// <summary>
        /// 顯示附件之HTML碼
        /// </summary>
        /// <param name="file_name">附件檔案</param>
        /// <param name="image_mp4">圖片OR影片</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static string Return_fileurl(string file_name, out string image_mp4, string height = "248")
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

        /// <summary>
        /// 回傳多媒體之HTML碼
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="height">高度</param>
        /// <param name="type">類型 mp4 OR 圖片副檔名</param>
        /// <returns></returns>
        public static string Return_file(string file, string height, string type = "")
        {
            if (type.ToLower() == "mp4")
                file = $"<a onclick=show_image('{file}','video')  data-toggle=\"modal\" data-target=\"#exampleModal_Image\"  href=\"javascript: void()\"><video  width=\"97%\" height=\"{height}px\" src={file} controls=\"\"></video></a>";
            else
                file = $"<a onclick=show_image('{file}','image') data-toggle=\"modal\" data-target=\"#exampleModal_Image\"  href=\"javascript: void()\"><img src={file} alt=\"...\" width=\"97%\" height=\"{height}px\"></a>";
            return $"<div class=\"col-md-4 col-xs-4 gradeX_Img\" style=\"margin-bottom:8px;padding:0\">{file}</div>";

        }

        /// <summary>
        /// 新增子項目 或 修改父子項目的內容
        /// </summary>
        /// <param name="type">進站報工/進佔維護</param>
        /// <param name="order">工單號碼</param>
        /// <param name="machine">機台名稱</param>
        /// <param name="machine_group">機台群組</param>
        /// <param name="acc">帳號</param>
        /// <param name="ID">異常維護編號</param>
        /// <param name="Reply_Type">回覆類型</param>
        /// <param name="Reply_Content">回覆內容</param>
        /// <param name="Reply_File">回覆附件</param>
        /// <param name="Post">是否有發送LINE</param>
        /// <param name="Number">區分回覆或是新增用</param>
        /// <param name="Close_Type">結案類型</param>
        /// <param name="Close_Content">結案內容</param>
        /// <param name="Close_File">結案附件</param>
        /// <returns></returns>
        public static bool Add_Content(string orderman, string type, string order, string machine, string machine_group, string acc, string ID, string Reply_Type, string Reply_Content, string Reply_File, string Post, int Number = 0, string Close_Type = "", string Close_Content = "", string Close_File = "")
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from error_report where 異常維護編號 = '{Number}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            int num = 0;

            //有=修改
            if (HtmlUtil.Check_DataTable(dt))
            {
                DataRow row = dt.NewRow();
                row["異常維護編號"] = dt.Rows[0]["異常維護編號"];
                row["維護內容"] = Reply_Content;
                row["圖片檔名"] = Reply_File;
                row["異常原因類型"] = Reply_Type;
                //子項目修改
                if (DataTableUtils.toString(dt.Rows[0]["父編號"]) != "")
                {
                    row["結案判定類型"] = Close_Type;
                    row["結案內容"] = Close_Content;
                    row["結案附檔"] = Close_File;
                }
                row["是否有發送LINE"] = Post == "1" ? "Y" : "N";
                row["時間紀錄"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Update_DataRow("error_report", $"異常維護編號 = '{Number}'", row))
                {
                    LineNote(type, machine, order, acc, Reply_Content, ID, orderman, Close_Content);
                    return true;
                }
                else
                    return false;
            }
            //沒有=新增
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                sqlcmd = $"select * from error_report where 異常維護編號 = '{ID}' and 工單號碼 = '{order}' and 機台名稱='{machine}' ";
                dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    //先找尋最大ID
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    sqlcmd = "SELECT max(異常維護編號) 異常維護編號 FROM error_report ";
                    DataTable dt_max = DataTableUtils.GetDataTable(sqlcmd);
                    num = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(DataTableUtils.toString(dt_max.Rows[0]["異常維護編號"])) + 1 : 1;

                    DataRow row = dt.NewRow();
                    row["異常維護編號"] = num;
                    row["工單號碼"] = order;
                    row["機台名稱"] = machine;
                    row["維護人員姓名"] = HtmlUtil.Search_acc_Column(acc, "USER_NAME");
                    row["維護人員單位"] = HtmlUtil.Search_acc_Column(acc, "DPM_NAME2");
                    row["異常原因類型"] = Reply_Type;
                    row["維護內容"] = Reply_Content;
                    row["圖片檔名"] = Reply_File;
                    row["父編號"] = ID;
                    row["結案附檔"] = Close_File;
                    row["結案判定類型"] = Close_Type;
                    row["結案內容"] = Close_Content;
                    row["時間紀錄"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                    row["是否有發送LINE"] = Post == "1" ? "Y" : "N";
                    row["模式"] = type;
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    if (DataTableUtils.Insert_DataRow("error_report", row))
                    {
                        LineNote(type, machine, order, acc, Reply_Content, ID, orderman, Close_Content);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Line 發送前
        /// </summary>
        /// <param name="machine">機台名稱</param>
        /// <param name="order">工單號碼</param>
        /// <param name="acc">帳號</param>
        /// <param name="content">異常內容</param>
        /// <param name="num">該訊息之ID</param>
        public static void LineNote(string type, string machine, string order, string acc, string content, string num, string orderman, string close_content = "")
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"SELECT line_sceptercol FROM line_info where mach_name = '{machine}' ";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            //若沒有抓到，就用預設的
            string linetoken = HtmlUtil.Check_DataTable(dt) ? DataTableUtils.toString(dt.Rows[0]["line_sceptercol"]) : WebUtils.GetAppSettings("Preset");

            //取得一些資料
            sqlcmd = $"select workorder_information.*,mach_show_name,machine_info.area_name group_name from workorder_information,machine_info where manu_id='{order}' and workorder_information.mach_name='{machine}' and type_mode='{type}' and order_status <> '出站' and workorder_information.mach_name = machine_info.mach_name";
            DataTable dt_information = DataTableUtils.GetDataTable(sqlcmd);

            string number = HtmlUtil.Check_DataTable(dt_information) ? DataTableUtils.toString(dt_information.Rows[0]["product_number"]) : "";
            string show_name = HtmlUtil.Check_DataTable(dt_information) ? DataTableUtils.toString(dt_information.Rows[0]["mach_show_name"]) : "";
            string group = HtmlUtil.Check_DataTable(dt_information) ? DataTableUtils.toString(dt_information.Rows[0]["group_name"]) : "";
            string staff = HtmlUtil.Check_DataTable(dt_information) ? DataTableUtils.toString(dt_information.Rows[0]["work_staff"]) : "";

            //產生url
            string url = $"machine={machine},order={order},type={type},number={number},show_name={show_name},group={group},staff={staff},num={num}";
            url = $"{WebUtils.GetAppSettings("Line_ip")}:{WebUtils.GetAppSettings("Line_port")}/pages/dp_CNC/Error_Detail.aspx?key={WebUtils.UrlStringEncode(url)}";

            //取代字串，有些在line無法使用(&,<,>,",',%)
            content = content.Replace('&', '，').Replace('+', '，').Replace('<', '，').Replace('>', '，').Replace('"', '，').Replace("'", "，").Replace('%', '，');
            if (close_content == "")
                lineNotify(linetoken, $"\r\n[機台名稱]:{show_name}\r\n[工單人員]:{orderman}\r\n[發送帳號]:{HtmlUtil.Search_acc_Column(acc, "USER_NAME")}\r\n[工單號碼]:{order}\r\n[異常內容]:{content}\r\n[連結]:{url}");
            else
                lineNotify(linetoken, $"\r\n[機台名稱]:{show_name}\r\n[工單人員]:{orderman}\r\n[發送帳號]:{HtmlUtil.Search_acc_Column(acc, "USER_NAME")}\r\n[工單號碼]:{order}\r\n[結案內容]:{close_content}\r\n[連結]:{url}");

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

        public static void Post_Email(string title, string subtitle, string content)
        {
            MailMessage mail = new MailMessage();
            //前面是發信email後面是顯示的名稱
            mail.From = new MailAddress("信箱帳號@....", title);

            //收信者email
            mail.To.Add("信箱帳號@....");


            //設定優先權
            mail.Priority = MailPriority.Normal;

            //標題
            mail.Subject = subtitle;

            //內容
            mail.Body = $"<h1>{content}</h1>";

            //內容使用html
            mail.IsBodyHtml = true;

            //設定gmail的smtp (這是google的)
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //您在gmail的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("信箱帳號@....", "信箱密碼");

            //開啟ssl
            MySmtp.EnableSsl = true;

            //發送郵件
            MySmtp.Send(mail);

            //放掉宣告出來的MySmtp
            MySmtp = null;

            //放掉宣告出來的mail
            mail.Dispose();
        }

        /// <summary>
        /// 設定廠區的下拉選單
        /// </summary>
        /// <param name="acc">帳號</param>
        /// <param name="factory">下拉選單ID</param>
        /// <param name="page_name">該頁面名稱</param>
        public static void Set_FactoryDropdownlist(string acc, DropDownList factory, string page_name)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            if (factory.Items.Count == 0)
            {
                string sql = "";
                DataTable dt_mach = new DataTable();
                string all_mach = "";

                ListItem listItem = new ListItem();
                List<string> machlist = new List<string>();

                DataTable dt_data = new DataTable();
                factory.Items.Clear();
                dt_data = DataTableUtils.GetDataTable("select distinct area_name from mach_group where area_name <> '全廠' and area_name <> '測試區'  ");
                if (HtmlUtil.Check_DataTable(dt_data))
                {
                    factory.Items.Add("--Select--");
                    string itemname = "";
                    string acc_power = CNCUtils.Find_Group(HtmlUtil.Search_acc_Column(acc, "Belong_Factory"));
                    if (acc_power == "")
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        string sqlcmd = "select * from mach_group where area_name = '全廠'";
                        DataTable dt_all = DataTableUtils.GetDataTable(sqlcmd);
                        //全廠的部分
                        if (HtmlUtil.Check_DataTable(dt_all))
                        {
                            //找出全廠設備
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                            sql = "SELECT mach_show_name FROM machine_info where area_name <> '測試區'";
                            dt_mach = DataTableUtils.GetDataTable(sql);

                            //取得所有機台
                            if (HtmlUtil.Check_DataTable(dt_mach))
                            {
                                foreach (DataRow rw in dt_mach.Rows)
                                    all_mach += DataTableUtils.toString(rw["mach_show_name"]) + "^";
                            }
                            itemname = $"--Select--,1^,";
                            foreach (DataRow row in dt_all.Rows)
                            {
                                if (DataTableUtils.toString(row["web_address"]) != "")
                                    itemname += $"{DataTableUtils.toString(row["group_name"])},{DataTableUtils.toString(row["web_address"])}{page_name},";
                                else
                                    itemname += $"{DataTableUtils.toString(row["group_name"])},1^{all_mach},";
                            }
                            listItem = new ListItem("全廠", itemname);
                            factory.Items.Add(listItem);
                        }

                        //其他廠區的部分
                        if (HtmlUtil.Check_DataTable(dt_data))
                        {
                            foreach (DataRow row in dt_data.Rows)
                            {
                                itemname = "";
                                machlist.Clear();
                                all_mach = "";
                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                                sqlcmd = $"select * from mach_group where area_name = '{DataTableUtils.toString(row["area_name"])}'";
                                dt_all = DataTableUtils.GetDataTable(sqlcmd);


                                foreach (DataRow rew in dt_all.Rows)
                                {

                                    if (DataTableUtils.toString(rew["web_address"]) != "")
                                        itemname += $"{DataTableUtils.toString(rew["group_name"])},{DataTableUtils.toString(rew["web_address"])}{page_name},";
                                    else
                                    {
                                        all_mach = "";
                                        string[] mach = DataTableUtils.toString(rew["mach_name"]).Split(',');
                                        for (int i = 0; i < mach.Length; i++)
                                        {
                                            if (mach[i] != "")
                                            {
                                                all_mach += CNCUtils.MachName_translation(mach[i]) + "^";
                                                machlist.Add(CNCUtils.MachName_translation(mach[i]));
                                            }
                                        }

                                        itemname += $"{DataTableUtils.toString(rew["group_name"])},1^{all_mach}^,";

                                    }
                                }
                                itemname = $"--Select--,1^," + itemname;
                                listItem = new ListItem(DataTableUtils.toString(row["area_name"]), itemname);
                                factory.Items.Add(listItem);
                            }
                        }
                    }
                    //特定廠區
                    else
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        string sqlcmd = $"select * from mach_group where area_name = '{acc_power}'";
                        DataTable dt_all = DataTableUtils.GetDataTable(sqlcmd);
                        //全廠的部分
                        if (HtmlUtil.Check_DataTable(dt_all))
                        {
                            itemname = "";
                            machlist.Clear();
                            all_mach = "";

                            foreach (DataRow row in dt_all.Rows)
                            {
                                if (DataTableUtils.toString(row["web_address"]) != "")
                                    itemname += $"{DataTableUtils.toString(row["group_name"])},{DataTableUtils.toString(row["web_address"])}{page_name},";
                                else
                                {
                                    all_mach = "";
                                    string[] mach = DataTableUtils.toString(row["mach_name"]).Split(',');
                                    for (int i = 0; i < mach.Length; i++)
                                    {
                                        if (mach[i] != "")
                                        {
                                            all_mach += CNCUtils.MachName_translation(mach[i]) + "^";
                                            machlist.Add(CNCUtils.MachName_translation(mach[i]));
                                        }
                                    }
                                    itemname += $"{DataTableUtils.toString(row["group_name"])},1^{all_mach}^,";
                                }
                            }

                            machlist = machlist.Distinct().ToList();
                            for (int i = 0; i < machlist.Count; i++)
                                all_mach += machlist[i] + "^";

                            itemname = $"--Select--,1^," + itemname;
                            listItem = new ListItem(acc_power, itemname);
                            factory.Items.Add(listItem);
                        }
                    }
                }
            }
        }
    }

    //報工用
    public static class CNCReport
    {


        /// <summary>
        /// 出站/回報數量(填入良品)
        /// </summary>
        /// <param name="dt_main">點擊之資料</param>
        /// <param name="dt_other">點擊之相關資料</param>
        /// <param name="Report_Qty">良品數量</param>
        /// <param name="now_time">回報時間</param>
        /// <param name="type">進站報工/進站維護</param>
        /// <param name="exit">是否為出站</param>
        /// <returns></returns>
        public static bool Order_OKExit(DataTable dt, double Report_Qty, string now_time, string type, string NowCount, bool exit = false)
        {
            bool ok = true;
            int count = 0;
            int total = dt.Rows.Count;
            foreach (DataRow row in dt.Rows)
            {
                //目標數量  50
                double target_qty = DataTableUtils.toInt(DataTableUtils.toString(row["exp_product_count_day"]));

                //已生產數量 0
                double producted_qty = DataTableUtils.toInt(DataTableUtils.toString(row["product_count_day"]));
                //剩餘數量 回報數量+已生產數量-目標數量
                double last_qty = Report_Qty + producted_qty - target_qty;
                //超過需求要修正到滿
                if (last_qty >= 0)
                    Report_Qty = last_qty;
                //數量夠的case
                if (count == total - 1 && Report_Qty >= 0 && last_qty >= 0)
                    target_qty = target_qty + Report_Qty;

                DataRow rows = dt.NewRow();
                rows["_id"] = row["_id"];
                /*   1.已完成數量 = 維護數量 + 良品數量(已入ERP) + 不良數量(已入ERP)
                     2.未完成數量 = 總需求量 - (維護數量 + 良品數量(已入ERP) + 不良數量(已入ERP))
                */
                rows["product_count_day"] = last_qty >= 0 ? target_qty : producted_qty + Report_Qty;
                rows["no_product_count_day"] = last_qty >= 0 ? 0 : target_qty - (producted_qty + Report_Qty);
                //rows["good_qty"] = last_qty >= 0 ? target_qty - producted_qty + DataTableUtils.toDouble(row["good_qty"]) : DataTableUtils.toDouble(NowCount.ToString());
                //不應該累加 因為在inf 是一入一出  為一筆資料
                rows["good_qty"] = last_qty >= 0 ? target_qty - producted_qty + DataTableUtils.toDouble(row["good_qty"]) : Report_Qty + DataTableUtils.toDouble(row["good_qty"]);
                rows["bad_qty"] = DataTableUtils.toDouble(row["bad_qty"]);//良品分配 所以不良品為0   0712 不改變  不良品分配才會正確

                rows["last_updatetime"] = now_time;
                if (!string.IsNullOrEmpty(NowCount))//0523-juiedit
                    rows["adj_qty"] = NowCount;
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                //更新workorder_information一次入站只會有一筆直到出站 以出站紀錄住
                if (DataTableUtils.Update_DataRow("workorder_information", $"mach_name='{row["mach_name"]}'  and manu_id='{row["manu_id"]}' and type_mode='{type}' and order_status='入站' ", rows))
                {
                    System.Threading.Thread.Sleep(1);
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    string sqlcmd = $"SELECT * FROM (SELECT *, (SELECT MIN(a.now_time) FROM record_worktime a WHERE workman_status = '出站' AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id and a.type_mode = record_worktime.type_mode) exit_time FROM record_worktime WHERE workman_status = '入站') a WHERE a.exit_time IS NULL AND product_number = '{row["product_number"]}' AND mach_name = '{row["mach_name"]}' AND now_time <= '{now_time}' AND manu_id = '{row["manu_id"]}' and type_mode = '{type}'";
                    DataTable dt_insert = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt_insert))
                    {
                        DataTable dt_clone = HtmlUtil.Get_HeadRow(dt_insert);
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        sqlcmd = "SELECT max(_id) _id FROM record_worktime";
                        DataTable dt_max = DataTableUtils.GetDataTable(sqlcmd);
                        int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["_id"].ToString()) + 1 : 1;
                        int j = 0;
                        foreach (DataRow dtrow in dt_insert.Rows)
                        {
                            rows = dt_clone.NewRow();
                            rows["mach_name"] = dtrow["mach_name"];
                            rows["manu_id"] = dtrow["manu_id"];
                            rows["product_number"] = dtrow["product_number"];
                            rows["product_name"] = dtrow["product_name"];
                            rows["work_staff"] = dtrow["work_staff"];
                            rows["workman_status"] = exit ? "良品出站" : "中途報工";
                            rows["report_qty"] = last_qty >= 0 ? target_qty - producted_qty : Report_Qty;
                            rows["qty_status"] = "良品";
                            rows["now_time"] = now_time;
                            rows["type_mode"] = type;
                            if (!string.IsNullOrEmpty(NowCount))//0523-juiedit
                                rows["adj_qty"] = NowCount;
                            dt_clone.Rows.Add(rows);
                            j++;
                        }
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        if (dt_clone.Rows.Count == DataTableUtils.Insert_TableRows("record_worktime", dt_clone))
                            ok = true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
                if (last_qty <= 0)
                    Report_Qty = 0;
                count++;
            }
            return ok;
        }

        /// <summary>
        /// 出站(填入不良品)
        /// </summary>
        /// <param name="dt_main">點擊之資料</param>
        /// <param name="dt_other">點擊之相關資料</param>
        /// <param name="Bad_Qty">不良品數量</param>
        /// <param name="now_time">回報時間</param>
        /// <param name="type">進站維護/進站報工</param>
        /// <param name="exit">是否為出站</param>
        /// <param name="list">不良品相關資訊</param>
        /// <returns></returns>
        public static bool Order_NGExit(DataTable dt, double Bad_Qty, string now_time, string type, bool exit, List<string> list = null)
        {
            bool ok = false;
            bool judge = false;
            int count = 0;
            int rowtotal = dt.Rows.Count;
            double bad_copy = Bad_Qty;
            double bad_total = 0;

            foreach (DataRow row in dt.Rows)
            {
                //目標數量 --50
                double target_qty = DataTableUtils.toInt(DataTableUtils.toString(row["exp_product_count_day"]));

                //已生產數量 --40
                double producted_qty = DataTableUtils.toInt(DataTableUtils.toString(row["product_count_day"]));

                //未生產數量 --10
                double unproducted_qty = target_qty - producted_qty;

                //剩餘不良數量 --0
                double last_qty = unproducted_qty >= 0 ? Bad_Qty - unproducted_qty : Bad_Qty;

                // --10
                if (last_qty >= 0)
                    Bad_Qty = last_qty;

                if (count == rowtotal - 1 && Bad_Qty >= 0 && last_qty >= 0)
                    target_qty = target_qty + Bad_Qty;

                if (count == rowtotal - 1)
                    judge = true;
                DataRow rows = dt.NewRow();
                rows["_id"] = row["_id"];

                //未生產量
                rows["no_product_count_day"] = last_qty >= 0 ? 0 : target_qty - (producted_qty + Bad_Qty);

                //不良品數量
                rows["bad_qty"] =  last_qty >= 0 ? (judge ? bad_copy : unproducted_qty) : (DataTableUtils.toDouble(row["bad_qty"].ToString()) + Bad_Qty);

                //預計產量
                // rows["product_count_day"] = DataTableUtils.toDouble(rows["bad_qty"].ToString()) + producted_qty;
                rows["product_count_day"] = Bad_Qty + producted_qty;
                //判斷存入的數量
                bad_total = last_qty >= 0 ? judge ? bad_copy : unproducted_qty : Bad_Qty;

                rows["last_updatetime"] = now_time;
                if (exit)
                    rows["order_status"] = "出站";

                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Update_DataRow("workorder_information", $"mach_name='{row["mach_name"]}'  and manu_id='{row["manu_id"]}' and type_mode='{type}' and order_status='入站' ", rows))
                {
                    //填入不良品
                    if (bad_total > 0)
                        Bad_Exit(row, list, bad_total, type);
                    //填入 record_worktime
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    string sqlcmd = $"SELECT * FROM (SELECT *, (SELECT MIN(a.now_time) FROM record_worktime a WHERE workman_status = '出站' AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id and a.type_mode = record_worktime.type_mode) exit_time FROM record_worktime WHERE workman_status = '入站') a WHERE a.exit_time IS NULL AND product_number = '{row["product_number"]}' AND mach_name = '{row["mach_name"]}' AND now_time <= '{now_time}' AND manu_id = '{row["manu_id"]}' and type_mode = '{type}'   ";
                    DataTable dt_insert = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt_insert))
                    {
                        DataTable dt_clone = HtmlUtil.Get_HeadRow(dt_insert);
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        sqlcmd = "SELECT max(_id) _id FROM record_worktime";
                        DataTable dt_max = DataTableUtils.GetDataTable(sqlcmd);
                        int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["_id"].ToString()) + 1 : 1;
                        int j = 0;
                        foreach (DataRow dtrow in dt_insert.Rows)
                        {
                            rows = dt_clone.NewRow();

                            rows["mach_name"] = dtrow["mach_name"];
                            rows["manu_id"] = dtrow["manu_id"];
                            rows["product_number"] = dtrow["product_number"];
                            rows["product_name"] = dtrow["product_name"];
                            rows["work_staff"] = dtrow["work_staff"];
                            if (exit)
                                rows["workman_status"] = "出站";
                            else
                                rows["workman_status"] = "中途報工";
                            rows["report_qty"] = last_qty >= 0 ? judge ? bad_copy : unproducted_qty : Bad_Qty;
                            rows["qty_status"] = "不良品";
                            rows["now_time"] = now_time;
                            rows["type_mode"] = type;
                            dt_clone.Rows.Add(rows);
                            j++;
                        }
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        if (dt_clone.Rows.Count == DataTableUtils.Insert_TableRows("record_worktime", dt_clone))
                            ok = true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
                count++;
                if (last_qty < 0)
                    Bad_Qty = 0;
            }
            return ok;
        }

        /// <summary>
        /// 出站(填入不良品類型 / 原因)
        /// </summary>
        /// <param name="row">資料內容</param>
        /// <param name="list">不良陣列</param>
        /// <param name="Bad_Qty">不良需求數量</param>
        /// <param name="type">進站維護/進站報工</param>
        /// <returns></returns>
        private static List<string> Bad_Exit(DataRow row, List<string> list, double bad_qty, string type)
        {
            if (list.Count > 3)
            {
                double bad_copy = bad_qty;
                //15
                double count = DataTableUtils.toDouble(list[1]);

                bad_qty = bad_qty - count >= 0 ? count : bad_qty;

                //取得結構
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                string sqlcmd = "select * from bad_total";
                DataTable dt_Bad = DataTableUtils.GetDataTable(sqlcmd);

                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                sqlcmd = "select max(id) id from bad_total";
                DataTable dt_max = DataTableUtils.GetDataTable(sqlcmd);

                int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["id"].ToString()) + 1 : 1;

                DataRow rows = dt_Bad.NewRow();
                rows["id"] = max;
                rows["mach_name"] = row["mach_name"];
                rows["manu_id"] = row["manu_id"];
                rows["now_time"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                rows["bad_qty"] = bad_qty;
                rows["exp_product_count_day"] = row["exp_product_count_day"];
                rows["work_staff"] = row["work_staff"];
                rows["type_mode"] = type;
                rows["product_number"] = row["product_number"];
                rows["delivery"] = row["delivery"];
                rows["bad_type"] = list[0];
                rows["bad_content"] = list[2];
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Insert_DataRow("bad_total", rows))
                {
                    //數量不足
                    if ((bad_copy - count) > 0)
                    {
                        list.RemoveAt(0);
                        list.RemoveAt(0);
                        list.RemoveAt(0);
                        if (list.Count > 2)
                            Bad_Exit(row, list, bad_copy - count, type);
                        else
                            return null;
                    }
                    //數量足夠 還可以回扣
                    else if ((bad_copy - count) < 0)
                    {
                        list[1] = (count - bad_qty).ToString();
                        return list;
                    }
                    else
                    {
                        list.RemoveAt(0);
                        list.RemoveAt(0);
                        list.RemoveAt(0);
                        return list;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 把維護的數量，寫入報工內//報工維護 混用 改這裡!
        /// </summary>
        public static void Maintain_to_Report()
        {
            //先找到報工的
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = " SELECT  workorder_information.*  FROM workorder_information LEFT JOIN (SELECT  * FROM workorder_information WHERE order_status = '出站' AND type_mode = '進站報工' ) a ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number AND a.now_time < workorder_information.now_time WHERE  workorder_information.order_status = '入站' AND workorder_information.type_mode = '進站報工'";
            DataTable dt_report = DataTableUtils.GetDataTable(sqlcmd);
            DataRow[] manu_rows;
            if (HtmlUtil.Check_DataTable(dt_report))
            {
                //依據報工找到派工
                string manu_id = "";
                foreach (DataRow row in dt_report.Rows)
                    manu_id += manu_id == "" ? $" manu_id='{row["manu_id"]}' " : $" or manu_id='{row["manu_id"]}'";

                if (manu_id != "")
                {
                    manu_id = $" and ( {manu_id} ) ";
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                    sqlcmd = $"SELECT  * FROM workorder_information WHERE  order_status = '出站' AND type_mode = '進站維護' {manu_id}  order by now_time desc";
                    DataTable dt_maintain = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt_maintain))
                    {
                        foreach (DataRow row in dt_report.Rows)
                        {
                            manu_rows = dt_maintain.Select($"manu_id='{row["manu_id"]}'");
                            if (manu_rows != null && manu_rows.Length > 0)
                            {
                                //ROW[0] 所以只有修訂第一次 應該要用集合才有辦法報工->暫停->維護->報工
                                DataRow rsw = dt_report.NewRow();
                                rsw["product_count_day"] = DataTableUtils.toDouble(manu_rows[0]["good_qty"]) + DataTableUtils.toDouble(row["product_count_day"]) - DataTableUtils.toDouble(row["good_qty"]);
                                rsw["no_product_count_day"] = DataTableUtils.toDouble(row["no_product_count_day"].ToString()) - DataTableUtils.toDouble(manu_rows[0]["good_qty"].ToString()) - DataTableUtils.toDouble(row["product_count_day"]) + DataTableUtils.toDouble(row["good_qty"]);
                                rsw["maintain_qty"] = DataTableUtils.toDouble(manu_rows[0]["good_qty"].ToString());

                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                                DataTableUtils.Update_DataRow("workorder_information", $"manu_id='{row["manu_id"]}' and type_mode='進站報工'", rsw);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 透過POST的方式，把資料透過API傳送至ERP內部
        /// </summary>
        /// <param name="order">工單號碼</param>
        /// <param name="machine">機台名稱</param>
        public static async void PostToERP(string order, string machine)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string msg = "";
            string sqlcmd = $"SELECT  " +
                            $"      'BBE' cus,  " +
                            $"      'BT' Company,  " + //需修改 0609 juiedit=>test->BT2>0621  >bt2->bt
                            $"    SUBSTRING(_id,1,CHARINDEX('-',_id)-1)  'NO', " +
                            $"     SUBSTRING(_id,CHARINDEX('-',_id)+1,len(SUBSTRING(_id,CHARINDEX('-',_id)-1,len(_id)-CHARINDEX('-',_id)))-CHARINDEX('-',_id)) 'Order', " +
                            $"     SUBSTRING(_id,CHARINDEX('-',_id,CHARINDEX('-',_id)+1)+1,len(_id)-CHARINDEX('-',_id,CHARINDEX('-',_id)+1)) ProcessSeq, " +
                            $"      mach_name Vendor_ID, " +
                            $"      craft_Number  ProcessCode, " +
                            $"      product_number ItemNum, " +
                            $"      product_name ItemName1, " +
                            $"      specification ItemName2," +
                            $"     cast ((IsNULL(cast(good_qty as int), 0) +IsNULL(cast(maintain_qty as int), 0)) as varchar) P_Qty, " +
                            $"      bad_qty N_Qty, " +
                            $"      '0' P_Time, " + //需修改
                            $"      '0' M_Time, " + //需修改
                            $"   CONVERT(varchar(100),  CONVERT(DATETIME, STUFF(STUFF(STUFF(now_time, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')), 20) S_date , " +
                            $"    SUBSTRING(last_updatetime,1,8) E_date      " +
                            $"  FROM workorder_information " +
                            $"  where manu_id='{order}' and mach_name = '{machine}' and type_mode='進站報工'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = new List<string>(order.Split('-'));
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].ReadOnly = false;
                    try
                    {
                        dt.Columns[i].MaxLength = 10000;
                    }
                    catch
                    {

                    }
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["NO"] = order_list[0];
                    row["Order"] = order_list[1];
                    row["ProcessSeq"] = order_list[2];
                }
                //List<Post_Order> od = HtmlUtil.ConvertToList<Post_Order>(dt);
                PostDt2Api<Post_Order>(dt, "post_api");
                //var Resoule = await PostDt2Api<Post_Order>(dt, "post_api");
                //return Resoule.ToString();
            }
            // "dataError";

        }
        public static async void PostDt2Api<T>(DataTable dt, string FuncIniName)
        {
            string msg = "";
            List<T> od = HtmlUtil.ConvertToList<T>(dt);
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(od);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = null;
            response = await httpClient.PostAsync(HtmlUtil.Get_Ini("Parameter", FuncIniName, ""), httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result.Contains("ERR"))
                    msg = result;
                else if (result.Contains("PASS"))
                    msg = "傳送成功";
                else
                    msg = "";
            }
            //return msg;
        }
        public static async Task<DataTable> PostDs2Api(DataSet ds, string FuncIniName)
        {
            string msg = "";
            var httpClient = new HttpClient();
            DataTable dt = new DataTable();
            var json = JsonConvert.SerializeObject(ds);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = null;
            response = await httpClient.PostAsync(HtmlUtil.Get_Ini("Parameter", FuncIniName, ""), httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                // var result = await response.Content.ReadAsStringAsync();
                dt = JsonToDataTable.JsonStringToDataTable(result);
                if (result.Contains("ERROR"))
                    msg = "可以抓到錯誤內容";
                else if (result.Contains("Ret"))
                    msg = "傳送成功";
            }
            return dt;
        }
        public static async Task<int> PostD1s2Api(DataSet ds, string FuncIniName)
        {
            string msg = "";
            var httpClient = new HttpClient();
            DataTable dt = new DataTable();
            var json = JsonConvert.SerializeObject(ds);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = null;
            response = await httpClient.PostAsync(HtmlUtil.Get_Ini("Parameter", FuncIniName, ""), httpContent);
            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadAsStringAsync();
                dt = JsonToDataTable.JsonStringToDataTable(result);
                if (result.Contains("ERROR"))
                    msg = "可以抓到錯誤內容";
                else if (result.Contains("Ret"))
                    msg = "傳送成功";
            }
            return 2;
        }
    }

    //Model
    public class Post_Order
    {
        public string cus { get; set; }
        public string Company { get; set; }
        public string NO { get; set; }
        public string Order { get; set; }
        public string ProcessSeq { get; set; }
        public string Vendor_ID { get; set; }
        public string ProcessCode { get; set; }
        public string ItemNum { get; set; }
        public string ItemName1 { get; set; }
        public string ItemName2 { get; set; }
        public string P_Qty { get; set; }
        public string N_Qty { get; set; }
        public string P_Time { get; set; }
        public string M_Time { get; set; }
        public string S_date { get; set; }
        public string E_date { get; set; }
    }
    public class Tags
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class Craft_Inf
    {
        public string OrderNum { get; set; }
        public string Mach { get; set; }
        public string Craft { get; set; }

    }
}