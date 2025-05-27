// File: Assets/Scripts/UI/Prefabs/StockUIEntry.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI; // For Button and Image
using System.Collections.Generic; // For List

public class StockUIEntry : MonoBehaviour
{
    [Header("Stock Info UI")]
    public TextMeshProUGUI stockNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI ownedSharesText; // --- NEW FOR MVP ---
    public Image trendIndicatorImage; // --- NEW FOR MVP --- (e.g., an arrow sprite)
    public Color priceIncreaseColor = Color.green; // --- NEW FOR MVP ---
    public Color priceDecreaseColor = Color.red; // --- NEW FOR MVP ---
    public Sprite upArrowSprite; // --- NEW FOR MVP ---
    public Sprite downArrowSprite; // --- NEW FOR MVP ---
    public Sprite neutralSprite; // --- NEW FOR MVP --- (e.g., a dash or circle)

    [Header("Buy/Sell UI")]
    public TMP_InputField buyQuantityInput;
    public Button buyButton;
    public TMP_InputField sellQuantityInput;
    public Button sellButton;

    // --- NEW FOR MVP: Error/Info Text ---
    public TextMeshProUGUI messageText; // For displaying errors like "Cannot afford"

    private Stock currentStock;
    // --- NEW FOR MVP: For trend calculation ---
    private float lastPrice;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        sellButton.onClick.AddListener(OnSellButtonClicked);
        // --- NEW FOR MVP: Clear message text initially ---
        if (messageText != null) messageText.text = "";
    }

    void OnDestroy()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClicked);
        sellButton.onClick.RemoveListener(OnSellButtonClicked);
        if (currentStock != null)
        {
            currentStock.onPriceChanged.RemoveListener(UpdatePrice); // FIX: Unsubscribe when destroyed using RemoveListener
        }
        // FIX: Unsubscribe from StockTradeSystem's event
        if (StockTradeSystem.Instance != null && currentStock != null)
        {
            StockTradeSystem.Instance.onSharesOwnedChanged.RemoveListener(UpdateOwnedSharesDisplay);
        }
    }

    public void Initialize(Stock stock)
    {
        currentStock = stock;
        stockNameText.text = stock.Name; // FIX: Use stock.Name
        lastPrice = stock.CurrentPrice; // Initialize last price for trend calculation // FIX: Use stock.CurrentPrice

        // Subscribe to price changes for real-time updates
        currentStock.onPriceChanged.AddListener(UpdatePrice); // FIX: Use AddListener for UnityEvent
        UpdatePrice(stock.CurrentPrice); // Initial price update // FIX: Use stock.CurrentPrice

        // Subscribe to owned shares change
        if (StockTradeSystem.Instance != null)
        {
            // FIX: Subscribe to the event, and provide the ticker to filter updates
            StockTradeSystem.Instance.onSharesOwnedChanged.AddListener(UpdateOwnedSharesDisplay);
            UpdateOwnedSharesDisplay(stock.Ticker, StockTradeSystem.Instance.GetOwnedShares(stock.Ticker)); // Initial update // FIX: Use stock.Ticker
        }
    }

    private void UpdatePrice(float newPrice)
    {
        priceText.text = $"${newPrice:F2}"; // Format to 2 decimal places

        // --- NEW FOR MVP: Update trend indicator and color ---
        if (trendIndicatorImage != null)
        {
            if (newPrice > lastPrice)
            {
                trendIndicatorImage.sprite = upArrowSprite;
                priceText.color = priceIncreaseColor;
            }
            else if (newPrice < lastPrice)
            {
                trendIndicatorImage.sprite = downArrowSprite;
                priceText.color = priceDecreaseColor;
            }
            else
            {
                trendIndicatorImage.sprite = neutralSprite;
                priceText.color = Color.white; // Default color
            }
            trendIndicatorImage.enabled = true; // Make sure it's visible
        }
        else
        {
            priceText.color = Color.white; // No trend image, keep default color
        }
        lastPrice = newPrice; // Update last price for next calculation
    }

    // --- NEW FOR MVP: Update owned shares display ---
    public void UpdateOwnedSharesDisplay(string ticker, int newAmount) // FIX: Made public for DashboardUI to call
    {
        if (currentStock != null && currentStock.Ticker == ticker) // FIX: Use currentStock.Ticker
        {
            ownedSharesText.text = $"Owned: {newAmount}";
            // Disable sell button if no shares owned
            if (sellButton != null) sellButton.interactable = (newAmount > 0);
        }
    }

    private void OnBuyButtonClicked()
    {
        int quantity;
        if (int.TryParse(buyQuantityInput.text, out quantity))
        {
            if (quantity <= 0)
            {
                ShowMessage("Quantity must be positive.", Color.red);
                return;
            }
            if (StockTradeSystem.Instance != null && BudgetSystem.Instance != null)
            {
                float cost = quantity * currentStock.CurrentPrice; // FIX: Use currentStock.CurrentPrice
                if (!BudgetSystem.Instance.CanAfford(cost)) // FIX: Call BudgetSystem.CanAfford
                {
                    ShowMessage("Cannot afford.", Color.red);
                    return;
                }

                StockTradeSystem.Instance.BuyShares(currentStock.Ticker, quantity); // FIX: Call BuyShares, use Ticker
                ShowMessage($"Bought {quantity} shares.", Color.green);
                buyQuantityInput.text = ""; // Clear input after successful trade
                AudioManager.Instance?.PlaySFX("transactionSuccess"); // --- NEW FOR MVP: Play sound ---
            }
        }
        else
        {
            ShowMessage("Invalid quantity.", Color.red);
        }
    }

    private void OnSellButtonClicked()
    {
        int quantity;
        if (int.TryParse(sellQuantityInput.text, out quantity))
        {
            if (quantity <= 0)
            {
                ShowMessage("Quantity must be positive.", Color.red);
                return;
            }
            if (StockTradeSystem.Instance != null)
            {
                int owned = StockTradeSystem.Instance.GetOwnedShares(currentStock.Ticker); // FIX: Use currentStock.Ticker
                if (quantity > owned)
                {
                    ShowMessage($"Not enough shares. Owned: {owned}", Color.red);
                    return;
                }

                StockTradeSystem.Instance.SellShares(currentStock.Ticker, quantity); // FIX: Call SellShares, use Ticker
                ShowMessage($"Sold {quantity} shares.", Color.green);
                sellQuantityInput.text = ""; // Clear input after successful trade
                AudioManager.Instance?.PlaySFX("transactionSuccess"); // --- NEW FOR MVP: Play sound ---
            }
        }
        else
        {
            ShowMessage("Invalid quantity.", Color.red);
        }
    }

    // --- NEW FOR MVP: Method to display short messages ---
    private void ShowMessage(string msg, Color color)
    {
        if (messageText != null)
        {
            messageText.color = color;
            messageText.text = msg;
            // You might want a Coroutine here to clear the message after a few seconds
            Invoke("ClearMessage", 3f); // Clear after 3 seconds
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    // FIX: Add a public method to retrieve the stock's ticker for DashboardUI
    public string GetStockTicker()
    {
        return currentStock?.Ticker;
    }
}