using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Services;
using System.Web.Services.Protocols;

/// <summary>
/// Summary description for WeatherService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WeatherService : System.Web.Services.WebService
{

    public WeatherService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public XmlDocument GetWeather(string city)
    {

        string xmlString = @"<?xml version=""1.0"" encoding=""UTF - 8""?><Weather></Weather>";
        XmlDocument xmlCity = new XmlDocument();
        XmlDocument xmlAllCities = new XmlDocument();
        try
        {
            string currentPath = Server.MapPath(".");
            string weatherFilesPath = currentPath + @"\WeatherFiles\";
            string weatherFileToReturn = weatherFilesPath + "all.xml";


            xmlAllCities.Load(weatherFileToReturn);
            if(city=="All")
            {
                // Return the entire file
                xmlString = xmlAllCities.InnerXml;
            }
            else
            {
                // Return just the node associated with the city
                XmlNode currentCity = xmlAllCities.SelectSingleNode("//city[@name='" + city + "']/..");
                xmlString = currentCity.OuterXml;
            }
            xmlCity.LoadXml(xmlString);
        }
        catch (Exception ex)
        {
            SoapException se = new SoapException("Fault occurred: " + ex.Message,
                SoapException.ClientFaultCode,
                Context.Request.Url.AbsoluteUri
                );
            throw se;
        }

        Context.Response.ContentType = "application/xml";
        return xmlCity;
    }

}
