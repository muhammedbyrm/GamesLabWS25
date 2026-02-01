using System.Collections.Generic;
using UnityEngine;

public class LaserPuzzleManager : MonoBehaviour
{
    // Keep track of every object the player places
    private List<GameObject> placedObjects = new List<GameObject>();

    [Header("UI References")]
    [SerializeField] private GameObject placementUIPanel; 
    // Add other UI elements you need to reset here

    // Call this whenever the player spawns a mirror or splitter
    public void RegisterPlacedObject(GameObject obj)
    {
        placedObjects.Add(obj);
    }

    // THIS IS THE TIME JUMP RESET
    public void ResetPuzzleState()
    {
        Debug.Log("<color=cyan>Time Jump: Resetting Laser Puzzle...</color>");

        // 1. Remove all placed mirrors/splitters
        foreach (GameObject obj in placedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        // Clear the list so we don't try to destroy them again
        placedObjects.Clear();

        // 2. Reset the UI
        ResetPuzzleUI();

        // 3. Reset any internal logic (like "Puzzle Solved" flags)
        // puzzleSolved = false;
    }

    private void ResetPuzzleUI()
    {
        // Example: Hide the placement menu or reset inventory counts
        if (placementUIPanel != null)
        {
            placementUIPanel.SetActive(false);
        }
        

    }
}