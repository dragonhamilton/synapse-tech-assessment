using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Serilog;
using Xunit;
using Moq;

namespace Synapse.DMEOrders.Tests.DeviceExtractors
{
    public class OrderExtractorCPAPTest
    {
        private readonly Mock<ILogger> _loggerMock;

        public OrderExtractorCPAPTest()
        {
            _loggerMock = new Mock<ILogger>();
        }

        [Fact]
        public void Extract_ShouldReturnExpectedJObject_WithFullData()
        {
            // Arrange
            var noteValues = new Dictionary<string, string>
            {
                { "diagnosis", "OSA" },
                { "ordering_provider", "Dr. Smith" },
                { "patient_name", "John Doe" },
                { "dob", "1980-01-01" },
                { "recommendation", "Patient needs Full face mask and heated humidifier." }
            };
            var extractor = new OrderExtractorCPAP(_loggerMock.Object);

            // Act
            JObject result = extractor.Extract(noteValues);

            // Assert
            Assert.Equal("CPAP", result["device"]);
            Assert.Equal("OSA", result["diagnosis"]);
            Assert.Equal("heated humidifier", result["add_ons"]);
            Assert.Equal("Dr. Smith", result["ordering physician"]);
            Assert.Equal("John Doe", result["patient name"]);
            Assert.Equal("1980-01-01", result["dob"]);
            Assert.Equal("Full face mask", result["mask_type"]);
        }

        [Fact]
        public void Extract_ShouldReturnDefaults_WhenKeysMissing()
        {
            // Arrange
            var noteValues = new Dictionary<string, string>();
            var extractor = new OrderExtractorCPAP(_loggerMock.Object);

            // Act
            JObject result = extractor.Extract(noteValues);

            // Assert
            Assert.Equal("CPAP", result["device"]);
            Assert.Equal("unknown", result["diagnosis"]);
            Assert.Equal("none", result["add_ons"]);
            Assert.Equal("unknown", result["ordering physician"]);
            Assert.Equal("unknown", result["patient name"]);
            Assert.Equal("unknown", result["dob"]);
            Assert.Equal("standard", result["mask_type"]);
        }

        [Fact]
        public void Extract_ShouldDetectFullFaceMask_OnlyInDiagnosisOrRecommendation()
        {
            // Arrange
            var noteValues = new Dictionary<string, string>
            {
                { "diagnosis", "Patient requires Full face mask" },
                { "ordering_provider", "Dr. Who" },
                { "patient_name", "Amy Pond" },
                { "dob", "1990-05-12" },
                { "recommendation", "Standard humidifier" }
            };
            var extractor = new OrderExtractorCPAP(_loggerMock.Object);

            // Act
            JObject result = extractor.Extract(noteValues);

            // Assert
            Assert.Equal("Full face mask", result["mask_type"]);
            Assert.Equal("none", result["add_ons"]);
        }

        [Fact]
        public void Extract_ShouldDetectHeatedHumidifier_OnlyInDiagnosisOrRecommendation()
        {
            // Arrange
            var noteValues = new Dictionary<string, string>
            {
                { "diagnosis", "OSA" },
                { "ordering_provider", "Dr. House" },
                { "patient_name", "Gregory House" },
                { "dob", "1959-06-11" },
                { "recommendation", "Recommend heated humidifier" }
            };
            var extractor = new OrderExtractorCPAP(_loggerMock.Object);

            // Act
            JObject result = extractor.Extract(noteValues);

            // Assert
            Assert.Equal("heated humidifier", result["add_ons"]);
            Assert.Equal("standard", result["mask_type"]);
        }

        [Fact]
        public void Extract_ShouldBeCaseInsensitive()
        {
            // Arrange
            var noteValues = new Dictionary<string, string>
            {
                { "diagnosis", "osa. full FACE MASK and HEATED HUMIDIFIER" },
                { "ordering_provider", "Dr. Sleep" },
                { "patient_name", "Jane Doe" },
                { "dob", "1975-03-22" }
            };
            var extractor = new OrderExtractorCPAP(_loggerMock.Object);

            // Act
            JObject result = extractor.Extract(noteValues);

            // Assert
            Assert.Equal("Full face mask", result["mask_type"]);
            Assert.Equal("heated humidifier", result["add_ons"]);
        }
    }
}