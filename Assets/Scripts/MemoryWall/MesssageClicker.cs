using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class MessageClicker : MonoBehaviour
{
    public UIDocument uiDoc;

    void OnEnable()
    {
        StartCoroutine(InitializeAfterEnable());
    }

    private IEnumerator InitializeAfterEnable()
    {
        while (uiDoc == null || uiDoc.rootVisualElement == null)
        {
            yield return null;
        }

        var root = uiDoc.rootVisualElement;

        var detailPanel = root.Q<VisualElement>("DetailedMessage");
        var closeButton = root.Q<Button>("CloseDetailPanel");
        var deleteNoteButton = root.Q<Button>("DeleteNote");
        var addNoteButton = root.Q<Button>("AddNoteButton");

        if (detailPanel != null)
        {
            detailPanel.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                bool isOpen = detailPanel.style.display == DisplayStyle.Flex;

                if (deleteNoteButton != null)
                    deleteNoteButton.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex;
                if (addNoteButton != null)
                    addNoteButton.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex;
            });
        }

        if (closeButton != null)
        {
            closeButton.clicked += () =>
            {
                if (detailPanel != null)
                    detailPanel.style.display = DisplayStyle.None;
            };
        }
    }
}