using UnityEngine;
using System.Collections.Generic;

public class ClueManager : MonoBehaviour
{
    [SerializeField] private InspectionUIController inspectionUI;
    [SerializeField] private List<ClueData> allRequiredClues = new(); // Drag all required here once
    
    private List<ClueData> foundClues = new();
    public bool IsInspectionActive { get; private set; } = false;
    public System.Action OnAllCluesFound; // Event for reconstruction unlock

    public void InspectClue(ClueData clue)
    {
        if (foundClues.Contains(clue)) return; // Already inspected

        foundClues.Add(clue);
        inspectionUI.Show(clue.clueTitle, clue.monologueText);
        IsInspectionActive = true;

        CheckCompletion();
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
            OnAllCluesFound?.Invoke(); // Fires reconstruction unlock
            Debug.Log("ALL CLUES FOUND! Reconstruction ready.");
        }
    }

    public void EndInspection()
    {
        IsInspectionActive = false;
    }

    // Public getters for UI (Clue Log, Reconstruction)
    public List<ClueData> GetFoundClues() => foundClues;
    public bool HasAllClues() => foundClues.Count == allRequiredClues.Count;
}