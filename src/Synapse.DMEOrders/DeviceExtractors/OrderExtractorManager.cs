using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Orchestrates device-specific extractors to produce a structured order from a note body.
    /// Initializes known extractors and delegates to the first that can handle the note.
    /// </summary>
    public class OrderExtractorManager
    {
        private readonly List<OrderExtractorBase> _extractors;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="OrderExtractorManager"/>, initializing supported extractors.
        /// </summary>
        /// <param name="logger">The logger instance to pass to extractors.</param>
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

        /// <summary>
        /// Extracts a structured JSON order from the given note body using the first matching extractor.
        /// </summary>
        /// <param name="noteBody">The raw physician note body (JSON or plain text).</param>
        /// <returns>Structured JSON order payload.</returns>
        /// <exception cref="Exception">Thrown when parsing yields no data or when no extractor can handle the note.</exception>
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
