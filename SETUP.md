# Puzzle Game - 3D Isometric Setup Guide

## Project Overview

A modular puzzle game framework with isometric perspective featuring point-and-click interaction, inventory system, and room-based exploration. Supports narrative puzzles, mysteries, and interactive environments.

### Key Features
- **3D Isometric Camera**: Fixed 45° angle fixed perspective
- **WASD Movement**: Player navigation with smooth rotation
- **Point-and-Click Interaction**: Click objects to examine/pickup items
- **Inventory System**: Visual inventory grid with event-driven updates
- **Save/Load System**: Multiple save slots with JSON serialization
- **Modular Architecture**: Scene-based room system for easy expansion
- **Validation Framework**: Edit-time warnings, runtime assertions

---

## Project Structure

```
puzzle-game/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs      # Main singleton coordinator
│   │   │   ├── GameState.cs        # Persistent game state
│   │   │   └── SaveSystem.cs       # Save/load with JSON
│   │   ├── Player/
│   │   │   ├── PlayerController.cs # WASD movement + collision
│   │   │   └── IsometricCamera.cs  # Fixed isometric camera
│   │   ├── Interaction/
│   │   │   ├── Interactable.cs     # Base class for clickables
│   │   │   ├── PickupItem.cs       # Items you can collect
│   │   │   ├── ExaminableObject.cs # Items you can examine
│   │   │   ├── Door.cs             # Room transitions
│   │   │   └── RoomManager.cs      # Scene/room management
│   │   ├── Inventory/
│   │   │   ├── InventoryItem.cs    # Item ScriptableObject
│   │   │   ├── Inventory.cs        # Item collection logic
│   │   │   ├── InventoryUI.cs      # Inventory grid display
│   │   │   └── InventorySlot.cs    # Individual slot with drag-drop
│   │   └── UI/
│   │       ├── DescriptionPanel.cs # Text display for examined items
│   │       ├── MainMenuUI.cs       # Main menu, load game
│   │       └── CursorManager.cs    # Interaction raycasting
│   ├── ScriptableObjects/
│   │   └── Items/                  # Item definitions (created via Asset menu)
│   ├── Prefabs/
│   │   ├── Player/                 # Player prefab
│   │   └── Interactables/          # Reusable interactable objects
│   ├── Scenes/
│   │   ├── MainMenu.unity          # Main menu scene
│   │   ├── Game.unity              # Master game scene
│   │   ├── Room_Foyer.unity        # Foyer room
│   │   ├── Room_Library.unity      # Library room
│   │   ├── Room_Study.unity        # Study room
│   │   └── Room_Kitchen.unity      # Kitchen room
│   └── Art/
│       └── Placeholder/            # Simple colored materials
└── Docs/
    ├── ARCHITECTURE.md             # Detailed system design
    └── ADDING_CONTENT.md           # How to add rooms, items, etc.
```

---

## Setup Instructions

### 1. Create the Unity Project

```bash
# Navigate to the puzzle-game directory
cd /home/jared/source/puzzle-game

# Create a new 3D Unity project (via Unity Hub or command line)
# Recommended: Unity 2022 LTS or newer
# Create: Assets folder, Packages folder, ProjectSettings folder
```

### 2. Import the Scaffolding

All C# scripts are already in `Assets/Scripts/`. Unity will auto-compile them.

### 3. Create Core GameObjects in Main Scene

The `Game.unity` scene should contain:

#### Scene Hierarchy:
```
Game (Scene)
├── GameManager (GameObject)
│   ├── GameManager.cs (script)
│   ├── GameState.cs (script)
│   └── SaveSystem.cs (script)
├── Player (GameObject)
│   ├── PlayerController.cs
│   ├── CharacterController (component)
│   ├── Capsule (visual, child)
│   └── CursorManager.cs
├── Camera
│   ├── Camera (component)
│   └── IsometricCamera.cs (script)
├── RoomManager (GameObject)
│   └── RoomManager.cs (script)
├── Canvas (UI)
│   ├── MainMenuUI.cs
│   ├── DescriptionPanel
│   │   ├── DescriptionPanel.cs
│   │   ├── Panel image
│   │   ├── Title text
│   │   ├── Description text
│   │   └── Close button
│   └── InventoryUI
│       ├── InventoryUI.cs
│       ├── Panel
│       │   ├── Grid layout
│       │   └── Inventory slots (instances of prefab)
│       ├── Toggle (to open/close)
│       └── InventorySlot.cs (on each slot)
├── Rooms (Empty GameObject - organizer)
│   ├── Foyer (Room scene additive loaded or single scene)
│   ├── Library (Room scene)
│   └── [etc]
```

### 4. Create Sample Items (ScriptableObjects)

Right-click in `Assets/ScriptableObjects/Items/`:
- Create → Inventory → Item
- Name: "GoldenKey"
- Set fields:
  - Item ID: `key_golden`
  - Item Name: `Golden Key`
  - Description: `An ornate key with intricate designs.`
  - Is Stackable: false

Repeat for other items as needed.

### 5. Create First Room (Foyer)

**Foyer.unity Scene Setup:**

```
Foyer (Scene)
├── Floor (Plane, scaled)
│   └── Material: placeholder brown
├── Walls (Cubes, positioned)
│   └── Material: placeholder gray
├── Door_toLibrary (Cube)
│   ├── Door.cs (script)
│   ├── Collider (Box, trigger)
│   ├── targetSceneName: "Library"
│   └── spawnPosition: (defined exit point)
├── Painting (Cube, small)
│   ├── ExaminableObject.cs
│   ├── displayName: "Dusty Portrait"
│   ├── description: "A portrait of someone important, now forgotten by time."
│   └── Collider (Box, trigger)
├── Table (Cube)
│   ├── ExaminableObject.cs
│   ├── displayName: "Oak Table"
│   ├── description: "A sturdy oak table. Nothing remarkable."
│   └── Collider
└── GoldenKey (Cube, small, glowing)
    ├── PickupItem.cs
    ├── inventoryItemTemplate: (drag GoldenKey ScriptableObject here)
    ├── Collider (trigger)
    └── destroyOnPickup: true
```

### 6. Set Up Layers for Raycasting

1. Create a new Layer: **"Interactable"**
2. Assign all interactable objects to this layer
3. In CursorManager, set `interactableLayer` to "Interactable"

### 7. Camera Configuration

In the **Game** scene Camera's IsometricCamera.cs:
- Rotation X: `30` (pitch down)
- Rotation Y: `45` (diagonal angle)
- Distance: `8`
- Height: `6`
- Use Orthographic: `true`
- Orthographic Size: `5`

### 8. Configure GameManager

In GameManager.cs, drag-and-drop references:
- `gameState`: The GameState component
- `saveSystem`: The SaveSystem component
- `roomManager`: The RoomManager component
- `playerController`: The PlayerController component
- `inventory`: The Inventory component

### 9. Validation & Editor Setup

This project uses **idiomatic Unity validation** to catch setup errors in the Editor before runtime.

**What happens automatically:**
- `OnValidate()` checks run whenever you change Inspector values
- Missing required fields show **yellow warnings** in the Console
- `[RequireComponent]` ensures critical components exist
- `Assert.IsNotNull()` catches problems in development builds

**Before hitting Play:**
1. Fix any yellow warnings in the Console
2. Each warning tells you exactly what field to assign
3. Assign the missing field in the Inspector
4. Warning disappears automatically

**Common Warnings & How to Fix:**

| Warning | Fix |
|---------|-----|
| "[PickupItem] No inventory item template assigned!" | Select PickupItem → Drag InventoryItem ScriptableObject to `inventoryItemTemplate` |
| "[Door] Target scene name not assigned!" | Select Door → Type scene name in `targetSceneName` field (must match exactly) |
| "[Interactable] No interactableId assigned" | Select Interactable → Type unique ID in `interactableId` field |
| "[GameManager] GameState component is required!" | GameManager GameObject must have GameState component attached |

**Pro Tip:** Use the Inspector's lock icon to prevent accidental deselection while fixing warnings.

---

## Quick Start Workflow

1. **Load the Game scene** in Unity Editor
2. **Press Play**
3. **WASD** to move, **Mouse** to click objects
4. **I** key to toggle inventory
5. **Click** objects to interact (examine/pickup)

---

## Common Tasks

### Add a New Room
1. Create new scene: `Assets/Scenes/Room_NewRoom.unity`
2. Add floor, walls, interactables
3. Create a Door pointing to this scene
4. Add RoomManager to the scene

### Add a New Item
1. Create ScriptableObject in `Assets/ScriptableObjects/Items/`
2. Fill in name, description, icon (optional)
3. Drag onto PickupItem.cs's `inventoryItemTemplate` field

### Add Story Flag
```csharp
// In any script with access to GameState:
GameState state = GameManager.Instance.GetGameState();
state.SetFlag("foundAncientLetter", true);

// Check later:
if (state.GetFlag("foundAncientLetter")) {
    // Show different dialogue/options
}
```

### Save/Load
```csharp
// Save to slot 0
GameManager.Instance.SaveGame(0);

// Load from slot 0
GameManager.Instance.LoadGame(0);
```

---

## Architecture Notes

### Singleton Pattern
- **GameManager** is a Singleton (one instance across scenes)
- Use `GameManager.Instance` to access all systems

### Scene Management
- **MainMenu.unity**: Entry point
- **Game.unity**: Master game scene (contains UI, managers)
- **Room_*.unity**: Individual room scenes (can be additive or single-load)

### Component Communication
```csharp
// Get inventory
Inventory inv = GameManager.Instance.GetInventory();

// Get game state
GameState state = GameManager.Instance.GetGameState();

// Interact with systems through GameManager
```

---

## Next Steps

1. **Build the Foyer** with placeholder art
2. **Create 2-3 rooms** with basic interactables
3. **Playtest movement & interaction**
4. **Iterate on visuals** (replace placeholder cubes with real art)
5. **Add story progression** via flags and narrative objects

---

## Common Issues

**Scripts not compiling?**
- Ensure all scripts are in `Assets/Scripts/` (compiler needs time to run)
- Check for duplicate class names
- Verify using statements are correct

**Interactables not clickable?**
- Ensure they have a Collider (not marked as Trigger)
- Actually, for raycasting they SHOULD be triggers OR non-trigger with colliders
- Verify they're on the "Interactable" layer
- Check CursorManager raycasting distance

**Camera looks wrong?**
- Verify IsometricCamera rotation X=30, Y=45
- Try Orthographic mode
- Adjust distance and height for your room size

**Inventory not showing items?**
- Ensure InventoryUI component exists and references the Inventory
- Call `RefreshDisplay()` after adding items
- Check Canvas setup (needs CanvasScaler if responsive)

---

## Resources

- [Unity Manual - Isometric Camera](https://docs.unity3d.com/Manual/)
- [Blue Prince](https://www.blueprincegame.com/) - Reference game
- [Scotch.io Isometric Games](https://scotch.io/) - Isometric technique articles

