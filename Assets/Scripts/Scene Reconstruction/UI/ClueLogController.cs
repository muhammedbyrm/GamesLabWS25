    using UnityEngine;
    using UnityEngine.UIElements;
    using System.Collections.Generic;

    public class ClueLogController : MonoBehaviour
    {
        private VisualElement root;
        private VisualElement thoughtList;
private Label clueCounter; // Reference to the counter text

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            // Fix: Query for the element named "Root" inside your UXML
            root = uiDocument.rootVisualElement.Q<VisualElement>("Root");
            thoughtList = root.Q<VisualElement>("ThoughtList");
clueCounter = root.Q<Label>("ClueCounter"); // Link to the UXML label
            // Ensure it starts hidden
            if (root != null) root.AddToClassList("hidden");
        }

        public void HideLog()
        {
            root?.AddToClassList("hidden");
        }

        public void UpdateClueLog(List<ClueData> foundClues)
        {
            if (root == null) return;
            
            thoughtList.Clear();
if (clueCounter != null) clueCounter.text = $"{foundClues.Count}/{5} Found";
            if (foundClues.Count == 0) return;

            // Show the UI when clues are added
            root.RemoveFromClassList("hidden");
            foreach (var clue in foundClues)
            {
                var bubble = new VisualElement();
                
                bubble.AddToClassList("ThoughtBubble");

                var title = new Label(clue.clueTitle);
                title.AddToClassList("thought-bubble-title");

                var summary = new Label(clue.shortSummary);
                summary.AddToClassList("thought-bubble-summary");

                bubble.Add(title);
                bubble.Add(summary);
                thoughtList.Add(bubble);

                int index = foundClues.IndexOf(clue);
                bubble.schedule.Execute(() => bubble.AddToClassList("visible")).StartingIn(index * 300);
            }
        }
    }   