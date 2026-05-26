# Adding Content Guide

This guide walks through creating rooms, items, and story progression.

---

## Validation & Setup

**Important:** This project uses Unity's idiomatic validation patterns. Setup errors are caught in the Editor, not at runtime.

### Before You Add Content

1. **Scene Needs GameManager**
   - Load the main Game scene (has GameManager prefab)
   - Or create a GameManager GameObject with these components:
     - GameState
     - SaveSystem
     - Inventory
   
2. **Assign Required Fields in Inspector**
   - OnValidate() warns immediately if something is missing
   - Fix warnings before hitting Play

3. **Use the "Interactable" Layer**
   - Select interactable object → Layer dropdown → Interactable
   - Required for raycasting to detect clicks

### Validation Will Warn You If:

| Component | Missing | Warning Message |
|-----------|---------|-----------------|
| PickupItem | inventoryItemTemplate | "[PickupItem] No inventory item template assigned!" |
| Door | targetSceneName | "[Door] Target scene name not assigned!" |
| Interactable | interactableId | "[Interactable] No interactableId assigned" |
| GameManager | GameState/SaveSystem/Inventory | RequireComponent enforced |

**What to do:** Fix the warning by assigning the missing field in the Inspector.

---

## Creating Items

### Step 1: Create ScriptableObject

In Project window:
1. Navigate to `Assets/ScriptableObjects/Items/`
2. Right-click → Create → Inventory → Item
3. Name the asset (e.g., `GoldenKey.asset`)

### Step 2: Configure Item

Select the asset, configure in Inspector:

```
Item ID: key_golden
Item Name: Golden Key
Description: An ornate key with intricate designs.
Icon: (optional texture)
Is Stackable: false
Max Stack Size: 1
```

### Step 3: Use in Scene

Create a GameObject with `PickupItem.cs`:
```
GoldenKey (GameObject)
├── PickupItem.cs (script)
│   └── Inventory Item Template: GoldenKey (drag asset)
├── Collider (Box, Is Trigger)
├── Mesh (Cube, visual)
└── Material (yellow glow for visibility)
```

---

## Creating Rooms

### Step 1: New Scene

1. File → New Scene
2. Save as `Assets/Scenes/Room_LibraryWing.unity`
3. Delete default Camera and Light (use GameManager's)

### Step 2: Build Room Layout

Create hierarchy:

```
Room_LibraryWing
├── Environment
│   ├── Floor (Plane, scale 10x10)
│   │   └── Material: wood_placeholder
│   ├── Walls (4 Cubes, positioned at edges)
│   │   └── Material: stone_placeholder
│   ├── Ceiling (Plane, scale 10x10, Y=3)
│   │   └── Material: ceiling_placeholder
│   └── Furniture
│       ├── Bookshelf (Cube, scale 1x2x0.5)
│       ├── Table (Cube, scale 2x1x1)
│       └── Chair (Cube, scale 0.5x1x0.5)
├── Interactables
│   ├── BookshelfExamine (Cube, small, on bookshelf)
│   │   ├── ExaminableObject.cs
│   │   ├── displayName: "Ancient Shelf"
│   │   ├── description: "Shelves filled with dusty tomes..."
│   │   └── Collider (trigger)
│   ├── Letter (Cube, small, glowing)
│   │   ├── PickupItem.cs
│   │   ├── inventoryItemTemplate: Letter asset
│   │   └── Collider (trigger)
│   └── Door_toFoyer (Cube)
│       ├── Door.cs
│       ├── targetSceneName: "Foyer"
│       ├── spawnPosition: (0, 0, 5)
│       └── Collider (trigger)
└── Spawn Point (Empty GameObject)
    └── Position: (0, 0, 0)  # Where player appears
```

### Step 3: Configure Colliders

For each interactable:
1. Add BoxCollider component
2. Enable "Is Trigger"
3. Adjust size to match object
4. Assign to "Interactable" layer

```csharp
// Quick script to set layer:
void Start() {
    gameObject.layer = LayerMask.NameToLayer("Interactable");
}
```

### Step 4: Connect Doors

On each Door GameObject:
```
Door_toLibrary (Door.cs)
├── targetSceneName: "Room_Library"
├── spawnPosition: (2, 0, 3)  # Where player exits
├── requiresItem: true
├── requiredItem: GoldenKey asset
```

### Step 5: Set RoomManager Spawn

In Room scene, create empty GameObject `SpawnPoint`:
```
RoomManager
├── playerSpawnPoint: drag SpawnPoint GameObject here
```

Or use default spawn position in RoomManager component.

---

## Creating Story Progression

### Using Flags

Track narrative state with binary flags:

```csharp
// When player discovers secret
GameState state = GameManager.Instance.GetGameState();
state.SetFlag("secretLibraryFound", true);

// Later, show different content
if (state.GetFlag("secretLibraryFound")) {
    // Door opens, NPC greets differently, etc
}
```

### Conditional Interactables

Create an interactable that checks story state:

```csharp
public class SecretDoor : Door {
    public override void OnInteract() {
        GameState state = GameManager.Instance.GetGameState();
        
        if (state.GetFlag("priestessAppeased")) {
            base.OnInteract();  // Load scene
        } else {
            DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
            panel.ShowDescription("Sealed", 
                "An ancient seal blocks passage. Only one blessed by the Priestess may enter.");
        }
    }
}
```

### Item-Based Gates

```csharp
public class LockedChest : ExaminableObject {
    public InventoryItem requiredKey;
    
    public override void OnInteract() {
        Inventory inv = GameManager.Instance.GetInventory();
        
        if (inv.HasItem(requiredKey.itemId)) {
            // Show treasure
            DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
            panel.ShowDescription("Golden Treasure", 
                "Mountains of gold and jewels! You're rich!");
            inv.RemoveItem(requiredKey.itemId);  // Consume key
        } else {
            base.OnInteract();  // Show locked description
        }
    }
}
```

---

## Creating Custom Interactables

### Example: Lever

```csharp
public class Lever : ExaminableObject {
    public string flagToSet;
    public string descriptionWhenPulled = "The lever is now in the down position.";
    
    public override void OnInteract() {
        GameState state = GameManager.Instance.GetGameState();
        
        if (!state.GetFlag(flagToSet)) {
            state.SetFlag(flagToSet, true);
            
            // Show success description
            DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
            panel.ShowDescription("Lever Pulled", 
                "You pull the ancient lever. Gears grind somewhere beneath the floor...");
            
            // Maybe unlock something
            // audioSource.PlayOneShot(leverSound);
        } else {
            base.OnInteract();
        }
    }
}
```

### Example: Narrative Object

```csharp
public class NarrativeObject : ExaminableObject {
    public string requiredFlag;
    public string unmetDescription;
    
    public override void OnInteract() {
        GameState state = GameManager.Instance.GetGameState();
        
        if (requiredFlag != "" && !state.GetFlag(requiredFlag)) {
            // Show alternate description
            DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
            panel.ShowDescription(displayName, unmetDescription);
        } else {
            base.OnInteract();
        }
    }
}
```

---

## Quest/Mystery Progression Template

### 1. Define Mystery States

```csharp
// In GameState or custom manager
private static class MysteryFlags {
    public const string FOUND_LETTER = "mystery_foundLetter";
    public const string READ_LETTER = "mystery_readLetter";
    public const string FOUND_LOCKET = "mystery_foundLocket";
    public const string CONFRONTED_BUTLER = "mystery_confrontedButler";
    public const string MYSTERY_SOLVED = "mystery_solved";
}
```

### 2. Create Objects That Progress Mystery

```csharp
public class MysteryLetter : PickupItem {
    public override void OnInteract() {
        base.OnInteract();  // Add to inventory
        
        // Set flag so other objects behave differently
        GameManager.Instance.GetGameState().SetFlag(
            MysteryFlags.FOUND_LETTER, true);
    }
}

public class Librarian : ExaminableObject {
    public override void OnInteract() {
        GameState state = GameManager.Instance.GetGameState();
        
        string dialogue = "Good morning.";
        
        if (state.GetFlag(MysteryFlags.FOUND_LETTER)) {
            dialogue = "Ah, you found that, did you? I was wondering when someone would...";
        }
        
        if (state.GetFlag(MysteryFlags.CONFRONTED_BUTLER)) {
            dialogue = "So you know the truth. The household will never be the same.";
        }
        
        DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
        panel.ShowDescription("Librarian", dialogue);
    }
}
```

### 3. Multi-Step Puzzles

```csharp
public class AncientDoor : Door {
    public string[] requiredItems = { "key_gold", "lens_crystal", "seal_bronze" };
    
    public override void OnInteract() {
        Inventory inv = GameManager.Instance.GetInventory();
        List<bool> hasItems = new List<bool>();
        
        foreach (string itemId in requiredItems) {
            hasItems.Add(inv.HasItem(itemId));
        }
        
        if (hasItems.Contains(false)) {
            int needed = requiredItems.Length - hasItems.FindAll(b => b).Count;
            DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
            panel.ShowDescription("Ancient Seal", 
                $"The seal requires {needed} more items before it will open.");
        } else {
            // All items present
            GameManager.Instance.GetGameState().SetFlag("finalDoorOpened", true);
            base.OnInteract();
        }
    }
}
```

---

## Room Template (Copy & Paste)

Create a new room with this structure:

```csharp
// Save as Assets/Scripts/Rooms/Room_TemplateSetup.cs
using UnityEngine;

public class Room_TemplateSetup : MonoBehaviour {
    private void Start() {
        // Called when scene loads
        // Use for initial setup, dialogue, etc
        Debug.Log("Room loaded!");
    }
}
```

Then add to scene as a component on empty GameObject.

---

## Testing Checklist

When adding a new room, test:

- [ ] Player spawns in correct location
- [ ] WASD movement works, no clipping through walls
- [ ] Camera angle looks good (isometric perspective correct)
- [ ] All interactables are clickable
- [ ] Doors transition to correct rooms
- [ ] Items can be picked up
- [ ] Descriptions display correctly
- [ ] Save/load preserves room state
- [ ] No errors in Console

### Quick Test Script

```csharp
// Add to new room for debugging
public class RoomDebug : MonoBehaviour {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Debug.Log("=== Room Debug Info ===");
            Debug.Log("Room: " + 
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Debug.Log("Player Position: " + 
                FindObjectOfType<PlayerController>().GetPosition());
            Debug.Log("Inventory Count: " + 
                GameManager.Instance.GetInventory().GetSlotCount());
        }
    }
}
```

---

## Performance Tips

### For Large Rooms

- Use LOD groups for distant objects
- Limit active interactables (cull non-visible)
- Batch similar materials

### For Many Items

- Use object pooling for common pickups
- Lazy-load descriptions (load from text files)
- Optimize textures (compress in import settings)

### Save File Size

- Compress save files (gzip)
- Only save changed state (deltas)
- Archive old saves periodically

---

## Example: Complete Mystery Room

```csharp
// Assets/Scenes/Room_Study.unity hierarchy:

Study
├── Environment
│   ├── Floor
│   ├── Walls (4)
│   ├── Ceiling
│   └── Furniture
│       ├── Desk
│       ├── Bookshelf
│       └── Fireplace
├── Interactables
│   ├── SketchOnWall
│   │   └── ExaminableObject
│   │       display: "Hastily drawn sketch"
│   │       desc: "A portrait of someone... scratched out in anger?"
│   ├── LockedBox
│   │   └── CustomScript: LockedBox.cs
│   │       requiredItem: GoldenKey
│   │       unlockedDesc: "Inside you find letters... a confession!"
│   ├── Letter_Confession
│   │   └── PickupItem
│   │       Sets flag: "found_confession"
│   ├── Desk
│   │   └── ExaminableObject
│   │       desc: "The desk is in disarray. Someone left in a hurry."
│   └── Door_toLibrary
│       └── Door
│           targetScene: "Room_Library"
│           spawn: (2, 0, 3)
└── SpawnPoint
    └── (0, 0, 0)
```

This room:
1. Requires key to progress (from previous room)
2. Reveals story through examination
3. Contains pickupable evidence
4. Sets flags that change dialogue elsewhere
5. Leads to next area

---

## Next Room Ideas

- **Kitchen**: Find poisoned wine (item collection)
- **Garden**: Hidden passage (flag gate)
- **Portrait Gallery**: Multiple NPCs (examination)
- **Secret Basement**: Final revelation (multi-item puzzle)
- **Attic**: Clues to mystery (story progression)

