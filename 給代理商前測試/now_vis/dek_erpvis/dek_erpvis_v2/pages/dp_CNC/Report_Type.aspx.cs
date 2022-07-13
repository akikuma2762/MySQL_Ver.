using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class Report_Type : System.Web.UI.Page
    {
        public string path = "";
        public string color = "";
        string acc = "";
        ERP_cnc cnc = new ERP_cnc();
        myclass myclass = new myclass();
        clsDB_Server clsdb_cnc = new clsDB_Server(myclass.GetConnByDekVisCnc_inside);
        clsDB_Server clsdb_mysql = new clsDB_Server(myclass.GetConnByDekVisCnc_insideMysql);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Copy_CNCData();

            //確保能從前端成功讀取API
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Response.AppendHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            Response.AppendHeader("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE");
            //確保能從前端成功讀取API
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {

                }
                else
                    Response.Write("<script>alert('您無此權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect("../login.aspx");
        }

        //複製加工VIS資料
        private void Copy_CNCData()
        {
            //複製機台
            string sqlcmd = "select * from machine_info";
            DataTable dt_source = clsdb_mysql.GetDataTable(sqlcmd);
            DataTable dt_target = clsdb_cnc.GetDataTable(sqlcmd);
            var normalReceive = dt_source.AsEnumerable().Except(dt_target.AsEnumerable(), DataRowComparer.Default).OrderBy(o => o.Field<int>("_id"));
            if (normalReceive.FirstOrDefault() != null && HtmlUtil.Check_DataTable(dt_source) && dt_target != null)
            {
                //清空
                clsdb_cnc.Delete_Record("machine_info", "_id <> 0");

                //複製
                clsdb_cnc.Insert_TableRows("machine_info", dt_source);
            }

            //複製群組
            sqlcmd = "select * from mach_group";
            dt_source = clsdb_mysql.GetDataTable(sqlcmd);
            dt_target = clsdb_cnc.GetDataTable(sqlcmd);
            var diff_mach_group = dt_source.AsEnumerable().Except(dt_target.AsEnumerable(), DataRowComparer.Default).OrderBy(o => o.Field<int>("_id"));
            if (diff_mach_group.FirstOrDefault() != null && HtmlUtil.Check_DataTable(dt_source) && dt_target != null)
            {
                //清空
                clsdb_cnc.Delete_Record("mach_group", "_id <> 0");

                //複製
                clsdb_cnc.Insert_TableRows("mach_group", dt_source);
            }

            string url = string.Format(HtmlUtil.Get_Ini("Parameter", "Get_staffapi"), "");
            string json = JsonToDataTable.HttpGetJson(url);
            dt_source = JsonToDataTable.JsonStringToDataTable(json);
            sqlcmd = "select * from staff_info";
            dt_target = clsdb_cnc.GetDataTable(sqlcmd);
            //var diff_staff_info = dt_source.AsEnumerable().Except(dt_target.AsEnumerable(), DataRowComparer.Default).OrderBy(o => o.Field<int>("_id"));
            if (HtmlUtil.Check_DataTable(dt_source) && dt_target != null && dt_target.Rows.Count != dt_source.Rows.Count)
            {
                //清空
                clsdb_cnc.Delete_Record("staff_info", "_id <> 0");

                //複製
                clsdb_cnc.Insert_TableRows("staff_info", dt_source);
            }

            //複製異常
            sqlcmd = "select * from error_type";
            dt_source = clsdb_mysql.GetDataTable(sqlcmd);
            dt_target = clsdb_cnc.GetDataTable(sqlcmd);
            var normalReceiveErr = dt_source.AsEnumerable().Except(dt_target.AsEnumerable(), DataRowComparer.Default).OrderBy(o => o.Field<int>("_id"));
            if (normalReceiveErr.FirstOrDefault() != null && dt_target != null && dt_target.Rows.Count != dt_source.Rows.Count)
            {
                //清空
                clsdb_cnc.Delete_Record("error_type", "_id <> 0");
                //複製
                clsdb_cnc.Insert_TableRows("error_type", dt_source);
            }


            //複製LINE權杖
            sqlcmd = "select * from line_info";
            dt_source = clsdb_mysql.GetDataTable(sqlcmd);
            dt_target = clsdb_cnc.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt_source) && dt_target != null && dt_target.Rows.Count != dt_source.Rows.Count)
            {
                //清空
                clsdb_cnc.Delete_Record("line_info", "_id <> 0");

                //複製
                clsdb_cnc.Insert_TableRows("line_info", dt_source);
            }
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            int count = 0;
            // 已經入站的工單確認 0702
            string Condition = "";
            DataTable dt_Exist;
            DataTable dt_record = cnc.Get_DataTable("record_worktime");
            DataTable dt_clone = HtmlUtil.Get_HeadRow(dt_record);
            //找尋最大ID

            string Now_Time = DateTime.Now.ToString("yyyyMMddHHmmss");
            List<string> machine_list = new List<string>(TextBox_SaveMachine.Text.Split('#'));
            List<string> allorder_list = new List<string>(TextBox_SaveAllOrder.Text.Split('#'));
            List<string> staff_list = new List<string>(TextBox_SaveStaff.Text.Split('#'));

            //確定輸入的工單數量
            int order_count = (allorder_list.Count - 1) / (DataTableUtils.toInt(WebUtils.GetAppSettings("Array_Length")) + 1);

            //確認入站人員
            string Enter_Staff = "";
            string Enter_StaffNumber = "";
            for (int i = 0; i < staff_list.Count - 1; i++)
            {
                if (i % DataTableUtils.toInt(WebUtils.GetAppSettings("Staff_Length")) == 0)
                    Enter_StaffNumber += Enter_StaffNumber == "" ? staff_list[i] : $"/{staff_list[i]}";
                if (i % DataTableUtils.toInt(WebUtils.GetAppSettings("Staff_Length")) == 1)
                    Enter_Staff += Enter_Staff == "" ? staff_list[i] : $"/{staff_list[i]}";
            }

            DataTable dt = cnc.Get_DataTable("workorder_information");
            List<bool> ok = new List<bool>();
            if (dt != null)
            {
                for (int i = 0; i < order_count; i++)
                {
                    //訂單都塞到長度裡面 用幾個代表一筆訂單 *n的概念
                    int orders = i * (DataTableUtils.toInt(WebUtils.GetAppSettings("Array_Length")) + 1);
                    List<string> order = new List<string>(allorder_list[orders].Split('_'));
                    DataRow row = dt.NewRow();
                    //工單序號
                    row["_id"] = allorder_list[orders];
                    //機台名稱
                    row["mach_name"] = machine_list[0].ToString().Trim();
                    //料號
                    row["product_number"] = allorder_list[orders + 1].ToString().Trim(); 
                    //工單序號
                    row["manu_id"] = allorder_list[orders].ToString().Trim();
                    //品名
                    row["product_name"] = allorder_list[orders + 2];
                    //規格
                    row["specification"] = allorder_list[orders + 3];
                    //加工順序
                    row["orderno"] = allorder_list[orders + 4].ToString().Trim();
                    //預計生產數量
                    row["exp_product_count_day"] = allorder_list[orders + 5];
                    //已完成數量
                    row["product_count_day"] = allorder_list[orders + 6];
                    //未完成量
                    row["no_product_count_day"] = allorder_list[orders + 7];
                    //預交日期
                    row["delivery"] = allorder_list[orders + 8];
                    //製程編號
                    row["craft_Number"] = allorder_list[orders + 9];
                    //製程名稱
                    row["craft_name"] = allorder_list[orders + 10];

                    //客戶代號
                    row["custom_number"] = allorder_list[orders + 11];
                    //客戶名稱
                    row["custom_name"] = allorder_list[orders + 12];
                    //製程型態
                    row["processType"] = allorder_list[orders + 13];
                    //報工 OR 維護
                    row["type_mode"] = TextBox_SaveMode.Text.ToString().Trim();
                    //工單狀態
                    row["order_status"] = "入站";
                    //現在時間
                    row["now_time"] = Now_Time;
                    //最後更新時間
                    row["last_updatetime"] = Now_Time;
                    //員工代號
                    row["staff_Number"] = Enter_StaffNumber;
                    //員工姓名
                    row["work_staff"] = Enter_Staff;
                    //工單歷史員工
                    row["history_workstaff"] = Enter_Staff;
                    //乘
                    row["multiplication"] = TextBox_Multiplication.Text;
                    //除
                    row["division"] = TextBox_Division.Text;
                    Condition = $"manu_id ='{row["manu_id"]}' and orderno ='{row["orderno"]}' and order_status ='入站'";
                    dt_Exist = clsdb_cnc.DataTable_GetRow($"select * from workorder_information where {Condition}");
                    if (dt_Exist == null || dt_Exist.Rows.Count == 0)
                    {
                        ok.Add(allorder_list[orders + 14] == "insert" ? clsdb_cnc.Insert_DataRow("workorder_information", row) : clsdb_cnc.Update_DataRow("workorder_information", $"_id='{allorder_list[orders]}'", row));
                        string EerrorMsgstr = clsdb_cnc.ErrorMessage;
                        //儲存入站資訊
                        for (int j = 0; j < staff_list.Count - 1; j++)
                        {
                            DataRow rows = dt_clone.NewRow();
                            rows["mach_name"] = machine_list[0].ToString().Trim();
                            rows["manu_id"] = allorder_list[orders].ToString().Trim();
                            rows["product_number"] = allorder_list[orders + 1].ToString().Trim();
                            rows["product_name"] = allorder_list[orders + 2];
                            rows["work_staff"] = staff_list[j + 1].ToString().Trim();
                            rows["workman_status"] = "入站";
                            rows["report_qty"] = "0";
                            rows["qty_status"] = "良品";
                            rows["type_mode"] = TextBox_SaveMode.Text.ToString().Trim();
                            rows["now_time"] = Now_Time;
                            rows["ProcessType"] = allorder_list[orders + 13];
                            dt_clone.Rows.Add(rows);
                            j++;
                            count++;
                        }
                    }
                    else
                    {
                        //表示這張單已經正在入站
                        ok.Add(true);
                    }
                }
                if (ok.IndexOf(false) == -1)
                {
                    if (clsdb_cnc.Insert_TableRows("record_worktime", dt_clone) == dt_clone.Rows.Count)
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('已新增成功{ok.Count}筆');location.href='Report_Type.aspx';</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增異常');location.href='Report_Type.aspx';</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增異常');location.href='Report_Type.aspx';</script>");
            }
        }
    }
}