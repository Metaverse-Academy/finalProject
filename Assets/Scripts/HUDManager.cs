using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    [Header("مراجع واجهة المستخدم")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image cooperationFillBar;
    [SerializeField] private Transform taskListContainer;
    [SerializeField] private GameObject taskItemPrefab;

    [Header("نوع الأصول")]
    [SerializeField] private bool use3DModels = false; // اختر: false = 2D sprites, true = 3D models

    [Header("النجوم 2D (Sprites)")]
    [SerializeField] private Image[] starImages2D;
    [SerializeField] private Sprite starFilled2D;
    [SerializeField] private Sprite starEmpty2D;

    [Header("النجوم 3D (Models)")]
    [SerializeField] private GameObject[] starModels3D; // النماذج الموجودة في المشهد
    [SerializeField] private Material starFilledMaterial; // مادة للنجمة الممتلئة
    [SerializeField] private Material starEmptyMaterial; // مادة للنجمة الفارغة
    [SerializeField] private float starRotationSpeed = 50f; // سرعة دوران النجوم

    [Header("الإعدادات")]
    [SerializeField] private float levelTimeInSeconds = 150f;
    
    private float currentTime;
    private int currentStars = 0;
    private float cooperationValue = 0.67f;
    private List<TaskItem> tasks = new List<TaskItem>();
    private Renderer[] starRenderers3D;

    private void Start()
    {
        currentTime = levelTimeInSeconds;
        
        // إعداد النجوم 3D إذا كانت مفعلة
        if (use3DModels && starModels3D.Length > 0)
        {
            starRenderers3D = new Renderer[starModels3D.Length];
            for (int i = 0; i < starModels3D.Length; i++)
            {
                starRenderers3D[i] = starModels3D[i].GetComponent<Renderer>();
            }
        }
        
        InitializeHUD();
    }

    private void Update()
    {
        UpdateTimer();
        
        // تدوير النجوم 3D
        if (use3DModels)
        {
            RotateStars3D();
        }
    }

    private void InitializeHUD()
    {
        UpdateStars(2);
        UpdateCooperationMeter(cooperationValue);
    }

    #region المؤقت
    private void UpdateTimer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
        else
        {
            currentTime = 0;
            timerText.text = "0:00";
            OnTimerEnd();
        }
    }

    public void SetTime(float seconds)
    {
        currentTime = seconds;
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }

    private void OnTimerEnd()
    {
        Debug.Log("انتهى الوقت!");
    }
    #endregion

    #region النجوم
    public void UpdateStars(int starCount)
    {
        starCount = Mathf.Clamp(starCount, 0, 3);
        currentStars = starCount;

        if (use3DModels)
        {
            UpdateStars3D(starCount);
        }
        else
        {
            UpdateStars2D(starCount);
        }
    }

    private void UpdateStars2D(int starCount)
    {
        if (starImages2D == null || starImages2D.Length == 0) return;

        for (int i = 0; i < starImages2D.Length; i++)
        {
            if (i < starCount)
            {
                starImages2D[i].sprite = starFilled2D;
            }
            else
            {
                starImages2D[i].sprite = starEmpty2D;
            }
        }
    }

    private void UpdateStars3D(int starCount)
    {
        if (starModels3D == null || starModels3D.Length == 0) return;

        for (int i = 0; i < starModels3D.Length; i++)
        {
            if (i < starCount)
            {
                // نجمة ممتلئة
                if (starRenderers3D[i] != null)
                {
                    starRenderers3D[i].material = starFilledMaterial;
                }
                starModels3D[i].transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                // نجمة فارغة
                if (starRenderers3D[i] != null)
                {
                    starRenderers3D[i].material = starEmptyMaterial;
                }
                starModels3D[i].transform.localScale = Vector3.one;
            }
        }
    }

    private void RotateStars3D()
    {
        if (starModels3D == null) return;

        for (int i = 0; i < starModels3D.Length; i++)
        {
            if (i < currentStars) // فقط النجوم الممتلئة تدور
            {
                starModels3D[i].transform.Rotate(Vector3.up, starRotationSpeed * Time.deltaTime);
            }
        }
    }

    public void AddStar()
    {
        if (currentStars < 3)
        {
            UpdateStars(currentStars + 1);
        }
    }

    public void RemoveStar()
    {
        if (currentStars > 0)
        {
            UpdateStars(currentStars - 1);
        }
    }
    #endregion

    #region مقياس التعاون
    public void UpdateCooperationMeter(float value)
    {
        cooperationValue = Mathf.Clamp01(value);
        cooperationFillBar.fillAmount = cooperationValue;

        if (cooperationValue >= 0.7f)
        {
            cooperationFillBar.color = new Color(0.3f, 0.8f, 0.3f); // أخضر
        }
        else if (cooperationValue >= 0.4f)
        {
            cooperationFillBar.color = new Color(1f, 0.8f, 0.2f); // أصفر
        }
        else
        {
            cooperationFillBar.color = new Color(0.9f, 0.3f, 0.3f); // أحمر
        }
    }

    public void ModifyCooperation(float delta)
    {
        UpdateCooperationMeter(cooperationValue + delta);
    }

    public float GetCooperationValue()
    {
        return cooperationValue;
    }
    #endregion

    #region المهام
    public void AddTask(string taskName, bool completed = false)
    {
        GameObject taskObj = Instantiate(taskItemPrefab, taskListContainer);
        TaskItem taskItem = taskObj.GetComponent<TaskItem>();
        
        if (taskItem != null)
        {
            taskItem.Initialize(taskName, completed);
            tasks.Add(taskItem);
        }
    }

    public void CompleteTask(string taskName)
    {
        TaskItem task = tasks.Find(t => t.TaskName == taskName);
        if (task != null)
        {
            task.SetCompleted(true);
        }
    }

    public void ClearTasks()
    {
        foreach (TaskItem task in tasks)
        {
            Destroy(task.gameObject);
        }
        tasks.Clear();
    }

    public bool AreAllTasksComplete()
    {
        foreach (TaskItem task in tasks)
        {
            if (!task.IsCompleted)
                return false;
        }
        return tasks.Count > 0;
    }
    #endregion

    #region وظائف مساعدة
    public float GetTimeRemaining()
    {
        return currentTime;
    }

    public int GetCurrentStars()
    {
        return currentStars;
    }
    #endregion
}

