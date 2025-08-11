using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Synapse.DMEOrders
{
    public class OrderInfoExtractor
    {
        private readonly ILogger _logger;

        public OrderInfoExtractor(ILogger logger)
        {
            _logger = logger;
        }
        
        public JObject Extract(string noteBody)
        {
            _logger.Information("Getting device info from note body");
            var device = GetDevice(noteBody);
            if (device == "Unknown")
            {
                throw new InvalidOperationException("Device type could not be determined from the note.");
            }
            var maskType = device == "CPAP" && noteBody.Contains("full face", StringComparison.OrdinalIgnoreCase) ? "full face" : null;
            var addOns = noteBody.Contains("humidifier", StringComparison.OrdinalIgnoreCase) ? new JArray("humidifier") : null;
            var qualifier = noteBody.Contains("AHI > 20") ? "AHI > 20" : "";

            _logger.Information("Extracting ordering physician from note body");
            var provider = ExtractOrderingPhysician(noteBody);

            _logger.Information("Creating device info JSON");
            var order = new JObject
            {
                ["device"] = device,
                ["mask_type"] = maskType,
                ["add_ons"] = addOns,
                ["qualifier"] = qualifier,
                ["ordering_provider"] = provider
            };

            if (device == "Oxygen Tank")
            {
                order["liters"] = ExtractLiters(noteBody);
                order["usage"] = ExtractUsage(noteBody);
            }

            return order;
        }


        /// <summary>
        /// Gets the a signled device type from the note
        /// Returns "Unknown" if no device is found
        /// Finds only one device type, ignoring others
        /// </summary>
        private string GetDevice(string note)
        {
            if (note.Contains("CPAP", StringComparison.OrdinalIgnoreCase))
            {
                return "CPAP";
            }
            if (note.Contains("oxygen", StringComparison.OrdinalIgnoreCase))
            {
                return "Oxygen Tank";
            }
            if (note.Contains("wheelchair", StringComparison.OrdinalIgnoreCase))
            {
                return "Wheelchair";
            }
            return "Unknown";
        }

        private string ExtractOrderingPhysician(string note)
        {
            var idx = note.IndexOf("Dr.");
            if (idx >= 0)
            {
                return note.Substring(idx).Replace("Ordered by ", "").Trim('.').Trim();
            }
            return "Unknown";
        }

        private string ExtractLiters(string note)
        {
            Match match = Regex.Match(note, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value + " L";
            return null;
        }

        private string ExtractUsage(string note)
        {
            bool sleep = note.Contains("sleep", StringComparison.OrdinalIgnoreCase);
            bool exertion = note.Contains("exertion", StringComparison.OrdinalIgnoreCase);
            if (sleep && exertion) return "sleep and exertion";
            if (sleep) return "sleep";
            if (exertion) return "exertion";
            return null;
        }
    }
}