namespace Beforeguard.Frostline.WoW

open System.Runtime.CompilerServices
open Beforeguard.Frostline.Core

/// C#-friendly, Task-returning wrappers over the WoW module functions
[<Extension>]
type BattleNetHttpClientExtensions =

    [<Extension>]
    static member GetCharacterProfileAsync(
        client: BattleNetHttpClient,
        realm: string,
        characterName: string
    ) =
        CharacterProfile.get client client.Region realm characterName
        |> AsyncResult.startAsTask

    [<Extension>]
    static member GetCharacterEquipmentAsync(
        client: BattleNetHttpClient,
        realm: string,
        characterName: string
    ) =
        CharacterEquipment.get client client.Region realm characterName
        |> AsyncResult.startAsTask