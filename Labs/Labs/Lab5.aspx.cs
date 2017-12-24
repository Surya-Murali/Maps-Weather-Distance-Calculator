using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Xsl;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;




namespace IS7024Labs.Labs
{
    public partial class Lab5 : System.Web.UI.Page
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


        protected void lbClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInClientMarkers();
        }

        protected void lbFieldReps_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInFieldRepMarkers();
        }

        protected void btnClientLoad_Click(object sender, EventArgs e)
        {
            string url = @"http://localhost:50178/People/xml";
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
            string url = @"http://localhost:50178/People/json";
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
            string PlaceXML = GetPlaceID(address);
            XmlDocument pXML = new XmlDocument();
            pXML.LoadXml(PlaceXML);
            XmlNode xn = pXML.SelectSingleNode("//place_id");
            string sPlaceID = xn.InnerXml;

            string GetLatitudeLongitudeXML = GetLatitudeLongitude(sPlaceID);

            pXML.LoadXml(GetLatitudeLongitudeXML);
            xn = pXML.SelectSingleNode("//geometry/location/lat");
            latitude = Convert.ToDecimal(xn.InnerXml);

            xn = pXML.SelectSingleNode("//geometry/location/lng");
            longitude = Convert.ToDecimal(xn.InnerXml);
        }

        public string GetPlaceID(string Address)
        {
            HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/autocomplete/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&input=" + Address);
            string AutoComplete = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    AutoComplete = rd.ReadToEnd();
                }
            }
            return AutoComplete;
        }

        public string GetLatitudeLongitude(string PlaceID)
        {
            HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/details/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&placeid=" + PlaceID);
            string PlaceDetails = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    PlaceDetails = rd.ReadToEnd();
                }
            }
            return PlaceDetails;
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
}