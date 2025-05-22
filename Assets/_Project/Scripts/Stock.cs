[System.Serializable]
public class Stock
{
    public string stockName;
    public float currentPrice;
    public float volatility;

    public int quantityOwned = 0;
    public float totalEarned = 0f;
}
