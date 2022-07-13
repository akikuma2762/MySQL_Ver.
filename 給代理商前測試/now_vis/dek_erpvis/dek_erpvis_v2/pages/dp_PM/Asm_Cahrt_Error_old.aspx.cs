using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using dek_erpvis_v2.cls;
using System.Data;
using Support;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class Asm_Cahrt_Error : System.Web.UI.Page
    {
        public string y_value = "次數";
        public string color = "";
        public string ChartData_Count = "";
        public string ChartData_Time = "";
        public string date_str = "";// new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyyMMdd");
        public string date_end = "";//
        public string timetype = "";
        public string ColumnsData = "";
        public string RowsData = "";
        public string SelectLine = "'ALL";
        public string chartName = "";
        public string chartName_Single = "";
        List<string> list = new List<string>();
        public string Choose_Line = "所有產線";
        public string LineNum = "";
        public string AxisSet = "";
        public string LineStatus = "";
        public string TimeTypeForSubTitle = DateTime.Now.ToString("yyyy-MM-dd");
        public string LineNameForSubTitle = "所有產線";
        public string time_area_text = "";
        public string TopStrForPieceCount = "0";
        public string TopStrForPieceTimes = "0";
        public string PieceUnit = ShareMemory.PieceUnit;
        public string TimeUnit = "";
        clsDB_Server clsDB_sw = new clsDB_Server("");
        ShareFunction SFun = new ShareFunction();
        myclass myclass = new myclass();
        HttpCookie userOpRec = new HttpCookie("Rec");// Request.Cookies["userOpRec"];
        string acc = "";
        德大機械 德大機械 = new 德大機械();

        private void GotoCenn()
        {
            clsDB_sw.dbOpen(SFun.GetConnByDekdekVisAssmHor);
            DataTableUtils.Conn_String = SFun.GetConnByDekdekVisAssmHor;
            if (clsDB_sw.IsConnected == true)
                load_page_data();
            else
            {
                Response.Write("<script language='javascript'>alert('伺服器回應 : 無法載入資料，" + clsDB_sw.ErrorMessage + " 請聯絡德科人員或檢查您的網路連線。');</script>");
                HtmlUtil.NoData(out ColumnsData, out RowsData);
            }
        }
        private void load_page_data()
        {
            if (GlobalVar.Conn_status == true) //資料庫連線成功要做的事()
                LoadData();
            else   //資料庫連線失敗要做的事()
            {
                Response.Write("<script language='javascript'>alert('伺服器回應 : 無法載入資料，" + " 請聯絡德科人員或檢查您的網路連線。');</script>");
                HtmlUtil.NoData(out ColumnsData, out RowsData);
            }

        }
        private void LoadData()
        {
            GetErrorData();
        }

        private void GetErrorData()
        {
            HttpCookie userOpRec = Request.Cookies["Rec"];
            string[] timeSet;
            string timeTypeSource = "";
            if (userOpRec["user_TimeType"] != null || userOpRec["user_TimeType"].ToString() != "")
                timeTypeSource = SFun.TrsDate(userOpRec["user_TimeType"].ToString());
            SelectLine = userOpRec["user_LineNum"].ToString();
            if (!string.IsNullOrEmpty(timeTypeSource))//timeTypeSource != null && timeTypeSource != "")
            {
                //SelectLine = "1";
                timeSet = SFun.GetTimeType(timeTypeSource);
                userOpRec["user_StartTime"] = timeSet[0];
                userOpRec["user_EndTime"] = timeSet[1];
                Response.Cookies.Add(userOpRec);
                SetDropDownList();
                if (SelectLine == "0" || string.IsNullOrEmpty(SelectLine))
                    GetDataInf(timeTypeSource, timeSet);
                else
                    GetDataInf(timeTypeSource, timeSet, SelectLine);
                TimeTypeForSubTitle = timeSet[4];
                if (Session["time_s"] != null && Session["time_e"] != null)
                {
                    TimeTypeForSubTitle = HtmlUtil.changetimeformat(Session["time_s"].ToString().Replace("010101", "")) + "~" + HtmlUtil.changetimeformat(Session["time_e"].ToString().Replace("235959", ""));
                    Session.Remove("time_s");
                    Session.Remove("time_e");

                }
            }
            else
            {
                if (SelectLine == "0" || SelectLine == null || SelectLine == "")
                    GetDataInf(timeTypeSource, null);
                else //First  -> show Today
                {
                    timeSet = SFun.GetTimeType("day");
                    GetDataInf("day", timeSet, SelectLine);
                    TimeTypeForSubTitle = timeSet[4];
                    if (Session["time_s"] != null && Session["time_e"] != null)
                    {
                        TimeTypeForSubTitle = Session["time_s"].ToString() + "-" + Session["time_e"].ToString();
                        Session.Remove("time_s");
                        Session.Remove("time_e");
                    }

                }
            }
        }
        private void GetDataInf(string TimeType, string[] timeset, string SelectLine = "0")
        {
            string time_st;
            string time_ed;
            string LineStr = "";
            if (list.Count == 0)
            {
                list.Add("0");
                Choose_Line = "所有產線";
            }

            if (Session["time_s"] != null && Session["time_e"] != null)
            {
                time_st = Session["time_s"].ToString();
                time_ed = Session["time_e"].ToString();
            }
            else
            {
                time_st = timeset[0];
                time_ed = timeset[1];
            }
            string[] StrInf;
            if (timeset != null && SelectLine == "0")
                StrInf = SFun.GetErrorInf(CheckBoxList_Line, TimeUnit, TimeType, time_st, time_ed, list);
            else if (timeset != null && SelectLine != null)
                StrInf = SFun.GetErrorInf(CheckBoxList_Line, TimeUnit, TimeType, time_st, time_ed, list);
            else if (timeset == null && SelectLine == "0")
                StrInf = SFun.GetErrorInf(CheckBoxList_Line, TimeUnit, TimeType, "Today", "Today", list);
            else
                StrInf = SFun.GetErrorInf(CheckBoxList_Line, TimeUnit, TimeType, "Today", "Today", list);
            ChartData_Count = "";
            if (StrInf.Length != 0 && StrInf[4] != null)
            {

                ChartData_Count = StrInf[0];
                ChartData_Count = ChartData_Count.Replace("\n", "");
                ChartData_Count = ChartData_Count.Replace("indexLabel", "label");

                time_area_text = textbox_dt1.Text.Replace('-', '/') + "~" + textbox_dt2.Text.Replace('-', '/');
                RowsData = StrInf[3];
                TopStrForPieceCount = StrInf[4];
                TopStrForPieceTimes = StrInf[5];
            }
            else
                HtmlUtil.NoData(out ColumnsData, out RowsData);
        }

        private void SetDropDownList()
        {
            CheckBoxList_Line.DataTextField = "LineName";//default show Text
            CheckBoxList_Line.DataValueField = "LineID";
            CheckBoxList_Line.DataSource = SFun.GetLineList();
            CheckBoxList_Line.DataBind();
            //Creat Table Column
            ColumnsData = SFun.GetCharstColumnName_Error("0", CheckBoxList_Line, "Asm_Cahrt_Error");
            if (RowsData == "<tr class=\"even gradeX\"> <td class=\"center\"> no data </td></tr>")
                ColumnsData = "<th class=\"center\">沒有資料載入</th>";
            //add all
            CheckBoxList_Line.Items.Insert(0, new ListItem("全部", "0"));

        }
        public string GetPieceForSeries(int NumTh, string Type)
        {
            string[] TopPiece;
            HttpCookie userOpRec = Request.Cookies["Rec"];
            if (TopStrForPieceTimes != null && TopStrForPieceCount != null)
            {
                if (Type == "Count")
                    TopPiece = TopStrForPieceCount.Split(',');
                else
                    TopPiece = TopStrForPieceTimes.Split(',');
                //
                if (TopPiece.Length >= NumTh)
                {
                    if (TopPiece[NumTh - 1] != "" && TopPiece[NumTh - 1] != null)
                    {
                        if (Type == "Count")
                            return TopPiece[NumTh - 1];
                        else
                            return SFun.TrsTime(TopPiece[NumTh - 1], userOpRec["user_unit"].ToString());
                    }
                    else
                        return "0";
                }
                else
                    return "0";
            }
            else
                return "0";
        }

        //--------------------------------------Event--------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {

            string CompLoacation = "";
            string acc = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);

                string URL_NAME = "Asm_Cahrt_Error";
                color = HtmlUtil.change_color(acc);
                string ped = DataTableUtils.toString(userInfo["user_PWD"]);

                CompLoacation = ShareFunction.Last_Place(acc);
                while (CompLoacation == "")
                {
                    CompLoacation = ShareFunction.Last_Place(acc);
                    if (CompLoacation != "")
                        break;
                }
                ShareFunction.Last_Place(acc, CompLoacation);
                if (myclass.user_view_check(URL_NAME, acc) || HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                {
                    //可以進入 -> 執行後面程式碼
                    if (HtmlUtil.check_login(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                    {
                        if (!IsPostBack)
                        {
                            if (textbox_dt1.Text == "" && textbox_dt2.Text == "")
                            {
                                string[] daterange = 德大機械.德大專用月份(acc).Split(',');
                                textbox_dt1.Text = HtmlUtil.changetimeformat(daterange[0], "-");
                                textbox_dt2.Text = HtmlUtil.changetimeformat(daterange[1], "-");
                            }

                            userOpRec["user_unit"] = ShareMemory.TimeUnit;
                            userOpRec["user_TimeType"] = "type_month";
                            userOpRec["user_LineNum"] = "0";
                            userOpRec["user_StartTime"] = DateTime.Now.ToString("yyyyMM" + "01010101");
                            userOpRec["user_EndTime"] = DateTime.Now.ToString("yyyyMM" + "30235959");
                            userOpRec.Expires = DateTime.Now.AddDays(1);
                            Response.Cookies.Add(userOpRec);

                            SetDropDownList();
                            SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;
                            GotoCenn();
                        }
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

        protected void LinkButton_day_Click(object sender, EventArgs e)
        {
            string dt_st = "";
            string dt_ed = "";
            HttpCookie userInfo = Request.Cookies["userInfo"];
            acc = DataTableUtils.toString(userInfo["user_ACC"]);
            string[] daterange = 德大機械.德大專用月份(acc).Split(',');
            HtmlUtil.Button_Click(DataTableUtils.toString(((Control)sender).ID.Split('_')[1]), daterange, DataTableUtils.toString(textbox_dt1.Text), DataTableUtils.toString(textbox_dt2.Text), out dt_st, out dt_ed);
            textbox_dt1.Text = HtmlUtil.changetimeformat(dt_st, "-");
            textbox_dt2.Text = HtmlUtil.changetimeformat(dt_ed, "-");
        }
        protected void button_select_Click(object sender, EventArgs e)
        {
            string[] timeSet = new string[2];
            string LineSelect = "";
            //
            Choose_Line = "";
            list.Clear();
            for (int i = 0; i < CheckBoxList_Line.Items.Count; i++)
            {
                if (CheckBoxList_Line.Items[i].Selected)
                {
                    list.Add(CheckBoxList_Line.Items[i].Value);
                    Choose_Line += CheckBoxList_Line.Items[i].Text + "、";

                }
            }
            foreach (string line in list)
            {
                if (list.IndexOf(line) == list.Count - 1)
                    LineSelect += line;
                else
                    LineSelect += line + "、";
            }
            //
            HttpCookie userOpRec = Request.Cookies["Rec"];
            if (textbox_dt1.Text != "")
            {
                timeSet[0] = textbox_dt1.Text.Replace("-", String.Empty) + "010101";//add mmhhss           
                timeSet[1] = textbox_dt2.Text.Replace("-", String.Empty) + "235959";//
                Session["time_s"] = timeSet[0];
                Session["time_e"] = timeSet[1];
            }
            else
            {
                timeSet[0] = textbox_dt1.Text.Replace("-", String.Empty) + "010101";//add mmhhss           
                timeSet[1] = textbox_dt2.Text.Replace("-", String.Empty) + "235959";//
                Session["time_s"] = timeSet[0];
                Session["time_e"] = timeSet[1];
            }



            SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;

            Session["list"] = list;
            Session["line"] = Choose_Line;

            GotoCenn();
        }


        protected void btn_cbx_Click(object sender, EventArgs e)
        {
            SFun.GetConnByDekVisTmp = SFun.GetConnByDekdekVisAssmHor;
            GotoCenn();

        }
    }
}