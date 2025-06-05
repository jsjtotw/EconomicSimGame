// StockMarketSystem.cs
// Manages stock data, prices, and market-related operations.

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockListWrapper
{
    public List<Stock> stocks;
}

public class StockMarketSystem : MonoBehaviour
{
    // Singleton instance for StockMarketSystem.
    public static StockMarketSystem Instance;

    public List<Stock> allStocks = new List<Stock>();

    // New: Multiplier for tech stock returns/performance.
    public float techBonusMultiplier = 1.0f; // Default: no bonus

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern and loads stock data.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStockData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads stock data from a JSON file in the Resources folder.
    /// </summary>
    void LoadStockData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("stock_data");
        if (jsonFile != null)
        {
            StockListWrapper wrapper = JsonUtility.FromJson<StockListWrapper>(jsonFile.text);
            allStocks = wrapper.stocks;
            Debug.Log($"[StockMarketSystem] Loaded {allStocks.Count} stocks.");
        }
        else
        {
            Debug.LogError("[StockMarketSystem] Failed to load stock_data.json from Resources.");
        }
    }

    /// <summary>
    /// Updates the price of a specific stock.
    /// </summary>
    /// <param name="stockID">The ID of the stock to update.</param>
    /// <param name="newPrice">The new price for the stock.</param>
    public void UpdateStockPrice(string stockID, float newPrice)
    {
        Stock stock = allStocks.Find(s => s.stockID == stockID);
        if (stock != null)
        {
            stock.price = newPrice;
            Debug.Log($"[StockMarketSystem] {stock.name} ({stock.stockID}) price updated to: {newPrice:F2}");
        }
    }
    
    /// <summary>
    /// Updates prices for all stocks.
    /// Assumes Stock class has an UpdatePrice method.
    /// </summary>
    public void UpdateAllPrices()
    {
        foreach (Stock stock in allStocks)
        {
            float oldPrice = stock.price;
            stock.UpdatePrice(); // Calls the individual stock's update logic
            Debug.Log($"[StockMarket] {stock.name} ({stock.stockID}): {oldPrice:F2} -> {stock.price:F2}");
        }
    }

    /// <summary>
    /// Retrieves a list of all unique industries present in the stock data.
    /// </summary>
    /// <returns>A list of unique industry names.</returns>
    public List<string> GetAllIndustries()
    {
        HashSet<string> industries = new HashSet<string>();
        foreach (var stock in allStocks)
        {
            if (!string.IsNullOrEmpty(stock.industry))
                industries.Add(stock.industry);
        }
        return new List<string>(industries);
    }

    /// <summary>
    /// Retrieves a list of all company codes (stock IDs).
    /// </summary>
    /// <returns>A list of all stock IDs.</returns>
    public List<string> GetAllCompanyCodes()
    {
        List<string> codes = new List<string>();
        foreach (var stock in allStocks)
        {
            if (!string.IsNullOrEmpty(stock.stockID))
                codes.Add(stock.stockID);
        }
        return codes;
    }

    /// <summary>
    /// Retrieves a Stock object by its company code (stock ID).
    /// </summary>
    /// <param name="code">The stock ID to search for.</param>
    /// <returns>The Stock object if found, otherwise null.</returns>
    public Stock GetStockByCode(string code)
    {
        return allStocks.Find(stock => stock.stockID == code);
    }

    /// <summary>
    /// Sets a bonus multiplier for tech-related stock returns.
    /// This method is called when the player chooses the TECH company type.
    /// </summary>
    /// <param name="bonusPercentage">The percentage bonus (e.g., 0.05 for 5%).</param>
    public void SetTechBonus(float bonusPercentage)
    {
        techBonusMultiplier = 1.0f + bonusPercentage;
        Debug.Log($"[StockMarketSystem] Tech company perk applied: +{bonusPercentage * 100}% base return on tech stocks.");
        // You might want to update stock price calculation logic to factor this multiplier in.
        // For example, when calling stock.UpdatePrice() or when calculating profit from sales.
    }
}
