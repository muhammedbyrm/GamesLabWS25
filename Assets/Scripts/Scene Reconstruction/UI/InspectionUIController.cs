using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class InspectionUIController : MonoBehaviour
{
    private VisualElement root;
    private Label clueTitle;
    private Label monologueText;
    
    [SerializeField] private FirstPersonControllerInteractable playerController;

    private bool isVisible = false;
    public bool IsVisible => isVisible;

    private void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement.Q<VisualElement>("Root")
               ?? uiDocument.rootVisualElement[0];

        // Make root focusable so it can receive keyboard events (backup method)
        root.focusable = true;
        root.tabIndex = 0;

        // Register keyboard event as backup (not strictly needed, but harmless)
        root.RegisterCallback<KeyDownEvent>(OnKeyDown);

        clueTitle = root.Q<Label>("ClueTitle");
        monologueText = root.Q<Label>("MonologueText");

        HideImmediate();
    }

    private void Update()
    {
        // Primary and most reliable way to close: raw input check
        if (isVisible && Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("InspectionUI: Escape pressed — closing UI");
            Hide();
        }
    }

    // Backup method: UI Toolkit keyboard event (works if focus is properly captured)
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (!isVisible) return;

        if (evt.keyCode == KeyCode.K)
        {
            Debug.Log("InspectionUI: Escape detected via UI event — closing UI");
            Hide();
            evt.StopPropagation();
        }
    }

    public void Show(string title, string monologue)
    {
        clueTitle.text = title;
        monologueText.text = monologue;

        // Remove hidden class to show UI
        root.RemoveFromClassList("hidden");
        isVisible = true;

        // Attempt to focus the root (helps with event-based input, but not required for raw input)
        root.Focus();

        // Disable player movement/input
        if (playerController != null)
            playerController.SetInputEnabled(false);

        // Unlock cursor for UI interaction
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        Debug.Log("InspectionUI: Shown with title: " + title);
    }

    public void Hide()
    {
        if (!isVisible)
            return; // Prevent double-hide

        Debug.Log("InspectionUI: Hide() called — closing inspection UI");

        root.AddToClassList("hidden");
        isVisible = false;

        // Re-enable player controls
        if (playerController != null)
            playerController.SetInputEnabled(true);

        // Lock cursor back to first-person mode
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
        FindObjectOfType<ClueManager>()?.EndInspection();
    }

    private void HideImmediate()
    {
        root.AddToClassList("hidden");
        isVisible = false;
    }
}