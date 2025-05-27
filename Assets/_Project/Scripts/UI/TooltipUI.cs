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
        transform.position = mousePos;

        // Optional: Adjust position to prevent going off-screen
        Vector2 pivot = Vector2.zero; // Default bottom-left
        if (mousePos.x + backgroundRect.sizeDelta.x > Screen.width)
        {
            pivot.x = 1; // Move pivot to right (anchor to right edge of mouse)
        }
        if (mousePos.y + backgroundRect.sizeDelta.y > Screen.height)
        {
            pivot.y = 1; // Move pivot to top (anchor to top edge of mouse)
        }
        backgroundRect.pivot = pivot;
    }
}