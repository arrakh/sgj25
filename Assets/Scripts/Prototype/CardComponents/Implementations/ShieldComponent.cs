/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using System;
using UnityEngine;
using Assets.SimpleLocalization.Scripts;
namespace Prototype.CardComponents.Implementations
{
    public class ShieldComponent : CardComponent, IModifyIncomingDamage
    {
        public override string DisplayName => LocalizationManager.Localize("shield-title");
        public override string Description => LocalizationManager.Localize("shield-description",cardInstance.Data.value);
        public override string[] Aliases => new[] {"shield"};

        public void Modify(ArenaController arena, ref int damage)
        {
            damage = Mathf.Clamp(damage - cardInstance.Data.value, 0, int.MaxValue);

            switch (cardInstance.Data.type)
            {
                case CardType.Weapon: arena.UnequipWeapon(); break;
                case CardType.Tool: arena.UnequipTool(); break;
                default:
                    throw new Exception($"CANNOT RESOLVE SHIELD IN CARD {cardInstance.Data.id} WITH TYPE {cardInstance.Data.type}");
            }
        }
    }
}