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
    public partial class WorkHourDetail : System.Web.UI.Page
    {
        public string Super_Link = "";
        public string G_Order = "";
        public string O_Order = "";
        public string T_Order = "";
        public string N_Order = "";
        public string color = "";
        string acc = "";
        public string get_js = "";
        public string th = "";
        string right = "";
        public string tr = "";
        myclass myclass = new myclass();
        //------------------------------------------------------------事件----------------------------------------------------------
        //網頁載入事件
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (!IsPostBack)
                {
                    Read_Data();
                    Super_Link = "<ol class=\"breadcrumb_\">" +
                                     "<li><u><a href=\"../index.aspx\">首頁 </a></u></li> " +
                                     "<li><u><a href=\"../dp_APS/OrderList.aspx\"> 報工清單 </a></u></li> " +
                                     $"<li><u><a href=\"../dp_APS/WorkHourList.aspx?key={WebUtils.UrlStringEncode($"OrderNum = {O_Order}")}\"> 報工列表 </a></u></li> " +
                                     "<li>報工明細</li> " +
                                 "</ol>";
                }
            }
            else Response.Redirect(myclass.logout_url);
        }

        //刪除該牌程(還沒做)
        protected void button_delete_Click(object sender, EventArgs e)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            string Condition = $"id = '{TextBox_textTemp.Text.Split('_')[1]}'"; ;
            DataRow dr = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLWorkHour_Detail, Condition);

            if (dr != null)
            {
                if (DataTableUtils.Delete_Record(ShareMemory.SQLWorkHour_Detail, $"id = '{TextBox_textTemp.Text.Split('_')[1]}'"))
                {
                    UpdataWorkHour(dr);
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('刪除成功');location.href='WorkHourDetail.aspx{Request.Url.Query}';</script>");
                }
            }
        }
        //-------------------------------------------------------------方法--------------------------------------------------------
        //讀取相關資料
        private void UpdataWorkHour(DataRow dr)
        {
            string Condition = $"Project='{dr["Project"]}' AND TaskName='{dr["TaskName"]}'";
            DataRow dr_WorkHour = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLWorkHour, Condition);
            DataTable dt_detail = DataTableUtils.DataTable_GetTable(ShareMemory.SQLWorkHour_Detail, Condition);
            int Count = (from s in dt_detail.AsEnumerable()
                         select new
                         {
                             piece = DataTableUtils.toInt(s.Field<string>("Piece"))
                         }).Select(w => w.piece).Sum();
            dr_WorkHour["Status"] = dt_detail.AsEnumerable().OrderByDescending(o => o.Field<string>("ID")).Select(s => s.Field<string>("Status")).FirstOrDefault();
            dr_WorkHour["CurrentPiece"] = Count.ToString();
            bool OK = DataTableUtils.Update_DataRow(ShareMemory.SQLWorkHour, Condition, dr_WorkHour);
        }
        private void Read_Data()
        {
            DataRow dr_Job;
            string condition = "";
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);

                G_Order = HtmlUtil.Search_Dictionary(keyValues, "WorkHourID");
                O_Order = HtmlUtil.Search_Dictionary(keyValues, "Project");
                T_Order = HtmlUtil.Search_Dictionary(keyValues, "TaskName");
                condition = $"ID='{G_Order}'";
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);

                dr_Job = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLWorkHour, condition);
                if (dr_Job != null)
                    N_Order = dr_Job["Job"].ToString();
                Get_Datatable();
            }
            else
                Response.Redirect("OrderList.aspx");
        }
        //載入表格
        private void Get_Datatable()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            string sqlcmd = "select id, " +
                            "staffname as 人員, " +
                            "piece as 數量," +
                            "status as 狀態," +
                            "Record_Time as 時間 " +
                            $"FROM {ShareMemory.SQLWorkHour_Detail}" + 
                            $" WHERE project = '{O_Order}' and " +
                            $"WorkHourID = '{G_Order}' and " +
                            $"TaskName = '{T_Order}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                if (ShareFunction_APS.check_right(acc) == "")
                {
                    dt.Columns.Add("編輯明細");
                    dt.Columns.Add("刪除明細");
                }
                List<string> list = new List<string>();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    th += $"<th>{dt.Columns[i]}</th>";
                    list.Add(DataTableUtils.toString(dt.Columns[i]));
                }
                dt.Columns.Add(" ");
                list.Add(" ");
                tr = HtmlUtil.Set_Table_Content(dt, list, WorkHourDetail_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        //callback事件
        private string WorkHourDetail_callback(DataRow row, string field_name)
        {
            string value = "";
            DataRow dr_Status;
            right = ShareFunction_APS.check_right(acc);
            if (field_name == "編輯明細")
            {
                if (right == "")
                    value = $"<td style=\"width:7%\"><u><a href=\"WorkHourDetailEdit.aspx?key={WebUtils.UrlStringEncode($"detailID ={ DataTableUtils.toString(row["id"])}")}\" >編輯</a></u></td>";
            }
           else if (field_name == "刪除明細")
            {
                if (right == "")
                    value = $"<td style=\"width:7%\"><u><a href=\"javascript:void(0)\" id=de_{DataTableUtils.toString(row["id"])} onclick=get_id(\"de_{DataTableUtils.toString(row["id"])}\")>刪除</a></u></td>";
            }
            else if(field_name == "時間")
            {
                if (DataTableUtils.toString(row[field_name]) != "")
                    value = $"<td>{HtmlUtil.StrToDate(DataTableUtils.toString(row[field_name])):yyyy/MM/dd HH:mm:ss}</td>";
            }
            else if (field_name == "狀態")
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                dr_Status = DataTableUtils.DataTable_GetDataRow(ShareMemory.SQLWorkHour_MachStatus, $"StatusEn='{row[field_name]}'");
                if (dr_Status != null)
                    value = $"<td>{DataTableUtils.toString(dr_Status["Status"])}</td>";
            }
            return value;
        }

    }
}