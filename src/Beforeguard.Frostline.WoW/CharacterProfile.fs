namespace Beforeguard.Frostline.WoW

open System.Text.Json.Serialization

module CharacterProfile =
    type Gender = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type Faction = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type Race = {
        [<JsonPropertyName("id")>]
        Id: int

        [<JsonPropertyName("name")>]
        Name: string
    }

    type Class = {
        [<JsonPropertyName("id")>]
        Id: int

        [<JsonPropertyName("name")>]
        Name: string
    }

    type Realm = {
        [<JsonPropertyName("id")>]
        Id: int

        [<JsonPropertyName("name")>]
        Name: string
    }

    type Specialization = {
        [<JsonPropertyName("id")>]
        Id: int

        [<JsonPropertyName("name")>]
        Name: string
    }

    type Guild = {
        [<JsonPropertyName("id")>]
        Id: int

        [<JsonPropertyName("name")>]
        Name: string

        [<JsonPropertyName("realm")>]
        Realm: string
    }

    type CharacterProfile = {
        [<JsonPropertyName("id")>]
        Id: int64

        [<JsonPropertyName("name")>]
        Name: string

        [<JsonPropertyName("gender")>]
        Gender: Gender

        [<JsonPropertyName("faction")>]
        Faction: Faction

        [<JsonPropertyName("race")>]
        Race: Race

        [<JsonPropertyName("character_class")>]
        CharacterClass: Class

        [<JsonPropertyName("active_spec")>]
        ActiveSpec: Specialization option

        [<JsonPropertyName("realm")>]
        Realm: Realm

        [<JsonPropertyName("level")>]
        Level: int

        [<JsonPropertyName("achievement_points")>]
        AchievementPoints: int

        [<JsonPropertyName("average_item_level")>]
        AverageItemLevel: int

        [<JsonPropertyName("equipped_item_level")>]
        EquippedItemLevel: int

        [<JsonPropertyName("guild")>]
        Guild: Guild option
    }

    let get (httpClient: Beforeguard.Frostline.Core.BattleNetHttpClient) 
            (region: Beforeguard.Frostline.Core.Region) 
            (realm: string) 
            (characterName: string) : Async<CharacterProfile> =
        async {
            // Normalize realm name (spaces to hyphens, lowercase)
            let normalizedRealm = realm.ToLower().Replace(" ", "-")
            let normalizedName = characterName.ToLower()
            
            // Build the API path
            let regionStr = Beforeguard.Frostline.Core.Region.toString region
            let path = sprintf "/profile/wow/character/%s/%s?namespace=profile-%s&locale=en_US" 
                              normalizedRealm normalizedName regionStr
            
            // Make the HTTP request
            let! json = httpClient.getAsync(path) |> Async.AwaitTask
            
            // Deserialize the JSON response
            let options = System.Text.Json.JsonSerializerOptions()
            options.PropertyNameCaseInsensitive <- true
            
            return System.Text.Json.JsonSerializer.Deserialize<CharacterProfile>(json, options)
        }