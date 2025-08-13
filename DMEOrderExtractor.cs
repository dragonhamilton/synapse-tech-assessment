using System;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Extracts order info from physician note and sends it to the API.
    /// </summary>
    class DMEOrderExtractor
    {
        static int Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("error.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Starting DME Order Extraction");
            try
            {
                Log.Information("Reading note body from file");
                FileReader fileReader = new FileReader(Log.Logger);
                string noteBody = fileReader.ReadFile("physician_note2.txt");
                Log.Information(noteBody);

                List<OrderExtractorBase> extractors = new List<OrderExtractorBase>
                {
                    new OrderExtractorCPAP(Log.Logger),
                    new OrderExtractorOxygenTank(Log.Logger),
                    new OrderExtractorWheelchair(Log.Logger)
                };

                Log.Information("Extracting device-specific order from note body");
                OrderExtractorManager extractManager = new OrderExtractorManager(extractors, Log.Logger);
                JObject orderInfo = extractManager.Extract(noteBody);

                Log.Information("Sending order to API");
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