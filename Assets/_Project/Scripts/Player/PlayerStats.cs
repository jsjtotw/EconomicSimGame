// File: Assets/Scripts/Player/PlayerStats.cs
using UnityEngine;
using System.Collections.Generic;
using System; // For Action
using UnityEngine.Events; // Needed if using UnityEvent for onXPChanged

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public int xp = 0;
    public int level = 1;
    public float xpGainMultiplier = 1.0f; // Adjusted by company selection

    // Define XP required per level
    private Dictionary<int, int> xpThresholds = new Dictionary<int, int>()
    {
        {1, 100}, // Level 1 to 2 requires 100 XP
        {2, 250},
        {3, 500},
        {4, 1000},
        {5, 2000},
        {6, 4000},
        {7, 8000},
        {8, 16000},
        {9, 32000},
        {10, 64000}
    };
    // Event for level up (for AchievementSystem to subscribe to)
    public event Action<int> onLevelUp;
    // FIX: Add onXPChanged event for UI updates
    public UnityEvent<int, int, float> onXPChanged; // currentXP, maxXPForLevel, progress (0-1)

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

        // FIX: Initialize the UnityEvent
        if (onXPChanged == null)
        {
            onXPChanged = new UnityEvent<int, int, float>();
        }
    }

    void Start()
    {
        // Initial UI update for XP/Level
        // FIX: Call the event directly, DashboardUI will subscribe
        UpdateXPUI();
    }

    public void GainXP(int amount)
    {
        if (GameManager.Instance.gameEnded) return; // Don't gain XP after game ends

        xp += Mathf.RoundToInt(amount * xpGainMultiplier); // Apply multiplier
        Debug.Log($"Gained {Mathf.RoundToInt(amount * xpGainMultiplier)} XP. Total XP: {xp}");
        CheckForLevelUp();
        UpdateXPUI(); // Update XP bar and level text
    }

    void CheckForLevelUp()
    {
        int xpToNextLevel = GetXPRequiredForLevel(level);
        if (xpToNextLevel > 0 && xp >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        xp = xp - GetXPRequiredForLevel(level - 1); // Carry over excess XP
        Debug.Log($"Leveled up! New Level: {level}");
        onLevelUp?.Invoke(level); // Notify subscribers
        AchievementSystem.Instance?.CheckAchievement("LevelUp", level);
        // Implement basic perks here (e.g., faster stock analysis or unlock employees in Week 3)
        UpdateXPUI(); // Update UI after level up
    }

    // FIX: New private method to invoke the XP UI update event
    private void UpdateXPUI()
    {
        int xpForCurrentLevel = GetXPRequiredForLevel(level - 1); // XP required for previous level
        int xpForNextLevel = GetXPRequiredForLevel(level);

        float progress = 0f;
        if (xpForNextLevel > 0)
        {
            progress = (float)(xp - xpForCurrentLevel) / (xpForNextLevel - xpForCurrentLevel);
        }
        else
        {
            progress = 1f; // Max level reached, bar is full
        }
        onXPChanged?.Invoke(xp, xpForNextLevel, progress);
    }

    public int GetXPRequiredForLevel(int targetLevel)
    {
        if (xpThresholds.ContainsKey(targetLevel))
        {
            return xpThresholds[targetLevel];
        }
        return -1; // Indicates no more defined levels
    }
}