using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Prototype
{
    public class RewardScreen : MonoBehaviour
    {
        [Header("Scene References")] 
        [SerializeField] private RectTransform rewardParent;
        [SerializeField] private GameDatabase gameDb;
        [SerializeField] private Button continueButton;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Prefab References")] 
        [SerializeField] private CardVisual rewardPrefab;

        private List<CardVisual> spawnedCards = new();

        private bool doneShowing = false;

        private void Awake()
        {
            continueButton.onClick.AddListener(() => doneShowing = true);
        }

        public IEnumerator DisplayRewards(string[] cards)
        {
            doneShowing = false;
            foreach (var card in spawnedCards)
                Destroy(card.gameObject);
            spawnedCards.Clear();

            continueButton.gameObject.SetActive(false);
            
            var wait = new WaitForSeconds(0.05f);
            foreach (var cardId in cards)
            {
                var data = gameDb.GetCard(cardId);
                var card = Instantiate(rewardPrefab, rewardParent);
                card.Display(new CardInstance(data), null);
                spawnedCards.Add(card);
                
                card.transform.localScale = Vector3.zero;
                card.transform
                    .DOScale(Vector3.one, 0.33f)
                    .SetEase(Ease.OutBack);

                scrollRect.ScrollToBottom();

                yield return wait;
            }
            
            continueButton.gameObject.SetActive(true);

            yield return new WaitUntil(() => doneShowing);
        }
    }
}