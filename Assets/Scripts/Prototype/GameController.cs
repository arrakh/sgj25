using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
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
        [SerializeField] private int maxHealth = 20;
        
        [Header("Scene References")] 
        [SerializeField] private RectTransform cardParent;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Card equippedCard;
        [SerializeField] private Button standardActionButton;
        [SerializeField] private TextMeshProUGUI standardActionText;

        [SerializeField] private Button fightActionButton;
        [SerializeField] private Button runActionButton;

        [Header("Prefabs")] 
        [SerializeField] private Card cardPrefab;

        private Queue<CardData> deck = new();
        private List<Card> spawnedCards = new();
        private List<CardData> discarded = new();
        private int health;

        private Card selectedCard;
        
        private void Update()
        {
            healthText.text = $" {health}<size=33%>/{maxHealth}";
            var fucksGiven = 0;
        }

        private void Start()
        {
            health = maxHealth;
            var data = JsonConvert.DeserializeObject<CardData[]>(cardJson.text);

            var random = data.OrderBy(_ => Random.value);
            foreach (var card in random)
                deck.Enqueue(card);

            SpawnCards();
        }

        private void SpawnCards()
        {
            foreach (var card in spawnedCards)
                Destroy(card.gameObject);
            
            spawnedCards.Clear();
            
            for (int i = 0; i < drawAmount; i++)
            {
                var card = deck.Dequeue();
                var cardObj = Instantiate(cardPrefab, cardParent);
                cardObj.Display(card, OnCardPicked);
            }
        }

        private void OnCardPicked(Card card)
        {
            if (selectedCard != null) selectedCard.SetSelected(false);
            
            selectedCard = card;
            selectedCard.SetSelected(true);
        }
    }
}