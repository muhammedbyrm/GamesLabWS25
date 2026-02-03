using UnityEngine;

[CreateAssetMenu(fileName = "TaskData", menuName = "Scriptable Objects/TaskData")]
public class TaskData : ScriptableObject
{
    public string taskTitle;
    public string taskDescription;
}
