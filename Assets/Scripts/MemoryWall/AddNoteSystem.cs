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
        
    }

    void OnEnable()
    {
       
        StartCoroutine(InitializeAfterEnable());
    }

    private IEnumerator InitializeAfterEnable()
    {
        while (ui == null || ui.rootVisualElement == null)
        {
            yield return null;
        }

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
            Debug.LogError("loop_words.json not found in Resources folder!");
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
        if (root == null) return;

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
        if (addNoteBtn != null)
        {
            addNoteBtn.clicked -= OnAddNoteClicked;
            addNoteBtn.clicked += OnAddNoteClicked;
        }

        if (closeAddNotePanelBtn != null)
        {
            closeAddNotePanelBtn.clicked -= OnCloseAddNoteClicked;
            closeAddNotePanelBtn.clicked += OnCloseAddNoteClicked;
        }

        if (saveBtn != null)
        {
            saveBtn.clicked -= SaveNewNote;
            saveBtn.clicked += SaveNewNote;
        }

        if (clearBtn != null)
        {
            clearBtn.clicked -= OnClearClicked;
            clearBtn.clicked += OnClearClicked;
        }

        if (tabActions != null)
            tabActions.clicked += () => ShowCategory("actions");
        if (tabPlaces != null)
            tabPlaces.clicked += () => ShowCategory("places");
        if (tabObjects != null)
            tabObjects.clicked += () => ShowCategory("objects");
        if (tabNumbers != null)
            tabNumbers.clicked += () => ShowCategory("numbers");
        if (tabCommon != null)
            tabCommon.clicked += () => ShowCategory("common");
    }

    void OnAddNoteClicked()
    {
        if (previewText != null)
            previewText.text = "";
        if (addNotePanel != null)
            addNotePanel.style.display = DisplayStyle.Flex;
        if (addNoteBtn != null)
            addNoteBtn.style.display = DisplayStyle.None;
    }

    void OnCloseAddNoteClicked()
    {
        if (addNotePanel != null)
            addNotePanel.style.display = DisplayStyle.None;
        if (addNoteBtn != null)
            addNoteBtn.style.display = DisplayStyle.Flex;
    }

    void OnClearClicked()
    {
        if (previewText != null)
            previewText.text = "";
    }

    void SetLoop(int loop)
    {
        if (loopData == null || loopData.loops == null) return;

        currentWords = loopData.loops.Find(x => x.loop == loop);

        if (currentWords != null)
            ShowCategory("actions");
    }

    void ShowCategory(string category)
    {
        if (wordGrid == null || currentWords == null) return;

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

        if (list == null) return;

        foreach (string w in list)
        {
            Button b = new Button(() => AddWord(w));
            b.text = w;
            wordGrid.Add(b);
        }
    }

    void AddWord(string word)
    {
        if (previewText == null) return;

        previewText.text = previewText.text.Length == 0
            ? word
            : previewText.text + " " + word;
    }

    void SaveNewNote()
    {
        if (isSaving || previewText == null || previewText.text.Length == 0)
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

        if (introLabel != null)
            introLabel.text = "Saved!";
        if (previewText != null)
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
        if (introLabel != null)
            introLabel.text = "Create your message by choosing words from the categories above.";
    }
}