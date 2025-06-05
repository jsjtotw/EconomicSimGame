// DashboardUI.cs
// Manages the display of player statistics, including cash, net worth, XP/Level,
// and now includes displaying achievement unlock notifications.

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Required for Slider

public class DashboardUI : MonoBehaviour
{
    [Header("Player Stats UI")]
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI netWorthText;
    public TextMeshProUGUI debtText;

    // --- XP and Level UI Elements ---
    [Header("XP and Level UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpProgressText; // Renamed for clarity, displays current XP / XP to next level
    public Slider xpSlider; // The progress bar for XP

    [Header("Stock UI")]
    public Transform stockListContainer; // The parent layout group
    public GameObject stockEntryPrefab;

    private List<StockUIEntry> activeStockEntries = new List<StockUIEntry>();

    void Start()
    {
        // Populate the stock list on start.
        PopulateStockList();

        // Subscribe to PlayerXP events for UI updates.
        if (PlayerXP.Instance != null)
        {
            PlayerXP.Instance.OnLevelUp += UpdateLevelDisplay; // Update level text on level up
            UpdateXPDisplay(); // Initial call to ensure XP UI is updated on start.
        }
        else
        {
            Debug.LogError("[DashboardUI] PlayerXP.Instance not found. XP and Level UI will not function.");
        }

        // --- NEW: Subscribe to AchievementSystem for notifications ---
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUnlocked += ShowAchievementNotification;
        }
        else
        {
            Debug.LogError("[DashboardUI] AchievementSystem.Instance not found. Achievement notifications will not display.");
        }

        // Initial update for player stats UI as well.
        UpdatePlayerStatsUI();
    }

    void Update()
    {
        // Update player stats and refresh stock prices every frame (consider moving to timed updates for performance).
        UpdatePlayerStatsUI();
        RefreshStockPrices();
        // Periodically update XP display (or just on XP gain/level up events)
        UpdateXPDisplay(); 
    }

    /// <summary>
    /// Updates the display of player's core stats (cash, net worth, debt).
    /// </summary>
    void UpdatePlayerStatsUI()
    {
        if (PlayerStats.Instance != null)
        {
            if (cashText != null) cashText.text = $"Cash:\n${PlayerStats.Instance.Cash}";
            if (netWorthText != null) netWorthText.text = $"Net Worth:\n${PlayerStats.Instance.NetWorth}";
            if (debtText != null) debtText.text = $"Debt:\n${PlayerStats.Instance.Debt}";
        }
    }

    /// <summary>
    /// Populates the UI with stock entries based on data from StockMarketSystem.
    /// </summary>
    void PopulateStockList()
    {
        if (StockMarketSystem.Instance == null)
        {
            Debug.LogError("[DashboardUI] StockMarketSystem.Instance not found. Cannot populate stock list.");
            return;
        }

        foreach (var stock in StockMarketSystem.Instance.allStocks)
        {
            GameObject entry = Instantiate(stockEntryPrefab, stockListContainer);
            StockUIEntry uiEntry = entry.GetComponent<StockUIEntry>();
            if (uiEntry != null)
            {
                uiEntry.Initialize(stock);
                activeStockEntries.Add(uiEntry);
            }
            else
            {
                Debug.LogWarning("[DashboardUI] StockUIEntry component not found on instantiated prefab.");
            }
        }

        Debug.Log($"[DashboardUI] Instantiated {activeStockEntries.Count} stock entries.");
    }

    /// <summary>
    /// Refreshes the displayed stock prices for all active stock entries.
    /// </summary>
    void RefreshStockPrices()
    {
        foreach (var entry in activeStockEntries)
        {
            entry.Refresh();
        }
    }

    /// <summary>
    /// Updates the level text on the UI. Called when the player levels up.
    /// </summary>
    /// <param name="newLevel">The new level of the player.</param>
    private void UpdateLevelDisplay(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Level: {newLevel}";
            Debug.Log($"[DashboardUI] UI updated to Level {newLevel}.");
        }
        UpdateXPDisplay(); // Ensure XP bar also updates immediately on level change.
    }

    /// <summary>
    /// Updates the XP text and slider to show current XP progress towards the next level.
    /// </summary>
    public void UpdateXPDisplay()
    {
        if (PlayerXP.Instance == null) return;

        int currentXP = PlayerXP.Instance.CurrentXP;
        int currentLevel = PlayerXP.Instance.CurrentLevel;
        int xpForCurrentLevel = PlayerXP.Instance.GetXPForLevel(currentLevel);
        int xpForNextLevel = PlayerXP.Instance.GetXPForNextLevel();

        // Calculate XP earned *within* the current level's progress
        int xpEarnedThisLevel = currentXP - xpForCurrentLevel;
        // Calculate the total XP needed to complete the current level (reach the next one)
        int xpNeededForThisLevelProgress = xpForNextLevel - xpForCurrentLevel;

        if (xpProgressText != null)
        {
            xpProgressText.text = $"XP: {xpEarnedThisLevel} / {xpNeededForThisLevelProgress}";
        }

        if (xpSlider != null)
        {
            xpSlider.minValue = 0;
            xpSlider.maxValue = xpNeededForThisLevelProgress;
            xpSlider.value = xpEarnedThisLevel;
        }
        Debug.Log($"[DashboardUI] XP UI updated. Current: {currentXP}, Level: {currentLevel}, Next Level XP: {xpForNextLevel}");
    }

    /// <summary>
    /// Displays a notification when an achievement is unlocked.
    /// </summary>
    /// <param name="achievement">The unlocked achievement data.</param>
    private void ShowAchievementNotification(Achievement achievement)
    {
        if (PopupManager.Instance != null)
        {
            // --- NEW: Use PopupManager.ShowMessage which now handles titles ---
            PopupManager.Instance.ShowMessage(
                $"Achievement Unlocked: {achievement.name}!", // Title
                achievement.description,                        // Message
                null // No callback needed for a simple notification dismissal
            );
            Debug.Log($"[DashboardUI] Displayed notification for achievement: {achievement.name}");
        }
        else
        {
            Debug.LogWarning("[DashboardUI] PopupManager.Instance not found. Cannot display achievement notification.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks.
        if (PlayerXP.Instance != null)
        {
            PlayerXP.Instance.OnLevelUp -= UpdateLevelDisplay;
        }
        // --- NEW: Unsubscribe from AchievementSystem ---
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUnlocked -= ShowAchievementNotification;
        }
    }
}
