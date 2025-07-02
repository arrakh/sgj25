using Prototype.CardComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class CardComponentVisualController : MonoBehaviour
    {
        [SerializeField] private RectTransform visualParent;
        [SerializeField] private CardComponentVisual[] visuals;
        
        public void Display(CardInstance instance)
        {
            int index = 0;
            foreach (var component in instance.Components)
            {
                if (index >= visuals.Length) break;

                visuals[index].Display(component);

                component.OnUpdated += OnComponentUpdated;
                
                visuals[index].gameObject.SetActive(true);

                index++;
            }

            for (int i = index; i < visuals.Length; i++)
                visuals[i].gameObject.SetActive(false);
        }

        private void OnComponentUpdated(CardComponent component)
        {
            Invoke(nameof(RebuildVisuals), 0.02f);
        }

        private void RebuildVisuals()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(visualParent);
        }
    }
}