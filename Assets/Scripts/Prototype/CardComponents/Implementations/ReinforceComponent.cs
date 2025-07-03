namespace Prototype.CardComponents.Implementations
{
    public class ReinforceComponent : CardComponent, IOnItemUse
    {
        public override string DisplayName => "Reinforce";

        public override string Description => $"Adds {cardInstance.Data.value} to equipped weapon. Nothing happens if no weapon is equipped";
        public override string[] Aliases => new []{"reinforce"};

        public void OnItemUse(ArenaController arena)
        {
            var finalValue = arena.EquippedWeapon.Data.value + cardInstance.Data.value;
            arena.SetWeaponValue(finalValue);
        }
    }
}