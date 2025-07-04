using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Prototype
{
    public class GameDatabase : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private TextAsset cardJson;
        [SerializeField] private TextAsset progressionJson;
        [SerializeField] private TextAsset startingDeckJson;
        
        public ProgressionEntry[] ProgressionData => progressionData;
        public CardData[] AllCards => allCards.Values.ToArray();
        public string[] StartingDeck => startingDeck;
        
        private Dictionary<string, CardData> allCards = new();
        private string[] startingDeck;
        private ProgressionEntry[] progressionData;

        public bool TryInitialize()
        {
            try
            {
                var cards = JsonConvert.DeserializeObject<CardData[]>(cardJson.text);
                foreach (var card in cards) allCards[card.id] = card;

                startingDeck = JsonConvert.DeserializeObject<string[]>(startingDeckJson.text);

                progressionData = JsonConvert.DeserializeObject<ProgressionEntry[]>(progressionJson.text);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }

        public CardData GetCard(string id)
        {
            if (!allCards.TryGetValue(id, out var card))
                throw new Exception($"CANNOT FIND CARD WITH ID {id}");

            return card;
        }
    }
}