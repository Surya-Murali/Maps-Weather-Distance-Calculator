using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Distance" in code, svc and config file together.
public class DistanceService : IDistanceService
{
    public void DoWork()
    {
    }

    public double DistanceBetweenTwoPoints(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        double returnDistanceInMiles = 0;
        double theta = longitude2 - longitude1;
        double thetaRadianDegrees = ConvertToRadian(theta);
        double latitude1RadianDegrees = ConvertToRadian(latitude1);
        double latitude2RadianDegrees = ConvertToRadian(latitude2);

        returnDistanceInMiles = (Math.Sin(latitude1RadianDegrees) * Math.Sin(latitude2RadianDegrees))
            + (Math.Cos(latitude1RadianDegrees) * Math.Cos(latitude2RadianDegrees) * Math.Cos(thetaRadianDegrees));

        returnDistanceInMiles = Math.Acos(returnDistanceInMiles);
        returnDistanceInMiles = ConvertToDegree(returnDistanceInMiles);
        returnDistanceInMiles = returnDistanceInMiles * 60 * 1.1515;  // If small distances
        return returnDistanceInMiles;
    }


    private double ConvertToRadian(double value)
    {
        double dblReturn;
        dblReturn = Math.PI * value / 180;
        return dblReturn;
    }
    private double ConvertToDegree(double value)
    {
        double dblReturn;
        dblReturn = value * 180 / Math.PI;
        return dblReturn;
    }
}
