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
                var noteReader = new NoteReader(Log.Logger);
                string noteBody = noteReader.Read("physician_note.txt");

                Log.Information("Extracting order from note body");
                var extractor = new OrderInfoExtractor(Log.Logger);
                JObject orderInfo = extractor.Extract(noteBody);

                Log.Information("Sending order to API");
                var sender = new OrderSender(Log.Logger);
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