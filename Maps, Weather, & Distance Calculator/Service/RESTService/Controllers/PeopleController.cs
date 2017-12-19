using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RESTService.Controllers
{
    public class PeopleController : ApiController
    {
        [HttpGet]
        [Route("{personType}/{format}/{id}")]
        public HttpResponseMessage GetPerson(string personType, string id, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;
            string role = GetPersonRole(personType);

            XmlDocument xmlPeople = GetPeopleDocument();
            string xPath = @"//Person[@id=" + id + " and Role='" + role + "']";
            XmlNode xnPerson = xmlPeople.SelectSingleNode(xPath);
            string strPerson = "";
            HttpStatusCode statusCode;
            if (xnPerson != null)
            {
                strPerson = xnPerson.OuterXml;
                statusCode = HttpStatusCode.OK;
            }
            else
            {
                strPerson = "";
                statusCode = HttpStatusCode.NotFound;
            }
            res = CreateResponse(format, strPerson);
            res.StatusCode = statusCode;

            return res;
        }

        [HttpGet]
        [Route("{personType}/{format}")]
        public HttpResponseMessage GetAllPeople(string personType, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;
            string role = GetPersonRole(personType);

            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            string xPath = @"//Person[Role='" + role + "']";
            XmlNodeList xnlPeople = xmlPeople.SelectNodes(xPath);

            XmlElement xe = xmlPeople.CreateElement("People"); //Need a root node
            foreach(XmlNode xnPerson in xnlPeople)
            {
                xe.InnerXml += xnPerson.OuterXml;
            }

            string strPeople = xe.OuterXml;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            res = CreateResponse(format, strPeople); //Common Routine to create an HttpResponseMessage
            res.StatusCode = statusCode;

            return res;
        }

        [HttpPost]
        [Route("{personType}/{format}")]
        public HttpResponseMessage AddPersonDetails(string personType, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;
            string role = GetPersonRole(personType);

            string strNewPerson = Request.Content.ReadAsStringAsync().Result; //The Person Node is in the body of the request
            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            XmlElement xe = xmlPeople.CreateElement("PeopleToInsert"); //Need a root node
            XmlDocumentFragment frgPersonNode = xmlPeople.CreateDocumentFragment(); //One technique to INSERT a node into an XML Document
            frgPersonNode.InnerXml = strNewPerson;

            //Assign the ID of this new person by getting the maximum one in the people file and adding 1
            XmlNodeList xnlIDs = xmlPeople.SelectNodes(@"//@id");
            Int32 maxID = Int32.MinValue;
            foreach (XmlNode xnID in xnlIDs)
            {
                if (Convert.ToInt32(xnID.InnerXml) > maxID)
                {
                    maxID = Convert.ToInt32(xnID.InnerXml);
                }
            }
            Int32 nextID = maxID + 1;
            XmlElement xePerson = (XmlElement)frgPersonNode.SelectSingleNode(".");
            xePerson.SetAttribute("id", nextID.ToString());

            xmlPeople.DocumentElement.AppendChild(frgPersonNode);
            string statusMessage = string.Empty;
            HttpStatusCode statusCode;
            bool bValidationStatus = MakeSureXMLIsStillValid(xmlPeople, ref statusMessage); //Common Routine to run the XSD against the new XML to make sure it is valid, and make sure there are unique IDs

            //Save the XML to the file
            if (bValidationStatus == true)
            {
                SavePeopleDocument(xmlPeople); //Common Routine to save the People.xml document
                statusMessage = "Creation Successful";
                statusCode = HttpStatusCode.Created;
            }
            else
            {
                statusCode = HttpStatusCode.BadRequest;
            }

            res = CreateResponse(format, statusMessage); //Common Routine to create an HttpResponseMessage
            res.StatusCode = statusCode;

            return res;
        }

        [HttpPut]
        [Route("{personType}/{format}/{id}")]
        public HttpResponseMessage UpdatePersonDetails(string personType, string Id, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;
            string role = GetPersonRole(personType);

            string strUpdatePerson = Request.Content.ReadAsStringAsync().Result; //The Person Node is in the body of the request
            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            XmlElement xe = xmlPeople.CreateElement("PeopleToUpdate"); //Need a root node
            XmlDocumentFragment xReplacement = xmlPeople.CreateDocumentFragment(); //One technique to INSERT a node into an XML Document
            xReplacement.InnerXml = strUpdatePerson;
            string statusMessage = string.Empty;
            HttpStatusCode statusCode;

            //Make sure the ID in the replacement person matches the ID in the URL
            string xPathToGetReplacementID = @"//Person/@id";
            XmlNode xnReplacementID = xReplacement.SelectSingleNode(xPathToGetReplacementID);
            if (xnReplacementID != null)
            {
                string strReplacementID = xnReplacementID.InnerText;
                if (strReplacementID == Id) // So far so good
                {
                    //Make sure there is a Person to replace
                    string xPathToFindPersonWithThisId = @"//Person[@id=" + Id + "]";
                    XmlNode xnPersonToBeReplaced = xmlPeople.SelectSingleNode(xPathToFindPersonWithThisId);
                    if (xnPersonToBeReplaced != null)
                    {
                        xnPersonToBeReplaced.ParentNode.ReplaceChild(xReplacement, xnPersonToBeReplaced);
                        bool bValidationStatus = MakeSureXMLIsStillValid(xmlPeople, ref statusMessage); //Common Routine to run the XSD against the new XML to make sure it is valid, and make sure there are unique IDs
                        if (bValidationStatus == true)
                        {
                            SavePeopleDocument(xmlPeople); //Common Routine to save the People.xml document
                            statusMessage = "Update Successful";
                            statusCode = HttpStatusCode.OK;
                        }
                        else
                        {
                            statusCode = HttpStatusCode.BadRequest;
                        }
                    }
                    else
                    {
                        statusMessage = "Update Unsuccessul -- Nothing to Update";
                        statusCode = HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    statusMessage = "Update Unsuccessul -- IDs do not match";
                    statusCode = HttpStatusCode.BadRequest;
                }
            }
            else
            {
                statusMessage = "Update Unsuccessul -- ID missing";
                statusCode = HttpStatusCode.BadRequest;
            }

            res = CreateResponse(format, statusMessage); //Common Routine to create an HttpResponseMessage
            res.StatusCode = statusCode;

            return res;
        }

        [HttpDelete]
        [Route("{personType}/{format}/{id}")]
        public HttpResponseMessage DeletePersonDetails(string personType, string Id, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;
            string role = GetPersonRole(personType);

            string strDeletePerson = Request.Content.ReadAsStringAsync().Result; //Usually there is nothing in the body for a delete
            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            string statusMessage = string.Empty;
            HttpStatusCode statusCode;

            //Make sure there is a Person to delete
            string xPathToFindPersonWithThisId = @"//Person[@id=" + Id + "]";
            XmlNode xnPersonToBeDeleted = xmlPeople.SelectSingleNode(xPathToFindPersonWithThisId);
            if (xnPersonToBeDeleted != null)
            {
                xnPersonToBeDeleted.ParentNode.RemoveChild(xnPersonToBeDeleted);
                bool bValidationStatus = MakeSureXMLIsStillValid(xmlPeople, ref statusMessage); //Common Routine to run the XSD against the new XML to make sure it is valid, and make sure there are unique IDs

                //Save the XML to the file
                if (bValidationStatus == true)
                {
                    SavePeopleDocument(xmlPeople); //Common Routine to save the People.xml document
                    statusMessage = "Delete Successful";
                    statusCode = HttpStatusCode.OK;
                }
                else
                {
                    statusCode = HttpStatusCode.BadRequest;
                }
            }
            else
            {
                statusMessage = "Delete Unsuccessul -- Nothing to Delete";
                statusCode = HttpStatusCode.BadRequest;
            }

            res = CreateResponse(format, statusMessage); //Common Routine to create an HttpResponseMessage
            res.StatusCode = statusCode;

            return res;
        }


        private HttpResponseMessage CreateResponse(string format, string message)
        {
            HttpResponseMessage hrmResponse = new HttpResponseMessage(); ;

            if (format == "xml")
            {
                // XML Section
                string returnxml = @"" + message;
                hrmResponse.Content = new StringContent(returnxml, System.Text.Encoding.UTF8, string.Format("text/{0}", format));
            }
            else
            {
                // JSON Section
                // To convert an XML node contained in string xml into a JSON string   
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(message);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                string jsonFormatted = JValue.Parse(jsonText).ToString(Newtonsoft.Json.Formatting.Indented);
                string returnjson = @"" + jsonFormatted;
                hrmResponse.Content = new StringContent(returnjson, System.Text.Encoding.UTF8, string.Format("application/{0}", format));
            }

            return hrmResponse;
        }

        private string GetPathOfXMLFolder()
        {
            string XmlFilesPath = string.Empty;

            string currentPath = System.Web.Hosting.HostingEnvironment.MapPath(@"\");
            XmlFilesPath = currentPath + @"XMLFiles\";

            return XmlFilesPath;
        }

        private void SavePeopleDocument(XmlDocument xmlPeople)
        {
            string xmlFilesPath = GetPathOfXMLFolder();
            string peopleFileToSave = xmlFilesPath + "People" + ".xml";

            xmlPeople.Save(peopleFileToSave);

        }

        private XmlDocument GetPeopleDocument()
        {
            XmlDocument xmlPeople = null;

            string xmlFilesPath = GetPathOfXMLFolder();
            string peopleFileToReturn = xmlFilesPath + "People" + ".xml";
            xmlPeople = new XmlDocument();
            xmlPeople.Load(peopleFileToReturn);
            if (xmlPeople.FirstChild.NodeType == XmlNodeType.XmlDeclaration) // Pitch the <?xml version...
            {
                xmlPeople.RemoveChild(xmlPeople.FirstChild);
            }

            return xmlPeople;
        }
        private string GetSchemaFilePath()
        {
            string peopleXSDFileToReturn = string.Empty;

            string xmlFilesPath = GetPathOfXMLFolder();
            peopleXSDFileToReturn = xmlFilesPath + "People" + ".xsd";

            return peopleXSDFileToReturn;
        }


        private bool MakeSureXMLIsStillValid(XmlDocument xmlToValidate, ref string statusMessage)
        {
            //Common Routine to run the XSD against the new XML to make sure it is valid, and make sure there are unique IDs
            bool bReturn = false;

            // Make sure the XML still conforms to the XSD
            string peopleSchemaFileToUse = GetSchemaFilePath();
            try
            {
                XmlSchemaSet sSet = new XmlSchemaSet();
                sSet.Add("", peopleSchemaFileToUse);
                xmlToValidate.Schemas = sSet;
                xmlToValidate.Validate(null);
                bReturn = true;
            }
            catch (XmlSchemaValidationException ex)
            {
                statusMessage = "Validation Unsuccessful";
            }

            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            if (bReturn == true)
            {
                // Make sure that there are no duplicate ids
                string xPathToGetCountDistinctIDs = @"count(//Person/@id[not(.=../following-sibling::Person/@id)])";
                string xPathToGetCountAllIDs = @"count(//Person/@id)";
                string xPathToSeeIfDupes = xPathToGetCountDistinctIDs + "!=" + xPathToGetCountAllIDs;
                XPathNavigator xpn = xmlToValidate.CreateNavigator();
                string DupesFound = xpn.Evaluate(xPathToSeeIfDupes).ToString();
                if (DupesFound.ToUpper() == "TRUE")
                {
                    statusMessage = "Duplicate Detected";
                    bReturn = false;
                }
            }

            return bReturn;
        }

        private string GetPersonRole(string personType)
        {
            string role = "INVALID ROLE";
            if (personType == "FieldReps")
            {
                role = "Field Rep";
            }
            else if (personType == "Clients")
            {
                role = "Client";
            }
            return role;
        }
    }
}
