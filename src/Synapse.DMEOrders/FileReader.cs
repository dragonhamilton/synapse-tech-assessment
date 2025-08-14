using System;
using System.IO;
using Serilog;

namespace Synapse.DMEOrders
{
    /// <summary>
    /// Simple file reader with logging for diagnostics.
    /// </summary>
    public class FileReader
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="FileReader"/>.
        /// </summary>
        /// <param name="logger">The Serilog logger.</param>
        public FileReader(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Reads the entire contents of the specified file.
        /// </summary>
        /// <param name="fileName">Path to the file to read.</param>
        /// <returns>The file contents as a string.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
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