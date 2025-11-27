using UnityEngine;
using UnityEngine.UIElements;

public class MessageClicker : MonoBehaviour
{
    public UIDocument uiDoc;

    void Start()
    {
        var root = uiDoc.rootVisualElement;

        var detailPanel = root.Q<VisualElement>("DetailedMessage");
        var closeButton = root.Q<Button>("CloseDetailPanel");

        var deleteNoteButton = root.Q<Button>("DeleteNote");
        var addNoteButton = root.Q<Button>("AddNoteButton");

        detailPanel.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            bool isOpen = detailPanel.style.display == DisplayStyle.Flex;

            deleteNoteButton.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex;
            addNoteButton.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex;
        });

        closeButton.RegisterCallback<ClickEvent>(evt =>
        {
            detailPanel.style.display = DisplayStyle.None;
        });
    }
}
