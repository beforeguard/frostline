namespace Beforeguard.Frostline.Core

open System.Threading.Tasks

module AsyncResult =

    let startAsTask
        (operation: Async<Result<'T, FrostlineError>>)
        : Task<'T> =
        async {
            let! result = operation

            match result with
            | Ok value ->
                return value
            | Error error ->
                return raise (FrostlineException(error))
        }
        |> Async.StartAsTask