using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    public class OrderExtractorWheelchair : OrderExtractorBase
    {
        private readonly ILogger _logger;

        public OrderExtractorWheelchair(ILogger logger)
        {
            DEVICE_NAME = "Wheelchair";
            DEVICE_CODE = "wheelchair";

            _logger = logger;
        }


        public override JObject Extract(Dictionary<string, string> noteValues)
        {
            Diagnosis = noteValues.ContainsKey(PROP_DIAGNOSIS) ? noteValues[PROP_DIAGNOSIS] : "Unknown";
            OrderingPhysician = noteValues.ContainsKey(PROP_ORDERING_PROVIDER) ? noteValues[PROP_ORDERING_PROVIDER] : "Unknown";
            PatientName = noteValues.ContainsKey(PROP_PATIENT_NAME) ? noteValues[PROP_PATIENT_NAME] : "Unknown";
            DOB = noteValues.ContainsKey(PROP_DOB) ? noteValues[PROP_DOB] : "Unknown";

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
