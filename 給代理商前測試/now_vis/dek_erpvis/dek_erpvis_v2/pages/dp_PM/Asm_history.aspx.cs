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
using ClosedXML.Excel;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class Asm_history : System.Web.UI.Page
    {
        public string color = "";
        public string ColumnsData = "<th class=\"center\">沒有資料載入</th>";
        public string RowsData = "<tr> <td class=\"center\"> no data </td></tr>";
        public string NowYear = "";
        public string NowMonth = "";
        public string NowDay = "";
        public string Key = "";
        public string ErrorType = "";
        public string LineNum = "";
        public string LineName = "";
        clsDB_Server clsDB_sw = new clsDB_Server("");
        ShareFunction SFun = new ShareFunction();
        myclass myclass = new myclass();
        private void GotoCenn()
        {
            clsDB_sw.dbOpen(myclass.GetConnByDekdekVisAssmHor);
            DataTableUtils.Conn_String = SFun.GetConnByDekdekVisAssmHor;
            if (clsDB_sw.IsConnected == true)
                load_page_data();
            else
                Response.Write($"<script language='javascript'>alert('伺服器回應 : 無法載入資料，{clsDB_sw.ErrorMessage} 請聯絡德科人員或檢查您的網路連線。');</script>");
        }
        private void load_page_data()
        {
            if (GlobalVar.Conn_status == true) //資料庫連線成功要做的事()
                LoadData();
            else
                Response.Write("<script language='javascript'>alert('伺服器回應 : 無法載入資料， 請聯絡德科人員或檢查您的網路連線。');</script>");
        }
        private void LoadData()
        {
            GetDropDownList_LineName();
        }
        private void GetTableData(string ErrorID, ListItem Line)
        {
            string msg = "";
            string[] result = new string[2];
            GetDropDownList_LineName();
            ColumnsData = SFun.GetColumnName("Asm_history");

            result = SFun.GetHistoryRowsData(ErrorID, Line, textbox_dt1.Text, textbox_dt2.Text);


            if (result[1] != null)
            {
                RowsData = result[1];
                Button_Print.Visible = true;
            }

            else
            {
                Response.Write("<script language='javascript'>alert('伺服器回應 : 查無此資料，確認輸入資訊正確');</script>");
                HtmlUtil.NoData(out ColumnsData, out RowsData);
                Button_Print.Visible = false;
            }
        }
        private void GetDropDownList_LineName()
        {
            if (DropDownList_Line.Items.Count == 0)
            {

                SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;
                Session["Location"] = "HOR";
                DropDownList_Line.DataTextField = "LineName";//default show Text
                DropDownList_Line.DataValueField = "LineID";
                DropDownList_Line.DataSource = SFun.GetLineList();
                DropDownList_Line.DataBind();
                DropDownList_Line.Items.Insert(0, new ListItem("--Select--", "00"));
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string CompLoacation = "";
            string acc = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);

                string URL_NAME = "Asm_history";
                if (Session["Location"] != null)
                    CompLoacation = Session["Location"].ToString();
                color = HtmlUtil.change_color(acc);
                if ( HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0])|| myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            if (CompLoacation.ToUpper().Contains("HOR"))
                                SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;
                            GotoCenn();

                            if (Request.QueryString["key"] != null)
                            {
                                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                                List<string> list = new List<string>(HtmlUtil.Search_Dictionary(keyValues, "Errorkey").Split('$'));
                                ColumnsData = SFun.GetColumnName("Asm_history");
                                string[] result = new string[2];
                                result = SFun.GetHistoryRowsData("", null, "", "", list, HtmlUtil.Search_Dictionary(keyValues, "ErrorLineNum"), HtmlUtil.Search_Dictionary(keyValues, "Local").ToLower(), HtmlUtil.Search_Dictionary(keyValues, "Date_str"), HtmlUtil.Search_Dictionary(keyValues, "Date_end"), HtmlUtil.Search_Dictionary(keyValues, "ErrorType"));
                                if (result[1] != null)
                                    RowsData = result[1];
                            }
                        }

                    }
                    //無法進入 -> 登入COOKIES
                    else
                        Response.Write("<script>alert('目前人數已滿，請稍後登入');location.href='../index.aspx';</script>");

                }
                else
                {
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管主管申請權限!');location.href='../index.aspx';</script>");
                    HtmlUtil.NoData(out ColumnsData, out RowsData);
                }
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        protected void DropDownList_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;
            Session["Location"] = "HOR";
            GetDropDownList_LineName();
        }
        protected void Unnamed_ServerClick(object sender, EventArgs e)
        {
            string CompLoacation = "";

            if (DropDownList_Line.SelectedItem.Text == "--Select--")
            {
                Response.Write("<script>alert('請選擇產線');location.href='Asm_history.aspx';</script>");
                HtmlUtil.NoData(out ColumnsData, out RowsData);
            }
            else
            {
                if (Session["Location"] != null)
                    CompLoacation = Session["Location"].ToString();
                if (CompLoacation.ToUpper().Contains("HOR"))
                    SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;
                GetTableData(Mant_Search.Value, DropDownList_Line.SelectedItem);
            }
        }



        protected void Button_Print_Click(object sender, EventArgs e)
        {
            string condition = "";
            if (DropDownList_Line.SelectedItem.Value != "0")
                condition = $" and 工作站異常維護資料表.工作站編號 = {DropDownList_Line.SelectedItem.Value}";
            if (Mant_Search.Value != "")
                condition += $" and  排程編號 = '{Mant_Search.Value}' ";
            if (textbox_dt1.Text != "" && textbox_dt2.Text != "")
                condition += $" and substring(時間紀錄,1,8) >= {textbox_dt1.Text.Replace("-", "")} and substring(時間紀錄,1,8) <= {textbox_dt2.Text.Replace("-", "")} ";


            DataTable ds = new DataTable();
            ds.Columns.Add("刀庫編號");
            ds.Columns.Add("發起時間");
            ds.Columns.Add("發起人");
            ds.Columns.Add("發起內容");
            ds.Columns.Add("回覆時間");
            ds.Columns.Add("回覆人");
            ds.Columns.Add("回覆內容");

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = $"select 異常維護編號, 排程編號, 工作站名稱,排程編號,維護人員姓名,維護人員單位,維護內容,時間紀錄,父編號,結案判定類型,結案內容 from 工作站異常維護資料表 left join 工作站型態資料表 on 工作站異常維護資料表.工作站編號 = 工作站型態資料表.工作站編號 where (父編號 is null OR 父編號 = 0) {condition} ";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    //一開始先加入父編號
                    DataRow rew = ds.NewRow();
                    rew["刀庫編號"] = DataTableUtils.toString(row["排程編號"]);
                    rew["發起時間"] = ShareFunction.StrToDate(DataTableUtils.toString(row["時間紀錄"]));
                    rew["發起人"] = DataTableUtils.toString(row["維護人員姓名"]);
                    rew["發起內容"] = DataTableUtils.toString(row["維護內容"]);
                    ds.Rows.Add(rew);
                    GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                    sqlcmd = $"select * from 工作站異常維護資料表 where 父編號 = {DataTableUtils.toString(row["異常維護編號"])}";
                    DataTable dd = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(dd))
                    {
                        foreach (DataRow rrw in dd.Rows)
                        {
                            rew = ds.NewRow();
                            //表尚未結案
                            if (DataTableUtils.toString(rrw["結案判定類型"]) == null || DataTableUtils.toString(rrw["結案判定類型"]) == "")
                            {
                                rew["刀庫編號"] = DataTableUtils.toString(row["排程編號"]);
                                rew["回覆時間"] = ShareFunction.StrToDate(DataTableUtils.toString(rrw["時間紀錄"]));
                                rew["回覆人"] = DataTableUtils.toString(rrw["維護人員姓名"]);
                                rew["回覆內容"] = DataTableUtils.toString(rrw["維護內容"]);
                            }
                            //表已結案
                            else
                            {
                                rew["刀庫編號"] = DataTableUtils.toString(row["排程編號"]);
                                rew["回覆時間"] = ShareFunction.StrToDate(DataTableUtils.toString(rrw["時間紀錄"]));
                                rew["回覆人"] = "[結案類型]：" + DataTableUtils.toString(rrw["結案判定類型"]);
                                if (DataTableUtils.toString(rrw["結案內容"]) == null || DataTableUtils.toString(rrw["結案內容"]) == "")
                                    rew["回覆內容"] = DataTableUtils.toString(rrw["維護內容"]);
                                else
                                    rew["回覆內容"] = DataTableUtils.toString(rrw["結案內容"]);
                            }
                            ds.Rows.Add(rew);
                        }
                    }
                    //用一個空格去做區分
                    rew = ds.NewRow();
                    rew["刀庫編號"] = "";
                    rew["發起時間"] = "";
                    rew["發起人"] = "";
                    rew["發起內容"] = "";
                    rew["回覆時間"] = "";
                    rew["回覆人"] = "";
                    rew["回覆內容"] = "";
                    ds.Rows.Add(rew);
                }
            }

            ToExcel(ds);


        }


        public void ToExcel(DataTable dt)
        {
            int ct = dt.Rows.Count;

            using (XLWorkbook wb = new XLWorkbook())
            {

                wb.Worksheets.Add(dt, "分頁");


                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename={DateTime.Now:yyyyMMdd}.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

        }
    }
}