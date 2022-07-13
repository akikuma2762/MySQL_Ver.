using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dekERP_dll.dekErp
{
    public class ERP_cnc : Get_cnc
    {
        iTechDB iTech = new iTechDB();
        IniManager iniManager = new IniManager("");

        public DataTable All_Schdule(string source = "cnc",List<string> list = null)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetAll_Schdule(list);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Enter_ReportView(string source = "cnc", List<string> list = null)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetEnter_ReportView(list);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Enter_MaintainView(string source = "cnc", List<string> list = null)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetEnter_MaintainView(list);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Person_columns(string usepage ,string acc, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetPerson_columns(usepage,acc);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable System_columns(string usepage, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetSystem_columns(usepage);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable ErrorType(string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetErrorType();
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Get_DataTable(string dt_name, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Get_dt(dt_name);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Get_IDMax(string dt_name, string column, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Get_Max(dt_name, column);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Save_Cloumn(string acc, string usepage, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Get_Save_Cloumn(acc, usepage);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Des_Defective(string type, string start, string end, string condition, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Get_Des_Defective(type, start, end, condition);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Des_Maintain(string start,string end,string type,string condition,string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Get_Des_Maintain(start, end, type, condition);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Des_Pause(string type, string start, string end, string condition, string source = "cnc")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Get_Des_Pause(type, start, end, condition);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        //----------------------------------------------------語法產生處---------------------------------------------------------
       
        //目前已入站的排程(報工 || 維護)
        string GetAll_Schdule(List<string> list = null)
        {
            string machname = "";
            string columns = iniManager.ReadIniFile("Parameter", "mach_column", "");

            if (columns != "")
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                        machname += machname == "" ? $" {columns}='{list[i]}' " : $" OR {columns}='{list[i]}' ";
                }
                machname = machname == "" ? "" : $" and ( {machname} ) ";
            }

            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "All_Schdule", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "All_Schdule", ""), machname);
            return sqlcmd.ToString();
        }
        //目前已入站的排程(報工)
        string GetEnter_ReportView(List<string> list = null)
        {
            string machname = "";
            string columns = iniManager.ReadIniFile("Parameter", "mach_column", "");

            if (columns != "")
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                        machname += machname == "" ? $" {columns}='{list[i]}' " : $" OR {columns}='{list[i]}' ";
                }
                machname = machname == "" ? "" : $" and ( {machname} ) ";
            }

            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Enter_ReportView", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Enter_ReportView", ""), machname);
            return sqlcmd.ToString();
        }
        //目前已入站的排程(維護)
        string GetEnter_MaintainView(List<string> list = null)
        {
            string machname = "";
            string columns = iniManager.ReadIniFile("Parameter", "mach_column", "");

            if (columns != "")
            {
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                        machname += machname == "" ? $" {columns}='{list[i]}' " : $" OR {columns}='{list[i]}' ";
                }
                machname = machname == "" ? "" : $" and ( {machname} ) ";
            }


            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Enter_ReportView", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Enter_MaintainView", ""), machname);
            return sqlcmd.ToString();
        }
        //取得個人設定之欄位顯示資訊
        string GetPerson_columns(string usepage,string acc)
        {
            usepage = usepage == "" ? "" : $" and realtime_info.use_page = '{usepage}'";
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Person_columns", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Person_columns", ""), acc, usepage);
            return sqlcmd.ToString();
        }
        //取得系統預設之欄位顯示資訊
        string GetSystem_columns(string usepage)
        {
            usepage = usepage == "" ? "" : $" and realtime_info.use_page = '{usepage}'";
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "System_columns", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "System_columns", ""), usepage);
            return sqlcmd.ToString();
        }
        //取得異常類型的資訊
        string GetErrorType()
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "ErrorType", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "ErrorType", ""));
            return sqlcmd.ToString();
        }
        //取得資料表資料
        string Get_dt(string dt_name)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Get_DataTable", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Get_DataTable", ""), dt_name);
            return sqlcmd.ToString();
        }
        //取得該資料表最大之ID
        string Get_Max(string dt_name, string column)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Get_IDMax", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Get_IDMax", ""), column, dt_name);
            return sqlcmd.ToString();
        }
        //取得個人儲存之欄位資訊
        string Get_Save_Cloumn(string acc, string usepage)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Save_Cloumn", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Save_Cloumn", ""), acc, usepage);
            return sqlcmd.ToString();
        }
        //取得不良資訊
        string Get_Des_Defective(string type, string start, string end, string condition)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Des_Defective", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Des_Defective", ""), type, start, end, condition);
            return sqlcmd.ToString();
        }
        //取得生產資訊
        string Get_Des_Maintain(string start, string end, string type, string condition)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Des_Maintain", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Des_Maintain", ""), start, end, type, condition);
            return sqlcmd.ToString();
        }
        //取得暫停資訊
        string Get_Des_Pause(string type, string start, string end, string condition)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Des_Pause", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Des_Pause", ""), type, start, end, condition);
            return sqlcmd.ToString();
        }
    }
}
