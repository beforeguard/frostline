open System
open System.IO
open System.Text.Json
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
    
    let clientId =
        match config.["BattleNet:ClientId"] with
        | null -> failwith "BattleNet:ClientId not configured. Use dotnet user-secrets or environment variables."
        | value when String.IsNullOrWhiteSpace value ->
            failwith "BattleNet:ClientId not configured. Use dotnet user-secrets or environment variables."
        | value -> value

    let clientSecret =
        match config.["BattleNet:ClientSecret"] with
        | null -> failwith "BattleNet:ClientSecret not configured. Use dotnet user-secrets or environment variables."
        | value when String.IsNullOrWhiteSpace value ->
            failwith "BattleNet:ClientSecret not configured. Use dotnet user-secrets or environment variables."
        | value -> value

    let regionStr =
        match config.["BattleNet:Region"] with
        | null | "" -> "US"
        | s -> s.ToUpperInvariant()
    
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
        printfn "  <%s> @ %s" guild.Name guild.Realm.Name
    | None -> ()
    
    printfn ""
    printfn "───────────────────────────────────────────────────────────"
    printfn ""

let toAnsiColor (color: QualityColor) =
    sprintf "\u001b[38;2;%d;%d;%dm" color.R color.G color.B

let resetColor = "\u001b[0m"

let displayEquipment (equipment: CharacterEquipment.CharacterEquipment) =
    printfn ""
    printfn "╔═══════════════════════════════════════════════════════════╗"
    printfn "║              CHARACTER EQUIPMENT                          ║"
    printfn "╚═══════════════════════════════════════════════════════════╝"
    printfn ""
    
    printfn "  %s @ %s" equipment.Character.Name equipment.Character.Realm.Slug
    printfn ""
    
    // Calculate average item level
    let avgItemLevel = 
        if equipment.EquippedItems.IsEmpty then 0.0
        else
            let total = equipment.EquippedItems |> List.sumBy (fun item -> float item.Level.Value)
            total / float equipment.EquippedItems.Length
    
    printfn "  AVERAGE ITEM LEVEL: %.0f" avgItemLevel
    printfn "  EQUIPPED ITEMS: %d" equipment.EquippedItems.Length
    printfn ""
    printfn "  %-15s %-40s %5s" "SLOT" "ITEM" "ILVL"
    printfn "  %s" (String.replicate 65 "─")
    
    // Sort items by slot name for consistent display
    let sortedItems = 
        equipment.EquippedItems 
        |> List.sortBy (fun item -> item.Slot.Name)
    
    for item in sortedItems do
        let color = ItemQuality.getColor item.Quality.Type |> toAnsiColor
        let enchantIndicator = 
            match item.Enchantments with
            | Some enchants when not enchants.IsEmpty -> " ✨"
            | _ -> ""
        
        let socketIndicator =
            match item.Sockets with
            | Some sockets when not sockets.IsEmpty -> " 💎"
            | _ -> ""
        
        let itemName = 
            if item.Name.Length > 35 then 
                item.Name.Substring(0, 32) + "..."
            else 
                item.Name
        
        printfn "  %-15s %s%-40s%s %5d%s%s" 
            item.Slot.Name 
            color
            itemName
            resetColor
            item.Level.Value
            enchantIndicator
            socketIndicator
    
    printfn ""
    
    // Show set bonus info if available
    match equipment.EquippedItemSets with
    | Some sets when not sets.IsEmpty ->
        printfn "  SET BONUSES"
        printfn "  %s" (String.replicate 65 "─")
        for itemSet in sets do
            printfn "  %s (%d/%d pieces)" itemSet.ItemSet.Name itemSet.Items.Length itemSet.Items.Length
            for effect in itemSet.Effects do
                printfn "    • %s" effect.DisplayString
        printfn ""
    | _ -> ()
    
    printfn "  Legend: ✨ Enchanted  💎 Socketed"
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
        let httpClientLogger = loggerFactory.CreateLogger<BattleNetHttpClient>()
        
        use httpClient = new BattleNetHttpClient(clientConfig, httpClientLogger)
        
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
            | Error error ->
                match error with
                | FrostlineError.NotFound resource ->
                    printfn "\n❌ Not Found: %s" resource
                    printfn "   The character or realm may not exist, or the name may be misspelled."
                    1
                | FrostlineError.Unauthorized message ->
                    printfn "\n❌ Unauthorized: %s" message
                    printfn "   Check your API credentials are valid."
                    1
                | FrostlineError.RateLimited retryAfter ->
                    match retryAfter with
                    | Some seconds ->
                        printfn "\n❌ Rate Limited: Please retry after %d seconds" seconds
                    | None ->
                        printfn "\n❌ Rate Limited: Too many requests. Please try again later."
                    1
                | FrostlineError.GeneralError(message, innerEx) ->
                    printfn "\n❌ Error: %s" message
                    match innerEx with
                    | Some ex -> printfn "   Details: %s" ex.Message
                    | None -> ()
                    1
            
        | "character" :: "equipment" :: realm :: characterName :: _ ->
            // Character equipment command
            printfn "\n🔍 Fetching equipment for: %s @ %s..." characterName realm
            printfn "   Authenticating..."
            
            let result = 
                CharacterEquipment.get httpClient clientConfig.Region realm characterName
                |> Async.RunSynchronously
            
            match result with
            | Ok equipment ->
                displayEquipment equipment
                0 // Success
            | Error error ->
                match error with
                | FrostlineError.NotFound resource ->
                    printfn "\n❌ Not Found: %s" resource
                    printfn "   The character or realm may not exist, or the name may be misspelled."
                    1
                | FrostlineError.Unauthorized message ->
                    printfn "\n❌ Unauthorized: %s" message
                    printfn "   Check your API credentials are valid."
                    1
                | FrostlineError.RateLimited retryAfter ->
                    match retryAfter with
                    | Some seconds ->
                        printfn "\n❌ Rate Limited: Please retry after %d seconds" seconds
                    | None ->
                        printfn "\n❌ Rate Limited: Too many requests. Please try again later."
                    1
                | FrostlineError.GeneralError(message, innerEx) ->
                    printfn "\n❌ Error: %s" message
                    match innerEx with
                    | Some ex -> printfn "   Details: %s" ex.Message
                    | None -> ()
                    1
            
        | "probe" :: path :: rest ->
            // Raw, untyped capture of any Battle.net endpoint response for scaffolding new models
            let printOnly = rest |> List.exists (fun a -> a = "--print" || a = "-p")
            let outputArgs = rest |> List.filter (fun a -> a <> "--print" && a <> "-p")

            let outputPath =
                match outputArgs with
                | file :: _ -> file
                | [] ->
                    let safeName = path.Split('?').[0].Trim('/').Replace('/', '-')
                    Path.Combine("samples", safeName + ".json")

            printfn "\n🔎 Probing: %s" path
            printfn "   Authenticating..."

            let result =
                httpClient.getAsync<JsonElement>(path)
                |> Async.AwaitTask
                |> Async.RunSynchronously

            match result with
            | Ok json ->
                let options = JsonSerializerOptions(WriteIndented = true)
                let pretty = JsonSerializer.Serialize(json, options)

                if printOnly then
                    printfn ""
                    printfn "%s" pretty
                    0
                else
                    match Path.GetDirectoryName(outputPath) with
                    | null
                    | "" -> ()
                    | directory -> Directory.CreateDirectory(directory) |> ignore

                    File.WriteAllText(outputPath, pretty)

                    printfn "\n✅ Saved response to: %s" outputPath
                    printfn "   %d bytes written" pretty.Length
                    0
            | Error error ->
                match error with
                | FrostlineError.NotFound resource ->
                    printfn "\n❌ Not Found: %s" resource
                    1
                | FrostlineError.Unauthorized message ->
                    printfn "\n❌ Unauthorized: %s" message
                    1
                | FrostlineError.RateLimited retryAfter ->
                    match retryAfter with
                    | Some seconds -> printfn "\n❌ Rate Limited: Please retry after %d seconds" seconds
                    | None -> printfn "\n❌ Rate Limited: Too many requests. Please try again later."
                    1
                | FrostlineError.GeneralError(message, innerEx) ->
                    printfn "\n❌ Error: %s" message
                    match innerEx with
                    | Some ex -> printfn "   Details: %s" ex.Message
                    | None -> ()
                    1

        | [] | ["help"] | ["-h"] | ["--help"] ->
            // Show usage
            printfn "\nUsage:"
            printfn "  frostline character get <realm> <characterName>"
            printfn "  frostline character equipment <realm> <characterName>"
            printfn "  frostline probe <path> [outputFile] [--print|-p]"
            printfn ""
            printfn "Examples:"
            printfn "  frostline character get tichondrius beforeguard"
            printfn "  frostline character equipment \"area 52\" thrall"
            printfn "  frostline probe \"/profile/wow/character/tichondrius/beforeguard/equipment?namespace=profile-us&locale=en_US\" --print"
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
        match ex.InnerException with
        | null -> ()
        | inner -> printfn "   Details: %s" inner.Message
        1