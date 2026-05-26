using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main menu UI controller.
/// Handles new game, load game, settings, and quit.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private Button[] loadSlotButtons;
    [SerializeField] private Button backFromLoadButton;

    private void Start()
    {
        // Main menu
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGame);
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(OnLoadGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        // Load menu
        if (backFromLoadButton != null)
            backFromLoadButton.onClick.AddListener(OnBackFromLoad);

        for (int i = 0; i < loadSlotButtons.Length; i++)
        {
            int slot = i; // Local copy for closure
            loadSlotButtons[i].onClick.AddListener(() => OnLoadSlot(slot));
        }

        if (loadGamePanel != null)
            loadGamePanel.SetActive(false);
    }

    /// <summary>
    /// Start a new game.
    /// </summary>
    private void OnNewGame()
    {
        GameManager.Instance.NewGame();
    }

    /// <summary>
    /// Show load game menu.
    /// </summary>
    private void OnLoadGame()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (loadGamePanel != null)
            loadGamePanel.SetActive(true);

        // Update slot button states
        SaveSystem saveSystem = GameManager.Instance.GetComponent<SaveSystem>();
        if (saveSystem != null)
        {
            for (int i = 0; i < loadSlotButtons.Length; i++)
            {
                bool slotExists = saveSystem.SaveExists(i);
                loadSlotButtons[i].interactable = slotExists;
            }
        }
    }

    /// <summary>
    /// Load a specific save slot.
    /// </summary>
    private void OnLoadSlot(int slot)
    {
        GameManager.Instance.LoadGame(slot);
        // After loading, transition to game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Show settings (placeholder).
    /// </summary>
    private void OnSettings()
    {
        Debug.Log("[MainMenuUI] Settings not implemented yet");
    }

    /// <summary>
    /// Back from load menu.
    /// </summary>
    private void OnBackFromLoad()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (loadGamePanel != null)
            loadGamePanel.SetActive(false);
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    private void OnQuit()
    {
        GameManager.Instance.Quit();
    }
}
