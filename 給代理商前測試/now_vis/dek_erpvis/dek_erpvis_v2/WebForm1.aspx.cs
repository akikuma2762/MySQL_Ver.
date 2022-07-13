using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using Support;

namespace dek_erpvis_v2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        clsDB_Server cls = new clsDB_Server(myclass.GetConnByDekVisCnc_inside);
        protected void Page_Load(object sender, EventArgs e)
        {
            string sqlcmd = "select * from realtime_info";
            DataTable dt = cls.GetDataTable(sqlcmd);

            if (dt != null)
            {
                DataTable dts = dt.Clone();
                DataRow row = dts.NewRow();
                row["info_name"] = "SDASDASd";
                dts.Rows.Add(row);
                row = dts.NewRow();
                row["info_name"] = "3";
                dts.Rows.Add(row);
                cls.Insert_TableRows("realtime_info", dts);
                string ss = cls.ErrorMessage;
            }

        }
    }
}