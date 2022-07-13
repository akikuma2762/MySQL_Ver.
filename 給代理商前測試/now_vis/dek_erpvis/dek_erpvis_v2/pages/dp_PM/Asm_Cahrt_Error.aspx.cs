using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class Asm_Cahrt_Error_new : System.Web.UI.Page
    {

        public string color = "";
        public StringBuilder th = new StringBuilder();
        public StringBuilder tr = new StringBuilder();
        public string date_str = "";
        public string date_end = "";
        public string path = "";
        public string time_area_text = "";
        public string col_data_point = "";
        string acc = "";
        myclass myclass = new myclass();
        DataTable dt_monthtotal = new DataTable();
        int count = 0;
        List<string> linelist = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);

                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.check_power(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]) || myclass.user_view_check(System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc))
                {
                    if (!IsPostBack)
                    {
                        string[] date = 德大機械.德大專用月份(acc).Split(',');
                        textbox_dt1.Text = HtmlUtil.changetimeformat(date[0], "-");
                        textbox_dt2.Text = HtmlUtil.changetimeformat(date[1], "-");
                        MainProcess();
                    }
                }
                else
                    Response.Write("<script>alert('您無此權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }


        protected void button_select_Click(object sender, EventArgs e)
        {
            MainProcess();
        }

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }

        private void MainProcess()
        {
            Set_CheckBox();
            Get_MonthTotal();
            Set_Chart();
            Set_Table();
        }

        private void Set_CheckBox()
        {
            if (CheckBoxList_Line.Items.Count == 0)
            {
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
                string sqlcmd = "select distinct 工作站編號,工作站名稱 from 工作站型態資料表 where 工作站是否使用中 = '1'";
                DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

                if (HtmlUtil.Check_DataTable(dt))
                {
                    ListItem list = new ListItem("全部", "all");
                    list.Selected = true;
                    CheckBoxList_Line.Items.Add(list);
                    foreach (DataRow row in dt.Rows)
                    {
                        list = new ListItem(row["工作站名稱"].ToString(), row["工作站編號"].ToString());
                        list.Selected = true;
                        CheckBoxList_Line.Items.Add(list);
                    }
                }
            }
        }

        private void Get_MonthTotal()
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string condition = "";
            for (int i = 1; i < CheckBoxList_Line.Items.Count; i++)
                if (CheckBoxList_Line.Items[i].Selected)
                    condition += condition == "" ? $" 工作站編號='{CheckBoxList_Line.Items[i].Value}' " : $" or 工作站編號='{CheckBoxList_Line.Items[i].Value}' ";
            condition = condition == "" ? "" : $" and ( {condition} ) ";

            string sqlcmd = $"select * from ( SELECT  工作站異常維護資料表.工作站編號,    工作站名稱,    排程編號,    (select max(時間紀錄) from 工作站異常維護資料表 a where a.父編號 = 工作站異常維護資料表.異常維護編號 and 結案判定類型 IS NOT NULL ) 結案時間,     (select 結案判定類型 from 工作站異常維護資料表 a where a.父編號 = 工作站異常維護資料表.異常維護編號 and 結案判定類型 IS NOT NULL and 結案時間 = a.時間紀錄 ) 異常類型,    '1' 次數 FROM     工作站異常維護資料表,工作站型態資料表     where 工作站異常維護資料表.工作站編號 = 工作站型態資料表.工作站編號     and (父編號 IS NULL OR 父編號 = 0) ) a     where a.異常類型 IS NOT NULL and  {textbox_dt1.Text.Replace("-", "")} <=substring(結案時間,1,8) and substring(結案時間,1,8) <= {textbox_dt2.Text.Replace("-", "")}    {condition} ";
            dt_monthtotal = DataTableUtils.GetDataTable(sqlcmd);
        }
        private void Set_Chart()
        {
            DataTable dt_copy = HtmlUtil.PrintChart_DataTable(dt_monthtotal, "異常類型", "次數");
            col_data_point = HtmlUtil.Set_Chart(dt_copy, "異常類型", "次數", "", out count);
        }
        private void Set_Table()
        {
            if (HtmlUtil.Check_DataTable(dt_monthtotal))
            {
                DataTable Line = dt_monthtotal.DefaultView.ToTable(true, new string[] { "工作站名稱" });
                DataTable type = dt_monthtotal.DefaultView.ToTable(true, new string[] { "異常類型" });

                //新增產線
                foreach (DataRow row in Line.Rows)
                {
                    type.Columns.Add(DataTableUtils.toString(row["工作站名稱"]));
                    linelist.Add(DataTableUtils.toString(row["工作站名稱"]));
                }

                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(type, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));
                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"");
                tr = HtmlUtil.Set_Table_Content(true, type, order_list, Asm_Cahrt_Error_callback);
            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string Asm_Cahrt_Error_callback(DataRow row, string field_name)
        {
            string value = "";

            if (linelist.IndexOf(field_name) != -1)
            {
                string sqlcmd = $"工作站名稱='{field_name}' and 異常類型='{row["異常類型"]}'";
                DataRow[] rows = dt_monthtotal.Select(sqlcmd);

                if (rows != null && rows.Length > 0)
                {
                    string Number = "";
                    for (int i = 0; i < rows.Length; i++)
                        Number += $"{rows[i]["排程編號"]}$";
                    string url = $"Local=Hor,ErrorLineNum={rows[0]["工作站編號"]},Errorkey={Number},Date_str={textbox_dt1.Text.Replace("-", "")},Date_end={textbox_dt2.Text.Replace("-", "")},ErrorType={row["異常類型"]}";
                    url = $"Asm_history.aspx?key={WebUtils.UrlStringEncode(url)}";
                    value = $"<u><a href=\"{url}\"> {rows.Length} </a></u>";
                }
                else
                    value = "0";
            }
            return value == "" ? "" : $"<td style=\"vertical-align: middle; text-align: center;\">{value}</td>";
        }

    }
}