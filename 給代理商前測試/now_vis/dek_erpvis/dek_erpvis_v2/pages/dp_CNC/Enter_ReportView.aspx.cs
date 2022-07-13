﻿using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using dekERP_dll.dekErp;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using System.Web.UI.WebControls;


namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class Enter_ReportView : System.Web.UI.Page
    {
        //--------------------------------引用 OR 參數---------------------------------------
        ERP_cnc CNC = new ERP_cnc();
        public string color = "";
        public string path = "";
        public string Refresh_Time = "";
        string acc = "";
        public StringBuilder tr = new StringBuilder();
        public StringBuilder th = new StringBuilder();
        DataTable dt_monthtotal = new DataTable();
        List<string> columns = new List<string>();
        public string mach = "";
        List<string> machlist = new List<string>();
        myclass myclass = new myclass();
        clsDB_Server cls_db = new clsDB_Server(myclass.GetConnByDekVisCnc_inside);
        //--------------------------------引用 OR 參數---------------------------------------
        //------------------------------------事件-------------------------------------------
        //載入事件
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                path = 德大機械.get_title_web_path("DES");
                color = HtmlUtil.change_color(acc);
                Refresh_Time = HtmlUtil.set_Reflashtime(acc);
                Refresh_Time = "5000000";
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    if (!IsPostBack)
                        MainProcess();
                }
                else
                    Response.Write("<script>alert('您無此權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }

        //暫停事件  OK
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            string sqlcmd = $"select * from (  SELECT  *, (SELECT  MIN(a.now_time) FROM record_worktime a WHERE workman_status = '出站'  AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id) exit_time FROM record_worktime WHERE workman_status = '入站') a where  a.exit_time IS null and product_number='{TextBox_Number.Text}' and mach_name='{TextBox_Machine.Text}'  and now_time <='{DateTime.Now:yyyyMMddHHmmss}'   and type_mode='進站報工' ";
            DataTable dt = cls_db.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable dt_clone = HtmlUtil.Get_HeadRow(dt);
                int j = 0;

                string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                //紀錄暫停原因跟類型
                foreach (DataRow row in dt.Rows)
                {
                    DataRow rows = dt_clone.NewRow();

                    rows["mach_name"] = row["mach_name"];
                    rows["manu_id"] = row["manu_id"];
                    rows["product_number"] = row["product_number"];
                    rows["product_name"] = row["product_name"];
                    rows["work_staff"] = row["work_staff"];
                    rows["workman_status"] = "暫停";
                    rows["report_qty"] = "0";
                    rows["qty_status"] = "良品";
                    rows["now_time"] = now_time;
                    rows["stop_type"] = DropDownList_StopType.SelectedItem.Text;
                    rows["stop_reason"] = TextBox_content.Text;
                    rows["type_mode"] = "進站報工";
                    dt_clone.Rows.Add(rows);
                    j++;
                }

                if (dt_clone.Rows.Count == cls_db.Insert_TableRows("record_worktime", dt_clone))
                {
                    //更新狀態
                    sqlcmd = $"select * from workorder_information where mach_name='{TextBox_Machine.Text}' and product_number='{TextBox_Number.Text}' and order_status='入站'   and type_mode='進站報工'";
                    dt = cls_db.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt))
                    {
                        List<bool> ok = new List<bool>();
                        foreach (DataRow row in dt.Rows)
                        {
                            DataRow rows = dt.NewRow();
                            rows["_id"] = row["_id"];
                            rows["order_status"] = "暫停";
                            rows["last_updatetime"] = now_time;
                            rows["error_type"] = DropDownList_StopType.SelectedItem.Text;
                            ok.Add(cls_db.Update_DataRow("workorder_information", $"mach_name='{TextBox_Machine.Text}' and product_number='{TextBox_Number.Text}' and manu_id='{row["manu_id"]}'  and order_status='入站'   and type_mode='進站報工'", rows));
                        }
                        if (ok.IndexOf(false) == -1)
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存成功');location.href='Enter_ReportView.aspx';</script>");
                        else
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Enter_ReportView.aspx';</script>");
                    }
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Enter_ReportView.aspx';</script>");
            }
        }

        //取消暫停事件  OK
        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            string sqlcmd = $"SELECT  * FROM (SELECT  *, (SELECT  MIN(a.now_time) FROM record_worktime a WHERE workman_status = '取消暫停' AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id) cancel_time FROM record_worktime WHERE workman_status = '暫停') a WHERE a.cancel_time IS NULL  and product_number='{TextBox_Number.Text}' and mach_name='{TextBox_Machine.Text}'  and now_time <='{DateTime.Now:yyyyMMddHHmmss}'   and type_mode='進站報工'";
            DataTable dt = cls_db.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable dt_clone = HtmlUtil.Get_HeadRow(dt);
                int j = 0;

                string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                //紀錄暫停原因跟類型
                foreach (DataRow row in dt.Rows)
                {
                    DataRow rows = dt_clone.NewRow();
                    rows["mach_name"] = row["mach_name"];
                    rows["manu_id"] = row["manu_id"];
                    rows["product_number"] = row["product_number"];
                    rows["product_name"] = row["product_name"];
                    rows["work_staff"] = row["work_staff"];
                    rows["workman_status"] = "取消暫停";
                    rows["report_qty"] = "0";
                    rows["qty_status"] = "良品";
                    rows["now_time"] = now_time;
                    rows["type_mode"] = "進站報工";
                    dt_clone.Rows.Add(rows);
                    j++;
                }

                if (dt_clone.Rows.Count == cls_db.Insert_TableRows("record_worktime", dt_clone))
                {
                    //更新狀態
                    sqlcmd = $"select * from workorder_information where mach_name='{TextBox_Machine.Text}' and product_number='{TextBox_Number.Text}' and order_status='暫停'  and type_mode='進站報工'";
                    dt = cls_db.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dt))
                    {
                        List<bool> ok = new List<bool>();
                        foreach (DataRow row in dt.Rows)
                        {
                            DataRow rows = dt.NewRow();
                            rows["_id"] = row["_id"];
                            rows["order_status"] = "入站";
                            rows["last_updatetime"] = now_time;
                            rows["error_type"] = "";

                            ok.Add(cls_db.Update_DataRow("workorder_information", $"mach_name='{TextBox_Machine.Text}' and product_number='{TextBox_Number.Text}' and manu_id='{row["manu_id"]}' and order_status='暫停'   and type_mode='進站報工'", rows));
                        }
                        if (ok.IndexOf(false) == -1)
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存成功');location.href='Enter_ReportView.aspx';</script>");
                        else
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Enter_ReportView.aspx';</script>");
                    }
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Enter_ReportView.aspx';</script>");
            }
        }

        //跳頁(跳至異常回復)  OK
        protected void Button_jump_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>(TextBox_Staffs.Text.Split('#'));
            string staff = "";
            for (int i = 0; i < list.Count - 1; i++)
            {
                i++;
                staff += staff == "" ? list[i] : $"/{list[i]}";
            }
            string url = $"machine={TextBox_Machine.Text},number={TextBox_Number.Text},order={TextBox_Order.Text},show_name={TextBox_Show.Text},group={TextBox_Group.Text},type=進站報工,staff={staff}";
            Response.Redirect($"Error_Detail.aspx?key={WebUtils.UrlStringEncode(url)}");
        }

        //工單出站 OR 報工 OK
        protected void Button_Exit_Click(object sender, EventArgs e)
        {
            //0607 juiedit
            string Order_Number = TextBox_Order.Text;
            string Taget_ordermo = TextBox_orderno.Text;
            string Taget_Product_Num = TextBox_Number.Text;
            string Run_ordermo = "";
            string Run_Product_Num = "";
            string good_qtytmp = TextBox_Good.Text;
            string bad_qtytmp = TextBox_badqty.Text;
            string NowCounttmp = "";
            //良品,不良品
            string good_qty = TextBox_Good.Text;//JS會先做 按了回覆數量 10(上次)+10(本次)
            string bad_qty = TextBox_badqty.Text;
            string NowCount = TextBox_NowCount.Text;
            string MachName = TextBox_Machine.Text;
            string Badinf = TextBox_BadInformation.Text;
            DataTable dt_ng, dt_exit;
            //0629 設定資料庫還原點
            //cls_db.dbOpen(myclass.GetConnByDekVisCnc_inside);
            //判斷為出站還是單純報工
            bool exit = DataTableUtils.toString(((Control)sender).ID) == "Button_Report" ? false : true;
            //找出要報工的資料表

            //MYSQL
            //string sqlcmd = $"SELECT  workorder_information.*, ifnull(a.good_qty ,0) report_good, ifnull(a.bad_qty ,0) report_bad FROM workorder_information LEFT JOIN (SELECT  * FROM workorder_information where order_status = '出站' order by now_time desc limit 1) a  ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number and a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE workorder_information.mach_name = '{TextBox_Machine.Text}' AND workorder_information.type_mode = '進站報工' AND workorder_information.order_status = '入站' ORDER BY workorder_information.delivery ASC";
            //MSSQL
            string sqlcmd = $"SELECT workorder_information.*, isnull(a.good_qty, 0) report_good, isnull(a.bad_qty, 0) report_bad,(SELECT is_collect   FROM machine_info   WHERE machine_info.mach_name = workorder_information.mach_name) is_collect FROM workorder_information LEFT JOIN (SELECT TOP(1) *  FROM workorder_information WHERE order_status = '出站' ORDER BY now_time DESC ) a ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number AND a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE workorder_information.mach_name =  '{MachName}' AND workorder_information.type_mode = '進站報工' AND workorder_information.order_status = '入站'  ORDER BY workorder_information.delivery ASC";
            DataTable dt = cls_db.GetDataTable(sqlcmd);

            //現在時間
            string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");

            //不良資訊
            List<string> bad_list = new List<string>(Badinf.Split('#'));
            if (HtmlUtil.Check_DataTable(dt))
            {
                dt = ProcessTypeOrder(dt, Order_Number);
                DataTable dt_new = dt.Clone();
                //分群組-
                //變更同品項同工藝才可以進行分配  0607 juiedit
                var dt_groupby = dt.AsEnumerable().GroupBy(w => new { g = w.Field<string>("product_number"), h = w.Field<string>("orderno") }).Select(s => s);
                //var dt_groupby = dt.AsEnumerable().Where(w=>w.Field<string>("product_number")== Taget_Product_Num && w.Field<string>("orderno")== Taget_ordermo).GroupBy(w => new { g = w.Field<string>("product_number") }).Select(s => s);
                foreach (var key in dt_groupby)
                {
                    //良品初始值
                    double OK_qty = 0;
                    //不良初始值
                    double NG_qty = 0;
                    var dt_gpSamekey = key.CopyToDataTable();
                    //需求的項目才需要累加(品項依樣的,工藝一樣的)
                    Run_ordermo = dt_gpSamekey.AsEnumerable().Select(s => s.Field<string>("orderno")).FirstOrDefault();
                    Run_Product_Num = dt_gpSamekey.AsEnumerable().Select(s => s.Field<string>("product_number")).FirstOrDefault();
                    if (Taget_ordermo == Run_ordermo && Taget_Product_Num == Run_Product_Num)
                    {
                        //JS關係 good_qty 是加上去的數字(處理後)
                        good_qtytmp = good_qty;
                        bad_qtytmp = bad_qty;
                        NowCounttmp = NowCount;
                    }
                    else
                    {
                        good_qtytmp = "0";
                        bad_qtytmp = "0";
                        NowCounttmp = "0";
                        exit = false;
                        continue;
                        //不同品項不同工藝 直接cotinu
                    }

                    foreach (DataRow row in dt_gpSamekey.Rows)
                    {
                        //處理該工單的good_qty
                        if (good_qtytmp == "0")
                            good_qtytmp = DataTableUtils.toInt(row["product_count_day"].ToString()).ToString();
                        //取出對應的不良資訊 
                        sqlcmd = $"select * from bad_total where mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}' and type_mode='進站報工'";
                        DataTable dt_bad = cls_db.GetDataTable(sqlcmd);

                        //刪除對應的不良資訊
                        if (HtmlUtil.Check_DataTable(dt_bad))
                            cls_db.Delete_Record("bad_total", $"mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}'");
                        //取出對應的紀錄資訊
                        sqlcmd = $"select * from record_worktime where mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}' and type_mode='進站報工' and workman_status = '中途報工'";
                        DataTable dt_record = cls_db.GetDataTable(sqlcmd);
                        //刪除對應的紀錄資訊(抓出中途報工的)
                        //cls_db.Txact_begin();
                        if (HtmlUtil.Check_DataTable(dt_record))
                            cls_db.Delete_Record("record_worktime", $"mach_name = '{row["mach_name"]}' and product_number = '{row["product_number"]}' and manu_id = '{row["manu_id"]}' and now_time>='{row["now_time"]}' and type_mode='進站報工' and workman_status = '中途報工'");
                        /*
                         已完成數量 = 維護數量 + 良品數量(已入ERP) + 不良數量(已入ERP)
                         未完成數量 = 總需求量 - (維護數量 + 良品數量(已入ERP) + 不良數量(已入ERP))
                         良品數量   = 良品數量(已入ERP)
                         不良數量   = 不良數量(已入ERP)*/
                        //row["product_count_day"] = DataTableUtils.toDouble(row["maintain_qty"]) + DataTableUtils.toDouble(row["report_good"]) + DataTableUtils.toDouble(row["report_bad"]);
                        //row["no_product_count_day"] = DataTableUtils.toDouble(row["exp_product_count_day"]) - DataTableUtils.toDouble(row["maintain_qty"]) - DataTableUtils.toDouble(row["report_bad"]) - DataTableUtils.toDouble(row["report_good"]);
                        //row["good_qty"] = DataTableUtils.toDouble(row["report_good"]);
                        //row["bad_qty"] = DataTableUtils.toDouble(row["report_bad"]);
                        //row["adj_qty"] = DataTableUtils.toString(DataTableUtils.toInt(row["adj_qty"].ToString()) + DataTableUtils.toInt(good_qtytmp));
                        /* 0705=================================
                        當下數量   = 未出站的累積
                        未完成數量 = 總需求量 - (維護數量 + 良品數量(已入ERP) + 不良數量(已入ERP))
                        良品數量   = 良品數量(已入ERP)
                        不良數量   = 不良數量(已入ERP)*/
                        row["product_count_day"] = DataTableUtils.toDouble(row["maintain_qty"]) + DataTableUtils.toDouble(row["report_good"]) + DataTableUtils.toDouble(row["report_bad"]);
                        row["no_product_count_day"] = DataTableUtils.toDouble(row["exp_product_count_day"]) - DataTableUtils.toDouble(row["maintain_qty"]) - DataTableUtils.toDouble(row["report_bad"]) - DataTableUtils.toDouble(row["report_good"]);
                        row["good_qty"] = DataTableUtils.toDouble(row["report_good"]);
                        row["bad_qty"] = DataTableUtils.toDouble(row["report_bad"]);
                        row["adj_qty"] = DataTableUtils.toString(DataTableUtils.toInt(row["adj_qty"].ToString()) + DataTableUtils.toInt(good_qtytmp));
                    }
                    //加上處理後(乘除)的良品數量
                    if (DataTableUtils.toString(dt_gpSamekey.Rows[0]["is_collect"]) == "否")
                        OK_qty += DataTableUtils.toDouble(good_qtytmp) * DataTableUtils.toDouble(DataTableUtils.toString(dt_gpSamekey.Rows[0]["multiplication"])) / DataTableUtils.toDouble(DataTableUtils.toString(dt_gpSamekey.Rows[0]["division"]));
                    else
                        OK_qty += DataTableUtils.toDouble(good_qtytmp);

                    //加入輸入之不良品數量
                    NG_qty += DataTableUtils.toDouble(bad_qtytmp);

                    //進行良品分配

                    if (CNCReport.Order_OKExit(dt_gpSamekey, OK_qty, now_time, "進站報工", NowCounttmp, exit))
                    {
                        if (WebUtils.GetAppSettings("Calculate_Ng") == "1")
                        {
                            //重新搜尋
                            sqlcmd = $"select * from workorder_information where mach_name='{MachName}' and type_mode='進站報工' and order_status ='入站' and product_number='{Run_Product_Num}' and orderno='{Run_ordermo}' order by delivery asc ";
                            dt_ng = cls_db.GetDataTable(sqlcmd);
                            dt_ng = ProcessTypeOrder(dt_ng, Order_Number);
                            //進行不良品分配
                            if (CNCReport.Order_NGExit(dt_ng, NG_qty, now_time, "進站報工", exit, bad_list))
                            {
                                //若為出站，則須將資料透過API傳至ERP內
                                if (exit)
                                {
                                    //透過API post回ERP內部
                                    sqlcmd = $"select * from workorder_information where mach_name = '{MachName}' and type_mode= '進站報工' and order_status ='出站'  and last_updatetime = '{now_time}' and product_number='{Run_Product_Num}' and orderno='{Run_ordermo}' order by delivery asc ";
                                    dt_exit = cls_db.GetDataTable(sqlcmd);
                                    if (HtmlUtil.Check_DataTable(dt_exit))
                                    {
                                        foreach (DataRow row in dt_exit.Rows)
                                        {
                                            CNCReport.PostToERP(row["manu_id"].ToString(), row["mach_name"].ToString());
                                            //showMsg(Resoult.ToString());
                                            //if (Resoult.ToString()!="PASS")
                                            //{
                                            //    cls_db.Txact_rollback();
                                            //    showMsg(Resoult.ToString());
                                            //    break;
                                            //}
                                        }

                                        //cls_db.Txact_commit();
                                        showMsg("儲存成功");
                                    }
                                    else
                                    {
                                        //cls_db.Txact_rollback();
                                        showMsg("儲存失敗");
                                    }
                                }
                                else
                                {
                                    //cls_db.Txact_commit();
                                    showMsg("儲存成功");
                                }
                            }
                            else
                            {
                                // cls_db.Txact_rollback();
                                showMsg("儲存失敗");
                            }
                        }
                        else
                        {
                            // cls_db.Txact_commit();
                            showMsg("儲存成功");
                        }
                    }
                    else
                    {
                        // cls_db.Txact_rollback();
                        showMsg("儲存失敗");
                    }
                }
            }
            else
            {
                cls_db.Txact_rollback();
                showMsg("儲存失敗");
            }
        }

        //欄位移動事件儲存 OK
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        //選取顯示欄位與廠區群組 OK
        protected void Button_Check_Click(object sender, EventArgs e)
        {
            //儲存需顯示之欄位

            string sqlcmd = $"select * from show_column where Account='{acc}' and use_page='Enter_ReportView'";
            DataTable dt = cls_db.GetDataTable(sqlcmd);
            if (dt != null)
            {
                //先刪除裡面的資料

                cls_db.Delete_Record("show_column", $"Account='{acc}' and use_page='Enter_ReportView'");

                //取得最大ID

                sqlcmd = "SELECT max(ID) ID FROM show_column";
                DataTable dt_max = cls_db.GetDataTable(sqlcmd);
                int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["ID"].ToString()) + 1 : 1;

                DataTable dt_clone = dt.Clone();

                for (int i = 0; i < CheckBoxList_Columns.Items.Count; i++)
                {
                    DataRow row = dt_clone.NewRow();
                    row["id"] = max + i;
                    row["Column_Name"] = CheckBoxList_Columns.Items[i].Text;
                    row["Account"] = acc;
                    row["Allow"] = CheckBoxList_Columns.Items[i].Selected ? "True" : "False";
                    row["use_page"] = "Enter_ReportView";
                    dt_clone.Rows.Add(row);
                }

                if (cls_db.Insert_TableRows("show_column", dt_clone) == dt_clone.Rows.Count)
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

                    machlist = new List<string>(TextBox_Machines.Text.Split(','));
                    MainProcess();
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Enter_ReportView.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Enter_ReportView.aspx';</script>");
        }

        //須執行之副程式 OK
        private void MainProcess()
        {
            if (WebUtils.GetAppSettings("add_maintain") == "1")
                CNCReport.Maintain_to_Report();
            Set_Dropdownlist();
            Set_Factory();
            Set_Checkbox();
            Get_MonthTotal();
            Set_Table();
        }

        //取得目前所有的狀態為未出站的資料 OK
        private void Get_MonthTotal()
        {
            if (machlist.Count > 0)
            {
                string sqlcmd = "select * from machine_info";
                DataTable dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    for (int i = 0; i < machlist.Count; i++)
                    {
                        DataRow[] rows = dt.Select($"mach_show_name='{machlist[i]}'");
                        if (rows != null && rows.Length > 0)
                            machlist[i] = DataTableUtils.toString(rows[0]["mach_name"]);
                    }
                }
            }
            dt_monthtotal = CNC.Enter_ReportView("cnc", machlist);
            changeMachAreaGroup();
            changeFinishQty();//所有的後續修正改在這...避免跟之前的衝突...7//7
        }
        //將區域名稱換成 群組名稱
        private void changeMachAreaGroup()
        {
            string sqlcmd = "select * from machine_info";
            DataTable dt = cls_db.GetDataTable(sqlcmd);
            sqlcmd = "select * from mach_group";
            DataTable dt_gp = cls_db.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt) && HtmlUtil.Check_DataTable(dt_gp))
            {
                dt_monthtotal.Columns["設備群組"].ReadOnly = false;
                for (int i = 0; i < dt_monthtotal.Rows.Count; i++)
                {
                    var gp = dt_gp.AsEnumerable().Where(w => !string.IsNullOrEmpty(w.Field<string>("mach_Name")) && w.Field<string>("mach_Name").Contains(dt_monthtotal.Rows[i]["設備代號"].ToString())).Select(s => s.Field<string>("group_name")).FirstOrDefault();
                    if (gp != null)
                        dt_monthtotal.Rows[i]["設備群組"] = gp;
                }
            }
        }
        private void changeFinishQty()
        {
            string sqlcmd = "";
            int Count = 0;
            DataTable dt;
            dt_monthtotal.Columns["已生產量"].ReadOnly = false;
            dt_monthtotal.Columns["未生產量"].ReadOnly = false;
            dt_monthtotal.Columns["進度"].ReadOnly = false;
            for (int i = 0; i < dt_monthtotal.Rows.Count; i++)
            {
                Count = 0;
                //最近的一筆生產累積數量(一定沒超過需求..超過需求ERp就會結案 就不會再拉到單子..所以可以進站表示一定還不滿足)
                sqlcmd = $"select product_count_day  as 已完成數量 from workorder_information where order_Status='出站' and mach_name='{dt_monthtotal.Rows[i]["設備代號"]}' and manu_id='{dt_monthtotal.Rows[i]["工單報工"]}' and product_number='{dt_monthtotal.Rows[i]["品號"]}' and orderno='{dt_monthtotal.Rows[i]["工序號碼"]}' order by last_updatetime desc";
                dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                    Count = DataTableUtils.toInt(dt.AsEnumerable().FirstOrDefault()["已完成數量"].ToString());
                dt_monthtotal.Rows[i]["已生產量"] = Count.ToString();
                dt_monthtotal.Rows[i]["未生產量"] = (DataTableUtils.toInt(dt_monthtotal.Rows[i]["預計產量"].ToString()) - Count).ToString();
                dt_monthtotal.Rows[i]["進度"] = DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString()) > 0 ? $"{ (DataTableUtils.toInt(dt_monthtotal.Rows[i]["當下產量"].ToString()) * 100 / DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString())).ToString():0}" : "100";

            }
        }
        //設定核取方塊 OK
        private void Set_Checkbox()
        {
            CheckBoxList_Columns.Items.Clear();
            //找到系統預設的
            DataTable dt_System = CNC.System_columns("Enter_ReportView");
            //找到個人設定的
            DataTable dt_Person = CNC.Person_columns("Enter_ReportView", acc);

            //個人的
            if (HtmlUtil.Check_DataTable(dt_System))
            {
                ListItem listItem = new ListItem();
                //個人的
                if (HtmlUtil.Check_DataTable(dt_Person))
                {
                    foreach (DataRow row in dt_System.Rows)
                    {
                        listItem = new ListItem(DataTableUtils.toString(row["info_chinese"]), DataTableUtils.toString(row["info_name"]));
                        var select = dt_Person.AsEnumerable().Where(w => w.Field<string>("info_name") == DataTableUtils.toString(row["info_name"]));
                        if (select.FirstOrDefault() != null)
                        {
                            listItem.Selected = true;
                            columns.Add(DataTableUtils.toString(row["info_chinese"]));
                        }
                        CheckBoxList_Columns.Items.Add(listItem);
                    }
                }
                //系統的
                else
                {
                    foreach (DataRow row in dt_System.Rows)
                    {
                        listItem = new ListItem(DataTableUtils.toString(row["info_chinese"]), DataTableUtils.toString(row["info_name"]));
                        listItem.Selected = true;
                        columns.Add(DataTableUtils.toString(row["info_chinese"]));
                        CheckBoxList_Columns.Items.Add(listItem);
                    }
                }
                columns.Add("");
            }

        }

        //設定異常類型 OK
        private void Set_Dropdownlist()
        {
            if (DropDownList_StopType.Items.Count == 0)
            {

                string sqlcmd = "select * from error_type where err_type_title = '暫停類型' and err_type <> 'ERROR'";
                DataTable dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    DropDownList_StopType.Items.Clear();
                    ListItem list = new ListItem();
                    foreach (DataRow row in dt.Rows)
                    {
                        list = new ListItem(DataTableUtils.toString(row["err_type"]));
                        DropDownList_StopType.Items.Add(list);
                    }
                    list = new ListItem("ERROR");
                    DropDownList_StopType.Items.Add(list);
                }
            }

            if (DropDownList_bad.Items.Count == 0)
            {

                string sqlcmd = "select * from error_type where err_type_title = '不良類型'";
                DataTable dt = cls_db.GetDataTable(sqlcmd);

                if (HtmlUtil.Check_DataTable(dt))
                {
                    DropDownList_bad.Items.Clear();
                    ListItem list = new ListItem();
                    list = new ListItem("");
                    DropDownList_bad.Items.Add(list);
                    foreach (DataRow row in dt.Rows)
                    {
                        list = new ListItem(DataTableUtils.toString(row["err_type"]));
                        DropDownList_bad.Items.Add(list);
                    }
                }
            }
        }

        //設定廠區群組的下拉選單 OK
        private void Set_Factory()
        {
            CNCError.Set_FactoryDropdownlist(acc, DropDownList_MachType, Request.FilePath);
        }

        //產生DataTable OK
        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                DataTable dt_mach = dt_monthtotal.DefaultView.ToTable(true, new string[] { "設備代號" });
                if (HtmlUtil.Check_DataTable(dt_mach))
                    foreach (DataRow row in dt_mach.Rows)
                        mach += $"{row["設備代號"]}#";
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(columns, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, dt_monthtotal, order_list, Enter_ReportView_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

        //例外處理 Ok
        private string Enter_ReportView_callback(DataRow row, string field_name)
        {
            string value = "";
            string orderno = "";
            int lastFinishCount = 0;
            DataTable dr_lastFinish;
            if (field_name == "工單報工")
            {
                string staff_information = "";
                List<string> staff_Number = new List<string>(DataTableUtils.toString(row["人員代號"]).Split('/'));
                List<string> staff_Name = new List<string>(DataTableUtils.toString(row["人員名稱"]).Split('/'));
                for (int i = 0; i < staff_Number.Count; i++)
                    staff_information += $"{staff_Number[i]}#{staff_Name[i]}#";
                string now_information = $"{row["設備名稱"].ToString().Replace(" ", "")}^{row["工單號碼"]}^{row["品號"]}^{row["品名"]}^{row["預計產量"]}^{row["當下產量"]}^{DataTableUtils.toDouble(row["已生產量"])}^{row["未生產量"]}^{DataTableUtils.toDouble(row["進度"]):0}%^{HtmlUtil.StrToDate(row["開工時間"].ToString()):yyyy/MM/dd HH:mm:ss}^{row["製程名稱"]}^{row["人員名稱"]}^";
                string bad_information = "";
                double ok_qty = 0;
                double ng_qty = 0;
                double ok_qty_Sum = 0;
                double ng_qty_Sum = 0;

                //抓到不良品的資訊 MYSQL
                //string sqlcmd = $"select bad_type,sum(bad_qty) bad_qty,bad_content from bad_total where mach_name = '{row["設備代號"]}' and product_number = '{row["品號"]}' and now_time >= '{row["開工時間"]}' and type_mode='進站報工' group by bad_type,bad_content";

                //MSSQL
                string sqlcmd = $"select bad_type,sum(cast(bad_qty as int)) bad_qty,bad_content from bad_total where mach_name = '{row["設備代號"]}' and product_number = '{row["品號"]}' and now_time >= '{row["開工時間"]}' and type_mode='進站報工' group by bad_type,bad_content";

                DataTable dt = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                    foreach (DataRow rew in dt.Rows)
                        bad_information += $"{rew["bad_type"]}Ω{rew["bad_qty"]}Ω{rew["bad_content"]}Ω";

                //ori
                //sqlcmd = $"select * from workorder_information where order_status = '入站' and mach_name = '{row["設備代號"]}' and product_number = '{row["品號"]}' and type_mode='進站報工'";
                //取得工藝編號 0607 juiedit 
                sqlcmd = $"select * from workorder_information where order_status = '入站' and mach_name = '{row["設備代號"]}' and product_number = '{row["品號"]}' and type_mode='進站報工' and manu_id='{row["工單報工"].ToString()}'";
                dt = cls_db.GetDataTable(sqlcmd);
                //取得工藝編號 0607 juiedit 
                orderno = dt.AsEnumerable().Where(w => w.Field<string>("manu_id") == row["工單報工"].ToString()).Select(s => s.Field<string>("orderno")).FirstOrDefault();
                //取得最近的完工紀錄 為了取得最近一次良品數
                sqlcmd = $"select * from record_worktime where workman_status = '中途報工' and mach_name = '{row["設備代號"]}' and product_number = '{row["品號"]}' and type_mode='進站報工' and manu_id='{row["工單報工"].ToString()}' and  qty_status='良品'";
                dr_lastFinish = cls_db.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dr_lastFinish))
                    lastFinishCount = DataTableUtils.toInt(dr_lastFinish.Rows[0]["report_qty"].ToString());
                if (HtmlUtil.Check_DataTable(dt))
                {

                    foreach (DataRow rew in dt.Rows)
                    {
                        //取得所有的良品數量
                        ok_qty += DataTableUtils.toDouble(rew["good_qty"]);

                        //取得所有不良品的數量
                        ng_qty += DataTableUtils.toDouble(rew["bad_qty"]);
                    }
                }

                sqlcmd = $"SELECT is_collect FROM machine_info WHERE mach_name='{row["設備代號"]}'";
                dt = cls_db.GetDataTable(sqlcmd);
                string collect = "否";
                collect = HtmlUtil.Check_DataTable(dt) ? DataTableUtils.toString(dt.Rows[0]["is_collect"]) : collect;
                //加入總量的參數 用來同品號同工藝的分配
                ok_qty_Sum = GetQtySum(row, true);
                ng_qty_Sum = GetQtySum(row, false);
                //value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/canclick.png\"  width=\"50px\" height=\"50px\" data-toggle = \"modal\" data-target = \"#Report_Model\" onclick=Set_Information(\"{row["設備群組"]}\",\"{row["設備名稱"].ToString().Replace(" ", "")}\",\"{row["工單狀態"]}\",\"{row["品號"].ToString().Trim()}\",\"{row["工單報工"]}\",\"{row["設備代號"]}\",\"{now_information.Replace(' ', '*')}\",\"{staff_information}\",\"{row["狀態"]}\",\"{bad_information}\",\"{ok_qty}\",\"{ng_qty}\",\"{collect}\",\"{orderno}\") /></a></label>";
                value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/canclick.png\"  width=\"50px\" height=\"50px\" data-toggle = \"modal\" data-target = \"#Report_Model\" onclick=Set_Information(\"{row["設備群組"]}\",\"{row["設備名稱"].ToString().Replace(" ", "")}\",\"{row["工單狀態"]}\",\"{row["品號"].ToString().Trim()}\",\"{row["工單報工"]}\",\"{row["設備代號"]}\",\"{now_information.Replace(' ', '*')}\",\"{staff_information}\",\"{row["狀態"]}\",\"{bad_information}\",\"{lastFinishCount.ToString()}\",\"{ng_qty}\",\"{collect}\",\"{orderno}\",\"{ok_qty_Sum}\",\"{ng_qty_Sum}\") /></a></label>";
            }
            else if (field_name == "工單狀態")
            {
                if (DataTableUtils.toString(row["狀態"]) == "ERROR")
                    value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/Light_ExStopping.PNG\"  width=\"50px\" height=\"50px\"  /></a></label>";
                else if (DataTableUtils.toString(row["狀態"]) != "ERROR" && DataTableUtils.toString(row["狀態"]) != "")
                    value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/Light_Stopping.PNG\"  width=\"50px\" height=\"50px\"  /></a></label>";
                else
                    value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/Light_Running.png\"  width=\"50px\" height=\"50px\"  /></a></label>";
            }
            else if (field_name == "開工時間")
                value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \">{HtmlUtil.StrToDate(row[field_name].ToString()):yyyy/MM/dd HH:mm:ss}</label>";
            else if (field_name == "進度")
                value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \">{DataTableUtils.toDouble(row[field_name]):0}%</label>";
            else
                value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \">{row[field_name]}</label>";
            return value == "" ? "" : $"<td align=\"center\" style=\"vertical-align:middle; color: black; \">{value}</td>";
        }
        //回饋良品與不良品  當下總數
        private double GetQtySum(DataRow row, bool good)
        {
            //取得工藝編號 0607 juiedit 
            //string sqlcmd = $"select * from workorder_information where order_status = '入站' and mach_name = '{row["設備代號"]}' and product_number = '{row["品號"]}' and processType='1' and orderno='{row["工序號碼"].ToString()}'";
            string sqlcmd = $"SELECT workorder_information.*, isnull(a.good_qty, 0) report_good, isnull(a.bad_qty, 0) report_bad,	   Isnull((((SELECT Sum(Cast(a.report_qty AS INT))FROM   (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM   record_worktime WHERE  record_worktime.manu_id =workorder_information.manu_id AND record_worktime.mach_name =workorder_information.mach_name AND record_worktime.type_mode =workorder_information.type_mode) a WHERE  a.workman_status = '中途報工' and a.qty_status='良品'))), 0)  當下良品,	   Isnull((((SELECT Sum(Cast(a.report_qty AS INT))FROM   (SELECT DISTINCT report_qty,now_time,workman_status,qty_status FROM   record_worktime WHERE  record_worktime.manu_id =workorder_information.manu_id AND record_worktime.mach_name =workorder_information.mach_name AND record_worktime.type_mode =workorder_information.type_mode) a WHERE  a.workman_status = '中途報工' and a.qty_status='不良品'))), 0)  當下不良品, (SELECT mach_show_name FROM machine_info WHERE machine_info.mach_name = workorder_information.mach_name) mach_show_name  , (SELECT is_collect   FROM machine_info   WHERE machine_info.mach_name = workorder_information.mach_name) is_collect FROM workorder_information  LEFT JOIN   (SELECT TOP(1) * FROM workorder_information WHERE order_status = '出站' ORDER BY now_time DESC ) a ON a.mach_name = workorder_information.mach_name AND a.manu_id = workorder_information.manu_id AND a.product_number = workorder_information.product_number AND a.type_mode = workorder_information.type_mode AND a.now_time < workorder_information.now_time WHERE  workorder_information.mach_name = '{row["設備代號"]}' and workorder_information.order_status ='入站' AND workorder_information.type_mode = '進站報工'   ORDER BY workorder_information.delivery ASC";
            DataTable dt = cls_db.GetDataTable(sqlcmd);
            double Sum = 0;
            if (HtmlUtil.Check_DataTable(dt))
            {
                if (good)
                    Sum = dt.AsEnumerable().Select(s => DataTableUtils.toDouble(s.Field<int>("當下良品"))).Sum();
                else
                    Sum = dt.AsEnumerable().Select(s => DataTableUtils.toDouble(s.Field<int>("當下不良品"))).Sum();
            }
            return Sum;


        }
        //顯示訊息
        private void showMsg(string msg)
        {
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('{msg}');location.href='Enter_ReportView.aspx';</script>");
        }
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
    }
}