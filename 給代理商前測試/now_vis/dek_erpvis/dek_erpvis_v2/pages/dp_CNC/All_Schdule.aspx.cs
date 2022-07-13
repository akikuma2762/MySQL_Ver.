using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
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

namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class All_Schdule : System.Web.UI.Page
    {
        //引用 OR 參數
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
                if (myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc) || HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
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

        //欄位移動事件儲存
        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        //選取顯示欄位與廠區群組
        protected void Button_Check_Click(object sender, EventArgs e)
        {

            DataTable dt = CNC.Save_Cloumn(acc, "All_Schdule");
            if (dt != null)
            {
                //先刪除裡面的資料
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                DataTableUtils.Delete_Record("show_column", $"Account='{acc}' and use_page ='All_Schdule' ");

                DataTable dt_max = CNC.Get_IDMax("show_column", "ID");
                int max = HtmlUtil.Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["ID"].ToString()) + 1 : 1;

                DataTable dt_clone = dt.Clone();

                for (int i = 0; i < CheckBoxList_Columns.Items.Count; i++)
                {
                    DataRow row = dt_clone.NewRow();
                    row["id"] = max + i;
                    row["Column_Name"] = CheckBoxList_Columns.Items[i].Text;
                    row["Account"] = acc;
                    row["Allow"] = CheckBoxList_Columns.Items[i].Selected ? "True" : "False";
                    row["use_page"] = "All_Schdule";
                    dt_clone.Rows.Add(row);
                }
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Insert_TableRows("show_column", dt_clone) == dt_clone.Rows.Count)
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

        //須執行之副程式
        private void MainProcess()
        {
            Set_Factory();
            Set_Checkbox();
            Get_MonthTotal();
            Set_Table();
        }

        //取得目前所有的狀態為未出站的資料
        private void Get_MonthTotal()
        {
            if (machlist.Count > 0)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                string sqlcmd = "select * from machine_info";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
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
            dt_monthtotal = CNC.All_Schdule("cnc", machlist);
            changeMachAreaGroup();
            changeFinishQty();
        }
        //將區域名稱換成 群組名稱
        private void changeMachAreaGroup()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = "select * from machine_info";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            sqlcmd = "select * from mach_group";
            DataTable dt_gp = DataTableUtils.GetDataTable(sqlcmd);
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
            DataTable dt;
            dt_monthtotal.Columns["已生產量"].ReadOnly = false;
            dt_monthtotal.Columns["未生產量"].ReadOnly = false;
            dt_monthtotal.Columns["進度"].ReadOnly = false;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            for (int i = 0; i < dt_monthtotal.Rows.Count; i++)
            {
                sqlcmd = $"select product_count_day  as 已完成數量 from workorder_information where order_Status='出站' and mach_name='{dt_monthtotal.Rows[i]["設備代號"]}' and manu_id='{dt_monthtotal.Rows[i]["工單報工"]}' and product_number='{dt_monthtotal.Rows[i]["品號"]}' and orderno='{dt_monthtotal.Rows[i]["工序號碼"]}' order by last_updatetime desc";
                dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    var Count = dt.AsEnumerable().FirstOrDefault();
                    dt_monthtotal.Rows[i]["已生產量"] = Count["已完成數量"].ToString();
                    dt_monthtotal.Rows[i]["未生產量"] = (DataTableUtils.toInt(dt_monthtotal.Rows[i]["預計產量"].ToString()) - DataTableUtils.toInt(Count["已完成數量"].ToString())).ToString();
                    dt_monthtotal.Rows[i]["進度"] = DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString()) > 0 ? $"{(DataTableUtils.toInt(dt_monthtotal.Rows[i]["當下產量"].ToString()) * 100 / DataTableUtils.toInt(dt_monthtotal.Rows[i]["未生產量"].ToString())).ToString():0}" : "100";
                }
            }
        }
        //設定核取方塊
        private void Set_Checkbox()
        {
            CheckBoxList_Columns.Items.Clear();
            //找到系統預設的
            DataTable dt_System = CNC.System_columns("All_Schdule");
            //找到個人設定的
            DataTable dt_Person = CNC.Person_columns("All_Schdule", acc);

            //個人的
            if (HtmlUtil.Check_DataTable(dt_System))
            {
                ListItem listItem = new ListItem();
                //個人的
                if (HtmlUtil.Check_DataTable(dt_Person))
                {
                    foreach (DataRow row in dt_System.Rows)
                    {
                        if (DataTableUtils.toString(row["info_chinese"]) != "工單報工")
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
                }
                //系統的
                else
                {
                    foreach (DataRow row in dt_System.Rows)
                    {
                        if (DataTableUtils.toString(row["info_chinese"]) != "工單報工")
                        {
                            listItem = new ListItem(DataTableUtils.toString(row["info_chinese"]), DataTableUtils.toString(row["info_name"]));
                            listItem.Selected = true;
                            columns.Add(DataTableUtils.toString(row["info_chinese"]));
                            CheckBoxList_Columns.Items.Add(listItem);
                        }
                    }
                }

                columns.Add("");
            }

        }


        //設定廠區群組的下拉選單
        private void Set_Factory()
        {
            CNCError.Set_FactoryDropdownlist(acc, DropDownList_MachType, Request.FilePath);
        }

        //產生DataTable
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
                tr = HtmlUtil.Set_Table_Content(true, dt_monthtotal, order_list, All_Schdule_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }

        //例外處理
        private string All_Schdule_callback(DataRow row, string field_name)
        {
            string value = "";

            if (field_name == "工單狀態")
            {
                if (DataTableUtils.toString(row["狀態"]) == "ERROR")
                    value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/Light_ExStopping.PNG\"  width=\"50px\" height=\"50px\"  /></a></label>";
                else if (DataTableUtils.toString(row["狀態"]) != "ERROR" && DataTableUtils.toString(row["狀態"]) != "")
                    value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/Light_Stopping.PNG\"  width=\"50px\" height=\"50px\"  /></a></label>";
                else if (DataTableUtils.toString(row["模式"]) == "進站維護")
                    value = $"<label id=\"{row["設備代號"]}_{field_name}_{row["工單報工"]}\" style=\"font - weight:normal;margin-bottom:0px \"><a href=\"javascript:void(0)\" ><img src=\"../../assets/images/Light_Maintain.PNG\"  width=\"50px\" height=\"50px\"  /></a></label>";
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

    }
}