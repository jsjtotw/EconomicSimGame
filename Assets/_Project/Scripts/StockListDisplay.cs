using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StockListDisplay : MonoBehaviour
{
    public GameObject stockEntryPrefab;  // Prefab with 3 TMP_Texts
    public Transform contentParent;      // Scroll view content area

    private List<GameObject> currentEntries = new List<GameObject>();

    public void RefreshStockList(List<Stock> stocks)
    {
        Debug.Log("Refreshing stock list...");

        foreach (var entry in currentEntries)
            Destroy(entry);
        currentEntries.Clear();

        if (stocks == null)
        {
            Debug.LogWarning("No stocks to display!");
            return;
        }

        foreach (var stock in stocks)
        {
            GameObject entry = Instantiate(stockEntryPrefab, contentParent);
            TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>();

            if (texts.Length < 3)
            {
                Debug.LogWarning("Stock prefab must have 3 TMP_Texts for name, quantity, and earned.");
                continue;
            }

            texts[0].text = stock.stockName;
            texts[1].text = $"{stock.quantityOwned}";
            texts[2].text = $"Earned: ${stock.totalEarned:F2}";

            currentEntries.Add(entry);
        }
    }
}
