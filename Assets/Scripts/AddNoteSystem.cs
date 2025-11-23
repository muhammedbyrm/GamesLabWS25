// This script handles creating, previewing, and saving player-generated notes based on loop-specific word categories.
// Currently this system uses a fixed/static structure and will be expanded in later versions.


using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


#region JSON Structures
[System.Serializable]
public class LoopCategory
{
    public int loop;
    public List<string> actions;
    public List<string> places;
    public List<string> objects;
    public List<string> numbers;
    public List<string> common;
}

[System.Serializable]
public class LoopDatabase
{
    public List<LoopCategory> loops;
}

[System.Serializable]
public class NoteEntry
{
    public string sender;
    public string title;
    public string fullMessage;
    public int level;
}
#endregion

public class AddNoteSystem : MonoBehaviour
{
    public UIDocument ui;
    public string playerName = "Player";
    public int currentLoop = 1;

    private VisualElement root;

    private VisualElement addNotePanel;
    private Button addNoteBtn;
    private Button closeAddNotePanelBtn;

    private Label previewText;
    private ScrollView wordGrid;

    private Button tabActions, tabPlaces, tabObjects, tabNumbers, tabCommon;
    private Button saveBtn, clearBtn;

    private Label introLabel;

    private LoopDatabase loopData;
    private LoopCategory currentWords;

    private List<NoteEntry> notes = new List<NoteEntry>();

    private string notesPath;

    private bool isSaving = false;

    void Awake()
    {
        root = ui.rootVisualElement;

        notesPath = Path.Combine(Application.dataPath, "Resources/messages.json");

        LoadLoopWords();
        LoadNotes();
        CacheUI();
        RegisterUI();
        SetLoop(currentLoop);
    }

    void LoadLoopWords()
    {
        TextAsset json = Resources.Load<TextAsset>("loop_words");

        if (json == null)
        {
            return;
        }

        loopData = JsonUtility.FromJson<LoopDatabase>(json.text);
    }

    void LoadNotes()
    {
        if (!File.Exists(notesPath))
        {
            notes = new List<NoteEntry>();
            SaveNotes();
            return;
        }

        string json = File.ReadAllText(notesPath);

        try
        {
            notes = JsonUtility.FromJson<Wrapper>("{\"list\":" + json + "}").list;
            if (notes == null)
                notes = new List<NoteEntry>();
        }
        catch
        {
            notes = new List<NoteEntry>();
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<NoteEntry> list;
    }

    void SaveNotes()
    {
        string wrapperJson = JsonUtility.ToJson(new Wrapper { list = notes }, true);

        int idx = wrapperJson.IndexOf('[');
        int idx2 = wrapperJson.LastIndexOf(']');

        string pureJson = wrapperJson.Substring(idx, idx2 - idx + 1);

        File.WriteAllText(notesPath, pureJson);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    void CacheUI()
    {
        addNotePanel = root.Q<VisualElement>("AddNotePanel");
        addNoteBtn = root.Q<Button>("AddNoteButton");
        closeAddNotePanelBtn = root.Q<Button>("CloseAddNotePanel");

        previewText = root.Q<Label>("PreviewText");
        wordGrid = root.Q<ScrollView>("WordGrid");

        tabActions = root.Q<Button>("TabActions");
        tabPlaces = root.Q<Button>("TabPlaces");
        tabObjects = root.Q<Button>("TabObjects");
        tabNumbers = root.Q<Button>("TabNumbers");
        tabCommon = root.Q<Button>("TabCommon");

        saveBtn = root.Q<Button>("SaveBtn");
        clearBtn = root.Q<Button>("ClearBtn");

        introLabel = root.Q<Label>("Intro");
    }

    void RegisterUI()
    {
        addNoteBtn.clicked -= OnAddNoteClicked;
        closeAddNotePanelBtn.clicked -= OnCloseAddNoteClicked;
        saveBtn.clicked -= SaveNewNote;
        clearBtn.clicked -= OnClearClicked;

        addNoteBtn.clicked += OnAddNoteClicked;
        closeAddNotePanelBtn.clicked += OnCloseAddNoteClicked;
        saveBtn.clicked += SaveNewNote;
        clearBtn.clicked += OnClearClicked;

        tabActions.clicked += () => ShowCategory("actions");
        tabPlaces.clicked += () => ShowCategory("places");
        tabObjects.clicked += () => ShowCategory("objects");
        tabNumbers.clicked += () => ShowCategory("numbers");
        tabCommon.clicked += () => ShowCategory("common");
    }

    void OnAddNoteClicked()
    {
        previewText.text = "";
        addNotePanel.style.display = DisplayStyle.Flex;
        addNoteBtn.style.display = DisplayStyle.None;
    }

    void OnCloseAddNoteClicked()
    {
        addNotePanel.style.display = DisplayStyle.None;
        addNoteBtn.style.display = DisplayStyle.Flex;
    }

    void OnClearClicked()
    {
        previewText.text = "";
    }

    void SetLoop(int loop)
    {
        currentWords = loopData.loops.Find(x => x.loop == loop);
        ShowCategory("actions");
    }

    void ShowCategory(string category)
    {
        wordGrid.Clear();

        List<string> list = category switch
        {
            "actions" => currentWords.actions,
            "places" => currentWords.places,
            "objects" => currentWords.objects,
            "numbers" => currentWords.numbers,
            "common" => currentWords.common,
            _ => null
        };

        foreach (string w in list)
        {
            Button b = new Button(() => AddWord(w));
            b.text = w;
            wordGrid.Add(b);
        }
    }

    void AddWord(string word)
    {
        previewText.text = previewText.text.Length == 0
            ? word
            : previewText.text + " " + word;
    }

    void SaveNewNote()
    {
        if (isSaving || previewText.text.Length == 0)
            return;

        isSaving = true;

        LoadNotes();

        NoteEntry n = new NoteEntry
        {
            sender = playerName,
            title = $"Note from {playerName}",
            fullMessage = previewText.text,
            level = currentLoop
        };

        notes.Add(n);

        SaveNotes();

        introLabel.text = "Saved!";
        previewText.text = "";

        StartCoroutine(RefreshUIDelayed());
        StartCoroutine(ResetIntro());
    }


    private IEnumerator RefreshUIDelayed()
    {
        yield return new WaitForSeconds(0.1f);

        var msgUI = Object.FindFirstObjectByType<MessageUIManager>();
        if (msgUI != null)
            msgUI.RefreshMessages();

        isSaving = false;
    }

    private IEnumerator ResetIntro()
    {
        yield return new WaitForSeconds(1.2f);
        introLabel.text = "Create your message by choosing words from the categories above.";
    }
}