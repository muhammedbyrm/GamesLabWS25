// This UI manager responsible for loading, displaying, and managing player notes.
// Player can delete only his own notes, thats why the players name should be uniquee.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageUIManager : MonoBehaviour
{
    [System.Serializable]
    public class MessageData
    {
        public string sender;
        public string title;
        public string fullMessage;
        public int level;
    }

    [System.Serializable]
    public class MessageDataList
    {
        public List<MessageData> messages;
    }

    public UIDocument uiDoc;
    private string playerName;

    private VisualElement messageContainer;
    private Label messageTemplate;

    private VisualElement detailedMessagePanel;
    private Label senderLabel;
    private Label fullMessageLabel;

    private Button deleteNoteButton;

    public MessageData selectedMessage;

    private List<MessageData> allMessages;
    private int currentLoop = 1;

    private Label deleteIntro;

    void Start()
    {
    }

    void OnEnable()
    {
        StartCoroutine(InitializeAfterEnable());
    }

    private IEnumerator InitializeAfterEnable()
    {
        while (uiDoc == null || uiDoc.rootVisualElement == null)
        {
            yield return null;
        }

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

        Button closeButton = root.Q<Button>("closeButton");
        if (closeButton != null)
            closeButton.clicked += CloseWindow;

        selectedMessage = null;

        deleteNoteButton = root.Q<Button>("DeleteNote");
        if (deleteNoteButton != null)
            deleteNoteButton.clicked += DeleteSelectedNote;

        deleteIntro = root.Q<Label>("DeleteIntro");

        var noteSystem = FindFirstObjectByType<AddNoteSystem>();
        if (noteSystem != null)
            playerName = noteSystem.playerName;

        LoadMessagesFromJson();
        PopulateMessages(currentLoop);
    }

    private void LoadMessagesFromJson()
    {
        string path = Path.Combine(Application.dataPath, "Resources/messages.json");

        if (!File.Exists(path))
        {
            Debug.LogError("Cannot find messages.json at: " + path);
            allMessages = new List<MessageData>();
            return;
        }

        string raw = File.ReadAllText(path).Trim();
        string wrappedJson = "{ \"messages\": " + raw + " }";

        try
        {
            allMessages = JsonUtility.FromJson<MessageDataList>(wrappedJson).messages;
            if (allMessages == null)
                allMessages = new List<MessageData>();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JSON PARSE ERROR: " + ex.Message);
            allMessages = new List<MessageData>();
        }
    }

    private void PopulateMessages(int loop)
    {
        if (messageContainer == null) return;

        messageContainer.Clear();

        foreach (var msg in allMessages)
        {
            if (msg.level > loop)
                continue;

            Label newLabel = new Label();
            newLabel.text = msg.title;
            newLabel.AddToClassList("message-item");

            float lastClick = 0f;
            const float doubleClickThreshold = 0.25f;

            newLabel.RegisterCallback<ClickEvent>((evt) =>
            {
                float time = Time.realtimeSinceStartup;

                if (time - lastClick < doubleClickThreshold)
                {
                    OpenDetailedMessage(msg.sender, msg.fullMessage);
                }
                else
                {
                    selectedMessage = msg;
                    HighlightSelected(newLabel);
                }

                lastClick = time;
            });

            messageContainer.Add(newLabel);
        }
    }

    private void HighlightSelected(Label selectedLabel)
    {
        if (messageContainer == null) return;

        foreach (var child in messageContainer.Children())
            child.RemoveFromClassList("selected-note");

        selectedLabel.AddToClassList("selected-note");
    }

    private void OpenDetailedMessage(string sender, string fullMsg)
    {
        if (detailedMessagePanel == null || senderLabel == null || fullMessageLabel == null) return;

        detailedMessagePanel.style.display = DisplayStyle.Flex;
        senderLabel.text = "Written by: " + sender;
        fullMessageLabel.text = fullMsg;
    }

    public void CloseDetails()
    {
        if (detailedMessagePanel != null)
            detailedMessagePanel.style.display = DisplayStyle.None;
    }

    public void DeleteSelectedNote()
    {
        if (selectedMessage == null)
        {
            if (deleteIntro != null)
            {
                deleteIntro.text = "No note selected.";
                StartCoroutine(ClearDeleteIntro());
            }
            return;
        }

        if (selectedMessage.sender != playerName)
        {
            if (deleteIntro != null)
            {
                deleteIntro.text = "You can only delete your own notes.";
                StartCoroutine(ClearDeleteIntro());
            }
            return;
        }

        string path = Application.dataPath + "/Resources/messages.json";
        string json = File.ReadAllText(path);
        string wrapped = "{ \"messages\": " + json + " }";
        MessageDataList wrapper = JsonUtility.FromJson<MessageDataList>(wrapped);

        MessageData toRemove = wrapper.messages.Find(m =>
            m.sender == selectedMessage.sender &&
            m.title == selectedMessage.title &&
            m.fullMessage == selectedMessage.fullMessage &&
            m.level == selectedMessage.level
        );

        if (toRemove == null)
        {
            if (deleteIntro != null)
            {
                deleteIntro.text = "Note not found!";
                StartCoroutine(ClearDeleteIntro());
            }
            return;
        }

        wrapper.messages.Remove(toRemove);

        string newJson = JsonListToPureArray(wrapper.messages);
        File.WriteAllText(path, newJson);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        selectedMessage = null;
        RefreshMessages();

        if (deleteIntro != null)
        {
            deleteIntro.text = "Note deleted.";
            StartCoroutine(ClearDeleteIntro());
        }
    }

    private IEnumerator ClearDeleteIntro()
    {
        yield return new WaitForSeconds(1.2f);
        if (deleteIntro != null)
            deleteIntro.text = "";
    }

    private string JsonListToPureArray(List<MessageData> list)
    {
        string wrapped = JsonUtility.ToJson(new PureWrapper { messages = list }, true);

        int start = wrapped.IndexOf('[');
        int end = wrapped.LastIndexOf(']');

        return wrapped.Substring(start, end - start + 1);
    }

    [System.Serializable]
    private class PureWrapper
    {
        public List<MessageData> messages;
    }

    public void RefreshMessages()
    {
        LoadMessagesFromJson();
        PopulateMessages(currentLoop);
    }

    public void CloseWindow()
    {
        if (uiDoc != null)
            uiDoc.gameObject.SetActive(false);

        var interaction = FindFirstObjectByType<MemoryWallInteraction>();
        if (interaction != null)
            interaction.CloseUI();
    }
}