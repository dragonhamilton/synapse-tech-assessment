using System.Net.Http;
using System.Text;
using Serilog;

namespace Synapse.DMEOrders
{
    public class OrderSender
    {
        private readonly ILogger _logger;

        public OrderSender(ILogger logger)
        {
            _logger = logger;
        }

        public void Send(string json)
        {
            using (var client = new HttpClient())
            {
                var url = "https://alert-api.com/DrExtract";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _logger.Information($"Sending order to API at {url}: {json}");
                // client.PostAsync(url, content).GetAwaiter().GetResult();
            }
        }
    }
}