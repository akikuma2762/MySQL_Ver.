using dek_erpvis_v2.cls;
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
    public partial class Worktime_Total : System.Web.UI.Page
    {

        public string color = "";

        public string tr = "";
        public string th = "";
        string acc = "";

        myclass myclass = new myclass();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (myclass.user_view_check("Worktime_Total", acc) || HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]))
                {
                    if (!IsPostBack)
                        Set_Html_Table();
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
        }


        private void Set_Html_Table()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
            string sqlcmd = "SELECT RealRsrc as 機台名稱,StaffName as 人員姓名,workhour_detail.Project as 單號,Priority as 加工順序,workhour_detail.Task as 製程代碼,workhour_detail.taskname as 製程名稱 ,JobID as 品號,Job as 品名,Record_Time as 進站時間 FROM  workhour_detail  LEFT JOIN    workhour ON     workhour_detail.project = workhour.project        AND workhour_detail.Task = workhour_detail.Task        AND workhour_detail.RealRsrc = workhour.Resource  and workhour_detail.TaskName = workhour.TaskName   where now_status = '入站'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                dt.Columns["進站時間"].ColumnName = "進站時間(開始時間)";
                dt.Columns.Add("出站時間(結束時間)");
                dt.Columns.Add("生產時間(分)");
                dt.Columns.Add("完工數量");
                dt.Columns.Add("不良品數量");
                dt.Columns.Add("秒/PCS");
                dt.Columns.Add("除外時間(分)");

                //填入資料區
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisAps);
                foreach (DataRow row in dt.Rows)
                {
                    //先填入出站時間
                    sqlcmd = $"select Record_Time,piece from workhour_detail where now_status = '出站' and     project = '{row["單號"]}'        AND Task = '{row["製程代碼"]}'        AND RealRsrc = '{row["機台名稱"]}'        AND TaskName = '{row["製程名稱"]}' and Record_Time>='{row["進站時間(開始時間)"]}'";
                    DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(ds))
                    {
                        row["出站時間(結束時間)"] = ds.Rows[0]["Record_Time"].ToString();
                        row["完工數量"] = ds.Rows[0]["piece"].ToString();
                    }

                    //填入不良品
                    sqlcmd = $"select piece from workhour_detail where Status = 'Defective' and     project = '{row["單號"]}'        AND Task = '{row["製程代碼"]}'        AND RealRsrc = '{row["機台名稱"]}'  AND TaskName = '{row["製程名稱"]}' and Record_Time>='{row["進站時間(開始時間)"]}'";
                    ds = DataTableUtils.GetDataTable(sqlcmd);
                    int defective = 0;
                    if (HtmlUtil.Check_DataTable(ds))
                    {
                        foreach (DataRow rew in ds.Rows)
                            defective += DataTableUtils.toInt(DataTableUtils.toString(rew["piece"]));


                    }
                    row["不良品數量"] = defective.ToString();

                    //填入暫停~取消暫停的時間
                    sqlcmd = $"select Record_Time from workhour_detail where (now_status='暫停' OR now_status='取消暫停') and     project = '{row["單號"]}'        AND Task = '{row["製程代碼"]}'        AND RealRsrc = '{row["機台名稱"]}'        AND TaskName = '{row["製程名稱"]}' and Record_Time>='{row["進站時間(開始時間)"]}' order by Record_Time asc, id asc";
                    ds = DataTableUtils.GetDataTable(sqlcmd);
                    string start_end = "";
                    if (HtmlUtil.Check_DataTable(ds))
                    {
                        foreach (DataRow rew in ds.Rows)
                            start_end += rew["Record_Time"].ToString() + ",";
                    }
                    row["除外時間(分)"] = start_end;
                }

                string title = "";
                th = HtmlUtil.Set_Table_Title(dt, out title);
                tr = HtmlUtil.Set_Table_Content(dt, title, Worktime_TotalListback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string Worktime_TotalListback(DataRow row, string fieldname)
        {

            if (fieldname == "進站時間(開始時間)" || fieldname == "出站時間(結束時間)")
            {
                if (DataTableUtils.toString(row[fieldname]) != "")
                    return $"<td>{HtmlUtil.StrToDate(DataTableUtils.toString(row[fieldname])):yyyy/MM/dd HH:mm:ss}</td>";
                else return "";
            }
            else if (fieldname == "生產時間(分)")
            {
                if (DataTableUtils.toString(row["出站時間(結束時間)"]) != "" && DataTableUtils.toString(row["進站時間(開始時間)"]) != "")
                {

                    TimeSpan span = HtmlUtil.StrToDate(DataTableUtils.toString(row["出站時間(結束時間)"])) - HtmlUtil.StrToDate(DataTableUtils.toString(row["進站時間(開始時間)"]));
                    return $"<td>{span.TotalMinutes:0.00}</td>";
                }
                else return "";
            }
            else if (fieldname == "秒/PCS")
            {
                if (DataTableUtils.toString(row["出站時間(結束時間)"]) != "" && DataTableUtils.toString(row["進站時間(開始時間)"]) != "")
                {
                    TimeSpan span = HtmlUtil.StrToDate(DataTableUtils.toString(row["出站時間(結束時間)"])) - HtmlUtil.StrToDate(DataTableUtils.toString(row["進站時間(開始時間)"]));
                    return $"<td>{span.TotalSeconds}</td>";
                }
                else return "";
            }
            else if (fieldname == "除外時間(分)")
            {
                List<string> list = new List<string>(DataTableUtils.toString(row["除外時間(分)"]).Split(','));
                Double ex_time = 0;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if(list[i + 1] != "" && list[i] != "")
                    {
                        TimeSpan span = HtmlUtil.StrToDate(list[i + 1]) - HtmlUtil.StrToDate(list[i]);
                        ex_time += span.TotalMinutes;
                        i += 1;
                    }
                }
                return $"<td>{ex_time:0.00}</td>";
            }

            else
                return "";
        }
    }
}
