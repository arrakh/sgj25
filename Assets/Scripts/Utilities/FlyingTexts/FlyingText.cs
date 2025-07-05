using System;
using TMPro;
using UnityEngine;

namespace Utilities.FlyingTexts
{
    public class FlyingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float flySpeed = 100;
        [SerializeField] private AnimationCurve opacityCurve;

        private float currentTimer = 0f;
        private float totalDuration = 0f;
        private Action<FlyingText> onDoneFlying;

        public TextMeshProUGUI Text => text;

        public void Display(string textToDisplay, float duration, Vector3 screenPos, Action<FlyingText> onDone)
        {
            currentTimer = 0;
            totalDuration = duration;
            text.text = textToDisplay;
            text.transform.position = screenPos;
            onDoneFlying = onDone;
        }

        private void Update()
        {
            text.transform.position += Vector3.up * (flySpeed * Time.deltaTime);
            
            currentTimer += Time.deltaTime;
            var alpha = currentTimer / totalDuration;
            if (alpha > 1f)
            {
                onDoneFlying?.Invoke(this);
                return;
            }

            var color = text.color;
            color.a = opacityCurve.Evaluate(alpha);
            text.color = color;
        }
    }
}