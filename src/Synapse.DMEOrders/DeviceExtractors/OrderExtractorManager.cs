using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    public class OrderExtractorManager
    {
        private readonly List<OrderExtractorBase> _extractors;
        private readonly ILogger _logger;

        public OrderExtractorManager(ILogger logger)
        {
            _logger = logger;

            _extractors = new List<OrderExtractorBase>
            {
                new OrderExtractorCPAP(_logger),
                new OrderExtractorOxygenTank(_logger),
                new OrderExtractorWheelchair(_logger)
            };
        }

        public JObject Extract(string noteBody)
        {

            foreach (var extractor in _extractors)
            {
                if (extractor.CanHandle(noteBody))
                {
                    _logger.Information($"Using extractor: {extractor.GetType().Name}");

                    // Try Json first
                    NoteParser noteParser = new NoteParser(_logger);
                    Dictionary<string, string> noteValues = noteParser.ParseNoteToDictionary(noteBody);
                    if (noteValues.Count == 0)
                    {
                        throw new Exception("No valid data found in note body.");
                    }

                    return extractor.Extract(noteValues);
                }
            }
            throw new System.InvalidOperationException("No suitable extractor found for the given note body.");
        }
    }
}
