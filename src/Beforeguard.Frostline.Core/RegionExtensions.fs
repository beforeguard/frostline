namespace Beforeguard.Frostline.Core

open System.Runtime.CompilerServices

/// C#-friendly helper for the Region union
[<Extension>]
type RegionExtensions =
    [<Extension>]
    static member ToHostname(region: Region) = Region.toHostname region
