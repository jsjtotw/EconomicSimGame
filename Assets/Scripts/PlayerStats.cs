// PlayerStats.cs
// Tracks core player statistics like cash, net worth, and debt.

using UnityEngine;
using System; // Required for Action delegate

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    private int cash;
    private int netWorth;
    private int debt;
    // private int xp; // XP is now managed by PlayerXP.cs

    public int MissedPayments = 0;
    public int OnTimePayments = 0;

    // Track previous net worth to determine if XP should be granted.
    private int previousNetWorth = 0;

    // --- NEW: Event for Net Worth changes ---
    public event Action<int> OnNetWorthChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[PlayerStats] Instance created.");
        }
        else
        {
            Debug.Log("[PlayerStats] Duplicate detected. Destroying extra instance.");
            Destroy(gameObject);
        }
    }

    public int Cash
    {
        get => cash;
        set
        {
            cash = value;
            Debug.Log($"[PlayerStats] Cash set to {cash}");
            UpdateNetWorth(); // Recalculate net worth when cash changes.
        }
    }

    public int NetWorth
    {
        get => netWorth;
        private set
        {
            netWorth = value;
            Debug.Log($"[PlayerStats] Net Worth updated to {netWorth}");
            
            // Grant XP based on Net Worth increase (if XP System is active)
            if (PlayerXP.Instance != null && netWorth > previousNetWorth)
            {
                int netWorthIncrease = netWorth - previousNetWorth;
                int xpGained = Mathf.RoundToInt(netWorthIncrease * 0.1f); // Example: 10% of net worth increase as XP

                if (xpGained > 0)
                {
                    PlayerXP.Instance.AddXP(xpGained);
                    Debug.Log($"[PlayerStats] Gained {xpGained} XP from net worth increase.");
                }
            }
            previousNetWorth = netWorth; // Update previous net worth for next calculation.

            // --- NEW: Invoke the OnNetWorthChanged event ---
            OnNetWorthChanged?.Invoke(netWorth);
        }
    }

    public int Debt
    {
        get => debt;
        set
        {
            debt = value;
            Debug.Log($"[PlayerStats] Debt set to {debt}");
            UpdateNetWorth(); // Recalculate net worth when debt changes.
        }
    }

    // The 'XP' property is now removed from PlayerStats as it's managed by PlayerXP.cs
    // public int XP
    // {
    //     get => xp;
    //     set
    //     {
    //         xp = value;
    //         Debug.Log($"[PlayerStats] XP set to {xp}");
    //     }
    // }

    // Simple net worth calculation: cash - debt
    private void UpdateNetWorth()
    {
        NetWorth = Cash - Debt;
    }

    // For testing purposes, increase/decrease cash by amount
    public void ChangeCash(int amount)
    {
        Cash += amount;
        Debug.Log($"[PlayerStats] Cash changed by {amount} to {Cash}");
    }
}
