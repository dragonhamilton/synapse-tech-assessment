using Newtonsoft.Json.Linq;

namespace Synapse.DMEOrders
{
    public interface IDeviceOrderExtractor
    {
        bool CanHandle(string noteBody);
    }
}