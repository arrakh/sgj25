using System;
using UnityEngine;
using Utilities;

namespace Prototype.CardComponents.Implementations
{
    public class PredictComponent : CardComponent, IOnItemUse, IOnNewRound
    {
        private int roundCount;
        
        public override string DisplayName => $"Predict {roundCount}";
        public override string Description => $"View cards that will appear on the next round for {roundCount} rounds";
        public override string[] Aliases => new[] {"predict"};

        protected override void OnInitialize(string[] args)
        {
            roundCount = args.GetInt(0, 1);
            Debug.Log($"ROUND COUNT IS NOW {roundCount}");
        }

        public void OnItemUse(ArenaController arena)
        {
            arena.SetShowNextCards(true);
        }

        public void OnNewRound(ArenaController arena)
        {
            roundCount--;
            Debug.Log($"ROUND COUNT IS NOW {roundCount}");

            if (roundCount > 0)
            {
                RaiseUpdateEvent();
                return;
            }

            switch (cardInstance.Data.type)
            {
                case CardType.Weapon: arena.UnequipWeapon(); break;
                case CardType.Tool:  arena.UnequipTool(); break;
                default: throw new Exception($"CANNOT HANDLE PREDICT ON CARD {cardInstance.Data.id} WITH TYPE {cardInstance.Data.type}");
            }
        }

    }
}