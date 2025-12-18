using UnityEngine;
using UnityEngine.UIElements;

public class InteractionPrompt : MonoBehaviour
{
    private VisualElement root;

    private void Awake()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("[InteractionPrompt] Missing UIDocument component! Add one and assign UXML.");
            enabled = false;
            return;
        }

        if (uiDoc.visualTreeAsset == null)
        {
            Debug.LogError("[InteractionPrompt] UIDocument has no Visual Tree Asset assigned!");
            return;
        }

        root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[InteractionPrompt] rootVisualElement is null — check UXML.");
            return;
        }

        Hide(); // Start hidden
    }

    public void Show()
    {
        if (root != null) root.RemoveFromClassList("hidden");
    }

    public void Hide()
    {
        if (root != null) root.AddToClassList("hidden");
    }
}