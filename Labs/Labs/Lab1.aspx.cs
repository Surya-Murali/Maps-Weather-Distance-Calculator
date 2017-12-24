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
    public partial class Lab1 : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tbPlaceID.Text = "";
                tbPlaceText.Text = "";
            }
        }

        protected void btnGooglePlaceAutocomplete_Click(object sender, EventArgs e)
        {
            var PlaceText = tbPlaceText.Text;
            var url = @"../Labs/Lab1A.aspx" + "?PlaceText=" + PlaceText;
            Server.Transfer(url, true);
        }

        protected void btnGooglePlaceDetails_Click(object sender, EventArgs e)
        {
            var PlaceID = tbPlaceID.Text;
            var url = @"../Labs/Lab1B.aspx" + "?PlaceID=" + PlaceID;
            Server.Transfer(url, true);

        }
    }
}