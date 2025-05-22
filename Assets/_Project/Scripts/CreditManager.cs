using UnityEngine;
using TMPro;

public class CreditManager : MonoBehaviour
{
    public float loanAmount;
    public float interestRate = 0.05f;

    public TextMeshProUGUI loanText;
    public TextMeshProUGUI interestText;

    public void InitializeCredit()
    {
        UpdateUI();
    }
    public void TakeLoan(float amount)
    {
        loanAmount += amount;
        BudgetManager.Instance.cashOnHand += amount;
        BudgetManager.Instance.loanDebt += amount;
        BudgetManager.Instance.UpdateNetWorth();
        BudgetManager.Instance.UpdateUI();
        UpdateUI();
    }
    public void PayLoan(float amount) {
    if (amount > loanAmount) {
        amount = loanAmount; // Pay off the entire loan if more than owed
    }
    loanAmount -= amount;
    BudgetManager.Instance.cashOnHand -= amount;
    BudgetManager.Instance.loanDebt -= amount;
    BudgetManager.Instance.UpdateNetWorth();
    BudgetManager.Instance.UpdateUI();
    UpdateUI();
    }

    public void ApplyInterest()
    {
        float interest = loanAmount * interestRate;
        BudgetManager.Instance.expenses += interest;
        UpdateUI();
    }

    public void UpdateUI()
    {
        loanText.text = $"Current Loan: ${loanAmount:F0}";
        interestText.text = $"Monthly Interest: ${loanAmount * interestRate:F0}";
    }
    
    void Awake() {
    if (Instance == null) {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    } else {
        Destroy(gameObject);
    }
}

    public static CreditManager Instance { get; private set; }
}
