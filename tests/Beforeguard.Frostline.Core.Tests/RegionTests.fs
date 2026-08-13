module RegionTests

open System
open Xunit
open Beforeguard.Frostline.Core

[<Fact>]
let ``toHostname returns correct hostname for US region`` () =
    let result = Region.toHostname Region.US
    Assert.Equal("us.api.blizzard.com", result)

[<Fact>]
let ``toHostname returns correct hostname for EU region`` () =
    let result = Region.toHostname Region.EU
    Assert.Equal("eu.api.blizzard.com", result)

[<Fact>]
let ``toHostname returns correct hostname for CN region`` () =
    let result = Region.toHostname Region.CN
    Assert.Equal("gateway.battlenet.com.cn", result)

[<Fact>]
let ``toString returns lowercase us for US region`` () =
    let result = Region.toString Region.US
    Assert.Equal("us", result)

[<Fact>]
let ``toString returns lowercase eu for EU region`` () =
    let result = Region.toString Region.EU
    Assert.Equal("eu", result)