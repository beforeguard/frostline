namespace Beforeguard.Frostline.Core

open System

/// Configuration for Battle.net API client
type ClientConfig = {
    ClientId: string
    ClientSecret: string
    Region: Region
}

module ClientConfig =
    /// Create a new configuration
    let create clientId clientSecret region =
        { 
            ClientId = clientId
            ClientSecret = clientSecret
            Region = region 
        }

    let fromEnvironment () =
        let clientId = Environment.GetEnvironmentVariable("BNET_CLIENT_ID")
        let clientSecret = Environment.GetEnvironmentVariable("BNET_CLIENT_SECRET")
        let regionStr = Environment.GetEnvironmentVariable("BNET_REGION")
        
        if String.IsNullOrWhiteSpace(clientId) then
            failwith "BNET_CLIENT_ID environment variable not set"
        if String.IsNullOrWhiteSpace(clientSecret) then
            failwith "BNET_CLIENT_SECRET environment variable not set"
        
        let region = 
            match (if String.IsNullOrWhiteSpace(regionStr) then "US" else regionStr.ToUpperInvariant()) with
            | "US" -> Region.US
            | "EU" -> Region.EU
            | "KR" -> Region.KR
            | "TW" -> Region.TW
            | "CN" -> Region.CN
            | _ -> failwithf "Invalid region: %s. Must be US, EU, KR, TW, or CN" regionStr
        
        create clientId clientSecret region

    /// Get the OAuth token endpoint for this region
    let getTokenEndpoint config =
        match config.Region with
        | CN -> "https://oauth.battlenet.com.cn/token"
        | _ -> "https://oauth.battle.net/token"