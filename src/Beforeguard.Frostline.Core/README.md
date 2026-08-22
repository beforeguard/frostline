# Frostline.Core

Core authentication and HTTP infrastructure for the Frostline Battle.net SDK.

## Features

- **OAuth 2.0 Client Credentials Flow** - Automatic token acquisition and management
- **Token Caching** - Smart caching with expiry handling
- **Multi-Region Support** - US, EU, KR, TW, and CN regions
- **Result-Based Error Handling** - Idiomatic F# error handling with `Result<'T, FrostlineError>`
- **Pure F# Implementation** - Built using only .NET 10 built-ins, no external OAuth libraries

## Installation

```bash
dotnet add package Beforeguard.Frostline.Core
```

## Quick Start

```fsharp
open Beforeguard.Frostline.Core

// Configure with your Battle.net credentials
let config = ClientConfig.create "your-client-id" "your-client-secret" Region.US

// Create token manager and HTTP client
use tokenManager = new TokenManager(config)
let httpClient = BattleNetHttpClient(Region.US, tokenManager)

// Make authenticated API calls
let! result = httpClient.GetAsync<'T>("profile/user/wow")
match result with
| Ok data -> printfn "Success: %A" data
| Error err -> printfn "Error: %A" err
```

## Supported Regions

```fsharp
Region.US  // United States
Region.EU  // Europe
Region.KR  // Korea
Region.TW  // Taiwan
Region.CN  // China
```

## Error Handling

All API operations return `Result<'T, FrostlineError>` for predictable error handling:

```fsharp
type FrostlineError =
    | GeneralError of string
    // More specific error types coming in future releases
```

## Documentation

- **Full Documentation**: [github.com/Beforeguard/Frostline](https://github.com/Beforeguard/Frostline)
- **API Reference**: [Frostline Wiki](https://github.com/Beforeguard/Frostline/wiki)
- **Issues & Support**: [GitHub Issues](https://github.com/Beforeguard/Frostline/issues)

## Related Packages

- **Beforeguard.Frostline.WoW** - World of Warcraft API bindings
- More game-specific packages coming soon

## License

MIT License - Copyright © 2026 Beforeguard
