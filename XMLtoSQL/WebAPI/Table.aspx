<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Table.aspx.cs" Inherits="WebAPI.Table" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Получить следующий результат" />
        <p>
            <asp:Label ID="lblRegionCode" runat="server" Text="Код Региона" Width="10%"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblRegion" runat="server" Text="Регион" Width="20%"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblRayon" runat="server" Text="Район"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblTown" runat="server" Text="Город"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblStreet" runat="server" Text="Улица"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblHouseNumber" runat="server"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblPostalCode" runat="server"></asp:Label>
        </p>
    </form>
</body>
</html>
