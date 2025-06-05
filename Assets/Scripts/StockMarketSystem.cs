using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockListWrapper
{
    public List<Stock> stocks;
}

public class StockMarketSystem : MonoBehaviour
{
    public static StockMarketSystem Instance;

    public List<Stock> allStocks = new List<Stock>();

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

    public void UpdateStockPrice(string stockID, float newPrice)
    {
        Stock stock = allStocks.Find(s => s.stockID == stockID);
        if (stock != null)
        {
            stock.price = newPrice;
        }
    }
    
    public void UpdateAllPrices()
    {
        foreach (Stock stock in allStocks)
        {
            float oldPrice = stock.price;
            stock.UpdatePrice();
            Debug.Log($"[StockMarket] {stock.name} ({stock.stockID}): {oldPrice:F2} → {stock.price:F2}");
        }
    }
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

    public Stock GetStockByCode(string code)
    {
        return allStocks.Find(stock => stock.stockID == code);
    }

}
