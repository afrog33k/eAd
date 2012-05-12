using System.ServiceModel;

namespace eAd.DataAccess
{
public interface IServiceCallback
{
    [OperationContract(IsOneWay = true)]
    void SendMessage(string message);
}
}