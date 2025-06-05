using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    private int cash;
    private int netWorth;
    private int debt;
    private int xp;
    public int MissedPayments = 0;
    public int OnTimePayments = 0;

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
            UpdateNetWorth();
        }
    }

    public int NetWorth
    {
        get => netWorth;
        private set
        {
            netWorth = value;
            Debug.Log($"[PlayerStats] Net Worth updated to {netWorth}");
        }
    }

    public int Debt
    {
        get => debt;
        set
        {
            debt = value;
            Debug.Log($"[PlayerStats] Debt set to {debt}");
            UpdateNetWorth();
        }
    }

    public int XP
    {
        get => xp;
        set
        {
            xp = value;
            Debug.Log($"[PlayerStats] XP set to {xp}");
        }
    }

    // Simple net worth calculation: cash - debt (you can expand later)
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
