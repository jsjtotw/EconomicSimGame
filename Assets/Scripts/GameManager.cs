// GameManager.cs
// Manages the overall game state and applies company-specific perks.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance for GameManager.
    public static GameManager Instance;

    public bool gameStarted = false;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the Singleton pattern.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Keep this GameObject alive when loading new scenes.
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Singleton instance set.");
        }
        else
        {
            // If another instance already exists, destroy this one.
            Debug.LogWarning("[GameManager] Duplicate found. Destroying.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called before the first frame update.
    /// This is now the entry point for the GameScene logic.
    /// </summary>
    private void Start()
    {
        // Ensure a company has been selected before proceeding.
        if (PlayerCompany.Instance == null || !PlayerCompany.Instance.IsCompanyChosen)
        {
            Debug.LogError("[GameManager] No company selected! Returning to TitleScene...");
            // If no company is chosen, return to the title scene.
            SceneManager.LoadScene("TitleScene");
            return; // Stop further execution in this Start method.
        }

        // If a company is chosen, proceed to start the game and apply its perk.
        StartGame();
        ApplyCompanyPerk(PlayerCompany.Instance.SelectedType);

        // --- NEW: Give starting capital ---
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.Cash = 10000;
            Debug.Log("[GameManager] Player started with $10,000 cash.");
        }
        else
        {
            Debug.LogError("[GameManager] PlayerStats.Instance not found. Cannot set starting cash.");
        }
        // --- END NEW ---

        // Display the backstory popup
        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.ShowMessage(
                "A New Beginning",
                "Your grandfather has passed away, leaving you a final gift: a $10,000 inheritance. His dying wish was for you to take this seed money and grow it into a million-dollar empire. The market awaits, and his legacy rests in your hands. Good luck!"
            );
        }
        else
        {
            Debug.LogError("[GameManager] PopupManager.Instance not found. Cannot display backstory.");
        }
    }

    /// <summary>
    /// Initiates the game, setting the gameStarted flag and starting the TimeManager clock.
    /// </summary>
    public void StartGame()
    {
        gameStarted = true;
        Debug.Log("[GameManager] Game started!");
        // Safely attempt to start the clock if TimeManager instance exists.
        TimeManager.Instance?.StartClock();
    }

    /// <summary>
    /// Applies a specific perk based on the chosen company type.
    /// This method calls new methods on other system Singletons.
    /// </summary>
    /// <param name="type">The type of company selected by the player.</param>
    private void ApplyCompanyPerk(CompanyType type)
    {
        Debug.Log($"[GameManager] Applying perk for {type} company.");
        switch (type)
        {
            case CompanyType.TECH:
                // Apply a bonus to the StockMarketSystem for tech companies.
                StockMarketSystem.Instance?.SetTechBonus(0.05f); // +5% return (example)
                break;
            case CompanyType.FINANCE:
                // Apply an interest modifier to the CreditSystem for finance companies.
                CreditSystem.Instance?.SetInterestModifier(0.8f); // -20% interest (example)
                break;
            case CompanyType.RETAIL:
                // Apply a bonus income multiplier to the BudgetSystem for retail companies.
                BudgetSystem.Instance?.SetBonusIncomeMultiplier(1.10f); // +10% event bonus (example)
                break;
            default:
                Debug.LogWarning($"[GameManager] No specific perk defined for company type: {type}");
                break;
        }
    }
}