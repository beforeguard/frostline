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

[<Theory>]
[<InlineData("US", "https://oauth.battle.net/token")>]
[<InlineData("EU", "https://oauth.battle.net/token")>]
[<InlineData("KR", "https://oauth.battle.net/token")>]
[<InlineData("TW", "https://oauth.battle.net/token")>]
[<InlineData("CN", "https://oauth.battlenet.com.cn/token")>]
let ``getTokenEndpoint returns correct endpoint for region`` (regionStr: string) (expectedEndpoint: string) =
    let region = 
        match regionStr with
        | "US" -> Region.US
        | "EU" -> Region.EU
        | "CN" -> Region.CN
        | "KR" -> Region.KR
        | "TW" -> Region.TW
        | _ -> failwith $"Unknown region: {regionStr}"
    
    let config = ClientConfig.create "id" "secret" region
    let endpoint = ClientConfig.getTokenEndpoint config

    Assert.Equal(expectedEndpoint, endpoint)