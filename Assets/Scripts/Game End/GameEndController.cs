using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement fadePanel;
    private VisualElement creditsPanel;
    private VisualElement creditsScroller;

    [Header("Settings")]
    [SerializeField] float fadeDuration = 3f;
    [SerializeField] float creditsDuration = 13f;
    [SerializeField] string startSceneName = "StartScreen";

    [SerializeField] private PauseManager pauseManager;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        fadePanel = uiDocument.rootVisualElement.Q<VisualElement>("FadePanel");
        creditsPanel = uiDocument.rootVisualElement.Q<VisualElement>("CreditsPanel");
        creditsScroller = uiDocument.rootVisualElement.Q<VisualElement>("CreditsScroller");

        uiDocument.rootVisualElement.pickingMode = PickingMode.Ignore;
        fadePanel.pickingMode = PickingMode.Ignore;
        creditsPanel.pickingMode = PickingMode.Ignore;
        creditsScroller.pickingMode = PickingMode.Ignore;
    }

    public void StartFadeOut()
    {
        if (pauseManager != null)
        {
            pauseManager.ResumeGame();         
            pauseManager.gameObject.SetActive(false); 
        }

        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        fadePanel.AddToClassList("fade-in");
        yield return new WaitForSeconds(fadeDuration);

        creditsPanel.RemoveFromClassList("hidden");
        yield return new WaitForSeconds(0.5f);

        creditsScroller.AddToClassList("credits-scroll");
        yield return new WaitForSeconds(creditsDuration);

        SceneManager.LoadScene(startSceneName);
    }
}