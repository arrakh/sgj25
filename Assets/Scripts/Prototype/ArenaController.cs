using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using Prototype.CardComponents;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Prototype
{
    public class ArenaController : MonoBehaviour
    {
        [Header("Design")] 
        [SerializeField] private int drawAmount = 4;
        [SerializeField] private int cardLeftToNextTurn = 1;
        [SerializeField] private int maxHealth = 20;
        [SerializeField] private int runAwayCooldown = 2;
        
        [Header("Scene References")] 
        [SerializeField] private RectTransform cardParent;
        [SerializeField] private TextMeshProUGUI roomText;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [FormerlySerializedAs("equippedCard")] [SerializeField] private CardVisual equippedCardVisual;
        [SerializeField] private TextMeshProUGUI equippedValueText;

        [SerializeField] private Button standardActionButton;
        [SerializeField] private TextMeshProUGUI standardActionText;

        [SerializeField] private Button fightActionButton;
        [SerializeField] private Button dungeonInfoButton;
        
        [SerializeField] private Button runActionButton;
        [SerializeField] private TextMeshProUGUI runActionText;

        [SerializeField] private RectTransform dungeonInfoRect;
        [SerializeField] private RectTransform infoParent;
        
        [SerializeField] private Slider dungeonProgress;
        [SerializeField] private GameObject resultScreen;
        [SerializeField] private TextMeshProUGUI resultScreenText;

        [FormerlySerializedAs("cardPrefab")]
        [Header("Prefabs")] 
        [SerializeField] private CardVisual cardVisualPrefab;
        [SerializeField] private TextMeshProUGUI infoPrefab;

        private Queue<CardInstance> deck = new();
        private List<CardVisual> spawnedCards = new();
        private List<CardInstance> discarded = new();
        private List<TextMeshProUGUI> spawnedInfo = new();
        private int health;
        private int roomCount = 0;
        private int cardInitialCount = 0;
        private int currentRunCooldown = 0;

        private CardVisual selectedCardVisual;
        private CardInstance equippedWeapon = null;

        private void Awake()
        {
            standardActionButton.onClick.AddListener(OnStandardAction);
            fightActionButton.onClick.AddListener(OnFightWithWeaponAction);
            runActionButton.onClick.AddListener(OnRunAction);
            dungeonInfoButton.onClick.AddListener(ToggleDungeonInfo);
            
            resultScreen.SetActive(false);
        }

        private void ToggleDungeonInfo()
        {
            var active = dungeonInfoRect.gameObject.activeInHierarchy;
            dungeonInfoRect.gameObject.SetActive(!active);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
                return;
            }
            
            healthText.text = $" {health}<size=33%>/{maxHealth}";

            var hasSelected = selectedCardVisual != null;
            var hasEquipped = equippedWeapon != null;
            
            standardActionButton.gameObject.SetActive(hasSelected);
            fightActionButton.gameObject.SetActive(hasSelected && hasEquipped && selectedCardVisual.Type == CardType.Monster );
            equippedCardVisual.gameObject.SetActive(hasEquipped);
            
            //runActionButton.gameObject.SetActive(roomCount > 1);
            var canRun = currentRunCooldown <= 0;
            runActionText.text = canRun ? "Run Away" : $"Cannot Run ({currentRunCooldown})";
            runActionButton.interactable = canRun;

            var fucksGiven = 0;
        }

        public void StartArena(CardData[] cards)
        {
            ResetState();

            cardInitialCount = cards.Length;
            var random = cards.OrderBy(_ => Random.value);
            foreach (var card in random)
                deck.Enqueue(new CardInstance(card));

            NextTurn();
        }

        private void ResetState()
        {
            roomCount = 0;
            currentRunCooldown = 0;
            health = maxHealth;
            discarded.Clear();
            ClearCards();
            selectedCardVisual = null;
            equippedWeapon = null;
        }

        private void ClearCards()
        {
            foreach (var card in spawnedCards)
                Destroy(card.gameObject);
            
            spawnedCards.Clear();
        }

        private void NextTurn()
        {
            if (health <= 0)
            {
                Death();
                return;
            }

            if (spawnedCards.Count <= cardLeftToNextTurn) NextRoom();

            UpdateInfo();

            if (deck.Count <= 0 && spawnedCards.Count <= 0) Victory();
        }

        private void NextRoom()
        {
            if (currentRunCooldown > 0) currentRunCooldown--;
            roomCount++;
            roomText.text = $"Room {roomCount}";
            
            //Spawn Cards
            for (int i = spawnedCards.Count; i < drawAmount; i++)
            {
                if (deck.Count <= 0) break;
                var card = deck.Dequeue();
                var cardObj = Instantiate(cardVisualPrefab, cardParent);
                cardObj.Display(card, OnCardPicked);
                
                spawnedCards.Add(cardObj);
            }
        }

        private void UpdateInfo()
        {
            foreach (var info in spawnedInfo)
                Destroy(info.gameObject);
            spawnedInfo.Clear();
            
            var topText = Instantiate(infoPrefab, infoParent);
            topText.text = $"In this Dungeon: ";
            spawnedInfo.Add(topText);

            var ordered = deck.OrderBy(x => x.Data.type).ThenBy(x => x.Data.value);
            foreach (var card in ordered)
            {
                var text = Instantiate(infoPrefab, infoParent);
                text.text = $"- <size=40%>({card.Data.type})</size> {card.Data.displayName} - {card.Data.value}";
                spawnedInfo.Add(text);
            }

            var remaining = (float) (deck.Count + spawnedCards.Count) / cardInitialCount;
            dungeonProgress.value = 1f - remaining;
        }

        private void OnCardPicked(CardVisual cardVisual)
        {
            if (selectedCardVisual != null) selectedCardVisual.SetSelected(false);
            
            selectedCardVisual = cardVisual;
            selectedCardVisual.SetSelected(true);

            standardActionText.text = selectedCardVisual.Type switch
            {
                CardType.Weapon => "Equip",
                CardType.Monster => "Fight Barehanded",
                CardType.Potion => "Heal",
                _ => throw new ArgumentOutOfRangeException()
            };

            bool canFightWithWeapon = selectedCardVisual.Type == CardType.Monster && equippedWeapon != null;
            fightActionButton.gameObject.SetActive(canFightWithWeapon);
        }
        
        private void OnStandardAction()
        {
            if (selectedCardVisual == null) return;
            
            switch (selectedCardVisual.Type)
            {
                case CardType.Weapon: OnEquipWeaponAction(); return;
                case CardType.Monster: OnFightWithBareHandsAction(); return;
                case CardType.Potion: OnHealAction(); return;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void OnHealAction()
        {
            AddHealth(selectedCardVisual.Value);
            ConsumeSelectedCard();
        }

        private void OnFightWithBareHandsAction()
        {
            AddHealth(-selectedCardVisual.Value);
            ConsumeSelectedCard();
        }

        public void AddHealth(int amount)
        {
            health += amount;
            health = Math.Clamp(health, 0, maxHealth);

            var shakeDur = Mathf.Clamp01(amount / 10f);
            var color = amount > 0 ? Color.green : Color.red;
            ShakeColorAnimateText(healthText, color, Mathf.Lerp(0.2f, 1.2f, shakeDur));
        }

        private void OnFightWithWeaponAction()
        {
            if (equippedWeapon == null) throw new Exception("EQUIPPED WEAPON IS EMPTY??");

            var remainder = equippedWeapon.Data.value - selectedCardVisual.Value;

            if (remainder < 0) //monster is stronger
            {
                health += remainder;
                
                var shakeDur = Mathf.Clamp01(remainder / 10f);
                ShakeColorAnimateText(healthText, Color.red, Mathf.Lerp(0.2f, 1.2f, shakeDur));
            }
            else //monster is weaker
            {
                int newValue = selectedCardVisual.Value;

                foreach (var component in equippedWeapon.Components)
                    if (component is IMitigateWeaponDegrade mitigate) 
                        mitigate.Mitigate(ref newValue);
                
                equippedWeapon.SetValue(newValue);
                
                equippedCardVisual.Display(equippedWeapon, null);
                
                ShakeColorAnimateText(equippedValueText, Color.red, 0.3f);
            }

            foreach (var component in equippedWeapon.Components)
                if (component is IOnDestroyMonsterWithWeapon comp) 
                    comp.OnDestroy(this, selectedCardVisual.Instance, equippedWeapon);
            ConsumeSelectedCard();
        }

        private void OnEquipWeaponAction()
        {
            equippedWeapon = selectedCardVisual.Instance;
            
            equippedCardVisual.Display(equippedWeapon, null);
            
            ShakeColorAnimateText(equippedValueText, Color.green, 0.3f);

            ConsumeSelectedCard();
        }

        private void OnRunAction()
        {
            if (currentRunCooldown > 0) return;
            currentRunCooldown = runAwayCooldown + 1;
            
            foreach (var card in spawnedCards)
            {
                deck.Enqueue(card.Instance);
                Destroy(card.gameObject);
            }
            
            spawnedCards.Clear();
            
            NextTurn();
        }

        public void UnequipWeapon()
        {
            equippedWeapon = null;
        }

        private void ConsumeSelectedCard()
        {
            spawnedCards.Remove(selectedCardVisual);
            
            discarded.Add(selectedCardVisual.Instance);
            
            Destroy(selectedCardVisual.gameObject);
            
            selectedCardVisual = null;

            NextTurn();
        }

        private void Death()
        {
            resultScreen.SetActive(true);
            resultScreenText.text = "You DIED! \n<size=40%>press [esc] to Retry";
        }

        private void Victory()
        {
            resultScreen.SetActive(true);
            resultScreenText.text = "You WIN! \n<size=40%>press [esc] to Retry";
        }

        private void ShakeColorAnimateText(TextMeshProUGUI text, Color color, float duration)
        {
            text.DOKill();
            
            text.transform.localScale = Vector3.one * 1.1f;
            text.transform.DOScale(Vector3.one, duration);
            text.transform.DOShakePosition(duration, 10f);
            var originalColor = text.color;
            text.color = color;
            text.DOColor(originalColor, duration);
        }
    }
}