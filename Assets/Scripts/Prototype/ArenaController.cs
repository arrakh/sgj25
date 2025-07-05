using System;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization.Scripts;
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
        [SerializeField] private ArenaEffects arenaEffect;
        [SerializeField] private RectTransform cardParent;
        [SerializeField] private RectTransform nextCardsParent;
        [SerializeField] private TextMeshProUGUI roomText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Slider healthSlider;
        
        [SerializeField] private EquippedInfo equippedToolVisual;
        [SerializeField] private EquippedInfo equippedWeaponVisual;
        [SerializeField] private TextMeshProUGUI equippedValueText;

        [SerializeField] private Button standardActionButton;
        [SerializeField] private TextMeshProUGUI standardActionText;

        [SerializeField] private Button fightActionButton;
        
        [SerializeField] private Button runActionButton;
        [SerializeField] private TextMeshProUGUI runActionText;

        [SerializeField] private ArenaInfo arenaInfo;

        [SerializeField] private Image[] roundSlots;
        [SerializeField] private Image[] roundOrbs;

        [Header("Prefabs")] 
        [SerializeField] private CardVisual cardVisualPrefab;
        [SerializeField] private CardVisual nextCardVisualPrefab;

        public CardInstance EquippedTool => equippedTool;
        public CardInstance EquippedWeapon => equippedWeapon;

        public bool IsGameRunning => isGameRunning;
        public GameResult GameResult => gameResult;
        
        private Queue<CardInstance> deck = new();
        private List<CardVisual> spawnedCards = new();
        private List<CardVisual> spawnedNextCards = new();
        private List<CardInstance> discarded = new();
        
        private int health;
        private int roomCount = 0;
        private int currentRunCooldown = 0;

        private bool isGameRunning = false;

        private GameResult gameResult;

        private CardVisual selectedCardVisual;
        private CardInstance equippedWeapon = null;
        private CardInstance equippedTool = null;

        private string selectText;

        private void Awake()
        {
            standardActionButton.onClick.AddListener(OnStandardAction);
            fightActionButton.onClick.AddListener(OnFightWithWeaponAction);
            runActionButton.onClick.AddListener(OnRunAction);
        }

        private void Update()
        {
            if (!isGameRunning) return;
            
            healthText.text = $" {health} / {maxHealth}";
            healthSlider.value = (float) health / maxHealth;

            var hasSelected = selectedCardVisual != null;
            var hasEquippedWeapon = equippedWeapon != null;
            var hasEquippedTool = equippedTool != null;
            
            standardActionButton.interactable = hasSelected;
            if (!hasSelected) standardActionText.text = selectText;

            fightActionButton.gameObject.SetActive(hasSelected && hasEquippedWeapon && selectedCardVisual.Type == CardType.Monster );
            equippedWeaponVisual.gameObject.SetActive(hasEquippedWeapon);
            equippedToolVisual.gameObject.SetActive(hasEquippedTool);

            var fucksGiven = 0;
        }

        public void StartArena(CardData[] cards)
        {
            Audio.PlayBgm("combat");
            selectText = LocalizationManager.Localize("select-target");

            ResetState();
            SetRoundSlots();

            var random = cards.OrderBy(_ => Random.value);
            foreach (var card in random)
                deck.Enqueue(new CardInstance(card));
            
            arenaInfo.Initialize(deck);

            isGameRunning = true;

            NextTurn();
        }

        private void SetRoundSlots()
        {
            for (var i = 0; i < roundSlots.Length; i++)
            {
                var slot = roundSlots[i];
                slot.gameObject.SetActive(i < drawAmount);
            }
        }

        private void SetDrawOrbs(int count)
        {
            for (int i = 0; i < roundOrbs.Length; i++)
            {
                var orb = roundOrbs[i];
                orb.gameObject.SetActive(i < count);
                if (i == count - 1) AnimateOrb(orb);
            }
        }

        private void AnimateOrb(Image orb)
        {
            orb.color = Color.clear;
            orb.DOColor(Color.white, 0.3f);
            
            orb.transform.localScale = Vector3.one * 1.2f;
            orb.transform.DOScale(1f, 0.6f).SetEase(Ease.OutBounce);
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
            
            nextCardsParent.gameObject.SetActive(false);
        }

        private void ClearCards()
        {
            foreach (var card in spawnedCards)
                Destroy(card.gameObject);
            
            spawnedCards.Clear();
        }

        private void NextTurn()
        {
            var canRun = currentRunCooldown <= 0;
            var runAway = LocalizationManager.Localize("run-away");
            var cannotRun = LocalizationManager.Localize("cannot-run", currentRunCooldown);
            runActionText.text = canRun ? runAway : cannotRun;
            runActionButton.interactable = canRun;
            
            if (health <= 0)
            {
                ConcludeGame(false);
                return;
            }

            var cardsDoneThisTurn = drawAmount - spawnedCards.Count;
            SetDrawOrbs(cardsDoneThisTurn);
            if (spawnedCards.Count <= cardLeftToNextTurn) NextRound();

            arenaInfo.UpdateInfo(deck, spawnedCards.Select(x => x.Instance), discarded);

            if (!HasEnemies()) ConcludeGame(true);
        }

        public bool HasEnemies()
        {
            foreach (var card in spawnedCards)
                if (card.Instance.Data.type == CardType.Monster)
                    return true;
            
            foreach (var card in deck)
                if (card.Data.type == CardType.Monster)
                    return true;

            return false;
        }

        private void NextRound()
        {
            if (currentRunCooldown > 0) currentRunCooldown--;
            roomCount++;
            roomText.text = $"Round {roomCount}";

            var remainingCards = new List<CardInstance>();
            foreach (var card in spawnedCards)
                remainingCards.Add(card.Instance);

            float delay = 0f;
            //Spawn Cards
            for (int i = spawnedCards.Count; i < drawAmount; i++)
            {
                if (deck.Count <= 0) break;
                SpawnCard(delay);
                delay += 0.25f;
            }
            
            SetDrawOrbs(0);

            UpdateNextCards();
            
            var components = new List<CardComponent>();
            if (equippedWeapon != null) components.AddRange(equippedWeapon.Components);
            if (equippedTool != null) components.AddRange(equippedTool.Components);
            
            if (remainingCards.Count > 0)
                foreach (var card in remainingCards)
                    components.AddRange(card.Components);
            
            foreach (var component in components)
                if(component is IOnNewRound newRound) newRound.OnNewRound(this);
        }

        private void SpawnCard(float animDelay = 0f)
        {
            var card = deck.Dequeue();
            var cardObj = Instantiate(cardVisualPrefab, cardParent);
            cardObj.Display(card, OnCardPicked);
            cardObj.RevealAnimation(animDelay);
            spawnedCards.Add(cardObj);
        }

        private void UpdateNextCards()
        {
            ClearNextCards();
            var nextCards = deck.Take(Mathf.Min(drawAmount, deck.Count));
            foreach (var card in nextCards)
            {
                var cardObj = Instantiate(nextCardVisualPrefab, nextCardsParent);
                cardObj.Display(card, null);
                
                spawnedNextCards.Add(cardObj);
            }
        }

        private void ClearNextCards()
        {
            foreach (var card in spawnedNextCards)
                Destroy(card.gameObject);
            spawnedNextCards.Clear();
        }

        private void OnCardPicked(CardVisual cardVisual)
        {
            if (selectedCardVisual != null) selectedCardVisual.SetSelected(false);
            
            selectedCardVisual = cardVisual;
            selectedCardVisual.SetSelected(true);

            var key = selectedCardVisual.Type switch
            {
                CardType.Weapon => "equip",
                CardType.Monster => "fight-barehanded",
                CardType.Item => "use",
                CardType.Tool => "equip",
                _ => throw new ArgumentOutOfRangeException()
            };

            standardActionText.text = LocalizationManager.Localize(key);

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
                case CardType.Item: OnUseAction(); return;
                case CardType.Tool: OnEquipToolAction(); return;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEquipToolAction()
        {
            equippedTool = selectedCardVisual.Instance;
            equippedWeaponVisual.Display(equippedTool, null);
            
            ConsumeSelectedCard();
        }

        private void OnUseAction()
        {
            foreach (var component in selectedCardVisual.Instance.Components)
                if (component is IOnItemUse comp) comp.OnItemUse(this);
            
            ConsumeSelectedCard();
        }

        private void OnFightWithBareHandsAction()
        {
            AddHealth(-selectedCardVisual.Value);
            ConsumeSelectedCard();
        }

        public void SetShowNextCards(bool show) => nextCardsParent.gameObject.SetActive(show);

        public void AddHealth(int amount)
        {
            var finalAmount = amount;
            
            var components = new List<CardComponent>();
            if (equippedWeapon != null) components.AddRange(equippedWeapon.Components);
            if (equippedTool != null) components.AddRange(equippedTool.Components);
            
            if (amount < 0)
            {
                var damage = -finalAmount;
                foreach (var comp in components)    
                    if (comp is IModifyIncomingDamage dmg) 
                        dmg.Modify(this, ref damage);
                finalAmount = -damage;
            }

            if (amount > 0)
            {
                foreach (var comp in components)    
                    if (comp is IModifyIncomingHeal heal) 
                        heal.Modify(this, ref finalAmount);
            }

            if (finalAmount == 0) return;

            var oldHealth = health;
            health += finalAmount;
            health = Math.Clamp(health, 0, maxHealth);
            arenaEffect.OnHealthChanged(oldHealth, health);

            var shakeDur = Mathf.Clamp01(finalAmount / 10f);
            var color = finalAmount > 0 ? Color.green : Color.red;
            ShakeColorAnimateText(healthText, color, Mathf.Lerp(0.2f, 1.2f, shakeDur));
        }

        private void OnFightWithWeaponAction()
        {
            if (equippedWeapon == null) throw new Exception("EQUIPPED WEAPON IS EMPTY??");

            var remainder = equippedWeapon.Data.value - selectedCardVisual.Value;
            
            var components = new List<CardComponent>();
            if (equippedWeapon != null) components.AddRange(equippedWeapon.Components);
            if (equippedTool != null) components.AddRange(equippedTool.Components);
            components.AddRange(selectedCardVisual.Instance.Components);

            if (remainder < 0) //monster is stronger
            {
                var damage = -remainder;
                foreach (var component in components)
                    if (component is IModifyOverkillDamage modify) 
                        modify.Modify(this, ref damage);
                
                AddHealth(-damage);
            }
            else //monster is weaker
            {
                int newValue = selectedCardVisual.Value;

                foreach (var component in components)
                    if (component is IMitigateWeaponDegrade mitigate) 
                        mitigate.Mitigate(ref newValue);
                
                SetWeaponValue(newValue);
            }

            foreach (var component in components)
                if (component is IOnDestroyMonsterWithWeapon comp) 
                    comp.OnDestroy(this, selectedCardVisual.Instance, equippedWeapon);
            ConsumeSelectedCard();
        }

        public void SetWeaponValue(int newValue)
        {
            var color = newValue > equippedWeapon.Data.value ? Color.green : Color.red;

            equippedWeapon.SetValue(newValue);
            equippedWeaponVisual.Display(equippedWeapon, null);
            ShakeColorAnimateText(equippedValueText, color, 0.3f);
            arenaEffect.OnWeaponValueChanged();
        }

        private void OnEquipWeaponAction()
        {
            equippedWeapon = selectedCardVisual.Instance;
            
            equippedWeaponVisual.Display(equippedWeapon, null);
            
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


        public void UnequipTool()
        {
            equippedTool = null;
        }

        public void SwapRandom(CardInstance source)
        {
            var rand = spawnedCards.Where(x => !x.Instance.Equals(source))
                .OrderBy(_ => Random.value).FirstOrDefault();
            
            Debug.Log($"WILL SWAP FROM {source.Data.id}");

            if (rand == default) return;

            Debug.Log($"GOT RANDOM {rand.Data.id}");
            
            ConsumeCard(rand);
            SpawnCard();
            UpdateNextCards();
        }

        private void ConsumeSelectedCard()
        {
            ConsumeCard(selectedCardVisual);
        }

        private void ConsumeCard(CardVisual card)
        {
            if (card.Equals(selectedCardVisual)) selectedCardVisual = null;

            spawnedCards.Remove(card);
            discarded.Add(card.Instance);
            Destroy(card.gameObject);
            
            NextTurn();
        }

        [ContextMenu("AutoWin")]
        private void AutoWin()
        {
            var list = deck.ToList();
            list.AddRange(spawnedCards.Select(x => x.Instance));
            
            ClearCards();
            deck.Clear();

            foreach (var card in list)
            {
                if (IsNotMonster(card)) deck.Enqueue(card);
                else discarded.Add(card);
            }
            
            ConcludeGame(true);
        }

        private void ConcludeGame(bool win)
        {
            var enemyDefeated = discarded.Where(x => x.Data.type == CardType.Monster).ToList();

            var itemsLeft = deck.Where(IsNotMonster).ToList();
            
            if (spawnedCards.Count > 0)
                itemsLeft.AddRange(spawnedCards.Select(x => x.Instance).Where(IsNotMonster));

            gameResult = new GameResult(win, enemyDefeated, itemsLeft);
            isGameRunning = false;
        }

        private bool IsNotMonster(CardInstance instance)
            => instance.Data.type switch
            {
                CardType.Monster => false,
                _ => true
            };

        private void ShakeColorAnimateText(TextMeshProUGUI text, Color color, float duration)
        {
            text.DOKill();
            
            text.transform.localScale = Vector3.one * 1.1f;
            text.transform.DOScale(Vector3.one, duration);
            text.transform.DOShakePosition(duration, 10f);
            var originalColor = Color.white;
            text.color = color;
            text.DOColor(originalColor, duration);
        }
    }
}