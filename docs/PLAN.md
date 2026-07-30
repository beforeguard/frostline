# Plan: Beforeguard.Frostline - F# SDK for Blizzard Battle.net APIs

**TL;DR**: Build a modern, idiomatic F# SDK targeting .NET 10 for Blizzard's Battle.net APIs under the Beforeguard namespace. Start with OAuth 2.0 authentication and World of Warcraft player/character data, using a modular Core + game-specific library architecture. Leverage F# strengths: strong typing with discriminated unions, async workflows, computation expressions, and type providers for rapid prototyping.

**Approach**: Core infrastructure first (HTTP, OAuth, retry/rate limiting) → WoW Profile API → expand to other games and features incrementally.

---

## Steps

### Phase 1: Project Foundation *(steps 1-4 can run in parallel after setup)*

1. **Initialize .NET 10 solution structure**
   - Create `Beforeguard.Frostline.sln` at repository root
   - Create `src/Beforeguard.Frostline.Core/Beforeguard.Frostline.Core.fsproj` targeting .NET 10
   - Create `src/Beforeguard.Frostline.WoW/Beforeguard.Frostline.WoW.fsproj` (depends on Core)
   - Create `tests/Beforeguard.Frostline.Core.Tests/Beforeguard.Frostline.Core.Tests.fsproj` (xUnit + FsUnit)
   - Create `tests/Beforeguard.Frostline.WoW.Tests/Beforeguard.Frostline.WoW.Tests.fsproj`
   - Add `.editorconfig`, `Directory.Build.props` for consistent F# formatting
   - Configure NuGet package metadata (authors, license, repo URL)

2. **Install core dependencies** *(parallel with step 1)*
   - Beforeguard.Frostline.Core: FsHttp, IdentityModel, Polly, FSharp.SystemTextJson, FsToolkit.ErrorHandling
   - Tests: xUnit, FsUnit, Unquote, FsCheck for property-based testing
   - Development: Paket or NuGet for package management

3. **Define shared domain types in Beforeguard.Frostline.Core** *(depends on step 1)*
   - `Region.fs`: Discriminated union for US | EU | KR | TW | CN with string converters
   - `Locale.fs`: Common locale types (en_US, es_MX, etc.)
   - `ApiError.fs`: Discriminated union for Unauthorized | NotFound | RateLimited of retryAfter | Throttled | NetworkError of exn | InvalidResponse of string
   - `ApiResult.fs`: Result<'T, ApiError> type alias and helper functions
   - `Common.fs`: Domain primitives (ItemId, CharacterId, RealmSlug) using single-case DUs

4. **Create configuration models** *(parallel with step 3)*
   - `ClientConfig.fs`: Record with ClientId, ClientSecret, Region, RetryConfig, RateLimitConfig
   - Support loading from environment variables and appsettings.json
   - Use IOptions<T> pattern for .NET integration

### Phase 2: Core HTTP Infrastructure *(steps 5-7 sequential)*

5. **Implement HTTP client wrapper** *(depends on steps 2-3)*
   - `Http/HttpClient.fs`: Create IHttpClient interface and FsHttp-based implementation
   - Singleton HttpClient with configurable base URL per region
   - Request/response logging with sanitized output (no secrets)
   - Async-first API: `GetAsync`, `PostAsync` returning `Async<Result<'T, ApiError>>`
   - Automatic JSON deserialization using FSharp.SystemTextJson

6. **Build retry and resilience layer** *(depends on step 5)*
   - `Retry/Policies.fs`: Configure Polly policies for exponential backoff, circuit breaker, timeout
   - Handle 429 (rate limit), 503 (service unavailable), 401 (token expiry) specifically
   - Parse and respect `Retry-After` headers
   - Add jitter to prevent thundering herd
   - Make retry attempts observable for logging/telemetry

7. **Implement rate limiting** *(depends on step 5)*
   - `Retry/RateLimiter.fs`: MailboxProcessor-based token bucket implementation
   - Per-region rate limits (100 requests/second, 36,000/hour for Blizzard)
   - Queue requests when limit approached, fail fast if queue saturated
   - Parse `X-RateLimit-*` response headers and adjust dynamically

### Phase 3: OAuth 2.0 Authentication *(steps 8-9 sequential)*

8. **Implement OAuth client credentials flow** *(depends on steps 5-6)*
   - `Auth/TokenManager.fs`: Token acquisition, caching, and refresh logic
   - Use IdentityModel.Client for OAuth protocol
   - Token state model: Record with AccessToken, ExpiresAt, TokenType
   - Auto-refresh 5 minutes before expiry
   - Thread-safe token access using MailboxProcessor or AsyncLock
   - Handle refresh failures gracefully (retry with backoff, eventually propagate error)

9. **Create authentication computation expression** *(depends on step 8)*
   - `Auth/AuthBuilder.fs`: Computation expression for authenticated API calls
   - Automatically inject bearer token into requests
   - Handle 401 responses by triggering token refresh and retry
   - Expose `authenticate` function returning `Async<Result<TokenInfo, ApiError>>`

### Phase 4: World of Warcraft API Foundation *(steps 10-12, 10-11 can parallel)*

10. **Define WoW domain models using type provider prototyping** *(depends on step 1)*
    - Use FSharp.Data.JsonProvider with sample Battle.net API responses for rapid iteration
    - Focus on Profile API: Character, CharacterSummary, CharacterEquipment, CharacterMedia
    - Create `Models/Character.fs`, `Models/Equipment.fs`, `Models/Media.fs`
    - Migrate to hand-written records + discriminated unions for production:
      - `PlayableClass`, `PlayableRace`, `Gender`, `Faction` as DUs
      - `Character` record with Id, Name, Realm, Level, Class, Race, etc.
    - Configure FSharp.SystemTextJson converters for camelCase ↔ PascalCase

11. **Implement WoW API endpoints** *(depends on steps 9-10)*
    - `Endpoints/ProfileApi.fs`: Functions for Character Profile, Equipment, Media, Achievements
    - `Endpoints/CharacterApi.fs`: GetCharacterProfile, GetCharacterEquipment returning `Async<Result<'T, ApiError>>`
    - Use namespace pattern: `static-{region}` for static data, `profile-{region}` for profile data
    - All functions take Region, Locale, and specific IDs as parameters
    - Leverage Core's retry and rate limiting transparently

12. **Create WoW client facade** *(depends on step 11)*
    - `Client.fs`: High-level `WoWClient` type with convenient methods
    - Constructor takes `ClientConfig` and initializes TokenManager, HttpClient
    - Methods: `GetCharacterAsync(realm, characterName)`, `GetCharacterEquipmentAsync`, etc.
    - Encapsulate all OAuth and HTTP complexity
    - Provide both Result-based and exception-throwing overloads

### Phase 5: Advanced Features *(steps 13-15 can run in parallel)*

13. **Implement caching layer** *(depends on step 5)*
    - `Cache/MemoryCache.fs`: IMemoryCache wrapper with TTL configuration
    - Cache static data (items, achievements) aggressively (24 hours)
    - Cache profile data conservatively (5 minutes)
    - Cache key strategy: "wow:profile:{region}:{realm}:{character}"
    - Provide cache bypass option for real-time data

14. **Build computation expression for fluent API calls** *(depends on steps 9, 11)*
    - `Builders/BattleNetBuilder.fs`: Computation expression for chaining API calls
    - Handle Result propagation automatically (stop on first error)
    - Example usage: `battlenet { let! char = getCharacter; let! equip = getEquipment char.Id; return (char, equip) }`
    - Integrate with AsyncResult from FsToolkit.ErrorHandling

15. **Add observability and telemetry** *(depends on step 5)*
    - `Telemetry/Logging.fs`: Structured logging using Microsoft.Extensions.Logging
    - Log request/response timings, rate limit status, retry attempts, token refreshes
    - Provide hooks for custom telemetry (Application Insights, OpenTelemetry)
    - Sanitize logs (never log tokens or secrets)

### Phase 6: Testing and Documentation

16. **Write comprehensive unit tests** *(depends on all prior steps)*
    - Core.Tests: Test retry logic, rate limiting, token refresh, error handling
    - WoW.Tests: Test API endpoint functions with mocked HTTP responses
    - Use FsCheck for property-based testing of domain models
    - Mock IHttpClient for deterministic tests
    - Test error scenarios: 401, 404, 429, 503, network failures, malformed JSON

17. **Create integration tests** *(depends on steps 12, 16)*
    - Test against live Blizzard APIs (requires real credentials, run in CI only)
    - Verify OAuth flow, actual API responses, rate limiting behavior
    - Use test credentials from environment variables
    - Tag with `[<Trait("Category", "Integration")>]` for selective execution

18. **Write documentation and samples** *(depends on step 12)*
    - `README.md`: Quick start guide, installation, basic usage examples
    - `docs/Authentication.md`: OAuth setup, client ID/secret registration
    - `docs/GettingStarted.md`: First API call tutorial
    - `docs/RateLimiting.md`: Explain rate limits and SDK handling
    - `samples/BasicProfile/Program.fs`: Console app demonstrating character lookup
    - XML doc comments on all public types and functions

### Phase 7: Packaging and Release

19. **Configure NuGet packaging** *(depends on step 18)*
    - Set package version, release notes in .fsproj
    - Include README, LICENSE in package
    - Pack Beforeguard.Frostline.Core and Beforeguard.Frostline.WoW separately
    - Create meta-package Beforeguard.Frostline that references both
    - Test local package installation

20. **Set up CI/CD pipeline** *(parallel with step 19)*
    - GitHub Actions workflow: build, test, pack on every PR
    - Publish to NuGet.org on tagged releases
    - Run integration tests only on main branch (protect credentials)
    - Generate code coverage reports

---

## Relevant Files

Initial files to create (in implementation order):

**Solution & Config:**
- `Beforeguard.Frostline.sln` — Solution file
- `Directory.Build.props` — Shared MSBuild properties (LangVersion, TreatWarningsAsErrors)
- `.editorconfig` — F# code formatting rules
- `.gitignore` — Exclude bin/, obj/, .vs/, *.user

**Beforeguard.Frostline.Core:**
- `src/Beforeguard.Frostline.Core/Common.fs` — Domain primitives and utilities
- `src/Beforeguard.Frostline.Core/Region.fs` — Region DU with JSON converters
- `src/Beforeguard.Frostline.Core/Locale.fs` — Locale types
- `src/Beforeguard.Frostline.Core/ApiError.fs` — Error discriminated union
- `src/Beforeguard.Frostline.Core/ClientConfig.fs` — Configuration record types
- `src/Beforeguard.Frostline.Core/Http/HttpClient.fs` — HTTP client interface and implementation
- `src/Beforeguard.Frostline.Core/Retry/Policies.fs` — Polly retry policies
- `src/Beforeguard.Frostline.Core/Retry/RateLimiter.fs` — MailboxProcessor rate limiter
- `src/Beforeguard.Frostline.Core/Auth/TokenManager.fs` — OAuth token manager
- `src/Beforeguard.Frostline.Core/Auth/AuthBuilder.fs` — Authentication computation expression
- `src/Beforeguard.Frostline.Core/Cache/MemoryCache.fs` — Caching abstraction
- `src/Beforeguard.Frostline.Core/Telemetry/Logging.fs` — Logging infrastructure

**Beforeguard.Frostline.WoW:**
- `src/Beforeguard.Frostline.WoW/Models/Character.fs` — Character domain models
- `src/Beforeguard.Frostline.WoW/Models/Equipment.fs` — Equipment models
- `src/Beforeguard.Frostline.WoW/Models/Common.fs` — Shared WoW types (PlayableClass, Race, etc.)
- `src/Beforeguard.Frostline.WoW/Endpoints/ProfileApi.fs` — Profile API endpoint functions
- `src/Beforeguard.Frostline.WoW/Client.fs` — High-level WoWClient facade
- `src/Beforeguard.Frostline.WoW/Builders/WoWBuilder.fs` — WoW-specific computation expressions

**Tests:**
- `tests/Beforeguard.Frostline.Core.Tests/HttpClientTests.fs` — Test HTTP layer
- `tests/Beforeguard.Frostline.Core.Tests/RateLimiterTests.fs` — Test rate limiting
- `tests/Beforeguard.Frostline.Core.Tests/TokenManagerTests.fs` — Test OAuth flow
- `tests/Beforeguard.Frostline.WoW.Tests/CharacterApiTests.fs` — Test WoW endpoints
- `tests/Beforeguard.Frostline.WoW.Tests/IntegrationTests.fs` — Live API tests

**Documentation:**
- `README.md` — Main documentation entry point
- `docs/Authentication.md` — OAuth guide
- `docs/GettingStarted.md` — Tutorial
- `samples/BasicProfile/Program.fs` — Example console app

---

## Verification

**After Core implementation (Phase 1-3):**
1. Run `dotnet build` — all projects compile without warnings
2. Run `dotnet test` — all unit tests pass
3. Create test console app that authenticates and retrieves an OAuth token
4. Verify rate limiter delays requests appropriately (100 req/s limit)
5. Trigger 429 response (exceed rate limit) and verify retry with backoff

**After WoW API implementation (Phase 4):**
1. Run integration test: fetch character profile for known character (e.g., "Arthas" on "Stormrage-US")
2. Verify character data deserializes correctly with strong types
3. Test error cases: non-existent character returns NotFound, invalid credentials return Unauthorized
4. Measure request timing: should complete in <500ms with caching, <2s without

**After Advanced Features (Phase 5):**
1. Verify cache: second identical request returns instantly from cache
2. Test computation expression: chain 3 API calls, verify early exit on first error
3. Enable verbose logging: verify all requests, responses, retries, token refreshes are logged
4. Load test: 1000 concurrent requests, verify rate limiting prevents API errors

**After Packaging (Phase 7):**
1. Install Beforeguard.Frostline NuGet package in fresh .NET 10 console app
2. Write 10-line program to fetch character data using only package (no source references)
3. Verify IntelliSense works for all public APIs
4. Run GitHub Actions workflow end-to-end on test branch

---

## Decisions

**Architecture Decisions:**
- **Modular structure (Core + WoW)**: Enables independent versioning, keeps Core reusable for all Blizzard games, clearer separation of concerns
- **Start with WoW**: Most popular Blizzard API, rich character data validates all SDK patterns (auth, profiles, static data, media links)
- **F# computation expressions**: Idiomatic F#, excellent developer experience for chaining API calls, natural error handling
- **Type providers for prototyping only**: Rapid development velocity during API exploration, migrate to hand-written types for production stability and IntelliSense
- **Result-based error handling**: Makes errors explicit, forces handling, no hidden exceptions; provide exception-throwing overloads for simpler scenarios
- **MailboxProcessor for rate limiting**: Pure F#, simple, reliable, no external dependencies, fits async model perfectly
- **Polly for retries**: Battle-tested, .NET standard, excellent F# interop, rich policy composition

**Technology Stack:**
- **.NET 10 target**: Latest features, preview but stable enough for 2026 SDK development
- **FsHttp**: Most idiomatic F# HTTP client with computation expressions
- **IdentityModel**: Production OAuth standard, well-maintained by .NET Foundation
- **FSharp.SystemTextJson**: Modern, performant, better .NET ecosystem integration than Newtonsoft.Json
- **FsToolkit.ErrorHandling**: Community standard for Result/AsyncResult, reduces boilerplate
- **xUnit + FsUnit**: Industry standard testing, excellent F# DSL

**Scope Decisions:**
- **Included in initial release**: OAuth, WoW Profile API (character, equipment, media), retry logic, rate limiting, basic caching, comprehensive error handling
- **Deferred to v2**: Other game APIs (Diablo, StarCraft, Overwatch), WoW static data (items, achievements), Community APIs (auction house), type providers for dynamic exploration, distributed caching (Redis)
- **Future considerations**: C# facade library for C# consumers, Source generators for compile-time API validation, gRPC support if Blizzard adds it, Webhook support for real-time updates

**Configuration:**
- Default rate limits: 100 req/s, 36,000 req/hour (Blizzard's published limits)
- Default retry: 3 attempts, exponential backoff starting at 1s, max 30s
- Default cache TTL: 5 minutes for profiles, 24 hours for static data
- All configurable via ClientConfig

---

## Further Considerations

**1. Blizzard API Regions & Namespaces**
- Each region (US, EU, KR, TW, CN) has separate API endpoints: `https://{region}.api.blizzard.com`
- China requires special handling (different domain, auth server)
- Namespaces determine data version: `static-{region}` for unchanging data, `profile-{region}` for player data, `dynamic-{region}` for realtime data
- **Recommendation**: Start with US/EU only, add China support in v1.1 after validating different auth flow

**2. Type Provider Strategy**
- FSharp.Data.JsonProvider excellent for prototyping, but:
  - Adds runtime dependency on sample JSON files
  - Type inference can be fragile with API changes
  - IntelliSense less informative than hand-written docs
- **Recommendation**: Use type providers in development branch to generate initial types, then hand-code production types with proper documentation. Keep type provider version in `tools/` for API exploration during future updates.

**3. Multi-Language Support**
- Blizzard APIs support 13+ locales (en_US, es_MX, pt_BR, de_DE, etc.)
- All text fields return in requested locale
- Some locales not available in all regions
- **Recommendation**: Default to en_US, make locale parameter on all data-fetching functions, provide Locale validation against Region compatibility matrix

**4. OAuth Scope Management**
- Blizzard supports scopes: `wow.profile`, `sc2.profile`, `d3.profile`, `openid`
- Different endpoints require different scopes
- **Recommendation**: For initial release, request all scopes (`wow.profile sc2.profile d3.profile`) to simplify implementation. Add granular scope configuration in v1.1 for security-conscious users.

**5. Testing Against Live API**
- Requires Blizzard Battle.net API credentials (free to register at https://develop.battle.net)
- Rate limits apply to test calls
- Test characters may change (levels, gear)
- **Recommendation**: Create dedicated test Battle.net account, use stable test data (classic WoW characters less likely to change), document required environment variables (BNET_CLIENT_ID, BNET_CLIENT_SECRET), run integration tests only in CI to preserve rate limit quota
