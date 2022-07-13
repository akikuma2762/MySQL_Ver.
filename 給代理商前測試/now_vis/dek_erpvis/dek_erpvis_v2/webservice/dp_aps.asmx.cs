using dek_erpvis_v2.cls;
using MongoDB.Driver.Builders;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Xml;
using System.Globalization;
using System.Net;

namespace dek_erpvis_v2.webservice
{
    /// <summary>
    ///dp_aps 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class dp_aps : System.Web.Services.WebService
    {

        [WebMethod]
        public XmlNode Now_Task(string Machine)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisCNC);
            string sqlcmd = $"select * from mach_content where Mach_Name='{Machine}' ";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (HtmlUtil.Check_DataTable(dt))
            {
                xmlElem.SetAttribute("Value", DataTableUtils.toString(dt.Rows.Count));
                xmlElem.SetAttribute("Machine", Machine);
                xmlDoc.AppendChild(xmlElem);
                foreach (DataRow row in dt.Rows)
                {
                    XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                    xmlElemA.SetAttribute("機台名稱", $"<center>{DataTableUtils.toString(row["Mach_Name"])}</center>");
                    xmlElemA.SetAttribute("品名規格", $"<center>{DataTableUtils.toString(row["Product_Name"])}</center>");
                    xmlElemA.SetAttribute("現在數量", $"<center>{DataTableUtils.toString(row["Now_QTY"])}</center>");
                    xmlElemA.SetAttribute("目標數量", $"<center>{DataTableUtils.toString(row["Target_QTY"])}</center>");
                    xmlElemA.SetAttribute("預計開工", $"<center>{HtmlUtil.StrToDate(DataTableUtils.toString(row["Predict_Start"]))}</center>");
                    xmlElemA.SetAttribute("預計結束", $"<center>{HtmlUtil.StrToDate(DataTableUtils.toString(row["Predict_End"]))}</center>");
                    xmlElemA.SetAttribute("製令單號", $"<center>{DataTableUtils.toString(row["Order_Number"])}</center>");

                    xmlDoc.DocumentElement.AppendChild(xmlElemA);
                }
                return xmlDoc.DocumentElement;
            }
            else
                xmlElem.SetAttribute("Value", "-1");
            xmlDoc.AppendChild(xmlElem);
            dt.Dispose();
            return xmlDoc.DocumentElement;
        }
    }
}
