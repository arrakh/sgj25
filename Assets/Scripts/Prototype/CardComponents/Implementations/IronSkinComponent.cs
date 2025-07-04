﻿/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using Assets.SimpleLocalization.Scripts;
using Utilities;

namespace Prototype.CardComponents.Implementations
{
    public class IronSkinComponent : CardComponent, IOnDestroyMonsterWithWeapon
    {
        private int damage = 1;
        public override string DisplayName => LocalizationManager.Localize("ironskin-title", damage);

        public override string Description => LocalizationManager.Localize("ironskin-description", damage);

        public override string[] Aliases => new[] {"ironskin"};

        protected override void OnInitialize(string[] args)
        {
            damage = args.GetInt(0, 1);
        }


        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            var newValue = arena.EquippedWeapon.Data.value - damage;
            if (newValue <= 0) arena.UnequipWeapon();
            else arena.SetWeaponValue(newValue);
        }
    }
}