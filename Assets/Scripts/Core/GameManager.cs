using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager - Singleton pattern.
/// Coordinates all game systems: player, rooms, inventory, save state.
/// 
/// SETUP: This GameObject must have GameState, SaveSystem, and Inventory components.
/// The [RequireComponent] attributes enforce this.
/// Assign RoomManager and PlayerController references in the Inspector.
/// </summary>
[RequireComponent(typeof(GameState))]
[RequireComponent(typeof(SaveSystem))]
[RequireComponent(typeof(Inventory))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState gameState;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory inventory;

    private void OnValidate()
    {
        // Editor-time validation: warn if critical references are missing
        if (roomManager == null)
            Debug.LogWarning("[GameManager] RoomManager not assigned in Inspector.", this);
        if (playerController == null)
            Debug.LogWarning("[GameManager] PlayerController not assigned in Inspector.", this);
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize all systems
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        // Required components on GameManager itself
        if (gameState == null)
            gameState = GetComponent<GameState>();
        Assert.IsNotNull(gameState, "[GameManager] GameState component is required! Add it to this GameObject.", this);

        if (saveSystem == null)
            saveSystem = GetComponent<SaveSystem>();
        Assert.IsNotNull(saveSystem, "[GameManager] SaveSystem component is required! Add it to this GameObject.", this);

        if (inventory == null)
            inventory = GetComponent<Inventory>();
        Assert.IsNotNull(inventory, "[GameManager] Inventory component is required! Add it to this GameObject.", this);

        // Optional: Can be found if not assigned
        if (roomManager == null)
            roomManager = FindObjectOfType<RoomManager>();
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        Debug.Log("[GameManager] Systems initialized.");
    }

    /// <summary>
    /// Load a saved game from slot.
    /// </summary>
    public void LoadGame(int slot)
    {
        saveSystem.LoadGame(slot);
        Debug.Log($"[GameManager] Loaded game from slot {slot}");
    }

    /// <summary>
    /// Save current game state to slot.
    /// </summary>
    public void SaveGame(int slot)
    {
        saveSystem.SaveGame(slot);
        Debug.Log($"[GameManager] Saved game to slot {slot}");
    }

    /// <summary>
    /// Start a new game.
    /// </summary>
    public void NewGame()
    {
        gameState.ResetState();
        SceneManager.LoadScene("Game");
        Debug.Log("[GameManager] New game started.");
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Get the current game state.
    /// </summary>
    public GameState GetGameState() => gameState;

    /// <summary>
    /// Get the inventory system.
    /// </summary>
    public Inventory GetInventory() => inventory;

    /// <summary>
    /// Get the room manager.
    /// </summary>
    public RoomManager GetRoomManager() => roomManager;
}
