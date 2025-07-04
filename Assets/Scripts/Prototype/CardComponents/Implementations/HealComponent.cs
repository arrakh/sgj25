/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using Assets.SimpleLocalization.Scripts;

namespace Prototype.CardComponents.Implementations
{
    public class HealComponent : CardComponent, IOnItemUse
    {
        public override string DisplayName => LocalizationManager.Localize("heal-title");
        public override string Description => LocalizationManager.Localize("heal-description", cardInstance.Data.value);
        public override string[] Aliases => new[] {"heal"};

        public void OnItemUse(ArenaController arena)
        {
            arena.AddHealth(cardInstance.Data.value);
        }
    }
}