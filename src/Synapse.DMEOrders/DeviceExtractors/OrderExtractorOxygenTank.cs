using Newtonsoft.Json.Linq;
using Serilog;
using System.Text.RegularExpressions;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Extractor for Oxygen Tank device orders.
    /// Parses liters per minute and usage from the prescription and note values.
    /// </summary>
    public class OrderExtractorOxygenTank : OrderExtractorBase
    {
        private readonly ILogger _logger;
        private const string PROP_LITERS = "liters";
        private const string PROP_USAGE = "usage";
        private const string LITERS_PATTERN = "(\\d+(\\.\\d+)?) ?L";
        private const string LITERS_SUFFIX = " L";
        public OrderExtractorOxygenTank(ILogger logger)
        {
            DEVICE_NAME = "Oxygen Tank";
            DEVICE_CODE = "oxygen";

            _logger = logger;
        }

        /// <summary>
        /// Builds the Oxygen Tank order JSON from parsed note values.
        /// </summary>
        /// <param name="noteValues">Parsed key/value pairs from the note.</param>
        /// <returns>Structured order JSON.</returns>
        public override JObject Extract(Dictionary<string, string> noteValues)
        {
            _logger.Information($"Extracting {DEVICE_NAME} order info");

            Diagnosis = noteValues.ContainsKey(PROP_DIAGNOSIS) ? noteValues[PROP_DIAGNOSIS] : UNKNOWN;
            OrderingPhysician = noteValues.ContainsKey(PROP_ORDERING_PROVIDER) ? noteValues[PROP_ORDERING_PROVIDER] : UNKNOWN;
            PatientName = noteValues.ContainsKey(PROP_PATIENT_NAME) ? noteValues[PROP_PATIENT_NAME] : UNKNOWN;
            DOB = noteValues.ContainsKey(PROP_DOB) ? noteValues[PROP_DOB] : UNKNOWN;
            Prescription = noteValues.ContainsKey(PROP_PRESCRIPTION) ? noteValues[PROP_PRESCRIPTION] : UNKNOWN;

            string liters = UNKNOWN;

            Match litersMatch = Regex.Match(Prescription, LITERS_PATTERN, RegexOptions.IgnoreCase);
            if (litersMatch.Success) liters = litersMatch.Groups[1].Value + LITERS_SUFFIX;

            string usage = noteValues.ContainsKey(PROP_USAGE) ? noteValues[PROP_USAGE] : UNKNOWN;

            var order = new JObject
            {
                [PROP_DEVICE] = DEVICE_NAME,
                [PROP_LITERS] = liters,
                [PROP_USAGE] = usage,
                [PROP_DIAGNOSIS] = Diagnosis,
                [PROP_ORDERING_PROVIDER] = OrderingPhysician,
                [PROP_PATIENT_NAME] = PatientName,
                [PROP_DOB] = DOB,
            };
            return order;
        }
    }
}
