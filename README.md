# Signal Booster Assignment

# Project notes

## Implementation overview

This repo implements the refactor with a small, testable architecture:
- src/Synapse.DMEOrders: app code
  - FileReader: reads the physician note from disk
  - NoteParser: parses JSON-wrapped notes (data field) or plain text into a dictionary
  - OrderExtractorManager: selects a device-specific extractor
  - Device extractors: CPAP, Oxygen Tank, Wheelchair
  - OrderSender: sends the structured JSON payload to the configured endpoint (logging enabled, sending disabled in config file)
- tests/Synapse.DMEOrders.Tests: xUnit tests for extractors

## How to run

Prereqs: .NET 8 SDK installed

Build and test
```bash
dotnet build
dotnet test --nologo
```

Run the app
```bash
dotnet run --project src/Synapse.DMEOrders
```
Note: The app reads the note file from appsettings.json. THe code could easily be changed to loop through the files in the configured directory.


## Tools used

- IDE: VS Code
- Runtime: .NET 8
- Libraries: Serilog, Serilog.Sinks.Console, Serilog.Sinks.File, Newtonsoft.Json
- Testing: xUnit, Microsoft.NET.Test.Sdk, coverlet.collector

## AI tools used

- GitHub Copilot
  - Prototyped unit tests
  - Investigated the more arcane bugs
  - Refactored variable names
  - Replaced magic strings with variables
  - Base documentation
  - Found dead code
  - Monitoring for code hygiene (SRP, etc.)
  - Personally validated all changes
  - Enhanced error handling

## Assumptions, limitations, and future improvements

- Input formats supported: plain text, and JSON with a top-level "data" string.
- Defaults: unspecified fields default to "Unknown" (or sensible defaults like "standard"/"none" for CPAP mask/add-ons).
- API POST: 
- Configuration: All configuration is in appsettings.json. That includes:
   The API endpoint 
   The note folder and file name
   A flag that to enable/disable sending to API in case the endpoint is not ready.
- New devices can be added by adding a new OrderExtractor classes, and adding them to the OrderExtractorManagver constructor
- AI parsing can be implemented in the NoteParser class
