using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Prototype
{
    //if needed multiple implementation, can be abstracted
    public class GameSaveController : MonoBehaviour
    {
        private const string SAVED_DECK = "saved-deck";
        private const string SAVED_LIBRARY = "saved-library";
        private const string STAGE = "stage";

        [SerializeField] private GameDatabase gameDb;

        public int LoadStageIndex() => PlayerPrefs.GetInt(STAGE, 0);
        public void SaveStageIndex(int stage) => PlayerPrefs.SetInt(STAGE, stage);

        public void SaveLibrary(LibraryData[] library)
        {
            var json = JsonConvert.SerializeObject(library);
            PlayerPrefs.SetString(SAVED_LIBRARY, json);
        }
        
        public IEnumerable<LibraryData> LoadLibrary()
        {
            if (!PlayerPrefs.HasKey(SAVED_LIBRARY))
                return new List<LibraryData>();

            var json = PlayerPrefs.GetString(SAVED_LIBRARY);
            return JsonConvert.DeserializeObject<LibraryData[]>(json);
        }

        public void SaveDeck(string[] deck)
        {
            var json = JsonConvert.SerializeObject(deck);
            PlayerPrefs.SetString(SAVED_DECK, json);
        }

        public string[] LoadDeck()
        {
            if (!PlayerPrefs.HasKey(SAVED_DECK))
                return gameDb.StartingDeck;

            var json = PlayerPrefs.GetString(SAVED_DECK);
            return JsonConvert.DeserializeObject<string[]>(json);
        }
    }
}