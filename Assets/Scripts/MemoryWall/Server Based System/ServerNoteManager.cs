using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ServerNoteManager : MonoBehaviour
{
    public ServerNoteAPI api;
    public AddNoteSystemServer noteSystem;

    public string playerName = "";
    public int currentLoop = 1;

    public List<ServerNoteEntry> allNotes = new List<ServerNoteEntry>();

    void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while (noteSystem == null)
            yield return null;

        while (noteSystem.playerName == "Player") 
            yield return null;

        playerName = noteSystem.playerName;
        currentLoop = noteSystem.currentLoop;


        RefreshNotes();
    }

    public void RefreshNotes()
    {
        StartCoroutine(api.GetNotes(OnNotesLoaded));
    }

    void OnNotesLoaded(List<ServerNoteEntry> notes)
    {
        if (notes == null)
        {
            return;
        }

        allNotes = notes;

        var ui = FindFirstObjectByType<ServerMessageUIManager>();
        if (ui != null)
            ui.RefreshWithServerNotes(allNotes);
    }

    public void CreateNote(string text)
    {
        ServerNoteEntry entry = new ServerNoteEntry
        {
            sender = playerName,
            title = $"Note from {playerName}",
            fullMessage = text,
            level = currentLoop
        };

        StartCoroutine(api.AddNote(entry, success =>
        {
            if (success)
                RefreshNotes();
        }));
    }

    public void DeleteNote(ServerNoteEntry entry)
    {
        StartCoroutine(api.DeleteNote(entry, success =>
        {
            if (success)
                RefreshNotes();
        }));
    }
}
