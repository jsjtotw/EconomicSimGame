using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance;

    public float loanAmount;
    public float interestRate = 0.05f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InvokeRepeating(nameof(ApplyInterest), 5f, 10f);
    }

    public void TakeLoan(float amount)
    {
        loanAmount += amount;
        BudgetManager.Instance.cashOnHand += amount;
        BudgetManager.Instance.loanDebt += amount;
        BudgetManager.Instance.UpdateNetWorth();
        // UI update removed
    }

    public void PayBackLoan(float amount)
    {
        if (amount <= 0f || amount > loanAmount || amount > BudgetManager.Instance.cashOnHand) return;

        loanAmount -= amount;
        BudgetManager.Instance.cashOnHand -= amount;
        BudgetManager.Instance.loanDebt -= amount;
        BudgetManager.Instance.UpdateNetWorth();
        // UI update removed
    }

    public void ApplyInterest()
    {
        float interest = loanAmount * interestRate;
        BudgetManager.Instance.expenses += interest;
        // UI update removed
    }
}
