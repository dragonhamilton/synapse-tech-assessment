using System;
using System.Collections.Generic;
using Serilog;
using System.Text.Json;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Parses a physician note body into key/value pairs.
    /// Tries JSON (expects a top-level "data" field) first, then falls back to plain text lines.
    /// </summary>
    public class NoteParser
    {
        private readonly ILogger _logger;

        private const string JSON_DATA_FIELD = "data";

    /// <summary>
    /// Creates a new <see cref="NoteParser"/>.
    /// </summary>
    /// <param name="logger">The Serilog logger.</param>
    public NoteParser(ILogger logger)
        {
            _logger = logger;
        }
    /// <summary>
    /// Parses the provided note body into a dictionary of key/value pairs.
    /// Attempts JSON parsing first; if that fails, falls back to plain text parsing.
    /// </summary>
    /// <param name="noteBody">The physician note body (JSON or plain text).</param>
    /// <returns>Dictionary of parsed key/value pairs (case-insensitive keys). Empty if no data found.</returns>
    public Dictionary<string, string> ParseNoteToDictionary(string noteBody)
        {
            string[] lines = Array.Empty<string>();
            try
            {
                lines = ParseJsonNote(noteBody);
            }
            catch (JsonException ex)
            {
                _logger.Information($"Note body is not valid JSON: {ex.Message}. Falling back to plain text parsing.");
            }
            // If empty, try plain text
            if (lines.Length == 0)
            {
                // Plain text parsing is not expected to throw JsonException
                lines = ParsePlainTextNote(noteBody);
            }
            // If still empty, log and return empty dictionary
            if (lines.Length == 0)
            {
                _logger.Information("No valid data found in note body after attempting both JSON and plain text parsing.");
                return new Dictionary<string, string>();
            }

            return ParseLinesToDictionary(lines);
        }

        /// <summary>
        /// Parses the lines of a plain text physician note body into key/value pairs and puts them in a dictionary.
        /// </summary>
        /// <param name="noteBody">The plain text physician note body</param>
        /// <returns>Dictionary of key/value pairs</returns>
        private string[] ParsePlainTextNote(string noteBody)
        {
            _logger.Information("Parsing plain text note body.");
            string[] lines = noteBody.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }

        /// <summary>
        /// Parses a JSON physician note body: splits the 'data' field into lines.
        /// </summary>
        /// <param name="noteBody">The JSON physician note body</param>
        /// <returns>Array of lines extracted from the data field.</returns>
        private string[] ParseJsonNote(string noteBody)
        {
            System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(noteBody);
            if (doc.RootElement.TryGetProperty(JSON_DATA_FIELD, out System.Text.Json.JsonElement dataElement))
            {
                string data = dataElement.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(data))
                {
                    throw new JsonException("The note is not formatted properly, the data field can't be parsed.");
                }
                else
                {
                    return data.Split('\n');
                }
            }
            else
            {
                throw new JsonException("The note is not formatted properly, it does not contain a 'data' field.");
            }
        }

        /// <summary>
        /// Converts lines in the format "key: value" to a case-insensitive dictionary.
        /// </summary>
        /// <param name="lines">Lines of text.</param>
        /// <returns>Dictionary of key/value pairs.</returns>
        private Dictionary<string, string> ParseLinesToDictionary(IEnumerable<string> lines)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                string[] parts = trimmed.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    dict[parts[0].Trim()] = parts[1].Trim();
                }
            }
            return dict;
        }
    }
}
