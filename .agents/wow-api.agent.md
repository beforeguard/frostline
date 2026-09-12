---
name: wow-api
description: Scaffolds new Battle.net WoW API endpoints with proper F# types and Result-based error handling
applyTo:
  - "src/Beforeguard.Frostline.WoW/**"
  - "tests/Beforeguard.Frostline.WoW.Tests/**"
---

# WoW API Agent

Helps create new Battle.net WoW API endpoint modules following Frostline conventions.

## Module Structure

Follow the `CharacterProfile.fs` pattern:

```fsharp
namespace Beforeguard.Frostline.WoW

open System.Text.Json.Serialization

module [ResourceName] =
    type [TypeName] = {
        [<JsonPropertyName("field_name")>]
        FieldName: string
    }
```

## JSON Serialization

- Always use `[<JsonPropertyName("...")]` attributes
- Convert snake_case to PascalCase: `character_class` → `CharacterClass`
- Use appropriate F# types: `int`, `int64`, `string`, nested records

## Test Structure

Follow `CharacterProfileTests.fs`:

```fsharp
module [ResourceName]Tests

open System.Text.Json
open Xunit
open Beforeguard.Frostline.WoW.[ResourceName]

let sample[Resource]Json = """{ "id": 123 }"""

[<Fact>]
let ``deserialize [resource] with all fields`` () =
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    let result = JsonSerializer.Deserialize<[TypeName]>(sample[Resource]Json, options)
    Assert.Equal(123, result.Id)
```

## Workflow

1. Capture a live sample: run `frostline probe <path> --print` to preview the response, or omit `--print` to save it to `samples/<name>.json` (gitignored — raw captures may contain live account/character data and should never be committed as-is)
2. Create domain model in `src/Beforeguard.Frostline.WoW/[Resource].fs`, matching the shape of the captured sample
3. Create test in `tests/Beforeguard.Frostline.WoW.Tests/[Resource]Tests.fs`, using a trimmed and sanitized excerpt of the captured sample as `sample[Resource]Json` (strip real character/realm/guild names before committing)
4. Update .fsproj files to include new files

## Conventions

- Use records for data types
- Include XML doc comments for public types
- Test deserialization with real Battle.net API response samples
- Use `frostline probe` to capture real API responses instead of hand-typing sample JSON
- All API types should be nested within their module

## TODO: Patterns Still Evolving

- API client functions (HTTP call structure)
- Error mapping from HttpClient to FrostlineError
- Async workflows and Result handling
