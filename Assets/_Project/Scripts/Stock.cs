// File: Assets/Scripts/Stock.cs (CREATE THIS NEW FILE)
using UnityEngine;
using UnityEngine.Events; // Needed for UnityEvent

[System.Serializable]
public class Stock
{
    public string stockName; // This can remain as a serialized field for inspector editing
    public string stockTicker; // Added for unique identification, especially for UI lookup
    public float initialPrice; // To set the starting price
    public float volatility;
    public string industry;

    // FIX: Public properties for external access
    public string Name => stockName;
    public string Ticker => stockTicker;

    // FIX: CurrentPrice property with a private setter, updated via SetPrice method
    private float _currentPrice;
    public float CurrentPrice
    {
        get { return _currentPrice; }
        private set { _currentPrice = value; } // Set private for direct modification only through SetPrice
    }

    // FIX: Event to notify listeners when price changes
    // This uses UnityEvent for easy subscription in the Inspector as well
    public UnityEvent<float> onPriceChanged;

    // Constructor (useful if you create Stocks programmatically)
    public Stock(string name, string ticker, float price, float vol, string ind)
    {
        stockName = name;
        stockTicker = ticker;
        initialPrice = price;
        volatility = vol;
        industry = ind;
        _currentPrice = price; // Initialize current price
        onPriceChanged = new UnityEvent<float>(); // Initialize the event
    }

    // Parameterless constructor for System.Serializable (if needed for DataManager loading)
    public Stock()
    {
        onPriceChanged = new UnityEvent<float>(); // Initialize
    }


    // FIX: Method to update price and invoke the event
    public void SetPrice(float newPrice)
    {
        if (_currentPrice != newPrice) // Only update if price actually changed
        {
            _currentPrice = newPrice;
            onPriceChanged?.Invoke(_currentPrice);
        }
    }
}