using System;
using UnityEngine;

public class GameMangarI : MonoBehaviour
{
    public static GameMangarI Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countDownToStartTimer = 3f;
    private float gamePlayingTimer = 1000f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameMangarI instance!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        state = State.WaitingToStart;
        Debug.Log("GameMangarI initialized with state: " + state);
    }

    private void Start()
    {
        // Ensure we start in the correct state
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Game state changed to: " + state);
                }
                break;
            case State.CountdownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer <= 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Game state changed to: " + state);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Game state changed to: " + state);
                }
                break;
            case State.GameOver:
                // Implement game over logic here
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countDownToStartTimer;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    // Helper method to check if player can interact
    public bool CanPlayerInteract()
    {
        return state == State.GamePlaying || state == State.CountdownToStart;
    }
}