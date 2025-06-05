using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashboardUI : MonoBehaviour
{
    [Header("Player Stats UI")]
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI netWorthText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI xpText;

    [Header("Stock UI")]
    public Transform stockListContainer; // The parent layout group
    public GameObject stockEntryPrefab;

    private List<StockUIEntry> activeStockEntries = new List<StockUIEntry>();

    void Start()
    {
        PopulateStockList();
    }

    void Update()
    {
        UpdatePlayerStatsUI();
        RefreshStockPrices();
    }

    void UpdatePlayerStatsUI()
    {
        if (PlayerStats.Instance != null)
        {
            cashText.text = $"Cash:\n${PlayerStats.Instance.Cash}";
            netWorthText.text = $"Net Worth:\n${PlayerStats.Instance.NetWorth}";
            debtText.text = $"Debt:\n${PlayerStats.Instance.Debt}";
            xpText.text = $"XP:\n{PlayerStats.Instance.XP}";
        }
    }

    void PopulateStockList()
    {
        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            GameObject entry = Instantiate(stockEntryPrefab, stockListContainer);
            StockUIEntry uiEntry = entry.GetComponent<StockUIEntry>();
            uiEntry.Initialize(stock);
            activeStockEntries.Add(uiEntry);
        }

        Debug.Log($"[DashboardUI] Instantiated {activeStockEntries.Count} stock entries.");
    }

    void RefreshStockPrices()
    {
        foreach (var entry in activeStockEntries)
        {
            entry.Refresh();
        }
    }
}
