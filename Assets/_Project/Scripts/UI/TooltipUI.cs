// File: Assets/Scripts/UI/TooltipUI.cs
using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    public TextMeshProUGUI tooltipText;
    public RectTransform backgroundRect;

    [SerializeField] private float textPaddingX = 10f;
    [SerializeField] private float textPaddingY = 10f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        HideTooltip(); // Start hidden
    }

    public void ShowTooltip(string text, Vector2 mousePos)
    {
        tooltipText.text = text;
        gameObject.SetActive(true);
        AdjustBackgroundSize();
        PositionTooltip(mousePos);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void AdjustBackgroundSize()
    {
        // Force text mesh to update so preferred sizes are accurate
        tooltipText.ForceMeshUpdate();

        Vector2 textSize = tooltipText.GetRenderedValues(false);
        Vector2 padding = new Vector2(textPaddingX, textPaddingY);
        backgroundRect.sizeDelta = textSize + padding;
    }

    private void PositionTooltip(Vector2 mousePos)
    {
        Vector2 pivot = new Vector2(0, 1); // Always anchor top-left of tooltip
        backgroundRect.pivot = pivot;

        Vector2 offset = new Vector2(10f, -10f); // Move slightly right and downward
        Vector2 anchoredPosition = mousePos + offset;

        // Optional: Clamp to stay within screen bounds
        float tooltipWidth = backgroundRect.sizeDelta.x;
        float tooltipHeight = backgroundRect.sizeDelta.y;

        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0, Screen.width - tooltipWidth);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, tooltipHeight, Screen.height);

        transform.position = anchoredPosition;
    }
}