using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance;

    private List<EventData> allEvents = new List<EventData>();

    public float minEventIntervalHours = 48f;
    public float maxEventIntervalHours = 6f;

    private float hoursUntilNextEvent = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
                LoadEventsFromJson();
                Debug.Log("[EventSystem] Loaded " + allEvents.Count + " events.");
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
    // Helper class for JSON array deserialization
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

    private void Start()
    {
        ResetEventTimer();

        // Subscribe to TimeManager's hour tick event
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourAdvanced += OnHourAdvanced;
        }
        else
        {
            Debug.LogError("[EventSystem] TimeManager instance not found.");
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourAdvanced -= OnHourAdvanced;
        }
    }

    private void OnHourAdvanced(int year, int quarter, int day, int hour)
    {
        // Decrease countdown
        hoursUntilNextEvent -= 1f;

        if (hoursUntilNextEvent <= 0f)
        {
            TriggerRandomEvent();
            ResetEventTimer();
        }
    }

    private void ResetEventTimer()
    {
        hoursUntilNextEvent = UnityEngine.Random.Range(minEventIntervalHours, maxEventIntervalHours);
        Debug.Log($"[EventSystem] Next event in {hoursUntilNextEvent:F2} game hours.");
    }

    private void LoadEventsFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Events");
        if (jsonText == null)
        {
            Debug.LogError("[EventSystem] Could not load events.json from Resources/Events");
            return;
        }

        try
        {
            allEvents = new List<EventData>(JsonHelper.FromJson<EventData>(jsonText.text));
        }
        catch (Exception ex)
        {
            Debug.LogError("[EventSystem] Failed to parse JSON: " + ex.Message);
        }
    }

    public void TriggerRandomEvent()
    {
        if (allEvents.Count == 0)
        {
            Debug.LogWarning("[EventSystem] No events loaded.");
            return;
        }

        EventData chosen = allEvents[UnityEngine.Random.Range(0, allEvents.Count)];
        EventData randomized = CloneWithRandomization(chosen);

        string formattedTitle = FormatEventText(randomized.title, randomized);
        string formattedDescription = FormatEventText(randomized.description, randomized);

        PopupManager.Instance.ShowMessage(formattedTitle, formattedDescription, () =>
        {
            ApplyEffect(randomized);
        });
    }

    private EventData CloneWithRandomization(EventData evt)
    {
        EventData clone = new EventData
        {
            id = evt.id,
            title = evt.title,
            description = evt.description,
            effect = evt.effect,
            value = (evt.minValue != 0 || evt.maxValue != 0)
                ? UnityEngine.Random.Range(evt.minValue, evt.maxValue)
                : evt.value
        };

        switch (evt.effect)
        {
            case "industry_boom":
            case "industry_crash":
                var industries = StockMarketSystem.Instance.GetAllIndustries();
                clone.industry = !string.IsNullOrEmpty(evt.industry) ? evt.industry : industries[UnityEngine.Random.Range(0, industries.Count)];
                break;

            case "company_boom":
            case "company_crash":
                var companies = StockMarketSystem.Instance.GetAllCompanyCodes();
                clone.company = !string.IsNullOrEmpty(evt.company) ? evt.company : companies[UnityEngine.Random.Range(0, companies.Count)];
                break;
        }

        return clone;
    }

    private string FormatEventText(string template, EventData evt)
    {
        string result = template;

        if (evt.value != 0)
        {
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
                PlayerStats.Instance.Cash += Mathf.RoundToInt(evt.value);
                Debug.Log($"[EventSystem] Bonus applied: +${evt.value}");
                break;
            case "loss":
                PlayerStats.Instance.Cash -= Mathf.RoundToInt(evt.value);
                Debug.Log($"[EventSystem] Loss applied: -${evt.value}");
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
                // No mechanical effect
                break;
            default:
                Debug.LogWarning("[EventSystem] Unknown effect type: " + evt.effect);
                break;
        }
    }

    private void ApplyGlobalPriceChange(float changeFraction)
    {
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            float oldPrice = stock.price;
            stock.price *= (1 + changeFraction);
            Debug.Log($"[EventSystem] Global change: {stock.name} price {oldPrice} -> {stock.price}");
        }
    }

    private void ApplyIndustryPriceChange(string industry, float changeFraction)
    {
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            if (stock.industry == industry)
            {
                float oldPrice = stock.price;
                stock.price *= (1 + changeFraction);
                Debug.Log($"[EventSystem] Industry change: {stock.name} price {oldPrice} -> {stock.price}");
            }
        }
    }

    private void ApplyCompanyPriceChange(string companyCode, float changeFraction)
    {
        var stock = StockMarketSystem.Instance.GetStockByCode(companyCode);
        if (stock != null)
        {
            float oldPrice = stock.price;
            stock.price *= (1 + changeFraction);
            Debug.Log($"[EventSystem] Company change: {stock.name} price {oldPrice} -> {stock.price}");
        }
        else
        {
            Debug.LogWarning($"[EventSystem] Company {companyCode} not found.");
        }
    }

    private void ApplyRandomPriceWobble(float maxFraction)
    {
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            float changeFraction = UnityEngine.Random.Range(-maxFraction, maxFraction);
            float oldPrice = stock.price;
            stock.price *= (1 + changeFraction);
            Debug.Log($"[EventSystem] Mixed shift: {stock.name} price {oldPrice} -> {stock.price}");
        }
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[EventSystem] 'E' key pressed — triggering a random event.");
            TriggerRandomEvent();
            ResetEventTimer();
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("[EventSystem] '1' key pressed — triggering Market Crash event.");
            TriggerEventById("market_crash");
            ResetEventTimer();
        }
    }

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
        }
        else
        {
            Debug.LogWarning($"[EventSystem] Event with ID '{eventId}' not found.");
        }
    }
}