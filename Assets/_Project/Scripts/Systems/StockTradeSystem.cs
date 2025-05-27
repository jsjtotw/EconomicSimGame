// File: Assets/Scripts/Systems/StockTradeSystem.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json; // Assuming you are using this for saving/loading
using UnityEngine.Events; // Needed for UnityEvent

public class StockTradeSystem : MonoBehaviour
{
    public static StockTradeSystem Instance { get; private set; }

    public Dictionary<string, int> playerOwnedStocks = new Dictionary<string, int>();

    // FIX: Add onSharesOwnedChanged event
    public UnityEvent<string, int> onSharesOwnedChanged; // ticker, sharesOwned

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

        // FIX: Initialize the UnityEvent
        if (onSharesOwnedChanged == null)
        {
            onSharesOwnedChanged = new UnityEvent<string, int>();
        }
    }

    void Start()
    {
        // Initialize owned stocks for all available stocks (from StockMarketSystem)
        if (GameManager.Instance?.StockMarket != null && GameManager.Instance.StockMarket.CurrentStocks != null) // FIX: Use CurrentStocks
        {
            foreach (var stock in GameManager.Instance.StockMarket.CurrentStocks) // FIX: Use CurrentStocks
            {
                if (!playerOwnedStocks.ContainsKey(stock.Ticker)) // FIX: Use stock.Ticker
                {
                    playerOwnedStocks.Add(stock.Ticker, 0); // FIX: Use stock.Ticker
                }
            }
        }
        RecalculateStockAssets(); // Calculate initial stock assets
    }

    // FIX: Renamed from BuyStock to BuyShares for consistency with StockUIEntry
    public void BuyShares(string ticker, int quantity) // FIX: Changed parameter name for clarity
    {
        Stock stockToBuy = GameManager.Instance.StockMarket.GetStock(ticker); // FIX: Use ticker
        if (stockToBuy == null)
        {
            Debug.LogWarning($"Stock {ticker} not found.");
            return;
        }

        float cost = stockToBuy.CurrentPrice * quantity; // FIX: Use stockToBuy.CurrentPrice
        if (GameManager.Instance.Budget.cashOnHand >= cost)
        {
            GameManager.Instance.Budget.cashOnHand -= cost;
            if (playerOwnedStocks.ContainsKey(ticker)) // FIX: Use ticker
            {
                playerOwnedStocks[ticker] += quantity; // FIX: Use ticker
            }
            else
            {
                playerOwnedStocks.Add(ticker, quantity); // FIX: Use ticker
            }
            RecalculateStockAssets();
            GameManager.Instance.Budget.UpdateNetWorth();
            GameManager.Instance.Player.GainXP(quantity * 5); // Gain XP for buying stock
            Debug.Log($"Bought {quantity} shares of {ticker} for ${cost:F2}. New shares: {playerOwnedStocks[ticker]}"); // FIX: Use ticker
            onSharesOwnedChanged?.Invoke(ticker, playerOwnedStocks[ticker]); // FIX: Invoke the event
            DashboardUI.Instance?.UpdateStockTradeUI(ticker, playerOwnedStocks[ticker]); // Update specific stock UI // FIX: Use ticker
        }
        else
        {
            Debug.LogWarning("Not enough cash to buy stock.");
        }
    }

    // FIX: Renamed from SellStock to SellShares for consistency with StockUIEntry
    public void SellShares(string ticker, int quantity) // FIX: Changed parameter name for clarity
    {
        Stock stockToSell = GameManager.Instance.StockMarket.GetStock(ticker); // FIX: Use ticker
        if (stockToSell == null)
        {
            Debug.LogWarning($"Stock {ticker} not found.");
            return;
        }

        if (playerOwnedStocks.ContainsKey(ticker) && playerOwnedStocks[ticker] >= quantity) // FIX: Use ticker
        {
            float revenue = stockToSell.CurrentPrice * quantity; // FIX: Use stockToSell.CurrentPrice
            GameManager.Instance.Budget.cashOnHand += revenue;
            playerOwnedStocks[ticker] -= quantity; // FIX: Use ticker
            RecalculateStockAssets();
            GameManager.Instance.Budget.UpdateNetWorth();
            GameManager.Instance.Player.GainXP(quantity * 5); // Gain XP for selling stock
            Debug.Log($"Sold {quantity} shares of {ticker} for ${revenue:F2}. Remaining shares: {playerOwnedStocks[ticker]}"); // FIX: Use ticker
            onSharesOwnedChanged?.Invoke(ticker, playerOwnedStocks[ticker]); // FIX: Invoke the event
            DashboardUI.Instance?.UpdateStockTradeUI(ticker, playerOwnedStocks[ticker]); // Update specific stock UI // FIX: Use ticker
        }
        else
        {
            Debug.LogWarning($"Not enough shares of {ticker} to sell {quantity}. Owned: {(playerOwnedStocks.ContainsKey(ticker) ? playerOwnedStocks[ticker] : 0)}"); // FIX: Use ticker
        }
    }

    public void RecalculateStockAssets()
    {
        float totalStockValue = 0f;
        if (GameManager.Instance?.StockMarket != null && GameManager.Instance.StockMarket.CurrentStocks != null) // FIX: Use CurrentStocks
        {
            foreach (var ownedStockEntry in playerOwnedStocks)
            {
                Stock stock = GameManager.Instance.StockMarket.GetStock(ownedStockEntry.Key); // ownedStockEntry.Key is the ticker
                if (stock != null)
                {
                    totalStockValue += stock.CurrentPrice * ownedStockEntry.Value; // FIX: Use stock.CurrentPrice
                }
            }
        }
        GameManager.Instance.Budget.stockAssets = totalStockValue; // Update BudgetSystem's stock assets
        GameManager.Instance.Budget.UpdateNetWorth(); // Recalculate net worth after stock assets change
    }

    public int GetOwnedShares(string ticker) // FIX: Changed parameter name for clarity
    {
        if (playerOwnedStocks.ContainsKey(ticker)) // FIX: Use ticker
        {
            return playerOwnedStocks[ticker];
        }
        return 0;
    }
}
