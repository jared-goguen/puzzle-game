using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tracks all game state: player position, picked-up items, story flags, discovered secrets.
/// 
/// IMPORTANT - Dual Source of Truth Pattern:
/// This class tracks a "pickedUpItems" list which is SEPARATE from the Inventory system.
/// 
/// - Inventory.cs: Tracks CURRENT inventory (what player has RIGHT NOW)
/// - GameState.PickupItem(): Tracks EVER COLLECTED items (permanent record)
/// 
/// Both are needed for different purposes:
/// * Inventory: for dropping items, checking current possession, UI display
/// * GameState: for narrative logic, preventing re-pickup, achievement tracking
/// 
/// When saving/loading:
/// 1. SaveSystem saves BOTH Inventory contents AND GameState pickedUpItems
/// 2. On load, restore Inventory first (current possession)
/// 3. Then restore GameState (historical record)
/// 
/// Consistency Check: If an item is in GameState.pickedUpItems but NOT in Inventory,
/// the player must have dropped it. This is intentional and valid.
/// </summary>
[System.Serializable]
public class GameState : MonoBehaviour
{
    [System.Serializable]
    public class StateData
    {
        public Vector3 playerPosition;
        public List<string> pickedUpItems = new List<string>();
        public Dictionary<string, bool> storyFlags = new Dictionary<string, bool>();
        public Dictionary<string, int> variables = new Dictionary<string, int>();
    }

    private StateData stateData = new StateData();

    /// <summary>
    /// Add an item to the picked-up list.
    /// </summary>
    public void PickupItem(string itemId)
    {
        if (!stateData.pickedUpItems.Contains(itemId))
        {
            stateData.pickedUpItems.Add(itemId);
            Debug.Log($"[GameState] Picked up item: {itemId}");
        }
    }

    /// <summary>
    /// Check if an item has been picked up.
    /// </summary>
    public bool HasPickedUpItem(string itemId) => stateData.pickedUpItems.Contains(itemId);

    /// <summary>
    /// Set a story flag (for tracking narrative progression).
    /// </summary>
    public void SetFlag(string flagName, bool value)
    {
        stateData.storyFlags[flagName] = value;
        Debug.Log($"[GameState] Flag '{flagName}' set to {value}");
    }

    /// <summary>
    /// Get a story flag value.
    /// </summary>
    public bool GetFlag(string flagName) => stateData.storyFlags.ContainsKey(flagName) && stateData.storyFlags[flagName];

    /// <summary>
    /// Set a numeric variable (for stats, counters, etc.).
    /// </summary>
    public void SetVariable(string varName, int value)
    {
        stateData.variables[varName] = value;
        Debug.Log($"[GameState] Variable '{varName}' set to {value}");
    }

    /// <summary>
    /// Get a numeric variable.
    /// </summary>
    public int GetVariable(string varName) => stateData.variables.ContainsKey(varName) ? stateData.variables[varName] : 0;

    /// <summary>
    /// Save player position for checkpoints.
    /// </summary>
    public void SetPlayerPosition(Vector3 position)
    {
        stateData.playerPosition = position;
    }

    /// <summary>
    /// Get saved player position.
    /// </summary>
    public Vector3 GetPlayerPosition() => stateData.playerPosition;

    /// <summary>
    /// Reset all state for new game.
    /// </summary>
    public void ResetState()
    {
        stateData = new StateData();
        Debug.Log("[GameState] State reset.");
    }

    /// <summary>
    /// Get the full state data (for serialization).
    /// </summary>
    public StateData GetStateData() => stateData;

    /// <summary>
    /// Load state from data (for deserialization).
    /// </summary>
    public void LoadStateData(StateData data)
    {
        stateData = data;
    }
}
