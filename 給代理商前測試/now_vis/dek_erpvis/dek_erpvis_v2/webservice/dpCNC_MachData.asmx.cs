using dek_erpvis_v2.cls;
using MongoDB.Driver.Builders;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Xml;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
//

namespace dek_erpvis_v2.webservice
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class dpCNC_MachData : System.Web.Services.WebService
    {
        CNCUtils cNC_Class = new CNCUtils();
        CNC_Web_Data Web_Data = new CNC_Web_Data();
        clsDB_Server cls_db = new clsDB_Server(myclass.GetConnByDekVisCnc_inside);

        /*--------------------------------------------------原加工VIS用---------------------------------------------------------------*/
        //得到機台資料
        /// <summary>
        /// 取得機台資料
        /// </summary>
        /// <param name="acc">帳號</param>
        /// <param name="machine">機台</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetMachineData(string acc, string machine)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string acc_power = HtmlUtil.Search_acc_Column(acc, "Belong_Factory");
            string condition = "";
            if (acc_power == "" || acc_power == "All" || acc_power == "全部" || acc_power == "全廠")
                condition = "  where area_name <> '測試區' ";
            else
                condition = $" where area_name = '{acc_power}' ";
            string machine_sqlcmd = "";

            if (machine != "")
            {
                List<string> machine_list = new List<string>(WebUtils.UrlStringDecode(machine).Split(','));
                for (int i = 0; i < machine_list.Count - 1; i++)
                {
                    if (i == 0)
                        machine_sqlcmd += $" mach_name = '{machine_list[i]}' ";
                    else
                        machine_sqlcmd += $" OR mach_name = '{machine_list[i]}' ";
                }
                if (condition == "")
                    machine_sqlcmd = $" where {machine_sqlcmd} ";
                else
                    machine_sqlcmd = $" and ({machine_sqlcmd}) ";
            }


            DataTable dt_data = null;
            List<string> ls_data = new List<string>();
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            dt_data = DataTableUtils.GetDataTable($"select mach_name from machine_info {condition} {machine_sqlcmd} ");
            if (HtmlUtil.Check_DataTable(dt_data))
            {
                foreach (DataRow row in dt_data.Rows)
                    ls_data.Add(row.ItemArray[0].ToString());
            }


            XmlDocument xmlDoc = new XmlDocument();

            if (ls_data.Count > 0)
            {
                XmlElement xmlElem = xmlDoc.CreateElement("ROOT_MACH");
                //--------------------------------------
                if (ls_data != null)
                {
                    if (ls_data.Count <= 0)
                        xmlElem.SetAttribute("count", "0");
                    else
                    {
                        xmlElem.SetAttribute("count", DataTableUtils.toString(ls_data.Count));
                        xmlDoc.AppendChild(xmlElem);
                        for (int iIndex = 0; iIndex < ls_data.Count; iIndex++)
                        {
                            XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                            dt_data = Web_Data.Get_MachInfo(ls_data[iIndex]);
                            if (dt_data != null)
                            {
                                string CheckStaff, WorkStaff, MachName, CustomName, ManuId, ProductName, ProductNumber, CraftName, CountTotal, CountToday, ExpCountToday, CountTodayRate, FinishTime, OperRate, MachStatus, AlarmMesg, ProgramRun;
                                CheckStaff = Web_Data.Get_CheckStaff(dt_data);
                                WorkStaff = Web_Data.Get_WorkStaff(dt_data);
                                MachName = Web_Data.Get_MachName(dt_data, ls_data[iIndex]);
                                CustomName = Web_Data.Get_CustomName(dt_data);
                                ManuId = Web_Data.Get_ManuID(dt_data);
                                ProductName = Web_Data.Get_ProductName(dt_data);
                                ProductNumber = Web_Data.Get_ProductNumber(dt_data);
                                CraftName = Web_Data.Get_CraftName(dt_data);
                                CountTotal = Web_Data.Get_CountTotal(dt_data);
                                CountToday = Web_Data.Get_CountToday(dt_data);
                                ExpCountToday = Web_Data.Get_ExpCountToday(dt_data);
                                CountTodayRate = Web_Data.Get_CountTodayRate(dt_data);
                                FinishTime = Web_Data.Get_FinishTime(dt_data);
                                OperRate = Web_Data.Get_Operate_Rate(dt_data, MachName);
                                MachStatus = Web_Data.Get_MachStatus(dt_data, ls_data[iIndex]);
                                AlarmMesg = Web_Data.Get_AlarmMesg(dt_data);
                                ProgramRun = Web_Data.Get_ProgramRun(dt_data, ls_data[iIndex]);

                                //20201201新增
                                string acts = Web_Data.Get_Information(dt_data, "acts");//主軸轉速
                                string spindleload = Web_Data.Get_Information(dt_data, "spindleload");//主軸負載
                                string spindlespeed = Web_Data.Get_Information(dt_data, "spindlespeed");//主軸速度
                                string spindletemp = Web_Data.Get_Information(dt_data, "spindletemp");//主軸溫度
                                string prog_main = Web_Data.Get_Information(dt_data, "prog_main"); ;//主程式
                                string prog_main_cmd = Web_Data.Get_Information(dt_data, "prog_main_cmd");//主程式註解
                                string prog_run_cmd = Web_Data.Get_Information(dt_data, "prog_run_cmd");//運行程式註解
                                string overrides = Web_Data.Get_Information(dt_data, "override");//進給率
                                string run_time = Web_Data.Get_Information(dt_data, "run_time");//運轉時間
                                string cut_time = Web_Data.Get_Information(dt_data, "cut_time");//切削時間
                                string poweron_time = Web_Data.Get_Information(dt_data, "poweron_time");//通電時間

                                //20210105新增
                                string complete_time = Web_Data.Get_Information(dt_data, "complete_time");//通電時間

                                //20210111新增
                                string order_number = Web_Data.Get_Information(dt_data, "order_number");//order_num

                                //計算後面還有多少排程
                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                                string sqlcmd = $"select * from aps_list_info where mach_name = '{MachName}' and CAST(order_number AS UNSIGNED)  > {order_number} ";
                                DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                                string count = "0";
                                if (HtmlUtil.Check_DataTable(ds))
                                    count = "" + ds.Rows.Count;
                                //計算目前時間與當前排程開始時間
                                double Date_Now = DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                double Date_Start = DataTableUtils.toDouble(dt_data.Rows[0]["start_time"].ToString());
                                string can_next = "";
                                if ((Date_Now - Date_Start) >= 0)
                                    can_next = "can";

                                string now_detailstatus = "";
                                if (HtmlUtil.Check_DataTable(dt_data))
                                {

                                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                                    sqlcmd = $"select * from record_worktime where WORK_MACHINE = '{MachName}' and start_time = '{dt_data.Rows[0]["start_time"]}' and end_time = '{dt_data.Rows[0]["end_time"]}' order by NOW_TIME desc limit 1";
                                    DataTable dt_status = DataTableUtils.GetDataTable(sqlcmd);

                                    if (HtmlUtil.Check_DataTable(dt_status))
                                        now_detailstatus = dt_status.Rows[0]["WORKMAN_STATUS"].ToString();
                                }



                                xmlElemA.SetAttribute("Dev_Name", MachName);
                                xmlElemA.SetAttribute("checkMachStaff", CheckStaff);
                                xmlElemA.SetAttribute("prodCustomName", CustomName);
                                xmlElemA.SetAttribute("prodNo", ProductNumber);
                                xmlElemA.SetAttribute("curParts", CountTotal);//總件數
                                xmlElemA.SetAttribute("prod_count", CountToday);//今日生產件數
                                xmlElemA.SetAttribute("tarParts", ExpCountToday);//預計生產件數
                                xmlElemA.SetAttribute("partsRate", CountTodayRate);
                                xmlElemA.SetAttribute("operRate", OperRate);
                                xmlElemA.SetAttribute("alarmMesg", AlarmMesg);
                                xmlElemA.SetAttribute("status", MachStatus);
                                xmlElemA.SetAttribute("workStaff", WorkStaff);
                                xmlElemA.SetAttribute("craftName", CraftName);
                                xmlElemA.SetAttribute("prodName", ProductName);
                                xmlElemA.SetAttribute("progRun", ProgramRun);
                                xmlElemA.SetAttribute("partsTime", FinishTime);
                                xmlElemA.SetAttribute("manuId", ManuId);

                                xmlElemA.SetAttribute("acts", acts);
                                xmlElemA.SetAttribute("spindleload", spindleload);
                                xmlElemA.SetAttribute("spindlespeed", spindlespeed);
                                xmlElemA.SetAttribute("spindletemp", spindletemp);
                                xmlElemA.SetAttribute("prog_main", prog_main);
                                xmlElemA.SetAttribute("prog_main_cmd", prog_main_cmd);
                                xmlElemA.SetAttribute("prog_run_cmd", prog_run_cmd);
                                xmlElemA.SetAttribute("override", overrides);
                                xmlElemA.SetAttribute("run_time", run_time);
                                xmlElemA.SetAttribute("cut_time", cut_time);
                                xmlElemA.SetAttribute("poweron_time", poweron_time);
                                xmlElemA.SetAttribute("complete_time", complete_time);
                                xmlElemA.SetAttribute("order_number", order_number.Trim());
                                xmlElemA.SetAttribute("count", count);
                                xmlElemA.SetAttribute("can_next", can_next);
                                xmlElemA.SetAttribute("now_detailstatus", now_detailstatus);
                                string now_list = $"{CNCUtils.MachName_translation(MachName)}^{WorkStaff}^{ManuId}^{CustomName}^{ProductName}^{ProductNumber}^{CraftName}^{DataTableUtils.toInt(CountToday)}^{DataTableUtils.toInt(ExpCountToday)}^";
                                xmlElemA.SetAttribute("now_list", now_list.Replace(' ', '*'));

                                //次筆排程資料
                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                                sqlcmd = $"select * from aps_list_info where mach_name = '{MachName}' and CAST(order_number AS double)  > {order_number}  order by CAST(order_number AS double) asc limit 1 ";
                                DataTable Next_DataTable = DataTableUtils.GetDataTable(sqlcmd);

                                string List_next = "";
                                if (HtmlUtil.Check_DataTable(Next_DataTable))
                                    List_next = $"{CNCUtils.MachName_translation(MachName)}^{Next_DataTable.Rows[0]["work_staff"]}^{Next_DataTable.Rows[0]["manu_id"]}^{Next_DataTable.Rows[0]["custom_name"]}^{Next_DataTable.Rows[0]["product_name"]}^{Next_DataTable.Rows[0]["product_number"]}^{Next_DataTable.Rows[0]["craft_name"]}^{"0"}^{Next_DataTable.Rows[0]["exp_product_count_day"]}^";

                                xmlElemA.SetAttribute("next_list", List_next.Replace(' ', '*'));


                                string staff = "";
                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                                sqlcmd = $"select staff_name from staff_info left join machine_info on machine_info.area_name = staff_info.area_name where machine_info.mach_name = '{MachName}'";
                                DataTable dt_staff = DataTableUtils.GetDataTable(sqlcmd);
                                if (HtmlUtil.Check_DataTable(dt_staff))
                                {
                                    foreach (DataRow row in dt_staff.Rows)
                                        staff += $"{row["staff_name"]}^";
                                }
                                xmlElemA.SetAttribute("staff", staff.Replace(" ", "*"));
                            }
                            xmlDoc.DocumentElement.AppendChild(xmlElemA);
                        }
                        return xmlDoc.DocumentElement;
                    }
                }
                else
                    xmlElem.SetAttribute("Value", "-1");
                //------------------------------------------------------------------------
            }
            return xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 取得整天的狀態BAR
        /// </summary>
        /// <param name="Mach_ID">機台</param>
        /// <param name="First_Day">起始時間</param>
        /// <param name="Last_Day">結束時間</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetMachStatus(string Mach_ID, string First_Day, string Last_Day)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            XmlDocument xmlDoc = new XmlDocument();
            DateTime FirstDay = Convert.ToDateTime(First_Day);
            DateTime LastDay = Convert.ToDateTime(Last_Day);
            LastDay = Web_Data.EndTime(LastDay);

            List<string> status_web = new List<string>();
            List<string> ST_Data_1 = new List<string>();
            List<string> ST_Data_2 = new List<string>();
            List<string> ST_Data_3 = new List<string>();
            string Update_time_date, Start_time_min, Cycle_time_min, Status, Start_time_line, End_time_line;
            DateTime Start_time, End_time;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from status_history_info where '{FirstDay:yyyyMMddHHmmss}' >= update_time and enddate_time >= '{FirstDay:yyyyMMddHHmmss}' and enddate_time <= '{LastDay:yyyyMMddHHmmss}' and mach_name = '{Mach_ID}'  ";
            DataTable dt1 = DataTableUtils.GetDataTable(sqlcmd);
            if (dt1 != null && dt1.Rows.Count != 0)
            {
                End_time = DateTime.ParseExact(dt1.Rows[0]["enddate_time"].ToString(), "yyyyMMddHHmmss.f", null, DateTimeStyles.AllowWhiteSpaces);
                Cycle_time_min = "Cycle_time=" + Math.Round(End_time.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                Status = "Nc_Status=" + dt1.Rows[0]["status"].ToString();

                Update_time_date = "Update_time=" + FirstDay.ToString("yyyyMMddHHmmss");
                Start_time_min = "Start_time=0";
                Start_time_line = "Start_time_line=" + FirstDay.ToString("MMddHHmmss");
                End_time_line = "End_time_line=" + End_time.ToString("MMddHHmmss");

                ST_Data_1.Add($"{Update_time_date},{Start_time_min},{Cycle_time_min},{Status},{Start_time_line},{End_time_line}");
            }
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            sqlcmd = $"select * from status_history_info where  '{FirstDay:yyyyMMddHHmmss}' <= update_time and enddate_time <= '{LastDay:yyyyMMddHHmmss}' and mach_name = '{Mach_ID}' ";
            DataTable dt2 = DataTableUtils.GetDataTable(sqlcmd);
            ST_Data_2 = cNC_Class.Status_Bar_Info(dt2, FirstDay);
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            sqlcmd = $"select * from status_currently_info where mach_name = '{Mach_ID}'";
            DataTable dt3 = DataTableUtils.GetDataTable(sqlcmd);
            if (dt3 != null && dt3.Rows.Count != 0)
            {
                Start_time = DateTime.ParseExact(dt3.Rows[0]["update_time"].ToString(), "yyyyMMddHHmmss.f", null, DateTimeStyles.AllowWhiteSpaces);

                if (Start_time > FirstDay)
                {
                    Update_time_date = "Update_time=" + dt3.Rows[0]["update_time"].ToString().Split('.')[0];
                    Start_time_min = "Start_time=" + Math.Round(Start_time.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                    Cycle_time_min = "Cycle_time=" + Math.Round(LastDay.Subtract(Start_time).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                    Start_time_line = "Start_time_line=" + dt3.Rows[0]["update_time"].ToString().Substring(4, 10);
                }
                else
                {
                    Update_time_date = "Update_time=" + FirstDay.ToString("yyyyMMddHHmmss");
                    Start_time_min = "Start_time=0";
                    Cycle_time_min = "Cycle_time=" + Math.Round(LastDay.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                    Start_time_line = "Start_time_line=" + FirstDay.ToString("MMddHHmmss");
                }
                Status = "Nc_Status=" + dt3.Rows[0]["status"].ToString();

                End_time_line = "End_time_line=" + LastDay.ToString("yyyyMMddHHmmss").Substring(4, 10);

                ST_Data_3.Add($"{Update_time_date},{Start_time_min},{Cycle_time_min},{Status},{Start_time_line},{End_time_line}");
            }

            status_web = ST_Data_1.Concat(ST_Data_2).ToList<string>().Concat(ST_Data_3).ToList<string>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Update_time");
            dt.Columns.Add("Start_time");
            dt.Columns.Add("Cycle_time");
            dt.Columns.Add("Nc_Status");
            dt.Columns.Add("Start_time_line");
            dt.Columns.Add("End_time_line");

            foreach (string val in status_web)
            {
                string data_str = "";
                DataRow row = dt.NewRow();
                data_str = val;
                row["Update_time"] = data_str.Split(',')[0].Split('=')[1];
                row["Start_time"] = data_str.Split(',')[1].Split('=')[1];
                row["Cycle_time"] = data_str.Split(',')[2].Split('=')[1];
                row["Nc_Status"] = data_str.Split(',')[3].Split('=')[1];
                row["Start_time_line"] = "開始時間：" + data_str.Split(',')[4].Split('=')[1].Substring(0, 2) + "/" + data_str.Split(',')[4].Split('=')[1].Substring(2, 2) + " " +
                                         data_str.Split(',')[4].Split('=')[1].Substring(4, 2) + ":" + data_str.Split(',')[4].Split('=')[1].Substring(6, 2) + ":" + data_str.Split(',')[4].Split('=')[1].Substring(8, 2);
                row["End_time_line"] = "結束時間：" + data_str.Split(',')[5].Split('=')[1].Substring(0, 2) + "/" + data_str.Split(',')[5].Split('=')[1].Substring(2, 2) + " " +
                                       data_str.Split(',')[5].Split('=')[1].Substring(4, 2) + ":" + data_str.Split(',')[5].Split('=')[1].Substring(6, 2) + ":" + data_str.Split(',')[5].Split('=')[1].Substring(8, 2);
                dt.Rows.Add(row);
            }

            XmlElement xmlElem = xmlDoc.CreateElement("ROOT");
            if (dt != null)
            {
                if (dt.Rows.Count == 0)
                {
                    xmlElem.SetAttribute("Value", "0");
                    xmlDoc.AppendChild(xmlElem);
                    return xmlDoc.DocumentElement;
                }
                xmlElem.SetAttribute("Value", dt.Rows.Count.ToString());

                xmlDoc.AppendChild(xmlElem);
                foreach (DataRow dr in dt.Rows)
                {
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                    xmlElemA.SetAttribute("Nc_Status", dr["Nc_Status"].ToString());
                    xmlElemA.SetAttribute("Cycle_time", dr["Cycle_time"].ToString());
                    xmlElemA.SetAttribute("Start_time", dr["Start_time"].ToString());
                    xmlElemA.SetAttribute("Update_time", dr["Update_time"].ToString());
                    xmlElemA.SetAttribute("Start_time_line", dr["Start_time_line"].ToString());
                    xmlElemA.SetAttribute("End_time_line", dr["End_time_line"].ToString());
                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                }
                return xmlDoc.DocumentElement;
            }
            else
            {
                xmlElem.SetAttribute("Value", "-1");
            }
            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 取得早上跟下午的狀態BAR
        /// </summary>
        /// <param name="Mach_ID">機台</param>
        /// <param name="First_Day">起始時間(早上/下午)</param>
        /// <param name="Last_Day">結束時間(早上/下午)</param>
        /// <param name="condition">參數</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetMachStatus_bar(string Mach_ID, string First_Day, string Last_Day, string condition = "")
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            XmlDocument xmlDoc = new XmlDocument();
            DateTime FirstDay = Convert.ToDateTime(First_Day);
            DateTime LastDay = Convert.ToDateTime(Last_Day);
            LastDay = Web_Data.EndTime(LastDay);

            List<string> status_web = new List<string>();
            List<string> ST_Data_1 = new List<string>();
            List<string> ST_Data_2 = new List<string>();
            List<string> ST_Data_3 = new List<string>();
            string Update_time_date, Start_time_min, Cycle_time_min, Status, Start_time_line, End_time_line;
            DateTime Start_time, End_time;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"SELECT * FROM status_history_info , status_change WHERE '{FirstDay:yyyyMMddHHmmss}' >=  update_time  AND  enddate_time  >= '{FirstDay:yyyyMMddHHmmss}' AND enddate_time <= '{LastDay:yyyyMMddHHmmss}' and mach_name = '{Mach_ID}' {WebUtils.UrlStringDecode(condition)} AND  status_change.status_english = status_history_info.status   ";
            DataTable dt1 = DataTableUtils.GetDataTable(sqlcmd);
            if (dt1 != null && dt1.Rows.Count != 0)
            {
                End_time = DateTime.ParseExact(dt1.Rows[0]["enddate_time"].ToString(), "yyyyMMddHHmmss.f", null, DateTimeStyles.AllowWhiteSpaces);
                Cycle_time_min = "Cycle_time=" + Math.Round(End_time.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                Status = "Nc_Status=" + dt1.Rows[0]["status"].ToString();

                Update_time_date = "Update_time=" + FirstDay.ToString("yyyyMMddHHmmss");
                Start_time_min = "Start_time=0";
                Start_time_line = "Start_time_line=" + FirstDay.ToString("MMddHHmmss");
                End_time_line = "End_time_line=" + End_time.ToString("MMddHHmmss");

                ST_Data_1.Add(Update_time_date + "," + Start_time_min + "," + Cycle_time_min + "," + Status + "," + Start_time_line + "," + End_time_line);
            }
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            sqlcmd = $"SELECT * FROM status_history_info,status_change WHERE  '{FirstDay:yyyyMMddHHmmss}' <= update_time AND enddate_time <= '{LastDay:yyyyMMddHHmmss}'   and mach_name = '{Mach_ID}'  {WebUtils.UrlStringDecode(condition)}  and status_change.status_english = status_history_info.status";
            DataTable dt2 = DataTableUtils.GetDataTable(sqlcmd);

            int count = dt2.Rows.Count;
            //持續超過一天以上
            if (dt2 != null && count == 0)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                sqlcmd = $"SELECT * FROM status_history_info,status_change WHERE '{FirstDay:yyyyMMddHHmmss}' >=  update_time  AND  enddate_time  >= '{LastDay:yyyyMMddHHmmss}' and  mach_name = '{Mach_ID}'  {WebUtils.UrlStringDecode(condition)}  and  status_change.status_english = status_history_info.status ";
                dt2 = DataTableUtils.GetDataTable(sqlcmd);

                foreach (DataRow dr in dt2.Rows) // search whole table
                {
                    dr["update_time"] = FirstDay.ToString("yyyyMMddHHmmss") + ".0";
                    dr["enddate_time"] = LastDay.ToString("yyyyMMddHHmmss") + ".0";
                }
            }
            //從上午跨到下午的部分
            if (First_Day.Contains("00:00:00") && Last_Day.Contains("11:59:59") && count != 0)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                //
                sqlcmd = $" SELECT  status_history_info._id, mach_name, status, update_time update_time, enddate_time, timespan, upload_time, status_chinese FROM   status_history_info, status_change,(select max(_id) _id from status_history_info where  '{FirstDay:yyyyMMddHHmmss}' <= update_time AND update_time <= '{LastDay:yyyyMMddHHmmss}' AND mach_name = '{Mach_ID}' {WebUtils.UrlStringDecode(condition)} 	AND CAST(timespan AS SIGNED) > 0) a WHERE '{FirstDay:yyyyMMddHHmmss}' <= update_time AND update_time <= '{LastDay:yyyyMMddHHmmss}' AND mach_name = '{Mach_ID}' {WebUtils.UrlStringDecode(condition)}  	AND CAST(timespan AS SIGNED) > 0	AND status_change.status_english = status_history_info.status	and a._id = status_history_info._id GROUP BY mach_name";
                DataTable dt_moon = DataTableUtils.GetDataTable(sqlcmd);
                double moon = DataTableUtils.toDouble(DataTableUtils.toString(dt_moon.Rows[0]["enddate_time"]));
                double last_time = DataTableUtils.toDouble(LastDay.ToString("yyyyMMddHHmmss.f"));
                if (moon > last_time)
                {
                    foreach (DataRow dr in dt_moon.Rows) // search whole table
                        dr["enddate_time"] = LastDay.ToString("yyyyMMddHHmmss") + ".0";
                }
                try
                {
                    dt2.Merge(dt_moon, true, MissingSchemaAction.Ignore);
                }
                catch
                {

                }
            }
            string ss = DateTime.Today.ToString("yyyy/MM/dd");
            bool o2k = First_Day.Contains(DateTime.Today.ToString("yyyy/MM/dd"));

            //抓取當天的最後一筆
            if (!First_Day.Contains(DateTime.Today.ToString("yyyy/MM/dd")) && (count != 0 || WebUtils.UrlStringDecode(condition).Contains("離線")))
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                sqlcmd = $"SELECT max(status_history_info._id)  _id,mach_name,status,update_time,enddate_time,timespan,upload_time,status_chinese FROM status_history_info,status_change WHERE  update_time <= '{FirstDay:yyyyMMdd235959}'  and   mach_name = '{Mach_ID}' AND status_change.status_english = status_history_info.status  group by mach_name";
                DataTable dt_last = DataTableUtils.GetDataTable(sqlcmd);

                foreach (DataRow dr in dt_last.Rows) // search whole table
                    dr["enddate_time"] = LastDay.ToString("yyyyMMdd235959.0");

                bool ok = WebUtils.UrlStringDecode(condition).Contains(DataTableUtils.toString(dt_last.Rows[0]["status_chinese"]));
                bool test = WebUtils.UrlStringDecode(condition).Contains("status_chinese");

                if (WebUtils.UrlStringDecode(condition).Contains(DataTableUtils.toString(dt_last.Rows[0]["status_chinese"])) || !WebUtils.UrlStringDecode(condition).Contains("status_chinese"))
                    dt2.Merge(dt_last, true, MissingSchemaAction.Ignore);
            }
            ST_Data_2 = cNC_Class.Status_Bar_Info(dt2, FirstDay);
            double First_Time = 0, Now_Time = 0;
            First_Time = DataTableUtils.toDouble(FirstDay.ToString("yyyyMMdd120000"));
            Now_Time = DataTableUtils.toDouble(DateTime.Now.ToString("yyyyMMddHHmmss"));

            if (First_Time > Now_Time && First_Day.Contains("00:00:00") && Last_Day.Contains("11:59:59"))
                First_Time = DataTableUtils.toDouble(FirstDay.ToString("yyyyMMdd000000"));

            //如果是今天
            if (First_Day.Contains(DateTime.Today.ToString("yyyy/MM/dd")) && Now_Time >= First_Time)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                sqlcmd = $"select * from(SELECT     _id,    mach_name,    status,    update_time,status_chinese,   {DateTime.Now.ToString("yyyyMMddHHmmss.f")} as enddate_time,    (CAST({DateTime.Now.ToString("yyyyMMddHHmmss")} AS double)  - CAST(update_time AS double)) as timespan FROM    status_currently_info    Left join status_change on status_change.status_english = status_currently_info.status    ) as a where a.mach_name = '" + Mach_ID + "' "
                        + WebUtils.UrlStringDecode(condition).Replace("timespan", "a.timespan").Replace("status_chinese", "a.status_chinese");
                DataTable dt3 = DataTableUtils.GetDataTable(sqlcmd);
                if (dt3 != null && dt3.Rows.Count != 0)
                {
                    Start_time = DateTime.ParseExact(dt3.Rows[0]["update_time"].ToString(), "yyyyMMddHHmmss.f", null, DateTimeStyles.AllowWhiteSpaces);

                    if (Start_time > FirstDay)
                    {
                        Update_time_date = "Update_time=" + dt3.Rows[0]["update_time"].ToString().Split('.')[0];
                        Start_time_min = "Start_time=" + Math.Round(Start_time.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                        Cycle_time_min = "Cycle_time=" + Math.Round(LastDay.Subtract(Start_time).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                        Start_time_line = "Start_time_line=" + dt3.Rows[0]["update_time"].ToString().Substring(4, 10);
                    }
                    else
                    {
                        Update_time_date = "Update_time=" + FirstDay.ToString("yyyyMMddHHmmss");
                        Start_time_min = "Start_time=0";
                        Cycle_time_min = "Cycle_time=" + Math.Round(LastDay.Subtract(FirstDay).Duration().TotalMinutes, 2, MidpointRounding.AwayFromZero).ToString();
                        Start_time_line = "Start_time_line=" + FirstDay.ToString("MMddHHmmss");
                    }
                    Status = "Nc_Status=" + dt3.Rows[0]["status"].ToString();

                    End_time_line = "End_time_line=" + LastDay.ToString("yyyyMMddHHmmss").Substring(4, 10);

                    ST_Data_3.Add(Update_time_date + "," + Start_time_min + "," + Cycle_time_min + "," + Status + "," + Start_time_line + "," + End_time_line);
                }
                status_web = ST_Data_1.Concat(ST_Data_2).ToList<string>().Concat(ST_Data_3).ToList<string>();
            }
            else
                status_web = ST_Data_1.Concat(ST_Data_2).ToList<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Update_time");
            dt.Columns.Add("Start_time");
            dt.Columns.Add("Cycle_time");
            dt.Columns.Add("Nc_Status");
            dt.Columns.Add("Start_time_line");
            dt.Columns.Add("End_time_line");

            foreach (string val in status_web)
            {
                string data_str = "";
                DataRow row = dt.NewRow();
                data_str = val;
                row["Update_time"] = data_str.Split(',')[0].Split('=')[1];
                row["Start_time"] = data_str.Split(',')[1].Split('=')[1];
                row["Cycle_time"] = data_str.Split(',')[2].Split('=')[1];
                row["Nc_Status"] = data_str.Split(',')[3].Split('=')[1];
                row["Start_time_line"] = "開始時間：" + data_str.Split(',')[4].Split('=')[1].Substring(0, 2) + "/" + data_str.Split(',')[4].Split('=')[1].Substring(2, 2) + " " +
                                         data_str.Split(',')[4].Split('=')[1].Substring(4, 2) + ":" + data_str.Split(',')[4].Split('=')[1].Substring(6, 2) + ":" + data_str.Split(',')[4].Split('=')[1].Substring(8, 2);
                row["End_time_line"] = "結束時間：" + data_str.Split(',')[5].Split('=')[1].Substring(0, 2) + "/" + data_str.Split(',')[5].Split('=')[1].Substring(2, 2) + " " +
                                       data_str.Split(',')[5].Split('=')[1].Substring(4, 2) + ":" + data_str.Split(',')[5].Split('=')[1].Substring(6, 2) + ":" + data_str.Split(',')[5].Split('=')[1].Substring(8, 2);
                dt.Rows.Add(row);
            }
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT");
            if (dt != null)
            {
                if (dt.Rows.Count == 0)
                {
                    xmlElem.SetAttribute("Value", "0");
                    xmlDoc.AppendChild(xmlElem);
                    return xmlDoc.DocumentElement;
                }
                xmlElem.SetAttribute("Value", dt.Rows.Count.ToString());

                xmlDoc.AppendChild(xmlElem);
                foreach (DataRow dr in dt.Rows)
                {
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                    xmlElemA.SetAttribute("Nc_Status", dr["Nc_Status"].ToString());
                    xmlElemA.SetAttribute("Cycle_time", dr["Cycle_time"].ToString());
                    xmlElemA.SetAttribute("Start_time", dr["Start_time"].ToString());
                    xmlElemA.SetAttribute("Update_time", dr["Update_time"].ToString());
                    xmlElemA.SetAttribute("Start_time_line", dr["Start_time_line"].ToString());
                    xmlElemA.SetAttribute("End_time_line", dr["End_time_line"].ToString());
                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                }
                return xmlDoc.DocumentElement;
            }
            else
                xmlElem.SetAttribute("Value", "-1");
            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }

        /*--------------------------------------------------原加工VIS用---------------------------------------------------------------*/


        /*--------------------------------------------------代理商需求用---------------------------------------------------------------*/
        //OK
        /// <summary>
        /// 人員出站
        /// </summary>
        /// <param name="order">工單</param>
        /// <param name="machine">機台</param>
        /// <param name="staff">出站人員</param>
        /// <param name="product_number">料號</param>
        /// <param name="type">類型(進站報工/進佔維護)</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode Staff_Exit(string order, string machine, string staff, string product_number, string type)
        {
            string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            List<string> staff_list = new List<string>(staff.Split('#'));
            string sqlcmd = "";
            DataTable dt = new DataTable();
            int x = 0;
            if (staff_list.Count > 0)
            {
                //找尋對應之入站紀錄

                sqlcmd = $"select * from (  SELECT  *, (SELECT  MIN(a.now_time) FROM record_worktime a WHERE workman_status = '出站'  AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id) exit_time FROM record_worktime WHERE workman_status = '入站') a where a.exit_time IS null and product_number='{product_number}' and mach_name='{machine}' and type_mode = '{type}' and work_staff='{staff_list[1]}' and now_time <='{DateTime.Now:yyyyMMddHHmmss}' ";
                dt = cls_db.GetDataTable(sqlcmd);

                //找尋最大的ID

                sqlcmd = "SELECT max(_id) _id FROM record_worktime";
                DataTable dt_max = cls_db.GetDataTable(sqlcmd);
                int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["_id"].ToString()) + 1 : 1;


                if (HtmlUtil.Check_DataTable(dt))
                {
                    foreach (DataRow rows in dt.Rows)
                    {
                        DataRow row = HtmlUtil.Get_HeadRow(dt).NewRow();

                        row["mach_name"] = rows["mach_name"];

                        row["manu_id"] = rows["manu_id"];
                        row["product_number"] = rows["product_number"];
                        row["product_name"] = rows["product_name"];
                        row["work_staff"] = rows["work_staff"];
                        row["workman_status"] = "出站";
                        row["report_qty"] = rows["report_qty"];
                        row["qty_status"] = rows["qty_status"];
                        row["now_time"] = now_time;
                        row["now_qty"] = rows["now_qty"];
                        row["type_mode"] = type;

                        //新增成功後，要修改workorder_information的人員部分

                        if (cls_db.Insert_DataRow("record_worktime", row))
                        {
                            x++;

                            sqlcmd = $"select * from workorder_information where product_number='{product_number}' and  manu_id='{rows["manu_id"]}' and mach_name='{machine}' and work_staff like '%{staff_list[1]}%'";
                            DataTable dts = cls_db.GetDataTable(sqlcmd);

                            if (HtmlUtil.Check_DataTable(dt))
                            {
                                //找到員工代號與員工姓名的欄位
                                List<string> list_number = new List<string>(dts.Rows[0]["staff_Number"].ToString().Split('/'));
                                List<string> list_staff = new List<string>(dts.Rows[0]["work_staff"].ToString().Split('/'));

                                //移除
                                list_number.Remove(staff_list[0]);
                                list_staff.Remove(staff_list[1]);

                                //重組
                                string number = "";
                                string staffs = "";

                                for (int i = 0; i < list_staff.Count; i++)
                                {
                                    if (list_staff[i] != "")
                                    {
                                        number += number == "" ? list_number[i] : $"/{list_number[i]}";
                                        staffs += staffs == "" ? list_staff[i] : $"/{list_staff[i]}";
                                    }
                                }

                                //存入資料庫
                                DataRow rowss = dts.NewRow();
                                rowss["_id"] = dts.Rows[0]["_id"];
                                rowss["staff_Number"] = number;
                                rowss["work_staff"] = staffs;
                                rowss["last_updatetime"] = now_time;

                                cls_db.Update_DataRow("workorder_information", $" manu_id='{rows["manu_id"]}' and mach_name='{machine}' and work_staff like '%{staff_list[1]}%'", rowss);
                            }
                        }

                    }
                }
            }

            return Update_data(order, machine, type);
        }

        //OK
        /// <summary>
        /// 人員入站
        /// </summary>
        /// <param name="order">工單</param>
        /// <param name="machine">機台</param>
        /// <param name="staff">入站人員</param>
        /// <param name="product_number">料號</param>
        /// <param name="type">類型(進站報工/進佔維護)</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode Staff_Save(string order, string machine, string staff, string product_number, string type)
        {
            string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            List<string> staff_list = new List<string>(staff.Split('#'));
            List<string> copy_list = staff_list.ToList();


            string sqlcmd = $"select * from workorder_information where product_number='{product_number}' and mach_name='{machine}' and type_mode= '{type}'";
            DataTable dt = cls_db.GetDataTable(sqlcmd);
            int j = 0;

            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow rows in dt.Rows)
                {
                    //先存入Record_worktime
                    List<string> list_number = new List<string>(rows["staff_Number"].ToString().Split('/'));
                    List<string> list_staff = new List<string>(rows["work_staff"].ToString().Split('/'));

                    List<string> list_all = new List<string>();
                    list_all.AddRange(list_number);
                    list_all.AddRange(list_staff);

                    for (int i = 0; i < list_all.Count; i++)
                        if (staff_list.IndexOf(list_all[i]) != -1)
                            staff_list.Remove(list_all[i]);

                    //為了複製資訊
                    //mysql
                    //  sqlcmd = $"select * from record_worktime where manu_id = '{rows["manu_id"]}' and mach_name='{machine}' limit 1";
                    //mssql
                    sqlcmd = $"select TOP(1) * from record_worktime where manu_id = '{rows["manu_id"]}' and mach_name='{machine}' ";
                    DataTable dt_information = cls_db.GetDataTable(sqlcmd);

                    //找尋最大的ID

                    sqlcmd = "SELECT max(_id) _id FROM record_worktime";
                    DataTable dt_max = cls_db.GetDataTable(sqlcmd);
                    int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["_id"].ToString()) + 1 : 1;
                    if (HtmlUtil.Check_DataTable(dt_information))
                    {
                        for (int i = 0; i < staff_list.Count - 1; i++)
                        {

                            sqlcmd = $"select * from record_worktime where manu_id = '{rows["manu_id"]}' and mach_name='{machine}' and work_staff = '{staff_list[i + 1]}' and type_mode='{type}'";
                            DataTable dt_record = cls_db.GetDataTable(sqlcmd);
                            DataTable dt_copy = HtmlUtil.Get_HeadRow(dt_record);
                            if (dt_record != null)
                            {
                                DataRow row = dt_copy.NewRow();

                                row["mach_name"] = dt_information.Rows[0]["mach_name"];

                                row["manu_id"] = dt_information.Rows[0]["manu_id"];
                                row["product_number"] = dt_information.Rows[0]["product_number"];
                                row["product_name"] = dt_information.Rows[0]["product_name"];
                                row["work_staff"] = staff_list[i + 1];
                                row["workman_status"] = dt_information.Rows[0]["workman_status"];
                                row["report_qty"] = dt_information.Rows[0]["report_qty"];
                                row["qty_status"] = dt_information.Rows[0]["qty_status"];
                                row["now_time"] = now_time;
                                row["now_qty"] = dt_information.Rows[0]["now_qty"];
                                row["type_mode"] = type;

                                if (cls_db.Insert_DataRow("record_worktime", row))
                                {
                                    i++;
                                    j++;
                                }
                            }
                        }

                        //更新排程

                        sqlcmd = $"select * from workorder_information where manu_id = '{rows["manu_id"]}' and mach_name='{machine}'";
                        DataTable dts = cls_db.GetDataTable(sqlcmd);
                        if (HtmlUtil.Check_DataTable(dt))
                        {
                            List<string> history_staff = new List<string>(DataTableUtils.toString(dt.Rows[0]["history_workstaff"]).Split('/'));
                            string number = "";
                            string staffs = "";
                            string historystaff = DataTableUtils.toString(dt.Rows[0]["history_workstaff"]);
                            for (int i = 0; i < copy_list.Count - 1; i++)
                            {
                                number += number == "" ? copy_list[i] : $"/{copy_list[i]}";
                                staffs += staffs == "" ? copy_list[i + 1] : $"/{copy_list[i + 1]}";
                                if (history_staff.IndexOf(copy_list[i + 1]) == -1)
                                    historystaff += $"/{copy_list[i + 1]}";
                                i++;
                            }

                            DataRow row = dts.NewRow();
                            row["_id"] = dts.Rows[0]["_id"];
                            row["staff_Number"] = number;
                            row["work_staff"] = staffs;
                            row["history_workstaff"] = historystaff;
                            row["last_updatetime"] = now_time;

                            cls_db.Update_DataRow("workorder_information", $" manu_id = '{rows["manu_id"]}' and mach_name='{machine}'", row);
                        }
                    }
                }
            }
            return Update_data(order, machine, type);
        }

        //OK
        /// <summary>
        /// 人員出入站後更新
        /// </summary>
        /// <param name="order">工單</param>
        /// <param name="machine">機台</param>
        /// <param name="type">類型(進站報工/進佔維護)</param>
        /// <returns></returns>
        private XmlNode Update_data(string order, string machine, string type)
        {
            //mysql
            // string sqlcmd = $"SELECT  _id 工單報工, mach_name 讀取設備, custom_name 客戶名稱, (SELECT  area_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) 設備群組, mach_name 設備代號, (SELECT  mach_show_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) 設備名稱, manu_id 工單號碼, product_number 品號, product_name 品名, exp_product_count_day 預計產量, product_count_day 已生產量, ((SELECT  SUM(a.report_qty) FROM (SELECT DISTINCT  report_qty, now_time FROM record_worktime WHERE record_worktime.manu_id = workorder_information.manu_id AND record_worktime.mach_name = workorder_information.mach_name AND record_worktime.type_mode = workorder_information.type_mode AND SUBSTRING(record_worktime.now_time, 1, 8) = DATE_FORMAT(NOW(), '%Y%m%d')) a) + (CASE WHEN SUBSTRING(workorder_information.now_time, 1, 8) = DATE_FORMAT(NOW(), '%Y%m%d') THEN IFNULL(maintain_qty, 0) ELSE 0 END)) 今日產量, (exp_product_count_day - product_count_day) 未生產量, (product_count_day * 100 / exp_product_count_day) 進度, craft_Number 製程代號, craft_name 製程名稱, order_status 工單狀態, staff_Number 人員代號, work_staff 人員名稱, error_type 狀態, now_time 開工時間 FROM workorder_information WHERE (order_status <> '出站' OR order_status IS NULL)  and manu_id='{order}' and type_mode='{type}' and workorder_information.mach_name='{machine}'";
            //mssql
            string sqlcmd = $"SELECT _id 工單報工, mach_name 讀取設備, custom_name 客戶名稱, (SELECT area_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) 設備群組, mach_name 設備代號, (SELECT mach_show_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) 設備名稱, manu_id 工單號碼, product_number 品號, product_name 品名, exp_product_count_day 預計產量, product_count_day 已生產量, ( (SELECT SUM(cast(a.report_qty as int)) FROM (SELECT DISTINCT report_qty, now_time FROM record_worktime WHERE record_worktime.manu_id = workorder_information.manu_id AND record_worktime.mach_name = workorder_information.mach_name AND record_worktime.type_mode = workorder_information.type_mode AND SUBSTRING(record_worktime.now_time, 1, 8) = Convert(CHAR(8),GETDATE(),112) ) a) + (CASE WHEN SUBSTRING(workorder_information.now_time, 1, 8) = Convert(CHAR(8),GETDATE(),112)  THEN IsNULL(maintain_qty, 0) ELSE 0 END)) 今日產量, (cast(exp_product_count_day as int) - cast(product_count_day as int)) 未生產量, (cast(product_count_day as int) * 100 / cast(exp_product_count_day as int)) 進度, craft_Number 製程代號, craft_name 製程名稱, order_status 工單狀態, staff_Number 人員代號, work_staff 人員名稱, error_type 狀態, now_time 開工時間 FROM workorder_information WHERE (order_status <> '出站' OR order_status IS NULL)   and manu_id='{order}' and type_mode='{type}' and workorder_information.mach_name='{machine}'";
            DataTable dt = cls_db.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> list_number = new List<string>(dt.Rows[0]["人員代號"].ToString().Split('/'));
                List<string> list_staff = new List<string>(dt.Rows[0]["人員名稱"].ToString().Split('/'));
                string staffs = "";
                string workstaff = "";
                for (int i = 0; i < list_staff.Count; i++)
                {
                    staffs += $"{list_number[i]}#{list_staff[i]}#";
                    workstaff += $"{list_staff[i]}#";
                }

                xmlElem.SetAttribute("Value", order);
                xmlElem.SetAttribute("Machine", machine);
                //更新至TextBox內
                xmlElem.SetAttribute("人員清單", staffs);
                //更新至Dropdownlist內
                xmlElem.SetAttribute("人員列表", workstaff);
                xmlDoc.AppendChild(xmlElem);
                foreach (DataRow row in dt.Rows)
                {
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                    xmlElemA.SetAttribute("設備名稱", $"<center>{row["設備名稱"]}</center>");
                    xmlElemA.SetAttribute("工單號碼", $"<center>{row["工單號碼"]}</center>");
                    xmlElemA.SetAttribute("品號", $"<center>{row["品號"]}</center>");
                    xmlElemA.SetAttribute("品名", $"<center>{row["品名"]}</center>");
                    xmlElemA.SetAttribute("預計產量", $"<center>{row["預計產量"]}</center>");
                    xmlElemA.SetAttribute("已生產量", $"<center>{row["已生產量"]}</center>");
                    xmlElemA.SetAttribute("未生產量", $"<center>{DataTableUtils.toInt(row["預計產量"].ToString()) - DataTableUtils.toInt(row["已生產量"].ToString())}</center>");
                    xmlElemA.SetAttribute("製程名稱", $"<center>{row["製程名稱"]}</center>");
                    xmlElemA.SetAttribute("人員名稱", $"<center>{row["人員名稱"]}</center>");
                    xmlElemA.SetAttribute("今日產量", $"<center>{row["今日產量"]}</center>");
                    xmlElemA.SetAttribute("開工時間", $"<center>{row["開工時間"]}</center>");
                    xmlElemA.SetAttribute("進度", $"<center>{row["進度"]}</center>");
                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                }
                return xmlDoc.DocumentElement;
            }
            else
                xmlElem.SetAttribute("Value", "-1");
            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }

        //OK
        /// <summary>
        /// 定時更新機台資訊
        /// </summary>
        /// <param name="machine">機台</param>
        /// <param name="type">類型(進站報工/進佔維護/)</param>
        /// <returns></returns> 
        [WebMethod]
        public XmlNode GetMachineList(string machine, string type)
        {
            //先做資料上的更新動作
            string sqlcmd = "";
            DataTable dt = new DataTable();
            DataTable dt_finished;
            double X = 0;
            double nowQty = 0;
            string mach = "";
            double unMake = 0;
            double Count = 0;
            int lastFinishQty = 0;
            int lastNgQty = 0;

            //修正報工用(原定報工是不會跳數字-07/01 現在改也要跟著跳)
            foreach (string str in machine.Split('#'))
            {
                if (str != "")
                    mach += mach == "" ? $" workorder_information.mach_name='{str}' " : $" or  workorder_information.mach_name='{str}' ";

                sqlcmd = $"SELECT is_collect FROM machine_info WHERE mach_name='{str}'";
                dt = cls_db.DataTable_GetTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt) && DataTableUtils.toString(dt.Rows[0]["is_collect"]) == "是")//&& type == "進站報工")// 為進站報工走原路,非報工 走回數量(只要是機聯網就跳數字)
                {// if (HtmlUtil.Check_DataTable(dt) && DataTableUtils.toString(dt.Rows[0]["is_collect"]) == "是" && type == "進站報工")// 為進站報工走原路,非報工 走回數量(不跳數字)
                    Count = Save_AutoCount(str, type);
                    if (Count > 0)//表示關機或是異常 就不更新資料庫避免錯誤 0703 juedit
                        nowQty = Count;
                    else
                        nowQty = 0;//
                }
            }
            mach = mach == "" ? "" : $" and ( {mach} ) ";
            type = type == "" ? "" : $" and type_mode='{type}' ";
            //mysql
            //sqlcmd = $"select workorder_information.*, ((SELECT SUM(a.report_qty) FROM (SELECT DISTINCT report_qty, now_time FROM record_worktime WHERE record_worktime.manu_id = workorder_information.manu_id AND record_worktime.mach_name = workorder_information.mach_name AND record_worktime.type_mode = workorder_information.type_mode AND SUBSTRING(record_worktime.now_time, 1, 8) = DATE_FORMAT(NOW(), '%Y%m%d')) a) + (CASE WHEN SUBSTRING(workorder_information.now_time, 1, 8) = DATE_FORMAT(NOW(), '%Y%m%d') THEN IFNULL(maintain_qty, 0) ELSE 0 END)) 今日產量,   (product_count_day *100 / exp_product_count_day) 進度,mach_show_name,machine_info.area_name group_name from  workorder_information,machine_info where order_status <> '出站' {mach} {type} and workorder_information.mach_name = machine_info.mach_name ";

            //mssql
            //sqlcmd = $"SELECT workorder_information.*, ( (SELECT SUM(cast(a.report_qty as int)) FROM (SELECT DISTINCT report_qty, now_time FROM record_worktime WHERE record_worktime.manu_id = workorder_information.manu_id AND record_worktime.mach_name = workorder_information.mach_name AND record_worktime.type_mode = workorder_information.type_mode AND SUBSTRING(record_worktime.now_time, 1, 8) = Convert(CHAR(8),GETDATE(),112)) a) + (CASE WHEN SUBSTRING(workorder_information.now_time, 1, 8) =Convert(CHAR(8),GETDATE(),112) THEN IsNULL(maintain_qty, 0) ELSE 0 END)) 當下產量, (product_count_day *100 / exp_product_count_day) 進度,mach_show_name,  (select TOP(1) group_name  from mach_group where  mach_group.mach_name like CONCAT('%',workorder_information.mach_name,'%'))  group_name FROM workorder_information, machine_info WHERE order_status <> '出站'  {mach} {type} and workorder_information.mach_name = machine_info.mach_name";
            sqlcmd = $"SELECT workorder_information.*, Isnull((((SELECT Sum(Cast(a.report_qty AS INT) ) FROM (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM record_worktime WHERE record_worktime.manu_id = workorder_information.manu_id AND   record_worktime.mach_name = workorder_information.mach_name AND   record_worktime.type_mode = workorder_information.type_mode) a WHERE  a.workman_status='中途報工' ) )),0) 當下產量,Isnull((((SELECT Sum(Cast(a.report_qty AS INT))FROM   (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM   record_worktime WHERE  record_worktime.manu_id =workorder_information.manu_id AND record_worktime.mach_name =workorder_information.mach_name AND record_worktime.type_mode =workorder_information.type_mode) a WHERE  a.workman_status = '中途報工' and a.qty_status='良品'))), 0)  當下良品,Isnull((((SELECT Sum(Cast(a.report_qty AS INT))FROM   (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM   record_worktime WHERE  record_worktime.manu_id =workorder_information.manu_id AND record_worktime.mach_name =workorder_information.mach_name AND record_worktime.type_mode =workorder_information.type_mode) a WHERE  a.workman_status = '中途報工' and a.qty_status='不良品'))), 0)  當下不良品, (product_count_day *100 / exp_product_count_day) 進度,mach_show_name,  (select TOP(1) group_name  from mach_group where  mach_group.mach_name like CONCAT('%',workorder_information.mach_name,'%'))  group_name FROM workorder_information, machine_info WHERE order_status <> '出站'  {mach} {type} and workorder_information.mach_name = machine_info.mach_name";
            dt = cls_db.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            int j = 0;

            if (HtmlUtil.Check_DataTable(dt))
            {
                xmlElem.SetAttribute("Value", type);
                xmlDoc.AppendChild(xmlElem);


                foreach (DataRow row in dt.Rows)
                {
                    double FinshEdQty = 0;
                    double unFinishQty = 0;
                    double progress = 0;
                    //string now_information = $"{row["mach_show_name"]}^{row["manu_id"]}^{row["product_number"]}^{row["product_name"]}^{row["exp_product_count_day"]}^{row["product_count_day"]}^{DataTableUtils.toDouble(row["當下產量"])}^{row["no_product_count_day"]}^{row["進度"]}%^{HtmlUtil.StrToDate(row["now_time"].ToString()):yyyy/MM/dd HH:mm:ss}^{row["craft_name"]}^{row["work_staff"]}^";
                    string now_information = "";// $"{row["mach_show_name"]}^{row["manu_id"]}^{row["product_number"]}^{row["product_name"]}^{row["exp_product_count_day"]}^{DataTableUtils.toDouble(row["當下產量"])}^{row["product_count_day"]}^{row["no_product_count_day"]}^{row["進度"]}%^{HtmlUtil.StrToDate(row["now_time"].ToString()):yyyy/MM/dd HH:mm:ss}^{row["craft_name"]}^{row["work_staff"]}^";
                    List<string> staff_number = new List<string>(row["staff_Number"].ToString().Split('/'));
                    List<string> staff_name = new List<string>(row["work_staff"].ToString().Split('/'));
                    string staff = "";
                    for (int i = 0; i < staff_number.Count; i++)
                        staff += $"{staff_number[i]}#{staff_name[i]}#";
                    string light = "";
                    string orderno = row["orderno"].ToString();
                    string bad_information = "";
                    double ok_qty = 0;
                    double ng_qty = 0;

                    //抓到不良品的資訊(以出站的)
                    //mysql
                    //  sqlcmd = $"select bad_type,sum(bad_qty) bad_qty,bad_content from bad_total where mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and now_time >= '{row["now_time"]}' {type} group by bad_type,bad_content";
                    //mssql
                    sqlcmd = $"select bad_type,sum(cast(bad_qty as int)) bad_qty,bad_content from bad_total where mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and orderno='{row["orderno"]}' and now_time >= '{row["now_time"]}' {type} group by bad_type,bad_content";
                    DataTable dts = cls_db.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dts))
                        foreach (DataRow rew in dts.Rows)
                            bad_information += $"{rew["bad_type"]}Ω{rew["bad_qty"]}Ω{rew["bad_content"]}Ω";
                    lastFinishQty = DataTableUtils.toInt(row["當下良品"].ToString());
                    lastNgQty = DataTableUtils.toInt(row["當下不良品"].ToString());
                     ////取得最近的完工紀錄 為了取得最近一次不良品數
                    //sqlcmd = $"select * from record_worktime where workman_status = '中途報工' and mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and type_mode='進站報工' and manu_id='{row["manu_id"].ToString()}' and  qty_status='不良品'";
                    //dt_lastFinishNG = cls_db.GetDataTable(sqlcmd);
                    //if (HtmlUtil.Check_DataTable(dt_lastFinishNG))
                    //    lastNgQty = DataTableUtils.toInt(dt_lastFinishNG.Rows[0]["report_qty"].ToString());
                    sqlcmd = $"select * from workorder_information where order_status = '入站' and mach_name = '{row["mach_name"]}' and orderno='{row["orderno"]}' and product_number = '{row["product_number"]}' {type}";
                    dts = cls_db.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dts))
                    {
                        foreach (DataRow rew in dts.Rows)
                        {
                            //取得所有的良品數量
                            ok_qty += DataTableUtils.toDouble(rew["good_qty"]);

                            //取得所有不良品的數量
                            ng_qty += DataTableUtils.toDouble(rew["bad_qty"]);
                        }
                    }
                    sqlcmd = $"SELECT is_collect FROM machine_info WHERE mach_name='{row["mach_name"]}'";
                    dt = cls_db.GetDataTable(sqlcmd);
                    string collect = "否";
                    collect = HtmlUtil.Check_DataTable(dt) ? DataTableUtils.toString(dt.Rows[0]["is_collect"]) : collect;
                    if (DataTableUtils.toString(row["error_type"]) == "ERROR")
                        light = $"<img src=\"../../assets/images/Light_ExStopping.PNG\"  width=\"50px\" height=\"50px\"  />";
                    else if (DataTableUtils.toString(row["error_type"]) != "ERROR" && DataTableUtils.toString(row["error_type"]) != "")
                        light = $"<img src=\"../../assets/images/Light_Stopping.PNG\"  width=\"50px\" height=\"50px\"  />";
                    else if (DataTableUtils.toString(row["type_mode"]) == "進站維護" && type == "")
                        light = $"<img src=\"../../assets/images/Light_Maintain.PNG\"  width=\"50px\" height=\"50px\"  />";
                    else
                        light = $"<img src=\"../../assets/images/Light_Running.png\"  width=\"50px\" height=\"50px\"  />";
                    //表格顯示資訊
                    //FinshEdQty = DataTableUtils.toInt(row["product_count_day"].ToString()) - DataTableUtils.toInt(row["當下產量"].ToString());
                    //取得最新的出站累積數量
                    sqlcmd = $"select * from workorder_information　where manu_id='{row["manu_id"]}' and mach_name='{row["mach_name"]}' and product_number='{DataTableUtils.toString(row["product_number"])}' and orderno='{DataTableUtils.toString(row["orderno"])}' and order_status='出站' order by　now_time desc ";
                    dt_finished = cls_db.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt_finished))
                    {
                        var Count_f = dt_finished.AsEnumerable().Select(s => s.Field<string>("product_count_day")).FirstOrDefault();
                        FinshEdQty =DataTableUtils.toDouble( Count_f);
                    }

                    unFinishQty = DataTableUtils.toInt(row["exp_product_count_day"].ToString()) - FinshEdQty;
                    progress = unFinishQty>0? DataTableUtils.toInt(row["當下產量"].ToString()) * 100 / unFinishQty:100;
                    //now_information = $"{row["mach_show_name"]}^{row["manu_id"]}^{row["product_number"]}^{row["product_name"]}^{row["exp_product_count_day"]}^{DataTableUtils.toDouble(row["當下產量"])}^{row["product_count_day"]}^{row["no_product_count_day"]}^{row["進度"]}%^{HtmlUtil.StrToDate(row["now_time"].ToString()):yyyy/MM/dd HH:mm:ss}^{row["craft_name"]}^{row["work_staff"]}^";
                    now_information = $"{row["mach_show_name"]}^{row["manu_id"]}^{row["product_number"]}^{row["product_name"]}^{row["exp_product_count_day"]}^{DataTableUtils.toDouble(row["當下產量"])}^{FinshEdQty.ToString()}^{unFinishQty.ToString()}^{progress.ToString()}%^{HtmlUtil.StrToDate(row["now_time"].ToString()):yyyy/MM/dd HH:mm:ss}^{row["craft_name"]}^{row["work_staff"]}^";
                    //X = DataTableUtils.toDouble(row["今日產量"]);
                    //nowQty +加上調整量  不然會每次都落差 juiedit
                    if (collect != "否")
                        nowQty = nowQty + DataTableUtils.toDouble(row["adj_qty"].ToString());
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");//lastFinishQty
                    //xmlElemA.SetAttribute("工單報工", $"<a href=\"javascript:void(0)\" ><img src=\"../../assets/images/canclick.png\"  width=\"50px\" height=\"50px\" data-toggle = \"modal\" data-target = \"#Report_Model\" onclick=Set_Information(\"{row["group_name"]}\",\"{row["mach_show_name"]}\",\"{row["order_status"]}\",\"{row["product_number"].ToString().Trim()}\",\"{row["manu_id"].ToString().Trim()}\",\"{row["mach_name"].ToString().Trim()}\",\"{now_information.Replace(' ', '*')}\",\"{staff}\",\"{row["error_type"]}\",\"{bad_information}\",\"{ok_qty}\",\"{ng_qty}\",\"{collect}\",\"{orderno}\") /></a>");
                    xmlElemA.SetAttribute("工單報工", $"<a href=\"javascript:void(0)\" ><img src=\"../../assets/images/canclick.png\"  width=\"50px\" height=\"50px\" data-toggle = \"modal\" data-target = \"#Report_Model\" onclick=Set_Information(\"{row["group_name"]}\",\"{row["mach_show_name"]}\",\"{row["order_status"]}\",\"{row["product_number"].ToString().Trim()}\",\"{row["manu_id"].ToString().Trim()}\",\"{row["mach_name"].ToString().Trim()}\",\"{now_information.Replace(' ', '*')}\",\"{staff}\",\"{row["error_type"]}\",\"{bad_information}\",\"{lastFinishQty.ToString()}\",\"{ng_qty}\",\"{collect}\",\"{orderno}\") /></a>");
                    xmlElemA.SetAttribute("設備群組", $"{row["group_name"]}");
                    xmlElemA.SetAttribute("設備代號", $"{row["mach_name"]}");
                    xmlElemA.SetAttribute("客戶名稱", $"{row["custom_name"]}");
                    xmlElemA.SetAttribute("設備名稱", $"{row["mach_show_name"]}");
                    xmlElemA.SetAttribute("工單號碼", $"{row["manu_id"]}");
                    xmlElemA.SetAttribute("品號", $"{row["product_number"]}");
                    xmlElemA.SetAttribute("品名", $"{row["product_name"]}");
                    xmlElemA.SetAttribute("預計產量", $"{row["exp_product_count_day"]}");
                    xmlElemA.SetAttribute("已生產量", $"{ProcessFinshQty(row, type, collect, nowQty.ToString())}");
                    //處理未生產量不要負數
                    unMake = DataTableUtils.toDouble(row["exp_product_count_day"].ToString()) - DataTableUtils.toDouble(row["product_count_day"].ToString());
                    unMake = unMake > 0 ? unMake : 0;
                    //新未生產量=不管當下 只管有出站的  需求-已生產(出站) 
                    //xmlElemA.SetAttribute("未生產量", $"{DataTableUtils.toDouble(row["exp_product_count_day"].ToString()) - DataTableUtils.toDouble(row["product_count_day"].ToString())}");
                    //unFinishQty = ProcessUnFinishQty(row, type, collect, nowQty.ToString());
                    xmlElemA.SetAttribute("未生產量", $"{unFinishQty.ToString()}");
                    //xmlElemA.SetAttribute("進度", $"{DataTableUtils.toDouble(row["進度"]):0}%");
                    xmlElemA.SetAttribute("進度", unFinishQty>0? $"{(DataTableUtils.toDouble(row["當下產量"]) * 100 / DataTableUtils.toDouble(unFinishQty)).ToString():0}%":"100%");
                    xmlElemA.SetAttribute("製程代號", $"{row["craft_Number"]}");
                    xmlElemA.SetAttribute("當下產量", $"{DataTableUtils.toDouble(row["當下產量"])}");//從SQL改了
                    xmlElemA.SetAttribute("製程名稱", $"{row["craft_name"]}");
                    xmlElemA.SetAttribute("工單狀態", light);
                    xmlElemA.SetAttribute("人員名稱", $"{row["work_staff"]}");
                    xmlElemA.SetAttribute("開工時間", $"{HtmlUtil.StrToDate(row["now_time"].ToString()):yyyy/MM/dd HH:mm:ss}");
                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                    j++;
                }
                return xmlDoc.DocumentElement;
            }
            else
                xmlElem.SetAttribute("Value", "-1");

            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }
        //修正數量 07/07
        //p
        private string ProcessFinshQty(DataRow row, string type, string collect, string nowQty)
        {
            //
            if (collect != "是")
            {
                //非機聯網的
                string sqlcmd = $"select product_count_day  as 已完成數量 from workorder_information where order_Status='出站' and mach_name='{row["mach_name"]}' and manu_id='{row["manu_id"]}' and product_number='{row["product_number"]}' and orderno='{row["orderno"]}' order by last_updatetime desc";
                DataTable dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    var Count = dt.AsEnumerable().FirstOrDefault();
                    return Count["已完成數量"].ToString();
                    //row["已生產量"] = Count["已完成數量"].ToString();
                    //row["未生產量"] = (DataTableUtils.toInt(dt_monthtotal.Rows[i]["預計產量"].ToString()) - DataTableUtils.toInt(Count["已完成數量"].ToString())).ToString();
                    //row["進度"] = (DataTableUtils.toInt(dt_monthtotal.Rows[i]["當下產量"].ToString()) * 100 / DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString())).ToString();
                }
                return nowQty;

            }
            else//機聯網的
                return (DataTableUtils.toDouble(row["product_count_day"]) + DataTableUtils.toDouble(row["adj_qty"].ToString())).ToString();
            //
            //if (!string.IsNullOrEmpty(type))
            //{
            //    if (collect != "是")
            //        return DataTableUtils.toDouble(row["product_count_day"]).ToString();
            //    else
            //        return (DataTableUtils.toDouble(row["product_count_day"]) + DataTableUtils.toDouble(row["adj_qty"].ToString())).ToString();
            //}
            //else  //nowQty
            //{
            //    if (collect != "是")
            //        return nowQty;
            //    else
            //        return $"{DataTableUtils.toDouble(row["product_count_day"]) + DataTableUtils.toDouble(row["adj_qty"].ToString())}";
            //}
        }
        private string ProcessUnFinishQty(DataRow row, string type, string collect, string nowQty)
        {
            //
            if (collect != "是")
            {
                //非機聯網的
                string sqlcmd = $"select product_count_day  as 已完成數量 from workorder_information where order_Status='出站' and mach_name='{row["mach_name"]}' and manu_id='{row["manu_id"]}' and product_number='{row["product_number"]}' and orderno='{row["orderno"]}' order by last_updatetime desc";
                DataTable dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    var Count = dt.AsEnumerable().FirstOrDefault();
                    //return Count["已完成數量"].ToString();
                    //row["已生產量"] = Count["已完成數量"].ToString();
                    return (DataTableUtils.toInt(row["exp_product_count_day"].ToString()) - DataTableUtils.toInt(Count["已完成數量"].ToString())).ToString();
                    //row["進度"] = (DataTableUtils.toInt(dt_monthtotal.Rows[i]["當下產量"].ToString()) * 100 / DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString())).ToString();
                }
                return nowQty;

            }
            else//機聯網的
                return $"{DataTableUtils.toDouble(row["exp_product_count_day"].ToString()) - DataTableUtils.toDouble(row["product_count_day"].ToString())}";
        }
        private string ProcessRightNowQty(DataRow row, string type, string collect, string nowQty)
        {
            //
            if (collect != "是")
            {
                //非機聯網的
                string sqlcmd = $"select product_count_day  as 已完成數量 from workorder_information where order_Status='出站' and mach_name='{row["mach_name"]}' and manu_id='{row["manu_id"]}' and product_number='{row["product_number"]}' and orderno='{row["orderno"]}' order by last_updatetime desc";
                DataTable dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    var Count = dt.AsEnumerable().FirstOrDefault();
                    //return Count["已完成數量"].ToString();
                    //row["已生產量"] = Count["已完成數量"].ToString();
                    return (DataTableUtils.toInt(row["exp_product_count_day"].ToString()) - DataTableUtils.toInt(Count["已完成數量"].ToString())).ToString();
                    //row["進度"] = (DataTableUtils.toInt(dt_monthtotal.Rows[i]["當下產量"].ToString()) * 100 / DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString())).ToString();
                }
                return nowQty;

            }
            else//機聯網的
                return $"{DataTableUtils.toDouble(row["exp_product_count_day"].ToString()) - DataTableUtils.toDouble(row["product_count_day"].ToString())}";
        }
        //OK
        /// <summary>
        /// 自動更新數量
        /// </summary>
        /// <param name="machine">機台代號</param>
        /// <param name="type">類型(進站報工/進站維護)</param>
        private double Save_AutoCount(string machine, string type)
        {
            //儲存不良陣列
            List<string> bad_list = new List<string>();
            //良品初始值
            double OK_qty = 0;
            //不良初始值
            double NG_qty = 0;
            //除數
            string multiplication = "1";
            //被除數
            string division = "1";

            if (!string.IsNullOrEmpty(type))
            {
                //進行機報工的數量更新，良品與不良品的數量分配

                //取得當前機台的工單
                //MYSQL
                //string sqlcmd = $"SELECT  workorder_information.*, ifnull(a.good_qty ,0) report_good, ifnull(a.bad_qty ,0) report_bad FROM workorder_information LEFT JOIN (SELECT  * FROM workorder_information where order_status = '出站' order by now_time desc limit 1) a  ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number and a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE workorder_information.mach_name = '{machine}' AND workorder_information.type_mode = '{type}' AND workorder_information.order_status <> '出站' ORDER BY workorder_information.delivery ASC";
                //MSSQL
                string sqlcmd = $"SELECT  workorder_information.*, isnull(a.good_qty ,0) report_good, isnull(a.bad_qty ,0) report_bad,(SELECT is_collect   FROM machine_info   WHERE machine_info.mach_name = workorder_information.mach_name) is_collect FROM workorder_information LEFT JOIN (SELECT TOP(1) * FROM workorder_information where order_status = '出站' order by now_time desc ) a  ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number and a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE workorder_information.mach_name = '{machine}' AND workorder_information.type_mode = '{type}' AND workorder_information.order_status <> '出站' ORDER BY workorder_information.delivery ASC";
                DataTable dt = cls_db.GetDataTable(sqlcmd);

                //現在時間
                string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (HtmlUtil.Check_DataTable(dt))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        //取出對應的不良資訊 
                        sqlcmd = $"select * from bad_total where mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}' and type_mode='{type}'";
                        DataTable dt_bad = cls_db.GetDataTable(sqlcmd);

                        if (HtmlUtil.Check_DataTable(dt_bad))
                        {
                            //存入LIST內
                            foreach (DataRow rew in dt_bad.Rows)
                            {
                                bad_list.Add(DataTableUtils.toString(rew["bad_type"]));
                                bad_list.Add(DataTableUtils.toString(rew["bad_qty"]));
                                bad_list.Add(DataTableUtils.toString(rew["bad_content"]));
                                NG_qty += DataTableUtils.toDouble(rew["bad_qty"]);
                            }
                            bad_list.Add("");
                            //刪除
                            cls_db.Delete_Record("bad_total", $"mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}'");
                        }

                        //取出對應的紀錄資訊
                        sqlcmd = $"select * from record_worktime where mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}' and type_mode='{type}' and workman_status = '中途報工'";
                        DataTable dt_record = cls_db.GetDataTable(sqlcmd);

                        //刪除對應的紀錄資訊
                        if (HtmlUtil.Check_DataTable(dt_record))
                            cls_db.Delete_Record("record_worktime", $"mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}' and type_mode='{type}' and workman_status = '中途報工'");

                        row["product_count_day"] = DataTableUtils.toDouble(row["maintain_qty"]) + DataTableUtils.toDouble(row["report_good"]) + DataTableUtils.toDouble(row["report_bad"]);
                        row["no_product_count_day"] = DataTableUtils.toDouble(row["exp_product_count_day"]) - DataTableUtils.toDouble(row["maintain_qty"]) - DataTableUtils.toDouble(row["report_bad"]) - DataTableUtils.toDouble(row["report_good"]);
                        row["good_qty"] = DataTableUtils.toDouble(row["report_good"]);
                        row["bad_qty"] = DataTableUtils.toDouble(row["report_bad"]);
                        row["adj_qty"] = DataTableUtils.toDouble(row["adj_qty"]);
                    }
                    //加入輸入之良品數量
                    //double dd = DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["multiplication"]));
                    //double ddd = DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["division"]));

                    OK_qty += DataTableUtils.toDouble(GetCurrent_Qtys(machine)) * DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["multiplication"])) / DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["division"]));
                    //除數 取到整數即可 juiedit
                    if (CNCReport.Order_OKExit(dt, (int)OK_qty, now_time, type, "", false))
                    {
                        //重新搜尋
                        sqlcmd = $"select * from workorder_information where mach_name='{machine}' and type_mode='{type}' and order_status ='入站' order by delivery asc ";
                        dt = cls_db.GetDataTable(sqlcmd);

                        //進行不良品分配
                        if (CNCReport.Order_NGExit(dt, NG_qty, now_time, type, false, bad_list))
                        {

                        }
                    }
                }
                return DataTableUtils.toDouble(OK_qty);
            }
            else//type=""-->all_schdule  0523-juiedit
                return DataTableUtils.toDouble(GetCurrent_Qtys(machine));
        }

        //OK
        /// <summary>
        /// 取得該機台資料(報工類型用)
        /// </summary>
        /// <param name="mach">機台</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetMachine(string mach)
        {

            string sqlcmd = $"select machine_info.*,mach_group.group_name from machine_info left join mach_group on  mach_group.mach_name like CONCAT('%',machine_info.mach_name,'%') where machine_info.mach_name = '{mach}'";
            DataTable dt = cls_db.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt))
            {
                xmlElem.SetAttribute("Value", mach);
                xmlDoc.AppendChild(xmlElem);

                XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                xmlElemA.SetAttribute("設備代號", DataTableUtils.toString(dt.Rows[0]["mach_name"]));
                xmlElemA.SetAttribute("設備群組", DataTableUtils.toString(dt.Rows[0]["group_name"]));
                xmlElemA.SetAttribute("設備名稱", DataTableUtils.toString(dt.Rows[0]["mach_show_name"]));
                xmlElemA.SetAttribute("是否採集", DataTableUtils.toString(dt.Rows[0]["is_collect"]));
                xmlDoc.DocumentElement.AppendChild(xmlElemA);
                return xmlDoc.DocumentElement;
            }
            else
                return null;

            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }

        //OK
        /// <summary>
        /// 取得該機台已入站且未出站資料
        /// </summary>
        /// <param name="mach"></param>
        /// <param name="type">類型(進站報工/進佔維護)</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetMachineOrder(string mach, string type)
        {

            string sqlcmd = $"select * from workorder_information where order_status <> '出站' and mach_name = '{mach}' and type_mode= '{type}'";
            DataTable dt = cls_db.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt))
            {
                xmlElem.SetAttribute("Value", type);
                xmlDoc.AppendChild(xmlElem);
                foreach (DataRow row in dt.Rows)
                {
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                    xmlElemA.SetAttribute("工單號碼", DataTableUtils.toString(row["_id"]));
                    xmlElemA.SetAttribute("品號", DataTableUtils.toString(row["product_number"]));
                    xmlElemA.SetAttribute("品名", DataTableUtils.toString(row["product_name"]));
                    xmlElemA.SetAttribute("規格", DataTableUtils.toString(row["specification"]));
                    xmlElemA.SetAttribute("加工順序", DataTableUtils.toString(row["orderno"]));
                    xmlElemA.SetAttribute("預計產量", DataTableUtils.toString(row["exp_product_count_day"]));
                    xmlElemA.SetAttribute("已生產量", DataTableUtils.toString(row["product_count_day"]));
                    xmlElemA.SetAttribute("未生產量", DataTableUtils.toString(row["no_product_count_day"]));
                    xmlElemA.SetAttribute("預交日期", HtmlUtil.changetimeformat(DataTableUtils.toString(row["delivery"])));
                    xmlElemA.SetAttribute("製程代號", DataTableUtils.toString(row["craft_Number"]));
                    xmlElemA.SetAttribute("製程名稱", DataTableUtils.toString(row["craft_name"]));
                    xmlElemA.SetAttribute("標準工時", DataTableUtils.toString(row["standardtime"]));
                    xmlElemA.SetAttribute("客戶代號", DataTableUtils.toString(row["custom_number"]));
                    xmlElemA.SetAttribute("客戶名稱", DataTableUtils.toString(row["custom_name"]));
                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                }
            }
            else
                return null;

            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }

        //OK
        /// <summary>
        /// 取得工單資料(來源API)
        /// </summary>
        /// <param name="ordernumber">工單號碼</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetOrderInfromation(string ordernumber, string machine)
        {
            string Newordernumber = Regex.Replace(ordernumber, @"\s", "");
            List<string> orderlist = new List<string>(Newordernumber.Trim().Split('-'));
            string url = "";
            string json = "";
            DataTable dt;
            try
            {
                if (orderlist.Count == 2)
                    url = string.Format(HtmlUtil.Get_Ini("Parameter", "Get_orderapi"), orderlist[0], orderlist[1], "", "");
                else if (orderlist.Count == 3)
                    url = string.Format(HtmlUtil.Get_Ini("Parameter", "Get_orderapi"), orderlist[0], orderlist[1], orderlist[2], "");
                else if (orderlist.Count == 4)
                    url = string.Format(HtmlUtil.Get_Ini("Parameter", "Get_orderapi"), orderlist[0], orderlist[1], orderlist[2], orderlist[3]);
                //url = url.Replace("BBE","CCC");//製造error
                json = JsonToDataTable.HttpGetJson(url);
                json = json.Replace("\\", "").TrimStart('\"').TrimEnd('\"');
                if (!checkError(json))
                {
                    // DataTable dt = JsonToDataTable.JsonStringToDataTable(json);
                    dt = JsonToDataTable.JsonStringToDataTable_Cs<OrderFormat>(json);
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
                    if (HtmlUtil.Check_DataTable(dt))
                    {
                        xmlElem.SetAttribute("Value", Newordernumber);
                        xmlDoc.AppendChild(xmlElem);
                        //進報工一定要出站才可以再入維護 05/30
                        if (WebUtils.GetAppSettings("Inbound_limit") == "1" && IsInbound(Newordernumber, machine))
                            return ReturnErrResoult("進維護前要先出站!");

                        XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                        xmlElemA.SetAttribute("工單號碼", Newordernumber.Trim());
                        xmlElemA.SetAttribute("品號", DataTableUtils.toString(dt.Rows[0]["ItemNum"]));
                        xmlElemA.SetAttribute("品名", DataTableUtils.toString(dt.Rows[0]["ItemName1"]).Replace('#', ','));
                        xmlElemA.SetAttribute("規格", DataTableUtils.toString(dt.Rows[0]["ItemName2"]));
                        xmlElemA.SetAttribute("加工順序", orderlist.Count >= 3 ? orderlist[2] : "");
                        xmlElemA.SetAttribute("預計產量", DataTableUtils.toString(dt.Rows[0]["BoughtCounts"]));
                        xmlElemA.SetAttribute("已生產量", DataTableUtils.toString(dt.Rows[0]["DoneCounts"]));
                        xmlElemA.SetAttribute("未生產量", (DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["BoughtCounts"])) - DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["DoneCounts"]))).ToString());
                        xmlElemA.SetAttribute("預交日期", HtmlUtil.changetimeformat(DataTableUtils.toString(dt.Rows[0]["DayTime"])));
                        xmlElemA.SetAttribute("製程代號", DataTableUtils.toString(dt.Rows[0]["ProcessCode"]));
                        xmlElemA.SetAttribute("製程名稱", DataTableUtils.toString(dt.Rows[0]["ProcessName"]));
                        xmlElemA.SetAttribute("標準工時", DataTableUtils.toString(dt.Rows[0]["StandardTime"]));
                        xmlElemA.SetAttribute("客戶代號", DataTableUtils.toString(dt.Rows[0]["CustomerID"]));
                        xmlElemA.SetAttribute("客戶名稱", DataTableUtils.toString(dt.Rows[0]["CustomerName"]));
                        xmlElemA.SetAttribute("製程型態", DataTableUtils.toString(dt.Rows[0]["ProcessType"]));
                        //xmlElemA.SetAttribute("製程型態", "2");
                        xmlElemA.SetAttribute("ERROR", "0");
                        xmlDoc.DocumentElement.AppendChild(xmlElemA);
                        return xmlDoc.DocumentElement;
                    }
                    else//沒這個工單
                        return ReturnErrResoult($"工單-{Newordernumber}-不存在");
                }
                else//錯誤訊息
                {
                    dt = JsonToDataTable.JsonStringToDataTable(json);
                    return ReturnErrResoult(dt.Rows[0]["ER"].ToString(), Newordernumber);
                }
            }
            catch (Exception ex)
            {
                string eemsg = ex.Message;
                return ReturnErrResoult("資料異常!");
            }
        }
        /// <summary>
        /// 綁定報工只有出站的狀態  才可以再進入維護
        /// </summary>
        /// <param name="manuid">工單號碼</param>
        /// <returns></returns>
        //OK
        private bool IsInbound(string manuid, string machid)
        {
            //取得目前資料庫
            string InboundRow = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $" select * from workorder_information where manu_id='{manuid}' and type_mode='進站報工' and mach_name='{machid}' order by now_time desc";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                InboundRow = dt.Rows[0]["order_status"].ToString();
                if (InboundRow == "暫停" || InboundRow == "入站")//出站  才可以再進入維護---05/30 juiedit
                    return true;
                else
                    return false;
            }
            else
                return false;//空白也是表示可以新增
        }
        private bool checkError(string Jason)
        {
            bool Error = false;
            if (Jason.Length > 3 && Jason.Substring(0, 3).ToUpper().Contains("ERR"))
                Error = true;
            return Error;
        }
        private XmlElement ReturnErrResoult(string Msg, string ErrNum = "401")
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            xmlElem.SetAttribute("Value", ErrNum);
            xmlDoc.AppendChild(xmlElem);
            XmlElement xmlElemA = xmlDoc.CreateElement("Group");
            xmlElemA.SetAttribute("ERROR", Msg);
            xmlDoc.DocumentElement.AppendChild(xmlElemA);
            return xmlDoc.DocumentElement;
        }
        /// <summary>
        /// 取得人員資料(來源API/文聖資料庫)
        /// </summary>
        /// <param name="staffNumber">人員代碼</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetStaff(string staffNumber)
        {
            //複製人員資料到cnc_db內
            Import_Data();

            string url = string.Format(HtmlUtil.Get_Ini("Parameter", "Get_staffapi"), staffNumber);
            string json = JsonToDataTable.HttpGetJson(url);
            DataTable dt = JsonToDataTable.JsonStringToDataTable(json);

            if (!HtmlUtil.Check_DataTable(dt))
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                string sqlcmd = $" select staff_name UserName from staff_info where staff_no = '{staffNumber}' ";
                dt = DataTableUtils.GetDataTable(sqlcmd);
            }

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt))
            {
                xmlElem.SetAttribute("Value", staffNumber);
                xmlDoc.AppendChild(xmlElem);

                XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                xmlElemA.SetAttribute("人員代號", staffNumber);
                xmlElemA.SetAttribute("人員姓名", DataTableUtils.toString(dt.Rows[0]["UserName"]));
                xmlDoc.DocumentElement.AppendChild(xmlElemA);
                return xmlDoc.DocumentElement;
            }
            else
                return null;
        }

        //OK
        /// <summary>
        /// 把ERP內的所有人員，寫入mdc的資料庫
        /// </summary>
        private void Import_Data()
        {
            //從API取得人員資料
            string url = string.Format(HtmlUtil.Get_Ini("Parameter", "Get_staffapi"), "");
            string json = JsonToDataTable.HttpGetJson(url);
            DataTable dt = JsonToDataTable.JsonStringToDataTable(json);

            //取得目前資料庫
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = " select * from staff_info  order by _id desc";
            DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt) && ds != null)
            {
                int max = HtmlUtil.Check_DataTable(ds) ? Convert.ToInt32(ds.Compute("max([_id])", string.Empty)) + 1 + 1 : 1;
                if (dt.Rows.Count > ds.Rows.Count)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow rows = ds.NewRow();
                        rows["_id"] = max;
                        rows["staff_no"] = row["UserID"];
                        rows["staff_name"] = row["UserName"];
                        ds.Rows.Add(rows);
                        max++;
                    }
                    cls_db.Delete_Record("staff_info", "_id <> 0");
                    cls_db.Insert_TableRows("staff_info", ds);
                }
            }
        }

        /// <summary>
        /// 重置機台計數數量
        /// </summary>
        /// <param name="mach"></param>
        /// <returns></returns>
        [WebMethod]
        public bool CNCCountInit(string mach)
        {
            DataSet Tags = new DataSet("Tags");
            DataTable dt = new DataTable();
            DataRow dr;
            dt.TableName = "Tags";
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");
            dr = dt.NewRow();
            dr["Name"] = mach;
            dr["Value"] = "0";
            dt.Rows.Add(dr);
            Tags.Tables.Add(dt);
            var json = JsonConvert.SerializeObject(Tags);
            Task<DataTable> ts = Task.Run(() => CNCReport.PostDs2Api(Tags, "Post_MachCountInit"));
            ts.Wait();
            DataTable dt_resoult = ts.Result;
            if (dt_resoult != null && dt_resoult.Rows.Count != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 取得目前指定機台已報工個數(來源API)
        /// </summary>
        /// <param name="Curreny_Qty">人員代碼</param>
        /// <returns></returns>   
        [WebMethod]
        public XmlNode GetCurrent_Qty(string mach)
        {
            //Get_MachCount
            DataSet Tags = new DataSet("Tags");
            DataTable dt = new DataTable();
            DataRow dr;
            dt.TableName = "Tags";
            dt.Columns.Add("Name");
            dr = dt.NewRow();
            dr["Name"] = mach;
            dt.Rows.Add(dr);
            Tags.Tables.Add(dt);
            var json = JsonConvert.SerializeObject(Tags);
            //
            Task<DataTable> Rs = Task.Run(() => CNCReport.PostDs2Api(Tags, "Get_MachCount"));
            Rs.Wait();
            DataTable dt_resoult = Rs.Result;

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt_resoult))
            {
                xmlElem.SetAttribute("Value", mach);
                xmlDoc.AppendChild(xmlElem);
                XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                //拆解出數字 回數字就好
                xmlElemA.SetAttribute("結果狀態", DataTableUtils.toString(dt_resoult.Rows[0]["Result"]));
                xmlElemA.SetAttribute("總共筆數", DataTableUtils.toString(dt_resoult.Rows[0]["Total"]));
                xmlElemA.SetAttribute("機台編號", DataTableUtils.toString(dt_resoult.Rows[0]["Values"]));
                xmlElemA.SetAttribute("目前數量", DataTableUtils.toString(dt_resoult.Rows[0]["Value"]));
                xmlElemA.SetAttribute("需求數量", DataTableUtils.toString(dt_resoult.Rows[0]["Quality"]));
                xmlDoc.DocumentElement.AppendChild(xmlElemA);
                return xmlDoc.DocumentElement;
            }
            else
                return null;
        }

        private string GetCurrent_Qtys(string mach)
        {
            //Get_MachCount
            DataSet Tags = new DataSet("Tags");
            DataTable dt = new DataTable();
            DataRow dr;
            dt.TableName = "Tags";
            dt.Columns.Add("Name");
            dr = dt.NewRow();
            dr["Name"] = mach;
            dt.Rows.Add(dr);
            Tags.Tables.Add(dt);
            var json = JsonConvert.SerializeObject(Tags);
            //
            Task<DataTable> Rs = Task.Run(() => CNCReport.PostDs2Api(Tags, "Get_MachCount"));
            Rs.Wait();
            DataTable dt_resoult = Rs.Result;

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt_resoult))
                return DataTableUtils.toString(dt_resoult.Rows[0]["Value"]);
            else
                return "0";
        }
        //----------------------------------------------------------------分配預覽部分--------------------------------------------------------
        //OK
        /// <summary>
        ///回傳良品與不良品分配的結果
        /// </summary>
        /// <param name="machine">機台</param>
        /// <param name="manu_id">製令</param>
        /// <param name="type_mode">類型(進站報工/進佔維護)</param>
        /// <param name="good_qty">良品數量</param>
        /// <param name="bad_qty">不良數量</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode Preview_Results_OLD(string machine, string manu_id, string type_mode, string good_qty, string bad_qty)
        {
            //取得資料(依據預交日期作排序，且狀態=入站的)
            string sqlcmd = $"SELECT workorder_information.*, isnull(a.good_qty, 0) report_good, isnull(a.bad_qty, 0) report_bad, (SELECT mach_show_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) mach_show_name  , (SELECT is_collect   FROM machine_info   WHERE machine_info.mach_name = workorder_information.mach_name) is_collect FROM workorder_information  LEFT JOIN   (SELECT TOP(1) * FROM workorder_information WHERE order_status = '出站' ORDER BY now_time DESC ) a ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number AND a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE  workorder_information.mach_name = '{machine}' and workorder_information.order_status ='入站' AND workorder_information.type_mode = '{type_mode}'   ORDER BY workorder_information.delivery ASC";

            DataTable dt = cls_db.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");

            if (HtmlUtil.Check_DataTable(dt))
            {
                //良品初始值
                double OK_qty = 0;
                //不良初始值
                double NG_qty = 0;
                foreach (DataRow row in dt.Rows)
                {
                    //數據回歸原始
                    row["product_count_day"] = DataTableUtils.toDouble(row["maintain_qty"]) + DataTableUtils.toDouble(row["report_good"]) + DataTableUtils.toDouble(row["report_bad"]);
                    row["no_product_count_day"] = DataTableUtils.toDouble(row["exp_product_count_day"]) - DataTableUtils.toDouble(row["maintain_qty"]) - DataTableUtils.toDouble(row["report_bad"]) - DataTableUtils.toDouble(row["report_good"]);
                    row["good_qty"] = DataTableUtils.toDouble(row["report_good"]);
                    row["bad_qty"] = DataTableUtils.toDouble(row["report_bad"]);
                }
                //加上處理後的良品數量
                if (DataTableUtils.toString(dt.Rows[0]["is_collect"]) == "否")
                    OK_qty += DataTableUtils.toDouble(good_qty) * DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["multiplication"])) / DataTableUtils.toDouble(DataTableUtils.toString(dt.Rows[0]["division"]));
                else
                    OK_qty += DataTableUtils.toDouble(good_qty);

                //加上處理後的不良數量
                NG_qty += DataTableUtils.toDouble(bad_qty);

                //填入良品數量
                OK_Distribute(dt, OK_qty);

                //填入不良數量
                NG_Distribute(dt, NG_qty);
                //回傳
                if (HtmlUtil.Check_DataTable(dt))
                {
                    xmlElem.SetAttribute("Value", manu_id);
                    xmlDoc.AppendChild(xmlElem);
                    foreach (DataRow row in dt.Rows)
                    {
                        XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                        xmlElemA.SetAttribute("工單號", DataTableUtils.toString(row["manu_id"]));
                        xmlElemA.SetAttribute("機台名稱", DataTableUtils.toString(row["mach_show_name"]));
                        xmlElemA.SetAttribute("品號", DataTableUtils.toString(row["product_number"]));
                        xmlElemA.SetAttribute("品名", DataTableUtils.toString(row["product_name"]));
                        xmlElemA.SetAttribute("預計產量", DataTableUtils.toString(row["exp_product_count_day"]));
                        xmlElemA.SetAttribute("已生產量", (DataTableUtils.toInt(DataTableUtils.toString(row["product_count_day"]))).ToString());
                        xmlElemA.SetAttribute("未生產量", DataTableUtils.toString(row["no_product_count_day"]));
                        xmlElemA.SetAttribute("製程代號", DataTableUtils.toString(row["craft_Number"]));
                        xmlElemA.SetAttribute("製程名稱", DataTableUtils.toString(row["craft_name"]));
                        xmlElemA.SetAttribute("維護數量", DataTableUtils.toInt(DataTableUtils.toString(row["maintain_qty"])).ToString());
                        xmlElemA.SetAttribute("良品數量", DataTableUtils.toString(row["good_qty"]));
                        xmlElemA.SetAttribute("不良數量", DataTableUtils.toString(row["bad_qty"]));
                        xmlDoc.DocumentElement.AppendChild(xmlElemA);
                    }
                    return xmlDoc.DocumentElement;
                }
            }
            else
                return null;
            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }
        /// <summary>
        /// 回傳良品與不良品分配的結果(要同品項同工藝才一起分配 不同單可以)-new 0607 juiedit
        /// 多了一個外部製成交貨報工用 因為交回來是個收貨站 不一定都是同樣東西 同品項設定一個站 太多站了所以要打破同品項才能入站
        /// </summary>
        /// <param name="machine">機台</param>
        /// <param name="manu_id">製令</param>
        /// <param name="type_mode">類型(進站報工/進佔維護)</param>
        /// <param name="good_qty">良品數量</param>
        /// <param name="bad_qty">不良數量</param>
        /// 0704 重新定義
        /// 修訂-預計產量-ERP要求的數量---永久不變不管回報多少顆除非訂單變更
        /// 修訂-已生產量-ERP來的數據--ERP的欄位給的可能會有時間差但是不管...用ERP  doneCount的值
        /// 修訂-未生產量-預計產量-已生產量
        /// 修訂-良品數量-當下良品的數量(未出站的總數量)
        /// 修訂-不良品數量-當下不良品的數量(未出站的總數量)
        /// 新增-當下數量-當下良品+當下不良品
        /// <returns></returns>
        [WebMethod]
        public XmlNode Preview_Results(string machine, string manu_id, string type_mode, string good_qty, string bad_qty)//good_qty 已經是相加的了
        {
            //取得資料(依據預交日期作排序，且狀態=入站的)
            //string sqlcmd = $"SELECT workorder_information.*, isnull(a.good_qty, 0) report_good, isnull(a.bad_qty, 0) report_bad, (SELECT mach_show_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) mach_show_name  , (SELECT is_collect   FROM machine_info   WHERE machine_info.mach_name = workorder_information.mach_name) is_collect FROM workorder_information  LEFT JOIN   (SELECT TOP(1) * FROM workorder_information WHERE order_status = '出站' ORDER BY now_time DESC ) a ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number AND a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE  workorder_information.mach_name = '{machine}' and workorder_information.order_status ='入站' AND workorder_information.type_mode = '{type_mode}'   ORDER BY workorder_information.delivery ASC";
            string sqlcmd = $"SELECT workorder_information.*, isnull(a.good_qty, 0) report_good, isnull(a.bad_qty, 0) report_bad,	   Isnull((((SELECT Sum(Cast(a.report_qty AS INT))FROM   (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM   record_worktime WHERE  record_worktime.manu_id =workorder_information.manu_id AND record_worktime.mach_name =workorder_information.mach_name AND record_worktime.type_mode =workorder_information.type_mode) a WHERE  a.workman_status = '中途報工' and a.qty_status='良品'))), 0)  當下良品,	   Isnull((((SELECT Sum(Cast(a.report_qty AS INT))FROM   (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM   record_worktime WHERE  record_worktime.manu_id =workorder_information.manu_id AND record_worktime.mach_name =workorder_information.mach_name AND record_worktime.type_mode =workorder_information.type_mode) a WHERE  a.workman_status = '中途報工' and a.qty_status='不良品'))), 0)  當下不良品, (SELECT mach_show_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) mach_show_name  , (SELECT is_collect   FROM machine_info   WHERE machine_info.mach_name = workorder_information.mach_name) is_collect FROM workorder_information  LEFT JOIN   (SELECT TOP(1) * FROM workorder_information WHERE order_status = '出站' ORDER BY now_time DESC ) a ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number AND a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE  workorder_information.mach_name = '{machine}' and workorder_information.order_status ='入站' AND workorder_information.type_mode = '{type_mode}'   ORDER BY workorder_information.delivery ASC";
            string Target_ordermo = "";
            string Target_Product_Num = "";
            string Run_ordermo = "";
            string Run_Product_Num = "";
            string good_qtyInput = good_qty;
            string bad_qtyInput = bad_qty;
            string Good_Now = "0";
            string bad_Now = "0";
            string FinshEdQty = "0";
            string UnFinshEdQty = "0";
            DataTable dt_finished;
            DataTable dt = cls_db.GetDataTable(sqlcmd);
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");

            //
            if (HtmlUtil.Check_DataTable(dt))
            {
                //委外先挑選起來
                dt = ProcessTypeOrder(dt, manu_id);
                //取得標的(解決不同品號或是不同工藝問題)
                Target_ordermo = dt.AsEnumerable().Where(w => w.Field<string>("manu_id") == manu_id).Select(s => s.Field<string>("orderno")).FirstOrDefault();
                Target_Product_Num = dt.AsEnumerable().Where(w => w.Field<string>("manu_id") == manu_id).Select(s => s.Field<string>("product_number")).FirstOrDefault();
                //0607
                DataTable dt_new = dt.Clone();
                //良品初始值
                double OK_qty = 0;
                //不良初始值
                double NG_qty = 0;
                //當下良品初始直
                double OK_qty_unReport = 0;
                //當下良品初始直
                double NG_qty_unReport = 0;
                //變更同品項同工藝才可以進行分配  0607 juiedit
                var dt_groupby = dt.AsEnumerable().GroupBy(w => new { g = w.Field<string>("product_number"), h = w.Field<string>("orderno") }).Select(s => s);
                //var dt_groupby = dt.AsEnumerable().Where(w => w.Field<string>("product_number") == Target_Product_Num && w.Field<string>("orderno") == Target_ordermo).GroupBy(w => new { g = w.Field<string>("product_number") }).Select(s => s);
                foreach (var key in dt_groupby)
                {
                    //不同品項要歸零
                    OK_qty = 0;
                    NG_qty = 0;
                    //分群組-
                    var dt_gpSamekey = key.CopyToDataTable();
                    //需求的項目才需要累加(品項依樣的,工藝一樣的)
                    Run_ordermo = dt_gpSamekey.AsEnumerable().Select(s => s.Field<string>("orderno")).FirstOrDefault();
                    Run_Product_Num = dt_gpSamekey.AsEnumerable().Select(s => s.Field<string>("product_number")).FirstOrDefault();
                    if (Target_ordermo == Run_ordermo && Target_Product_Num == Run_Product_Num)
                    {
                        good_qtyInput = good_qty;
                        bad_qtyInput = bad_qty;
                    }
                    else
                    {
                        good_qtyInput = "0";
                        bad_qtyInput = "0";
                    }

                    foreach (DataRow row in dt_gpSamekey.Rows)
                    {
                        //數據回歸原始 0701/ 跟改定義
                        //product_count_day :今日生產 在預覽就是報工的數量不再管過去----該次當下生產 =當下良品+當下不良品(當下:未出站的累計)
                        row["product_count_day"] = DataTableUtils.toDouble(row["maintain_qty"]) + DataTableUtils.toDouble(row["report_good"]) + DataTableUtils.toDouble(row["report_bad"]);
                        row["no_product_count_day"] = DataTableUtils.toDouble(row["exp_product_count_day"]) - DataTableUtils.toDouble(row["maintain_qty"]) - DataTableUtils.toDouble(row["report_bad"]) - DataTableUtils.toDouble(row["report_good"]);
                        row["good_qty"] = DataTableUtils.toDouble(row["report_good"]);
                        row["bad_qty"] = DataTableUtils.toDouble(row["report_bad"]);
                        //row["product_count_day"] = DataTableUtils.toDouble(row["maintain_qty"]) + DataTableUtils.toDouble(row["report_good"]) + DataTableUtils.toDouble(row["report_bad"]);
                        //row["no_product_count_day"] = DataTableUtils.toDouble(row["exp_product_count_day"]) - DataTableUtils.toDouble(row["maintain_qty"]) - DataTableUtils.toDouble(row["report_bad"]) - DataTableUtils.toDouble(row["report_good"]);
                        //row["good_qty"] = DataTableUtils.toDouble(row["report_good"]);
                        //row["bad_qty"] = DataTableUtils.toDouble(row["report_bad"]);
                    }
                    //加上處理後的良品數量
                    if (DataTableUtils.toString(dt_gpSamekey.Rows[0]["is_collect"]) == "否")
                        OK_qty += DataTableUtils.toDouble(good_qtyInput) * DataTableUtils.toDouble(DataTableUtils.toString(dt_gpSamekey.Rows[0]["multiplication"])) / DataTableUtils.toDouble(DataTableUtils.toString(dt_gpSamekey.Rows[0]["division"]));
                    else
                        OK_qty += DataTableUtils.toDouble(good_qtyInput);

                    //加上處理後的不良數量
                    NG_qty += DataTableUtils.toDouble(bad_qtyInput);

                    //填入良品數量

                    OK_Distribute(dt_gpSamekey, OK_qty);
                    //OK_qty_unReport = OK_qty;//當下良品紀錄
                    //填入不良數量
                    NG_Distribute(dt_gpSamekey, NG_qty);
                    //組到新的table表
                    foreach (DataRow dr in dt_gpSamekey.Rows)
                    {
                        //同品項  同工藝再加入就好
                        if (dr["product_number"].ToString() == Target_Product_Num && dr["orderno"].ToString() == Target_ordermo)
                            dt_new.ImportRow(dr);
                    }
                }
                //回傳
                if (HtmlUtil.Check_DataTable(dt_new))
                {
                    xmlElem.SetAttribute("Value", manu_id);
                    xmlDoc.AppendChild(xmlElem);

                    foreach (DataRow row in dt_new.Rows)
                    {
                        FinshEdQty = "0";
                        //取得最新的出站累積數量
                        sqlcmd = $"select * from workorder_information　where manu_id='{row["manu_id"]}' and mach_name='{machine}' and product_number='{DataTableUtils.toString(row["product_number"])}' and orderno='{DataTableUtils.toString(row["orderno"])}' and order_status='出站' order by　now_time desc ";
                        dt_finished = cls_db.GetDataTable(sqlcmd);
                        if (HtmlUtil.Check_DataTable(dt_finished))
                        {
                            var Count = dt_finished.AsEnumerable().Select(s => s.Field<string>("product_count_day")).FirstOrDefault();
                            FinshEdQty = Count;
                        }
                        //
                        XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                        xmlElemA.SetAttribute("工單號", DataTableUtils.toString(row["manu_id"]));
                        xmlElemA.SetAttribute("機台名稱", DataTableUtils.toString(row["mach_show_name"]));
                        xmlElemA.SetAttribute("品號", DataTableUtils.toString(row["product_number"]));
                        xmlElemA.SetAttribute("品名", DataTableUtils.toString(row["product_name"]));
                        xmlElemA.SetAttribute("預計產量", DataTableUtils.toString(row["exp_product_count_day"]));
                        xmlElemA.SetAttribute("已生產量", FinshEdQty);
                        xmlElemA.SetAttribute("未生產量", (DataTableUtils.toInt(DataTableUtils.toString(row["exp_product_count_day"])) - DataTableUtils.toInt(DataTableUtils.toString(FinshEdQty))).ToString());
                        //xmlElemA.SetAttribute("已生產量", OK_qty != 0 ? (DataTableUtils.toInt(DataTableUtils.toString(row["report_good"]))).ToString() : "0");
                        //xmlElemA.SetAttribute("未生產量", (DataTableUtils.toInt(DataTableUtils.toString(row["exp_product_count_day"])) - DataTableUtils.toInt(DataTableUtils.toString(row["report_good"]))).ToString());
                        //xmlElemA.SetAttribute("未生產量", DataTableUtils.toString(row["no_product_count_day"]));
                        //xmlElemA.SetAttribute("當下產量", (OK_qty + NG_qty).ToString());
                        xmlElemA.SetAttribute("當下產量", (DataTableUtils.toInt(DataTableUtils.toString(row["product_count_day"]))).ToString());
                        xmlElemA.SetAttribute("製程代號", DataTableUtils.toString(row["craft_Number"]));
                        xmlElemA.SetAttribute("製程名稱", DataTableUtils.toString(row["craft_name"]));
                        xmlElemA.SetAttribute("維護數量", DataTableUtils.toInt(DataTableUtils.toString(row["maintain_qty"])).ToString());
                        xmlElemA.SetAttribute("良品數量",  DataTableUtils.toString(row["good_qty"]));
                        xmlElemA.SetAttribute("不良數量", DataTableUtils.toString(row["當下不良品"]));
                        //xmlElemA.SetAttribute("不良數量", bad_qty);
                        xmlDoc.DocumentElement.AppendChild(xmlElemA);
                    }
                    return xmlDoc.DocumentElement;
                }
            }
            else
                return null;
            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }
        //處理委外的  如果是委外狀況 直接就只有報該工單其他不能一起報
        private DataTable ProcessTypeOrder(DataTable dt, string Taget_ordernum)
        {
            DataTable dt_new = dt.Clone();
            DataRow dr_new = dt_new.NewRow();
            var dt_out = dt.AsEnumerable().Where(w => w.Field<string>("manu_id") == Taget_ordernum);
            if (dt_out.FirstOrDefault() != null)
            {
                dr_new = dt_out.FirstOrDefault();
                if (dr_new["processType"].ToString() == "2")
                {
                    dt_new.ImportRow(dr_new);
                    return dt_new;
                }
                else
                    return dt;
            }
            else
                return dt;

        }
        //OK
        /// <summary>
        /// 良品分配
        /// </summary>
        /// <param name="dt">所有入站之工單</param>
        /// <param name="Report_Qty">良品數量</param>
        /// <returns></returns>
        private DataTable OK_Distribute(DataTable dt, double Report_Qty)
        {
            int count = 0;
            int rowtotal = dt.Rows.Count;
            if (Report_Qty > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //目標數量  
                    double target_qty = DataTableUtils.toInt(DataTableUtils.toString(row["exp_product_count_day"]));

                    //取得未生產數量
                    double noproducted_qty = DataTableUtils.toDouble(DataTableUtils.toString(row["no_product_count_day"]));

                    //回報數量減去需求數量
                    double last_qty = Report_Qty - noproducted_qty;

                    //如果數量可以扣除的話，則讓它扣吧
                    Report_Qty = last_qty >= 0 ? last_qty : Report_Qty;

                    //如果為最後一筆，則全部塞滿它
                    if (count == rowtotal - 1 && Report_Qty >= 0 && last_qty >= 0)
                        target_qty = Report_Qty + noproducted_qty;

                    row["product_count_day"] = last_qty >= 0 ? target_qty : Report_Qty;
                    row["no_product_count_day"] = last_qty >= 0 ? 0 : noproducted_qty - Report_Qty;

                    row["good_qty"] = last_qty >= 0 ? target_qty : Report_Qty;

                    if (last_qty <= 0)
                        Report_Qty = 0;
                    count++;
                }
            }

            return dt;

        }

        //OK
        /// <summary>
        /// 不良品分配
        /// </summary>
        /// <param name="dt">所有入站之工單</param>
        /// <param name="Bad_Qty">不良品數量</param>
        /// <returns></returns>
        private DataTable NG_Distribute(DataTable dt, double Bad_Qty)
        {
            int count = 0;
            int rowtotal = dt.Rows.Count;
            double bad_copy = Bad_Qty;
            double bad_total = 0;
            bool judge = false;
            if (Bad_Qty > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //取得目標數量
                    double target_qty = DataTableUtils.toDouble(DataTableUtils.toString(row["exp_product_count_day"]));

                    //已生產數量 
                    double producted_qty = DataTableUtils.toInt(DataTableUtils.toString(row["product_count_day"]));

                    //取得未生產數量
                    double noproducted_qty = DataTableUtils.toDouble(DataTableUtils.toString(row["no_product_count_day"]));

                    //剩餘不良數量
                    double last_qty = noproducted_qty >= 0 ? Bad_Qty - noproducted_qty : Bad_Qty;

                    //如果數量足夠的情況下，要扣除
                    Bad_Qty = last_qty >= 0 ? last_qty : Bad_Qty;

                    //若為最後一筆，則塞滿它
                    if (count == rowtotal - 1 && Bad_Qty >= 0 && last_qty >= 0)
                        target_qty = target_qty + Bad_Qty;

                    if (count == rowtotal - 1)
                        judge = true;

                    row["no_product_count_day"] = last_qty >= 0 ? 0 : target_qty - (producted_qty + Bad_Qty);
                    //row["no_product_count_day"] = last_qty >= 0 ? 0 : (noproducted_qty - Bad_Qty);//0705 jui
                    //不良品數量 都塞到最後一筆
                    row["bad_qty"] = last_qty >= 0 ? (judge ? bad_copy : noproducted_qty) : Bad_Qty;
                    row["當下不良品"] = judge ? Bad_Qty:0;
                    //預計產量
                    row["product_count_day"] = DataTableUtils.toDouble(row["bad_qty"].ToString()) + DataTableUtils.toDouble(row["product_count_day"].ToString());

                    //判斷存入的數量
                    bad_total = last_qty >= 0 ? judge ? Bad_Qty : noproducted_qty : Bad_Qty;

                    if (last_qty < 0)
                        Bad_Qty = 0;
                    bad_copy = Bad_Qty;
                    count++;
                }

            }
            return dt;
        }

        //----------------------------------------------------------------分配預覽部分--------------------------------------------------------

        //----------------------------------------------------------------取得APS的報工資料--------------------------------------------------------
        [WebMethod]
        public XmlNode Get_ApsData(string machine)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            string sqlcmd = $"select mach_show_name 機台名稱,project 訂單號碼,Task 製程代號,Job 工藝名稱,TargetPiece 預計數量 from dek_aps.workhour , cnc_db_test.machine_info  where dek_aps.workhour.Resource = cnc_db_test.machine_info.mach_name and cnc_db_test.machine_info.mach_name  = '{machine}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt))
            {
                xmlElem.SetAttribute("Value", machine);
                xmlDoc.AppendChild(xmlElem);
                foreach (DataRow row in dt.Rows)
                {
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                    xmlElemA.SetAttribute("機台名稱", $"{row["機台名稱"]}");
                    xmlElemA.SetAttribute("訂單號碼", $"{row["訂單號碼"]}");
                    xmlElemA.SetAttribute("製程代號", $"{row["製程代號"]}");
                    xmlElemA.SetAttribute("工藝名稱", $"{row["工藝名稱"]}");
                    xmlElemA.SetAttribute("預計數量", $"{row["預計數量"]}");
                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                }
                return xmlDoc.DocumentElement;
            }
            else
                xmlElem.SetAttribute("Value", "-1");

            xmlDoc.AppendChild(xmlElem);
            return xmlDoc.DocumentElement;
        }
        //----------------------------------------------------------------取得APS的報工資料--------------------------------------------------------

        /*--------------------------------------------------代理商需求用---------------------------------------------------------------*/

    }
}

