using UnityEngine;

public class CreditSystem : MonoBehaviour
{
    public static CreditSystem Instance;

    public int currentLoanAmount = 0;
    public float interestRate = 0.05f; // 5% monthly interest rate
    public int monthlyRepayment = 0;

    public int CreditScore { get; private set; } = 700; // Start at a neutral credit score

    public delegate void LoanChanged();
    public event LoanChanged OnLoanChanged;

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

    // Take out loan
    public void TakeLoan(int amount)
    {
        currentLoanAmount += amount;
        PlayerStats.Instance.Cash += amount;
        PlayerStats.Instance.Debt += amount;

        Debug.Log($"[CreditSystem] Loan taken: {amount}. Total loan: {currentLoanAmount}");

        UpdateCreditScore();
        OnLoanChanged?.Invoke();
    }

    // Repay loan with available cash
    public bool RepayLoan(int amount)
    {
        if (PlayerStats.Instance.Cash < amount)
        {
            Debug.LogWarning("[CreditSystem] Not enough cash to repay loan.");
            PlayerStats.Instance.MissedPayments++;  // Count missed payment
            UpdateCreditScore();
            return false;
        }

        amount = Mathf.Min(amount, currentLoanAmount);
        PlayerStats.Instance.ChangeCash(-amount);
        currentLoanAmount -= amount;
        PlayerStats.Instance.Debt -= amount;

        Debug.Log($"[CreditSystem] Loan repaid: {amount}. Remaining loan: {currentLoanAmount}");

        PlayerStats.Instance.OnTimePayments++;  // Count on-time payment
        UpdateCreditScore();
        OnLoanChanged?.Invoke();

        return true;
    }

    // Called monthly to add interest and process repayment
    public void ProcessMonthlyLoan()
    {
        if (currentLoanAmount <= 0)
            return;

        // Add interest
        int interest = Mathf.CeilToInt(currentLoanAmount * interestRate);
        currentLoanAmount += interest;
        PlayerStats.Instance.Debt += interest;

        Debug.Log($"[CreditSystem] Loan interest added: {interest}. New loan total: {currentLoanAmount}");

        // Attempt automatic repayment
        if (monthlyRepayment > 0)
        {
            if (RepayLoan(monthlyRepayment))
            {
                Debug.Log($"[CreditSystem] Monthly repayment of {monthlyRepayment} made.");
            }
            else
            {
                Debug.LogWarning("[CreditSystem] Failed to repay monthly repayment amount.");
            }
        }

        UpdateCreditScore();
        OnLoanChanged?.Invoke();
    }

    // Set monthly repayment amount (must be <= loan amount)
    public void SetMonthlyRepayment(int amount)
    {
        monthlyRepayment = Mathf.Clamp(amount, 0, currentLoanAmount);
        Debug.Log($"[CreditSystem] Monthly repayment set to {monthlyRepayment}");
    }

    // Update credit score based on loan and payment history
    private void UpdateCreditScore()
    {
        int score = 700;

        // Factor 1: Loan amount impact
        if (currentLoanAmount > 10000)
            score -= 50;
        else if (currentLoanAmount > 5000)
            score -= 25;

        // Factor 2: Missed payments penalty
        score -= PlayerStats.Instance.MissedPayments * 20;

        // Factor 3: On-time payments bonus
        score += PlayerStats.Instance.OnTimePayments * 5;

        // Clamp score between 300 and 850
        CreditScore = Mathf.Clamp(score, 300, 850);

        Debug.Log($"[CreditSystem] Credit score updated to {CreditScore}");
    }
}