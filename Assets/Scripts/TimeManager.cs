using System.Collections;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public TextMeshProUGUI timeText;

    public float baseTimeStep = 1f; // Real-time seconds per game hour at 1x speed
    private float speedMultiplier = 1f; // 0 = paused, 1 = normal, 2 = double, 4 = quadruple

    private int hour = 0;
    private int day = 1;
    private int quarter = 1;
    private int year = 1;

    private Coroutine timeCoroutine;

    // Delegate + event to notify listeners on each hour advance
    public delegate void HourAdvancedHandler(int year, int quarter, int day, int hour);
    public event HourAdvancedHandler OnHourAdvanced;

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

        if (hour >= 24)
        {
            hour = 0;
            day++;

            if (day > 90)
            {
                day = 1;
                quarter++;

                if (quarter > 4)
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

        // Notify subscribers
        OnHourAdvanced?.Invoke(year, quarter, day, hour);
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
        else
        {
            timeCoroutine = StartCoroutine(TimeTicker());
        }
    }
}
