<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Create_INI.aspx.cs" Inherits="dek_erpvis_v2.Create_INI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="background-color: cyan">
                產生資料庫相關資料(須加密)<br />
                資料庫類型：<asp:TextBox ID="TextBox1" runat="server">MySQL</asp:TextBox><br />
                資料庫名稱：<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox><br />
                資料庫IP：<asp:TextBox ID="TextBox3" runat="server"></asp:TextBox><br />
                資料庫帳號：<asp:TextBox ID="TextBox4" runat="server"></asp:TextBox><br />
                資料庫密碼：<asp:TextBox ID="TextBox5" runat="server"></asp:TextBox><br />
                <asp:Button ID="Button1" runat="server" Text="產生INI" OnClick="Button1_Click" />
            </div>
            <div style="background-color: silver">
                產生語法或其他(無須加密)<br />
                資料庫名稱：<asp:TextBox ID="TextBox6" runat="server"></asp:TextBox><br />
                資料表名稱：<asp:TextBox ID="TextBox7" runat="server" TextMode="MultiLine" style="resize:none;height:500%" ></asp:TextBox><br />
                資料表語法：<asp:TextBox ID="TextBox8" runat="server" TextMode="MultiLine" ></asp:TextBox><br />
                <asp:Button ID="Button2" runat="server" Text="產生INI" OnClick="Button2_Click" />
            </div>
            <div style="background-color: greenyellow">
                產生資料(須加密)<br />
                加密名稱：<asp:TextBox ID="TextBox9" runat="server"></asp:TextBox><br />
                加密資料：<asp:TextBox ID="TextBox10" runat="server" ></asp:TextBox><br />
                <asp:Button ID="Button3" runat="server" Text="產生INI" OnClick="Button3_Click" />
            </div>
        </div>
    </form>
</body>
</html>
