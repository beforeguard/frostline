namespace Beforeguard.Frostline.Core

open System

/// Configuration for Battle.net API client
type ClientConfig = {
    ClientId: string
    ClientSecret: string
    Region: Region
}

    with
    /// C#-friendly factory; the ClientConfig module's `create` is renamed to ClientConfigModule by the compiler
    static member Create(clientId, clientSecret, region) =
        { ClientId = clientId; ClientSecret = clientSecret; Region = region }

module ClientConfig =
    /// Create a new configuration
    let create clientId clientSecret region =
        { 
            ClientId = clientId
            ClientSecret = clientSecret
            Region = region 
        }

    /// Get the OAuth token endpoint for this region
    let getTokenEndpoint config =
        match config.Region with
        | CN -> "https://oauth.battlenet.com.cn/token"
        | _ -> "https://oauth.battle.net/token"