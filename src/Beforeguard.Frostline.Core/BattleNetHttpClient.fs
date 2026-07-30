namespace Beforeguard.Frostline.Core

open System
open System.Net.Http
open System.Threading.Tasks

/// Simple HTTP client for making requests to Blizzard APIs
type BattleNetHttpClient(region: Region) =
    
    let httpClient = new HttpClient()
    let baseUrl = sprintf "https://%s" (Region.toHostname region)
    
    /// Make a GET request to the specified path
    member this.GetAsync(path: string) : Task<string> =
        async {
            let url = sprintf "%s%s" baseUrl path
            printfn "Making GET request to: %s" url
            
            let! response = httpClient.GetAsync(url) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return content
        }
        |> Async.StartAsTask
    
    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()