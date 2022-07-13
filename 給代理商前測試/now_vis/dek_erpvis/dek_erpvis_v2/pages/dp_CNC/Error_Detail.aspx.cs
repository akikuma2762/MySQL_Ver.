using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using System.Data;
using Support;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_CNC
{
    public partial class Error_Detail : System.Web.UI.Page
    {
        //----------------------------------------------參數OR引用---------------------------------------------
        public string color = "";
        public string order = "";
        public string machine_name = "";
        public string Number = "";
        public string th = "";
        public string tr = "";
        string acc = "";
        string mantid = "";
        ERP_cnc CNC = new ERP_cnc();
        //----------------------------------------------事件---------------------------------------------------
        //載入事件
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (acc != "")
                {
                    if (!IsPostBack)
                        Mainprocess();
                }
                else
                    Response.Redirect(myclass.logout_url);
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //儲存父項目事件
        protected void Unnamed_ServerClick(object sender, EventArgs e)
        {
            string File_Url = HtmlUtil.FileUpload_Name(FileUpload_image, "Backup_Error_Image");
            bool ok = CNCError.Save_FatherMessage(Label_Staff.Text,Label_Type.Text, Label_Order.Text, Label_Machine.Text, Label_Group.Text, acc, DropDownList_Errorfa.SelectedItem.Text, MantStr.Text, File_Url, RadioButtonList_Post.SelectedItem.Value);
            if (ok)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                string sqlcmd = $"select * from workorder_information where mach_name='{Label_Machine.Text}'  and product_number='{Label_Number.Text}' and order_status <> '出站'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    List<bool> oks = new List<bool>();
                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow rows = dt.NewRow();
                        rows["_id"] = row["_id"];
                        rows["order_status"] = "暫停";
                        rows["error_type"] = "ERROR";
                        rows["last_updatetime"] = now_time;
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        oks.Add(DataTableUtils.Update_DataRow("workorder_information", $"manu_id='{row["manu_id"]}' and  mach_name='{Label_Machine.Text}' and product_number='{Label_Number.Text}' and order_status <> '出站'", rows));
                    }
                    if (oks.IndexOf(false) == -1)
                    {
                        //存入暫停
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                        sqlcmd = $"select * from (  SELECT  *, (SELECT  MIN(a.now_time) FROM record_worktime a WHERE workman_status = '出站'  AND a.mach_name = record_worktime.mach_name AND a.work_staff = record_worktime.work_staff AND a.now_time >= record_worktime.now_time AND a.manu_id = record_worktime.manu_id) exit_time FROM record_worktime WHERE workman_status = '入站') a where a.exit_time IS null and product_number='{Label_Number.Text}' and mach_name='{Label_Machine.Text}'  and now_time <='{DateTime.Now:yyyyMMddHHmmss}' ";
                        DataTable dts = DataTableUtils.GetDataTable(sqlcmd);
                        if (HtmlUtil.Check_DataTable(dts))
                        {
                            DataTable dt_clone = HtmlUtil.Get_HeadRow(dts);
                            
                            foreach (DataRow row in dts.Rows)
                            {
                                DataRow rows = dt_clone.NewRow();
                                for (int x = 1; x <= dts.Columns.Count-1; x++)
                                {
                                    if(dts.Columns[x].ColumnName == "workman_status")
                                        rows["workman_status"] = "暫停";
                                    else if (dts.Columns[x].ColumnName == "now_time")
                                        rows["now_time"] = now_time;      
                                    else if (dts.Columns[x].ColumnName == "stop_type")
                                        rows["stop_type"] = "ERROR";      
                                    else if (dts.Columns[x].ColumnName == "stop_reason")
                                        rows["stop_reason"] = MantStr.Text;
                                    else
                                        rows[dts.Columns[x].ColumnName] = row[dts.Columns[x].ColumnName];
                                }
                                dt_clone.Rows.Add(rows);

                            }
                            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                            if (dts.Rows.Count == DataTableUtils.Insert_TableRows("record_worktime", dt_clone))
                                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存成功');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                            else
                                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                        }
                        else
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                    }
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                }
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('儲存失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
        }
        //儲存子項目事件
        protected void AddContent_Click(object sender, EventArgs e)
        {
            //回覆檔案
            string Reply_File = "";
            //結案檔案
            string Close_File = "";

            if (TextBox_File.Text != "")
                Reply_File = $"{TextBox_File.Text.Replace('^', '\n')}\n";
            Reply_File += HtmlUtil.FileUpload_Name(FileUpload_Content, "Backup_Error_Image");

            if (TextBox_Close.Text != "")
                Close_File = $"{TextBox_Close.Text.Replace('^', '\n')}\n";
            Close_File += HtmlUtil.FileUpload_Name(FileUpload_Close, "Backup_File");

            string[] status = TextBox_textTemp.Text.Split('_');
            int ID = 0;

            if (status.Length == 2)
                ID = DataTableUtils.toInt(status[1]);

            if (DropDownList_Status.SelectedItem.Text == "處理")
            {
                //修改
                if (status.Length == 2)
                {
                    //更新
                    if (CNCError.Add_Content(Label_Staff.Text,Label_Type.Text, Label_Order.Text, Label_Machine.Text, Label_Group.Text, acc, ID.ToString(), DropDownList_ErrorChild.SelectedItem.Text, TextContent.Text, Reply_File, RadioButtonList_Upost.SelectedItem.Value, ID))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('更新成功');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('更新失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                }
                //新增
                else
                {
                    if (CNCError.Add_Content(Label_Staff.Text, Label_Type.Text, Label_Order.Text, Label_Machine.Text, Label_Group.Text, acc, TextBox_textTemp.Text, DropDownList_ErrorChild.SelectedItem.Text, TextContent.Text, Reply_File, RadioButtonList_Upost.SelectedItem.Value))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增成功');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                }
            }
            else if (DropDownList_Status.SelectedItem.Text == "結案")
            {
                //修改
                if (status.Length == 2)
                {
                    //更新
                    if (CNCError.Add_Content(Label_Staff.Text,Label_Type.Text, Label_Order.Text, Label_Machine.Text, Label_Group.Text, acc, ID.ToString(), DropDownList_ErrorChild.SelectedItem.Text, TextContent.Text, Reply_File, RadioButtonList_Upost.SelectedItem.Value, ID, DropDownList_ErrorChild.SelectedItem.Text, TextBox_Report.Text, Close_File))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('更新成功');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('更新失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                }
                //新增
                else
                {
                    if (CNCError.Add_Content(Label_Staff.Text,Label_Type.Text, Label_Order.Text, Label_Machine.Text, Label_Group.Text, acc, TextBox_textTemp.Text, DropDownList_ErrorChild.SelectedItem.Text, TextContent.Text, Reply_File, RadioButtonList_Upost.SelectedItem.Value, 0, DropDownList_ErrorChild.SelectedItem.Text, TextBox_Report.Text, Close_File))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增成功');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                }
            }

        }
        //刪除事件(父OR子)
        protected void bt_del_ServerClick(object sender, EventArgs e)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
            string sqlcmd = $"select * from error_report where 異常維護編號 = '{TextBox_num.Text}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCnc_inside);
                if (DataTableUtils.Delete_Record("error_report", $"異常維護編號 = '{TextBox_num.Text}'"))
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('已成功刪除');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('刪除失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('刪除失敗');location.href='Error_Detail.aspx{Request.Url.Query}';</script>");
        }

        //----------------------------------------------方法---------------------------------------------------
        private void Mainprocess()
        {
            Set_Dropdownlist();
            Set_Paramer();
            Set_DataTable();
        }

        //設定下拉選單的值
        private void Set_Dropdownlist()
        {
            //清空下拉選單
            DropDownList_Errorfa.Items.Clear();
            DropDownList_ErrorChild.Items.Clear();

            DataTable dt = CNC.ErrorType();
            ListItem list = new ListItem("", "");
            DropDownList_Errorfa.Items.Add(list);
            DropDownList_ErrorChild.Items.Add(list);

            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list = new ListItem(DataTableUtils.toString(row["異常類型"]), DataTableUtils.toString(row["異常類型"]));
                    DropDownList_Errorfa.Items.Add(list);
                    DropDownList_ErrorChild.Items.Add(list);
                }
            }
        }

        //解析參數
        private void Set_Paramer()
        {
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                Label_Order.Text = HtmlUtil.Search_Dictionary(keyValues, "order");
                Label_Number.Text = HtmlUtil.Search_Dictionary(keyValues, "number");
                Label_Machine.Text = HtmlUtil.Search_Dictionary(keyValues, "machine");
                Label_Group.Text = HtmlUtil.Search_Dictionary(keyValues, "group");
                Label_Type.Text = HtmlUtil.Search_Dictionary(keyValues, "type");
                Label_Staff.Text = HtmlUtil.Search_Dictionary(keyValues, "staff");
                Label_ShowMachine.Text = HtmlUtil.Search_Dictionary(keyValues, "show_name");
                mantid = HtmlUtil.Search_Dictionary(keyValues, "num");
            }
        }

        //設定表格
        private void Set_DataTable()
        {
            th = "<tr id=\"tr_row\"><th style=\"text-align:center;\">人員</th><th style=\"text-align:center;\">內容</th><th style=\"text-align:center;\">狀態</th></tr>";
            tr = CNCError.Question_History(Label_Type.Text, acc, Label_Order.Text, Label_Machine.Text, Label_Group.Text, mantid);
        }
    }
}