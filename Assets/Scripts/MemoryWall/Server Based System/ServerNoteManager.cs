using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ServerNoteManager : MonoBehaviour
{
    public ServerNoteAPI api;

    string playerName;

    public List<ServerNoteEntry> allNotes = new List<ServerNoteEntry>();

    void OnEnable()
    {
        playerName = PlayerPrefs.GetString("PlayerNickname", "Player");
        RefreshNotes();
    }

    public void RefreshNotes()
    {
        StartCoroutine(api.GetNotes(OnNotesLoaded));
    }

    void OnNotesLoaded(List<ServerNoteEntry> notes)
    {
        if (notes == null)
            return;

        allNotes = notes;

        var ui = FindFirstObjectByType<ServerMessageUIManager>();
        if (ui != null)
            ui.RefreshWithServerNotes(allNotes);
    }

    public void CreateNote(string text)
    {
        int currentLoop = GameManager.Instance.GetLoopCount();

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
