<%@ Page Title="Lab5" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lab5.aspx.cs" Inherits="IS7024Labs.Labs.Lab5" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to the IS 7024 Lab codebase.</h2>

    <p>Labs will show up weekly in the instructors' GITHUB account.   Run the lab as it becomes available to you.   Modifications should be in your local repository.</p>


    <table>
        <tr>
            <td style="background-color: lightcyan; width: 20%; text-align: center;">Lab 5
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px; width: 197px;">
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    <asp:Label ID="lblClients" runat="server" Text="Clients"></asp:Label>
                    <asp:ListBox ID="lbClients" runat="server" Height="166px" Width="219px" AutoPostBack="True" OnSelectedIndexChanged="lbClients_SelectedIndexChanged"></asp:ListBox>
                    <asp:Button ID="btnClientLoad" runat="server" OnClick="btnClientLoad_Click" Text="Load" />
                </div>
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px; width: 192px;">
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    <asp:Label ID="lblFieldReps" runat="server" Text="Field Reps"></asp:Label>
                    <asp:ListBox ID="lbFieldReps" runat="server" Height="166px" Width="219px" AutoPostBack="True" OnSelectedIndexChanged="lbFieldReps_SelectedIndexChanged"></asp:ListBox>
                    <asp:Button ID="btnFieldRepLoad" runat="server" OnClick="btnFieldRepLoad_Click" Text="Load" />
                </div>
            </td>
        </tr>
        <tr>
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
                   , icon: "/Labs/Images/FieldRep.png"
                });
            }

            for (i = 0; i < ClientMarkers.length; i++) {
                position = new google.maps.LatLng(ClientMarkers[i][0], ClientMarkers[i][1]);
                marker = new google.maps.Marker({
                    position: position,
                    map: map
                    , icon: "/Labs/Images/Client.png"
                });
            }
            position = new google.maps.LatLng(SelectedClientMarker[0], SelectedClientMarker[1]);
            marker = new google.maps.Marker({
                position: position,
                map: map
                , icon: "/Labs/Images/SelectedClient.png"
            });

            position = new google.maps.LatLng(SelectedFieldRepMarker[0], SelectedFieldRepMarker[1]);
            marker = new google.maps.Marker({
                position: position,
                map: map
                , icon: "/Labs/Images/SelectedFieldRep.png"
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

</asp:Content>
