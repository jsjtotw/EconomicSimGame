// File: Assets/Scripts/Core/GameManager.cs
using UnityEngine;
using System; // For Action
using System.Collections.Generic; // For List

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // References to other core systems
    public BudgetSystem Budget { get; private set; }
    public PlayerStats Player { get; private set; }
    public StockMarketSystem StockMarket { get; private set; }
    public CreditSystem Credit { get; private set; }
    public EventSystem EventManager { get; private set; }
    public AchievementSystem AchievementManager { get; private set; }
    public StockTradeSystem StockTrader { get; private set; }

    // Game state variables
    public float winNetWorthTarget = 1000000f; // $1 Million
    public float bankruptcyThreshold = -10000f; // E.g., if net worth goes below -$10,000
    public bool gameEnded = false;
    // Company specific properties (can be moved to PlayerStats if preferred)
    public float playerEventSensitivity = 1.0f; // Default, adjusted by CompanySelector

    // Events for global state changes
    public static event Action OnGameStart;
    public static event Action OnGameWin;
    public static event Action OnGameLose;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // This destroys *duplicate* managers
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This makes the *first* manager persist
        }
    // Get references to all systems in the scene
    // FIX: Replaced FindObjectOfType with FindFirstObjectByType
    Budget = FindFirstObjectByType<BudgetSystem>();
        Player = FindFirstObjectByType<PlayerStats>();
        StockMarket = FindFirstObjectByType<StockMarketSystem>();
        Credit = FindFirstObjectByType<CreditSystem>();
        EventManager = FindFirstObjectByType<EventSystem>();
        AchievementManager = FindFirstObjectByType<AchievementSystem>();
        StockTrader = FindFirstObjectByType<StockTradeSystem>();
    }

    void Start()
    {
        // Subscribe to budget changes for win/lose conditions
        if (Budget != null)
        {
            Budget.onNetWorthChanged += CheckWinLoseConditions;
        }

        // Game starts via StartupManager (or CompanySelector will call StartGame)
    }

    void OnDestroy()
    {
        if (Budget != null)
        {
            Budget.onNetWorthChanged -= CheckWinLoseConditions;
        }
    }

    public void StartGame(CompanyProfile chosenCompany = null)
    {
        if (gameEnded) return;
        Debug.Log("Game Started!");
        Time.timeScale = 1; // Ensure time is running

        // Apply company bonuses if a company was chosen
        if (chosenCompany != null)
        {
            // Apply cash bonus
            Budget.cashOnHand += chosenCompany.startingCashBonus;
            Budget.UpdateNetWorth(); // Update net worth immediately after cash change

            // Apply stock value bonus (StockMarketSystem will handle applying to initial stocks)
            // PlayerStats should apply XP multiplier
            Player.xpGainMultiplier = chosenCompany.xpGainRateMultiplier;
            playerEventSensitivity = chosenCompany.eventSensitivityMultiplier;

            Debug.Log($"Chosen company: {chosenCompany.companyName}. Bonuses applied.");
        }

        // Initial setup for systems. They should ideally handle their own Start() logic.
        // If specific initialization order is needed, call public init methods here.
        OnGameStart?.Invoke(); // Notify other systems that the game has started
    }

    void CheckWinLoseConditions(float currentNetWorth)
    {
        if (gameEnded) return;
        if (currentNetWorth >= winNetWorthTarget)
        {
            WinGame();
        }
        else if (currentNetWorth <= bankruptcyThreshold)
        {
            LoseGame();
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("YOU WIN!");
        Time.timeScale = 0; // Pause game time
        WinLoseManager.Instance?.ShowWinScreen(); // Delegate to WinLoseManager
        OnGameWin?.Invoke();
    }

    public void LoseGame() // Public so CreditSystem can trigger if loans can't be repaid
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("YOU LOSE!");
        Time.timeScale = 0; // Pause game time
        WinLoseManager.Instance?.ShowLoseScreen(); // Delegate to WinLoseManager
        OnGameLose?.Invoke();
    }
}