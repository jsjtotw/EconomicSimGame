// File: Assets/Scripts/Core/TimeManager.cs
using UnityEngine;
using System; // For Action

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    public float timeScale = 1.0f; // Multiplier for how fast real-time passes in game-time. 1.0f is normal.
    public float secondsPerGameHour = 1.0f; // How many real seconds equal one game hour
    public bool isPaused = false;

    [Header("Current Game Time")]
    public int year = 2025;
    public int month = 1; // 1-12
    public int day = 1;   // 1-30 (assuming 30 days per month for simplicity)
    public int hour = 0;  // 0-23

    private float timer = 0f;

    // Events for other systems to subscribe to
    public event Action onHourAdvanced; // Triggered every game hour
    public event Action onDayAdvanced;  // Triggered when a new game day starts
    public event Action onMonthAdvanced; // Triggered when a new game month starts
    public event Action onYearAdvanced;  // Triggered when a new game year starts

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep TimeManager active across scenes
        }
    }

    void Start()
    {
        // Initial UI update
        DashboardUI.Instance?.UpdateTimeText(year, month, day, hour);
    }

    void Update()
    {
        if (isPaused || GameManager.Instance.gameEnded) return; // Don't advance time if paused or game ended

        timer += Time.deltaTime * timeScale; // Apply the timeScale multiplier

        if (timer >= secondsPerGameHour)
        {
            timer -= secondsPerGameHour; // Subtract exactly one hour's worth of time
            AdvanceHour();
        }
    }

    void AdvanceHour()
    {
        hour++;
        onHourAdvanced?.Invoke(); // Notify subscribers for hourly updates

        if (hour >= 24)
        {
            hour = 0;
            AdvanceDay();
        }

        // Update UI with current time
        DashboardUI.Instance?.UpdateTimeText(year, month, day, hour);
    }

    void AdvanceDay()
    {
        day++;
        onDayAdvanced?.Invoke(); // Notify subscribers for daily updates

        if (day > 30) // Assuming 30 days per month for simplicity
        {
            day = 1;
            AdvanceMonth();
        }
    }

    void AdvanceMonth()
    {
        month++;
        onMonthAdvanced?.Invoke(); // Notify subscribers for monthly updates

        if (month > 12)
        {
            month = 1;
            AdvanceYear();
        }
    }

    void AdvanceYear()
    {
        year++;
        onYearAdvanced?.Invoke(); // Notify subscribers for yearly updates
        Debug.Log($"New Year: {year}");
    }

    public void SetTimeScale(float newScale)
    {
        timeScale = Mathf.Max(0f, newScale); // Ensure it's not negative
        Debug.Log($"Time scale set to: {timeScale}");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : timeScale; // Unity's global time scale for physics/animations
        Debug.Log($"Game {(isPaused ? "Paused" : "Unpaused")}");
    }
    
    // You can also expose methods to jump time for testing or specific game events
    public void SkipDays(int daysToSkip)
    {
        for (int i = 0; i < daysToSkip; i++)
        {
            for (int h = hour; h < 24; h++) // Finish current day's hours
            {
                AdvanceHour();
            }
            // After finishing current day, start advancing next day
            if (i < daysToSkip - 1) // Don't call AdvanceHour twice for the last day
            {
                 hour = 0; // Reset hour for the new day
            }
        }
        Debug.Log($"Skipped {daysToSkip} days. Current time: {year}/{month}/{day} {hour}:00");
    }
}