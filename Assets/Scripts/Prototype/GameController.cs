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
        [SerializeField] private FadeScreen fadeScreen;
        [SerializeField] private ArenaController arenaController;
        [SerializeField] private DeckBuildingScreen deckBuildingScreen;
        [SerializeField] private GameDatabase gameDb; 
        [SerializeField] private ResultScreenController resultScreen;
        [SerializeField] private RewardScreen rewardScreen;
        [SerializeField] private GameSaveController gameSave;
        [SerializeField] private DialogueController dialogue;

        [Header("Data")]
        [SerializeField] private SpriteDatabase[] spriteDatabases;

        private int stageIndex, lastStageIndex;
        
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
            
            Audio.FadeBgm(0f, 1f, 2f);
            fadeScreen.FadeOut(1.2f);

            stageIndex = gameSave.LoadStageIndex();
            lastStageIndex = gameDb.ProgressionData.Length - 1;

            if (stageIndex < lastStageIndex) yield return CampaignPlay();

            yield return FreePlay();
        }

        private IEnumerator FreePlay()
        {
            var data = gameDb.ProgressionData.Last();
            while (true) yield return GameLoop(data, true, false);
        }

        private IEnumerator CampaignPlay()
        {
            Debug.Log("PLAYING CAMPAIGN");
            
            if (stageIndex == 0) yield return dialogue.Initialize();
            
            var progression = gameDb.ProgressionData;

            Debug.Log($"Campaign: Stage {stageIndex}");

            while (stageIndex < lastStageIndex)
            {
                var data = progression[stageIndex];
                yield return GameLoop(data, false, stageIndex == 0);
                
                var score = resultScreen.Score;
                if (score < data.targetScore) continue;

                AddAndSaveRewardToLibrary(data);

                rewardScreen.gameObject.SetActive(true);
                yield return rewardScreen.DisplayRewards(data.cardRewards);
                
                stageIndex++;
                gameSave.SaveStageIndex(stageIndex);
            }
        }

        private void AddAndSaveRewardToLibrary(ProgressionEntry data)
        {
            var library = gameSave.LoadLibrary().ToList();
            Dictionary<string, LibraryData> libraryData = library.ToDictionary(x => x.id, y => y);
            foreach (var cardId in data.cardRewards)
            {
                if (libraryData.TryGetValue(cardId, out var existing))
                    libraryData[cardId] = new LibraryData(cardId, existing.amount + 1);
                else libraryData[cardId] = new LibraryData(cardId, 1);
            }

            gameSave.SaveLibrary(libraryData.Values.ToArray());
        }

        private IEnumerator GameLoop(ProgressionEntry progression, bool isFreePlay, bool skipDeckBuilding)
        {
            resultScreen.gameObject.SetActive(false);
            rewardScreen.gameObject.SetActive(false);
            
            Debug.Log($"Start of Game Loop! Skip Deck Build? {skipDeckBuilding}");

            if (!skipDeckBuilding)
            {
                deckBuildingScreen.Initialize(progression.minHype, !isFreePlay);
                deckBuildingScreen.gameObject.SetActive(true);
                yield return new WaitUntil(() => deckBuildingScreen.CompleteBuilding);
                var deck = deckBuildingScreen.Deck;
                deckBuildingScreen.gameObject.SetActive(false);  
                arenaController.StartArena(deck);
            }
            else
            {
                deckBuildingScreen.gameObject.SetActive(false);  
                var deckData = gameSave.LoadDeck();
                var deck = deckData.Select(x => gameDb.GetCard(x)).ToArray();
                arenaController.StartArena(deck);
            }
            
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