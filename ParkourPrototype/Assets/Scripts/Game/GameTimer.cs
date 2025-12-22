using UnityEngine;
using UnityEngine.UI;
using System;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [SerializeField] private Text timerText; // Assign in Inspector
    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool hasStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }

        // Start timer on first WASD input
        if (!hasStarted && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                           Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if (!hasStarted)
        {
            isRunning = true;
            hasStarted = true;
            Debug.Log("Timer started");
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer stopped at: {FormatTime(elapsedTime)}");
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        hasStarted = false;
        UpdateTimerDisplay();
    }

    public float GetCurrentTime() => elapsedTime;

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    public static string FormatTime(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:00}:{1:00}.{2:00}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds / 10);
    }
}