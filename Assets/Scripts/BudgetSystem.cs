// BudgetSystem.cs
// Manages monthly income, expenses, and net cash flow.

using System;
using UnityEngine;

public class BudgetSystem : MonoBehaviour
{
    // Singleton instance for BudgetSystem.
    public static BudgetSystem Instance;

    public int monthlyIncome = 0;
    public int monthlyExpenses = 0;

    // New: Multiplier for bonus income from certain events (e.g., for Retail company).
    public float bonusIncomeMultiplier = 1.0f; // Default: no bonus

    // Event triggered when budget figures are updated.
    public event Action OnBudgetUpdated;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern.
    /// </summary>
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

    /// <summary>
    /// Adds an amount to the total monthly income.
    /// </summary>
    /// <param name="amount">The amount of income to add.</param>
    public void AddIncome(int amount)
    {
        monthlyIncome += amount;
        Debug.Log($"[BudgetSystem] Income increased by {amount}, total income: {monthlyIncome}");
        OnBudgetUpdated?.Invoke(); // Notify subscribers of budget change.
    }

    /// <summary>
    /// Removes an amount from the total monthly income.
    /// Income will not go below zero.
    /// </summary>
    /// <param name="amount">The amount of income to remove.</param>
    public void RemoveIncome(int amount)
    {
        monthlyIncome = Mathf.Max(0, monthlyIncome - amount);
        Debug.Log($"[BudgetSystem] Income decreased by {amount}, total income: {monthlyIncome}");
        OnBudgetUpdated?.Invoke(); // Notify subscribers of budget change.
    }

    /// <summary>
    /// Adds a recurring expense to the total monthly expenses.
    /// </summary>
    /// <param name="amount">The amount of expense to add.</param>
    public void AddExpense(int amount)
    {
        monthlyExpenses += amount;
        Debug.Log($"[BudgetSystem] Expense increased by {amount}, total expenses: {monthlyExpenses}");
        OnBudgetUpdated?.Invoke(); // Notify subscribers of budget change.
    }

    /// <summary>
    /// Removes a recurring expense from the total monthly expenses.
    /// Expenses will not go below zero.
    /// </summary>
    /// <param name="amount">The amount of expense to remove.</param>
    public void RemoveExpense(int amount)
    {
        monthlyExpenses = Mathf.Max(0, monthlyExpenses - amount);
        Debug.Log($"[BudgetSystem] Expense decreased by {amount}, total expenses: {monthlyExpenses}");
        OnBudgetUpdated?.Invoke(); // Notify subscribers of budget change.
    }

    /// <summary>
    /// Calculates the net monthly cash flow (income - expenses).
    /// </summary>
    /// <returns>The net cash flow for the month.</returns>
    public int GetNetCashFlow()
    {
        return monthlyIncome - monthlyExpenses;
    }

    /// <summary>
    /// Applies the net monthly cash flow to the player's cash.
    /// This should be called once per game month.
    /// </summary>
    public void ApplyMonthlyBudget()
    {
        int netFlow = GetNetCashFlow();
        // Change player's cash by the net flow amount.
        PlayerStats.Instance.ChangeCash(netFlow);
        Debug.Log($"[BudgetSystem] Monthly budget applied. Net cash flow: {netFlow}");
    }

    /// <summary>
    /// Sets a bonus multiplier for certain income events (e.g., for Retail company).
    /// This method is called when the player chooses the RETAIL company type.
    /// </summary>
    /// <param name="multiplier">The multiplier to apply (e.g., 1.10 for +10%).</param>
    public void SetBonusIncomeMultiplier(float multiplier)
    {
        bonusIncomeMultiplier = multiplier;
        Debug.Log($"[BudgetSystem] Retail company perk applied: Bonus income multiplier set to {multiplier}.");
        // Any income gained from events or specific sources should now be multiplied by this.
        // Example: If you have an 'event' that grants $100, it would become $100 * bonusIncomeMultiplier.
    }
}
