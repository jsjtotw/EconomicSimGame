// File: Assets/Scripts/Core/DataManager.cs
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json; // Requires Newtonsoft Json.NET
using System.Linq;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public List<GameEvent> Events { get; private set; }
    public List<Stock> Stocks { get; private set; }
    public List<CompanyProfile> CompanyTemplates { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
    }

    void LoadAllData()
    {
        LoadEvents();
        LoadStocks();
        LoadCompanyTemplates();
    }

    void LoadEvents()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/EventList");
        if (jsonFile != null)
        {
            Events = JsonConvert.DeserializeObject<List<GameEvent>>(jsonFile.text);
            Debug.Log($"Loaded {Events.Count} events from EventList.json.");
        }
        else
        {
            Debug.LogError("EventList.json not found in Resources/Data folder!");
            Events = new List<GameEvent>();
        }
    }

    void LoadStocks()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/StockData");
        if (jsonFile != null)
        {
            Stocks = JsonConvert.DeserializeObject<List<Stock>>(jsonFile.text);
            Debug.Log($"Loaded {Stocks.Count} stocks from StockData.json.");
        }
        else
        {
            Debug.LogError("StockData.json not found in Resources/Data folder!");
            Stocks = new List<Stock>();
        }
    }

    void LoadCompanyTemplates()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/CompanyTemplates");
        if (jsonFile != null)
        {
            CompanyTemplates = JsonConvert.DeserializeObject<List<CompanyProfile>>(jsonFile.text);
            Debug.Log($"Loaded {CompanyTemplates.Count} company templates from CompanyTemplates.json.");
        }
        else
        {
            Debug.LogError("CompanyTemplates.json not found in Resources/Data folder!");
            CompanyTemplates = new List<CompanyProfile>();
        }
    }
}