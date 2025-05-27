// File: Assets/Scripts/GameFlow/CompanySelector.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic;

// CompanyProfile data class (can be in its own file or within CompanySelector.cs or DataManager)
[System.Serializable]
public class CompanyProfile
{
    public string id;
    public string companyName;
    public string description;
    public float startingCashBonus;
    public float startingStockValueBonus; // Percentage increase for stocks in primary industry
    public float xpGainRateMultiplier;
    public string primaryIndustry; // "AI", "Food Chain", "Green Energy"
    public float eventSensitivityMultiplier; // How much events affect this company's stocks
}

public class CompanySelector : MonoBehaviour
{
    public static CompanySelector Instance { get; private set; }

    public GameObject selectionPanel; // UI Panel for company selection
    public TextMeshProUGUI companyNameText;
    public TextMeshProUGUI companyDescriptionText;
    public TextMeshProUGUI companyBonusText;
    public int currentSelectionIndex = 0;

    private List<CompanyProfile> companyOptions;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Load company templates from DataManager
        if (DataManager.Instance != null)
        {
            companyOptions = new List<CompanyProfile>(DataManager.Instance.CompanyTemplates);
        }
        else
        {
            Debug.LogError("DataManager not found. CompanySelector cannot load templates.");
            companyOptions = new List<CompanyProfile>();
        }

        if (companyOptions.Count > 0)
        {
            DisplayCompanyOption(currentSelectionIndex);
        }
        else
        {
            Debug.LogError("No company options loaded!");
            // Automatically start game if no options (e.g., for testing)
            GameManager.Instance?.StartGame();
            HideSelectionPanel();
        }
    }

    public void ShowSelectionPanel()
    {
        selectionPanel?.SetActive(true);
        if (companyOptions.Count > 0)
        {
            DisplayCompanyOption(currentSelectionIndex);
        }
    }

    public void HideSelectionPanel()
    {
        selectionPanel?.SetActive(false);
    }

    void DisplayCompanyOption(int index)
    {
        if (index >= 0 && index < companyOptions.Count)
        {
            CompanyProfile selectedCompany = companyOptions[index];
            companyNameText.text = selectedCompany.companyName;
            companyDescriptionText.text = selectedCompany.description;
            companyBonusText.text = $"Starting Cash: +${selectedCompany.startingCashBonus:N0}\n" +
                                    $"XP Gain: +{(selectedCompany.xpGainRateMultiplier - 1) * 100:F0}%\n" +
                                    $"Primary Industry: {selectedCompany.primaryIndustry}";
        }
    }

    public void NextOption()
    {
        currentSelectionIndex = (currentSelectionIndex + 1) % companyOptions.Count;
        DisplayCompanyOption(currentSelectionIndex);
    }

    public void PreviousOption()
    {
        currentSelectionIndex--;
        if (currentSelectionIndex < 0)
        {
            currentSelectionIndex = companyOptions.Count - 1;
        }
        DisplayCompanyOption(currentSelectionIndex);
    }

    public void ConfirmSelection()
    {
        CompanyProfile chosenCompany = companyOptions[currentSelectionIndex];
        ApplyCompanyBonuses(chosenCompany);
        HideSelectionPanel();
        GameManager.Instance?.StartGame(chosenCompany); // Pass chosen company to GameManager
    }

    void ApplyCompanyBonuses(CompanyProfile profile)
    {
        // Apply stock value bonus to relevant initial stocks
        if (GameManager.Instance.StockMarket != null)
        {
            // FIX: Changed from currentStocks to CurrentStocks (public property)
            foreach (var stock in GameManager.Instance.StockMarket.CurrentStocks)
            {
                if (stock.industry == profile.primaryIndustry)
                {
                    // FIX: Changed from stock.currentPrice to stock.CurrentPrice (public property)
                    stock.SetPrice(stock.CurrentPrice * (1 + profile.startingStockValueBonus)); // Use SetPrice to update
                    Debug.Log($"Applied {profile.startingStockValueBonus * 100}% bonus to {stock.Name} (Initial Price: {stock.CurrentPrice:F2})"); // Use stock.Name and stock.CurrentPrice
                }
            }
        }
        // Other bonuses like cash and XP multiplier are passed to GameManager for application
    }
}