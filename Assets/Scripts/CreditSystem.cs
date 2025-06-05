// CreditSystem.cs
// Manages player loans, repayments, interest, and credit score.

using System;
using UnityEngine;

public class CreditSystem : MonoBehaviour
{
    // Singleton instance for CreditSystem.
    public static CreditSystem Instance;

    public int currentLoanAmount = 0;
    public float interestRate = 0.05f; // 5% monthly interest rate

    // New: Modifier for the interest rate (e.g., for Finance company).
    public float interestModifier = 1.0f; // Default: no modifier

    public int monthlyRepayment = 0;

    public int CreditScore { get; private set; } = 700; // Start at a neutral credit score

    // Event triggered when loan details change.
    public delegate void LoanChanged();
    public event LoanChanged OnLoanChanged;

    // --- NEW: Event triggered when a loan is taken ---
    public event Action<int> OnLoanTaken; // Passes the amount of the loan taken

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
    /// Allows the player to take out a loan.
    /// </summary>
    /// <param name="amount">The amount of loan to take.</param>
    public void TakeLoan(int amount)
    {
        currentLoanAmount += amount;
        PlayerStats.Instance.Cash += amount; // Add loan amount to player's cash.
        PlayerStats.Instance.Debt += amount; // Increase player's total debt.

        Debug.Log($"[CreditSystem] Loan taken: {amount}. Total loan: {currentLoanAmount}");

        UpdateCreditScore(); // Recalculate credit score after taking a loan.
        OnLoanChanged?.Invoke(); // Notify subscribers of loan change.
        OnLoanTaken?.Invoke(amount); // --- NEW: Invoke the OnLoanTaken event ---
    }

    /// <summary>
    /// Attempts to repay a portion of the loan using available cash.
    /// </summary>
    /// <param name="amount">The amount to repay.</param>
    /// <returns>True if repayment was successful, false otherwise (e.g., insufficient cash).</returns>
    public bool RepayLoan(int amount)
    {
        if (PlayerStats.Instance.Cash < amount)
        {
            Debug.LogWarning("[CreditSystem] Not enough cash to repay loan.");
            PlayerStats.Instance.MissedPayments++; // Increment missed payments if repayment fails.
            UpdateCreditScore(); // Update score due to missed payment.
            return false;
        }

        // Ensure we don't repay more than the current loan amount.
        amount = Mathf.Min(amount, currentLoanAmount);
        PlayerStats.Instance.ChangeCash(-amount); // Deduct repayment from player's cash.
        currentLoanAmount -= amount; // Reduce loan amount.
        PlayerStats.Instance.Debt -= amount; // Reduce player's total debt.

        Debug.Log($"[CreditSystem] Loan repaid: {amount}. Remaining loan: {currentLoanAmount}");

        PlayerStats.Instance.OnTimePayments++; // Increment on-time payments.
        UpdateCreditScore(); // Update score due to on-time payment.
        OnLoanChanged?.Invoke(); // Notify subscribers of loan change.

        return true;
    }

    /// <summary>
    /// Processes monthly loan obligations: adds interest and attempts automatic repayment.
    /// This should be called once per game month.
    /// </summary>
    public void ProcessMonthlyLoan()
    {
        if (currentLoanAmount <= 0)
        {
            // No loan outstanding, nothing to process.
            return;
        }

        // Calculate and add interest, applying the interest modifier.
        int interest = Mathf.CeilToInt(currentLoanAmount * interestRate * interestModifier);
        currentLoanAmount += interest;
        PlayerStats.Instance.Debt += interest; // Increase player's total debt by interest.

        Debug.Log($"[CreditSystem] Loan interest added: {interest}. New loan total: {currentLoanAmount}");

        // Attempt automatic monthly repayment if set.
        if (monthlyRepayment > 0)
        {
            if (RepayLoan(monthlyRepayment))
            {
                Debug.Log($"[CreditSystem] Monthly repayment of {monthlyRepayment} made.");
            }
            else
            {
                Debug.LogWarning("[CreditSystem] Failed to make full monthly repayment amount.");
            }
        }

        UpdateCreditScore(); // Recalculate credit score after monthly processing.
        OnLoanChanged?.Invoke(); // Notify subscribers of loan change.
    }

    /// <summary>
    /// Sets the target monthly repayment amount for the loan.
    /// </summary>
    /// <param name="amount">The desired monthly repayment amount.</param>
    public void SetMonthlyRepayment(int amount)
    {
        // Ensure repayment amount is not negative and not more than the loan outstanding.
        monthlyRepayment = Mathf.Clamp(amount, 0, currentLoanAmount);
        Debug.Log($"[CreditSystem] Monthly repayment set to {monthlyRepayment}");
    }

    /// <summary>
    /// Updates the player's credit score based on various factors.
    /// </summary>
    private void UpdateCreditScore()
    {
        int score = 700; // Base credit score.

        // Factor 1: Loan amount impact (higher loan, lower score).
        if (currentLoanAmount > 10000)
            score -= 50;
        else if (currentLoanAmount > 5000)
            score -= 25;

        // Factor 2: Missed payments penalty (more missed payments, lower score).
        score -= PlayerStats.Instance.MissedPayments * 20;

        // Factor 3: On-time payments bonus (more on-time payments, higher score).
        score += PlayerStats.Instance.OnTimePayments * 5;

        // Clamp the credit score within a realistic range.
        CreditScore = Mathf.Clamp(score, 300, 850);

        Debug.Log($"[CreditSystem] Credit score updated to {CreditScore}");
    }

    /// <summary>
    /// Sets a modifier for the interest rate.
    /// This method is called when the player chooses the FINANCE company type.
    /// </summary>
    /// <param name="modifier">The multiplier for the interest rate (e.g., 0.8f for 20% less interest).</param>
    public void SetInterestModifier(float modifier)
    {
        interestModifier = modifier;
        Debug.Log($"[CreditSystem] Finance company perk applied: Interest modifier set to {modifier}.");
        // The ProcessMonthlyLoan method will now use this modifier.
    }

    /// <summary>
    /// Determines if a loan can be approved based on credit score and player level (example perk).
    /// </summary>
    /// <param name="requestedAmount">The amount of the loan being requested.</param>
    /// <returns>True if the loan can be approved, false otherwise.</returns>
    public bool CanApproveLoan(int requestedAmount)
    {
        // Example: Base approval on current credit score
        bool baseApproval = CreditScore >= 600; // Require a minimum credit score

        // Example Perk: Easier approval for higher level players (e.g., Finance perk effect)
        if (PlayerXP.Instance != null && PlayerXP.Instance.CurrentLevel >= 3)
        {
            // At Level 3+, maybe the credit score requirement is lower, or higher amounts are allowed.
            // Or if they chose Finance company, maybe they get an even bigger boost.
            if (PlayerCompany.Instance != null && PlayerCompany.Instance.SelectedType == CompanyType.FINANCE)
            {
                Debug.Log($"[CreditSystem] Finance company perk gives favorable loan terms. Player Level: {PlayerXP.Instance.CurrentLevel}");
                return true; // Auto-approve if Finance company and decent level, for example.
            }
            // General level-based approval
            if (PlayerXP.Instance.CurrentLevel >= 5 && requestedAmount <= 50000) // Approve small loans more easily at higher levels
            {
                Debug.Log($"[CreditSystem] Loan approved due to player level: {PlayerXP.Instance.CurrentLevel}");
                return true;
            }
        }
        
        Debug.Log($"[CreditSystem] Loan approval status: {baseApproval} (Credit Score: {CreditScore})");
        return baseApproval;
    }
}
