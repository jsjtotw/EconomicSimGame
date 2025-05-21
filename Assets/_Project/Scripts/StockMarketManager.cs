using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StockMarketManager : MonoBehaviour {
    public List<Stock> stocks = new List<Stock>();
    public List<TextMeshProUGUI> stockTextDisplays;

    public float updateInterval = 5f;

    private List<float> lastPrices = new List<float>();

    void Start() {
        // Initialize with some sample stocks
        stocks.Add(new Stock { stockName = "Logacorp", currentPrice = 100f, volatility = 0.05f });
        stocks.Add(new Stock { stockName = "Millieuia Ltd", currentPrice = 85f, volatility = 0.1f });
        stocks.Add(new Stock { stockName = "TetraTech", currentPrice = 70f, volatility = 0.07f });
        stocks.Add(new Stock { stockName = "AetherWorks", currentPrice = 120f, volatility = 0.08f });
        stocks.Add(new Stock { stockName = "ChronaCom", currentPrice = 150f, volatility = 0.03f });

        foreach (var stock in stocks)
            lastPrices.Add(stock.currentPrice);

        InvokeRepeating("UpdateStockPrices", 0f, updateInterval);
    }

    void UpdateStockPrices() {
        for (int i = 0; i < stocks.Count; i++) {
            var stock = stocks[i];
            float change = stock.currentPrice * Random.Range(-stock.volatility, stock.volatility);
            lastPrices[i] = stock.currentPrice;
            stock.currentPrice += change;
            stock.currentPrice = Mathf.Max(1f, stock.currentPrice); // Clamp price

            UpdateStockUI(i);
        }
    }

    void UpdateStockUI(int index) {
        if (index >= stockTextDisplays.Count) return;

        float prev = lastPrices[index];
        float curr = stocks[index].currentPrice;
        var textField = stockTextDisplays[index];

        textField.text = $"{stocks[index].stockName}: ${curr:F2}";

        if (curr > prev) {
            textField.color = Color.green;
        } else if (curr < prev) {
            textField.color = Color.red;
        } else {
            textField.color = Color.white;
        }
    }
}