using UnityEngine;
using TMPro;

public class BudgetManager : MonoBehaviour {
    public static BudgetManager Instance; // Singleton

    public float income = 5000f;
    public float expenses = 2000f;
    public float cashOnHand = 10000f;
    public float stockAssets = 15000f;
    public float loanDebt = 0f;
    public float netWorth;

    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI netWorthText;

    void Awake() {
        Instance = this;
    }

    void Start() {
        UpdateNetWorth();
        UpdateUI();
        InvokeRepeating(nameof(SimulateIncomeExpense), 2f, 10f);
    }

    public void UpdateNetWorth() {
        netWorth = cashOnHand + stockAssets - loanDebt;
    }

    public void SimulateIncomeExpense() {
        cashOnHand += income - expenses;
        UpdateNetWorth();
        UpdateUI();
    }

    public void UpdateUI() {
        incomeText.text = $"Income: ${income:F0}";
        expensesText.text = $"Expenses: ${expenses:F0}";
        cashText.text = $"Cash: ${cashOnHand:F0}";
        netWorthText.text = $"Net Worth: ${netWorth:F0}";
    }
}
