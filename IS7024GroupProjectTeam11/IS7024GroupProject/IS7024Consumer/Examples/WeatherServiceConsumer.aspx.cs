using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void btnGetCityList_Click(object sender, EventArgs e)
    {
        WeatherService.WeatherServiceSoapClient weather = new WeatherService.WeatherServiceSoapClient();
        string city = "All";
        XElement xeWeather = weather.GetWeather(city);

        XmlDocument xmlWeather = new XmlDocument();
        string xml = xeWeather.ToString();
        xmlWeather.LoadXml(xml);

        XmlNodeList xnl = xmlWeather.SelectNodes(@"//city/@name");

        lbCity.Items.Clear();
        foreach (XmlNode node in xnl)
        {
            lbCity.Items.Add(new ListItem(node.Value));
        }
    }

    protected void lbCity_SelectedIndexChanged(object sender, EventArgs e)
    {
        tbSunRise.Text = "";
        string city = lbCity.SelectedValue;
        WeatherService.WeatherServiceSoapClient weather = new WeatherService.WeatherServiceSoapClient();
        try
        {
            XElement xeWeather = weather.GetWeather(city);
            if (xeWeather != null)
            {
                string xml = xeWeather.ToString();

                XmlDocument xmlWeather = new XmlDocument();
                xmlWeather.LoadXml(xml);

                XmlNode xn = xmlWeather.SelectSingleNode(@"/current/city/sun/@rise");
                DateTime dt = Convert.ToDateTime(xn.Value);
                string timePortion = dt.ToShortTimeString();
                tbSunRise.Text = timePortion;
            }
        }
        catch (Exception ex)
        { }
    }



    protected void btnLoadPeople_Click(object sender, EventArgs e)
    {
        XmlDocument xmlPeople = new XmlDocument();
        string peopleFilePath = MapPath(".") + @"\..\XMLFiles\People.xml";
        xmlPeople.Load(peopleFilePath);

        XmlNodeList xnlPersons = xmlPeople.SelectNodes("//Name");
        foreach (XmlNode Name in xnlPersons)
        {
            string firstName = Name.SelectSingleNode("First").InnerXml;
            string lastName = Name.SelectSingleNode("Last").InnerXml;
            string city = Name.SelectSingleNode("../WorkLocation[1]/City").InnerXml;
            string id = Name.SelectSingleNode("../@id").InnerXml;
            lbPeople.Items.Add(new ListItem(firstName + " " + lastName, id + "|" + city));
        }

    }

    protected void lbPeople_SelectedIndexChanged(object sender, EventArgs e)
    {
        string ListBoxValue = lbPeople.SelectedItem.Value;
        string[] value = ListBoxValue.Split('|'); // value[0] has the id, value[1] has the city
        tbPersonCity.Text = value[1];

        WeatherService.WeatherServiceSoapClient weather = new WeatherService.WeatherServiceSoapClient();
        try
        {
            XElement xeWeather = weather.GetWeather(value[1]);
            if (xeWeather != null)
            {
                string xml = xeWeather.ToString();

                XmlDocument xmlWeather = new XmlDocument();
                xmlWeather.LoadXml(xml);

                XmlNode xn = xmlWeather.SelectSingleNode(@"/current/temperature/@value");
                double tempKelvin = Convert.ToDouble(xn.Value);
                double tempCelcius = Math.Round(tempKelvin - 273.15, 2);
                double tempFahrenheit = Math.Round(tempCelcius * 1.8 + 32, 2);

                string temp = tempCelcius.ToString() + "C" + "   " + tempFahrenheit.ToString() + "F";
                tbCityTemperature.Text = temp;
            }
        }
        catch (Exception ex)
        { }


    }
}