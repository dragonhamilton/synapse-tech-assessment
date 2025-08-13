using Newtonsoft.Json.Linq;
using Serilog;
using System.Text.RegularExpressions;

namespace Synapse.DMEOrders
{
    public class OrderExtractorOxygenTank : OrderExtractorBase
    {
        private readonly ILogger _logger;
    private readonly string PROP_LITERS = "liters";
    private readonly string PROP_USAGE = "usage";
    private readonly string PROP_ORDERING_PHYSICIAN = "ordering physician";
        public OrderExtractorOxygenTank(ILogger logger)
        {
            DEVICE_NAME = "Oxygen Tank";
            DEVICE_CODE = "oxygen";

            _logger = logger;
        }

        public override JObject Extract(Dictionary<string, string> noteValues)
        {
            _logger.Information($"Extracting {DEVICE_NAME} order info");

            Diagnosis = noteValues.ContainsKey(PROP_DIAGNOSIS) ? noteValues[PROP_DIAGNOSIS] : UNKNOWN;
            OrderingPhysician = noteValues.ContainsKey(PROP_ORDERING_PHYSICIAN) ? noteValues[PROP_ORDERING_PHYSICIAN] : UNKNOWN;
            PatientName = noteValues.ContainsKey(PROP_PATIENT_NAME) ? noteValues[PROP_PATIENT_NAME] : UNKNOWN;
            DOB = noteValues.ContainsKey(PROP_DOB) ? noteValues[PROP_DOB] : UNKNOWN;
            Prescription = noteValues.ContainsKey(PROP_PRESCRIPTION) ? noteValues[PROP_PRESCRIPTION] : UNKNOWN;

            string liters = UNKNOWN;

            Match litersMatch = Regex.Match(Prescription, "(\\d+(\\.\\d+)?) ?L", RegexOptions.IgnoreCase);
            if (litersMatch.Success) liters = litersMatch.Groups[1].Value + " L";

            string usage = noteValues.ContainsKey(PROP_USAGE) ? noteValues[PROP_USAGE] : UNKNOWN;

            var order = new JObject
            {
                [PROP_DEVICE] = DEVICE_NAME,
                [PROP_LITERS] = liters,
                [PROP_USAGE] = usage,
                [PROP_DIAGNOSIS] = Diagnosis,
                [PROP_ORDERING_PHYSICIAN] = OrderingPhysician,
                [PROP_PATIENT_NAME] = PatientName,
                [PROP_DOB] = DOB,
            };
            return order;
        }
    }
}
