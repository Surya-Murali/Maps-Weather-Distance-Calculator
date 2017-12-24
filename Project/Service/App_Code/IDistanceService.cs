using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDistance" in both code and config file together.
[ServiceContract]
public interface IDistanceService
{
    [OperationContract]
    void DoWork();

    [OperationContract]
    double DistanceBetweenTwoPoints(double latitiude1, double longitude1, double latitude2, double longitude2);


}
