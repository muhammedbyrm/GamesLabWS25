using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ClueManager : MonoBehaviour
{
    [SerializeField] private InspectionUIController inspectionUI;
    [SerializeField] private ClueLogController clueLogUI;
    [SerializeField] private ReconstructionController reconstructionUI;

    [SerializeField] private List<ClueData> allRequiredClues = new();

    private List<ClueData> foundClues = new();
    public bool IsInspectionActive { get; private set; } = false;

    private bool hasOpenedReconstruction = false; 

    public void InspectClue(ClueData clue)
    {
        bool wasNew = !foundClues.Contains(clue);

        if (wasNew)
        {
            foundClues.Add(clue);
            clueLogUI?.UpdateClueLog(foundClues);
            CheckCompletion(); 
        }

        inspectionUI.Show(clue.clueTitle, clue.monologueText);
        IsInspectionActive = true;
    }

    private IEnumerator DelayedOpenReconstruction()
    {
        // This handles the 3 second pause
        yield return new WaitForSeconds(5f);
        // FORCE CLOSE the inspection UI so it doesn't block the screen/input
    if (inspectionUI != null)
    {
        // Assuming your InspectionUIController has a Hide or Close method
        inspectionUI.Hide(); 
        IsInspectionActive = false; 
    }
        reconstructionUI?.OpenReconstruction(foundClues);
        Debug.Log("Opening reconstruction timeline after 3 seconds.");
    }

    private void CheckCompletion()
    {
        if (hasOpenedReconstruction || allRequiredClues.Count == 0) return;

        bool allFound = true;
        foreach (var required in allRequiredClues)
        {
            if (!foundClues.Contains(required))
            {
                allFound = false;
                break;
            }
        }

        if (allFound)
        {
            hasOpenedReconstruction = true;
            // ONLY start the coroutine. REMOVE the direct OpenReconstruction call.
            StartCoroutine(DelayedOpenReconstruction());
            Debug.Log("ALL CLUES FOUND! Waiting 3 seconds...");
        }
    }

    public void EndInspection()
    {
        IsInspectionActive = false;
    }

    public List<ClueData> GetFoundClues() => foundClues;
}