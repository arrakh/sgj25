using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class EquippedInfo : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CardComponentVisualController componentController;
        
        private CardInstance instance;

        public void Display(CardInstance cardInstance, Action<CardVisual> onCardPicked)
        {
            if (instance == null || instance.Data.id.Equals(cardInstance.Data.id))
                componentController.Display(cardInstance);
            
            instance = cardInstance;

            nameText.text = instance.Data.displayName;

            valueText.text = instance.Data.value.ToString();
            icon.sprite = SpriteDatabase.Get(instance.Data.spriteId);
        }
    }
}