using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Tooltip
{
    public class TooltipWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title, content;
        [SerializeField] private RectTransform windowRect;

        [Header("Animation")] 
        [SerializeField] private CanvasGroup alphaGroup;
        [SerializeField] private AnimationCurve animCurve;
        [SerializeField] private float animTime;
        [SerializeField] private float pivotLerpSpeed = 10f;

        private bool hasExtraContent, isExtraVisible;
        private TooltipData currentData;
        
        public void Display(TooltipData data)
        {
            currentData = data;
            title.text = data.title;
            content.text = data.content;
            
            Invoke(nameof(DelayedRedraw), 0.001f);
        }

        private void Update()
        {
            CalculatePosition();
        }

        void DelayedRedraw()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(windowRect);
        }

        private void CalculatePosition()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 mousePosition = Input.mousePosition;
            transform.position = mousePosition;

            Vector2 normalizedMousePos = new Vector2(mousePosition.x / screenSize.x, mousePosition.y / screenSize.y);
            
            int quadrantX = normalizedMousePos.x < 0.5f ? 0 : 1;
            int quadrantY = normalizedMousePos.y < 0.5f ? 0 : 1;

            Vector2 targetPivot = new Vector2(quadrantX, quadrantY);

            Vector2 currentPivot = windowRect.pivot;
            windowRect.pivot = Vector2.Lerp(currentPivot, targetPivot, Time.unscaledDeltaTime * pivotLerpSpeed);
        }

        public void Animate(bool isAnimatingIn)
        {
            DOTween.Kill(this);

            var from = isAnimatingIn ? 0f : 1f;
            var target = isAnimatingIn ? 1f : 0f;
            
            var scale = windowRect.localScale;
            scale.y = from;
            windowRect.localScale = scale;

            alphaGroup.alpha = from;
            
            windowRect.DOScaleY(target, animTime).SetEase(animCurve).SetUpdate(true);
            alphaGroup.DOFade(target, animTime).SetEase(animCurve).SetUpdate(true);
        }
    }
}