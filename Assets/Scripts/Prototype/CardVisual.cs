using System;
using System.Collections.Generic;
using DG.Tweening;
using Prototype.CardComponents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace Prototype
{
    //Too monolithic and only specific to arena, should've been visual only
    public class CardVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button cardButton;
        [SerializeField] private Image icon;
        [SerializeField] private Image nameplate;
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image highlight, lockedImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CardComponentVisualController componentController;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool canSelect = true;
        [SerializeField] private RectTransform visualParent;
        
        [Header("Animation: Reveal")]
        [SerializeField] private float revealTime = 1.2f;
        [SerializeField] private float inDistance = 100f;
        [SerializeField] private AnimationCurve inCurve;

        public CardType Type => instance.Data.type;
        public CardData Data => instance.Data;
        public CardInstance Instance => instance;
        public int Value => instance.Data.value;

        private CardInstance instance;


        private bool isSelected;
        private bool isLocked;

        private Action<CardVisual> pickedCallback;

        private void Awake()
        {
            cardButton.onClick.AddListener(Pick);
        }

        public void Display(CardInstance cardInstance, Action<CardVisual> onCardPicked)
        {
            if (instance == null || instance.Data.id.Equals(cardInstance.Data.id))
                componentController.Display(cardInstance);
            
            instance = cardInstance;

            nameText.text = instance.Data.displayName;
            
            pickedCallback = onCardPicked;

            valueText.text = Value.ToString();
            icon.sprite = SpriteDatabase.Get(instance.Data.spriteId);

            var type = instance.Data.type;
            background.sprite = type.GetBackground();
            nameplate.sprite = type.GetBanner();
            
            OnDisplay(instance);
        }

        protected virtual void OnDisplay(CardInstance instance){}

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
            if (!canSelect) return;
            isSelected = selected;
            highlight.gameObject.SetActive(isSelected);
            if (!isSelected) return;
            highlight.color = Color.white;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!canSelect) return;
            if (isSelected) return;
            highlight.color = Color.red;
            highlight.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!canSelect) return;
            if (isSelected) return;
            highlight.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            highlight.gameObject.SetActive(false);
        }

        public void RevealAnimation(float delay)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, revealTime).SetDelay(delay);

            var localPos = visualParent.localPosition;
            localPos.y -= inDistance;
            visualParent.localPosition = localPos;

            visualParent.DOLocalMoveY(0f, revealTime).SetEase(inCurve).SetDelay(delay);
        }
    }
}
