using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Support;

namespace dek_erpvis_v2.cls
{
    public class myclass
    {
        //
        public static string NowdbType = "";
        //MSSQL 用戶基本資料連線
        //public static string GetConnByDekVisErp = GetIniLink("des_dekviserp");
        public static string GetConnByDekVisErp = GetIniLink("dekviserp");
        //加工可視化連線
        public static string GetConnByDekVisCnc_inside = GetIniLink("cnc_db");
        //加工VIS連線(MySQL)
        public static string GetConnByDekVisCnc_insideMysql = GetIniLink("cnc_db_test", "", "MySQL");
        //public static string GetConnByDekVisCnc_insideMysql = GetIniLink("cnc_db");
        //立式廠連線
        public static string GetConnByDekdekVisAssm = GetIniLink("dekvisassm");
        //臥式廠連線
        public static string GetConnByDekdekVisAssmHor = GetIniLink("dekvishor"); // dekvishor
        //APS連線
        public static string GetConnByDekVisCNC = GetIniLink("cnc_db_aps");
        public static string GetConnByDekVisAps = GetIniLink("dek_aps");
        public static string logout_url = "../../login.aspx";
        public static string GetIniLink(string title, string export = "", string type = "")
        {
            type = type == "" ? HtmlUtil.Decrypted_Text(HtmlUtil.Get_Ini(HtmlUtil.Encrypted_Text(title + export), "inikey1")) : type;
            string IP = HtmlUtil.Decrypted_Text(HtmlUtil.Get_Ini(HtmlUtil.Encrypted_Text(title + export), "inikey2"));
            string DB = title == "cnc_db_test" ? "cnc_db" : title;
            string Acc = HtmlUtil.Decrypted_Text(HtmlUtil.Get_Ini(HtmlUtil.Encrypted_Text(title + export), "inikey3"));
            string PWD = HtmlUtil.Decrypted_Text(HtmlUtil.Get_Ini(HtmlUtil.Encrypted_Text(title + export), "inikey4"));
            if (!string.IsNullOrEmpty(type))//有被改變過後才紀錄(因為不可能一系統可視化用兩資料庫不是my就是ms)
                NowdbType = type;
            if (type == "MySQL")
                //   return clsDB_Server.GetConntionString_MySQL("172.23.10.106", DB, "root", "asus54886961");
                return clsDB_Server.GetConntionString_MySQL(IP, DB, Acc, PWD);
            else
                //   return clsDB_Server.GetConntionString_MsSQL("172.23.10.106", DB, "sa", "dek1234");
                return clsDB_Server.GetConntionString_MsSQL(IP, DB, Acc, PWD);
        }

        public static string Base64Encode(string AStr)
        {   //編碼兩次
            AStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(AStr));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(AStr));

        }
        public static string Base64Decode(string ABase64)
        {
            //解碼兩次
            ABase64 = Encoding.UTF8.GetString(Convert.FromBase64String(ABase64));
            return Encoding.UTF8.GetString(Convert.FromBase64String(ABase64));
        }
        public bool user_view_check(string URL_NAME, string user_ID)
        {
            clsDB_Server clsDB = new clsDB_Server(GetConnByDekVisErp);
            string sqlcmd = $"SELECT * FROM WEB_USER where WB_URL = '{URL_NAME}' and user_ACC = '{user_ID}' and VIEW_NY = 'Y'";
            DataTable dt = clsDB.DataTable_GetRow(sqlcmd);
            return HtmlUtil.Check_DataTable(dt);
        }
        public string check_user_power(string acc_)
        {
            DataTableUtils.Conn_String = myclass.GetConnByDekVisErp;
            DataRow row = DataTableUtils.DataTable_GetDataRow("USERS", $"USER_ACC = '{acc_}'");
            if (row != null)
                return DataTableUtils.toString(row["ADM"]);
            else
                return "";
        }
        public static int get_ran_id()
        {
            Random Rnd = new Random();
            return Rnd.Next(99999);
        }
        public DataTable user_login(string USER_ACC, string USER_PWD)
        {
            string cmd = "";
            if (Regex.IsMatch(USER_ACC, @"^09[0-9]{8}$"))
                cmd = $"user_num = '{USER_ACC}'";
            else
                cmd = $"user_acc = '{USER_ACC}'";

            clsDB_Server clsDB = new clsDB_Server(GetConnByDekVisErp);
            string sqlcmd = $"SELECT * FROM USERS where {cmd} and user_pwd = '{USER_PWD}' and STATUS = 'ON'";
            DataTable dt = clsDB.DataTable_GetRow(sqlcmd);
            return dt;
        }
        public DataView Add_LINE_GROUP(DataTable dt)
        {
            //有結果的存放於此，才不用一直開關資料庫
            List<string> list = new List<string>();

            DataTable dr = dt;
            string LINE_ID = "";
            string LINE_GROUP = "";
            dt.Columns.Add("產線群組", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                if (list.Count == 0 || list.IndexOf(DataTableUtils.toString(row["產線代號"])) == -1)
                {
                    LINE_ID = DataTableUtils.toString(row["產線代號"]);
                    LINE_GROUP = "";
                    GlobalVar.UseDB_setConnString(GetConnByDekVisErp);//切換至可視化資料庫
                    string sqlcmd = 德大機械.業務部_成品庫存分析.產線群組列表(LINE_ID);
                    DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(ds))
                        LINE_GROUP = DataTableUtils.toString(ds.Rows[0]["GROUP_NAME"]);
                    else
                        LINE_GROUP = "";

                    list.Add(LINE_ID);
                    list.Add(LINE_GROUP);
                }
                else
                {
                    LINE_ID = list[list.IndexOf(DataTableUtils.toString(row["產線代號"]))];
                    LINE_GROUP = list[list.IndexOf(DataTableUtils.toString(row["產線代號"])) + 1];
                }

                if (LINE_GROUP == "")
                    row["產線群組"] = "未設定機種";
                else
                    row["產線群組"] = LINE_GROUP;

            }
            DataView dv = new DataView(dt);
            dv.Sort = "產線群組 asc";
            dv.ToTable();

            return dv;

        }
        public DataView Add_LINE_GROUP(DataTable dt, string 群組欄位名稱, string 產線欄位名稱)
        {
            GlobalVar.UseDB_setConnString(GetConnByDekVisErp);

            DataTable dr = dt;
            dt.Columns.Add(群組欄位名稱, typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                string LINE_ID = DataTableUtils.toString(row[產線欄位名稱]);
                string LINE_GROUP = DataTableUtils.toString(row[群組欄位名稱]);
                if (LINE_GROUP == "")
                {
                    string sqlcmd = 德大機械.業務部_成品庫存分析.產線群組列表(LINE_ID);
                    DataRow row_ = null;
                    try
                    {
                        row_ = DataTableUtils.DataTable_GetDataRow(sqlcmd);
                        row[群組欄位名稱] = DataTableUtils.toString(row_["GROUP_NAME"]);
                    }
                    catch
                    {
                        dr.Columns.RemoveAt(dr.Columns.Count - 1);
                        Add_LINE_GROUP(dr, 群組欄位名稱, 產線欄位名稱);
                    }
                }
            }
            DataView dv = new DataView(dt);
            dv.Sort = "" + 群組欄位名稱 + " asc";
            dv.ToTable();
            return dv;
        }
        public DataView Add_LINE_GROUP(DataTable dt, string Type)
        {
            //有結果的存放於此，才不用一直開關資料庫
            List<string> list = new List<string>();

            DataTable dr = dt;
            string LINE_ID = "";
            string LINE_GROUP = "";
            dt.Columns.Add("產線群組", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                if (list.Count == 0 || list.IndexOf(DataTableUtils.toString(row["產線代號"])) == -1)
                {
                    LINE_ID = DataTableUtils.toString(row["產線代號"]);
                    LINE_GROUP = "";
                    GlobalVar.UseDB_setConnString(GetConnByDekdekVisAssmHor);//切換至可視化資料庫
                    string sqlcmd = $"select * from 工作站型態資料表 where 工作站編號='{LINE_ID}'";
                    DataTable ds = DataTableUtils.GetDataTable(sqlcmd);
                    if (HtmlUtil.Check_DataTable(ds))
                        LINE_GROUP = DataTableUtils.toString(ds.Rows[0]["工作站名稱"]);
                    else
                        LINE_GROUP = "";

                    list.Add(LINE_ID);
                    list.Add(LINE_GROUP);
                }
                else
                {
                    LINE_ID = list[list.IndexOf(DataTableUtils.toString(row["產線代號"]))];
                    LINE_GROUP = list[list.IndexOf(DataTableUtils.toString(row["產線代號"])) + 1];
                }

                if (LINE_GROUP == "")
                    row["產線群組"] = "沒有產線代號";
                else
                    row["產線群組"] = LINE_GROUP;

            }
            DataView dv = new DataView(dt);
            dv.Sort = "產線群組 asc";
            dv.ToTable();

            return dv;
        }
        public string date_trn(string days)
        {
            Double val = DataTableUtils.toDouble(days);
            return DataTableUtils.toString(DateTime.Now.AddDays(-val).ToString("yyyyMMdd"));
        }
        //增加查詢條件
        public static string Insert_Condition(string field_name, List<string> list, string or_and)
        {
            if (list.Count > 0)
            {
                StringBuilder Condition = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    Condition.Append($" {field_name}='{list[i]}' ");
                    if (i < list.Count - 1)
                        Condition.Append($" {or_and} ");
                }
                return Condition.ToString();
            }
            else
                return "";
        }
    }
}
