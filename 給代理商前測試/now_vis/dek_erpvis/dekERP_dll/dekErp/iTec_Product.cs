using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dekERP_dll.dekErp
{
    public class iTec_Product : IdepProduct
    {
        iTechDB iTech = new iTechDB();
        const string DateFormat = "yyyyMMdd";
        IniManager iniManager = new IniManager(ConfigurationManager.AppSettings["ini_road"]);

        //生產推移圖
        public DataTable waitingfortheproduction(string start, string end)
        {
            string sqlcmd = Getwaitingfortheproduction(start, end);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }
        public DataTable waitingfortheproduction(DateTime start, DateTime end)
        {
            return waitingfortheproduction(start.ToString(DateFormat), end.ToString(DateFormat));
        }


        //--------------------------------------------------SQL指令專區----------------------------------------------

        string Getwaitingfortheproduction(string start, string end)
        {
            string condition = "";
            if (start != "" && end != "")
                condition = $"  CLDS.DAT_ENDS >={start} and CLDS.DAT_ENDS <={end} ";
            else if (start != "" && end == "")
                condition = $"  CLDS.DAT_ENDS <{start} ";

            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "waitingfortheproduction", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "waitingfortheproduction", ""), condition);
            return sqlcmd.ToString();
        }
    }
}
