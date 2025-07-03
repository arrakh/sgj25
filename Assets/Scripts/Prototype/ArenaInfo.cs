using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Prototype
{
    public class ArenaInfo : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private RectTransform arenaParent;
        [SerializeField] private RectTransform discardedParent;
        [SerializeField] private RectTransform poolParent;

        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;

        [SerializeField] private TextMeshProUGUI monsterCount;
        [SerializeField] private TextMeshProUGUI weaponCount;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private TextMeshProUGUI toolCount;
        
        [Header("Prefab References")] 
        [SerializeField] private CardVisual cardPrefab;

        private ObjectPool<CardVisual> visualPool;
        private List<CardVisual> arenaCards = new();
        private List<CardVisual> discardedCards = new();
        private int monsterTotal;

        public void Initialize(IEnumerable<CardInstance> deck)
        {
            visualPool = new(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, true, 80);

            monsterTotal = deck.Count(x => x.Data.type == CardType.Monster);
        }

        public void UpdateInfo(IEnumerable<CardInstance> deck, IEnumerable<CardInstance> drawn,
            IEnumerable<CardInstance> discarded)
        {
            foreach (var vis in arenaCards)
                visualPool.Release(vis);
            arenaCards.Clear();
            
            foreach (var card in deck.OrderBy(x => x.Data.type).ThenBy(x => x.Data.value))
            {
                var vis = visualPool.Get();
                vis.transform.SetParent(arenaParent, false);
                vis.Display(card, null);
                arenaCards.Add(vis);
            }

            foreach (var vis in discardedCards)
                visualPool.Release(vis);
            discardedCards.Clear();
            
            foreach (var card in discarded)
            {
                var vis = visualPool.Get();
                vis.transform.SetParent(discardedParent, false);
                vis.Display(card, null);
                discardedCards.Add(vis);
            }
            
            //Update Header
            
            List<CardInstance> cards = new();
            cards.AddRange(deck);
            cards.AddRange(drawn);
            var discardedMonsters = discarded.Count(x => x.Data.type == CardType.Monster);

            UpdateHeader(discardedMonsters, cards);
        }

        private void UpdateHeader(int progressCount, List<CardInstance> cards)
        {
            var progress = (float) progressCount / monsterTotal;

            progressSlider.value = progress;
            progressText.text = $"{(progress * 100):F1}% Cleared";

            int monster = 0, weapon = 0, item = 0, tool = 0;

            foreach (var card in cards)
            {
                switch (card.Data.type)
                {
                    case CardType.Weapon:
                        weapon++;
                        break;
                    case CardType.Monster:
                        monster++;
                        break;
                    case CardType.Tool:
                        tool++;
                        break;
                    case CardType.Item:
                        item++;
                        break;
                    default: throw new Exception($"TRYING TO COUNT CARDS BUT ID {card.Data.id} IS TYPE {card.Data.type}");
                }
            }

            monsterCount.text = monster.ToString();
            weaponCount.text = weapon.ToString();
            itemCount.text = item.ToString();
            toolCount.text = tool.ToString();
        }

        #region Pool Functions

        private void ActionOnDestroy(CardVisual v)
            => Destroy(v.gameObject);

        private void ActionOnRelease(CardVisual v)
        {
            v.transform.SetParent(poolParent, false);
            v.gameObject.SetActive(false);
        }

        private void ActionOnGet(CardVisual v)
            => v.gameObject.SetActive(true);

        private CardVisual CreateFunc()
        {
            var vis = Instantiate(cardPrefab, poolParent);
            vis.gameObject.SetActive(false);
            return vis;
        }

        #endregion
    }
}