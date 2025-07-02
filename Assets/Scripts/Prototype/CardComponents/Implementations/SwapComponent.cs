namespace Prototype.CardComponents.Implementations
{
    public class SwapComponent : CardComponent, IOnItemUse
    {
        public override string DisplayName => "Swap";

        public override string Description =>
            "Randomly selects 1 card and sends it to the bottom of the deck. Draw the next card to replace it";

        public override string[] Aliases => new []{"swap"};


        public void OnItemUse(ArenaController arena)
        {
            arena.SwapRandom(cardInstance);
        }
    }
}