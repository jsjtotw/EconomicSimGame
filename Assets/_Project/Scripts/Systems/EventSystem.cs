// File: Assets/Scripts/Systems/EventSystem.cs
using UnityEngine;
using System.Collections.Generic;
using System; // For Action

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    public List<GameEvent> availableEvents;
    public float eventIntervalInDays = 7f; // Trigger an event every 7 in-game days
    private float daysSinceLastEvent = 0f;

    // Event to notify other systems when an event triggers
    public static event Action<GameEvent> onEventTriggered;

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
        // Load events from DataManager
        if (DataManager.Instance != null)
        {
            availableEvents = new List<GameEvent>(DataManager.Instance.Events);
        }
        else
        {
            Debug.LogError("DataManager not found. EventSystem cannot load events.");
            availableEvents = new List<GameEvent>();
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced += CheckForEventTrigger;
        }
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.onHourAdvanced -= CheckForEventTrigger;
        }
    }

    void CheckForEventTrigger()
    {
        if (TimeManager.Instance.hour == 0) // Only check once per day (at hour 0)
        {
            daysSinceLastEvent++;
            if (daysSinceLastEvent >= eventIntervalInDays)
            {
                TriggerRandomEvent();
                daysSinceLastEvent = 0f;
            }
        }
    }

    public void TriggerRandomEvent()
    {
        if (availableEvents.Count == 0)
        {
            Debug.LogWarning("No events to trigger.");
            return;
        }

        GameEvent randomEvent = availableEvents[UnityEngine.Random.Range(0, availableEvents.Count)];
        Debug.Log($"Event Triggered: {randomEvent.title} - {randomEvent.description}");

        // Broadcast event
        onEventTriggered?.Invoke(randomEvent);
        PopupManager.Instance?.ShowEventPopup(randomEvent); // Show UI popup

        GameManager.Instance.Player.GainXP(50); // Player gains XP for an event occurring
        AchievementSystem.Instance?.CheckAchievement("EventSurvived", randomEvent.id); // Check achievement
    }
}

// GameEvent data class (can be in its own file or within EventSystem.cs)
[System.Serializable]
public class GameEvent
{
    public string id;
    public string title;
    public string description;
    public string target; // e.g., "AI", "all", "Food Chain", "Green Energy"
    public string effect; // "increase", "decrease", "volatility"
    public float magnitude;
    public string achievementTrigger; // Optional: ID of achievement triggered by this event
}