namespace Beforeguard.Frostline.Core

open System

type FrostlineError =
    | GeneralError of message: string * innerException: exn option
    | NotFound of resource: string
    | Unauthorized of message: string
    | RateLimited of retryAfter: int option

    /// Human-readable description, avoids requiring C# callers to pattern-match the union
    member this.Message =
        match this with
        | GeneralError(message, _) -> message
        | NotFound resource -> sprintf "Not found: %s" resource
        | Unauthorized message -> message
        | RateLimited(Some seconds) -> sprintf "Rate limited, retry after %ds" seconds
        | RateLimited None -> "Rate limited"

[<Sealed>]
type FrostlineException(error: FrostlineError) =
    inherit Exception(error.Message)

    member _.Error = error