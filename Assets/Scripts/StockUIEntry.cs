using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StockUIEntry : MonoBehaviour
{
    public TextMeshProUGUI stockNameText;
    public TextMeshProUGUI stockPriceText;
    public TextMeshProUGUI holdingText;

    public Button buyButton;
    public Button sellButton;
    public TMPro.TMP_InputField buyAmountInput; // Assign this in inspector

    private Stock stock;

    private float lastPrice;
    private bool firstUpdate = true;
    private Color currentColor = Color.white;
    private Coroutine colorFlashCoroutine;

    public void Initialize(Stock stockData)
    {
        stock = stockData;
        lastPrice = stock.price;
        currentColor = Color.white;
        Refresh();

        buyButton.onClick.AddListener(OnBuyClicked);
        sellButton.onClick.AddListener(OnSellClicked);
    }

     public void Refresh()
    {
        stockNameText.text = stock.name;
        stockPriceText.text = $"${stock.price:F2}";

        int holdings = StockTradeSystem.Instance.GetHolding(stock.stockID);
        holdingText.text = $"Owned: {holdings}";

        if (stock.price > lastPrice)
        {
            currentColor = Color.green;
            lastPrice = stock.price;
        }
        else if (stock.price < lastPrice)
        {
            currentColor = Color.red;
            lastPrice = stock.price;
        }
        // else price == lastPrice, keep currentColor unchanged

        stockPriceText.color = currentColor;
    }

    private void StartColorFlash(Color flashColor)
    {
        if (colorFlashCoroutine != null)
            StopCoroutine(colorFlashCoroutine);

        colorFlashCoroutine = StartCoroutine(ColorFlashRoutine(flashColor, 0.5f));
    }

    private IEnumerator ColorFlashRoutine(Color flashColor, float duration)
    {
        stockPriceText.color = flashColor;
        yield return new WaitForSeconds(duration);
        stockPriceText.color = Color.white;
        colorFlashCoroutine = null;
    }

void OnBuyClicked()
{
    int amount = 1;
    if (buyAmountInput != null && int.TryParse(buyAmountInput.text, out int parsedAmount))
    {
        amount = Mathf.Max(1, parsedAmount); // minimum 1
    }

    StockTradeSystem.Instance.BuyStock(stock, amount, success =>
    {
        if (success)
            Refresh(); // update UI after successful buy
    });
}

void OnSellClicked()
{
    int amount = 1;
    if (buyAmountInput != null && int.TryParse(buyAmountInput.text, out int parsedAmount))
    {
        amount = Mathf.Max(1, parsedAmount);
    }

    StockTradeSystem.Instance.SellStock(stock, amount, success =>
    {
        if (success)
            Refresh(); // update UI after successful sell
    });
}

}
