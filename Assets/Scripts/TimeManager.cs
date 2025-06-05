// TimeManager.cs
// Manages the game's time progression (hours, days, weeks, quarters, years).

using System;
using System.Collections;
using TMPro;
using UnityEngine;

// --- New: Class to wrap TimeManager settings for JSON deserialization ---
[Serializable]
public class TimeSettings
{
    public float baseTimeStep = 1f; // Real-time seconds per game hour at 1x speed
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public TextMeshProUGUI timeText;

    private float baseTimeStep = 1f; // This will now be loaded from JSON
    private float speedMultiplier = 1f; // 0 = paused, 1 = normal, 2 = double, 4 = quadruple

    private int hour = 0;
    private int day = 1;
    private int quarter = 1;
    private int year = 1;
    private int week = 1; // Track current week

    private Coroutine timeCoroutine;

    // Delegate + event to notify listeners on each hour advance
    public delegate void HourAdvancedHandler(int year, int quarter, int day, int hour);
    public event HourAdvancedHandler OnHourAdvanced;

    // --- NEW: Event to notify listeners on each week advance ---
    public event Action OnWeekAdvanced;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTimeSettings(); // Load settings from JSON
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartClock();
    }

    public void StartClock()
    {
        if (timeCoroutine == null && speedMultiplier > 0f)
        {
            timeCoroutine = StartCoroutine(TimeTicker());
        }
    }

    IEnumerator TimeTicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(baseTimeStep / speedMultiplier);
            AdvanceTime();
        }
    }

    private void AdvanceTime()
    {
        hour++;

        // Notify subscribers of hour advance
        OnHourAdvanced?.Invoke(year, quarter, day, hour);

        if (hour >= 24) // End of a day
        {
            hour = 0;
            day++;

            // Notify subscribers of day advance (if needed, not explicitly defined as an event here)
            // OnDayAdvanced?.Invoke(); // Uncomment if you add this event

            // --- NEW: Check for Week Advance ---
            // A new week starts every 7 days. If 'day' becomes 8, 15, 22, etc., it's a new week.
            // Or simpler, check when 'day' is a multiple of 7 relative to the start of the year/quarter,
            // or just trigger weekly checks if 'day' is day 7, 14, 21 etc.
            // For simplicity, let's trigger it at the start of each new week after day 1.
            if ((day -1) % 7 == 0 && day > 1) // On Day 8, Day 15, Day 22, etc. (after initial week 1 is done)
            {
                AdvanceWeek();
            }

            if (day > 90) // Assuming 90 days per quarter
            {
                day = 1;
                quarter++;
                week = 1; // Reset week count for new quarter

                if (quarter > 4) // End of a year
                {
                    quarter = 1;
                    year++;
                }
            }
        }

        Debug.Log($"[TimeManager] Y{year} • Q{quarter} • D{day} • {hour:00}:00");

        // Update stocks every hour
        if (StockMarketSystem.Instance != null)
        {
            StockMarketSystem.Instance.UpdateAllPrices();
        }

        UpdateTimeText();
    }

    /// <summary>
    /// Advances the game time by one week.
    /// This method is called internally when a new week begins.
    /// </summary>
    private void AdvanceWeek()
    {
        week++;
        OnWeekAdvanced?.Invoke(); // Invoke the OnWeekAdvanced event
        Debug.Log($"[TimeManager] Week advanced: Week {week}.");
    }

    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            timeText.text = $"Year {year} • Q{quarter} • Day {day} • {hour:00}:00";
        }
    }

    public void PauseClock()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
            Debug.Log("[TimeManager] Clock paused.");
        }
        speedMultiplier = 0f;
    }

    public void ResumeClock()
    {
        if (speedMultiplier <= 0f)
        {
            speedMultiplier = 1f;
        }

        if (timeCoroutine == null)
        {
            timeCoroutine = StartCoroutine(TimeTicker());
            Debug.Log("[TimeManager] Clock resumed.");
        }
    }

    // Set speed multiplier: 0 = pause, 1 = normal, 2 = double, 4 = quadruple
    public void SetSpeed(float newSpeed)
    {
        if (newSpeed <= 0f)
        {
            PauseClock();
            Debug.Log("[TimeManager] Speed set to pause.");
            return;
        }

        speedMultiplier = newSpeed;
        Debug.Log($"[TimeManager] Speed set to x{speedMultiplier}");

        // Restart coroutine to apply new speed immediately
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = StartCoroutine(TimeTicker());
        }
        else // If for some reason it was null but speed > 0, start it.
        {
            timeCoroutine = StartCoroutine(TimeTicker());
        }
    }

    /// <summary>
    /// Loads time settings from a JSON file in the Resources folder.
    /// </summary>
    private void LoadTimeSettings()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("time_settings"); // Assumes settings JSON is named "time_settings.json"
        if (jsonFile != null)
        {
            TimeSettings settings = JsonUtility.FromJson<TimeSettings>(jsonFile.text);
            baseTimeStep = settings.baseTimeStep;
            Debug.Log($"[TimeManager] Loaded time settings from JSON. Base Time Step: {baseTimeStep}");
        }
        else
        {
            Debug.LogWarning("[TimeManager] 'time_settings.json' not found in Resources folder. Using default baseTimeStep.");
            // Keep baseTimeStep at its default if JSON is not found.
        }
    }
}
