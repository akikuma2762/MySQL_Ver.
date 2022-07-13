using dek_erpvis_v2.cls;
using Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using dekERP_dll.dekErp;
using Newtonsoft.Json;
using System.ComponentModel;

namespace dek_erpvis_v2.cls
{

}

public class HtmlUtil
{
    public static string GetConnByDekVisErp = myclass.GetConnByDekVisErp;

    //把等號兩邊做成制式化格式
    public static string AttibuteValue(string item, string value, string quotsymbol = "'")
    {
        return $"{item}={quotsymbol}{value}{quotsymbol}";
    }
    //把文字加底線拉、斜體等等
    public static string ToTag(string tagname, string value)
    {
        return $"<{tagname}>{value}</{tagname}>";
    }
    //html的格式
    public static string ToHref(string text, string href, string target = "", string title = "")
    {
        // <a href="要連結的 URL 放這裡" target="連結目標" title="連結替代文字"> 要顯示的連結文字或圖片放這裡 </a>
        string result = "<a " + AttibuteValue("href", href);
        if (target != "")
            result += AttibuteValue("target", target);
        if (title != "")
            result += AttibuteValue("title", title);
        result += string.Format(" >{0}</a>", text);
        return result;
    }
    //印出每個欄位的名稱(dt→表格,field_name→迴圈的欄位名稱,Add_title→與輸出表格不符合時，要新增的欄位名稱)
    public static string Set_Table_Title(DataTable dt, out string field_name, string Add_title = "", string Jump_field = "")//Add_title→要印出之列的欄位名稱
    {
        string th = "";
        string title_name = "";//紀錄每個資料欄位名稱用
        string col_name = "";
        //dt與輸出表格相符
        if (Add_title == "")
        {
            //沒有資料的處理方式
            if (dt.Rows.Count <= 0)
            {
                th = "<th class=\"center\">沒有資料載入</th>";
                title_name = "";
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (Jump_field.Contains(dt.Columns[i].ColumnName) == false)
                    {
                        col_name = dt.Columns[i].ColumnName;
                        th += $"<th>{col_name}</th>";
                        title_name += col_name + ",";
                    }

                }
            }
        }
        //dt與輸入表格不符(要新增欄位名稱)
        else
        {
            //沒有資料的處理方式
            if (dt.Rows.Count <= 0)
            {
                th = "<th class=\"center\">沒有資料載入</th>";
                title_name = "";
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    //if (col_name != DataTableUtils.toString(row[Add_title]))
                    //{
                    //    col_name = DataTableUtils.toString(row[Add_title]);
                    //    title_name += col_name + ",";
                    //    th += $"<th>{col_name}</th>";
                    //}
                    if (row[Add_title] != null)
                    {
                        if (col_name != DataTableUtils.toString(row[Add_title]))
                        {
                            col_name = DataTableUtils.toString(row[Add_title]);
                            title_name += col_name + ",";
                            th += $"<th>{col_name}</th>";
                        }
                    }
                }
            }
        }
        field_name = title_name;
        return th;
    }

    //利用表格以及字串去獲取每個欄位內容(dt→表格,第二個是欄位名稱,callback事件)
    public static string Set_Table_Content(DataTable dt, string[] TitleList, Func<DataRow, string, string> callback = null)
    {
        string tr = "";
        string field_value;
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                tr += "<tr>";
                for (int i = 0; i < TitleList.Length - 1; i++)
                {
                    field_value = "";
                    if (callback != null)
                        field_value = callback(row, TitleList[i]);
                    if (field_value == "" || field_value == "<td></td>")
                        field_value = $"<td>{DataTableUtils.toString(row[TitleList[i]])}</td>";

                    tr += field_value;
                }
                tr += "</tr>";
            }
        }
        else
        {
            tr = "<tr> <td class=\"center\"> no data </td></tr>";
        }
        return tr;
    }
    public static string Set_Table_Content(DataTable dt, string Title_comma_text, Func<DataRow, string, string> callback = null)
    {
        return Set_Table_Content(dt, Title_comma_text.Split(','), callback);
    }
    public static string Set_Table_Content(DataTable dt, List<string> TitleList, Func<DataRow, string, string> callback = null)
    {
        return Set_Table_Content(dt, TitleList.ToArray(), callback);
    }
    public static StringBuilder Set_Table_Title(bool ok, DataTable dt, out string field_name, string Add_title = "", string Jump_field = "")
    {
        StringBuilder th = new StringBuilder();
        string title_name = "";//紀錄每個資料欄位名稱用
        string col_name = "";
        //dt與輸出表格相符
        if (Add_title == "")
        {
            //沒有資料的處理方式
            if (dt.Rows.Count <= 0)
            {
                th.Append("<th class=\"center\">沒有資料載入</th>");
                title_name = "";
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (Jump_field.Contains(dt.Columns[i].ColumnName) == false)
                    {
                        col_name = dt.Columns[i].ColumnName;
                        th.Append($"<th>{col_name}</th>");
                        title_name += col_name + ",";
                    }

                }
            }
        }
        //dt與輸入表格不符(要新增欄位名稱)
        else
        {
            //沒有資料的處理方式
            if (dt.Rows.Count <= 0)
            {
                th.Append("<th class=\"center\">沒有資料載入</th>");
                title_name = "";
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    //if (col_name != DataTableUtils.toString(row[Add_title]))
                    //{
                    //    col_name = DataTableUtils.toString(row[Add_title]);
                    //    title_name += col_name + ",";
                    //    th += $"<th>{col_name}</th>";
                    //}
                    if (row[Add_title] != null)
                    {
                        if (col_name != DataTableUtils.toString(row[Add_title]))
                        {
                            col_name = DataTableUtils.toString(row[Add_title]);
                            title_name += col_name + ",";
                            th.Append($"<th>{col_name}</th>");
                        }
                    }
                }
            }
        }
        field_name = title_name;
        return th;
    }
    public static StringBuilder Set_Table_Title(List<string> list, string style = "")
    {
        StringBuilder th = new StringBuilder();
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count - 1; i++)
                th.Append($"<th {style}>{list[i]}</th>");
        }
        else
            th.Append("<th class=\"center\">沒有資料載入</th>");
        return th;
    }
    public static StringBuilder Set_Table_Content(bool ok, DataTable dt, List<string> TitleList, Func<DataRow, string, string> callback = null)
    {
        return Set_Table_Content(ok, dt, TitleList.ToArray(), callback);
    }
    public static StringBuilder Set_Table_Content(bool ok, DataTable dt, string[] TitleList, Func<DataRow, string, string> callback = null)
    {
        StringBuilder tr = new StringBuilder();
        string field_value;
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                tr.Append("<tr>");
                for (int i = 0; i < TitleList.Length - 1; i++)
                {
                    field_value = "";
                    if (callback != null)
                        field_value = callback(row, TitleList[i]);
                    if (field_value == "" || field_value == "<td></td>")
                        field_value = $"<td style=\"vertical-align: middle; text-align: center;\">{DataTableUtils.toString(row[TitleList[i]])}</td>";

                    tr.Append(field_value);
                }
                tr.Append("</tr>");
            }
        }
        else
        {
            tr.Append("<tr> <td style=\"vertical-align: middle; text-align: center;\"> no data </td></tr>");
        }
        return tr;
    }
    public static StringBuilder Set_Table_Content(bool ok, DataTable dt, string Title_comma_text, Func<DataRow, string, string> callback = null)
    {
        return Set_Table_Content(ok, dt, Title_comma_text.Split(','), callback);
    }

    //沒有資料時顯示
    public static string NoData(out string th, out string tr)
    {
        th = "<th class=\"center\">沒有資料載入</th>";
        tr = "<tr> <td class=\"center\"> no data </td></tr>";
        return "'沒有資料'";
    }
    public static string NoData(out StringBuilder th, out StringBuilder tr)
    {
        StringBuilder ta = new StringBuilder();
        StringBuilder tb = new StringBuilder();

        ta.Append("<th  class=\"center\">沒有資料載入</th>");
        tb.Append("<tr> <td class=\"center\"> no data </td></tr>");

        th = ta;
        tr = tb;

        return "'沒有資料'";
    }
    //繪製長條圖(dt→表格,x_value→x軸,y_value→y軸,unit→單位,backvalue→合計數量/金額)
    public static string Set_Chart(DataTable dt, string x_value, string y_value, string unit, out int backvalue, int total = 0, bool sort = false)
    {
        //dt重複值合併
        DataTable ds = new DataTable();
        for (int i = 0; i < dt.Columns.Count; i++)
            ds.Columns.Add(DataTableUtils.toString(dt.Columns[i]));

        DataTable Line = dt.DefaultView.ToTable(true, new string[] { x_value });
        double count = 0;
        string sqlcmd = "";
        foreach (DataRow row in Line.Rows)
        {
            DataRow rew = ds.NewRow();
            count = 0;
            sqlcmd = x_value + " ='" + DataTableUtils.toString(row[x_value]) + "'";
            DataRow[] rows = dt.Select(sqlcmd);
            for (int i = 0; i < rows.Length; i++)
                count += DataTableUtils.toDouble(DataTableUtils.toString(rows[i][y_value]));

            for (int i = 0; i < ds.Columns.Count; i++)
            {
                object obj = ds.Columns[i];

                if (DataTableUtils.toString(ds.Columns[i]) == y_value)
                    rew[ds.Columns[i]] = count;
                else
                    rew[ds.Columns[i]] = rows[0][DataTableUtils.toString(ds.Columns[i])];
            }
            ds.Rows.Add(rew);
        }

        dt = ds;


        string value = "";
        string x_text;
        string y_text;
        int add_value = 0;

        if (sort)
        {
            DataTable dt_clone = dt.Clone();
            dt_clone.Columns[y_value].ReadOnly = false;
            dt_clone.Columns[y_value].DataType = Type.GetType("System.Double");
            foreach (DataRow row in dt.Rows)
                dt_clone.ImportRow(row);


            DataView dv = new DataView(dt_clone);
            dv.Sort = $"{y_value} desc";
            dt_clone = dv.ToTable();
            dt = dt_clone;
        }


        int x = 0;
        //先針對暫停原因跟生產資訊做處理後續再想通用 0608 juiedit
        //string tootiptype = "";
        //string tootipvalue = "";
        foreach (DataRow row in dt.Rows)
        {
            x_text = DataTableUtils.toString(row[x_value]);
            y_text = (DataTableUtils.toString(row[y_value])).Split('.')[0];
            //tootiptype = y_value.Contains("數") ? "時間" : "數量";
            //if (y_value.Contains("數"))
            //{
            //    if (y_value == "次數")
            //        tootipvalue = y_value.Contains("數") ? row["暫停工時(分)"].ToString() : row[y_value].ToString();
            //    else if (y_value.Contains("不良"))
            //        tootipvalue = y_text;
            //    else
            //        tootipvalue = y_value.Contains("數") ? row["生產工時(分)"].ToString() : row[y_value].ToString();
            //}
            //if (string.IsNullOrEmpty(tootipvalue))
            //    tootipvalue = "0";

            add_value += DataTableUtils.toInt(y_text);
            // if (y_value.Contains("不良"))
            value += "{" + $"y: {y_text}, label:'{Change_ColumnName(x_text)}',indexLabel:'{ y_text}{unit}'" + "},";
            //else
            //    value += "{" + $"y: {y_text}, label:'{Change_ColumnName(x_text)}',indexLabel:'{ y_text}{unit}', title1:'{y_value}',title2:'{tootiptype}',value2:'{tootipvalue}'" + "},";
            x++;
            if (x == total - 1)
                break;
        }
        backvalue = add_value;
        return value;
    }
    //繪製長條圖(dt→表格,x_value→x軸,y_value→y軸,unit→單位,backvalue→合計數量/金額)
    public static string Set_Chart(DataTable dt, string x_value, string y_value, string unit, out int backvalue, out List<string> LineName, List<string> LineNum = null, bool show_number = true)
    {
        if (!Check_DataTable(dt))
        {
            backvalue = 0;
            LineName = null;
            return "";
        }


        List<string> Returt_Line = new List<string>();
        //dt重複值合併
        DataTable ds = new DataTable();
        for (int i = 0; i < dt.Columns.Count; i++)
            ds.Columns.Add(DataTableUtils.toString(dt.Columns[i]));

        DataTable Line = dt.DefaultView.ToTable(true, new string[] { x_value });
        double count = 0;
        string sqlcmd = "";
        foreach (DataRow row in Line.Rows)
        {
            DataRow rew = ds.NewRow();
            count = 0;
            sqlcmd = $"{x_value}='{DataTableUtils.toString(row[x_value])}'";
            DataRow[] rows = dt.Select(sqlcmd);
            for (int i = 0; i < rows.Length; i++)
                count += DataTableUtils.toDouble(DataTableUtils.toString(rows[i][y_value]));

            for (int i = 0; i < ds.Columns.Count; i++)
            {
                object obj = ds.Columns[i];

                if (DataTableUtils.toString(ds.Columns[i]) == y_value)
                    rew[ds.Columns[i]] = count;
                else
                    rew[ds.Columns[i]] = rows[0][DataTableUtils.toString(ds.Columns[i])];
            }
            ds.Rows.Add(rew);
        }

        dt = ds;


        string value = "";
        string x_text;
        string y_text;
        int add_value = 0;
        if (LineNum == null)
        {
            foreach (DataRow row in dt.Rows)
            {
                string index = "";
                x_text = DataTableUtils.toString(row[x_value]);
                y_text = (DataTableUtils.toString(row[y_value])).Split('.')[0];
                add_value += DataTableUtils.toInt(y_text);
                if (show_number)
                    index = ",indexLabel:'" + y_text + unit + "'";
                if (y_text != "0")
                    value += "{ y: " + y_text + ", label: '" + Change_ColumnName(x_text) + "'" + index + " },";
                else
                    value += "{ y: " + y_text + ", label: '" + Change_ColumnName(x_text) + "' },";

                Returt_Line.Add(Change_ColumnName(x_text));

            }

        }
        else
        {
            //再算前n名的有異常
            for (int i = 0; i < LineNum.Count; i++)
            {
                sqlcmd = $"{x_value}='{LineNum[i]}'";
                DataRow[] row = dt.Select(sqlcmd);
                if (row != null && row.Length > 0)
                {
                    string index = "";
                    x_text = DataTableUtils.toString(row[0][x_value]);
                    y_text = (DataTableUtils.toString(row[0][y_value])).Split('.')[0];
                    if (show_number)
                        index = ",indexLabel:'" + y_text + unit + "'";


                    if (y_text != "0")
                        value += "{ y: " + y_text + ", label: '" + Change_ColumnName(x_text) + "' " + index + " },";
                    else
                        value += "{ y: " + y_text + ", label: '" + Change_ColumnName(x_text) + "' },";
                }
                else
                    value += "{ y: 0, label: '" + Change_ColumnName(LineNum[i]) + "' },";
            }
            foreach (DataRow row in dt.Rows)
                add_value += DataTableUtils.toInt((DataTableUtils.toString(row[y_value])).Split('.')[0]);
        }


        LineName = Returt_Line;
        backvalue = add_value;
        return value;
    }
    public static string Set_Image(List<string> list_x, List<string> list_y, out int backvalue)
    {
        string value = "";
        int add_value = 0;
        for (int i = 0; i < list_x.Count; i++)
        {
            add_value += DataTableUtils.toInt(list_y[i + 1]);
            value += "{ y: " + list_y[i + 1] + ", label: '" + list_x[i] + "',indexLabel:' " + list_y[i + 1] + "' },";
        }
        backvalue = add_value;
        return value;
    }

    //按鈕的事件
    public static string Button_Click(string btnID, string[] s, string txt_time_str, string txt_time_end, out string date_str, out string date_end)
    {
        string dt_st = "";
        string dt_ed = "";
        string wairning = "";
        switch (btnID)
        {
            case "week":
                dt_st = DateTime.Now.Date.AddDays(-(int)(DateTime.Now.DayOfWeek) + 1).ToString("yyyyMMdd");//当前周的开始日期
                dt_ed = DateTime.Now.Date.AddDays(7 - (int)(DateTime.Now.DayOfWeek)).ToString("yyyyMMdd");//当前周的结束日期
                break;
            case "month":
                dt_st = s[0];
                dt_ed = s[1];
                break;
            case "firsthalf":
                dt_st = DateTime.Now.ToString("yyyy0101");
                dt_ed = DateTime.Now.ToString("yyyy0630");
                break;
            case "lasthalf":
                dt_st = DateTime.Now.ToString("yyyy0701");
                dt_ed = DateTime.Now.ToString("yyyy1231");
                break;
            case "year":
                dt_st = DateTime.Now.ToString("yyyy0101");
                dt_ed = DateTime.Now.ToString("yyyy1231");
                break;
            case "select":
                DateTime d_st = DateTime.ParseExact(DataTableUtils.toString(txt_time_str), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime d_ed = DateTime.ParseExact(DataTableUtils.toString(txt_time_end), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                dt_st = d_st.ToString("yyyyMMdd");
                dt_ed = d_ed.ToString("yyyyMMdd");
                break;
        }
        date_str = dt_st;
        date_end = dt_ed;
        return wairning;
    }

    /// <summary>
    /// 變更時間顯示方式
    /// </summary>
    /// <param name="date">日期</param>
    /// <param name="symbol">格式</param>
    /// <returns></returns>
    public static string changetimeformat(string date, string symbol = "/")
    {
        if (date != "" && date.Length == 8)
        {
            date = date.Insert(6, symbol);
            date = date.Insert(4, symbol);
        }
        return date;
    }

    /// <summary>
    /// 套用使用者所選之CSS
    /// </summary>
    /// <param name="acc">帳號</param>
    /// <returns></returns>
    public static string change_color(string acc)
    {
        clsDB_Server clsDB = new clsDB_Server(GetConnByDekVisErp);
        string color = "";
        clsDB.dbOpen(myclass.GetConnByDekVisErp);
        string sqlcmd = $"SELECT * FROM Account_Image where Account_name = '{acc}' and Image_link is null";
        DataTable dt = clsDB.DataTable_GetTable(sqlcmd);
        if (Check_DataTable(dt))
            color = DataTableUtils.toString(dt.Rows[0]["background"]);
        else
            color = "custom";

        if (color == "custom")
            return $"<link href=\"../../assets/build/css/{color}.css\" rel=\"stylesheet\" /> \n <link href=\"../../assets/build/css/Change_Table_Button.css\" rel=\"stylesheet\" /> \n ";
        else if (color == "custom_old")
            return $"<link href=\"../../assets/build/css/{color}.css\" rel=\"stylesheet\" /> \n <link href=\"../../assets/build/css/Change_Table_Button_old.css\" rel=\"stylesheet\"  /> \n ";
        else
            return $"<link href=\"../../assets/build/css/{color}.css\" rel=\"stylesheet\" /> \n <link href=\"../../assets/build/css/Change_Table_Button_person.css\" rel=\"stylesheet\" /> \n ";
    }
    public static void initColorSet2DB(string acc, string type = "custom_old")
    {
        clsDB_Server clsDB = new clsDB_Server(myclass.GetConnByDekVisErp);
        clsDB.dbOpen(myclass.GetConnByDekVisErp);
        DataTable dt = clsDB.DataTable_GetTable($"select * from Account_Image order by 'UID' desc ", 0, 1);
        bool OK = false;
        if (HtmlUtil.Check_DataTable(dt))
        {
            DataRow dr = dt.NewRow();
            dr["UID"] = (DataTableUtils.toInt(dt.Rows[0]["UID"].ToString()) + 1).ToString();
            dr["Account_name"] = acc;
            dr["background"] = "custom_old";
            OK = clsDB.Insert_DataRow("Account_Image", dr);
        }
    }

    /// <summary>
    /// 紀錄網頁所載入的時間
    /// </summary>
    /// <param name="acc">帳號</param>
    /// <param name="page">頁面名稱</param>
    /// <param name="start_time">開始時間</param>
    /// <param name="end_time">結束時間</param>
    /// <param name="bowser">用的瀏覽器</param>
    public static void Time_Look(string acc, string page, DateTime start_time, DateTime end_time, string bowser)
    {
        clsDB_Server clsDB = new clsDB_Server(myclass.GetConnByDekVisErp);
        clsDB.dbOpen(myclass.GetConnByDekVisErp);
        //表示有進入Page_Load
        if (end_time == DateTime.Parse("1990/01/01 上午 00:00:00"))
        {
            string sqlcmd = "SELECT * FROM Time_Look";
            DataTable dt = clsDB.DataTable_GetTable(sqlcmd);
            if (dt != null)
            {
                DataRow row = dt.NewRow();
                row["Account"] = acc;
                row["Page_name"] = page;
                row["Load_Time"] = start_time;
                row["Start_Time"] = start_time;
                row["Browser_Device"] = bowser;
                if (clsDB.Insert_DataRow("Time_Look", row))
                {

                }
            }
        }
        else
        {
            string sqlcmd = $"SELECT * FROM Time_Look where Account='{acc}' and Page_name = '{page}' and Start_Time = '{start_time}'";
            DataTable dt = clsDB.DataTable_GetTable(sqlcmd);
            if (Check_DataTable(dt))
            {
                DataRow row = dt.NewRow();
                row["Account"] = acc;
                row["Page_name"] = page;
                row["Load_Time"] = start_time;
                row["Start_Time"] = start_time;
                row["End_Time"] = end_time;
                TimeSpan ts = end_time - start_time;
                row["Count_Time"] = ts.TotalSeconds.ToString();
                row["Browser_Device"] = bowser;
                if (clsDB.Update_DataRow("Time_Look", $" Account='{acc}' and Page_name = '{page}' and Start_Time = '{start_time}'", row))
                {

                }
            }
        }

    }

    //回傳字串分割的陣列(舊的方式，之後會慢慢移除)
    public static string[] Return_str(string value, string key = "")
    {
        string keyword = ConfigurationManager.AppSettings["URL_ENCODE"];
        string[] str = null;
        if (keyword == "1")
        {
            if (key == "")
                value = WebUtils.UrlStringDecode(value);
        }
        value = value.Trim();
        value = value.Replace(",", "^").Replace("=", "^");
        str = value.Split('^');
        return str;
    }

    /// <summary>
    /// 把網址內的參數轉換成Dictionary  
    /// </summary>
    /// <param name="value">Request.QueryString["key"]</param>
    /// <param name="key">是否有進行加密過</param>
    /// <returns></returns>
    public static Dictionary<string, string> Return_dictionary(string value, string key = "")
    {
        Dictionary<string, string> url_dictionary = new Dictionary<string, string>();
        string keyword = WebUtils.GetAppSettings("URL_ENCODE");
        List<string> list = new List<string>();
        if (keyword == "1")
        {
            if (key == "")
                value = WebUtils.UrlStringDecode(value);
        }
        value = value.Trim();
        value = value.Replace(",", "^").Replace("=", "^");
        list = new List<string>(value.Split('^'));
        for (int i = 0; i < list.Count; i++)
        {
            if (i % 2 == 0)
                url_dictionary.Add(list[i], list[i + 1]);
        }
        return url_dictionary;
    }

    /// <summary>
    /// 查詢Dictonary的值(沒有回傳"")
    /// </summary>
    /// <param name="keyValues">Dictonary的變數名稱</param>
    /// <param name="value">欲查詢的值</param>
    /// <returns></returns>
    public static string Search_Dictionary(Dictionary<string, string> keyValues, string value)
    {
        try
        {
            return keyValues[value];
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// 可上傳之文件格式
    /// </summary>
    /// <returns></returns>
    public static string Check_File()
    {
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        string sqlcmd = "SELECT Name from File_Extension where Open_YN = 'Y'";
        DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
        string file_name = "";
        if (Check_DataTable(dt))
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                    file_name += DataTableUtils.toString(dt.Rows[i]["Name"]);
                else
                    file_name += "|" + DataTableUtils.toString(dt.Rows[i]["Name"]);
            }
        }
        return file_name;
    }

    /// <summary>
    /// 查詢帳號對應之單一欄位(user的資料表 部門資料表)
    /// </summary>
    /// <param name="acc">帳號</param>
    /// <param name="field">欄位名稱</param>
    /// <returns></returns>
    public static string Search_acc_Column(string acc, string field = "power")
    {
        if (acc != "")
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = $"Select * From Users left join department on department.DPM_NAME = Users.USER_DPM  where User_acc = '{acc}' OR USER_NAME='{acc}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            if (Check_DataTable(dt))
                return DataTableUtils.toString(dt.Rows[0][field]);
            else
                return "";
        }
        else
            return "";
    }

    /// <summary>
    /// 上傳圖片用(槽區在Webconfig設定)
    /// </summary>
    /// <param name="file">FileUpload元件</param>
    /// <param name="road">路徑</param>
    /// <param name="recover">是否可覆蓋</param>
    /// <param name="Machine">上傳單一圖片的檔名</param>
    /// <returns></returns>
    public static string FileUpload_Name(FileUpload file, string road, bool recover = false, string Machine = "")
    {
        string Image_Save = "";
        if (file.FileName != "")
        {
            //表示不要被覆蓋(例如異常圖片之類的不可覆蓋)
            if (recover == false)
            {
                foreach (HttpPostedFile postedFile in file.PostedFiles)
                {
                    string ext = Path.GetExtension(postedFile.FileName).Replace(".", "");
                    Regex regex = new Regex(@"^" + Check_File() + "$", RegexOptions.IgnoreCase);
                    bool ok = regex.IsMatch(ext);
                    if (ok == true)
                        Image_Save += checkImage(postedFile, Image_Save, ext, road);
                    else
                        return "檔案有誤，請重新上傳";
                }
            }
            else//表示可以覆蓋(像是機台或是機型照片那種唯一的)
            {
                foreach (HttpPostedFile postedFile in file.PostedFiles)
                {
                    string path = WebConfigurationManager.AppSettings["disk"] + ":\\" + road + "\\";
                    string ext = Path.GetExtension(postedFile.FileName).Replace(".", "");
                    Regex regex = new Regex(@"^" + Check_File() + "$", RegexOptions.IgnoreCase);
                    bool ok = regex.IsMatch(ext);
                    if (ok == true)
                    {
                        if (Machine != "")
                            postedFile.SaveAs(path + Machine + ".jpg");
                        else
                            postedFile.SaveAs(path + postedFile.FileName);

                        Image_Save += "" + road + "/" + postedFile.FileName + "\n";
                    }
                }
                return Image_Save;
            }
        }
        else
            return "";

        return Image_Save;

    }
    //變更圖片名稱(改成日期)
    static string checkImage(HttpPostedFile postedFile, string Image_Save, string ext, string local)
    {
        string replace_name = "";
        string image_name = "";//最後回傳的
        string name = "";
        string path2 = WebConfigurationManager.AppSettings["disk"] + ":\\" + local + "\\";
        if (ext != "")
        {
            //找到目前時間
            string timenow = DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
            checkName(timenow, Image_Save, out replace_name);
            //加上副檔名
            replace_name += "." + ext;
            postedFile.SaveAs(path2 + replace_name);
            name = "" + local + "/" + replace_name;
            image_name = name + "\n";
        }
        return image_name;
    }
    //看看目前時間是否已被使用
    static string checkName(string name, string Image_Save, out string replace_name, int num = 1)
    {
        if (Image_Save.IndexOf(name) > 0)
        {
            name = name.Split('(')[0];
            name = name + "(" + num + ")";
            num++;
            checkName(name, Image_Save, out replace_name, num);
        }
        else
            replace_name = name;
        return name;
    }

    /// <summary>
    /// 檢查DataTable 是否為空或是null
    /// </summary>
    /// <param name="dt">資料表</param>
    /// <returns></returns>
    public static bool Check_DataTable(DataTable dt)
    {
        if (dt == null)
            return false;
        else if (dt.Rows.Count == 0)
            return false;

        //後續由DLL傳入的保險，可看語法 連結字串 錯誤 是否有結構產生用
        string data = "";
        try
        {
            data = DataTableUtils.toString(dt.Columns[0]);
        }
        catch
        {
            try
            {
                data = DataTableUtils.toString(dt.Columns[0]);
            }
            catch
            {
                data = "";
            }
        }

        if (dt != null && dt.Rows.Count > 0 && data != "語法")
            return true;
        else
            return false;
    }

    /// <summary>
    /// 儲存資訊到Cookies，以便上一頁時返回該頁(第4頁進入明細，返回上一頁時也會在第4頁)
    /// </summary>
    /// <param name="pagename">頁面名稱</param>
    /// <param name="value">值(通常為唯一碼，例如顧客/品號)</param>
    /// <returns></returns>
    public static HttpCookie Save_Cookies(string pagename, string value)
    {
        HttpCookie cookies = new HttpCookie(pagename);
        cookies[pagename + "_cust"] = value;
        cookies.Expires = DateTime.Now.AddDays(30);
        return cookies;
    }

    //為了加工可視化做的FUNCTION
    static string Change_ColumnName(string x_text)
    {
        if (x_text == "工藝名稱")
            return "產品名稱(運行程式)";
        else
            return x_text;
    }

    /// <summary>
    /// 字串轉時間
    /// </summary>
    /// <param name="_date">字串(長度為8 或 14碼)</param>
    /// <returns></returns>
    public static string StrToDates(string _date)
    {
        if (_date == "")
            return "";
        else
            return StrToDate(_date).ToString();
    }
    public static DateTime StrToDate(string _date)
    {
        if (_date != null && _date.Length < 14 && _date.Substring(0, 1) != "0")
            _date = _date + "080000";
        DateTime Trs;
        Trs = StrToDateTime(_date, "yyyyMMddHHmmss");

        return Trs;
    }
    public static DateTime StrToDateTime(string time, string Sourceformat)
    {
        try
        {
            return DateTime.ParseExact(time, Sourceformat, System.Globalization.CultureInfo.CurrentCulture);
        }
        catch
        {
            return new DateTime();
        }
    }

    /// <summary>
    /// 轉換成時間戳記(配合目前的gantt)
    /// </summary>
    /// <param name="time">時間</param>
    /// <returns></returns>
    public static string GetTimeStamp(DateTime time)
    {
        //  DateTime time = DateTime.Now;
        long ts = ConvertDateTimeToInt(time);
        return ts.ToString();
    }
    private static long ConvertDateTimeToInt(DateTime time)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
        return t;
    }

    /// <summary>
    /// 取得對應的入庫日(針對有排程編號，沒有寄入庫日之ERP)
    /// </summary>
    /// <param name="dt">資料表</param>
    /// <param name="day_before">在某天之前</param>
    /// <param name="istable">圖/表</param>
    /// <param name="isdetail">是否為明細</param>
    /// <returns></returns>
    public static DataTable Get_Warehousingdate(DataTable dt, string day_before, bool istable = false, bool isdetail = false)
    {
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
        string sqlcmd = "select 排程編號,substring(實際完成時間,1,8) as 實際完成時間 from 工作站狀態資料表";
        DataTable ds = DataTableUtils.GetDataTable(sqlcmd);

        if (Check_DataTable(ds))
        {
            //塞完入庫日
            foreach (DataRow row in dt.Rows)
            {
                sqlcmd = $"排程編號='{row["製令號"]}'";
                DataRow[] rows = ds.Select(sqlcmd);
                if (rows != null && rows.Length > 0)
                    row["入庫日"] = rows[0]["實際完成時間"].ToString();
            }

            if (!istable)
            {
                DataTable dt_return = new DataTable();
                dt_return.Columns.Add("產線群組");
                dt_return.Columns.Add("一般數量");
                dt_return.Columns.Add("逾期數量");
                dt_return.Columns.Add("總數量");

                DataTable Line = dt.DefaultView.ToTable(true, new string[] { "產線群組" });
                foreach (DataRow row in Line.Rows)
                {
                    int normal = 0, abnormal = 0;
                    sqlcmd = $"入庫日<='{day_before}' and 產線群組='{row["產線群組"]}'";
                    DataRow[] rows = dt.Select(sqlcmd);
                    if (rows != null && rows.Length > 0)
                        abnormal = rows.Length;

                    sqlcmd = $"入庫日>'{day_before}'and 產線群組='{row["產線群組"]}'";
                    rows = dt.Select(sqlcmd);
                    if (rows != null && rows.Length > 0)
                        normal = rows.Length;

                    dt_return.Rows.Add(DataTableUtils.toString(row["產線群組"]), normal, abnormal, normal + abnormal);
                }
                return dt_return;
            }
            else
            {
                //排除 入庫日>day_before→表示入庫時間在N天內
                sqlcmd = $"入庫日 >= '{day_before}'";
                DataRow[] row = dt.Select(sqlcmd);
                if (row != null && row.Length > 0)
                {
                    for (int i = 0; i < row.Length; i++)
                        row[i].Delete();
                }
                if (isdetail)
                    return dt;

                //先統計有幾個客戶
                DataTable custom = dt.DefaultView.ToTable(true, new string[] { "客戶簡稱" });

                //再統計有幾條產線
                DataTable Line = dt.DefaultView.ToTable(true, new string[] { "產線群組" });

                DataTable dt_return = new DataTable();
                dt_return.Columns.Add("客戶簡稱");
                dt_return.Columns.Add("產線群組");
                dt_return.Columns.Add("數量");

                foreach (DataRow rew in custom.Rows)
                {
                    foreach (DataRow rsw in Line.Rows)
                    {
                        sqlcmd = $" 客戶簡稱='{rew["客戶簡稱"]}' and 產線群組='{rsw["產線群組"]}' ";
                        DataRow[] rows = dt.Select(sqlcmd);
                        if (rows != null && rows.Length > 0)
                            dt_return.Rows.Add(DataTableUtils.toString(rew["客戶簡稱"]), DataTableUtils.toString(rsw["產線群組"]), rows.Length);
                    }
                }
                return dt_return;
            }

        }
        else
            return dt;
    }

    /// <summary>
    /// 控制人數登入的地方
    /// </summary>
    /// <param name="acc">帳號</param>
    /// <param name="page">頁面名稱</param>
    /// <param name="department">部門</param>
    /// <returns></returns>
    public static bool check_login(string acc, string page, string department = "")
    {
        int manage = 0;
        int product = 0;

        //找出目前該帳號申請的部門
        department = Search_acc_Column(acc, "USER_DPM");
        List<string> list_product = new List<string>();
        List<string> list_manage = new List<string>();

        //顯示目前生產部的登入人數
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        string sqlcmd = "select * from now_login left join department_relation on now_login.department = department_relation.code   where ispmd = 'PMD' ";
        DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

        if (Check_DataTable(dt))
        {
            product = dt.Rows.Count;
            //foreach (DataRow row in dt.Rows)
            //    list_product.Add(DataTableUtils.toString(row["acc"]));
            list_product = dt.AsEnumerable().Select(s => s.Field<string>("acc")).ToList();
        }

        //顯示目前非生產部的登入人數
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        sqlcmd = "select * from now_login left join department_relation on now_login.department = department_relation.code   where ispmd <> 'PMD' OR ispmd is null   ";
        dt = DataTableUtils.GetDataTable(sqlcmd);
        if (Check_DataTable(dt))
        {
            manage = dt.Rows.Count;
            //foreach (DataRow row in dt.Rows)
            //    list_manage.Add(DataTableUtils.toString(row["acc"]));
            list_manage = dt.AsEnumerable().Select(s => s.Field<string>("acc")).ToList();
        }

        //沒讀到的話，只能讓一個人登入
        string manage_number = Decrypted_Text(Get_Ini(Encrypted_Text("manage_number"), "inikey", "DtRf3qB9HWTAkJ1jRJcQOw=="));

        string product_number = Decrypted_Text(Get_Ini(Encrypted_Text("product_number"), "inikey", "DtRf3qB9HWTAkJ1jRJcQOw=="));

        //生產VIS登入人數大於授權人數 -> 踢掉
        if (product + 1 > DataTableUtils.toInt(product_number) && list_product.IndexOf(acc) == -1)
            return false;
        //管理VIS登入人數大於授權人數 -> 踢掉
        if (manage + 1 > DataTableUtils.toInt(manage_number) && list_manage.IndexOf(acc) == -1)
            return false;

        //找到該帳號目前的該筆資料
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        sqlcmd = $"select * from now_login where acc = '{acc}'";
        dt = DataTableUtils.GetDataTable(sqlcmd);
        //目前在資料庫的
        if (Check_DataTable(dt))
        {
            DataRow row = dt.NewRow();
            row["id"] = dt.Rows[0]["id"];
            row["acc"] = acc;
            row["page_name"] = page;
            row["now_time"] = DateTime.Now.ToString("yyyyMMddHHmmss");
            row["department"] = department;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataTableUtils.Update_DataRow("now_login", $"acc = '{acc}'", row);
            return true;
        }
        //目前不在資料庫的
        else if (dt != null && dt.Rows.Count == 0)
        {
            DataRow row = dt.NewRow();
            row["acc"] = acc;
            row["page_name"] = page;
            row["now_time"] = DateTime.Now.ToString("yyyyMMddHHmmss");
            row["department"] = department;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataTableUtils.Insert_DataRow("now_login", row);
            return true;
        }
        //資料庫開啟異常
        else
            return false;
    }
    /// <summary>
    /// 檢查帳號是否在線上
    /// </summary>
    /// <returns></returns>
    public static bool check_loginLive(string acc, string IP, string Platform)
    {
        bool Islogin = false;
        bool IsSameIP = false;
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        string sqlcmd = $"SELECT * FROM SYSTEM_USERSLOGIN_log where user_acc = '{acc}' order by LOGIN_TIME desc ";
        DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
        //同一個裝置?  同一個IP //先依德上需求  同帳號管理就好 0519-juiedit
        if (HtmlUtil.Check_DataTable(dt))
        {
            //同一IP也要加入驗證 不然無法在原機登出
            Islogin = dt.Rows[0]["STATUS"].ToString() == "1" ? true : false;
            IsSameIP = dt.Rows[0]["CLIENT_IP"].ToString() == IP ? true : false;
            if (Islogin && IsSameIP)        //同一個裝置(IP),登入中  可以過
                return true;
            else if (!Islogin && IsSameIP)  //同一個裝置 非登入中 可以過
                return true;
            else if (!Islogin && !IsSameIP) //不同個裝置 非登入中 可以過
                return true;
            else                             //不同裝置 登入中 不能過
                return (Islogin && IsSameIP);
        }
        else
        {
            if (dt.Rows.Count == 0)//等於0表示第一次登入直接給 true
                return true;
            else
                return false;
        }
    }
    /// <summary>
    /// 群組功能驗證處
    /// </summary>
    /// <param name="acc">帳號</param>
    /// <param name="page_name">頁面名稱</param>
    /// <returns></returns>
    public static bool check_power(string acc, string page_name)
    {
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        string sqlcmd = $"select user_group.* from user_group , page_tree   where is_open = 1 and user_acc = '{acc}' and page_name = '{page_name}' and page_tree.dpm = user_group.group_name";
        DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
        return Check_DataTable(dt);
    }

    /// <summary>
    /// 設定下拉選單 ||  核取方塊
    /// </summary>
    /// <param name="dt">DataTable內容</param>
    /// <param name="text">文字</param>
    /// <param name="value">直</param>
    /// <param name="drop">下拉選單</param>
    /// <param name="cbx">核取方塊</param>
    public static void Set_Element(DataTable dt, string text, string value = "", DropDownList drop = null, CheckBoxList cbx = null)
    {
        ListItem list = new ListItem();
        foreach (DataRow row in dt.Rows)
        {
            if (value == "")
                list = new ListItem(DataTableUtils.toString(row[text]), DataTableUtils.toString(row[text]));
            else
                list = new ListItem(DataTableUtils.toString(row[text]), DataTableUtils.toString(row[value]));

            if (drop != null)
                drop.Items.Add(list);
            if (cbx != null)
                cbx.Items.Add(list);
        }
    }

    /// <summary>
    /// 儲存設定的欄位至資料庫內
    /// </summary>
    /// <param name="Columns">欄位名稱</param>
    /// <param name="pagename">頁面名稱</param>
    /// <param name="acc">帳號</param>
    public static void Save_Columns(string Columns, string pagename, string acc)
    {
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        string sqlcmd = $"select * from save_pagecolumns where acc = '{acc}' and pageName='{pagename}'";
        DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

        //先刪除舊有的
        if (Check_DataTable(dt))
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataTableUtils.Delete_Record("save_pagecolumns", $"acc = '{acc}' and pageName='{pagename}'");
        }

        //取得目前最大
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        sqlcmd = "select max(id) id from save_pagecolumns";
        DataTable dt_max = DataTableUtils.GetDataTable(sqlcmd);

        int max = Check_DataTable(dt_max) ? DataTableUtils.toInt(dt_max.Rows[0]["id"].ToString()) + 1 : 1;

        if (dt != null)
        {
            List<string> list = new List<string>(Columns.Split(','));
            DataTable dt_clone = dt.Clone();
            string now_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            for (int i = 0; i < list.Count - 1; i++)
            {
                DataRow row = dt_clone.NewRow();
                row["id"] = max + i;
                row["columnname"] = list[i];
                row["orders"] = i;
                row["acc"] = acc;
                row["pagename"] = pagename;
                row["changetime"] = now_time;
                dt_clone.Rows.Add(row);
            }
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataTableUtils.Insert_TableRows("save_pagecolumns", dt_clone);
        }
    }

    /// <summary>
    /// 從資料庫讀取欄位順序
    /// </summary>
    /// <param name="acc">帳號</param>
    /// <param name="pagename">頁面名稱</param>
    /// <returns></returns>
    public static List<string> Get_ColumnsList(string acc, string pagename)
    {
        GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
        //MySQL
        //  string sqlcmd = $"select * from save_pagecolumns where acc= '{acc}' and pagename='{pagename}' order by cast( orders as signed) asc";
        //MSSQL
        string sqlcmd = $"select * from save_pagecolumns where acc= '{acc}' and pagename='{pagename}' order by cast( orders as int) asc";
        DataTable columns = DataTableUtils.GetDataTable(sqlcmd);
        List<string> columns_list = new List<string>();
        if (Check_DataTable(columns))
        {
            foreach (DataRow row in columns.Rows)
                columns_list.Add(DataTableUtils.toString(row["columnname"]));
            columns_list.Add("");
        }
        return columns_list;
    }

    /// <summary>
    /// 加密文字
    /// </summary>
    /// <param name="value">文字</param>
    /// <returns></returns>
    public static string Encrypted_Text(string value)
    {
        //加密文字
        string code_name = dekSecure.dekHashCode.SHA256("dek54886961deta1115");
        return dekSecure.dekEncDec.Encrypt(value, code_name);
    }

    /// <summary>
    /// 解密文字
    /// </summary>
    /// <param name="value">文字</param>
    /// <returns></returns>
    public static string Decrypted_Text(string value)
    {
        //解密文字
        string code_name = dekSecure.dekHashCode.SHA256("dek54886961deta1115");
        return dekSecure.dekEncDec.Decrypt(value, code_name);
    }

    /// <summary>
    /// 設定文字至ini
    /// </summary>
    /// <param name="title">標題名稱</param>
    /// <param name="key">key名稱</param>
    /// <param name="value">欲設的內容</param>
    /// <param name="road">ini路徑->請至webconfig修改</param>
    public static void Set_Ini(string title, string key, string value, string road = "ini_road")
    {
        IniManager iniManager = new IniManager(WebUtils.GetAppSettings(road));
        iniManager.WriteIniFile(title, key, value);
    }

    /// <summary>
    /// 取得ini文字
    /// </summary>
    /// <param name="title">標題名稱</param>
    /// <param name="key">key名稱</param>
    /// <param name="value">取不到時的預設內容</param>
    /// <param name="road">ini路徑->請至webconfig修改</param>
    public static string Get_Ini(string title, string key, string defaultvalue = "", string road = "ini_road")
    {
        IniManager iniManager = new IniManager(WebUtils.GetAppSettings(road));
        return iniManager.ReadIniFile(title, key, defaultvalue);
    }
    public static Dictionary<string, string> Get_Ini_Section(string FileName, string Section)
    {
        string path = WebUtils.GetAppSettings("ini_local") + FileName + ".ini";
        IniManager iniManager = new IniManager(path);
        Support.SetupIniIP ini = new SetupIniIP(path, Section, false);
        string[] keyArray = ini.ReadSection(Section);
        Dictionary<string, string> iniInf = new Dictionary<string, string>();
        foreach (string key in keyArray)
            iniInf.Add(key.Split('=')[0], key.Split('=')[1]);
        return iniInf;//.ReadIniFile(title, key, defaultvalue);
    }

    /// <summary>
    /// 回傳印出圖片的表格
    /// </summary>
    /// <param name="dt">明細資料表</param>
    /// <param name="X_Text">X軸名稱</param>
    /// <param name="Y_Text">Y軸名稱</param>
    /// <returns></returns>
    public static DataTable PrintChart_DataTable(DataTable dt, string X_Text, string Y_Text, string condition = "", bool no_count = false)
    {

        DataTable dt_Return = new DataTable();
        dt_Return.Columns.Add(X_Text);
        dt_Return.Columns.Add(Y_Text, typeof(double));
        string sqlcmd = "";
        double count = 0;

        //避免有空白之類的廠商重複出現
        List<string> avoid_again = new List<string>();
        if (Check_DataTable(dt))
        {
            DataTable dt_Xtext = dt.DefaultView.ToTable(true, new string[] { X_Text });
            foreach (DataRow row in dt_Xtext.Rows)
            {
                if (avoid_again.IndexOf(DataTableUtils.toString(row[X_Text]).Trim()) == -1)
                {
                    count = 0;
                    sqlcmd = $"{X_Text}='{row[X_Text]}' {condition}";
                    DataRow[] rows = dt.Select(sqlcmd);
                    if (rows != null && rows.Length > 0)
                    {
                        if (!no_count)
                            for (int i = 0; i < rows.Length; i++)
                                count += DataTableUtils.toDouble(DataTableUtils.toString(rows[i][Y_Text]));
                        else
                            count = rows.Length;
                    }
                    else
                        count = 0;
                    avoid_again.Add(DataTableUtils.toString(row[X_Text]).Trim());
                    dt_Return.Rows.Add(DataTableUtils.toString(row[X_Text]), count);
                }
            }
            return dt_Return;
        }
        else
            return dt_Return;
    }

    /// <summary>
    /// 設定所需之欄位
    /// </summary>
    /// <param name="dt">資料表</param>
    /// <param name="cloumn">需要之內容</param>
    /// <param name="remove">移除 true,複製 false</param>
    /// <param name="judge_column">該欄位為空值移除</param>
    /// <returns></returns>
    public static DataTable Print_DataTable(DataTable dt, string column, bool remove = false, string judge_column = "")
    {

        DataTable dt_return = new DataTable();
        List<string> list = new List<string>(column.Split(','));
        if (judge_column != "")
        {
            DataRow[] rows = dt.Select($"{judge_column} = '' OR {judge_column} IS NULL");
            for (int i = 0; i < rows.Length; i++)
                rows[i].Delete();
            dt.AcceptChanges();
        }

        if (!remove)
        {
            //建立TABLE
            foreach (string item in list)
                dt_return.Columns.Add(item);
        }
        else
        {
            //建立TABLE
            foreach (string item in list)
                dt.Columns.Remove(item);
            dt_return = dt.Clone();
            //清空list 
            list.Clear();
            //填入欄位
            for (int i = 0; i < dt.Columns.Count; i++)
                list.Add(dt.Columns[i].ToString());
        }

        //填入資料
        foreach (DataRow row in dt.Rows)
        {
            DataRow rows = dt_return.NewRow();
            foreach (string item in list)
                rows[item] = row[item];
            dt_return.Rows.Add(rows);
        }

        //重複資料去除
        dt_return = dt_return.DefaultView.ToTable(true);

        return dt_return;
    }

    /// <summary>
    /// 合併對應值
    /// </summary>
    /// <param name="dt">資料表</param>
    /// <param name="stay_cloumn">保留欄位</param>
    /// <param name="remove_cloumn">捨棄欄位</param>
    /// <returns></returns>
    public static DataTable Merge_DataTable(DataTable dt, string stay_cloumn, string remove_cloumn)
    {
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            dt.Columns[i].ReadOnly = false;
        }
        List<string> stay_list = new List<string>(stay_cloumn.Split(','));
        List<string> remove_list = new List<string>(remove_cloumn.Split(','));
        foreach (DataRow row in dt.Rows)
        {
            for (int i = 0; i < stay_list.Count; i++)
                if (row[stay_list[i]].ToString() == "")
                    row[stay_list[i]] = row[remove_list[i]];
        }
        for (int i = 0; i < remove_list.Count; i++)
            dt.Columns.Remove(remove_list[i]);

        return dt;
    }

    /// <summary>
    /// 比對順序
    /// </summary>
    /// <param name="dt">系統給的</param>
    /// <param name="history_list">已經儲存的</param>
    /// <returns></returns>
    public static List<string> Comparison_ColumnOrder(DataTable dt, List<string> history_list)
    {
        //取得dt的欄位順序
        List<string> columns = new List<string>();
        for (int i = 0; i < dt.Columns.Count; i++)
            columns.Add(dt.Columns[i].ToString());
        columns.Add("");

        if (history_list == null || history_list.Count == 0)
            return columns;
        else
        {
            List<string> order_list = new List<string>();

            for (int i = 0; i < history_list.Count; i++)
                if (columns.IndexOf(history_list[i]) != -1)
                    order_list.Add(history_list[i]);

            //第二次比對是否有新增
            for (int i = 0; i < columns.Count; i++)
                if (history_list.IndexOf(columns[i]) == -1)
                    order_list.Add(columns[i]);

            //空白永遠移到最後
            order_list.Remove("");
            order_list.Add("");
            return order_list;
        }

    }
    /// <summary>
    /// 94多型
    /// </summary>
    /// <param name="noworder_list">系統給的</param>
    /// <param name="history_list">已經儲存的</param>
    /// <returns></returns>
    public static List<string> Comparison_ColumnOrder(List<string> noworder_list, List<string> history_list)
    {
        if (history_list == null || history_list.Count == 0)
            return noworder_list;
        else
        {
            List<string> order_list = new List<string>();
            for (int i = 0; i < history_list.Count; i++)
                if (noworder_list.IndexOf(history_list[i]) != -1)
                    order_list.Add(history_list[i]);

            //第二次比對是否有新增
            for (int i = 0; i < noworder_list.Count; i++)
                if (history_list.IndexOf(noworder_list[i]) == -1)
                    order_list.Add(noworder_list[i]);

            //空白永遠移到最後
            order_list.Remove("");
            order_list.Add("");
            return order_list;
        }

    }

    /// <summary>
    /// dt轉list
    /// </summary>
    /// <typeparam name="T">model類型</typeparam>
    /// <param name="dt">dt內容</param>
    /// <returns></returns>
    public static List<T> ConvertToList<T>(DataTable dt)
    {
        var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
        var properties = typeof(T).GetProperties();
        return dt.AsEnumerable().Select(row =>
        {
            var objT = Activator.CreateInstance<T>();
            foreach (var pro in properties)
            {
                if (columnNames.Contains(pro.Name.ToLower()))
                {
                    try
                    {
                        pro.SetValue(objT, row[pro.Name]);
                    }
                    catch (Exception ex)
                    { }
                }
            }
            return objT;
        }).ToList();
    }

    public static DataTable Get_HeadRow(DataTable dt)
    {
        DataTable dts = new DataTable();
        foreach (DataColumn column in dt.Columns)
            dts.Columns.Add(column.ToString());
        return dts;
    }
    /// <summary>
    /// 顯示個人設定的刷新時間0518-juiedit 
    /// </summary>
    /// <returns></returns>
    public static string set_Reflashtime(string acc, int InitSecTime = 10)
    {
        if (WebUtils.GetAppSettings("Com_ReflashTime") == "1")
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataRow dr = DataTableUtils.DataTable_GetDataRow("Users", $"user_ID='U000000'");
            if (dr != null)
                acc = dr["user_acc"].ToString();
        }
        string second = HtmlUtil.Search_acc_Column(acc, "Set_Time");
        if (second == "")
            return (InitSecTime * 1000).ToString();
        else
            return second + "000";
    }
}
//儲存錯誤的地方
public class Utilities
{
    public static Exception LastError;
}
//Json to DataTable
public static class JsonToDataTable
{
    /// <summary>
    /// 從網址中取得xml內容
    /// </summary>
    /// <param name="url">xml網址</param>
    /// <returns></returns>
    public static string HttpGetXml(string url)
    {
        Encoding encoding = Encoding.UTF8;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.Accept = "text/html, application/xhtml+xml, */*";
        request.ContentType = "application/xml";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            string urls = reader.ReadToEnd();
            int start = urls.IndexOf("[");
            int end = urls.LastIndexOf("]");

            urls = urls.Substring(start, end - start + 1);
            return urls;
        }

    }
    /// <summary>
    /// 從網址中取得json內容
    /// </summary>
    /// <param name="url">json網址</param>
    /// <returns></returns>
    public static string HttpGetJson(string url)
    {
        Encoding encoding = Encoding.UTF8;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.Accept = "text/html, application/xhtml+xml, */*";
        request.ContentType = "application/json";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }

    }

    /// <summary>
    /// 把json內容轉成DataTable
    /// </summary>
    /// <param name="jsonString">json內容</param>
    /// <returns></returns>
    public static DataTable JsonStringToDataTable(string jsonString)
    {
        DataTable dt = new DataTable();
        string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
        List<string> ColumnsName = new List<string>();
        foreach (string jSA in jsonStringArray)
        {
            string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
            foreach (string ColumnsNameData in jsonStringData)
            {
                try
                {
                    int idx = ColumnsNameData.IndexOf(":");
                    string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                    if (!ColumnsName.Contains(ColumnsNameString))
                    {
                        ColumnsName.Add(ColumnsNameString);
                    }
                }
                catch (Exception ex)
                {
                    //如果沒有資料，直接回傳NULL，避免出錯
                    //throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    return null;
                }
            }
            break;
        }
        foreach (string AddColumnName in ColumnsName)
        {
            dt.Columns.Add(AddColumnName.Replace("\\", ""));
        }
        foreach (string jSA in jsonStringArray)
        {
            string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
            DataRow nr = dt.NewRow();
            foreach (string rowData in RowData)
            {
                try
                {
                    int idx = rowData.IndexOf(":");
                    string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "").Replace("\\", "");
                    string RowDataString = rowData.Substring(idx + 1).Replace("\"", "").Replace("\\", "");
                    nr[RowColumns] = RowDataString;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            dt.Rows.Add(nr);
        }
        return dt;
    }
    public static DataTable JsonStringToDataTable_Cs<T>(string jsonString)
    {
        List<T> order = JsonConvert.DeserializeObject<List<T>>(jsonString);
        return ConvertToDataTable<T>(order);
    }
    public static DataTable ConvertToDatatable<T>(T model)
    {
        List<T> modelList = new List<T>();
        modelList.Add(model);
        return ConvertToDataTable(modelList);
    }
    public static DataTable ConvertToDataTable<T>(IList<T> data)
    {
        PropertyDescriptorCollection properties =
           TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }

}
//生產推移圖專用 - 測試
public static class Set_Html
{
    public static string Set_TabModel(string LineName, string th, string tr, List<string> value, string s_th = "", string s_tr = "")
    {


        return
        $"<div id=\"div_{LineName}\">" +
            //TAB部分
            $"<ul id=\"myTab_{LineName}\" class=\"nav nav-tabs\" role=\"tablist\">" +
                $"<li role=\"presentation\" class=\"active\" style=\"box-shadow: 3px 3px 9px gray;\"><a href=\"#tab_content1_{LineName}\" id=\"home-tab_{LineName}\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"true\">圖片模式</a>" +
                $"</li>" +
                $"<li role=\"presentation\" class=\"\" style=\"box-shadow: 3px 3px 9px gray;\"><a href=\"#tab_content2_{LineName}\" id=\"profile-tab_{LineName}\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">表格模式</a>" +
                $"</li>" +
            $"</ul>" +

            $"<div id=\"myTabContent\" class=\"tab-content\">" +
                $"<div role=\"tabpanel\" class=\"tab-pane fade active in\" id=\"tab_content1_{LineName}\" aria-labelledby=\"home-tab\">" +
                    $"<div class=\"x_panel Div_Shadow\">" +
                        $"<div class=\"row\">" +
                            $"<div class=\"dashboard_graph x_panel\">" +
                                $"<div class=\"col-md-12 col-sm-12 col-xs-12\" id=\"hidepercent_{LineName}\">" +
                                    $"<div class=\"x_content\">" +
                                        $"<div style=\"text-align: right; width: 100%; padding: 0;\">" +
                                            $"<button style=\"display: none\" type=\"button\" id=\"exportChart_{LineName}\" title=\"另存成圖片\">" +
                                                $"<img src=\"../../assets/images/download.jpg\" style=\"width: 36.39px; height: 36.39px;\">" +
                                            $"</button>" +
                                        $"</div>" +
                                        $"<div class=\"row \">" +
                                            $"<div class=\"col-md-12 col-sm-12 col-xs-12\">" +
                                                $"<div class=\"col-md-12 col-sm-12 col-xs-12\">" +
                                                    $"<div id=\"chart_bar_{LineName}\" class=\"Canvas_height\">" +
                                                    $"</div>" +
                                                $"</div>" +
                                            $"</div>" +
                                        $"</div>" +
                                    $"</div>" +
                                $"</div>" +
                            $"</div>" +
                        $"</div>" +
                    $"</div>" +
                    $"<div class=\"col-md-12 col-sm-12 col-xs-12\" id=\"hidediv_{LineName}\" style=\"display: none\">" +
                        $"<div class=\"dashboard_graph x_panel\">" +
                            $"<div class=\"x_content\">" +
                                $"<div style=\"text-align: right; width: 100%; padding: 0;\">" +
                                    $"<button style=\"display: none\" type=\"button\" id=\"exportimage_{LineName}\" title=\"另存成圖片\">" +
                                        $"<img src=\"../../assets/images/download.jpg\" style=\"width: 36.39px; height: 36.39px;\">" +
                                    $"</button>" +
                                $"</div>" +
                                $"<div class=\"row Canvas_height\">" +
                                    $"<div class=\"col-md-12 col-sm-12 col-xs-12\">" +
                                        $"<div class=\"col-md-12 col-sm-12 col-xs-10\">" +
                                            $"<div id=\"chartContainer_{LineName}\" style=\"height: 500px; max-width: 100%;\">" +
                                            $"</div>" +
                                        $"</div>" +
                                    $"</div>" +
                                $"</div>" +
                            $"</div>" +
                        $"</div>" +
                    $"</div>" +
                $"</div>" +





                $"<div role=\"tabpanel\" class=\"tab-pane fade\" id=\"tab_content2_{LineName}\" aria-labelledby=\"profile-tab\">" +
                    $"<div class=\"x_panel Div_Shadow\">" +
                        $"<div class=\"row\">" +
                            $"<div class=\"col-md-12 col-sm-12 col-xs-12\">" +
                                $"<div class=\"x_panel zpanel\">" +
                                    $"<div class=\"x_title\">" +
                                            $"<h1 class=\"text-center _mdTitle\" style=\"width: 100%\"><b>{LineName}未結案列表</b></h1>" +
                                            $"<h3 class=\"text-center _xsTitle\" style=\"width: 100%\"><b>{LineName}未結案列表</b></h3>" +
                                        $"<div class=\"clearfix\">" +
                                        $"</div>" +
                                    $"</div>" +
                                    $"<div class=\"x_content\">" +
                                        $"<p class=\"text-muted font-13 m-b-30\">" +
                                        $"</p>" +
                                        $"<table id=\"datatable_{LineName}\" class=\"table table-ts table-bordered nowrap\" cellspacing=\"0\" width=\"100%\">" +
                                            $"<thead>" +
                                                $"<tr id=\"tr_row\">" +
                                                    th +
                                                $"</tr>" +
                                            $"</thead>" +
                                            $"<tbody>" +
                                                tr +
                                            $"</tbody>" +
                                        $"</table>" +
                                    $"</div>" +
                                $"</div>" +
                            $"</div>" +
                        $"</div>" +
                    $"</div>" +
                $"</div>" +
            $"<div class=\"clearfix\"></div>" +
            $"<br/>" +
        $"</div>" +
        $"</div>";

    }
    public static string Set_Image(string LineName, string TimeRange, string Value1, string Value2)
    {
        return $"set_image('', \"chartContainer\",{TimeRange}, [{Value1}], [{Value2}], \"exportimage\", \"chart_bar\", \"exportChart\");";
    }
    public static string Set_Table(string LineName, string table_num = "")
    {
        return $"set_Table('#datatable-buttons');";
    }
}
//HTML 引用JS
public class Use_Javascript
{
    public static string Quote_Javascript(bool ok = true)
    {
        string value = "<script src=\"../../assets/vendors/jquery/dist/jquery.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/bootstrap/dist/js/bootstrap.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/fastclick/lib/fastclick.js\"></script>\n" +
                "<script src=\"../../assets/vendors/iCheck/icheck.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/moment/min/moment.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/bootstrap-daterangepicker/daterangepicker.js\"></script>\n" +
                "<script src=\"../../assets/vendors/switchery/dist/switchery.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/select2/dist/js/select2.full.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/autosize/dist/autosize.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js\"></script>\n" +
                "<script src=\"../../assets/build/js/custom.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/FloatingActionButton/js/index.js\"></script>\n" +
                "<script src=\"../../assets/vendors/canvas_js/canvasjs.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/bootstrap-touchspin-master/dist/jquery.bootstrap-touchspin.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net/js/jquery.dataTables.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-buttons/js/dataTables.buttons.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-buttons/js/buttons.flash.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-buttons/js/buttons.html5.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-buttons/js/buttons.print.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-responsive/js/dataTables.responsive.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js\"></script>\n" +
                "<script src=\"../../assets/vendors/datatables.net-scroller/js/dataTables.scroller.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/jszip/dist/jszip.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/pdfmake/build/pdfmake.min.js\"></script>\n" +
                "<script src=\"../../assets/vendors/pdfmake/build/vfs_fonts.js\"></script>\n" +
                "<script src=\"../../assets/vendors/jQuery-Smart-Wizard/js/jquery.smartWizard.js\"></script>" +
                "<script src=\"../../assets/vendors/nprogress/nprogress.js\"></script>" +
                "<script src=\"../../assets/vendors/Json-xml/json2xml.js\"></script>" +
                "<script src=\"../../assets/vendors/Json-xml/xml2json.js\"></script>" +
                "<script src=\"../../assets/vendors/Create_HtmlCode/HtmlCode20211117.js\"></script>";

        return ok ? value + "<script src=\"../../assets/vendors/datatables.net-colReorder/dataTables.colReorder.min.js\"></script>\n" : value;
    }
}
//另開視窗用
public static class RedirectHelper
{
    public static void Redirect(this HttpResponse response,
    string url, string target, string windowFeatures)
    {
        if ((String.IsNullOrEmpty(target) ||
        target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
        String.IsNullOrEmpty(windowFeatures))
        {
            response.Redirect(url);
        }
        else
        {
            Page page = (Page)HttpContext.Current.Handler; if (page == null)
            {
                throw new
                InvalidOperationException("Cannot redirect to new window .");
            }
            url = page.ResolveClientUrl(url);
            string script;
            if (!String.IsNullOrEmpty(windowFeatures))
            {
                script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
            }
            else
            {
                script = @"window.open(""{0}"", ""{1}"");";
            }
            script = String.Format(script, url, target, windowFeatures);
            ScriptManager.RegisterStartupScript(page,
           typeof(Page), "Redirect", script, true);
        }
    }
}
//樹狀圖用
public class TreeViewOps
{
    public static bool ForEachNode(TreeNode root, Func<TreeNode, object, bool> callback, object obj)
    {
        if (root == null || callback == null) return true;
        if (!callback(root, obj)) return false;
        foreach (TreeNode node in root.ChildNodes)
            if (!ForEachNode(node, callback, obj))
                return false;
        return true;
    }

    static bool Find_NodeText(TreeNode root, string text_to_find, List<TreeNode> list, bool isFindOne, bool isContains, bool isCaseSentive)
    {
        if (root == null) return false;
        string text = (isCaseSentive) ? root.Text : root.Text.ToLower();
        bool ok = (isContains) ? text.Contains(text_to_find) : (root.Text == text);
        if (ok)
        {
            list.Add(root);
            if (isFindOne) return true;
        }
        foreach (TreeNode node in root.ChildNodes)
        {
            if (Find_NodeText(node, text_to_find, list, isFindOne, isContains, isCaseSentive))
            {
                if (isFindOne) return true;
            }
        }
        return false;
    }
    public static List<TreeNode> FindNode_All(TreeNode root, string text_to_find, bool isContains, bool isCaseSentive)
    {
        if (!isCaseSentive)
            text_to_find = text_to_find.ToLower();
        List<TreeNode> list = new List<TreeNode>();
        Find_NodeText(root, text_to_find, list, false, isContains, isCaseSentive);
        return list;
    }

    public static TreeNode FindNode_One(TreeNode root, string text_to_find, bool isContains, bool isCaseSentive)
    {
        if (!isCaseSentive)
            text_to_find = text_to_find.ToLower();
        List<TreeNode> list = new List<TreeNode>();
        if (Find_NodeText(root, text_to_find, list, true, isContains, isCaseSentive))
        {
            return list[0];
        }
        return null;
    }

    public static bool AddNode_Root(TreeView tree_view, TreeNode node, int index)
    {
        if (node == null) return false;
        if (index < 0) index = 0;
        if (index >= tree_view.Nodes.Count)
            tree_view.Nodes.Add(node);
        else tree_view.Nodes.AddAt(index, node);
        return true;
    }

    public static bool AddNode_Child(TreeView tree_view, TreeNode parent, TreeNode node, int index)
    {
        if (node == null) return false;
        if (parent == null) // add to tree_view
            return AddNode_Root(tree_view, node, index);
        else
        {
            if (index >= parent.ChildNodes.Count)
                parent.ChildNodes.Add(node);
            else
            {
                if (index < 0) index = 0;
                parent.ChildNodes.AddAt(index, node);
            }
        }
        return true;
    }

    public static bool AddNode_Sibling(TreeView tree_view, TreeNode cur_node, TreeNode node, int index)
    {
        return AddNode_Child(tree_view, cur_node.Parent, node, index);
    }

    public static bool MoveToIndex(TreeNode node, int index, TreeView Treeview)
    {
        if (node.Parent.ChildNodes.IndexOf(node) != index)
        {
            TreeNode parent = node.Parent;
            if (parent == null) //TreeView
            {
                TreeView view = Treeview;
                node.ChildNodes.Remove(node);
                if (index >= view.Nodes.Count)
                    view.Nodes.Add(node);
                else
                {
                    if (index < 0) index = 0;
                    view.Nodes.AddAt(index, node);
                }
            }
            else
            {
                node.ChildNodes.Remove(node);
                if (index >= parent.ChildNodes.Count)
                    parent.ChildNodes.Add(node);
                else
                {
                    if (index < 0) index = 0;
                    parent.ChildNodes.AddAt(index, node);
                }
            }
        }
        return true;
    }

    public static bool MoveForward(TreeNode node, int steps = 1, TreeView Treeview = null)
    {
        return MoveToIndex(node, node.ChildNodes.IndexOf(node) + steps, Treeview);
    }

    /// <summary>
    /// 將該點移動至父項目
    /// </summary>
    /// <param name="node">選擇的點</param>
    /// <param name="Treeview">treeview元件名稱</param>
    /// <returns></returns>
    public static bool MoveToParent(TreeNode node, TreeView Treeview)
    {
        TreeNode parent = node.Parent;
        if (parent == null) return false;
        node.ChildNodes.Remove(node);
        int index = parent.ChildNodes.IndexOf(node) + 1;
        int index_i = 0;
        if (parent.Parent != null)
            index_i = parent.Parent.ChildNodes.Count;
        else
            index_i = Treeview.Nodes.Count;
        if (index > index_i)
            index = index_i;
        if (parent.Parent == null)
            Treeview.Nodes.AddAt(index, node);
        else
            parent.Parent.ChildNodes.AddAt(index, node);
        return true;
    }

    /// <summary>
    /// 將該點移動至子項目
    /// </summary>
    /// <param name="node">選擇的點</param>
    /// <param name="Treeview">treeview元件名稱</param>
    /// <returns></returns>
    public static bool MoveToChild(TreeNode node, TreeView Treeview)
    {
        TreeNode test = node.Parent;
        int index = 0;
        if (test == null)
            return false;
        else
            index = node.Parent.ChildNodes.IndexOf(node) - 1;
        if (index == -1)
            index = 1;
        // not the first node
        if (index < 0) return false;
        TreeNode parent = node.Parent;
        if (parent == null)
            parent = Treeview.Nodes[index];
        else
        {
            try
            {
                parent = parent.ChildNodes[index];
            }
            catch (Exception e)
            {
                return false;
            }

        }
        node.ChildNodes.Remove(node);
        parent.ChildNodes.Add(node);
        return true;
    }

    /// <summary>
    /// 將該點上下移動
    /// </summary>
    /// <param name="view">treeview元件名稱</param>
    /// <param name="str">頂/底</param>
    /// <param name="ok">移動完畢後，是否取消該節點之選取</param>
    /// <returns></returns>
    public static string Move_Up_Down(TreeView view, string str, bool ok = false)
    {
        string Return_str = "";
        TreeNode tns = view.SelectedNode;
        if (tns != null)
        {
            TreeNode parent = tns.Parent;
            if (tns.Parent != null)
            {
                int index = parent.ChildNodes.IndexOf(tns);
                if (str == "頂")
                {
                    if (index == 0)
                        return "已經到達該節點的" + str + "端囉";
                    parent.ChildNodes.RemoveAt(index);
                    parent.ChildNodes.AddAt(index - 1, tns);

                }
                else
                {
                    if (index == parent.ChildNodes.Count - 1)
                        return "已經到達該節點的" + str + "端囉";
                    parent.ChildNodes.RemoveAt(index);
                    parent.ChildNodes.AddAt(index + 1, tns);
                }
            }
            else
            {
                int index = view.Nodes.IndexOf(tns);
                if (str == "頂")
                {
                    if (index == 0)
                        return "已經到達該節點的" + str + "端囉";
                    view.Nodes.RemoveAt(index);
                    view.Nodes.AddAt(index - 1, tns);
                }
                else
                {
                    if (index == view.Nodes.Count - 1)
                        return "已經到達該節點的" + str + "端囉";
                    view.Nodes.RemoveAt(index);
                    view.Nodes.AddAt(index + 1, tns);
                }
            }
            tns.Selected = ok;
        }
        else
        {
            Return_str = "請選擇一節點";
        }
        return Return_str;
    }

    /// <summary>
    /// 編輯或是刪除該節點
    /// </summary>
    /// <param name="TreeView_Result">treeview元件名稱</param>
    /// <param name="status">刪除節點/更新節點名稱</param>
    /// <param name="Edit">編輯節點之名稱</param>
    /// <returns></returns>
    public static string Change_Node(TreeView TreeView_Result, string status, string Edit = "")
    {
        string Return_string = "";
        TreeNode tns = TreeView_Result.SelectedNode;
        if (tns != null)
        {
            if (tns.Parent != null)
            {
                TreeNode targetParent = TreeView_Result.FindNode(tns.Parent.ValuePath);
                if (status == "移除")
                    targetParent.ChildNodes.Remove(tns);
                else
                    tns.Text = Edit;
            }
            else
            {
                if (status == "移除")
                    TreeView_Result.Nodes.Remove(tns);
                else
                    tns.Text = Edit;
            }
            tns.Selected = false;
        }
        else
            Return_string = "請選擇一項，以便" + status + "的進行";
        return Return_string;
    }

    /// <summary>
    /// 新增父節點
    /// </summary>
    /// <param name="TreeView_Result">treeview元件名稱</param>
    /// <param name="TextBox_dpm">父節點之名稱(text)</param>
    /// <param name="TextBox_Value">父節點的值(value)</param>
    /// <returns></returns>
    public static string Add_Dpm(TreeView TreeView_Result, string TextBox_dpm, string TextBox_Value = "")
    {
        string Return_string = "";
        TreeNode tns = TreeView_Result.SelectedNode;
        TreeNode P_Node = new TreeNode();
        P_Node.Text = TextBox_Value == "" ? TextBox_dpm : $"{TextBox_dpm} ({TextBox_Value})";
        if (TextBox_Value != "")
            P_Node.Value = TextBox_Value;
        Return_string = "";
        if (tns != null)
        {
            tns.ChildNodes.Add(P_Node);
            P_Node.Checked = true;
            tns.Selected = false;
        }
        else
        {
            TreeView_Result.Nodes.Add(P_Node);
            P_Node.Checked = true;
        }
        //
        return Return_string;
    }

    /// <summary>
    /// 新增子節點
    /// </summary>
    /// <param name="TreeView_Result">treeview元件名稱</param>
    /// <param name="cbx">欲新增之子節點checkbox</param>
    /// <returns></returns>
    public static string Add_Node(TreeView TreeView_Result, CheckBoxList cbx, string SelectNode = "")
    {
        string Return_String = "";
        bool Exist = false;
        TreeNode tns;
        if (string.IsNullOrEmpty(SelectNode))
            tns = TreeView_Result.SelectedNode;
        else
            tns = TreeView_Result.FindNode(SelectNode);
        for (int i = 0; i < cbx.Items.Count; i++)
        {
            if (cbx.Items[i].Selected)
            {
                TreeNode P_Node = new TreeNode();
                P_Node.Text = cbx.Items[i].Text;
                P_Node.Value = cbx.Items[i].Value;

                if (tns != null) //0525-juiedit 沒加過的才加
                {
                    foreach (TreeNode nd in tns.ChildNodes)
                    {
                        if (nd.Text.Contains(P_Node.Text))
                            Exist = true;
                    }
                    if (!Exist)
                    {
                        tns.ChildNodes.Add(P_Node);
                        P_Node.Checked = true;
                        tns.Checked = true;
                        tns.Selected = false;
                    }
                }
                else
                    TreeView_Result.Nodes.Add(P_Node);
            }
            cbx.Items[i].Selected = false;
        }
        //新增完畢之後，要將Treeview展開
        TreeView_Result.CollapseAll();
        return Return_String;
    }
}

public class OrderFormat
{
    public string NO { get; set; }
    public string Order { get; set; }
    public string OrderNO { get; set; }
    public string War { get; set; }
    public string Location { get; set; }
    public string itemnum { get; set; }
    public string itemname1 { get; set; }
    public string itemname2 { get; set; }
    public string BoughtCounts { get; set; }
    public string DoneCounts { get; set; }
    public string DayTime { get; set; }
    public string Unit { get; set; }
    public string ChangeMolecular { get; set; }
    public string ChangeDenominator { get; set; }
    public string OrderLevel { get; set; }
    public string CanGetSumCounts { get; set; }
    public string CanGetCounts { get; set; }
    public string LotNum { get; set; }
    public string PickCounts { get; set; }
    public string M_Order { get; set; }
    public string ProducteType { get; set; }
    public string Qty { get; set; }
    public string NextProcessNO { get; set; }
    public string Cus { get; set; }
    public string Log { get; set; }
    public string UserIDog { get; set; }
    public string UserName { get; set; }
    public string CreatTime { get; set; }
    public string Vender_ID { get; set; }
    public string Vender_Name { get; set; }
    public string Remark01 { get; set; }
    public string ProcessSeq { get; set; }
    public string StandardTime { get; set; }
    public string CustomerID { get; set; }
    public string CustomerName { get; set; }
    public string ProcessCode { get; set; }
    public string ProcessName { get; set; }
    public string ProcessType { get; set; }
    public string Assembly_ID { get; set; }
    public string Assembly_Name { get; set; }
    public string Scrap_Qty { get; set; }
    public string TaVandNo { get; set; }
    public string SeqNo { get; set; }
    public string ToVandName { get; set; }
}
