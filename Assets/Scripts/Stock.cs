// Stock.cs
// Defines the properties and behavior of a single stock.

using UnityEngine;
using System; // Required for StringComparison

[System.Serializable]
public class Stock
{
    public string name;
    public string industry; // e.g., "Technology", "Finance", "Retail"
    public string stockID;
    public float price;
    public float volatility; // percentage (e.g., 0.05 = 5% for daily fluctuation)
    public int volume;

    /// <summary>
    /// Updates the stock's price based on its volatility and industry.
    /// Applies a tech bonus if the stock is in the Technology industry.
    /// </summary>
    public void UpdatePrice()
    {
        // Simple random fluctuation based on volatility
        float changePercent = UnityEngine.Random.Range(-volatility, volatility);
        float changeAmount = price * changePercent;

        // Apply tech bonus if the industry is "Technology" and StockMarketSystem is available
        // Using StringComparison.OrdinalIgnoreCase for robust case-insensitive comparison
        if (industry.Equals("Technology", StringComparison.OrdinalIgnoreCase) && StockMarketSystem.Instance != null)
        {
            // If the price change is positive, amplify it by the tech bonus multiplier.
            // If the price change is negative, reduce its magnitude (make it less severe).
            if (changeAmount > 0)
            {
                changeAmount *= StockMarketSystem.Instance.techBonusMultiplier;
            }
            else if (changeAmount < 0)
            {
                // Divide to make the negative change smaller in magnitude
                // e.g., -10 * 1.05 = -10.5 (larger negative) vs -10 / 1.05 = -9.5 (smaller negative)
                changeAmount /= StockMarketSystem.Instance.techBonusMultiplier;
            }
        }

        price += changeAmount;

        // Ensure price never drops below a minimum value (e.g., 0.01 to avoid division by zero or negative prices)
        price = Mathf.Max(price, 0.01f);
    }
}
