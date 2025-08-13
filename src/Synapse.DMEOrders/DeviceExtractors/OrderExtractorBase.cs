using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Synapse.DMEOrders
{
    public abstract class OrderExtractorBase
    {
        public abstract JObject Extract(Dictionary<string, string> noteValues);
        protected const string UNKNOWN = "Unknown";
        protected string DEVICE_NAME = UNKNOWN;
        protected string DEVICE_CODE = UNKNOWN;
        protected const string PROP_DEVICE = "device";
        protected const string PROP_DIAGNOSIS = "diagnosis";
        protected const string PROP_PRESCRIPTION = "prescription";
        protected const string PROP_ORDERING_PROVIDER = "ordering physician";
        protected const string PROP_PATIENT_NAME = "patient name";
        protected const string PROP_DOB = "dob";

        protected string _noteBody = String.Empty;

        protected string Diagnosis = UNKNOWN;
        protected string Prescription = UNKNOWN;
        protected string OrderingPhysician = UNKNOWN;
        protected string PatientName = UNKNOWN;
        protected string DOB = UNKNOWN;

        public bool CanHandle(string noteBody)
        {
            bool canHandle = noteBody.Contains(DEVICE_CODE, System.StringComparison.OrdinalIgnoreCase);
            if (canHandle)
            {
                _noteBody = noteBody;
            }
            return canHandle;
        }

        protected Dictionary<string, string> ParseNoteToDictionary(string noteBody)
        {
            var dict = new Dictionary<string, string>();
            var lines = noteBody.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().ToLower().Replace(" ", "_");
                    var value = parts[1].Trim();
                    dict[key] = value;
                }
            }
            return dict;
        }
    }
}
