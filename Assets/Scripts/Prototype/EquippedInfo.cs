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
        [SerializeField] private RectTransform valueRect;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CardComponentVisualController componentController;
        
        private CardInstance instance;

        public Image Icon => icon;

        public void Display(CardInstance cardInstance)
        {
            if (instance == null || instance.Data.id.Equals(cardInstance.Data.id))
                componentController.Display(cardInstance);
            
            instance = cardInstance;
            
            var type = instance.Data.type;
            background.sprite = type.GetBackground();
            nameplate.sprite = type.GetBanner();
            
            nameText.text = instance.Data.displayName;
            
            valueRect.gameObject.SetActive(instance.Data.value > 0);

            valueText.text = instance.Data.value.ToString();
            icon.sprite = SpriteDatabase.Get(instance.Data.spriteId);
        }

        public void SetShowIcon(bool show)
        {
            icon.enabled = show;
        }
    }
}