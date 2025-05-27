// File: Assets/Scripts/UI/TooltipTrigger.cs
using UnityEngine;
using UnityEngine.EventSystems; // For IPointerEnterHandler, IPointerExitHandler

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] // Makes the string field multi-line in Inspector
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.ShowTooltip(tooltipMessage, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.HideTooltip();
        }
    }
}