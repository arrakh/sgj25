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
        private const string STAGE = "stage";
        private static bool _staticInitialized = false;
        
        [Header("Scene")]
        [SerializeField] private ArenaController arenaController;
        [SerializeField] private DeckBuildingScreen deckBuildingScreen;
        [SerializeField] private GameDatabase gameDb; 
        [SerializeField] private ResultScreenController resultScreen;

        [Header("Data")]
        [SerializeField] private SpriteDatabase[] spriteDatabases;

        private int stage, lastStage;
        
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
            
            //yield return TestResultScreen();
            //yield break;
            
            stage = PlayerPrefs.GetInt(STAGE, 0);
            lastStage = gameDb.ProgressionData.Length - 1;

            if (stage < lastStage) yield return CampaignPlay();

            yield return FreePlay();
        }

        private IEnumerator FreePlay()
        {
            var data = gameDb.ProgressionData.Last();
            while (true) yield return GameLoop(data, true);
        }

        private IEnumerator CampaignPlay()
        {
            var progression = gameDb.ProgressionData;

            for (int i = stage; i < progression.Length; i++)
            {
                var data = progression[i];
                yield return GameLoop(data, false);
                
                var score = resultScreen.Score;
                if (score < data.targetScore) continue;

                //show result screen
                stage++;
                PlayerPrefs.SetInt(STAGE, stage);
            }
        }

        private IEnumerator GameLoop(ProgressionEntry progression, bool isFreePlay)
        {
            deckBuildingScreen.Initialize(progression.minHype, !isFreePlay);
            deckBuildingScreen.gameObject.SetActive(true);
            resultScreen.gameObject.SetActive(false);
                
            yield return new WaitUntil(() => deckBuildingScreen.CompleteBuilding);
            var deck = deckBuildingScreen.Deck;
            deckBuildingScreen.gameObject.SetActive(false);
            
            arenaController.StartArena(deck);
            yield return new WaitUntil(() => !arenaController.IsGameRunning);
            
            resultScreen.gameObject.SetActive(true);
            yield return resultScreen.PlayResult(arenaController.GameResult, progression.targetScore);
        }

        IEnumerator TestResultScreen()
        {
            resultScreen.gameObject.SetActive(true);
            var cards = gameDb.AllCards.Select(x => new CardInstance(x));
            List<CardInstance> defeated = new(), remaining = new();
            foreach (var card in cards)
                if (card.Data.type == CardType.Monster) defeated.Add(card);
                else remaining.Add(card);

            var result = new GameResult(true, defeated, remaining);
            yield return resultScreen.PlayResult(result, 999);
        }
    }
}