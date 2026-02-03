using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TaskUIManager : MonoBehaviour
{
    public UIDocument taskUIDocument;
    private Label taskLabel;
    private Label descriptionLabel;
    private VisualElement root;

    [SerializeField] private List<TaskData> allTasks = new();
    #region TaskManager
    private static TaskUIManager _instance;
    public static TaskUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("TaskManager is NUll");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    void Start()
    {
        taskLabel = taskUIDocument.rootVisualElement.Q<Label>("Task");
        descriptionLabel = taskUIDocument.rootVisualElement.Q<Label>("Description");
        root = taskUIDocument.rootVisualElement.Q<VisualElement>("TaskBackground");

        ChangeTask(0);

        //ChangeVisbility(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeTask(int number)
    {
        if (number < 0 || number >= allTasks.Count)
        {
            Debug.LogError("Task number out of range");
            return;
        }
        TaskData task = allTasks[number];
        taskLabel.text = task.taskTitle;
        descriptionLabel.text = task.taskDescription;
    }

    public void ChangeVisbility(bool visible)
    {
        if (visible)
        {
            //taskUIDocument.rootVisualElement.RemoveFromClassList("hidden");
            root.style.opacity = 1f;
        }
        else
        {
            //taskUIDocument.rootVisualElement.AddToClassList("hidden");
            root.style.opacity = 0f;
        }
    }
}
