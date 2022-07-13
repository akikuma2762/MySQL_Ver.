using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using Support;


namespace dek_erpvis_v2.pages.SYS_CONTROL
{
    public partial class Set_CraftOrder : System.Web.UI.Page
    {
        int count = 1;
        public string acc = "";
        public string color = "";
        clsDB_Server cls_hor = new clsDB_Server(myclass.GetConnByDekdekVisAssmHor);
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
                    if (!IsPostBack)
                    {
                        MainProcess();
                        TreeView_Result.CollapseAll();
                    }
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }

        private void MainProcess()
        {
            Show_Treeview();
            Set_MachineCheckbox();
            Set_CraftCheckbox();
        }

        //建立樹狀圖
        private void Show_Treeview()
        {
            //列印所有的機種部分
 
            string sqlcmd = "select 機形工藝.*  from 機形工藝  order by CONVERT(SUBSTRING_INDEX(機形工藝.編號, '_', -1) , signed)  ";
            DataTable dt = cls_hor.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                DataTable machine = dt.DefaultView.ToTable(true, new string[] { "所屬機型" });
                foreach (DataRow row in machine.Rows)
                {
                    TreeNode P_Node = new TreeNode();
                    P_Node.Text = DataTableUtils.toString(row["所屬機型"]);
                    P_Node.Value = DataTableUtils.toString(row["所屬機型"]);
                    P_Node.Checked = true;
                    TreeView_Result.Nodes.Add(P_Node);
                    checkchild(dt, P_Node, DataTableUtils.toString(row["所屬機型"]));
                }
            }
        }
        /// <summary>
        /// 顯示時，檢查子項目
        /// </summary>
        /// <param name="dt">大表DataTable</param>
        /// <param name="node">節點</param>
        /// <param name="PID">機型/上階編號</param>
        /// <param name="first">是否第一次進入Function Y→PID=機型 N→PID=上階編號</param>----OK
        private void checkchild(DataTable dt, TreeNode node, string PID, bool first = true)
        {
            string sqlcmd = "";

            if (first)
                sqlcmd = $"所屬機型='{PID}' and 上階工藝編號 IS NULL";
            else
                sqlcmd = $"上階工藝編號 = '{PID}'";

            DataRow[] rows = dt.Select(sqlcmd);

            for (int i = 0; i < rows.Length; i++)
            {
                TreeNode P_Node = new TreeNode();
                P_Node.Text = DataTableUtils.toString(rows[i]["工藝名稱"]);
                P_Node.Value = DataTableUtils.toString(rows[i]["工藝名稱"]);
                P_Node.Checked = true;
                node.ChildNodes.Add(P_Node);
                checkchild(dt, P_Node, DataTableUtils.toString(rows[i]["編號"]), false);
            }
        }

        //顯示出所有的機形
        private void Set_MachineCheckbox()
        {
          
            string sqlcmd = "select distinct NAM_ITEM from 組裝資料表";
            DataTable dt = cls_hor.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt) && CheckBoxList_Machine.Items.Count == 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ListItem listItem = new ListItem(DataTableUtils.toString(row["NAM_ITEM"]), DataTableUtils.toString(row["NAM_ITEM"]));
                    CheckBoxList_Machine.Items.Add(listItem);
                }
            }

        }

        //顯示出所有的已輸入工藝
        private void Set_CraftCheckbox()
        {
           
            string sqlcmd = "select distinct 工藝名稱 from 機形工藝";
            DataTable dt = cls_hor.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt) && CheckBoxList_Craft.Items.Count == 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ListItem listItem = new ListItem(DataTableUtils.toString(row["工藝名稱"]), DataTableUtils.toString(row["工藝名稱"]));
                    CheckBoxList_Craft.Items.Add(listItem);
                }
            }
        }
        /*------------------------------------------------------按鈕事件----------------------------------------------------------*/

        //儲存樹狀圖
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            Delete_Data();
            //取得整體樹狀結構

        }

        //刪除資料
        private void Delete_Data()
        {
            cls_hor.Delete_Record("機形工藝", "編號 IS NOT NULL");
            cls_hor.Delete_Record("機形名稱", "編號 IS NOT NULL");
        }
        //存入資料庫

        //跑遞迴


        //-----------------------------------OK---------------------------------------------
        //匯入工藝
        protected void Button_Craft_Click(object sender, EventArgs e)
        {
            //ASP無法對檔案路徑做讀取，所以先儲存到別的地方，之後再讀取那個地方
            string filename = FileUpload_File.PostedFile.FileName;
            HtmlUtil.FileUpload_Name(FileUpload_File, "Backup_Error_Image", true);

            //讀取EXCEL
            DataTable dt = new DataTable();
            int _Column_count = 0;
            int _Rows_count = 0;
            int _excel_statrRos = 1;
            int _excel_KeyRow = 1;
            ExcelIO excel = new ExcelIO($"{WebConfigurationManager.AppSettings["disk"]}:\\Backup_Error_Image\\{filename}");
            string _Name = "";

            //取得EXCEL的內容
            while (excel.GetValue(_excel_statrRos, _Column_count + 1) != "")
            {
                _Name = excel.GetValue(_excel_statrRos, _Column_count);
                _Column_count++;
            }
            while (excel.GetValue(_Rows_count + 1, _excel_KeyRow) != "")
                _Rows_count++;
            DataRow dr = dt.NewRow();
            for (int i = 1; i < _Rows_count; i++)
            {
                dr = dt.NewRow();
                for (int j = 1; j <= _Column_count; j++)
                {
                    if (i == 1)
                        dt.Columns.Add(excel.GetValue(1, j).ToString());
                    dr[excel.GetValue(1, j).ToString()] = excel.GetValue(i + 1, j).ToString();
                }
                dt.Rows.Add(dr);
            }

            string nowtime = DateTime.Now.ToString("yyyyMMddHHmmss");

            //存入資料庫
           
            string sqlcmd = "select * from 機形工藝";
            DataTable dts = cls_hor.GetDataTable(sqlcmd);

            if (dts != null)
            {
               
                if (dt.Rows.Count == cls_hor.Insert_TableRows("機形工藝", dt))
                {
                    //分離機型，存入資料庫
                    DataTable machine = dt.DefaultView.ToTable(true, new string[] { "所屬機型" });
                    machine.Columns.Add("編號");
                    machine.Columns.Add("使用中");
                    machine.Columns.Add("更新時間");
                    machine.Columns["編號"].SetOrdinal(0);
                    int count = 1;
                    foreach (DataRow row in machine.Rows)
                    {
                        row["編號"] = $"M_{count}";
                        row["使用中"] = $"Y";
                        row["更新時間"] = nowtime;
                        count++;
                    }
                    //存入資料庫
                 
                    sqlcmd = "select * from 機形名稱";
                    dts = cls_hor.GetDataTable(sqlcmd);

                    if (dts != null)
                    {
                      
                        if (machine.Rows.Count == cls_hor.Insert_TableRows("機形名稱", machine))
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('匯入成功');location.href='Set_CraftOrder.aspx';</script>");
                        else
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('匯入失敗');location.href='Set_CraftOrder.aspx';</script>");
                    }
                    else
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('匯入失敗');location.href='Set_CraftOrder.aspx';</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('匯入失敗');location.href='Set_CraftOrder.aspx';</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", $"<script>alert('匯入失敗');location.href='Set_CraftOrder.aspx';</script>");
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

        //刪除節點
        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            string result = TreeViewOps.Change_Node(TreeView_Result, "移除");
            result = result == "" ? "移除成功" : result;
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
        }

        //新增機形
        protected void Button_AddMachine_Click(object sender, EventArgs e)
        {
            TreeViewOps.Add_Node(TreeView_Result, CheckBoxList_Machine);
        }

        //新增工藝
        protected void Button_AddCraft_Click(object sender, EventArgs e)
        {
            if (TextBox_Craft.Text == "")
                TreeViewOps.Add_Node(TreeView_Result, CheckBoxList_Craft);
            else
                TreeViewOps.Add_Dpm(TreeView_Result, TextBox_Craft.Text);
            TextBox_Craft.Text = "";
        }
    }
}