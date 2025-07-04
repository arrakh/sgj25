﻿/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using System;
using UnityEngine;
using Utilities;
using Assets.SimpleLocalization.Scripts;

namespace Prototype.CardComponents.Implementations
{
    public class PredictComponent : CardComponent, IOnEquip, IOnNewRound
    {
        private int roundCount;
        
        public override string DisplayName => LocalizationManager.Localize("predict-title",roundCount);
        public override string Description => LocalizationManager.Localize("predict-description",roundCount);
        public override string[] Aliases => new[] {"predict"};

        protected override void OnInitialize(string[] args)
        {
            roundCount = args.GetInt(0, 1);
            Debug.Log($"ROUND COUNT IS NOW {roundCount}");
        }

        public void OnEquip(ArenaController arena)
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
            
            arena.SetShowNextCards(false);

            switch (cardInstance.Data.type)
            {
                case CardType.Weapon: arena.UnequipWeapon(); break;
                case CardType.Tool:  arena.UnequipTool(); break;
                default: throw new Exception($"CANNOT HANDLE PREDICT ON CARD {cardInstance.Data.id} WITH TYPE {cardInstance.Data.type}");
            }
        }
    }
}