// CompanySelectionPanel.cs
// Manages the UI for selecting a company type.

using UnityEngine;
using UnityEngine.UI;

public class CompanySelectionPanel : MonoBehaviour
{
    // Reference to the main selection panel GameObject.
    // THIS MUST BE ASSIGNED IN THE INSPECTOR TO THE UI PANEL ITSELF.
    public GameObject selectionPanel;
    // Buttons for selecting different company types.
    public Button techButton;
    public Button financeButton;
    public Button retailButton; // Changed from agriButton to retailButton as per instructions.
    // Button to confirm the selection.
    public Button confirmButton;

    // Stores the currently selected company type.
    private CompanyType selected;

    /// <summary>
    /// Called before the first frame update.
    /// Initializes UI state and adds button listeners.
    /// </summary>
    private void Start()
    {
        // Hide the panel and disable the confirm button initially.
        // This line relies on 'selectionPanel' being correctly assigned.
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("[CompanySelectionPanel] 'selectionPanel' GameObject reference is missing in the Inspector!");
        }

        if (confirmButton != null)
        {
            confirmButton.interactable = false;
        }
        else
        {
             Debug.LogError("[CompanySelectionPanel] 'confirmButton' Button reference is missing in the Inspector!");
        }
       

        // Add listeners to company type selection buttons.
        techButton.onClick.AddListener(() => SelectCompany(CompanyType.TECH));
        financeButton.onClick.AddListener(() => SelectCompany(CompanyType.FINANCE));
        retailButton.onClick.AddListener(() => SelectCompany(CompanyType.RETAIL));
        // Add listener to the confirm button.
        confirmButton.onClick.AddListener(ConfirmSelection);

        Debug.Log("[CompanySelectionPanel] Initialized.");
    }

    /// <summary>
    /// Activates and displays the company selection panel.
    /// </summary>
    public void OpenPanel()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
            Debug.Log("[CompanySelectionPanel] Panel opened.");
        }
        else
        {
            Debug.LogError("[CompanySelectionPanel] Cannot open panel: 'selectionPanel' GameObject reference is missing!");
        }
        
        // Reset confirm button state in case it was previously interactable.
        if (confirmButton != null)
        {
            confirmButton.interactable = false;
        }
    }

    /// <summary>
    /// Sets the currently selected company type and enables the confirm button.
    /// </summary>
    /// <param name="type">The CompanyType chosen by the player.</param>
    private void SelectCompany(CompanyType type)
    {
        selected = type;
        if (confirmButton != null)
        {
            confirmButton.interactable = true; // Enable confirm button once a selection is made.
        }
        Debug.Log($"[CompanySelectionPanel] Selected: {type}");
    }

    /// <summary>
    /// Confirms the selected company, sets it in PlayerCompany, and starts the game.
    /// </summary>
    private void ConfirmSelection()
    {
        // Set the chosen company type in the PlayerCompany singleton.
        if (PlayerCompany.Instance != null)
        {
            PlayerCompany.Instance.SetCompany(selected);
        }
        else
        {
            Debug.LogError("[CompanySelectionPanel] PlayerCompany instance not found!");
            return; // Prevent further execution if critical instance is missing.
        }

        // Hide the selection panel. THIS IS THE LINE IN QUESTION.
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
            Debug.Log("[CompanySelectionPanel] Company selection panel disabled.");
        }
        else
        {
            Debug.LogError("[CompanySelectionPanel] Cannot disable panel: 'selectionPanel' GameObject reference is missing!");
        }
       
        // Find the StartupManager and call its *internal* StartGame method to load the next scene.
        StartupManager startupManager = FindObjectOfType<StartupManager>();
        if (startupManager != null)
        {
            Debug.Log("[CompanySelectionPanel] Confirming selection and starting game via StartupManager.");
            startupManager.StartGameInternal();
        }
        else
        {
            Debug.LogError("[CompanySelectionPanel] StartupManager not found in scene! Cannot load next scene.");
        }
    }
}
