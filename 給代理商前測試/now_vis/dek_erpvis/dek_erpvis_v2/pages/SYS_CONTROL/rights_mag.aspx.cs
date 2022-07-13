using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace dek_erpvis_v2.pages.sys_control
{
    public partial class rights_mag : System.Web.UI.Page
    {
        DataTable dt_department = new DataTable();
        public string color = "";
        public string th = "";
        public string tr = "";
        public string title_text = "";
        public string type = "";
        public string type_code = "";
        public string title = "";
        public string search_condi = "";
        public string safty_text = "";
        public string min_text = "";
        public string chart_card_text = "";
        string acc = "";
        string adm = "";
        clsDB_Server clsDB_sw = new clsDB_Server(myclass.GetConnByDekVisErp);
        myclass myclass = new myclass();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                adm = myclass.check_user_power(acc);
                if (adm == "Y")
                    GotoCenn();
                else
                    Response.Write("<script>alert('您無此權限進入!');location.href='../index.aspx';</script>");

                Label_code.Text = new Random().Next(9999).ToString();
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        private void GotoCenn()
        {
            clsDB_sw.dbOpen(myclass.GetConnByDekVisErp);
            if (clsDB_sw.IsConnected)
                load_page_data();
            else
            {
                Response.Write($"<script language='javascript'>alert('伺服器回應 : 無法載入資料，{clsDB_sw.ErrorMessage} 請聯絡德科人員或檢查您的網路連線。');</script>");
                無資料處理();
            }
        }
        private void 無資料處理()
        {
            title_text = "'沒有資料載入'";
            th = "<th class='center'>沒有資料載入</th>";
            tr = "<tr class='even gradeX'> <td class='center'> no data </td></tr>";
        }
        private void load_page_data()
        {

            set_dropdownlist();
            set_department();
            set_table_title();
            clsDB_sw.dbOpen(myclass.GetConnByDekVisErp);
        }
        //設定人名至下拉選單(權限複製用)
        private void set_dropdownlist()
        {
            clsDB_sw.dbOpen(myclass.GetConnByDekVisErp);
            string sqlcmd = "select USER_ACC,USER_NAME from users where STATUS ='ON'";
            DataTable dt = clsDB_sw.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt) && DropDownList_Source.Items.Count == 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ListItem listItem = new ListItem(DataTableUtils.toString(row["USER_NAME"]), DataTableUtils.toString(row["USER_ACC"]));
                    DropDownList_NoReset.Items.Add(listItem);
                }
            }

        }
        //設定部門(新增使用者用)
        private void set_department()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataTable dt = DataTableUtils.GetDataTable("SELECT distinct DPM_NAME,DPM_NAME2 FROM DEPARTMENT where DPM_NAME2 != '系統'");
            DataRow dr_dpOpen;
            string SQLcmd = "";
            if (HtmlUtil.Check_DataTable(dt))
            {
                ListItem list = new ListItem("-請選擇-", "");
                if (!DropDownList_depart.Items.Contains(list))
                    DropDownList_depart.Items.Add(list);
                foreach (DataRow row in dt.Rows)
                {
                    if (WebUtils.GetAppSettings("Dep_defin") == "0")
                    {
                        //原始四個部門
                        SQLcmd = $"select * from WEB_PAGES where WEB_DPM='{row["dpm_name"]}' and web_open='Y' ";
                        dr_dpOpen = DataTableUtils.DataTable_GetDataRow(SQLcmd);
                        if (dr_dpOpen != null)
                        {
                            addDepListItem(row);
                            //list = new ListItem(DataTableUtils.toString(row["DPM_NAME2"]), DataTableUtils.toString(row["DPM_NAME"]));
                            //if (!DropDownList_depart.Items.Contains(list))
                            //    DropDownList_depart.Items.Add(list);
                        }
                    }
                    else
                    {
                        //自訂義部門
                        addDepListItem(row);
                    }
                }
            }
        }
        private void addDepListItem(DataRow row)
        {
            ListItem list = new ListItem(DataTableUtils.toString(row["DPM_NAME2"]), DataTableUtils.toString(row["DPM_NAME"]));
            if (!DropDownList_depart.Items.Contains(list))
                DropDownList_depart.Items.Add(list);
        }
        private void set_table_title()
        {
            th = "<th>用戶名稱</th>\n<th>所屬部門</th>\n<th>用戶帳號</th>\n<th>狀態</th></th><th>建立時間</th><th>權限</th><th>操作</th><th>啟用/停用 該帳號</th><th>權限複製</th><th>所屬群組</th><th>群組</th>\n";
        }
        public string set_table_content()
        {
            List<string> powerlist = new List<string>();
            string PowerDisable = "";//disabled
            powerlist.Add("一般使用者");
            //LITZ功能
            if (WebUtils.GetAppSettings("Power_defin") == "1")
            {
                powerlist.Add("生管人員");
                powerlist.Add("現場人員");
                powerlist.Add("現場主管");
            }
            powerlist.Add("管理者");
            string groupname = "";
            clsDB_sw.dbOpen(myclass.GetConnByDekVisErp);
            if (clsDB_sw.IsConnected && adm == "Y")
            {
                //找出目前存在於資料庫的所有群組
                string sql = "select distinct dpm from page_tree";
                DataTable dt_group = clsDB_sw.GetDataTable(sql);

                if (HtmlUtil.Check_DataTable(dt_group) && CheckBoxList_Group.Items.Count == 0)
                {
                    foreach (DataRow row in dt_group.Rows)
                        CheckBoxList_Group.Items.Add(DataTableUtils.toString(row["dpm"]));
                }

                //找出目前所有使用者的群組

                sql = "select  * from user_group where is_open='1'";
                DataTable dt_groupuser = clsDB_sw.GetDataTable(sql);


                string sqlcmd = "SELECT USER_ACC,USER_NAME,DEPARTMENT.DPM_NAME2,STATUS,'' loginStatus,ADD_TIME,USER_ID,Power,ADM,Can_Close FROM USERS left join DEPARTMENT on USERS.USER_DPM = DEPARTMENT.DPM_NAME";
                DataTable dt = clsDB_sw.DataTable_GetTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        //開發者權限直接不顯示 0530 juiedit
                        PowerDisable = "";
                        if (WebUtils.GetAppSettings("Show_PowerUser") != "1" && row["USER_ACC"].ToString() == "admindek")
                            continue;
                        string droplist = "";
                        string identify = "";
                        string url = myclass.Base64Encode("user_acc=" + DataTableUtils.toString(row["USER_ACC"]) + ",user_name=" + DataTableUtils.toString(row["USER_NAME"]) + "");
                        tr += "<tr>\n";
                        tr += $"<td><u><a href='rights_mag_details.aspx?password_={url}' target='_blank'>{DataTableUtils.toString(row["USER_NAME"])}</u></td>\n";
                        tr += $"<td>{DataTableUtils.toString(row["DPM_NAME2"])}</td>\n";
                        tr += $"<td>{DataTableUtils.toString(row["USER_ACC"])}</td>\n";
                        tr += $"<td>{DataTableUtils.toString(row["STATUS"])}</td>\n";
                        tr += $"<td>{DataTableUtils.toString(row["ADD_TIME"])}</td>\n";

                        for (int i = 0; i < powerlist.Count; i++)
                        {
                            if (DataTableUtils.toString(row["ADM"]) == "Y" && powerlist[i] == "管理者")
                                droplist += $"<option selected=\"selected\" value={powerlist[i]}>{powerlist[i]}</option>";
                            else if (DataTableUtils.toString(row["power"]) == "Y" && powerlist[i] == "生管人員")
                                droplist += $"<option selected=\"selected\" value={powerlist[i]}>{powerlist[i]}</option>";
                            else if (DataTableUtils.toString(row["Can_Close"]) == "A" && powerlist[i] == "現場人員")
                                droplist += $"<option selected=\"selected\" value={powerlist[i]}>{powerlist[i]}</option>";
                            else if (DataTableUtils.toString(row["Can_Close"]) == "Y" && powerlist[i] == "現場主管")
                                droplist += $"<option selected=\"selected\" value={powerlist[i]}>{powerlist[i]}</option>";
                            else
                                droplist += $"<option value={powerlist[i]}>{powerlist[i]}</option>";
                            //==============
                            if (DataTableUtils.toString(row["ADM"]) == "Y" && DataTableUtils.toString(row["power"]) == "Y" && DataTableUtils.toString(row["Can_Close"]) == "Y")
                                identify = "管理者";
                            else if (droplist.IndexOf("selected") != -1 && identify == "")
                                identify = powerlist[i];
                            else if (i == powerlist.Count - 1 && identify == "")
                                identify = "一般使用者";
                        }


                        //設定使用者權限
                        if (acc == DataTableUtils.toString(row["USER_ACC"]))
                            PowerDisable = "disabled";
                        tr += $"<td><span style=\"display:none\">{identify}</span>" +
                              $"<select name=\"ctl00$ContentPlaceHolder1$DropDownList_{DataTableUtils.toString(row["USER_ID"])}\" onchange=func(\"{DataTableUtils.toString(row["USER_ID"])}\")  id =\"ContentPlaceHolder1_DropDownList_{ DataTableUtils.toString(row["USER_ID"])}\" {PowerDisable}>";
                        tr += droplist;
                        tr += "</select></td>";

                        //刪除
                        tr += $"<td><button id='{DataTableUtils.toString(row["USER_NAME"])}' type=\"button\" class=\"btn btn-danger btn-s dt-delete\" onclick=\"button_delete('{myclass.Base64Encode($"{adm},{acc},{DataTableUtils.toString(row["USER_ACC"])}")}')\"><span class =\"glyphicon glyphicon-remove\" aria-hidden='true'></span> 刪除</button></ td>\n";

                        //啟用/停用
                        if (DataTableUtils.toString(row["STATUS"]) == "ON")
                            tr += $"<td><button type=\"button\" class=\"btn btn-danger\" onclick=isfreezeacc('{DataTableUtils.toString(row["USER_ID"])}','OFF')>停用</button> <button type=\"button\" class=\"btn btn-warning\" onclick=ForceLogdout('{DataTableUtils.toString(row["USER_ID"])}','OFF')>強制登出</button></td>";
                        else
                            tr += $"<td><button type=\"button\" class=\"btn btn-success\" onclick=isfreezeacc('{DataTableUtils.toString(row["USER_ID"])}','ON')>啟用</button><button type=\"button\" class=\"btn btn-warning\" onclick=ForceLogdout('{DataTableUtils.toString(row["USER_ID"])}','OFF')>強制登出</button></td>";//
                        //
                        tr += $"<td><button type=\"button\" class=\"btn btn-info\" data-toggle = \"modal\" data-target = \"#exampleModal\" onclick=set_userinformation('{DataTableUtils.toString(row["USER_ID"])}','{DataTableUtils.toString(row["USER_NAME"])}','{DataTableUtils.toString(row["USER_ACC"])}') >權限複製</button></td>";

                        groupname = "";
                        //設定群組
                        sqlcmd = $"user_acc='{row["USER_ACC"]}'";
                        DataRow[] rows = dt_groupuser.Select(sqlcmd);
                        if (row != null && rows.Length > 0)
                        {
                            for (int x = 0; x < rows.Length; x++)
                                groupname += DataTableUtils.toString(rows[x]["group_name"]) + "^";
                        }

                        //所屬群組
                        tr += $"<td>{Search_group(DataTableUtils.toString(row["USER_ACC"]))}</td>";
                        tr += $"<td><button type=\"button\" class=\"btn btn-info\" data-toggle = \"modal\" data-target = \"#GroupModal\" onclick=Set_Group(\"{groupname}\",\"{DataTableUtils.toString(row["USER_ACC"])}\") >設定群組</button></td>";
                        tr += "</tr>\n";
                    }
                    Response.Write(tr);
                    Response.Flush();
                    Response.Clear();
                    tr = "";
                }
                dt.Dispose();
            }
            else
                無資料處理();
            return "";
        }
        //寫入帳號變更log
        private bool permission_log(string acc, string user_id, string permissionNum)
        {
            string sqlcmd = "select * from permission_log";
            DataTable dt = clsDB_sw.DataTable_GetTable(sqlcmd, 0, 1);
            DataRow dr_log;//= dt.NewRow();
            DataTable dt_user;
            bool ok = false;
            if (dt != null)
            {
                sqlcmd = $"select * from users where user_id='{user_id}'";
                dt_user = clsDB_sw.DataTable_GetRow(sqlcmd);
                dr_log = dt.NewRow();
                dr_log["acc"] = dt_user.Rows[0]["user_acc"].ToString();
                dr_log["changer"] = acc;
                dr_log["changetime"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                dr_log["permission"] = permissionNum;
                ok = clsDB_sw.Insert_DataRow("permission_log", dr_log);
            }
            return ok;
        }
        //找尋群組
        private string Search_group(string acc)
        {

            string sqlcmd = $"select * from user_group where is_open='1' and user_acc='{acc}'";
            DataTable dt = clsDB_sw.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                string group = "";
                foreach (DataRow row in dt.Rows)
                    group += $"{row["group_name"]}<br />";
                return group;
            }
            else
                return "";
        }
        //啟用 || 停用
        protected void Button_Act_Click(object sender, EventArgs e)
        {

            string sqlcmd = $"select * from users where USER_ID = '{TextBox_ID.Text}'";
            DataTable dt = clsDB_sw.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataRow row = dt.NewRow();
                row["USER_ID"] = dt.Rows[0]["USER_ID"];
                row["STATUS"] = TextBox_Act.Text;

                if (clsDB_sw.Update_DataRow("users", $"USER_ID = '{TextBox_ID.Text}'", row))
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改成功');location.href='rights_mag.aspx';</script>");
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");

        }
        //複製使用者權限
        protected void Button_copy_Click(object sender, EventArgs e)
        {
            string target_acc = HtmlUtil.Search_acc_Column(TextBox_TargetName.Text, "USER_ACC");
            string copy_acc = HtmlUtil.Search_acc_Column(TextBox_Source.Text, "USER_ACC");
            string sqlcmd = "";
            /*---------------------------------------以下金額複製的部分------------------------------------------------*/
            if (CheckBox_HighPower.Checked)
            {

                clsDB_sw.Delete_Record("system_pmr", $"USER_ACC = '{target_acc}'");

                sqlcmd = $"select * from system_pmr where USER_ACC = '{copy_acc}'";
                DataTable dt_power = clsDB_sw.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt_power))
                {
                    foreach (DataRow row in dt_power.Rows)
                    {
                        DataRow rew = dt_power.NewRow();
                        rew["USER_ACC"] = target_acc;
                        rew["FUNC_DES"] = row["FUNC_DES"];
                        rew["FUNC_YN"] = row["FUNC_YN"];

                        clsDB_sw.Insert_DataRow("system_pmr", rew);
                    }
                }
            }

            /*---------------------------------------以下權限複製的部分------------------------------------------------*/
            string condition = "";
            //先找出拷貝目標

            sqlcmd = $"select WB_URL from web_user left join users on users.USER_ACC = web_user.USER_ACC where USER_NAME = '{TextBox_TargetName.Text}'";
            DataTable dt_source = clsDB_sw.GetDataTable(sqlcmd);

            //保留原本的權限
            if (CheckBox_SaveOriginal.Checked)
            {
                //已經有的就不要複製了
                if (HtmlUtil.Check_DataTable(dt_source))
                {
                    bool ok = false;
                    foreach (DataRow row in dt_source.Rows)
                    {
                        if (!ok)
                            condition = $" WB_URL <> '{DataTableUtils.toString(row["WB_URL"])}' ";
                        else
                            condition += $" and  WB_URL <> '{DataTableUtils.toString(row["WB_URL"])}' ";
                        ok = true;
                    }
                    if (condition != "")
                        condition = $" and ( {condition} ) ";
                }
            }

            //找出拷貝來源

            sqlcmd = $"select WB_URL,VIEW_NY from web_user left join users on users.USER_ACC = web_user.USER_ACC where USER_NAME = '{TextBox_Source.Text}' {condition} ";
            DataTable dt_target = clsDB_sw.GetDataTable(sqlcmd);

            //拷貝目標不一定有資料，但是拷貝來源需有資料
            if (dt_source != null && HtmlUtil.Check_DataTable(dt_target))
            {
                int i = 0;

                //不保留的情況下 刪除
                if (!CheckBox_SaveOriginal.Checked)
                {

                    bool ok = DataTableUtils.Delete_Record("show_page", $"account='{target_acc}'");
                    ok = clsDB_sw.Delete_Record("web_user", $"USER_ACC='{target_acc}'");
                }

                DataTable dt_web = clsDB_sw.GetDataTable("select * from web_user");

                DataTable dt_show = clsDB_sw.GetDataTable("select * from show_page");

                foreach (DataRow row in dt_target.Rows)
                {
                    if (dt_web != null)
                    {
                        DataRow rew = dt_web.NewRow();
                        rew["WB_URL"] = row["WB_URL"];
                        rew["USER_ACC"] = target_acc;
                        rew["VIEW_NY"] = row["VIEW_NY"];
                        if (clsDB_sw.Insert_DataRow("web_user", rew))
                            i++;
                    }
                    if (dt_show != null)
                    {
                        DataRow rsw = dt_show.NewRow();
                        rsw["URL"] = row["WB_URL"];
                        rsw["account"] = target_acc;
                        rsw["Allow"] = row["VIEW_NY"];
                        clsDB_sw.Insert_DataRow("show_page", rsw);
                    }
                }
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('已成功複製{i}筆');location.href='rights_mag.aspx';</script>");
            }
            else if (dt_source != null && dt_target != null)
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('複製對象以開通權限');location.href='rights_mag.aspx';</script>");
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('複製失敗');location.href='rights_mag.aspx';</script>");
        }
        //儲存使用者權限
        protected void Button_change_Click(object sender, EventArgs e)
        {
            int permissionnum = 0;
            List<string> list = new List<string>(TextBox_textTemp.Text.Split('^'));
            if (list.Count == 2)
            {

                string sqlcmd = $"select * from users where USER_ID = '{list[0]}'";
                DataTable dt = clsDB_sw.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    DataRow row = dt.NewRow();
                    if (list[1] == "管理者")
                    {
                        row["ADM"] = "Y";
                        row["Power"] = "Y";
                        row["Can_Close"] = "Y";
                        permissionnum = 1;
                    }
                    else if (list[1] == "生管人員")
                    {
                        row["ADM"] = "N";
                        row["Power"] = "Y";
                        row["Can_Close"] = "N";
                        permissionnum = 2;
                    }
                    else if (list[1] == "現場人員")
                    {
                        row["ADM"] = "N";
                        row["Power"] = "N";
                        row["Can_Close"] = "A";
                        permissionnum = 3;
                    }
                    else if (list[1] == "現場主管")
                    {
                        row["ADM"] = "N";
                        row["Power"] = "N";
                        row["Can_Close"] = "Y";
                        permissionnum = 4;
                    }

                    else if (list[1] == "一般使用者")
                    {
                        row["ADM"] = "N";
                        row["Power"] = "N";
                        row["Can_Close"] = "N";
                        permissionnum = 5;
                    }

                    if (clsDB_sw.Update_DataRow("users", $"USER_ID = '{list[0]}'", row))
                    {
                        //permission_log 0602 juiedit
                        permission_log(acc, list[0], permissionnum.ToString());
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改成功');location.href='rights_mag.aspx';</script>");
                    }
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");
        }
        //儲存使用者群組
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            //先清除該名使用者的群組資料

            clsDB_sw.Delete_Record("user_group", $"user_acc='{TextBox_temp.Text}'");

            //把選到的產線寫入資料庫
            List<string> list = new List<string>(TextBox_group.Text.Split('^'));

            string sqlcmd = "select * from user_group order by id desc";
            DataTable dt = clsDB_sw.GetDataTable(sqlcmd);
            int id = 1;
            if (dt != null)
            {
                if (HtmlUtil.Check_DataTable(dt))
                    id = DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[0]["ID"])) + 1;

                DataTable dr = dt.Clone();
                for (int i = 0; i < list.Count - 1; i++)
                {
                    DataRow row = dr.NewRow();
                    row["ID"] = id;
                    row["user_acc"] = TextBox_temp.Text;
                    row["group_name"] = list[i];
                    row["is_open"] = "1";
                    dr.Rows.Add(row);
                    id++;
                }

                if (clsDB_sw.Insert_TableRows("user_group", dr) == dr.Rows.Count)
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增成功');location.href='rights_mag.aspx';</script>");
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增失敗');location.href='rights_mag.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增失敗');location.href='rights_mag.aspx';</script>");
        }
        //批次新增使用者->Excel匯入
        protected void Button_AddUsers_Click(object sender, EventArgs e)
        {
            //ASP無法對檔案路徑做讀取，所以先儲存到別的地方，之後再讀取那個地方
            string filename = FileUpload_File.PostedFile.FileName;
            HtmlUtil.FileUpload_Name(FileUpload_File, "Backup_Error_Image", true);


            string sqlcmd = "select * from department where DPM_GROUP is null";
            dt_department = clsDB_sw.GetDataTable(sqlcmd);

            //讀取EXCEL
            DataTable dt = new DataTable();
            int _Column_count = 0;
            int _Rows_count = 0;
            int _excel_statrRos = 1;
            int _excel_KeyRow = 1;
            ExcelIO excel = new ExcelIO(WebConfigurationManager.AppSettings["disk"] + ":\\Backup_Error_Image\\" + filename);
            string _Name = "";
            while (excel.GetValue(_excel_statrRos, _Column_count + 1) != "")
            {
                _Name = excel.GetValue(_excel_statrRos, _Column_count);
                _Column_count++;
            }
            while (excel.GetValue(_Rows_count + 1, _excel_KeyRow) != "")
                _Rows_count++;

            int count = 1;

            //mysql
            if (myclass.NowdbType == "MySQL")
                sqlcmd = "select * from users order by cast(substring(USER_ID,2,length(USER_ID)) as signed) desc";
            //mssql
            else
                sqlcmd = "SELECT * FROM USERS order by cast(SUBSTRING(user_id,2,LEN(user_id)-1) as int)  desc";
            DataTable dts = clsDB_sw.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dts))
                count = DataTableUtils.toInt(DataTableUtils.toString(dts.Rows[0]["USER_ID"]).Trim('U')) + 1;

            DataRow dr = dt.NewRow();
            for (int i = 1; i < _Rows_count; i++)
            {
                dr = dt.NewRow();
                for (int j = 1; j <= _Column_count; j++)
                {
                    if (j == 1 && i == 1)
                        dt.Columns.Add("USER_ID");
                    if (i == 1)
                        dt.Columns.Add(mapping_column(excel.GetValue(1, j).ToString()));
                    if (excel.GetValue(i + 1, 1).ToString() != "範例1")
                    {
                        if (j == 1)
                            dr["USER_ID"] = "U" + count;
                        dr[mapping_column(excel.GetValue(1, j).ToString())] = check_value(excel.GetValue(i + 1, j).ToString(), mapping_column(excel.GetValue(1, j).ToString()));
                    }
                }
                if (excel.GetValue(i + 1, 1).ToString() != "範例1")
                {
                    count++;
                    dt.Rows.Add(dr);
                }
            }

            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable dt_Power;
                //mysql
                if (myclass.NowdbType == "MySQL")
                    dt_Power = DataTableUtils.GetDataTable("select * from user_group limit 0");
                //mssql
                else
                    dt_Power = clsDB_sw.GetDataTable("select TOP(0) * from user_group");

                //加上後面的資料
                dt.Columns.Add("STATUS");
                dt.Columns.Add("ADD_TIME");
                dt.Columns.Add("ADM");
                dt.Columns.Add("Power");
                dt.Columns.Add("Can_Close");
                dt.Columns.Add("Last_Model");

                foreach (DataRow row in dt.Rows)
                {
                    row["STATUS"] = "ON";
                    row["ADD_TIME"] = DateTime.Now.ToString("yyyy/MM/dd tt HH:mm:ss");
                    row["Last_Model"] = "Hor";

                    switch (DataTableUtils.toString(row["使用者權限"]))
                    {
                        case "管理者":
                            row["ADM"] = "Y";
                            row["Power"] = "Y";
                            row["Can_Close"] = "Y";
                            break;

                        case "生管人員":
                            row["ADM"] = "N";
                            row["Power"] = "Y";
                            row["Can_Close"] = "N";
                            break;

                        case "現場人員":
                            row["ADM"] = "N";
                            row["Power"] = "N";
                            row["Can_Close"] = "A";
                            break;
                        case "現場主管":

                            row["ADM"] = "N";
                            row["Power"] = "N";
                            row["Can_Close"] = "Y";
                            break;
                        case "一般使用者":
                            row["ADM"] = "N";
                            row["Power"] = "N";
                            row["Can_Close"] = "N";
                            break;
                    }


                    if (dt_Power != null)
                    {
                        DataRow rows = dt_Power.NewRow();

                        rows["user_acc"] = row["USER_ACC"];
                        rows["group_name"] = row["使用者群組"];
                        rows["is_open"] = "1";
                        dt_Power.Rows.Add(rows);
                    }


                }


                dt.Columns.Remove("使用者權限");
                dt.Columns.Remove("使用者群組");
                //新增權限
                clsDB_sw.Insert_TableRows("user_group", dt_Power);

                if (dt.Rows.Count == clsDB_sw.Insert_TableRows("users", dt))
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('批次新增成功');location.href='rights_mag.aspx';</script>");
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('批次新增失敗');location.href='rights_mag.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('批次新增失敗');location.href='rights_mag.aspx';</script>");
        }

        //欄位對應
        private string mapping_column(string column)
        {
            switch (column)
            {
                case "使用者姓名":
                    return "USER_NAME";
                case "使用者帳號":
                    return "USER_ACC";
                case "使用者密碼":
                    return "USER_PWD";
                case "使用者部門":
                    return "USER_DPM";
                case "使用者信箱":
                    return "USER_EMAIL";
                case "使用者生日":
                    return "USER_BIRTHDAY";
                case "使用者電話":
                    return "USER_NUM";
            }
            return column;
        }

        //確認是否有需要另外處理的數值
        private string check_value(string value, string column)
        {
            switch (column)
            {
                case "USER_PWD":
                    return dekSecure.dekHashCode.SHA256(value);
                case "USER_DPM":
                    if (HtmlUtil.Check_DataTable(dt_department))
                    {
                        DataRow[] row = dt_department.Select($"DPM_NAME2='{value}'");
                        if (row != null && row.Length > 0)
                            value = DataTableUtils.toString(row[0]["DPM_NAME"]);
                    }
                    return value;
            }
            return value;
        }
        //新增使用者(單一)
        protected void Button_Add_Click(object sender, EventArgs e)
        {
            string acc_ = DataTableUtils.toString(TextBox_Acc.Text.Trim());
            string pwd_ = DataTableUtils.toString(TextBox_Pwd1.Text.Trim());
            bool error = true;

            if (acc_ != "" && pwd_ != "")
            {
                Regex regex = new Regex("[-']");
                error = regex.IsMatch(acc_) || regex.IsMatch(pwd_);
            }
            if (error != true)
            {

                string sqlcmd = $"SELECT * FROM USERS where USER_ID = '{TextBox_Acc.Text}'";
                DataTable dt = clsDB_sw.GetDataTable(sqlcmd);
                int ds_max = 1;
                int x = 0;
                string initColorSet = "";
                DataTable ds;

                //mysql
                if (myclass.NowdbType == "MySQL")
                    ds = clsDB_sw.GetDataTable("SELECT * FROM USERS order by CAST(substring(USER_ID,2)  AS double)  desc");
                //mssql
                else
                    ds = clsDB_sw.GetDataTable("SELECT * FROM USERS order by cast(SUBSTRING(user_id,2,LEN(user_id)-1) as int)  desc");
                if (HtmlUtil.Check_DataTable(ds))
                {
                    x = ds.Rows.Count;
                    ds_max = DataTableUtils.toInt(ds.Rows[0]["USER_ID"].ToString().Trim('U')) + 1;
                }

                if (dt != null && dt.Rows.Count <= 0)
                {
                    DataRow row = dt.NewRow();
                    if (x == 0)
                        row["USER_ID"] = "U000000";
                    else if (x > 0)
                        row["USER_ID"] = $"U{ds_max}";

                    row["USER_ACC"] = TextBox_Acc.Text;
                    row["USER_NAME"] = TextBox_Name.Text;
                    row["USER_PWD"] = dekSecure.dekHashCode.SHA256(TextBox_Pwd1.Text);
                    row["USER_NUM"] = TextBox_phone.Text;
                    row["USER_DPM"] = DataTableUtils.toString(DropDownList_depart.SelectedValue);
                    row["USER_BIRTHDAY"] = TextBox_birth.Text.Replace("-", "");
                    row["USER_EMAIL"] = TextBox_email.Text;
                    row["STATUS"] = "ON";
                    if (x == 0)
                    {
                        row["ADM"] = "Y";
                        row["Can_Close"] = "Y";
                    }

                    else if (x > 0)
                        row["ADM"] = "N";
                    row["ADD_TIME"] = DateTime.Now.ToString("yyyy/MM/dd tt HH:mm:ss");
                    row["Last_Model"] = "Hor";


                    if (clsDB_sw.Insert_DataRow("USERS", row))
                    {
                        //顏色初始設定
                        initColorSet = WebUtils.GetAppSettings("default_color");
                        if (initColorSet == "1")
                        {
                            HtmlUtil.initColorSet2DB(TextBox_Acc.Text);
                        }
                        //
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增成功');location.href='rights_mag.aspx';</script>");
                    }
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('此帳號已經有人使用! 請輸入其它帳號');location.href='rights_mag.aspx';</script>");

            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('帳號或密碼包含特殊符號，請重新輸入');location.href='rights_mag.aspx';</script>");

        }
        //0520-juiEdit
        protected void Button_FLogout_Click(object sender, EventArgs e)
        {
            string sqlcmd = $"select * from users where USER_ID = '{TextBox_ID.Text}'";
            //string sqlcmd = $"SELECT * FROM USERS left join SYSTEM_USERSLOGIN_log on USERS.USER_ACC = SYSTEM_USERSLOGIN_log.USER_ACC where USER_ID = '{TextBox_ID.Text}' and LOGIN_TIME > {DateTime.Now.AddDays(7).ToString("yyyyMMddHHmmss")}order by LOGIN_TIME desc ";
            DataTable dt = clsDB_sw.DataTable_GetTable(sqlcmd);

            DataTable dt_log;// = clsDB_sw.DataTable_GetTable(sqlcmd);
            DataRow row;//= dt_log.NewRow();
            Random Rnd = new Random();
            if (HtmlUtil.Check_DataTable(dt))
            {
                sqlcmd = $"select * from SYSTEM_USERSLOGIN_log where User_ACC = '{dt.Rows[0]["User_ACC"]}' and LOGIN_TIME > {DateTime.Now.AddDays(-7).ToString("yyyyMMddHHmmss")} order by LOGIN_TIME desc";
                dt_log = clsDB_sw.DataTable_GetTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt_log))
                {
                    //取最近的第一筆 看一下狀態  
                    row = dt_log.AsEnumerable().FirstOrDefault();
                    if (row != null)
                    {
                        row["SU_ID"] = DataTableUtils.toString("SU" + Rnd.Next(99999));
                        row["STATUS"] = false;
                        row["LOGIN_TIME"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                        //row["SU_ID"] = DataTableUtils.toString("SU" + Rnd.Next(99999));
                        //row["USER_ACC"] = dt.Rows[0]["USER_ACC"];
                        //row["LOGIN_TIME"] = dt.Rows[0]["LOGIN_TIME"];
                        //row["CLIENT_IP"] = dt.Rows[0]["CLIENT_IP"];
                        //row["STATUS"] = false;
                        //row["FROM_"] = dt.Rows[0]["FROM_"];
                        if (clsDB_sw.Insert_DataRow("SYSTEM_USERSLOGIN_log", row))
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('已強制登出');location.href='rights_mag.aspx';</script>");
                        else
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");
                    }
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('該員一周內無登入紀錄!');location.href='rights_mag.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改失敗');location.href='rights_mag.aspx';</script>");

        }
    }
}