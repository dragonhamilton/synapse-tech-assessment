using System;
using System.IO;
using Serilog;

namespace Synapse.DMEOrders
{
    public class FileReader
    {
        private readonly ILogger _logger;

        public FileReader(ILogger logger)
        {
            _logger = logger;
        }

        public string ReadFile(string fileName)
        {
            _logger.Information("Reading file: {FileName}", fileName);
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                throw new FileNotFoundException($"File {fileName} not found.");
            }
        }
    }
}