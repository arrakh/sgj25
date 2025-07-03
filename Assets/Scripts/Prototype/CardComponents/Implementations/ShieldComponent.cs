using System;
using UnityEngine;

namespace Prototype.CardComponents.Implementations
{
    public class ShieldComponent : CardComponent, IModifyIncomingDamage
    {
        public override string DisplayName => "Shield";
        public override string Description => $"Reduces incoming damage once by {cardInstance.Data.value}, then breaks";
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