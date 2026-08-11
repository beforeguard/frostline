namespace Beforeguard.Frostline.Core

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Threading.Tasks

/// Simple HTTP client for making requests to Blizzard APIs with OAuth authentication
type BattleNetHttpClient(region: Region, tokenManager: TokenManager) =
    
    let httpClient = new HttpClient()
    let baseUrl = sprintf "https://%s" (Region.toHostname region)
    
    /// Make an authenticated GET request to the specified path
    member this.getAsync(path: string) : Task<string> =
        async {
            // Get a valid access token (cached or fresh)
            let! token = tokenManager.getAccessToken()
            
            // Build the full URL
            let url = sprintf "%s%s" baseUrl path
            printfn "Making authenticated GET request to: %s" url
            
            // Set the Authorization header with bearer token
            httpClient.DefaultRequestHeaders.Authorization <- 
                new AuthenticationHeaderValue("Bearer", token)
            
            // Make the request
            let! response = httpClient.GetAsync(url) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            
            // Read and return the response content
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return content
        }
        |> Async.StartAsTask
    
    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()