using System.Collections;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    public int hour = 0;
    public int day = 1;
    public float tickInterval = 1f; // Seconds per hour tick

    [Header("UI")]
    public TextMeshProUGUI timeText;

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
            return;
        }
    }

    private void Start()
    {
        if (timeText == null)
        {
            //Debug.LogWarning("TimeManager: timeText is not assigned!");
        }
        UpdateTimeUI();
        StartCoroutine(TimeTickCoroutine());
    }

    private IEnumerator TimeTickCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
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
            // Example call when a day passes - replace with your actual logic
            CreditManager creditManager = FindFirstObjectByType<CreditManager>();
            if (creditManager != null)
                creditManager.ApplyInterest();
        }

        UpdateTimeUI();

        // Example calls on every tick - replace with your actual managers
        BudgetManager.Instance?.SimulateIncomeExpense();
        StockMarketManager stockMarket = FindFirstObjectByType<StockMarketManager>();
        if (stockMarket != null)
            stockMarket.UpdateStockPrices();

        //Debug.Log($"Time advanced to Day {day}, Hour {hour}");
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
        {
            timeText.text = $"Day {day}, Hour {hour:D2}:00";
        }
    }

    // Optional: method to externally set the timeText UI element
    public void SetTimeText(TextMeshProUGUI newTimeText)
    {
        timeText = newTimeText;
        UpdateTimeUI();
    }
}
