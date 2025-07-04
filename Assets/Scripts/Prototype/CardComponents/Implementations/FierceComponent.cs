/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using Assets.SimpleLocalization.Scripts;
using UnityEngine;

namespace Prototype.CardComponents.Implementations
{
    public class FierceComponent : CardComponent, IModifyOverkillDamage
    {
        public override string DisplayName => LocalizationManager.Localize("fierce-title");

        public override string Description => LocalizationManager.Localize("fierce-description");

        public override string[] Aliases => new []{"fierce"};
        
        public void Modify(ArenaController arena, ref int overkillDamage)
        {
            var from = overkillDamage;
            overkillDamage *= 2;
            Debug.Log($"Doubled damage from {from} to {overkillDamage}");
        }
    }
}