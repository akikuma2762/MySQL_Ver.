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

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class Asm_ErrorDetail : System.Web.UI.Page
    {
        string startdate = "";
        string enddate = "";
        string errortype = "";
        string db = "";
        public string UrlLink = "";
        public string color = "";
        public string ColumnsData = "<th class='center'>沒有資料載入</th>";
        public string RowsData = "<tr class='even gradeX'> <td class='center'> no data </ td ></ tr >";
        public string Key = "";
        public string ErrorType = "";
        public string LineNum = "";
        public string LineName = "";
        public string[] RowsDataArray = new string[5] { "", "", "", "", "" };
        public string[] ErrorTitleDisplayDep = new string[5] { "nodata", "nodata", "nodata", "nodata", "nodata", };
        public string[] ErrorTitleDisplayStatus = new string[5] { "nodata", "nodata", "nodata", "nodata", "nodata", };
        string MantID = "";
        string acc = "";
        string dpm = "";
        public string go_back = "";
        clsDB_Server clsDB_sw = new clsDB_Server("");
        ShareFunction SFun = new ShareFunction();
        public string word = "";
        string CompLoacation = "";
        /*----------------20200424留言板功能---------*/
        string Return_Image = "";
        /*----------------20200424留言板功能---------*/
        private void GotoCenn()
        {
            if (CompLoacation == "Hor")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            }
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
            }

            load_page_data();


        }
        private void GetTableData(string ErrorID, string LineNum)
        {
            string[] result;
            string[] ErrorTitleShow = new string[5] { "", "", "", "", "" };

            ColumnsData = SFun.GetColumnName("Asm_ErrorDetail");
            result = SFun.GetMantRowsData(acc, ErrorID, LineNum, ref ErrorTitleShow, ref ErrorTitleDisplayDep, ref ErrorTitleDisplayStatus, "", MantID, startdate, enddate, errortype, db);

            if (result[0] != "")
                RowsDataArray = result;
        }
        private void GetDropDownList_Error()
        {
            if (CompLoacation == "Hor")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            }
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
            }
            DropDownList_ErrorChild.Items.Clear();
            DropDownList_Errorfa.Items.Clear();
            DropDownList_Errorfa.DataSource = SFun.CaseErrorType(SFun.GetConnByDekVisTmp);
            DropDownList_Errorfa.DataBind();
            DropDownList_ErrorChild.DataSource = SFun.CaseErrorType(SFun.GetConnByDekVisTmp);
            DropDownList_ErrorChild.DataBind();
        }

        private void GetDropDownList_Status()
        {
            if (CompLoacation == "Hor")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
            }
            else
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
            }
            DropDownList_Status.DataSource = SFun.GetErrorProcessStatus();
            DropDownList_Status.DataBind();
        }
        private void load_page_data()
        {
            LoadData();
        }
        private void LoadData()
        {
            Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
            if (keyValues.Count != 0)
            {
                if (keyValues.Count > 1)
                {
                    LineNum = HtmlUtil.Search_Dictionary(keyValues, "ErrorLineNum");
                    LineName = HtmlUtil.Search_Dictionary(keyValues, "ErrorLineName");
                    if (CompLoacation == "Hor")
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                        SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                    }
                    else
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                        SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                    }
                }
                Key = HtmlUtil.Search_Dictionary(keyValues, "ErrorID");
                GetDropDownList_Error();
                Set_Dropdownlist();
                GetDropDownList_Status();
                GetTableData(Key, LineNum);
            }
            else
                Response.Redirect("Asm_LineTotalView.aspx");
        }

        private void Set_Dropdownlist()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = "SELECT * FROM department where DPM_GROUP IS NULL and DPM_NAME <> 'L00'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                if (DropDownList_respone.Items.Count == 0 && DropDownList_responechild.Items.Count == 0)
                {
                    ListItem list = new ListItem("", "");
                    DropDownList_respone.Items.Add(list);
                    DropDownList_responechild.Items.Add(list);

                    foreach (DataRow row in dt.Rows)
                    {
                        list = new ListItem(DataTableUtils.toString(row["DPM_NAME2"]), DataTableUtils.toString(row["DPM_NAME2"]));
                        DropDownList_respone.Items.Add(list);
                        DropDownList_responechild.Items.Add(list);

                    }
                }
            }
        }

        //---------------------------Event----------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {


            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                CompLoacation = ShareFunction.Last_Place(acc);
                while (CompLoacation == "")
                {
                    CompLoacation = ShareFunction.Last_Place(acc);
                    if (CompLoacation != "")
                        break;
                }
                dpm = acc_dpm(acc);

                string save = "";

                if (Request.QueryString["key"] != null)
                {
                    Dictionary<string, string> Url_List = HtmlUtil.Return_dictionary(Request.QueryString["key"]);

                    if (Url_List.Count < 3)
                        Response.Redirect("Asm_LineTotalView.aspx");
                    if (HtmlUtil.Search_Dictionary(Url_List, "history") == "1")
                        PlaceHolder_hidden.Visible = false;
                    MantID = HtmlUtil.Search_Dictionary(Url_List, "MantID");
                    startdate = HtmlUtil.Search_Dictionary(Url_List, "Date_str");
                    enddate = HtmlUtil.Search_Dictionary(Url_List, "Date_end");
                    errortype = HtmlUtil.Search_Dictionary(Url_List, "ErrorType");
                    db = HtmlUtil.Search_Dictionary(Url_List, "db");

                    if (CompLoacation == "Hor")
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                        SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssmHor;
                    }
                    else
                    {
                        GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssm);
                        SFun.GetConnByDekVisTmp = myclass.GetConnByDekdekVisAssm;
                    }
                    save = CompLoacation;

                    ShareFunction.Last_Place(acc, save);
                    UrlLink = WebUtils.UrlStringEncode($"LineNum={HtmlUtil.Search_Dictionary(Url_List, "ErrorLineNum")},ReqType=Line");
                    if (MantID != "")
                        go_back = $"<li><u><a href=\"Asm_ErrorDetail.aspx?key={WebUtils.UrlStringEncode($"ErrorID={HtmlUtil.Search_Dictionary(Url_List, "ErrorID")},ErrorLineNum={HtmlUtil.Search_Dictionary(Url_List, "ErrorLineNum")},ErrorLineName={HtmlUtil.Search_Dictionary(Url_List, "ErrorLineName")}")}\">上一頁</a></u></li>";
                }
                else
                    Response.Redirect("Asm_LineTotalView.aspx");

                /*-----------------------------------------------------------------------*/

                //可以進入 -> 執行後面程式碼
                if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                {
                    if (!IsPostBack)
                        GotoCenn();
                }
                //無法進入 -> 登入COOKIES
                else
                    Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //父項目新增
        protected void Unnamed_ServerClick(object sender, EventArgs e)
        {
            string CompLoacation = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            acc = DataTableUtils.toString(userInfo["user_ACC"]);
            if (acc != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);

                Key = HtmlUtil.Search_Dictionary(keyValues, "ErrorID");
                if (keyValues.Count > 1)
                    LineNum = HtmlUtil.Search_Dictionary(keyValues, "ErrorLineNum");

                if (LineNum == "0")
                {
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                    string sqlcmd = $"select * from 工作站狀態資料表 where 排程編號='{Key}'";
                    DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(ds))
                        LineNum = DataTableUtils.toString(ds.Rows[0]["工作站編號"]);
                }

                CompLoacation = ShareFunction.Last_Place(acc);
                if (DropDownList_Errorfa.SelectedItem.Text != "")
                {
                    if (!SFun.SetMantDataToDataTable(acc, Key, DropDownList_Errorfa.SelectedItem.Text, MantStr.Text, dpm, "處理", keyValues, RadioButtonList_Post.SelectedValue, DropDownList_respone.SelectedItem.Value, RadioButtonList_show.SelectedItem.Value, HtmlUtil.FileUpload_Name(FileUpload_image, "Backup_Error_Image"), LineNum))
                        Response.Write("<script language='javascript'>alert('伺服器回應 : 新增異常，請洽管理者協助。');</script>");
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('伺服器回應 : 新增成功');location.href='Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('伺服器回應 : 請選擇異常類型');location.href='Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
            }
            else
                Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管主管申請權限!');location.href='../index.aspx';</script>");
        }
        //刪除父 子項目
        protected void bt_del_ServerClick(object sender, EventArgs e)
        {
            string CompLoacation = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            acc = DataTableUtils.toString(userInfo["user_ACC"]);
            CompLoacation = ShareFunction.Last_Place(acc);
            var num = TextBox_num.Text;
            if (num != null && num != "")
                Response.Write($"<script language='javascript'>alert('伺服器回應 : {SFun.ErrorDetailDeleteProcess(num, acc)}');location.href='../dp_PM/Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
            else
                Response.Write($"<script language='javascript'>alert('伺服器回應 : 請先選擇刪除項目');location.href='../dp_PM/Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
        }
        /*----------------20200424留言板功能---------*/

        //新增子項目
        protected void AddContent_Click(object sender, EventArgs e)
        {
            //修改+回覆的檔案
            if (TextBox_File.Text != "")
                Return_Image += $"{TextBox_File.Text.Replace('^', '\n')}\n";
            Return_Image += HtmlUtil.FileUpload_Name(FileUpload_Content, "Backup_Error_Image");

            string close = "";
            if (TextBox_Close.Text != "")
                close += $"{TextBox_Close.Text.Replace('^', '\n')}\n";
            close += HtmlUtil.FileUpload_Name(FileUpload_Close, "Backup_File");

            string[] status = TextBox_textTemp.Text.Split('_');
            int ID = 0;

            if (status.Length == 2)
                ID = DataTableUtils.toInt(status[1]);

            // ------------------20200424------------------------
            if (DropDownList_Status.SelectedItem.Text == "處理")
            {
                //修改
                if (status.Length == 2)
                {
                    if (SFun.Add_Content(DropDownList_responechild.SelectedItem.Value, RadioButtonList_showchild.SelectedItem.Value, RadioButtonList_Upost.SelectedValue, ID.ToString(), DropDownList_ErrorChild.SelectedItem.Text, TextBox_ErrorID.Text, acc, dpm, TextContent.Text, Return_Image, ID))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改成功');location.href='Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
                }
                //新增
                else
                {
                    if (SFun.Add_Content(DropDownList_responechild.SelectedItem.Value, RadioButtonList_showchild.SelectedItem.Value, RadioButtonList_Upost.SelectedValue, TextBox_textTemp.Text, DropDownList_ErrorChild.SelectedItem.Text, TextBox_ErrorID.Text, acc, dpm, TextContent.Text, Return_Image))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增成功');location.href='Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
                }
            }
            else if (DropDownList_Status.SelectedItem.Text == "結案")
            {
                //修改
                if (status.Length == 2)
                {
                    if (SFun.Add_Content(DropDownList_responechild.SelectedItem.Value, RadioButtonList_showchild.SelectedItem.Value, RadioButtonList_Upost.SelectedValue, ID.ToString(), DropDownList_ErrorChild.SelectedItem.Text, TextBox_ErrorID.Text, acc, dpm, TextContent.Text, Return_Image, ID, DropDownList_ErrorChild.SelectedItem.Text, TextBox_Report.Text, close))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('修改成功');location.href='Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
                }
                //新增
                else
                {
                    if (SFun.Add_Content(DropDownList_responechild.SelectedItem.Value, RadioButtonList_showchild.SelectedItem.Value, RadioButtonList_Upost.SelectedValue, TextBox_textTemp.Text, DropDownList_ErrorChild.SelectedItem.Text, TextBox_ErrorID.Text, acc, dpm, TextContent.Text, Return_Image, 0, DropDownList_ErrorChild.SelectedItem.Text, TextBox_Report.Text, close))
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('新增成功');location.href='Asm_ErrorDetail.aspx{Request.Url.Query}';</script>");
                }
            }
        }

        //查詢所屬的部門
        private string acc_dpm(string acc)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = $"select DPM_NAME2 from users left join DEPARTMENT on USERS.USER_DPM = DEPARTMENT.DPM_NAME where USER_ACC = '{acc}'";
            DataRow row = DataTableUtils.DataTable_GetDataRow(sqlcmd);
            if (row != null)
                return DataTableUtils.toString(row["DPM_NAME2"]);
            else
                return "";
        }
    }
}