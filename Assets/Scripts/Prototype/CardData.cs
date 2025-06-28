using System;

namespace Prototype
{
    [Serializable]
    public struct CardData
    {
        public CardType type;
        public int value;

        public static CardData Empty => new() {type = CardType.None, value = 0};
    }
}