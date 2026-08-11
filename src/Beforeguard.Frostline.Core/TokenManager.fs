namespace Beforeguard.Frostline.Core

open System
open System.Collections.Generic
open System.Net.Http
open System.Text.Json

/// Response from OAuth token endpoint
type TokenResponse = {
    access_token: string
    token_type: string
    expires_in: int
}

/// Manages OAuth access tokens
type TokenManager(config: ClientConfig) =
    
    let httpClient = new HttpClient()
    let mutable cachedToken: string option = None
    let mutable tokenExpiry: DateTimeOffset option = None
    
    /// Request a new access token from Battle.net
    member private this.requestNewToken() =
        async {
            let tokenEndpoint = ClientConfig.getTokenEndpoint config
            printfn "Requesting new token from: %s" tokenEndpoint
            
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
            
            printfn "Token received, expires in %d seconds" tokenResponse.expires_in
            
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
                printfn "Using cached token"
                return token
            | _ ->
                printfn "Token expired or missing, requesting new one"
                return! this.requestNewToken()
        }
    
    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()