<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            height: 286px;
        }

        .auto-style3 {
            width: 141px;
        }

        .auto-style4 {
            width: 554px;
        }

        .auto-style5 {
            width: 443px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style5">
                        <asp:Label ID="lblClients" runat="server" Text="Clients"></asp:Label>
                        <br />
                        <asp:ListBox ID="lbClients" runat="server" AutoPostBack="True" Height="176px" Width="350px" OnSelectedIndexChanged="lbClients_SelectedIndexChanged"></asp:ListBox>
                        <asp:Button ID="btnClientLoad" runat="server" OnClick="btnClientLoad_Click" Text="Load" />
                        <%--<asp:Button ID="btnReloadClients" runat="server" Text="Reload" OnClick="btnReloadClients_Click" />--%>
                    </td>
                    <td class="auto-style3">
                        <asp:Label ID="lblDistance" runat="server" Text="Distance"></asp:Label>
                        <br />
                        <asp:TextBox ID="tbDistance" runat="server" ReadOnly="True" Width="81px"></asp:TextBox>
                    </td>
                    <td class="auto-style4">
                        <asp:Label ID="lblFieldReps" runat="server" Text="Field Reps"></asp:Label>
                        <br />  
                        <asp:ListBox ID="lbFieldReps" runat="server" Height="176px" Width="350px" AutoPostBack="True" OnSelectedIndexChanged="lbFieldReps_SelectedIndexChanged"></asp:ListBox>
                        <asp:Button ID="btnFieldRepLoad" runat="server" OnClick="btnFieldRepLoad_Click" Text="Load" />
                        <%--<asp:Button ID="btnReloadFieldReps" runat="server" Text="Reload" OnClick="btnReloadFieldReps_Click" />--%>
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
                    <td class="auto-style5">
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
                        <asp:Button ID="btnAddEncounter" runat="server" Text="Add" OnClick="btnAddEncounter_Click" />
                    </td>
                    <td style="background-color: lightcyan; width: 20%; text-align: center;">
                <asp:TextBox ID="tbClientMarkers" runat="server" ReadOnly="True" Visible="False" Width="45px" Height="5px"></asp:TextBox>
                <asp:TextBox ID="tbFieldRepMarkers" runat="server" ReadOnly="True" Visible="False" Width="45px" Height="5px"></asp:TextBox>
                <asp:TextBox ID="tbSelectedFieldRepMarker" runat="server" ReadOnly="True" Visible="False" Width="45px" Height="5px"></asp:TextBox>
                <asp:TextBox ID="tbSelectedClientMarker" runat="server" ReadOnly="True" Visible="False" Width="45px" Height="5px"></asp:TextBox>
                <asp:TextBox ID="tbDistancePathCoordinatesOfSelected" runat="server" ReadOnly="True" Visible="False" Width="45px" Height="5px"></asp:TextBox>
            </td>
                    <td colspan="2">
                        <div id="map_populate" style="width: 100%; height: 400px; border: 5px solid #5E5454;">
                        </div>
                    </td>
                </tr>
               
            </table>
            <br />
        </div>
    </form>
</body>
    <script src="https://maps.googleapis.com/maps/api/js?libraries=places&key=AIzaSyD7ctrGDGTFxWZVJEADDhFkfH4kLl0AwbY&v=3.exp&sensor=false" type="text/javascript"></script>
    <script type="text/javascript">  
        var mapcode;
        var map;
        function initialize() {
            mapcode = new google.maps.Geocoder();
            var lnt = new google.maps.LatLng(38.8990715, -93.2106506);
            var mapChoice = {
                zoom: 4,
                center: lnt,
                diagId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById('map_populate'), mapChoice);

            var FieldRepMarkers = <%= MyFieldRepMarkerPositions %>;
            var ClientMarkers = <%= MyClientMarkerPositions %>;
            var SelectedFieldRepMarker = <%= MySelectedFieldRepMarkerPosition %>;
            var SelectedClientMarker = <%= MySelectedClientMarkerPosition %>;
            var DistancePathCoordinates = <%= DistancePathCoordinatesOfSelected %>;

            var i, marker, position;
            for (i = 0; i < FieldRepMarkers.length; i++) {
                position = new google.maps.LatLng(FieldRepMarkers[i][0], FieldRepMarkers[i][1]);
                marker = new google.maps.Marker({
                    position: position,
                    map: map
                   , icon: "/Images/FieldRep.png"
                });
            }

            for (i = 0; i < ClientMarkers.length; i++) {
                position = new google.maps.LatLng(ClientMarkers[i][0], ClientMarkers[i][1]);
                marker = new google.maps.Marker({
                    position: position,
                    map: map
                    , icon: "/Images/Client.png"
                });
            }
            position = new google.maps.LatLng(SelectedClientMarker[0], SelectedClientMarker[1]);
            marker = new google.maps.Marker({
                position: position,
                map: map
                , icon: "/Images/SelectedClient.png"
            });

            position = new google.maps.LatLng(SelectedFieldRepMarker[0], SelectedFieldRepMarker[1]);
            marker = new google.maps.Marker({
                position: position,
                map: map
                , icon: "/Images/SelectedFieldRep.png"
            });

            var DistancePath = new google.maps.Polyline({
                path: DistancePathCoordinates,
                geodesic: true,
                strokeColor: '#FF0000',
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            DistancePath.setMap(map);


        }
        google.maps.event.addDomListener(window, 'load', initialize);
    </script>

</html>
