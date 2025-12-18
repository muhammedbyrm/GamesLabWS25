using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerNoteAPI : MonoBehaviour
{
    [Header("Supabase Settings")]
    public string projectUrl = "https://YOUR_PROJECT.supabase.co";
    public string anonKey = "YOUR_ANON_KEY";

    private string NotesEndpoint => projectUrl + "/rest/v1/Notes";

    // GET NOTES
    public IEnumerator GetNotes(System.Action<List<ServerNoteEntry>> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get(NotesEndpoint + "?select=*");

        req.SetRequestHeader("apikey", anonKey);
        req.SetRequestHeader("Authorization", "Bearer " + anonKey);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            NotesWrapper wrapper = JsonUtility.FromJson<NotesWrapper>("{\"list\":" + json + "}");
            callback(wrapper.list);
        }
        else
        {
            callback(null);
        }
    }

    // POST NOTE
    public IEnumerator AddNote(ServerNoteEntry entry, System.Action<bool> callback)
    {
        string jsonBody = "[" + JsonUtility.ToJson(entry) + "]";

        UnityWebRequest req = new UnityWebRequest(NotesEndpoint, "POST");
        byte[] body = Encoding.UTF8.GetBytes(jsonBody);

        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("apikey", anonKey);
        req.SetRequestHeader("Authorization", "Bearer " + anonKey);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();

        callback(req.result == UnityWebRequest.Result.Success);
    }

    // delete note by sender
    public IEnumerator DeleteNote(ServerNoteEntry entry, System.Action<bool> callback)
    {
        string sender = UnityWebRequest.EscapeURL(entry.sender);
        string msg = UnityWebRequest.EscapeURL(entry.fullMessage);

        string url = $"{NotesEndpoint}?sender=eq.{sender}&fullMessage=eq.{msg}";

        UnityWebRequest req = UnityWebRequest.Delete(url);

        req.SetRequestHeader("apikey", anonKey);
        req.SetRequestHeader("Authorization", "Bearer " + anonKey);
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        callback(ok);
    }

    [System.Serializable]
    private class NotesWrapper
    {
        public List<ServerNoteEntry> list;
    }
}
