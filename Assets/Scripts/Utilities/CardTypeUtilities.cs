using System;
using Prototype;
using UnityEngine;

namespace Utilities
{
    public static class CardTypeUtilities
    {
        public static Sprite GetBackground(this CardType type)
        {
            var spriteId = type switch
            {
                CardType.Weapon => "card-bg-yellow",
                CardType.Monster => "card-bg-red",
                CardType.Tool => "card-bg-blue",
                CardType.Item => "card-bg-purple",
                _ => "error"
            };

            return SpriteDatabase.Get(spriteId);
        }
        
        public static Sprite GetBanner(this CardType type)
        {
            var spriteId = type switch
            {
                CardType.Weapon => "card-banner-yellow",
                CardType.Monster => "card-banner-red",
                CardType.Tool => "card-banner-blue",
                CardType.Item => "card-banner-purple",
                _ => "error"
            };

            return SpriteDatabase.Get(spriteId);
        }
    }
}