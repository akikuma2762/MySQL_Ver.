using dek_erpvis_v2.cls;
using dek_erpvis_v2.webservice;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dekERP_dll;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.pages.dp_PD
{
    public partial class materialrequirementplanning_details : System.Web.UI.Page
    {
        public string color = "";
        public string title_text = "";
        public string pie_data_points = "";
        public string col_data_points = "";
        public string for_what = "";
        public string th = "";
        public string tr = "";
        public string date_str = "";
        public string date_end = "";
        string item_code = "";
        string acc = "";
        public string start = "";
        public string end = "";
        iTec_Materials PCD = new iTec_Materials();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (acc != "")
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
                    Response.Redirect("materialrequirementplanning.aspx");
            }
            else
                Response.Redirect("materialrequirementplanning.aspx");
        }

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }
        private void load_page_data()
        {
            Response.BufferOutput = false;
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                item_code = HtmlUtil.Search_Dictionary(keyValues, "item_code");
                date_str = HtmlUtil.Search_Dictionary(keyValues, "date_str");
                date_end = HtmlUtil.Search_Dictionary(keyValues, "date_end");
                //儲存cookie
                Response.Cookies.Add(HtmlUtil.Save_Cookies("materialrequirementplanning", item_code));
                title_text = "         " + item_code;
                start = HtmlUtil.changetimeformat(date_str);
                end = HtmlUtil.changetimeformat(date_end);
                set_col_value();
                set_table_title();
            }
            else
                Response.Redirect("materialrequirementplanning.aspx");
        }
        private void set_col_value()
        {
            DataTable dt = PCD.materialrequirementplanning_Detail(item_code, date_str, date_end, dekModel.Image);
            if(HtmlUtil.Check_DataTable(dt))
            {
                int i = 0;
                DataTable use = dt.DefaultView.ToTable(true, new string[] { "用途說明" });
                DataTable month = dt.DefaultView.ToTable(true, new string[] { "領料月份" });
                int TotalOfPart = 0;
                string dataPoint = "";
                foreach (DataRow row in use.Rows)
                {
                    dataPoint = "";
                    TotalOfPart = 0;
                    foreach (DataRow rew in month.Rows)
                    {
                        if (DataTableUtils.toString(dt.Rows[i]["領料月份"]) == DataTableUtils.toString(rew["領料月份"]) && DataTableUtils.toString(dt.Rows[i]["用途說明"]) == DataTableUtils.toString(row["用途說明"]))
                        {
                            int val = DataTableUtils.toInt(DataTableUtils.toString(dt.Rows[i]["領料數"]).Split('.')[0]);
                            TotalOfPart += val;
                            dataPoint += "{ " +
                                            $"y: {val}, label: '{DataTableUtils.toString(rew["領料月份"])}' " +
                                         "},";
                        }
                        else
                            dataPoint += "{ " +
                                            $"y: {0}, label: '{DataTableUtils.toString(rew["領料月份"])}' " +
                                         "},";
                        i++;
                    }
                    col_data_points += "{ " +
                                          $"type: 'stackedColumn', showInLegend: true, name:'{DataTableUtils.toString(row["用途說明"])}', dataPoints: [{dataPoint}] " +
                                       "},";



                    
                    if (i == dt.Rows.Count)
                        pie_data_points += "{" +
                                             $"y:{TotalOfPart},name:'{DataTableUtils.toString(row["用途說明"])} {TotalOfPart}', label:'{DataTableUtils.toString(row["用途說明"])}',exploded: true" +
                                            "},";
                    else
                        pie_data_points += "{" +
                                             $"y:{TotalOfPart},name:'{DataTableUtils.toString(row["用途說明"])} {TotalOfPart}', label:'{DataTableUtils.toString(row["用途說明"])}'" +
                                           "},";
                }
            }
           
        }
        private void set_table_title()
        {
            DataTable dt = PCD.materialrequirementplanning_Detail(item_code, date_str, date_end, dekModel.Table);
            if (HtmlUtil.Check_DataTable(dt))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(dt, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));

                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, dt, order_list, materialrequirementplanning_details_callback).ToString();
            }
            else
                HtmlUtil.NoData(out th, out tr);

        }

        private string materialrequirementplanning_details_callback(DataRow row, string field_name)
        {
            string value = "";

            if (field_name == "領料單日期")
                value = HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]));
            else if (field_name == "領料數量")
                value = DataTableUtils.toString(row[field_name]).Split('.')[0];
            return $"<td>{value}</td>";
        }
    }
}
