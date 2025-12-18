using UnityEngine;

[CreateAssetMenu(fileName = "NewClue", menuName = "Clue Data")]
public class ClueData : ScriptableObject
{
    public string clueTitle;
    [TextArea(3, 10)]
    public string monologueText;
    
    [Header("Reconstruction")]
    public int chronologicalOrder; // 0 = first event, 1 = second, etc.
    public bool isRequiredForReconstruction = true; // Only these count for "all found"
    
    [Header("Log Display")]
    public string shortSummary; // For Clue Log UI
}