/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using Assets.SimpleLocalization.Scripts;
using UnityEngine;

namespace Prototype.CardComponents.Implementations
{
    public class DrainComponent : CardComponent, IOnDestroyMonsterWithWeapon
    {
        public override string DisplayName => LocalizationManager.Localize("drain-title");
        public override string Description => LocalizationManager.Localize("drain-description");
        public override string[] Aliases => new[] {"drain"};

        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            var health = Mathf.FloorToInt(monster.Data.value/2f);
            arena.AddHealth(health);
        }
    }
}