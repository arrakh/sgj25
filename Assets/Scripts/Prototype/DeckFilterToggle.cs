using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class DeckFilterToggle : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private CardType type;
        [SerializeField] private DeckBuildingScreen deckBuildingScreen;
        
        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool on)
        {
            if (!on) return;
            
            deckBuildingScreen.SetTypeFilter(type);
        }
    }
}