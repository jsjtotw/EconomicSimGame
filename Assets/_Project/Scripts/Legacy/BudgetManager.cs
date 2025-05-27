/*
using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    public static BudgetManager Instance;

    public float income = 5000f;
    public float expenses = 2000f;
    public float cashOnHand = 10000f;
    public float stockAssets = 15000f;
    public float loanDebt = 0f;
    public float netWorth;

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

    void Start()
    {
        UpdateNetWorth();
        // UI update removed
    }

    public void UpdateNetWorth()
    {
        netWorth = cashOnHand + stockAssets - loanDebt;
    }

    public void SimulateIncomeExpense()
    {
        cashOnHand += income - expenses;
        UpdateNetWorth();
        // UI update removed
    }
}
*/
