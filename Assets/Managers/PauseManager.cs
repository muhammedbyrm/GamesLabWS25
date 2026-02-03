using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public UIDocument uiDocument;
    public string mainMenuSceneName = "StartScene";
    public FirstPersonControllerInteractable player;
    [SerializeField] private GameObject objectToHide;

    private VisualElement root;
    private bool isPaused = false;

    void OnEnable()
    {
        if (uiDocument == null)
        {
            enabled = false;
            return;
        }

        root = uiDocument.rootVisualElement;

        var rootPanel = root.Q<VisualElement>("root");
        if (rootPanel != null)
            rootPanel.style.display = DisplayStyle.None;

        var resumeBtn = root.Q<Button>("resumeBtn");
        var menuBtn = root.Q<Button>("menuBtn");
        var exitBtn = root.Q<Button>("exitBtn");

        if (resumeBtn != null) resumeBtn.clicked += ResumeGame;
        if (menuBtn != null) menuBtn.clicked += GoToMainMenu;
        if (exitBtn != null) exitBtn.clicked += ExitGame;
    }

    void Update()
    {
        // ESC was consumed by another UI this frame
        if (GameManager.Instance != null &&
            GameManager.Instance.escConsumedThisFrame)
        {
            GameManager.Instance.escConsumedThisFrame = false;
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // Block pause if another UI is open (unless pause is already active)
        if (!isPaused &&
            GameManager.Instance != null &&
            GameManager.Instance.isUIOpen)
        {
            return;
        }

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void LateUpdate()
    {
        if (!isPaused)
            return;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    void PauseGame()
    {
        isPaused = true;

        if (GameManager.Instance != null)
            GameManager.Instance.isUIOpen = true;

        var rootPanel = root.Q<VisualElement>("root");
        if (rootPanel != null)
            rootPanel.style.display = DisplayStyle.Flex;

        Time.timeScale = 0f;

        if (player != null)
            player.enabled = false;

        if (objectToHide != null)
            objectToHide.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (GameManager.Instance != null)
            GameManager.Instance.isUIOpen = false;

        var rootPanel = root.Q<VisualElement>("root");
        if (rootPanel != null)
            rootPanel.style.display = DisplayStyle.None;

        Time.timeScale = 1f;

        if (player != null)
            player.enabled = true;

        if (objectToHide != null)
            objectToHide.SetActive(true);

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (GameManager.Instance != null)
            GameManager.Instance.isUIOpen = false;

        if (player != null)
            player.enabled = true;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnDisable()
    {
        if (root == null) return;

        var resumeBtn = root.Q<Button>("resumeBtn");
        var menuBtn = root.Q<Button>("menuBtn");
        var exitBtn = root.Q<Button>("exitBtn");

        if (resumeBtn != null) resumeBtn.clicked -= ResumeGame;
        if (menuBtn != null) menuBtn.clicked -= GoToMainMenu;
        if (exitBtn != null) exitBtn.clicked -= ExitGame;
    }
}
