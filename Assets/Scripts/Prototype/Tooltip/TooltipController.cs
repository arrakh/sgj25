using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Tooltip
{
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private TooltipWindow tooltipWindow;
        
        private ITooltipElement currentElement;

        private EventSystem eventSystem;

        [SerializeField] private bool tooltipEnabled = false;

        private void Awake()
        {
            eventSystem = EventSystem.current;
        }

        private void Update()
        {
            // Check if the mouse is over a UI element
            if (eventSystem == null) eventSystem = EventSystem.current;
            if (eventSystem.IsPointerOverGameObject())
            {
                // Raycast to find UI elements
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;

                List<RaycastResult> results = new(); 
                eventSystem.RaycastAll(pointerEventData, results);

                currentElement = null;

                foreach (var result in results)
                {
                    if (!result.gameObject.TryGetComponent<ITooltipElement>(out var tooltipElement))
                        continue;

                    if (currentElement != null)
                    {
                        bool sameElement = tooltipElement.UniqueId.Equals(currentElement.UniqueId);
                        if (sameElement) return;
                    }

                    if (!tooltipElement.HasData()) continue;
                    currentElement = tooltipElement;
                    break;
                }

                if (currentElement == null || !currentElement.HasData()) DisableTooltip();
                else EnableTooltip(currentElement.GetData());
            }
            else DisableTooltip();
        }

        public void EnableTooltip(TooltipData data)
        {
            if (tooltipEnabled) return;
            tooltipEnabled = true;
            tooltipWindow.Display(data);
            tooltipWindow.Animate(true);
        }

        public void DisableTooltip()
        {
            if (!tooltipEnabled) return;
            tooltipEnabled = false;
            tooltipWindow.Animate(false);
        }
    }
}