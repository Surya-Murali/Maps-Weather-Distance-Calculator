using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Summary description for MapService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class MapService : System.Web.Services.WebService
{

    public MapService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public XmlDocument AddressAutoComplete(string Address)
    {
        XmlDocument xmlAddresses = new XmlDocument();

        HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/autocomplete/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&input=" + Address);

        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                string soapResult = rd.ReadToEnd();
                xmlAddresses.LoadXml(soapResult);
            }
        }
        return xmlAddresses;
    }

    [WebMethod]
    public XmlDocument PlaceDetails(string PlaceID)
    {
        XmlDocument xmlPlaceDetails = new XmlDocument();

        HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/details/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&placeid=" + PlaceID);
        using (WebResponse response = request.GetResponse())
        {
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                string soapResult = rd.ReadToEnd();
                xmlPlaceDetails.LoadXml(soapResult);
            }
        }
        return xmlPlaceDetails;
    }

    [WebMethod]
    public XmlDocument AddressAutoCompleteAndPlaceDetailsCombined(string Address)
    {
        XmlDocument xmlAddresses = new XmlDocument();
        //Add functionality here
        //string PlaceXML = GetPlaceID(address);
        xmlAddresses = AddressAutoComplete(Address);
        
        XmlNode xn = xmlAddresses.SelectSingleNode("//place_id");
        string sPlaceID = xn.InnerXml;

        //string GetLatitudeLongitudeXML = GetLatitudeLongitude(sPlaceID);
        XmlDocument latlong = PlaceDetails(sPlaceID);
        return latlong;
    }

    /// <summary>
    /// Create a soap webrequest to [Url]
    /// </summary>
    /// <returns></returns>
    public HttpWebRequest CreateWebRequest(string sSite)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sSite);
        webRequest.Headers.Add(@"SOAP:Action");
        webRequest.ContentType = "text/xml; charset=utf-8";
        webRequest.Accept = "text/xml";
        webRequest.Method = "GET";
        return webRequest;
    }




}
