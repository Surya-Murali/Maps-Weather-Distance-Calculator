using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

public partial class Examples_MapServicesConsumer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnAutoComplete_Click(object sender, EventArgs e)
    {
        MapService.MapServiceSoapClient mapService = new MapService.MapServiceSoapClient();
        XElement xeAutoComplete = mapService.AddressAutoComplete(tbAddress.Text);

        string xml = xeAutoComplete.ToString();

        string xmlFormatted = XDocument.Parse(xml).ToString();
        tbAutoCompleteResults.Text = xmlFormatted;


    }
}