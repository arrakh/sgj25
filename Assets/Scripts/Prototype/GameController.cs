using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;

namespace Prototype
{
    public class GameController : MonoBehaviour
    {
        private static bool _staticInitialized = false;
        
        [Header("Scene")]
        [SerializeField] private ArenaController arenaController;
        [SerializeField] private DeckBuildingScreen deckBuildingScreen;
        [SerializeField] private GameDatabase gameDb; 
        [SerializeField] private ResultScreenController resultScreen;

        [Header("Data")]
        [SerializeField] private SpriteDatabase[] spriteDatabases;

        private IEnumerator Start()
        {
            if (!_staticInitialized)
            {
                foreach (var db in spriteDatabases)
                    db.Initialize();
                
                LocalizationManager.Read();

                _staticInitialized = true;
            }

            if (!gameDb.TryInitialize())
                throw new Exception("CANNOT INITIALIZE GAME DB, CHECK WHATS WRONG WITH DATA");
            
            //DEBUG TestResultScreen();

            deckBuildingScreen.Initialize(55);
            deckBuildingScreen.gameObject.SetActive(true);
            resultScreen.gameObject.SetActive(false);

            yield return new WaitUntil(() => deckBuildingScreen.CompleteBuilding);
            var deck = deckBuildingScreen.Deck;
            
            deckBuildingScreen.gameObject.SetActive(false);
            
            arenaController.StartArena(deck);
            yield return new WaitUntil(() => !arenaController.IsGameRunning);
            
            resultScreen.gameObject.SetActive(true);
            yield return resultScreen.PlayResult(arenaController.GameResult, 999);
            
            Debug.Log("DONE");
        }

        void TestResultScreen()
        {
            resultScreen.gameObject.SetActive(true);
            var cards = gameDb.AllCards.Select(x => new CardInstance(x));
            List<CardInstance> defeated = new(), remaining = new();
            foreach (var card in cards)
                if (card.Data.type == CardType.Monster) defeated.Add(card);
                else remaining.Add(card);

            var result = new GameResult(true, defeated, remaining);
            resultScreen.PlayResult(result, 999);
        }
    }
}