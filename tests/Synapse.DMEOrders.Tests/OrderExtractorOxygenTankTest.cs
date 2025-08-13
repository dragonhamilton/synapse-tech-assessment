using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Serilog;
using Xunit;
using Moq;

namespace Synapse.DMEOrders.Tests.DeviceExtractors
{
    public class OrderExtractorOxygenTankTest
    {
        private readonly Mock<ILogger> _mockLogger;

        public OrderExtractorOxygenTankTest()
        {
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public void Extract_ReturnsCorrectJObject_WithAllFieldsPresent()
        {
            var extractor = new OrderExtractorOxygenTank(_mockLogger.Object);
            var noteValues = new Dictionary<string, string>
            {
                { "diagnosis", "COPD" },
                { "ordering_provider", "Dr. Smith" },
                { "patient_name", "John Doe" },
                { "dob", "01/01/1970" },
                { "prescription", "Use 2.5 L oxygen daily" },
                { "usage", "Continuous" }
            };

            JObject result = extractor.Extract(noteValues);

            Assert.Equal("Oxygen Tank", result["device"]);
            Assert.Equal("2.5 L", result["liters"]);
            Assert.Equal("Continuous", result["usage"]);
            Assert.Equal("COPD", result["diagnosis"]);
            Assert.Equal("Dr. Smith", result["ordering physician"]);
            Assert.Equal("John Doe", result["patient name"]);
            Assert.Equal("01/01/1970", result["dob"]);
        }

        [Fact]
        public void Extract_ReturnsUnknown_WhenFieldsMissing()
        {
            var extractor = new OrderExtractorOxygenTank(_mockLogger.Object);
            var noteValues = new Dictionary<string, string>();

            JObject result = extractor.Extract(noteValues);

            Assert.Equal("Oxygen Tank", result["device"]);
            Assert.Equal("Unknown", result["liters"]);
            Assert.Equal("Unknown", result["usage"]);
            Assert.Equal("Unknown", result["diagnosis"]);
            Assert.Equal("Unknown", result["ordering physician"]);
            Assert.Equal("Unknown", result["patient name"]);
            Assert.Equal("Unknown", result["dob"]);
        }

        [Theory]
        [InlineData("O2 at 3 L/min", "3 L")]
        [InlineData("Give 1.5L oxygen", "1.5 L")]
        [InlineData("Oxygen 10 L", "10 L")]
        [InlineData("No liters mentioned", "Unknown")]
        public void Extract_ParsesLiters_FromPrescription(string prescription, string expectedLiters)
        {
            var extractor = new OrderExtractorOxygenTank(_mockLogger.Object);
            var noteValues = new Dictionary<string, string>
            {
                { "prescription", prescription }
            };

            JObject result = extractor.Extract(noteValues);

            Assert.Equal(expectedLiters, result["liters"]);
        }
    }
}