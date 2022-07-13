using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dekERP_dll.dekErp
{
    public class ERP_Product : Get_Product
    {
        iTechDB iTech = new iTechDB();
        const string DateFormat = "yyyyMMdd";
        IniManager iniManager = new IniManager("");

        public DataTable KnifeSet(string start, string end, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetKnifeSet(start, end);
            return iTech.Get_DataTable(sqlcmd, source);
        }


        public DataTable KnifeSet(DateTime start, DateTime end, string source)
        {
            return KnifeSet(start.ToString(DateFormat), end.ToString(DateFormat), source);
        }

         string GetKnifeSet(string start, string end)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "KnifeSet", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "KnifeSet", ""), start, end);
            return sqlcmd.ToString();
        }
    }
}
