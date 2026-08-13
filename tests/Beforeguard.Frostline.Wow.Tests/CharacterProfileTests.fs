module CharacterProfileTests

open System
open System.Text.Json
open Xunit
open Beforeguard.Frostline.WoW.CharacterProfile

// Sample JSON response from Battle.net API
let sampleCharacterJson = """
{
  "id": 123456789,
  "name": "Thrall",
  "gender": {
    "type": "MALE",
    "name": "Male"
  },
  "faction": {
    "type": "HORDE",
    "name": "Horde"
  },
  "race": {
    "id": 2,
    "name": "Orc"
  },
  "character_class": {
    "id": 7,
    "name": "Shaman"
  },
  "active_spec": {
    "id": 262,
    "name": "Enhancement"
  },
  "realm": {
    "id": 1,
    "name": "Area 52"
  },
  "level": 70,
  "achievement_points": 9850,
  "average_item_level": 463,
  "equipped_item_level": 463,
  "guild": {
    "id": 12345,
    "name": "Test Guild",
    "realm": "Area 52"
  }
}
"""

[<Fact>]
let ``deserialize complete character profile with all fields`` () =
    // Arrange
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    
    // Act
    let profile = JsonSerializer.Deserialize<CharacterProfile>(sampleCharacterJson, options)
    
    // Assert
    Assert.Equal(123456789L, profile.Id)
    Assert.Equal("Thrall", profile.Name)
    Assert.Equal("Male", profile.Gender.Name)
    Assert.Equal("Horde", profile.Faction.Name)
    Assert.Equal("Orc", profile.Race.Name)
    Assert.Equal("Shaman", profile.CharacterClass.Name)
    Assert.Equal(70, profile.Level)
    Assert.Equal(463, profile.EquippedItemLevel)
    
    // Check optional fields are present
    Assert.True(profile.ActiveSpec.IsSome)
    Assert.Equal("Enhancement", profile.ActiveSpec.Value.Name)
    
    Assert.True(profile.Guild.IsSome)
    Assert.Equal("Test Guild", profile.Guild.Value.Name)

let characterWithoutOptionalFields = """
{
"id": 987654321,
"name": "Gamon",
"gender": {
    "type": "MALE",
    "name": "Male"
},
"faction": {
    "type": "HORDE",
    "name": "Horde"
},
"race": {
    "id": 2,
    "name": "Orc"
},
"character_class": {
    "id": 1,
    "name": "Warrior"
},
"realm": {
    "id": 1,
    "name": "Area 52"
},
"level": 70,
"achievement_points": 100,
"average_item_level": 200,
"equipped_item_level": 200
}
"""

[<Fact>]
let ``deserialize character without optional fields`` () =
    // Arrange
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    
    // Act
    let profile = JsonSerializer.Deserialize<CharacterProfile>(characterWithoutOptionalFields, options)
    
    // Assert
    Assert.Equal("Gamon", profile.Name)
    Assert.Equal(70, profile.Level)
    
    // Check optional fields are absent
    Assert.True(profile.ActiveSpec.IsNone)
    Assert.True(profile.Guild.IsNone)