using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace dek_erpvis_v2.webservice
{
    /// <summary>
    ///WebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
     [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        clsDB_Server clsDB_sw = new clsDB_Server("");
        myclass myclass = new myclass();
        [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
        //方法不可宣告static
        public string[] GetCompletionList(string prefixText, int count)
        {
         //   clsDB_sw.dbOpen(myclass.GetConnByDetaSowon);
            string sqlcmd = @"select Top (" + count + ") FACT_NO from fact Where FACT_NO Like '%" + prefixText + "%' ";
            DataTable dt = clsDB_sw.DataTable_GetTable(sqlcmd);
            ArrayList array = new ArrayList();//儲存撈出來的字串集合


                foreach (DataRow row in dt.Rows)
                {
                    array.Add(row["FACT_NO"].ToString());
                }

            //}

            return (string[])array.ToArray(typeof(string));

        }
    }
}
