using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages room/scene transitions and player spawning.
/// 
/// SETUP: Place a spawn point Transform in the scene if you want custom spawning.
/// Otherwise uses defaultSpawnPosition.
/// </summary>
public class RoomManager : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        Assert.IsNotNull(playerController, "PlayerController not found in scene!", this);
        SpawnPlayerInRoom();
    }

    /// <summary>
    /// Spawn player at the designated spawn point.
    /// </summary>
    private void SpawnPlayerInRoom()
    {
        Vector3 spawnPos = defaultSpawnPosition;

        // Try to use designated spawn point
        if (playerSpawnPoint != null)
            spawnPos = playerSpawnPoint.position;

        playerController.Teleport(spawnPos);
        Debug.Log($"[RoomManager] Player spawned at {spawnPos}");
    }

    /// <summary>
    /// Set the spawn point for the next room load.
    /// </summary>
    public void SetSpawnPoint(Vector3 position)
    {
        defaultSpawnPosition = position;
    }

    /// <summary>
    /// Get the current scene/room name.
    /// </summary>
    public string GetCurrentRoom()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Load a new room by name.
    /// </summary>
    public void LoadRoom(string roomName)
    {
        SceneManager.LoadScene(roomName);
    }
}
