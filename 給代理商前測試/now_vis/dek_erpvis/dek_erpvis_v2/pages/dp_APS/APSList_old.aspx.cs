﻿using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_APS
{
    public partial class APSList_old : System.Web.UI.Page
    {
        public string color = "";
        string acc = "";
        public string th = "";
        public string tr = "";
        public string title = "";
        public string content = "";
        myclass myclass = new myclass();
        //網頁載入事件
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                if (string.IsNullOrEmpty(TextBox_Start.Text))
                {
                    TextBox_Start.Text = DateTime.Now.ToString("yyyy-MM-dd");// "2019-01-01";
                    TextBox_End.Text = DateTime.Now.ToString("yyyy-MM-dd"); ;
                }

                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);

                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc) )
                {
                    if (!IsPostBack)
                    {
                        getdata(TextBox_Start.Text, TextBox_End.Text);
                        getdata(TextBox_Start.Text, TextBox_End.Text, true);
                    }
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");

            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //按鈕查詢事件()
        protected void Button_Search_Click(object sender, EventArgs e)
        {
            getdata(TextBox_Start.Text, TextBox_End.Text);
            getdata(TextBox_Start.Text, TextBox_End.Text, true);
        }
        //印出表格欄位以及內容(type 詳細內容/簡短內容)
        private void getdata(string start, string end, bool ok = false)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            double Start_Time = DataTableUtils.toDouble(start.Replace("-", "") + "000000");
            double End_Time = DataTableUtils.toDouble(end.Replace("-", "") + "235959");
            string sql_cmd = "";
            DataTable dt = new DataTable();
            if (!ok)
            {
                sql_cmd = "SELECT  " +
                                   "    a.Status as 狀態, " +
                                   "    a.Resource as 機台名稱, " +
                                   "    a.Project as 送料單號, " +
                                   "    a.Job as 品名規格, " +
                                   "    a.TaskName as 工藝名稱, " +
                                   "    a.StartTime as 起始時間, " +
                                   "    a.EndTime as 結束時間, " +
                                   "    a.CurrentPiece as 累積數量, " +
                                   "    a.TargetPiece as 需求總數量 " +
                                   $"    FROM {ShareMemory.SQLWorkHour} as a " +
                                   $"    left join {ShareMemory.SQLWorkHour_Order} as b on a.Project = b.Project " +
                                   $"    where  ((b.StartTime >= '{Start_Time}' and b.StartTime <= '{End_Time}') or " +
                                   $"           (b.EndTime >= '{Start_Time}' and b.EndTime <= '{End_Time}') ) ";

                dt = DataTableUtils.GetDataTable(sql_cmd);
                DataTable dt_HourWork = DataTableUtils.DataTable_GetTable(ShareMemory.SQLWorkHour, "");
                if (HtmlUtil.Check_DataTable(dt))
                {
                    List<string> list = new List<string>();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        th += $"<th align=\"center\">{dt.Columns[i]}</th>";
                        list.Add(DataTableUtils.toString(dt.Columns[i]));
                    }
                    list.Add(" ");
                    tr = HtmlUtil.Set_Table_Content(dt, list, aps_callback);
                }
                else
                    HtmlUtil.NoData(out th, out tr);


            }
            else if (ok)
            {
                sql_cmd = "SELECT distinct " +
                                "    a.Project as 送料單號, " +
                                "    a.Job as 品名規格, " +
                                "    a.CurrentPiece as 累積數量, " +
                                "    b.StartTime as 起始時間, " +
                                "    b.EndTime as 結束時間 " +
                                $"    FROM {ShareMemory.SQLWorkHour} as a " +
                                $"    left join {ShareMemory.SQLWorkHour_Order} as b on a.Project = b.Project " +
                                $"    where  ((b.StartTime >= '{Start_Time}' and b.StartTime <= '{End_Time}') or " +
                                $"           (b.EndTime >= '{Start_Time}' and b.EndTime <= '{End_Time}') )";
                dt = DataTableUtils.GetDataTable(sql_cmd);
                if (HtmlUtil.Check_DataTable(dt))
                {
                    dt.Columns.Add("明細");
                    List<string> list = new List<string>();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        title += $"<th align=\"center\">{dt.Columns[i]}</th>";
                        list.Add(DataTableUtils.toString(dt.Columns[i]));
                    }
                    list.Add(" ");

                    content = HtmlUtil.Set_Table_Content(dt, list, aps_callback);
                }
                else
                    HtmlUtil.NoData(out title, out content);
            }
        }
        //callback執行的地方
        private string aps_callback(DataRow row, string field_name)
        {
            string value = "";
            string Status = "";
            if (field_name == "狀態")
            {
                if (string.IsNullOrEmpty(DataTableUtils.toString(row["狀態"])))
                    Status = "READY";
                else
                    Status = DataTableUtils.toString(row["狀態"]);
                value = $"<img class=\"img-rounded\" src=\"../../assets/images/{Status}.PNG\" alt=\"...\" style=\"width:35px;height:35px;align-items:center;\">";
                value = $"<td  style=\"width:1%\">{value}</td>";
            }
            else if (field_name == "累積數量")
            {
                if (string.IsNullOrEmpty(DataTableUtils.toString(row[field_name])))
                    value = "<td  style=\"width:1%\">0</td>";
                else
                    value = $"<td  style=\"width:1%\">{DataTableUtils.toString(row[field_name])}</td>";
            }

            else if (field_name == "起始時間" || field_name == "結束時間")
            {
                DateTime dt = DateTime.ParseExact(DataTableUtils.toString(row[field_name]), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                value = dt.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture) + "&nbsp " + dt.ToString("HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                value = $"<td  style=\"width:15%\">{value}</td>";
            }
            else if (field_name == "明細")
                value = $"<td  style=\"width:10%\"><a href=\"javascript:void(0)\" id={DataTableUtils.toString(row["送料單號"])} onclick=jump(\"{WebUtils.UrlStringEncode("Project=" + DataTableUtils.toString(row["送料單號"]))}\")><u>明細</u></a></td>";
            else
                value = $"<td  style=\"width:15%\">{row[field_name]}</td>";

            if (value == "")
                return "";
            else
                return value;
        }

    }
}