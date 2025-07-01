using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prototype
{
    public class DeckBuildingScreen : MonoBehaviour
    {
        [Serializable]
        public struct LibraryEntry
        {
            public string id;
            public int amount;
        }
        
        private const string SAVED_DECK = "saved-deck";
        private const string SAVED_LIBRARY = "saved-library";

        [FormerlySerializedAs("cardDb")]
        [Header("Scene")] 
        [SerializeField] private GameDatabase gameDb;
        [SerializeField] private RectTransform deckCardParent;
        [SerializeField] private RectTransform libraryCardParent;
        [SerializeField] private Slider hypeSlider;
        [SerializeField] private Image hypeSliderFill;
        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private TextMeshProUGUI hypeValue;
        [SerializeField] private Button fightButton;
        [SerializeField] private Button clearButton;
        
        [Header("Prefabs")] 
        [SerializeField] private DeckCard deckCardPrefab;
        [SerializeField] private LibraryCard libraryCardPrefab;

        [Header("Data")] 
        [SerializeField] private Gradient hypeGradient;

        public bool CompleteBuilding { get; private set; } = false;
        public CardData[] Deck => currentDeck.Select(x => x.Data).ToArray();
        
        private List<DeckCard> currentDeck = new();
        private Dictionary<string, LibraryCard> currentLibrary = new();

        private int hype = 0;
        private int hypeMin = 0;

        public void Initialize(int minimumHype)
        {
            hypeMin = minimumHype;
            LibraryEntry[] library = GetSavedLibrary();

            foreach (var entry in library)
            {
                var cardData = gameDb.GetCard(entry.id);
                var card = AddLibraryCard(cardData);
                card.SetAmount(entry.amount);
            }

            string[] deck = GetSavedDeck();

            foreach (var cardId in deck)
            {
                var cardData = gameDb.GetCard(cardId);
                AddDeckCard(cardData);
            }

            RecalculateHype();
        }

        private void RecalculateHype()
        {
            hype = 0;

            foreach (var card in currentDeck)
                hype += card.Data.cost;

            hypeValue.text = $"{hype}/{hypeMin}";

            var hypeAlpha = (float) hype / hypeMin;
            var hypeColorAlpha = Mathf.Clamp01((hypeAlpha - 1f) / 3f);
            hypeValue.color = hype < hypeMin ? Color.white : hypeGradient.Evaluate(hypeColorAlpha);
            hypeSlider.value = Mathf.Clamp01(hypeAlpha);
            hypeSliderFill.color = hypeGradient.Evaluate(hypeColorAlpha);

            hypeLabel.text = "Hype Lvl: " + hypeAlpha switch
            {
                < 1f => "Low",
                < 1.2f => "High",
                < 1.6f => "Very High",
                < 2.3f => "Extreme",
                < 3f => "Extra Extreme",
                _ => throw new ArgumentOutOfRangeException()
            };

            //fightButton.interactable = hype >= hypeMin;
        }

        private void OnLibraryCardPicked(Card card)
        {
            Debug.Log($"PICKED LIBRARY {card.Data.id}");
            if (card is not LibraryCard libraryCard) throw new Exception("HUH???");
            
            //this is stupid, the card should be visual only, not handling amount logic
            if (libraryCard.Amount <= 1)
            {
                Destroy(libraryCard.gameObject);
                currentLibrary.Remove(card.Data.id);
            }
            else libraryCard.SetAmount(libraryCard.Amount - 1);

            AddDeckCard(card.Data);
            
            RecalculateHype();
        }

        private void OnDeckCardPicked(Card card)
        {
            Debug.Log($"PICKED DECK {card.Data.id}");

            if (card is not DeckCard deckCard) throw new Exception("HUH???");

            currentDeck.Remove(deckCard);
            AddLibraryCard(card.Data);
            
            Destroy(deckCard.gameObject);
            RecalculateHype();
        }

        private DeckCard AddDeckCard(CardData card)
        {
            var newCard = Instantiate(deckCardPrefab, deckCardParent);
            newCard.Display(card, OnDeckCardPicked);
            currentDeck.Add(newCard);
            return newCard;
        }

        private LibraryCard AddLibraryCard(CardData card)
        {
            if (currentLibrary.TryGetValue(card.id, out var libraryCard))
            {
                libraryCard.SetAmount(libraryCard.Amount + 1);
                return libraryCard;
            }
            else
            {
                var newCard = Instantiate(libraryCardPrefab, libraryCardParent);
                newCard.Display(card, OnLibraryCardPicked);
                currentLibrary[card.id] = newCard;
                newCard.SetAmount(1);
                return newCard;
            }
        }

        private void Awake()
        {
            fightButton.onClick.AddListener(OnFightButton);
            clearButton.onClick.AddListener(OnClearDeck);
        }

        private void OnClearDeck()
        {
            foreach (var card in currentDeck)
            {
                AddLibraryCard(card.Data);
                Destroy(card.gameObject);
            }   
            
            currentDeck.Clear();
            
            RecalculateHype();
        }

        private void OnFightButton()
        {
            //if (hype < hypeMin) return;
            
            CompleteBuilding = true;
        }
        
        #region MOVE THESE INTO A GAMESAVE CLASS
        //////////////////////////////////////////////
        private LibraryEntry[] GetSavedLibrary()
        {
            if (!PlayerPrefs.HasKey(SAVED_LIBRARY))
                return DEBUG_Library();

            var libraryJson = PlayerPrefs.GetString(SAVED_LIBRARY);
            var library = JsonConvert.DeserializeObject<LibraryEntry[]>(libraryJson);
            return library;
        }

        private LibraryEntry[] DEBUG_Library()
            => gameDb.AllCards.Select(x => new LibraryEntry{id = x.id, amount = 1}).ToArray();

        private string[] GetSavedDeck()
        {
            if (!PlayerPrefs.HasKey(SAVED_DECK))
                return gameDb.StartingDeck;

            var deckJson = PlayerPrefs.GetString(SAVED_DECK);
            var deck = JsonConvert.DeserializeObject<string[]>(deckJson);
            return deck;
        }
        //////////////////////////////////////////////
        #endregion
    }
}