using System.Net.Http;
using System.Text;
using Serilog;
using System;

namespace Synapse.DMEOrders
{
    public class OrderSender
    {
        private readonly ILogger _logger;
        private const string ENDPOINT_URL = "https://alert-api.com/DrExtract";
        private const string CONTENT_TYPE = "application/json";
        private const string LOG_SENDING = "Sending order to API at {Url}: {Json}";
        private const string LOG_SUCCESS = "Order sent successfully with status {StatusCode}";
        private const string LOG_NON_SUCCESS = "API returned non-success status {StatusCode}: {ReasonPhrase}. Body: {Body}";
        private const string LOG_HTTP_ERROR = "HTTP error while sending order: {Message}";
        private const string LOG_TIMEOUT = "Timed out sending order to API";
        private const string LOG_UNEXPECTED = "Unexpected error while sending order: {Message}";
        private const string LOG_SKIPPING = "Skipping API call to API at {Url}: {Json}(SEND_ORDERS not enabled)";
        private const int HTTP_TIMEOUT_SECONDS = 10;
        private const string ENV_SEND_ORDERS = "SEND_ORDERS";

        public OrderSender(ILogger logger)
        {
            _logger = logger;
        }

        // Backward-compatible overload: ignore parameters and use current env/const-based config
        public OrderSender(ILogger logger, string apiUrl, int timeoutSeconds, bool sendOrders)
        {
            _logger = logger;
            // This overload is retained for compatibility with previous signatures.
            // Current implementation uses ENDPOINT_URL constant, HTTP_TIMEOUT_SECONDS, and SEND_ORDERS env flag.
            // You can wire these values in if/when the app fully moves to file-based config.
        }

        public void Send(string json)
        {
            var url = ENDPOINT_URL;

            var sendFlag = Environment.GetEnvironmentVariable(ENV_SEND_ORDERS);
            bool shouldSend = string.Equals(sendFlag, "1", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(sendFlag, "true", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(sendFlag, "yes", StringComparison.OrdinalIgnoreCase);
            if (!shouldSend)
            {
                _logger.Information(LOG_SKIPPING, url, json);
                return;
            }
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECONDS) })
            {
                var content = new StringContent(json, Encoding.UTF8, CONTENT_TYPE);
                _logger.Information(LOG_SENDING, url, json);
                try
                {
                    using var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information(LOG_SUCCESS, (int)response.StatusCode);
                    }
                    else
                    {
                        string reason = response.ReasonPhrase ?? string.Empty;
                        string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        _logger.Error(LOG_NON_SUCCESS, (int)response.StatusCode, reason, body);
                    }
                }
                catch (TaskCanceledException ex)
                {
                    _logger.Error(ex, LOG_TIMEOUT);
                    throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.Error(ex, LOG_HTTP_ERROR, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, LOG_UNEXPECTED, ex.Message);
                    throw;
                }
            }
        }
    }
}