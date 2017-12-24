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

namespace IS7024Labs
{
    public partial class Lab1A : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var PlaceText = Request.Params.Get("PlaceText");

            if (PlaceText.Trim() == "")
            {
                PlaceText = "100 Joe Nuxall Way";
            }

            Execute(PlaceText);
        }
        public void Execute(string Place)
        {
            HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/autocomplete/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&input=" + Place);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    Debug.WriteLine(soapResult);
                    Response.ContentType = "text/xml";
                    Response.Write(soapResult);
                }
            }
        }
        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
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