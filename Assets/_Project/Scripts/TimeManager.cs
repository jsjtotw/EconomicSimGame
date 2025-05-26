using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public int hour = 0;
    public int day = 1;
    public int month = 1;
    public int year = 2025;

    public float realSecondsPerHour = 1f; // Speed: 1 second = 1 in-game hour

    private float timer = 0f;

    void Awake()
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

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= realSecondsPerHour)
        {
            AdvanceHour();
            timer = 0f;
        }
    }

    void AdvanceHour()
    {
        hour++;
        if (hour >= 24)
        {
            hour = 0;
            day++;
            if (day > 30)
            {
                day = 1;
                month++;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
            }
        }

        // Trigger system updates on every in-game hour
        BudgetManager.Instance?.SimulateIncomeExpense();
        CreditManager credit = FindObjectOfType<CreditManager>();
        credit?.ApplyInterest();

        StockMarketManager stockManager = FindObjectOfType<StockMarketManager>();
        stockManager?.UpdateStockPrices();
    }
}
