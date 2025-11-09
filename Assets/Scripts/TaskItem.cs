using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Separate script for task items
public class TaskItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskNameText;
    [SerializeField] private Image checkboxImage;
    [SerializeField] private Sprite checkedSprite;
    [SerializeField] private Sprite uncheckedSprite;

    private string taskName;
    private bool isCompleted;

    public string TaskName => taskName;
    public bool IsCompleted => isCompleted;

    public void Initialize(string name, bool completed)
    {
        taskName = name;
        SetCompleted(completed);
    }

    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (taskNameText != null)
        {
            taskNameText.text = taskName;

            if (isCompleted)
            {
                taskNameText.fontStyle = FontStyles.Strikethrough;
                taskNameText.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }
            else
            {
                taskNameText.fontStyle = FontStyles.Normal;
                taskNameText.color = Color.white;
            }
        }
    }
}