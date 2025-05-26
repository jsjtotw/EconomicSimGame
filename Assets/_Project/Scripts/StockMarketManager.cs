using System.Collections.Generic;
using UnityEngine;

public class StockMarketManager : MonoBehaviour
{
    public static StockMarketManager Instance;

    public List<Stock> stocks = new List<Stock>();

    public float updateInterval = 5f;
    private List<float> lastPrices = new List<float>();

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
        stocks.Add(new Stock { stockName = "Logacorp", currentPrice = 100f, volatility = 0.05f });
        stocks.Add(new Stock { stockName = "Millieuia Ltd", currentPrice = 85f, volatility = 0.1f });
        stocks.Add(new Stock { stockName = "TetraTech", currentPrice = 70f, volatility = 0.07f });
        stocks.Add(new Stock { stockName = "AetherWorks", currentPrice = 120f, volatility = 0.08f });
        stocks.Add(new Stock { stockName = "ChronaCom", currentPrice = 150f, volatility = 0.03f });

        foreach (var stock in stocks)
            lastPrices.Add(stock.currentPrice);

        InvokeRepeating("UpdateStockPrices", 0f, updateInterval);
    }

    public void UpdateStockPrices()
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            var stock = stocks[i];
            float change = stock.currentPrice * Random.Range(-stock.volatility, stock.volatility);
            lastPrices[i] = stock.currentPrice;
            stock.currentPrice += change;
            stock.currentPrice = Mathf.Max(1f, stock.currentPrice);
            // UI update removed
        }
    }

    public void BuyStock(int index, int quantity)
    {
        if (index < 0 || index >= stocks.Count || quantity <= 0) return;

        Stock stock = stocks[index];
        float totalCost = stock.currentPrice * quantity;

        if (BudgetManager.Instance.cashOnHand >= totalCost)
        {
            stock.sharesOwned += quantity;
            BudgetManager.Instance.cashOnHand -= totalCost;
            BudgetManager.Instance.stockAssets += totalCost;
            BudgetManager.Instance.UpdateNetWorth();
            // UI update removed
        }
    }

    public void SellStock(int index, int quantity)
    {
        if (index < 0 || index >= stocks.Count || quantity <= 0) return;

        Stock stock = stocks[index];
        if (stock.sharesOwned >= quantity)
        {
            float totalValue = stock.currentPrice * quantity;
            stock.sharesOwned -= quantity;
            BudgetManager.Instance.cashOnHand += totalValue;
            BudgetManager.Instance.stockAssets -= totalValue;
            BudgetManager.Instance.UpdateNetWorth();
            // UI update removed
        }
    }
}
