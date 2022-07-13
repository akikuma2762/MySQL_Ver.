using dek_erpvis_v2.cls;
using dekERP_dll.dekErp;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dek_erpvis_v2.pages.dp_PM
{
    public partial class waitingfortheproduction_details_Ver2 : System.Web.UI.Page
    {
        iTec_Product PMD = new iTec_Product();

        myclass myclass = new myclass();
        ShareFunction sFun = new ShareFunction();
        string acc = "";
        public string color = "";
        public string th = "";
        public string tr = "";
        public string status = "未結案";
        public string cust_sname = "";
        public string date_str = "";
        public string date_end = "";
        public string Type = "";
        public string LineNumber = "";

        //本月應生產之數量
        DataTable dt_本月應生產 = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
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
                Response.Redirect("waitingfortheproduction.aspx", false);
        }

        protected void Button_SaveColumns_Click(object sender, EventArgs e)
        {
            HtmlUtil.Save_Columns(TextBox_SaveColumn.Text, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0], acc);
        }


        private void GotoCenn()
        {
            if (Request.QueryString["key"] != null)
            {
                Dictionary<string, string> keyValues = HtmlUtil.Return_dictionary(Request.QueryString["key"]);
                cust_sname = HtmlUtil.Search_Dictionary(keyValues, "cust_name");
                date_str = HtmlUtil.Search_Dictionary(keyValues, "date_str");
                date_end = HtmlUtil.Search_Dictionary(keyValues, "date_end");
                Type = HtmlUtil.Search_Dictionary(keyValues, "Type");
                LineNumber = HtmlUtil.Search_Dictionary(keyValues, "LineNumber");
                Get_Schdule(date_str, date_end);
                Set_Table();
            }
            else
                Response.Redirect("waitingfortheproduction.aspx", false);
        }

        private void Set_Table()
        {
            string sqlcmd = "";
            DataRow[] row = null;

            //已上線
            if (Type == "0")
            {
                status = "已上線";
                sqlcmd = $"客戶簡稱='{cust_sname}' and (狀態 IS NULL OR 狀態<>'完成') and 產線群組='{LineNumber}' and 預計完工日='{DateTime.Now.ToString("yyyyMMdd")}'";

            }
            //未如期下架
            else if (Type == "1")
            {
                status = "未如期下架";
                sqlcmd = $"客戶簡稱='{cust_sname}' and (狀態 IS NULL OR 狀態<>'完成') and 產線群組='{LineNumber}' and 預計完工日<'{DateTime.Now.ToString("yyyyMMdd")}'";

            }
            //未上線
            else if (Type == "2")
            {
                status = "未上線";
                sqlcmd = $"客戶簡稱='{cust_sname}' and (狀態 IS NULL OR 狀態<>'完成') and 產線群組='{LineNumber}' and 預計完工日>'{DateTime.Now.ToString("yyyyMMdd")}'";
            }
            //全部
            else if (Type == "3")
            {
                status = "未結案";
                sqlcmd = $"客戶簡稱='{cust_sname}' and (狀態 IS NULL OR 狀態<>'完成') and 產線群組='{LineNumber}'";
            }
            //全產線
            else
            {
                status = "未結案";
                sqlcmd = $"客戶簡稱='{cust_sname}' and (狀態 IS NULL OR 狀態<>'完成')";
            }

            row = dt_本月應生產.Select(sqlcmd);
            DataTable ds = dt_本月應生產.Clone();
            for (int i = 0; i < row.Length; i++)
                ds.ImportRow(row[i]);

            if (HtmlUtil.Check_DataTable(ds))
            {
                List<string> order_list = HtmlUtil.Comparison_ColumnOrder(ds, HtmlUtil.Get_ColumnsList(acc, System.IO.Path.GetFileName(Request.PhysicalPath).Split('.')[0]));


                th = HtmlUtil.Set_Table_Title(order_list, "style=\"vertical-align: middle; text-align: center;\"").ToString();
                tr = HtmlUtil.Set_Table_Content(true, ds, order_list, waitingfortheproduction_details_callback).ToString();


            }
            else
                HtmlUtil.NoData(out th, out tr);
        }
        private string waitingfortheproduction_details_callback(DataRow row ,string field_name)
        {
            string value = "";
            if (field_name == "預計完工日")
                value = HtmlUtil.changetimeformat(DataTableUtils.toString(row[field_name]));
            return value == "" ? "" : $"<td>{value}</td>";
        }
        private void Get_Schdule(string date_str, string date_end)
        {


            //取得預計開工日 OR 預計完工日若在本月的
            DataTable dt_預生產數量 = PMD.waitingfortheproduction(date_str, date_end);
            if (HtmlUtil.Check_DataTable(dt_預生產數量))
                dt_預生產數量 = sFun.Get_Imformation(dt_預生產數量, date_str, date_end, 1);
            DataTable dt_未完工列表 = new DataTable();
            DataTable dt_未如期下架 = new DataTable();
            if (DataTableUtils.toInt(DateTime.Now.ToString("yyyyMMdd")) >= DataTableUtils.toInt(date_str))
            {
                //未如期下架數量

                dt_未如期下架 = PMD.waitingfortheproduction(date_str, "");
                if (HtmlUtil.Check_DataTable(dt_未如期下架))
                    dt_未如期下架 = sFun.Get_Imformation(dt_未如期下架, date_str, date_end, 2);
                //未完成之數量

                dt_未完工列表 = PMD.waitingfortheproduction(date_str, "");
                if (HtmlUtil.Check_DataTable(dt_未完工列表))
                    dt_未完工列表 = sFun.Get_Imformation(dt_未完工列表, date_str, date_end, 3);
            }



            if (HtmlUtil.Check_DataTable(dt_預生產數量))
            {
                dt_本月應生產 = dt_預生產數量.Clone();
                dt_預生產數量.PrimaryKey = new DataColumn[] { dt_預生產數量.Columns["排程編號"] };
                dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"] };
                dt_本月應生產.Merge(dt_預生產數量);
            }
            if (HtmlUtil.Check_DataTable(dt_未如期下架))
            {
                if (HtmlUtil.Check_DataTable(dt_本月應生產))
                {
                    dt_未如期下架.PrimaryKey = new DataColumn[] { dt_未如期下架.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未如期下架, true, MissingSchemaAction.Ignore);
                }
                else
                {
                    dt_本月應生產 = dt_未如期下架.Clone();
                    dt_未如期下架.PrimaryKey = new DataColumn[] { dt_未如期下架.Columns["排程編號"] };
                    dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未如期下架);
                }
            }
            if (HtmlUtil.Check_DataTable(dt_未完工列表))
            {
                if (HtmlUtil.Check_DataTable(dt_本月應生產))
                {
                    dt_未完工列表.PrimaryKey = new DataColumn[] { dt_未完工列表.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未完工列表, true, MissingSchemaAction.Ignore);
                }
                else
                {
                    dt_本月應生產 = dt_未完工列表.Clone();
                    dt_未完工列表.PrimaryKey = new DataColumn[] { dt_未完工列表.Columns["排程編號"] };
                    dt_本月應生產.PrimaryKey = new DataColumn[] { dt_本月應生產.Columns["排程編號"] };
                    dt_本月應生產.Merge(dt_未完工列表);
                }

            }


            //依據產線作排序
            DataView dv_mant = new DataView(dt_本月應生產);
            dv_mant.Sort = "產線代號 asc";
            dt_本月應生產 = dv_mant.ToTable();
            //加入產線名稱
            dt_本月應生產 = myclass.Add_LINE_GROUP(dt_本月應生產).ToTable(true, "客戶簡稱", "產線群組", "排程編號", "訂單號碼", "客戶訂單", "品號", "訂單日期", "預計完工日", "進度", "狀態", "組裝日");
        }
    }
}