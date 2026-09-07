namespace Beforeguard.Frostline.Core

open System
open System.Runtime.CompilerServices

/// C#-friendly helpers for Result-based APIs, avoiding FSharp.Core pattern matching
[<Extension>]
type ResultExtensions =

    [<Extension>]
    static member Match(result: Result<'T, FrostlineError>, onOk: Func<'T, 'R>, onError: Func<FrostlineError, 'R>) : 'R =
        match result with
        | Ok value -> onOk.Invoke(value)
        | Error err -> onError.Invoke(err)

    [<Extension>]
    static member TryGetValue(result: Result<'T, FrostlineError>, value: outref<'T>) : bool =
        match result with
        | Ok v ->
            value <- v
            true
        | Error _ ->
            value <- Unchecked.defaultof<'T>
            false
