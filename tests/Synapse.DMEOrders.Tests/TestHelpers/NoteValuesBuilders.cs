using System.Collections.Generic;

namespace Synapse.DMEOrders.Tests.TestHelpers;

/// <summary>
/// Factory helpers to build note value dictionaries for different device extractors.
/// Keeps test bodies focused on intent instead of key casing & boilerplate.
/// </summary>
public static class NoteValues
{
    // Wheelchair extractor appears to expect PascalCase keys.
    public static Dictionary<string, string> Wheelchair(
        string? diagnosis = null,
        string? orderingPhysician = null,
        string? patientName = null,
        string? dob = null) => new()
    {
        { "diagnosis", diagnosis ?? "Paralysis" },
        { "ordering physician", orderingPhysician ?? "Dr. Smith" },
        { "patient name", patientName ?? "John Doe" },
        { "dob", dob ?? "1990-01-01" }
    };

    // CPAP extractor uses lower-case / spaced keys.
    public static Dictionary<string, string> CPAP(
        string? diagnosis = null,
        string? orderingProvider = null,
        string? patientName = null,
        string? dob = null,
        string? recommendation = null) => new()
    {
        { "diagnosis", diagnosis ?? "OSA" },
        { "ordering physician", orderingProvider ?? "Dr. Smith" },
        { "patient name", patientName ?? "John Doe" },
        { "dob", dob ?? "1980-01-01" },
        { "recommendation", recommendation ?? "Standard setup" }
    };

    // Oxygen tank extractor uses similar lower-case keys.
    public static Dictionary<string, string> OxygenTank(
        string? diagnosis = null,
        string? orderingProvider = null,
        string? patientName = null,
        string? dob = null,
        string? prescription = null,
        string? usage = null) => new()
    {
        { "diagnosis", diagnosis ?? "COPD" },
        { "ordering physician", orderingProvider ?? "Dr. Smith" },
        { "patient name", patientName ?? "John Doe" },
        { "dob", dob ?? "01/01/1970" },
        { "prescription", prescription ?? "Use 2 L oxygen daily" },
        { "usage", usage ?? "Continuous" }
    };
}
