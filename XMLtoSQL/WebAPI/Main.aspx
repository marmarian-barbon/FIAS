<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="WebAPI.Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="Label1" runat="server" BackColor="#999966" Text="Номер региона" Width="30%"></asp:Label>
        <asp:TextBox ID="tbRegionName" runat="server" Width="10%"></asp:TextBox>
        <p>
            <asp:Label ID="Label2" runat="server" BackColor="#999966" Text="Название района" Width="20%"></asp:Label>
            <asp:CheckBox ID="chbRayonOff" runat="server" Font-Size="X-Small" Text="официальное" Width="10%" />
            <asp:TextBox ID="tbRayonName" runat="server" Width="30%"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="Label3" runat="server" BackColor="#999966" Text="Название города" Width="20%"></asp:Label>
            <asp:CheckBox ID="chbTownOff" runat="server" Font-Size="X-Small" Text="официальное" Width="10%" />
            <asp:TextBox ID="tbTownName" runat="server" Width="30%"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="Label4" runat="server" BackColor="#999966" Text="Название улицы" Width="20%"></asp:Label>
            <asp:CheckBox ID="chbStreetOff" runat="server" Font-Size="X-Small" Text="официальное" Width="10%" />
            <asp:TextBox ID="tbStreetName" runat="server" Width="30%"></asp:TextBox>
        </p>
        <p>
            <asp:CheckBox ID="chbUseHouse" runat="server" Text="использовать данные о домах" />
        </p>
        <p>
            <asp:Label ID="Label5" runat="server" BackColor="#999966" Text="Номер дома" Width="30%"></asp:Label>
            <asp:TextBox ID="tbHouseNumber" runat="server" Width="30%"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="Label6" runat="server" BackColor="#999966" Text="Почтовый индекс" Width="30%"></asp:Label>
            <asp:TextBox ID="tbPostalIndex" runat="server" Width="30%"></asp:TextBox>
        </p>
        <asp:Button ID="btnSend" runat="server" OnClick="SendToNewPage" Text="Создать запрос" />
    </form>
</body>
</html>
