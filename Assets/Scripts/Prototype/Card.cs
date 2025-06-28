using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite weapon, monster, potion;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image highlight, lockedImage;

        public CardType Type { private set; get; }
        public int Value { private set; get; }
        
        private bool isSelected;
        private bool isLocked;

        private Action<Card> pickedCallback;
        
        public void Display(CardData data, Action<Card> onCardPicked)
        {
            Value = data.value;
            Type = data.type;

            pickedCallback = onCardPicked;

            valueText.text = Value.ToString();
            icon.sprite = Type switch
            {
                CardType.Weapon => weapon,
                CardType.Monster => monster,
                CardType.Potion => potion,
                _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
            };
        }

        public void Pick()
        {
            if (isLocked) return;
            pickedCallback?.Invoke(this);
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
            lockedImage.gameObject.SetActive(locked);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            highlight.gameObject.SetActive(isSelected);
            if (!isSelected) return;
            highlight.color = Color.yellow;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isSelected) return;
            highlight.color = Color.red;
            highlight.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isSelected) return;
            highlight.gameObject.SetActive(false);
        }
    }
}
