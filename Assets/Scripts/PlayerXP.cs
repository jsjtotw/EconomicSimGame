// PlayerXP.cs
// Manages the player's experience points (XP) and level progression.

using System;
using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    // Singleton instance for PlayerXP.
    public static PlayerXP Instance;

    // Current XP of the player.
    [SerializeField] private int currentXP = 0;
    // Current level of the player.
    [SerializeField] private int currentLevel = 1;

    // Event triggered when the player levels up.
    // Subscribers can listen to this to apply perks, update UI, etc.
    public event Action<int> OnLevelUp;

    // --- XP & Leveling Configuration ---
    // Base XP needed for Level 2 (Level 1 requires 0 XP).
    public int baseXPForLevel2 = 100;
    // Multiplier for XP needed per subsequent level.
    // For example, if 1.5, Level 3 needs 100 * 1.5 = 150 more XP than Level 2.
    public float xpCurveMultiplier = 1.2f; // Each level requires 20% more XP than the previous one

    // --- Employee Slots (Example Perk) ---
    public int baseEmployeeSlots = 3;
    public int employeeSlotsPerLevel = 1;

    /// <summary>
    /// Property to access and set current XP.
    /// Any time XP is set, it checks for level ups.
    /// </summary>
    public int CurrentXP
    {
        get => currentXP;
        private set
        {
            currentXP = value;
            Debug.Log($"[PlayerXP] XP changed to {currentXP}");
            CheckForLevelUp(); // Always check for level up when XP changes.
        }
    }

    /// <summary>
    /// Property to get the current level.
    /// </summary>
    public int CurrentLevel
    {
        get => currentLevel;
        private set
        {
            if (currentLevel != value) // Only update if level actually changed
            {
                int oldLevel = currentLevel;
                currentLevel = value;
                Debug.Log($"[PlayerXP] Player leveled up: Level {oldLevel} -> Level {currentLevel}");
                OnLevelUp?.Invoke(currentLevel); // Trigger level up event.
            }
        }
    }

    /// <summary>
    /// Calculates the total XP required to reach a specific level.
    /// Uses an exponential curve.
    /// </summary>
    /// <param name="level">The target level.</param>
    /// <returns>Total XP needed for that level.</returns>
    public int GetXPForLevel(int level)
    {
        if (level <= 1) return 0;
        // For Level 2, it's baseXPForLevel2.
        // For Level 3, it's baseXPForLevel2 + (baseXPForLevel2 * xpCurveMultiplier)
        // For Level N, it's baseXPForLevel2 * (1 + xpCurveMultiplier + xpCurveMultiplier^2 + ... + xpCurveMultiplier^(N-2))
        // This is a geometric series sum: a * (r^n - 1) / (r - 1)
        // Where a = baseXPForLevel2, r = xpCurveMultiplier, n = level - 1
        float totalXP = baseXPForLevel2;
        for (int i = 2; i < level; i++)
        {
            totalXP += baseXPForLevel2 * Mathf.Pow(xpCurveMultiplier, i - 1);
        }
        return Mathf.RoundToInt(totalXP);
    }

    /// <summary>
    /// Calculates the XP required for the *next* level.
    /// </summary>
    /// <returns>XP needed for the next level.</returns>
    public int GetXPForNextLevel()
    {
        return GetXPForLevel(CurrentLevel + 1);
    }

    /// <summary>
    /// Gets the player's current available employee slots.
    /// This is an example of a dynamic perk.
    /// </summary>
    public int TotalEmployeeSlots
    {
        get { return baseEmployeeSlots + (CurrentLevel - 1) * employeeSlotsPerLevel; }
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[PlayerXP] Instance created.");
        }
        else
        {
            Debug.LogWarning("[PlayerXP] Duplicate detected. Destroying extra instance.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the script starts.
    /// Initial check for level up in case XP is loaded from save.
    /// </summary>
    private void Start()
    {
        // Initial check if CurrentXP was set before Start (e.g., from a save game).
        CheckForLevelUp();
        Debug.Log($"[PlayerXP] Player starts at Level {CurrentLevel} with {CurrentXP} XP. Next level at {GetXPForNextLevel()} XP.");
    }

    /// <summary>
    /// Adds XP to the player's current total and checks for level ups.
    /// </summary>
    /// <param name="amount">The amount of XP to add.</param>
    public void AddXP(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("[PlayerXP] Attempted to add negative XP. Use RemoveXP if needed.");
            return;
        }
        CurrentXP += amount;
        Debug.Log($"[PlayerXP] Added {amount} XP. Current XP: {CurrentXP}");
    }

    /// <summary>
    /// Checks if the player has gained enough XP to level up.
    /// Continues leveling up until XP is below the next level's requirement.
    /// </summary>
    private void CheckForLevelUp()
    {
        // Loop to handle multiple level-ups at once (e.g., from a large XP gain).
        while (currentXP >= GetXPForNextLevel())
        {
            CurrentLevel++; // Increment the level.
            Debug.Log($"[PlayerXP] Player reached Level {CurrentLevel}!");
            // No need to remove XP, GetXPForLevel calculates total XP needed for *next* level.
            // Player's XP effectively 'carries over' towards the next milestone.
        }
    }
}
