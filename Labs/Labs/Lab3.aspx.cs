using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace IS7024Labs.Labs
{
    public partial class Lab3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void bntInvokeA_Click(object sender, EventArgs e)
        {
            //Take the latitude/longitude values in the text boxes (tbLab3ALatitude1, tbLab3ALongitude1, tbLab3ALatitude2, tbLab3ALongitude2)
            //and call the "DistanceBetweenTwoPoints"
            //Show the results in the Distance textbox (tbLab3ADistance)

            string lat1 = tbLab3ALatitude1.Text;
            string lng1 = tbLab3ALongitude1.Text;
            string lat2 = tbLab3ALatitude2.Text;
            string lng2 = tbLab3ALongitude2.Text;

            double dist = DistanceBetweenTwoPoints(double.Parse(lat1), double.Parse(lng1), double.Parse(lat2), double.Parse(lng2));

            tbLab3ADistance.Text = dist.ToString();
        }

        protected void btnInvokeB_Click(object sender, EventArgs e)
        {
            //Take each address in the textboxes (tbLab3BAddress1. tbLab3BAddress2)
            //  1) Call GetPlaceID
            //  2) Convert the string to XML (See Lab2 for a Hint -- but use LoadXML)
            //  3) Using XPath, locate the first PlaceID (See Lab2 for a Hint)
            //  4) Call GetLatitudeLongitude
            //  5) Convert the string to XML
            //  6) Using XPath, locate the first Latitude and Longitute
            //Call the DistanceBetweenTwoPoints with the 2 sets of Latitude and Lognitude
            //Show the results in the Distance textbox

            string address1 = tbLab3BAddress1.Text;
            string address2 = tbLab3BAddress2.Text;

            string Place1XML = GetPlaceID(address1);
            string Place2XML = GetPlaceID(address2);

            XmlDocument p1XML = new XmlDocument();
            p1XML.LoadXml(Place1XML);
            XmlNode xn1 = p1XML.SelectSingleNode("//place_id");
            string sPlaceID1 = xn1.InnerXml;

            XmlDocument p2XML = new XmlDocument();
            p2XML.LoadXml(Place2XML);
            XmlNode xn2 = p2XML.SelectSingleNode("//place_id");
            string sPlaceID2 = xn2.InnerXml;


            string GetLatitudeLongitude1XML = GetLatitudeLongitude(sPlaceID1);
            string GetLatitudeLongitude2XML = GetLatitudeLongitude(sPlaceID2);

            p1XML.LoadXml(GetLatitudeLongitude1XML);
            xn1 = p1XML.SelectSingleNode("//geometry/location/lat");
            string lat1 = xn1.InnerXml;

            xn1 = p1XML.SelectSingleNode("//geometry/location/lng");
            string lng1 = xn1.InnerXml;


            p2XML.LoadXml(GetLatitudeLongitude2XML);
            xn2 = p2XML.SelectSingleNode("//geometry/location/lat");
            string lat2 = xn2.InnerXml;

            xn2 = p2XML.SelectSingleNode("//geometry/location/lng");
            string lng2 = xn2.InnerXml;


            double dist = DistanceBetweenTwoPoints(double.Parse(lat1), double.Parse(lng1), double.Parse(lat2), double.Parse(lng2));

            tbLab3BDistance.Text = dist.ToString();


        }

        public double DistanceBetweenTwoPoints(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double returnDistanceInMiles = 0;
            double theta = longitude2 - longitude1;
            double thetaRadianDegrees = ConvertToRadian(theta);
            double latitude1RadianDegrees = ConvertToRadian(latitude1);
            double latitude2RadianDegrees = ConvertToRadian(latitude2);

            returnDistanceInMiles = (Math.Sin(latitude1RadianDegrees) * Math.Sin(latitude2RadianDegrees))
                + (Math.Cos(latitude1RadianDegrees) * Math.Cos(latitude2RadianDegrees) * Math.Cos(thetaRadianDegrees));

            returnDistanceInMiles = Math.Acos(returnDistanceInMiles);
            returnDistanceInMiles = ConvertToDegree(returnDistanceInMiles);
            returnDistanceInMiles = returnDistanceInMiles * 60 * 1.1515;  // If small distances
            return returnDistanceInMiles;
        }

        private double ConvertToRadian(double value)
        {
            double dblReturn;
            dblReturn = Math.PI * value / 180;
            return dblReturn;
        }
        private double ConvertToDegree(double value)
        {
            double dblReturn;
            dblReturn = value * 180 / Math.PI;
            return dblReturn;
        }

         public string GetPlaceID(string Address)
        {
            HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/autocomplete/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&input=" + Address);
            string AutoComplete="";
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
            string PlaceDetails="";
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

    }
}