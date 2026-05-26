using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI panel that displays object descriptions and dialogue.
/// Appears when player examines something.
/// </summary>
public class DescriptionPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelGameObject;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button closeButton;
    [SerializeField] private float displayDuration = 0f; // 0 = infinite until closed
    [SerializeField] private float fadeInDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private Coroutine displayCoroutine;

    private void Start()
    {
        canvasGroup = panelGameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panelGameObject.AddComponent<CanvasGroup>();

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        // Start hidden
        panelGameObject.SetActive(false);
    }

    /// <summary>
    /// Show a description on the panel.
    /// </summary>
    public void ShowDescription(string title, string description)
    {
        // Stop any existing display
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        titleText.text = title;
        descriptionText.text = description;

        if (!panelGameObject.activeSelf)
        {
            displayCoroutine = StartCoroutine(FadeIn());
        }

        // Auto-close after duration
        if (displayDuration > 0)
        {
            displayCoroutine = StartCoroutine(AutoClose());
        }
    }

    /// <summary>
    /// Close the description panel.
    /// </summary>
    public void Close()
    {
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        displayCoroutine = StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Fade panel in.
    /// </summary>
    private IEnumerator FadeIn()
    {
        panelGameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Fade panel out.
    /// </summary>
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeInDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panelGameObject.SetActive(false);
    }

    /// <summary>
    /// Auto-close after duration.
    /// </summary>
    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeOut());
    }
}
