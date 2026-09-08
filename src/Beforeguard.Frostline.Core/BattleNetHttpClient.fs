namespace Beforeguard.Frostline.Core

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Text.Json
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions

/// Simple HTTP client for making requests to Blizzard APIs with OAuth authentication
type BattleNetHttpClient(config: ClientConfig, logger: ILogger<BattleNetHttpClient>) =
    
    let tokenManager = new TokenManager(config)
    let httpClient = new HttpClient()
    let baseUrl = sprintf "https://%s" (Region.toHostname config.Region)
    
    new(config: ClientConfig) =
        new BattleNetHttpClient(
            config,
            NullLogger<BattleNetHttpClient>.Instance :> ILogger<BattleNetHttpClient>
        )

    /// The region this client was configured for
    member this.Region = config.Region
    
    /// Make an authenticated GET request to the specified path
    member this.getAsync<'T>(path: string) : Task<Result<'T, FrostlineError>> =
        async {
            try
                let! token = tokenManager.getAccessToken()
                let url = sprintf "%s%s" baseUrl path
                logger.LogDebug("Making authenticated GET request to: {Url}", url)
                
                httpClient.DefaultRequestHeaders.Authorization <- 
                    new AuthenticationHeaderValue("Bearer", token)
                
                let! response = httpClient.GetAsync(url) |> Async.AwaitTask
                
                // Check status code and map to specific errors
                if response.IsSuccessStatusCode then
                    let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                    
                    logger.LogDebug("Received JSON response: {Json}", content)
                    
                    // Deserialize here
                    let options = JsonSerializerOptions()
                    options.PropertyNameCaseInsensitive <- true
                    let result = 
                        match JsonSerializer.Deserialize<'T>(content, options) with
                        | null -> raise (FrostlineException(FrostlineError.GeneralError("Get response was empty or invalid", None)))
                        | value -> value
                    
                    logger.LogInformation("Successfully deserialized {Type} from {Path}", typeof<'T>.Name, path)
                    
                    return Ok result
                else
                    // Map HTTP status codes to specific errors
                    let error = 
                        match int response.StatusCode with
                        | 404 -> 
                            logger.LogWarning("Resource not found: {Path}", path)
                            FrostlineError.NotFound(path)
                        | 401 | 403 -> 
                            logger.LogWarning("Unauthorized access to: {Path}", path)
                            FrostlineError.Unauthorized("Authentication failed or insufficient permissions")
                        | 429 ->
                            let retryAfter = 
                                match response.Headers.RetryAfter with
                                | null -> None
                                | header when header.Delta.HasValue -> 
                                    Some (int header.Delta.Value.TotalSeconds)
                                | _ -> None
                            logger.LogWarning("Rate limited on: {Path}. Retry after: {RetryAfter}s", path, retryAfter)
                            FrostlineError.RateLimited(retryAfter)
                        | statusCode ->
                            logger.LogError("HTTP {StatusCode} error for: {Path}", statusCode, path)
                            FrostlineError.GeneralError(sprintf "HTTP %d error" statusCode, None)
                    
                    return Error error
            with
            | ex -> 
                logger.LogError(ex, "HTTP request failed for path: {Path}", path)
                return Error (FrostlineError.GeneralError("HTTP request failed", Some ex))
        }
        |> Async.StartAsTask
    
    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()
            (tokenManager :> IDisposable).Dispose()