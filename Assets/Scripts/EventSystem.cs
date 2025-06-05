// EventSystem.cs
// Manages the triggering and application of various game events.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // For keyboard input, if used for debugging/testing events

public class EventSystem : MonoBehaviour
{
    // Singleton instance for EventSystem.
    public static EventSystem Instance;

    // List of all loaded event data.
    private List<EventData> allEvents = new List<EventData>();

    // Range for how often random events occur (in game hours).
    public float minEventIntervalHours = 6f; // Corrected min/max as per typical usage, max should be greater than min
    public float maxEventIntervalHours = 48f;

    // Countdown until the next random event.
    private float hoursUntilNextEvent = 0f;

    // --- NEW: Event for when an event's effect is applied ---
    public event Action<EventData> OnEventApplied;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern and loads event data.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this GameObject across scenes.
            LoadEventsFromJson();
            Debug.Log("[EventSystem] Loaded " + allEvents.Count + " events.");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }
    
    // Helper class for JSON array deserialization (utility for loading events from JSON).
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(FixJson(json));
            return wrapper.Items;
        }
    
        private static string FixJson(string value)
        {
            value = value.Trim();
            if (!value.StartsWith("{"))
            {
                // JsonUtility cannot directly deserialize a root array, so wrap it.
                value = "{\"Items\":" + value + "}";
            }
            return value;
        }
    
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    /// <summary>
    /// Called before the first frame update.
    /// Sets the initial event timer and subscribes to TimeManager.
    /// </summary>
    private void Start()
    {
        ResetEventTimer();

        // Subscribe to TimeManager's hour tick event to trigger events periodically.
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourAdvanced += OnHourAdvanced;
            Debug.Log("[EventSystem] Subscribed to TimeManager.OnHourAdvanced.");
        }
        else
        {
            Debug.LogError("[EventSystem] TimeManager instance not found. Events will not advance automatically.");
        }
    }

    /// <summary>
    /// Called when the GameObject is destroyed.
    /// Unsubscribe from TimeManager to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourAdvanced -= OnHourAdvanced;
            Debug.Log("[EventSystem] Unsubscribed from TimeManager.OnHourAdvanced.");
        }
    }

    /// <summary>
    /// Callback method for TimeManager.OnHourAdvanced event.
    /// Decreases the event countdown and triggers a new event if timer reaches zero.
    /// </summary>
    private void OnHourAdvanced(int year, int quarter, int day, int hour)
    {
        hoursUntilNextEvent -= 1f; // Decrease countdown by one game hour.

        if (hoursUntilNextEvent <= 0f)
        {
            TriggerRandomEvent(); // Trigger an event.
            ResetEventTimer();    // Reset the timer for the next event.
        }
    }

    /// <summary>
    /// Resets the timer for the next random event.
    /// </summary>
    private void ResetEventTimer()
    {
        hoursUntilNextEvent = UnityEngine.Random.Range(minEventIntervalHours, maxEventIntervalHours);
        Debug.Log($"[EventSystem] Next random event in {hoursUntilNextEvent:F2} game hours.");
    }

    /// <summary>
    /// Loads event data from a JSON file in the Resources folder.
    /// </summary>
    private void LoadEventsFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Events"); // Assumes events JSON is named "Events.json"
        if (jsonText == null)
        {
            Debug.LogError("[EventSystem] Could not load 'Events.json' from Resources folder. Make sure the file exists and is named 'Events'.");
            return;
        }

        try
        {
            allEvents = new List<EventData>(JsonHelper.FromJson<EventData>(jsonText.text));
            Debug.Log($"[EventSystem] Successfully parsed {allEvents.Count} events from JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[EventSystem] Failed to parse JSON for events: {ex.Message}");
        }
    }

    /// <summary>
    /// Triggers a random event from the loaded list.
    /// </summary>
    public void TriggerRandomEvent()
    {
        if (allEvents.Count == 0)
        {
            Debug.LogWarning("[EventSystem] No events loaded to trigger.");
            return;
        }

        EventData chosen = allEvents[UnityEngine.Random.Range(0, allEvents.Count)];
        // Create a clone with randomized values for the specific event instance.
        EventData randomized = CloneWithRandomization(chosen);

        // Format event text with dynamic values.
        string formattedTitle = FormatEventText(randomized.title, randomized);
        string formattedDescription = FormatEventText(randomized.description, randomized);

        // Show the event message to the player via PopupManager, then apply its effect.
        PopupManager.Instance.ShowMessage(formattedTitle, formattedDescription, () =>
        {
            ApplyEffect(randomized);
        });
        Debug.Log($"[EventSystem] Random event triggered: {randomized.title}");
    }

    /// <summary>
    /// Creates a clone of an EventData object and randomizes its 'value' if min/max are set.
    /// Also sets industry/company for generic events.
    /// </summary>
    /// <param name="evt">The base EventData to clone.</param>
    /// <returns>A new EventData instance with randomized values.</returns>
    private EventData CloneWithRandomization(EventData evt)
    {
        EventData clone = new EventData
        {
            id = evt.id,
            title = evt.title,
            description = evt.description,
            effect = evt.effect,
            // Randomize value if min/max are specified in the event data.
            value = (evt.minValue != 0 || evt.maxValue != 0)
                ? UnityEngine.Random.Range(evt.minValue, evt.maxValue)
                : evt.value,
            industry = evt.industry, // Copy existing industry/company
            company = evt.company
        };

        // If the event effect targets a generic industry or company (not specified in JSON),
        // randomly select one from the market.
        switch (evt.effect)
        {
            case "industry_boom":
            case "industry_crash":
                if (string.IsNullOrEmpty(clone.industry) && StockMarketSystem.Instance != null)
                {
                    var industries = StockMarketSystem.Instance.GetAllIndustries();
                    if (industries.Count > 0)
                        clone.industry = industries[UnityEngine.Random.Range(0, industries.Count)];
                }
                break;

            case "company_boom":
            case "company_crash":
                if (string.IsNullOrEmpty(clone.company) && StockMarketSystem.Instance != null)
                {
                    var companies = StockMarketSystem.Instance.GetAllCompanyCodes();
                    if (companies.Count > 0)
                        clone.company = companies[UnityEngine.Random.Range(0, companies.Count)];
                }
                break;
        }

        return clone;
    }

    /// <summary>
    /// Formats event text templates by replacing placeholders with actual values.
    /// </summary>
    /// <param name="template">The text template (e.g., "Market {industry} is booming by {percentage}%!").</param>
    /// <param name="evt">The EventData containing the values to insert.</param>
    /// <returns>The formatted string.</returns>
    private string FormatEventText(string template, EventData evt)
    {
        string result = template;

        if (evt.value != 0)
        {
            // Format value as percentage or raw amount.
            string percentageStr = (evt.value * 100f).ToString("F1");
            string amountStr = Mathf.RoundToInt(evt.value).ToString();

            result = result.Replace("{percentage}", percentageStr);
            result = result.Replace("{amount}", amountStr);
        }

        if (!string.IsNullOrEmpty(evt.industry))
        {
            result = result.Replace("{industry}", evt.industry);
        }

        if (!string.IsNullOrEmpty(evt.company))
        {
            result = result.Replace("{company}", evt.company);
        }

        return result;
    }

    /// <summary>
    /// Applies the effect of an event to the game state.
    /// This is where the 'Retail' company bonus will be applied for 'bonus' effects.
    /// </summary>
    /// <param name="evt">The EventData whose effect is to be applied.</param>
    private void ApplyEffect(EventData evt)
    {
        switch (evt.effect)
        {
            case "crash":
                ApplyGlobalPriceChange(-evt.value);
                break;
            case "boom":
                ApplyGlobalPriceChange(evt.value);
                break;
            case "bonus":
                int baseBonusAmount = Mathf.RoundToInt(evt.value);
                float finalBonusAmount = baseBonusAmount;

                // Check if PlayerCompany exists and if the selected type is RETAIL
                if (PlayerCompany.Instance != null && PlayerCompany.Instance.IsCompanyChosen &&
                    PlayerCompany.Instance.SelectedType == CompanyType.RETAIL && BudgetSystem.Instance != null)
                {
                    finalBonusAmount *= BudgetSystem.Instance.bonusIncomeMultiplier;
                    Debug.Log($"[EventSystem] Retail perk applied to bonus event. Base: ${baseBonusAmount}, Final: ${finalBonusAmount:F0}");
                }

                PlayerStats.Instance.Cash += Mathf.RoundToInt(finalBonusAmount);
                Debug.Log($"[EventSystem] Bonus applied: +${Mathf.RoundToInt(finalBonusAmount)}. Gaining XP for bonus event.");
                // Add XP for successful bonus event
                if (PlayerXP.Instance != null)
                {
                    PlayerXP.Instance.AddXP(Mathf.RoundToInt(finalBonusAmount * 0.2f)); // Example: 20% of bonus cash as XP
                }
                break;
            case "loss":
                PlayerStats.Instance.Cash -= Mathf.RoundToInt(evt.value);
                Debug.Log($"[EventSystem] Loss applied: -${evt.value}. Gaining XP for surviving loss event.");
                // Add XP for surviving a loss event (even if cash decreases)
                if (PlayerXP.Instance != null)
                {
                    PlayerXP.Instance.AddXP(Mathf.RoundToInt(evt.value * 0.1f)); // Example: 10% of loss value as XP for surviving
                }
                break;
            case "industry_boom":
                ApplyIndustryPriceChange(evt.industry, evt.value);
                break;
            case "industry_crash":
                ApplyIndustryPriceChange(evt.industry, -evt.value);
                break;
            case "company_boom":
                ApplyCompanyPriceChange(evt.company, evt.value);
                break;
            case "company_crash":
                ApplyCompanyPriceChange(evt.company, -evt.value);
                break;
            case "mixed_shift":
                ApplyRandomPriceWobble(evt.value);
                break;
            case "news_fluff":
                // No mechanical effect, primarily for narrative or flavor.
                Debug.Log($"[EventSystem] News Fluff event: {evt.title} - No mechanical effect.");
                break;
            default:
                Debug.LogWarning("[EventSystem] Unknown effect type: " + evt.effect + " for event: " + evt.title);
                break;
        }
        // --- NEW: Invoke the OnEventApplied event ---
        OnEventApplied?.Invoke(evt);
    }

    /// <summary>
    /// Applies a global price change to all stocks in the market.
    /// </summary>
    /// <param name="changeFraction">The fraction by which to change prices (e.g., 0.1 for +10%, -0.05 for -5%).</param>
    private void ApplyGlobalPriceChange(float changeFraction)
    {
        if (StockMarketSystem.Instance == null) return;
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            float oldPrice = stock.price;
            stock.price *= (1 + changeFraction);
            // Ensure price remains positive.
            stock.price = Mathf.Max(stock.price, 0.01f);
            Debug.Log($"[EventSystem] Global change: {stock.name} price {oldPrice:F2} -> {stock.price:F2}");
        }
    }

    /// <summary>
    /// Applies a price change to stocks within a specific industry.
    /// </summary>
    /// <param name="industry">The target industry (e.g., "Technology").</param>
    /// <param name="changeFraction">The fraction by which to change prices.</param>
    private void ApplyIndustryPriceChange(string industry, float changeFraction)
    {
        if (StockMarketSystem.Instance == null) return;
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            if (stock.industry.Equals(industry, StringComparison.OrdinalIgnoreCase)) // Case-insensitive comparison
            {
                float oldPrice = stock.price;
                stock.price *= (1 + changeFraction);
                stock.price = Mathf.Max(stock.price, 0.01f);
                Debug.Log($"[EventSystem] Industry change ({industry}): {stock.name} price {oldPrice:F2} -> {stock.price:F2}");
            }
        }
    }

    /// <summary>
    /// Applies a price change to a specific company's stock.
    /// </summary>
    /// <param name="companyCode">The stock ID of the target company.</param>
    /// <param name="changeFraction">The fraction by which to change the price.</param>
    private void ApplyCompanyPriceChange(string companyCode, float changeFraction)
    {
        if (StockMarketSystem.Instance == null) return;
        var stock = StockMarketSystem.Instance.GetStockByCode(companyCode);
        if (stock != null)
        {
            float oldPrice = stock.price;
            stock.price *= (1 + changeFraction);
            stock.price = Mathf.Max(stock.price, 0.01f);
            Debug.Log($"[EventSystem] Company change ({companyCode}): {stock.name} price {oldPrice:F2} -> {stock.price:F2}");
        }
        else
        {
            Debug.LogWarning($"[EventSystem] Company stock with ID '{companyCode}' not found for effect application.");
        }
    }

    /// <summary>
    /// Applies a random price fluctuation (wobble) to all stocks.
    /// </summary>
    /// <param name="maxFraction">The maximum absolute fraction for random price change.</param>
    private void ApplyRandomPriceWobble(float maxFraction)
    {
        if (StockMarketSystem.Instance == null) return;
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            float changeFraction = UnityEngine.Random.Range(-maxFraction, maxFraction);
            float oldPrice = stock.price;
            stock.price *= (1 + changeFraction);
            stock.price = Mathf.Max(stock.price, 0.01f);
            Debug.Log($"[EventSystem] Mixed shift: {stock.name} price {oldPrice:F2} -> {stock.price:F2}");
        }
    }

    /// <summary>
    /// This Update method is typically used for continuous game logic.
    /// Here, it's used for debugging input to manually trigger events.
    /// </summary>
    void Update()
    {
        // Debugging / Testing: Trigger random event with 'E' key
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[EventSystem] 'E' key pressed — triggering a random event.");
            TriggerRandomEvent();
            ResetEventTimer(); // Reset timer after manual trigger too.
        }

        // Debugging / Testing: Trigger specific event by ID with '1' key
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("[EventSystem] '1' key pressed — triggering Market Crash event.");
            TriggerEventById("market_crash"); // Ensure "market_crash" exists in your Events.json
            ResetEventTimer(); // Reset timer after manual trigger too.
        }
    }

    /// <summary>
    /// Triggers a specific event by its ID.
    /// </summary>
    /// <param name="eventId">The unique ID of the event to trigger.</param>
    public void TriggerEventById(string eventId)
    {
        EventData evt = allEvents.Find(e => e.id == eventId);
        if (evt != null)
        {
            EventData randomized = CloneWithRandomization(evt);
            string formattedTitle = FormatEventText(randomized.title, randomized);
            string formattedDescription = FormatEventText(randomized.description, randomized);

            PopupManager.Instance.ShowMessage(formattedTitle, formattedDescription, () =>
            {
                ApplyEffect(randomized);
            });
            Debug.Log($"[EventSystem] Event triggered by ID: {eventId}");
        }
        else
        {
            Debug.LogWarning($"[EventSystem] Event with ID '{eventId}' not found.");
        }
    }
}
