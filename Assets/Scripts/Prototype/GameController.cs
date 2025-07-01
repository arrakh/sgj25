using System;
using System.Collections;
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

        [Header("Data")]
        [SerializeField] private SpriteDatabase[] spriteDatabases;

        private IEnumerator Start()
        {
            if (!_staticInitialized)
            {
                foreach (var db in spriteDatabases)
                    db.Initialize();

                _staticInitialized = true;
            }

            if (!gameDb.TryInitialize())
                throw new Exception("CANNOT INITIALIZE GAME DB, CHECK WHATS WRONG WITH DATA");

            deckBuildingScreen.Initialize(132);
            deckBuildingScreen.gameObject.SetActive(true);

            
            yield return new WaitUntil(() => deckBuildingScreen.CompleteBuilding);
            var deck = deckBuildingScreen.Deck;
            
            deckBuildingScreen.gameObject.SetActive(false);
            
            arenaController.StartArena(deck);
        }
    }
}