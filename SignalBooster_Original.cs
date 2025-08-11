/*
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Synapse.DMEOrdersOld
{
    /// <summary>
    /// Extracts order info from physician note.
    /// </summary>
    class IgnoreMe
    {
        static int Main(string[] args)
        {
            try
            {
                string noteBody = ReadPhysicianNoteFromFile("physician_note.txt");

                var orderInfo = ExtractOrderInfoFromNoteBody(noteBody);
                var OrderJSON = orderInfo.ToString();

                SendOrderToApi(OrderJSON);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing physician note: {ex.Message}");
                return 1;
            }
            return 0;
        }

        static string ReadPhysicianNoteFromFile(string fileName = "physician_note.txt")
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                throw new FileNotFoundException("Physician note file not found.");
            }
        }

        static JObject ExtractOrderInfoFromNoteBody(string noteBody)
        {
            var device = ReadDeviceFromNote(noteBody);
            var maskType = device == "CPAP" && noteBody.Contains("full face", StringComparison.OrdinalIgnoreCase) ? "full face" : null;
            var addOns = noteBody.Contains("humidifier", StringComparison.OrdinalIgnoreCase) ? new JArray("humidifier") : null;
            var qualifier = noteBody.Contains("AHI > 20") ? "AHI > 20" : "";
            var provider = ExtractProvider(noteBody);

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

        static string ReadDeviceFromNote(string note)
        {
            if (note.Contains("CPAP", StringComparison.OrdinalIgnoreCase)) return "CPAP";
            if (note.Contains("oxygen", StringComparison.OrdinalIgnoreCase)) return "Oxygen Tank";
            if (note.Contains("wheelchair", StringComparison.OrdinalIgnoreCase)) return "Wheelchair";
            return "Unknown";
        }

        static string ExtractProvider(string note)
        {
            var idx = note.IndexOf("Dr.");
            if (idx >= 0)
            {
                return note.Substring(idx).Replace("Ordered by ", "").Trim('.').Trim();
            }
            return "Unknown";
        }

        static string ExtractLiters(string note)
        {
            Match match = Regex.Match(note, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value + " L";
            return null;
        }

        static string ExtractUsage(string note)
        {
            bool sleep = note.Contains("sleep", StringComparison.OrdinalIgnoreCase);
            bool exertion = note.Contains("exertion", StringComparison.OrdinalIgnoreCase);
            if (sleep && exertion) return "sleep and exertion";
            if (sleep) return "sleep";
            if (exertion) return "exertion";
            return null;
        }

        static void SendOrderToApi(string json)
        {
            using (var client = new HttpClient())
            {
                var url = "https://alert-api.com/DrExtract";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(url, content).GetAwaiter().GetResult();
            }
        }

        static int OldMain(string[] args)
        {
            /*
            Read from note
            // Begin the initialization of patient-order broadcast extraction synthesis
            string x;
            try
            {
                var p = "physician_note.txt";
                if (File.Exists(p))
                {
                    x = File.ReadAllText(p);
                }
                else
                {
                    x = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
                }
            }
            catch (Exception) { x = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron."; }
            */

            /* Redundant Code
            // redundant safety backup read - not used, but good to keep for future AI expansion
            try
            {
                var dp = "notes_alt.txt";
                if (File.Exists(dp)) { File.ReadAllText(dp); }
            }
            catch (Exception) { }
            

            /* Process the note 
            var d = "Unknown";
            if (x.Contains("CPAP", StringComparison.OrdinalIgnoreCase)) d = "CPAP";
            else if (x.Contains("oxygen", StringComparison.OrdinalIgnoreCase)) d = "Oxygen Tank";
            else if (x.Contains("wheelchair", StringComparison.OrdinalIgnoreCase)) d = "Wheelchair";

            string m = d == "CPAP" && x.Contains("full face", StringComparison.OrdinalIgnoreCase) ? "full face" : null;
            var a = x.Contains("humidifier", StringComparison.OrdinalIgnoreCase) ? "humidifier" : null;
            var q = x.Contains("AHI > 20") ? "AHI > 20" : "";

            var pr = "Unknown";
            int idx = x.IndexOf("Dr.");
            if (idx >= 0) pr = x.Substring(idx).Replace("Ordered by ", "").Trim('.');

            string l = null;
            var f = (string)null;
            if (d == "Oxygen Tank")
            {
                Match lm = Regex.Match(x, "(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
                if (lm.Success) l = lm.Groups[1].Value + " L";

                if (x.Contains("sleep", StringComparison.OrdinalIgnoreCase) && x.Contains("exertion", StringComparison.OrdinalIgnoreCase)) f = "sleep and exertion";
                else if (x.Contains("sleep", StringComparison.OrdinalIgnoreCase)) f = "sleep";
                else if (x.Contains("exertion", StringComparison.OrdinalIgnoreCase)) f = "exertion";
            }

            var r = new JObject
            {
                ["device"] = d,
                ["mask_type"] = m,
                ["add_ons"] = a != null ? new JArray(a) : null,
                ["qualifier"] = q,
                ["ordering_provider"] = pr
            };

            if (d == "Oxygen Tank")
            {
                r["liters"] = l;
                r["usage"] = f;
            }

            var sj = r.ToString();

            using (var h = new HttpClient())
            {
                var u = "https://alert-api.com/DrExtract";
                var c = new StringContent(sj, Encoding.UTF8, "application/json");
                var resp = h.PostAsync(u, c).GetAwaiter().GetResult();
            }

            return 0;
        }
    }
}
*/