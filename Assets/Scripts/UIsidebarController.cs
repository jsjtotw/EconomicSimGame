using UnityEngine;
using UnityEngine.UI;

public class UISidebarController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject stockViewUI;
    public GameObject financeDashboardPanel;
    public GameObject achievementPanelUI;
    public GameObject glossaryPanelUI; // NEW: Reference to the glossary panel

    [Header("Sidebar Buttons")]
    public Button stockViewButton;
    public Button financeDashboardButton;
    public Button achievementsButton;
    public Button glossaryButton; // NEW: Button for the glossary tab

    private void Start()
    {
        // Add button click listeners
        if (stockViewButton != null)
        {
            stockViewButton.onClick.AddListener(ShowStockView);
        }
        else
        {
            Debug.LogError("[UISidebarController] stockViewButton is not assigned!");
        }

        if (financeDashboardButton != null)
        {
            financeDashboardButton.onClick.AddListener(ShowFinanceDashboard);
        }
        else
        {
            Debug.LogError("[UISidebarController] financeDashboardButton is not assigned!");
        }

        if (achievementsButton != null)
        {
            achievementsButton.onClick.AddListener(ShowAchievementsPanel);
        }
        else
        {
            Debug.LogError("[UISidebarController] achievementsButton is not assigned!");
        }

        // NEW: Add listener for the glossary button
        if (glossaryButton != null)
        {
            glossaryButton.onClick.AddListener(ShowGlossaryPanel);
        }
        else
        {
            Debug.LogError("[UISidebarController] glossaryButton is not assigned!");
        }

        // Start by showing the stock view and hiding other panels
        ShowStockView();
    }

    /// <summary>
    /// Activates the Stock View UI and deactivates others.
    /// </summary>
    private void ShowStockView()
    {
        if (stockViewUI != null) stockViewUI.SetActive(true);
        if (financeDashboardPanel != null) financeDashboardPanel.SetActive(false);
        if (achievementPanelUI != null) achievementPanelUI.SetActive(false);
        if (glossaryPanelUI != null) glossaryPanelUI.SetActive(false); // NEW: Hide glossary panel
        Debug.Log("[UISidebarController] Showing Stock View.");
    }

    /// <summary>
    /// Activates the Finance Dashboard UI and deactivates others.
    /// </summary>
    private void ShowFinanceDashboard()
    {
        if (stockViewUI != null) stockViewUI.SetActive(false);
        if (financeDashboardPanel != null) financeDashboardPanel.SetActive(true);
        if (achievementPanelUI != null) achievementPanelUI.SetActive(false);
        if (glossaryPanelUI != null) glossaryPanelUI.SetActive(false); // NEW: Hide glossary panel
        Debug.Log("[UISidebarController] Showing Finance Dashboard.");
    }

    /// <summary>
    /// Activates the Achievements Panel UI and deactivates others.
    /// Also triggers the AchievementUI to populate its list.
    /// </summary>
    private void ShowAchievementsPanel()
    {
        if (stockViewUI != null) stockViewUI.SetActive(false);
        if (financeDashboardPanel != null) financeDashboardPanel.SetActive(false);
        if (achievementPanelUI != null) achievementPanelUI.SetActive(true);
        if (glossaryPanelUI != null) glossaryPanelUI.SetActive(false); // NEW: Hide glossary panel
        
        // Call the OpenPanel method on the AchievementUI script to refresh/display content
        if (AchievementUI.Instance != null)
        {
            AchievementUI.Instance.OpenPanel();
        }
        else
        {
            Debug.LogError("[UISidebarController] AchievementUI.Instance not found. Cannot open achievement panel content.");
        }
        Debug.Log("[UISidebarController] Showing Achievements Panel.");
    }

    /// <summary>
    /// NEW: Activates the Glossary Panel UI and deactivates others.
    /// Also triggers the GlossaryUI to populate its content.
    /// </summary>
    private void ShowGlossaryPanel()
    {
        if (stockViewUI != null) stockViewUI.SetActive(false);
        if (financeDashboardPanel != null) financeDashboardPanel.SetActive(false);
        if (achievementPanelUI != null) achievementPanelUI.SetActive(false);
        if (glossaryPanelUI != null) glossaryPanelUI.SetActive(true); // NEW: Show glossary panel

        // Call the OpenPanel method on the GlossaryUI script to refresh/display content
        if (GlossaryUI.Instance != null)
        {
            GlossaryUI.Instance.OpenPanel();
        }
        else
        {
            Debug.LogError("[UISidebarController] GlossaryUI.Instance not found. Cannot open glossary panel content.");
        }
        Debug.Log("[UISidebarController] Showing Glossary Panel.");
    }

    private void OnDestroy()
    {
        // Clean up listeners to prevent memory leaks
        if (stockViewButton != null) stockViewButton.onClick.RemoveAllListeners();
        if (financeDashboardButton != null) financeDashboardButton.onClick.RemoveAllListeners();
        if (achievementsButton != null) achievementsButton.onClick.RemoveAllListeners();
        if (glossaryButton != null) glossaryButton.onClick.RemoveAllListeners(); // NEW: Remove listener
    }
}