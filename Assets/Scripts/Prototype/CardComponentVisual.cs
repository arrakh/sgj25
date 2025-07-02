using Prototype.CardComponents;
using TMPro;
using UnityEngine;

namespace Prototype
{
    public class CardComponentVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI content;

        private CardComponent component;
        
        public void Display(CardComponent cardComponent)
        {
            component = cardComponent;

            content.text = component.DisplayName;

            component.OnUpdated += OnUpdate;
        }

        private void OnUpdate(CardComponent obj)
        {
            content.text = obj.DisplayName;
        }
    }
}