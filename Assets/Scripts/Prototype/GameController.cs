using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Prototype
{
    public class GameController : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private TextAsset cardJson;

        [Header("Design")] 
        [SerializeField] private int drawAmount = 4;
        [SerializeField] private int cardLeftToNextTurn = 1;
        [SerializeField] private int maxHealth = 20;
        [SerializeField] private int runAwayCooldown = 2;
        
        [Header("Scene References")] 
        [SerializeField] private RectTransform cardParent;
        [SerializeField] private TextMeshProUGUI roomText;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [SerializeField] private Card equippedCard;
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

        [Header("Prefabs")] 
        [SerializeField] private Card cardPrefab;
        [SerializeField] private TextMeshProUGUI infoPrefab;

        private Queue<CardData> deck = new();
        private List<Card> spawnedCards = new();
        private List<CardData> discarded = new();
        private List<TextMeshProUGUI> spawnedInfo = new();
        private int health;
        private int roomCount = 0;
        private int cardInitialCount = 0;
        private int currentRunCooldown = 0;

        private Card selectedCard;
        private CardData equippedWeapon = CardData.Empty;

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

            var hasSelected = selectedCard != null;
            var hasEquipped = !equippedWeapon.Equals(CardData.Empty);
            
            standardActionButton.gameObject.SetActive(hasSelected);
            fightActionButton.gameObject.SetActive(hasSelected && hasEquipped && selectedCard.Type == CardType.Monster );
            equippedCard.gameObject.SetActive(hasEquipped);
            
            runActionButton.gameObject.SetActive(roomCount > 1);
            var canRun = currentRunCooldown <= 0;
            runActionText.text = canRun ? "Run Away" : $"Cannot Run ({currentRunCooldown})";
            runActionButton.interactable = canRun;

            var fucksGiven = 0;
        }

        private void Start()
        {
            health = maxHealth;
            var data = JsonConvert.DeserializeObject<CardData[]>(cardJson.text);
            cardInitialCount = data.Length;

            var random = data.OrderBy(_ => Random.value);
            foreach (var card in random)
                deck.Enqueue(card);

            NextTurn();
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
                var cardObj = Instantiate(cardPrefab, cardParent);
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

            var ordered = deck.OrderBy(x => x.type).ThenBy(x => x.value);
            foreach (var card in ordered)
            {
                var text = Instantiate(infoPrefab, infoParent);
                text.text = $"- {card.type} - {card.value}";
                spawnedInfo.Add(text);
            }

            var remaining = (float) (deck.Count + spawnedCards.Count) / cardInitialCount;
            dungeonProgress.value = 1f - remaining;
        }

        private void OnCardPicked(Card card)
        {
            if (selectedCard != null) selectedCard.SetSelected(false);
            
            selectedCard = card;
            selectedCard.SetSelected(true);

            standardActionText.text = selectedCard.Type switch
            {
                CardType.Weapon => "Equip",
                CardType.Monster => "Fight Barehanded",
                CardType.Potion => "Heal",
                _ => throw new ArgumentOutOfRangeException()
            };

            bool canFightWithWeapon = selectedCard.Type == CardType.Monster && !equippedWeapon.Equals(CardData.Empty);
            fightActionButton.gameObject.SetActive(canFightWithWeapon);
        }
        
        private void OnStandardAction()
        {
            if (selectedCard == null) return;
            
            switch (selectedCard.Type)
            {
                case CardType.Weapon: OnEquipWeaponAction(); return;
                case CardType.Monster: OnFightWithBareHandsAction(); return;
                case CardType.Potion: OnHealAction(); return;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void OnHealAction()
        {
            health += selectedCard.Value;
            health = Math.Clamp(health, 0, maxHealth);
            ShakeColorAnimateText(healthText, Color.green, 0.3f);
            
            ConsumeSelectedCard();
        }

        private void OnFightWithBareHandsAction()
        {
            health -= selectedCard.Value;
            health = Math.Clamp(health, 0, maxHealth);

            var shakeDur = Mathf.Clamp01(selectedCard.Value / 10f);
            ShakeColorAnimateText(healthText, Color.red, Mathf.Lerp(0.2f, 1.2f, shakeDur));

            ConsumeSelectedCard();
        }

        private void OnFightWithWeaponAction()
        {
            if (equippedWeapon.Equals(CardData.Empty)) throw new Exception("EQUIPPED WEAPON IS EMPTY??");

            var remainder = equippedWeapon.value - selectedCard.Value;

            if (remainder < 0) //monster is stronger
            {
                health += remainder;
                
                var shakeDur = Mathf.Clamp01(remainder / 10f);
                ShakeColorAnimateText(healthText, Color.red, Mathf.Lerp(0.2f, 1.2f, shakeDur));
            }
            else
            {
                equippedWeapon.value = selectedCard.Value;
                equippedCard.Display(equippedWeapon, null);
                
                ShakeColorAnimateText(equippedValueText, Color.red, 0.3f);
            }

            ConsumeSelectedCard();
        }

        private void OnEquipWeaponAction()
        {
            equippedWeapon = selectedCard.Data;
            
            equippedCard.Display(equippedWeapon, null);
            
            ShakeColorAnimateText(equippedValueText, Color.green, 0.3f);

            ConsumeSelectedCard();
        }

        private void OnRunAction()
        {
            if (currentRunCooldown > 0) return;
            currentRunCooldown = runAwayCooldown + 1;
            
            foreach (var card in spawnedCards)
            {
                deck.Enqueue(new CardData()
                {
                    type = card.Type,
                    value = card.Value
                });
                
                Destroy(card.gameObject);
            }
            
            spawnedCards.Clear();
            
            NextTurn();
        }

        private void ConsumeSelectedCard()
        {
            spawnedCards.Remove(selectedCard);
            
            discarded.Add(selectedCard.Data);
            
            Destroy(selectedCard.gameObject);
            
            selectedCard = null;

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