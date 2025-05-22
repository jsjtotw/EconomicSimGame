using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StockMarketManager : MonoBehaviour
{
    public List<Stock> stocks = new List<Stock>();
    public List<TextMeshProUGUI> stockTextDisplays;
    public TMP_Dropdown stockDropdown; // <-- ADDED
    public StockListDisplay stockListDisplay;

    private List<float> lastPrices = new List<float>();

    void Start()
    {
        stocks.Add(new Stock { stockName = "Logacorp", currentPrice = 100f, volatility = 0.05f });
        stocks.Add(new Stock { stockName = "Millieuia Ltd", currentPrice = 85f, volatility = 0.1f });
        stocks.Add(new Stock { stockName = "TetraTech", currentPrice = 70f, volatility = 0.07f });
        stocks.Add(new Stock { stockName = "AetherWorks", currentPrice = 120f, volatility = 0.08f });
        stocks.Add(new Stock { stockName = "ChronaCom", currentPrice = 150f, volatility = 0.03f });

        foreach (var stock in stocks)
            lastPrices.Add(stock.currentPrice);
    }

    public void InitializeStocks()
    {
        UpdateStockPrices();
        PopulateDropdown(); // <-- ADDED
    }

    public void UpdateStockPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            var stock = stocks[i];
            float change = stock.currentPrice * Random.Range(-stock.volatility, stock.volatility);
            lastPrices[i] = stock.currentPrice;
            stock.currentPrice = Mathf.Max(1f, stock.currentPrice + change);

            UpdateStockUI(i);
        }
    }

    void UpdateStockUI(int index)
    {
        if (index >= stockTextDisplays.Count) return;

        float prev = lastPrices[index];
        float curr = stocks[index].currentPrice;
        var textField = stockTextDisplays[index];

        textField.text = $"{stocks[index].stockName}: ${curr:F2}";
        textField.color = curr > prev ? Color.green : curr < prev ? Color.red : Color.white;
    }

    public void PopulateDropdown()
    {
        if (stockDropdown == null)
        {
            Debug.LogWarning("Stock dropdown not assigned.");
            return;
        }

        stockDropdown.ClearOptions();

        if (stocks == null || stocks.Count == 0)
        {
            Debug.LogWarning("Stock list is empty when populating dropdown.");
            return;
        }

        List<string> options = new List<string>();
        foreach (var stock in stocks)
        {
            options.Add(stock.stockName);
        }

        stockDropdown.AddOptions(options);
    }

public void BuyStock(Stock stock, int quantity)
{
    float totalCost = stock.currentPrice * quantity;
    if (BudgetManager.Instance.cashOnHand >= totalCost)
    {
        BudgetManager.Instance.cashOnHand -= totalCost;
        BudgetManager.Instance.stockAssets += totalCost;

        stock.quantityOwned += quantity;
        stock.totalEarned -= totalCost; // Negative because it's an expense

        BudgetManager.Instance.UpdateNetWorth();
        BudgetManager.Instance.UpdateUI();

        stockListDisplay.RefreshStockList(stocks); // <-- Add this
    }
    else
    {
        Debug.Log("Not enough cash to buy stock.");
    }
}

public void SellStock(Stock stock, int quantity)
{
    if (stock.quantityOwned >= quantity)
    {
        float totalSale = stock.currentPrice * quantity;
        BudgetManager.Instance.cashOnHand += totalSale;
        BudgetManager.Instance.stockAssets -= totalSale;

        stock.quantityOwned -= quantity;
        stock.totalEarned += totalSale;

        BudgetManager.Instance.UpdateNetWorth();
        BudgetManager.Instance.UpdateUI();

        stockListDisplay.RefreshStockList(stocks); // <-- Add this
    }
    else
    {
        Debug.Log("Not enough shares to sell.");
    }
}

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

    public static StockMarketManager Instance { get; private set; }
}
