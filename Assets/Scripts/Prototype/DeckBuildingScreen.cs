using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class DeckBuildingScreen : MonoBehaviour
    {
        [Header("Scene References")] 
        [SerializeField] private CardDatabase cardDb;
        [SerializeField] private RectTransform deckCardParent;
        [SerializeField] private RectTransform libraryCardParent;
        [SerializeField] private Slider hypeSlider;
        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private TextMeshProUGUI hypeValue;
        [SerializeField] private Button fightButton;
        
        [Header("Prefabs")] 
        [SerializeField] private Card deckCardPrefab;
        [SerializeField] private Card libraryCardPrefab;

        public TaskCompletionSource<(CardData[], int)> DoneBuildingTcs => doneBuildingTcs;
        
        private TaskCompletionSource<(CardData[], int)> doneBuildingTcs = new();
        private List<DeckCard> currentDeck;
        private Dictionary<string, LibraryCard> currentLibrary;

        public void Initialize()
        {
            doneBuildingTcs = new();
            
        }
        
        private void Awake()
        {
            fightButton.onClick.AddListener(OnFightButton);
        }

        private void OnFightButton()
        {
            
        }
    }
}