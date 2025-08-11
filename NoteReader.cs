using System;
using System.IO;
using Serilog;

namespace Synapse.DMEOrders
{
    public class NoteReader
    {
        private readonly ILogger _logger;

        public NoteReader(ILogger logger)
        {
            _logger = logger;
        }

        public string Read(string fileName = "physician_note.txt")
        {
            if (File.Exists(fileName))
            {
                _logger.Information("Reading file: {FileName}", fileName);
                return File.ReadAllText(fileName);
            }
            throw new FileNotFoundException("Physician note file not found.");
        }
    }
}