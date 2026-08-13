module RegionTests

open System
open Xunit
open Beforeguard.Frostline.Core

[<Theory>]
[<InlineData("US", "us.api.blizzard.com")>]
[<InlineData("EU", "eu.api.blizzard.com")>]
[<InlineData("CN", "gateway.battlenet.com.cn")>]
[<InlineData("KR", "kr.api.blizzard.com")>]
[<InlineData("TW", "tw.api.blizzard.com")>]
let ``toHostname returns correct hostname for region`` (regionStr: string) (expected: string) =
    let region = 
        match regionStr with
        | "US" -> Region.US
        | "EU" -> Region.EU
        | "CN" -> Region.CN
        | "KR" -> Region.KR
        | "TW" -> Region.TW
        | _ -> failwith $"Unknown region: {regionStr}"
    
    let result = Region.toHostname region
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("US", "us")>]
[<InlineData("EU", "eu")>]
[<InlineData("KR", "kr")>]
[<InlineData("TW", "tw")>]
[<InlineData("CN", "cn")>]
let ``toString returns lowercase string for region`` (regionStr: string) (expected: string) =
    let region = 
        match regionStr with
        | "US" -> Region.US
        | "EU" -> Region.EU
        | "CN" -> Region.CN
        | "KR" -> Region.KR
        | "TW" -> Region.TW
        | _ -> failwith $"Unknown region: {regionStr}"
    
    let result = Region.toString region
    Assert.Equal(expected, result)