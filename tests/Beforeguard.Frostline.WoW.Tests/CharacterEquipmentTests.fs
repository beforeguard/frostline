module CharacterEquipmentTests

open System
open System.Text.Json
open Xunit
open Beforeguard.Frostline.WoW.CharacterEquipment

// Sample JSON response from Battle.net API with complete equipment
let sampleEquipmentJson = """
{
  "character": {
    "id": 123456789,
    "name": "Thrall",
    "realm": {
      "id": 1,
      "slug": "area-52"
    }
  },
  "equipped_items": [
    {
      "item": {
        "id": 207172
      },
      "slot": {
        "type": "HEAD",
        "name": "Head"
      },
      "quantity": 1,
      "quality": {
        "type": "EPIC",
        "name": "Epic"
      },
      "name": "Amirdrassil Vengeance",
      "media": {
        "id": 4588951
      },
      "item_class": {
        "id": 4,
        "name": "Armor"
      },
      "item_subclass": {
        "id": 1,
        "name": "Leather"
      },
      "inventory_type": {
        "type": "HEAD",
        "name": "Head"
      },
        "binding": {
          "type": "ON_ACQUIRE",
          "name": "Binds when picked up"
        },
        "armor": {
          "value": 285,
          "display": {
            "display_string": "285 Armor",
            "color": {
              "r": 255,
              "g": 255,
              "b": 255,
              "a": 1.0
            }
          }
        },
        "level": {
          "value": 528,
          "display_string": "Item Level 528"
        },
        "enchantments": [
        {
          "display_string": "Enchanted: +50 Intellect",
          "enchantment_id": 6643,
          "enchantment_slot": {
            "id": 1,
            "type": "PERMANENT"
          }
        }
      ]
    },
    {
      "item": {
        "id": 193001
      },
      "slot": {
        "type": "CHEST",
        "name": "Chest"
      },
      "quantity": 1,
      "quality": {
        "type": "RARE",
        "name": "Rare"
      },
      "name": "Obsidian Cobraskin Vest",
      "media": {
        "id": 4235987
      },
      "item_class": {
        "id": 4,
        "name": "Armor"
      },
      "item_subclass": {
        "id": 1,
        "name": "Leather"
      },
      "inventory_type": {
        "type": "CHEST",
        "name": "Chest"
      },
      "binding": {
        "type": "ON_EQUIP",
        "name": "Binds when equipped"
      },
      "armor": {
        "value": 350,
        "display": {
          "display_string": "350 Armor",
          "color": {
            "r": 255,
            "g": 255,
            "b": 255,
            "a": 1.0
          }
        }
      },
      "level": {
        "value": 424,
        "display_string": "Item Level 424"
      }
    }
  ]
}
"""

[<Fact>]
let ``deserialize complete character equipment with all fields`` () =
    // Arrange
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    
    // Act
    let equipment = JsonSerializer.Deserialize<CharacterEquipment>(sampleEquipmentJson, options)
    
    // Assert
    Assert.Equal(123456789L, equipment.Character.Id)
    Assert.Equal("Thrall", equipment.Character.Name)
    Assert.Equal("area-52", equipment.Character.Realm.Slug)
    Assert.Equal(2, equipment.EquippedItems.Length)
    
    // Check first item (head with enchantment)
    let headItem = equipment.EquippedItems.[0]
    Assert.Equal("Amirdrassil Vengeance", headItem.Name)
    Assert.Equal("HEAD", headItem.Slot.Type)
    Assert.Equal("Epic", headItem.Quality.Name)
    Assert.Equal(528, headItem.Level.Value)
    Assert.True(headItem.Enchantments.IsSome)
    Assert.Single(headItem.Enchantments.Value) |> ignore
    Assert.Equal("Enchanted: +50 Intellect", headItem.Enchantments.Value.[0].DisplayString)
    
    // Check second item (chest without enchantment)
    let chestItem = equipment.EquippedItems.[1]
    Assert.Equal("Obsidian Cobraskin Vest", chestItem.Name)
    Assert.Equal("CHEST", chestItem.Slot.Type)
    Assert.Equal("Rare", chestItem.Quality.Name)
    Assert.Equal(424, chestItem.Level.Value)
    Assert.True(chestItem.Enchantments.IsNone)

// Minimal equipment response
let minimalEquipmentJson = """
{
  "character": {
    "id": 987654321,
    "name": "Gamon",
    "realm": {
      "id": 1,
      "slug": "area-52"
    }
  },
  "equipped_items": [
    {
      "item": {
        "id": 25
      },
      "slot": {
        "type": "MAIN_HAND",
        "name": "Main Hand"
      },
      "quantity": 1,
      "quality": {
        "type": "COMMON",
        "name": "Common"
      },
      "name": "Worn Shortsword",
      "media": {
        "id": 12345
      },
      "item_class": {
        "id": 2,
        "name": "Weapon"
      },
      "item_subclass": {
        "id": 7,
        "name": "Sword"
      },
      "inventory_type": {
        "type": "MAIN_HAND",
        "name": "Main Hand"
      },
      "binding": {
        "type": "NONE",
        "name": "None"
      },
      "level": {
        "value": 5,
        "display_string": "Item Level 5"
      }
    }
  ]
}
"""

[<Fact>]
let ``deserialize minimal character equipment without optional fields`` () =
    // Arrange
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    
    // Act
    let equipment = JsonSerializer.Deserialize<CharacterEquipment>(sampleEquipmentJson, options)
    
    // Assert
    Assert.NotNull(equipment)
    Assert.NotNull(equipment.Character)
    Assert.NotEmpty(equipment.EquippedItems)
    
    // Verify optional fields can be absent
    let item = equipment.EquippedItems |> List.find (fun i -> i.Slot.Type = "CHEST")
    Assert.True(item.Enchantments.IsNone || item.Enchantments.Value.IsEmpty)

[<Fact>]
let ``equipment items have correct slot types`` () =
    // Arrange
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    
    // Act
    let equipment = JsonSerializer.Deserialize<CharacterEquipment>(sampleEquipmentJson, options)
    
    // Assert
    let slots = equipment.EquippedItems |> List.map (fun item -> item.Slot.Type)
    Assert.Contains("HEAD", slots)
    Assert.Contains("CHEST", slots)
