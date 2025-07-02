using Utilities;

namespace Prototype.CardComponents.Implementations
{
    public class IronSkinComponent : CardComponent, IOnDestroyMonsterWithWeapon
    {
        private int damage = 1;
        public override string DisplayName => $"Iron Skin {damage}";

        public override string Description =>
            $"When damaged, will reduce weapon's value by {damage}. Weapon will break when it reaches 0";

        public override string[] Aliases => new[] {"ironskin"};

        protected override void OnInitialize(string[] args)
        {
            damage = args.GetInt(0, 1);
        }


        public void OnDestroy(ArenaController arena, CardInstance monster, CardInstance weapon)
        {
            var newValue = arena.EquippedWeapon.Data.value - damage;
            if (newValue <= 0) arena.UnequipWeapon();
            else arena.SetWeaponValue(newValue);
        }
    }
}