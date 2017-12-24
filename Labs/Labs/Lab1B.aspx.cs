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
    public partial class Lab1B : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var PlaceID = Request.Params.Get("PlaceID");

            if (PlaceID == "")
            {
                PlaceID = "EjIxMDAgSm9lIE51eGhhbGwgV2F5LCBDaW5jaW5uYXRpLCBPSCwgVW5pdGVkIFN0YXRlcw";
            }

            Execute(PlaceID);
        }
        public void Execute(string Place)
        {
            HttpWebRequest request = CreateWebRequest(@"https://maps.googleapis.com/maps/api/place/details/xml?key=AIzaSyDLjdKk5e_fgW4jCiIGjM6BCZfEuQaQAew&placeid=" + Place);

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