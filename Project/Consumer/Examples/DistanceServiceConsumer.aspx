<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DistanceServiceConsumer.aspx.cs" Inherits="Examples_DistanceServiceConsumer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblLat1" runat="server" Text="Latitude1"></asp:Label>
            <asp:TextBox ID="tbLat1" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblLat2" runat="server" Text="Longitude1"></asp:Label>
            <asp:TextBox ID="tbLng1" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblLat3" runat="server" Text="Latitude2"></asp:Label>
            <asp:TextBox ID="tbLat2" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblLat4" runat="server" Text="Longitude2"></asp:Label>
            <asp:TextBox ID="tbLng2" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnInvoke" runat="server" Text="Distance" OnClick="btnInvoke_Click" />
            <asp:TextBox ID="tbDistance" runat="server"></asp:TextBox>
        </div>
    </form>
</body>
</html>
