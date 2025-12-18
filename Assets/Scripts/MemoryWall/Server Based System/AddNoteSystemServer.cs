using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddNoteSystemServer : MonoBehaviour
{
    public UIDocument ui;
    public string playerName = "Player";
    public int currentLoop = 1;

    VisualElement root;

    VisualElement panel;
    Button openBtn;
    Button closeBtn;

    Label previewText;
    ScrollView wordGrid;

    Button tabActions, tabPlaces, tabObjects, tabNumbers, tabCommon;
    Button saveBtn, clearBtn;

    Label introLabel;

    LoopDatabase loopData;
    LoopCategory currentWords;

    void OnEnable()
    {
        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        while (ui == null || ui.rootVisualElement == null)
            yield return null;

        root = ui.rootVisualElement;

        LoadLoopWords();
        CacheUI();
        RegisterUI();
        SetLoop(currentLoop);

        var server = FindFirstObjectByType<ServerNoteManager>();
        if (server != null)
        {
            server.playerName = playerName;
            server.currentLoop = currentLoop;
        }
    }

    void LoadLoopWords()
    {
        TextAsset json = Resources.Load<TextAsset>("loop_words");
        if (json == null)  
            return; 
        loopData = JsonUtility.FromJson<LoopDatabase>(json.text);
    }

    void CacheUI()
    {
        panel = root.Q<VisualElement>("AddNotePanel");
        openBtn = root.Q<Button>("AddNoteButton");
        closeBtn = root.Q<Button>("CloseAddNotePanel");

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
        openBtn.clicked += () =>
        {
            previewText.text = "";
            panel.style.display = DisplayStyle.Flex;
            openBtn.style.display = DisplayStyle.None;
        };

        closeBtn.clicked += () =>
        {
            panel.style.display = DisplayStyle.None;
            openBtn.style.display = DisplayStyle.Flex;
        };

        clearBtn.clicked += () => previewText.text = "";

        saveBtn.clicked += SaveNote;

        tabActions.clicked += () => ShowCategory("actions");
        tabPlaces.clicked += () => ShowCategory("places");
        tabObjects.clicked += () => ShowCategory("objects");
        tabNumbers.clicked += () => ShowCategory("numbers");
        tabCommon.clicked += () => ShowCategory("common");
    }

    void SetLoop(int loop)
    {
        currentWords = loopData.loops.Find(x => x.loop == loop);
        if (currentWords != null)
            ShowCategory("actions");
    }

    void ShowCategory(string cat)
    {
        wordGrid.Clear();

        List<string> list = cat switch
        {
            "actions" => currentWords.actions,
            "places" => currentWords.places,
            "objects" => currentWords.objects,
            "numbers" => currentWords.numbers,
            "common" => currentWords.common,
            _ => null
        };

        foreach (var w in list)
        {
            Button b = new Button(() => AddWord(w));
            b.text = w;
            wordGrid.Add(b);
        }
    }

    void AddWord(string w)
    {
        previewText.text =
            (previewText.text.Length == 0) ? w : previewText.text + " " + w;
    }

    void SaveNote()
    {
        if (previewText.text.Length == 0)
            return;

        var server = FindFirstObjectByType<ServerNoteManager>();
        if (server != null)
            server.CreateNote(previewText.text);

        introLabel.text = "Saved!";
        previewText.text = "";

        StartCoroutine(ResetIntro());
    }

    IEnumerator ResetIntro()
    {
        yield return new WaitForSeconds(1.2f);
        introLabel.text = "Create your message by choosing words from the categories above.";
    }
}
