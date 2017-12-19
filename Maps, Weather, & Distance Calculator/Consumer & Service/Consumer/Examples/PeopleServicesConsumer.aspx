<%@ Page Language="C#" validateRequest="false" AutoEventWireup="true" CodeFile="PeopleServicesConsumer.aspx.cs" Inherits="Examples_PeopleServicesConsumer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            height: 286px;
        }
        .auto-style2 {
            width: 432px;
        }
        .auto-style3 {
            width: 141px;
        }
        .auto-style4 {
            width: 637px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lblClients" runat="server" Text="Clients"></asp:Label>
                        <br />
                        <asp:ListBox ID="lbClients" runat="server" AutoPostBack="True" Height="176px" OnSelectedIndexChanged="lbClients_SelectedIndexChanged" Width="350px"></asp:ListBox>
                        <asp:Button ID="btnReloadClients" runat="server" OnClick="btnReloadClients_Click" Text="Reload" />
                    </td>
                    <td class="auto-style3">
                        <asp:Label ID="lblDistance" runat="server" Text="Distance"></asp:Label>
                        <br />
                        <asp:TextBox ID="tbDistance" runat="server" ReadOnly="True"></asp:TextBox>
                    </td>
                    <td class="auto-style4">
                        <asp:Label ID="lblFieldReps" runat="server" Text="Field Reps"></asp:Label>
                        <br />
                        <asp:ListBox ID="lbFieldReps" runat="server" Height="176px" OnSelectedIndexChanged="lbFieldReps_SelectedIndexChanged" Width="350px" AutoPostBack="True"></asp:ListBox>
                        <asp:Button ID="btnReloadFieldReps" runat="server" OnClick="btnReloadFieldReps_Click" Text="Reload" />
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lblClientTemperature" runat="server" Text="Temperature"></asp:Label>
                        <asp:TextBox ID="tbClientTemperature" runat="server" ReadOnly="True"></asp:TextBox>
                        <br />
                        <asp:Label ID="lblClientPrecipitation" runat="server" Text="Precipitation"></asp:Label>
                        <asp:TextBox ID="tbClientPrecipitation" runat="server" ReadOnly="True"></asp:TextBox>
                    </td>
                    <td class="auto-style3">
                        <asp:TextBox ID="tbClientAddress" runat="server" ReadOnly="True" Visible="False" Width="45px"></asp:TextBox>
                        <asp:TextBox ID="tbFieldRepAddress" runat="server" ReadOnly="True" Visible="False" Width="37px"></asp:TextBox>
                        </td>
                    <td class="auto-style4">
                        <asp:Label ID="lblFieldRepsTemperature" runat="server" Text="Temperature"></asp:Label>
                        <asp:TextBox ID="tbFieldRepsTemperature" runat="server" ReadOnly="True"></asp:TextBox>
                        <br />
                        <asp:Label ID="lblFieldRepsHumidity" runat="server" Text="Humidity"></asp:Label>
                        <asp:TextBox ID="tbFieldRepsHumidity" runat="server" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <br />
                        <asp:Label ID="lblClientEncounters" runat="server" Text="Encounters"></asp:Label>
                        <br />
                        <asp:ListBox ID="lbClientEncounters" runat="server" Height="94px" Width="349px"></asp:ListBox>
                        <br />
                        <br />
                        <asp:Label ID="lblNewEncounter" runat="server" Text="Add Encounter"></asp:Label>
                        <br />
                        <asp:Label ID="lblEncounterType" runat="server" Text="Type"></asp:Label>
                        <asp:DropDownList ID="ddlEncounterTypes" runat="server">
                            <asp:ListItem>Visit</asp:ListItem>
                            <asp:ListItem>Call</asp:ListItem>
                            <asp:ListItem>eMail</asp:ListItem>
                            <asp:ListItem>Text</asp:ListItem>
                        </asp:DropDownList>
&nbsp;
                        <br />
                        <asp:Label ID="lblEncounterInfo" runat="server" Text="Info"></asp:Label>
                        <asp:TextBox ID="tbEncounter" runat="server" Height="87px" TextMode="MultiLine" Width="303px"></asp:TextBox>
                        <asp:Button ID="btnAddEncounter" runat="server" OnClick="btnAddEncounter_Click" Text="Add" />
                    </td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                </tr>
            </table>
            <br />
        </div>
    </form>
</body>
</html>

