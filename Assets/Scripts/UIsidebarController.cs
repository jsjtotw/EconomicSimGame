using UnityEngine;
using UnityEngine.UI;

public class UISidebarController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject stockViewUI;
    public GameObject financeDashboardPanel;

    [Header("Sidebar Buttons")]
    public Button stockViewButton;
    public Button financeDashboardButton;

    private void Start()
    {
        // Add button click listeners
        stockViewButton.onClick.AddListener(ShowStockView);
        financeDashboardButton.onClick.AddListener(ShowFinanceDashboard);

        // Start by showing the stock view and hiding finance dashboard
        ShowStockView();
    }

    private void ShowStockView()
    {
        stockViewUI.SetActive(true);
        financeDashboardPanel.SetActive(false);
    }

    private void ShowFinanceDashboard()
    {
        stockViewUI.SetActive(false);
        financeDashboardPanel.SetActive(true);
    }
}
