using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using System.Data;
using Support;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class Asm_TotalAll : System.Web.UI.Page
    {
        public string power = "";
        public string color = "";
        public string TagetPiece = "0";
        public string TagetPerson = "0";
        public string FinishPiece = "0";
        public string OnLinePiece = "0";
        public string ErrorPiece = "0";
        public string LineName = "";
        public string thCol = "";
        public string ColumnsData = "<th class='center'>沒有資料載入</th>";
        public string RowsData = "<tr class='even gradeX'> <td class='center'> no data </ td ></ tr >";
        string acc = "";
        public int behind = 0;
        public int alarm_total = 0;
        ShareFunction SFun = new ShareFunction();
        public string PieceUnit = ShareMemory.PieceUnit;
        clsDB_Server clsdb = new clsDB_Server(myclass.GetConnByDekdekVisAssmHor);
        private void GotoCenn()
        {
            AddProgressList();
            LoadData();
            check_power(acc);
        }
        private void GetTableData(int LineNum, string reqType, string schdule = "")
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            acc = DataTableUtils.toString(userInfo["user_ACC"]);
            string[] result;
            string[] reqStr = new string[2];
            if (reqType != "")
                reqStr = reqType.Split('=');
            if (acc != null)
            {
                ColumnsData = SFun.GetColumnName("Asm_LineSearch");
                string judge = "";
                if (ShareFunction.Last_Place(acc) == "Hor")
                    judge = ShareFunction.Last_Place(acc);
                result = SFun.GetOverViewData(LineName, acc, reqStr[1], ref behind, ref alarm_total, judge);
                OnLinePiece = result[0];
                FinishPiece = result[1];
                ErrorPiece = result[2];
                RowsData = result[6];
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        private void LoadData()
        {
            GetTableData(0, $"ReqType=Line");
        }
        private void AddProgressList()
        {
            for (int i = 0; i < 100; i = i + 10)
                DropDownList_progress.Items.Add(i.ToString() + "%");
            DropDownList_progress.Items.Add("99" + "%");
            DropDownList_progress.Items.Add("100" + "%");
        }
        private void check_power(string acc)
        {
            bool ok = SFun.GetRW(acc);
            string dep = HtmlUtil.Search_acc_Column(acc, "USER_DPM");
            string powers = HtmlUtil.Search_acc_Column(acc, "power");
            string can_close = HtmlUtil.Search_acc_Column(acc, "can_close");
            if (ok || dep == "PMD" || powers == "Y" || can_close == "Y" || can_close == "A")
            {
                RadioButtonList_select_type.Enabled = true;
                DropDownList_progress.Enabled = true;
                TextBox_Report.Enabled = true;
                power = "PMD";
            }
            else
            {
                RadioButtonList_select_type.Enabled = false;
                DropDownList_progress.Enabled = false;
                TextBox_Report.Enabled = false;
                power = "";
            }
        }
        //--------------------Event-------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            string CompLoacation = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                CompLoacation = ShareFunction.Last_Place(acc);
                ShareFunction.Last_Place(acc, CompLoacation);

                //可以進入 -> 執行後面程式碼
                if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                {
                    if (!IsPostBack)
                        GotoCenn();
                    //效能測試
                }
                //無法進入 -> 登入COOKIES
                else
                    Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        protected void button_select_Click(object sender, EventArgs e)
        {
            string condition = "";
            string Key = TextBox_show.Text;
            string CompLoacation = "";
            string Linenumber = "";

            DataTable dts = clsdb.GetDataTable($"select * from 工作站狀態資料表 where 排程編號 = '{Key}'");
            if (HtmlUtil.Check_DataTable(dts))
                Linenumber = DataTableUtils.toString(dts.Rows[0]["工作站編號"]);
            if (Key != "")
            {
                CompLoacation = ShareFunction.Last_Place(acc);
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
                condition = $"排程編號='{Key}' AND 工作站編號='{Linenumber}'";
                DataTableUtils.Conn_String = SFun.GetConnByDekVisTmp;
                DataRow dr = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLAsm_WorkStation_State, condition);

                DataTable dt = clsdb.GetDataTable($"select * from {ShareMemory.SQLAsm_WorkStation_State} where {condition}");
                bool updataok = SFun.change_status(SFun.GetConnByDekVisTmp, dt, ShareMemory.SQLAsm_WorkStation_State, condition, RadioButtonList_select_type.SelectedItem.Text, TextBox_Report.Text.Replace("'", "^").Replace('"', '#').Replace(" ", "$").Replace("\r\n", "@"), DropDownList_progress.SelectedItem.Text.Trim('%'));
                SFun.Set_MachineID_Line_Updata(dr["工作站編號"].ToString());
            }
            else
                Response.Write("<script language='javascript'>alert('伺服器回應 : 輸入資訊異常!');</script>");
            Response.Redirect($"../dp_PM/Asm_TotalAll.aspx");
        }
        public string Set_CSS(int Cloumn_Number)
        {
            return $"   #TB tbody tr th:nth-child({Cloumn_Number}) " +
                "{" +
                " display: none;" +
                "}" +
                $"#TB thead tr th:nth-child({Cloumn_Number}) " +
                "{" +
                " display: none;" +
                "}" +
                $"#TB tbody tr td:nth-child({Cloumn_Number}) " +
                "{" +
                " display: none;" +
                "}";
        }

    }
}