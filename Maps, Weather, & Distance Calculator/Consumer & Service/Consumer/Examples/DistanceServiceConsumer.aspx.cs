using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Examples_DistanceServiceConsumer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnInvoke_Click(object sender, EventArgs e)
    {
        DistanceService.DistanceServiceClient dc = new DistanceService.DistanceServiceClient();
        double dblDistance = dc.DistanceBetweenTwoPoints(double.Parse(tbLat1.Text), double.Parse(tbLng1.Text), double.Parse(tbLat2.Text), double.Parse(tbLng2.Text));
        tbDistance.Text = dblDistance.ToString();
    }
}