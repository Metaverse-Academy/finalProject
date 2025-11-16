using System;
using UnityEngine;
using UnityEngine.Pool;

public class GameMangarI : MonoBehaviour
{
    public static GameMangarI Instance { get; private set; }

    public event EventHandler OnStateChanged;
    private enum State
    {
        WaitingToStsrt,
        CounrdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countDownToStartTimer = 3f;
    private float gamePlayingTimer = 2f;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStsrt;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStsrt:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    state = State.CounrdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CounrdownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer <= 0f)
                {
                    state = State.GamePlaying; // Transition to GamePlaying for demonstration
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                // Implement game over logic here
                break;
        }
        Debug.Log(state);
    }



    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CounrdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countDownToStartTimer;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }
}




