namespace Beforeguard.Frostline.WoW

open System.Text.Json.Serialization

module CharacterEquipment =
    type ItemQuality = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type ItemMedia = {
        [<JsonPropertyName("id")>]
        Id: int
    }

    type ItemClass = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type ItemSubclass = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type InventoryType = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type Binding = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type Enchantment = {
        [<JsonPropertyName("display_string")>]
        DisplayString: string
        
        [<JsonPropertyName("source_item")>]
        SourceItem: ItemReference option
        
        [<JsonPropertyName("enchantment_id")>]
        EnchantmentId: int
        
        [<JsonPropertyName("enchantment_slot")>]
        EnchantmentSlot: EnchantmentSlot
    }
    
    and EnchantmentSlot = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("type")>]
        Type: string
    }
    
    and ItemReference = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type Socket = {
        [<JsonPropertyName("socket_type")>]
        SocketType: SocketType
        
        [<JsonPropertyName("item")>]
        Item: ItemReference option
        
        [<JsonPropertyName("display_string")>]
        DisplayString: string option
    }
    
    and SocketType = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type Stat = {
        [<JsonPropertyName("type")>]
        Type: StatType
        
        [<JsonPropertyName("value")>]
        Value: int
        
        [<JsonPropertyName("display")>]
        Display: StatDisplay option
    }
    
    and StatType = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }
    
    and StatDisplay = {
        [<JsonPropertyName("display_string")>]
        DisplayString: string
        
        [<JsonPropertyName("color")>]
        Color: ColorInfo
    }
    
    and ColorInfo = {
        [<JsonPropertyName("r")>]
        R: int
        
        [<JsonPropertyName("g")>]
        G: int
        
        [<JsonPropertyName("b")>]
        B: int
        
        [<JsonPropertyName("a")>]
        A: float
    }

    type ArmorInfo = {
        [<JsonPropertyName("value")>]
        Value: int
        
        [<JsonPropertyName("display")>]
        Display: ArmorDisplay
    }
    
    and ArmorDisplay = {
        [<JsonPropertyName("display_string")>]
        DisplayString: string
        
        [<JsonPropertyName("color")>]
        Color: ColorInfo
    }

    type SellPrice = {
        [<JsonPropertyName("value")>]
        Value: int
        
        [<JsonPropertyName("display_strings")>]
        DisplayStrings: SellPriceDisplay
    }
    
    and SellPriceDisplay = {
        [<JsonPropertyName("header")>]
        Header: string
        
        [<JsonPropertyName("gold")>]
        Gold: string
        
        [<JsonPropertyName("silver")>]
        Silver: string
        
        [<JsonPropertyName("copper")>]
        Copper: string
    }

    type LevelInfo = {
        [<JsonPropertyName("value")>]
        Value: int
        
        [<JsonPropertyName("display_string")>]
        DisplayString: string
    }

    type Item = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("name")>]
        Name: string option
        
        [<JsonPropertyName("quality")>]
        Quality: ItemQuality option
        
        [<JsonPropertyName("item_class")>]
        ItemClass: ItemClass option
        
        [<JsonPropertyName("item_subclass")>]
        ItemSubclass: ItemSubclass option
        
        [<JsonPropertyName("inventory_type")>]
        InventoryType: InventoryType option
        
        [<JsonPropertyName("binding")>]
        Binding: Binding option
        
        [<JsonPropertyName("media")>]
        Media: ItemMedia option
    }
    
    and Requirements = {
        [<JsonPropertyName("level")>]
        Level: LevelRequirement option
    }
    
    and LevelRequirement = {
        [<JsonPropertyName("value")>]
        Value: int
        
        [<JsonPropertyName("display_string")>]
        DisplayString: string
    }

    type EquippedItem = {
        [<JsonPropertyName("item")>]
        Item: Item
        
        [<JsonPropertyName("slot")>]
        Slot: SlotType
        
        [<JsonPropertyName("quantity")>]
        Quantity: int
        
        [<JsonPropertyName("context")>]
        Context: int option
        
        [<JsonPropertyName("bonus_list")>]
        BonusList: int list option
        
        [<JsonPropertyName("quality")>]
        Quality: ItemQuality
        
        [<JsonPropertyName("name")>]
        Name: string
        
        [<JsonPropertyName("modified_appearance_id")>]
        ModifiedAppearanceId: int option
        
        [<JsonPropertyName("media")>]
        Media: ItemMedia
        
        [<JsonPropertyName("item_class")>]
        ItemClass: ItemClass
        
        [<JsonPropertyName("item_subclass")>]
        ItemSubclass: ItemSubclass
        
        [<JsonPropertyName("inventory_type")>]
        InventoryType: InventoryType
        
        [<JsonPropertyName("binding")>]
        Binding: Binding
        
        [<JsonPropertyName("armor")>]
        Armor: ArmorInfo option
        
        [<JsonPropertyName("stats")>]
        Stats: Stat list option
        
        [<JsonPropertyName("sell_price")>]
        SellPrice: SellPrice option
        
        [<JsonPropertyName("requirements")>]
        Requirements: Requirements option
        
        [<JsonPropertyName("level")>]
        Level: LevelInfo
        
        [<JsonPropertyName("enchantments")>]
        Enchantments: Enchantment list option
        
        [<JsonPropertyName("sockets")>]
        Sockets: Socket list option
    }
    
    and SlotType = {
        [<JsonPropertyName("type")>]
        Type: string
        
        [<JsonPropertyName("name")>]
        Name: string
    }

    type CharacterEquipment = {
        [<JsonPropertyName("character")>]
        Character: CharacterReference
        
        [<JsonPropertyName("equipped_items")>]
        EquippedItems: EquippedItem list
        
        [<JsonPropertyName("equipped_item_sets")>]
        EquippedItemSets: EquippedItemSet list option
    }
    
    and CharacterReference = {
        [<JsonPropertyName("id")>]
        Id: int64
        
        [<JsonPropertyName("name")>]
        Name: string
        
        [<JsonPropertyName("realm")>]
        Realm: RealmReference
    }
    
    and RealmReference = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("slug")>]
        Slug: string
    }
    
    and EquippedItemSet = {
        [<JsonPropertyName("item_set")>]
        ItemSet: ItemSetReference
        
        [<JsonPropertyName("items")>]
        Items: ItemReference list
        
        [<JsonPropertyName("effects")>]
        Effects: SetEffect list
    }
    
    and ItemSetReference = {
        [<JsonPropertyName("id")>]
        Id: int
        
        [<JsonPropertyName("name")>]
        Name: string
    }
    
    and SetEffect = {
        [<JsonPropertyName("display_string")>]
        DisplayString: string
        
        [<JsonPropertyName("required_count")>]
        RequiredCount: int
    }

    let get (httpClient: Beforeguard.Frostline.Core.BattleNetHttpClient) 
            (region: Beforeguard.Frostline.Core.Region) 
            (realm: string) 
            (characterName: string) =
        async {
            let normalizedRealm = realm.ToLower().Replace(" ", "-")
            let normalizedName = characterName.ToLower()
            let regionStr = Beforeguard.Frostline.Core.Region.toString region
            let path = sprintf "/profile/wow/character/%s/%s/equipment?namespace=profile-%s&locale=en_US" 
                            normalizedRealm normalizedName regionStr
            
            let! result = httpClient.getAsync<CharacterEquipment>(path) |> Async.AwaitTask
            return result
        }