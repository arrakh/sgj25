using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Prototype
{
    public class CardDealer : MonoBehaviour
    {
        public Transform deckPosition;
        public Transform[] handPositions = new Transform[4];
        public GameObject cardPrefab;

        public List<CardData> cardPool = new(); // isi contoh kartu di Inspector

        private List<CardData> deck = new();

        void Start()
        {
            GenerateDeck();
            ShuffleDeck();
            StartCoroutine(ShuffleVisualDeck());
        }

        void GenerateDeck()
        {
            deck.Clear();
            for (int i = 0; i < 20; i++)
            {
                var randomCard = cardPool[Random.Range(0, cardPool.Count)];
                deck.Add(randomCard);
            }
        }

        void ShuffleDeck()
        {
            for (int i = 0; i < deck.Count; i++)
            {
                var temp = deck[i];
                int rand = Random.Range(i, deck.Count);
                deck[i] = deck[rand];
                deck[rand] = temp;
            }
        }

        IEnumerator ShuffleVisualDeck()
        {
            List<GameObject> visualDeck = new List<GameObject>();
            int visualDeckSize = 10;

            // Buat kartu visual (dummy)
            for (int i = 0; i < visualDeckSize; i++)
            {
                GameObject card = Instantiate(cardPrefab, deckPosition);
                card.transform.localScale = Vector3.one * 0.9f;
                visualDeck.Add(card);
            }

            // Gerak acak cepat beberapa kali
            for (int i = 0; i < 8; i++)
            {
                foreach (var card in visualDeck)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f),
                        0
                    );
                    card.transform.DOMove(deckPosition.position + offset, 0.1f).SetEase(Ease.Linear);
                }
                yield return new WaitForSeconds(0.1f);
            }

            // Hapus visual deck
            foreach (var c in visualDeck)
                Destroy(c);

            visualDeck.Clear();

            // Mulai draw kartu beneran
            StartCoroutine(DrawCardsRoutine(4));
        }

        IEnumerator DrawCardsRoutine(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (deck.Count == 0) yield break;

                var cardData = deck[0];
                deck.RemoveAt(0);

                GameObject cardObj = Instantiate(cardPrefab, deckPosition);
                var cardScript = cardObj.GetComponent<CardVisual>();
                cardScript.Display(new CardInstance(cardData), OnCardPicked);

                cardObj.transform.localScale = Vector3.zero;
                cardObj.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
                cardObj.transform.DOMove(handPositions[i].position, 0.4f).SetEase(Ease.OutCubic);

                yield return new WaitForSeconds(0.2f);
            }
        }

        void OnCardPicked(CardVisual pickedCardVisual)
        {
            Debug.Log($"Picked: {pickedCardVisual.Type} - {pickedCardVisual.Value}");
        }
    }
}