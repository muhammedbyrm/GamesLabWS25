using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class ReconstructionController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement timeline;
    private VisualElement cluePool;
    private Button confirmButton;

    private List<VisualElement> slots = new();
    private List<ClueData> correctOrder;

    [SerializeField] private ClueLogController clueLogUI;
    [SerializeField] private InspectionUIController inspectionUI; // <-- NEW REFERENCE

    private void Awake()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("Root");

        timeline = root.Q<VisualElement>("Timeline");
        cluePool = root.Q<VisualElement>("CluePool");
        confirmButton = root.Q<Button>("ConfirmButton");

        foreach (var slot in timeline.Children())
        {
            slots.Add(slot);
            slot.RegisterCallback<ClickEvent>(evt => ReturnClueFromSlot(slot));
        }

        confirmButton.clicked += OnConfirm;
        root.AddToClassList("hidden");
    }

    public void OpenReconstruction(List<ClueData> foundClues)
    {
        root.RemoveFromClassList("hidden");
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        cluePool.Clear();
        foreach (var slot in slots) slot.Clear();
        correctOrder = foundClues.OrderBy(c => c.chronologicalOrder).ToList();

        foreach (var clue in foundClues)
        {
            var card = CreateClueCard(clue);
            cluePool.Add(card);
        }
    }

    private VisualElement CreateClueCard(ClueData data)
    {
        var card = new Button();
        card.AddToClassList("ClueCard");
        card.text = data.clueTitle;
        card.userData = data;
        card.clicked += () => TryPlaceClue(card);
        return card;
    }

    private void TryPlaceClue(VisualElement card)
    {
        var emptySlot = slots.FirstOrDefault(s => s.childCount == 0);
        if (emptySlot != null)
        {
            card.RemoveFromHierarchy();
            emptySlot.Add(card);
            card.style.width = Length.Percent(100);
            card.style.height = Length.Percent(100);
        }
    }

    private void ReturnClueFromSlot(VisualElement slot)
    {
        if (slot.childCount > 0)
        {
            var card = slot[0];
            card.RemoveFromHierarchy();
            card.style.width = 180; 
            card.style.height = 120;
            cluePool.Add(card);
        }
    }

    private void OnConfirm()
    {
        bool isCorrect = true;
        for (int i = 0; i < slots.Count; i++)
        {
            var card = slots[i].childCount > 0 ? slots[i][0] : null;
            if (card == null || (ClueData)card.userData != correctOrder[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            // SUCCESS SEQUENCE
            Debug.Log("Reconstruction Successful!");
            
            // 1. Hide the puzzle board
            root.AddToClassList("hidden");
            
            if (clueLogUI != null)
            {
                clueLogUI.HideLog();
            }

            // 2. Trigger the final epiphany monologue
            if (inspectionUI != null)
            {
                inspectionUI.Show(
                    "REVELATION", 
                    "Everything makes sense now...\nI know what happened! I need to go back in time to prevent her death!"
                );
            }
        }
        else
        {
            Debug.Log("The order is still unclear. Try again.");
        }
    }
}