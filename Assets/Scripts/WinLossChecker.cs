// WinLossChecker.cs
// Defines and checks the win/loss conditions for the game.

using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes.

public class WinLossChecker : MonoBehaviour
{
    // Singleton instance for WinLossChecker.
    public static WinLossChecker Instance;

    // Flag to prevent multiple win/loss triggers.
    private bool gameEnded = false;

    // --- Win/Loss Thresholds (Adjustable in Inspector) ---
    public int winNetWorthTarget = 1000000; // Target net worth for victory ($1,000,000)
    // Loss condition: Debt exceeds current Cash.
    // If you want Debt > NetWorth (Debt > (Cash - Debt)), the logic would be:
    // (PlayerStats.Instance.Debt > (PlayerStats.Instance.Cash - PlayerStats.Instance.Debt))
    // which simplifies to (2 * PlayerStats.Instance.Debt > PlayerStats.Instance.Cash).
    // For now, let's keep it simple: Debt > Cash.
    public int lossDebtVsCashThreshold = 0; // If PlayerStats.Debt > PlayerStats.Cash, it's a loss.

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
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    /// <summary>
    /// Called when the script starts.
    /// Subscribes to TimeManager's OnWeekAdvanced event to check conditions regularly.
    /// </summary>
    private void Start()
    {
        // Subscribe to TimeManager's OnWeekAdvanced event.
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnWeekAdvanced += CheckWinLossConditions;
            Debug.Log("[WinLossChecker] Subscribed to TimeManager.OnWeekAdvanced.");
        }
        else
        {
            Debug.LogError("[WinLossChecker] TimeManager instance not found. Win/Loss conditions will not be checked automatically.");
        }
    }

    /// <summary>
    /// Unsubscribes from events to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnWeekAdvanced -= CheckWinLossConditions;
        }
    }

    /// <summary>
    /// Checks the win and loss conditions.
    /// If a condition is met, it triggers the end game sequence.
    /// </summary>
    private void CheckWinLossConditions()
    {
        if (gameEnded) return; // Prevent checks if the game has already ended.

        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("[WinLossChecker] PlayerStats instance not found. Cannot check win/loss conditions.");
            return;
        }

        // --- Win Condition ---
        if (PlayerStats.Instance.NetWorth >= winNetWorthTarget)
        {
            TriggerWin();
            return; // Game has ended, stop further checks.
        }

        // --- Loss Condition (Debt > Cash) ---
        // Player's current debt (liability) exceeds their liquid cash (asset).
        // This means they can't cover their immediate financial obligations.
        if (PlayerStats.Instance.Debt > PlayerStats.Instance.Cash)
        {
            TriggerLoss("You ran out of liquid cash to cover your debts!");
            return; // Game has ended, stop further checks.
        }
        
        Debug.Log($"[WinLossChecker] Checked conditions: NetWorth ${PlayerStats.Instance.NetWorth}, Cash ${PlayerStats.Instance.Cash}, Debt ${PlayerStats.Instance.Debt}. No win/loss yet.");
    }

    /// <summary>
    /// Triggers the game victory sequence.
    /// </summary>
    private void TriggerWin()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("[WinLossChecker] Game WON!");
        
        // Pause game time.
        // TimeManager.Instance?.PauseGame(); // You might need a Pause method in TimeManager.

        PopupManager.Instance.ShowMessage(
            "VICTORY!",
            $"Congratulations! You've reached a net worth of ${winNetWorthTarget:N0} and built a financial empire!",
            ReturnToMainMenu
        );
    }

    /// <summary>
    /// Triggers the game loss sequence.
    /// </summary>
    /// <param name="reason">The reason for the loss.</param>
    private void TriggerLoss(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log($"[WinLossChecker] Game LOST! Reason: {reason}");

        // Pause game time.
        // TimeManager.Instance?.PauseGame(); // You might need a Pause method in TimeManager.

        PopupManager.Instance.ShowMessage(
            "GAME OVER",
            $"Your financial empire has crumbled. {reason}",
            ReturnToMainMenu
        );
    }

    /// <summary>
    /// Returns the player to the main menu scene after win/loss.
    /// </summary>
    private void ReturnToMainMenu()
    {
        Debug.Log("[WinLossChecker] Returning to TitleScene...");
        SceneManager.LoadScene("TitleScene"); // Make sure "TitleScene" is the correct name of your main menu scene.
        // Reset the gameEnded flag for a new play-through.
        gameEnded = false; 
    }
}
