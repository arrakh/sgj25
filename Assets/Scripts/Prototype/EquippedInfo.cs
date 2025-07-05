using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Prototype
{
    public class EquippedInfo : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private Image nameplate;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CardComponentVisualController componentController;
        
        private CardInstance instance;

        public void Display(CardInstance cardInstance, Action<CardVisual> onCardPicked)
        {
            if (instance == null || instance.Data.id.Equals(cardInstance.Data.id))
                componentController.Display(cardInstance);
            
            instance = cardInstance;
            
            var type = instance.Data.type;
            background.sprite = type.GetBackground();
            nameplate.sprite = type.GetBanner();
            
            nameText.text = instance.Data.displayName;

            valueText.text = instance.Data.value.ToString();
            icon.sprite = SpriteDatabase.Get(instance.Data.spriteId);
        }
    }
}