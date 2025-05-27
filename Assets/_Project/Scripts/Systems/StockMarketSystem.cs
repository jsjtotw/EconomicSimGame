// File: Assets/Scripts/Systems/StockMarketSystem.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events; // Needed for UnityEvent

public class StockMarketSystem : MonoBehaviour
{
    public static StockMarketSystem Instance { get; private set; }

    // FIX: Changed from currentStocks to CurrentStocks and made public, also initialized list
    public List<Stock> CurrentStocks { get; private set; } = new List<Stock>();
    public float updateIntervalInHours = 6f; // Update prices every 6 in-game hours

    // FIX: Added IsInitialized property
    public bool IsInitialized { get; private set; } = false;
    // FIX: Added onStocksInitialized event
    public UnityEvent onStocksInitialized;

    private Dictionary<string, List<float>> stockPriceHistory = new Dictionary<string, List<float>>();
    private int maxHistoryPoints = 100; // Limit history points for graphs

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
        if (onStocksInitialized == null)
        {
            onStocksInitialized = new UnityEvent();
        }
    }

    void Start()
    {
        // Load initial stock data from DataManager
        if (DataManager.Instance != null)
        {
            CurrentStocks = new List<Stock>(DataManager.Instance.Stocks); // Create a mutable copy // FIX: Use CurrentStocks
            foreach (var stock in CurrentStocks) // FIX: Use CurrentStocks
            {
                // FIX: Use stock.Ticker for history dictionary key for consistency
                if (!stockPriceHistory.ContainsKey(stock.Ticker))
                {
                    stockPriceHistory[stock.Ticker] = new List<float>();
                }
                stockPriceHistory[stock.Ticker].Add(stock.CurrentPrice); // Add initial price to history // FIX: Use stock.CurrentPrice
            }
            IsInitialized = true; // Mark as initialized
            onStocksInitialized?.Invoke(); // Trigger the event
        }
        else
        {
            Debug.LogError("DataManager not found. StockMarketSystem cannot initialize stocks.");
            // CurrentStocks already initialized to new List<Stock>() in property declaration
        }

        // Subscribe to TimeManager's hour advance event
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced += OnHourAdvancedForStockUpdate;
        }
        // Subscribe to events from EventSystem
        EventSystem.onEventTriggered += HandleGameEvent;
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced -= OnHourAdvancedForStockUpdate;
        }
        EventSystem.onEventTriggered -= HandleGameEvent;
    }

    void OnHourAdvancedForStockUpdate()
    {
        if (TimeManager.Instance.hour % updateIntervalInHours == 0)
        {
            UpdateStockPrices();
        }
    }

    public void UpdateStockPrices()
    {
        foreach (var stock in CurrentStocks) // FIX: Use CurrentStocks
        {
            float change = stock.CurrentPrice * UnityEngine.Random.Range(-stock.volatility, stock.volatility); // FIX: Use stock.CurrentPrice
            stock.SetPrice(stock.CurrentPrice + change); // FIX: Use a SetPrice method to trigger the event
            stock.SetPrice(Mathf.Max(1f, stock.CurrentPrice)); // Ensure price doesn't go below 1

            // Add to history
            if (!stockPriceHistory.ContainsKey(stock.Ticker)) // FIX: Use stock.Ticker
            {
                stockPriceHistory[stock.Ticker] = new List<float>();
            }
            stockPriceHistory[stock.Ticker].Add(stock.CurrentPrice); // FIX: Use stock.Ticker, stock.CurrentPrice
            if (stockPriceHistory[stock.Ticker].Count > maxHistoryPoints) // FIX: Use stock.Ticker
            {
                stockPriceHistory[stock.Ticker].RemoveAt(0); // Remove oldest
            }
        }
        DashboardUI.Instance?.UpdateStockDisplays(CurrentStocks); // Update UI // FIX: Use CurrentStocks
        StockTradeSystem.Instance?.RecalculateStockAssets(); // Recalculate player's owned stock value
    }

    public void HandleGameEvent(GameEvent gameEvent)
    {
        float sensitivity = GameManager.Instance?.playerEventSensitivity ?? 1.0f; // Get player's company sensitivity

        foreach (var stock in CurrentStocks) // FIX: Use CurrentStocks
        {
            bool appliesToStock = gameEvent.target == "all" || stock.industry == gameEvent.target;

            if (appliesToStock)
            {
                float newPrice = stock.CurrentPrice; // FIX: Use stock.CurrentPrice
                if (gameEvent.effect == "increase")
                {
                    newPrice *= (1 + gameEvent.magnitude * sensitivity);
                }
                else if (gameEvent.effect == "decrease")
                {
                    newPrice *= (1 - gameEvent.magnitude * sensitivity);
                }
                else if (gameEvent.effect == "volatility")
                {
                    stock.volatility *= (1 + gameEvent.magnitude * sensitivity); // Increase volatility
                }
                stock.SetPrice(newPrice); // FIX: Use SetPrice to update price and trigger event
            }
        }
        DashboardUI.Instance?.UpdateStockDisplays(CurrentStocks); // Force UI update // FIX: Use CurrentStocks
    }

    public Stock GetStock(string ticker) // FIX: Changed parameter to ticker for consistency
    {
        return CurrentStocks.Find(s => s.Ticker == ticker); // FIX: Use CurrentStocks and s.Ticker
    }

    public List<float> GetStockPriceHistory(string ticker) // FIX: Changed parameter to ticker
    {
        if (stockPriceHistory.ContainsKey(ticker)) // FIX: Use ticker
        {
            return stockPriceHistory[ticker];
        }
        return new List<float>();
    }
}

// NOTE: The Stock class definition (lines 142-150 in your snippet)
// has been REMOVED from this file. It should ONLY exist in Assets/Scripts/Stock.cs.