using UnityEngine;
using TMPro;

public class CreditManager : MonoBehaviour {
    public float loanAmount;
    public float interestRate = 0.05f;

    public TextMeshProUGUI loanText;
    public TextMeshProUGUI interestText;

    void Start() {
        InvokeRepeating(nameof(ApplyInterest), 5f, 10f); // Apply interest every 10 seconds
        UpdateUI();
    }

    public void TakeLoan(float amount) {
        loanAmount += amount;
        BudgetManager.Instance.cashOnHand += amount;
        BudgetManager.Instance.loanDebt += amount;
        BudgetManager.Instance.UpdateNetWorth();
        BudgetManager.Instance.UpdateUI();
        UpdateUI();
    }

    public void ApplyInterest() {
        float interest = loanAmount * interestRate;
        BudgetManager.Instance.expenses += interest;
        UpdateUI();
    }

    void UpdateUI() {
        loanText.text = $"Current Loan: ${loanAmount:F0}";
        interestText.text = $"Monthly Interest: ${loanAmount * interestRate:F0}";
    }
}
