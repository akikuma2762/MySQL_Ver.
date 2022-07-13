using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dekERP_dll.dekErp
{
    public class ERP_Customized : Get_Customized
    {
        iTechDB iTech = new iTechDB();
        IniManager iniManager = new IniManager(ConfigurationManager.AppSettings["ini_road"]);

        public DataTable Picking_List(string ordernumber, string item, string itemname)
        {
            string sqlcmd = GetPicking_List(ordernumber, item, itemname);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }


        public DataTable Stock_Details(string itemname)
        {
            string sqlcmd = GetStock_Details(itemname);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Account_Outstanding_Details()
        {
            string sqlcmd = GetAccount_Outstanding_Details();
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        //-----------------------------------SQL語法專區----------------------------------------------------------

        string GetStock_Details(string itemname)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Stock_Details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Stock_Details", ""), itemname);
            return sqlcmd.ToString();
        }

        string GetPicking_List(string ordernumber, string item, string itemname)
        {
            string condition1 = "";
            if (itemname != "")
            {
                if (iniManager.ReadIniFile("Parameter", "Item_name", "") != "")
                    condition1 = string.Format(iniManager.ReadIniFile("Parameter", "Item_name", ""), itemname);
            }
            string condition2 = "";
            if (item != "")
            {
                if (iniManager.ReadIniFile("Parameter", "Item", "") != "")
                    condition1 = string.Format(iniManager.ReadIniFile("Parameter", "Item", ""), item);
            }

            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Picking_List", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Picking_List", ""), ordernumber, condition1, condition2);
            return sqlcmd.ToString();
        }

        string GetAccount_Outstanding_Details()
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Account_Outstanding_Details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Account_Outstanding_Details", ""));
            return sqlcmd.ToString();
        }
    }
}
