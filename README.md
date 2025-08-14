# Signal Booster Assignment

📘 Scenario

You’ve inherited a core utility from a developer who believed in “moving fast and breaking things.” The tool reads a physician’s note, extracts relevant information about the patient’s durable medical equipment (DME) needs — such as CPAPs or oxygen tanks — and sends the structured data to an external API.

Unfortunately, this developer took minimalism to an extreme:
- All logic is packed into `Main`
- Variable names are cryptic and inconsistent
- The code includes misleading comments and unused logic
- There’s no logging, no error handling, and no unit tests

Now, it’s your responsibility to clean it up. The business needs this feature to be reliable, maintainable, and production-ready and they need it fast.


🧪 Your Mission

Refactor the provided code into something that’s understandable, testable, and maintainable. Specifically:

1. **Refactor the logic into well-named, testable methods**
   - Improve structure and readability
   - Remove redundant or dead code
   - Use clear and consistent naming

2. **Introduce logging and basic error handling**
   - Avoid swallowing exceptions
   - Log meaningful steps for observability

3. **Write at least one unit test**
   - Show how you’d test a meaningful part of the logic

4. **Replace misleading or unclear comments with helpful ones**

5. **Keep it functional**
   - Your version must still:
     - Read a physician note from a file
     - Extract structured data (device type, provider, etc.)
     - POST the data to `https://alert-api.com/DrExtract` (Not a real link)

6. **(Optional stretch goals)**
   - Replace the manual extraction logic with an LLM (e.g., OpenAI or Azure OpenAI)
   - Accept multiple input formats (e.g., JSON-wrapped notes)
   - Add configurability for file path or API endpoint
   - Support more DME device types or qualifiers

📄 README Requirements

Please include a short `README.md` file in your submission with the following:

- What IDE or tools you used (e.g., VS Code, Rider, Visual Studio)
- Whether you used any AI development tools (e.g., GitHub Copilot, Cursor, Cody)
- Any assumptions, limitations, or future improvements
- Instructions to run the project (if needed)

✅ We encourage the use of AI tools to help you complete this assignment part of what we're evaluating is how you integrate modern development practices.

✅ If you are not a C# developer, we want you to re-write this into the language of your choice then follow the above.

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
Note: The app reads the note file from appsettings.json. THe code could easily be changed to loopo through the files in the configured directory.


## Tools used

- IDE: VS Code
- Runtime: .NET 8
- Libraries: Serilog, Serilog.Sinks.Console, Serilog.Sinks.File, Newtonsoft.Json
- Testing: xUnit, Microsoft.NET.Test.Sdk, coverlet.collector

## AI tools used

- GitHub Copilot

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
