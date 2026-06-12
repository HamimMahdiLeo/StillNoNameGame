using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    [Header("Tasks")]
    // Tasks are defined here in order — edit freely
    private List<string> tasks = new List<string>
    {
        "Clock in at the punch machine.",
        "Pick up the company files from your desk.",
        "Transfer the files to the server room.",
    };

    private int currentTaskIndex = -1;

    [Header("Memo UI")]
    public GameObject memoPanel;        // the full memo UI panel
    public Text taskText;               // text field inside the memo
    public Text taskNumberText;         // e.g. "Task 1 / 3" (optional)

    private bool memoOpen = false;
    private bool tasksStarted = false;

    void Start()
    {
        if (memoPanel != null)
            memoPanel.SetActive(false);
    }

    void Update()
    {
        if (!tasksStarted) return;

        // Tab toggles memo
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            memoOpen = !memoOpen;
            memoPanel.SetActive(memoOpen);
        }
    }

    // Called by OpeningSequence when Fokyu's call ends
    public void StartTasks()
    {
        tasksStarted = true;
        AdvanceTask();
    }

    // Call this when player completes the current task
    public void CompleteCurrentTask()
    {
        AdvanceTask();
    }

    void AdvanceTask()
    {
        currentTaskIndex++;

        if (currentTaskIndex >= tasks.Count)
        {
            // All tasks done
            SetMemoText("All tasks complete.");
            return;
        }

        SetMemoText(tasks[currentTaskIndex]);
    }

    void SetMemoText(string text)
    {
        if (taskText != null)
            taskText.text = text;

        if (taskNumberText != null)
        {
            if (currentTaskIndex < tasks.Count)
                taskNumberText.text = $"Task {currentTaskIndex + 1} / {tasks.Count}";
            else
                taskNumberText.text = "";
        }
    }

    public int GetCurrentTaskIndex() => currentTaskIndex;
}