using UnityEngine;

[System.Serializable]
public class Stock
{
    public string name;
    public string industry;
    public string stockID;
    public float price;
    public float volatility; // percenstockIDe (e.g., 0.05 = 5%)
    public int volume;

    public void UpdatePrice()
    {
        // Simple random fluctuation
        float changePercent = Random.Range(-volatility, volatility);
        float changeAmount = price * changePercent;
        price += changeAmount;

        // Ensure price never drops below zero
        price = Mathf.Max(price, 0.01f);
    }
}
