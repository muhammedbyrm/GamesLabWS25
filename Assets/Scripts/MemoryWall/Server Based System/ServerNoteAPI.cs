using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerNoteAPI : MonoBehaviour
{
    public string projectUrl = "https://YOUR_PROJECT.supabase.co";
    public string anonKey = "YOUR_ANON_KEY";

    string NotesEndpoint => projectUrl + "/rest/v1/Notes";

    public IEnumerator GetNotes(System.Action<List<ServerNoteEntry>> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get(NotesEndpoint + "?select=*");
        req.SetRequestHeader("apikey", anonKey);
        req.SetRequestHeader("Authorization", "Bearer " + anonKey);
        req.SetRequestHeader("Accept", "application/json");

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

    public IEnumerator AddNote(ServerNoteEntry entry, System.Action<bool> callback)
    {
        string jsonBody = "[" + JsonUtility.ToJson(entry) + "]";
        UnityWebRequest req = new UnityWebRequest(NotesEndpoint, "POST");

        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("apikey", anonKey);
        req.SetRequestHeader("Authorization", "Bearer " + anonKey);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();

        callback(req.result == UnityWebRequest.Result.Success);
    }

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

        callback(req.result == UnityWebRequest.Result.Success);
    }

    public IEnumerator CheckSenderExists(string senderLower, System.Action<bool> callback)
    {
        string sender = UnityWebRequest.EscapeURL(senderLower);
        string url = NotesEndpoint + "?sender=ilike." + sender + "&select=sender";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("apikey", anonKey);
        req.SetRequestHeader("Authorization", "Bearer " + anonKey);
        req.SetRequestHeader("Accept", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            callback(false);
            yield break;
        }

        callback(req.downloadHandler.text != "[]");
    }

    [System.Serializable]
    class NotesWrapper
    {
        public List<ServerNoteEntry> list;
    }
}
