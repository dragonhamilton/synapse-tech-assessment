using System;
using Newtonsoft.Json.Linq;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Base class for device-specific order extractors.
    /// Provides common constants, state, and a capability check.
    /// </summary>
    public abstract class OrderExtractorBase
    {
        /// <summary>
        /// Extracts a structured JSON order from parsed note values.
        /// </summary>
        /// <param name="noteValues">Parsed key/value pairs from the physician note.</param>
        /// <returns>Structured JSON order payload.</returns>
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

        protected string Diagnosis = UNKNOWN;
        protected string Prescription = UNKNOWN;
        protected string OrderingPhysician = UNKNOWN;
        protected string PatientName = UNKNOWN;
        protected string DOB = UNKNOWN;

        /// <summary>
        /// Indicates whether this extractor can handle the note based on the device code.
        /// </summary>
        /// <param name="noteBody">The raw physician note body.</param>
        /// <returns>True if the extractor can handle the note, otherwise false.</returns>
        public bool CanHandle(string noteBody)
        {
            return noteBody.Contains(DEVICE_CODE, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
