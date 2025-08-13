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
  - OrderSender: sends the structured JSON payload (logging enabled; POST currently commented to avoid calling a fake endpoint)
- tests/Synapse.DMEOrders.Tests: xUnit tests for extractors and parsing

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

Note: The app reads the sample note file bundled in src/Synapse.DMEOrders/physician_note1.txt by default.

You can override the note file path using an environment variable:
```bash
dotnet run --project src/Synapse.DMEOrders
```

## Tools used

- OS/IDE: macOS, VS Code (C# Dev Kit)
- Runtime: .NET 8
- Libraries: Serilog, Serilog.Sinks.Console, Serilog.Sinks.File, Newtonsoft.Json
- Testing: xUnit, Microsoft.NET.Test.Sdk, coverlet.collector

## AI tools used

- GitHub Copilot

## Assumptions, limitations, and future improvements

- Input formats supported: plain text and JSON with a top-level "data" string.
- Key naming: output uses "ordering physician" as the provider key for consistency across tests and extractors.
- Defaults: unspecified fields default to "Unknown" (or sensible defaults like "standard"/"none" for CPAP mask/add-ons).
- API POST: The example endpoint is not real; the HTTP POST is left commented to prevent accidental external calls. Consider enabling POST behind a flag or environment variable (e.g., SEND_ORDERS=true).
- Configuration: File path and API endpoint are currently hard-coded; these could be made configurable via command-line args or environment variables.
- Cleanup opportunities: unify constant naming (e.g., rename PROP_ORDERING_PROVIDER to PROP_ORDERING_PHYSICIAN in the base), remove dead code (legacy parsing helper in the base), and normalize unknown casing across outputs.
