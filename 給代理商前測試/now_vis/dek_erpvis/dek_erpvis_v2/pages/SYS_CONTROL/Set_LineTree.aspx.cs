using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using dek_erpvis_v2.cls;
using dekERP_dll.dekErp;
using Support;

namespace dek_erpvis_v2.pages.SYS_CONTROL
{
    public partial class Set_LineTree : System.Web.UI.Page
    {
        clsDB_Server cls_erp = new clsDB_Server(myclass.GetConnByDekVisErp);
        clsDB_Server cls_hor = new clsDB_Server(myclass.GetConnByDekdekVisAssmHor);
        DataTable dataTable = null;
        //全域變數
        int id = 1;
        int count = 1;
        public string acc = "";
        public string color = "";
        iTec_Materials PCD = new iTec_Materials();
        //畫面一開始進來呈現的地方
        protected void Page_Load(object sender, EventArgs e)
        {
            //效能測試
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.Search_acc_Column(acc) == "Y")
                {
                    string sqlcmd = "select assembly_line.LINE_ID as 產品名稱 ,assembly_group.GROUP_NAME as 群組名稱 from assembly_group left join assembly_line_group on assembly_line_group.GROUP_ID =  assembly_group.GROUP_ID  left join assembly_line on assembly_line.LINE_ID = assembly_line_group.LINE_ID ";
                    dataTable = cls_erp.GetDataTable(sqlcmd);
                    if (!IsPostBack)
                    {
                        show();
                        if (dataTable != null)
                            show_treeview();
                        TreeView_Result.CollapseAll();
                    }
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //印出資料庫內部所有機台名稱
        private void show(string Textbox_Select = "")
        {
            //  DataTable dt = PCD.Item_DataTable("Nam_Item");
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekdekVisAssmHor);
            string sqlcmd = "select distinct NAM_ITEM from 組裝資料表";
            DataTable dt = cls_hor.GetDataTable(sqlcmd);
            //已設定過(存入資料庫)的機種
            string sqlcmd2 = "select  assembly_line_group.LINE_ID NAM_ITEM, LINE_NAME from assembly_line_group,assembly_line where assembly_line_group.LINE_ID = assembly_line.LINE_ID  order by cast(substring(ALG_ID,5,length(ALG_ID)) as signed)  asc";
            DataTable dt_alive = cls_erp.GetDataTable(sqlcmd2);

            DataTable dt_copy = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                DataRow[] rows = dt_alive.Select($"NAM_ITEM='{DataTableUtils.toString(row["NAM_ITEM"])}'");
                if (rows != null && rows.Length > 0)
                {
                }
                else
                    dt_copy.ImportRow(row);
            }

            ListItem list = null;
            if (HtmlUtil.Check_DataTable(dt))
            {
                //未有機種分類之機型(以灰色字體呈現)
                if (HtmlUtil.Check_DataTable(dt_copy))
                {
                    foreach (DataRow row in dt_copy.Rows)
                    {
                        list = new ListItem($"<b style=\"color:gray\">{DataTableUtils.toString(row["NAM_ITEM"])}</b>", DataTableUtils.toString(row["NAM_ITEM"]));
                        CheckBoxList_Edit.Items.Add(list);
                    }
                }

                //已有機種分類之機型(已藍字呈現，後面附上分類之機種)
                if (HtmlUtil.Check_DataTable(dt_alive))
                {
                    foreach (DataRow row in dt_alive.Rows)
                    {
                        list = new ListItem($"<b style=\"color:blue\">{DataTableUtils.toString(row["NAM_ITEM"])}</b><br /><b style=\"color:red\">({DataTableUtils.toString(row["LINE_NAME"])})</b>", DataTableUtils.toString(row["NAM_ITEM"]));
                        CheckBoxList_Edit.Items.Add(list);
                    }
                }
            }
        }
        ////如果表單內有資料，就顯示在Treeview上
        private void show_treeview()
        {
            //出現checkbox
            TreeView_Result.ShowCheckBoxes = TreeNodeTypes.All;

            string sql_cmd = "SELECT distinct  GROUP_ID,GROUP_NAME FROM assembly_group order by cast(substring(GROUP_ID,3,length(GROUP_ID)) as signed)";
            DataTable dt = cls_erp.GetDataTable(sql_cmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    TreeNode P_Node = new TreeNode();
                    P_Node.Text = DataTableUtils.toString(row["GROUP_NAME"]);
                    P_Node.Value = DataTableUtils.toString(row["GROUP_NAME"]);
                    //確認子項目是否有勾選的

                    sql_cmd = $"select assembly_line.LINE_ID as 產品名稱 ,assembly_group.GROUP_NAME as 群組名稱,assembly_line.LINE_STATUS as 是否顯示 from assembly_group left join assembly_line_group on assembly_line_group.GROUP_ID =  assembly_group.GROUP_ID  left join assembly_line on assembly_line.LINE_ID = assembly_line_group.LINE_ID where assembly_group.GROUP_ID = '{row["GROUP_ID"]}' and assembly_line.LINE_ID IS NOT NULL and LENGTH(assembly_line.LINE_ID)>0 and assembly_line.LINE_ID <> ' ' and assembly_line.LINE_STATUS = 1";
                    DataTable dt_check = cls_erp.GetDataTable(sql_cmd);

                    P_Node.Checked = HtmlUtil.Check_DataTable(dt_check);

                    TreeView_Result.Nodes.Add(P_Node);
                    checkchild(P_Node, DataTableUtils.toString(row["GROUP_NAME"]));
                }
            }
        }
        ////檢查子項目
        private void checkchild(TreeNode node, string PID)
        {
            string sqlcmd = $"select assembly_line.LINE_ID as 產品名稱 ,assembly_group.GROUP_NAME as 群組名稱,assembly_line.LINE_STATUS as 是否顯示 from assembly_group left join assembly_line_group on assembly_line_group.GROUP_ID =  assembly_group.GROUP_ID  left join assembly_line on assembly_line.LINE_ID = assembly_line_group.LINE_ID where  assembly_group.GROUP_NAME='{PID}' and assembly_line.LINE_ID IS NOT NULL and LENGTH(assembly_line.LINE_ID)>0 and assembly_line.LINE_ID <> ' ' ";
            DataTable dt = cls_erp.GetDataTable(sqlcmd);

            foreach (DataRow row in dt.Rows)
            {
                TreeNode P_Node = new TreeNode();
                P_Node.Text = DataTableUtils.toString(row["產品名稱"]);
                P_Node.Value = DataTableUtils.toString(row["產品名稱"]);
                P_Node.Checked = Convert.ToBoolean(DataTableUtils.toInt(DataTableUtils.toString(row["是否顯示"])));

                node.ChildNodes.Add(P_Node);

                checkchild(P_Node, DataTableUtils.toString(row["產品名稱"]));
            }
        }
        //檢查是否含有子項目
        private void CheckItems(TreeNode node)
        {
            //將產線加入至dekviserp的資料庫內
            string condition = "";
            string Line_Name = "";

            string sqlcmd = "select * from assembly_group";
            DataTable dt_AG = cls_erp.GetDataTable(sqlcmd);


            sqlcmd = "select * from assembly_line";
            DataTable dt_AL = cls_erp.GetDataTable(sqlcmd);

            sqlcmd = "select * from assembly_line_group";
            DataTable dt_ALG = cls_erp.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt_AG) && dt_AL != null && dt_ALG != null)
            {
                id = dt_AG.Rows.Count - 1;

                DataTable dt_ALCopy = dt_AL.Clone();
                DataTable dt_ALGCopy = dt_ALG.Clone();
                foreach (TreeNode childNode in node.ChildNodes)
                {
                    string[] str = childNode.ValuePath.ToString().Split('/');
                    if (node.Checked)
                        dt_ALCopy.Rows.Add($"AL_{count}", childNode.Value, str[0], Convert.ToInt32(childNode.Checked));
                    else
                        dt_ALCopy.Rows.Add($"AL_{count}", childNode.Value, str[0], "0");

                    dt_ALGCopy.Rows.Add($"ALG_{count}", $"G_{id}", childNode.Value);

                    condition += childNode.Value + ",";
                    Line_Name = str[0];
                    count++;
                }

                cls_erp.Insert_TableRows("assembly_line", dt_ALCopy);
                cls_erp.Insert_TableRows("assembly_line_group", dt_ALGCopy);
            }

            //將產線加入至dekvishor內
            if (condition != "" && Line_Name != "")
            {
                //新增產線到工作站型態資料表

                DataTable dt = cls_hor.GetDataTable("select * from 工作站型態資料表");
                if (dt != null)
                {
                    DataRow row = dt.NewRow();
                    row["工作站編號"] = id + 1;
                    row["工作站名稱"] = Line_Name;
                    row["工作站群組"] = condition;
                    row["工作站是否使用中"] = "1";
                    cls_hor.Insert_DataRow("工作站型態資料表", row);
                }
            }
        }
        //存入資料庫 child->false 表父項目 child->true 表子項目
        private void Save_DB(int id, string PID, string part, string value, string name, bool father = true, DataTable dt_AL = null, DataTable dt_ALG = null, bool show = true)
        {
            //表該節點為產線
            if (father)
            {
                string sqlcmd = "select * from assembly_group";
                DataTable dt = cls_erp.GetDataTable(sqlcmd);

                //有資料結構的情況下
                if (dt != null)
                {
                    DataRow row = dt.NewRow();
                    row["AG_ID"] = $"AG_{dt.Rows.Count}";
                    row["GROUP_ID"] = $"G_{dt.Rows.Count}";
                    row["GROUP_NAME"] = name;
                    cls_erp.Insert_DataRow("assembly_group", row);
                }
            }
            //表該節點為機型
            else
            {
                DataRow row = dt_AL.NewRow();
                row["AL_ID"] = $"AL_{count}";
                row["LINE_ID"] = name;
                row["LINE_NAME"] = part;
                if (show)
                    row["LINE_STATUS"] = "1";
                else
                    row["LINE_STATUS"] = "0";
                cls_erp.Insert_DataRow("assembly_line", row);

                DataRow rew = dt_ALG.NewRow();
                rew["ALG_ID"] = $"ALG_{count}";
                rew["GROUP_ID"] = $"G_{id}";
                rew["LINE_ID"] = name;
                cls_erp.Insert_DataRow("assembly_line_group", rew);
            }
        }

        //執行新增動作的時候，要把相關資料表清空
        private void Delete_Data()
        {
            //清空機型關聯資料表
            cls_erp.Delete_Record("assembly_group", "AG_ID is not null");
            cls_erp.Delete_Record("assembly_line", "AL_ID is not null");
            cls_erp.Delete_Record("assembly_line_group", "ALG_ID is not null");

            //先清空工作站型態資料表
            cls_hor.Delete_Record("工作站型態資料表", " 工作站編號 <> '0' ");
        }

        /*------------------------------------------------------按鈕事件----------------------------------------------------------*/
        //新增產線節點
        protected void Button_dpm_Click(object sender, EventArgs e)
        {
            if (TextBox_dpm.Text != "")
            {
                string result = TreeViewOps.Add_Dpm(TreeView_Result, TextBox_dpm.Text);

                if (result == "")
                    TextBox_dpm.Text = "";
                else
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
            }
            else
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('請輸入機種');", true);
        }
        //刪除節點
        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            string result = TreeViewOps.Change_Node(TreeView_Result, "移除");
            if (result == "")
                TextBox_dpm.Text = "";
            else
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
        }
        //編輯節點名稱
        protected void Button_Edit_Click(object sender, EventArgs e)
        {
            string result = TreeViewOps.Change_Node(TreeView_Result, "編輯", TextBox_Edit.Text);
            if (result == "")
                TextBox_Edit.Text = "";
            else
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
        }
        //在部門內新增節點
        protected void Button_Add_Click(object sender, EventArgs e)
        {
            string result = TreeViewOps.Add_Node(TreeView_Result, CheckBoxList_Edit);
            if (result != "")
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
        }
        //垂直移動
        protected void Button_Vertical_Click(object sender, EventArgs e)
        {
            string ID_Name = ((ImageButton)sender).ID.ToString();
            if (ID_Name == "ImageButton_Up")
            {
                string result = TreeViewOps.Move_Up_Down(TreeView_Result, "頂");
                if (result != "")
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
            }
            else
            {
                string result = TreeViewOps.Move_Up_Down(TreeView_Result, "底");
                if (result != "")
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
            }
        }
        //水平移動
        protected void Button_Horizontal_Click(object sender, EventArgs e)
        {
            string ID_Name = ((ImageButton)sender).ID.ToString();
            TreeNode tn = TreeView_Result.SelectedNode;
            if (tn == null)
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('請選擇一節點');", true);
            else
            {
                if (ID_Name == "ImageButton_Left")
                {
                    if (TreeViewOps.MoveToParent(tn, TreeView_Result) == false)
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('已經位於節點最左邊囉');", true);
                }
                else
                {
                    if (TreeViewOps.MoveToChild(tn, TreeView_Result) == false)
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('已經位於節點最右邊囉');", true);
                }
                tn.Selected = false;
            }
        }
        //展開 收合 清除 TreeView
        protected void Button_Action_Click(object sender, EventArgs e)
        {
            string ID_Name = ((Button)sender).ID.ToString();
            if (ID_Name == "Button_Open")
                TreeView_Result.ExpandAll();
            else if (ID_Name == "Button_Shrink")
                TreeView_Result.CollapseAll();
            else
                TreeView_Result.Nodes.Clear();
        }
        //快速搜尋
        protected void Button_Search_Click(object sender, EventArgs e)
        {
            CheckBoxList_Edit.Items.Clear();
            show(TextBox_Search.Text);
            TextBox_Search.Text = "";
        }
        //儲存置資料庫
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            Delete_Data();
            foreach (TreeNode node in TreeView_Result.Nodes)
            {
                Save_DB(id, "", node.Text, node.Value, node.Text);
                CheckItems(node);
            }
            //-------------------------------------------------------更新狀態資料表-------------------------------------
            //找尋所有排程

            string sqlcmd = "SELECT  工作站狀態資料表.*, NAM_ITEM 機型,工作站型態資料表.工作站編號 工作站號碼 FROM 工作站狀態資料表 LEFT JOIN 組裝資料表 ON 工作站狀態資料表.排程編號 = 組裝資料表.排程編號 left join 工作站型態資料表 on 工作站型態資料表.工作站群組 LIKE CONCAT ('%', 組裝資料表.NAM_ITEM, ',%')";
            DataTable dt_NoLine = cls_hor.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt_NoLine))
            {
                foreach (DataRow row in dt_NoLine.Rows)
                    row["工作站編號"] = row["工作站號碼"].ToString() != "" ? row["工作站號碼"].ToString() : "0";
                //移除機型
                dt_NoLine.Columns.Remove("機型");
                dt_NoLine.Columns.Remove("工作站號碼");
                //刪除所有的工作站編號

                cls_hor.Delete_Record("工作站狀態資料表", " 工作站編號 >= '0' ");

                //填回去

                cls_hor.Insert_TableRows("工作站狀態資料表", dt_NoLine);
            }

            //-------------------------------------------------------更新狀態資料表-------------------------------------

            //-------------------------------------------------------更新異常維護-------------------------------------

            //找尋所有排程

            sqlcmd = "SELECT 工作站異常維護資料表.*,NAM_ITEM 機型,工作站型態資料表.工作站編號 工作站號碼 FROM 工作站異常維護資料表 left join 組裝資料表 on 工作站異常維護資料表.排程編號 = 組裝資料表.NUM_PS  left join 工作站型態資料表 on 工作站型態資料表.工作站群組 LIKE CONCAT ('%', 組裝資料表.NAM_ITEM, ',%')";
            dt_NoLine = cls_hor.GetDataTable(sqlcmd);

            if (HtmlUtil.Check_DataTable(dt_NoLine))
            {
                foreach (DataRow row in dt_NoLine.Rows)
                    row["工作站編號"] = row["工作站號碼"].ToString() != "" ? row["工作站號碼"].ToString() : "0";
                //移除機型
                dt_NoLine.Columns.Remove("機型");
                dt_NoLine.Columns.Remove("工作站號碼");
                //刪除所有的工作站編號

                cls_hor.Delete_Record("工作站異常維護資料表", " 工作站編號 >= '0' ");

                //填回去
                cls_hor.Insert_TableRows("工作站異常維護資料表", dt_NoLine);
            }
            //-------------------------------------------------------更新異常維護-------------------------------------
            Response.Write("<script>alert('設定完畢，即將進入設定畫面');location.href='Set_LineTree.aspx'; </script>");
        }
        //列印未設定之機型
        protected void Button_print_Click(object sender, EventArgs e)
        {
            string sqlcmd = "select distinct NAM_ITEM from 組裝資料表";
            DataTable dt = cls_hor.GetDataTable(sqlcmd);

            //已設定過(存入資料庫)的機種
            string sqlcmd2 = "select  assembly_line_group.LINE_ID NAM_ITEM, LINE_NAME from assembly_line_group,assembly_line where assembly_line_group.LINE_ID = assembly_line.LINE_ID order by cast(substring(ALG_ID,5,length(ALG_ID)) as signed)  asc";
            DataTable dt_alive = cls_hor.GetDataTable(sqlcmd2);

            DataTable dt_copy = dt.Clone();
            //排除已有機種之機型
            foreach (DataRow row in dt.Rows)
            {
                DataRow[] rows = dt_alive.Select($"NAM_ITEM='{DataTableUtils.toString(row["NAM_ITEM"])}'");
                if (rows != null && rows.Length > 0)
                {
                }
                else
                    dt_copy.ImportRow(row);
            }
            ToExcel(dt_copy);
        }
        //列印
        public void ToExcel(DataTable dt)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "分頁");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename={DateTime.Now:yyyyMMdd}.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

        }
    }
}