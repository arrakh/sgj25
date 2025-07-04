﻿using System;
using Prototype.CardComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class CardComponentVisualController : MonoBehaviour
    {
        [SerializeField] private RectTransform visualParent;
        [SerializeField] private CardComponentVisual[] visuals;

        private CardInstance cardInstance;

        public void Display(CardInstance instance)
        {
            if (cardInstance != null)
                foreach (var c in cardInstance.Components)
                    c.OnUpdated -= OnComponentUpdated;

            cardInstance = instance;

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

        private void OnDestroy()
        {
            if (cardInstance == null) return;
            foreach (var component in cardInstance.Components)
                component.OnUpdated -= OnComponentUpdated;
        }

        private void OnComponentUpdated(CardComponent component)
        {
            if (visualParent == null) return;
            Invoke(nameof(RebuildVisuals), 0.02f);
        }

        private void RebuildVisuals()
        {
            if (visualParent == null) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(visualParent);
        }
    }
}