using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Singleton instance set.");
        }
        else
        {
            Debug.LogWarning("[GameManager] Duplicate found. Destroying.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gameStarted = true;
        Debug.Log("[GameManager] Game started!");
        TimeManager.Instance?.StartClock();
    }
}
 