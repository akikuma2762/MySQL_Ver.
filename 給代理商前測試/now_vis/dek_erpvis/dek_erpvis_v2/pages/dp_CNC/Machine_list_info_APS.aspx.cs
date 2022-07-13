using System;
using Support;
using dek_erpvis_v2.cls;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using System.Net;
using System.IO;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class Aps_Project : System.Web.UI.Page
    {
        public string show = "";
        public string workmans = "";
        public string status_button = "";
        public string Gantt_Data = "";
        public string color = "";
        public string Mysql_conn_str = myclass.GetConnByDekVisCNC;
        public DateTime FirstDay = new DateTime();
        public DateTime LastDay = new DateTime();
        public string str_First_Day = "";
        public string str_Last_Day = "";
        public string dev_name = "";
        public int run = 0;//運轉
        public int rest = 0;//待機
        public int alert = 0;//警告
        public int noline = 0;//離線
        public double percent = 0;//稼動率
        public double progress = 0;//生產進度
        public string th = "";
        public string tr = "";
        public string area = "";
        public string str_Dev_Name = "";
        public string str_Dev_Status = "";
        public bool b_Page_Load = true;
        myclass myclass = new myclass();
        clsDB_Server clsDB_vis = new clsDB_Server("");
        public int stop = 0;
        public int shutdown = 0;
        public int Finish = 0;
        public int Ready = 0;
        public string get_js = "";
        public string information = "";
        List<string> field_name = new List<string>();
        List<string> Parameter = new List<string>();
        List<string> list = new List<string>();
        string acc = "";
        enum classcolor { ganttRed, ganttGreen, ganttOrange, ganttBlue, ganttPurple };
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                string URL_NAME = "Machine_list_info_APS";
                if (myclass.user_view_check(URL_NAME, acc) || HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    set_page_content();
                else Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
            }
        }
        //------------------------------------------------方法事件----------------------------------------------------------
        private void set_page_content()
        {
            Set_information();
            create_status();
            Read_Data();
            Set_Image();
            Set_workman();
            check_config();

        }
        private void Read_Data()
        {
            th = "";
            tr = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            //顯示所有資訊的DATATABLE
            string sql_cmd = $"SELECT ID as Next_Button, Mach_Name,Product_Name,Mach_Status,Now_QTY,Target_QTY,Predict_Start,Predict_End,'' as worktime,'' as workman," +
                             $"Fix,Product_Number,Now_Task,Next_Task,Number,Percent,Order_Status,Order_Number,Program_Name," +
                             $"IOrder_Number,Mach_Type,Now_Status,mach_status,order_number,now_task,id FROM {ShareMemory.SQLMach_content}";
            DataTable dt = DataTableUtils.GetDataTable(sql_cmd);

            if (HtmlUtil.Check_DataTable(dt))
                Set_Table(dt);
            else
                HtmlUtil.NoData(out th, out tr);
        }
        //表格模式
        private void Set_Table(DataTable dt)
        {
            bool ok = true;
            th = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            DataTable dt_machStatus = DataTableUtils.DataTable_GetTable(ShareMemory.SQLWorkHour_MachStatus, "");

            DateTime dt_trs = new DateTime();
            dt = UpdataNullCNC(dt);
            foreach (DataRow row in dt.Rows)
            {
                tr += $"<tr style='color:white; background-color:{backgroundcolor_change(DataTableUtils.toString(row["Mach_status"]))};'>";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (list.IndexOf(dt.Columns[i].ToString()) != -1)
                    {
                        if (dt.Columns[i].ToString() == "Predict_Start" || dt.Columns[i].ToString() == "Predict_End")
                        {
                            if (!string.IsNullOrEmpty(row[dt.Columns[i].ToString()].ToString()))
                            {
                                dt_trs = ShareFunction.StrToDate(DataTableUtils.toString(row[dt.Columns[i].ToString()]));
                                tr += $"<td style=\"text-align:center;vertical-align:middle;\">{dt_trs.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture)}</br>{dt_trs.ToString(" HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)}</td>\n";
                            }
                            else
                                tr += "<td></td>\n";
                        }
                        else if (dt.Columns[i].ToString() == "Mach_Status")
                        {
                            var MachStatus = dt_machStatus.AsEnumerable().Where(w => w.Field<string>("StatusEn") == DataTableUtils.toString(row[dt.Columns[i].ToString()])).Select(s => s.Field<string>("Status")).FirstOrDefault();
                            if (MachStatus == null)
                                tr += $"<td style=\"text-align:center;vertical-align:middle;\">{dt_machStatus.Rows[0]["Status"]}</td>\n";
                            else
                                tr += $"<td style=\"text-align:center;vertical-align:middle;\">{MachStatus}</td>\n";
                        }
                        else if (dt.Columns[i].ToString() == "Percent")
                        {
                            if (DataTableUtils.toInt(row["Target_QTY"].ToString()) > 0)
                                tr += $"<td style=\"text-align:center;vertical-align:middle;\">{DataTableUtils.toInt(row["Now_QTY"].ToString()) * 100 / DataTableUtils.toInt(row["Target_QTY"].ToString())}%</td>\n";
                            else
                                tr += $"<td style=\"text-align:center;vertical-align:middle;\">0%</td>\n";
                        }
                        else if (dt.Columns[i].ToString() == "Next_Button")
                        {
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                            string sqlcmd = $"select * from workhour where Project = '{row["order_number"]}' and TaskName = '{row["now_task"]}'";
                            DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

                            string Lost_status = "";
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                            sqlcmd = $"select * from workhour_detail where Project = '{row["order_number"]}' and TaskName = '{row["now_task"]}' and RealRsrc = '{row["Mach_Name"]}' order by Record_Time desc , ID desc";
                            DataTable dw = DataTableUtils.GetDataTable(sqlcmd);
                            if (HtmlUtil.Check_DataTable(dw))
                                Lost_status = dw.Rows[0]["now_status"].ToString();

                            string now_detail = $"{row["Mach_Name"]}^{row["Product_Name"]}^{DataTableUtils.toInt(DataTableUtils.toString(row["Now_QTY"]))}^{DataTableUtils.toInt(DataTableUtils.toString(row["Target_QTY"]))}^{HtmlUtil.StrToDate(DataTableUtils.toString(row["Predict_Start"])):yyyy/MM/dd HH:mm:ss}^{HtmlUtil.StrToDate(DataTableUtils.toString(row["Predict_End"])):yyyy/MM/dd HH:mm:ss}^{row["order_number"]}^";
                            string next_detail = "";
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                            sqlcmd = $"select * from workhour where ID = '{row["Next_Task"]}'";
                            DataTable dr = DataTableUtils.GetDataTable(sqlcmd);

                            if (HtmlUtil.Check_DataTable(dr))
                                next_detail = $"{row["Mach_Name"]}^{dr.Rows[0]["Job"]}^{DataTableUtils.toInt(DataTableUtils.toString(dr.Rows[0]["CurrentPiece"]))}^{DataTableUtils.toInt(DataTableUtils.toString(dr.Rows[0]["TargetPiece"]))}^{HtmlUtil.StrToDate(DataTableUtils.toString(dr.Rows[0]["StartTime"])):yyyy/MM/dd HH:mm:ss}^{HtmlUtil.StrToDate(DataTableUtils.toString(dr.Rows[0]["EndTime"])):yyyy/MM/dd HH:mm:ss}^{dr.Rows[0]["Project"]}^";

                            if (HtmlUtil.Check_DataTable(ds))
                                tr += $"<td style=\"text-align:center;vertical-align:middle;background-color:#ffffff\"><a href=\"javascript:void(0)\"><img src=\"../../assets/images/canclick.png\"  width=\"50px\" height=\"50px\" data-toggle = \"modal\" data-target = \"#exampleModal_Report\" onclick=\"set_value('{ds.Rows[0]["Status"].ToString().Trim()}','{row["order_number"].ToString().Trim()}','{row["now_task"].ToString().Trim()}','{row["mach_name"].ToString().Trim()}','{ds.Rows[0]["Task"].ToString().Trim()}','{ds.Rows[0]["ID"].ToString().Trim()}','{Lost_status}','{next_detail.Replace(' ', '*')}','{now_detail.Replace(' ', '*')}')\"  /></a></td>\n";

                            else
                                tr += $"<td style=\"text-align:center;vertical-align:middle;background-color:#ffffff\">  <img src=\"../../assets/images/unclick.png\" width=\"50px\" height=\"50px\" /> </td>\n";
                        }
                        else if (dt.Columns[i].ToString() == "worktime")
                        {
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                            string sqlcmd = $"select * from workhour where Project = '{row["order_number"]}' and TaskName = '{row["now_task"]}'";
                            DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

                            if (HtmlUtil.Check_DataTable(ds))
                            {
                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                                sqlcmd = $"select * from workhour_detail where  Project = '{row["order_number"]}' and Task = '{ds.Rows[0]["Task"]}' and RealRsrc='{row["Mach_Name"]}' order by Record_Time asc,ID asc";
                                ds = DataTableUtils.GetDataTable(sqlcmd);
                                if (HtmlUtil.Check_DataTable(ds))
                                {
                                    dt_trs = ShareFunction.StrToDate(DataTableUtils.toString(ds.Rows[0]["Record_Time"]));
                                    tr += $"<td style=\"text-align:center;vertical-align:middle;\">{dt_trs.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture)}</br>{dt_trs.ToString(" HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)}</td>\n";
                                }
                                else
                                    tr += "<td></td>\n";
                            }
                            else
                                tr += "<td></td>\n";
                        }
                        else if (dt.Columns[i].ToString() == "workman")
                        {
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                            string sqlcmd = $"select * from workhour where Project = '{row["order_number"]}' and TaskName = '{row["now_task"]}'";
                            DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

                            if (HtmlUtil.Check_DataTable(ds))
                            {
                                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                                sqlcmd = $"select * from workhour_detail where Project = '{row["order_number"]}' and Task = '{ds.Rows[0]["Task"]}'  and RealRsrc='{row["Mach_Name"]}' order by Record_Time desc,ID desc";
                                ds = DataTableUtils.GetDataTable(sqlcmd);
                                if (HtmlUtil.Check_DataTable(ds))
                                    tr += $"<td style=\"text-align:center;vertical-align:middle;\">{ds.Rows[0]["StaffName"]}</td>\n";
                                else
                                    tr += "<td></td>\n";
                            }
                            else
                                tr += "<td></td>\n";
                        }
                        else
                            tr += $"<td style=\"text-align:center;vertical-align:middle;\">{DataTableUtils.toString(row[dt.Columns[i].ToString()])}</td>\n";

                        if (ok)
                            th += $"<th style=\"text-align:center;vertical-align:middle;\">{list[list.IndexOf(dt.Columns[i].ToString()) + 1]}</th>";
                    }
                }
                tr += "</tr>";
                ok = false;
            }

            //SUM Caculator
            run = dt.AsEnumerable().Select(s => s.Field<string>("Mach_status") == "RUN").Count();
            stop = dt.AsEnumerable().Select(s => s.Field<string>("Mach_status") == "STOP").Count();
            shutdown = dt.AsEnumerable().Select(s => s.Field<string>("Mach_status") == "SHUTDOWN").Count();
            Finish = dt.AsEnumerable().Select(s => s.Field<string>("Mach_status") == "FINISH").Count();
            Ready = dt.AsEnumerable().Select(s => s.Field<string>("Mach_status") == "READY").Count();
        }
        private DataTable UpdataNullCNC(DataTable dt_ori)
        {
            string Condition = "";
            int WorkHourindex = 0;
            int CncIndex = 0;
            bool OK = false;
            var dt_notNull = dt_ori.AsEnumerable().Where(s => string.IsNullOrEmpty(s.Field<string>("Predict_Start"))).Select(s => s);
            try

            {
                if (dt_notNull != null)
                {
                    Condition = ShareFunction_APS.CombinConditionStr(dt_notNull.CopyToDataTable(), "Mach_Name");
                    Condition = Condition.Replace("Mach_Name", "Resource");
                    Condition += " AND Status=" + "'" + MachineStatus.READY.ToString() + "'";
                    DataTable dt_workHourForNullMach = DataTableUtils.DataTable_GetTable(ShareMemory.SQLWorkHour, Condition);
                    if (dt_workHourForNullMach != null && dt_workHourForNullMach.Rows.Count != 0)
                    {
                        var NullmachList = dt_workHourForNullMach.AsEnumerable().GroupBy(g => g.Field<string>("Resource")).Select(s => s.Key).ToList();
                        foreach (string MachName in NullmachList)
                        {
                            //find Target WorkHour Row
                            var dt_WHTargetmach = dt_workHourForNullMach.AsEnumerable().Where(s => s.Field<string>("Resource") == MachName).OrderBy(s => s.Field<string>("StartTime"));

                            //WorkHourindex = dt_workHourForNullMach.Rows.IndexOf(dr_WHTargetmach);
                            if (WorkHourindex != -1)
                            {
                                DataRow dr_WHTargetmach = dt_WHTargetmach.FirstOrDefault();
                                var dr_CNCTargetMach = dt_notNull.AsEnumerable().Where(w => w.Field<string>("Mach_Name") == MachName).Select(s => s).FirstOrDefault();
                                CncIndex = dt_ori.Rows.IndexOf(dr_CNCTargetMach);

                                dt_ori.Rows[CncIndex]["Mach_Status"] = dr_WHTargetmach["Status"].ToString();
                                dt_ori.Rows[CncIndex]["Now_Task"] = dr_WHTargetmach["TaskName"].ToString();
                                dt_ori.Rows[CncIndex]["Predict_Start"] = dr_WHTargetmach["StartTime"].ToString();
                                dt_ori.Rows[CncIndex]["Predict_End"] = dr_WHTargetmach["EndTime"].ToString();
                                dt_ori.Rows[CncIndex]["Product_Number"] = dr_WHTargetmach["JobID"].ToString();
                                dt_ori.Rows[CncIndex]["Now_QTY"] = 0;
                                dt_ori.Rows[CncIndex]["Target_QTY"] = dr_WHTargetmach["TargetPiece"].ToString();
                                dt_ori.Rows[CncIndex]["Order_Number"] = dr_WHTargetmach["Project"].ToString();
                                dt_ori.Rows[CncIndex]["Product_Name"] = dr_WHTargetmach["Job"].ToString();
                                dt_ori.Rows[CncIndex]["Fix"] = dr_WHTargetmach["Fix"].ToString();
                                var Next2Process = dt_WHTargetmach.Where(w => w.Field<string>("id").ToString() != dr_WHTargetmach["id"].ToString()).FirstOrDefault();
                                if (Next2Process != null)
                                    dt_ori.Rows[CncIndex]["Next_Task"] = Next2Process["id"].ToString();
                                else
                                    dt_ori.Rows[CncIndex]["Next_Task"] = "";
                                Condition = "id=" + "'" + dt_ori.Rows[CncIndex]["id"].ToString() + "'";
                                OK = DataTableUtils.Update_DataRow(ShareMemory.SQLMach_content, Condition, dt_ori.Rows[CncIndex]);


                            }
                        }
                    }
                }
                // DataTable dt_wh = DataTableUtils.DataTable_GetTable()
                return dt_ori;
            }
            catch
            {
                return dt_ori;
            }
        }
        //圖塊模式
        private void Set_Image()
        {
            //顯示可列之欄位
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            string sqlcmd = $"SELECT * FROM {ShareMemory.SQLMach_contentSelect}";// where show_status='Y'";
            DataTable dt_slc = DataTableUtils.GetDataTable(sqlcmd);

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            //顯示所有資訊的DATATABLE
            string sql_cmd = $"SELECT * FROM {ShareMemory.SQLMach_content}";
            DataTable dt = DataTableUtils.GetDataTable(sql_cmd);
            DataRow dr_next;
            DataRow dr_MachNext;
            DateTime now = DateTime.Now;
            DateTime start_time;
            DateTime end_time;
            DateTime Nextstart_time;
            DateTime Nextend_time;
            string TimeColor = "block";
            string NextTimeColor = "block";
            int p_open = 0;
            int p_end = 0;
            string light = "";
            string[] ArTooltipColumnsName = new string[] { "Job", "TaskName", "StartTime", "Task" };
            string TooltipColumnsName = "";

            area += "<div >";
            area += "<div class='col-md-9 col-sm-9 col-xs-12'>";
            foreach (DataRow row in dt.Rows)
            {

                if (!string.IsNullOrEmpty(row["Predict_Start"].ToString()) || !string.IsNullOrEmpty(row["Predict_End"].ToString()))
                {
                    start_time = ShareFunction.StrToDate(DataTableUtils.toString(row["Predict_Start"]));
                    end_time = ShareFunction.StrToDate(DataTableUtils.toString(row["Predict_End"]));
                }
                else
                {
                    start_time = DateTime.Now;
                    end_time = DateTime.Now;
                }
                if (end_time < DateTime.Now)
                    TimeColor = "red";
                TimeSpan open = start_time - now;
                TimeSpan end = end_time - now;

                p_open = DataTableUtils.toInt(open.TotalMinutes.ToString("00"));
                p_end = DataTableUtils.toInt(end.TotalMinutes.ToString("00"));


                area += $"<div id='chart_{DataTableUtils.toString(row["id"])}' class='col-md-3 col-sm-3 col-xs-12'  >\n";
                area += "   <div class='dashboard_graph x_panel' >\n";
                area += "       <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;'>\n";
                //-----------------------------------------------機台名稱-------------------------------------------------

                //TOOLTIP 資訊填入處
                string tooltip = ShareFunction_APS.GetDayWorkListTotoolTip(row, ArTooltipColumnsName, ref TooltipColumnsName);

                area += "           <div class='col-md-8 col-sm-12 col-xs-6'>\n";
                area += $"               <div style='font-size:18px;'><b  data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"right\" title=\"\" data-original-title=\" {tooltip}\">{DataTableUtils.toString(row["Mach_Name"])}</b></div>";
                area += "           </div>";
                //-----------------------------------------------------------燈號---------------------------------------
                area += "           <div class='col-md-4 col-sm-12 col-xs-6' style='text-align: right;' >\n";
                //圖片閃爍 開始前15分鐘閃  結束前15分鐘閃
                //if (p_open < 15 && p_open > 0)
                //    light = $"             <a href=\"javascript:void(0)\">  <img class='img-rounded' src='../../assets/images/run.gif' alt='...' style='width:35px;height:35px;align-items:left;' data-toggle = \"modal\" data-target = \"#exampleModal_status\" onclick=print_machine('{DataTableUtils.toString(row["Mach_Name"])}')></a>";
                //else if (p_end < 15 && p_end > 0)
                //    light = $"             <a href=\"javascript:void(0)\">  <img class='img-rounded' src='../../assets/images/stop.gif' alt='...' style='width:35px;height:35px;align-items:left;' data-toggle = \"modal\" data-target = \"#exampleModal_status\" onclick=print_machine('{DataTableUtils.toString(row["Mach_Name"])}')></a>";
                //else
                //{
                if (string.IsNullOrEmpty(DataTableUtils.toString(row["Mach_status"])))
                    light = $"            <a href=\"javascript:void(0)\">   <img class='img-rounded' src='../../assets/images/STOP.PNG' alt='...' style='width:35px;height:35px;align-items:left;' data-toggle = \"modal\" data-target = \"#exampleModal_status\" onclick=print_machine('{DataTableUtils.toString(row["Mach_Name"])}')></a>";
                else
                    light = $"            <a href=\"javascript:void(0)\">   <img class='img-rounded' src='../../assets/images/{DataTableUtils.toString(row["Mach_status"])}.PNG' alt='...' style='width:35px;height:35px;align-items:left;' data-toggle = \"modal\" data-target = \"#exampleModal_status\" onclick=print_machine('{DataTableUtils.toString(row["Mach_Name"])}')></a>";
                //}
                area += light;
                area += "           </div>";

                area += "      </div>\n";

                //品名規格
                area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7;height:100px'>\n";

                area += $"                <div style='font-size:16px;' ><b><br><a href=\"javascript:void(0)\" id={DataTableUtils.toString(row["id"])} onclick=get_information('{js_information(dt_slc, row, "now")}') style='color:block;'>{DataTableUtils.toString(row["Order_Number"])} <br /> {DataTableUtils.toString(row["Product_Name"])} </a></b></div>";
                area += "           </div>";

                //工藝名稱
                area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7'>\n";
                area += $"                <div style='font-size:16px;' ><b> {DataTableUtils.toString(row["Now_Task"])}</b></div>";
                area += "           </div>";

                //結束時間
                area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7'>\n";
                area += $"                <div style='font-size:16px;color: {TimeColor};' ><b> {end_time.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture)}{end_time.ToString(" HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)}</b></div>";
                area += "           </div>";

                //分隔島
                area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7;padding-right:0px;padding-left:0px'>\n";
                area += "                <div style='font-size:16px;height:16px;background-color:#383c45' ></div>";
                area += "           </div>";

                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                sql_cmd = $"Select * from {ShareMemory.SQLWorkHour} where ID = '{DataTableUtils.toString(row["Next_Task"])}'";
                dr_next = DataTableUtils.DataTable_GetDataRow(sql_cmd);
                dr_MachNext = WorkHourTrsRowToMach_content(dr_next);
                if (dr_next != null)//&& ds.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dr_next["StartTime"].ToString()) || !string.IsNullOrEmpty(dr_next["EndTime"].ToString()))
                    {
                        Nextstart_time = ShareFunction.StrToDate(DataTableUtils.toString(dr_next["StartTime"]));
                        //DateTime end_time = DateTime.Parse(DataTableUtils.toString(row["Predict_End"]));
                        Nextend_time = ShareFunction.StrToDate(DataTableUtils.toString(dr_next["EndTime"]));
                    }
                    else
                    {
                        Nextstart_time = DateTime.Now;
                        Nextend_time = DateTime.Now;
                    }
                    if (Nextend_time < DateTime.Now)
                        NextTimeColor = "red";

                    //接續品名規格
                    area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7;height:100px'>\n";
                    area += $"                <div style='font-size:16px;' ><b><br><a href=\"javascript:void(0)\"  id={DataTableUtils.toString(row["Next_Task"])} onclick=get_information('{js_information(dt_slc, dr_MachNext, "next")}')  style='color: bolck;'>{DataTableUtils.toString(dr_next["Project"])}<br />{DataTableUtils.toString(dr_next["Job"])}</a></b></div>";
                    area += "           </div>";

                    //接續工藝名稱
                    area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7'>\n";
                    area += $"                <div style='font-size:16px;' ><b>{DataTableUtils.toString(dr_next["TaskName"])}</b></div>";
                    area += "           </div>";

                    //接續結束時間
                    area += "           <div class='col-md-12 col-sm-12 col-xs-12' style='border-style:solid;background-color:#ffffe7'>\n";
                    area += $"                <div style='font-size:16px;color: {NextTimeColor};' ><b> {Nextend_time.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture)}{Nextend_time.ToString(" HH:mm", System.Globalization.CultureInfo.CurrentCulture)}</b></div>";
                    area += "           </div>";
                }

                area += "   </div>\n";
                area += "</div>\n";

            }
            area += "</div>\n";

            area += "<div id = \"machine_information\"></div>";


            area += "</div>\n";

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
        }
        //用checkbox給使用者選取
        private void Set_information()
        {
            string sqlcmd = "";
            DataTable dt = new DataTable();
            if (CheckBoxList_cloumn.Items.Count == 0)
            {
                CheckBoxList_cloumn.Items.Clear();
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                sqlcmd = $"select * from show_column left join mach_content_select on show_column.Column_Name = mach_content_select.name where Account = '{acc}'  and show_status = 'True' and mach_content_select.name <> 'Mach_Name' ";
                dt = DataTableUtils.GetDataTable(sqlcmd);
                //已儲存過資料的，已儲存的資料為主
                if (HtmlUtil.Check_DataTable(dt))
                {
                    ListItem item = new ListItem();
                    foreach (DataRow row in dt.Rows)
                    {
                        item = new ListItem(DataTableUtils.toString(row["chinese_name"]), DataTableUtils.toString(row["Column_Name"]));
                        item.Selected = Boolean.Parse(DataTableUtils.toString(row["Allow"]).ToLower());
                        CheckBoxList_cloumn.Items.Add(item);
                    }
                }
                //為儲存資料的，已預設為主
                else
                {
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                    sqlcmd = "select * from mach_content_select  where show_status = 'True' and name <> 'mach_name'";
                    dt = DataTableUtils.GetDataTable(sqlcmd);

                    if (HtmlUtil.Check_DataTable(dt))
                    {
                        ListItem item = new ListItem();
                        foreach (DataRow row in dt.Rows)
                        {
                            item = new ListItem(DataTableUtils.toString(row["chinese_name"]), DataTableUtils.toString(row["name"]));
                            item.Selected = Boolean.Parse(DataTableUtils.toString(row["show_status"]).ToLower());
                            CheckBoxList_cloumn.Items.Add(item);
                        }
                    }
                }


            }
            for (int i = 0; i < CheckBoxList_cloumn.Items.Count; i++)
            {
                if (CheckBoxList_cloumn.Items[i].Selected == true)
                {
                    list.Add(CheckBoxList_cloumn.Items[i].Value);
                    list.Add(CheckBoxList_cloumn.Items[i].Text);
                }

            }

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            sqlcmd = "select chinese_name from mach_content_select where name = 'Mach_Name'";
            dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                list.Add("Mach_Name");
                list.Add(dt.Rows[0]["chinese_name"].ToString());
            }

        }

        //依據狀態改變顏色(之後放到CLS裡面)
        private string backgroundcolor_change(string status)
        {
            string back_color = "";
            switch (status.ToUpper())
            {
                case "RUN":
                    back_color = "#04Ba26";
                    break;
                case "FINISH":
                    back_color = "#FFAE00";
                    break;
                case "STOP":
                    back_color = "#FFAE00";
                    break;
                case "READY":
                    back_color = "#9C9C9C";
                    break;
                case "SHUTDOWN":
                    back_color = "#9C9C9C";
                    break;
                default:
                    back_color = "#000000";
                    break;
            }
            return back_color;
        }
        //取得要跳頁的參數
        private string Get_Parameter(string Task = "", string Number = "", string id = "")
        {
            string url = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            string sqlcmd = $"SELECT * FROM {ShareMemory.SQLWorkHour}";
            if (id == "")
                sqlcmd = $"{sqlcmd} where TaskName = '{Task}'  and  Project = '{Number}'";
            else
                sqlcmd = $"{sqlcmd} where ID = '{id}'";

            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                url = $"OrderNum={DataTableUtils.toString(dt.Rows[0]["Project"])}";// + ",TaskName=" + DataTableUtils.toString(dt.Rows[0]["TaskName"]);
            }
            return url;
        }
        //判斷是否為時間格式
        private string Number_to_DateTime(string Text)
        {
            string Time = "";
            Regex NumberPattern = new Regex("^[0-9]*[1-9][0-9]*$");
            if (Text.Length == 14 && NumberPattern.IsMatch(Text) == true)
            {
                Time = ShareFunction.StrToDate(Text).ToString("yyyy/MM/dd HH:mm:ss");
                return Time.ToString();
            }
            else if (Text.Contains("上午") || Text.Contains("下午"))
            {
                if (Text.Contains("上午 12"))
                    Text = Text.Replace("上午 12", "00");
                string[] date = Text.Split(' ');
                if (date.Length != 3)
                    return Text;
                else
                    return date[1] + date[2];
            }
            else if (Text == "")
                return "無資料";
            else
                return Text;

        }

        private void create_status()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            DataTable dt = DataTableUtils.GetDataTable(ShareMemory.SQLWorkHour_MachStatus, "");
            if (HtmlUtil.Check_DataTable(dt) && DropDownList_errorstatus.Items.Count == 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["StatusEn"].ToString() == "ERROR" || row["StatusEn"].ToString() == "READY")//緯凡不要全列
                    {
                        ListItem list = new ListItem(DataTableUtils.toString(row["Status"]), DataTableUtils.toString(row["StatusEn"]));
                        DropDownList_errorstatus.Items.Add(list);

                    }
                    //    status_button += "<div class=\"col-xs-6 col-sm-12 col-md-6\" style=\"text-align:center;margin-bottom:15px\">" +
                    //                            $"<button type=\"button\" class=\"btn btn-success\" onclick=\"status_change('{DataTableUtils.toString(row["StatusEn"])}')\">{DataTableUtils.toString(row["Status"]) }</button>" +
                    //                        "</div>";
                }
            }

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            dt = DataTableUtils.GetDataTable("error_type", "");
            if (HtmlUtil.Check_DataTable(dt) && DropDownList_tpye.Items.Count == 0)
            {
                foreach (DataRow row in dt.Rows)
                    DropDownList_tpye.Items.Add(DataTableUtils.toString(row["name"]));
            }

        }
        //甘特圖模式
        private void Set_Gantt()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            string sqlcmd = $"select DISTINCT Resource from {ShareMemory.SQLWorkHour}";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            foreach (DataRow row in dt.Rows)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                sqlcmd = $"select Job ,TaskName,StartTime,EndTime from {ShareMemory.SQLWorkHour} where Resource = '{DataTableUtils.toString(row["Resource"])}'";
                DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                Gantt_Data += Get_GanttData(ds, DataTableUtils.toString(row["Resource"]));
            }
        }
        //輸出甘特圖資料
        private string Get_GanttData(DataTable dt, string machine)
        {
            int i = 0;
            string gantt_value = "";
            gantt_value += "{";
            gantt_value += $"name:'{machine}',";
            gantt_value += "desc:'',";
            gantt_value += "values:[";

            foreach (DataRow row in dt.Rows)
            {
                gantt_value += "{ " +
                               $"from:'/Date({ShareFunction.GetTimeStamp(ShareFunction.StrToDate(DataTableUtils.toString(row["StartTime"])))})/', " +
                               $"  to:'/Date({ShareFunction.GetTimeStamp(ShareFunction.StrToDate(DataTableUtils.toString(row["EndTime"])))})/',      " +
                               $"  customClass: \"{Enum.GetName(typeof(classcolor), i % 5)}\"," +
                               $"  desc: \"<b>機台名稱：{machine}</b> <br> " +
                               $"          <b>工藝名稱: {DataTableUtils.toString(row["Job"]).Replace(@"""", "")}</b> <br>" +
                               $"          <b>段取名稱：{DataTableUtils.toString(row["TaskName"])}</b> <br>" +
                               $"          <b>開始時間：{ShareFunction.StrToDate(DataTableUtils.toString(row["StartTime"]))}</b> <br>" +
                               $"          <b>結束時間：{ShareFunction.StrToDate(DataTableUtils.toString(row["EndTime"]))}</b> <br>\", " +
                               "},";
                i++;

            }

            gantt_value += "]";
            gantt_value += "},";

            return gantt_value;
        }
        private void check_config()
        {
            if (WebUtils.GetAppSettings("show_gantt") == "1")
            {
                show = "<li role=\"presentation\" class=\"\" style=\"box-shadow: 3px 3px 9px gray;\"><a href=\"#tab_content3\" role=\"tab\" id=\"profile-tab2\" onclick=\"gantt_show()\" data-toggle=\"tab\" aria-expanded=\"false\">甘特圖模式</a></li>";
                Set_Gantt();
            }
        }
        //設定人員
        private void Set_workman()
        {
            if (DropDownList_Workman.Items.Count == 0)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                string sqlcmd = "select workman_name from work_man where workman_status = 'Y'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    foreach (DataRow row in dt.Rows)
                        DropDownList_Workman.Items.Add(DataTableUtils.toString(row["workman_name"]));

                }

            }
        }
        private DataRow WorkHourTrsRowToMach_content(DataRow dr)
        {
            if (dr != null)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                DataTable dt_new = DataTableUtils.DataTable_GetRowHeader(ShareMemory.SQLMach_content);
                DataRow dr_new = dt_new.NewRow();
                dr_new["ID"] = dr["ID"].ToString();
                dr_new["Mach_Name"] = dr["Resource"].ToString();
                dr_new["Mach_Status"] = dr["Status"].ToString();
                dr_new["Predict_Start"] = dr["StartTime"].ToString();
                dr_new["Now_Task"] = dr["TaskName"].ToString();
                dr_new["Predict_End"] = dr["EndTime"].ToString();
                //dr_new["Next_Task"] = dr[""].ToString();
                dr_new["Product_Number"] = dr["JobID"].ToString();
                dr_new["Now_QTY"] = dr["CurrentPiece"].ToString();
                dr_new["Target_QTY"] = dr["TargetPiece"].ToString();
                dr_new["Percent"] = (DataTableUtils.toInt(dr["CurrentPiece"].ToString()) * 100 / DataTableUtils.toInt(dr["TargetPiece"].ToString())).ToString();
                dr_new["Order_Number"] = dr["Project"].ToString();
                dr_new["Product_Name"] = dr["Job"].ToString();
                dr_new["Order_Status"] = dr["ID"].ToString();
                //dr_new["Program_Name"] = dr["ID"].ToString();
                //dr_new["IOrder_Number"] = dr["ID"].ToString();
                dr_new["Fix"] = dr["Fix"].ToString();
                //dr_new["Now_Status"] = dr["ID"].ToString();
                dr_new["Now_TaskNum"] = dr["Task"].ToString();
                return dr_new;
            }
            else return dr;

        }
        //產生給前端的資訊
        private string js_information(DataTable dt, DataRow row, string type)
        {
            string url = "";
            string js_text = "";
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            DataTable dt_machineStatus = DataTableUtils.DataTable_GetTable(ShareMemory.SQLWorkHour_MachStatus, "");

            string ColumnsStr = "";
            string ValueStr = "";
            //前面要顯示的欄位名與資訊
            if (HtmlUtil.Check_DataTable(dt) && row != null)
            {
                //印出所有欄位名稱
                foreach (DataRow field in dt.Rows)
                {
                    //限制要顯示的項目
                    if (DataTableUtils.toString(field["show_status"]) == "True")
                    {
                        try
                        {
                            string aaa = DataTableUtils.toString(row[DataTableUtils.toString(field["name"])]);
                            string ewo = DataTableUtils.toString(field["name"]);

                            ColumnsStr = DataTableUtils.toString(field["chinese_name"]);
                            if (DataTableUtils.toString(field["name"]) == "Mach_Status")
                                ValueStr = dt_machineStatus.AsEnumerable().Where(w => w.Field<string>("StatusEn") == DataTableUtils.toString(row[DataTableUtils.toString(field["name"])])).Select(s => s.Field<string>("Status")).FirstOrDefault();
                            else
                                ValueStr = Number_to_DateTime(DataTableUtils.toString(row[DataTableUtils.toString(field["name"])]));
                            //
                            js_text += $"{ColumnsStr},{ValueStr.Replace(" ", "&")},";
                        }
                        catch
                        {

                        }
                    }
                }
            }

            if (type == "now")
                url = Get_Parameter(DataTableUtils.toString(row["Now_Task"]), DataTableUtils.toString(row["Order_Number"]));
            else
                url = Get_Parameter("", "", DataTableUtils.toString(row["id"]));
            js_text += WebUtils.UrlStringEncode(url);
            return js_text;
        }
        //------------------------------------------------按鈕事件----------------------------------------------------------
        //使用者選擇要顯示的資訊
        protected void button_select_Click(object sender, EventArgs e)
        {

            //先刪除該帳號記錄的資料
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            string sqlcmd = $"select * from show_column where Account = '{acc}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            //有資料的刪除，沒資料的不動
            if (HtmlUtil.Check_DataTable(dt))
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                DataTableUtils.Delete_Record("show_column", $"Account = '{acc}'");
            }

            DataRow rew = dt.NewRow();
            rew["Column_Name"] = "Mach_Name";
            rew["Account"] = acc;

            rew["Allow"] = "True";

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            DataTableUtils.Insert_DataRow("show_column", rew);

            for (int i = 0; i < CheckBoxList_cloumn.Items.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["Column_Name"] = CheckBoxList_cloumn.Items[i].Value;
                row["Account"] = acc;
                if (CheckBoxList_cloumn.Items[i].Selected)
                    row["Allow"] = "True";
                else
                    row["Allow"] = "False";
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                DataTableUtils.Insert_DataRow("show_column", row);
            }
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存成功');location.href='Machine_list_info_APS.aspx#tab_content2';</script>");
        }
        //變更狀態
        protected void Button_status_Click(object sender, EventArgs e)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            string sqlcmd = $"select * from mach_content where Mach_Name = '{TextBox_machine.Text}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataRow row = dt.NewRow();
                row["ID"] = DataTableUtils.toString(dt.Rows[0]["ID"]);
                row["Mach_Status"] = DropDownList_errorstatus.SelectedItem.Value;

                if (DataTableUtils.Update_DataRow("mach_content", $" Mach_Name = '{TextBox_machine.Text}'", row))
                {
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                    sqlcmd = "select * from change_history";
                    DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

                    if (ds != null)
                    {
                        DataRow rew = ds.NewRow();
                        rew["machine"] = TextBox_machine.Text;
                        rew["change_man"] = acc;
                        rew["old_status"] = dt.Rows[0]["Mach_Status"].ToString();
                        rew["new_status"] = DropDownList_errorstatus.SelectedItem.Value;
                        rew["happen_type"] = DropDownList_tpye.SelectedItem.Text;
                        rew["happen_reason"] = TextBox_reason.Text;
                        rew["happen_time"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (DataTableUtils.Insert_DataRow("change_history", rew))
                            Response.Write($"<script>alert('狀態修改完成');location.href='Machine_list_info_APS.aspx'</script>");

                    }
                }
            }
        }

        protected void button_save_Click(object sender, EventArgs e)
        {
            ////當下機台狀態
            //string machine_status = TextBox_machstatus.Text;
            ////當下機台訂單
            //string project = TextBox_project.Text;
            ////當下工藝名稱
            //string taskname = TextBox_taskname.Text;
            ////當下機台名稱
            //string machname = TextBox_machname.Text;
            ////當下機台工藝編號
            //string task = TextBox_task.Text;
            ////當下機台的工作編號
            //string _id = TextBox_id.Text;
            ////使用者選擇的狀態
            //string select_status = TextBox_ddlstatus.Text;
            ////符合條件的最後一筆狀態
            //string lost_status = TextBox_nowstatus.Text;

            string alert = "";
            if (WebUtils.GetAppSettings("alert_qty") == "1")
                alert = ShareFunction_APS.Calculate_QTY(TextBox_project.Text, TextBox_task.Text, TextBox_QTY.Text, DataTableUtils.toInt(TextBox_id.Text));
            if (alert == "")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                string sqlcmd = "select * from workhour_detail";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (dt != null)
                {
                    ShareFunction_APS.Insert_workdetail(dt, TextBox_QTY.Text, TextBox_machstatus.Text,
                                                         TextBox_project.Text, TextBox_taskname.Text, TextBox_id.Text,
                                                         DropDownList_Workman.SelectedItem.Text, TextBox_machname.Text,
                                                         TextBox_task.Text, TextBox_ddlstatus.Text, TextBox_nowstatus.Text, TextBox_bad.Text);
                    if (RadioButtonList_status.SelectedItem.Value == "完成")
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                        //Updata order APS List Status 
                        ShareFunction_APS.UpdataWorkHourData(TextBox_project.Text, TextBox_task.Text, WorkHourEditSource.報工明細, TextBox_machstatus.Text);
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
                        //Updata CNC VIS Status
                        ShareFunction_APS.UpdataCNCVisStatus(TextBox_project.Text, TextBox_task.Text, TextBox_machstatus.Text, WorkHourEditSource.報工明細);
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                        //Order Status 
                        ShareFunction_APS.UpdataWorkHourOrderData(TextBox_project.Text, TextBox_task.Text, WorkHourEditSource.報工明細, TextBox_machstatus.Text);
                        // Response.Write("<script>alert('該工藝開始');location.href='WorkHourList.aspx" + Request.Url.Query + "';</script>");
                    }
                }

                Response.Redirect("Machine_list_info_APS.aspx#tab_content2");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('數量超過{alert}個，請重新填寫');location.href='Machine_list_info_APS.aspx#tab_content2';</script>");

        }
    }
}