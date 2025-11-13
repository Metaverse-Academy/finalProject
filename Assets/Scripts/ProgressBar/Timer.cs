using UnityEngine;

public class Timer : MonoBehaviour
{
    public int minutes = 3;
    public int seconds = 60;
    private float totalTime;
    private float remainingTime;
    private bool TimeUp;

    public void Start()
    {
        totalTime = (minutes * 60) + seconds;
        remainingTime = totalTime;
    }
    public void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
            {
                remainingTime = 0;
            }
        }
    }
    public float GetRemainingTime()
    {
        return remainingTime;
    }
    public float GetTotalTime()
    {
        return totalTime;
    }
    public bool IsTimeUp()
    {
        return remainingTime <= 0;
    }
}
