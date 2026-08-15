namespace Beforeguard.Frostline.Core

open System
open System.Collections.Generic
open System.Net.Http
open System.Text.Json
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions

/// Response from OAuth token endpoint
type TokenResponse = {
    access_token: string
    token_type: string
    expires_in: int
}

/// Manages OAuth access tokens
type TokenManager(config: ClientConfig, ?logger: ILogger<TokenManager>) =
    
    let logger = defaultArg logger (NullLogger<TokenManager>.Instance :> ILogger<TokenManager>)
    let httpClient = new HttpClient()
    let mutable cachedToken: string option = None
    let mutable tokenExpiry: DateTimeOffset option = None
    
    /// Request a new access token from Battle.net
    member private this.requestNewToken() =
        async {
            let tokenEndpoint = ClientConfig.getTokenEndpoint config
            logger.LogDebug("Requesting new token from: {TokenEndpoint}", tokenEndpoint)
            
            // Create form data for client credentials grant
            let formData = new FormUrlEncodedContent([
                KeyValuePair("grant_type", "client_credentials")
                KeyValuePair("client_id", config.ClientId)
                KeyValuePair("client_secret", config.ClientSecret)
            ])
            
            let! response = httpClient.PostAsync(tokenEndpoint, formData) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            
            let! json = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json)
            
            logger.LogInformation("Token received, expires in {ExpiresIn} seconds", tokenResponse.expires_in)
            
            cachedToken <- Some tokenResponse.access_token
            tokenExpiry <- Some (DateTimeOffset.UtcNow.AddSeconds(float tokenResponse.expires_in))
            
            return tokenResponse.access_token
        }
    
    /// Check if the cached token is still valid
    member private this.isTokenValid() =
        match tokenExpiry with
        | None -> false
        | Some expiry -> 
            // Refresh 5 minutes before expiry
            expiry > DateTimeOffset.UtcNow.AddMinutes(5.0)
    
    /// Get a valid access token (cached or new)
    member this.getAccessToken() =
        async {
            match cachedToken, this.isTokenValid() with
            | Some token, true ->
                logger.LogDebug("Using cached access token")
                return token
            | _ ->
                logger.LogInformation("Token expired or missing, requesting new token")
                return! this.requestNewToken()
        }
    
    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()