using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Extractor for CPAP device orders.
    /// Determines mask type and add-ons from diagnosis and recommendation text.
    /// </summary>
    public class OrderExtractorCPAP : OrderExtractorBase
    {
        private const string FULL_FACE_MASK = "Full face mask";
        private const string HEATED_HUMIDIFIER = "heated humidifier";
        private const string PROP_ADD_ONS = "add_ons";
        private const string PROP_MASK_TYPE = "mask_type";
        private const string PROP_RECOMMENDATION = "recommendation";
        private const string DEFAULT_MASK = "standard";
        private const string DEFAULT_ADDONS = "none";
        private const string SENTENCE_JOIN = ". ";
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new CPAP order extractor.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public OrderExtractorCPAP(ILogger logger)
        {
            DEVICE_NAME = "CPAP";
            DEVICE_CODE = "CPAP";

            _logger = logger;
        }

        /// <summary>
        /// Builds the CPAP order JSON from parsed note values.
        /// </summary>
        /// <param name="noteValues">Parsed key/value pairs from the note.</param>
        /// <returns>Structured order JSON.</returns>
        public override JObject Extract(Dictionary<string, string> noteValues)
        {
            _logger.Information($"Extracting {DEVICE_NAME} order info");

            Diagnosis = noteValues.ContainsKey(PROP_DIAGNOSIS) ? noteValues[PROP_DIAGNOSIS] : UNKNOWN; ;
            OrderingPhysician = noteValues.ContainsKey(PROP_ORDERING_PROVIDER) ? noteValues[PROP_ORDERING_PROVIDER] : UNKNOWN;
            PatientName = noteValues.ContainsKey(PROP_PATIENT_NAME) ? noteValues[PROP_PATIENT_NAME] : UNKNOWN;
            DOB = noteValues.ContainsKey(PROP_DOB) ? noteValues[PROP_DOB] : UNKNOWN;
            string recommendation = noteValues.ContainsKey(PROP_RECOMMENDATION) ? noteValues[PROP_RECOMMENDATION] : string.Empty;
            string combined = Diagnosis + SENTENCE_JOIN + recommendation;

            string maskType = DEFAULT_MASK;
            if (combined.Contains(FULL_FACE_MASK, StringComparison.InvariantCultureIgnoreCase))
            {
                maskType = FULL_FACE_MASK;
            }

            string addOns = DEFAULT_ADDONS;
            if (combined.Contains(HEATED_HUMIDIFIER, StringComparison.InvariantCultureIgnoreCase))
            {
                addOns = HEATED_HUMIDIFIER;
            }

            return new JObject
            {
                [PROP_DEVICE] = DEVICE_NAME,
                [PROP_DIAGNOSIS] = Diagnosis,
                [PROP_ADD_ONS] = addOns,
                [PROP_ORDERING_PROVIDER] = OrderingPhysician,
                [PROP_PATIENT_NAME] = PatientName,
                [PROP_DOB] = DOB,
                [PROP_MASK_TYPE] = maskType
            };
        }
    }
}
