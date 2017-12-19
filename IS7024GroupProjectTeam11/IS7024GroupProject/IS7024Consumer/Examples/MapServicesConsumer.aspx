<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MapServicesConsumer.aspx.cs" Inherits="Examples_MapServicesConsumer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="tbAddress" runat="server" Width="424px"></asp:TextBox>
            <asp:Button ID="btnAutoComplete" runat="server" OnClick="btnAutoComplete_Click" Text="Invoke AddressAutoComplete" />
            <br />
            <br />
            <br />
            <asp:TextBox ID="tbAutoCompleteResults" runat="server" Height="385px" TextMode="MultiLine" Width="699px" Wrap="False"></asp:TextBox>
            <br />
        </div>
    </form>
</body>
</html>
