using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using System.Data;
using Support;


namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class Asm_LineTotalView : System.Web.UI.Page
    {
        public int no_solved = 0;
        public int behind = 0;
        public string color = "";
        public string ColumnsData = "<th class='center'>沒有資料載入</th>";
        public string RowsData = "<tr class='even gradeX'> <td class='center'> no data </ td ></ tr >";
        public string NowYear = "";
        public string NowMonth = "";
        public string TotalTagetPiece = "0";
        public string TotalTagetPerson = "0";
        public string TotalFinishPiece = "0";
        public string TotalOnLinePiece = "0";
        public string TotalErrorPiece = "0";
        public string td_TotalFinishPiece = "0";
        public string td_TotalOnLinePiece = "0";
        public string td_TotalErrorPiece = "0";
        public string NowDay = "";
        public string PieceUnit = ShareMemory.PieceUnit;
        clsDB_Server clsDB_sw = new clsDB_Server("");
        ShareFunction SFun = new ShareFunction();
        myclass myclass = new myclass();
        protected void Page_Load(object sender, EventArgs e)
        {



            HttpCookie userInfo = Request.Cookies["userInfo"];
            string acc = "";
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                            load_page_data();
                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //============Event=======================
        private void load_page_data()
        {
            try
            {
                //資料庫連線成功要做的事()
                string[] date = new string[6];
                ColumnsData = SFun.GetColumnName("Asm_LineTotalView");
                date = SFun.GetLineTotal(6, ref no_solved, ref behind); // columns 6 //
                RowsData = date[1];
                TotalTagetPiece = SFun.cTotalTagetPiece;
                TotalTagetPerson = SFun.cTotalTagetPerson;
                TotalFinishPiece = SFun.cTotalFinishPiece;
                TotalOnLinePiece = SFun.cTotalOnLinePiece;
                TotalErrorPiece = SFun.cTotalErrorPiece;
                td_TotalFinishPiece = SFun.td_cTotalFinishPiece;
                td_TotalOnLinePiece = SFun.td_cTotalOnLinePiece;
                td_TotalErrorPiece = SFun.td_cTotalErrorPiece;
            }
            catch
            {
                Response.Write("<script>alert('資料庫連線異常!');location.href='../index.aspx';</script>");
            }

        }
        protected void Button_Search_Click(object sender, EventArgs e)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            List<string> schdule = new List<string>(TextBox_Schdule.Text.Split('#'));
            string schdules = "";
            for (int i = 0; i < schdule.Count; i++)
            {
                if (schdule[i] != "")
                    schdules += schdules == "" ? $" 排程編號='{schdule[i]}' " : $" OR 排程編號='{schdule[i]}' ";
            }
            schdules = schdules != "" ? $" where {schdules} " : "000";


            string sqlcmd = $"select * from 工作站狀態資料表 {schdules}";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                if (DataTableUtils.toString(dt.Rows[0]["進度"]) != "100" && DataTableUtils.toString(dt.Rows[0]["狀態"]) != "完成")
                    Response.Redirect($"Asm_LineSearch.aspx?key={WebUtils.UrlStringEncode($"ReqType=Line,schdule={TextBox_Schdule.Text}")}");
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('該排程已完成,請重新輸入');location.href='Asm_LineTotalView.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('未找到該排程,請重新輸入');location.href='Asm_LineTotalView.aspx';</script>");
        }
    }
}