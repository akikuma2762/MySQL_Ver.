﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dekERP_dll.dekErp
{
    public class ERP_Sales : Get_Sales
    {
        iTechDB iTech = new iTechDB();
        const string DateFormat = "yyyyMMdd";
        IniManager iniManager = new IniManager("");
        public DataTable Orders_Detail(string start, string end, OrderStatus status, string source, string custom = "")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetOrders_Detail(start, end, status);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Orders_Detail(DateTime start, DateTime end, OrderStatus status, string source, string custom = "")
        {
            return Orders_Detail(start.ToString(DateFormat), end.ToString(DateFormat), status, source, custom);
        }

        public DataTable Orders_Over_Detail(string start, string source, string custom = "")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetOrders_Over_Detail(start);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Orders_Over_Detail(DateTime start, string source, string custom = "")
        {
            return Orders_Over_Detail(start.ToString(DateFormat), source, custom);
        }

        public DataTable Shipment_Detail(string start, string end, string source, string custom = "")
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetShipment_Detail(start, end);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Shipment_Detail(DateTime start, DateTime end, string source, string custom = "")
        {
            return Shipment_Detail(start.ToString(DateFormat), end.ToString(DateFormat), source, custom);
        }

        public DataTable Get_Shipment(string start, string end, string custom, string item, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetShipment(start, end, custom, item);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Get_Shipment(DateTime start, DateTime end, string custom, string item, string source)
        {
            return Get_Shipment(start.ToString(DateFormat), end.ToString(DateFormat), custom, item, source);
        }

        public DataTable Transportrackstatistics( string acc, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetTransportrackstatistics(acc);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable UntradedCustomer(string start, string end, string symbol, int day, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetUntradedCustomer(start, end, symbol, day, source);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable UntradedCustomer(DateTime start, DateTime end, string symbol, int day, string source)
        {
            return UntradedCustomer(start.ToString(DateFormat), end.ToString(DateFormat), symbol, day, source);
        }

        public DataTable Recordsofchangetheorder_Details(string start, string end, string source)
        {
            iniManager = new IniManager($"{ConfigurationManager.AppSettings["ini_local"]}{source}Erp.ini");
            string sqlcmd = GetRecordsofchangetheorder_Details(start, end);
            return iTech.Get_DataTable(sqlcmd, source);
        }

        public DataTable Recordsofchangetheorder_Details(DateTime start, DateTime end, string source)
        {
            return Recordsofchangetheorder_Details(start.ToString(DateFormat), end.ToString(DateFormat), source);
        }


        //----------------------------------------------------語法產生處---------------------------------------------------------
        //訂單數量與金額統計
        string GetOrders_Detail(string start, string end, OrderStatus status)
        {
            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder Condition = new StringBuilder();

            if (status != OrderStatus.All && iniManager.ReadIniFile("Parameter", "Orders_" + status, "") != "")
                Condition.Append(iniManager.ReadIniFile("Parameter", "Orders_" + status, ""));

            if (iniManager.ReadIniFile("dekERPVIS", "Orders_Details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_Details", ""), start, end, Condition);

            return sqlcmd.ToString();
        }
        //訂單逾期數量
        string GetOrders_Over_Detail(string start)
        {
            StringBuilder sqlcmd = new StringBuilder();

            if (iniManager.ReadIniFile("dekERPVIS", "Orders_Over_Detail", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Orders_Over_Detail", ""), start);

            return sqlcmd.ToString();
        }
        //出貨統計表
        string GetShipment_Detail(string start, string end)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Shipment_Detail", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Shipment_Detail", ""), start, end);
            return sqlcmd.ToString();
        }
        //出貨小計
        string GetShipment(string start, string end, string custom, string item)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "Get_Shipment", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "Get_Shipment", ""), start, end, custom, item);
            return sqlcmd.ToString();
        }
        //未交易客戶
        string GetUntradedCustomer(string start, string end, string symbol, int day, string source)
        {

            StringBuilder sqlcmd = new StringBuilder();
            StringBuilder Condition = new StringBuilder();

            if (start != "" && end != "" && iniManager.ReadIniFile("Parameter", "UntradedCustomer_Time", "") != "")
                Condition.AppendFormat(iniManager.ReadIniFile("Parameter", "UntradedCustomer_Time", ""), start, end);

            if (iniManager.ReadIniFile("dekERPVIS", "UntradedCustomer", "") != "")
            {
                if (source == "dek")
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "UntradedCustomer", ""), symbol, day, Condition, DateTime.Now.ToString(DateFormat));
                else
                    sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "UntradedCustomer", ""), symbol, day, Condition);
            }


            return sqlcmd.ToString();
        }
        //訂單變更紀錄
        string GetRecordsofchangetheorder_Details(string start, string end)
        {
            StringBuilder sqlcmd = new StringBuilder();
            if (iniManager.ReadIniFile("dekERPVIS", "recordsofchangetheorder_details", "") != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", "recordsofchangetheorder_details", ""), start, end);
            return sqlcmd.ToString();
        }
        string GetTransportrackstatistics( string acc)
        {
            StringBuilder sqlcmd = new StringBuilder();
            List<string> Transport_rack = new List<string>();
            string condition = "";
            if (iniManager.ReadIniFile("Get_Item", "Item_No", "") != "")
                Transport_rack = new List<string>(iniManager.ReadIniFile("Get_Item", "Item_No", "").Split(','));
            string columns = iniManager.ReadIniFile("Parameter", "Transport", "");
            int x = acc == "visrd" || acc == "detawink" || acc == "" ? 0 : 1;

            for (int i = 0; i < Transport_rack.Count - x; i++)
                condition += condition == "" ? $" {columns}='{Transport_rack[i]}' " : $" or  {columns}='{Transport_rack[i]}' ";

            condition = condition != "" ? $" and ( {condition} ) " : "";

            if (iniManager.ReadIniFile("dekERPVIS", $"Transportrackstatistics", "") != "" && condition != "")
                sqlcmd.AppendFormat(iniManager.ReadIniFile("dekERPVIS", $"Transportrackstatistics", ""), condition);

            return sqlcmd.ToString();

        }
    }
}
