using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FinanceDashboardPanel : MonoBehaviour
{
    [Header("Finance Overview")]
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI creditScoreText;

    [Header("Loan Info")]
    public TextMeshProUGUI debtOwedText;
    public TextMeshProUGUI interestRateText;
    public TextMeshProUGUI monthlyPaymentText;

    [Header("Portfolio Info")]
    public TextMeshProUGUI totalInvestmentText;
    public TextMeshProUGUI avgReturnText;

    [Header("Buttons")]
    public Button requestLoanButton;
    public Button repayLoanButton;

    // Optional inputs
    public TMP_InputField loanAmountInput;
    public TMP_InputField repaymentAmountInput;

    void Start()
    {
        requestLoanButton.onClick.AddListener(OnTakeLoanClicked);
        repayLoanButton.onClick.AddListener(OnRepayLoanClicked);
        Refresh();
    }

    void Refresh()
    {
        incomeText.text = $"${BudgetSystem.Instance.monthlyIncome}/mo";
        expensesText.text = $"${BudgetSystem.Instance.monthlyExpenses}/mo";
        savingsText.text = $"${PlayerStats.Instance.Cash}";
        creditScoreText.text = $"{CreditSystem.Instance.CreditScore} Score";

        debtOwedText.text = $"Debt Owed: ${PlayerStats.Instance.Debt}";
        interestRateText.text = $"Interest Rate: {CreditSystem.Instance.interestRate * 100}%";
        monthlyPaymentText.text = $"Monthly Payments: ${CreditSystem.Instance.monthlyRepayment}";

        totalInvestmentText.text = $"Total Investment: ${PortfolioSystem.Instance.TotalInvested}";
        avgReturnText.text = $"Avg Return: {PortfolioSystem.Instance.AvgReturnRate}%";
    }

    private void OnTakeLoanClicked()
    {
        int loanAmount = int.Parse(loanAmountInput.text); // Make sure input is valid
        CreditSystem.Instance.TakeLoan(loanAmount);
        Refresh();
    }

    private void OnRepayLoanClicked()
    {
        int repaymentAmount = int.Parse(repaymentAmountInput.text); // Make sure input is valid
        CreditSystem.Instance.RepayLoan(repaymentAmount);
        Refresh();
    }
}
