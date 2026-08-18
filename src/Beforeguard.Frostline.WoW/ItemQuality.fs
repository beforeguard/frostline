namespace Beforeguard.Frostline.WoW

/// Represents an RGB color value
type QualityColor = {
    R: int
    G: int
    B: int
}

/// Item quality utilities and color mappings
module ItemQuality =
    
    /// Get the RGB color for a given item quality type
    let getColor (qualityType: string) : QualityColor =
        match qualityType with
        | "POOR" -> { R = 157; G = 157; B = 157 }      // Gray
        | "COMMON" -> { R = 255; G = 255; B = 255 }    // White
        | "UNCOMMON" -> { R = 30; G = 255; B = 0 }     // Green
        | "RARE" -> { R = 0; G = 112; B = 221 }        // Blue
        | "EPIC" -> { R = 163; G = 53; B = 238 }       // Purple
        | "LEGENDARY" -> { R = 255; G = 128; B = 0 }   // Orange
        | "ARTIFACT" -> { R = 229; G = 204; B = 127 }  // Gold
        | "HEIRLOOM" -> { R = 0; G = 204; B = 255 }    // Light Blue
        | _ -> { R = 255; G = 255; B = 255 }           // Default to white
