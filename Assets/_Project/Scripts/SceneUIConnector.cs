using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneUIConnector : MonoBehaviour
{
    [Header("Stock Market UI Texts")]
    public List<TextMeshProUGUI> stockTextDisplays;

    [Header("Budget UI Texts")]
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI netWorthText;

    [Header("Credit UI Texts")]
    public TextMeshProUGUI loanText;
    public TextMeshProUGUI interestText;

    [Header("Time UI Text")]
    public TextMeshProUGUI timeText;

    [Header("Stock List Display")]
    public StockListDisplay stockListDisplay;

    [Header("Dropdown")]
    public TMP_Dropdown stockDropdown;
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void Start() => ConnectUI();
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ConnectUI();

    private void ConnectUI()
    {
        // === Stock Market UI ===
        if (StockMarketManager.Instance != null)
        {
            StockMarketManager.Instance.stockTextDisplays = stockTextDisplays;
            StockMarketManager.Instance.UpdateStockPrices();

            if (stockListDisplay != null)
            {
                stockListDisplay.RefreshStockList(StockMarketManager.Instance.stocks);
            }
            else
            {
                Debug.LogWarning("[SceneUIConnector] StockListDisplay not assigned.");
            }
        }

        // === Budget UI ===
        if (BudgetManager.Instance != null)
        {
            BudgetManager.Instance.incomeText = incomeText;
            BudgetManager.Instance.expensesText = expensesText;
            BudgetManager.Instance.cashText = cashText;
            BudgetManager.Instance.netWorthText = netWorthText;
            BudgetManager.Instance.UpdateUI();
        }

        // === Credit UI ===
        if (CreditManager.Instance != null)
        {
            CreditManager.Instance.loanText = loanText;
            CreditManager.Instance.interestText = interestText;
            CreditManager.Instance.UpdateUI();
        }

        // === Time UI ===
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.SetTimeText(timeText);
        }

        // === Dropdowns and Buttons ===
        SetupStockDropdownAndButtons();
        SetupLoanButtons();
        StockMarketManager.Instance.stockDropdown = stockDropdown;
        StockMarketManager.Instance.stockDropdown = stockDropdown;
        StockMarketManager.Instance.InitializeStocks();
         
    }

    private void SetupStockDropdownAndButtons()
    {
        Button buyBtn = GameObject.Find("Canvas/BuyStock")?.GetComponent<Button>();
        Button sellBtn = GameObject.Find("Canvas/SellStock")?.GetComponent<Button>();

        if (buyBtn != null)
        {
            TMP_Dropdown buyDropdown = buyBtn.GetComponentInChildren<TMP_Dropdown>();
            PopulateStockDropdown(buyDropdown);
            SetupStockTransactionButton(buyBtn, buyDropdown, isBuying: true);
        }

        if (sellBtn != null)
        {
            TMP_Dropdown sellDropdown = sellBtn.GetComponentInChildren<TMP_Dropdown>();
            PopulateStockDropdown(sellDropdown);
            SetupStockTransactionButton(sellBtn, sellDropdown, isBuying: false);
        }
    }

    private void SetupLoanButtons()
    {
        SetupLoanButton("Canvas/TakeLoan", isTaking: true);
        SetupLoanButton("Canvas/PayOffLoan", isTaking: false);
    }

    private void SetupLoanButton(string buttonPath, bool isTaking)
    {
        Button button = GameObject.Find(buttonPath)?.GetComponent<Button>();
        TMP_InputField input = button?.GetComponentInChildren<TMP_InputField>();

        if (button == null || input == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (float.TryParse(input.text, out float amount) && amount > 0)
            {
                if (isTaking)
                    CreditManager.Instance?.TakeLoan(amount);
                else
                    CreditManager.Instance?.PayLoan(amount);
            }
            else
            {
                Debug.LogWarning($"Invalid loan input in {(isTaking ? "TakeLoan" : "PayOffLoan")}.");
            }
        });
    }

    private void SetupStockTransactionButton(Button button, TMP_Dropdown dropdown, bool isBuying)
    {
        TMP_InputField qtyField = button.GetComponentInChildren<TMP_InputField>();
        if (dropdown == null || qtyField == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (!int.TryParse(qtyField.text, out int qty) || qty <= 0)
            {
                Debug.LogWarning($"Invalid stock quantity input in {(isBuying ? "BuyStock" : "SellStock")}.");
                return;
            }

            int selectedIndex = dropdown.value;
            if (StockMarketManager.Instance != null && selectedIndex >= 0 && selectedIndex < StockMarketManager.Instance.stocks.Count)
            {
                Stock selectedStock = StockMarketManager.Instance.stocks[selectedIndex];
                if (isBuying)
                    StockMarketManager.Instance.BuyStock(selectedStock, qty);
                else
                    StockMarketManager.Instance.SellStock(selectedStock, qty);
            }
        });
    }

    private void PopulateStockDropdown(TMP_Dropdown dropdown)
    {
        if (StockMarketManager.Instance == null || dropdown == null) return;

        dropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var stock in StockMarketManager.Instance.stocks)
        {
            options.Add(stock.stockName);
        }
        dropdown.AddOptions(options);
    }
}
