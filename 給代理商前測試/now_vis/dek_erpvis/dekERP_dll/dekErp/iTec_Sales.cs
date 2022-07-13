using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dekERP_dll.dekErp
{
    public class iTec_Sales : IdepSales
    {
        iTechDB iTech = new iTechDB();
        const string DateFormat = "yyyyMMdd";
        IniManager iniManager = new IniManager(ConfigurationManager.AppSettings["ini_road"]);

        //訂單數量與金額統計
        public DataTable Orders(string start, string end, string status, dekModel img_or_tbl, OrderLineorCust line_or_custom, OrderType qty_or_amt, int max_records)
        {
            string sqlcmd = GetOrders(start, end, status, img_or_tbl, line_or_custom, qty_or_amt, max_records);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Orders(DateTime dt_start, DateTime dt_end, string status, dekModel img_or_tbl, OrderLineorCust line_or_custom, OrderType qty_or_amt, int max_records)
        {
            return Orders(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), status, img_or_tbl, line_or_custom, qty_or_amt, max_records);
        }

        //逾期訂單數量與金額統計
        public DataTable Orders_NoFinish(string start, OrderType qty_or_amt, dekModel img_or_tbl, OrderLineorCust line_or_custom)
        {
            string sqlcmd = GetOrders_NoFinish(start, qty_or_amt, img_or_tbl, line_or_custom);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Orders_NoFinish(DateTime dt_start, OrderType qty_or_amt, dekModel img_or_tbl, OrderLineorCust line_or_custom)
        {
            return Orders_NoFinish(dt_start.ToString(DateFormat), qty_or_amt, img_or_tbl, line_or_custom);
        }

        //訂單詳細表
        public DataTable Orders_Details(string start, string end, string custom, string status)
        {
            string sqlcmd = GetOrders_Details(start, end, custom, status);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Orders_Details(DateTime dt_start, DateTime dt_end, string custom, string status)
        {
            return Orders_Details(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), custom, status);
        }

        //逾期訂單詳細表
        public DataTable Orders_NotFinish_Details(string start, string custom)
        {
            string sqlcmd = GetOrders_NotFinish_Details(start, custom);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }
        public DataTable Orders_NotFinish_Details(DateTime dt_start, string custom)
        {
            return Orders_NotFinish_Details(dt_start.ToString(DateFormat), custom);
        }


        //出貨統計表
        public DataTable Shipment(string start, string end, dekModel img_or_tbl)
        {
            string sqlcmd = GetShipment(start, end, img_or_tbl);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Shipment(DateTime dt_start, DateTime dt_end, dekModel img_or_tbl)
        {
            return Shipment(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), img_or_tbl);
        }

        //出貨詳細表
        public DataTable Shipment_Detail(string start, string end, string custom)
        {
            string sqlcmd = GetShipment_Detail(start, end, custom);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Shipment_Detail(DateTime dt_start, DateTime dt_end, string custom)
        {
            return Shipment_Detail(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), custom);
        }

        //出貨詳細表 小計
        public DataTable Get_Shipment(string start, string end, string custom, string item)
        {
            string sqlcmd = GetShipment_Data(start, end, custom, item);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Get_Shipment(DateTime dt_start, DateTime dt_end, string custom, string item)
        {
            return Get_Shipment(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), custom, item);
        }

        //訂單變更
        public DataTable Get_recordsofchangetheorder(string start, string end, dekModel img_or_tbl, string top)
        {
            string sqlcmd = Getrecordsofchangetheorder(start, end, img_or_tbl, top);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable Get_recordsofchangetheorder(DateTime dt_start, DateTime dt_end, dekModel img_or_tbl, string top)
        {
            return Get_recordsofchangetheorder(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), img_or_tbl, top);
        }

        //運輸架未歸還紀錄
        public DataTable Transportrackstatistics(dekModel img_or_tbl, TransportrackstatisticsImageType type, string condition)
        {
            string sqlcmd = GetTransportrackstatistics(img_or_tbl, type, condition);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        //未交易客戶
        public DataTable UntradedCustomer(string start, string end, string symbol, int day)
        {
            string sqlcmd = GetUntradedCustomer(start, end, symbol, day);
            DataTable dt = iTech.Get_DataTable(sqlcmd);
            return iTech.Error_DataTable(dt, sqlcmd);
        }

        public DataTable UntradedCustomer(DateTime dt_start, DateTime dt_end, string symbol, int day)
        {
            return UntradedCustomer(dt_start.ToString(DateFormat), dt_end.ToString(DateFormat), symbol, day);
        }

        //測試用
        public DataTable TestLinkTable(string sqlcmd)
        {
            return iTech.Error_DataTable(iTech.Get_DataTable(sqlcmd), sqlcmd);
        }

        //-----------------------------------SQL語法專區----------------------------------------------------------
        //訂單金額及數量統計
        string GetOrders(string start, string end, string status, dekModel img_or_tbl, OrderLineorCust line_or_custom, OrderType qty_or_amt, int max_records)
        {
            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder Condition = new StringBuilder();
            StringBuilder max = new StringBuilder();

            if (status != "all")
                Condition.Append($" and CODD.code = {status}");

            if (img_or_tbl == dekModel.Image)
            {
                if (line_or_custom == OrderLineorCust.Line)
                {
                    if (iniManager.ReadIniFile("dekERPVIS", "Orders_" + line_or_custom, "") != "")
                        //開始時間 結束時間 訂單狀態
                        sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_" + line_or_custom, ""), start, end, Condition);
                 
                }
                else if (line_or_custom == OrderLineorCust.Custom)
                {
                    if (max_records != 0)
                        max.Append($" where ROWNUM <={max_records}");
                    if (iniManager.ReadIniFile("dekERPVIS", "Orders_" + line_or_custom, "") != "")
                        //開始時間 結束時間 訂單狀態 排序依據(數量 金額) 最多取幾筆
                        sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_" + line_or_custom, ""), start, end, Condition, qty_or_amt, max);
                }
            }
            else if (img_or_tbl == dekModel.Table)
            {
                if (iniManager.ReadIniFile("dekERPVIS", "Orders_" + img_or_tbl, "") != "")
                    //開始時間 結束時間 訂單狀態
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_" + img_or_tbl, ""), start, end, Condition);
            }
            return sqlcmd.ToString();
        }

        //逾期訂單數量與金額統計
        string GetOrders_NoFinish(string start, OrderType qty_or_amt, dekModel img_or_tbl, OrderLineorCust line_or_custom)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (img_or_tbl == dekModel.Image)
            {
                if (line_or_custom == OrderLineorCust.Line)
                {
                    if (iniManager.ReadIniFile("dekERPVIS", "Orders_NoFinish_" + line_or_custom, "") != "")
                        //開始時間 結束時間 訂單狀態
                        sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_NoFinish_" + line_or_custom, ""), start);
                }
                else if (line_or_custom == OrderLineorCust.Custom)
                {

                    if (iniManager.ReadIniFile("dekERPVIS", "Orders_NoFinish_" + line_or_custom, "") != "")
                        //開始時間 結束時間 訂單狀態 排序依據(數量 金額) 最多取幾筆
                        sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_NoFinish_" + line_or_custom, ""), start);
                }
            }
            else if (img_or_tbl == dekModel.Table)
            {
                if (iniManager.ReadIniFile("dekERPVIS", "Orders_NoFinish_" + img_or_tbl, "") != "")
                    //開始時間 結束時間 訂單狀態
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_NoFinish_" + img_or_tbl, ""), start);
            }
            return sqlcmd.ToString();

        }

        //訂單詳細表
        string GetOrders_Details(string start, string end, string custom, string status)
        {
            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder Condition = new StringBuilder();
            if (status != "all")
                Condition.Append($" and CODD.code = {status}");

            if (iniManager.ReadIniFile("dekERPVIS", "Orders_Details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_Details", ""), start, end, custom, Condition);
            return sqlcmd.ToString();
        }

        //訂單詳細表
        string GetOrders_NotFinish_Details(string start, string custom)
        {
            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder Condition = new StringBuilder();

            if (iniManager.ReadIniFile("dekERPVIS", "Orders_NotFinish_Details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_NotFinish_Details", ""), start, custom);
            return sqlcmd.ToString();
        }


        //出貨統計表
        string GetShipment(string start, string end, dekModel img_or_tbl)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Shipment_" + img_or_tbl, "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Shipment_" + img_or_tbl, ""), start, end);
            return sqlcmd.ToString();
        }

        //出貨詳細表
        string GetShipment_Detail(string start, string end, string custom)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Shipment_Details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Shipment_Details", ""), start, end, custom);
            return sqlcmd.ToString();
        }

        //出貨詳細表 小計
        string GetShipment_Data(string start, string end, string custom, string item)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "GetShipment_details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "GetShipment_details", ""), start, end, custom, item);
            return sqlcmd.ToString();
        }

        //訂單變更紀錄
        string Getrecordsofchangetheorder(string start, string end, dekModel img_or_tbl, string top)
        {
            string condition = "";
            StringBuilder sqlcmd = new StringBuilder();

            if (top != "0")
                condition = $"  where ROWNUM <= {top} ";
            if (iniManager.ReadIniFile("dekERPVIS", "Recordsofchangetheorder", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Recordsofchangetheorder", ""), start, end, condition);
            return sqlcmd.ToString();
        }

        //運輸架未歸還統計
        string GetTransportrackstatistics(dekModel img_or_tbl, TransportrackstatisticsImageType type, string condition)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (img_or_tbl == dekModel.Image)
            {
                if (iniManager.ReadIniFile("dekERPVIS", "Transportrackstatistics_" + type, "") != "")
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Transportrackstatistics_" + type, ""), condition);
            }
            else if (img_or_tbl == dekModel.Table)
            {
                if (iniManager.ReadIniFile("dekERPVIS", "Transportrackstatistics_" + img_or_tbl, "") != "")
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Transportrackstatistics_" + img_or_tbl, ""), condition);
            }
            return sqlcmd.ToString();
        }

        //未交易客戶
        string GetUntradedCustomer(string start, string end, string symbol, int day)
        {
            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder condition = new StringBuilder();

            if (start != "" && end != "")
                condition.AppendFormat(iniManager.ReadIniFile("Parameter", "UntradedCustomer_Time", ""), start, end);

            if (iniManager.ReadIniFile("dekERPVIS", "UntradedCustomer", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "UntradedCustomer", ""), DateTime.Now.ToString("yyyyMMdd"), symbol, day, condition);

            return sqlcmd.ToString();
        }
    }
}
