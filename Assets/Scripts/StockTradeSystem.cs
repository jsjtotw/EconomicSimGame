using System.Collections.Generic;
using UnityEngine;

public class StockTradeSystem : MonoBehaviour
{
    public static StockTradeSystem Instance;

    // Player's owned stocks: stockID -> amount owned
    private Dictionary<string, int> playerHoldings = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[StockTradeSystem] Instance created.");
        }
        else
        {
            Debug.Log("[StockTradeSystem] Duplicate detected. Destroying extra instance.");
            Destroy(gameObject);
        }
    }

    // Get player's current holding for a stock stockID
    public int GetHolding(string stockstockID)
    {
        if (playerHoldings.TryGetValue(stockstockID, out int amount))
            return amount;
        return 0;
    }

    // Buy multiple shares of the stock
    public void BuyStock(Stock stock, int amount, System.Action<bool> onComplete)
    {
        int pricePerShare = Mathf.CeilToInt(stock.price);
        int totalCost = pricePerShare * amount;

        if (PlayerStats.Instance.Cash < totalCost)
        {
            Debug.LogWarning($"[StockTradeSystem] Not enough cash to buy {amount} shares of {stock.name}");
            onComplete?.Invoke(false);
            return;
        }

        PopupManager.Instance.ShowConfirmation(
            $"Buy {amount} shares of {stock.name} for ${totalCost}?",
            confirmed =>
            {
            if (confirmed)
            {
                PlayerStats.Instance.Cash -= totalCost;

                if (playerHoldings.ContainsKey(stock.stockID))
                    playerHoldings[stock.stockID] += amount;
                else
                    playerHoldings[stock.stockID] = amount;

                PortfolioSystem.Instance.RegisterInvestment(stock, amount);

                Debug.Log($"[StockTradeSystem] Bought {amount} shares of {stock.name} for ${totalCost}. New cash: {PlayerStats.Instance.Cash}, Holdings: {playerHoldings[stock.stockID]}");
                onComplete?.Invoke(true);
            }
                else
                {
                    Debug.Log("[StockTradeSystem] Buy cancelled.");
                    onComplete?.Invoke(false);
                }
            });
    }

    // Sell multiple shares of the stock
    public void SellStock(Stock stock, int amount, System.Action<bool> onComplete)
    {
        int holdings = GetHolding(stock.stockID);
        if (holdings < amount)
        {
            Debug.LogWarning($"[StockTradeSystem] Not enough shares to sell {amount} of {stock.name}");
            onComplete?.Invoke(false);
            return;
        }

        int pricePerShare = Mathf.CeilToInt(stock.price);
        int totalRevenue = pricePerShare * amount;

        PopupManager.Instance.ShowConfirmation(
            $"Sell {amount} shares of {stock.name} for ${totalRevenue}?",
            confirmed =>
            {
            if (confirmed)
            {
                playerHoldings[stock.stockID] -= amount;
                PlayerStats.Instance.Cash += totalRevenue;

                PortfolioSystem.Instance.RegisterSale(stock, amount);

                Debug.Log($"[StockTradeSystem] Sold {amount} shares of {stock.name} for ${totalRevenue}. New cash: {PlayerStats.Instance.Cash}, Holdings: {playerHoldings[stock.stockID]}");
                onComplete?.Invoke(true);
            }
                else
                {
                    Debug.Log("[StockTradeSystem] Sell cancelled.");
                    onComplete?.Invoke(false);
                }
            });
    }
}
