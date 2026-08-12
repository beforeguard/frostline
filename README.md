# ❄️ Frostline

**A modern, idiomatic F# SDK for Blizzard Entertainment's Battle.net APIs**

Frostline provides type-safe, async-first access to World of Warcraft, Diablo, StarCraft, and other Blizzard game APIs with a clean functional interface.

## Features

✨ **Pure F# Implementation**
- Built from scratch using only .NET 10 built-ins
- No external OAuth libraries - complete control over authentication flow
- Leverages F#'s discriminated unions, records, and async workflows

🔐 **OAuth 2.0 Client Credentials Flow**
- Automatic token acquisition and caching
- Smart token refresh with expiry handling
- Region-aware authentication endpoints

🌍 **Multi-Region Support**
- US, EU, KR, TW, CN regions
- Region-specific API endpoints
- China special handling

🎮 **Game API Coverage** *(in development)*
- World of Warcraft Profile API
- Character, equipment, and media endpoints
- Additional games coming soon

## Status

**Early Development** - Core OAuth infrastructure complete and tested against live Battle.net APIs. WoW data models in progress.

This is a learning-focused project demonstrating F# best practices for SDK development.

## Quick Start

```fsharp
open Beforeguard.Frostline.Core

// Configure with your Battle.net credentials
let config = ClientConfig.create "your-client-id" "your-secret" Region.US

// Authenticate and make API calls
use tokenManager = new TokenManager(config)
let httpClient = BattleNetHttpClient(Region.US, tokenManager)

// Call any Battle.net endpoint
let! response = 
    httpClient.getAsync("/data/wow/achievement-category/index?namespace=static-us&locale=en_US")
    |> Async.AwaitTask
```

## Project Structure

```
src/
  Beforeguard.Frostline.Core/     # Core OAuth and HTTP infrastructure
  Beforeguard.Frostline.WoW/      # World of Warcraft API wrapper

cli/
  Beforeguard.Frostline.Cli/      # Test harness and examples

tests/
  Beforeguard.Frostline.Core.Tests/
  Beforeguard.Frostline.WoW.Tests/
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Battle.net API credentials (get them at https://develop.battle.net)

### Configuration

The SDK uses environment variables for configuration:

```bash
# Set your Battle.net credentials
export BNET_CLIENT_ID="your-client-id"
export BNET_CLIENT_SECRET="your-client-secret"
export BNET_REGION="US"  # US, EU, KR, TW, or CN
```

For the CLI test harness, use .NET User Secrets:

```bash
dotnet user-secrets set "BattleNet:ClientId" "your-client-id" --project cli/Beforeguard.Frostline.Cli
dotnet user-secrets set "BattleNet:ClientSecret" "your-secret" --project cli/Beforeguard.Frostline.Cli
dotnet user-secrets set "BattleNet:Region" "us" --project cli/Beforeguard.Frostline.Cli
```

### Running the CLI

```bash
dotnet run --project cli/Beforeguard.Frostline.Cli
```

## Why F#?

- **Type Safety**: Discriminated unions model API regions and enums perfectly
- **Null Safety**: Option types eliminate null reference errors
- **Async-First**: Native async workflows for API calls
- **Pattern Matching**: Elegant error handling and response parsing
- **Immutability**: Thread-safe by default

## Roadmap

- [x] OAuth 2.0 Client Credentials authentication
- [x] Multi-region support
- [x] Token caching and automatic refresh
- [x] HTTP client with bearer token injection
- [ ] Error handling with Result types
- [ ] Retry logic and rate limiting
- [ ] WoW Character Profile API
- [ ] WoW Equipment and Media APIs
- [ ] Static data endpoints (items, achievements)
- [ ] Additional game APIs (Diablo, StarCraft)

## Contributing

This project is in active development. Contributions, suggestions, and feedback welcome!

## License

MIT

---

*Built with ❄️ by developers who love both F# and Blizzard games*
