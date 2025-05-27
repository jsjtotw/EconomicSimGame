// File: Assets/Scripts/Systems/AchievementSystem.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Achievement
{
    public string id;
    public string name;
    public string description;
    public bool unlocked;
}

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance { get; private set; }

    public List<Achievement> achievements = new List<Achievement>();

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
        InitializeAchievements();
        // Subscribe to relevant events from other systems
        GameManager.Instance.Budget.onNetWorthChanged += (netWorth) => CheckAchievement("NetWorth", netWorth);
        GameManager.Instance.Player.onLevelUp += (level) => CheckAchievement("LevelUp", level);
        // EventSystem.onEventTriggered += (gameEvent) => CheckAchievement("EventSurvived", gameEvent.id); // Already handled in EventSystem, but good to know
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.Budget != null)
        {
            GameManager.Instance.Budget.onNetWorthChanged -= (netWorth) => CheckAchievement("NetWorth", netWorth);
        }
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.onLevelUp -= (level) => CheckAchievement("LevelUp", level);
        }
    }

    void InitializeAchievements()
    {
        achievements.Add(new Achievement { id = "profit10K", name = "First $10K Profit", description = "Reach a net worth of $10,000.", unlocked = false });
        achievements.Add(new Achievement { id = "profit50K", name = "Mid-Tier Mogul", description = "Reach a net worth of $50,000.", unlocked = false });
        achievements.Add(new Achievement { id = "profit100K", name = "Six-Figure Success", description = "Reach a net worth of $100,000.", unlocked = false });
        achievements.Add(new Achievement { id = "survivedRecession", name = "Economic Resilience", description = "Survive a major economic recession event.", unlocked = false });
        achievements.Add(new Achievement { id = "level5", name = "Adept Trader", description = "Reach Player Level 5.", unlocked = false });
        achievements.Add(new Achievement { id = "loanRepaid", name = "Debt Free!", description = "Repay your first loan.", unlocked = false });
    }

    public void UnlockAchievement(string achievementId)
    {
        Achievement achievement = achievements.Find(a => a.id == achievementId);
        if (achievement != null && !achievement.unlocked)
        {
            achievement.unlocked = true;
            Debug.Log($"Achievement Unlocked: {achievement.name}");
            PopupManager.Instance?.ShowAchievementToast(achievement.name);
            GameManager.Instance.Player.GainXP(100); // Reward XP for achievement
            // Play sound, show badge, etc.
        }
    }

    // Generic check method
    public void CheckAchievement(string type, object value)
    {
        if (type == "NetWorth" && value is float netWorth)
        {
            if (netWorth >= 10000 && !achievements.Find(a => a.id == "profit10K").unlocked)
            {
                UnlockAchievement("profit10K");
            }
            if (netWorth >= 50000 && !achievements.Find(a => a.id == "profit50K").unlocked)
            {
                UnlockAchievement("profit50K");
            }
            if (netWorth >= 100000 && !achievements.Find(a => a.id == "profit100K").unlocked)
            {
                UnlockAchievement("profit100K");
            }
        }
        else if (type == "LevelUp" && value is int level)
        {
            if (level >= 5 && !achievements.Find(a => a.id == "level5").unlocked)
            {
                UnlockAchievement("level5");
            }
        }
        else if (type == "EventSurvived" && value is string eventId)
        {
            // You can add more specific checks here based on eventId
            // For now, let's just use 'survivedRecession' as a general example
            if (eventId == "event001" && !achievements.Find(a => a.id == "survivedRecession").unlocked)
            {
                UnlockAchievement("survivedRecession");
            }
        }
        else if (type == "LoanRepaid" && value is bool repaid)
        {
            if (repaid && !achievements.Find(a => a.id == "loanRepaid").unlocked)
            {
                UnlockAchievement("loanRepaid");
            }
        }
    }
}