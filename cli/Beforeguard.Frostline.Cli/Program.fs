open System
open Beforeguard.Frostline.Core
open Microsoft.Extensions.Configuration

type Marker = class end

[<EntryPoint>]
let main argv =
    printfn "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    printfn "🎮 Frostline SDK - Battle.net API Client"
    printfn "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    try
        // Load configuration from User Secrets and environment variables
        printfn "\n📋 Loading configuration..."
        let config = 
            ConfigurationBuilder()
                .AddUserSecrets<Marker>()
                .AddEnvironmentVariables()
                .Build()
        
        // Set environment variables for Core library to read
        Environment.SetEnvironmentVariable("BNET_CLIENT_ID", config.["BattleNet:ClientId"])
        Environment.SetEnvironmentVariable("BNET_CLIENT_SECRET", config.["BattleNet:ClientSecret"])
        Environment.SetEnvironmentVariable("BNET_REGION", config.["BattleNet:Region"])
        
        // Create client configuration from environment
        let clientConfig = ClientConfig.fromEnvironment()
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