using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dek_erpvis_v2.cls;
using Support;

namespace dek_erpvis_v2.pages.SYS_CONTROL
{
    public partial class Set_DPM : System.Web.UI.Page
    {
        public string acc = "";
        public string color = "";
        //畫面一開始進來呈現的地方
        protected void Page_Load(object sender, EventArgs e)
        {
            //效能測試
            HttpCookie userInfo = Request.Cookies["userInfo"];
            if (userInfo != null)
            {
                acc = DataTableUtils.toString(userInfo["user_ACC"]);
                color = HtmlUtil.change_color(acc);
                if (HtmlUtil.Search_acc_Column(acc, "ADM") == "Y")
                {
                    if (!IsPostBack)
                    {
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

        ////如果表單內有資料，就顯示在Treeview上
        private void show_treeview()
        {
            //出現checkbox
            TreeView_Result.ShowCheckBoxes = TreeNodeTypes.All;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);

            string sql_cmd = "";
            if (myclass.NowdbType == "MySQL")
                sql_cmd = "select * from department where ( length(DPM_GROUP) = 0  OR DPM_GROUP IS NULL )and DPM_NAME <> 'SLS' and DPM_NAME <> 'PCD' and DPM_NAME <> 'WHE' and DPM_NAME <> 'PMD'";
            else
                sql_cmd = "select * from department where ( DATALENGTH(DPM_GROUP) = 0  OR DPM_GROUP IS NULL )and DPM_NAME <> 'SLS' and DPM_NAME <> 'PCD' and DPM_NAME <> 'WHE' and DPM_NAME <> 'PMD'";

            DataTable dt = DataTableUtils.GetDataTable(sql_cmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    TreeNode P_Node = new TreeNode();
                    P_Node.Text = DataTableUtils.toString(row["DPM_NAME2"]);
                    P_Node.Value = DataTableUtils.toString(row["DPM_NAME"]);
                    P_Node.Checked = true;

                    TreeView_Result.Nodes.Add(P_Node);
                    checkchild(P_Node, DataTableUtils.toString(row["DPM_NAME"]));
                }
            }
        }
        ////檢查子項目
        private void checkchild(TreeNode node, string PID)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = $"SELECT distinct page_tree.page_name,web_pages.WEB_PAGENAME FROM page_tree left join web_pages on web_pages.WEB_URL = page_tree.page_name where dpm = '{PID}'";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);

            foreach (DataRow row in dt.Rows)
            {
                TreeNode P_Node = new TreeNode();
                P_Node.Text = DataTableUtils.toString(row["WEB_PAGENAME"]);
                P_Node.Value = DataTableUtils.toString(row["page_name"]);
                P_Node.Checked = true;
                node.ChildNodes.Add(P_Node);
            }
        }
        //檢查是否含有子項目
        private int CheckItems(TreeNode node, DataTable dt, int Count)
        {
            bool OK = false;
            int count = Count + 1;
            if (dt != null)
            {
                DataTable dt_Copy = dt.Clone();
                dt_Copy.Rows.Add(count, node.Value, node.Text, "");
                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
                foreach (DataRow dr in dt_Copy.Rows)
                    OK = DataTableUtils.Insert_DataRow("department", dr);
                return count;
                //DataTableUtils.Insert_TableRows("department", dt_Copy);
            }
            else
                return 0;
        }

        //執行新增動作的時候，要把資料表清空
        private void Delete_Data()
        {
            //先清空該資料表
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            bool ok = DataTableUtils.Delete_Record("department", " DPM_NAME <> 'SLS' and DPM_NAME <> 'PCD' and DPM_NAME <> 'WHE' and DPM_NAME <> 'PMD' ");

        }

        /*------------------------------------------------------按鈕事件----------------------------------------------------------*/
        //新增產線節點
        protected void Button_dpm_Click(object sender, EventArgs e)
        {
            string result = TreeViewOps.Add_Dpm(TreeView_Result, TextBox_dpm.Text, TextBox_code.Text);
            if (result != "")
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
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
            //string result = TreeViewOps.Add_Node(TreeView_Result, CheckBoxList_Edit);
            //if (result != "")
            //    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
        }
        ////垂直移動
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
        ////水平移動
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
        ////展開 收合 清除 TreeView
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


        ////按鈕事件
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            Delete_Data();
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = "select * from department";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            int count = 0;
            if (dt != null && dt.Rows.Count != 0)
                count = dt.Rows.Count;
            foreach (TreeNode node in TreeView_Result.Nodes)
                count = CheckItems(node, dt, count);
            Response.Write("<script>alert('設定完畢，即將進入設定畫面');location.href='Set_DPM.aspx'; </script>");
        }
    }
}