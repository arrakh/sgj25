using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private Image blackImage;

        private Tween tween;

        private void Update()
        {
            var alpha = blackImage.color.a;
            blackImage.gameObject.SetActive(alpha > 0f);
        }

        public void FadeIn(float duration)
        {
            Init();
            blackImage.color = new Color(0f, 0f, 0f, 0f);
            tween = blackImage.DOFade(1f, duration).OnComplete(OnDoneFade);
        }

        public void FadeOut(float duration)
        {
            Init();
            blackImage.color = new Color(0f, 0f, 0f, 1f);
            tween = blackImage.DOFade(0f, duration).OnComplete(OnDoneFade);
        }
        
        public IEnumerator FadeInCoroutine(float duration)
        {
            Init();
            blackImage.color = new Color(0f, 0f, 0f, 0f);
            tween = blackImage.DOFade(1f, duration).OnComplete(OnDoneFade);
            yield return new WaitForSeconds(duration);
        }

        public IEnumerator FadeOutCoroutine(float duration)
        {
            Init();
            blackImage.color = new Color(0f, 0f, 0f, 1f);
            tween = blackImage.DOFade(0f, duration).OnComplete(OnDoneFade);
            yield return new WaitForSeconds(duration);
        }

        private void Init()
        {
            blackImage.gameObject.SetActive(true);
            tween?.Kill();
        }

        private void OnDoneFade()
        {
            blackImage.gameObject.SetActive(false);
        }
    }
}