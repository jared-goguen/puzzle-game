using UnityEngine;
using System.IO;

/// <summary>
/// Handles saving and loading game state to/from JSON files.
/// Supports multiple save slots.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    private const string SAVE_DIRECTORY = "Saves";
    private const string SAVE_FILE_PREFIX = "save_";
    private const string SAVE_FILE_EXTENSION = ".json";

    private GameState gameState;
    private Inventory inventory;

    private void Start()
    {
        gameState = GameManager.Instance.GetGameState();
        inventory = GameManager.Instance.GetInventory();
        EnsureSaveDirectoryExists();
    }

    /// <summary>
    /// Ensure the save directory exists.
    /// </summary>
    private void EnsureSaveDirectoryExists()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_DIRECTORY);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log($"[SaveSystem] Created save directory: {path}");
        }
    }

    /// <summary>
    /// Save game to a specific slot.
    /// </summary>
    public void SaveGame(int slot)
    {
        try
        {
            // Create a container for all save data
            SaveData saveData = new SaveData
            {
                timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                gameState = gameState.GetStateData(),
                inventoryItems = inventory.GetAllItems()
            };

            string savePath = GetSavePath(slot);
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);

            Debug.Log($"[SaveSystem] Game saved to slot {slot}: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Load game from a specific slot.
    /// </summary>
    public void LoadGame(int slot)
    {
        try
        {
            string savePath = GetSavePath(slot);

            if (!File.Exists(savePath))
            {
                Debug.LogWarning($"[SaveSystem] Save file not found at slot {slot}");
                return;
            }

            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // Restore game state
            gameState.LoadStateData(saveData.gameState);
            inventory.LoadItems(saveData.inventoryItems);

            Debug.Log($"[SaveSystem] Game loaded from slot {slot}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load game: {e.Message}");
        }
    }

    /// <summary>
    /// Check if a save file exists.
    /// </summary>
    public bool SaveExists(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    /// <summary>
    /// Get the file path for a save slot.
    /// </summary>
    private string GetSavePath(int slot)
    {
        string directory = Path.Combine(Application.persistentDataPath, SAVE_DIRECTORY);
        return Path.Combine(directory, $"{SAVE_FILE_PREFIX}{slot}{SAVE_FILE_EXTENSION}");
    }

    /// <summary>
    /// Container for all save data.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public string timestamp;
        public GameState.StateData gameState;
        public System.Collections.Generic.List<InventoryItemInstance> inventoryItems;
    }
}
