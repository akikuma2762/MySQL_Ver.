using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace dekERP_dll.dekErp
{
    public class ERP_Materials : Get_Material
    {
        iTechDB iTech = new iTechDB();
        const string DateFormat = "yyyyMMdd";
        IniManager iniManager = new IniManager("");
        public DataTable SupplierShortage(SupplierShortageType type, string supplier, string supplierName, string start, string end, string itemNo, string Reminder_Date, string source)
        {
            return null;
        }
        public DataTable SupplierShortage(SupplierShortageType type, string supplier, string supplierName, DateTime start, DateTime end, string itemNo, string Reminder_Date, string source)
        {
            return SupplierShortage(type, supplier, supplierName, start.ToString(DateFormat), end.ToString(DateFormat), itemNo, Reminder_Date, source);
        }

        public DataTable Supplierscore_Detail(string start, string end, string source, string custom = "")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetSupplierscore_Detail(start, end);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Supplierscore_Detail(DateTime start, DateTime end, string source, string custom = "")
        {
            return Supplierscore_Detail(start.ToString(DateFormat), end.ToString(DateFormat), source, custom);
        }

        public DataTable materialrequirementplanning_Detail(string start, string end, RadioButtonList rbx, string item, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Getmaterialrequirementplanning_Detail(start, end, rbx, item);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable materialrequirementplanning_Detail(DateTime start, DateTime end, RadioButtonList rbx, string item, string source)
        {
            return materialrequirementplanning_Detail(start.ToString(DateFormat), end.ToString(DateFormat), rbx, item, source);
        }

        public DataTable pick_list_title(string Number, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Getpick_list_title(Number);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable pick_list_datatable(string Number, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = Getpick_list_datatable(Number);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Item_DataTable(string ini_Name, string source, string start = "", string end = "")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetItem_DataTable(ini_Name, start, end);
            return iTech.Get_DataTable(sqlcmd, source);
        }
        //----------------------------------------------------語法產生處---------------------------------------------------------
        //供應商達交率
        string GetSupplierscore_Detail(string start, string end)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Supplierscore_Detail", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Supplierscore_Detail", ""), start, end);
            return sqlcmd.ToString();
        }

        //物料領用統計表
        string Getmaterialrequirementplanning_Detail(string start, string end, RadioButtonList rbx, string item)
        {
            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder Condition = new StringBuilder();
            string sql = rbx.SelectedItem.Value == "1" ? "item" : rbx.SelectedItem.Value == "4" ? "dtl" : "Name";
            //看要查詢的是品號還是品名
            if (item != "" && iniManager.ReadIniFile("Parameter", $"materialrequirementplanning_{sql}", "") != "")
                Condition.AppendFormat(iniManager.ReadIniFile("Parameter", $"materialrequirementplanning_{sql}", ""), item);

            if (iniManager.ReadIniFile("dekERPVIS", "materialrequirementplanning_Detail", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "materialrequirementplanning_Detail", ""), start, end, Condition);

            return sqlcmd.ToString();
        }

        //取得下拉選單 核取方塊 單選框來源
        string GetItem_DataTable(string ini_name, string start = "", string end = "")
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("Get_Item", ini_name, "") != "")
            {
                if (start != "" && end != "")
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("Get_Item", ini_name, ""), start, end);
                else
                    sqlcmd.Append(iniManager.ReadIniFile("Get_Item", ini_name, ""));
            }
            return sqlcmd.ToString();
        }

        //產生領料表表頭
        string Getpick_list_title(string Number)
        {
            List<string> list = new List<string>(Number.Split('#'));
            string lot_no = iniManager.ReadIniFile("Parameter", "lot_no", "");
            string mkord_no = iniManager.ReadIniFile("Parameter", "mkord_no", "");
            string condition = "";
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] != "")
                    condition += condition == "" ? $" {lot_no}='{list[i]}' OR {mkord_no}='{list[i]}' " : $" OR {lot_no}='{list[i]}' OR {mkord_no}='{list[i]}' ";
            }
            condition = condition == "" ? "" : $" and ( {condition} ) ";

            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "pick_list_title", "") != "" && condition != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "pick_list_title", ""), condition);

            return sqlcmd.ToString();
        }

        //產生領料表表格
        string Getpick_list_datatable(string Number)
        {
            List<string> list = new List<string>(Number.Split('#'));
            string condition = "";
            string lot_no = iniManager.ReadIniFile("Parameter", "lot_no", "");
            string mkord_no = iniManager.ReadIniFile("Parameter", "mkord_no", "");
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] != "")
                    condition += condition == "" ? $" {lot_no}='{list[i]}' OR {mkord_no}='{list[i]}' " : $" OR {lot_no}='{list[i]}' OR {mkord_no}='{list[i]}' ";
            }
            condition = condition == "" ? "" : $" and ( {condition} ) ";

            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "pick_list_datatable", "") != "" && condition != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "pick_list_datatable", ""), condition);
            return sqlcmd.ToString();

        }

    }
}
