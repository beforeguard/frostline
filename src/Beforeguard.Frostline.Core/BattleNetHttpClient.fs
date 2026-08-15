namespace Beforeguard.Frostline.Core

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Text.Json
open System.Threading.Tasks

/// Simple HTTP client for making requests to Blizzard APIs with OAuth authentication
type BattleNetHttpClient(region: Region, tokenManager: TokenManager) =
    
    let httpClient = new HttpClient()
    let baseUrl = sprintf "https://%s" (Region.toHostname region)
    
    /// Make an authenticated GET request to the specified path
    member this.getAsync<'T>(path: string) : Task<Result<'T, FrostlineError>> =
        async {
            try
                let! token = tokenManager.getAccessToken()
                let url = sprintf "%s%s" baseUrl path
                printfn "Making authenticated GET request to: %s" url
                
                httpClient.DefaultRequestHeaders.Authorization <- 
                    new AuthenticationHeaderValue("Bearer", token)
                
                let! response = httpClient.GetAsync(url) |> Async.AwaitTask
                response.EnsureSuccessStatusCode() |> ignore
                
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                // Deserialize here
                let options = JsonSerializerOptions()
                options.PropertyNameCaseInsensitive <- true
                let result = JsonSerializer.Deserialize<'T>(content, options)
                
                return Ok result
            with
            | ex -> return Error (FrostlineError.GeneralError("HTTP request failed", Some ex))
        }
        |> Async.StartAsTask
    
    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()