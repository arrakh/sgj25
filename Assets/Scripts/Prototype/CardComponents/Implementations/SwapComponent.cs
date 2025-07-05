/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
 using Assets.SimpleLocalization.Scripts;
 
namespace Prototype.CardComponents.Implementations
{
    public class SwapComponent : CardComponent, IOnItemUse
    {
        public override string DisplayName => LocalizationManager.Localize("swap-tile");

        public override string Description => LocalizationManager.Localize("swap-description");

        public override string[] Aliases => new[] { "swap" };


        public void OnItemUse(ArenaController arena)
        {
            arena.SwapRandom(cardInstance);
        }
    }
}