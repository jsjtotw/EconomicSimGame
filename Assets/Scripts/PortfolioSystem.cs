using System.Collections.Generic;
using UnityEngine;

public class PortfolioSystem : MonoBehaviour
{
    public static PortfolioSystem Instance;

    public float TotalInvested { get; private set; }
    public float AvgReturnRate { get; private set; }

    // Internal storage for original investment values
    private Dictionary<string, float> investedAmountPerStock = new Dictionary<string, float>();
    private Dictionary<string, float> originalPrices = new Dictionary<string, float>();

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

    // Call this when player buys stock
    public void RegisterInvestment(Stock stock, int amount)
    {
        Debug.Log($"[PortfolioSystem] Registering investment: {stock.stockID}, Amount: {amount}, Price: {stock.price}");
        float cost = stock.price * amount;
        TotalInvested += cost;

        if (investedAmountPerStock.ContainsKey(stock.stockID))
            investedAmountPerStock[stock.stockID] += cost;
        else
            investedAmountPerStock[stock.stockID] = cost;

        if (!originalPrices.ContainsKey(stock.stockID))
            originalPrices[stock.stockID] = stock.price;

        UpdateReturnRate();
    }

    // Call this when player sells stock
    public void RegisterSale(Stock stock, int amount)
    {
        float value = stock.price * amount;
        float invested = 0f;

        if (investedAmountPerStock.ContainsKey(stock.stockID))
        {
            invested = Mathf.Min(investedAmountPerStock[stock.stockID], value);
            investedAmountPerStock[stock.stockID] -= invested;
            if (investedAmountPerStock[stock.stockID] <= 0f)
            {
                investedAmountPerStock.Remove(stock.stockID);
                originalPrices.Remove(stock.stockID);
            }
        }

        TotalInvested = Mathf.Max(0f, TotalInvested - invested);
        UpdateReturnRate();
    }

    private void UpdateReturnRate()
    {
        if (investedAmountPerStock.Count == 0)
        {
            AvgReturnRate = 0f;
            return;
        }

        float totalReturn = 0f;
        float totalWeight = 0f;

        foreach (var entry in investedAmountPerStock)
        {
            string stockID = entry.Key;
            float invested = entry.Value;

            Stock stock = StockMarketSystem.Instance.allStocks.Find(s => s.stockID == stockID);
            if (stock != null && originalPrices.ContainsKey(stockID))
            {
                float originalPrice = originalPrices[stockID];
                if (originalPrice > 0f)
                {
                    float returnRate = (stock.price - originalPrice) / originalPrice * 100f;
                    totalReturn += returnRate * invested;
                    totalWeight += invested;
                }
            }
        }

        AvgReturnRate = totalWeight > 0f ? totalReturn / totalWeight : 0f;
    }
}
