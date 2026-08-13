# Frostline Roadmap

> A modern, idiomatic F# SDK for Blizzard APIs.

## Vision

Frostline is a community-built .NET/F# SDK that provides a clean, strongly typed interface to Blizzard's APIs.

The project should prioritize:

- Idiomatic F# API design
- Strong domain models
- Excellent developer experience
- Small, composable packages
- Good documentation and testing
- Real-world usability over exhaustive API coverage

The **Frostline CLI** will serve as the primary consumer and reference application.

---

# Phase 1 — Foundation

**Goal:** Establish the project and prove the basic architecture.

### Frostline

- [ ] Create `Beforeguard/Frostline`
- [ ] Create F# solution
- [ ] Establish project structure
- [ ] Set up testing
- [ ] Set up CI
- [ ] Establish package naming and versioning conventions
- [ ] Create initial documentation

### Core

- [ ] HTTP client infrastructure
- [ ] OAuth authentication
- [ ] Token management
- [ ] JSON serialization
- [ ] Basic error handling
- [ ] Configuration

**Milestone:** Frostline can authenticate and make an authenticated Blizzard API request.

---

# Phase 2 — WoW SDK

**Goal:** Build the first useful Blizzard API surface.**

Create:

~~~text
Beforeguard.Frostline.WoW
~~~

Start with a small number of endpoints.

### Initial capabilities

- [ ] Character profile
- [ ] Character equipment
- [ ] Guild information

### F# design

- [ ] Strong domain types
- [ ] Records
- [ ] Discriminated unions where appropriate
- [ ] `Option` for optional data
- [ ] `Result` for expected failures
- [ ] Map external API models into Frostline models

**Milestone:** Frostline provides a clean, strongly typed F# interface to a useful subset of WoW APIs.

---

# Phase 3 — Frostline CLI

**Goal:** Build a real application that consumes Frostline.**

Create:

~~~text
Frostline.Cli
~~~

Initial capabilities:

~~~text
frostline wow character get
frostline wow character equipment
frostline wow guild get
~~~

The CLI should remain relatively simple.

Its purpose is to:

- Demonstrate Frostline
- Exercise the public API
- Provide a useful sample application
- Reveal weaknesses in the SDK design

**Milestone:** The CLI can perform useful WoW queries entirely through Frostline.

---

# Phase 4 — Frostline 1.0

**Goal:** Turn the prototype into a publishable library.**

### SDK

- [ ] Review public API design
- [ ] Improve error handling
- [ ] Handle rate limits
- [ ] Support cancellation
- [ ] Improve configuration
- [ ] Improve documentation
- [ ] Expand test coverage
- [ ] Remove unnecessary abstractions

### Packages

~~~text
Beforeguard.Frostline.Core
Beforeguard.Frostline.WoW
~~~

### Publishing

- [ ] NuGet metadata
- [ ] Package documentation
- [ ] Source Link
- [ ] Symbols
- [ ] Semantic versioning
- [ ] Automated releases
- [ ] Publish `1.0.0`

**Milestone:** A developer can install Frostline from NuGet and use it as a legitimate F#/.NET library.

---

# Phase 5 — Expand WoW

**Goal:** Build meaningful coverage of the WoW API without trying to implement everything.**

Potential areas:

- [ ] Characters
- [ ] Guilds
- [ ] Mythic+
- [ ] Auctions
- [ ] Professions
- [ ] Achievements
- [ ] Collections
- [ ] Items
- [ ] Media
- [ ] Game data

Prioritize APIs based on actual use cases rather than endpoint count.

**Milestone:** Frostline becomes a useful general-purpose WoW API client.

---

# Phase 6 — Additional Blizzard APIs

**Goal:** Expand Frostline beyond World of Warcraft.**

Potential packages:

~~~text
Beforeguard.Frostline.Diablo
Beforeguard.Frostline.Hearthstone
Beforeguard.Frostline.Overwatch
~~~

Each game should be independently consumable.

For example:

~~~text
Beforeguard.Frostline.WoW
Beforeguard.Frostline.Diablo
~~~

should not require the user to install unrelated game modules.

**Milestone:** Frostline becomes a multi-game Blizzard API SDK.

---

# Phase 7 — SDK Maturity

**Goal:** Make Frostline feel like a mature open-source library.**

### Developer experience

- [ ] Consistent API conventions
- [ ] Comprehensive XML documentation
- [ ] Strong examples
- [ ] Getting-started guides
- [ ] API reference documentation
- [ ] Clear upgrade guides

### Engineering

- [ ] Robust rate-limit handling
- [ ] Resilience
- [ ] Configurable HTTP pipeline
- [ ] Logging integration
- [ ] Diagnostics
- [ ] Performance testing
- [ ] Contract testing

### Packaging

- [ ] Automated NuGet releases
- [ ] Release notes
- [ ] API compatibility checks
- [ ] Versioning policy

**Milestone:** Frostline is a library you would be comfortable recommending to another .NET developer.

---

# Phase 8 — Advanced F# Features

**Goal:** Take advantage of F# where it genuinely improves the SDK.**

Potential areas:

- [ ] More expressive domain types
- [ ] Computation expressions where appropriate
- [ ] Type-safe API construction
- [ ] Functional error handling
- [ ] Validation
- [ ] Better pagination abstractions
- [ ] Async workflows
- [ ] Streaming APIs where appropriate

Avoid adding F# features simply because they are interesting. They should improve the public API.

**Milestone:** Frostline demonstrates what a well-designed F# library can look like.

---

# Phase 9 — Frostline Tooling

**Goal:** Turn the CLI into a more complete developer tool.**

Potential capabilities:

~~~text
frostline wow character
frostline wow guild
frostline wow item
frostline wow mythic
frostline diablo character
frostline hearthstone card
~~~

Potential features:

- [ ] Interactive terminal mode
- [ ] JSON output
- [ ] Table output
- [ ] Configuration profiles
- [ ] Shell-friendly output
- [ ] Export capabilities

The CLI remains primarily a **showcase and practical consumer** of the SDK.

---

# Phase 10 — Real-World Applications

**Goal:** Build applications that prove Frostline is useful beyond examples.**

Potential projects:

### Character Explorer

Explore WoW characters from the terminal.

### Guild Dashboard

Analyze guild members and progression.

### Mythic+ Analyzer

Explore Mythic+ activity and performance.

### Blizzard CLI

A unified terminal interface to supported Blizzard APIs.

### Discord Integration

Use Frostline as the API layer for a Discord bot.

These should remain separate applications rather than becoming part of the core SDK.

---

# Phase 11 — Community / Open Source

**Goal:** Make Frostline usable by people other than its creator.**

- [ ] Contribution guidelines
- [ ] Issue templates
- [ ] Feature request process
- [ ] Architecture documentation
- [ ] Contributor documentation
- [ ] Automated quality checks
- [ ] Community contributions
- [ ] Release cadence

Eventually:

~~~text
Beforeguard/Frostline
        │
        ├── Core
        ├── WoW
        ├── Diablo
        ├── Hearthstone
        └── Other Blizzard APIs
~~~

---

# Long-Term Vision

The ultimate Frostline ecosystem could look something like:

~~~text
Frostline
│
├── Core
│   ├── Authentication
│   ├── HTTP
│   ├── Serialization
│   ├── Errors
│   └── Diagnostics
│
├── Blizzard APIs
│   ├── WoW
│   ├── Diablo
│   ├── Hearthstone
│   └── Other supported APIs
│
├── CLI
│   └── Frostline.Cli
│
└── Applications
    ├── Character Explorer
    ├── Guild Dashboard
    ├── Mythic+ Analyzer
    └── Discord integrations
~~~

---

# Guiding Principles

### 1. Build from real usage

The CLI and other applications should drive SDK development.

### 2. Prefer depth over breadth

A small, excellent API is better than hundreds of poorly designed endpoints.

### 3. Keep the core small

Game-specific functionality belongs in game-specific packages.

### 4. Don't expose Blizzard's API directly

Frostline should provide its own clean, idiomatic F# domain model.

### 5. Optimize for developers

The primary question should always be:

> "Would I enjoy using this library?"

### 6. Don't over-engineer early

Start with:

~~~text
Core
WoW
CLI
~~~

and allow the architecture to evolve as real requirements appear.

---

# Major Milestones

| Version | Focus |
|---|---|
| `0.1` | Project foundation + authentication |
| `0.2` | First WoW endpoint |
| `0.3` | CLI + additional WoW endpoints |
| `1.0` | Stable, documented, published SDK |
| `1.x` | Expand WoW coverage |
| `2.0` | Additional Blizzard game APIs |
| `2.x` | SDK maturity + advanced features |
| `3.0+` | Broader ecosystem, tooling, and community |

The important part is that **1.0 does not mean "all Blizzard APIs."** It means Frostline has reached the point where its architecture, public API, documentation, testing, and packaging are good enough that someone else could confidently consume it.