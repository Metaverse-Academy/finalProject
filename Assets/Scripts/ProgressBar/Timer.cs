using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float startTimeSeconds = 300f;  
    [SerializeField] private Text timerText;
    [SerializeField] private bool autoStart = true;

    [Header("On End")]
    [SerializeField] private GameObject dayOverPanel;         
    [SerializeField] private bool pauseGameOnEnd = true;      
    public UnityEvent onTimerEnded;      
    

    private float timeLeft;
    private bool timerOn;

    private void Start()
    {
        if (dayOverPanel != null) dayOverPanel.SetActive(false);

        timeLeft = Mathf.Max(0f, startTimeSeconds);
        UpdateTimerDisplay(timeLeft);

        timerOn = autoStart;
    }

    private void Update()
    {
        if (!timerOn) return;

        // ‰ﬁ’ «·Êﬁ 
        timeLeft = Mathf.Max(0f, timeLeft - Time.unscaledDeltaTime); 
        UpdateTimerDisplay(timeLeft);

        // «‰ ÂÏ «·Êﬁ ø
        if (timeLeft <= 0f)
        {
            EndDay();
        }
    }

    private void EndDay()
    {
        if (!timerOn) return; // „‰⁄ «· ﬂ—«—
        timerOn = false;

        if (pauseGameOnEnd) Time.timeScale = 0f;

        if (dayOverPanel != null) dayOverPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        onTimerEnded?.Invoke();

        Debug.Log("Time is up! Day finished.");
    }

    private void UpdateTimerDisplay(float currentTime)
    {
        int total = Mathf.CeilToInt(currentTime);
        int minutes = total / 60;
        int seconds = total % 60;

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (dayOverPanel != null) dayOverPanel.SetActive(false);
    }

    public void ResetAndStart(float newStartSeconds = -1f)
    {
        if (newStartSeconds > 0f) startTimeSeconds = newStartSeconds;
        timeLeft = Mathf.Max(0f, startTimeSeconds);
        UpdateTimerDisplay(timeLeft);

        if (dayOverPanel != null) dayOverPanel.SetActive(false);
        Time.timeScale = 1f;
        timerOn = true;
    }

    public void SetTimerOn(bool on) => timerOn = on;
}
