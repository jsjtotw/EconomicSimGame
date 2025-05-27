// File: Assets/Scripts/Systems/CreditSystem.cs
using UnityEngine;
using UnityEngine.Events; // Needed for UnityEvent

public class CreditSystem : MonoBehaviour
{
    public static CreditSystem Instance { get; private set; }

    private float _loanAmount = 0f; // Made private for property setter
    public float loanAmount
    {
        get { return _loanAmount; }
        set
        {
            _loanAmount = value;
            onDebtChanged?.Invoke(_loanAmount); // FIX: Invoke event
        }
    }

    private float _interestRatePerDay = 0.005f; // Made private for property setter
    public float interestRatePerDay
    {
        get { return _interestRatePerDay; }
        set
        {
            _interestRatePerDay = value;
            onInterestRateChanged?.Invoke(_interestRatePerDay); // FIX: Invoke event
        }
    }
    public float bankruptcyThreshold = -10000f; // Duplicated from GameManager, ensure consistent

    // FIX: Add UnityEvents for specific credit changes
    public UnityEvent<float> onDebtChanged;
    public UnityEvent<float> onInterestRateChanged;


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
        if (onDebtChanged == null) onDebtChanged = new UnityEvent<float>();
        if (onInterestRateChanged == null) onInterestRateChanged = new UnityEvent<float>();
    }

    void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced += OnHourAdvancedForCredit;
        }
        // Initial UI updates are now triggered by events
        onDebtChanged?.Invoke(loanAmount);
        onInterestRateChanged?.Invoke(interestRatePerDay);
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced -= OnHourAdvancedForCredit;
        }
    }

    void OnHourAdvancedForCredit()
    {
        // Apply interest daily (at end of day)
        if (TimeManager.Instance.hour == 0 && loanAmount > 0)
        {
            ApplyInterest();
        }

        // Check for bankruptcy if net worth is too low and debt is high
        if (GameManager.Instance.Budget.netWorth < bankruptcyThreshold && loanAmount > 0)
        {
            GameManager.Instance.LoseGame(); // Trigger lose condition
        }
    }

    public void TakeLoan(float amount)
    {
        loanAmount += amount;
        GameManager.Instance.Budget.cashOnHand += amount;
        GameManager.Instance.Budget.loanDebt = loanAmount; // Keep BudgetSystem's debt in sync
        GameManager.Instance.Budget.UpdateNetWorth();
        // DashboardUI.Instance?.UpdateCreditUI(loanAmount, loanAmount * interestRatePerDay); // FIX: This line can be removed as events update the UI
        GameManager.Instance.Player.GainXP(20); // Gain XP for taking a loan
        Debug.Log($"Loan taken: ${amount:F2}. Total Loan: ${loanAmount:F2}");
    }

    public void PayBackLoan(float amount)
    {
        if (amount > loanAmount) amount = loanAmount;
        if (GameManager.Instance.Budget.cashOnHand < amount)
        {
            Debug.LogWarning("Not enough cash to repay loan!");
            return;
        }

        loanAmount -= amount;
        GameManager.Instance.Budget.cashOnHand -= amount;
        GameManager.Instance.Budget.loanDebt = loanAmount;
        GameManager.Instance.Budget.UpdateNetWorth();
        // DashboardUI.Instance?.UpdateCreditUI(loanAmount, loanAmount * interestRatePerDay); // FIX: This line can be removed as events update the UI
        GameManager.Instance.Player.GainXP(30); // Gain XP for repaying loan
        Debug.Log($"Loan repaid: ${amount:F2}. Remaining Loan: ${loanAmount:F2}");
    }

    void ApplyInterest()
    {
        float interest = loanAmount * interestRatePerDay;
        GameManager.Instance.Budget.Expenses += interest; // Interest adds to expenses // FIX: Use Budget.Expenses property
        loanAmount += interest; // Interest also adds to principal for simple compounding
        GameManager.Instance.Budget.loanDebt = loanAmount; // Update debt in BudgetSystem
        GameManager.Instance.Budget.UpdateNetWorth();
        // DashboardUI.Instance?.UpdateCreditUI(loanAmount, loanAmount * interestRatePerDay); // FIX: This line can be removed as events update the UI
        Debug.Log($"Applied daily interest: ${interest:F2}. New loan amount: ${loanAmount:F2}");
    }
}