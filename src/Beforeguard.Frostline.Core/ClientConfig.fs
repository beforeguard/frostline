namespace Beforeguard.Frostline.Core

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
    
    /// Get the OAuth token endpoint for this region
    let getTokenEndpoint config =
        match config.Region with
        | CN -> "https://oauth.battlenet.com.cn/token"
        | _ -> sprintf "https://%s/oauth/token" (Region.toHostname config.Region)