using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Moq;
using Serilog;
using Synapse.DMEOrders;
using Xunit;
using Synapse.DMEOrders.Tests.TestHelpers;

namespace Synapse.DMEOrders.Tests.DeviceExtractors
{
    public class OrderExtractorWheelchairTest
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly OrderExtractorWheelchair _extractor;

        public OrderExtractorWheelchairTest()
        {
            _loggerMock = new Mock<ILogger>();
            _extractor = new OrderExtractorWheelchair(_loggerMock.Object);
        }

        [Fact]
        public void Extract_ReturnsCorrectJObject_WhenAllFieldsPresent()
        {
            var noteValues = TestHelpers.NoteValues.Wheelchair(
                diagnosis: "Paralysis",
                orderingPhysician: "Dr. Smith",
                patientName: "John Doe",
                dob: "1990-01-01");

            JObject result = _extractor.Extract(noteValues);

            result.AssertField("device", "Wheelchair");
            result.AssertField("diagnosis", "Paralysis");
            result.AssertField("ordering physician", "Dr. Smith");
            result.AssertField("patient name", "John Doe");
            result.AssertField("dob", "1990-01-01");
        }

        [Fact]
        public void Extract_ReturnsUnknown_WhenFieldsMissing()
        {
            var noteValues = new Dictionary<string, string>();

            JObject result = _extractor.Extract(noteValues);

            result.AssertField("device", "Wheelchair");
            result.AssertUnknownDefaults("diagnosis", "ordering physician", "patient name", "dob");
        }

        [Fact]
        public void Extract_LogsInformation()
        {
            var noteValues = new Dictionary<string, string>();
            _extractor.Extract(noteValues);

            _loggerMock.Verify(
                l => l.Information(It.Is<string>(s => s.Contains("Extracting Wheelchair order info"))),
                Times.Once);
        }
    }
}