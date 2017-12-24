using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Newtonsoft.Json.Linq;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Reset the JavaScript connectors
            MyClientMarkerPositions = "[]";
            MyFieldRepMarkerPositions = "[]";
            MySelectedClientMarkerPosition = "[]";
            MySelectedFieldRepMarkerPosition = "[]";
            DistancePathCoordinatesOfSelected = "[]";
        }
    }

    protected void btnClientLoad_Click(object sender, EventArgs e)
    {
        string url = @"http://localhost:52236/Clients/xml";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string restResult = "";
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }
        }
        XmlDocument xmlPeople = new XmlDocument();
        xmlPeople.LoadXml(restResult);

        string currentPath = Server.MapPath(".");
        string xmlPath = currentPath + @"\XMLFiles\";
        string xsdFilePath = xmlPath + "PeopleClients.xslt";
        string tempxmlFilePath = xmlPath + "TempPeopleClients.xml";
        string tempTextFilePath = xmlPath + "TempPeopleClients.txt";

        //Create a new XslTransform object.
        XslCompiledTransform xslt = new XslCompiledTransform();

        //Load the stylesheet.
        xslt.Load(xsdFilePath);
        xmlPeople.Save(tempxmlFilePath);

        xslt.Transform(tempxmlFilePath, tempTextFilePath);

        string[] Clients = File.ReadAllLines(tempTextFilePath);
        lbClients.Items.Clear();

        for (int index = 0; index <= Clients.Length - 1; index++)
        {
            string[] ClientStuff = Clients[index].Split('|');
            string address = ClientStuff[4] + ", " + ClientStuff[5];
            string fullname = ClientStuff[3] + " " + ClientStuff[1] + " " + ClientStuff[2];
            string id = ClientStuff[0];
            decimal latitude = 0;
            decimal longitude = 0;
            GetGeoPoints(address, ref latitude, ref longitude);
            string value = id + "|" + address + "|" + latitude.ToString() + "|" + longitude.ToString();
            string display = fullname;
            lbClients.Items.Add(new ListItem(display, value));
        }
        FillInClientMarkers();
    }

    protected void btnFieldRepLoad_Click(object sender, EventArgs e)
    {
        // Using JSON, load the Listbox
        string url = @"http://localhost:52236/FieldReps/json";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string restResult = "";
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }
        }

        dynamic FieldRepStuff = JObject.Parse(restResult);
        lbFieldReps.Items.Clear();
        foreach (var Person in FieldRepStuff.People.Person)
        {
            string role = Person.Role;
            if (role == "Field Rep")
            {
                string id = Person["@id"];
                string nameLast = "";
                string nameSuffix = "";

                try
                {
                    nameLast = Person.Name.Last["#text"];
                    nameSuffix = Person.Name.Last["@suffix"];
                }
                catch
                {
                    nameLast = Person.Name.Last;
                }
                string nameFirst = Person.Name.First;
                string streetAddress = "";
                string city = "";

                try
                {
                    streetAddress = Person.WorkLocation[1]["StreetAddress"];
                    city = Person.WorkLocation[1]["City"];
                }
                catch
                {
                    streetAddress = Person.WorkLocation["StreetAddress"];
                    city = Person.WorkLocation["City"];
                }

                string fullname = nameFirst + " " + nameLast + " " + nameSuffix;
                string address = streetAddress + ", " + city;
                decimal latitude = 0;
                decimal longitude = 0;
                GetGeoPoints(streetAddress, ref latitude, ref longitude);
                string value = id + "|" + address + "|" + latitude.ToString() + "|" + longitude.ToString();
                string display = fullname;
                lbFieldReps.Items.Add(new ListItem(display, value));
            }
            FillInFieldRepMarkers();
        }
    }


    protected void lbClients_SelectedIndexChanged(object sender, EventArgs e)
    {
        string id = lbClients.SelectedItem.Value;
        if (id.Contains("|"))
        {
            string[] ClientStuff = id.Split('|');
            id = ClientStuff[0];
        }
        string url = @"http://localhost:52236/Clients/xml";
        url += "/" + id;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string restResult = "";
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }

        }
        XmlDocument xmlClient = new XmlDocument();
        xmlClient.LoadXml(restResult);
        string ClientCity = xmlClient.SelectSingleNode("//City[1]")?.InnerXml;
        string ClientAddress = xmlClient.SelectSingleNode("//StreetAddress[1]")?.InnerXml;
        tbClientAddress.Text = ClientAddress + "," + ClientCity;  //Save it off in a hidden field

        lbClientEncounters.Items.Clear();
        tbEncounter.Text = "";
        ddlEncounterTypes.SelectedIndex = -1;
        XmlNodeList xnlClientEncounters = xmlClient.SelectNodes("//Person/Encounters/Encounter");
        foreach (XmlNode xnEncounter in xnlClientEncounters)
        {
            string EncounterType = xnEncounter.SelectSingleNode("./@type")?.InnerXml;
            string EncounterDescription = xnEncounter.InnerXml;
            lbClientEncounters.Items.Add(new ListItem(EncounterType + ": " + EncounterDescription));
        }


        tbClientPrecipitation.Text = "";
        tbClientTemperature.Text = "";

        //**Making the SOAP Weather Service Call**

        WeatherService.WeatherServiceSoapClient weather = new WeatherService.WeatherServiceSoapClient();
        try
        {
            XElement xeWeather = weather.GetWeather(ClientCity);
            if (xeWeather != null)
            {
                string xml = xeWeather.ToString();

                XmlDocument xmlWeather = new XmlDocument();
                xmlWeather.LoadXml(xml);

                XmlNode xn = xmlWeather.SelectSingleNode(@"/current/temperature/@value");
                double tempKelvin = Convert.ToDouble(xn.Value);
                double tempCelcius = Math.Round(tempKelvin - 273.15, 2);
                double tempFahrenheit = Math.Round(tempCelcius * 1.8 + 32, 2);
                string temp = tempCelcius.ToString() + "C" + "   " + tempFahrenheit.ToString() + "F";
                tbClientTemperature.Text = temp;

                xn = xmlWeather.SelectSingleNode(@"/current/precipitation");
                string precipitation = xn.InnerXml;
                tbClientPrecipitation.Text = precipitation;
            }
        }
        catch (Exception ex)
        { }

        if (tbFieldRepAddress.Text != "") // A field rep has been selected and the address saved off
        {
            string addressClient = tbClientAddress.Text;
            string addressFieldRep = tbFieldRepAddress.Text;

            //**Making the SOAP Map Service Call**

            MapService.MapServiceSoapClient mapService = new MapService.MapServiceSoapClient();
            XElement xeAddressAutoComplete = mapService.AddressAutoComplete(addressClient);
            XmlDocument xmlAddressAutoComplete = new XmlDocument();
            xmlAddressAutoComplete.LoadXml(xeAddressAutoComplete.ToString());
            XmlNode xnPlaceID = xmlAddressAutoComplete.SelectSingleNode("//place_id");
            string sPlaceIDClient = xnPlaceID.InnerXml;

            xeAddressAutoComplete = mapService.AddressAutoComplete(addressFieldRep);
            xmlAddressAutoComplete = new XmlDocument();
            xmlAddressAutoComplete.LoadXml(xeAddressAutoComplete.ToString());
            xnPlaceID = xmlAddressAutoComplete.SelectSingleNode("//place_id");
            string sPlaceIDFieldRep = xnPlaceID.InnerXml;


            XElement xePlaceDetails = mapService.PlaceDetails(sPlaceIDClient);
            XmlDocument xmlPlaceDetails = new XmlDocument();
            xmlPlaceDetails.LoadXml(xePlaceDetails.ToString());
            string ClientLat = xmlPlaceDetails.SelectSingleNode("//geometry/location/lat")?.InnerXml;
            string ClientLng = xmlPlaceDetails.SelectSingleNode("//geometry/location/lng")?.InnerXml;

            xePlaceDetails = mapService.PlaceDetails(sPlaceIDFieldRep);
            xmlPlaceDetails = new XmlDocument();
            xmlPlaceDetails.LoadXml(xePlaceDetails.ToString());
            string FieldRepLat = xmlPlaceDetails.SelectSingleNode("//geometry/location/lat")?.InnerXml;
            string FieldRepLng = xmlPlaceDetails.SelectSingleNode("//geometry/location/lng")?.InnerXml;


            DistanceService.DistanceServiceClient distanceService = new DistanceService.DistanceServiceClient();
            double dblDistance = distanceService.DistanceBetweenTwoPoints(double.Parse(ClientLat), double.Parse(ClientLng), double.Parse(FieldRepLat), double.Parse(FieldRepLng));
            tbDistance.Text = Math.Round(dblDistance, 2).ToString();
        }
        FillInClientMarkers();
    }

    protected void lbFieldReps_SelectedIndexChanged(object sender, EventArgs e)
    {
        string id = lbFieldReps.SelectedItem.Value;
        if (id.Contains("|"))
        {
            string[] FieldRepStuff = id.Split('|');
            id = FieldRepStuff[0];
        }
        string url = @"http://localhost:52236/FieldReps/xml";
        url += "/" + id;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string restResult = "";
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }

        }
        XmlDocument xmlFieldRep = new XmlDocument();
        xmlFieldRep.LoadXml(restResult);
        string FieldRepCity = xmlFieldRep.SelectSingleNode("//City[1]")?.InnerXml;
        string FieldRepAddress = xmlFieldRep.SelectSingleNode("//StreetAddress[1]")?.InnerXml;
        tbFieldRepAddress.Text = FieldRepAddress + "," + FieldRepCity;  //Save it off in a hidden field

        tbFieldRepsHumidity.Text = "";
        tbFieldRepsTemperature.Text = "";
        WeatherService.WeatherServiceSoapClient weather = new WeatherService.WeatherServiceSoapClient();
        try
        {
            XElement xeWeather = weather.GetWeather(FieldRepCity);
            if (xeWeather != null)
            {
                string xml = xeWeather.ToString();

                XmlDocument xmlWeather = new XmlDocument();
                xmlWeather.LoadXml(xml);

                XmlNode xn = xmlWeather.SelectSingleNode(@"/current/temperature/@value");
                double tempKelvin = Convert.ToDouble(xn.Value);
                double tempCelcius = Math.Round(tempKelvin - 273.15, 2);
                double tempFahrenheit = Math.Round(tempCelcius * 1.8 + 32, 2);
                string temp = tempCelcius.ToString() + "C" + "   " + tempFahrenheit.ToString() + "F";
                tbFieldRepsTemperature.Text = temp;

                xn = xmlWeather.SelectSingleNode(@"/current/humidity/@value");
                string humidity = xn.InnerXml;
                xn = xmlWeather.SelectSingleNode(@"/current/humidity/@unit");
                humidity += xn.InnerXml;
                tbFieldRepsHumidity.Text = humidity;
            }
        }
        catch (Exception ex)
        { }

        if (tbClientAddress.Text != "") // A Client has been selected and the address saved off
        {
            string addressClient = tbClientAddress.Text;
            string addressFieldRep = tbFieldRepAddress.Text;

            MapService.MapServiceSoapClient mapService = new MapService.MapServiceSoapClient();
            XElement xeAddressAutoComplete = mapService.AddressAutoComplete(addressClient);
            XmlDocument xmlAddressAutoComplete = new XmlDocument();
            xmlAddressAutoComplete.LoadXml(xeAddressAutoComplete.ToString());
            XmlNode xnPlaceID = xmlAddressAutoComplete.SelectSingleNode("//place_id");
            string sPlaceIDClient = xnPlaceID.InnerXml;

            xeAddressAutoComplete = mapService.AddressAutoComplete(addressFieldRep);
            xmlAddressAutoComplete = new XmlDocument();
            xmlAddressAutoComplete.LoadXml(xeAddressAutoComplete.ToString());
            xnPlaceID = xmlAddressAutoComplete.SelectSingleNode("//place_id");
            string sPlaceIDFieldRep = xnPlaceID.InnerXml;


            XElement xePlaceDetails = mapService.PlaceDetails(sPlaceIDClient);
            XmlDocument xmlPlaceDetails = new XmlDocument();
            xmlPlaceDetails.LoadXml(xePlaceDetails.ToString());
            string ClientLat = xmlPlaceDetails.SelectSingleNode("//geometry/location/lat")?.InnerXml;
            string ClientLng = xmlPlaceDetails.SelectSingleNode("//geometry/location/lng")?.InnerXml;

            xePlaceDetails = mapService.PlaceDetails(sPlaceIDFieldRep);
            xmlPlaceDetails = new XmlDocument();
            xmlPlaceDetails.LoadXml(xePlaceDetails.ToString());
            string FieldRepLat = xmlPlaceDetails.SelectSingleNode("//geometry/location/lat")?.InnerXml;
            string FieldRepLng = xmlPlaceDetails.SelectSingleNode("//geometry/location/lng")?.InnerXml;


            DistanceService.DistanceServiceClient distanceService = new DistanceService.DistanceServiceClient();
            double dblDistance = distanceService.DistanceBetweenTwoPoints(double.Parse(ClientLat), double.Parse(ClientLng), double.Parse(FieldRepLat), double.Parse(FieldRepLng));
            tbDistance.Text = Math.Round(dblDistance, 2).ToString();
        }
        FillInFieldRepMarkers();
    }

    protected void btnAddEncounter_Click(object sender, EventArgs e)
    {
        string id = lbClients.SelectedItem.Value;
        if (id.Contains("|"))
        {
            string[] FieldRepStuff = id.Split('|');
            id = FieldRepStuff[0];
        }
        string url = @"http://localhost:52236/Clients/xml";
        url += "/" + id;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string restResult = "";
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }

        }
        XmlDocument xmlClient = new XmlDocument();
        xmlClient.LoadXml(restResult);

        XmlNode ndEncounters = xmlClient.SelectSingleNode("//Person/Encounters");
        if (ndEncounters == null)
        {
            // Need to create the Encounters node
            ndEncounters = xmlClient.CreateElement("Encounters");
            xmlClient.SelectSingleNode("//Person").AppendChild(ndEncounters);
        }
        XmlNode ndNewEncounter = xmlClient.CreateNode("element", "Encounter", "");
        XmlAttribute typeOfEncounter = xmlClient.CreateAttribute("type");
        typeOfEncounter.InnerXml = ddlEncounterTypes.SelectedValue;
        ndNewEncounter.Attributes.Append(typeOfEncounter);
        ndNewEncounter.InnerXml = tbEncounter.Text;
        ndEncounters.AppendChild(ndNewEncounter);


        // Now save it
        // Pull the edited person into an XMLDocument

        //Get the ID from the Person XML in the textbox

        byte[] payload = Encoding.UTF8.GetBytes(xmlClient.OuterXml);
        request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = "text/xml; charset=utf-8";
        request.Method = "Put";
        request.ContentLength = payload.Length;

        request.GetRequestStream().Write(payload, 0, payload.Length);

        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }

        }

        // Reload the encounters
        lbClientEncounters.Items.Clear();
        tbEncounter.Text = "";
        ddlEncounterTypes.SelectedIndex = -1;
        XmlNodeList xnlClientEncounters = xmlClient.SelectNodes("//Person/Encounters/Encounter");
        foreach (XmlNode xnEncounter in xnlClientEncounters)
        {
            string EncounterType = xnEncounter.SelectSingleNode("./@type")?.InnerXml;
            string EncounterDescription = xnEncounter.InnerXml;
            lbClientEncounters.Items.Add(new ListItem(EncounterType + ": " + EncounterDescription));
        }

    }


    //Google Maps

    private void FillInFieldRepMarkers()
    {

        // For Google Maps, the format for adding markers on a map is
        // [ [latitude, longitude], [latitude, longitude], [latitude, longitude] ...]
        // This routine will cycle through the listbox, grab the latitude and the longitude,
        //  and systematically create the format required
        MySelectedFieldRepMarkerPosition = "[]";
        MyFieldRepMarkerPositions = "";
        foreach (ListItem li in lbFieldReps.Items)
        {
            string[] value = li.Value.Split('|');
            if (li.Selected == true)
            {
                MySelectedFieldRepMarkerPosition = "[" + value[2] + "," + value[3] + "]";
            }
            else
            {
                MyFieldRepMarkerPositions += ", [" + value[2] + "," + value[3] + "]";
            }
        }
        if (MyFieldRepMarkerPositions.Length > 0)
        {
            MyFieldRepMarkerPositions = MyFieldRepMarkerPositions.Substring(1); // Get rid of leading comma
        }
        MyFieldRepMarkerPositions = "[" + MyFieldRepMarkerPositions + "]";

        ConnectSelectedClientAndFieldRep();
    }


    private void FillInClientMarkers()
    {
        // For Google Maps, the format for adding markers on a map is
        // [ [latitude, longitude], [latitude, longitude], [latitude, longitude] ...]
        // This routine will cycle through the listbox, grab the latitude and the longitude,
        //  and systematically create the format required

        MySelectedClientMarkerPosition = "[]";
        MyClientMarkerPositions = "";
        foreach (ListItem li in lbClients.Items)
        {
            string[] value = li.Value.Split('|');
            if (li.Selected == true)
            {
                MySelectedClientMarkerPosition = "[" + value[2] + "," + value[3] + "]";
            }
            else
            {
                MyClientMarkerPositions += ", [" + value[2] + "," + value[3] + "]";
            }
        }
        if (MyClientMarkerPositions.Length > 0)
        {
            MyClientMarkerPositions = MyClientMarkerPositions.Substring(1); // Get rid of leading comma
        }
        MyClientMarkerPositions = "[" + MyClientMarkerPositions + "]";

        ConnectSelectedClientAndFieldRep();
    }

    private void ConnectSelectedClientAndFieldRep()
    {
        // For Google Maps, the format for creating a line between 2 points is
        // [ {lat: FirstLatitude, lng: FirstLongitude}, {lat: SecondLatitude, lng: SecondLongitude}]
        // This routine will cycle through the listboxes, grab the latitude and the longitude of the selected items,
        //  and systematically create the format required

        if (lbFieldReps.SelectedIndex > -1 && lbClients.SelectedIndex > -1)
        {

            string[] value = lbFieldReps.SelectedValue.Split('|');
            DistancePathCoordinatesOfSelected = "[";
            DistancePathCoordinatesOfSelected += "{lat: " + value[2] + ", lng: " + value[3] + " }";

            DistancePathCoordinatesOfSelected += ",";

            value = lbClients.SelectedValue.Split('|');
            DistancePathCoordinatesOfSelected += "{lat: " + value[2] + ", lng: " + value[3] + " }";
            DistancePathCoordinatesOfSelected += "]";
        }
        else
        {
            DistancePathCoordinatesOfSelected = "[]";
        }
    }

    private void GetGeoPoints(string address, ref decimal latitude, ref decimal longitude)
    {
        MapService.MapServiceSoapClient mapService = new MapService.MapServiceSoapClient();
        
        XmlDocument pXML = new XmlDocument();

        XElement latlong = mapService.AddressAutoCompleteAndPlaceDetailsCombined(address);
        string GetLatitudeLongitudeXML = latlong.ToString();

        pXML.LoadXml(GetLatitudeLongitudeXML);
        XmlNode xn = pXML.SelectSingleNode("//geometry/location/lat");
        latitude = Convert.ToDecimal(xn.InnerXml);

        xn = pXML.SelectSingleNode("//geometry/location/lng");
        longitude = Convert.ToDecimal(xn.InnerXml);      

    }


    public static HttpWebRequest CreateWebRequest(string sSite)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sSite);
        webRequest.Headers.Add(@"SOAP:Action");
        webRequest.ContentType = "text/xml; charset=utf-8";
        webRequest.Accept = "text/xml";
        webRequest.Method = "GET";
        return webRequest;
    }


    protected string MyClientMarkerPositions
    { //JavaScript Connection
        set { tbClientMarkers.Text = value; }
        get { return tbClientMarkers.Text; }
    }

    protected string MyFieldRepMarkerPositions
    { //JavaScript Connection
        set { tbFieldRepMarkers.Text = value; }
        get { return tbFieldRepMarkers.Text; }
    }

    protected string MySelectedClientMarkerPosition
    { //JavaScript Connection
        set { tbSelectedClientMarker.Text = value; }
        get { return tbSelectedClientMarker.Text; }
    }

    protected string MySelectedFieldRepMarkerPosition
    { //JavaScript Connection
        set { tbSelectedFieldRepMarker.Text = value; }
        get { return tbSelectedFieldRepMarker.Text; }
    }

    protected string DistancePathCoordinatesOfSelected
    { //JavaScript Connection
        set { tbDistancePathCoordinatesOfSelected.Text = value; }
        get { return tbDistancePathCoordinatesOfSelected.Text; }
    }

}
