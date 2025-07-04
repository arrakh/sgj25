/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */

using System;
using Assets.SimpleLocalization.Scripts;
using Mono.Cecil.Cil;
using UnityEngine.UIElements;
using Utilities;

namespace Prototype.CardComponents.Implementations
{
    
    public class RangedComponent : CardComponent, IMitigateWeaponDegrade, IOnDestroyMonsterWithWeapon
    {
        public Action<CardComponent> OnUpdateVisual { get; }

        private int durability = 1;

        public override string DisplayName => LocalizationManager.Localize("ranged-title", durability);
        public override string Description => LocalizationManager.Localize("ranged-description", durability);
        public override string[] Aliases => new[] {"ranged", "range"};

        protected override void OnInitialize(string[] args)
        {
            durability = args.GetInt(0, 1);
        }

        public void Mitigate(ref int newWeaponValue)
        {
            newWeaponValue = cardInstance.Data.value;
            OnUpdateVisual?.Invoke(this);
        }

        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            durability--;
            if (durability <= 0) arena.UnequipWeapon();
            else RaiseUpdateEvent();
        }

    }
}