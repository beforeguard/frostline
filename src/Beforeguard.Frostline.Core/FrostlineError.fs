namespace Beforeguard.Frostline.Core

type FrostlineError =
    | GeneralError of message: string * innerException: exn option
    | NotFound of resource: string
    | Unauthorized of message: string
    | RateLimited of retryAfter: int option