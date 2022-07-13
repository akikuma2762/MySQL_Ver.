using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using dek_erpvis_v2.cls;
using Support;
using System.Text.RegularExpressions;
using System.Web.Services;

namespace dek_erpvis_v2
{
    public partial class create_acc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label_code.Text = new Random().Next(9999).ToString();
            InitDropDown();
            Set_DropDownlist();
            Label1.Text = myclass.GetConnByDekVisErp + "   \n";
            Label1.Text += myclass.GetConnByDekVisCnc_inside + "   \n";
            Label1.Text += myclass.GetConnByDekVisCnc_insideMysql + "   \n";
        }

        private void InitDropDown()
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
                    //SQLcmd = $"select * from WEB_PAGES where WEB_DPM='{row["dpm_name"]}' and web_open='Y' ";
                    //dr_dpOpen = DataTableUtils.DataTable_GetDataRow(SQLcmd);
                    //if (dr_dpOpen != null)
                    //{
                    //    list = new ListItem(DataTableUtils.toString(row["DPM_NAME2"]), DataTableUtils.toString(row["DPM_NAME"]));
                    //    if (!DropDownList_depart.Items.Contains(list))
                    //        DropDownList_depart.Items.Add(list);
                    //}
                }
            }
        }
        private void addDepListItem(DataRow row)
        {
            ListItem list = new ListItem(DataTableUtils.toString(row["DPM_NAME2"]), DataTableUtils.toString(row["DPM_NAME"]));
            if (!DropDownList_depart.Items.Contains(list))
                DropDownList_depart.Items.Add(list);
        }
        protected void Button_add_Click(object sender, EventArgs e)
        {
            string acc_ = DataTableUtils.toString(TextBox_Acc.Text.Trim());
            string pwd_ = DataTableUtils.toString(TextBox_Pwd1.Text.Trim());
            bool error = true;

            if (acc_ != "" && pwd_ != "")
            {
                Regex regex = new Regex("[-']");
                error = regex.IsMatch(acc_) || regex.IsMatch(pwd_);
            }
            //
            if (error != true)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                string sqlcmd = $"SELECT * FROM USERS where USER_ID = '{TextBox_Acc.Text}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                int x = 0;
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                DataTable ds = DataTableUtils.GetDataTable("SELECT * FROM USERS");
                if (HtmlUtil.Check_DataTable(ds))
                    x = ds.Rows.Count;
                if (dt != null && dt.Rows.Count <= 0)
                {
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                    sqlcmd = $"SELECT * FROM USERS where USER_NUM = '{TextBox_phone.Text}'";
                    DataTable dr = DataTableUtils.GetDataTable(sqlcmd);
                    if (dr != null && dr.Rows.Count <= 0)
                    {
                        DataRow row = dt.NewRow();
                        if (x == 0)
                            row["USER_ID"] = "U000000";
                        else if (x > 0)
                            row["USER_ID"] = $"U{myclass.get_ran_id()}";

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
                            row["power"] = "Y";
                            row["ADM"] = "Y";
                            row["Can_Close"] = "Y";
                        }
                        else if (x > 0)
                            row["ADM"] = "N";
                        row["ADD_TIME"] = DateTime.Now.ToString("yyyy/MM/dd tt HH:mm:ss");
                        row["Last_Model"] = "Hor";
                        //row["Belong_Factory"] = DropDownList_Factory.SelectedItem.Value;
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                        if (DataTableUtils.Insert_DataRow("USERS", row))
                        {
                            //default_color set--220513-jui
                            if (WebUtils.GetAppSettings("default_color") == "1")
                                HtmlUtil.initColorSet2DB(TextBox_Acc.Text);
                            Response.Write("<script>alert('伺服器回應 : 已申請成功! 即將跳轉至登入頁!');location.href='../login.aspx';</script>");
                        }
                    }
                    else
                        Response.Write("<script>alert('伺服器回應 : 此手機號碼已經有人使用! 請輸入其它號碼!');</script>");
                }
                else
                    Response.Write("<script>alert('伺服器回應 : 此帳號已經有人使用! 請輸入其它帳號!');</script>");
            }
            else
            {
                Response.Write("<script>alert('伺服器回應 : 帳號或密碼未輸入或包含特殊符號，請重新輸入');</script>");
            }
        }
        private void Set_DropDownlist()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = "SELECT distinct area_name FROM account_info where (area_name <> '全廠' and area_name <> '測試區')  ";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            DropDownList_Factory.Items.Clear();
            ListItem listItem = new ListItem();
            listItem = new ListItem("全廠", "全廠");
            if (!DropDownList_Factory.Items.Contains(listItem))
                DropDownList_Factory.Items.Add(listItem);

            if (HtmlUtil.Check_DataTable(dt) && DropDownList_Factory.Items.Count == 1)
            {
                foreach (DataRow row in dt.Rows)
                {
                    listItem = new ListItem(DataTableUtils.toString(row["area_name"]), DataTableUtils.toString(row["area_name"]));
                    if (!DropDownList_Factory.Items.Contains(listItem))
                        DropDownList_Factory.Items.Add(listItem);
                }
            }

        }
        //0606 juiedit
        [WebMethod]
        public static string CheckAcc(string ACC)
        {
            bool error = true;
            if (ACC != "")
            {
                //檢查特殊符號
                Regex regex = new Regex("[-']");
                error = regex.IsMatch(ACC);
            }
            else
                return "2";//正常
            if (error != true)
            {
                //檢查帳號是否重覆
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                string sqlcmd = $"SELECT * FROM USERS where USER_ACC = '{ACC}'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    return "1";//有代表註冊了給異常
                }
                return "2";//正常
            }
            else
                return "3";
        }
    }
}