open System
open Beforeguard.Frostline.Core
open Beforeguard.Frostline.WoW
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

type Marker = class end

let loadConfigFromEnvironment () =
    let config = 
        ConfigurationBuilder()
            .AddUserSecrets<Marker>()
            .AddEnvironmentVariables()
            .Build()
    
    let clientId = config.["BattleNet:ClientId"]
    let clientSecret = config.["BattleNet:ClientSecret"]
    let regionStr = config.["BattleNet:Region"]
    
    if String.IsNullOrWhiteSpace(clientId) then
        failwith "BattleNet:ClientId not configured. Use dotnet user-secrets or environment variables."
    if String.IsNullOrWhiteSpace(clientSecret) then
        failwith "BattleNet:ClientSecret not configured. Use dotnet user-secrets or environment variables."
    
    let region = 
        match (if String.IsNullOrWhiteSpace(regionStr) then "US" else regionStr.ToUpperInvariant()) with
        | "US" -> Region.US
        | "EU" -> Region.EU
        | "KR" -> Region.KR
        | "TW" -> Region.TW
        | "CN" -> Region.CN
        | _ -> failwithf "Invalid region: %s. Must be US, EU, KR, TW, or CN" regionStr
    
    ClientConfig.create clientId clientSecret region

let displayCharacterCard (profile: CharacterProfile.CharacterProfile) =
    printfn ""
    printfn "╔═══════════════════════════════════════════════════════════╗"
    printfn "║              CHARACTER PROFILE                            ║"
    printfn "╚═══════════════════════════════════════════════════════════╝"
    printfn ""
    
    // Name and basic info
    printfn "  %s" profile.Name
    printfn "  %s" (String.replicate profile.Name.Length "─")
    
    // Level, Race, Class
    let specInfo = 
        match profile.ActiveSpec with
        | Some spec -> sprintf " (%s)" spec.Name
        | None -> ""
    printfn "  Level %d %s %s%s" profile.Level profile.Race.Name profile.CharacterClass.Name specInfo
    
    // Faction
    let factionIcon = if profile.Faction.Type = "HORDE" then "🔴" else "🔵"
    printfn "  %s %s" factionIcon profile.Faction.Name
    
    printfn ""
    printfn "  SERVER"
    printfn "  %s" profile.Realm.Name
    
    printfn ""
    printfn "  ITEM LEVEL"
    printfn "  %d equipped | %d average" profile.EquippedItemLevel profile.AverageItemLevel
    
    printfn ""
    printfn "  ACHIEVEMENT POINTS"
    printfn "  %s" (profile.AchievementPoints.ToString("N0"))
    
    match profile.Guild with
    | Some guild ->
        printfn ""
        printfn "  GUILD"
        printfn "  <%s> @ %s" guild.Name guild.Realm
    | None -> ()
    
    printfn ""
    printfn "───────────────────────────────────────────────────────────"
    printfn ""

[<EntryPoint>]
let main argv =
    printfn "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    printfn "🎮 Frostline SDK - Battle.net API Client"
    printfn "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    try
        // Create logger factory
        use loggerFactory = LoggerFactory.Create(fun builder ->
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug)
            |> ignore
        )
        
        // Load configuration
        let clientConfig = loadConfigFromEnvironment()
        
        // Create loggers and components
        let tokenManagerLogger = loggerFactory.CreateLogger<TokenManager>()
        let httpClientLogger = loggerFactory.CreateLogger<BattleNetHttpClient>()
        
        use tokenManager = new TokenManager(clientConfig, tokenManagerLogger)
        use httpClient = new BattleNetHttpClient(clientConfig.Region, tokenManager, httpClientLogger)
        
        // Parse command-line arguments
        match argv |> Array.toList with
        | "character" :: "get" :: realm :: characterName :: _ ->
            // Character get command
            printfn "\n🔍 Fetching character: %s @ %s..." characterName realm
            printfn "   Authenticating..."
            
            let result = 
                CharacterProfile.get httpClient clientConfig.Region realm characterName
                |> Async.RunSynchronously
            
            match result with
            | Ok profile ->
                displayCharacterCard profile
                0 // Success
            | Error (FrostlineError.GeneralError(message, innerEx)) ->
                printfn "\n❌ Error: %s" message
                match innerEx with
                | Some ex -> printfn "   Details: %s" ex.Message
                | None -> ()
                1
            
        | [] | ["help"] | ["-h"] | ["--help"] ->
            // Show usage
            printfn "\nUsage:"
            printfn "  frostline character get <realm> <characterName>"
            printfn ""
            printfn "Examples:"
            printfn "  frostline character get tichondrius beforeguard"
            printfn "  frostline character get \"area 52\" thrall"
            printfn ""
            printfn "Configuration:"
            printfn "  Region: %s (from config)" (Region.toString clientConfig.Region)
            0
            
        | _ ->
            printfn "\n❌ Unknown command: %s" (String.concat " " argv)
            printfn "Run 'frostline help' for usage information."
            1
            
    with
    | ex ->
        printfn "\n❌ Error: %s" ex.Message
        if ex.InnerException <> null then
            printfn "   Details: %s" ex.InnerException.Message
        1