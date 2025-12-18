using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ClueLogController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement thoughtList;

    private void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        thoughtList = root.Q<VisualElement>("ThoughtList");

        root.AddToClassList("hidden");
    }

    public void UpdateClueLog(List<ClueData> foundClues)
    {
        thoughtList.Clear();

        if (foundClues.Count == 0) return;

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

            // Fade in with stagger
            int index = foundClues.IndexOf(clue);
            bubble.schedule.Execute(() => bubble.AddToClassList("visible")).StartingIn(index * 300);
        }
    }
}