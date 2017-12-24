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

namespace IS7024Labs.Labs
{
    public partial class Lab4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGetPeople_Click(object sender, EventArgs e)
        {
            string url = @"http://localhost:50178/People/xml";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string soapResult = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                string StatusDescription = ((HttpWebResponse)response).StatusDescription;
                int StatusCode = (int)((HttpWebResponse)response).StatusCode;
                lblStatus.Text = "Status: " + StatusCode.ToString() + " - " + StatusDescription;

            }
            XmlDocument xmlPeople = new XmlDocument();
            xmlPeople.LoadXml(soapResult);
            XmlNodeList xnlPeople = xmlPeople.SelectNodes(@"//Person");
            foreach(XmlNode person in xnlPeople)
            {
                string firstName = person.SelectSingleNode(@"Name/First")?.InnerText;
                string lastName = person.SelectSingleNode(@"Name/Last")?.InnerText;
                string suffixName = person.SelectSingleNode(@"Name/Last/@suffix")?.InnerText;
                string id = person.SelectSingleNode(@"@id")?.InnerText;
                string fullname = lastName + " " + suffixName + ", " + firstName;
                lbPeople.Items.Add(new ListItem(fullname, id));
            }


            // Cycle through the answer and put into the listbox
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //Get the ID from the selected listbox item
            string idOfPerson = lbPeople.SelectedValue;

            string url = @"http://localhost:50178/People/xml";
            url += @"/" + idOfPerson;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string soapResult = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                string StatusDescription = ((HttpWebResponse)response).StatusDescription;
                int StatusCode = (int)((HttpWebResponse)response).StatusCode;
                lblStatus.Text = "Status: " + StatusCode.ToString() + " - " + StatusDescription;

            }
            string xmlFormatted = XDocument.Parse(soapResult).ToString();
            tbEditPerson.Text = xmlFormatted;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Pull the edited person into an XMLDocument
            XmlDocument xmlEditedPerson = new XmlDocument();
            string editedPerson = tbEditPerson.Text;
            xmlEditedPerson.LoadXml(editedPerson);

            //Get the ID from the Person XML in the textbox
            string idOfPerson = xmlEditedPerson.SelectSingleNode(@"Person/@id")?.InnerText;

            string url = @"http://localhost:50178/People/xml";
            url += @"/" + idOfPerson;
            byte[] payload = Encoding.UTF8.GetBytes(editedPerson);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/xml; charset=utf-8";
            request.Method = "Put";
            request.ContentLength = payload.Length;

            request.GetRequestStream().Write(payload, 0, payload.Length);

            string soapResult = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }

                string StatusDescription = ((HttpWebResponse)response).StatusDescription;
                int StatusCode = (int)((HttpWebResponse)response).StatusCode;
                lblStatus.Text = "Status: " + StatusCode.ToString() + " - " + StatusDescription;
            }

            lbPeople.Items.Clear();
            tbEditPerson.Text = "";

        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            // The Lab is to put in the code to create a new person
            // Hint:  This will be similar to a PUT, but you will use a POST
        }
    }
}