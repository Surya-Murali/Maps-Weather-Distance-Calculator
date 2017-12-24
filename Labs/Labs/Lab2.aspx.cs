using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;

namespace IS7024Labs.Labs
{
    public partial class Lab2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btlLoadPeople_Click(object sender, EventArgs e)
        {
            try
            {
                string currentPath = Server.MapPath(".");
                string peopleFilesPath = currentPath + @"\XMLFiles\";
                string peopleFileToReturn = peopleFilesPath + "People" + ".xml";

                XmlDocument xmlPeople = new XmlDocument();
                xmlPeople.Load(peopleFileToReturn);

                XmlNodeList xnlFieldReps = xmlPeople.SelectNodes(@"//Person[Role='Field Rep']");

                lbFieldReps.Items.Clear();
                foreach (XmlNode node in xnlFieldReps)
                {
                    XmlNode lastName = node.SelectSingleNode("Name/Last");
                    XmlNode suffix = lastName.SelectSingleNode("@suffix");
                    string fullName = lastName?.InnerText + " " + suffix?.InnerText;

                    // Add more information to the name

                    lbFieldReps.Items.Add(new ListItem(fullName));
                }

                // Get Client Information


            }
            catch (Exception ex)
            { }


        }


        protected void btnXSD_Click(object sender, EventArgs e)
        {
            string currentPath = Server.MapPath(".");
            string peopleFilesPath = currentPath + @"\XMLFiles\";
            string peopleFileToUse = peopleFilesPath + "People" + ".xml";
            string peopleSchemaFileToUse = peopleFilesPath + tbXSD.Text.Trim() + ".xsd";

            XmlDocument xmlPeople = new XmlDocument();
            try
            {
                xmlPeople.Load(peopleFileToUse);

                XmlSchemaSet sSet = new XmlSchemaSet();
                sSet.Add("", peopleSchemaFileToUse);
                xmlPeople.Schemas = sSet;
                xmlPeople.Validate(null);
                tbXSDResults.Text = "Validation Completed Successfully";
            }
            catch (XmlSchemaValidationException ex)
            {
                tbXSDResults.Text = "Validation Unsuccessful";
                tbXSDResults.Text += "\n\n" + ex.Message;
            }

            catch (Exception ex)
            {
                tbXSDResults.Text = ex.Message;
            }

        }
    }
}