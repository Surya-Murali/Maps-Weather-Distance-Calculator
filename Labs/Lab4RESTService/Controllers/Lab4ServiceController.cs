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

namespace Lab4RESTService.Controllers
{
    public class Lab4ServiceController : ApiController
    {
        [HttpGet]
        [Route("People/{format}/{id}")]
        public HttpResponseMessage Get(string id, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;

            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            string xPath = @"//Person[@id=" + id + "]"; //XPath to get the Person Node with this ID
            XmlNode xnPerson = xmlPeople.SelectSingleNode(xPath);
            string strPersonNode = string.Empty;
            HttpStatusCode statusCode;
            if (xnPerson != null)
            {
                strPersonNode = xnPerson.OuterXml;
                statusCode = HttpStatusCode.OK;
            }
            else
            {
                strPersonNode = "";
                statusCode = HttpStatusCode.NotFound;
            }
            res = CreateResponse(format, strPersonNode); //Common Routine to create an HttpResponseMessage
            res.StatusCode = statusCode;

            return res;
        }

        [HttpGet]
        [Route("People/{format}")]
        public HttpResponseMessage GetAllPeople([FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;

            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            string strPeople = xmlPeople.OuterXml; //Return the entire document
            HttpStatusCode statusCode = HttpStatusCode.OK;
            res = CreateResponse(format, strPeople); //Common Routine to create an HttpResponseMessage
            res.StatusCode = statusCode;

            return res;
        }

        [HttpPost]
        [Route("People/{format}")]
        public HttpResponseMessage AddPeopleDetails([FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;

            string strNewPerson = Request.Content.ReadAsStringAsync().Result; //The Person Node is in the body of the request
            XmlDocument xmlPeople = GetPeopleDocument(); //Common Routine to load up the People.xml document
            XmlElement xe = xmlPeople.CreateElement("PeopleToInsert"); //Need a root node
            XmlDocumentFragment frgPersonNode = xmlPeople.CreateDocumentFragment(); //One technique to INSERT a node into an XML Document
            frgPersonNode.InnerXml = strNewPerson;
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
        [Route("People/{format}/{id}")]
        public HttpResponseMessage UpdatePeopleDetails(string Id, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;

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
        [Route("People/{format}/{id}")]
        public HttpResponseMessage DeletePeopleDetails(string Id, [FromUri]string format = "xml")
        {
            HttpResponseMessage res = null;

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

        private HttpResponseMessage CreateResponse(string format, string message) //Common Routine to create an HttpResponseMessage
        {
            HttpResponseMessage hrmResponse = new HttpResponseMessage();

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

        private string GetPathOfXMLFolder() //Common Routine to fet the XMLFiles directory path
        {
            string XmlFilesPath = string.Empty;

            string currentPath = System.Web.Hosting.HostingEnvironment.MapPath(@"\");
            XmlFilesPath = currentPath + @"..\IS7024Labs\Labs\XMLFiles\";

            return XmlFilesPath;
        }

        private void SavePeopleDocument(XmlDocument xmlPeople) //Common Routine to save the People.xml document
        {
            string xmlFilesPath = GetPathOfXMLFolder();
            string peopleFileToSave = xmlFilesPath + "People" + ".xml";

            xmlPeople.Save(peopleFileToSave);
        }
        private XmlDocument GetPeopleDocument() //Common Routine to load up the People.xml document
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
        private string GetSchemaFilePath() //Common Routine to get the People2.xsd path
        {
            string peopleXSDFileToReturn = string.Empty;

            string xmlFilesPath = GetPathOfXMLFolder();
            peopleXSDFileToReturn = xmlFilesPath + "People2" + ".xsd";

            return peopleXSDFileToReturn;
        }


        private bool MakeSureXMLIsStillValid (XmlDocument xmlToValidate, ref string statusMessage)
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

    }
}
