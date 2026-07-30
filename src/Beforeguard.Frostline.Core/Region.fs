namespace Beforeguard.Frostline.Core

/// Represents the Blizzard API regions
type Region =
    | US
    | EU
    | KR
    | TW
    | CN

module Region =
    /// Convert a Region to its API hostname
    let toHostname region =
        match region with
        | US -> "us.api.blizzard.com"
        | EU -> "eu.api.blizzard.com"
        | KR -> "kr.api.blizzard.com"
        | TW -> "tw.api.blizzard.com"
        | CN -> "gateway.battlenet.com.cn"
    
    /// Convert a Region to its lowercase string representation
    let toString region =
        match region with
        | US -> "us"
        | EU -> "eu"
        | KR -> "kr"
        | TW -> "tw"
        | CN -> "cn"