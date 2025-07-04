﻿using System;
using Prototype.CardComponents;
using Prototype.Tooltip;
using TMPro;
using UnityEngine;

namespace Prototype
{
    public class CardComponentVisual : MonoBehaviour, ITooltipElement
    {
        [SerializeField] private TextMeshProUGUI content;

        private CardComponent component;
        
        public void Display(CardComponent cardComponent)
        {
            if (component != null)
                component.OnUpdated -= OnUpdate;
            
            component = cardComponent;

            content.text = component.DisplayName;

            component.OnUpdated += OnUpdate;
        }

        private void OnUpdate(CardComponent obj)
        {
            content.text = obj.DisplayName;
        }

        private void OnDestroy()
        {
            if (component == null) return;
            component.OnUpdated -= OnUpdate;
        }

        public bool HasData() => component != null;

        public TooltipData GetData() => new(component.DisplayName, component.Description);

        public int UniqueId => gameObject.GetInstanceID();
    }
}