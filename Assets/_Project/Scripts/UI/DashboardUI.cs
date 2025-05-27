// File: Assets/Scripts/UI/DashboardUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Make sure this is included for Buttons
using System.Collections.Generic; // Needed for List<Stock>
using UnityEngine.Events; // Needed for UnityEvents

public class DashboardUI : MonoBehaviour
{
    public static DashboardUI Instance { get; private set; }

    [Header("Top Bar UI")]
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI netWorthText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI xpLevelText; // For XP and Level display
    public Slider xpBar; // For XP bar
    public TextMeshProUGUI levelText; // Dedicated Level text
    public TextMeshProUGUI timeText;
    [Header("Budget & Credit UI")]
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI loanText;
    public TextMeshProUGUI interestText;
    [Header("Stock Market UI")]
    public Transform stockListContent; // Parent transform for stock entries
    public GameObject stockEntryPrefab; // Prefab for individual stock entries

    // --- NEW FOR MVP: Time Control Buttons ---
    [Header("Time Control UI")]
    public Button pauseButton;
    public Button playSpeed1xButton;
    public Button playSpeed2xButton;
    public Button playSpeed4xButton; // Or whatever speeds you want

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Initial UI updates (ensuring managers are initialized)
        UpdateCash(0); // Dummy update, will be updated by BudgetSystem
        UpdateNetWorth(0);
        UpdateDebt(0);
        UpdateXPUI(0, 100, 0); // Dummy update, passes currentXP, maxXP, progress (0-1)
        UpdateTimeText(2025, 1, 1, 0); // Dummy update

        // Ensure managers are assigned and subscribe to their events
        if (BudgetSystem.Instance != null)
        {
            BudgetSystem.Instance.onCashChanged.AddListener(UpdateCash);
            // FIX: Changed from AddListener to += for standard C# event
            BudgetSystem.Instance.onNetWorthChanged += UpdateNetWorth;
            BudgetSystem.Instance.onIncomeChanged.AddListener(UpdateIncome);
            BudgetSystem.Instance.onExpensesChanged.AddListener(UpdateExpenses);
        }
        if (CreditSystem.Instance != null)
        {
            CreditSystem.Instance.onDebtChanged.AddListener(UpdateDebt);
            CreditSystem.Instance.onInterestRateChanged.AddListener(UpdateInterest);
        }
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onXPChanged.AddListener(UpdateXPUI);
        }
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced += UpdateTimeBasedOnHour;
        }
        if (StockMarketSystem.Instance != null)
        {
            StockMarketSystem.Instance.onStocksInitialized.AddListener(PopulateStockList);
        }

        // --- NEW FOR MVP: Link Time Control Buttons ---
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseButtonClicked);
        if (playSpeed1xButton != null) playSpeed1xButton.onClick.AddListener(() => OnSpeedButtonClicked(1.0f));
        if (playSpeed2xButton != null) playSpeed2xButton.onClick.AddListener(() => OnSpeedButtonClicked(2.0f));
        if (playSpeed4xButton != null) playSpeed4xButton.onClick.AddListener(() => OnSpeedButtonClicked(4.0f));

        // Initial populate if stocks are already ready
        if (StockMarketSystem.Instance != null && StockMarketSystem.Instance.IsInitialized)
        {
            PopulateStockList();
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (BudgetSystem.Instance != null)
        {
            BudgetSystem.Instance.onCashChanged.RemoveListener(UpdateCash);
            // FIX: Changed from RemoveListener to -= for standard C# event
            BudgetSystem.Instance.onNetWorthChanged -= UpdateNetWorth;
            BudgetSystem.Instance.onIncomeChanged.RemoveListener(UpdateIncome);
            BudgetSystem.Instance.onExpensesChanged.RemoveListener(UpdateExpenses);
        }
        if (CreditSystem.Instance != null)
        {
            CreditSystem.Instance.onDebtChanged.RemoveListener(UpdateDebt);
            CreditSystem.Instance.onInterestRateChanged.RemoveListener(UpdateInterest);
        }
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onXPChanged.RemoveListener(UpdateXPUI);
        }
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced -= UpdateTimeBasedOnHour;
        }
        if (StockMarketSystem.Instance != null)
        {
            StockMarketSystem.Instance.onStocksInitialized.RemoveListener(PopulateStockList);
        }

        // --- NEW FOR MVP: Unsubscribe Time Control Buttons ---
        if (pauseButton != null) pauseButton.onClick.RemoveAllListeners();
        if (playSpeed1xButton != null) playSpeed1xButton.onClick.RemoveAllListeners();
        if (playSpeed2xButton != null) playSpeed2xButton.onClick.RemoveAllListeners();
        if (playSpeed4xButton != null) playSpeed4xButton.onClick.RemoveAllListeners();
    }

    // Existing update methods...
    public void UpdateCash(float newCash)
    {
        cashText.text = $"Cash: ${newCash:N0}";
    }

    public void UpdateNetWorth(float newNetWorth)
    {
        netWorthText.text = $"Net Worth: ${newNetWorth:N0}";
    }

    public void UpdateDebt(float newDebt)
    {
        debtText.text = $"Debt: ${newDebt:N0}";
    }

    public void UpdateIncome(float newIncome)
    {
        incomeText.text = $"Income: ${newIncome:N0}/mo";
    }

    public void UpdateExpenses(float newExpenses)
    {
        expensesText.text = $"Expenses: ${newExpenses:N0}/mo";
    }

    public void UpdateInterest(float newInterestRate)
    {
        interestText.text = $"Interest: {newInterestRate:P1}"; // P1 for percentage with 1 decimal
    }

    public void UpdateXPUI(int currentXP, int xpForNextLevel, float progress)
    {
        xpLevelText.text = $"LVL {PlayerStats.Instance.level} | XP: {currentXP}";
        levelText.text = $"Level: {PlayerStats.Instance.level}";
        xpBar.value = progress;
        xpBar.maxValue = 1f;
    }

    public void UpdateTimeText(int year, int month, int day, int hour)
    {
        string formattedHour = hour.ToString("D2") + ":00";
        timeText.text = $"Date: {month}/{day}/{year} | {formattedHour}";
    }

    private void UpdateTimeBasedOnHour()
    {
        if (TimeManager.Instance != null)
        {
            UpdateTimeText(TimeManager.Instance.year, TimeManager.Instance.month, TimeManager.Instance.day, TimeManager.Instance.hour);
        }
    }

    public void UpdateStockDisplays(List<Stock> stocks)
    {
        if (stocks != null && stocks.Count > 0)
        {
            PopulateStockList();
        }
    }

    public void PopulateStockList()
    {
        foreach (Transform child in stockListContent)
        {
            Destroy(child.gameObject);
        }

        if (StockMarketSystem.Instance != null && StockMarketSystem.Instance.CurrentStocks != null)
        {
            foreach (var stock in StockMarketSystem.Instance.CurrentStocks)
            {
                GameObject stockUI = Instantiate(stockEntryPrefab, stockListContent);
                StockUIEntry entry = stockUI.GetComponent<StockUIEntry>();
                if (entry != null)
                {
                    entry.Initialize(stock);
                }
            }
        }
    }

    public void UpdateStockTradeUI(string ticker, int sharesOwned)
    {
        foreach (Transform child in stockListContent)
        {
            StockUIEntry entry = child.GetComponent<StockUIEntry>();
            if (entry != null && entry.GetStockTicker() == ticker)
            {
                entry.UpdateOwnedSharesDisplay(ticker, sharesOwned);
                break;
            }
        }
    }

    // --- NEW FOR MVP: Time Control Methods ---
    private void OnPauseButtonClicked()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.TogglePause();
        }
    }

    private void OnSpeedButtonClicked(float speed)
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.SetTimeScale(speed);
        }
    }

    public void UpdateBudgetUI(float income, float expenses, float cash, float netWorth)
    {
        UpdateIncome(income);
        UpdateExpenses(expenses);
        UpdateCash(cash);
        UpdateNetWorth(netWorth);
    }

    public void UpdateCreditUI(float loanAmount, float interestRate)
    {
        UpdateDebt(loanAmount);
        UpdateInterest(interestRate);
    }
}