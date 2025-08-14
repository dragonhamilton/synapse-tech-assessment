using System.Net.Http;
using System.Text;
using Serilog;
using System;
using System.IO;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Sends the structured DME order JSON payload to the configured HTTP API.
    /// Reads configuration (ApiUrl, SendOrders) from appsettings.json, logs all activity,
    /// and applies basic timeout and error handling.
    /// </summary>
    public class OrderSender
    {
        private readonly ILogger _logger;
        private const string CONTENT_TYPE = "application/json";
        private const string LOG_SENDING = "Sending order to API at {Url}: {Json}";
        private const string LOG_SUCCESS = "Order sent successfully with status {StatusCode}";
        private const string LOG_NON_SUCCESS = "API returned non-success status {StatusCode}: {ReasonPhrase}. Body: {Body}";
        private const string LOG_HTTP_ERROR = "HTTP error while sending order: {Message}";
        private const string LOG_TIMEOUT = "Timed out sending order to API";
        private const string LOG_UNEXPECTED = "Unexpected error while sending order: {Message}";
        private const string LOG_SKIPPING = "Skipping API call to API at {Url}: {Json} (SendOrders disabled)";
        private const int HTTP_TIMEOUT_SECONDS = 10;

        /// <summary>
        /// Creates a new <see cref="OrderSender"/> with the provided logger.
        /// </summary>
        /// <param name="logger">The Serilog logger instance.</param>
        public OrderSender(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends the provided JSON payload to the configured API endpoint.
        /// Honors the SendOrders flag in appsettings.json. When disabled, logs and returns without sending.
        /// </summary>
        /// <param name="json">The JSON string to POST.</param>
        /// <exception cref="Exception">Thrown if configuration cannot be read or on HTTP failures/timeouts.</exception>
        public void Send(string json)
        {
            var url = "";
            bool shouldSend = false;
            try
            {
                var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if (File.Exists(settingsPath))
                {
                    var jsonText = File.ReadAllText(settingsPath);
                    using var doc = System.Text.Json.JsonDocument.Parse(jsonText);
                    if (doc.RootElement.TryGetProperty("ApiUrl", out var apiProp))
                    {
                        var configured = apiProp.GetString();
                        if (!string.IsNullOrWhiteSpace(configured)) url = configured!;
                    }
                    if (doc.RootElement.TryGetProperty("SendOrders", out var sendProp))
                    {
                        shouldSend = sendProp.GetBoolean();
                    }
                }
            }
            catch
            {
                throw new Exception("API Endpoint is not configured correctly.");
            }
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