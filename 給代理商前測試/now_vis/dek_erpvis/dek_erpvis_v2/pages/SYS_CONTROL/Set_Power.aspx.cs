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
    public partial class Set_Power : System.Web.UI.Page
    {
        int count = 1;
        public string acc = "";
        public string color = "";
        public string[] GroupShow = new string[7] { "none", "none", "none", "none", "none", "none", "none" };
        public string AutoSetShow = "False";
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
                        show();
                        show_treeview();
                        TreeView_Result.CollapseAll();
                    }
                    else
                    {
                        show("");
                    }
                }
                else
                    Response.Write("<script>alert('您無法瀏覽此頁面 請向該單位主管申請權限!');location.href='../index.aspx';</script>");
            }
            else
                Response.Redirect(myclass.logout_url);
        }
        //印出目前所有功能
        private void show(string Textbox_Select = "")
        {
            string condition = "";
            if (Textbox_Select != "")
                condition = $" where LINE_ID like '%{Textbox_Select}%' ";
            //之後改成撈取ORACLE的產品編號
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            //string sqlcmd = $" select * from web_pages where WEB_OPEN = 'Y' and  judge = '1'";
            string sqlcmd = $" select * from web_pages";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            ListItem list = null;
            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list = new ListItem(DataTableUtils.toString(row["WEB_PAGENAME"]), DataTableUtils.toString(row["WEB_URL"]));
                    switch (row["WEB_DPM"].ToString().ToUpper())
                    {
                        case "SLS":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_SLS.Items.Contains(list))
                                    CheckBoxList_SLS.Items.Add(list);
                                GroupShow[0] = "";
                            }
                            break;
                        case "PCD":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_PCD.Items.Contains(list))
                                    CheckBoxList_PCD.Items.Add(list);
                                GroupShow[1] = "";
                            }

                            break;
                        case "WHE":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_WHE.Items.Contains(list))
                                    CheckBoxList_WHE.Items.Add(list);
                                GroupShow[2] = "";
                            }

                            break;
                        case "PMD":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_PMD.Items.Contains(list))
                                    CheckBoxList_PMD.Items.Add(list);
                                GroupShow[3] = "";
                            }

                            break;
                        case "CNC":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_CNC.Items.Contains(list))
                                    CheckBoxList_CNC.Items.Add(list);
                                GroupShow[4] = "";
                            }

                            break;

                        case "APS":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_APS.Items.Contains(list))
                                    CheckBoxList_APS.Items.Add(list);
                                GroupShow[5] = "";
                            }
                            break;
                        case "DES":

                            if (DataTableUtils.toString(row["WEB_OPEN"]) == "Y")
                            {
                                if (!CheckBoxList_DES.Items.Contains(list))
                                    CheckBoxList_DES.Items.Add(list);
                                GroupShow[6] = "";
                            }
                            break;

                    }
                    //CheckBoxList_Edit.Items.Add(list);
                }
            }

            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            sqlcmd = $" select DPM_NAME,DPM_NAME2 from department where DPM_GROUP IS NULL";
            dt = DataTableUtils.GetDataTable(sqlcmd);
            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list = new ListItem(DataTableUtils.toString(row["DPM_NAME2"]), DataTableUtils.toString(row["DPM_NAME"]));
                    CheckBoxList_dpm.Items.Add(list);
                }
            }
            //兩段式新增改一段 0531 juiEdit 
            if (WebUtils.GetAppSettings("SetPower_AutoFlow") == "0")
                Button_dpm.Visible = false;
        }
        ////如果表單內有資料，就顯示在Treeview上
        private void show_treeview()
        {
            //出現checkbox
            //TreeView_Result.ShowCheckBoxes = TreeNodeTypes.All;
            TreeView_Result.ShowCheckBoxes = TreeNodeTypes.None;
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sql_cmd = "select distinct dpm from page_tree ";
            DataTable dt = DataTableUtils.GetDataTable(sql_cmd);

            if (HtmlUtil.Check_DataTable(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    TreeNode P_Node = new TreeNode();
                    P_Node.Text = DataTableUtils.toString(row["dpm"]);
                    P_Node.Value = DataTableUtils.toString(row["dpm"]);
                    P_Node.Checked = true;

                    TreeView_Result.Nodes.Add(P_Node);
                    checkchild(P_Node, DataTableUtils.toString(row["dpm"]));
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
        private void CheckItems(TreeNode node)
        {
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            string sqlcmd = "select * from page_tree";
            DataTable dt = DataTableUtils.GetDataTable(sqlcmd);
            bool ok = false;
            if (dt != null)
            {
                DataTable dt_Copy = dt.Clone();

                foreach (TreeNode childNode in node.ChildNodes)
                {
                    dt_Copy.Rows.Add(count, node.Value, childNode.Value);
                    count++;
                }

                GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);

                //foreach(DataRow dr in dt_Copy.Rows)
                //    DataTableUtils.Insert_DataRow("page_tree", dr);
                DataTableUtils.Insert_TableRows("page_tree", dt_Copy);
            }
        }

        //執行新增動作的時候，要把資料表清空
        private void Delete_Data()
        {
            //先清空該資料表
            GlobalVar.UseDB_setConnString(myclass.GetConnByDekVisErp);
            DataTableUtils.Delete_Record("page_tree", " id <> 0 ");
        }
        private void ClearCheckList()
        {
            foreach (Control ct in Panel1.Controls)
            {
                if (ct is CheckBoxList)
                {
                    CheckBoxList ctt = (CheckBoxList)ct;
                    ctt.Items.Clear();
                }
            }
        }

        /*------------------------------------------------------按鈕事件----------------------------------------------------------*/
        //新增產線節點
        protected void Button_dpm_Click(object sender, EventArgs e)
        {
            if (TextBox_dpm.Text != "")
            {
                string result = TreeViewOps.Add_Dpm(TreeView_Result, TextBox_dpm.Text);

                if (result == "")
                {
                    if (WebUtils.GetAppSettings("SetPower_AutoFlow") == "1")
                        TextBox_dpm.Text = "";
                }
                else
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
                if (WebUtils.GetAppSettings("SetPower_AutoFlow") == "1")
                    ClearCheckList();
                show("");
            }
            else
            {
                if (WebUtils.GetAppSettings("SetPower_AutoFlow") == "1")
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('請輸入群組');", true);
            }
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
            //autosetion 0531 juiedit
            string nodetxt = TextBox_dpm.Text;
            if (WebUtils.GetAppSettings("SetPower_AutoFlow") == "0")
            {
                Button_dpm_Click(null, null);
                TextBox_dpm.Text = "";
            }
            foreach (Control ct in Panel1.Controls)
            {
                if (ct is CheckBoxList)
                {
                    string result = TreeViewOps.Add_Node(TreeView_Result, (CheckBoxList)ct, nodetxt);//CheckBoxList_Edit
                    if (result != "")
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{result}');", true);
                }
            }
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
        //快速搜尋
        protected void Button_Search_Click(object sender, EventArgs e)
        {
            ClearCheckList();
            show(TextBox_Search.Text);
            TextBox_Search.Text = "";
        }
        ////按鈕事件
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            Delete_Data();
            foreach (TreeNode node in TreeView_Result.Nodes)
                CheckItems(node);
            Response.Write("<script>alert('設定完畢，即將進入設定畫面');location.href='Set_Power.aspx'; </script>");
        }

        protected void ImageButton_Open_Click(object sender, ImageClickEventArgs e)
        {
            string ID_Name = ((ImageButton)sender).ID.ToString();
            if (ID_Name == "ImageButton_Open")
                TreeView_Result.ExpandAll();
            else if (ID_Name == "ImageButton_Shrink")
                TreeView_Result.CollapseAll();
            else
                TreeView_Result.Nodes.Clear();
            show("");
        }

        protected void ImageButton_Shrink_Click(object sender, ImageClickEventArgs e)
        {
            string ID_Name = ((ImageButton)sender).ID.ToString();
            if (ID_Name == "ImageButton_Open")
                TreeView_Result.ExpandAll();
            else if (ID_Name == "ImageButton_Shrink")
                TreeView_Result.CollapseAll();
            else
                TreeView_Result.Nodes.Clear();
            show("");
        }
    }
}