using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class DeckBuildingScreen : MonoBehaviour
    {
        private const string SAVED_DECK = "saved-deck";
        private const string SAVED_LIBRARY = "saved-library";

        [Header("Scene")] [SerializeField] private GameDatabase gameDb;
        [SerializeField] private RectTransform deckCardParent;
        [SerializeField] private RectTransform libraryCardParent;
        [SerializeField] private Slider hypeSlider;
        [SerializeField] private Image hypeSliderFill;
        [SerializeField] private TextMeshProUGUI hypeLabel;
        [SerializeField] private TextMeshProUGUI hypeValue;
        [SerializeField] private Button fightButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private TextMeshProUGUI deckLabel;
        [SerializeField] private TextMeshProUGUI libraryLabel;

        [Header("Prefabs")] [SerializeField] private DeckCardVisual deckCardVisualPrefab;
        [SerializeField] private LibraryCardVisual libraryCardVisualPrefab;

        [Header("Data")] [SerializeField] private Gradient hypeGradient;

        public bool CompleteBuilding { get; private set; }
        public CardData[] Deck => currentDeck.Select(i => i.Data).ToArray();

        private readonly List<CardInstance> currentDeck = new();
        private readonly Dictionary<string, LibraryEntry> currentLibrary = new();

        private readonly Dictionary<CardInstance, DeckCardVisual> deckVisByInst = new();
        private readonly Dictionary<string, LibraryCardVisual> libVisById = new();

        private readonly Stack<DeckCardVisual> deckPool = new();
        private readonly Stack<LibraryCardVisual> libPool = new();

        private CardType typeFilter = CardType.None;
        private int hype;
        private int hypeMin;

        private bool shouldRestrictHype;

        public void Initialize(int minimumHype, bool restrictHype)
        {
            hypeMin = minimumHype;
            shouldRestrictHype = restrictHype;

            foreach (var entry in GetSavedLibrary())
                currentLibrary.Add(entry.id, new LibraryEntry(entry, gameDb));

            foreach (var cardId in GetSavedDeck())
                currentDeck.Add(new CardInstance(gameDb.GetCard(cardId)));

            foreach (var (id, entry) in currentLibrary)
            {
                var vis = GetLibVisual();
                vis.Display(entry.instance, OnLibraryCardPicked);
                vis.SetAmount(entry.Amount);
                libVisById[id] = vis;
                Debug.Log($"ADDING TO LIBRARY ID {id} AMOUNT {entry.Amount}");
            }

            foreach (var inst in currentDeck)
            {
                var vis = GetDeckVisual();
                vis.Display(inst, OnDeckCardPicked);
                deckVisByInst[inst] = vis;
            }

            RecalculateHype();
            UpdateCountLabels();
        }

        private DeckCardVisual GetDeckVisual()
        {
            return deckPool.Count > 0
                ? deckPool.Pop().Enable(deckCardParent)
                : Instantiate(deckCardVisualPrefab, deckCardParent);
        }

        private void ReturnDeckVisual(DeckCardVisual vis)
        {
            vis.Disable();
            deckPool.Push(vis);
        }

        private LibraryCardVisual GetLibVisual()
        {
            return libPool.Count > 0
                ? libPool.Pop().Enable(libraryCardParent)
                : Instantiate(libraryCardVisualPrefab, libraryCardParent);
        }

        private void ReturnLibVisual(LibraryCardVisual vis)
        {
            vis.Disable();
            libPool.Push(vis);
        }

        public void SetTypeFilter(CardType type)
        {
            typeFilter = type;
            ApplyTypeFilter();
        }

        private void ApplyTypeFilter()
        {
            foreach (var v in libVisById.Values)
                v.gameObject.SetActive(typeFilter == CardType.None || v.Data.type == typeFilter);

            foreach (var v in deckVisByInst.Values)
                v.gameObject.SetActive(typeFilter == CardType.None || v.Data.type == typeFilter);

            UpdateCountLabels();
        }

        public void ResortDeck(Func<CardData, IComparable> keySelector)
        {
            var ordered = deckVisByInst.Values.OrderBy(v => keySelector(v.Data)).ToArray();
            for (var i = 0; i < ordered.Length; ++i)
                ordered[i].transform.SetSiblingIndex(i);
        }

        private void OnLibraryCardPicked(CardVisual visual)
        {
            var id = visual.Data.id;
            var entry = currentLibrary[id];
            var inst = entry.instance;

            // library side
            entry.SetAmount(entry.Amount - 1);
            libVisById[id].SetAmount(entry.Amount);
            if (entry.Amount == 0)
            {
                ReturnLibVisual(libVisById[id]);
                libVisById.Remove(id);
                currentLibrary.Remove(id);
            }

            // deck side
            var newInst = new CardInstance(inst.Data);
            var deckVis = GetDeckVisual();
            deckVis.Display(newInst, OnDeckCardPicked);
            deckVisByInst.Add(newInst, deckVis);
            currentDeck.Add(newInst);

            RecalculateHype();
            UpdateCountLabels();
        }

        private void OnDeckCardPicked(CardVisual visual)
        {
            var inst = visual.Instance;

            // deck side
            currentDeck.Remove(inst);
            ReturnDeckVisual(deckVisByInst[inst]);
            deckVisByInst.Remove(inst);

            // library side
            AddToLibrary(inst);

            RecalculateHype();
            UpdateCountLabels();
        }

        private void AddToLibrary(CardInstance inst)
        {
            if (currentLibrary.TryGetValue(inst.Data.id, out var entry))
            {
                entry.SetAmount(entry.Amount + 1);
                libVisById[inst.Data.id].SetAmount(entry.Amount);
            }
            else
            {
                var newEntry = new LibraryEntry(inst, 1);
                currentLibrary.Add(inst.Data.id, newEntry);

                var vis = GetLibVisual();
                vis.Display(inst, OnLibraryCardPicked);
                vis.SetAmount(1);
                libVisById.Add(inst.Data.id, vis);

                // keep alphabetical order for library visuals
                vis.transform.SetAsLastSibling();
            }
        }

        private void RecalculateHype()
        {
            hype = currentDeck.Sum(c => c.Data.cost);

            hypeValue.text = $"{hype}/{hypeMin}";
            var alpha = (float) hype / hypeMin;
            var color = hypeGradient.Evaluate(Mathf.Clamp01((alpha - 1f) / 3f));
            hypeValue.color = hype < hypeMin ? Color.white : color;

            hypeSlider.value = Mathf.Clamp01(alpha);
            hypeSliderFill.color = color;

            hypeLabel.text = "Hype Lvl: " + alpha switch
            {
                < 1f => "Low",
                < 1.2f => "High",
                < 1.6f => "Very High",
                < 2.3f => "Extreme",
                < 3f => "Extra Extreme",
                _ => "??"
            };

            fightButton.interactable = !shouldRestrictHype || hype >= hypeMin;
        }

        private void UpdateCountLabels()
        {
            var libCount = 0;
            foreach (var (_, entry) in currentLibrary) libCount += entry.Amount;
            libraryLabel.text = $"Library ({libCount})";
            deckLabel.text = $"Arena ({deckVisByInst.Values.Count(v => v.gameObject.activeSelf)})";
        }

        private void Awake()
        {
            fightButton.onClick.AddListener(OnFightButton);
            clearButton.onClick.AddListener(OnClearDeck);
        }

        private void OnClearDeck()
        {
            foreach (var inst in currentDeck.ToArray())
                OnDeckCardPicked(deckVisByInst[inst]);
        }

        private void OnFightButton()
        {
            if (shouldRestrictHype && hype < hypeMin) return;
            CompleteBuilding = true;
        }

        #region MOVE TO GAME SAVE

        private LibraryData[] GetSavedLibrary()
        {
            if (!PlayerPrefs.HasKey(SAVED_LIBRARY))
                return DEBUG_Library();

            var json = PlayerPrefs.GetString(SAVED_LIBRARY);
            return JsonConvert.DeserializeObject<LibraryData[]>(json);
        }

        private LibraryData[] DEBUG_Library()
        {
            return gameDb.AllCards.Select(x => new LibraryData {id = x.id, amount = 1}).ToArray();
        }

        private string[] GetSavedDeck()
        {
            if (!PlayerPrefs.HasKey(SAVED_DECK))
                return gameDb.StartingDeck;

            var json = PlayerPrefs.GetString(SAVED_DECK);
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        #endregion
    }

    internal static class PoolExtensions
    {
        public static T Enable<T>(this T mono, Transform parent) where T : MonoBehaviour
        {
            mono.transform.SetParent(parent, false);
            mono.gameObject.SetActive(true);
            return mono;
        }

        public static void Disable<T>(this T mono) where T : MonoBehaviour
        {
            mono.gameObject.SetActive(false);
        }
    }
}