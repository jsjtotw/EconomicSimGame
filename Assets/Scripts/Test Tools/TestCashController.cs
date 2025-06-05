using UnityEngine;

public class TestCashController : MonoBehaviour
{
    public int cashChangeAmount = 100;

    public void IncreaseCash()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ChangeCash(cashChangeAmount);
            Debug.Log("[TestCashController] Increased cash.");
        }
    }

    public void DecreaseCash()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ChangeCash(-cashChangeAmount);
            Debug.Log("[TestCashController] Decreased cash.");
        }
    }
}
