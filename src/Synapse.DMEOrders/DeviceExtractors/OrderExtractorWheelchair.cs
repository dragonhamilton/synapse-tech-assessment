using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Extractor for Wheelchair device orders.
    /// Produces a minimal order payload with standard patient and diagnosis fields.
    /// </summary>
    public class OrderExtractorWheelchair : OrderExtractorBase
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new Wheelchair order extractor.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public OrderExtractorWheelchair(ILogger logger)
        {
            DEVICE_NAME = "Wheelchair";
            DEVICE_CODE = "wheelchair";

            _logger = logger;
        }

        /// <summary>
        /// Builds the Wheelchair order JSON from parsed note values.
        /// </summary>
        /// <param name="noteValues">Parsed key/value pairs from the note.</param>
        /// <returns>Structured order JSON.</returns>
        public override JObject Extract(Dictionary<string, string> noteValues)
        {
            Diagnosis = noteValues.ContainsKey(PROP_DIAGNOSIS) ? noteValues[PROP_DIAGNOSIS] : UNKNOWN;
            OrderingPhysician = noteValues.ContainsKey(PROP_ORDERING_PROVIDER) ? noteValues[PROP_ORDERING_PROVIDER] : UNKNOWN;
            PatientName = noteValues.ContainsKey(PROP_PATIENT_NAME) ? noteValues[PROP_PATIENT_NAME] : UNKNOWN;
            DOB = noteValues.ContainsKey(PROP_DOB) ? noteValues[PROP_DOB] : UNKNOWN;

            _logger.Information($"Extracting {DEVICE_NAME} order info");
            return new JObject
            {
                [PROP_DEVICE] = DEVICE_NAME,
                [PROP_DIAGNOSIS] = Diagnosis,
                [PROP_ORDERING_PROVIDER] = OrderingPhysician,
                [PROP_PATIENT_NAME] = PatientName,
                [PROP_DOB] = DOB
            };
        }
    }
}
