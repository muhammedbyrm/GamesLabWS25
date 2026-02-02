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

    public bool isCorrect = false;

    [SerializeField] private ClueLogController clueLogUI;
    [SerializeField] private InspectionUIController inspectionUI; // <-- NEW REFERENCE
    [SerializeField] private MonoBehaviour playerMovementScript;


    // Consume ESC so Pause cannot open while Reconstruction UI is active
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !root.ClassListContains("hidden"))
        {
            GameManager.Instance.escConsumedThisFrame = true;
        }
    }




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

        // set OpenUI flag 
        GameManager.Instance.isUIOpen = true;

        if (playerMovementScript != null) playerMovementScript.enabled = false;

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
        card.text = data.shortSummary;
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
        isCorrect = true;
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
            if (playerMovementScript != null) playerMovementScript.enabled = true;

            // set UIOpen flag
            GameManager.Instance.isUIOpen = false;

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

            if (clueLogUI != null)
            {
                clueLogUI.HideLog();
            }

            // 2. Trigger the final epiphany monologue
            if (inspectionUI != null)
            {
                inspectionUI.Show(
                    "REVELATION",
                    "Everything makes sense now...\nI know what happened! I need to go back in time to prevent her death! But first, I must interact with the memory wall and read the notes."
                );
            }

            GameManager.Instance.FinishSceneReconstruction();
        }
        else
        {
            Debug.Log("The order is still unclear. Try again.");
        }
    }

    public void StartGameUI()
    {
        //in present state & gmanager
        inspectionUI.Show(
                    "AMNESIA",
                    "What happened? I think there was a blackout but I'm not sure...is that Alice? Is she...? Is she dead?"
        );
    }

    public void BeforeMemoryWallUI()
    {
        //in fpc & gmanager
        inspectionUI.Show(
                    "FOCUS",
                    "I need to take a look at the Memory Wall.\nIt can hold informations forever. There must be some explanations behind what happened."
        );
    }

    public void AfterMemoryWallUI()
    {
        //in memorywall & gmanager
        inspectionUI.Show(
                    "GOAL",
                    "I got an idea. I will use our Time Machine to jump back in time and save Alice.\nOnce I'm in the past I just need to get rid of the knife and she will not be killed!"
        );
    }

    public void ClosedDoorUI()
    {
        //in fpc & gmanager
        inspectionUI.Show(
                    "DAMN",
                    "I know Alice and the Murder Weapon are behind this door. But it's locked and...I must have forgotten the correct code\nI have to look around, we always have clues scattered around for our hidden door code!"
        );
    }

    public void OpeningDoorUI()
    {
        //in keypad & gmanager
        inspectionUI.Show(
                    "FINALLY",
                    "Yes, I got the code right. And I can see Alice is alive.\nA knife was used to kill her, it has to be in this room. I need to find it first, then..."
        );
    }

    public void MurderWeaponUI()
    {
        //in gmanager
        inspectionUI.Show(
                    "THE KNIFE",
                    "This weapon is the one Alice will get stabbed with. To save her I need to get rid of it, maybe I can get it out of this timeline."
        );
    }

    public void AfterFirstTimeJump()
    {
        //in present state & gmanager
        inspectionUI.Show(
                    "MY HEAD HURTS",
                    "What happened? The lights went out and now my head feels kind of strange. I have to check up on Alice!"
        );
    }
    public void AfterSecondTimeJump()
    {
        //in present state & gmanager
        inspectionUI.Show(
                    "MY HEAD HURTS",
                    "Uggghh...We never tested the Time Machine much before. This truly is a live experiment, but I have to do this for Alice"
        );
    }
    public void AfterThirdTimeJump()
    {
        //in present state & gmanager
        inspectionUI.Show(
                    "MY HEAD HURTS",
                    "No...this is not right...something needs to change...and my head feels like it's gonna explode..."
        );
    }
    public void AfterFinishingGame()
    {
        //in present state & gmanager
        inspectionUI.Show(
                    "ALICE?",
                    "I must have done it! The knife never could have been used now and Alice must live...right?"
        );
    }
}