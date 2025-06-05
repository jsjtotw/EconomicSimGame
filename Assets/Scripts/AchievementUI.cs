// AchievementUI.cs
// Manages the display of achievements in a dedicated UI tab.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    // Singleton instance for AchievementUI (if you only want one instance).
    public static AchievementUI Instance;

    [Header("UI Elements (Assign in Inspector)")]
    public GameObject achievementPanel;       // The parent GameObject for the entire achievement tab
    public Transform achievementListContainer; // The parent transform where individual achievement entries will be instantiated
    public GameObject achievementEntryPrefab; // Prefab for a single achievement item in the list
    public Button closeButton;                // Button to close the achievement panel

    [Header("nlocked/Locked Visuals")]
    public Color unlockedColor = Color.green;
    public Color lockedColor = Color.gray;
    // You could also add Sprite references for icons instead of just colors

    private List<GameObject> instantiatedEntries = new List<GameObject>();

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Consider if this panel should persist or be per-scene.
                                           // For a pop-up tab, usually it's part of the persistent UI.
                                           // If it's part of the GameScene UI, no DontDestroyOnLoad needed.
        }
        else
        {
            Destroy(gameObject);
        }

        // Initially hide the achievement panel.
        if (achievementPanel != null)
        {
            achievementPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("[AchievementUI] 'achievementPanel' GameObject is not assigned in the Inspector!");
        }

        // Add listener for the close button.
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogError("[AchievementUI] 'closeButton' is not assigned in the Inspector!");
        }
    }

    /// <summary>
    /// Called when the script starts.
    /// Subscribes to the AchievementSystem's unlock event.
    /// </summary>
    private void Start()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUnlocked += OnAchievementUnlockedHandler;
            Debug.Log("[AchievementUI] Subscribed to AchievementSystem.OnAchievementUnlocked.");
        }
        else
        {
            Debug.LogError("[AchievementUI] AchievementSystem.Instance not found. Achievement UI will not update dynamically.");
        }
    }

    /// <summary>
    /// Unsubscribes from events to prevent memory leaks when the GameObject is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (AchievementSystem.Instance != null)
        {
            AchievementSystem.Instance.OnAchievementUnlocked -= OnAchievementUnlockedHandler;
            Debug.Log("[AchievementUI] Unsubscribed from AchievementSystem.OnAchievementUnlocked.");
        }
    }

    /// <summary>
    /// Opens the achievement panel and populates the list.
    /// </summary>
    public void OpenPanel()
    {
        if (achievementPanel != null)
        {
            achievementPanel.SetActive(true);
            PopulateAchievementList(); // Refresh the list every time the panel is opened.
            Debug.Log("[AchievementUI] Achievement panel opened.");
        }
    }

    /// <summary>
    /// Closes the achievement panel.
    /// </summary>
    public void ClosePanel()
    {
        if (achievementPanel != null)
        {
            achievementPanel.SetActive(false);
            Debug.Log("[AchievementUI] Achievement panel closed.");
        }
    }

    /// <summary>
    /// Populates the achievement list by instantiating prefabs for each achievement.
    /// </summary>
    private void PopulateAchievementList()
    {
        // Clear previous entries to avoid duplicates on refresh.
        foreach (GameObject entry in instantiatedEntries)
        {
            Destroy(entry);
        }
        instantiatedEntries.Clear();

        if (achievementListContainer == null || achievementEntryPrefab == null)
        {
            Debug.LogError("[AchievementUI] Missing UI references for achievement list. Cannot populate.");
            return;
        }
        if (AchievementSystem.Instance == null)
        {
            Debug.LogError("[AchievementUI] AchievementSystem.Instance not found. Cannot populate achievement list.");
            return;
        }

        // Sort achievements by unlocked status, then by ID for consistency
        List<Achievement> sortedAchievements = new List<Achievement>(AchievementSystem.Instance.achievements);
        sortedAchievements.Sort((a1, a2) => {
            if (a1.isUnlocked && !a2.isUnlocked) return -1; // Unlocked comes first
            if (!a1.isUnlocked && a2.isUnlocked) return 1;  // Locked comes after
            return string.Compare(a1.id, a2.id);             // Then sort by ID
        });


        foreach (Achievement achievement in sortedAchievements)
        {
            GameObject entryGO = Instantiate(achievementEntryPrefab, achievementListContainer);
            instantiatedEntries.Add(entryGO);

            // Assuming achievementEntryPrefab has TextMeshProUGUI components for name and description
            TextMeshProUGUI nameText = entryGO.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = entryGO.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI statusText = entryGO.transform.Find("StatusText")?.GetComponent<TextMeshProUGUI>(); // Optional: for "Unlocked" / "Locked" text
            Image statusIcon = entryGO.transform.Find("StatusIcon")?.GetComponent<Image>(); // Optional: for a checkmark/lock icon

            if (nameText != null) nameText.text = achievement.name;
            if (descriptionText != null) descriptionText.text = achievement.description;

            // Update visual based on unlocked status
            if (statusText != null)
            {
                statusText.text = achievement.isUnlocked ? "UNLOCKED" : "LOCKED";
                statusText.color = achievement.isUnlocked ? unlockedColor : lockedColor;
            }
            if (statusIcon != null)
            {
                statusIcon.color = achievement.isUnlocked ? unlockedColor : lockedColor; // Use color for simple icon.
                // You could also assign different sprites here:
                // statusIcon.sprite = achievement.isUnlocked ? unlockedSprite : lockedSprite;
            }
        }
        Debug.Log($"[AchievementUI] Achievement list populated with {instantiatedEntries.Count} entries.");
    }

    /// <summary>
    /// Event handler for when an achievement is unlocked.
    /// Refreshes the achievement list to reflect the new status.
    /// </summary>
    /// <param name="achievement">The achievement that was just unlocked.</param>
    private void OnAchievementUnlockedHandler(Achievement achievement)
    {
        Debug.Log($"[AchievementUI] Received OnAchievementUnlocked for: {achievement.name}. Refreshing UI.");
        // If the panel is open, refresh it immediately.
        if (achievementPanel != null && achievementPanel.activeSelf)
        {
            PopulateAchievementList();
        }
        // Even if not open, the next time it opens, it will refresh.
    }
}
