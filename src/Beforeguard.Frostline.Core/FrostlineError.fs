namespace Beforeguard.Frostline.Core

type FrostlineError =
    | GeneralError of message: string * innerException: exn option