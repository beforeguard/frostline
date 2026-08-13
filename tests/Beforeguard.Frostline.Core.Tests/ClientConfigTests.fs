module ClientConfigTests

open System
open Xunit
open Beforeguard.Frostline.Core

[<Fact>]
let ``create builds config with all provided values`` () =
    let config = ClientConfig.create "my-client-id" "my-secret" Region.EU

    Assert.Equal("my-client-id", config.ClientId)
    Assert.Equal("my-secret", config.ClientSecret)
    Assert.Equal(Region.EU, config.Region)

[<Fact>]
let ``getTokenEndpoint returns battle.net for US region`` () =
    let config = ClientConfig.create "id" "secret" Region.US
    let endpoint = ClientConfig.getTokenEndpoint config

    Assert.Equal("https://oauth.battle.net/token", endpoint)

[<Fact>]
let ``getTokenEndpoint returns battlenet.com.cn for CN region`` () =
    let config = ClientConfig.create "id" "secret" Region.CN
    let endpoint = ClientConfig.getTokenEndpoint config

    Assert.Equal("https://oauth.battlenet.com.cn/token", endpoint)

[<Fact>]
let ``getTokenEndpoint returns battle.net for EU region`` () =
    let config = ClientConfig.create "id" "secret" Region.EU
    let endpoint = ClientConfig.getTokenEndpoint config

    Assert.Equal("https://oauth.battle.net/token", endpoint)

[<Fact>]
let ``getTokenEndpoint returns battle.net for KR region`` () =
    let config = ClientConfig.create "id" "secret" Region.KR
    let endpoint = ClientConfig.getTokenEndpoint config

    Assert.Equal("https://oauth.battle.net/token", endpoint)

[<Fact>]
let ``getTokenEndpoint returns battle.net for TW region`` () =
    let config = ClientConfig.create "id" "secret" Region.TW
    let endpoint = ClientConfig.getTokenEndpoint config

    Assert.Equal("https://oauth.battle.net/token", endpoint)