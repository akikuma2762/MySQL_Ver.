using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace dekERP_dll
{
    //建立參數
    public enum OrderStatus { All, Finished, Unfinished, Scheduled, Unscheduled };
    public enum dekModel { Image, Table };
    public enum OrderLineorCust { Line, Custom };
    public enum OrderType { 數量, 金額 };
    public enum TransportrackstatisticsImageType { Normal, Abnormal, All }
    public enum SupplierShortageType { 收貨單, 採購單 };

    //業務部
    interface IdepSales
    {
        //訂單數量及金額統計 start(開始時間) end(結束時間) status(訂單狀態) amt_or_qty(數量/金額) img_or_tbl(圖/表) line_or_custom(產線/客戶) max_records(前N筆資料)
        DataTable Orders(string start, string end, string status, dekModel img_or_tbl, OrderLineorCust line_or_custom, OrderType qty_or_amt, int max_records);
        DataTable Orders(DateTime start, DateTime end, string status, dekModel img_or_tbl, OrderLineorCust line_or_custom, OrderType qty_or_amt, int max_records);

        //逾期訂單數量與金額統計 start(開始時間) amt_or_qty(數量/金額) img_or_tbl(圖/表) line_or_custom(產線/客戶)
        DataTable Orders_NoFinish(string start, OrderType qty_or_amt, dekModel img_or_tbl, OrderLineorCust line_or_custom);
        DataTable Orders_NoFinish(DateTime start, OrderType qty_or_amt, dekModel img_or_tbl, OrderLineorCust line_or_custom);


        //訂單詳細表 start(開始時間) end(結束時間) custom(客戶名稱) status(訂單狀態)
        DataTable Orders_Details(string start, string end, string custom, string status);
        DataTable Orders_Details(DateTime start, DateTime end, string custom, string status);

        //逾期訂單詳細表 start(開始時間) custom(客戶名稱)
        DataTable Orders_NotFinish_Details(string start, string custom);
        DataTable Orders_NotFinish_Details(DateTime start, string custom);

        //出貨統計表 start(開始時間) end(結束時間) img_or_tbl(圖/表)
        DataTable Shipment(string start, string end, dekModel img_or_tbl);
        DataTable Shipment(DateTime start, DateTime end, dekModel img_or_tbl);

        //出貨詳細表 start(開始時間) end(結束時間) custom(客戶名稱)
        DataTable Shipment_Detail(string start, string end, string custom);
        DataTable Shipment_Detail(DateTime start, DateTime end, string custom);

        //出貨詳細表 小計 start(開始時間) end(結束時間) custom(客戶名稱) item(物品名稱)
        DataTable Get_Shipment(string start, string end, string custom, string item);
        DataTable Get_Shipment(DateTime start, DateTime end, string custom, string item);
        //訂單變更紀錄 start(開始時間) end(結束時間) img_or_tbl(圖 or 表) top(前N名)
        DataTable Get_recordsofchangetheorder(string start, string end, dekModel img_or_tbl, string top);
        DataTable Get_recordsofchangetheorder(DateTime start, DateTime end, dekModel img_or_tbl, string top);

        //運輸架未歸還統計 img_or_tbl(圖/表) type(正常/異常/全部) 
        DataTable Transportrackstatistics(dekModel img_or_tbl, TransportrackstatisticsImageType type, string condition);

        //未交易客戶 start(開始時間) end(結束時間) symbol(< = >) day(天數)
        DataTable UntradedCustomer(string start, string end, string symbol, int day);
        DataTable UntradedCustomer(DateTime start, DateTime end, string symbol, int day);

        DataTable TestLinkTable(string sqlcmd);
    }
    //資材部
    interface IdepMaterial
    {
        //物料領用統計表 start(開始時間) end(結束時間) type(品號 or 品名)  item(品號品名的相關字眼) judge(內含item名稱或是等於item名稱) length(擷取幾個位數 0.不擷取 1.擷取該數字)
        DataTable materialrequirementplanning(string start, string end, string type, string item, string judge, string length);
        DataTable materialrequirementplanning(DateTime start, DateTime end, string type, string item, string judge, string length);

        //物料領用統計表詳細資訊 start(開始時間) end(結束時間) item(物料) img_or_tbl(圖/表)
        DataTable materialrequirementplanning_Detail(string item, string start, string end, dekModel img_or_tbl);
        DataTable materialrequirementplanning_Detail(string item, DateTime start, DateTime end, dekModel img_or_tbl);

        //供應商達交率 start(開始時間) end(結束時間)
        DataTable Supplierscore(string start, string end);
        DataTable Supplierscore(DateTime start, DateTime end);

        //供應商 start(開始時間) end(結束時間) custom(客戶名稱)
        DataTable Supplierscore_Detail(string start, string end, string custom);
        DataTable Supplierscore_Detail(DateTime start, DateTime end, string custom);

        //供應商催料總表 type(收貨單/採購單) supplier(供應商) supplierName(供應商簡稱) searchDate(常駐"") start(開始時間) end(結束時間) itemNO(品號) Reminder_Date(收貨單→催料單號/採購單→計算後日期)
        DataTable SupplierShortage(SupplierShortageType type, string supplier, string supplierName, string searchDate, string start, string end, string itemNo, string Reminder_Date);

        //取得CheckBoxList RadioButtonList DropDownList的內容
        DataTable Item_DataTable(string ini_Name, string start = "", string end = "");
    }

    //倉管部
    interface IdepHouse
    {
        //成品庫存分析 img_or_tbl(圖/表) start(開始時間) end(結束時間) day(逾期天數)  type(資料庫是否存在入庫日 0->沒有，抓生產部的入庫日 1->有，抓他的)
        DataTable stockanalysis(dekModel img_or_tbl, int day, string type);

        //成品庫存分析細節  start(開始時間) end(結束時間) custom(客戶名稱)
        DataTable stockanalysis_Details(string start, string custom);
        DataTable stockanalysis_Details(DateTime start, string custom);

        //呆滯物料統計表  item_type(物料類別) day(期限日) warehouse(倉庫位置) item_num(物料編號)
        DataTable InactiveInventory(string item_type, string warehouse, string day);

        //報廢數量統計表 start(開始時間) end(結束時間) condition(報廢人員的SQL語法)
        DataTable Scrapped(string start, string end, string condition);
        DataTable Scrapped(DateTime start, DateTime end, string condition);
    }

    //生產部
    interface IdepProduct
    {
        DataTable waitingfortheproduction(string start, string end);
        DataTable waitingfortheproduction(DateTime start, DateTime end);
    }

    //加工部
    interface IdenMachine
    {

    }

    //---------------------------------------------新的方式------------------------------------------------------
    //業務部
    interface Get_Sales
    {
        /// <summary>
        /// 訂單及數量統計明細
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="status">訂單狀態(0全部 1已結 2未結 3已排程但未結 4未排程且未結)</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <param name="custom">客戶名稱(可不填)</param>
        /// <returns></returns>
        DataTable Orders_Detail(DateTime start, DateTime end, OrderStatus status, string source, string custom = "");
        DataTable Orders_Detail(string start, string end, OrderStatus status, string source, string custom = "");

        /// <summary>
        /// 訂單逾期數量明細
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <param name="custom">客戶名稱(可不填)</param>
        /// <returns></returns>
        DataTable Orders_Over_Detail(DateTime start, string source, string custom = "");
        DataTable Orders_Over_Detail(string start, string source, string custom = "");

        /// <summary>
        /// 出貨統計明細
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <param name="custom">客戶名稱(可不填)</param>
        /// <returns></returns>
        DataTable Shipment_Detail(DateTime start, DateTime end, string source, string custom = "");
        DataTable Shipment_Detail(string start, string end, string source, string custom = "");

        /// <summary>
        /// 出貨統計表小計
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="custom">客戶名稱</param>
        /// <param name="item">料號</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable Get_Shipment(DateTime start, DateTime end, string custom, string item, string source);
        DataTable Get_Shipment(string start, string end, string custom, string item, string source);

        /// <summary>
        /// 未交易客戶
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="symbol">符號(><)</param>
        /// <param name="day">天數(ex：365)</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable UntradedCustomer(DateTime start, DateTime end, string symbol, int day, string source);
        DataTable UntradedCustomer(string start, string end, string symbol, int day, string source);

        /// <summary>
        /// 運輸架未歸還統計
        /// </summary>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable Transportrackstatistics(string acc, string source);

        /// <summary>
        /// 訂單變更紀錄
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable Recordsofchangetheorder_Details(DateTime start, DateTime end, string source);
        DataTable Recordsofchangetheorder_Details(string start, string end, string source);
    }

    //資材部
    interface Get_Material
    {
        /// <summary>
        /// 供應商達交率明細
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <param name="custom">客戶名稱(可不填)</param>
        /// <returns></returns>
        DataTable Supplierscore_Detail(DateTime start, DateTime end, string source, string custom = "");
        DataTable Supplierscore_Detail(string start, string end, string source, string custom = "");

        /// <summary>
        /// 供應商催料總表
        /// </summary>
        /// <param name="type">單據類型(0收貨單 1採購單)</param>
        /// <param name="supplier">供應商代碼</param>
        /// <param name="supplierName">供應商簡稱</param>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="itemNo">催料單號</param>
        /// <param name="Reminder_Date">催料品項</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable SupplierShortage(SupplierShortageType type, string supplier, string supplierName, DateTime start, DateTime end, string itemNo, string Reminder_Date, string source);
        DataTable SupplierShortage(SupplierShortageType type, string supplier, string supplierName, string start, string end, string itemNo, string Reminder_Date, string source);

        /// <summary>
        /// 物料領用統計表明細
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="rbx">RadioButtonList的ID</param>
        /// <param name="item">輸入的名稱</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param> 
        /// <returns></returns>
        DataTable materialrequirementplanning_Detail(DateTime start, DateTime end, RadioButtonList rbx, string item, string source);
        DataTable materialrequirementplanning_Detail(string start, string end, RadioButtonList rbx, string item, string source);

        /// <summary>
        /// 產生領料單表頭
        /// </summary>
        /// <param name="Number">刀庫編號 OR 刀庫編號#刀庫編號#(須轉List)</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param> 
        /// <returns></returns>
        DataTable pick_list_title(string Number, string source);

        /// <summary>
        /// 產生領料單明細
        /// </summary>
        /// <param name="Number">刀庫編號 OR 刀庫編號#刀庫編號#(須轉List)</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param> 
        /// <returns></returns>
        DataTable pick_list_datatable(string Number, string source);

        /// <summary>
        /// 取得CheckBoxList RadioButtonList DropDownList的內容
        /// </summary>
        /// <param name="ini_Name">ini的值</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <param name="start">開始時間(可不填)</param>
        /// <param name="end">結束時間(可不填)</param>
        /// <returns></returns>
        DataTable Item_DataTable(string ini_Name, string source, string start = "", string end = "");
    }

    //倉管部
    interface Get_House
    {
        /// <summary>
        /// 成品庫存明細
        /// </summary>
        /// <param name="start">開始天數</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <param name="custom">客戶名稱(可不填)</param>
        /// <returns></returns>
        DataTable stockanalysis_Details(string source, string custom = "");

        /// <summary>
        /// 庫存明細列表
        /// </summary>
        /// <param name="cbx">物料倉儲位置</param>
        /// <returns></returns>
        DataTable Inventory_Total_Amount(CheckBoxList warehouse, string source);

        /// <summary>
        /// 呆滯物料統計表
        /// </summary>
        /// <param name="item_type">物料類別</param>
        /// <param name="warehouse">物料倉儲位置</param>
        /// <param name="day">小於某天</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable InactiveInventory(CheckBoxList item_type, CheckBoxList warehouse, string day, string source);

        /// <summary>
        /// 報廢數量統計表
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="condition">報廢人員語法( (... OR ....) )</param>
        /// <param name="source">來源( dek 德科,sowon 首旺,itec 鉅茂,....→讀取ini用)</param>
        /// <returns></returns>
        DataTable Scrapped(DateTime start, DateTime end, CheckBoxList Scrapped_personnel, string source);
        DataTable Scrapped(string start, string end, CheckBoxList Scrapped_personnel, string source);
    }

    //生產部
    interface Get_Product
    {
        DataTable KnifeSet(string start, string end, string source);
        DataTable KnifeSet(DateTime start, DateTime end, string source);
    }

    //加工部
    interface Get_cnc
    {
        /// <summary>
        /// 取得目前所有排程的情況
        /// </summary>
        /// <param name="source">預設cnc</param>
        /// <param name="list">機台群組清單</param>
        /// <returns></returns>
        DataTable All_Schdule(string source = "cnc", List<string> list = null);

        /// <summary>
        /// 取得狀態為非出站且為進站報工的項目
        /// </summary>
        /// <param name="source">預設cnc</param>
        /// <param name="list">機台群組清單</param>
        /// <returns></returns>
        DataTable Enter_ReportView(string source = "cnc", List<string> list = null);

        /// <summary>
        /// 取得狀態為非出站且為進站維護的項目
        /// </summary>
        /// <param name="source">預設cnc</param>
        /// <param name="list">機台群組清單</param>
        /// <returns></returns>
        DataTable Enter_MaintainView(string source = "cnc", List<string> list = null);

        /// <summary>
        /// 取得系統預設顯示之欄位
        /// </summary>
        /// <param name="usepage">使用頁面</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable System_columns(string usepage, string source = "cnc");
        /// <summary>
        /// 取得個人設定之欄位
        /// </summary>
        /// <param name="usepage">使用頁面</param>
        /// <param name="acc">使用者帳號</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Person_columns(string usepage, string acc, string source = "cnc");
        /// <summary>
        /// 取得異常類型
        /// </summary>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable ErrorType(string source = "cnc");

        /// <summary>
        /// 取得資料表資料
        /// </summary>
        /// <param name="dt_name">資料表名稱</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Get_DataTable(string dt_name, string source = "cnc");

        /// <summary>
        /// 取得某資料表最ID最大值
        /// </summary>
        /// <param name="dt_name">資料表名稱</param>
        /// <param name="column">欄位</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Get_IDMax(string dt_name, string column, string source = "cnc");

        /// <summary>
        /// 儲存需顯示之欄位
        /// </summary>
        /// <param name="acc">帳號</param>
        /// <param name="usepage">使用頁面</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Save_Cloumn(string acc, string usepage, string source = "cnc");

        /// <summary>
        /// 取得不良品相關訊息
        /// </summary>
        /// <param name="type">類型 進站維護/進站報工</param>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="condition">參數 廠區之類的...</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Des_Defective(string type, string start, string end, string condition, string source = "cnc");

        /// <summary>
        /// 取得生產相關訊息
        /// </summary>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="type">類型 進站維護/進站報工</param>
        /// <param name="condition">參數 廠區之類的...</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Des_Maintain(string start, string end, string type, string condition, string source = "cnc");

        /// <summary>
        /// 取得生產相關訊息
        /// </summary>
        /// <param name="type">類型 進站維護/進站報工</param>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <param name="condition">參數 廠區之類的...</param>
        /// <param name="source">預設cnc</param>
        /// <returns></returns>
        DataTable Des_Pause(string type, string start, string end, string condition, string source = "cnc");
    }

    //客製化部分
    interface Get_Customized
    {
        /// <summary>
        /// 麗馳客製化部分-取得目前料號詳細資料
        /// </summary>
        /// <param name="itemname">料號</param>
        /// <returns></returns>
        DataTable Stock_Details(string itemname);

        /// <summary>
        /// 麗馳客製化部分-取得該機型的領料資料
        /// </summary>
        /// <param name="ordernumber">製令號</param>
        /// <param name="item">料號</param>
        /// <param name="itemname">品名</param>
        /// <returns></returns>
        DataTable Picking_List(string ordernumber,string item,string itemname);

        /// <summary>
        /// 麗馳客製化部分-應收帳款未收明細查詢
        /// </summary>
        /// <returns></returns>
        DataTable Account_Outstanding_Details();
    }
}
