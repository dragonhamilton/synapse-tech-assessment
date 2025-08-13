using System;
using Newtonsoft.Json.Linq;
using Serilog;
using System.IO;
using System.Text.Json;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Extracts order info from physician note and sends it to the API.
    /// </summary>
    class DMEOrderExtractor
    {
        private const string LOG_START = "Starting DME Order Extraction";
        private const string LOG_READING = "Reading note body from file";
        private const string LOG_EXTRACTING = "Extracting device-specific order from note body";
        private const string LOG_SENDING = "Sending order to API";
        private const string ENV_NOTE_FILE = "DME_NOTE_FILE";
        private const string DEFAULT_NOTE_FILE = "physician_note1.txt";
        private const string LOG_FILE_NAME = "error.log";
        static int Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(LOG_FILE_NAME, rollingInterval: RollingInterval.Day)
            .CreateLogger();

            Log.Information(LOG_START);
            try
            {
                Log.Information(LOG_READING);
                FileReader fileReader = new FileReader(Log.Logger);
                // Load from appsettings.json; fallback to DEFAULT_NOTE_FILE if missing
                string notePath = DEFAULT_NOTE_FILE;
                var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if (File.Exists(settingsPath))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(File.ReadAllText(settingsPath));
                        if (doc.RootElement.TryGetProperty("NoteFilePath", out var prop))
                        {
                            var fromFile = prop.GetString();
                            if (!string.IsNullOrWhiteSpace(fromFile)) notePath = fromFile!;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to read appsettings.json; using default note path");
                    }
                }
                Log.Information($"Using note file path: {notePath}");
                string noteBody = fileReader.ReadFile(notePath);
                Log.Information(noteBody);

                List<OrderExtractorBase> extractors = new List<OrderExtractorBase>
                {
                    new OrderExtractorCPAP(Log.Logger),
                    new OrderExtractorOxygenTank(Log.Logger),
                    new OrderExtractorWheelchair(Log.Logger)
                };

                Log.Information(LOG_EXTRACTING);
                OrderExtractorManager extractManager = new OrderExtractorManager(extractors, Log.Logger);
                JObject orderInfo = extractManager.Extract(noteBody);

                Log.Information(LOG_SENDING);
                OrderSender sender = new OrderSender(Log.Logger);
                sender.Send(orderInfo.ToString());
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error processing physician note: {ex.Message}";
                Log.Error(ex, errorMessage);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return 0;
        }
    }
}