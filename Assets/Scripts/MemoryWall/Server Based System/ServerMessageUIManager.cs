using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerMessageUIManager : MonoBehaviour
{
    public UIDocument uiDoc;

    VisualElement messageContainer;
    Label messageTemplate;

    VisualElement detailedMessagePanel;
    Label senderLabel;
    Label fullMessageLabel;

    Button deleteNoteButton;
    Label deleteIntro;

    ServerNoteEntry selectedNote;

    string playerName;
    int currentLoop;

    void OnEnable()
    {
        StartCoroutine(InitUI());
    }

    IEnumerator InitUI()
    {
        while (uiDoc == null || uiDoc.rootVisualElement == null)
            yield return null;

        playerName = PlayerPrefs.GetString("PlayerNickname", "Player");
        currentLoop = GameManager.Instance.GetLoopCount();

        var root = uiDoc.rootVisualElement;

        messageContainer = root.Q<VisualElement>("MessageContainer");
        messageTemplate = root.Q<Label>("message");
        if (messageTemplate != null)
            messageTemplate.style.display = DisplayStyle.None;

        detailedMessagePanel = root.Q<VisualElement>("DetailedMessage");
        if (detailedMessagePanel != null)
            detailedMessagePanel.style.display = DisplayStyle.None;

        senderLabel = root.Q<Label>("SenderLabel");
        fullMessageLabel = root.Q<Label>("FullMessage");
        deleteIntro = root.Q<Label>("DeleteIntro");

        deleteNoteButton = root.Q<Button>("DeleteNote");
        if (deleteNoteButton != null)
            deleteNoteButton.clicked += OnDeleteNoteClicked;

        var closeBtn = root.Q<Button>("closeButton");
        if (closeBtn != null)
            closeBtn.clicked += CloseWindow;

        var manager = FindFirstObjectByType<ServerNoteManager>();
        if (manager != null)
            manager.RefreshNotes();
    }

    public void RefreshWithServerNotes(List<ServerNoteEntry> notes)
    {
        if (messageContainer == null)
            return;

        currentLoop = GameManager.Instance.GetLoopCount();
        messageContainer.Clear();

        foreach (var n in notes)
        {
            if (n.level > currentLoop)
                continue;

            Label item = new Label(n.title);
            item.AddToClassList("message-item");

            float lastClick = 0f;
            const float dbl = 0.25f;

            item.RegisterCallback<ClickEvent>((evt) =>
            {
                float t = Time.realtimeSinceStartup;

                if (t - lastClick < dbl)
                    OpenDetail(n);
                else
                {
                    selectedNote = n;
                    Highlight(item);
                }

                lastClick = t;
            });

            messageContainer.Add(item);
        }
    }

    void Highlight(Label target)
    {
        foreach (var c in messageContainer.Children())
            c.RemoveFromClassList("selected-note");

        target.AddToClassList("selected-note");
    }

    void OpenDetail(ServerNoteEntry n)
    {
        detailedMessagePanel.style.display = DisplayStyle.Flex;
        senderLabel.text = "Written by: " + n.sender;
        fullMessageLabel.text = n.fullMessage;

        string titleUpper = n.title.Trim().ToUpper();

        if (titleUpper.Contains("BEFORE USING TIME MACHINE"))
        {
            GameManager.Instance.memoryWallMessageRead = true;
        }

        if (titleUpper.Contains("SCIENCE IS THE KEY"))
        {
            GameManager.Instance.ScienceMessageRead = true;
        }
    }

    void OnDeleteNoteClicked()
    {
        if (selectedNote == null)
        {
            deleteIntro.text = "No note selected.";
            StartCoroutine(ClearDeleteIntro());
            return;
        }

        if (selectedNote.sender != playerName)
        {
            deleteIntro.text = "You can only delete your own notes.";
            StartCoroutine(ClearDeleteIntro());
            return;
        }

        var mgr = FindFirstObjectByType<ServerNoteManager>();
        if (mgr != null)
        {
            mgr.DeleteNote(selectedNote);
            deleteIntro.text = "Note deleted.";
            StartCoroutine(ResetAfterDelete());
        }
    }

    IEnumerator ClearDeleteIntro()
    {
        yield return new WaitForSeconds(1.2f);
        deleteIntro.text = "";
    }

    public void CloseWindow()
    {
        uiDoc.gameObject.SetActive(false);

        var memoryWall = FindFirstObjectByType<MemoryWallInteraction>();
        if (memoryWall != null)
            memoryWall.CloseUI();
    }

    IEnumerator ResetAfterDelete()
    {
        yield return new WaitForSeconds(2f);

        deleteIntro.text = "";
        selectedNote = null;

        if (detailedMessagePanel != null)
            detailedMessagePanel.style.display = DisplayStyle.None;
    }
}