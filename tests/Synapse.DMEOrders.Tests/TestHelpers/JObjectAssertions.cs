using Newtonsoft.Json.Linq;
using Xunit;

namespace Synapse.DMEOrders.Tests.TestHelpers;

/// <summary>
/// Extension assertions for JObject to reduce repetitive null / value boilerplate.
/// </summary>
public static class JObjectAssertions
{
    public static void AssertField(this JObject obj, string fieldName, string expected)
    {
        Assert.True(obj.ContainsKey(fieldName), $"Expected field '{fieldName}' to exist. JSON: {obj}");
        Assert.Equal(expected, obj[fieldName]!.Value<string>());
    }

    public static void AssertUnknownDefaults(this JObject obj, params string[] fieldNames)
    {
        foreach (var field in fieldNames)
        {
            obj.AssertField(field, "Unknown");
        }
    }
}
