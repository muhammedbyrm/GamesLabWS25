using UnityEngine;
using System.Collections.Generic;

public class ClueManager : MonoBehaviour
{
    [SerializeField] private InspectionUIController inspectionUI;
    [SerializeField] private ClueLogController clueLogUI; // ← NEW: Drag ClueLogController here

    [SerializeField] private List<ClueData> allRequiredClues = new();
    
    private List<ClueData> foundClues = new();
    public bool IsInspectionActive { get; private set; } = false;
    public System.Action OnAllCluesFound;

    public void InspectClue(ClueData clue)
    {
        // Allow re-inspection (as requested)
        if (!foundClues.Contains(clue))
        {
            foundClues.Add(clue);
            
            clueLogUI?.UpdateClueLog(foundClues); // ← Update log on new clue
            CheckCompletion();
        }

        inspectionUI.Show(clue.clueTitle, clue.monologueText);
        IsInspectionActive = true;
    }

    private void CheckCompletion()
    {
        int foundRequired = 0;
        foreach (var required in allRequiredClues)
        {
            if (foundClues.Contains(required)) foundRequired++;
        }

        if (foundRequired == allRequiredClues.Count)
        {
            OnAllCluesFound?.Invoke();
            Debug.Log("ALL CLUES FOUND! Reconstruction ready.");
        }
    }

    public void EndInspection()
    {
        IsInspectionActive = false;
    }

    public List<ClueData> GetFoundClues() => foundClues;
}