// File: Assets/Scripts/UI/PopupManager.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("Event Popup")]
    public GameObject eventPopupPanel;
    public TextMeshProUGUI eventPopupTitle;
    public TextMeshProUGUI eventPopupDescription;

    [Header("Achievement Toast")]
    public GameObject achievementToastPanel;
    public TextMeshProUGUI achievementToastText;

    [Header("Event Log")]
    public GameObject eventLogPanel;
    public TextMeshProUGUI eventLogText; // Use a TextMeshProUGUI for the log content

    private Queue<string> eventLog = new Queue<string>();
    private int maxLogEntries = 10;

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
        eventPopupPanel?.SetActive(false);
        achievementToastPanel?.SetActive(false);
        eventLogPanel?.SetActive(false);
    }

    public void ShowEventPopup(GameEvent gameEvent)
    {
        if (eventPopupPanel != null)
        {
            eventPopupPanel.SetActive(true);
            if (eventPopupTitle != null) eventPopupTitle.text = gameEvent.title;
            if (eventPopupDescription != null) eventPopupDescription.text = gameEvent.description;
            AddEventToLog(gameEvent);
            Invoke("HideEventPopup", 5f);
        }
    }

    void HideEventPopup()
    {
        eventPopupPanel?.SetActive(false);
    }

    void AddEventToLog(GameEvent gameEvent)
    {
        string logEntry = $"[{TimeManager.Instance.year}/{TimeManager.Instance.month}/{TimeManager.Instance.day}] {gameEvent.title}: {gameEvent.description}";
        eventLog.Enqueue(logEntry);
        if (eventLog.Count > maxLogEntries)
        {
            eventLog.Dequeue();
        }
        UpdateEventLogUI();
    }

    void UpdateEventLogUI()
    {
        if (eventLogText != null)
        {
            eventLogText.text = string.Join("\n", eventLog.ToArray());
        }
    }

    public void ToggleEventLogPanel()
    {
        if (eventLogPanel != null)
        {
            eventLogPanel.SetActive(!eventLogPanel.activeSelf);
        }
    }

    public void ShowAchievementToast(string achievementName)
    {
        if (achievementToastPanel != null)
        {
            achievementToastPanel.SetActive(true);
            if (achievementToastText != null) achievementToastText.text = $"Achievement Unlocked: {achievementName}!";
            Invoke("HideAchievementToast", 3f);
        }
    }

    void HideAchievementToast()
    {
        achievementToastPanel?.SetActive(false);
    }
}