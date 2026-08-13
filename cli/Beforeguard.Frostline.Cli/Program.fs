open System
open Beforeguard.Frostline.Core
open Microsoft.Extensions.Configuration

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

[<EntryPoint>]
let main argv =
    printfn "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    printfn "🎮 Frostline SDK - Battle.net API Client"
    printfn "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    try
        // Load configuration from User Secrets and environment variables
        printfn "\n📋 Loading configuration..."
        let clientConfig = loadConfigFromEnvironment()
        printfn "✅ Region: %s" (Region.toString clientConfig.Region)
        printfn "✅ Client ID: %s..." (clientConfig.ClientId.Substring(0, min 8 clientConfig.ClientId.Length))
        
        // Test OAuth token acquisition
        printfn "\n🔐 Testing OAuth Token Acquisition..."
        use tokenManager = new TokenManager(clientConfig)
        
        let token = tokenManager.getAccessToken() |> Async.RunSynchronously
        printfn "✅ Access token acquired!"
        printfn "   Token preview: %s..." (token.Substring(0, min 30 token.Length))
        printfn "   Token length: %d characters" token.Length
        
        // Test HTTP client
        printfn "\n🌐 Testing HTTP Client..."
        let httpClient = new BattleNetHttpClient(clientConfig.Region, tokenManager)
        
        // Try a real API call - WoW realm index
        printfn "   Fetching WoW realm index..."
        let result = 
            httpClient.getAsync("/data/wow/achievement/index?namespace=static-us&locale=en_US") 
            |> Async.AwaitTask 
            |> Async.RunSynchronously
        
        printfn "✅ API Response received!"
        printfn "   Response size: %d bytes" result.Length
        printfn "   Response preview:"
        printfn "   %s..." (result.Substring(0, min 200 result.Length))
        
        printfn "\n✨ All tests passed!"
        0 // Success exit code
        
    with
    | ex ->
        printfn "\n❌ Error: %s" ex.Message
        printfn "Stack trace:\n%s" ex.StackTrace
        1 // Error exit code