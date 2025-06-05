// AchievementSystem.cs
// Manages the definition, tracking, and unlocking of game achievements.
// Now loads achievement data from a JSON file and includes early-game achievements.

using System;
using System.Collections.Generic;
using UnityEngine; // Ensure this is present!

// --- Achievement Data Structure (remains the same) ---
[Serializable]
public class Achievement
{
    public string id;          // Unique identifier for the achievement (e.g., "FIRST_50K_NETWORTH")
    public string name;        // Display name (e.g., "Humble Beginnings")
    public string description; // What the player needs to do (e.g., "Reach a net worth of $50,000.")
    public bool isUnlocked;    // True if the achievement has been unlocked.

    public Achievement(string id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.isUnlocked = false; // Achievements start locked.
    }
}

// --- Wrapper class for JSON array deserialization ---
[Serializable]
public class AchievementListWrapper
{
    public List<Achievement> achievements;
}

// --- CRITICAL FIX: Inherit from MonoBehaviour ---
public class AchievementSystem : MonoBehaviour
{
    // Singleton instance for AchievementSystem.
    public static AchievementSystem Instance;

    // List of all achievements in the game (will be loaded from JSON).
    public List<Achievement> achievements = new List<Achievement>();

    // Event triggered when an achievement is unlocked.
    public event Action<Achievement> OnAchievementUnlocked;

    // --- Private Tracking Variables for Achievements ---
    private int investmentsMade = 0; // Tracks total successful investments.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern and loads achievement data.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievementsFromJson(); // Load achievements from JSON instead of initializing programmatically.
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    /// <summary>
    /// Called when the script starts.
    /// Subscribes to events from other systems to check for achievement conditions.
    /// </summary>
    private void Start()
    {
        // Subscribe to PlayerStats' OnNetWorthChanged event (also implicitly triggers on Cash changes).
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnNetWorthChanged += CheckNetWorthAchievements;
        }
        else
        {
            Debug.LogError("[AchievementSystem] PlayerStats instance not found. Net worth/Cash achievements will not be checked.");
        }

        // Subscribe to StockTradeSystem's OnInvestmentMade event.
        if (StockTradeSystem.Instance != null)
        {
            StockTradeSystem.Instance.OnInvestmentMade += OnInvestmentMadeHandler;
        }
        else
        {
            Debug.LogError("[AchievementSystem] StockTradeSystem instance not found. Investment achievements will not be checked.");
        }

        // Subscribe to EventSystem's OnEventApplied event.
        if (EventSystem.Instance != null)
        {
            EventSystem.Instance.OnEventApplied += OnEventAppliedHandler;
        }
        else
        {
            Debug.LogError("[AchievementSystem] EventSystem instance not found. Event-based achievements will not be checked.");
        }
        
        // Subscribe to CreditSystem's OnLoanTaken event
        if (CreditSystem.Instance != null)
        {
            CreditSystem.Instance.OnLoanTaken += OnLoanTakenHandler;
        }
        else
        {
            Debug.LogError("[AchievementSystem] CreditSystem instance not found. Loan achievements will not be checked.");
        }

        Debug.Log("[AchievementSystem] Subscribed to relevant game events.");
    }

    /// <summary>
    /// Unsubscribes from events to prevent memory leaks when the GameObject is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnNetWorthChanged -= CheckNetWorthAchievements;
        }
        if (StockTradeSystem.Instance != null)
        {
            StockTradeSystem.Instance.OnInvestmentMade -= OnInvestmentMadeHandler;
        }
        if (EventSystem.Instance != null)
        {
            EventSystem.Instance.OnEventApplied -= OnEventAppliedHandler;
        }
        if (CreditSystem.Instance != null)
        {
            CreditSystem.Instance.OnLoanTaken -= OnLoanTakenHandler;
        }
        Debug.Log("[AchievementSystem] Unsubscribed from game events.");
    }

    /// <summary>
    /// Loads achievement data from a JSON file in the Resources folder.
    /// </summary>
    private void LoadAchievementsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("achievements_data"); // Assumes JSON is named "achievements_data.json"
        if (jsonFile != null)
        {
            // Use the wrapper class to deserialize the JSON array.
            AchievementListWrapper wrapper = JsonUtility.FromJson<AchievementListWrapper>(jsonFile.text);
            achievements = wrapper.achievements;

            // Ensure achievements are initialized as unlocked = false when loaded (unless it's a save/load system).
            // For a fresh start, explicitly set them to locked.
            foreach (var achievement in achievements)
            {
                achievement.isUnlocked = false;
            }

            Debug.Log($"[AchievementSystem] Loaded {achievements.Count} achievements from JSON.");
        }
        else
        {
            Debug.LogError("[AchievementSystem] 'achievements_data.json' not found in Resources folder. Achievements will not be loaded.");
            // If the JSON is not found, the 'achievements' list will remain empty.
        }
    }

    /// <summary>
    /// Unlocks an achievement by its ID if it's not already unlocked.
    /// Triggers the OnAchievementUnlocked event.
    /// </summary>
    /// <param name="id">The unique ID of the achievement to unlock.</param>
    public void UnlockAchievement(string id)
    {
        Achievement achievement = achievements.Find(a => a.id == id);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            Debug.Log($"[AchievementSystem] Achievement Unlocked: {achievement.name} ({achievement.description})");
            OnAchievementUnlocked?.Invoke(achievement); // Notify UI or other systems.
        }
    }

    // --- Achievement Condition Checkers (Event Handlers) ---

    /// <summary>
    /// Checks for net worth and cash based achievements when player's net worth changes.
    /// </summary>
    /// <param name="newNetWorth">The player's current net worth.</param>
    private void CheckNetWorthAchievements(int newNetWorth)
    {
        // Check for FIRST_50K_NETWORTH
        if (newNetWorth >= 50000)
        {
            UnlockAchievement("FIRST_50K_NETWORTH");
        }
        // Check for MILLIONAIRE_CLUB (already in JSON)
        if (newNetWorth >= 1000000)
        {
            UnlockAchievement("MILLIONAIRE_CLUB");
        }
        // Check for FIRST_1K_CASH
        if (PlayerStats.Instance != null && PlayerStats.Instance.Cash >= 11000)
        {
            UnlockAchievement("FIRST_1K_CASH");
        }
    }

    /// <summary>
    /// Handles the OnInvestmentMade event from StockTradeSystem to track investments.
    /// </summary>
    /// <param name="stock">The stock that was invested in.</param>
    /// <param name="amount">The amount of shares bought.</param>
    private void OnInvestmentMadeHandler(Stock stock, int amount)
    {
        // Check for FIRST_INVESTMENT
        UnlockAchievement("FIRST_INVESTMENT"); // Unlocked on the very first investment.

        investmentsMade++;
        Debug.Log($"[AchievementSystem] Total investments made: {investmentsMade}");
        if (investmentsMade >= 10)
        {
            UnlockAchievement("TEN_INVESTMENTS");
        }
        // Add more investment-based achievements here.
    }

    /// <summary>
    /// Handles the OnEventApplied event from EventSystem to check for crash events.
    /// </summary>
    /// <param name="appliedEvent">The EventData that was just applied.</param>
    private void OnEventAppliedHandler(EventData appliedEvent)
    {
        if (appliedEvent.effect == "crash" || appliedEvent.effect == "market_crash") // Assuming specific effects for a crash
        {
            UnlockAchievement("SURVIVED_CRASH");
            Debug.Log($"[AchievementSystem] Detected a crash event: {appliedEvent.title}. Checked 'SURVIVED_CRASH' achievement.");
        }
        // Add more event-based achievements here.
    }

    /// <summary>
    /// Handles the OnLoanTaken event from CreditSystem to track loans.
    /// </summary>
    /// <param name="loanAmount">The amount of the loan taken.</param>
    private void OnLoanTakenHandler(int loanAmount)
    {
        UnlockAchievement("FIRST_LOAN"); // Unlocked on the very first loan taken.
        Debug.Log($"[AchievementSystem] Detected a loan taken event. Checked 'FIRST_LOAN' achievement.");
    }

    // Optional: Method to reset achievements for testing or new game.
    public void ResetAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = false;
        }
        investmentsMade = 0;
        Debug.Log("[AchievementSystem] All achievements reset.");
    }
}