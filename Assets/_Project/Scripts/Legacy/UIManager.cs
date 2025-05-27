/*
using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Top Bar UI Elements")]
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI netWorthText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI xpLevelText;
    public TextMeshProUGUI timeText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(BindUIRoutine());
    }

    public void UpdateTopBar(float cash, float netWorth, float debt, int xp, int level)
    {
        if (cashText != null) cashText.text = $"Cash: ${cash:N0}";
        if (netWorthText != null) netWorthText.text = $"Net Worth: ${netWorth:N0}";
        if (debtText != null) debtText.text = $"Debt: ${debt:N0}";
        if (xpLevelText != null) xpLevelText.text = $"XP: {xp} | Level: {level}";
    }

    public void UpdateTimeText(int year, int month, int day, int hour)
    {
        if (timeText != null)
            timeText.text = $"Time: {year:D4}/{month:D2}/{day:D2} - {hour:00}:00";
    }

    void TryBindUI()
    {
        if (cashText == null) cashText = GameObject.Find("CashText")?.GetComponent<TextMeshProUGUI>();
        if (netWorthText == null) netWorthText = GameObject.Find("NetWorthText")?.GetComponent<TextMeshProUGUI>();
        if (debtText == null) debtText = GameObject.Find("DebtText")?.GetComponent<TextMeshProUGUI>();
        if (xpLevelText == null) xpLevelText = GameObject.Find("XPLevelText")?.GetComponent<TextMeshProUGUI>();
        if (timeText == null) timeText = GameObject.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
    }

    IEnumerator BindUIRoutine()
    {
        while (cashText == null || netWorthText == null || debtText == null || xpLevelText == null || timeText == null)
        {
            TryBindUI();
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("UIManager: All UI elements successfully bound.");
        StartCoroutine(UpdateTopBarRoutine()); //  Start updating UI continuously
    }

    IEnumerator UpdateTopBarRoutine()
    {
        while (true)
        {
            float cash = BudgetManager.Instance?.cashOnHand ?? 0;
            float netWorth = BudgetManager.Instance?.netWorth ?? 0;
            float debt = BudgetManager.Instance?.loanDebt ?? 0;
            int xp = PlayerManager.Instance?.xp ?? 0;
            int level = PlayerManager.Instance?.level ?? 1;

            UpdateTopBar(cash, netWorth, debt, xp, level);

            var time = TimeManager.Instance;
            if (time != null)
                UpdateTimeText(time.year, time.month, time.day, time.hour);

            yield return new WaitForSeconds(0.25f); //  Adjust update rate as needed
        }
    }
}
*/
