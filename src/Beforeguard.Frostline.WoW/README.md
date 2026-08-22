# Frostline.WoW

World of Warcraft API bindings for the Frostline Battle.net SDK.

## Features

- **Type-Safe Character Profiles** - Strongly-typed character data with F# records
- **Equipment Information** - Item details, quality, and enchantments
- **Result-Based Error Handling** - Idiomatic F# patterns with `Result<'T, FrostlineError>`
- **Idiomatic F# Design** - Records, discriminated unions, and Option types throughout
- **Zero-Cost Abstractions** - Clean API without performance overhead

## Installation

```bash
dotnet add package Beforeguard.Frostline.WoW
dotnet add package Beforeguard.Frostline.Core
```

## Quick Start

```fsharp
open Beforeguard.Frostline.Core
open Beforeguard.Frostline.WoW

// Configure authentication
let config = ClientConfig.create "your-client-id" "your-client-secret" Region.US
use tokenManager = new TokenManager(config)
let httpClient = BattleNetHttpClient(Region.US, tokenManager)

// Get character profile
let! result = CharacterProfile.get httpClient "tichondrius" "charactername"
match result with
| Ok profile ->
    printfn "Character: %s" profile.Name
    printfn "Level: %d" profile.Level
    printfn "Class: %s" profile.CharacterClass.Name
| Error err ->
    printfn "Failed to fetch profile: %A" err
```

## Available APIs

### Character Profile ✅
```fsharp
CharacterProfile.get httpClient realm characterName
```

Retrieves character information including:
- Name, level, race, class
- Faction (Horde/Alliance)
- Guild information
- Average item level

### Coming Soon
- Character Equipment
- Guild Roster & Details
- Mythic+ Progress
- Collections & Achievements

## Item Quality

```fsharp
type ItemQuality =
    | Poor        // Gray
    | Common      // White
    | Uncommon    // Green
    | Rare        // Blue
    | Epic        // Purple
    | Legendary   // Orange
    | Artifact    // Golden
    | Heirloom    // Light blue
```

## Documentation

- **Full Documentation**: [github.com/Beforeguard/Frostline](https://github.com/Beforeguard/Frostline)
- **API Reference**: [Frostline Wiki](https://github.com/Beforeguard/Frostline/wiki)
- **Issues & Support**: [GitHub Issues](https://github.com/Beforeguard/Frostline/issues)

## Related Packages

- **Beforeguard.Frostline.Core** - Required for authentication and HTTP
- More Blizzard game packages coming soon (Diablo, Hearthstone, etc.)

## License

MIT License - Copyright © 2026 Beforeguard

---

> **Note:** This is an unofficial, community-created project and is not affiliated with, endorsed by, or supported by Blizzard Entertainment, Inc.
