/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
 
 using Assets.SimpleLocalization.Scripts;
namespace Prototype.CardComponents.Implementations
{
    public class ReinforceComponent : CardComponent, IOnItemUse
    {
        public override string DisplayName => LocalizationManager.Localize("reinforce-title");

        public override string Description => LocalizationManager.Localize("reinforce-description",cardInstance.Data.value);
        public override string[] Aliases => new[] { "reinforce" };

        public void OnItemUse(ArenaController arena)
        {
            if (arena.EquippedWeapon == null) return;
            
            var finalValue = arena.EquippedWeapon.Data.value + cardInstance.Data.value;
            arena.SetWeaponValue(finalValue);
        }
    }
}