using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;

public partial class Examples_PeopleServicesConsumer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnReloadClients_Click(object sender, EventArgs e)
    {
        //Place the code to call the PeopleService to GET all of the Clients
        //Place the information into the listbox similar to that in Lab 4

        tbDistance.Text = "";
        tbClientAddress.Text = "";
        lbClientEncounters.Items.Clear();
        tbEncounter.Text = "";
        tbClientTemperature.Text = "";
        tbClientPrecipitation.Text = "";


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
        XmlDocument xmlClients = new XmlDocument();
        xmlClients.LoadXml(restResult);
        XmlNodeList xnlPeople = xmlClients.SelectNodes(@"//Person");
        lbClients.Items.Clear();
        foreach (XmlNode person in xnlPeople)
        {
            string firstName = person.SelectSingleNode(@"Name/First")?.InnerText;
            string lastName = person.SelectSingleNode(@"Name/Last")?.InnerText;
            string suffixName = person.SelectSingleNode(@"Name/Last/@suffix")?.InnerText;
            string id = person.SelectSingleNode(@"@id")?.InnerText;
            string fullname = lastName + " " + suffixName + ", " + firstName;
            lbClients.Items.Add(new ListItem(fullname, id));
        }

    }

    protected void btnReloadFieldReps_Click(object sender, EventArgs e)
    {
        //Place the code to call the PeopleService to GET all of the Field Reps
        //Place the information into the listbox similar to that in Lab 4
        tbDistance.Text = "";
        tbFieldRepAddress.Text = "";
        tbFieldRepsTemperature.Text = "";
        tbFieldRepsHumidity.Text = "";
        string url = @"http://localhost:52236/FieldReps/xml";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string restResult = "";
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                restResult = rd.ReadToEnd();
            }

        }
        XmlDocument xmlFieldReps = new XmlDocument();
        xmlFieldReps.LoadXml(restResult);
        XmlNodeList xnlPeople = xmlFieldReps.SelectNodes(@"//Person");
        lbFieldReps.Items.Clear();
        foreach (XmlNode person in xnlPeople)
        {
            string firstName = person.SelectSingleNode(@"Name/First")?.InnerText;
            string lastName = person.SelectSingleNode(@"Name/Last")?.InnerText;
            string suffixName = person.SelectSingleNode(@"Name/Last/@suffix")?.InnerText;
            string id = person.SelectSingleNode(@"@id")?.InnerText;
            string fullname = lastName + " " + suffixName + ", " + firstName;
            lbFieldReps.Items.Add(new ListItem(fullname, id));
        }
    }

    protected void lbClients_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Place the code to call the PeopleService to GET the specific Client selected
        //      Use XPath to find the City in the first WorkLocation
        //      Use XPath to find the Address in the first Work Location
        //      Use XPath to find the Encounters
        //          Add each encounter into the listbox
        //          Clear out the encounter textbox
        //Place the code to call the Weather Service to obtain the temperature and precipitation based on the city of the client
        //      You will need to use XPath to locate the information in the weather information returned
        //If a Field Rep is also selected, call the DistanceService to obtain the distance between them using the first WorkLocation in each

        string id = lbClients.SelectedItem.Value;
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

    }

    protected void lbFieldReps_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Place the code to call the PeopleService to GET the specific Field Rep selected
        //      Use XPath to find the City in the first WorkLocation
        //      Use XPath to find the Address in the first Work Location
        //Place the code to call the Weather Service to obtain the temperature and humidity based on the city of the Field Rep
        //      You will need to use XPath to locate the information in the weather information returned
        //If a Client is also selected, call the DistanceService to obtain the distance between them using the first WorkLocation in each

        string id = lbFieldReps.SelectedItem.Value;
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
    }

    protected void btnAddEncounter_Click(object sender, EventArgs e)
    {
        //Place the code to call the PeopleService to GET the specific Client selected
        //Insert a new encounter using the encounter type from the drop down and the information in the textbox
        //Place the code to call the PeopleService to PUT the new client record.  This will be similar to that in Lab 4


        string id = lbClients.SelectedItem.Value;
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
        XmlNode ndNewEncounter =  xmlClient.CreateNode("element", "Encounter", "");
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



}