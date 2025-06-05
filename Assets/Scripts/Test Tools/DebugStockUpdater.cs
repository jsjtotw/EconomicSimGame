using UnityEngine;
using UnityEngine.InputSystem; // 👈 NEW Input System

public class DebugStockUpdater : MonoBehaviour
{
    public StockMarketSystem stockMarket;

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("[DebugStockUpdater] 'R' key pressed — refreshing stock prices.");
            StockMarketSystem.Instance.UpdateStockPrice("TCH", Random.Range(100f, 150f));
        }
    }
}
