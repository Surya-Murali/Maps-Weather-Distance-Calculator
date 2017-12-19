<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="WeatherServiceConsumer.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 217px;
        }
        .auto-style2 {
            height: 253px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="auto-style1">
    
        <br />
        <asp:Button ID="btnGetCityList" runat="server" OnClick="btnGetCityList_Click" Text="Get List Of Cities" />
        <br />
        <br />
        <asp:ListBox ID="lbCity" runat="server" OnSelectedIndexChanged="lbCity_SelectedIndexChanged" Width="329px" AutoPostBack="True"></asp:ListBox>
        <br />
        <br />
        <asp:Label ID="lblSunRise" runat="server" Text="Sunrise"></asp:Label>
        <br />
        <asp:TextBox ID="tbSunRise" runat="server" Enabled="False" ></asp:TextBox>
    
        <br />
        <br />
        <br />
    
    </div>

            <div class="auto-style2">
    
        <br />
        <asp:Button ID="btnLoadPeople" runat="server" Text="Load List Of People" OnClick="btnLoadPeople_Click" />
        <br />
        <br />
        <asp:ListBox ID="lbPeople" runat="server" Width="329px" AutoPostBack="True" OnSelectedIndexChanged="lbPeople_SelectedIndexChanged"></asp:ListBox>
        <br />
                <br />
        <asp:Label ID="lblPersonCity" runat="server" Text="City"></asp:Label>
        <asp:TextBox ID="tbPersonCity" runat="server" Enabled="False" ></asp:TextBox>
    
        <br />
                <br />
        <asp:Label ID="lblCityTemprature" runat="server" Text="Temperature"></asp:Label>
        <br />
        <asp:TextBox ID="tbCityTemperature" runat="server" Enabled="False" ></asp:TextBox>
    
        <br />
        <br />
        <br />
    
    </div>

    </form>
</body>
</html>
