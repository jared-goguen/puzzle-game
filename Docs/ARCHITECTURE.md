# Architecture Documentation

## System Overview

This game is built on a modular, event-driven architecture. Each system is loosely coupled and communicates through the central `GameManager` singleton.

```
┌─────────────────────────────────────────────────────┐
│                    GameManager                       │
│                   (Singleton)                        │
├─────────────────────────────────────────────────────┤
│ Coordinates:                                         │
│  - GameState (persistent data)                      │
│  - SaveSystem (serialization)                       │
│  - PlayerController (input/movement)                │
│  - RoomManager (scene transitions)                  │
│  - Inventory (item collection)                      │
│  - UI Systems (Canvas, panels)                      │
└─────────────────────────────────────────────────────┘
```

---

## Core Systems

### 1. GameManager

**File**: `Assets/Scripts/Core/GameManager.cs`

**Purpose**: Central hub for all game systems.

**Key Methods**:
- `LoadGame(slot)` - Load save file
- `SaveGame(slot)` - Save game state
- `NewGame()` - Reset and start new game
- `GetGameState()` - Access game state
- `GetInventory()` - Access inventory
- `GetRoomManager()` - Access room manager

**Pattern**: Singleton with `DontDestroyOnLoad()` for persistence across scenes.

```csharp
// Usage anywhere:
GameManager.Instance.SaveGame(0);
GameState state = GameManager.Instance.GetGameState();
```

---

### 2. GameState

**File**: `Assets/Scripts/Core/GameState.cs`

**Purpose**: Tracks all game state data (flags, variables, inventory status, player position).

**Structure**:
```csharp
StateData {
    Vector3 playerPosition
    List<string> pickedUpItems
    Dictionary<string, bool> storyFlags
    Dictionary<string, int> variables
}
```

**Key Methods**:
- `SetFlag(name, value)` / `GetFlag(name)` - Story progression tracking
- `SetVariable(name, value)` / `GetVariable(name)` - Numeric state
- `PickupItem(itemId)` / `HasPickedUpItem(itemId)` - Item tracking
- `SetPlayerPosition()` / `GetPlayerPosition()` - Checkpoint system

**Example**:
```csharp
GameState state = GameManager.Instance.GetGameState();
state.SetFlag("librarianGreeted", true);
if (state.GetFlag("librarianGreeted")) {
    // Show different dialogue
}
```

---

### 3. SaveSystem

**File**: `Assets/Scripts/Core/SaveSystem.cs`

**Purpose**: Serializes/deserializes game state to JSON files.

**Save Format**:
```json
{
    "timestamp": "2024-01-15 14:30:00",
    "gameState": {
        "playerPosition": {...},
        "pickedUpItems": ["key_golden", "letter_old"],
        "storyFlags": {"librarianGreeted": true}
    },
    "inventoryItems": [...]
}
```

**Storage**: `Application.persistentDataPath/Saves/save_X.json`

**Key Methods**:
- `SaveGame(slot)` - Serialize current state
- `LoadGame(slot)` - Deserialize and restore state
- `SaveExists(slot)` - Check if save exists

---

### 4. PlayerController

**File**: `Assets/Scripts/Player/PlayerController.cs`

**Purpose**: Handles player movement with WASD, rotation, and collision.

**Input**:
- `W` / `S` / `A` / `D` - Move forward/backward/left/right
- Movement normalized for diagonal consistency

**Physics**:
- Uses `CharacterController` component for collision
- Applies gravity automatically
- Smooth rotation toward movement direction

**Key Methods**:
- `HandleInput()` - Parse keyboard input
- `HandleMovement()` - Apply movement and gravity
- `Teleport(position)` - Room transition spawning

**Configuration**:
- `moveSpeed` - Units per second
- `rotationSpeed` - Degrees per second

---

### 5. IsometricCamera

**File**: `Assets/Scripts/Player/IsometricCamera.cs`

**Purpose**: Fixed isometric camera that follows player.

**Angles**:
- Rotation X: 30-35° (pitch down)
- Rotation Y: 45° (diagonal view)
- Creates classic isometric perspective

**Modes**:
- **Orthographic**: True isometric (parallel projection)
- **Perspective**: Slight depth perception

**Key Methods**:
- `SetZoom(size)` - Adjust camera size
- `SetOrthographic(bool)` - Toggle projection mode

**Calculation**:
```csharp
offset = Quaternion.Euler(30, 45, 0) * (Vector3.back * distance + Vector3.up * height);
position = target.position + offset;
```

---

### 6. Interactable System

**Base Class**: `Assets/Scripts/Interaction/Interactable.cs`

All clickable objects inherit from `Interactable`.

```csharp
public abstract class Interactable : MonoBehaviour {
    public abstract void OnInteract();  // Called on click
    public virtual void Highlight();    // Called on hover
    public virtual void Unhighlight();  // Called on leave
}
```

#### 6a. PickupItem

**Purpose**: Items that can be collected into inventory.

```csharp
public class PickupItem : Interactable {
    public InventoryItem inventoryItemTemplate;
    public bool destroyOnPickup = true;
}
```

**Behavior**:
1. Player clicks item
2. `OnInteract()` called
3. Item added to Inventory
4. GameObject destroyed (optional)
5. GameState updated

#### 6b. ExaminableObject

**Purpose**: Objects that show descriptions when clicked.

```csharp
public class ExaminableObject : Interactable {
    // Shows in DescriptionPanel
}
```

**Behavior**:
1. Player clicks object
2. `OnInteract()` called
3. Description displayed in UI
4. Object remains in world

#### 6c. Door

**Purpose**: Room transitions.

```csharp
public class Door : Interactable {
    public string targetSceneName;
    public Vector3 spawnPosition;
    public InventoryItem requiredItem;  // Optional lock
}
```

**Behavior**:
1. Player clicks door
2. Check if locked (requires item)
3. Load target scene
4. Spawn player at spawn point

---

### 7. Inventory System

**Files**:
- `InventoryItem.cs` - ScriptableObject definition
- `Inventory.cs` - Collection logic
- `InventoryUI.cs` - UI display
- `InventorySlot.cs` - Individual slot component

**Flow**:

```
PickupItem.OnInteract()
    ↓
Inventory.AddItem(template)
    ↓
Create InventoryItemInstance
    ↓
InventoryUI.RefreshDisplay()
    ↓
Create UI slots from instances
```

**Data Structures**:

```csharp
// Definition (ScriptableObject)
InventoryItem {
    string itemId
    string itemName
    string description
    Texture2D icon
    bool isStackable
    int maxStackSize
}

// Runtime instance
InventoryItemInstance {
    string itemId
    string itemName
    string description
    int quantity
}
```

**Key Methods**:
```csharp
Inventory inv = GameManager.Instance.GetInventory();
inv.AddItem(itemTemplate);
inv.RemoveItem("key_golden");
inv.HasItem("key_golden");
inv.GetAllItems();
```

---

### 8. UI Systems

#### DescriptionPanel

**File**: `Assets/Scripts/UI/DescriptionPanel.cs`

**Purpose**: Display text descriptions from examined objects.

**Features**:
- Fade in/out animations
- Auto-close after duration (optional)
- Close button

**Usage**:
```csharp
DescriptionPanel panel = FindObjectOfType<DescriptionPanel>();
panel.ShowDescription("Dusty Painting", "A portrait of someone important...");
```

#### InventoryUI

**File**: `Assets/Scripts/UI/InventoryUI.cs`

**Purpose**: Visual grid display of inventory items.

**Controls**:
- Press `I` to toggle inventory panel
- Drag items (basic support)

**Integration**:
- Listens to `Inventory.AddItem()` changes
- Calls `RefreshDisplay()` to rebuild UI
- Instantiates `InventorySlot` prefabs

#### MainMenuUI

**File**: `Assets/Scripts/UI/MainMenuUI.cs`

**Purpose**: Main menu navigation.

**Screens**:
- Main Menu (New/Load/Settings/Quit)
- Load Game (5 save slots)

#### CursorManager

**File**: `Assets/Scripts/UI/CursorManager.cs`

**Purpose**: Interaction detection via raycasting.

**Process**:
```
Every frame:
    1. Cast ray from camera through mouse position
    2. Check for Interactable in hit collider
    3. If hit: highlight + change cursor
    4. If click: call OnInteract()
```

**Configuration**:
- `interactableLayer` - What layer to raycast against
- `raycastDistance` - How far to raycast

---

### 9. RoomManager

**File**: `Assets/Scripts/Interaction/RoomManager.cs`

**Purpose**: Scene transitions and player spawning.

**Key Methods**:
- `SpawnPlayerInRoom()` - Place player at spawn point
- `SetSpawnPoint(position)` - Set next spawn location
- `LoadRoom(roomName)` - Load new scene
- `GetCurrentRoom()` - Get current scene name

**Integration**:
- Door.cs calls `SceneManager.LoadScene()`
- RoomManager.SpawnPlayerInRoom() called at scene start
- PlayerController.Teleport() positions player

---

## Data Flow Examples

### Example 1: Picking Up an Item

```
Player clicks on GoldenKey
    ↓
CursorManager raycasts, finds PickupItem
    ↓
CursorManager calls PickupItem.OnInteract()
    ↓
PickupItem loads InventoryItem template
    ↓
PickupItem calls Inventory.AddItem(template)
    ↓
Inventory creates InventoryItemInstance
    ↓
Inventory calls InventoryUI.RefreshDisplay()
    ↓
InventoryUI rebuilds UI grid with slots
    ↓
PickupItem destroys itself from world
    ↓
GameState.PickupItem() marks it as collected
```

### Example 2: Examining an Object

```
Player clicks on Painting
    ↓
CursorManager raycasts, finds ExaminableObject
    ↓
CursorManager calls ExaminableObject.OnInteract()
    ↓
ExaminableObject.OnInteract() calls DescriptionPanel.ShowDescription()
    ↓
DescriptionPanel fades in with title + description
    ↓
Painting remains in world, interactable again
```

### Example 3: Entering a Door

```
Player clicks on Door_toLibrary
    ↓
CursorManager raycasts, finds Door
    ↓
CursorManager calls Door.OnInteract()
    ↓
Door checks if locked (requires item)
    ↓
Door saves current position to GameState
    ↓
Door calls SceneManager.LoadScene("Library")
    ↓
Scene loads, RoomManager.Start() called
    ↓
RoomManager.SpawnPlayerInRoom() positions player
    ↓
IsometricCamera follows player in new room
```

### Example 4: Saving Game

```
Player presses save button (or auto-save triggers)
    ↓
GameManager.SaveGame(0)
    ↓
SaveSystem collects data:
    - GameState.GetStateData()
    - Inventory.GetAllItems()
    - Timestamp
    ↓
SaveSystem creates SaveData object
    ↓
JsonUtility.ToJson() serializes
    ↓
File.WriteAllText() writes to disk
    ↓
Saves to: persistentDataPath/Saves/save_0.json
```

---

## Extension Points

### Adding a New Interactable Type

1. Create class inheriting from `Interactable`
2. Implement `OnInteract()` method
3. Attach to GameObject
4. Assign to "Interactable" layer
5. Add Collider (can be trigger)

```csharp
public class Lever : Interactable {
    public override void OnInteract() {
        // Toggle a door, trigger animation, etc
        GameManager.Instance.GetGameState().SetFlag("leverPulled", true);
    }
}
```

### Adding a New Story Flag

Flags persist across saves and can gate content:

```csharp
// Set flag when story event occurs
if (player.HasItem("letter")) {
    GameManager.Instance.GetGameState().SetFlag("foundLetter", true);
}

// Check flag elsewhere
if (GameManager.Instance.GetGameState().GetFlag("foundLetter")) {
    // Show hidden door / new dialogue / etc
}
```

### Adding Conditional Interactables

```csharp
public class LockedDoor : Door {
    public override void OnInteract() {
        GameState state = GameManager.Instance.GetGameState();
        if (state.GetFlag("priestessAppeased")) {
            base.OnInteract();  // Allow entry
        } else {
            DescriptionPanel.ShowDescription("Sealed", "An ancient seal blocks the way.");
        }
    }
}
```

---

## Performance Considerations

### Raycasting Optimization

- Raycasting happens **every frame** in CursorManager
- Limit raycast distance to visible area
- Use layer masks to exclude non-interactables
- Consider spatial partitioning if 100+ interactables

### Save File Size

- Save files are JSON (human-readable, ~1-10 KB typical)
- Stored in Application.persistentDataPath
- Can be backed up / synced easily

### Inventory UI

- `RefreshDisplay()` clears and rebuilds all slots
- Called on every inventory change
- For large inventories (100+ items), consider pooling

---

## Validation & Setup

This project uses **idiomatic Unity validation patterns** to prevent null reference errors at edit-time, not runtime.

### RequireComponent Attributes

Critical systems use `[RequireComponent]` to enforce setup:

```csharp
[RequireComponent(typeof(GameState))]
[RequireComponent(typeof(SaveSystem))]
[RequireComponent(typeof(Inventory))]
public class GameManager : MonoBehaviour { }
```

**What this means:**
- These components MUST exist on the same GameObject
- Unity will warn if they're missing
- Prevents runtime crashes

### OnValidate() Pattern

All components with serialized dependencies use `OnValidate()` to warn at edit-time:

```csharp
private void OnValidate()
{
    if (inventoryItemTemplate == null)
        Debug.LogWarning("[PickupItem] No inventory item template assigned!", this);
}
```

**When it runs:**
- In the Editor only (not at runtime)
- When you assign/change values in the Inspector
- When you load the scene in the Editor

**What to do:**
- Fix warnings immediately in the Inspector
- Each warning tells you exactly what to assign

### Assert.IsNotNull() Pattern

Critical checks at runtime (development builds only):

```csharp
private void Start()
{
    Assert.IsNotNull(inventoryItemTemplate, 
        "[PickupItem] Item template required!", this);
}
```

**Development Builds Only:**
- Assertions only work in Editor and Development Builds
- Disabled in Release builds for performance
- Catches missing setup early during testing

### Setup Checklist

Before testing a scene:

1. **GameManager Prefab**
   - [ ] Has GameState component
   - [ ] Has SaveSystem component
   - [ ] Has Inventory component
   - [ ] RoomManager assigned (if level transitions)
   - [ ] PlayerController assigned (if needed)

2. **Interactable Objects**
   - [ ] Assigned unique `interactableId`
   - [ ] Have `displayName` and `description`
   - [ ] Have `Renderer` component for highlighting

3. **Doors**
   - [ ] `targetSceneName` assigned (matches a scene name)
   - [ ] Spawn position configured if needed

4. **Pickup Items**
   - [ ] `inventoryItemTemplate` assigned (ScriptableObject)
   - [ ] `interactableId` assigned

5. **Room Setup**
   - [ ] PlayerController spawned in scene
   - [ ] RoomManager placed with spawn point set
   - [ ] All interactables on "Interactable" layer

---

## Best Practices

1. **Always access systems through GameManager**
   ```csharp
   // ✓ Good
   GameManager.Instance.GetGameState().SetFlag(...);
   
   // ✗ Bad
   FindObjectOfType<GameState>().SetFlag(...);
   ```

2. **Use Flags for Story Progression**
   ```csharp
   state.SetFlag("act1_complete", true);
   if (state.GetFlag("act1_complete")) { ... }
   ```

3. **Mark All Interactables on "Interactable" Layer**
   - Required for raycasting to work

4. **Test Save/Load Frequently**
   - Ensure all important state is captured
   - Check serialization works correctly

5. **Use Prefabs for Reusable Objects**
   - Common furniture pieces
   - Standard door templates
   - Pickup item variants

