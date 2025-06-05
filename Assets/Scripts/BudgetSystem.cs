using System;
using UnityEngine;

public class BudgetSystem : MonoBehaviour
{
    public static BudgetSystem Instance;

    public int monthlyIncome = 0;
    public int monthlyExpenses = 0;

    public event Action OnBudgetUpdated;

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

    // Call this to add income source (e.g. salary, dividends)
    public void AddIncome(int amount)
    {
        monthlyIncome += amount;
        Debug.Log($"[BudgetSystem] Income increased by {amount}, total income: {monthlyIncome}");
        OnBudgetUpdated?.Invoke();
    }

    // Remove income source
    public void RemoveIncome(int amount)
    {
        monthlyIncome = Mathf.Max(0, monthlyIncome - amount);
        Debug.Log($"[BudgetSystem] Income decreased by {amount}, total income: {monthlyIncome}");
        OnBudgetUpdated?.Invoke();
    }

    // Add recurring expense (e.g. rent, bills, loan payments)
    public void AddExpense(int amount)
    {
        monthlyExpenses += amount;
        Debug.Log($"[BudgetSystem] Expense increased by {amount}, total expenses: {monthlyExpenses}");
        OnBudgetUpdated?.Invoke();
    }

    // Remove recurring expense
    public void RemoveExpense(int amount)
    {
        monthlyExpenses = Mathf.Max(0, monthlyExpenses - amount);
        Debug.Log($"[BudgetSystem] Expense decreased by {amount}, total expenses: {monthlyExpenses}");
        OnBudgetUpdated?.Invoke();
    }

    // Calculate net monthly cash flow
    public int GetNetCashFlow()
    {
        return monthlyIncome - monthlyExpenses;
    }

    // Call this each month to apply cash flow effect to player
    public void ApplyMonthlyBudget()
    {
        int netFlow = GetNetCashFlow();
        PlayerStats.Instance.ChangeCash(netFlow);
        Debug.Log($"[BudgetSystem] Monthly budget applied. Net cash flow: {netFlow}");
    }
}
