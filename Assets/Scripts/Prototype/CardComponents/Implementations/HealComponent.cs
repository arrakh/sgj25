namespace Prototype.CardComponents.Implementations
{
    public class HealComponent : CardComponent, IOnItemUse
    {
        public override string DisplayName => "Heal";
        public override string Description => $"Increases health by {cardInstance.Data.value}";
        public override string[] Aliases => new[] {"heal"};

        public void OnItemUse(ArenaController arena)
        {
            arena.AddHealth(cardInstance.Data.value);
        }
    }
}