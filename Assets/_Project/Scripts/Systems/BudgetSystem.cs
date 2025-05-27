// File: Assets/Scripts/Systems/BudgetSystem.cs
using UnityEngine;
using System; // For Action
using UnityEngine.Events; // Needed for UnityEvent

public class BudgetSystem : MonoBehaviour
{
    public static BudgetSystem Instance { get; private set; }

    private float _income = 5000f; // Made private for property setter
    public float Income
    {
        get { return _income; }
        set
        {
            _income = value;
            onIncomeChanged?.Invoke(_income); // FIX: Invoke event
            UpdateNetWorth(); // Income change can affect net worth indirectly over time
        }
    }

    private float _expenses = 2000f; // Made private for property setter
    public float Expenses
    {
        get { return _expenses; }
        set
        {
            _expenses = value;
            onExpensesChanged?.Invoke(_expenses); // FIX: Invoke event
            UpdateNetWorth();
        }
    }

    private float _cashOnHand = 10000f; // Made private for property setter
    public float cashOnHand // Public property for external access, still using onCashChanged
    {
        get { return _cashOnHand; }
        set
        {
            _cashOnHand = value;
            onCashChanged?.Invoke(_cashOnHand); // FIX: Invoke event
        }
    }

    public float stockAssets = 0f; // This will be managed by StockTradeSystem
    public float loanDebt = 0f; // This will be managed by CreditSystem
    public float netWorth;

    // Event for net worth changes (for GameManager to subscribe to)
    public event Action<float> onNetWorthChanged;

    // FIX: Add UnityEvents for specific budget changes
    public UnityEvent<float> onCashChanged;
    public UnityEvent<float> onIncomeChanged;
    public UnityEvent<float> onExpensesChanged;

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

        // FIX: Initialize the UnityEvents
        if (onCashChanged == null) onCashChanged = new UnityEvent<float>();
        if (onIncomeChanged == null) onIncomeChanged = new UnityEvent<float>();
        if (onExpensesChanged == null) onExpensesChanged = new UnityEvent<float>();
    }

    void Start()
    {
        // Subscribe to TimeManager's hour advance event for income/expense
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced += OnHourAdvancedForBudget;
        }
        UpdateNetWorth();
        // Initial UI updates are now triggered by events
        onCashChanged?.Invoke(cashOnHand);
        onIncomeChanged?.Invoke(Income);
        onExpensesChanged?.Invoke(Expenses);
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced -= OnHourAdvancedForBudget;
        }
    }

    void OnHourAdvancedForBudget()
    {
        // Simulate income/expense less frequently, e.g., daily (every 24 hours)
        if (TimeManager.Instance.hour == 0) // End of a day
        {
            SimulateIncomeExpense();
        }
    }

    public void UpdateNetWorth()
    {
        netWorth = cashOnHand + stockAssets - loanDebt;
        onNetWorthChanged?.Invoke(netWorth); // Notify subscribers (GameManager, AchievementSystem)
        // DashboardUI.Instance?.UpdateBudgetUI(income, expenses, cashOnHand, netWorth); // FIX: This line can be removed as individual events update the UI
    }

    public void SimulateIncomeExpense()
    {
        cashOnHand += Income - Expenses; // FIX: Use Income and Expenses properties
        Debug.Log($"Income/Expense cycle: Cash {Income} - {Expenses}. New cash: {cashOnHand:F2}");
        UpdateNetWorth();
    }

    // FIX: Add CanAfford method
    public bool CanAfford(float amount)
    {
        return cashOnHand >= amount;
    }
}