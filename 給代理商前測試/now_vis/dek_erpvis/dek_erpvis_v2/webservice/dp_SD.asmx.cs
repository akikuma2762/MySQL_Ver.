using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using dekERP_dll;
using dekERP_dll.dekErp;

namespace dek_erpvis_v2.webservice
{
    /// <summary>
    ///dp_SD 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class dp_SD : System.Web.Services.WebService
    {

        iTec_Sales SLS = new iTec_Sales();
        iTec_House WHE = new iTec_House();
     
        /// <summary>
        /// 用於出貨可看見訂單資訊
        /// </summary>
        /// <param name="cust_name">客戶名稱</param>
        /// <param name="item_code">品號</param>
        /// <param name="date_str">起始日</param>
        /// <param name="date_end">結束日</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode GetShipment_details(string cust_name, string item_code, string date_str, string date_end)
        {
            DataTable dt = SLS.Get_Shipment(date_str, date_end, cust_name, item_code);

            XmlDocument xmlDoc = new XmlDocument();

            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (dt != null)
            {
                if (dt.Rows.Count <= 0)
                {
                    xmlElem.SetAttribute("Value", "0");
                }
                else
                {
                    xmlElem.SetAttribute("Value", DataTableUtils.toString(dt.Rows.Count));
                    xmlElem.SetAttribute("item_code", item_code);
                    xmlElem.SetAttribute("cust_name", cust_name);
                    xmlDoc.AppendChild(xmlElem);
                    foreach (DataRow row in dt.Rows)
                    {
                        XmlElement xmlElemA = xmlDoc.CreateElement("Group");
                        xmlElemA.SetAttribute("序列", DataTableUtils.toString(dt.Rows.IndexOf(row) + 1));
                        xmlElemA.SetAttribute("出貨日期", HtmlUtil.changetimeformat(DataTableUtils.toString(row["出貨日期"])));
                        xmlElemA.SetAttribute("出貨單號", DataTableUtils.toString(row["出貨單號"]));
                        xmlElemA.SetAttribute("客戶料號", DataTableUtils.toString(row["客戶料號"]));
                        xmlElemA.SetAttribute("製令號", DataTableUtils.toString(row["製令號"]));
                        xmlElemA.SetAttribute("備註", DataTableUtils.toString(row["備註"]));
                        xmlDoc.DocumentElement.AppendChild(xmlElemA);
                    }
                    return xmlDoc.DocumentElement;
                }
            }
            else
            {
                xmlElem.SetAttribute("Value", "-1");
            }

            xmlDoc.AppendChild(xmlElem);
            dt.Dispose();
            return xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 此處為麗馳所要求之規格 → 用於在庫存看到訂單資訊
        /// </summary>
        /// <param name="order">訂單號碼</param>
        /// <param name="cust">客戶</param>
        /// <param name="date_str">開始日期</param>
        /// <returns></returns>
        [WebMethod]
        public XmlNode Getorder(string order, string cust, string date_str)
        {
            DataTable dt = new DataTable();

            if (cust != "")
                dt = WHE.stockanalysis_Details(date_str, $" = '{cust}'");
            else
                dt = WHE.stockanalysis_Details(date_str, $" IS NULL ");

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("ROOT_PIE");
            if (dt != null)
            {
                if (dt.Rows.Count <= 0)
                {
                    xmlElem.SetAttribute("Value", "0");
                }
                else
                {
                    DataRow[] rows = dt.Select($"訂單號碼='{order}'");
                    if(rows != null && rows.Length>0)
                        xmlElem.SetAttribute("item_code", rows[0]["訂單規格"].ToString());
                    else
                        xmlElem.SetAttribute("item_code", "無規格");

                    xmlDoc.AppendChild(xmlElem);

                    return xmlDoc.DocumentElement;
                }
            }
            else
                xmlElem.SetAttribute("Value", "-1");

            xmlDoc.AppendChild(xmlElem);
            dt.Dispose();
            return xmlDoc.DocumentElement;
        }
    }
}
